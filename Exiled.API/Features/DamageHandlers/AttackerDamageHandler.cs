// -----------------------------------------------------------------------
// <copyright file="AttackerDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    public abstract class AttackerDamageHandler : DamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackerDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="attacker">The attacker to be set.</param>
        protected AttackerDamageHandler(Player target, Player attacker)
            : base(target, attacker)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackerDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="baseHandler"><inheritdoc cref="DamageHandlerBase.Base"/></param>
        protected AttackerDamageHandler(Player target, BaseHandler baseHandler)
            : base(target, baseHandler)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the friendly fire should be forced.
        /// </summary>
        public bool ForceFullFriendlyFire
        {
            get => Is(out PlayerStatsSystem.AttackerDamageHandler handler) && handler.ForceFullFriendlyFire;
            set
            {
                if (Is(out PlayerStatsSystem.AttackerDamageHandler handler))
                    handler.ForceFullFriendlyFire = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the action is due to suicide.
        /// </summary>
        public bool IsSuicide
        {
            get => Is(out PlayerStatsSystem.AttackerDamageHandler handler) && handler.IsSuicide;
            set
            {
                if (Is(out PlayerStatsSystem.AttackerDamageHandler handler))
                    handler.IsSuicide = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the self damage is allowed.
        /// </summary>
        public bool AllowSelfDamage
        {
            get => Is(out PlayerStatsSystem.AttackerDamageHandler handler) && handler.AllowSelfDamage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the damage is friendly fire.
        /// </summary>
        public bool IsFriendlyFire
        {
            get => Is(out PlayerStatsSystem.AttackerDamageHandler handler) && handler.IsFriendlyFire;
            set
            {
                if (Is(out PlayerStatsSystem.AttackerDamageHandler handler))
                    handler.IsFriendlyFire = value;
            }
        }

        /// <summary>
        /// Computes and processes the damage.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to damage.</param>
        public override void ProcessDamage(Player player)
        {
            if (!Is(out PlayerStatsSystem.AttackerDamageHandler _))
                return;

            /*
            if ((player.IsSpawnProtected && (player != Attacker)) ||
                (!PlayerStatsSystem.AttackerDamageHandler._allowSpawnProtectedDamage &&
                 Attacker is not null && Attacker.IsSpawnProtected))
            {
                Damage = 0f;
                return;
            }
            */

            if ((player != Attacker) && !ForceFullFriendlyFire)
            {
                if (HitboxIdentity.CheckFriendlyFire(Attacker.Role, player.Role, true))
                    return;

                Damage *= PlayerStatsSystem.AttackerDamageHandler._ffMultiplier;
                IsFriendlyFire = true;
            }
            else
            {
                IsSuicide = AllowSelfDamage || ForceFullFriendlyFire;

                if (IsSuicide)
                    return;

                Damage = 0f;
            }
        }
    }
}