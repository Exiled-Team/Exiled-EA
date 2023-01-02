// -----------------------------------------------------------------------
// <copyright file="Destroying.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600

    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Destroying
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            newInstructions[0].labels.Add(continueLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player player = Player.Get(this)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // DestroyingEventArgs ev = new(Player)
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DestroyingEventArgs))[0]),

                    // Handlers.Player.OnDestroying(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDestroying))),

                    // Player.Dictionary.Remove(player.GameObject)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.Dictionary))),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.GameObject))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<GameObject, Player>), nameof(Dictionary<GameObject, Player>.Remove), new[] { typeof(GameObject) })),
                    new(OpCodes.Pop),

                    // Player.UnverifiedPlayers.Remove(this)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.UnverifiedPlayers))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, Method(typeof(ConditionalWeakTable<ReferenceHub, Player>), nameof(ConditionalWeakTable<ReferenceHub, Player>.Remove), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Pop),

                    // Player.IdsCache.Remove(player.Id)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.IdsCache))),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Id))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<int, Player>), nameof(Dictionary<int, Player>.Remove), new[] { typeof(int) })),
                    new(OpCodes.Pop),

                    // if (player.UserId == null)
                    //    goto continueLabel;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // Player.UserIdsCache.Remove(player.UserId)
                    new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.UserIdsCache))),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<string, Player>), nameof(Dictionary<string, Player>.Remove), new[] { typeof(string) })),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}