// -----------------------------------------------------------------------
// <copyright file="TriggeringDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079DoorStateChanger.ServerProcessCmd" />.
    ///     Adds the <see cref="TriggeringDoorEventArgs" /> event for  SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079DoorStateChanger), nameof(Scp079DoorStateChanger.ServerProcessCmd))]
    internal static class TriggeringDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(TriggeringDoorEventArgs));

            int offset = -2;
            int index = newInstructions.FindIndex(
                instruction => instruction.LoadsField(Field(typeof(DoorVariant), nameof(DoorVariant.TargetState)))) + offset;

            // var ev = new TriggeringDoorEventArgs(Player.Get(base.Owner), this.LastDoor, (float)this.GetCostForDoor(this.TargetAction, this.LastDoor));
            //
            // Handlers.Scp079.OnTriggeringDoor(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this.LastDoor
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079DoorStateChanger), nameof(Scp079DoorStateChanger.LastDoor))),

                    // (float)this.GetCostForDoor(this.TargetAction, this.LastDoor)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079DoorAbility), nameof(Scp079DoorAbility.TargetAction))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079DoorStateChanger), nameof(Scp079DoorStateChanger.LastDoor))),
                    new(OpCodes.Callvirt, Method(typeof(Scp079DoorAbility), nameof(Scp079DoorAbility.GetCostForDoor))),
                    new(OpCodes.Conv_R4),

                    // var ev = new TriggeringDoorEventArgs(Player, DoorVariant, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringDoorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp079.OnTriggeringDoor(ev);
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnTriggeringDoor))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringDoorEventArgs), nameof(TriggeringDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // this.LastDoor = ev.Door
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringDoorEventArgs), nameof(TriggeringDoorEventArgs.Door))),
                    new(OpCodes.Stfld, Field(typeof(Scp079DoorStateChanger), nameof(Scp079DoorStateChanger.LastDoor))),
                });

            // Replace "(float)this.GetCostForDoor(this.TargetAction, this.LastDoor)" with "ev.AuxiliaryPowerCost"
            offset = -5;
            index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(Scp079DoorAbility), nameof(Scp079DoorAbility.GetCostForDoor)))) + offset;

            newInstructions.RemoveRange(index, 7);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringDoorEventArgs), nameof(TriggeringDoorEventArgs.AuxiliaryPowerCost))),
               });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}