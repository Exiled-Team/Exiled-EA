// -----------------------------------------------------------------------
// <copyright file="Scp079Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;
    using Discord;
    using Interactables.Interobjects.DoorUtils;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp096;
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

            if (!SubroutineModule.TryGetSubroutine(out Scp079SpeakerAbility scp079SpeakerAbility))
                Log.Error("Scp079SpeakerAbility subroutine not found in Scp079Role::ctor");

            SpeakerAbility = scp079SpeakerAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger scp079DoorLockChanger))
                Log.Error("Scp079DoorLockChanger subroutine not found in Scp079Role::ctor");
            DoorLockChanger = scp079DoorLockChanger;

            if (!SubroutineModule.TryGetSubroutine(out Scp079AuxManager scp079AuxManager))
                Log.Error("Scp079AuxManager not found in Scp079Role::ctor");

            AuxManager = scp079AuxManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp079TierManager scp079TierManager))
                Log.Error("Scp079TierManager subroutine not found in Scp079Role::ctor");

            TierManager = scp079TierManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp079LockdownRoomAbility scp079LockdownRoomAbility))
                Log.Error("Scp079LockdownRoomAbility subroutine not found in Scp079Role::ctor");

            LockdownRoomAbility = scp079LockdownRoomAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079BlackoutRoomAbility scp079BlackoutRoomAbility))
                Log.Error("Scp079BlackoutRoomAbility subroutine not found in Scp079Role::ctor");

            BlackoutRoomAbility = scp079BlackoutRoomAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079BlackoutZoneAbility scp079BlackoutZoneAbility))
                Log.Error("Scp079BlackoutZoneAbility subroutine not found in Scp079Role::ctor");

            BlackoutZoneAbility = scp079BlackoutZoneAbility;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp079;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079SpeakerAbility SpeakerAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079DoorLockChanger DoorLockChanger { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079AuxManager AuxManager { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079TierManager TierManager { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079LockdownRoomAbility LockdownRoomAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079BlackoutRoomAbility BlackoutRoomAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp079BlackoutZoneAbility BlackoutZoneAbility { get; }

        /// <summary>
        /// Gets the camera SCP-079 is currently controlling.
        /// </summary>
        public Camera Camera => Camera.Get(Internal.CurrentCamera);

        /// <summary>
        /// Gets a value indicating whether or not SCP-079 can transmit its voice to a speaker.
        /// </summary>
        public bool CanTransmit => SpeakerAbility.CanTransmit;

        /// <summary>
        /// Gets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public Scp079Speaker Speaker => Scp079Speaker.TryGetSpeaker(Internal.CurrentCamera, out Scp079Speaker speaker) ? speaker : null;

        /// <summary>
        /// Gets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public IEnumerable<Door> LockedDoors => DoorLockChanger._lockedDoors.Select(x => Door.Get(x));

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public Scp079AbilityBase[] Abilities
        {
            get => AuxManager._abilities;
            set => AuxManager._abilities = value;
        }

        /// <summary>
        /// Gets or sets the amount of experience SCP-079 has.
        /// </summary>
        public int Experience
        {
            get => TierManager.TotalExp;
            set => TierManager.TotalExp = value;
        }

        /// <summary>
        /// Gets or sets SCP-079's level.
        /// </summary>
        public int Level
        {
            get => TierManager.AccessTierLevel;
            set => Experience = value <= 1 ? 0 : TierManager.AbsoluteThresholds[Mathf.Clamp(value - 2, 0, TierManager.AbsoluteThresholds.Length - 1)];
        }

        /// <summary>
        /// Gets or sets SCP-079's level index.
        /// </summary>
        public int LevelIndex
        {
            get => TierManager.AccessTierIndex;
            set => Level = value + 1;
        }

        /// <summary>
        /// Gets SCP-079's next level threshold.
        /// </summary>
        public int NextLevelThreshold => TierManager.NextLevelThreshold;

        /// <summary>
        /// Gets or sets SCP-079's energy.
        /// </summary>
        public float Energy
        {
            get => AuxManager.CurrentAux;
            set => AuxManager.CurrentAux = value;
        }

        /// <summary>
        /// Gets or sets SCP-079's max energy.
        /// </summary>
        public float MaxEnergy
        {
            get => AuxManager.MaxAux;
            set => AuxManager._maxPerTier[LevelIndex] = value;
        }

        /// <summary>
        /// Gets or sets SCP-079's room lockdown cooldown.
        /// </summary>
        public float RoomLockdownCooldown
        {
            get => LockdownRoomAbility.RemainingCooldown;
            set
            {
                LockdownRoomAbility.RemainingCooldown = value;
                LockdownRoomAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets the amount of rooms that SCP-079 has blacked out.
        /// </summary>
        public int BlackoutCount => BlackoutRoomAbility.RoomsOnCooldown;

        /// <summary>
        /// Gets the maximum amount of rooms that SCP-079 can black out at its current <see cref="Level"/>.
        /// </summary>
        public int BlackoutCapacity => BlackoutRoomAbility.CurrentCapacity;

        /// <summary>
        /// Gets or sets the amount of time until SCP-079 can use its blackout zone ability again.
        /// </summary>
        public float BlackoutZoneCooldown
        {
            get => BlackoutZoneAbility._cooldownTimer.Remaining;
            set
            {
                BlackoutZoneAbility._cooldownTimer.Remaining = value;
                BlackoutZoneAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets SCP-079's energy regeneration speed.
        /// </summary>
        public float EnergyRegenerationSpeed => AuxManager.RegenSpeed;

        /// <summary>
        /// Gets the game <see cref="Scp079GameRole"/>.
        /// </summary>
        protected Scp079GameRole Internal { get; }

        /// <summary>
        /// Unlocks all doors that SCP-079 has locked.
        /// </summary>
        public void UnlockAllDoors() => DoorLockChanger.ServerUnlockAll();

        /// <summary>
        /// Locks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to lock.</param>
        /// <returns>.</returns>
        public bool LockDoor(Door door) => DoorLockChanger.SetDoorLock(door.Base, true);

        /// <summary>
        /// Unlocks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to unlock.</param>
        public void UnlockDoor(Door door) => DoorLockChanger.SetDoorLock(door.Base, false);
    }
}