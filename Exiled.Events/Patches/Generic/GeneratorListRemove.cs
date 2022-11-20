// -----------------------------------------------------------------------
// <copyright file="GeneratorListRemove.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Generator.OnDestroy"/>.
    /// </summary>
    // [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.OnDestroy))]
    internal class GeneratorListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(codeInstructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldsfld, Field(typeof(Generator), nameof(Generator.GeneratorValues))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Generator), nameof(Generator.Get), new[] { typeof(Scp079Generator) })),
                    new(OpCodes.Callvirt, Method(typeof(List<Generator>), nameof(List<Generator>.Remove))),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}