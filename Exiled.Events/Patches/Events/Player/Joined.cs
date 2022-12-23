// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
/*
namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600

    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ReferenceHub.Awake" />.
    ///     Adds the <see cref="Handlers.Player.Joined" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Awake))]
    internal static class Joined
    {
        internal static void CallEvent(ReferenceHub hub, out Player player)
        {
            try
            {
#if DEBUG
                Log.Debug("Creating new player object");
#endif
                player = new Player(hub);
#if DEBUG
                Log.Debug($"Object exists {player is not null}");
                Log.Debug($"Creating player object for {hub.nicknameSync.Network_displayName}");
#endif
                Player.UnverifiedPlayers.Add(hub, player);

                Handlers.Player.OnJoined(new JoinedEventArgs(player));
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(CallEvent)}: {exception}\n{exception.StackTrace}");
                player = null;
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();
            Label serverNotFull = generator.DefineLabel();

            LocalBuilder outPlayer = generator.DeclareLocal(typeof(Player));

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (this.isServer)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.isServer))),
                    new(OpCodes.Brtrue_S, ret),

                    // if (ReferenceHub.HostHub == null)
                    //    goto continueLabel;
                    new(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.HostHub))),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, ret),

                    // if (ReferenceHub.LocalHub == null)
                    //    goto continueLabel;
                    new(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.LocalHub))),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, ret),

                    // if (ReferenceHub.AllHubs.Count < CustomNetworkManager.slots)
                    //    goto serverNotFull;
                    new(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.AllHubs))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count))),
                    new(OpCodes.Ldsfld, Field(typeof(CustomNetworkManager), nameof(CustomNetworkManager.slots))),
                    new(OpCodes.Blt_S, serverNotFull),

                    // MultiAdminFeatures.CallEvent(EventType.SERVER_FULL)
                    new(OpCodes.Ldc_I4_4),
                    new(OpCodes.Call, Method(typeof(MultiAdminFeatures), nameof(MultiAdminFeatures.CallEvent))),
                    new(OpCodes.Pop),

                    // serverNotFull:
                    // CallEvent(this, out Player player)
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(serverNotFull),
                    new(OpCodes.Ldloca_S, outPlayer),
                    new(OpCodes.Call, Method(typeof(Joined), nameof(CallEvent))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
*/