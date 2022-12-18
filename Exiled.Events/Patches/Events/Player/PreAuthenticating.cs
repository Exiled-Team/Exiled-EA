// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using GameCore;
using Mirror.LiteNetLib4Mirror;

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Handlers;

    using HarmonyLib;

    using LiteNetLib;
    using LiteNetLib.Utils;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)" />.
    ///     Adds the <see cref="Player.PreAuthenticating" /> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest), typeof(ConnectionRequest))]
    internal static class PreAuthenticating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);


            LocalBuilder jumpConditions = generator.DeclareLocal(typeof(int));

            Label acceptLabel = generator.DefineLabel();
            Label fullRejectLabel = generator.DefineLabel();
            Label continueConditions = generator.DefineLabel();

            int offset = 0;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(LiteNetLib4MirrorCore), nameof(LiteNetLib4MirrorCore.Host)))) + offset;

            int acceptIndex = newInstructions.FindIndex(index, instruction => instruction.LoadsField(Field(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.UserIds))));
            newInstructions[acceptIndex].WithLabels(acceptLabel);

            int offsetCondition = 2;
            int continueConditionsIndex = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots)))) + offsetCondition;
            newInstructions[continueConditionsIndex].WithLabels(continueConditions);


            int rejectOffset = 2;
            int rejectIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode)))) + rejectOffset;
            newInstructions[rejectIndex].WithLabels(fullRejectLabel);

            object returnLabel = newInstructions.FindLast(instruction => instruction.opcode == OpCodes.Br_S).operand;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_S, 10).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldarg_1),
                    // b3 (flags)
                    new(OpCodes.Ldloc_S, 12),
                    // text2 (country)
                    new(OpCodes.Ldloc_S, 13),
                    // Players connected
                    new(OpCodes.Ldsfld, Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots))),
                    new(OpCodes.Call, Method(typeof(PreAuthenticating), nameof(PreAuthenticating.authenticatePlayer))),
                    new(OpCodes.Stloc, jumpConditions.LocalIndex),

                    // If allow connection override
                    new(OpCodes.Ldloc, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Beq, acceptLabel),

                    // If allow further if condition checks
                    new(OpCodes.Ldloc, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Beq, continueConditions),

                    // If server full
                    new(OpCodes.Ldloc, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Beq, fullRejectLabel),

                    // If fully reject
                    new(OpCodes.Ldloc, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_M1),
                    new(OpCodes.Beq, returnLabel),

                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static int authenticatePlayer(string userId, ConnectionRequest request, byte flags, string country, int cur_slots)
        {
            PreAuthenticatingEventArgs ev = new(userId, request, request.Data.Position,  flags, country, cur_slots);

            Player.OnPreAuthenticating(ev);

            // If there is a desire to accept the connection naturally (server not full, and accept condition left alone)
            if (ev.AcceptConnection)
            {
                API.Features.Log.Info($"What is current peer count and such {LiteNetLib4MirrorCore.Host.ConnectedPeersCount} and {cur_slots} and max {LiteNetLib4MirrorNetworkManager.singleton.maxConnections}" +
                                             $" flags {flags} and country {country}");
                return 2;
            }

            // If the server wants the rest of NW condition checks
            if (ev.AllowFurtherChecks)
            {
                API.Features.Log.Info("Allowing further checks");
                return 0;
            }

            // If the connection should be accepted due to ForceAcceptConnection default/
            if (ev.ForceAllowConnection)
            {
                API.Features.Log.Info("Force allowing connection");
                return 2;
            }

            // If the server is "full"
            if (ev.ServerFull)
            {
                API.Features.Log.Info("Server is currently full");
                return 1;
            }

            // If the event was not allowed via isAllowed

            string failedMessage = string.Format($"Player {userId} tried to pre-authenticated from endpoint {request.RemoteEndPoint}, but the request has been rejected by a plugin.");

            ServerConsole.AddLog(failedMessage);
            ServerLogs.AddLog(ServerLogs.Modules.Networking, failedMessage, ServerLogs.ServerLogType.ConnectionUpdate);


            Exiled.API.Features.Log.Info($"Rejected by isAllowed {LiteNetLib4MirrorCore.Host.ConnectedPeersCount} and {cur_slots} and max {LiteNetLib4MirrorNetworkManager.singleton.maxConnections}" +
                                         $" flags {flags} and country {country}");

            // Exiled.API.Features.Log.Info("Well time to accept connection");
            return -1;

        }
    }
}