// -----------------------------------------------------------------------
// <copyright file="ChangingAmmoEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before changing firearm ammo.
    /// </summary>
    public class ChangingAmmoEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAmmoEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="firearm"><inheritdoc cref="Firearm"/></param>
        /// <param name="oldAmmo"><inheritdoc cref="OldAmmo"/></param>
        /// <param name="newAmmo"><inheritdoc cref="NewAmmo"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingAmmoEventArgs(Player player, InventorySystem.Items.ItemBase firearm, byte oldAmmo, byte newAmmo, bool isAllowed = true)
        {
            if (firearm is not InventorySystem.Items.Firearms.Firearm firearmBase)
                return;

            Item item = Item.Get(firearmBase);
            if (item is not Firearm firearmItem)
                return;

            Player = player;
            Firearm = firearmItem;
            OldAmmo = oldAmmo;
            NewAmmo = newAmmo;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing the <see cref="Firearm"/>'s ammo.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> the ammo is being changed to.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the old ammo.
        /// </summary>
        public byte OldAmmo { get; }

        /// <summary>
        /// Gets or sets the new ammo to be used by the firearm.
        /// </summary>
        public byte NewAmmo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ammo can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
