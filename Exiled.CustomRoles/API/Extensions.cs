// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> of the player's current custom roles.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for roles.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all current custom roles.</returns>
        public static ReadOnlyCollection<CustomRole> GetCustomRoles(this Player player)
        {
            List<CustomRole> roles = new();

            foreach (CustomRole customRole in CustomRole.Registered)
            {
                if (customRole.Check(player))
                    roles.Add(customRole);
            }

            return roles.AsReadOnly();
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="customRoles"><see cref="CustomRole"/>s to be registered.</param>
        public static void Register(this IEnumerable<CustomRole> customRoles)
        {
            if (customRoles is null)
                throw new ArgumentNullException(nameof(customRoles));

            foreach (CustomRole customItem in customRoles)
                customItem.TryRegister();
        }

        /// <summary>
        /// Registers a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomRole"/> to be registered.</param>
        public static void Register(this CustomRole role) => role.TryRegister();

        /// <summary>
        /// Registers a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be registered.</param>
        public static void Register(this CustomAbility ability) => ability.TryRegister();

        /// <summary>
        /// Unregisters an <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="customRoles"><see cref="CustomRole"/>s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomRole> customRoles)
        {
            if (customRoles is null)
                throw new ArgumentNullException(nameof(customRoles));

            foreach (CustomRole customItem in customRoles)
                customItem.TryUnregister();
        }

        /// <summary>
        /// Unregisters a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="role"><see cref="CustomRole"/> to be unregistered.</param>
        public static void Unregister(this CustomRole role) => role.TryUnregister();

        /// <summary>
        /// Unregisters a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="ability">The <see cref="CustomAbility"/> to be unregistered.</param>
        public static void Unregister(this CustomAbility ability) => ability.TryUnregister();
    }
}