// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;
    using System.Diagnostics;

    using Enums;
    using Exiled.API.Features.Spawn;
    using Extensions;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using UnityEngine;

    using HumanGameRole = PlayerRoles.HumanRole;
    using OverwatchGameRole = PlayerRoles.Spectating.OverwatchRole;
    using Scp049GameRole = PlayerRoles.PlayableScps.Scp049.Scp049Role;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;
    using Scp096GameRole = PlayerRoles.PlayableScps.Scp096.Scp096Role;
    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;
    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;
    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;
    using SpectatorGameRole = PlayerRoles.Spectating.SpectatorRole;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    public abstract class Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="PlayerRoleBase"/>.</param>
        protected Role(PlayerRoleBase baseRole)
        {
            if (!baseRole.TryGetOwner(out ReferenceHub hub) || !Player.TryGet(hub, out Player player))
            {
                Log.Error($"Unknown player {new StackTrace()}");
                return;
            }

            Base = baseRole;
            Owner = player;
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
        /// Gets the base <see cref="PlayerRoleBase"/>.
        /// </summary>
        public PlayerRoleBase Base { get; }

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
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => Type == Owner.RoleManager.CurrentRole.RoleTypeId;

        /// <summary>
        /// Gets a random spawn position of this role.
        /// </summary>
        /// <returns>The spawn position.</returns>
        public virtual SpawnLocation RandomSpawnLocation => Type.GetRandomSpawnLocation();

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
        /// <param name="role">The <see cref="PlayerRoleBase"/>.</param>
        /// <returns>The created <see cref="Role"/> instance.</returns>
        internal static Role Create(PlayerRoleBase role) => role switch
        {
            Scp049GameRole scp049Role => new Scp049Role(scp049Role),
            ZombieRole scp0492Role => new Scp0492Role(scp0492Role),
            Scp079GameRole scp079Role => new Scp079Role(scp079Role),
            Scp096GameRole scp096Role => new Scp096Role(scp096Role),
            Scp106GameRole scp106Role => new Scp106Role(scp106Role),
            Scp173GameRole scp173Role => new Scp173Role(scp173Role),
            Scp939GameRole scp939Role => new Scp939Role(scp939Role),
            OverwatchGameRole overwatchRole => new OverwatchRole(overwatchRole),
            SpectatorGameRole spectatorRole => new SpectatorRole(spectatorRole),
            HumanGameRole humanRole => new HumanRole(humanRole),
            _ => new NoneRole(role),
        };
    }
}
