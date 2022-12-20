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
    using PlayerRoles.PlayableScps.Scp939;
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
            SubroutineModule = (Base as Scp939GameRole).SubroutineModule;
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
            get => SubroutineModule.TryGetSubroutine(out Scp939ClawAbility ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp939ClawAbility ability))
                {
                    ability.Cooldown.Remaining = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 is currently using its focus ability.
        /// </summary>
        public bool IsFocused => SubroutineModule.TryGetSubroutine(out Scp939FocusAbility focus) ? focus.TargetState : false;

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 is currently lunging.
        /// </summary>
        public bool IsLunging => SubroutineModule.TryGetSubroutine(out Scp939LungeAbility ability) ? ability.State != Scp939LungeState.None : false;

        /// <summary>
        /// Gets or sets SCP-939's <see cref="Scp939LungeState"/>.
        /// </summary>
        public Scp939LungeState State
        {
            get => SubroutineModule.TryGetSubroutine(out Scp939LungeAbility ability) ? ability.State : Scp939LungeState.None;
            set
            {
                if (!SubroutineModule.TryGetSubroutine(out Scp939LungeAbility ability))
                    return;

                ability.State = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can use its amnestic cloud ability again.
        /// </summary>
        public float AmnesticCloudCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability))
                {
                    ability.Cooldown.Remaining = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets a list of players this SCP-939 instance can see regardless of their movement.
        /// </summary>
        public List<Player> VisiblePlayers { get; } = new();
    }
}