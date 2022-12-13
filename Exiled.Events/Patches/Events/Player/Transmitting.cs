// -----------------------------------------------------------------------
// <copyright file="Transmitting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Items;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles.Voice;
    using VoiceChat.Playbacks;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="PersonalRadioPlayback.IsTransmitting(ReferenceHub)" />.
    ///     Adds the <see cref="Handlers.Player.Transmitting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PersonalRadioPlayback), nameof(PersonalRadioPlayback.IsTransmitting))]
    internal static class Transmitting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = 4;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Isinst) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // hub
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // voiceModule
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Dup),

                    // voiceModule.ServerIsSending
                    new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ServerIsSending))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new TransmittingEventArgs(Player, VoiceModuleBase, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TransmittingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnTransmitting(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTransmitting))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TransmittingEventArgs), nameof(TransmittingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}