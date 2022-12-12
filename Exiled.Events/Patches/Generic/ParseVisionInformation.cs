// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp096;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;
    using Scp096Role = API.Features.Roles.Scp096Role;

    /// <summary>
    /// Patches <see cref="Scp096VisibilityController.ValidateVisibility(ReferenceHub)"/>.
    /// Adds the <see cref="Scp096Role.TurnedPlayers"/> support.
    /// </summary>
    [HarmonyPatch(typeof(Scp096VisibilityController), nameof(Scp096VisibilityController.ValidateVisibility))]
    internal static class ParseVisionInformation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Return pointer
            // Used to return execution
            // if both checks fail
            Label returnLabel = generator.DefineLabel();

            // Second check pointer
            // We use it to pass execution
            // to the second check if the first check fails,
            // otherwise the second check won't be executed
            Label secondCheckPointer = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial && !ExiledEvents.Instance.Config.CanTutorialTriggerScp096)
                    //      continue;
                    // START
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new(OpCodes.Bne_Un_S, secondCheckPointer),

                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanTutorialTriggerScp096))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // END
                    // if (Scp096Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
                    //      continue;
                    // START
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096Role), nameof(Scp096Role.TurnedPlayers))).WithLabels(secondCheckPointer),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brtrue_S, returnLabel),

                    // END
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}