// -----------------------------------------------------------------------
// <copyright file="PreAuthenticatingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using Interfaces;

    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    ///     Contains all information before pre-authenticating a player.
    /// </summary>
    public class PreAuthenticatingEventArgs : IExiledEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PreAuthenticatingEventArgs" /> class.
        /// </summary>
        /// <param name="userId">
        ///     <inheritdoc cref="UserId" />
        /// </param>
        /// <param name="request">
        ///     <inheritdoc cref="Request" />
        /// </param>
        /// <param name="readerStartPosition">
        ///     <inheritdoc cref="ReaderStartPosition" />
        /// </param>
        /// <param name="flags">
        ///     <inheritdoc cref="Flags" />
        /// </param>
        /// <param name="country">
        ///     <inheritdoc cref="Country" />
        /// </param>
        public PreAuthenticatingEventArgs(string userId, ConnectionRequest request, int readerStartPosition, CentralAuthPreauthFlags flags, string country)
        {
            UserId = userId;
            Request = request;
            ReaderStartPosition = readerStartPosition;
            Flags = flags;
            Country = country;
        }

        /// <summary>
        ///     Gets the player's user id.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        ///     Gets the reader starting position for reading the preauth.
        /// </summary>
        public int ReaderStartPosition { get; }

        /// <summary>
        ///     Gets the flags.
        /// </summary>
        public CentralAuthPreauthFlags Flags { get; }

        /// <summary>
        ///     Gets the player's country.
        /// </summary>
        public string Country { get; }

        /// <summary>
        ///     Gets the connection request.
        /// </summary>
        public ConnectionRequest Request { get; }

        /// <summary>
        ///     Gets a value indicating whether the player can be authenticated or not.
        /// </summary>
        public bool IsAllowed { get; private set; } = true;

        /// <summary>
        ///     Delays the connection.
        /// </summary>
        /// <param name="seconds">The delay in seconds.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Delay(byte seconds, bool isForced)
        {
            if (seconds is < 1 or > 25)
                throw new ArgumentOutOfRangeException(nameof(seconds), "Delay duration must be between 1 and 25 seconds.");

            Reject(RejectionReason.Delay, isForced, null, 0, seconds);
        }

        /// <summary>
        ///     Rejects the player and redirects them to another server port.
        /// </summary>
        /// <param name="port">The new server port.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Redirect(ushort port, bool isForced) => Reject(RejectionReason.Redirect, isForced, null, 0, 0, port);

        /// <summary>
        ///     Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="banReason">The ban reason.</param>
        /// <param name="expiration">The ban expiration time.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void RejectBanned(string banReason, DateTime expiration, bool isForced) => Reject(RejectionReason.Banned, isForced, banReason, expiration.Ticks);

        /// <summary>
        ///     Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="banReason">The ban reason.</param>
        /// <param name="expiration">The ban expiration time in .NET Ticks.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void RejectBanned(string banReason, long expiration, bool isForced) => Reject(RejectionReason.Banned, isForced, banReason, expiration);

        /// <summary>
        ///     Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="writer">The <see cref="NetDataWriter" /> instance.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Reject(NetDataWriter writer, bool isForced)
        {
            if (!IsAllowed)
                return;

            IsAllowed = false;

            if (isForced)
                Request.RejectForce(writer);
            else
                Request.Reject(writer);
        }

        /// <summary>
        ///     Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="rejectionReason">The custom rejection reason.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        public void Reject(string rejectionReason, bool isForced) => Reject(RejectionReason.Custom, isForced, rejectionReason);

        /// <summary>
        ///     Rejects a player who's trying to authenticate.
        /// </summary>
        /// <param name="rejectionReason">The rejection reason.</param>
        /// <param name="isForced">Indicates whether the player has to be rejected forcefully or not.</param>
        /// <param name="customReason">The custom rejection reason (Banned and Custom reasons only).</param>
        /// <param name="expiration">The ban expiration ticks (Banned reason only).</param>
        /// <param name="seconds">The delay in seconds (Delay reason only).</param>
        /// <param name="port">The redirection port (Redirect reason only).</param>
        public void Reject(RejectionReason rejectionReason, bool isForced, string customReason = null, long expiration = 0, byte seconds = 0, ushort port = 0)
        {
            if (customReason is not null && (customReason.Length > 400))
                throw new ArgumentOutOfRangeException(nameof(rejectionReason), "Reason can't be longer than 400 characters.");

            if (!IsAllowed)
                return;

            IsAllowed = false;

            NetDataWriter rejectData = new();

            rejectData.Put((byte)rejectionReason);

            switch (rejectionReason)
            {
                case RejectionReason.Banned:
                    rejectData.Put(expiration);
                    rejectData.Put(customReason);
                    break;

                case RejectionReason.Custom:
                    rejectData.Put(customReason);
                    break;

                case RejectionReason.Delay:
                    rejectData.Put(seconds);
                    break;

                case RejectionReason.Redirect:
                    rejectData.Put(port);
                    break;
            }

            if (isForced)
                Request.RejectForce(rejectData);
            else
                Request.Reject(rejectData);
        }

        /// <summary>
        ///     Disallows the connection without sending any reason. Should only be used when the connection has already been
        ///     terminated by the plugin itself.
        /// </summary>
        public void Disallow() => IsAllowed = false;
    }
}