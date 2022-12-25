// -----------------------------------------------------------------------
// <copyright file="PlacingBulletHole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Modules;

    using NorthwoodLib.Pools;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="StandardHitregBase.PlaceBulletholeDecal" />.
    ///     Adds the <see cref="Map.PlacingBulletHole" /> event.
    /// </summary>
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal static class PlacingBulletHole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EventArgs.Map.PlacingBulletHole));
            LocalBuilder rotation = generator.DeclareLocal(typeof(Quaternion));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // PlacingBulletHole ev = new(Player, RaycastHit)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EventArgs.Map.PlacingBulletHole))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Map.OnPlacingBulletHole(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnPlacingBulletHole))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EventArgs.Map.PlacingBulletHole), nameof(EventArgs.Map.PlacingBulletHole.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // hit.info = ev.Position
                    new(OpCodes.Ldarga_S, 2),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EventArgs.Map.PlacingBulletHole), nameof(EventArgs.Map.PlacingBulletHole.Position))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(RaycastHit), nameof(RaycastHit.point))),

                    // hit.normal = ev.Rotation
                    new(OpCodes.Ldarga_S, 2),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EventArgs.Map.PlacingBulletHole), nameof(EventArgs.Map.PlacingBulletHole.Rotation))),
                    new(OpCodes.Stloc_S, rotation),
                    new(OpCodes.Ldloca_S, rotation),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Quaternion), nameof(Quaternion.eulerAngles))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(RaycastHit), nameof(RaycastHit.normal))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}