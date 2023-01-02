// -----------------------------------------------------------------------
// <copyright file="Scp173Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

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
        /// Gets a value indicating whether or not SCP-173 is currently being viewed by one or more players.
        /// </summary>
        public bool IsObserved => SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker ability) && ability.IsObserved;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of players that are currently viewing SCP-173. Can be empty.
        /// </summary>
        public IReadOnlyCollection<Player> ObservingPlayers
        {
            get
            {
                HashSet<Player> players = new();

                if (SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker ability))
                {
                    foreach (ReferenceHub player in ability.Observers)
                        players.Add(Player.Get(player));
                }

                return players;
            }
        }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp173MovementModule MovementModule { get; }

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
            get => SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker ability) ? ability.SimulatedStare : 0;
            set
            {
                if (!SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker ability))
                    return;

                ability.SimulatedStare = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173 is able to blink.
        /// </summary>
        public bool BlinkReady
        {
            get => SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability) && ability.AbilityReady;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability))
                {
                    ability._endSustainTime = -1;
                    ability._totalCooldown = 0;
                    ability._initialStopTime = NetworkTime.time;
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can blink.
        /// </summary>
        public float BlinkCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability) ? ability.RemainingBlinkCooldown : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability))
                {
                    ability._initialStopTime = NetworkTime.time;
                    ability._totalCooldown = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the max distance that SCP-173 can move in a blink. Factors in <see cref="BreakneckActive"/>.
        /// </summary>
        public float BlinkDistance => SubroutineModule.TryGetSubroutine(out Scp173TeleportAbility ability) ? ability.EffectiveBlinkDistance : 0;

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173's breakneck speed is active.
        /// </summary>
        public bool BreakneckActive
        {
            get => SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability) && ability._breakneckSpeedsAbility.IsActive;
            set
            {
                if (!SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer ability))
                    return;

                ability._breakneckSpeedsAbility.IsActive = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can use breakneck speed again.
        /// </summary>
        public float BreakneckCooldown
        {
            get => 40; // It's hardcoded
            set { }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can place a tantrum.
        /// </summary>
        public float TantrumCooldown
        {
            get => 30; // It's hardcoded
            set { }
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

            if (cooldown > 0 && SubroutineModule.TryGetSubroutine(out Scp173TantrumAbility ability))
                ability.Cooldown.Trigger(cooldown);

            return Owner.PlaceTantrum();
        }
    }
}