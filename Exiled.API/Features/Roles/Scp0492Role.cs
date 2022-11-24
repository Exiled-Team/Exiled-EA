// -----------------------------------------------------------------------
// <copyright file="Scp0492Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayableScps;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using YamlDotNet.Core.Tokens;

    /// <summary>
    /// Defines a role that represents SCP-049-2.
    /// </summary>
    public class Scp0492Role : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp0492Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp0492Role(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="Scp049_2PlayerScript"/> for this role.
        /// </summary>
        public ZombieRole Script => Owner.RoleManager.CurrentRole as ZombieRole;

        /// <summary>
        /// Gets or sets the SCP-049-2 attack distance.
        /// </summary>
        public float AttackDistance
        {
            get => Script.distance;
            set => Script.distance = value;
        }

        /// <summary>
        /// Gets or sets the SCP-049-2 attack damage.
        /// </summary>
        public float AttackDamage
        {
            get => 40;
            set { }
        }

        /// <summary>
        /// Gets or sets the amount of time in between SCP-049-2 attacks.
        /// </summary>
        public float AttackCooldown
        {
            get => Script.attackCooldown;
            set => Script.attackCooldown = value;
        }

        /// <inheritdoc/>
        internal override RoleTypeId TypeId => RoleTypeId.Scp0492;
    }
}