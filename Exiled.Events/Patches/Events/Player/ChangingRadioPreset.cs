// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPreset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Radio;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="RadioItem.ServerProcessCmd(RadioMessages.RadioCommand)" />.
    ///     Adds the <see cref="Handlers.Player.ChangingRadioPreset" /> event.
    /// </summary>
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    internal static class ChangingRadioPreset
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingRadioPresetEventArgs));

            const int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // (RadioRangeLevel)this._rangeId
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RadioItem), nameof(RadioItem._rangeId))),
                    new(OpCodes.Conv_I1),

                    // (RadioRangeLevel)b
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Conv_I1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = ChangingRadioPresetEventArgs(Player, byte, byte, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRadioPresetEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnChangingRadioPreset(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingRadioPreset))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRadioPresetEventArgs), nameof(ChangingRadioPresetEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // b = (byte)ev.NewValue
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRadioPresetEventArgs), nameof(ChangingRadioPresetEventArgs.NewValue))),
                    new(OpCodes.Conv_U1),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}