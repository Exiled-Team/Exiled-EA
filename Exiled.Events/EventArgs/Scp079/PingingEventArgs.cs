// -----------------------------------------------------------------------
// <copyright file="Scp079PingingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;
    using Exiled.API.Enums;
    using Interfaces;
    using UnityEngine;

    /// <summary>
    ///     Contains all information before SCP-079 gains a level.
    /// </summary>
    public class PingingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PingingEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="Position" />
        /// </param>
        /// <param name="proccesorindex">
        ///     <inheritdoc cref="Type" />
        /// </param>
        /// <param name="powerCost">
        ///     <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PingingEventArgs(ReferenceHub hub, Vector3 position, int powerCost, byte proccesorindex,  bool isAllowed = true)
        {
            Player = Player.Get(hub);
            Position = position;
            AuxiliaryPowerCost = powerCost;
            Type = (Scp079Ping)proccesorindex;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the event is allowed to continue.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets or sets the amount of auxiliary power required to trigger a door through SCP-079.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the position of the ping.
        /// </summary>
        public Scp079Ping Type { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the position of the ping.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        ///     Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }
    }
}