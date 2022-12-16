// -----------------------------------------------------------------------
// <copyright file="GrenadeFuseTimeFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Items;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableItem"/> to fix fuse times being unchangeable.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow), typeof(float), typeof(float), typeof(Vector3), typeof(Vector3))]
    internal static class GrenadeFuseTimeFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label notExplosiveLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            LocalBuilder timeGrenade = generator.DeclareLocal(typeof(TimeGrenade));
            LocalBuilder explosiveGrenade = generator.DeclareLocal(typeof(ExplosiveGrenade));
            LocalBuilder flashGrenade = generator.DeclareLocal(typeof(FlashGrenade));
            LocalBuilder item = generator.DeclareLocal(typeof(Item));

            const int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (thrownProjectils is not TimeGrenade timeGrenade)
                    //    goto skipLabel;
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Isinst, typeof(TimeGrenade)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Brfalse, skipLabel),

                    // Item item = Item.Get(this);
                    //
                    // if (item == null)
                    //    goto skipLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get))),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, item.LocalIndex),
                    new(OpCodes.Brfalse, skipLabel),

                    // if (item is not ExplosiveGrenade explosiveGrenade)
                    //    goto notExplosiveLabel;
                    new(OpCodes.Ldloc_S, item.LocalIndex),
                    new(OpCodes.Isinst, typeof(ExplosiveGrenade)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, explosiveGrenade.LocalIndex),
                    new(OpCodes.Brfalse, notExplosiveLabel),

                    // timeGrenade._fuseTime = explosiveGrenade.FuseTime
                    new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Ldloc_S, explosiveGrenade.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.FuseTime))),
                    new(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),

                    // ExplosiveGrenade.GrenadeToItem.Add(timeGrenade, explosiveGrenade)
                    new(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GrenadeToItem))),
                    new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Isinst, typeof(ExplosionGrenade)),
                    new(OpCodes.Ldloc_S, explosiveGrenade.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosionGrenade, ExplosiveGrenade>.Add))),

                    // timeGrenade.ServerActivate()
                    // return
                    new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                    new(OpCodes.Ret),

                    // if (item is FlashGrenade flashGrenade)
                    //    goto skipLabel;
                    new CodeInstruction(OpCodes.Ldloc_S, item.LocalIndex).WithLabels(notExplosiveLabel),
                    new(OpCodes.Isinst, typeof(FlashGrenade)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, flashGrenade.LocalIndex),
                    new(OpCodes.Brfalse, skipLabel),

                    // timeGrenade._fuseTime = flashGrenade.FuseTime
                    new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Ldloc_S, flashGrenade.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.FuseTime))),
                    new(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),

                    // FlashGrenade.GrenadeToItem.Add(timeGrenade, flashGrenade)
                    new(OpCodes.Call, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.GrenadeToItem))),
                    new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Isinst, typeof(FlashbangGrenade)),
                    new(OpCodes.Ldloc_S, flashGrenade.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<FlashbangGrenade, FlashGrenade>), nameof(Dictionary<FlashbangGrenade, FlashGrenade>.Add))),

                    // timeGrenade.ServerActivate();
                    //    return;
                    new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                    new(OpCodes.Ret),

                    // skipLabel:
                    //
                    // skips all of the above code, and runs base-game serverActivate instead.
                    new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}