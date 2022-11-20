// -----------------------------------------------------------------------
// <copyright file="HumanRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;

    /// <summary>
    /// Defines a role that represents a human class.
    /// </summary>
    public class HumanRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanRole"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        /// <param name="type">The RoleTypeId.</param>
        internal HumanRole(Player player, RoleTypeId type)
        {
            Owner = player;
            RoleTypeId = type;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        internal override RoleTypeId RoleTypeId { get; }
    }
}