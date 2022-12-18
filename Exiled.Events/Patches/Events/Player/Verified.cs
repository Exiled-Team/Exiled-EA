// -----------------------------------------------------------------------
// <copyright file="Verified.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ServerRoles.UserCode_CmdServerSignatureComplete" />.
    ///     Adds the <see cref="Handlers.Player.OnVerified" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.UserCode_CmdServerSignatureComplete))]
    internal static class Verified
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label callJoined = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions)))) + offset;

            // if(!Player.UnverifiedPlayers.TryGetValue(this._hub, out Player player)) {
            //     Means the player connected before WaitingForPlayers event is fired
            //     Let's call Joined event, since it wasn't called, to avoid breaking the logic of the order of event calls
            //     Blame NorthWood
            //
            //     Joined.CallEvent(_hub, out player);
            // }
            //
            // #if DEBUG
            // Log.Debug("{player.Nickname} has verified!");
            // #endif
            //
            // Player.Dictionary.Add(this._hub.gameObject, player);
            //
            // player.IsVerified = true;
            // player.RawUserId = player.UserId.GetRawUserId();
            //
            // Log.SendRaw("Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green);
            //
            // Player.OnVerified(new VerifiedEventArgs(player));
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Verified), nameof(Verified.handleCmdServerSignature))),

                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static void handleCmdServerSignature(ServerRoles inst)
        {
            if(!Player.UnverifiedPlayers.TryGetValue(inst._hub, out Player player)) {
                Joined.CallEvent(inst._hub, out player);
            }
            //

            Player.Dictionary.Add(inst._hub.gameObject, player);

            player.IsVerified = true;
            player.RawUserId = player.UserId.GetRawUserId();

            Log.SendRaw("Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green);

            Handlers.Player.OnVerified(new VerifiedEventArgs(player));
        }
    }
}