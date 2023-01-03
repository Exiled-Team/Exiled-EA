// -----------------------------------------------------------------------
// <copyright file="ZoneBlackout.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079BlackoutZoneAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="ZoneBlackoutEventArgs" /> event for  SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility.ServerProcessCmd))]
    internal static class ZoneBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 4;
            int index = newInstructions.FindIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._syncZone)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Remove NW answer
                    new(OpCodes.Pop),

                    // Pass Scp079BlackoutZoneAbility instance
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(ZoneBlackout), nameof(ProcessZoneBlackout))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool ProcessZoneBlackout(Scp079BlackoutZoneAbility instance)
        {
            API.Features.Player currentPlayer = API.Features.Player.Get(instance.Owner);
            
            ZoneBlackoutEventArgs ev = new(currentPlayer, instance._syncZone, instance._cost, instance._duration, instance._cooldown, instance.ErrorCode);
            
            Handlers.Scp079.OnZoneBlackout(ev);
            
            if (ev.IsAllowed)
            {
                instance._duration = ev.BlackoutDuration;
                instance._cooldown = ev.Cooldown;

                // Gets casted to float above, even though it is an int, joy.
                instance._cost = (int)ev.AuxiliaryPowerCost;
            }

            return ev.IsAllowed;
        }
    }
}