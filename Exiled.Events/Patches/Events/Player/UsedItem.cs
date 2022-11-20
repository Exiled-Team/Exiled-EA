// -----------------------------------------------------------------------
// <copyright file="UsedItem.cs" company="Exiled Team">
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

    using InventorySystem.Items;
    using InventorySystem.Items.Usables;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    ///     Patches <see cref="Consumable.ServerOnUsingCompleted" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.ServerOnUsingCompleted))]
    internal static class UsedItem
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;

            newInstructions.InsertRange(index, InstructionsToInject());

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        internal static List<CodeInstruction> InstructionsToInject()
        {
            return new List<CodeInstruction>
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsedItemEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsedItem))),
            };
        }
    }

    /// <summary>
    ///     Patches <see cref="Scp268.ServerOnUsingCompleted" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    // [HarmonyPatch(typeof(Scp268), nameof(Scp268.ServerOnUsingCompleted))]
    internal static class UsedItem268
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;

            newInstructions.InsertRange(index, UsedItem.InstructionsToInject());

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}