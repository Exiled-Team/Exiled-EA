// -----------------------------------------------------------------------
// <copyright file="MicroHid.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items.MicroHID;

    using MEC;

    /// <summary>
    /// A wrapper class for <see cref="MicroHIDItem"/>.
    /// </summary>
    public class MicroHid : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="MicroHIDItem"/> class.</param>
        public MicroHid(MicroHIDItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class, as well as a new Micro HID item.
        /// </summary>
        internal MicroHid()
            : this((MicroHIDItem)Server.Host.Inventory.CreateItemInstance(ItemType.MicroHID, false))
        {
        }

        /// <summary>
        /// Gets or sets the remaining energy in the MicroHID.
        /// </summary>
        public float Energy
        {
            get => Base.RemainingEnergy;
            set => Base.RemainingEnergy = value;
        }

        /// <summary>
        /// Gets the <see cref="MicroHIDItem"/> base of the item.
        /// </summary>
        public new MicroHIDItem Base { get; }

        /// <summary>
        /// Gets or sets the <see cref="HidState"/>.
        /// </summary>
        public HidState State
        {
            get => Base.State;
            set => Base.State = value;
        }

        /// <summary>
        /// Starts firing the MicroHID.
        /// </summary>
        public void Fire()
        {
            Base.UserInput = HidUserInput.Fire;
            State = HidState.Firing;
        }

        /// <summary>
        /// Returns the MicroHid in a human readable format.
        /// </summary>
        /// <returns>A string containing MicroHid-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}| -{State}-";

        /// <summary>
        /// Clones current <see cref="MicroHid"/> object.
        /// </summary>
        /// <returns> New <see cref="MicroHid"/> object. </returns>
        public override Item Clone()
        {
            MicroHid cloneableItem = new();

            Timing.CallDelayed(
                1f,
                () =>
                {
                    cloneableItem.State = State;
                    cloneableItem.Energy = Energy;
                });

            return cloneableItem;
        }
    }
}