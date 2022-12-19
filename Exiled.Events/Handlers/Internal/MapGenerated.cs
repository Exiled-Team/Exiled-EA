// -----------------------------------------------------------------------
// <copyright file="MapGenerated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.Items;
    using API.Structs;
    using Exiled.API.Extensions;
    using Interactables.Interobjects;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using MapGeneration;
    using MapGeneration.Distributors;
    using MEC;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using Utils.NonAllocLINQ;

    using Broadcast = Broadcast;
    using Camera = API.Features.Camera;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles <see cref="Handlers.Map.Generated"/> event.
    /// </summary>
    internal static class MapGenerated
    {
        /// <summary>
        /// Called once the map is generated.
        /// </summary>
        /// <remarks>
        /// This fixes an issue where
        /// all those extensions that
        /// require calling the central
        /// property of the Map class in
        /// the API were corrupted due to
        /// a missed call, such as before
        /// getting the elevator type.
        /// </remarks>
        public static void OnMapGenerated()
        {
            Map.ClearCache();
            Timing.CallDelayed(0.25f, GenerateCache);
        }

        private static void GenerateCache()
        {
            Warhead.SitePanel = Object.FindObjectOfType<AlphaWarheadNukesitePanel>();
            Warhead.OutsitePanel = Object.FindObjectOfType<AlphaWarheadOutsitePanel>();
            Server.Host = new Player(ReferenceHub.HostHub);
            Server.Broadcast = ReferenceHub.HostHub.GetComponent<Broadcast>();

            GenerateCamera();
            GenerateTeslaGates();
            GenerateRooms();
            GenerateWindows();
            GenerateLifts();
            GeneratePocketTeleports();
            GenerateAttachments();
            GenerateLockers();

            Map.AmbientSoundPlayer = ReferenceHub.HostHub.GetComponent<AmbientSoundPlayer>();

            Handlers.Map.OnGenerated();

            Timing.CallDelayed(0.1f, Handlers.Server.OnWaitingForPlayers);
        }

        private static void GenerateRooms()
        {
            // Get bulk of rooms with sorted.
            List<RoomIdentifier> roomIdentifiers = ListPool<RoomIdentifier>.Shared.Rent(RoomIdentifier.AllRoomIdentifiers);

            // If no rooms were found, it means a plugin is trying to access this before the map is created.
            if (roomIdentifiers.Count == 0)
                throw new InvalidOperationException("Plugin is trying to access Rooms before they are created.");

            foreach (RoomIdentifier roomIdentifier in roomIdentifiers)
                Room.RoomIdentifierToRoom.Add(roomIdentifier, Room.CreateComponent(roomIdentifier.gameObject));

            ListPool<RoomIdentifier>.Shared.Return(roomIdentifiers);
        }

        private static void GenerateWindows()
        {
            foreach (BreakableWindow breakableWindow in Object.FindObjectsOfType<BreakableWindow>())
                new Window(breakableWindow);
        }

        private static void GenerateLifts()
        {
            foreach (ElevatorChamber elevatorChamber in Object.FindObjectsOfType<ElevatorChamber>())
                new Lift(elevatorChamber);
        }

        private static void GenerateCamera()
        {
            foreach (Scp079Camera camera079 in Object.FindObjectsOfType<Scp079Camera>())
                new Camera(camera079);
        }

        private static void GenerateTeslaGates()
        {
            foreach (global::TeslaGate teslaGate in TeslaGateController.Singleton.TeslaGates)
                new TeslaGate(teslaGate);
        }

        private static void GeneratePocketTeleports() => Map.TeleportsValue.AddRange(Object.FindObjectsOfType<PocketDimensionTeleport>());

        private static void GenerateLockers() => Map.LockersValue.AddRange(Object.FindObjectsOfType<Locker>());

        private static void GenerateAttachments()
        {
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                if (!type.IsWeapon(false))
                    continue;

                if (Item.Create(type) is not Firearm firearm)
                    continue;

                Firearm.ItemTypeToFirearmInstance.Add(type, firearm);

                List<AttachmentIdentifier> attachmentIdentifiers = new();
                HashSet<AttachmentSlot> attachmentsSlots = new();

                uint code = 1;

                foreach (Attachment attachment in firearm.Attachments)
                {
                    attachmentsSlots.Add(attachment.Slot);
                    attachmentIdentifiers.Add(new(code, attachment.Name, attachment.Slot));
                    code *= 2U;
                }

                uint baseCode = 0;

                attachmentsSlots
                    .ForEach(slot => baseCode += attachmentIdentifiers
                    .Where(attachment => attachment.Slot == slot)
                    .Aggregate((curMin, nextEntry) => nextEntry.Code < curMin.Code ? nextEntry : curMin));

                Firearm.BaseCodesValue.Add(type, baseCode);
                Firearm.AvailableAttachmentsValue.Add(type, attachmentIdentifiers.ToArray());
            }
        }
    }
}