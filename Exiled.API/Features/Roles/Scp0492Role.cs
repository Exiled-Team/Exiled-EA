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
            SubroutineModule = (Base as ZombieRole).SubroutineModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp0492;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets the SCP-049-2 attack damage.
        /// </summary>
        public float AttackDamage
        {
            get => SubroutineModule.TryGetSubroutine(out ZombieAttackAbility ability) ? ability.DamageAmount : 0;
        }

        /// <summary>
        /// Gets or sets a value indicating the amount of time to simulate SCP-049-2's Bloodlust ability.
        /// </summary>
        public float SimulatedStare
        {
            get => SubroutineModule.TryGetSubroutine(out ZombieBloodlustAbility ability) ? ability.SimulatedStare : 0;
            set
            {
                if (!SubroutineModule.TryGetSubroutine(out ZombieBloodlustAbility ability))
                    return;
                ability.SimulatedStare = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-049-2 is currently pursuing a target (Bloodlust ability).
        /// </summary>
        public bool BloodlustActive
        {
            get => SubroutineModule.TryGetSubroutine(out ZombieBloodlustAbility ability) ? ability.LookingAtTarget : false;
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-049-2 is consuming a ragdoll.
        /// </summary>
        public bool IsConsuming => SubroutineModule.TryGetSubroutine(out ZombieConsumeAbility ability) ? ability.IsInProgress : false;

        /// <summary>
        /// Gets the <see cref="Ragdoll"/> that SCP-049-2 is currently consuming. Will be <see langword="null"/> if <see cref="IsConsuming"/> is <see langword="false"/>.
        /// </summary>
        public Ragdoll RagdollConsuming => SubroutineModule.TryGetSubroutine(out ZombieConsumeAbility ability) ? Ragdoll.Get(ability.CurRagdoll) : null;

        /// <summary>
        /// Gets the amount of time in between SCP-049-2 attacks.
        /// </summary>
        public float AttackCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out ZombieAttackAbility ability) ? ability.BaseCooldown : 0;
        }
    }
}