// -----------------------------------------------------------------------
// <copyright file="Lift.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    using static Interactables.Interobjects.ElevatorChamber;
    using static Interactables.Interobjects.ElevatorManager;

    /// <summary>
    /// The in-game lift.
    /// </summary>
    public class Lift
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="ElevatorChamber"/>s and their corresponding <see cref="Lift"/>.
        /// </summary>
        internal static readonly Dictionary<ElevatorChamber, Lift> ElevatorChamberToLift = new(8);

        /// <summary>
        /// Initializes a new instance of the <see cref="Lift"/> class.
        /// </summary>
        /// <param name="elevator">The <see cref="ElevatorChamber"/> to wrap.</param>
        internal Lift(ElevatorChamber elevator)
        {
            Base = elevator;
            ElevatorChamberToLift.Add(elevator, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains all the <see cref="Lift"/> instances.
        /// </summary>
        public static IEnumerable<Lift> List => ElevatorChamberToLift.Values;

        /// <summary>
        /// Gets a random <see cref="Lift"/>.
        /// </summary>
        /// <returns><see cref="Lift"/> object.</returns>
        public static Lift Random => List.ToArray()[UnityEngine.Random.Range(0, ElevatorChamberToLift.Count)];

        /// <summary>
        /// Gets the base <see cref="ElevatorChamber"/>.
        /// </summary>
        public ElevatorChamber Base { get; }

        /// <summary>
        /// Gets the lift's name.
        /// </summary>
        public string Name => Base.AssignedGroup.ToString();

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the lift.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the lift's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => GameObject.transform;

        /// <summary>
        /// Gets or sets the lift's position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.transform.position;
            set => Base.transform.position = value;
        }

        /// <summary>
        /// Gets or sets the lift's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.transform.rotation;
            set => Base.transform.rotation = value;
        }

        /// <summary>
        /// Gets or sets the lift's <see cref="ElevatorChamber"/> status.
        /// </summary>
        public ElevatorSequence Status
        {
            get => Base._curSequence;
            set => Base._curSequence = value;
        }

        /// <summary>
        /// Gets the lift's <see cref="ElevatorType"/>.
        /// </summary>
        public ElevatorType Type => Base.AssignedGroup switch
        {
            ElevatorGroup.Scp049 => ElevatorType.Scp049,
            ElevatorGroup.GateA => ElevatorType.GateA,
            ElevatorGroup.GateB => ElevatorType.GateB,
            ElevatorGroup.LczA01 or ElevatorGroup.LczA02 => ElevatorType.LczA,
            ElevatorGroup.LczB01 or ElevatorGroup.LczB02 => ElevatorType.LczB,
            ElevatorGroup.Nuke => ElevatorType.Nuke,
            _ => ElevatorType.Unknown,
        };

        /// <summary>
        /// Gets the <see cref="ElevatorGroup"/>.
        /// </summary>
        public ElevatorGroup Group => Base.AssignedGroup;

        /// <summary>
        /// Gets a value indicating whether the lift is operative.
        /// </summary>
        public bool IsOperative => Base.IsReady;

        /// <summary>
        /// Gets a value indicating whether the lift is currently moving.
        /// </summary>
        public bool IsMoving => Status == ElevatorSequence.MovingAway || Status == ElevatorSequence.Arriving;

        /// <summary>
        /// Gets or sets a value indicating whether the lift is locked.
        /// </summary>
        public bool IsLocked
        {
            get => Base.ActiveLocks != DoorLockReason.None;
            set => Base.ActiveLocks = DoorLockReason.AdminCommand;
        }

        /// <summary>
        /// Gets or sets the <see cref="AnimationTime"/>.
        /// </summary>
        public float AnimationTime
        {
            get => Base._animationTime;
            set => Base._animationTime = value;
        }

        /// <summary>
        /// Gets the <see cref="CurrentLevel"/>.
        /// </summary>
        public int CurrentLevel => Base.CurrentLevel;

        /// <summary>
        /// Gets the <see cref="CurrentDestination"/>.
        /// </summary>
        public ElevatorDoor CurrentDestination => Base.CurrentDestination;

        /// <summary>
        /// Compares two operands: <see cref="Lift"/> and <see cref="Lift"/>.
        /// </summary>
        /// <param name="left">The first <see cref="Lift"/> to compare.</param>
        /// <param name="right">The second <see cref="Lift"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Lift left, Lift right) => left.Base.Equals(right.Base);

        /// <summary>
        /// Compares two operands: <see cref="Lift"/> and <see cref="Lift"/>.
        /// </summary>
        /// <param name="left">The first <see cref="Lift"/> to compare.</param>
        /// <param name="right">The second <see cref="Lift"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Lift left, Lift right) => !(left == right);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains all the <see cref="Lift"/> instances from the specified <see cref="Status"/>.
        /// </summary>
        /// <param name="status">The specified <see cref="ElevatorChamber"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Lift> Get(ElevatorSequence status) => Get(lift => lift.Status == status);

        /// <summary>
        /// Gets the <see cref="Lift"/> belonging to the <see cref="ElevatorChamber"/>, if any.
        /// </summary>
        /// <param name="elevator">The <see cref="ElevatorChamber"/> instance.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(ElevatorChamber elevator) => ElevatorChamberToLift.TryGetValue(elevator, out Lift lift) ? lift : new(elevator);

        /// <summary>
        /// Gets the <see cref="Lift"/> corresponding to the specified <see cref="ElevatorType"/>, if any.
        /// </summary>
        /// <param name="type">The <see cref="ElevatorType"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(ElevatorType type) => Get(lift => lift.Type == type).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="Lift"/> corresponding to the specified name, if any.
        /// </summary>
        /// <param name="name">The lift's name.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(string name) => Get(lift => lift.Name == name).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="Lift"/> belonging to the <see cref="UnityEngine.GameObject"/>, if any.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(GameObject gameObject) => Get(lift => lift.GameObject == gameObject).FirstOrDefault();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Lift> Get(Func<Lift, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Tries to melt a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to melt.</param>
        /// <returns><see langword="true"/> if the player was melted successfully; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="Player.EnableEffect(EffectType, float, bool)"/>
        public static bool TryMeltPlayer(Player player)
        {
            if (player.Position.y is >= 200 or <= -200)
                return false;

            player.EnableEffect(EffectType.Decontaminating);

            return true;
        }

        /// <summary>
        /// Tries to start the lift.
        /// </summary>
        /// <param name="level">The destination level.</param>
        /// <param name="isForced">Indicates whether the start will be forced or not.</param>
        /// <returns><see langword="true"/> if the lift was started successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryStart(int level, bool isForced = false) => TrySetDestination(Base.AssignedGroup, level, isForced);

        /// <inheritdoc/>
        public override bool Equals(object obj) => Base.Equals(obj);

        /// <inheritdoc/>
        public override int GetHashCode() => Base.GetHashCode();

        /// <summary>
        /// Returns the Lift in a human-readable format.
        /// </summary>
        /// <returns>A string containing Lift-related data.</returns>
        public override string ToString() => $"{Type} {Status} [{CurrentLevel}] *{IsLocked}*";
    }
}