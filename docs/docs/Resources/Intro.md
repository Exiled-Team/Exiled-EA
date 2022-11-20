---
sidebar_position: 1
---

### Index

- [RoleType, Team, Side, Faction](#roletype-team-side-and-faction)
- [ItemType](#itemtype)
- [AmmoType](#ammotype)
- [DoorType](#doortype)
- [RoomType](#roomtype)
- [ElevatorType](#elevatortype)
- [DamageType](#damagetype)
- [Damage Handlers](#damagehandlers)
- [EffectType](#effecttype)
- [Keycard Permissions](#keycardpermissions)
- [DoorLockType](#doorlocktype)
- [StructureType](#structuretype)
- [BloodType](#bloodtype)
- [GeneratorState](#generatorstate)
- [HotKeyButton](#hotkeybutton)
- [IntercomStates](#intercomstates)
- [BroadcastType](#broadcasttype)
- [Attachment Names](#attachmentnames)
- [Spawn Reasons](#spawnreasons)
- [Prefabs](#prefabs)

### External resources

- [Available Colors (en.scpslgame.com)](https://en.scpslgame.com/index.php/Docs:Permissions#Colors)

## Resources

### RoleType, Team, Side and Faction

<details><summary> <b>Roles</b></summary>

```md title="Latest Updated: 08/23/2021"
| Id  | RoleType       | Team | Side            | Faction         |
|-----|----------------|------|-----------------|-----------------|
| -1  | None           | RIP  | None            | Others          |
| 0   | Scp173         | SCP  | Scp             | SCP             |
| 1   | ClassD         | CDP  | ChaosInsurgency | FoundationEnemy |
| 2   | Spectator      | RIP  | None            | Others          |
| 3   | Scp106         | SCP  | Scp             | SCP             |
| 4   | NtfSpecialist  | MTF  | Mtf             | FoundationStaff |
| 5   | Scp049         | SCP  | Scp             | SCP             |
| 6   | Scientist      | RSC  | Mtf             | FoundationStaff |
| 7   | Scp079         | SCP  | Scp             | SCP             |
| 8   | ChaosConscript | CHI  | ChaosInsurgency | FoundationEnemy |
| 9   | Scp096         | SCP  | Scp             | SCP             |
| 10  | Scp0492        | SCP  | Scp             | SCP             |
| 11  | NtfSergeant    | MTF  | Mtf             | FoundationStaff |
| 12  | NtfCaptain     | MTF  | Mtf             | FoundationStaff |
| 13  | NtfPrivate     | MTF  | Mtf             | FoundationStaff |
| 14  | Tutorial       | TUT  | Tutorial        | Others          |
| 15  | FacilityGuard  | MTF  | Mtf             | FoundationStaff |
| 16  | Scp93953       | SCP  | Scp             | SCP             |
| 17  | Scp93989       | SCP  | Scp             | SCP             |
| 18  | ChaosRifleman  | CHI  | ChaosInsurgency | FoundationEnemy |
| 19  | ChaosRepressor | CHI  | ChaosInsurgency | FoundationEnemy |
| 20  | ChaosMarauder  | CHI  | ChaosInsurgency | FoundationEnemy |
```

</details>

### ItemType

<details><summary> <b>Items</b></summary>

```md  title="Latest Updated: 05/08/2022"
<Item>                        (<id>)
None                         -1
KeycardJanitor                0
KeycardScientist              1
KeycardResearchCoordinator    2
KeycardZoneManager            3
KeycardGuard                  4
KeycardNTFOfficer             5
KeycardContainmentEngineer    6
KeycardNTFLieutenant          7
KeycardNTFCommander           8
KeycardFacilityManager        9
KeycardChaosInsurgency       10
KeycardO5                    11
Radio                        12
GunCOM15                     13
Medkit                       14
Flashlight                   15
MicroHID                     16
SCP500                       17
SCP207                       18
Ammo12gauge                  19
GunE11SR                     20
GunCrossvec                  21
Ammo556x45                   22
GunFSP9                      23
GunLogicer                   24
GrenadeHE                    25
GrenadeFlash                 26
Ammo44cal                    27
Ammo762x39                   28
Ammo9x19                     29
GunCOM18                     30
SCP018                       31
SCP268                       32
Adrenaline                   33
Painkillers                  34
Coin                         35
ArmorLight                   36
ArmorCombat                  37
ArmorHeavy                   38
GunRevolver                  39
GunAK                        40
GunShotgun                   41
SCP330                       42
SCP2176                      43
SCP244a                      44
SCP244b                      45
```

</details>


### AmmoType

<details><summary> <b>Ammo</b></summary>

```md title="Latest Updated: 05/08/2022"
Nato9
Nato556
Nato762
Ammo12Gauge
Ammo44Cal
```

</details>

### DoorType

<details><summary> <b>Doors</b></summary>

```md title="Latest Updated: 05/08/2022"
UnknownDoor
Scp914Door
GR18Inner
Scp049Gate
Scp049Armory
Scp079First
Scp079Second
Scp096
Scp106Bottom
Scp106Primary
Scp106Secondary
Scp173Gate
Scp173Connector
Scp173Armory
Scp173Bottom
GR18Gate
Scp914Gate
CheckpointEntrance
CheckpointLczA
CheckpointLczB
EntranceDoor
EscapePrimary
EscapeSecondary
ServersBottom
GateA
GateB
HczArmory
HeavyContainmentDoor
HID
HIDLeft
HIDRight
Intercom
LczArmory
LczCafe
LczWc
LightContainmentDoor
NukeArmory
NukeSurface
PrisonDoor
SurfaceGate
Scp330
Scp330Chamber
```

</details>



### RoomType

<details><summary> <b>Rooms</b></summary>

```md title="Latest Updated: 05/08/2022"
Unknown
LczArmory
LczCurve
LczStraight
Lcz330
Lcz914
LczCrossing
LczTCross
LczCafe
LczPlants
LczToilets
LczAirlock
Lcz173
LczClassDSpawn
LczChkpB
LczGlassBox
LczChkpA
Hcz079
HczEzCheckpoint
HczArmory
Hcz939
HczHid
Hcz049
HczChkpA
HczCrossing
Hcz106
HczNuke
HczTesla
HczServers
HczChkpB
HczTCross
HczCurve
Hcz096
EzVent
EzIntercom
EzGateA
EzDownstairsPcs
EzCurve
EzPcs
EzCrossing
EzCollapsedTunnel
EzConference
EzStraight
EzCafeteria
EzUpstairsPcs
EzGateB
EzShelter
Pocket
Surface
```

</details>

### ElevatorType

<details><summary> <b>Elevators</b></summary>

```md title="Latest Updated: 05/08/2022"
Unknown
GateA
GateB
Nuke
Scp049
LczA
LczB
```

</details>

### DamageType

<details><summary> <b>DamageType</b></summary>

```md title="Latest Updated: 05/08/2022"
Unknown
Falldown
Warhead
Decontamination
Asphyxiation
Poison
Bleeding
MicroHid
Tesla
Scp
Explosion
Scp018
Scp207
Recontainment
Crushed
FemurBreaker
PocketDimension
FriendlyFireDetector
SeveredHands
Custom
Scp049
Scp0492
Scp096
Scp173
Scp106
Scp939
Crossvec
Logicer
Revolver
Shotgun
AK
Com15
Com18
Fsp9
E11Sr
Hypothermia
```

</details>

### DamageHandlers

<details><summary> <b>Damage Handlers</b></summary>

```md title="Latest Updated: 05/08/2022"
All available DamageHandlers

+ Symbol ':' literally means "inherits from"
* In C#, inheritance is a process in which one object acquires all the properties and behaviors of its parent object automatically.

PlayerStatsSystem::DamageHandlerBase
PlayerStatsSystem::StandardDamageHandler : DamageHandlerBase
PlayerStatsSystem::AttackerDamageHandler : StandardDamageHandler
PlayerStatsSystem::CustomReasonDamageHandler : StandardDamageHandler
PlayerStatsSystem::UniversalDamageHandler : StandardDamageHandler
PlayerStatsSystem::WarheadDamageHandler : StandardDamageHandler
PlayerStatsSystem::RecontainmentDamageHandler : AttackerDamageHandler
PlayerStatsSystem::FirearmDamageHandler : AttackerDamageHandler
PlayerStatsSystem::ScpDamageHandler : AttackerDamageHandler
PlayerStatsSystem::Scp096DamageHandler : AttackerDamageHandler
PlayerStatsSystem::MicroHidDamageHandler : AttackerDamageHandler
PlayerStatsSystem::ExplosionDamageHandler : AttackerDamageHandler
PlayerStatsSystem::Scp018DamageHandler : AttackerDamageHandler
```

</details>

### EffectType

<details><summary> <b>Effects</b></summary>

```md title="Latest Updated: 05/08/2022"
Amnesia
Asphyxiated
Bleeding
Blinded
Burned
Concussed
Corroding
Deafened
Decontaminating
Disabled
Ensnared
Exhausted
Flashed
Hemorrhage
Invigorated
BodyshotReduction
Poisoned
Scp207
Invisible
SinkHole
Visuals939
DamageReduction
MovementBoost
RainbowTaste
SeveredHands
Stained
Visual173Blink
Vitality
Hypothermia
Scp1853
```

</details>

### KeycardPermissions

<details><summary> <b>Keycard Perms</b></summary>

```md title="Latest Updated: 05/08/2022"
<0> None
<1> Checkpoints
<2> ExitGates
<4> Intercom
<8> AlphaWarhead
<16> ContainmentLevelOne
<32> ContainmentLevelTwo
<64> ContainmentLevelThree
<128> ArmoryLevelOne
<256> ArmoryLevelTwo
<512> ArmoryLevelThree
<1024> ScpOverride
```

</details>

### DoorLockType

<details><summary> <b>Lock Type</b></summary>

```md title="Latest Updated: 05/08/2022"
[0] <unlocked> None 
[1] Regular079 
[2] Lockdown079 
[4] Warhead 
[8] AdminCommand
[16] DecontLockdown
[32] DecontEvacuate
[64] SpecialDoorFeature
[128] NoPower
[256] Isolation
```

</details>

### StructureType

<details><summary> <b>Structures</b></summary>

```md title="Latest Updated: 05/08/2022"
WorkStation
LargeGunLocker
RifleRack
MiscLocker
Generator
RegularMedkit
AdrenalineMedkit
Scp018Pedestal
Scp207Pedestal
Scp244Pedestal
Scp268Pedestal
Scp500Pedestal
Scp1853Pedestal
Scp2176Pedestal
```

</details>

### BloodType

<details><summary> <b>Blood</b></summary>

```md title="Latest Updated: 02/13/2022"
Default
Scp106
Spreaded
Faded
```

</details>

### GeneratorState

<details><summary> <b>GeneratorState</b></summary>

```md title="Latest Updated: 02/13/2022"
[1] None
[2] Unlocked
[4] Open
[8] Activating
[16] Engaged
```

</details>

### HotKeyButton

<details><summary> <b>Hot Keys</b></summary>

```md title="Latest Updated: 02/13/2022"
Keycard
PrimaryFirearm
SecondaryFirearm
Medical
Grenade
```

</details>

### IntercomStates

<details><summary> <b>Intercom States</b></summary>

```md title="Latest Updated: 02/13/2022"
Ready
Transmitting
TransmittingBypass
Restarting
AdminSpeaking
Muted
Custom
```

</details>

### BroadcastType

<details><summary> <b>BroadcastType</b></summary>

```md title="Latest Updated: 02/13/2022"
Normal
Monospaced
AdminChat
```

</details>



### AttachmentNames

<details><summary> <b>Attachment Names</b></summary>

```md title="Latest Updated: 04/27/2022"
None
IronSights
DotSight
HoloSight
NightVisionSight
AmmoSight
ScopeSight
StandardStock
ExtendedStock
RetractedStock
LightweightStock
HeavyStock
RecoilReducingStock
Foregrip
Laser
Flashlight
AmmoCounter
StandardBarrel
ExtendedBarrel
SoundSuppressor
FlashHider
MuzzleBrake
MuzzleBooster
StandardMagFMJ
StandardMagAP
StandardMagJHP
ExtendedMagFMJ
ExtendedMagAP
ExtendedMagJHP
DrumMagFMJ
DrumMagAP
DrumMagJHP
LowcapMagFMJ
LowcapMagAP
LowcapMagJHP
CylinderMag4
CylinderMag6
CylinderMag8
CarbineBody
RifleBody
ShortBarrel
ShotgunChoke
ShotgunExtendedBarrel
NoRifleStock
ShotgunSingleShot
ShotgunDoubleShot
```

</details>

### SpawnReasons

<details><summary> <b>Spawn Reasons</b></summary>

```md title="Latest Updated: 02/13/2022"
None
RoundStart
LateJoin
Respawn
Died
Escaped
Revived
ForceClass
Overwatch

```

</details>

### Prefabs

<details><summary> <b>Available Prefabs</b></summary>

```md title="Latest Updated: 02/13/2022"
Guid                                 | Name

43658aa2-f339-6044-eb2b-937db0c2c4bd | Player  
5bfd1bbe-10a4-e184-4a2e-381314b3380c | PlaybackLobby  
9a77040d-663e-8a14-a8a2-297249bce483 | Pickup  
307eb9b0-d080-9dc4-78e6-673847876412 | Work Station  
0b58d568-fcd7-5384-abce-593a7931d65d | SCP-173_Ragdoll  
f602bb4b-88de-d554-5976-5c2e18af4479 | Ragdoll_1  
ea314e24-bddd-5264-5b08-dadd1bcfa75e | SCP-106_Ragdoll  
2b0290fb-6764-8f44-48ab-9294fe063c8f | Ragdoll_4  
05488a04-eda9-a724-18c9-bf2edbe23031 | Ragdoll_6  
e12d94d4-66ef-c734-2af0-aef522db57cb | Ragdoll_7  
9d7cf7ef-eec0-ece4-196c-4fd2c3cfd03a | Ragdoll_8  
e53f7b09-ad63-f924-6a96-0be4381af7f0 | SCP-096_Ragdoll  
be41bb5a-3b5f-bc84-4ad4-d4e24dfa168f | Ragdoll_10  
c87cf6f7-fc36-f144-6ae5-727c8c8f4b9b | Ragdoll_14  
b8d25875-6346-0314-68a9-7d1b7ec71167 | SCP-939-53_Ragdoll  
d2e872e1-1133-0984-186d-d3cdc686883f | SCP-939-89_Ragdoll  
c69da0e5-a829-6a04-c8d9-f404a1073cfe | Grenade Flash  
8063e113-c1f1-1514-7bc5-840ea8ee5f01 | Grenade Frag  
38f8296e-fcf4-44f4-491b-b5dc69b8125b | Grenade SCP-018  
33f5e0b4-fb1c-0134-493f-5d7aec09dc38 | EZ BreakableDoor  
5fbbe939-51c2-ef74-a9ed-bc0abfefa132 | HCZ BreakableDoor  
b82d6236-b9f5-33d4-e8ee-8ee33fba6edd | LCZ BreakableDoor  
3353122b-0ba2-5d14-fa64-886c45425967 | sportTargetPrefab  
422b08ed-0bc0-6cb4-7a7f-81dd37c430c0 | dboyTargetPrefab  
4f03f7fa-f417-ae84-382b-962c31614d1a | binaryTargetPrefab  
a0e7ee93-b802-e5a4-38bd-95e27cc133ea | TantrumObj  
43c40e13-5a2a-b3a4-9ba8-29c7002cedaf | Tutorial_Ragdoll  
bf9a7ae6-aaea-0174-d807-e0d4adb1c524 | PrimitiveObjectToy  
6996edbf-2adf-a5b4-e8ce-e089cf9710ae | LightSourceToy  
19b3629a-3298-8324-0ad0-e841def23244 | RegularKeycardPickup  
ef69975c-5a03-b9c4-fa26-0b6145b05824 | ChaosKeycardPickup  
8359dd57-d964-98c4-5871-586da0d50878 | RadioPickup  
52f9fa65-832f-b0f4-ab15-0ac33a45b853 | Com15Pickup  
06361fcf-1355-ea54-7a0b-d7a29244eae9 | MedkitPickup  
9902569b-0bc8-cf74-b814-a69789ed8c5a | FlashlightPickup  
35f6c267-d9b6-f5a4-4a87-5523b7424052 | MicroHidPickup  
30d95cc3-8b1f-bd14-4b66-f7350cf3bae9 | SCP500Pickup  
46572711-4d8b-f8a4-2a81-b1ca2ff15b5d | SCP207Pickup  
e7588f50-a788-bd44-89bf-f9dae4ab2071 | Ammo12gaPickup  
9958e2c0-668f-9f14-c9ed-1cd97281f3d3 | E11SRPickup  
7a39d145-d2d1-5724-7ad5-660cbe2f5757 | CrossvecPickup  
0282bdfe-9880-d284-1807-2d4e11fc540d | Ammo556mmPickup  
d32145e1-e7d9-d674-fbaa-078247910c49 | Fsp9Pickup  
4ce1ab59-83ff-aa14-db7a-65e79c48cf8e | LogicerPickup  
3f98e495-a544-11b4-dbc3-a03797786f52 | HegPickup  
6e4bfac7-e1c9-9af4-9a76-c025cc8bbb37 | FlashbangPickup  
8627c2a9-e397-2164-08dd-97f9fddab207 | Ammo44calPickup  
ecba736b-7b69-0f14-ea94-7c9067dc7ea8 | Ammo762mmPickup  
89a36c3a-be6b-5914-7b75-1287c79f19dc | Ammo9mmPickup  
2a12ef7e-b39d-ed34-6979-571e541231b1 | Com18Pickup  
a1d0c7dd-6523-8a34-3b4a-5124f47b93dd | Scp018Projectile  
6fbfc036-04fb-1f94-7af0-1335064c0198 | SCP268Pickup  
9695f1b9-46d6-7054-c9af-a35a4fefafe1 | AdrenalinePrefab  
9925eed6-900f-7444-880f-393468fa1a63 | PainkillersPickup  
522f199f-ce6f-5814-9a67-f0191d0110a9 | CoinPickup  
51703b4d-a309-11c4-8af7-bdb8d95214c0 | Light Armor Pickup  
02e10b6d-9d4d-ed14-2b8b-f5219522da77 | Combat Armor Pickup  
19d03dd5-b491-acc4-ea16-be8ad5a33783 | Heavy Armor Pickup  
635a3623-281c-e5c4-297d-7f07cd6a0eef | RevolverPickup  
1821b416-953c-98f4-c9b8-09d2c192b8b1 | AkPickup  
d6abff39-0c5c-1804-58de-ac4478538837 | ShotgunPickup  
65141804-5071-27e4-c8c0-23c547ce629c | Scp330Pickup  
830e7527-1f40-d0d4-3a3e-ff49f5a6176c | Scp2176Projectile  
2401ec76-dce3-cf34-b858-7a9c7dc83b0b | SCP244APickup Variant  
39825db8-2df8-eed4-caa5-a4c334c669a0 | SCP244BPickup Variant  
68f13209-e652-6024-2b89-0f75fb88a998 | Scp268PedestalStructure Variant  
17054030-9461-d104-5b92-9456c9eb0ab7 | Scp207PedestalStructure Variant  
f4149b66-c503-87a4-0b93-aabfe7c352da | Scp500PedestalStructure Variant  
a149d3eb-11bd-de24-f9dd-57187f5771ef | Scp018PedestalStructure Variant  
5ad5dc6d-7bc5-3154-8b1a-3598b96e0d5b | LargeGunLockerStructure  
850f84ad-e273-1824-8885-11ae5e01e2f4 | RifleRackStructure  
d54bead1-286f-3004-facd-74482a872ad8 | MiscLocker  
daf3ccde-4392-c0e4-882d-b7002185c6b8 | GeneratorStructure  
ad8a455f-062d-dea4-5b47-ac9217d4c58b | Spawnable Work Station Structure  
5b227bd2-1ed2-8fc4-2aa1-4856d7cb7472 | RegularMedkitStructure  
db602577-8d4f-97b4-890b-8c893bfcd553 | AdrenalineMedkitStructure  
fff1c10c-a719-bea4-d95c-3e262ed03ab2 | Scp2176PedestalStructure Variant  
53cd67d2-995b-3374-4892-4190ffd48ee9 | HegProjectile  
2a6e5abb-7999-b8d4-a926-310e3e9e2a13 | FlashbangProjectile
```

</details>

