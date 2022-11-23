// -----------------------------------------------------------------------
// <copyright file="StartPryingGateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;
    using Interactables.Interobjects;
    using Interfaces;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    ///     Contains all information before SCP-096 begins prying a gate open.
    /// </summary>
    public class StartPryingGateEventArgs : IPlayerEvent, IDeniableEvent, IDoorEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StartPryingGateEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="scp096">
        ///     <inheritdoc cref="Scp096" />
        /// </param>
        /// <param name="gate">
        ///     <inheritdoc cref="Door" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public StartPryingGateEventArgs(Scp096 scp096, Player player, PryableDoor gate, bool isAllowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Door = Door.Get(gate);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the SCP-096 instance.
        /// </summary>
        public Scp096 Scp096 { get; }

        /// <summary>
        ///     Gets or Sets a value indicating whether or not the gate can be pried open by SCP-096.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the <see cref="PryableDoor" /> to be pried open.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        ///     Gets the player that is controlling SCP-096.
        /// </summary>
        public Player Player { get; }
    }
}