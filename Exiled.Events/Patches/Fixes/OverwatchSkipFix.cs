// -----------------------------------------------------------------------
// <copyright file="OverwatchSkipFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

/*
namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.RunDefaultClassPicker(bool, out int[], out Dictionary{GameObject, RoleTypeId})"/>.
    /// Fixes overwatch players not spawning correctly.
    /// </summary>
    // [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RunDefaultClassPicker))]
    internal static class OverwatchSkipFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset
            int offset = 2;
            int lastBrOffset = 6;

            // Find the first Fraction.Others call.
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_3) + offset;

            // Find labels.
            Label continueLabel = (Label)newInstructions[index - 1].operand;
            Label originalLabel = (Label)newInstructions[index - lastBrOffset].operand;

            // Fix original label.
            Label fixLabel = generator.DefineLabel();
            newInstructions[index - lastBrOffset].operand = fixLabel;

            // if (referenceHub.serverRoles.OverwatchEnabled)
            // {
            //      list.Add(RoleTypeId.Spectator);
            //      playersRoleList.Add(referenceHub.gameObject, RoleTypeId.Spectator);
            //      continue;
            // }
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (referenceHub.serverRoles.OverwatchEnabled)
                    new(OpCodes.Ldloc_S, 9),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.serverRoles))),
                    new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles.OverwatchEnabled))),
                    new(OpCodes.Brfalse_S, originalLabel),

                    // list.Add(RoleTypeId.Spectator);
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Call, Method(typeof(List<int>), nameof(List<int>.Add))),

                    // playersRoleList.Add(referenceHub.gameObject, RoleTypeId.Spectator);
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Ldind_Ref),
                    new(OpCodes.Ldloc_S, 9),
                    new(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Call, Method(typeof(Dictionary<GameObject, RoleTypeId>), nameof(Dictionary<GameObject, RoleTypeId>.Add))),

                    // continue;
                    new(OpCodes.Br, continueLabel),
                });

            // Set original label.
            newInstructions[index].labels.Add(fixLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
*/