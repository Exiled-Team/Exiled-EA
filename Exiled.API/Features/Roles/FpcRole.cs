// -----------------------------------------------------------------------
// <copyright file="FpcRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerStatsSystem;

    /// <summary>
    /// Defines a role that represents an fpc class.
    /// </summary>
    public abstract class FpcRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FpcRole"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="PlayerRoleBase"/>.</param>
        protected FpcRole(FpcStandardRoleBase baseRole)
            : base(baseRole)
        {
            FirstPersonController = baseRole;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FpcRole"/> class.
        /// </summary>
        ~FpcRole() => HashSetPool<Player>.Shared.Return(IsInvisibleFor);

        /// <summary>
        /// Gets the <see cref="FirstPersonController"/>.
        /// </summary>
        public FpcStandardRoleBase FirstPersonController { get; }

        /// <summary>
        /// Gets or sets the <see cref="Role"/> walking speed.
        /// </summary>
        public float WalkingSpeed
        {
            get => FirstPersonController.FpcModule.WalkSpeed;
            set => FirstPersonController.FpcModule.WalkSpeed = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Role"/> sprinting speed.
        /// </summary>
        public float SprintingSpeed
        {
            get => FirstPersonController.FpcModule.SprintSpeed;
            set => FirstPersonController.FpcModule.SprintSpeed = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Role"/> jumping speed.
        /// </summary>
        public float JumpingSpeed
        {
            get => FirstPersonController.FpcModule.JumpSpeed;
            set => FirstPersonController.FpcModule.JumpSpeed = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Role"/> crouching speed.
        /// </summary>
        public float CrouchingSpeed
        {
            get => FirstPersonController.FpcModule.CrouchSpeed;
            set => FirstPersonController.FpcModule.CrouchSpeed = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player can send inputs.
        /// </summary>
        public bool CanSendInputs => FirstPersonController.FpcModule.LockMovement;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is invisible.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// Gets a list of players who can't see the player.
        /// </summary>
        public HashSet<Player> IsInvisibleFor { get; } = HashSetPool<Player>.Shared.Rent();

        /// <summary>
        /// Gets or sets the player's current <see cref="PlayerMovementState"/>.
        /// </summary>
        public PlayerMovementState MoveState
        {
            get => FirstPersonController.FpcModule.CurrentMovementState;
            set => FirstPersonController.FpcModule.CurrentMovementState = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is crouching or not.
        /// </summary>
        public bool IsCrouching => FirstPersonController.FpcModule.StateProcessor.CrouchPercent > 0;

        /// <summary>
        /// Gets a value indicating whether or not the player is on the ground.
        /// </summary>
        public bool IsGrounded => FirstPersonController.FpcModule.IsGrounded;

        /// <summary>
        /// Gets the <see cref="Player"/>'s current movement speed.
        /// </summary>
        public virtual float MovementSpeed => FirstPersonController.FpcModule.VelocityForState(MoveState, IsCrouching);

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="Player"/> is in darkness.
        /// </summary>
        public bool IsInDarkness => FirstPersonController.InDarkness;

        /// <summary>
        /// Gets a value indicating whether or not this role is protected by a hume shield.
        /// </summary>
        public bool IsHumeShieldedRole => this is IHumeShieldRole;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has noclip enabled.
        /// </summary>
        /// <returns><see cref="bool"/> indicating status.</returns>
        /// <remarks>For permitting a player to enter and exit noclip freely, see <see cref="Player.IsNoclipPermitted"/>.</remarks>
        /// <seealso cref="Player.IsNoclipPermitted"/>
        public bool IsNoclipEnabled
        {
            get => Owner.ReferenceHub.playerStats.GetModule<AdminFlagsStat>().HasFlag(AdminFlags.Noclip);
            set => Owner.ReferenceHub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.Noclip, value);
        }
    }
}