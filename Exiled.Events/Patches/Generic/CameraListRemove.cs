// -----------------------------------------------------------------------
// <copyright file="CameraListRemove.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079InteractableBase.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch]
    public static class CameraListRemove
    {
#pragma warning disable CS1591
#pragma warning disable SA1600
        public static MethodBase TargetMethod()
        {
            foreach (var method in typeof(Scp079InteractableBase).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (method.Name.Contains("OnDestroy"))
                {
                    Log.Info($" method.Name {method.Name}");
                    return method;
                }
            }
            return null;

        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CameraListPatch_Body(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(codeInstructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Scp079Camera));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                    // if (this is Scp079Camera scp079cam)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Isinst,  typeof(Scp079Camera)),
                    new(OpCodes.Stloc_0, player),
                    new(OpCodes.Ldloc_0, player),
                    new(OpCodes.Brfalse_S, ret),

                    // Camera.Camera079ToCamera.Remove(scp079cam)
                    new(OpCodes.Ldsfld, Field(typeof(Camera), nameof(Camera.Camera079ToCamera))),
                    new(OpCodes.Ldloc_0, player),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<Scp079Camera, Camera>), nameof(Dictionary<Scp079Camera, Camera>.Remove))),
                    new(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Nop).WithLabels(ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

}
