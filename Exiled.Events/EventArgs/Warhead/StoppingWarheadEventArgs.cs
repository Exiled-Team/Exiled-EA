// -----------------------------------------------------------------------
// <copyright file="StoppingWarheadEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Warhead
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before stopping the warhead.
    /// </summary>
    public class StoppingWarheadEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StoppingWarheadEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public StoppingWarheadEventArgs(Player player, bool isAllowed = true)
        {
            Player = player ?? Server.Host;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the warhead can be stopped.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's going to stop the warhead.
        /// </summary>
        public Player Player { get; }
    }
}