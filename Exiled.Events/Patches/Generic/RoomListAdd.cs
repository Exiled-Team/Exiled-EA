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

    using NorthwoodLib.Pools;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoomIdentifier.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomIdentifier), nameof(RoomIdentifier.Awake))]
    internal class RoomListAdd
    {
        private static void Prefix(RoomIdentifier __instance)
        {
            Room.RoomIdentifierToRoom.Add(__instance, Room.CreateComponent(__instance.gameObject));
        }

        // idk, it throws me real time error
        /*private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(codeInstructions);

            // Room.RoomIdentifierToRoom.Add(this, Room.CreateComponent(this.gameObject));
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldsfld, Field(typeof(Room), nameof(Room.RoomIdentifierToRoom))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new(OpCodes.Call, Method(typeof(Room), nameof(Room.CreateComponent))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<RoomIdentifier, Room>), nameof(Dictionary<RoomIdentifier, Room>.Add), new[] { typeof(RoomIdentifier), typeof(Room) })),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }*/
    }
}