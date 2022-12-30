// -----------------------------------------------------------------------
// <copyright file="ChangingMoveState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles.FirstPersonControl;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="FirstPersonMovementModule.SyncMovementState" /> setter.
    ///     Adds the <see cref="Player.ChangingMoveState" /> event.
    /// </summary>
    [HarmonyPatch(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.SyncMovementState), MethodType.Setter)]
    internal static class ChangingMoveState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingMoveStateEventArgs));

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            const int index = 0;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // oldState
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.SyncMovementState))),

                    // value
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingMoveStateEventArgs ev = new(Player, PlayerMovementState, PlayerMovementState, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMoveStateEventArgs))[0]),
                    new(OpCodes.Dup, ev.LocalIndex),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // if (ev.OldState == ev.NewState)
                    //    goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.OldState))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.NewState))),
                    new(OpCodes.Beq_S, continueLabel),

                    // load ev twice
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Dup),

                    // Player.OnChangingMoveState(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingMoveState))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // value = ev.NewState
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.NewState))),
                    new(OpCodes.Starg_S, 1),
                });

            // return the state
            newInstructions[newInstructions.Count - 2].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}