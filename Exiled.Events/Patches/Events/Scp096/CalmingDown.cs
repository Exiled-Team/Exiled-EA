// -----------------------------------------------------------------------
// <copyright file="CalmingDown.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.Events.EventArgs.Scp096;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp096RageManager.ServerEndEnrage(bool)" />.
    ///     Adds the <see cref="Handlers.Scp096.CalmingDown" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096RageManager), nameof(Scp096RageManager.ServerEndEnrage))]
    internal static class CalmingDown
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(CalmingDownEventArgs));

            const int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + offset;

            // CalmingDownEventArgs ev = new CalmingDownEventArgs(this, Player, clearTime, true);
            //
            // Handlers.Scp096.OnCalmingDown(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // base.ScpRole
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.ScpRole))),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // clearTime
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new CalmingDownEventArgs(Scp096Role, Player, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CalmingDownEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp096.OnCalmingDown(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnCalmingDown))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CalmingDownEventArgs), nameof(CalmingDownEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // clearTime = ev.ShouldClearEnragedTimeLeft
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CalmingDownEventArgs), nameof(CalmingDownEventArgs.ShouldClearEnragedTimeLeft))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}