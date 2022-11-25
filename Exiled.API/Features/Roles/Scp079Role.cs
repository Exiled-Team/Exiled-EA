// -----------------------------------------------------------------------
// <copyright file="Scp079Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Interactables.Interobjects.DoorUtils;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.PlayableScps.Subroutines;
    using System.Collections.Generic;
    using UnityEngine;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;

    /// <summary>
    /// Defines a role that represents SCP-079.
    /// </summary>
    public class Scp079Role : ScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Role"/> class.
        /// </summary>
        /// <param name="owner">The encapsulated <see cref="Player"/>.</param>
        public Scp079Role(Player owner)
            : base(owner)
        {
            Internal = Base as Scp079GameRole;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp079;

        /// <inheritdoc/>
        public override SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// </summary>
        public Scp079Camera Camera // TODO: Convert to Features.Camera
        {
            get => Internal.CurrentCamera;
            set => Internal._curCamSync.CurrentCamera = value;
        }

        /// <summary>
        /// Gets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public Scp079Speaker Speaker
        {
            get
            {
                if (Camera != null && Scp079Speaker.TryGetSpeaker(Camera, out Scp079Speaker speaker))
                    return speaker;

                return null;
            }
        }


        /// <summary>
        /// Gets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public HashSet<DoorVariant> LockedDoors => SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger ability) ? ability._lockedDoors : null;

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public Scp079AbilityBase[] Abilities
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability) ? ability._abilities : null;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability))
                    return;

                ability._abilities = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience SCP-079 has.
        /// </summary>
        public int Experience
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079TierManager ability) ? ability.TotalExp : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp079TierManager ability))
                    return;

                ability.TotalExp = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's level.
        /// </summary>
        public int Level
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079TierManager ability) ? ability.AccessTierLevel : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp079TierManager ability))
                    return;

                ability.AccessTierIndex = value - 1;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's level index.
        /// </summary>
        public int LevelIndex
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079TierManager ability) ? ability.AccessTierIndex : 0;
            set => Level = value + 1;
        }

        /// <summary>
        /// Gets SCP-079's next level threshold.
        /// </summary>
        public int NextLevelThreshold => SubroutineModule.TryGetSubroutine(out Scp079TierManager ability) ? ability.NextLevelThreshold : 0;

        /// <summary>
        /// Gets or sets SCP-079's energy.
        /// </summary>
        public float Energy
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability) ? ability.CurrentAux : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability))
                    return;

                ability.CurrentAux = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's max energy.
        /// </summary>
        public float MaxEnergy
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability) ? ability.MaxAux : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability))
                    return;

                ability._maxPerTier[LevelIndex] = value;
            }
        }

        /// <summary>
        /// Gets SCP-079's energy regeneration speed.
        /// </summary>
        public float EnergyRegenerationSpeed => SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability) ? ability.RegenSpeed : 0;

        /// <summary>
        /// Gets the game <see cref="Scp079GameRole"/>.
        /// </summary>
        protected Scp079GameRole Internal { get; }

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="cameraId">Camera ID.</param>
        public void SetCamera(ushort cameraId)
        {
            // TODO
        }

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="cameraType">The <see cref="Enums.CameraType"/>.</param>
        public void SetCamera(Enums.CameraType cameraType) => SetCamera(Camera.Get(cameraType));

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="camera">The <see cref="Camera"/> object to switch to.</param>
        public void SetCamera(Camera camera)
        {
            // TODO
        }

        /// <summary>
        /// Unlocks all doors that SCP-079 has locked.
        /// </summary>
        public void UnlockAllDoors()
        {
            if (SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger ability))
                ability.ServerUnlockAll();
        }
    }
}