// -----------------------------------------------------------------------
// <copyright file="Scp939Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Subroutines;

    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;

    /// <summary>
    /// Defines a role that represents SCP-939.
    /// </summary>
    public class Scp939Role : ScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp939Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public Scp939Role(Player owner)
            : base(owner)
        {
            SubroutineModule = (Owner.RoleManager.CurrentRole as Scp939GameRole).SubroutineModule;
        }

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp939;

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => 0.6f; // It's hardcoded
            set { }
        }

        /// <summary>
        /// Gets a list of players this SCP-939 instance can see regardless of their movement.
        /// </summary>
        public List<Player> VisiblePlayers { get; } = new();
    }
}