// -----------------------------------------------------------------------
// <copyright file="RoomBlackout.cs" company="Exiled Team">
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
    ///     Patches <see cref="Scp079BlackoutRoomAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="RoomBlackoutEventArgs" /> event for SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility.ServerProcessCmd))]
    internal static class RoomBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(RoomBlackoutEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(Scp079AbilityBase), nameof(Scp079AbilityBase.LostSignalHandler)))) + offset;
            newInstructions.RemoveRange(index, 5);

            // this._syncPos = ev.Position;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),

                    // this._roomController.Room
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._roomController))),
                    new(OpCodes.Call, PropertyGetter(typeof(FlickerableLightController), nameof(FlickerableLightController.Room))),

                    // this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cost))),

                    // this._blackoutDuration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._blackoutDuration))),

                    // this._cooldown
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cooldown))),

                    // this.LostSignalHandler.Lost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(Scp079AbilityBase), nameof(Scp079AbilityBase.LostSignalHandler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.Lost))),

                    // ZoneBlackoutEventArgs ev = new(Player player, this._roomController.Room, this._cost, this._blackoutDuration, this._cooldown, this.LostSignalHandler.Lost);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoomBlackoutEventArgs))[0]),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnRoomBlackout(ev);
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnRoomBlackout))),

                    // if(!ev.IsAllowed) return;
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // Replace "(float)this._cost" with "ev.AuxiliaryPowerCost"
            offset = -1;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cost)))) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.AuxiliaryPowerCost))),
               });

            // Replace "this._blackoutDuration" with "ev.BlackoutDuration"
            offset = -1;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._blackoutDuration)))) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.BlackoutDuration))),
               });

            // Replace "(double)this._cooldown" with "ev.Cooldown"
            offset = -1;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cooldown)))) + offset;

            newInstructions.RemoveRange(index, 1);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.Cooldown))),
               });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}