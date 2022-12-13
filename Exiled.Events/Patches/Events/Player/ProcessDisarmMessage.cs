// -----------------------------------------------------------------------
// <copyright file="ProcessDisarmMessage.cs" company="Exiled Team">
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

    using InventorySystem.Disarming;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="DisarmingHandlers.ServerProcessDisarmMessage" />.
    ///     Adds the <see cref="Handlers.Player.Handcuffing" /> and <see cref="Handlers.Player.RemovingHandcuffs" /> events.
    /// </summary>
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class ProcessDisarmMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -4;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(DisarmedPlayers), nameof(DisarmedPlayers.SetDisarmedStatus)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(msg.PlayerToDisarm)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingHandcuffsEventArgs ev = new(Player, Player, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnRemovingHandcuffs(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingHandcuffs))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = -5;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(DisarmedPlayers), nameof(DisarmedPlayers.SetDisarmedStatus)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(msg.PlayerToDisarm)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // HandcuffingEventArgs evHandcuffing = new(Player, Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(HandcuffingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnHandcuffing(evHandcuffing)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHandcuffing))),

                    // if (!evHandcuffing.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HandcuffingEventArgs), nameof(HandcuffingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}