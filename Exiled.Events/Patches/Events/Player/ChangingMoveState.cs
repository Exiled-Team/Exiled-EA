// -----------------------------------------------------------------------
// <copyright file="ChangingMoveState.cs" company="Exiled Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="AnimationController.UserCode_CmdChangeSpeedState" />.
    ///     Adds the <see cref="Player.ChangingMoveState" /> event.
    /// </summary>
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.UserCode_CmdChangeSpeedState))]
    internal static class ChangingMoveState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingMoveStateEventArgs));

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player.Get(this._hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(AnimationController), nameof(AnimationController._hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),

                    // player.MoveState
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.MoveState))),

                    // newState
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingMoveStateEventArgs ev = new(Player, PlayerMovementState, PlayerMovementState, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMoveStateEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.OnChangingMoveState(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingMoveState))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // newState = ev.NewState
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.NewState))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}