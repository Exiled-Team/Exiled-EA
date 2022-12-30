// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
/*
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

            int offset = -1;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(ConnectionRequest), nameof(ConnectionRequest.Accept)))) + offset;

            object returnLabel = newInstructions.FindLast(instruction => instruction.opcode == OpCodes.Br_S).operand;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // userid
                    new CodeInstruction(OpCodes.Ldloc_S, 10).MoveLabelsFrom(newInstructions[index]),

                    // Request
                    new(OpCodes.Ldarg_1),

                    // Request.Data.Position
                    new(OpCodes.Dup),
                    new(OpCodes.Ldfld, Field(typeof(ConnectionRequest), nameof(ConnectionRequest.Data))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(NetDataReader), nameof(NetDataReader.Position))),

                    // b3 (flags)
                    new(OpCodes.Ldloc_S, 12),

                    // text2 (country)
                    new(OpCodes.Ldloc_S, 13),

                    // new PreAuthenticatingEventArgs(string userId, ConnectionRequest request, int readerStartPosition, byte flags, string country)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnPreAuthenticating(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnPreAuthenticating))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
*/