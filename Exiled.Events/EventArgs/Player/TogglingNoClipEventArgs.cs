// -----------------------------------------------------------------------
// <copyright file="TogglingNoClipEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before a player toggles the NoClip mode.
    /// </summary>
    public class TogglingNoClipEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TogglingNoClipEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newValue">
        ///     <inheritdoc cref="IsEnabled" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public TogglingNoClipEventArgs(Player player, bool newValue, bool isAllowed = true)
        {
            Player = player;
            IsEnabled = newValue;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's toggling the NoClip mode.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the NoClip mode will be enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can toggle the NoClip mode.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}