// -----------------------------------------------------------------------
// <copyright file="SpawningAndSpawned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Spawnpoints;
    using RelativePositioning;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager.Init"/> delegate.
    /// Adds the <see cref="SpawningAndSpawned"/> event.
    /// </summary>
    [HarmonyPatch]
    internal static class SpawningAndSpawned
    {
        private static MethodInfo TargetMethod()
        {
            return Method(TypeByName("PlayerRoles.FirstPersonControl.Spawnpoints.RoleSpawnpointManager").GetNestedTypes(all)[1], "<Init>b__2_0");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.IsLdarg(1)) + offset;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this.Hub)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // roleBase
                    new(OpCodes.Ldarg_3),

                    // position
                    new(OpCodes.Ldloc_1),

                    // var ev = new SpawningEventArgs(Player, PlayerRoleBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    // relativePosition = new RelativePosition(ev.Position)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                    new(OpCodes.Stloc_1),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (player == null)
                    //    return;
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // var ev = new SpawnedEventArgs(Player)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawnedEventArgs))[0]),

                    // Handlers.Player.OnSpawned(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawned))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}