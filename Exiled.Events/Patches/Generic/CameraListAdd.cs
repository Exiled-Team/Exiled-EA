// -----------------------------------------------------------------------
// <copyright file="GeneratorListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079.Cameras;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Camera.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Camera), nameof(Scp079Camera.Awake))]
    internal class CameraListAdd
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(codeInstructions);

            // new Camera(this)
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Camera))[0]),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}