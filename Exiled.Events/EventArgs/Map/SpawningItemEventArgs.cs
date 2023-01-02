// -----------------------------------------------------------------------
// <copyright file="SpawningItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information before the server spawns an item.
    /// </summary>
    public class SpawningItemEventArgs : IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpawningItemEventArgs" /> class.
        /// </summary>
        /// <param name="pickupBase">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public SpawningItemEventArgs(ItemPickupBase pickupBase, bool isAllowed = true)
        {
            Pickup = Pickup.Get(pickupBase);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the item can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the pickup being spawned.
        /// </summary>
        public Pickup Pickup { get; set; }
    }
}