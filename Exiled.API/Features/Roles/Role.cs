// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Enums;
    using Extensions;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    public abstract class Role
    {
#pragma warning disable 1584
        /// <summary>
        /// Gets the player this role is referring to.
        /// </summary>
        public abstract Player Owner { get; }

        /// <summary>
        /// Gets or sets the <see cref="TypeId"/> of this player.
        /// </summary>
        public RoleTypeId Type
        {
            get => TypeId;
            set => Owner?.SetRole(value);
        }

        /// <summary>
        /// Gets the <see cref="PlayerRoles.Team"/> of this role.
        /// </summary>
        public Team Team => RoleExtensions.GetTeam(Type);

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this role.
        /// </summary>
        public Side Side => Type.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this role.
        /// </summary>
        public Color Color => Type.GetColor();

        /// <summary>
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => Type == Owner.RoleManager.CurrentRole.RoleTypeId;

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> belonging to this role.
        /// </summary>
        internal abstract RoleTypeId TypeId { get; }

        /// <summary>
        /// Converts a role to its appropriate <see cref="PlayerRoles.RoleTypeId"/>.
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
        public static bool operator !=(Role left, Role right)
        {
            if (left is null)
                return right is not null;

            return !left.Equals(right);
        }

        /// <summary>
        /// Returns whether or not the role has the same RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="typeId">The <see cref="PlayerRoles.RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleTypeId typeId) => role?.Type == typeId;

        /// <summary>
        /// Returns whether or not the role has a different RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="typeId">The <see cref="PlayerRoles.RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleTypeId typeId) => role?.Type != typeId;

        /// <summary>
        /// Returns whether or not the role has the same RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="PlayerRoles.RoleTypeId"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleTypeId type, Role role) => role?.Type == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="PlayerRoles.RoleTypeId"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleTypeId type, Role role) => role?.Type != type;

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
        // public override string ToString() => $"{Side} {Team} {Type} {IsValid}";

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Creates a role from RoleTypeId and Player.
        /// </summary>
        /// <param name="type">The RoleTypeId.</param>
        /// <param name="player">The Player.</param>
        /// <returns>A role.</returns>
        internal static Role Create(RoleTypeId type, Player player) => type switch
        {
            RoleTypeId.Scp049 => new Scp049Role(player),
            RoleTypeId.Scp0492 => new Scp0492Role(player),
            RoleTypeId.Scp079 => new Scp079Role(player),
            RoleTypeId.Scp096 => new Scp096Role(player),
            RoleTypeId.Scp106 => new Scp106Role(player),
            RoleTypeId.Scp173 => new Scp173Role(player),
            RoleTypeId.Scp939 => new Scp939Role(player, type),
            RoleTypeId.Spectator => new SpectatorRole(player),
            RoleTypeId.None => new NoneRole(player),
            _ => new HumanRole(player, type),
        };
    }
}