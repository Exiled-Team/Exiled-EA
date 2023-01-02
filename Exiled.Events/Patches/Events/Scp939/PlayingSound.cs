// -----------------------------------------------------------------------
// <copyright file="PlayingSound.cs" company="Exiled Team">
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
    using PlayerRoles.PlayableScps.Scp939.Mimicry;

    /// <summary>
    ///     Patches <see cref="Scp939AmnesticCloudAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.PlayingSound" /> event.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.ServerProcessCmd))]
    internal static class PlayingSound
    {
        private static bool Prefix(EnvironmentalMimicry __instance, NetworkReader reader)
        {
            byte category = reader.ReadByte();
            byte option = reader.ReadByte();

            EnvMimicryOption sound = __instance.Categories[category].Options[option];

            PlayingSoundEventArgs ev = new(API.Features.Player.Get(__instance.Owner), sound, __instance.Cooldown.IsReady, __instance._activationCooldown, __instance.Cooldown.IsReady);

            if (ev.IsReady && ev.IsAllowed)
            {
                __instance._syncCat = category;
                __instance._syncSound = option;
                __instance.Cooldown.Trigger(ev.Cooldown);
                __instance.ServerSendRpc(toAll: true);
            }

            return false;
        }
    }
}
