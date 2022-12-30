// -----------------------------------------------------------------------
// <copyright file="OverwatchRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;

    using OverwatchGameRole = PlayerRoles.Spectating.OverwatchRole;

    /// <summary>
    /// Defines a role that represents a player with overwatch enabled.
    /// </summary>
    public class OverwatchRole : SpectatorRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverwatchRole"/> class.
        /// </summary>
        /// <param name="baseRole">The encapsulated <see cref="OverwatchGameRole"/>.</param>
        internal OverwatchRole(OverwatchGameRole baseRole)
            : base(baseRole)
        {
        }

        /// <inheritdoc/>
        public override RoleTypeId Type => RoleTypeId.Overwatch;
    }
}
