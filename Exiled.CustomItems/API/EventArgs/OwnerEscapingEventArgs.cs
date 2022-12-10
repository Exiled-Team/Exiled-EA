// -----------------------------------------------------------------------
// <copyright file="OwnerEscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using PlayerRoles;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Player"/> escapes.
    /// </summary>
    public class OwnerEscapingEventArgs : EscapingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerEscapingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public OwnerEscapingEventArgs(Item item, EscapingEventArgs ev)
            : this(item, ev.Player, ev.NewRole)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerEscapingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="player"><inheritdoc cref="EscapingEventArgs.Player"/></param>
        /// <param name="newRole"><inheritdoc cref="EscapingEventArgs.NewRole"/></param>
        public OwnerEscapingEventArgs(Item item, Player player, RoleTypeId newRole)
            : base(player, newRole)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}