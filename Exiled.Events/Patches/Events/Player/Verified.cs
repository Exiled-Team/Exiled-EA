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
    using System.Reflection;
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
                    // Player.UnverifiedPlayers
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.UnverifiedPlayers))),

                    // this._hub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),

                    // if(Player.UnverifiedPlayers.TryGetValue(this._hub, out Player player))
                    //    goto callJoined;
                    new(OpCodes.Ldloca_S, player.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(ConditionalWeakTable<ReferenceHub, Player>), nameof(ConditionalWeakTable<ReferenceHub, Player>.TryGetValue))),
                    new(OpCodes.Brtrue_S, callJoined),

                    // Joined.CallEvent(this._hub, out Player player)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),
                    new(OpCodes.Ldloca_S, player.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Joined), nameof(Joined.CallEvent))),

                    // callJoined:
                    new CodeInstruction(OpCodes.Nop).WithLabels(callJoined),
#if DEBUG
                    // Log.Debug($"player.Nickname has been verified", true)
                    new(OpCodes.Ldstr, "{0} has been verified!"),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Nickname))),
                    new(OpCodes.Callvirt, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object) })),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Call, Method(typeof(Log), nameof(Log.Debug), new[] { typeof(string), typeof(bool) })),
#endif

                    // Player.Dictionary.Add(this._hub.gameObject, player)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.Dictionary))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<GameObject, Player>), nameof(Dictionary<GameObject, Player>.Add))),

                    // loads player three times
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // player.IsVerified = true
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Player), nameof(Player.IsVerified))),

                    // player.RawUserId = player.UserId.GetRawUserId()
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Call, Method(typeof(StringExtensions), nameof(StringExtensions.GetRawUserId))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Player), nameof(Player.RawUserId))),

                    // Log.SendRaw($"Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green)
                    new CodeInstruction(OpCodes.Ldstr, "Player {0} ({1}) ({2}) connected with the IP: {3}"),
                    new(OpCodes.Ldc_I4_4),
                    new(OpCodes.Newarr, typeof(object)),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Nickname))),
                    new(OpCodes.Stelem_Ref),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Stelem_Ref),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Id))),
                    new(OpCodes.Box, typeof(int)),
                    new(OpCodes.Stelem_Ref),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IPAddress))),
                    new(OpCodes.Stelem_Ref),
                    new(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object[]) })),
                    new(OpCodes.Ldc_I4_S, 10),
                    new(OpCodes.Call, Method(typeof(Log), nameof(Log.SendRaw), new[] { typeof(string), typeof(ConsoleColor) })),

                    // Handlers.Player.OnVerified(new VerifiedEventArgs(Player))
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VerifiedEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnVerified))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}