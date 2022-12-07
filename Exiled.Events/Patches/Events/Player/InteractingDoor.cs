// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1005
#pragma warning disable SA1515
#pragma warning disable SA1513
#pragma warning disable SA1512
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;
    using PlayerRoles;

    /// <summary>
    ///     Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    ///     Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                InteractingDoorEventArgs ev = new(Player.Get(ply), __instance, false);

                bool bypassDenied = false;
                bool allowInteracting = false;

                if (__instance.ActiveLocks > 0 && !ply.serverRoles.BypassMode)
                {
                    DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)__instance.ActiveLocks);
                    if ((!mode.HasFlagFast(DoorLockMode.CanClose) || !mode.HasFlagFast(DoorLockMode.CanOpen)) &&
                        (!mode.HasFlagFast(DoorLockMode.ScpOverride) || !ply.IsSCP(true)) &&
                        (mode == DoorLockMode.FullLock || (__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanClose)) ||
                        (!__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                    {
                        //>EXILED
                        ev.IsAllowed = false;
                        bypassDenied = true;
                        //<EXILED
                    }
                }

                if (!bypassDenied && (allowInteracting = __instance.AllowInteracting(ply, colliderId)))
                {
                    if (ply.GetRoleId() == RoleTypeId.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.CurInstance, ply))
                    {
                        //>EXILED
                        ev.IsAllowed = true;
                        //<EXILED
                    }
                    else
                    {
                        //>EXILED
                        ev.IsAllowed = false;
                        //<EXILED
                    }
                }

                //>EXILED
                Handlers.Player.OnInteractingDoor(ev);

                if (ev.IsAllowed && allowInteracting)
                {
                    __instance.NetworkTargetState = !__instance.TargetState;
                    __instance._triggerPlayer = ply;
                }
                else if (bypassDenied)
                {
                    __instance.LockBypassDenied(ply, colliderId);
                }
                // Don't call the RPC if the door is still moving
                else if (allowInteracting)
                {
                    // To avoid breaking their API, call the access denied event
                    // when our event prevents the door from opening
                    __instance.PermissionsDenied(ply, colliderId);
                    DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
                }
                //<EXILED

                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(InteractingDoor).FullName}.{nameof(Prefix)}:\n{exception}");
                return true;
            }
        }
    }
}
