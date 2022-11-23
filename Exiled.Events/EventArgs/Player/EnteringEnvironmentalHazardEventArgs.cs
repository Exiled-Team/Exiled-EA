// -----------------------------------------------------------------------
// <copyright file="EnteringEnvironmentalHazardEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Hazards;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player enters in an environmental hazard.
    /// </summary>
    public class EnteringEnvironmentalHazardEventArgs : IPlayerEvent, IDeniableEvent, IHazardEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnteringEnvironmentalHazardEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="environmentalHazard"><inheritdoc cref="EnvironmentalHazard"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EnteringEnvironmentalHazardEventArgs(API.Features.Player player, EnvironmentalHazard environmentalHazard, bool isAllowed = true)
        {
            Player = player;
            EnvironmentalHazard = environmentalHazard;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's entering in the environmental hazard.
        /// </summary>
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the environmental hazard that the player is entering in.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player should be affected by the environmental hazard.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}