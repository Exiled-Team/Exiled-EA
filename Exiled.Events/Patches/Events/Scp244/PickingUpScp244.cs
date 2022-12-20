// -----------------------------------------------------------------------
// <copyright file="PickingUpScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp244;
    using Handlers;

    using HarmonyLib;

    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp244SearchCompletor" /> to add missing event handler to the
    ///     <see cref="Scp244SearchCompletor" />.
    /// </summary>
    [HarmonyPatch(typeof(Scp244SearchCompletor), nameof(Scp244SearchCompletor.Complete))]
    internal static class PickingUpScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(Scp244SearchCompletor), nameof(Scp244SearchCompletor.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // scp244DeployablePickup
                    new(OpCodes.Ldloc_0),

                    // PickingUpScp244EventArgs ev = new(Player, Scp244DeployablePickup)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpScp244EventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp244.OnPickingUpScp244(ev)
                    new(OpCodes.Call, Method(typeof(Scp244), nameof(Scp244.OnPickingUpScp244))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpScp244EventArgs), nameof(PickingUpScp244EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}