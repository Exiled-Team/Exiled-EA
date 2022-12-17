// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using UnityEngine;

    using BaseTeslaGate = TeslaGate;

    /// <summary>
    ///     Patches <see cref="TeslaGateController.FixedUpdate" />.
    ///     Adds the <see cref="Handlers.Player.TriggeringTesla" /> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {
        private static bool Prefix(TeslaGateController __instance)
        {
            try
            {
                if (!Round.IsStarted)
                    return false;

                if (TeslaGate.BaseTeslaGateToTeslaGate.Count == 0)
                    return true;
                foreach (BaseTeslaGate baseTeslaGate in __instance.TeslaGates)
                {
                    if (!baseTeslaGate.isActiveAndEnabled || baseTeslaGate.InProgress)
                        continue;

                    if (baseTeslaGate.NetworkInactiveTime > 0f)
                    {
                        baseTeslaGate.NetworkInactiveTime = Mathf.Max(0f, baseTeslaGate.InactiveTime - Time.fixedDeltaTime);
                        continue;
                    }

                    TeslaGate teslaGate = TeslaGate.Get(baseTeslaGate);
                    bool inIdleRange = false;
                    bool isTriggerable = false;

                    foreach (Player player in Player.List)
                    {
                        try
                        {
                            if (player is null || !teslaGate.CanBeIdle(player))
                                continue;

                            TriggeringTeslaEventArgs ev = new(player, teslaGate);

                            Handlers.Player.OnTriggeringTesla(ev);

                            if (ev.IsAllowed && !isTriggerable)
                                isTriggerable = ev.IsAllowed;

                            if (ev.IsInIdleRange && !inIdleRange)
                                inIdleRange = ev.IsInIdleRange;
                        }
#pragma warning disable CS0168
                        catch (Exception exception)
#pragma warning restore CS0168
                        {
#if DEBUG
                            Log.Error($"{nameof(TriggeringTesla)}.Prefix: {exception}");
#endif
                        }
                    }

                    if (isTriggerable)
                        teslaGate.Trigger();

                    if (inIdleRange != teslaGate.IsIdling)
                        teslaGate.IsIdling = inIdleRange;
                }

                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.TriggeringTesla: {exception}\n{exception.StackTrace}");
                return true;
            }
        }
    }
}
