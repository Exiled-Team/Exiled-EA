// -----------------------------------------------------------------------
// <copyright file="TurningOffLights.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Map;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlickerableLightController.ServerFlickerLights"/>.
    /// Adds the <see cref="Handlers.Map.TurningOffLights"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FlickerableLightController), nameof(FlickerableLightController.ServerFlickerLights))]
    internal static class TurningOffLights
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(TurningOffLightsEventArgs));

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // dur
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var ev = new TurningOffLightsEventArgs(FlickerableLightController, float, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TurningOffLightsEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Map.OnTurningOffLights(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnTurningOffLights))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TurningOffLightsEventArgs), nameof(TurningOffLightsEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // dur = ev.TurningOffLightsEventArgs.Duration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TurningOffLightsEventArgs), nameof(TurningOffLightsEventArgs.Duration))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}