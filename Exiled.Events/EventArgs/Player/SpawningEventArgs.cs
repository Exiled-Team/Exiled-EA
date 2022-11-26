// -----------------------------------------------------------------------
// <copyright file="SpawningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using CustomPlayerEffects;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Roles;
    using Interfaces;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    ///     Contains all information before spawning a player.
    /// </summary>
    public class SpawningEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawningEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="roleType">
        ///     <inheritdoc cref="RoleType" />
        /// </param>
        public SpawningEventArgs(Player player, RoleTypeId roleType)
        {
            Player = player;
            RoleType = roleType;

            (Vector3 position, float rotation) = roleType.GetRandomSpawnProperties();

            if (position == Vector3.zero && player.Role.Type == RoleTypeId.Spectator)
            {
                Position = (player.Role as SpectatorRole).DeathPosition.Position;

                // RotationY = new PlayerMovementSync.PlayerRotation(0f, 0f);
            }
            else
            {
                Position = position;

                // RotationY = new PlayerMovementSync.PlayerRotation(0f, rotation);
            }
        }

        /// <summary>
        ///     Gets the player role type.
        /// </summary>
        public RoleTypeId RoleType { get; }

        /// <summary>
        ///     Gets or sets the player's spawning position.
        /// </summary>
        public Vector3 Position { get; set; }

        /*/// <summary>
        ///     Gets or sets the rotation y axis of the player.
        /// </summary>
        public PlayerMovementSync.PlayerRotation RotationY { get; set; }*/

        /// <summary>
        ///     Gets the spawning player.
        /// </summary>
        public Player Player { get; }
    }
}