// -----------------------------------------------------------------------
// <copyright file="ChangingMoveStateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.FirstPersonControl;

    /// <summary>
    ///     Contains all information before changing movement state.
    /// </summary>
    public class ChangingMoveStateEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingMoveStateEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldState">
        ///     <inheritdoc cref="OldState" />
        /// </param>
        /// <param name="newState">
        ///     <inheritdoc cref="NewState" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingMoveStateEventArgs(Player player, PlayerMovementState oldState, PlayerMovementState newState, bool isAllowed = true)
        {
            Player = player;
            OldState = oldState;
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's changing the movement state.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the old state.
        /// </summary>
        public PlayerMovementState OldState { get; }

        /// <summary>
        ///     Gets or sets the new state.
        /// </summary>
        public PlayerMovementState NewState { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the player can change the movement state.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}