// -----------------------------------------------------------------------
// <copyright file="Scp049Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayableScps;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    using Scp049GameRole = PlayerRoles.PlayableScps.Scp049.Scp049Role;

    /// <summary>
    /// Defines a role that represents SCP-049.
    /// </summary>
    public class Scp049Role : ScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp049Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public Scp049Role(Player owner)
            : base(owner)
        {
            SubroutineModule = (Base as Scp049GameRole).SubroutineModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp049;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-049 is currently recalling a player.
        /// </summary>
        public bool IsRecalling => SubroutineModule.TryGetSubroutine(out Scp049ResurrectAbility ability) && ability.IsInProgress;

        /// <summary>
        /// Gets the player that is currently being revived by SCP-049. Will be <see langword="null"/> if <see cref="IsRecalling"/> is <see langword="false"/>.
        /// </summary>
        public Player RecallingPlayer
        {
            get
            {
                if (!IsRecalling || !SubroutineModule.TryGetSubroutine(out Scp049ResurrectAbility ability))
                    return null;

                return Player.Get(ability.CurRagdoll.Info.OwnerHub);
            }
        }

        /// <summary>
        /// Gets the ragdoll that is currently being revived by SCP-049. Will be <see langword="null"/> if <see cref="IsRecalling"/> is <see langword="false"/>.
        /// </summary>
        public Ragdoll RecallingRagdoll
        {
            get
            {
                if (!IsRecalling || !SubroutineModule.TryGetSubroutine(out Scp049ResurrectAbility ability))
                    return null;

                return Ragdoll.Get(ability.CurRagdoll);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-049 can use its Doctor's Call ability again.
        /// </summary>
        public float CallCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp049CallAbility ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp049CallAbility ability))
                {
                    ability.Cooldown.Remaining = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-049 can use its Good Sense ability again.
        /// </summary>
        public float AmnesticCloudCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp049SenseAbility ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp049SenseAbility ability))
                {
                    ability.Cooldown.Remaining = value;
                }
            }
        }

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049 is close enough to a ragdoll to revive it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049 is close enough to the body to revive it; the body may have expired. Make sure to check <see cref="Ragdoll.AllowRecall"/> to ensure the body can be revived.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to revive the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInRecallRange(BasicRagdoll ragdoll) => SubroutineModule.TryGetSubroutine(out Scp049ResurrectAbility ability) ? ability.IsCloseEnough(Owner.Position, ragdoll.transform.position) : false;

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049 is close enough to a ragdoll to revive it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049 is close enough to the body to revive it; the body may have expired. Make sure to check <see cref="Ragdoll.AllowRecall"/> to ensure the body can be revived.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to revive the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInRecallRange(Ragdoll ragdoll) => IsInRecallRange(ragdoll.Base);
    }
}