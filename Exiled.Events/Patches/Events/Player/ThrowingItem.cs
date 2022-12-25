// -----------------------------------------------------------------------
// <copyright file="ThrowingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ThrowableNetworkHandler.ServerProcessRequest" />.
    ///     Adds the <see cref="Handlers.Player.ThrowingItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessRequest))]
    internal static class ThrowingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ThrowingItemEventArgs));

            const int offset = 4;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Dup) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // throwableItem
                    new(OpCodes.Ldloc_1),

                    // msg.Request
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Request))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ThrowingItemEventArgs ev = new(Player, ThrowableItem, ThrowableNetworkHandler.RequestType, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnThrowingItem(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrowingItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // throwableItem = ev.Item.Base
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.Item))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                    new(OpCodes.Stloc_1),

                    // msg = new ThrowableNetworkHandler.ThrowableItemRequestMessage(msg.Serial, ev.RequestType, msg.CameraRotation, msg.CameraPosition, msg.PlayerVelocity)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Serial))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.RequestType))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraRotation))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraPosition))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.PlayerVelocity))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage))[0]),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}