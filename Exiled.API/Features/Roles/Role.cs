// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;

    using Enums;
    using Extensions;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    public abstract class Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Owner"/>'s <see cref="Role"/>.</param>
        protected Role(Player owner)
        {
            Owner = owner;
            FirstPersonController = Base as FpcStandardRoleBase;
        }

        /// <summary>
        /// Gets the <see cref="Player"/> this role is referring to.
        /// </summary>
        public virtual Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> of this <see cref="Player"/>.
        /// </summary>
        public abstract RoleTypeId Type { get; }

        /// <summary>
        /// Gets the <see cref="FirstPersonController"/>.
        /// </summary>
        public FpcStandardRoleBase FirstPersonController { get; }

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/>.
        /// </summary>
        public PlayerRoleBase Base => Owner.RoleManager.CurrentRole;

        /// <summary>
        /// Gets the <see cref="SpawnReason"/>.
        /// </summary>
        public RoleChangeReason SpawnReason => Base.ServerSpawnReason;

        /// <summary>
        /// Gets the <see cref="PlayerRoles.Team"/> of this <see cref="Role"/>.
        /// </summary>
        public Team Team => RoleExtensions.GetTeam(Type);

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this <see cref="Role"/>.
        /// </summary>
        public Side Side => Type.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this <see cref="Role"/>.
        /// </summary>
        public Color Color => Type.GetColor();

        /// <summary>
        /// Gets the <see cref="Role"/> full name.
        /// </summary>
        public string Name => Base.RoleName;

        /// <summary>
        /// Gets the last time the <see cref="Role"/> was active.
        /// </summary>
        public TimeSpan ActiveTime => TimeSpan.FromSeconds((double)Base.ActiveTime);

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
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => Type == Owner.RoleManager.CurrentRole.RoleTypeId;

        /// <summary>
        /// Converts a role to its appropriate <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        public static implicit operator RoleTypeId(Role role) => role?.Type ?? RoleTypeId.None;

        /// <summary>
        /// Returns whether or not 2 roles are the same.
        /// </summary>
        /// <param name="left">The role.</param>
        /// <param name="right">The other role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role left, Role right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        /// <summary>
        /// Returns whether or not the two roles are different.
        /// </summary>
        /// <param name="left">The role.</param>
        /// <param name="right">The other role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role left, Role right) => !(left == right);

        /// <summary>
        /// Returns whether or not the role has the same RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleTypeId typeId) => role?.Type == typeId;

        /// <summary>
        /// Returns whether or not the role has a different RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleTypeId typeId) => !(role == typeId);

        /// <summary>
        /// Returns whether or not the role has the same RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleTypeId type, Role role) => role == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleTypeId type, Role role) => role != type;

        /// <summary>
        /// Casts the role to the specified role type.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>The casted class, if possible.</returns>
        public T As<T>()
            where T : Role => this as T;

        /// <summary>
        /// Safely casts the role to the specified role type.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <param name="role">The casted class, if possible.</param>
        /// <returns><see langword="true"/> if the cast was successful; otherwise, <see langword="false"/>.</returns>
        public bool Is<T>(out T role)
            where T : Role
        {
            role = this is T type ? type : null;

            return role is not null;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Returns the role in a human-readable format.
        /// </summary>
        /// <returns>A string containing role-related data.</returns>
        public override string ToString() => $"{Side} {Team} {Type} {IsValid}";

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="reason">The <see cref="SpawnReason"/> defining why the player's role was changed.</param>
        public virtual void Set(RoleTypeId newRole, SpawnReason reason = Enums.SpawnReason.ForceClass) => Owner.RoleManager.ServerSetRole(newRole, (RoleChangeReason)reason);

        /// <summary>
        /// Creates a role from <see cref="RoleTypeId"/> and <see cref="Player"/>.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/>.</param>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <returns>The created <see cref="Role"/> instance.</returns>
        internal static Role Create(Player owner, RoleTypeId type) => type switch
        {
            RoleTypeId.Scp049 => new Scp049Role(owner),
            RoleTypeId.Scp0492 => new Scp0492Role(owner),
            RoleTypeId.Scp079 => new Scp079Role(owner),
            RoleTypeId.Scp096 => new Scp096Role(owner),
            RoleTypeId.Scp106 => new Scp106Role(owner),
            RoleTypeId.Scp173 => new Scp173Role(owner),
            RoleTypeId.Scp939 => new Scp939Role(owner),
            RoleTypeId.Spectator or RoleTypeId.Overwatch => new SpectatorRole(owner),
            RoleTypeId.None => new NoneRole(owner),
            _ => new GenericHumanRole(owner, type),
        };
    }
}