// -----------------------------------------------------------------------
// <copyright file="ChangingCamera.cs" company="Exiled Team">
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
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079CurrentCameraSync.ServerProcessCmd(NetworkReader)" />.
    ///     Adds the <see cref="ChangingCamera" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync.ServerProcessCmd))]
    internal static class ChangingCamera
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Conv_R4) + offset;

            // Define the first label of the last "ret" and retrieve it.
            Label returnLabel = newInstructions[newInstructions.Count - 1].WithLabels(generator.DefineLabel()).labels[0];

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingCameraEventArgs));

            // var ev = new ChangingCameraEventArgs(Player.Get(this.gameObject), camera, num,  num <= this.curMana)
            //
            // Handlers.Scp079.OnChangingCamera(ev)
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // num = ev.AuxiliaryPowerCost
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // camera
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync._switchTarget))),

                    // num (auxiliary power cost)
                    new(OpCodes.Ldloc_0),

                    // !(num > this.curMana) --> num <= this.curMana
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync._auxManager))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079AuxManager), nameof(Scp079AuxManager.CurrentAux))),
                    new(OpCodes.Cgt),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // var ev = new ChangingCameraEventArgs(Player, Scp079Camera, float, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingCameraEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp079.OnChangingCamera(ev)
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnChangingCamera))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // num = ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingCameraEventArgs), nameof(ChangingCameraEventArgs.AuxiliaryPowerCost))),
                    new(OpCodes.Stloc_0),

                    // TODO: Set ev.Camera to _switchTarget
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}