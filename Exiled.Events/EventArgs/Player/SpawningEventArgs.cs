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
    using Exiled.API.Extensions;
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
        /// <param name="RoleTypeId">
        ///     <inheritdoc cref="RoleTypeId" />
        /// </param>
        public SpawningEventArgs(Player player, RoleTypeId RoleTypeId)
        {
            Player = player;
            RoleTypeId = RoleTypeId;
            (Vector3 position, float rotation) = RoleTypeId.GetRandomSpawnProperties();
            if (position == Vector3.zero)
            {
                // Position = player.ReferenceHub.characterClassManager.DeathPosition;
                // RotationY = new PlayerMovementSync.PlayerRotation(0f, 0f);
            }
            else
            {
                // Position = position;
                // RotationY = new PlayerMovementSync.PlayerRotation(0f, rotation);
            }
        }

        /// <summary>
        ///     Gets the player role type.
        /// </summary>
        public RoleTypeId RoleTypeId { get; }

        /// <summary>
        ///     Gets or sets the player's spawning position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        ///     Gets or sets the rotation y axis of the player.
        /// </summary>
       //  public PlayerMovementSync.PlayerRotation RotationY { get; set; }

        /// <summary>
        ///     Gets the spawning player.
        /// </summary>
        public Player Player { get; }
    }
}