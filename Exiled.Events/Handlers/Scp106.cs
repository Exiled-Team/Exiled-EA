// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp106;
    using Extensions;

    using static Events;

    /// <summary>
    ///     SCP-106 related events.
    /// </summary>
    public static class Scp106
    {
        /// <summary>
        ///     Invoked before SCP-106 creates a portal.
        /// </summary>
        public static event CustomEventHandler<CreatingPortalEventArgs> CreatingPortal;

        /// <summary>
        ///     Invoked before SCP-106 teleports using a portal.
        /// </summary>
        public static event CustomEventHandler<TeleportingEventArgs> Teleporting;

        /// <summary>
        ///     Called before SCP-106 creates a portal.
        /// </summary>
        /// <param name="ev">The <see cref="CreatingPortalEventArgs" /> instance.</param>
        public static void OnCreatingPortal(CreatingPortalEventArgs ev) => CreatingPortal.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-106 teleports using a portal.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs" /> instance.</param>
        public static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);
    }
}