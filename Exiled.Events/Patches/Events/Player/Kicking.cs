// -----------------------------------------------------------------------
// <copyright file="Kicking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PluginAPI.Enums;
    using PluginAPI.Events;

    using Log = API.Features.Log;

    /// <summary>
    ///     Patches <see cref="BanPlayer.KickUser(ReferenceHub, ReferenceHub, string)" />.
    ///     Adds the <see cref="Handlers.Player.Kicking" /> event.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), typeof(ReferenceHub), typeof(ReferenceHub), typeof(string))]
    internal static class Kicking
    {
        private static bool Prefix(ReferenceHub target, ReferenceHub issuer, string reason)
        {
            try
            {
                string message = $"You have been kicked. {(!string.IsNullOrEmpty(reason) ? "Reason: " + reason : string.Empty)}";

                KickingEventArgs ev = new(Player.Get(target), Player.Get(issuer), reason, message);

                Handlers.Player.OnKicking(ev);

                if (!ev.IsAllowed)
                    return false;

                reason = ev.Reason;
                message = ev.FullMessage;

                if (!EventManager.ExecuteEvent(ServerEventType.PlayerKicked, target, issuer, reason))
                    return false;

                ServerConsole.Disconnect(target.gameObject, message);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.Kicking: {exception}\n{exception.StackTrace}");

                return true;
            }
        }
    }
}