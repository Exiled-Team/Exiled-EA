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

            (Vector3 position, Vector3 rotation) = roleType.GetRandomSpawnProperties();

            if (position == Vector3.zero && player.Role.Type == RoleTypeId.Spectator)
            {
                Position = player.Role.As<SpectatorRole>().DeathPosition.Position;
                Rotation = Vector3.zero;
            }
            else
            {
                Position = position;
                Rotation = rotation;
            }
        }

        /// <summary>
        ///     Gets the <see cref="Player"/>'s role type.
        /// </summary>
        public RoleTypeId RoleType { get; }

        /// <summary>
        ///     Gets or sets the <see cref="Player"/>'s spawning position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Player"/>'s rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        ///     Gets the spawning <see cref="Player"/>.
        /// </summary>
        public Player Player { get; }
    }
}