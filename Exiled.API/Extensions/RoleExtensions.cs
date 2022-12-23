// -----------------------------------------------------------------------
// <copyright file="RoleExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Spawn;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl.Spawnpoints;
    using UnityEngine;

    using Team = PlayerRoles.Team;

    /// <summary>
    /// A set of extensions for <see cref="RoleTypeId"/>.
    /// </summary>
    public static class RoleExtensions
    {
        /// <summary>
        /// Get a <see cref="RoleTypeId">role's</see> <see cref="Color"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/> to get the color of.</param>
        /// <returns>The <see cref="Color"/> of the role.</returns>
        public static Color GetColor(this RoleTypeId typeId) => typeId == RoleTypeId.None ? Color.white : typeId.GetRoleBase().RoleColor;

        /// <summary>
        /// Get a <see cref="RoleTypeId">role's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/> to check the side of.</param>
        /// <returns><see cref="Side"/>.</returns>
        public static Side GetSide(this RoleTypeId typeId) => GetTeam(typeId).GetSide();

        /// <summary>
        /// Get a <see cref="Team">team's</see> <see cref="Side"/>.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to get the <see cref="Side"/> of.</param>
        /// <returns><see cref="Side"/>.</returns>.
        public static Side GetSide(this Team team) => team switch
        {
            Team.SCPs => Side.Scp,
            Team.FoundationForces or Team.Scientists => Side.Mtf,
            Team.ChaosInsurgency or Team.ClassD => Side.ChaosInsurgency,
            Team.OtherAlive => Side.Tutorial,
            _ => Side.None,
        };

        /// <summary>
        /// Get the <see cref="Team"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see cref="Team"/>.</returns>
        public static Team GetTeam(this RoleTypeId typeId) => typeId switch
        {
            RoleTypeId.ChaosConscript or RoleTypeId.ChaosMarauder or RoleTypeId.ChaosRepressor or RoleTypeId.ChaosRifleman => Team.ChaosInsurgency,
            RoleTypeId.Scientist => Team.Scientists,
            RoleTypeId.ClassD => Team.ClassD,
            RoleTypeId.Scp049 or RoleTypeId.Scp939 or RoleTypeId.Scp0492 or RoleTypeId.Scp079 or RoleTypeId.Scp096 or RoleTypeId.Scp106 or RoleTypeId.Scp173 => Team.SCPs,
            RoleTypeId.FacilityGuard or RoleTypeId.NtfCaptain or RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfSpecialist => Team.FoundationForces,
            RoleTypeId.Tutorial => Team.OtherAlive,
            _ => Team.Dead,
        };

        /// <summary>
        /// Gets the full name of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The full name.</returns>
        public static string GetFullName(this RoleTypeId typeId) => typeId.GetRoleBase().RoleName;

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/> of the given <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The <see cref="PlayerRoleBase"/>.</returns>
        public static PlayerRoleBase GetRoleBase(this RoleTypeId typeId) => Server.Host.RoleManager.GetRoleBase(typeId);

        /// <summary>
        /// Get the <see cref="LeadingTeam"/>.
        /// </summary>
        /// <param name="team">Team.</param>
        /// <returns><see cref="LeadingTeam"/>.</returns>
        public static LeadingTeam GetLeadingTeam(this Team team) => team switch
        {
            Team.ClassD or Team.ChaosInsurgency => LeadingTeam.ChaosInsurgency,
            Team.FoundationForces or Team.Scientists => LeadingTeam.FacilityForces,
            Team.SCPs => LeadingTeam.Anomalies,
            _ => LeadingTeam.Draw,
        };

        /// <summary>
        /// Gets a random spawn point of a <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> to get the spawn point from.</param>
        /// <returns>Returns a <see cref="SpawnPosition"/> representing the spawn, or <see langword="null"/> if no spawns were found.</returns>
        public static SpawnPosition GetRandomSpawnProperties(this RoleTypeId roleType)
        {
            return !RoleSpawnpointManager.TryGetSpawnpointForRole(roleType, out ISpawnpointHandler spawnpoint) ||
                !spawnpoint.TryGetSpawnpoint(out Vector3 position, out float horizontalRotation) ?
                null :
                new SpawnPosition(roleType, position, horizontalRotation);
        }
    }
}