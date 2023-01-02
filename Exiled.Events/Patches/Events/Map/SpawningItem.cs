// -----------------------------------------------------------------------
// <copyright file="SpawningItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Map;
    using Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ItemDistributor.SpawnPickup" />.
    ///     Adds the <see cref="Map.SpawningItem" /> and <see cref="Map.SpawningItem" /> events.
    /// </summary>
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.SpawnPickup))]
    internal static class SpawningItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningItem));
            Label returnLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // ipb
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // SpawningItemEventArgs ev = new(ItemPickupBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningItemEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Map.OnSpawningItem(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnSpawningItem))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}