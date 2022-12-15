// -----------------------------------------------------------------------
// <copyright file="SpawningAndSpawned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using RelativePositioning;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FpcStandardRoleBase.ReadSpawnData(NetworkReader)"/>.
    /// Adds the <see cref="SpawningAndSpawned"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FpcStandardRoleBase), nameof(FpcStandardRoleBase.ReadSpawnData))]
    internal static class SpawningAndSpawned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this.Hub)
                    //
                    // if (player == Server.Host)
                    //    goto continueLabel;
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // roleBase
                    new(OpCodes.Ldarg_0),

                    // var ev = new SpawningEventArgs(Player, PlayerRoleBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),

                    new(OpCodes.Newobj, Constructor(typeof(RelativePosition), new System.Type[] { typeof(Vector3) })),
                    new(OpCodes.Stloc_0),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (player == Server.Host)
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