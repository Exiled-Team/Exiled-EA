// -----------------------------------------------------------------------
// <copyright file="FlashbangGrenadeFieldsFix.cs" company="Exiled Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.PlayExplosionEffects"/> to sync it's fields with <see cref="FlashGrenade"/> properties.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    internal static class FlashbangGrenadeFieldsFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;
            Label skipLabel = generator.DefineLabel();
            LocalBuilder flash = generator.DeclareLocal(typeof(FlashGrenade));

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (!FlashGrenade.GrenadeToItem.TryGetValue(this, out FlashGrenade flash)
                    //     goto SKIP_LABEL
                    new(OpCodes.Call, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.GrenadeToItem))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloca_S, flash.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<FlashbangGrenade, FlashGrenade>), nameof(Dictionary<FlashbangGrenade, FlashGrenade>.TryGetValue))),
                    new(OpCodes.Brfalse, skipLabel),

                    // this._blindingOverDistance = flash.BlindCurve;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc, flash.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.BlindCurve))),
                    new(OpCodes.Stfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._blindingOverDistance))),

                    // this._deafenDurationOverDistance = flash.DeafenCurve;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc, flash.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.DeafenCurve))),
                    new(OpCodes.Stfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._deafenDurationOverDistance))),

                    // this._surfaceZoneDistanceIntensifier = flash.SurfaceDistanceIntensifier;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc, flash.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.SurfaceDistanceIntensifier))),
                    new(OpCodes.Stfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._surfaceZoneDistanceIntensifier))),

                    // SKIP_LABEL
                    new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}