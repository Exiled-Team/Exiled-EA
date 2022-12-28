// -----------------------------------------------------------------------
// <copyright file="Scp0492Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.PlayableScps.Subroutines;

    /// <summary>
    /// Defines a role that represents SCP-049-2.
    /// </summary>
    public class Scp0492Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp0492Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="ZombieRole"/>.</param>
        internal Scp0492Role(ZombieRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp0492;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets or sets the amount of times this SCP-049-2 has been resurrected.
        /// </summary>
        public int ResurrectNumber
        {
            get => Scp049ResurrectAbility.GetResurrectionsNumber(Owner.ReferenceHub);
            set => Scp049ResurrectAbility.ResurrectedPlayers[Owner.ReferenceHub.netId] = value;
        }

        /// <summary>
        /// Gets the SCP-049-2 attack damage.
        /// </summary>
        public float AttackDamage => SubroutineModule.TryGetSubroutine(out ZombieAttackAbility ability) ? ability.DamageAmount : 0;

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
        public bool BloodlustActive => SubroutineModule.TryGetSubroutine(out ZombieBloodlustAbility ability) && ability.LookingAtTarget;

        /// <summary>
        /// Gets a value indicating whether or not SCP-049-2 is consuming a ragdoll.
        /// </summary>
        public bool IsConsuming => SubroutineModule.TryGetSubroutine(out ZombieConsumeAbility ability) && ability.IsInProgress;

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

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049-2 is close enough to a ragdoll to consume it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049-2 is close enough to the body to consume it; the body may have been consumed previously. Make sure to check <see cref="Ragdoll.IsConsumed"/> to ensure the body can be consumed.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to consume the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInConsumeRange(BasicRagdoll ragdoll) => SubroutineModule.TryGetSubroutine(out ZombieConsumeAbility ability) && ability.IsCloseEnough(Owner.Position, ragdoll.transform.position);

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049-2 is close enough to a ragdoll to consume it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049-2 is close enough to the body to consume it; the body may have been consumed previously. Make sure to check <see cref="Ragdoll.IsConsumed"/> to ensure the body can be consumed.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to consume the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInConsumeRange(Ragdoll ragdoll) => IsInConsumeRange(ragdoll.Base);
    }
}