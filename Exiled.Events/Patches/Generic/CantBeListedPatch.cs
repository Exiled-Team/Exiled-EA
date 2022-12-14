// -----------------------------------------------------------------------
// <copyright file="CantBeListedPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="ServerConsole.RefreshEmailSetStatus"/> to prevent the server from being listed.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshEmailSetStatus))]
    internal class CantBeListedPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int index = 0;

            // Delete the first 7 instructions
            newInstructions.RemoveRange(index, 7);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // false
                    new(OpCodes.Ldc_I4_0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
