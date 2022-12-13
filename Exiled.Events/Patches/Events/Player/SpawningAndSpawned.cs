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
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerRoleManager.InitializeNewRole(RoleTypeId, RoleChangeReason, NetworkReader)"/>.
    /// Adds the <see cref="SpawningAndSpawned"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    internal static class SpawningAndSpawned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + offset;

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
                    new(OpCodes.Call, PropertyGetter(typeof(Server), nameof(Server.Host))),
                    new(OpCodes.Beq_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // roleBase
                    new(OpCodes.Ldloc_2),

                    // var ev = new SpawningEventArgs(Player, PlayerRoleBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),

                    // Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (player == Server.Host)
                    //    return;
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Call, PropertyGetter(typeof(Server), nameof(Server.Host))),
                    new(OpCodes.Beq_S, returnLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // var ev = new SpawnedEventArgs(Player)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawnedEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnSpawned(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawned))),

                    // ev.Player.Role = Role.Create(player, targetId)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawnedEventArgs), nameof(SpawnedEventArgs.Player))),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Role), nameof(Role.Create))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Player), nameof(Player.Role))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}