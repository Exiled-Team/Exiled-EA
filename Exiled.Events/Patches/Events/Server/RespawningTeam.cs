// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using Respawning;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patch the <see cref="RespawnManager.Spawn" />.
    ///     Adds the <see cref="Server.RespawningTeam" /> event.
    /// </summary>
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class RespawningTeam
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_3) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(RespawningTeamEventArgs));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // GetPlayers(list);
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetPlayers))),

                    // maxWaveSize
                    new(OpCodes.Ldloc_2),

                    // this.NextKnownTeam
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RespawningTeamEventArgs ev = new(players, num, this.NextKnownTeam)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RespawningTeamEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Server.OnRespawningTeam(ev)
                    new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRespawningTeam))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // this.NextKnownTeam = SpawnableTeam.None
                    //    return;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Stfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),
                    new(OpCodes.Ret),

                    // load "ev" three times
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // num = ev.MaximumRespawnAmount
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.MaximumRespawnAmount))),
                    new(OpCodes.Stloc_2),

                    // spawnableTeamHandler = ev.SpawnableTeam
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.SpawnableTeam))),
                    new(OpCodes.Stloc_0),

                    // list = GetHubs(ev.Players)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Players))),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetHubs))),
                    new(OpCodes.Stloc_1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<Player> GetPlayers(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();

        private static List<ReferenceHub> GetHubs(List<Player> players) => players.Select(player => player.ReferenceHub).ToList();
    }
}