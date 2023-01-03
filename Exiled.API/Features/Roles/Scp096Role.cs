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

            if (!SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility scp096RageCycleAbility))
                Log.Debug("RageCycleAbility subroutine not found in Scp096Role::ctor");

            RageCycleAbility = scp096RageCycleAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp096RageManager scp096RageManager))
                Log.Debug("RageManager subroutine not found in Scp096Role::ctor");

            RageManager = scp096RageManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp096TargetsTracker scp096TargetsTracker))
                Log.Debug("TargetsTracker not found in Scp096Role::ctor");

            TargetsTracker = scp096TargetsTracker;
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
        /// Gets .
        /// </summary>
        public Scp096RageCycleAbility RageCycleAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp096RageManager RageManager { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp096TargetsTracker TargetsTracker { get; }

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
        public bool CanReceiveTargets => RageCycleAbility._targetsTracker.CanReceiveTargets;

        /// <summary>
        /// Gets or sets the amount of time in between SCP-096 charges.
        /// </summary>
        public float ChargeCooldown
        {
            get => RageCycleAbility._timeToChangeState;
            set
            {
                RageCycleAbility._timeToChangeState = value;
                RageCycleAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        public float EnrageCooldown
        {
            get => RageCycleAbility._activationTime.Remaining;
            set
            {
                RageCycleAbility._activationTime.Remaining = value;
                RageCycleAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets enraged time left.
        /// </summary>
        public float EnragedTimeLeft
        {
            get => RageManager.EnragedTimeLeft;
            set
            {
                RageManager.EnragedTimeLeft = value;
                RageManager.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets enraged time left.
        /// </summary>
        public float TotalEnrageTime
        {
            get => RageManager.TotalRageTime;
            set
            {
                RageManager.TotalRageTime = value;
                RageManager.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of Players that are currently targeted by SCP-096.
        /// </summary>
        public IReadOnlyCollection<Player> Targets => RageCycleAbility._targetsTracker.Targets.Select(Player.Get).ToList().AsReadOnly();

        /// <summary>
        /// Gets the <see cref="Scp096GameRole"/>.
        /// </summary>
        protected Scp096GameRole Internal { get; }

        /// <summary>
        /// e.
        /// </summary>
        /// <param name="player">.</param>
        public void AddTarget(Player player) => TargetsTracker.AddTarget(player.ReferenceHub);

        /// <summary>
        /// e.
        /// </summary>
        /// <param name="player">.</param>
        public void RemoveTarget(Player player) => TargetsTracker.RemoveTarget(player.ReferenceHub);

        /// <summary>
        /// .
        /// </summary>
        /// <param name="time">e.</param>
        public void Enrage(float time) => RageManager.ServerEnrage(time);

        /// <summary>
        /// .
        /// </summary>
        /// <param name="player">ee.</param>
        /// <returns>e.</returns>
        public bool HasTarget(Player player) => TargetsTracker.HasTarget(player.ReferenceHub);

        /// <summary>
        /// .
        /// </summary>
        /// <param name="player">ee.</param>
        /// <returns>e.</returns>
        public bool IsObserved(Player player) => TargetsTracker.IsObservedBy(player.ReferenceHub);

        /// <summary>
        /// .
        /// </summary>
        public void ClearTarget() => TargetsTracker.ClearAllTargets();
    }
}