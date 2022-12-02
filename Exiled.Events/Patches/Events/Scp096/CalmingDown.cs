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

    using static HarmonyLib.AccessTools;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    ///     Patches <see cref="Scp096.EndEnrage" />.
    ///     Adds the <see cref="Handlers.Scp096.CalmingDown" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.EndEnrage))]
    internal static class CalmingDown
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            // CalmingDownEventArgs ev = new CalmingDownEventArgs(this, Player, true);
            //
            // Handlers.Scp096.OnCalmingDown(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CalmingDownEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnCalmingDown))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CalmingDownEventArgs), nameof(CalmingDownEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}