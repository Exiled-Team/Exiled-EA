// -----------------------------------------------------------------------
// <copyright file="FirearmRequestReceived.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    using NorthwoodLib.Pools;
    using PluginAPI.Enums;
    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="FirearmBasicMessagesHandler.ServerRequestReceived" />.
    ///     Adds <see cref="Player.ReloadingWeapon" />, <see cref="Player.UnloadingWeapon" />,
    ///     <see cref="Player.DryfiringWeapon" />, <see cref="Player.AimingDownSight" /> and
    ///     <see cref="Player.TogglingWeaponFlashlight" /> events.
    /// </summary>
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerRequestReceived))]
    internal static class FirearmRequestReceived
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingWeaponFlashlightEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            int offset = 0;
            int index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == (sbyte)ServerEventType.PlayerReloadWeapon) + offset;

            Label returnLabel = generator.DefineLabel();
            Label skipAdsLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new ReloadingWeaponEventArgs(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReloadingWeaponEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnReloadingWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReloadingWeapon))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReloadingWeaponEventArgs), nameof(ReloadingWeaponEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            offset = 0;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == (sbyte)ServerEventType.PlayerUnloadWeapon) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new ReloadingWeaponEventArgs(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnloadingWeaponEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnUnloadingWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnUnloadingWeapon))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UnloadingWeaponEventArgs), nameof(UnloadingWeaponEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            offset = 0;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == (sbyte)ServerEventType.PlayerDryfireWeapon) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new DryfiringWeaponEventArgs(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DryfiringWeaponEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnDryfiringWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDryfiringWeapon))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DryfiringWeaponEventArgs), nameof(DryfiringWeaponEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            offset = 0;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == (sbyte)ServerEventType.PlayerAimWeapon) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // var player = Player.Get(referenceHub)
                    //
                    // if (player == null)
                    //    goto skipAdsLabel;
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, player.LocalIndex),
                    new(OpCodes.Brfalse_S, skipAdsLabel),

                    // player
                    new(OpCodes.Ldloc, player.LocalIndex),

                    // true (adsIn)
                    new(OpCodes.Ldc_I4_1),

                    // false (adsOut)
                    new(OpCodes.Ldc_I4_0),

                    // var ev = new AimingDownSightEventArgs(Player, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AimingDownSightEventArgs))[0]),

                    // Player.OnAimingDownSight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnAimingDownSight))),

                    // skipAdsLabel:
                    new CodeInstruction(OpCodes.Nop).WithLabels(skipAdsLabel),
                });

            offset = 0;
            index = newInstructions.FindLastIndex(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == (sbyte)ServerEventType.PlayerAimWeapon) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // var player = Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // false (adsIn)
                    new(OpCodes.Ldc_I4_0),

                    // true (adsOut)
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new AimingDownSightEventArgs(Player, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AimingDownSightEventArgs))[0]),

                    // Player.OnAimingDownSight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnAimingDownSight))),
                });

            offset = -7;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ceq) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // var player = Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // !flag
                    new(OpCodes.Ldloc_S, 6),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new AimingDownSightEventArgs(Player, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingWeaponFlashlightEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.OnTogglingWeaponFlashlight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnTogglingWeaponFlashlight))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // flag = !ev.NewState
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.NewState))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new(OpCodes.Stloc_S, 6),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}