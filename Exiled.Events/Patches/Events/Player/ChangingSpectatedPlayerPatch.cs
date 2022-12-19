// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using Exiled.Events.EventArgs.Player;
using GameCore;
using PlayerRoles.Spectating;
using Log = Exiled.API.Features.Log;

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;


    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SpectatorRole.SyncedSpectatedNetId" /> setter.
    ///     Adds the <see cref="Player.ChangingSpectatedPlayer" />.
    /// </summary>
    [HarmonyPatch(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId), MethodType.Setter)]
    internal static class ChangingSpectatedPlayerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarga_S, 1),
                    new(OpCodes.Call, Method(typeof(ChangingSpectatedPlayerPatch), nameof(processNewPlayer), new []{ typeof(SpectatorRole), typeof(uint).MakeByRefType()})),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static void processNewPlayer(SpectatorRole spec, ref int new_netid)
        {
            if (new_netid == 0)
            {
                return;
            }
            if (!spec.TryGetOwner(out ReferenceHub owner))
            {
                return;
            }
            API.Features.Player player =  API.Features.Player.Get(owner);
            API.Features.Player previousSpectatedPlayer = API.Features.Player.Get(spec.SyncedSpectatedNetId);
            API.Features.Player getFuturePlayer = API.Features.Player.Get(new_netid);
            ChangingSpectatedPlayerEventArgs temp = new ChangingSpectatedPlayerEventArgs(player, previousSpectatedPlayer, getFuturePlayer, true);
            Handlers.Player.OnChangingSpectatedPlayer(temp);
            //NOT RECOMMENDED PER IRACLE AND I BELIEVING CLIENT SIDE PICKS TARGET.
            if (!temp.IsAllowed)
            {
                return;
            }

            if (temp.NewTarget != null)
            {
                new_netid = (int) temp.NewTarget.NetworkIdentity.netId;
            }
            else
            {
                new_netid = (int) temp.Player.NetworkIdentity.netId;
            }
        }
    }
}