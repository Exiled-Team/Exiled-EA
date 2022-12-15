// -----------------------------------------------------------------------
// <copyright file="Camera.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using UnityEngine;

    using CameraType = Enums.CameraType;

    /// <summary>
    /// The in-game Scp079Camera.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="Scp079Camera"/>s and their corresponding <see cref="Camera"/>.
        /// </summary>
        internal static readonly Dictionary<Scp079Camera, Camera> Camera079ToCamera = new(250);

        private static readonly Dictionary<string, CameraType> NameToCameraType = new()
        {
            // Light Containment
            ["173 chamber"] = CameraType.Lcz173Chamber,
            ["173 hallway"] = CameraType.Lcz173Hallway,
            ["173 gunroom"] = CameraType.Lcz173Armory,
            ["914 hallway"] = CameraType.Lcz914Hallway,
            ["airlock"] = CameraType.LczAirlock,
            ["armory"] = CameraType.LczArmory,
            ["d cells"] = CameraType.LczClassDSpawn,
            ["etrcp @ a"] = CameraType.LczBEntrance,
            ["etrcp @ b"] = CameraType.LczAEntrance,
            ["ex @ a"] = CameraType.HczBEntrance,
            ["ex @ b"] = CameraType.HczAEntrance,
            ["glassroom"] = CameraType.LczGlassRoom,
            ["greenhouse"] = CameraType.LczGreenhouse,
            ["lcz @ a"] = CameraType.LczALifts,
            ["lcz @ b"] = CameraType.LczBLifts,
            ["scp-173 stairs"] = CameraType.Lcz173Bottom,
            ["scp-914"] = CameraType.Lcz914,
            ["tc-01 chamber"] = CameraType.Lcz330Chamber,
            ["tc-01 hall"] = CameraType.Lcz330Hall,
            ["wc"] = CameraType.LczWC,

            // Heavy Containment
            ["049 hall 1"] = CameraType.Hcz049Hall,
            ["049 hall 2"] = CameraType.Hcz049Hall,
            ["049 hall 3"] = CameraType.Hcz049Hall,
            ["049 hall 4"] = CameraType.Hcz049Hall,
            ["049 hall 5"] = CameraType.Hcz049Armory,
            ["106 ent a"] = CameraType.Hcz106Primary,
            ["106 ent b"] = CameraType.Hcz106Secondary,
            ["106 stairway"] = CameraType.Hcz106Stairs,
            ["downservs"] = CameraType.HczServerBottom,
            ["ez entrance"] = CameraType.EzEntrance,
            ["hcz @ a"] = CameraType.HczALifts,
            ["hcz @ b"] = CameraType.HczBLifts,
            ["hcz armory"] = CameraType.HczArmory,
            ["hcz entrance"] = CameraType.HczEntrance,
            ["hallway cam"] = CameraType.Hcz079Hallway,
            ["head armory"] = CameraType.HczWarheadArmory,
            ["head panel"] = CameraType.HczWarheadSwitch,
            ["head top"] = CameraType.HczWarheadRoom,
            ["hid hall"] = CameraType.HczHidHall,
            ["hid interior"] = CameraType.HczHidInterior,
            ["pre-hallway cam"] = CameraType.Hcz079PreHallway,
            ["sacrificer"] = CameraType.Hcz106Recontainer,
            ["servers"] = CameraType.HczServerTop,
            ["servhall"] = CameraType.HczServerHall,
            ["scp-049 hall"] = CameraType.Hcz049Elevator,
            ["scp-079 control"] = CameraType.Hcz079Control,
            ["scp-079 main cam"] = CameraType.Hcz079Main,
            ["scp-096 cr"] = CameraType.Hcz096,
            ["scp-106 main cam"] = CameraType.Hcz106First,
            ["scp-106 second"] = CameraType.Hcz106Second,
            ["scp-939 cr"] = CameraType.Hcz939,
            ["tesla gate"] = CameraType.HczTeslaGate,
            ["warhead hall"] = CameraType.HczWarheadHall,

            // Entrance Zone
            ["hallway"] = CameraType.EzHall,
            ["icom hall"] = CameraType.EzIntercomHall,
            ["icom room"] = CameraType.EzIntercomInterior,
            ["intercom"] = CameraType.EzIntercomHall,
            ["t intersection"] = CameraType.EzTIntersection,
            ["x intersection"] = CameraType.EzXIntersection,

            // Surface
            ["bridge"] = CameraType.Bridge,
            ["backstreet"] = CameraType.Backstreet,
            ["exit"] = CameraType.Exit,
            ["escape zone"] = CameraType.EscapeZone,
            ["helipad"] = CameraType.Helipad,
            ["streetcam"] = CameraType.Streetcam,
            ["tower"] = CameraType.Tower,

            // Unspecified
            ["corner"] = CameraType.Corner,
            ["x-type inters"] = CameraType.XIntersection,
            ["t-type inters"] = CameraType.TIntersection,
            ["straight"] = CameraType.Hallway,
            ["offices"] = CameraType.Office,
            ["gate a"] = CameraType.GateA,
            ["gate b"] = CameraType.GateB,
        };

        private Room room;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="camera079">The base camera.</param>
        internal Camera(Scp079Camera camera079) => Base = camera079;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the <see cref="Camera"/> instances.
        /// </summary>
        public static IEnumerable<Camera> List => Camera079ToCamera.Values;

        /// <summary>
        /// Gets a random <see cref="Camera"/>.
        /// </summary>
        /// <returns><see cref="Camera"/> object.</returns>
        public static Camera Random => List.ElementAt(UnityEngine.Random.Range(0, List.Count()));

        /// <summary>
        /// Gets the base <see cref="Scp079Camera"/>.
        /// </summary>
        public Scp079Camera Base { get; }

        /// <summary>
        /// Gets the camera's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the camera's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the camera's name.
        /// </summary>
        public string Name => Base.Label;

        /// <summary>
        /// Gets the camera's id.
        /// </summary>
        public ushort Id => Base.SyncId;

        /// <summary>
        /// Gets the generator's <see cref="Room"/>.
        /// </summary>
        public Room Room => room ??= Room.Get(Base.Room);

        /// <summary>
        /// Gets the camera's <see cref="ZoneType"/>.
        /// </summary>
        public ZoneType Zone => Room.Zone;

        /// <summary>
        /// Gets the camera's <see cref="CameraType"/>.
        /// </summary>
        public CameraType Type
        {
            get
            {
                string cameraName = Name.ToLower();

                if (NameToCameraType.ContainsKey(cameraName))
                    return NameToCameraType[cameraName];

                if (Room is null)
                    return CameraType.Unknown;

                return cameraName switch
                {
                    "corner" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.LczCorner,
                        ZoneType.HeavyContainment => CameraType.HczCorner,
                        ZoneType.Entrance => CameraType.EzCorner,
                        _ => CameraType.Unknown,
                    },
                    "x-type inters" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.LczXIntersection,
                        ZoneType.HeavyContainment => CameraType.HczXIntersection,
                        _ => CameraType.Unknown,
                    },
                    "t-type inters" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.LczTIntersection,
                        ZoneType.HeavyContainment => CameraType.HczTIntersection,
                        _ => CameraType.Unknown,
                    },
                    "straight" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.LczHall,
                        ZoneType.HeavyContainment => CameraType.HczHall,
                        _ => CameraType.Unknown,
                    },
                    "offices" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.LczCafe,
                        ZoneType.HeavyContainment => CameraType.EzOffice,
                        _ => CameraType.Unknown,
                    },
                    "gate a" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.EzGateA,
                        ZoneType.HeavyContainment => CameraType.SurfaceGateA,
                        _ => CameraType.Unknown,
                    },
                    "gate b" => Zone switch
                    {
                        ZoneType.LightContainment => CameraType.EzGateB,
                        ZoneType.HeavyContainment => CameraType.SurfaceGate,
                        _ => CameraType.Unknown,
                    },
                    _ => CameraType.Unknown,
                };
            }
        }

        /// <summary>
        /// Gets the camera's position.
        /// </summary>
        public Vector3 Position => Base.Position;

        /// <summary>
        /// Gets or sets the camera's rotation.
        /// </summary>
        public Vector3 Rotation
        {
            get => Base._cameraAnchor.rotation.eulerAngles;
            set => Base._cameraAnchor.rotation = Quaternion.Euler(value);
        }

        /// <summary>
        /// Gets the value of the <see cref="Camera"/> zoom.
        /// </summary>
        public float Zoom => Base.ZoomAxis.CurrentZoom;

        /// <summary>
        /// Gets or sets a value indicating whether this camera is being used by SCP-079.
        /// </summary>
        public bool IsBeingUsed
        {
            get => Base.IsActive;
            set => Base.IsActive = value;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the <see cref="Camera"/> instances given a <see cref="IEnumerable{T}"/> of <see cref="Scp079Camera"/>.
        /// </summary>
        /// <param name="cameras">The <see cref="IEnumerable{T}"/> of <see cref="Scp079Camera"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/>.</returns>
        public static IEnumerable<Camera> Get(IEnumerable<Scp079Camera> cameras) => cameras.Select(Get);

        /// <summary>
        /// Gets the <see cref="Camera"/> belonging to the <see cref="Scp079Camera"/>, if any.
        /// </summary>
        /// <param name="camera079">The base <see cref="Scp079Camera"/>.</param>
        /// <returns>A <see cref="Camera"/> or <see langword="null"/> if not found.</returns>
        public static Camera Get(Scp079Camera camera079) => List.FirstOrDefault(camera => camera.Base == camera079) ?? new Camera(camera079);

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified id.
        /// </summary>
        /// <param name="cameraId">The camera id to be searched for.</param>
        /// <returns>The <see cref="Camera"/> with the given id or <see langword="null"/> if not found.</returns>
        public static Camera Get(uint cameraId) => List.FirstOrDefault(camera => camera.Id == cameraId);

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified name.
        /// </summary>
        /// <param name="cameraName">The name of the camera.</param>
        /// <returns>The <see cref="Camera"/> or <see langword="null"/> if not found.</returns>
        public static Camera Get(string cameraName) => List.FirstOrDefault(camera => camera.Name == cameraName);

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <see cref="CameraType"/>.
        /// </summary>
        /// <param name="cameraType">The <see cref="CameraType"/> to search for.</param>
        /// <returns>The <see cref="Camera"/> with the given <see cref="CameraType"/> or <see langword="null"/> if not found.</returns>
        public static Camera Get(CameraType cameraType) => List.FirstOrDefault(camera => camera.Type == cameraType);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Camera> Get(Func<Camera, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Returns the Camera in a human-readable format.
        /// </summary>
        /// <returns>A string containing Camera-related data.</returns>
        public override string ToString() => $"{Zone} ({Type}) [{Room}] *{Name}* |{Id}| ={IsBeingUsed}=";
    }
}