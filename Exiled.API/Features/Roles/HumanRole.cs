// -----------------------------------------------------------------------
// <copyright file="HumanRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Respawning;

    using HumanGameRole = PlayerRoles.HumanRole;

    /// <summary>
    /// Defines a role that represents a human class.
    /// </summary>
    public abstract class HumanRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanRole"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        protected HumanRole(Player owner)
            : base(owner)
        {
            Internal = Base as HumanGameRole;
        }

        /// <summary>
        /// Gets or sets the <see cref="SpawnableTeamType"/>.
        /// </summary>
        public SpawnableTeamType SpawnableTeamType
        {
            get => Internal.AssignedSpawnableTeam;
            set => Internal.AssignedSpawnableTeam = value;
        }

        /// <summary>
        /// Gets the <see cref="UnitName"/>.
        /// </summary>
        public string UnitName => Internal.UnitName;

        /// <summary>
        /// Gets or sets the <see cref="UnitNameId"/>.
        /// </summary>
        public byte UnitNameId
        {
            get => Internal.UnitNameId;
            set => Internal.UnitNameId = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="HumanRole"/> uses unit names or not.
        /// </summary>
        public bool UsesUnitNames => Internal.UsesUnitNames;

        /// <summary>
        /// Gets the game <see cref="HumanGameRole"/>.
        /// </summary>
        protected HumanGameRole Internal { get; }

        /// <summary>
        /// Gets the <see cref="HumanRole"/> armor efficacy based on a specific <see cref="HitboxType"/> and the armor the <see cref="Role.Owner"/> is wearing.
        /// </summary>
        /// <param name="hitbox">The <see cref="HitboxType"/>.</param>
        /// <returns>The armor efficacy.</returns>
        public int GetArmorEfficacy(HitboxType hitbox) => Internal.GetArmorEfficacy(hitbox);
    }
}