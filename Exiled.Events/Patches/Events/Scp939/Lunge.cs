// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.Lunging" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.ServerProcessCmd))]
    internal static class Lunge
    {
        private static bool Prefix(Scp939LungeAbility __instance, NetworkReader reader)
        {
            if (__instance.State != Scp939LungeState.Triggered && !__instance.IsReady)
                return false;

            LungingEventArgs ev = new(API.Features.Player.Get(__instance.Owner));
            Scp939.OnLunging(ev);
            return ev.IsAllowed;
        }
    }
}
