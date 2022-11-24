// -----------------------------------------------------------------------
// <copyright file="ScpRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles.PlayableScps.Subroutines;

    /// <summary>
    /// Defines a role that represents an SCP class.
    /// </summary>
    public abstract class ScpRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScpRole"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        protected ScpRole(Player owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the SCP <see cref="SubroutineManagerModule"/>.
        /// </summary>
        public abstract SubroutineManagerModule SubroutineModule { get; }
    }
}