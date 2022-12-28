// -----------------------------------------------------------------------
// <copyright file="Scp096Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Subroutines;

    using Scp096GameRole = PlayerRoles.PlayableScps.Scp096.Scp096Role;

    /// <summary>
    /// Defines a role that represents SCP-096.
    /// </summary>
    public class Scp096Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        private readonly IReadOnlyCollection<Player> emptyList = new List<Player>().AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp096GameRole"/>.</param>
        internal Scp096Role(Scp096GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
            Internal = baseRole;
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-096.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp096;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets a value indicating SCP-096's ability state.
        /// </summary>
        public Scp096AbilityState AbilityState => Internal.StateController.AbilityState;

        /// <summary>
        /// Gets a value indicating SCP-096's rage state.
        /// </summary>
        public Scp096RageState RageState => Internal.StateController.RageState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 can receive targets.
        /// </summary>
        public bool CanReceiveTargets => SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility ability) && ability._targetsTracker.CanReceiveTargets;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently enraged.
        /// </summary>
        public bool IsEnraged => RageState == Scp096RageState.Enraged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently docile.
        /// </summary>
        public bool IsDocile => !IsEnraged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently trying not to cry behind a door.
        /// </summary>
        public bool TryingNotToCry => AbilityState == Scp096AbilityState.TryingNotToCry;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently prying a gate.
        /// </summary>
        public bool IsPryingGate => AbilityState == Scp096AbilityState.PryingGate;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently charging.
        /// </summary>
        public bool IsCharging => AbilityState == Scp096AbilityState.Charging;

        /// <summary>
        /// Gets or sets the amount of time in between SCP-096 charges.
        /// </summary>
        public float ChargeCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility ability) ? ability._timeToChangeState : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility ability))
                {
                    ability._timeToChangeState = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        public float EnrageCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility ability) ? ability._activationTime.Remaining : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility ability))
                {
                    ability._activationTime.Remaining = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets enraged time left.
        /// </summary>
        public float EnragedTimeLeft
        {
            get => SubroutineModule.TryGetSubroutine(out Scp096RageManager ability) ? ability.EnragedTimeLeft : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp096RageManager ability))
                {
                    ability.EnragedTimeLeft = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets enraged time left.
        /// </summary>
        public float TotalEnrageTime
        {
            get => SubroutineModule.TryGetSubroutine(out Scp096RageManager ability) ? ability.TotalRageTime : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp096RageManager ability))
                {
                    ability.TotalRageTime = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of Players that are currently targeted by SCP-096.
        /// </summary>
        public IReadOnlyCollection<Player> Targets
            => SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility ability) ? ability._targetsTracker.Targets.Select(Player.Get).ToList().AsReadOnly() : emptyList;

        /// <summary>
        /// Gets the <see cref="Scp096GameRole"/>.
        /// </summary>
        protected Scp096GameRole Internal { get; }
    }
}