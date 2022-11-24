// -----------------------------------------------------------------------
// <copyright file="Scp106Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;

    /// <summary>
    /// Defines a role that represents SCP-106.
    /// </summary>
    public class Scp106Role : ScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp106Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        internal Scp106Role(Player owner)
            : base(owner)
        {
            Internal = Owner.RoleManager.CurrentRole as Scp106GameRole;
            SubroutineModule = Internal.SubroutineModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp106;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets the <see cref="Scp106GameRole"/>.
        /// </summary>
        protected Scp106GameRole Internal { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of an object.
        /// </summary>
        //public bool IsInsideObject =>;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently submerged.
        /// </summary>
        public bool IsSubmerged => Internal.IsSubmerged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of a door.
        /// </summary>
        // public bool IsInsideDoor => Script.DoorCurrentlyIn is not null;

        /// <summary>
        /// Gets the door that SCP-106 is currently inside of.
        /// </summary>
        // public Door InsideDoor => Door.Get(Script.DoorCurrentlyIn);

        /// <summary>
        /// Gets or sets the location of SCP-106's portal.
        /// </summary>
        /// <remarks>
        /// Note: Every alive SCP-106 uses the same portal.
        /// </remarks>
        public Vector3 PortalPosition
        {
            get => Internal.Sinkhole.transform.position;
            set => Internal.Sinkhole.transform.position = value;
        }

        /// <summary>
        /// Gets or sets the amount of time in between player captures.
        /// </summary>
        public float CaptureCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp106Attack ability) ? ability._hitCooldown : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp106Attack ability))
                    ability._hitCooldown = value;
            }
        }

        /// <summary>
        /// Forces SCP-106 to use its portal, if one is placed.
        /// </summary>
        public void UsePortal()
        {
            if (SubroutineModule.TryGetSubroutine(out Scp106HuntersAtlasAbility ability))
                ability.SetSubmerged(true);
        }

        /// <summary>
        /// Contains SCP-106.
        /// </summary>
        /// <param name="container">The player who recontained SCP-106.</param>
        /// <exception cref="System.ArgumentException">Container cannot be <see langword="null"/>.</exception>
        public void Contain(Player container)
        {
            /*if (container is null)
                throw new System.ArgumentException("Container cannot be null.", nameof(container));

            Script.Contain(container.Footprint);*/
        }
    }
}