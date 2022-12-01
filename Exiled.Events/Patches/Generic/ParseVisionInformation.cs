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
    using Exiled.API.Features.Roles;
    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayableScps;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="Scp096.UpdateVision"/>.
    /// Adds the <see cref="Scp096Role.TurnedPlayers"/> support.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.UpdateVision))]
    internal static class ParseVisionInformation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            const int offset = -3;
            const int continueLabelOffset = -3;

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int index = newInstructions.FindIndex(ci => ci.opcode == OpCodes.Div);

            // Quick check if it's the end
            if (index + 1 >= newInstructions.Count)
            {
                Log.Error($"Couldn't patch '{typeof(Scp096).FullName}.{nameof(Scp096.UpdateVision)}': invalid index - {index}");
                ListPool<CodeInstruction>.Shared.Return(newInstructions);
                yield break;
            }

            index += offset;

            // Continuation pointer
            // Used to continue execution
            // if both checks fail
            Label continueLabel = generator.DefineLabel();
            newInstructions[newInstructions.FindIndex(ci => ci.opcode == OpCodes.Leave_S) + continueLabelOffset].WithLabels(continueLabel);

            // Second check pointer
            // We use it to pass execution
            // to the second check if the first check fails,
            // otherwise the second check won't be executed
            Label secondCheckPointer = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial && !ExiledEvents.Instance.Config.CanTutorialTriggerScp096)
                    //      continue;
                    // START
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new(OpCodes.Bne_Un_S, secondCheckPointer),

                    new(OpCodes.Call, PropertyGetter(typeof(ExiledEvents), nameof(ExiledEvents.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Plugin<Config>), nameof(Plugin<Config>.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanTutorialTriggerScp096))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // END
                    // if (Scp096Role.TurnedPlayers.Contains(Player.Get(referenceHub)))
                    //      continue;
                    // START
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096Role), nameof(Scp096Role.TurnedPlayers))).WithLabels(secondCheckPointer),
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // END
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}