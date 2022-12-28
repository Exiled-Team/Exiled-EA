// -----------------------------------------------------------------------
// <copyright file="AmmoPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using BaseAmmo = InventorySystem.Items.Firearms.Ammo.AmmoPickup;

    /// <summary>
    /// A wrapper class for Ammo Pickup.
    /// </summary>
    public class AmmoPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmmoPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseAmmo"/> class.</param>
        internal AmmoPickup(BaseAmmo pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmmoPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal AmmoPickup(ItemType type)
            : base(type)
        {
            Base = (BaseAmmo)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseAmmo"/> that this class is encapsulating.
        /// </summary>
        public new BaseAmmo Base { get; }

        /// <summary>
        /// Gets the max ammo.
        /// </summary>
        public int MaxDisplayedAmmo => Base._maxDisplayedValue;

        /// <summary>
        /// Gets the <see cref="Enums.AmmoType"/> of the item.
        /// </summary>
        public AmmoType AmmoType => Type.GetAmmoType();

        /// <summary>
        /// Gets or Sets the number of ammo.
        /// </summary>
        public ushort Ammo
        {
            get => Base.SavedAmmo;
            set => Base.NetworkSavedAmmo = value;
        }

        /// <summary>
        /// Returns the AmmoPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing AmmoPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{MaxDisplayedAmmo}| -{Ammo}-";
    }
}
