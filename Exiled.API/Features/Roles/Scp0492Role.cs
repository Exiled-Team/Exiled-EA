// -----------------------------------------------------------------------
// <copyright file="Scp0492Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.PlayableScps.Subroutines;

    /// <summary>
    /// Defines a role that represents SCP-049-2.
    /// </summary>
    public class Scp0492Role : ScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp0492Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public Scp0492Role(Player owner)
            : base(owner)
        {
            SubroutineModule = (Owner.RoleManager.CurrentRole as ZombieRole).SubroutineModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp0492;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets or sets the SCP-049-2 attack distance.
        /// </summary>
        public float AttackDistance
        {
            get => SubroutineModule.TryGetSubroutine(out ZombieAttackAbility ability) ? ability._range : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out ZombieAttackAbility ability))
                    ability._range = value;
            }
        }

        /// <summary>
        /// Gets or sets the SCP-049-2 attack damage.
        /// </summary>
        public float AttackDamage
        {
            get => 40; // It's hardcoded.
            set { }
        }

        /// <summary>
        /// Gets or sets the amount of time in between SCP-049-2 attacks.
        /// </summary>
        public float AttackCooldown
        {
            get => 1.3f; // It's hardcoded.
            set { }
        }
    }
}