// -----------------------------------------------------------------------
// <copyright file="StartingAndFinishingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.Events.EventArgs.Scp049;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayableScps;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Ragdoll = Ragdoll;

    /// <summary>
    ///     Patches <see cref="Scp049.BodyCmd_ByteAndGameObject(byte, GameObject)" />.
    ///     Adds the <see cref="Handlers.Scp049.StartingRecall" /> and <see cref="Handlers.Scp049.FinishingRecall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049), nameof(Scp049.BodyCmd_ByteAndGameObject))]
    internal static class StartingAndFinishingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -4;

            int index = newInstructions.FindIndex(
                instruction => (instruction.opcode == OpCodes.Stfld) &&
                               ((FieldInfo)instruction.operand == Field(typeof(Scp049), nameof(Scp049._recallHubServer)))) + offset;

            LocalBuilder finishRecallAllowed = generator.DeclareLocal(typeof(bool));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(component2.Info.OwnerHub)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldflda, Field(typeof(Ragdoll), nameof(Ragdoll.Info))),
                    new(OpCodes.Ldfld, Field(typeof(RagdollInfo), nameof(RagdollInfo.OwnerHub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp049), nameof(Scp049.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // component2
                    new(OpCodes.Ldloc_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = StartingRecallEventArgs(Player, Player, Ragdoll, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingRecallEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp049.OnStartingRecall(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnStartingRecall))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StartingRecallEventArgs), nameof(StartingRecallEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Beq_S);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // store whether player's role is spectator or not
                    new(OpCodes.Ceq),
                    new(OpCodes.Stloc_S, finishRecallAllowed.LocalIndex),

                    // Player.Get(ownerHub)
                    new(OpCodes.Ldloc_S, 6),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp049), nameof(Scp049.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // component
                    new(OpCodes.Ldloc, 5),

                    // is the player a spectator?
                    new(OpCodes.Ldloc_S, finishRecallAllowed.LocalIndex),

                    // var ev = new FinishingRecallEventArgs(Player, Player, Ragdoll, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingRecallEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp049.OnFinishingRecall(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingRecall))),

                    // load ev.IsAllowed for original methods beq to evaluate
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FinishingRecallEventArgs), nameof(FinishingRecallEventArgs.IsAllowed))),
                    new(OpCodes.Ldc_I4_1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}