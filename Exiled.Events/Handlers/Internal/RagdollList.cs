// -----------------------------------------------------------------------
// <copyright file="RagdollList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using Exiled.API.Features;

    /// <summary>
    /// Handles <see cref="Ragdoll"/> <see Ragdoll.BasicRagdollToRagdoll >.
    /// </summary>
    internal static class RagdollList
    {
        /// <summary>
        /// .
        /// </summary>
        /// <param name="ragdoll">e.</param>
        internal static void OnSpawnedRagdoll(BasicRagdoll ragdoll) => Ragdoll.Get(ragdoll);

        /// <summary>
        /// .
        /// </summary>
        /// <param name="ragdoll">e.</param>
        internal static void OnRemovedRagdoll(BasicRagdoll ragdoll) => Ragdoll.BasicRagdollToRagdoll.Remove(ragdoll);
    }
}
