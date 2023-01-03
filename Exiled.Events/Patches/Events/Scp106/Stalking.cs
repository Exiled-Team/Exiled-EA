// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Mirror;

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp106HuntersAtlasAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Teleporting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.ServerProcessCmd))]
    internal static class Stalking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // SCp106StalkAbility, NetworkReader
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    // Returns DoctorSenseEventArgs
                    new(OpCodes.Call, Method(typeof(Stalking), nameof(Scp106Stalking))),
                    // If !ev.IsAllowed, return
                    new(OpCodes.Brfalse_S, returnLabel),

                });



            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool Scp106Stalking(Scp106StalkAbility instance, NetworkReader reader)
        {
            API.Features.Player currentPlayer = API.Features.Player.Get(instance.Owner);
            StalkingEventArgs stalkingEventArgs = new StalkingEventArgs(currentPlayer, instance);
            Handlers.Scp106.OnStalking(stalkingEventArgs);

            if (instance.IsActive && !stalkingEventArgs.MustUseAllVigor)
			{
                instance.IsActive = false;
			}

            if (!stalkingEventArgs.IsAllowed)
            {
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlashAnimation();
                }
                instance.ServerSendRpc(false);
                return false;
            }

            if (stalkingEventArgs.BypassChecks)
            {
                if (instance.IsActive && !stalkingEventArgs.MustUseAllVigor)
                {
                    instance.IsActive = false;
                }
                else
                {
                    instance.IsActive = true;
                }

                return false;
            }

            if (stalkingEventArgs.ValidateNewVigor && stalkingEventArgs.MinimumVigor > instance.Vigor.VigorAmount)
            {
                if (instance.IsActive && !stalkingEventArgs.MustUseAllVigor)
                {
                    instance.IsActive = false;
                }
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlashAnimation();
                }
                instance.ServerSendRpc(false);
                return false;
            }

            return true;
        }
    }
}