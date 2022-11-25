// -----------------------------------------------------------------------
// <copyright file="Respawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Linq;

    using Enums;

    using Respawning;

    using UnityEngine;

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        /// <summary>
        /// Gets the next known <see cref="SpawnableTeamType"/> that will spawn.
        /// </summary>
        public static SpawnableTeamType NextKnownTeam => RespawnManager.Singleton.NextKnownTeam;

        /// <summary>
        /// Gets the amount of seconds before the next respawn will occur.
        /// </summary>
        [Obsolete("Use TimeUntilSpawnWave.TotalSeconds.")]
        public static int TimeUntilRespawn => (int)TimeUntilSpawnWave.TotalSeconds;

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> indicating the amount of time before the next respawn wave will occur.
        /// </summary>
        public static TimeSpan TimeUntilSpawnWave => TimeSpan.FromSeconds(RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the moment in UTC time the next respawn wave will occur.
        /// </summary>
        public static DateTime NextTeamTime => DateTime.UtcNow.AddSeconds(TimeUntilSpawnWave.TotalSeconds);

        /// <summary>
        /// Gets a value indicating whether or not a team is currently being spawned or the animations are playing for a team.
        /// </summary>
        public static bool IsSpawning => RespawnManager.Singleton._curSequence == RespawnManager.RespawnSequencePhase.PlayingEntryAnimations || RespawnManager.Singleton._curSequence == RespawnManager.RespawnSequencePhase.SpawningSelectedTeam;

        /// <summary>
        /// Gets or sets the amount of spawn tickets belonging to the NTF.
        /// </summary>
        /// <seealso cref="ChaosTickets"/>
        public static float NtfTickets
        {
            get => RespawnTokensManager.Counters[0].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, value);
        }

        /// <summary>
        /// Gets or sets the amount of spawn tickets belonging to the Chaos Insurgency.
        /// </summary>
        /// <seealso cref="NtfTickets"/>
        public static float ChaosTickets
        {
            get => RespawnTokensManager.Counters[1].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, value);
        }

        /// <summary>
        /// Gets the actual <see cref="RespawnEffectsController"/>.
        /// </summary>
        [Obsolete("Using this will lead to indefinable errors", true)]
        public static RespawnEffectsController Controller => RespawnEffectsController.AllControllers.FirstOrDefault(controller => controller != null);

        /// <summary>
        /// Play an effect when a certain class spawns.
        /// </summary>
        /// <param name="effect">The effect to be played.</param>
        public static void PlayEffect(byte effect) => PlayEffects(new[] { effect });

        /// <summary>
        /// Play an effect when a certain class spawns.
        /// </summary>
        /// <param name="effect">The effect to be played.</param>
        public static void PlayEffect(RespawnEffectType effect) => PlayEffects(new[] { effect });

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="effects">The effects to be played.</param>
        public static void PlayEffects(byte[] effects)
        {
            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                    controller.RpcPlayEffects(effects);
            }
        }

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="effects">The effects to be played.</param>
        public static void PlayEffects(RespawnEffectType[] effects) => PlayEffects(effects.Select(effect => (byte)effect).ToArray());

        /// <summary>
        /// Summons the NTF chopper.
        /// </summary>
        public static void SummonNtfChopper() => PlayEffects(new[] { RespawnEffectType.SummonNtfChopper });

        /// <summary>
        /// Summons the <see cref="Side.ChaosInsurgency"/> van.
        /// </summary>
        /// <param name="playMusic">Whether or not to play the Chaos Insurgency spawn music.</param>
        public static void SummonChaosInsurgencyVan(bool playMusic = true)
        {
            PlayEffects(
                playMusic
                    ? new[]
                    {
                        RespawnEffectType.PlayChaosInsurgencyMusic,
                        RespawnEffectType.SummonChaosInsurgencyVan,
                    }
                    : new[]
                    {
                        RespawnEffectType.SummonChaosInsurgencyVan,
                    });
        }

        /// <summary>
        /// Grants tickets to a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to grant tickets to.</param>
        /// <param name="amount">The amount of tickets to grant.</param>
        // /// <returns>Whether or not tickets were granted successfully.</returns>
        public static void GrantTickets(SpawnableTeamType team, float amount) => RespawnTokensManager.GrantTokens(team, amount);

        /// <summary>
        /// Forces a spawn of the given <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to spawn.</param>
        /// <param name="playEffects">Whether or not effects will be played with the spawn.</param>
        public static void ForceWave(SpawnableTeamType team, bool playEffects = false)
        {
            if (playEffects)
            {
                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, team);
            }

            RespawnManager.Singleton.ForceSpawnTeam(team);
        }
    }
}