// -----------------------------------------------------------------------
// <copyright file="DoorBeepType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Door beep types.
    /// </summary>
    public enum DoorBeepType
    {
        /// <summary>
        /// Permission denied beep.
        /// </summary>
        PermissionDenied,

        /// <summary>
        /// Lock bypass is denied.
        /// </summary>
        LockBypassDenied,

        /// <summary>
        /// Interaction denied.
        /// </summary>
        InteractionDenied,

        /// <summary>
        /// Interaction allowed.
        /// </summary>
        InteractionAllowed,
    }
}