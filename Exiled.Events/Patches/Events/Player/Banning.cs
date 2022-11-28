// -----------------------------------------------------------------------
// <copyright file="Banning.cs" company="Exiled Team">
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
    ///     Patches <see cref="BanPlayer.BanUser(ReferenceHub, ReferenceHub, string, long)" />.
    ///     Adds the <see cref="Handlers.Player.Banning" /> event.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(ReferenceHub), typeof(ReferenceHub), typeof(string), typeof(long))]
    internal static class Banning
    {
        private static bool Prefix(ReferenceHub target, ReferenceHub issuer, string reason, long duration)
        {
            try
            {
                if (duration == 0L)
                    return BanPlayer.KickUser(target, issuer, reason);

                if (duration > long.MaxValue)
                    duration = long.MaxValue;

                if (target.serverRoles.BypassStaff)
                    return false;

                long issuanceTime = TimeBehaviour.CurrentTimestamp();
                long banExpirationTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
                string originalName = BanPlayer.ValidateNick(target.nicknameSync.MyNick);
                string message = $"You have been banned. {(!string.IsNullOrEmpty(reason) ? "Reason: " + reason : string.Empty)}";

                BanningEventArgs ev = new(Player.Get(target), Player.Get(issuer), duration, reason, message);

                Handlers.Player.OnBanning(ev);

                if (!ev.IsAllowed)
                    return false;

                duration = ev.Duration;
                reason = ev.Reason;
                message = ev.FullMessage;

                if (!EventManager.ExecuteEvent(ServerEventType.PlayerBanned, target, issuer, reason, duration))
                    return false;

                BanPlayer.ApplyIpBan(target, issuer, reason, duration);

                BanHandler.IssueBan(
                    new BanDetails
                {
                    OriginalName = originalName,
                    Id = target.characterClassManager.UserId,
                    IssuanceTime = issuanceTime,
                    Expires = banExpirationTime,
                    Reason = reason,
                    Issuer = issuer.isServer ? BanPlayer._serverId : (issuer.characterClassManager.UserId ?? issuer.connectionToClient.address),
                }, BanHandler.BanType.UserId);

                if (!string.IsNullOrEmpty(target.characterClassManager.UserId2))
                {
                    BanHandler.IssueBan(
                        new BanDetails
                    {
                        OriginalName = originalName,
                        Id = target.characterClassManager.UserId2,
                        IssuanceTime = issuanceTime,
                        Expires = banExpirationTime,
                        Reason = reason,
                        Issuer = issuer.isServer ? BanPlayer._serverId : (issuer.characterClassManager.UserId ?? issuer.connectionToClient.address),
                    }, BanHandler.BanType.UserId);
                }

                ServerConsole.Disconnect(target.gameObject, message);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.Banning: {exception}\n{exception.StackTrace}");

                return true;
            }
        }
    }
}