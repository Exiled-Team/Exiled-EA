// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

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
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches
    ///     <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(ReferenceHub, PlayerStatsSystem.DamageHandlerBase)" />.
    ///     Adds the <see cref="Map.AnnouncingScpTermination" /> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingScpTerminationEventArgs));

            Label ret = generator.DefineLabel();
            Label jcc = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            newInstructions.RemoveRange(0, 12);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Player.Get(scp)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // hit
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new AnnouncingScpTerminationEventArgs(Player, DamageHandlerBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Map.OnAnnouncingScpTermination(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // ev.DamageHandler is DamageHandler handler
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.DamageHandler))),
                    new(OpCodes.Isinst, typeof(DamageHandler)),

                    // hit = handler.Base
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Base))),
                    new(OpCodes.Starg, 1),

                    // NineTailedFoxAnnouncer.singleton.scpListTimer = 0
                    new(OpCodes.Ldsfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.singleton))),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.scpListTimer))),

                    // if (!ev.Player.IsSCP)
                    //     goto jmp;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.Target))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsScp))),
                    new(OpCodes.Brfalse_S, jmp),

                    // if (ev.Role != RoleTypeId.Scp0492)
                    //     goto jcc;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.Role))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Type))),
                    new(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Scp0492),
                    new(OpCodes.Bne_Un_S, jcc),

                    // return
                    new CodeInstruction(OpCodes.Ret).WithLabels(jmp),

                    // announcement = ev.TerminationCause
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jcc),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.TerminationCause))),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}