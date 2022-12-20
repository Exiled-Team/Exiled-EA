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
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player">owner</see>.</param>
        /// <param name="handler">The player's <see cref="DamageHandlerBase"/>.</param>
        /// <param name="canBeSpawned">A value that represents whether the ragdoll can be spawned.</param>
        public Ragdoll(Player player, DamageHandlerBase handler, bool canBeSpawned = false)
        {
            if (player.Role.Base is not IRagdollRole ragdollRole)
                return;

            GameObject modelRagdoll = ragdollRole.Ragdoll.gameObject;

            if (modelRagdoll == null || !Object.Instantiate(modelRagdoll).TryGetComponent(out BasicRagdoll ragdoll))
                return;

            ragdoll.NetworkInfo = new RagdollData(player.ReferenceHub, handler, modelRagdoll.transform.localPosition, modelRagdoll.transform.localRotation);

            Base = ragdoll;

            Map.RagdollsValue.Add(this);

            if (canBeSpawned)
                Spawn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="networkInfo">The ragdoll's <see cref="RagdollData"/>.</param>
        /// <param name="canBeSpawned">A value that represents whether the ragdoll can be spawned.</param>
        public Ragdoll(RagdollData networkInfo, bool canBeSpawned = false)
        {
            if (networkInfo.RoleType.GetRoleBase() is not IRagdollRole ragdollRole)
                return;

            GameObject modelRagdoll = ragdollRole.Ragdoll.gameObject;

            if (modelRagdoll == null || !Object.Instantiate(modelRagdoll).TryGetComponent(out BasicRagdoll ragdoll))
                return;

            ragdoll.NetworkInfo = networkInfo;

            Base = ragdoll;

            Map.RagdollsValue.Add(this);

            if (canBeSpawned)
                Spawn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="BasicRagdoll"/>.</param>
        internal Ragdoll(BasicRagdoll ragdoll) => Base = ragdoll;

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
        /// Gets the <see cref="Ragdoll"/> belonging to the <see cref="BasicRagdoll"/>, if any.
        /// </summary>
        /// <param name="ragdoll">The <see cref="BasicRagdoll"/> to get.</param>
        /// <returns>A <see cref="Ragdoll"/> or <see langword="null"/> if not found.</returns>
        public static Ragdoll Get(BasicRagdoll ragdoll) => Map.Ragdolls.FirstOrDefault(rd => rd.Base == ragdoll);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(Player player) => Map.Ragdolls.Where(rd => rd.Owner == player);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="IEnumerable{T}"/> of <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="players">The <see cref="Player"/>s to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(IEnumerable<Player> players) => players.SelectMany(pl => Map.Ragdolls.Where(rd => rd.Owner == pl));

        /// <summary>
        /// Spawns a <see cref="Ragdoll"/> on the map.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player"/> owner.</param>
        /// <param name="handler">The ragdoll's <see cref="DamageHandlerBase"/>.</param>
        /// <returns>The created <see cref="Ragdoll"/>.</returns>
        public static Ragdoll Spawn(Player player, DamageHandlers.DamageHandlerBase handler) => new(player, handler, true);

        /// <summary>
        /// Deletes the ragdoll.
        /// </summary>
        public void Delete()
        {
            Object.Destroy(GameObject);
            Map.RagdollsValue.Remove(this);
        }

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