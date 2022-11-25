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

    using PlayableScps;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Subroutines;

    /// <summary>
    /// Defines a role that represents SCP-096.
    /// </summary>
    public class Scp096Role : ScpRole
    {
        private Scp096 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public Scp096Role(Player owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-096.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp096;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule => throw new System.NotImplementedException();

        /// <summary>
        /// Gets the <see cref="Scp096"/> script for the role.
        /// </summary>
        public Scp096 Script => script ??= Owner.ReferenceHub.scpsController.CurrentScp as Scp096;

        /// <summary>
        /// Gets a value indicating SCP-096's state.
        /// </summary>
        public Scp096PlayerState State => Script.PlayerState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 can receive targets.
        /// </summary>
        public bool CanReceiveTargets => Script.CanReceiveTargets;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently enraged.
        /// </summary>
        public bool IsEnraged => Script.Enraged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently docile.
        /// </summary>
        public bool IsDocile => !IsEnraged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently trying not to cry behind a door.
        /// </summary>
        public bool TryingNotToCry => State == Scp096PlayerState.TryNotToCry;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently prying a gate.
        /// </summary>
        public bool IsPryingGate => State == Scp096PlayerState.PryGate;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently charging.
        /// </summary>
        public bool IsCharging => Script.Charging;

        /// <summary>
        /// Gets or sets the amount of time in between SCP-096 charges.
        /// </summary>
        public float ChargeCooldown
        {
            get => Script._chargeCooldown;
            set => Script._chargeCooldown = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        public float EnrageCooldown
        {
            get => Script.RemainingEnrageCooldown;
            set => Script.RemainingEnrageCooldown = value;
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of Players that are currently targeted by SCP-096.
        /// </summary>
        public IReadOnlyCollection<Player> Targets => Script._targets.Select(Player.Get).ToList().AsReadOnly();
    }
}