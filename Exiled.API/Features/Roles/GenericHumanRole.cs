// -----------------------------------------------------------------------
// <copyright file="GenericHumanRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;

    /// <summary>
    /// Defines a role that represents a generic human class.
    /// </summary>
    public class GenericHumanRole : HumanRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericHumanRole"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        /// <param name="type">The <see cref="Role.Type"/>.</param>
        public GenericHumanRole(Player owner, RoleTypeId type)
            : base(owner) => Type = type;

        /// <inheritdoc/>
        public override RoleTypeId Type { get; }
    }
}