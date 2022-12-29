// -----------------------------------------------------------------------
// <copyright file="Stopping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Warhead = Handlers.Warhead;

    /// <summary>
    ///     Patches <see cref="AlphaWarheadController.CancelDetonation(ReferenceHub)" />.
    ///     Adds the <see cref="Warhead.StoppingWarhead" /> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), typeof(ReferenceHub))]
    internal static class StoppingWarhead
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int oldCount = newInstructions.Count;

            Label returnLabel = generator.DefineLabel();

            // if (!AlphaWarheadController.inProgress)
            //   return;
            //
            // StoppingWarheadEventArgs ev = new(Player.Get(disabler), true);
            //
            // Handlers.Warhead.OnStopping(ev);
            //
            // if (!ev.IsAllowed || Warhead.IsWarheadLocked)
            //   return;
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // if (!AlphaWarheadController.InProgress)
                    //    return;
                    new(OpCodes.Call, PropertyGetter(typeof(AlphaWarheadController), nameof(AlphaWarheadController.InProgress))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // Player.Get(disabler)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StoppingWarheadEventArgs ev = new(Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingWarheadEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Warhead.OnStopping(ev);
                    new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnStoppingWarhead))),

                    // if (!ev.IsAllowed || Warhead.IsWarheadLocked)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingWarheadEventArgs), nameof(StoppingWarheadEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Call, PropertyGetter(typeof(API.Features.Warhead), nameof(API.Features.Warhead.IsLocked))),
                    new(OpCodes.Brtrue_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}