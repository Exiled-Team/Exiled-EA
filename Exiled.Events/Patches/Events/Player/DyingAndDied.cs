// -----------------------------------------------------------------------
// <copyright file="DyingAndDied.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="PlayerStats.KillPlayer(DamageHandlerBase)" />.
    ///     Adds the <see cref="Handlers.Player.Died" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal static class DyingAndDied
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder oldRole = generator.DeclareLocal(typeof(RoleTypeId));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player player = Player.Get(this._hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(PlayerStats), nameof(PlayerStats._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    /*new(OpCodes.Dup),*/
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    /*// handler
                    new(OpCodes.Ldarg_1),

                    // DyingEventArgs ev = new(Player, DamageHandlerBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DyingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnDying(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDying))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, ret),

                    // oldRole = player.Role.Type
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Type))),
                    new(OpCodes.Stloc, oldRole.LocalIndex),*/
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // oldRole
                    new(OpCodes.Ldloc_S, oldRole.LocalIndex),

                    // handler
                    new(OpCodes.Ldarg_1),

                    // DiedEventArgs evDied = new(Player, RoleTypeId, DamageHandlerBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DiedEventArgs))[0]),

                    // Handlers.Player.OnDied(evDied)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDied))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}