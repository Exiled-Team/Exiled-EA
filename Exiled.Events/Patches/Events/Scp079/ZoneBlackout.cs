// -----------------------------------------------------------------------
// <copyright file="ZoneBlackout.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp079;

    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Pinging;
    using PlayerRoles.PlayableScps.Subroutines;
    using RelativePositioning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079BlackoutZoneAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="ZoneBlackoutEventArgs" /> event for  SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility.ServerProcessCmd))]
    internal static class ZoneBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ZoneBlackoutEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._syncZone)))) + offset;
            newInstructions.RemoveRange(index, 5);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // referenceHub
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),

                    // this._syncZone
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._syncZone))),

                    // this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cost))),

                    // this.ErrorCode
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility.ErrorCode))),

                    // this._duration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._duration))),

                    // this._cooldown
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cooldown))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ZoneBlackoutEventArgs ev = new(referenceHub, this._syncZone, this._cost, this._duration, this._cooldown, this.ErrorCode, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ZoneBlackoutEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp079.OnZoneBlackout(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnZoneBlackout))),

                    // if(!ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // Replace "this._duration" with "ev.BlackoutDuration"
            offset = 0;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._duration)))) + offset;

            newInstructions.RemoveRange(index, 1);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.BlackoutDuration))),
               });

            // Replace "this._cooldown" with "ev.Cooldown"
            offset = 0;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cooldown)))) + offset;

            newInstructions.RemoveRange(index, 1);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.Cooldown))),
               });

            // Replace "(float)this._cost" with "ev.AuxiliaryPowerCost"
            offset = 0;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cost)))) + offset;

            newInstructions.RemoveRange(index, 1);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.AuxiliaryPowerCost))),
               });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}