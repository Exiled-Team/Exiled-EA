// -----------------------------------------------------------------------
// <copyright file="SpectatorRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;

    using PlayerRoles;
    using PlayerRoles.Spectating;
    using RelativePositioning;

    /// <summary>
    /// Defines a role that represents a spectator.
    /// </summary>
    public class SpectatorRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpectatorRole"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public SpectatorRole(Player owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type => Base is OverwatchRole ? RoleTypeId.Overwatch : RoleTypeId.Spectator;

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the player died.
        /// </summary>
        public DateTime DeathTime => Round.StartedTime + ActiveTime;

        /// <summary>
        /// Gets a value indicating whether or not this role represents a player on overwatch.
        /// </summary>
        public bool IsOverwatch => Type is RoleTypeId.Overwatch;

        /// <summary>
        /// Gets the total amount of time the player has been dead.
        /// </summary>
        public TimeSpan DeadTime => DateTime.UtcNow - DeathTime;

        /// <summary>
        /// Gets the <see cref="Player"/>'s death position.
        /// </summary>
        public RelativePosition DeathPosition => InternalSpectatorRole.DeathPosition;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is ready to respawn or not.
        /// </summary>
        public bool IsReadyToRespawn => InternalSpectatorRole.ReadyToRespawn;

        /// <summary>
        /// Gets currently spectated <see cref="Player"/> by this <see cref="Player"/>. May be <see langword="null"/>.
        /// </summary>
        public Player SpectatedPlayer
        {
            get
            {
                Player spectatedPlayer = Player.Get(InternalSpectatorRole.SyncedSpectatedNetId);

                return spectatedPlayer != Owner ? spectatedPlayer : null;
            }
        }

        /// <summary>
        /// Gets the game <see cref="PlayerRoles.HumanRole"/>.
        /// </summary>
        private PlayerRoles.Spectating.SpectatorRole InternalSpectatorRole { get; }
    }
}