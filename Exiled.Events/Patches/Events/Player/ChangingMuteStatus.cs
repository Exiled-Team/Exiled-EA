// -----------------------------------------------------------------------
// <copyright file="ChangingMuteStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

/*
namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Assets._Scripts.Dissonance;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="DissonanceUserSetup.AdministrativelyMuted" />.
    ///     Adds the <see cref="ChangingMuteStatus" /> event.
    /// </summary>
    // [HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.AdministrativelyMuted), MethodType.Setter)]
    internal static class ChangingMuteStatus
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            Label retLabel = generator.DefineLabel();
            Label jccLabel = generator.DefineLabel();
            Label cdcLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(cdcLabel);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.netId))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(uint) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMuteStatusEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingMuteStatus))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMuteStatusEventArgs), nameof(ChangingMuteStatusEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, cdcLabel),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Brtrue_S, jccLabel),
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Call, Method(typeof(MuteHandler), nameof(MuteHandler.IssuePersistentMute))),
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(jccLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.UserId))),
                    new(OpCodes.Call, Method(typeof(MuteHandler), nameof(MuteHandler.RevokePersistentMute))),
                    new(OpCodes.Br_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
*/