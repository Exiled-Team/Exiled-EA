// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using Exiled.Events.EventArgs.Player;
using GameCore;
using PlayerRoles.Spectating;

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

            API.Features.Log.Debug($"\n\n\n\n\n\n\nWAHHHHHHHHHHHHHHHHHHHHHHHH\n\n: Loading events.");
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
            Exiled.API.Features.Log.Info($"ProcessNewPlayer 1 -- {new_netid}");
            if (!spec.TryGetOwner(out ReferenceHub owner))
            {
                return;
            }
            Exiled.API.Features.Log.Info("ProcessNewPlayer 2");
            API.Features.Player player =  API.Features.Player.Get(owner);
            Exiled.API.Features.Log.Info("ProcessNewPlayer 3");
            API.Features.Player previousSpectatedPlayer = API.Features.Player.Get(spec.SyncedSpectatedNetId);
            Exiled.API.Features.Log.Info("ProcessNewPlayer 4");
            API.Features.Player getFuturePlayer = API.Features.Player.Get(new_netid);
            Exiled.API.Features.Log.Info("ProcessNewPlayer 5");
            ChangingSpectatedPlayerEventArgs temp = new ChangingSpectatedPlayerEventArgs(player, previousSpectatedPlayer, getFuturePlayer, true);
            Exiled.API.Features.Log.Info("ProcessNewPlayer 6");
            Handlers.Player.OnChangingSpectatedPlayer(temp);
            Exiled.API.Features.Log.Info("ProcessNewPlayer 7");
            // if (!temp.IsAllowed)
            // {
            //     Exiled.API.Features.Log.Info("ProcessNewPlayer 8");
            //     return;
            // }

            // Exiled.API.Features.Log.Info("ProcessNewPlayer 9");
            // if (temp.NewTarget != null)
            // {
            //     Exiled.API.Features.Log.Info("ProcessNewPlayer 10");
            //     new_netid = (int) temp.NewTarget.NetworkIdentity.netId;
            // }
            // else
            // {
            //     Exiled.API.Features.Log.Info("ProcessNewPlayer 11");
            //     new_netid = (int) temp.Player.NetworkIdentity.netId;
            // }
        }
    }
}