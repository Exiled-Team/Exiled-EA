// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Warhead;
    using Extensions;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;

    using static Events;

    /// <summary>
    ///     Handles warhead related events.
    /// </summary>
    public class Warhead
    {
        /// <summary>
        ///     Invoked before stopping the warhead.
        /// </summary>
        public static event CustomEventHandler<StoppingWarheadEventArgs> StoppingWarhead;

        /// <summary>
        ///     Invoked before starting the warhead.
        /// </summary>
        public static event CustomEventHandler<StartingWarheadEventArgs> StartingWarhead;

        /// <summary>
        ///     Invoked before changing the warhead lever status.
        /// </summary>
        public static event CustomEventHandler<ChangingLeverStatusEventArgs> ChangingLeverStatus;

        /// <summary>
        ///     Invoked after the warhead has been detonated.
        /// </summary>
        public static event CustomEventHandler DetonatedWarhead;

        /// <summary>
        ///     Invoked before detonating the warhead.
        /// </summary>
        public static event CustomEventHandler<DetonatingWarheadEventArgs> Detonating;

        /// <summary>
        ///     Called before stopping the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingWarheadEventArgs" /> instance.</param>
        public static void OnStoppingWarhead(StoppingWarheadEventArgs ev) => StoppingWarhead.InvokeSafely(ev);

        /// <summary>
        ///     Called before starting the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StartingWarheadEventArgs" /> instance.</param>
        public static void OnStartingWarhead(StartingWarheadEventArgs ev) => StartingWarhead.InvokeSafely(ev);

        /// <summary>
        ///     Called before changing the warhead lever status.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingLeverStatusEventArgs" /> instance.</param>
        public static void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev) => ChangingLeverStatus.InvokeSafely(ev);

        /// <summary>
        ///     Called after the warhead has been detonated.
        /// </summary>
        public static void OnDetonatedWarhead() => DetonatedWarhead.InvokeSafely();

        /// <summary>
        ///     Called before detonating the warhead.
        /// </summary>
        /// <returns>Returns whether the event is allowed or not.</returns>
        [PluginEvent(ServerEventType.WarheadDetonation)]
        public bool OnDetonatingWarhead()
        {
            DetonatingWarheadEventArgs ev = new();

            Detonating.InvokeSafely(ev);

            return ev.IsAllowed;
        }
    }
}