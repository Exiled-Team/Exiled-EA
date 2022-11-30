// -----------------------------------------------------------------------
// <copyright file="RoomExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Enums;

    /// <summary>
    /// A set of extensions for <see cref="RoomType"/>.
    /// </summary>
    public static class RoomExtensions
    {
        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a gate.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a gate or not.</returns>
        public static bool IsGate(this RoomType room) => room is RoomType.EzGateA or RoomType.EzGateB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a checkpoint or not.</returns>
        public static bool IsCheckpoint(this RoomType room) => room is RoomType.LczCheckpointA
            or RoomType.LczCheckpointB or RoomType.HczCheckpointToEntranceZone
            or RoomType.HczCheckpointA or RoomType.HczCheckpointB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a Lcz checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a Lcz checkpoint or not.</returns>
        public static bool IsLczCheckpoint(this RoomType room) => room is RoomType.LczCheckpointA or RoomType.LczCheckpointB;

        /// <summary>
        /// Checks if a <see cref="RoomType">room type</see> is a Hcz checkpoint.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> is a Hcz checkpoint or not.</returns>
        public static bool IsHczCheckpoint(this RoomType room) => room is RoomType.HczCheckpointA or RoomType.HczCheckpointB;

        /// <summary>
        ///  Checks if a <see cref="RoomType">room type</see> contains any SCP.
        /// </summary>
        /// <param name="room">The room to be checked.</param>
        /// <returns>Returns whether the <see cref="RoomType"/> contains any SCP or not.</returns>
        public static bool IsScp(this RoomType room)
            => room is RoomType.Lcz173 or RoomType.Lcz330 or RoomType.Lcz914 or RoomType.Hcz049 or RoomType.Hcz079 or
                RoomType.Hcz096 or RoomType.Hcz106 or RoomType.Hcz939;
    }
}