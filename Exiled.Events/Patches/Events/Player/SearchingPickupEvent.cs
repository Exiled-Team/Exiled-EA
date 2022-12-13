// -----------------------------------------------------------------------
// <copyright file="SearchingPickupEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SearchCoordinator.ReceiveRequestUnsafe" />.
    ///     Adds the <see cref="Handlers.Player.SearchingPickup" /> event.
    /// </summary>
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ReceiveRequestUnsafe))]
    internal static class SearchingPickupEvent
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label allowLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SearchingPickupEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stind_Ref) + offset;

            // remove base-game check and `SearchSession body` setter
            newInstructions.RemoveRange(index, 14);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchCoordinator), nameof(SearchCoordinator.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // request.Target
                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Target))),

                    // request.Body
                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Body))),

                    // completor
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldind_Ref),

                    // request.Target.SearchTimeForPlayer(Hub)
                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Target))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchCoordinator), nameof(SearchCoordinator.Hub))),
                    new(OpCodes.Callvirt, Method(typeof(ItemPickupBase), nameof(ItemPickupBase.SearchTimeForPlayer))),

                    // SearchingPickupEventArgs ev = new(Player, ItemPickupBase, SearchSession, SearchCompletor, float);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SearchingPickupEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnSearchPickupRequest(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSearchPickupRequest))),

                    // if (ev.IsAllowed)
                    //    goto allowLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, allowLabel),

                    // session = default;
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Initobj, typeof(SearchSession)),

                    // completor = null
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Stind_Ref),

                    // return true
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ret),

                    // allowLabel:
                    //
                    // completor = ev.SearchCompletor
                    new CodeInstruction(OpCodes.Ldarg_2).WithLabels(allowLabel),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.SearchCompletor))),
                    new(OpCodes.Stind_Ref),

                    // session = ev.SearchSession
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.SearchSession))),
                    new(OpCodes.Stloc_1),
                });

            offset = -5;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder { LocalIndex: 4 }) + offset;

            // remove base-game SearchTime setter
            newInstructions.RemoveRange(index, 5);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // num3 = ev.SearchTime
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.SearchTime))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}