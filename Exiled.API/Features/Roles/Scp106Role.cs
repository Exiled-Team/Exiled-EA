// -----------------------------------------------------------------------
// <copyright file="Scp106Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;

    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;

    /// <summary>
    /// Defines a role that represents SCP-106.
    /// </summary>
    public class Scp106Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp106Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp106GameRole"/>.</param>
        internal Scp106Role(Scp106GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
            Internal = baseRole;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp106;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets the <see cref="HumeShieldModuleBase"/>.
        /// </summary>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets or sets SCP-106's Vigor.
        /// </summary>
        public float Vigor
        {
            get => SubroutineModule.TryGetSubroutine(out Scp106Vigor ability) ? ability.VigorAmount : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp106Vigor ability))
                    ability.VigorAmount = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of an object.
        /// </summary>
        public bool IsInsideObject => false; // TODO

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently submerged.
        /// </summary>
        public bool IsSubmerged => Internal.IsSubmerged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 can activate the shock.
        /// </summary>
        public bool CanActivateShock => Internal.CanActivateShock;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is ready for idle.
        /// </summary>
        public bool CanActivateIdle => Internal.CanActivateIdle;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of a door.
        /// </summary>
        public bool IsInsideDoor => false; // TODO

        /// <summary>
        /// Gets the door that SCP-106 is currently inside of.
        /// </summary>
        public Door InsideDoor => null; // TODO

        /// <summary>
        /// Gets the <see cref="Scp106SinkholeController"/>.
        /// </summary>
        public Scp106SinkholeController SinkholeController => Internal.Sinkhole;

        /// <summary>
        /// Gets or sets the amount of time in between player captures.
        /// </summary>
        public float CaptureCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp106Attack ability) ? ability._hitCooldown : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp106Attack ability))
                {
                    ability._hitCooldown = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Scp106GameRole"/>.
        /// </summary>
        protected Scp106GameRole Internal { get; }

        /// <summary>
        /// Forces SCP-106 to use its portal, if one is placed.
        /// </summary>
        public void UsePortal()
        {
            if (SubroutineModule.TryGetSubroutine(out Scp106HuntersAtlasAbility ability))
                ability.SetSubmerged(true);
        }

        /// <summary>
        /// Causes SCP-106 to enter his stalking mode.
        /// </summary>
        public void Stalk()
        {
            if (SubroutineModule.TryGetSubroutine(out Scp106StalkAbility ability))
                ability.IsActive = true;
        }
    }
}