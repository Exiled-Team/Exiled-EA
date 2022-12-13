// -----------------------------------------------------------------------
// <copyright file="ChangingSpeakerStatusAndVoiceChatting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.Voice;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches Scp079VoiceModule.ServerIsSending />.
    ///     Adds the <see cref="ChangingSpeakerStatusEventArgs" /> event for SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ServerIsSending), MethodType.Setter)]
    internal static class ChangingSpeakerStatusAndVoiceChatting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int index = 0;

            newInstructions[index].WithLabels(continueLabel);

            // var voiceChattingEv = new VoiceChattingEventArgs(Player.Get(this.Owner), this, true)
            //
            // Handlers.Player.OnVoiceChatting(voiceChattingEv)
            //
            // if (!voiceChattingEv.IsAllowed)
            //    return;
            //
            // if (base.CurrentChannel != VoiceChatChannel.Proximity)
            //    goto continueLabel;
            //
            // var ev = new ChangingSpeakerStatusEventArgs(Player.Get(base.Owner), value);
            //
            // Scp079.OnStartingSpeaker(ev);
            //
            // value = ev.IsAllowed
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // var voiceChattingEv = new VoiceChattingEventArgs(Player, VoiceModuleBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VoiceChattingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnVoiceChatting(voiceChattingEv)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnVoiceChatting))),

                    // if (!voiceChattingEv.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceChattingEventArgs), nameof(VoiceChattingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // if (this.Role.RoleTypeId != RoleTypeId.Scp079)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.Role))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_7),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // if (this.CurrentChannel != VoiceChatChannel.Proximity)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.CurrentChannel))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // value
                    new(OpCodes.Ldarg_1),

                    // var ev = new ChangingSpeakerStatusEventArgs(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingSpeakerStatusEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp079.OnChangingSpeakerStatus(ev);
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnChangingSpeakerStatus))),

                    // value = ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingSpeakerStatusEventArgs), nameof(ChangingSpeakerStatusEventArgs.IsAllowed))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}