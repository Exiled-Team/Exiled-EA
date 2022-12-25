// -----------------------------------------------------------------------
// <copyright file="FinishingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.Events.EventArgs.Scp049;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.FinishingRecall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerComplete))]
    internal static class FinishingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ownerHub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp049Role>), nameof(ScpStandardSubroutine<Scp049Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // base.CurRagdoll
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(RagdollAbilityBase<Scp049Role>), nameof(RagdollAbilityBase<Scp049Role>.CurRagdoll))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // FinishingRecallEventArgs ev = new(Player, Player, BasicRagdoll, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingRecallEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp049.OnFinishingRecall(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingRecall))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingRecallEventArgs), nameof(FinishingRecallEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}