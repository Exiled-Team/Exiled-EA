// -----------------------------------------------------------------------
// <copyright file="ChangingFocusEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-939 changes its target focus.
    /// </summary>
    public class ChangingFocusEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingFocusEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingFocusEventArgs(Player player, bool state, bool isAllowed = true)
        {
            Player = player;
            State = state;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-939 can focus.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets whether or not SCP-939 is currently focusing or un-focusing.
        /// </summary>
        public bool State { get; }

        /// <summary>
        ///     Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }
    }
}