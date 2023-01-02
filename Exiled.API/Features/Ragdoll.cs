// -----------------------------------------------------------------------
// <copyright file="Ragdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DeathAnimations;

    using Enums;
    using Exiled.API.Extensions;
    using Interactables.Interobjects.DoorUtils;
    using MapGeneration;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.Ragdolls;
    using PlayerStatsSystem;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to handle the ragdolls more easily.
    /// </summary>
    public class Ragdoll
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BasicRagdoll"/>s and their corresponding <see cref="Ragdoll"/>.
        /// </summary>
        internal static readonly Dictionary<BasicRagdoll, Ragdoll> BasicRagdollToRagdoll = new(250);

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="BasicRagdoll"/>.</param>
        internal Ragdoll(BasicRagdoll ragdoll)
        {
            Base = ragdoll;
            BasicRagdollToRagdoll.Add(ragdoll, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> which contains all the <see cref="Ragdoll"/> instances.
        /// </summary>
        public static IEnumerable<Ragdoll> List => BasicRagdollToRagdoll.Values;

        /// <summary>
        /// Gets or sets the <see cref="BasicRagdoll"/>s clean up time.
        /// </summary>
        public static int CleanUpTime
        {
            get => RagdollManager.CleanupTime;
            set => RagdollManager.CleanupTime = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the clean up event can be executed.
        /// </summary>
        public bool AllowCleanUp => NetworkInfo.ExistenceTime < CleanUpTime;

        /// <summary>
        /// Gets the <see cref="BasicRagdoll"/> instance of the ragdoll.
        /// </summary>
        public BasicRagdoll Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the ragdoll.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/> of the ragdoll.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RagdollData"/>.
        /// </summary>
        public RagdollData NetworkInfo
        {
            get => Base.NetworkInfo;
            set => Base.NetworkInfo = value;
        }

        /// <summary>
        /// Gets the ragdoll's <see cref="DamageHandlerBase"/>.
        /// </summary>
        public DamageHandlerBase DamageHandler => NetworkInfo.Handler;

        /// <summary>
        /// Gets the ragdoll's <see cref="Rigidbody"/>[].
        /// </summary>
        public Rigidbody[] SpecialRigidbodies => Base is DynamicRagdoll ragdoll ? ragdoll.LinkedRigidbodies : Array.Empty<Rigidbody>();

        /// <summary>
        /// Gets all ragdoll's <see cref="DeathAnimation"/>[].
        /// </summary>
        public DeathAnimation[] DeathAnimations => Base.AllDeathAnimations;

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll has been already cleaned up.
        /// </summary>
        public bool IsCleanedUp => Base._cleanedUp;

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll can be cleaned up.
        /// </summary>
        public bool CanBeCleanedUp
        {
            get => IgnoredRagdolls.Contains(Base);
            set
            {
                if (!value)
                    IgnoredRagdolls.Remove(Base);
                else
                    IgnoredRagdolls.Add(Base);
            }
        }

        /// <summary>
        /// Gets the ragdoll's name.
        /// </summary>
        public string Name => Base.name;

        /// <summary>
        /// Gets the owner <see cref="Player"/>. Can be <see langword="null"/> if the ragdoll does not have an owner.
        /// </summary>
        public Player Owner => Player.Get(NetworkInfo.OwnerHub);

        /// <summary>
        /// Gets the time that the ragdoll was spawned.
        /// </summary>
        public DateTime CreationTime => new((long)NetworkInfo.CreationTime);

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> of the ragdoll.
        /// </summary>
        public RoleTypeId Role => NetworkInfo.RoleType;

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll has expired and SCP-049 is unable to revive it.
        /// </summary>
        public bool IsExpired => NetworkInfo.ExistenceTime > PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.HumanCorpseDuration;

        /// <summary>
        /// Gets or sets a value indicating whether or not this ragdoll has been consumed by an SCP-049-2.
        /// </summary>
        public bool IsConsumed
        {
            get => ZombieConsumeAbility.ConsumedRagdolls.Contains(Base);
            set
            {
                if (value && !ZombieConsumeAbility.ConsumedRagdolls.Contains(Base))
                    ZombieConsumeAbility.ConsumedRagdolls.Add(Base);
                else if (!value && ZombieConsumeAbility.ConsumedRagdolls.Contains(Base))
                    ZombieConsumeAbility.ConsumedRagdolls.Remove(Base);
            }
        }

        /// <summary>
        /// Gets the <see cref="Features.Room"/> the ragdoll is located in.
        /// </summary>
        public Room Room => Map.FindParentRoom(GameObject);

        /// <summary>
        /// Gets the <see cref="ZoneType"/> the ragdoll is in.
        /// </summary>
        public ZoneType Zone => Room.Zone;

        /// <summary>
        /// Gets or sets the ragdoll's position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);

                Base.transform.position = value;

                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);

                Base.transform.rotation = value;

                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => Base.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);

                Base.transform.localScale = value;

                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets the ragdoll's death reason.
        /// </summary>
        public string DeathReason => DamageHandler.ServerLogsText;

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="BasicRagdoll"/>'s that will be ignored by clean up event.
        /// </summary>
        internal static HashSet<BasicRagdoll> IgnoredRagdolls { get; set; } = new();

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="networkInfo">The data associated with the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        /// <exception cref="ArgumentException">Provided RoleType is not a valid ragdoll role (Spectator, Scp079, etc).</exception>
        /// <exception cref="InvalidOperationException">Unable to create a ragdoll.</exception>
        public static Ragdoll Create(RagdollData networkInfo)
        {
            if (networkInfo.RoleType.GetRoleBase() is not IRagdollRole ragdollRole)
                throw new ArgumentException($"Provided RoleType '{networkInfo.RoleType}' is not a valid ragdoll role.");

            GameObject modelRagdoll = ragdollRole.Ragdoll.gameObject;

            if (modelRagdoll == null || !Object.Instantiate(modelRagdoll).TryGetComponent(out BasicRagdoll ragdoll))
                throw new InvalidOperationException("Unable to create a ragdoll.");

            ragdoll.NetworkInfo = networkInfo;

            return new(ragdoll)
            {
                Position = networkInfo.StartPosition,
                Rotation = networkInfo.StartRotation,
            };
        }

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="damageHandler">The damage handler responsible for the ragdoll's death.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll Create(RoleTypeId roleType, string name, DamageHandlerBase damageHandler, Player owner = null)
            => Create(new(owner?.ReferenceHub ?? Server.Host.ReferenceHub, damageHandler, roleType, default, default, name, NetworkTime.time));

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason the ragdoll died.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll Create(RoleTypeId roleType, string name, string deathReason, Player owner = null)
            => Create(new(owner?.ReferenceHub ?? Server.Host.ReferenceHub, new CustomReasonDamageHandler(deathReason), roleType, default, default, name, NetworkTime.time));

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="networkInfo">The data associated with the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll CreateAndSpawn(RagdollData networkInfo)
        {
            Ragdoll doll = Create(networkInfo);
            doll.Spawn();

            return doll;
        }

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="damageHandler">The damage handler responsible for the ragdoll's death.</param>
        /// <param name="position">The position of the ragdoll.</param>
        /// <param name="rotation">The rotation of the ragdoll.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll CreateAndSpawn(RoleTypeId roleType, string name, DamageHandlerBase damageHandler, Vector3 position, Quaternion rotation, Player owner = null)
            => CreateAndSpawn(new(owner?.ReferenceHub ?? Server.Host.ReferenceHub, damageHandler, roleType, position, rotation, name, NetworkTime.time));

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason the ragdoll died.</param>
        /// <param name="position">The position of the ragdoll.</param>
        /// <param name="rotation">The rotation of the ragdoll.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll CreateAndSpawn(RoleTypeId roleType, string name, string deathReason, Vector3 position, Quaternion rotation, Player owner = null)
            => CreateAndSpawn(new(owner?.ReferenceHub ?? Server.Host.ReferenceHub, new CustomReasonDamageHandler(deathReason), roleType, position, rotation, name, NetworkTime.time));

        /// <summary>
        /// Gets the <see cref="Ragdoll"/> belonging to the <see cref="BasicRagdoll"/>, if any.
        /// </summary>
        /// <param name="ragdoll">The <see cref="BasicRagdoll"/> to get.</param>
        /// <returns>A <see cref="Ragdoll"/> or <see langword="null"/> if not found.</returns>
        public static Ragdoll Get(BasicRagdoll ragdoll) => BasicRagdollToRagdoll.ContainsKey(ragdoll)
            ? BasicRagdollToRagdoll[ragdoll]
            : new Ragdoll(ragdoll);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(Player player) => Ragdoll.List.Where(rd => rd.Owner == player);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="IEnumerable{T}"/> of <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="players">The <see cref="Player"/>s to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(IEnumerable<Player> players) => players.SelectMany(pl => Ragdoll.List.Where(rd => rd.Owner == pl));

        /// <summary>
        /// Destroys the ragdoll.
        /// </summary>
        public void Destroy() => Object.Destroy(GameObject);

        /// <summary>
        /// Spawns the ragdoll.
        /// </summary>
        public void Spawn() => NetworkServer.Spawn(GameObject);

        /// <summary>
        /// Un-spawns the ragdoll.
        /// </summary>
        public void UnSpawn() => NetworkServer.UnSpawn(GameObject);

        /// <summary>
        /// Returns the Ragdoll in a human-readable format.
        /// </summary>
        /// <returns>A string containing Ragdoll-related data.</returns>
        public override string ToString() => $"{Owner} ({Name}) [{DeathReason}] *{Role}* |{CreationTime}| ={IsExpired}=";
    }
}