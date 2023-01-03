using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.Events.EventArgs.Scp049;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using PluginAPI.Core;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace Exiled.Events.Patches.Events.Scp049
{
    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.FinishingRecall" /> event.
    /// </summary>


    [HarmonyPatch]
    public class StartingZombieConsume
    {

        private static MethodInfo GetSendMethod()
        {
            foreach (MethodInfo method in typeof(RagdollAbilityBase<ZombieRole>).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // if(method.Name.Contains("Server"))

                    if (method.Name.Contains("ServerValidateAny") && method.GetGenericArguments().Length == 0)
                    {
                        Log.Info($"What is current method: {method} and {method.Name.Contains("ServerValidateAny")}");
                        return method;
                        //return method.MakeGenericMethod(typeof(ZombieConsumeAbility));
                    }
            }

            return null;
        }

        [HarmonyPatch(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.ServerProcessCmd))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ZombieServerProcessCmdPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Finds ServerValidateAny which check if corpse is nearby
            int offset = -1;
            // int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ZombieConsumeAbility), nameof(ZombieConsumeAbility.ServerValidateAny)))) + offset;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.ServerValidateAny)))) + offset;
            newInstructions.RemoveRange(index, 3);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);

        }

        [HarmonyPatch(typeof(ZombieConsumeAbility), nameof(ZombieConsumeAbility.ServerValidateBegin))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Scp049SenseAbility, BasicRagdoll
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    // Returns Byte
                    new(OpCodes.Call, Method(typeof(StartingZombieConsume), nameof(ZombieConsumeConditions))),
                    new(OpCodes.Br, returnLabel),

                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Basically rewrites the ServerProcessCmd - It really is not worth it to not do this
        /// </summary>
        /// <param name="senseAbility"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static byte ZombieConsumeConditions(ZombieConsumeAbility zombieConsumeInstance, BasicRagdoll ragdoll)
        {
            API.Features.Player currentPlayer = API.Features.Player.Get(zombieConsumeInstance.Owner);
            API.Features.Player target = API.Features.Player.Get(ragdoll.Info.OwnerHub);
            ZombieConsumeEventArgs zombieConsumeEvent = new ZombieConsumeEventArgs(currentPlayer, target, ZombieConsumeAbility.ConsumedRagdolls);
            Handlers.Scp049.OnStartingConsume(zombieConsumeEvent);

            if (!zombieConsumeEvent.IsAllowed)
            {
                return 0;
            }

            if (ZombieConsumeAbility.ConsumedRagdolls.Contains(ragdoll))
            {
                return 2;
            }
            if ( (!ragdoll.Info.RoleType.IsHuman() || !zombieConsumeInstance.ServerValidateAny()) && !zombieConsumeEvent.AllowNonHumans)
            {
                return 3;
            }
            if (zombieConsumeInstance.Owner.playerStats.GetModule<HealthStat>().NormalizedValue == 1f)
            {
                return 8;
            }
            foreach (ZombieConsumeAbility zombieConsumeAbility in ZombieConsumeAbility.AllAbilities)
            {
                if (zombieConsumeAbility.IsInProgress && zombieConsumeAbility.CurRagdoll == ragdoll)
                {
                    return 9;
                }
            }
            return 0;
        }

    }
}