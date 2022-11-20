// -----------------------------------------------------------------------
// <copyright file="ExitingEnvironmentalHazardEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Interfaces;
    using Hazards;

    /// <summary>
    /// Contains all information before a player exits an environmental hazard.
    /// </summary>
    public class ExitingEnvironmentalHazardEventArgs : IPlayerEvent, IDeniableEvent, IHazardEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitingEnvironmentalHazardEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="environmentalHazard"><inheritdoc cref="EnvironmentalHazard"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ExitingEnvironmentalHazardEventArgs(API.Features.Player player, EnvironmentalHazard environmentalHazard, bool isAllowed = true)
        {
            Player = player;
            EnvironmentalHazard = environmentalHazard;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's exiting from the environmental hazard.
        /// </summary>
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the environmental hazard that the player is exiting from.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player should be affected by the environmental hazard.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}