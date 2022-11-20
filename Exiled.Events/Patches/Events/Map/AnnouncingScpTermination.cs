// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

/*
namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using API.Features.Roles;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches
    ///     <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(ReferenceHub, PlayerStatsSystem.DamageHandlerBase)" />.
    ///     Adds the <see cref="Map.AnnouncingScpTermination" /> event.
    /// </summary>
    // [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingScpTerminationEventArgs));

            Label ret = generator.DefineLabel();
            Label jcc = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            newInstructions.RemoveRange(0, 19);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.DamageHandler))),
                    new(OpCodes.Isinst, typeof(DamageHandler)),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Base))),
                    new(OpCodes.Starg, 1),
                    new(OpCodes.Ldsfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.singleton))),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.scpListTimer))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.CurRole))),
                    new(OpCodes.Stloc_0),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.team))),
                    new(OpCodes.Brtrue_S, jmp),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.roleId))),
                    new(OpCodes.Ldc_I4_S, 10),
                    new(OpCodes.Bne_Un_S, jcc),
                    new CodeInstruction(OpCodes.Ret).WithLabels(jmp),
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jcc),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.TerminationCause))),
                    new(OpCodes.Stloc_1),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
*/