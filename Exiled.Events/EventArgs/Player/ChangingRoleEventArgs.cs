// -----------------------------------------------------------------------
// <copyright file="ChangingRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Collections.Generic;

    using API.Enums;
    using API.Features;
    using Interfaces;

    using InventorySystem.Configs;
    using PlayerRoles;

    /// <summary>
    ///     Contains all information before a player's <see cref="RoleTypeId" /> changes.
    /// </summary>
    public class ChangingRoleEventArgs : IPlayerEvent, IDeniableEvent
    {
        private RoleTypeId newRole;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingRoleEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newRole">
        ///     <inheritdoc cref="NewRole" />
        /// </param>
        /// <param name="reason">
        ///     <inheritdoc cref="Reason" />
        /// </param>
        public ChangingRoleEventArgs(Player player, RoleTypeId newRole, RoleChangeReason reason)
        {
            Player = player;
            NewRole = newRole;
            Reason = (SpawnReason)reason;
        }

        /// <summary>
        ///     Gets the player whose <see cref="RoleTypeId" /> is changing.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the new player's role.
        /// </summary>
        public RoleTypeId NewRole
        {
            get => newRole;
            set
            {
                if (StartingInventories.DefinedInventories.ContainsKey(value))
                {
                    Items.Clear();
                    Ammo.Clear();

                    foreach (ItemType itemType in StartingInventories.DefinedInventories[value].Items)
                        Items.Add(itemType);

                    foreach (KeyValuePair<ItemType, ushort> ammoPair in StartingInventories.DefinedInventories[value].Ammo)
                        Ammo.Add(ammoPair.Key, ammoPair.Value);
                }

                newRole = value;
            }
        }

        /// <summary>
        ///     Gets base items that the player will receive.
        /// </summary>
        public List<ItemType> Items { get; } = new();

        /// <summary>
        ///     Gets the base ammo values for the new role.
        /// </summary>
        public Dictionary<ItemType, ushort> Ammo { get; } = new();

        /// <summary>
        ///     Gets or sets a value indicating whether the inventory will be preserved or not.
        /// </summary>
        public bool ShouldPreserveInventory { get; set; } = false;

        /// <summary>
        ///     Gets or sets the reason for their class change.
        /// </summary>
        public SpawnReason Reason { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the event can continue.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}