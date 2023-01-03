// -----------------------------------------------------------------------
// <copyright file="RoomListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using HarmonyLib;
    using MapGeneration;
    using MapGeneration.Distributors;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoomIdentifier.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(SpawnablesDistributorBase), nameof(SpawnablesDistributorBase.Start))]
    internal class ItemDirtributorStart
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(codeInstructions);

            // Server.ItemDistributor = this
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Server), nameof(Server.ItemDistributor))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}