// -----------------------------------------------------------------------
// <copyright file="KeycardInteractingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using API.Features;
    using API.Features.Items;
    using Interfaces;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items.Keycards;

    /// <summary>
    /// Contains all information before a keycard interacts with a door.
    /// </summary>
    public class KeycardInteractingEventArgs : IPlayerEvent, IDeniableEvent, IDoorEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardInteractingEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public KeycardInteractingEventArgs(KeycardPickup pickup, Player player, DoorVariant door, bool isAllowed = true)
        {
            Pickup = Pickup.Get(pickup);
            Player = player;
            Door = Door.Get(door);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the pickup that's interacting with the door.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets the player who's threw the keycard.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Door"/> instance.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the keycard can interact with the door.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}