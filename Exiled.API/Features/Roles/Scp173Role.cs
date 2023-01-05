// -----------------------------------------------------------------------
// <copyright file="Scp173Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;

    using UnityEngine;

    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;

    /// <summary>
    /// Defines a role that represents SCP-173.
    /// </summary>
    public class Scp173Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp173Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp173GameRole"/>.</param>
        internal Scp173Role(Scp173GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
            MovementModule = FirstPersonController.FpcModule as Scp173MovementModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker scp173ObserversTracker))
                Log.Error("Scp173ObserversTracker not found in Scp173Role::ctor");

            ObserversTracker = scp173ObserversTracker;

            if (!SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer scp173BlinkTimer))
                Log.Error("Scp173BlinkTimer not found in Scp096Role::ctor");

            BlinkTimer = scp173BlinkTimer;

            if (!SubroutineModule.TryGetSubroutine(out Scp173TeleportAbility scp173TeleportAbility))
                Log.Error("Scp173TeleportAbility not found in Scp096Role::ctor");

            TeleportAbility = scp173TeleportAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp173TantrumAbility scp173TantrumAbility))
                Log.Error("Scp173TantrumAbility not found in Scp096Role::ctor");

            TantrumAbility = scp173TantrumAbility;
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-173.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp173;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp173MovementModule MovementModule { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173ObserversTracker"/>.
        /// </summary>
        public Scp173ObserversTracker ObserversTracker { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173BlinkTimer"/>.
        /// </summary>
        public Scp173BlinkTimer BlinkTimer { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173TeleportAbility"/>.
        /// </summary>
        public Scp173TeleportAbility TeleportAbility { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173TantrumAbility"/>.
        /// </summary>
        public Scp173TantrumAbility TantrumAbility { get; }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can use breakneck speed again.
        /// </summary>
        public float BreakneckCooldown { get; set; } = 40f; // It's hardcoded //TODO

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can place a tantrum.
        /// </summary>
        public float TantrumCooldown { get; set; } = 30f; // It's hardcoded //TODO

        /// <summary>
        /// Gets a value indicating whether or not SCP-173 is currently being viewed by one or more players.
        /// </summary>
        public bool IsObserved => ObserversTracker.IsObserved;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of players that are currently viewing SCP-173. Can be empty.
        /// </summary>
        public IEnumerable<Player> ObservingPlayers => ObserversTracker.Observers.Select(x => Player.Get(x));

        /// <summary>
        /// Gets SCP-173's max move speed.
        /// </summary>
        public float MaxMovementSpeed => MovementModule.MaxMovementSpeed;

        /// <summary>
        /// Gets the SCP-173's movement speed.
        /// </summary>
        public override float MovementSpeed => MovementModule.ServerSpeed;

        /// <summary>
        /// Gets or sets SCP-173's simulated stare. SCP-173 will be treated as though it is being looked at while this value is greater than <c>0</c>.
        /// </summary>
        public float SimulatedStare
        {
            get => ObserversTracker.SimulatedStare;
            set => ObserversTracker.SimulatedStare = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173 is able to blink.
        /// </summary>
        public bool BlinkReady
        {
            get => BlinkTimer.AbilityReady;
            set
            {
                BlinkTimer._endSustainTime = -1;
                BlinkTimer._totalCooldown = 0;
                BlinkTimer._initialStopTime = NetworkTime.time;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can blink.
        /// </summary>
        public float BlinkCooldown
        {
            get => BlinkTimer.RemainingBlinkCooldown;
            set
            {
                BlinkTimer._initialStopTime = NetworkTime.time;
                BlinkTimer._totalCooldown = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the max distance that SCP-173 can move in a blink. Factors in <see cref="BreakneckActive"/>.
        /// </summary>
        public float BlinkDistance => TeleportAbility.EffectiveBlinkDistance;

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173's breakneck speed is active.
        /// </summary>
        public bool BreakneckActive
        {
            get => TeleportAbility._breakneckSpeedsAbility.IsActive;
            set => TeleportAbility._breakneckSpeedsAbility.IsActive = value;
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) under the player.
        /// </summary>
        /// <param name="failIfObserved">Whether or not to place the tantrum if SCP-173 is currently being viewed.</param>
        /// <param name="cooldown">The cooldown until SCP-173 can place a tantrum again. Set to <c>0</c> to not affect the cooldown.</param>
        /// <returns>The tantrum's <see cref="GameObject"/>, or <see langword="null"/> if it cannot be placed.</returns>
        public GameObject Tantrum(bool failIfObserved = false, float cooldown = 0)
        {
            if (failIfObserved && IsObserved)
                return null;

            TantrumAbility.Cooldown.Trigger(cooldown);

            return Owner.PlaceTantrum();
        }
    }
}