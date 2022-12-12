// -----------------------------------------------------------------------
// <copyright file="FlippingCoin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Coin;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;
    using Utils.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches
    ///     <see cref="CoinNetworkHandler.ServerProcessMessage(NetworkConnection, CoinNetworkHandler.CoinFlipMessage)" />.
    ///     Adds the <see cref="Handlers.Player.FlippingCoin" /> event.
    /// </summary>
    [HarmonyPatch(typeof(CoinNetworkHandler), nameof(CoinNetworkHandler.ServerProcessMessage))]
    internal static class FlippingCoin
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(FlippingCoinEventArgs));

            const int instructionsToRemove = 10;
            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(ReferenceHub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // IsTails = Random.value >= 0.5f
                    new(OpCodes.Call, PropertyGetter(typeof(Random), nameof(Random.value))),
                    new(OpCodes.Ldc_R4, 0.5f),
                    new(OpCodes.Clt_Un),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = FlippingCoinEventArgs(Player, IsTails, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FlippingCoinEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // OnFlippingCoin(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFlippingCoin))),

                    // if (ev.IsAllowed)
                    //   return
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // coin.SerialNumber
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.ItemSerial))),

                    // ev.IsTails
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsTails))),

                    // var coinFlipMessage = new CoinFlipMessage(SerialNumber, IsTails)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CoinNetworkHandler.CoinFlipMessage))[0]),

                    // 0
                    new(OpCodes.Ldc_I4_0),

                    // NetworkUtils.SendToAuthenticated<CoinFlipMessage>(coinFlipMessage, 0)
                    new(OpCodes.Call, Method(typeof(NetworkUtils), nameof(NetworkUtils.SendToAuthenticated), null, new[] { typeof(CoinNetworkHandler.CoinFlipMessage) })),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}