// -----------------------------------------------------------------------
// <copyright file="PickupControlPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Inventory = InventorySystem.Inventory;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup(Inventory, ItemBase, PickupSyncInfo, bool)"/> to save scale for pickups and control <see cref="Pickup.IsSpawned"/> property.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class PickupControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_3) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // pickup = Pickup.Get(pickupBase);
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),
                new(OpCodes.Dup),

                // Item.Get(itemBase);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),

                // pickup.Scale = item.Scale
                new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Scale))),
                new(OpCodes.Callvirt, PropertySetter(typeof(Pickup), nameof(Pickup.Scale))),

                // pickup.IsSpawned = spawn
                new(OpCodes.Ldarg_3),
                new(OpCodes.Callvirt, PropertySetter(typeof(Pickup), nameof(Pickup.IsSpawned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
