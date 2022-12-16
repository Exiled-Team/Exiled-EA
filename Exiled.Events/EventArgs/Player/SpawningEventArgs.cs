// -----------------------------------------------------------------------
// <copyright file="SpawningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
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
        /// <param name="role">
        ///     <inheritdoc cref="Role" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="Position" />
        /// </param>
        public SpawningEventArgs(Player player, PlayerRoleBase role, Vector3 position)
        {
            Player = player;
            Role = role;
            Position = position;
        }

        /// <summary>
        ///     Gets the spawning <see cref="Player"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the <see cref="Player"/>'s role type.
        /// </summary>
        public PlayerRoleBase Role { get; }

        /// <summary>
        ///     Gets or sets the <see cref="Player"/>'s spawning position.
        /// </summary>
        public Vector3 Position { get; set; }
    }
}