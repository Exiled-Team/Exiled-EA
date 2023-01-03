// -----------------------------------------------------------------------
// <copyright file="Scp939Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;

    /// <summary>
    /// Defines a role that represents SCP-939.
    /// </summary>
    public class Scp939Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp939Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp939GameRole"/>.</param>
        internal Scp939Role(Scp939GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp939ClawAbility sp939ClawAbility))
                Log.Error("Scp939ClawAbility not found in Scp939Role::ctor");

            ClawAbility = sp939ClawAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp939FocusAbility scp939FocusAbility))
                Log.Error("Scp939FocusAbility not found in Scp939Role::ctor");

            FocusAbility = scp939FocusAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp939LungeAbility scp939LungeAbility))
                Log.Error("Scp939LungeAbility not found in Scp939Role::ctor");

            LungeAbility = scp939LungeAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility scp939AmnesticCloudAbility))
                Log.Error("Scp939AmnesticCloudAbility not found in Scp939Role::ctor");

            AmnesticCloudAbility = scp939AmnesticCloudAbility;

            if (!SubroutineModule.TryGetSubroutine(out EnvironmentalMimicry environmentalMimicry))
                Log.Error("EnvironmentalMimicry not found in Scp939Role::ctor");

            EnvironmentalMimicry = environmentalMimicry;

            if (!SubroutineModule.TryGetSubroutine(out MimicryRecorder mimicryRecorder))
                Log.Error("MimicryRecorder not found in Scp939Role::ctor");

            MimicryRecorder = mimicryRecorder;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp939;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp939ClawAbility ClawAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp939FocusAbility FocusAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp939LungeAbility LungeAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp939AmnesticCloudAbility AmnesticCloudAbility { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public EnvironmentalMimicry EnvironmentalMimicry { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public MimicryRecorder MimicryRecorder { get; }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => ClawAbility.Cooldown.Remaining;
            set
            {
                ClawAbility.Cooldown.Remaining = value;
                ClawAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 is currently using its focus ability.
        /// </summary>
        public bool IsFocused => FocusAbility.TargetState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 is currently lunging.
        /// </summary>
        public bool IsLunging => LungeAbility.State is not Scp939LungeState.None;

        /// <summary>
        /// Gets SCP-939's <see cref="Scp939LungeState"/>.
        /// </summary>
        public Scp939LungeState LungeState => LungeAbility.State;

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can use its amnestic cloud ability again.
        /// </summary>
        public float AmnesticCloudCooldown
        {
            get => AmnesticCloudAbility.Cooldown.Remaining;
            set
            {
                AmnesticCloudAbility.Cooldown.Remaining = value;
                AmnesticCloudAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can use any of its mimicry abilities again.
        /// </summary>
        public float MimicryCooldown
        {
            get => EnvironmentalMimicry.Cooldown.Remaining;
            set
            {
                EnvironmentalMimicry.Cooldown.Remaining = value;
                EnvironmentalMimicry.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets a value indicating the amount of voices that SCP-939 has saved.
        /// </summary>
        public int SavedVoices => MimicryRecorder.SavedVoices.Count;

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 has a placed mimic point.
        /// </summary>
        public bool MimicryPointActive => EnvironmentalMimicry._mimicPoint.Active;

        /// <summary>
        /// Gets a value indicating the position of SCP-939's mimic point. May be <see langword="null"/> if <see cref="MimicryPointActive"/> is <see langword="false"/>.
        /// </summary>
        public Vector3? MimicryPointPosition => EnvironmentalMimicry._mimicPoint.Active ? EnvironmentalMimicry._mimicPoint.MimicPointTransform.position : null;

        /// <summary>
        /// Gets a list of players this SCP-939 instance can see regardless of their movement.
        /// </summary>
        public List<Player> VisiblePlayers { get; } = new();

        /// <summary>
        /// Removes all recordings of player voices. Provide an optional target to remove all the recordings of a single player.
        /// </summary>
        /// <param name="target">If provided, will only remove recordings of the targeted player.</param>
        public void ClearRecordings(Player target)
        {
            if (target is null)
                return;
            MimicryRecorder.RemoveRecordingsOfPlayer(target.ReferenceHub);
            MimicryRecorder.SavedVoicesModified = true;
        }

        /// <summary>
        /// Removes all recordings of player voices. Provide an optional target to remove all the recordings of a single player.
        /// </summary>
        public void ClearRecordings()
        {
            MimicryRecorder.SavedVoices.Clear();
            MimicryRecorder._serverSentVoices.Clear();
            MimicryRecorder.SavedVoicesModified = true;
        }
    }
}