// -----------------------------------------------------------------------
// <copyright file="AddingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before adding a target to SCP-096.
    /// </summary>
    public class AddingTargetEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AddingTargetEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public AddingTargetEventArgs(Player scp096, Player target, bool isAllowed = true)
        {
            Player = scp096;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the <see cref="Player" /> that is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the <see cref="Player" /> being added as a target.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the target is allowed to be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}