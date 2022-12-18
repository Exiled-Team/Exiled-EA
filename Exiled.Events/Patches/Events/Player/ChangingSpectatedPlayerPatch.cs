// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.Spectating;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SpectatorRole.SyncedSpectatedNetId" /> setter.
    ///     Adds the <see cref="Player.ChangingSpectatedPlayer" />.
    /// </summary>
    [HarmonyPatch(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId), MethodType.Setter)]
    internal static class ChangingSpectatedPlayerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label elseLabel = generator.DefineLabel();
            Label nullLabel = generator.DefineLabel();
            Label skipNull = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder owner = generator.DeclareLocal(typeof(ReferenceHub));
            LocalBuilder previousSpectatedPlayer = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingSpectatedPlayerEventArgs));

            const int index = 0;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (value == 0)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // if (!this.TryGetOwner(out ReferenceHub owner))
                    //    return;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloca_S, owner.LocalIndex),
                    new(OpCodes.Call, Method(typeof(PlayerRoleBase), nameof(PlayerRoleBase.TryGetOwner), new[] { typeof(ReferenceHub).MakeByRefType() })),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // Player.Get(owner)
                    new(OpCodes.Ldloc_S, owner.LocalIndex),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player previousSpectatedPlayer = Player.Get(this.SyncedSpectatedNetId)
                    //
                    // if (previousSpectatedPlayer == null)
                    //    goto nullLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, previousSpectatedPlayer.LocalIndex),
                    new(OpCodes.Brfalse_S, nullLabel),

                    // previousSpectatedPlayer
                    //
                    // goto skipNull
                    new(OpCodes.Ldloc_S, previousSpectatedPlayer.LocalIndex),
                    new(OpCodes.Br_S, skipNull),

                    // nullLabel:
                    //
                    // null
                    new CodeInstruction(OpCodes.Ldnull).WithLabels(nullLabel),

                    // skippNull:
                    //
                    // Player.Get(value)
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(skipNull),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),

                    // true
                    new CodeInstruction(OpCodes.Ldc_I4_1),

                    // ChangingSpectatedPlayerEventArgs ev = new(Player, Player, Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingSpectatedPlayerEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev),

                    // Player.OnChangingSpectatedPlayer(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingSpectatedPlayer))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingSpectatedPlayerEventArgs), nameof(ChangingSpectatedPlayerEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // ev.NewTarget
                    new CodeInstruction(OpCodes.Ldloc_S, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingSpectatedPlayerEventArgs), nameof(ChangingSpectatedPlayerEventArgs.NewTarget))),

                    // if (ev.NewTarget != null)
                    //    goto elseLabel;
                    new(OpCodes.Dup),
                    new(OpCodes.Brtrue_S, elseLabel),

                    // value = ev.Player.ReferenceHub;
                    new(OpCodes.Pop),
                    new(OpCodes.Ldloc_S, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingSpectatedPlayerEventArgs), nameof(ChangingSpectatedPlayerEventArgs.Player))),

                    // value = ev.NewTarget.ReferenceHub;
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.NetworkIdentity))).WithLabels(elseLabel),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}