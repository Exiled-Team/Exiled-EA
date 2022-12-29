// -----------------------------------------------------------------------
// <copyright file="Pinging.cs" company="Exiled Team">
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
    ///     Patches <see cref="Scp079PingAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="PingingEventArgs" /> event for SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PingAbility), nameof(Scp079PingAbility.ServerProcessCmd))]
    internal static class Pinging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PingingEventArgs));

            int offset = -12;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(ScpSubroutineBase), nameof(ScpSubroutineBase.ServerSendRpc)))) + offset;

            // this._syncPos = ev.Position;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // referenceHub
                    new(OpCodes.Ldloca_S, 0),

                    // reader.ReadRelativePosition()
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(RelativePositionSerialization), nameof(RelativePositionSerialization.ReadRelativePosition))),

                    // this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._cost))),

                    // this._syncProcessorIndex
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncProcessorIndex))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // Scp079PingingEventArgs ev = new(referenceHub, reader.ReadRelativePosition(), this._cost, this._syncProcessorIndex, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PingingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp079.Scp079Pinging(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnPinging))),

                    // if(!ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // this._syncNormal = ev.Position;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.Position))),
                    new(OpCodes.Stfld, Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._syncNormal))),
                });

            // Replace "this._syncPos.Position" with "ev.Position"
            offset = -2;
            index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(RelativePositionSerialization), nameof(RelativePositionSerialization.ReadRelativePosition)))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.AuxiliaryPowerCost))),
               });

            // Replace "(float)this._cost" with "ev.AuxiliaryPowerCost"
            offset = 0;
            index = newInstructions.FindLastIndex(
            instruction => instruction.LoadsField(Field(typeof(Scp079PingAbility), nameof(Scp079PingAbility._cost)))) + offset;

            newInstructions.RemoveRange(index, 1);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PingingEventArgs), nameof(PingingEventArgs.AuxiliaryPowerCost))),
               });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}