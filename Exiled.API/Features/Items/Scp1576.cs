// -----------------------------------------------------------------------
// <copyright file="Scp1576.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp1576;

    /// <summary>
    /// A wrapper class for <see cref="Scp1576Item"/>.
    /// </summary>
    public class Scp1576 : Usable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1576"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp1576Item"/> class.</param>
        public Scp1576(Scp1576Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1576"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the usable item.</param>
        internal Scp1576(ItemType type)
            : this((Scp1576Item)Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <summary>
        /// Gets the <see cref="UsableItem"/> that this class is encapsulating.
        /// </summary>
        public new Scp1576Item Base { get; }

        /// <summary>
        /// Gets Scp1576Playback.
        /// </summary>
        public Scp1576Playback PlaybackTemplate => Base.PlaybackTemplate;

        /// <summary>
        /// .
        /// </summary>
        public void StopTransmitting() => Base.ServerStopTransmitting();
    }
}
