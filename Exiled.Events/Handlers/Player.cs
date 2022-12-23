// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Player;
    using Extensions;
    using MapGeneration.Distributors;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Core.Interfaces;
    using PluginAPI.Enums;

    using static Events;

    /// <summary>
    /// Player related events.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Invoked before authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<PreAuthenticatingEventArgs> PreAuthenticating;

        /// <summary>
        /// Invoked before reserved slot is finalized for a <see cref="API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<ReservedSlotsCheckEventArgs> ReservedSlot;

        /// <summary>
        /// Invoked before kicking a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        public static event CustomEventHandler<KickingEventArgs> Kicking;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been kicked from the server.
        /// </summary>
        public static event CustomEventHandler<KickedEventArgs> Kicked;

        /// <summary>
        /// Invoked before banning a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        public static event CustomEventHandler<BanningEventArgs> Banning;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been banned from the server.
        /// </summary>
        public static event CustomEventHandler<BannedEventArgs> Banned;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> uses an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        /// <remarks>
        /// Invoked after <see cref="UsedItem"/>, if a player's class has
        /// changed during their health increase, won't fire.
        /// </remarks>
        public static event CustomEventHandler<UsedItemEventArgs> UsedItem;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has stopped the use of a <see cref="API.Features.Items.Usable"/>.
        /// </summary>
        public static event CustomEventHandler<CancellingItemUseEventArgs> CancellingItemUse;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> interacted with something.
        /// </summary>
        public static event CustomEventHandler<InteractedEventArgs> Interacted;

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Player"/> <see cref="API.Features.Ragdoll"/>.
        /// </summary>
        public static event CustomEventHandler<SpawningRagdollEventArgs> SpawningRagdoll;

        /// <summary>
        /// Invoked before activating the warhead panel.
        /// </summary>
        public static event CustomEventHandler<ActivatingWarheadPanelEventArgs> ActivatingWarheadPanel;

        /// <summary>
        /// Invoked before activating a workstation.
        /// </summary>
        public static event CustomEventHandler<ActivatingWorkstationEventArgs> ActivatingWorkstation;

        /// <summary>
        /// Invoked before deactivating a workstation.
        /// </summary>
        public static event CustomEventHandler<DeactivatingWorkstationEventArgs> DeactivatingWorkstation;

        /// <summary>
        /// Invoked before using an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<UsingItemEventArgs> UsingItem;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has joined the server.
        /// </summary>
        public static event CustomEventHandler<JoinedEventArgs> Joined;

        /// <summary>
        /// Ivoked after a <see cref="API.Features.Player"/> has been verified.
        /// </summary>
        public static event CustomEventHandler<VerifiedEventArgs> Verified;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has left the server.
        /// </summary>
        public static event CustomEventHandler<LeftEventArgs> Left;

        /// <summary>
        /// Invoked before destroying a <see cref="API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<DestroyingEventArgs> Destroying;

        /// <summary>
        /// Invoked before hurting a <see cref="API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<HurtingEventArgs> Hurting;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> dies.
        /// </summary>
        public static event CustomEventHandler<DyingEventArgs> Dying;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> died.
        /// </summary>
        public static event CustomEventHandler<DiedEventArgs> Died;

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Player"/> role.
        /// </summary>
        /// <remarks>If you set IsAllowed to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static event CustomEventHandler<ChangingRoleEventArgs> ChangingRole;

        /// <summary>
        /// Invoked before throwing an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<ThrowingItemEventArgs> ThrowingItem;

        /// <summary>
        /// Invoked before dropping an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<DroppingItemEventArgs> DroppingItem;

        /// <summary>
        /// Invoked before dropping a null <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<DroppingNothingEventArgs> DroppingNothing;

        /// <summary>
        /// Invoked before picking up ammo.
        /// </summary>
        public static event CustomEventHandler<PickingUpAmmoEventArgs> PickingUpAmmo;

        /// <summary>
        /// Invoked before picking up armor.
        /// </summary>
        public static event CustomEventHandler<PickingUpArmorEventArgs> PickingUpArmor;

        /// <summary>
        /// Invoked before picking up an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<PickingUpItemEventArgs> PickingUpItem;

        /// <summary>
        /// Invoked before handcuffing a <see cref="API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<HandcuffingEventArgs> Handcuffing;

        /// <summary>
        /// Invoked before freeing a handcuffed <see cref="API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<RemovingHandcuffsEventArgs> RemovingHandcuffs;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> escapes.
        /// </summary>
        public static event CustomEventHandler<EscapingEventArgs> Escaping;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        public static event CustomEventHandler<IntercomSpeakingEventArgs> IntercomSpeaking;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> gets shot.
        /// </summary>
        public static event CustomEventHandler<ShotEventArgs> Shot;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        public static event CustomEventHandler<ShootingEventArgs> Shooting;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EnteringPocketDimensionEventArgs> EnteringPocketDimension;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EscapingPocketDimensionEventArgs> EscapingPocketDimension;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<FailingEscapePocketDimensionEventArgs> FailingEscapePocketDimension;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters killer collision.
        /// </summary>
        public static event CustomEventHandler<EnteringKillerCollisionEventArgs> EnteringKillerCollision;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> reloads a weapon.
        /// </summary>
        public static event CustomEventHandler<ReloadingWeaponEventArgs> ReloadingWeapon;

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Player"/>(called only when possibly to change position).
        /// use <see cref="Spawned"/> or <see cref="ChangingRole"/>for all class changes.
        /// </summary>
        public static event CustomEventHandler<SpawningEventArgs> Spawning;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has spawned.
        /// </summary>
        public static event CustomEventHandler<SpawnedEventArgs> Spawned;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> held <see cref="API.Features.Items.Item"/> changes.
        /// </summary>
        public static event CustomEventHandler<ChangingItemEventArgs> ChangingItem;

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Player"/> group.
        /// </summary>
        public static event CustomEventHandler<ChangingGroupEventArgs> ChangingGroup;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a door.
        /// </summary>
        public static event CustomEventHandler<InteractingDoorEventArgs> InteractingDoor;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with an elevator.
        /// </summary>
        public static event CustomEventHandler<InteractingElevatorEventArgs> InteractingElevator;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a locker.
        /// </summary>
        public static event CustomEventHandler<InteractingLockerEventArgs> InteractingLocker;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> triggers a tesla gate.
        /// </summary>
        public static event CustomEventHandler<TriggeringTeslaEventArgs> TriggeringTesla;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> unlocks a generator.
        /// </summary>
        public static event CustomEventHandler<UnlockingGeneratorEventArgs> UnlockingGenerator;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> opens a generator.
        /// </summary>
        public static event CustomEventHandler<OpeningGeneratorEventArgs> OpeningGenerator;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> closes a generator.
        /// </summary>
        public static event CustomEventHandler<ClosingGeneratorEventArgs> ClosingGenerator;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> turns on the generator by switching lever.
        /// </summary>
        public static event CustomEventHandler<ActivatingGeneratorEventArgs> ActivatingGenerator;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> turns off the generator by switching lever.
        /// </summary>
        public static event CustomEventHandler<StoppingGeneratorEventArgs> StoppingGenerator;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> receives a status effect.
        /// </summary>
        public static event CustomEventHandler<ReceivingEffectEventArgs> ReceivingEffect;

        /// <summary>
        /// Invoked before muting a user.
        /// </summary>
        public static event CustomEventHandler<IssuingMuteEventArgs> IssuingMute;

        /// <summary>
        /// Invoked before unmuting a user.
        /// </summary>
        public static event CustomEventHandler<RevokingMuteEventArgs> RevokingMute;

        /// <summary>
        /// Invoked before a user's radio battery charge is changed.
        /// </summary>
        public static event CustomEventHandler<UsingRadioBatteryEventArgs> UsingRadioBattery;

        /// <summary>
        /// Invoked before a user's radio preset is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingRadioPresetEventArgs> ChangingRadioPreset;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingMicroHIDStateEventArgs> ChangingMicroHIDState;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        public static event CustomEventHandler<UsingMicroHIDEnergyEventArgs> UsingMicroHIDEnergy;

        /// <summary>
        /// Called before processing a hotkey.
        /// </summary>
        public static event CustomEventHandler<ProcessingHotkeyEventArgs> ProcessingHotkey;

        /// <summary>
        /// Invoked before dropping ammo.
        /// </summary>
        public static event CustomEventHandler<DroppingAmmoEventArgs> DroppingAmmo;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        public static event CustomEventHandler<InteractingShootingTargetEventArgs> InteractingShootingTarget;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damages a shooting target.
        /// </summary>
        public static event CustomEventHandler<DamagingShootingTargetEventArgs> DamagingShootingTarget;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> flips a coin.
        /// </summary>
        public static event CustomEventHandler<FlippingCoinEventArgs> FlippingCoin;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the flashlight.
        /// </summary>
        public static event CustomEventHandler<TogglingFlashlightEventArgs> TogglingFlashlight;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> unloads a weapon.
        /// </summary>
        public static event CustomEventHandler<UnloadingWeaponEventArgs> UnloadingWeapon;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> triggers an aim action.
        /// </summary>
        public static event CustomEventHandler<AimingDownSightEventArgs> AimingDownSight;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        public static event CustomEventHandler<TogglingWeaponFlashlightEventArgs> TogglingWeaponFlashlight;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> dryfires a weapon.
        /// </summary>
        public static event CustomEventHandler<DryfiringWeaponEventArgs> DryfiringWeapon;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the voicechat key.
        /// </summary>
        public static event CustomEventHandler<VoiceChattingEventArgs> VoiceChatting;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> makes noise.
        /// </summary>
        public static event CustomEventHandler<MakingNoiseEventArgs> MakingNoise;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> jumps.
        /// </summary>
        public static event CustomEventHandler<JumpingEventArgs> Jumping;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> lands.
        /// </summary>
        public static event CustomEventHandler<LandingEventArgs> Landing;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the transmission key.
        /// </summary>
        public static event CustomEventHandler<TransmittingEventArgs> Transmitting;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> changes move state.
        /// </summary>
        public static event CustomEventHandler<ChangingMoveStateEventArgs> ChangingMoveState;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> changed spectated player.
        /// </summary>
        public static event CustomEventHandler<ChangingSpectatedPlayerEventArgs> ChangingSpectatedPlayer;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        public static event CustomEventHandler<TogglingNoClipEventArgs> TogglingNoClip;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles overwatch.
        /// </summary>
        public static event CustomEventHandler<TogglingOverwatchEventArgs> TogglingOverwatch;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> searches a Pickup.
        /// </summary>
        public static event CustomEventHandler<SearchingPickupEventArgs> SearchingPickup;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damage a Window.
        /// </summary>
        public static event CustomEventHandler<DamagingWindowEventArgs> PlayerDamageWindow;

        /// <summary>
        /// Invoked after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        public static event CustomEventHandler<ItemAddedEventArgs> ItemAdded;

        /// <summary>
        /// Invoked before KillPlayer is called.
        /// </summary>
        public static event CustomEventHandler<KillingPlayerEventArgs> KillingPlayer;

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        public static event CustomEventHandler<EnteringEnvironmentalHazardEventArgs> EnteringEnvironmentalHazard;

        /// <summary>
        /// Invoked when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        public static event CustomEventHandler<StayingOnEnvironmentalHazardEventArgs> StayingOnEnvironmentalHazard;

        /// <summary>
        /// Invoked when a <see cref="API.Features.Player"/> exists from an environmental hazard.
        /// </summary>
        public static event CustomEventHandler<ExitingEnvironmentalHazardEventArgs> ExitingEnvironmentalHazard;

        /// <summary>
        /// Called before pre-authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="PreAuthenticatingEventArgs"/> instance.</param>
        public static void OnPreAuthenticating(PreAuthenticatingEventArgs ev) => PreAuthenticating.InvokeSafely(ev);

        /// <summary>
        /// Called before reserved slot is resolved for a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ReservedSlotsCheckEventArgs"/> instance.</param>
        public static void OnReservedSlot(ReservedSlotsCheckEventArgs ev) => ReservedSlot.InvokeSafely(ev);

        /// <summary>
        /// Called before kicking a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickingEventArgs"/> instance.</param>
        public static void OnKicking(KickingEventArgs ev) => Kicking.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has been kicked from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickedEventArgs"/> instance.</param>
        public static void OnKicked(KickedEventArgs ev) => Kicked.InvokeSafely(ev);

        /// <summary>
        /// Called before banning a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BanningEventArgs"/> instance.</param>
        public static void OnBanning(BanningEventArgs ev) => Banning.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has been banned from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BannedEventArgs"/> instance.</param>
        public static void OnBanned(BannedEventArgs ev) => Banned.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> used a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="UsedItemEventArgs"/> instance.</param>
        public static void OnUsedItem(UsedItemEventArgs ev) => UsedItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has stopped the use of a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="CancellingItemUseEventArgs"/> instance.</param>
        public static void OnCancellingItemUse(CancellingItemUseEventArgs ev) => CancellingItemUse.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> interacted with something.
        /// </summary>
        /// <param name="ev">The <see cref="InteractedEventArgs"/> instance.</param>
        public static void OnInteracted(InteractedEventArgs ev) => Interacted.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a <see cref="API.Features.Player"/> ragdoll.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningRagdollEventArgs"/> instance.</param>
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev) => SpawningRagdoll.InvokeSafely(ev);

        /// <summary>
        /// Called before activating the warhead panel.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWarheadPanelEventArgs"/> instance.</param>
        public static void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev) => ActivatingWarheadPanel.InvokeSafely(ev);

        /// <summary>
        /// Called before activating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWorkstation"/> instance.</param>
        public static void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev) => ActivatingWorkstation.InvokeSafely(ev);

        /// <summary>
        /// Called before deactivating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingWorkstationEventArgs"/> instance.</param>
        public static void OnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev) => DeactivatingWorkstation.InvokeSafely(ev);

        /// <summary>
        /// Called before using a usable item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingItemEventArgs"/> instance.</param>
        public static void OnUsingItem(UsingItemEventArgs ev) => UsingItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has joined the server.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs"/> instance.</param>
        public static void OnJoined(JoinedEventArgs ev) => Joined.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has been verified.
        /// </summary>
        /// <param name="ev">The <see cref="VerifiedEventArgs"/> instance.</param>
        public static void OnVerified(VerifiedEventArgs ev) => Verified.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has left the server.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs"/> instance.</param>
        public static void OnLeft(LeftEventArgs ev) => Left.InvokeSafely(ev);

        /// <summary>
        /// Called before destroying a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DestroyingEventArgs"/> instance.</param>
        public static void OnDestroying(DestroyingEventArgs ev) => Destroying.InvokeSafely(ev);

        /// <summary>
        /// Called before hurting a player.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> instance.</param>
        public static void OnHurting(HurtingEventArgs ev) => Hurting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs"/> instance.</param>
        public static void OnDying(DyingEventArgs ev) => Dying.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> died.
        /// </summary>
        /// <param name="ev">The <see cref="DiedEventArgs"/> instance.</param>
        public static void OnDied(DiedEventArgs ev) => Died.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Player"/> role.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        /// <remarks>If you set IsAllowed to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static void OnChangingRole(ChangingRoleEventArgs ev) => ChangingRole.InvokeSafely(ev);

        /// <summary>
        /// Called before throwing a grenade.
        /// </summary>
        /// <param name="ev">The <see cref="ThrowingItemEventArgs"/> instance.</param>
        public static void OnThrowingItem(ThrowingItemEventArgs ev) => ThrowingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> instance.</param>
        public static void OnDroppingItem(DroppingItemEventArgs ev) => DroppingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping a null item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingNothingEventArgs"/> instance.</param>
        public static void OnDroppingNothing(DroppingNothingEventArgs ev) => DroppingNothing.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up ammo.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpAmmoEventArgs"/> instance.</param>
        public static void OnPickingUpAmmo(PickingUpAmmoEventArgs ev) => PickingUpAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up armor.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpArmorEventArgs"/> instance.</param>
        public static void OnPickingUpArmor(PickingUpArmorEventArgs ev) => PickingUpArmor.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up an item.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpItemEventArgs"/> instance.</param>
        public static void OnPickingUpItem(PickingUpItemEventArgs ev) => PickingUpItem.InvokeSafely(ev);

        /// <summary>
        /// Called before handcuffing a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public static void OnHandcuffing(HandcuffingEventArgs ev) => Handcuffing.InvokeSafely(ev);

        /// <summary>
        /// Called before freeing a handcuffed <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="RemovingHandcuffsEventArgs"/> instance.</param>
        public static void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev) => RemovingHandcuffs.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public static void OnEscaping(EscapingEventArgs ev) => Escaping.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        /// <param name="ev">The <see cref="IntercomSpeakingEventArgs"/> instance.</param>
        public static void OnIntercomSpeaking(IntercomSpeakingEventArgs ev) => IntercomSpeaking.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShotEventArgs"/> instance.</param>
        public static void OnShot(ShotEventArgs ev) => Shot.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShootingEventArgs"/> instance.</param>
        public static void OnShooting(ShootingEventArgs ev) => Shooting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringPocketDimensionEventArgs"/> instance.</param>
        public static void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev) => EnteringPocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingPocketDimensionEventArgs"/> instance.</param>
        public static void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev) => EscapingPocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="FailingEscapePocketDimensionEventArgs"/> instance.</param>
        public static void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev) => FailingEscapePocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters killer collision.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringKillerCollisionEventArgs"/> instance.</param>
        public static void OnEnteringKillerCollision(EnteringKillerCollisionEventArgs ev) => EnteringKillerCollision.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> reloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ReloadingWeaponEventArgs"/> instance.</param>
        public static void OnReloadingWeapon(ReloadingWeaponEventArgs ev) => ReloadingWeapon.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningEventArgs"/> instance.</param>
        public static void OnSpawning(SpawningEventArgs ev) => Spawning.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has spawned.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> instance.</param>
        /// <param name="oldRole">The player's old <see cref="PlayerRoleBase"/> instance.</param>
        /// <param name="newRole">The player's new <see cref="PlayerRoleBase"/> instance.</param>
        public static void OnSpawned(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole) => Spawned.InvokeSafely(new SpawnedEventArgs(hub, oldRole));

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> held item changes.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingItemEventArgs"/> instance.</param>
        public static void OnChangingItem(ChangingItemEventArgs ev) => ChangingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Player"/> group.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingGroupEventArgs"/> instance.</param>
        public static void OnChangingGroup(ChangingGroupEventArgs ev) => ChangingGroup.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingDoorEventArgs"/> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingElevatorEventArgs"/> instance.</param>
        public static void OnInteractingElevator(InteractingElevatorEventArgs ev) => InteractingElevator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a locker.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingLockerEventArgs"/> instance.</param>
        public static void OnInteractingLocker(InteractingLockerEventArgs ev) => InteractingLocker.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> triggers a tesla.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringTeslaEventArgs"/> instance.</param>
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev) => TriggeringTesla.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> receives a status effect.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingEffectEventArgs"/> instance.</param>
        public static void OnReceivingEffect(ReceivingEffectEventArgs ev) => ReceivingEffect.InvokeSafely(ev);

        /// <summary>
        /// Called before muting a user.
        /// </summary>
        /// <param name="ev">The <see cref="IssuingMuteEventArgs"/> instance.</param>
        public static void OnIssuingMute(IssuingMuteEventArgs ev) => IssuingMute.InvokeSafely(ev);

        /// <summary>
        /// Called before unmuting a user.
        /// </summary>
        /// <param name="ev">The <see cref="RevokingMuteEventArgs"/> instance.</param>
        public static void OnRevokingMute(RevokingMuteEventArgs ev) => RevokingMute.InvokeSafely(ev);

        /// <summary>
        /// Called before a user's radio battery charge is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev) => UsingRadioBattery.InvokeSafely(ev);

        /// <summary>
        /// Called before a user's radio preset is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance.</param>
        public static void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev) => ChangingRadioPreset.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance.</param>
        public static void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev) => ChangingMicroHIDState.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingMicroHIDEnergyEventArgs"/> instance.</param>
        public static void OnUsingMicroHIDEnergy(UsingMicroHIDEnergyEventArgs ev) => UsingMicroHIDEnergy.InvokeSafely(ev);

        /// <summary>
        /// Called before processing a hotkey.
        /// </summary>
        /// <param name="ev">The <see cref="ProcessingHotkeyEventArgs"/> instance.</param>
        public static void OnProcessingHotkey(ProcessingHotkeyEventArgs ev) => ProcessingHotkey.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping ammo.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingAmmoEventArgs"/> instance.</param>
        public static void OnDroppingAmmo(DroppingAmmoEventArgs ev) => DroppingAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingShootingTargetEventArgs"/> instance.</param>
        public static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev) => InteractingShootingTarget.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> damages a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingShootingTargetEventArgs"/> instance.</param>
        public static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev) => DamagingShootingTarget.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> flips a coin.
        /// </summary>
        /// <param name="ev">The <see cref="FlippingCoinEventArgs"/> instance.</param>
        public static void OnFlippingCoin(FlippingCoinEventArgs ev) => FlippingCoin.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles the flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingFlashlightEventArgs"/> instance.</param>
        public static void OnTogglingFlashlight(TogglingFlashlightEventArgs ev) => TogglingFlashlight.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> unloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingWeaponEventArgs"/> instance.</param>
        public static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev) => UnloadingWeapon.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> triggers an aim action.
        /// </summary>
        /// <param name="ev">The <see cref="AimingDownSightEventArgs"/> instance.</param>
        public static void OnAimingDownSight(AimingDownSightEventArgs ev) => AimingDownSight.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingWeaponFlashlightEventArgs"/> instance.</param>
        public static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev) => TogglingWeaponFlashlight.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> dryfires a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="DryfiringWeaponEventArgs"/> instance.</param>
        public static void OnDryfiringWeapon(DryfiringWeaponEventArgs ev) => DryfiringWeapon.InvokeSafely(ev);

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the voicechat key.
        /// </summary>
        /// <param name="ev">The <see cref="VoiceChattingEventArgs"/> instance.</param>
        public static void OnVoiceChatting(VoiceChattingEventArgs ev) => VoiceChatting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> makes noise.
        /// </summary>
        /// <param name="animatedCharacterModel"> The <see cref="AnimatedCharacterModel"/> instance.</param>
        /// <param name="distance">The footsteps distance.</param>
        public static void OnMakingNoise(AnimatedCharacterModel animatedCharacterModel, float distance)
            => MakingNoise.InvokeSafely(new MakingNoiseEventArgs(animatedCharacterModel, distance));

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> jumps.
        /// </summary>
        /// <param name="ev">The <see cref="JumpingEventArgs"/> instance.</param>
        public static void OnJumping(JumpingEventArgs ev) => Jumping.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> lands.
        /// </summary>
        /// <param name="ev">The <see cref="LandingEventArgs"/> instance.</param>
        public static void OnLanding(LandingEventArgs ev) => Landing.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> presses the transmission key.
        /// </summary>
        /// <param name="ev">The <see cref="TransmittingEventArgs"/> instance.</param>
        public static void OnTransmitting(TransmittingEventArgs ev) => Transmitting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> changes move state.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMoveStateEventArgs"/> instance.</param>
        public static void OnChangingMoveState(ChangingMoveStateEventArgs ev) => ChangingMoveState.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> changes spectated player.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingSpectatedPlayerEventArgs"/> instance.</param>
        public static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev) => ChangingSpectatedPlayer.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingNoClipEventArgs"/> instance.</param>
        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev) => TogglingNoClip.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles overwatch.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingOverwatchEventArgs"/> instance.</param>
        public static void OnTogglingOverwatch(TogglingOverwatchEventArgs ev) => TogglingOverwatch.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> searches a Pickup.
        /// </summary>
        /// <param name="ev">The <see cref="SearchingPickupEventArgs"/> instance.</param>
        public static void OnSearchPickupRequest(SearchingPickupEventArgs ev) => SearchingPickup.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> damage a window.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingWindowEventArgs"/> instance.</param>
        public static void OnPlayerDamageWindow(DamagingWindowEventArgs ev) => PlayerDamageWindow.InvokeSafely(ev);

        /// <summary>
        ///  Called before KillPlayer is called.
        /// </summary>
        /// <param name="ev">The <see cref="KillingPlayerEventArgs"/> event handler. </param>
        public static void OnKillPlayer(KillingPlayerEventArgs ev) => KillingPlayer.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> the item was added to.</param>
        /// <param name="itemBase">The added <see cref="InventorySystem.Items.ItemBase"/>.</param>
        /// <param name="pickupBase">The <see cref="InventorySystem.Items.Pickups.ItemPickupBase"/> the <see cref="InventorySystem.Items.ItemBase"/> originated from, or <see langword="null"/> if the item was not picked up.</param>
        public static void OnItemAdded(ReferenceHub referenceHub, InventorySystem.Items.ItemBase itemBase, InventorySystem.Items.Pickups.ItemPickupBase pickupBase)
            => ItemAdded.InvokeSafely(new ItemAddedEventArgs(referenceHub, itemBase, pickupBase));

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnEnteringEnvironmentalHazard(EnteringEnvironmentalHazardEventArgs ev) => EnteringEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="StayingOnEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnStayingOnEnvironmentalHazard(StayingOnEnvironmentalHazardEventArgs ev) => StayingOnEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> exits from an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="ExitingEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnExitingEnvironmentalHazard(ExitingEnvironmentalHazardEventArgs ev) => ExitingEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> unlocks a generator.
        /// </summary>
        /// <param name="player">The <see cref="PluginAPI.Core.Player"/> instance.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <returns>Returns whether the event is allowed or not.</returns>
        [PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        public bool OnUnlockingGenerator(IPlayer player, Scp079Generator generator)
        {
            UnlockingGeneratorEventArgs ev = new(API.Features.Player.Get(player.ReferenceHub), generator);

            UnlockingGenerator.InvokeSafely(ev);

            return ev.IsAllowed;
        }

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> opens a generator.
        /// </summary>
        /// <param name="player">The <see cref="PluginAPI.Core.Player"/> instance.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <returns>Returns whether the event is allowed or not.</returns>
        [PluginEvent(ServerEventType.PlayerOpenGenerator)]
        public bool OnOpeningGenerator(IPlayer player, Scp079Generator generator)
        {
            OpeningGeneratorEventArgs ev = new(API.Features.Player.Get(player.ReferenceHub), generator);

            OpeningGenerator.InvokeSafely(ev);

            return ev.IsAllowed;
        }

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> closes a generator.
        /// </summary>
        /// <param name="player">The <see cref="PluginAPI.Core.Player"/> instance.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <returns>Returns whether the event is allowed or not.</returns>
        [PluginEvent(ServerEventType.PlayerCloseGenerator)]
        public bool OnClosingGenerator(IPlayer player, Scp079Generator generator)
        {
            ClosingGeneratorEventArgs ev = new(API.Features.Player.Get(player.ReferenceHub), generator);

            ClosingGenerator.InvokeSafely(ev);

            return ev.IsAllowed;
        }

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> turns on the generator by switching lever.
        /// </summary>
        /// <param name="player">The <see cref="PluginAPI.Core.Player"/> instance.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <returns>Returns whether the event is allowed or not.</returns>
        [PluginEvent(ServerEventType.PlayerActivateGenerator)]
        public bool OnActivatingGenerator(IPlayer player, Scp079Generator generator)
        {
            ActivatingGeneratorEventArgs ev = new(API.Features.Player.Get(player.ReferenceHub), generator);

            ActivatingGenerator.InvokeSafely(ev);

            return ev.IsAllowed;
        }

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> turns off the generator by switching lever.
        /// </summary>
        /// <param name="player">The <see cref="PluginAPI.Core.Player"/> instance.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <returns>Returns whether the event is allowed or not.</returns>
        [PluginEvent(ServerEventType.PlayerDeactivatedGenerator)]
        public bool OnStoppingGenerator(IPlayer player, Scp079Generator generator)
        {
            StoppingGeneratorEventArgs ev = new(API.Features.Player.Get(player.ReferenceHub), generator);

            StoppingGenerator.InvokeSafely(ev);

            return ev.IsAllowed;
        }
    }
}