// -----------------------------------------------------------------------
// <copyright file="FinishingRecallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mirror;

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-049 finishes recalling a player.
    /// </summary>
    public class ZombieConsumeEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="FinishingRecallEventArgs" /> class.
        /// </summary>
        /// <param name="scp049"> <inheritdoc cref="Player" /> </param>
        /// <param name="target"> <inheritdoc cref="Target" /> </param>
        /// <param name="reader"> <inheritdoc cref="Reader" /></param>
        /// <param name="bypassChecks"> <inheritdoc cref="BypassChecks" /></param>
        /// <param name="isAllowed"> <inheritdoc cref="IsAllowed" /></param>
        public ZombieConsumeEventArgs(Player scp049, Player target, HashSet<BasicRagdoll> consumedRagdolls, bool isAllowed = true)
        {
            Player = scp049;
            Target = target;
            ConsomedRagdolls = consumedRagdolls;
            IsAllowed = isAllowed;
            AllowNonHumans = false;
        }

        /// <summary>
        /// Allow consumption of non-humans
        /// </summary>
        public bool AllowNonHumans { get; set; }

        /// <summary>
        /// Scp0492 ragdolls consumed thus far.
        /// </summary>
        public HashSet<BasicRagdoll> ConsomedRagdolls { get; set; }

        /// <summary>
        ///     Gets the player who is currently being targeted.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the server will send 049 information on the recall.
        /// </summary>
        public bool IsAllowed { get; set; }

    }
}