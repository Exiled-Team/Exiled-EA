// -----------------------------------------------------------------------
// <copyright file="BreakingScp2176.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.Events.EventArgs.Map;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Map = Exiled.Events.Handlers.Map;

    /// <summary>
    ///     Patches <see cref="Scp2176Projectile.ServerShatter" />.
    ///     Supplements the <see cref="Handlers.Map.ExplodingGrenade" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerShatter))]
    internal static class BreakingScp2176
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The return label
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // new ExplodingGrenadeEventArgs(Player.Get(PreviousOwner.Hub), this, Array.Empty<Collider>());
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldflda, Field(typeof(Scp2176Projectile), nameof(Scp2176Projectile.PreviousOwner))),
                    new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Newarr, typeof(Collider)),
                    new(OpCodes.Newobj, DeclaredConstructor(typeof(ExplodingGrenadeEventArgs), new[] { typeof(Player), typeof(EffectGrenade), typeof(Collider[]) })),
                    new(OpCodes.Dup),

                    // Handlers.Map.OnExplodingGrenade(ev);
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnExplodingGrenade))),

                    // if(!ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}