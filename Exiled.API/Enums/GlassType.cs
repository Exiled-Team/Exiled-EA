// -----------------------------------------------------------------------
// <copyright file="GlassType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    /// <summary>
    /// Unique identifier for the different types of Window.
    /// </summary>
    /// <seealso cref="Window.Type"/>
    public enum GlassType
    {
        /// <summary>
        /// Represents an unknown Window.
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.LczGlassBox"/>.
        /// </summary>
        GR18,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.Hcz049"/>.
        /// </summary>
        Scp049,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.HczHid"/>.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.Hcz079"/>.
        /// </summary>
        Scp079,

        /// <summary>
        /// Represents the <see cref="Recontainer.ActivatorWindow"/>.
        /// </summary>
        Scp079Trigger,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.Lcz330"/>.
        /// </summary>
        Scp330,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.LczPlants"/>.
        /// </summary>
        Plants,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.HczEzCheckpoint"/>.
        /// </summary>
        HczEzCheckpoint,

        /// <summary>
        /// Represents all the Window in <see cref="RoomType.HczTestRoom"/>.
        /// </summary>
        TestRoom,

    }
}