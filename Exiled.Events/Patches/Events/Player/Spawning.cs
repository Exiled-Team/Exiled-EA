// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerRoleManager.InitializeNewRole(RoleTypeId, RoleChangeReason, NetworkReader)"/>.
    /// Adds the <see cref="Spawning"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    internal static class Spawning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Find the index of the ldarg.0 before the only ldfld CharacterClassManager::SpawnProtected
            const int offset = 1;
            int index = newInstructions.FindIndex(
                i => i.Calls(Method(typeof(PlayerRoleBase), nameof(PlayerRoleBase.SpawnPoolObject)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // roleBase
                    new(OpCodes.Ldloc_2),

                    // var ev = new SpawningEventArgs(Player, PlayerRoleBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),

                    // Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}