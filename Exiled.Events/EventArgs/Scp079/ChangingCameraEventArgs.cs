// -----------------------------------------------------------------------
// <copyright file="ChangingCameraEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
/*
namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a SCP-079 changes the current camera.
    /// </summary>
    public class ChangingCameraEventArgs : IPlayerEvent, ICameraEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingCameraEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="camera">
        ///     <inheritdoc cref="Camera" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        ///     <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingCameraEventArgs(Player player, Camera079 camera, float auxiliaryPowerCost, bool isAllowed = true)
        {
            Player = player;
            Camera = Camera.Get(camera);
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the amount of auxiliary power that will be required to switch cameras.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        ///     Gets or sets the camera SCP-079 will be moved to.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-079 can switch cameras.
        ///     <para>Defaults to a value describing whether or not SCP-079 has enough auxiliary power to switch.</para>
        ///     <br>Can be set to <see langword="true" /> to allow a switch regardless of SCP-079's auxiliary power amount.</br>
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who is SCP-079.
        /// </summary>
        public Player Player { get; }
    }
}
*/