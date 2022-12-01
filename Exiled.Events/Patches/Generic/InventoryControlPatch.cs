// -----------------------------------------------------------------------
// <copyright file="InventoryControlPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;

    using MEC;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Inventory = InventorySystem.Inventory;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem"/> to help manage <see cref="Player.Items"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal static class InventoryControlAddPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = -2;
            int index = newInstructions.FindIndex(
                i =>
                    (i.opcode == OpCodes.Callvirt) &&
                    ((MethodInfo)i.operand == Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

            // AddItem(Player.Get(inv._hub), itemInstance)
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(inv._hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemInstance
                    new(OpCodes.Ldloc_1),

                    // AddItem(player, itemInstance)
                    new(OpCodes.Call, Method(typeof(InventoryControlAddPatch), nameof(AddItem))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void AddItem(Player player, ItemBase item) => player?.ItemsValue.Add(Item.Get(item));
    }

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerDropItem"/> to help manage <see cref="Player.Items"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerRemoveItem))]
    internal static class InventoryControlRemovePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Throw) + offset;

            // RemoveItem(Player.Get(inv._hub), itemSerial)
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(inv._hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemSerial
                    new(OpCodes.Ldarg_1),

                    // RemoveItem(Player.Get(inv._hub), itemSerial)
                    new(OpCodes.Call, Method(typeof(InventoryControlRemovePatch), nameof(RemoveItem))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void RemoveItem(Player player, ushort serial)
        {
#if DEBUG
            Log.Debug($"Removing item ({serial}) from a player (before null check)");
#endif
            if (player is null)
            {
#if DEBUG
                Log.Debug("Attempted to remove item from null player, returning.");
#endif
                return;
            }

            if (!player.Inventory.UserInventory.Items.ContainsKey(serial))
            {
#if DEBUG
                Log.Debug("Attempted to remove an item the player doesn't own, returning.");
#endif
                return;
            }
#if DEBUG
            Log.Debug(
                $"Inventory Info (before): {player.Nickname} - {player.Items.Count} ({player.Inventory.UserInventory.Items.Count})");
            foreach (Item item in player.Items)
                Log.Debug($"{item.Type} ({item.Serial})");
#endif
            ItemBase itemBase = player.Inventory.UserInventory.Items[serial];

            player.ItemsValue.Remove(Item.Get(itemBase));

            Timing.CallDelayed(
                0.15f,
                () =>
                {
                    if (player.Inventory.UserInventory.Items.ContainsKey(serial))
                    {
                        player.Inventory.UserInventory.Items.Remove(serial);
                        player.Inventory.SendItemsNextFrame = true;
#if DEBUG
                    Log.Debug($"Removed orphaned item from {player.Nickname} inventory dict.");
#endif
                    }
#if DEBUG
                Log.Debug($"Item ({serial}) removed from {player.Nickname}");
                Log.Debug(
                    $"Inventory Info (after): {player.Nickname} - {player.Items.Count} ({player.Inventory.UserInventory.Items.Count})");
                foreach (Item item in player.Items)
                    Log.Debug($"{item.Type} ({item.Serial})");
#endif
                });
        }
    }
}