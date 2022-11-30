// -----------------------------------------------------------------------
// <copyright file="RoomType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of rooms.
    /// </summary>
    /// <seealso cref="Features.Room.Type"/>
    /// <seealso cref="Features.Room.Get(RoomType)"/>
    public enum RoomType
    {
        /// <summary>
        /// Unnamed Room Type.
        /// </summary>
        Unnamed,

        /// <summary>
        /// Lower Containment Class-D Spawn Room.
        /// </summary>
        LczClassDSpawn,

        /// <summary>
        /// Entrance PC Room With Upstairs.
        /// </summary>
        LczComputerRoom,

        /// <summary>
        /// Lower Containment Checkpoint A Room.
        /// </summary>
        LczCheckpointA,

        /// <summary>
        /// Lower Containment Checkpoint B Room.
        /// </summary>
        LczCheckpointB,

        /// <summary>
        /// Lower Containment Toilets Room.
        /// </summary>
        LczToilets,

        /// <summary>
        /// Lower Containment Armory.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Lower Containment SCP-173 Room.
        /// </summary>
        Lcz173,

        /// <summary>
        /// Lower Containment Glass Box Room.
        /// </summary>
        LczGlassroom,

        /// <summary>
        /// Lower Containment SCP-330 Room.
        /// </summary>
        Lcz330,

        /// <summary>
        /// Lower Containment SCP-914 Room.
        /// </summary>
        Lcz914,

        /// <summary>
        /// Represents the greenhouse room in LCZ.
        /// </summary>
        LczGreenhouse,

        /// <summary>
        /// Lower Containment Airlock Room.
        /// </summary>
        LczAirlock,

        /// <summary>
        /// Heavy Containment Entrance Checkpoint Room.
        /// </summary>
        HczCheckpointToEntranceZone,

        /// <summary>
        /// Heavy Containment Checkpoint A Room.
        /// </summary>
        HczCheckpointA,

        /// <summary>
        /// Heavy Containment Checkpoint B Room.
        /// </summary>
        HczCheckpointB,

        /// <summary>
        /// Heavy Containment Nuke Room.
        /// </summary>
        HczWarhead,

        /// <summary>
        /// Heavy Containment SCP-049 Room.
        /// </summary>
        Hcz049,

        /// <summary>
        /// Heavy Containment SCP-079 Room.
        /// </summary>
        Hcz079,

        /// <summary>
        /// Heavy Containment SCP-096 Room.
        /// </summary>
        Hcz096,

        /// <summary>
        /// Heavy Containment SCP-106 Room.
        /// </summary>
        Hcz106,

        /// <summary>
        /// Heavy Containment SCP-939 Room.
        /// </summary>
        Hcz939,

        /// <summary>
        /// Heavy Containment HID-Spawn Room.
        /// </summary>
        HczMicroHID,

        /// <summary>
        /// Heavy Containment T-Shaped Armory Room.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Heavy Containment Servers Room.
        /// </summary>
        HczServers,

        /// <summary>
        /// Heavy Containment Tesla Room.
        /// </summary>
        HczTesla,

        /// <summary>
        /// Entrance Red Collapsed Tunnel Room.
        /// </summary>
        EzCollapsedTunnel,

        /// <summary>
        /// Entrance Gate A Room.
        /// </summary>
        EzGateA,

        /// <summary>
        /// Entrance Gate B Room.
        /// </summary>
        EzGateB,

        /// <summary>
        /// Entrance Red Room.
        /// </summary>
        EzRedroom,

        /// <summary>
        /// Entrance Shelter Room.
        /// </summary>
        EzEvacShelter,

        /// <summary>
        /// Entrance Intercom Room.
        /// </summary>
        EzIntercom,

        /// <summary>
        /// Entrance Storied Office Room.
        /// </summary>
        EzOfficeStoried,

        /// <summary>
        /// Entrance Large Office Room.
        /// </summary>
        EzOfficeLarge,

        /// <summary>
        /// Entrance Small Office Room.
        /// </summary>
        EzOfficeSmall,

        /// <summary>
        /// The Surface.
        /// </summary>
        Surface,

        /// <summary>
        /// Pocket Dimension.
        /// </summary>
        Pocket,

        /// <summary>
        /// Heavy Containment Test Room.
        /// </summary>
        HczTestroom,

        /// <summary>
        /// Lower Containment L-Shaped Room.
        /// </summary>
        LczCurve,

        /// <summary>
        /// Lower Containment |-Shaped Room.
        /// </summary>
        LczStraight,

        /// <summary>
        /// Lower Containment X-Shaped Room.
        /// </summary>
        LczCrossing,

        /// <summary>
        /// Lower Containment T-Shaped Room.
        /// </summary>
        LczTCross,

        /// <summary>
        /// Lower Containment Cafe Room.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Lower Containment T-Shaped Plants Room.
        /// </summary>
        LczPlants,

        /// <summary>
        /// Heavy Containment X-Shaped Room.
        /// </summary>
        HczCrossing,

        /// <summary>
        /// Heavy Containment T-Shaped Room.
        /// </summary>
        HczTCross,

        /// <summary>
        /// Heavy Containment L-Shaped Room.
        /// </summary>
        HczCurve,

        /// <summary>
        /// Entrance Red Vent Room.
        /// </summary>
        EzVent,

        /// <summary>
        /// Entrance PC Room With Downstairs.
        /// </summary>
        EzDownstairsPcs,

        /// <summary>
        /// Entrance L-Shaped Room.
        /// </summary>
        EzCurve,

        /// <summary>
        /// Entrance PC Room.
        /// </summary>
        EzPcs,

        /// <summary>
        /// Entrance X-Shaped Room.
        /// </summary>
        EzCrossing,

        /// <summary>
        /// Entrance |-Shaped Dr.L Room.
        /// </summary>
        EzConference,

        /// <summary>
        /// Entrance |-Shaped Room
        /// </summary>
        EzStraight,

        /// <summary>
        /// Entrance Cafeteria Room.
        /// </summary>
        EzCafeteria,

        /// <summary>
        /// Heavy Containment |-Shaped Room.
        /// </summary>
        HczStraight,

        /// <summary>
        /// Entrance T-Shaped Room.
        /// </summary>
        EzTCross,
    }
}