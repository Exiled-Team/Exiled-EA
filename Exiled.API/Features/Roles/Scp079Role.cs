// -----------------------------------------------------------------------
// <copyright file="Scp079Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using Interactables.Interobjects.DoorUtils;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Subroutines;

    using Mathf = UnityEngine.Mathf;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;

    /// <summary>
    /// Defines a role that represents SCP-079.
    /// </summary>
    public class Scp079Role : Role, ISubroutinedScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp079GameRole"/>.</param>
        internal Scp079Role(Scp079GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            Internal = baseRole;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp079;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets the camera SCP-079 is currently controlling.
        /// </summary>
        public Camera Camera => Camera.Get(Internal.CurrentCamera);

        /// <summary>
        /// Gets a value indicating whether or not SCP-079 can transmit its voice to a speaker.
        /// </summary>
        public bool CanTransmit => SubroutineModule.TryGetSubroutine(out Scp079SpeakerAbility ability) && ability.CanTransmit;

        /// <summary>
        /// Gets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public Scp079Speaker Speaker
        {
            get
            {
                if (Internal.CurrentCamera != null && Scp079Speaker.TryGetSpeaker(Internal.CurrentCamera, out Scp079Speaker speaker))
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
                if (!SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability))
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
                if (!SubroutineModule.TryGetSubroutine(out Scp079TierManager ability))
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
                if (!SubroutineModule.TryGetSubroutine(out Scp079TierManager ability))
                    return;

                Experience = value <= 1 ? 0 : ability.AbsoluteThresholds[Mathf.Clamp(value - 2, 0, ability.AbsoluteThresholds.Length - 1)];
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
                if (!SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability))
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
                if (!SubroutineModule.TryGetSubroutine(out Scp079AuxManager ability))
                    return;

                ability._maxPerTier[LevelIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's room lockdown cooldown.
        /// </summary>
        public float RoomLockdownCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079LockdownRoomAbility ability) ? ability.RemainingCooldown : 0;
            set
            {
                if (!SubroutineModule.TryGetSubroutine(out Scp079LockdownRoomAbility ability))
                    return;

                ability.RemainingCooldown = value;
                ability.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets the amount of rooms that SCP-079 has blacked out.
        /// </summary>
        public int BlackoutCount => SubroutineModule.TryGetSubroutine(out Scp079BlackoutRoomAbility ability) ? ability.RoomsOnCooldown : 0;

        /// <summary>
        /// Gets the maximum amount of rooms that SCP-079 can black out at its current <see cref="Level"/>.
        /// </summary>
        public int BlackoutCapacity => SubroutineModule.TryGetSubroutine(out Scp079BlackoutRoomAbility ability) ? ability.CurrentCapacity : 0;

        /// <summary>
        /// Gets or sets the amount of time until SCP-079 can use its blackout zone ability again.
        /// </summary>
        public float BlackoutZoneCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp079BlackoutZoneAbility ability) ? ability._cooldownTimer.Remaining : 0;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp079BlackoutZoneAbility ability))
                {
                    ability._cooldownTimer.Remaining = value;
                    ability.ServerSendRpc(true);
                }
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
        /// Unlocks all doors that SCP-079 has locked.
        /// </summary>
        public void UnlockAllDoors()
        {
            if (SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger ability))
                ability.ServerUnlockAll();
        }

        /// <summary>
        /// Locks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to lock.</param>
        public void LockDoor(Door door)
        {
            if (door is null || !SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger ability))
                return;

            ability.SetDoorLock(door.Base, true);
        }

        /// <summary>
        /// Unlocks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to unlock.</param>
        public void UnlockDoor(Door door)
        {
            if (door is null || !SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger ability))
                return;

            ability.SetDoorLock(door.Base, false);
        }
    }
}