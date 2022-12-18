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


            Label acceptLabel = generator.DefineLabel();
            Label fullRejectLabel = generator.DefineLabel();


            int offset = 0;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(LiteNetLib4MirrorCore), nameof(LiteNetLib4MirrorCore.Host)))) + offset;

            int acceptIndex = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots))));
            newInstructions[acceptIndex].WithLabels(acceptLabel);

            int rejectIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode)))) + 2;
            newInstructions[rejectIndex].WithLabels(fullRejectLabel);
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
                    new(OpCodes.Brtrue, acceptLabel),
                    new(OpCodes.Br, fullRejectLabel),

                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static bool authenticatePlayer(string userId, ConnectionRequest request, byte flags, string country, int cur_slots)
        {
            PreAuthenticatingEventArgs ev = new(userId, request, request.Data.Position,  flags, country, cur_slots);

            Player.OnPreAuthenticating(ev);

            if (!ev.IsAllowed)
            {
              string failedMessage = string.Format($"Player {userId} tried to pre-authenticated from endpoint {request.RemoteEndPoint}, but the request has been rejected by a plugin.");

              ServerConsole.AddLog(failedMessage);
              ServerLogs.AddLog(ServerLogs.Modules.Networking, failedMessage, ServerLogs.ServerLogType.ConnectionUpdate);
              return false;
            }

            return true;

        }
    }
}