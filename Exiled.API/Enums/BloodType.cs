// -----------------------------------------------------------------------
// <copyright file="BloodType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of blood decals.
    /// </summary>
    /// <seealso cref="Features.Map.PlaceBlood(UnityEngine.Vector3, BloodType, float)"/>
    /// <seealso cref="Features.Player.PlaceBlood(BloodType, float)"/>
    public enum BloodType
    {
        /// <summary>
        /// The default blood decal.
        /// </summary>
        Default,

        /// <summary>
        /// The blood decal placed after Scp106 sends someone to the pocket dimension.
        /// </summary>
        Scp106,

        /// <summary>
        /// The spreaded blood decal.
        /// </summary>
        Spreaded,

        /// <summary>
        /// The faded blood decal.
        /// </summary>
        Faded,
    }
}