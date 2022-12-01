// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp173;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.UpdateObserver(ReferenceHub)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObservers), new[] { typeof(ReferenceHub) })]
    internal static class Scp173BeingLooked
    {
        private static bool Prefix(Scp173ObserversTracker __instance, ReferenceHub targetHub, ref int __return)
        {
            if (Player.Get(targetHub) is Player player &&
                player.Role.Type == RoleTypeId.Tutorial &&
                !ExiledEvents.Instance.Config.CanTutorialBlockScp173 &&
                __instance.IsObservedBy(targetHub, 0.2f))
            {
                __return = __instance.Observers.Remove(targetHub) ? -1 : 0;
                return false;
            }

            return true;
        }
    }
}