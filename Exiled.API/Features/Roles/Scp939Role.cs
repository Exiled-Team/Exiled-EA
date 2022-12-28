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
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using PlayerRoles.PlayableScps.Subroutines;

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
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp939;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp939ClawAbility ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp939ClawAbility ability))
                {
                    ability.Cooldown.Remaining = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 is currently using its focus ability.
        /// </summary>
        public bool IsFocused => SubroutineModule.TryGetSubroutine(out Scp939FocusAbility focus) && focus.TargetState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 is currently lunging.
        /// </summary>
        public bool IsLunging => SubroutineModule.TryGetSubroutine(out Scp939LungeAbility ability) && ability.State != Scp939LungeState.None;

        /// <summary>
        /// Gets or sets SCP-939's <see cref="Scp939LungeState"/>.
        /// </summary>
        public Scp939LungeState State
        {
            get => SubroutineModule.TryGetSubroutine(out Scp939LungeAbility ability) ? ability.State : Scp939LungeState.None;
            set
            {
                if (!SubroutineModule.TryGetSubroutine(out Scp939LungeAbility ability))
                    return;

                ability.State = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can use its amnestic cloud ability again.
        /// </summary>
        public float AmnesticCloudCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability))
                {
                    ability.Cooldown.Remaining = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can use any of its mimicry abilities again.
        /// </summary>
        public float MimicryCooldown
        {
            get => SubroutineModule.TryGetSubroutine(out EnvironmentalMimicry ability) ? ability.Cooldown.Remaining : 0f;
            set
            {
                if (SubroutineModule.TryGetSubroutine(out EnvironmentalMimicry ability))
                {
                    ability.Cooldown.Remaining = value;
                    ability.ServerSendRpc(true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the amount of voices that SCP-939 has saved.
        /// </summary>
        public int SavedVoices => SubroutineModule.TryGetSubroutine(out MimicryRecorder ability) ? ability.SavedVoices.Count : 0;

        /// <summary>
        /// Gets a value indicating whether or not SCP-939 has a placed mimic point.
        /// </summary>
        public bool MimicryPointActive => SubroutineModule.TryGetSubroutine(out EnvironmentalMimicry ability) && ability._mimicPoint.Active;

        /// <summary>
        /// Gets a value indicating the position of SCP-939's mimic point. May be <see langword="null"/> if <see cref="MimicryPointActive"/> is <see langword="false"/>.
        /// </summary>
        public UnityEngine.Vector3? MinicryPointPosition => SubroutineModule.TryGetSubroutine(out EnvironmentalMimicry ability) && ability._mimicPoint.Active ? ability._mimicPoint.MimicPointTransform.position : null;

        /// <summary>
        /// Gets a list of players this SCP-939 instance can see regardless of their movement.
        /// </summary>
        public List<Player> VisiblePlayers { get; } = new();

        /// <summary>
        /// Removes all recordings of player voices. Provide an optional target to remove all the recordings of a single player.
        /// </summary>
        /// <param name="target">If provided, will only remove recordings of the targeted player.</param>
        public void ClearRecordings(Player target = null)
        {
            if (!SubroutineModule.TryGetSubroutine(out MimicryRecorder ability))
                return;

            if (target is null)
            {
                ability.SavedVoices.Clear();
                ability._serverSentVoices.Clear();
            }
            else
            {
                ability.RemoveRecordingsOfPlayer(target.ReferenceHub);
            }

            ability.SavedVoicesModified = true;
        }
    }
}