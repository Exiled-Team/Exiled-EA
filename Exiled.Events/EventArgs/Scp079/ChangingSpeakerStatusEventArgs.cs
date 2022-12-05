// -----------------------------------------------------------------------
// <copyright file="ChangingSpeakerStatusEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-079 uses a speaker.
    /// </summary>
    public class ChangingSpeakerStatusEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingSpeakerStatusEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingSpeakerStatusEventArgs(Player player, bool isAllowed)
        {
            Player = player;
            Room = Room.Get(player.Role.As<Scp079Role>().Speaker.Room);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the room that the speaker is located in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether SCP-079 is speaking or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}