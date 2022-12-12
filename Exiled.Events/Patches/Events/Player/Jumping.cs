// -----------------------------------------------------------------------
// <copyright file="Jumping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="FpcMotor.UpdateGrounded(ref Vector3, ref bool, float)" />
    ///     Adds the <see cref="Player.Jumping" /> event.
    /// </summary>
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdateGrounded))]
    internal static class Jumping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(JumpingEventArgs));
            LocalBuilder direction = generator.DeclareLocal(typeof(Vector3));

            Label ret = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(FpcMotor), nameof(FpcMotor.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // moveDir
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldobj, typeof(Vector3)),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new JumpingEventArgs(Player, Vector3, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JumpingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.OnJumping(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnJumping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // moveDir = ev.Direction
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.Direction))),
                    new(OpCodes.Stloc_S, direction.LocalIndex),
                    new(OpCodes.Ldloca_S, direction.LocalIndex),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}