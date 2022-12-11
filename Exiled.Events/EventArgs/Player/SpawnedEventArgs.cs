// -----------------------------------------------------------------------
// <copyright file="SpawnedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information after spawning a player.
    /// </summary>
    public class SpawnedEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawnedEventArgs" /> class.
        /// </summary>
        /// <param name="player"><see cref="Player" /> of the spawned player.</param>
        public SpawnedEventArgs(Player player) => Player = player;

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> who spawned.
        /// </summary>
        public Player Player { get; }
    }
}