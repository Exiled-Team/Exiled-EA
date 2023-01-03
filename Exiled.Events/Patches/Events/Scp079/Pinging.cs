// -----------------------------------------------------------------------
// <copyright file="Pinging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079.Pinging;
    using PlayerRoles.PlayableScps.Subroutines;
    using RelativePositioning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079PingAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="PingingEventArgs" /> event for  SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PingAbility), nameof(Scp079PingAbility.ServerProcessCmd))]
    internal static class Pinging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = 0;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(RelativePositionSerialization), nameof(RelativePositionSerialization.ReadRelativePosition)))) + offset;

            // this._syncPos = ev.Position;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // new(OpCodes.Ldarg_0, 0),
                    // new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Pinging), nameof(Pinging.ProcessPinging))),
                    new(OpCodes.Ret),

                    //To satisfy harmony but I should never get here.
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void ProcessPinging(Scp079PingAbility instance, NetworkReader reader)
        {
            RelativePosition curRelativePos = reader.ReadRelativePosition();

            PingingEventArgs ev = new PingingEventArgs(instance.Owner, curRelativePos, instance._cost, instance._syncProcessorIndex);

            Handlers.Scp079.OnPinging(ev);
           
             if (ev.IsAllowed)
            {
                instance._syncNormal = ev.Position;
                instance._syncPos = curRelativePos;
                instance.ServerSendRpc(hub => instance.ServerCheckReceiver(hub, ev.Position, (int)ev.Type));
                instance.AuxManager.CurrentAux = ev.AuxiliaryPowerCost;
                instance._rateLimiter.RegisterInput();
            }
        }
    }
}