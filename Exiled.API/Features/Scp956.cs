// -----------------------------------------------------------------------
// <copyright file="Scp956.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A set of tools to modify SCP-956's behaviour.
    /// </summary>
    public static class Scp956
    {
        /// <summary>
        /// Gets get all the Targets of Scp956.
        /// </summary>
        public static IEnumerable<Player> ActiveTargets => Scp956Pinata.ActiveTargets.Select(x => Player.Get(x));

        /// <summary>
        /// Gets get all the Compatible door for Scp956.
        /// </summary>
        public static IEnumerable<Door> CompatibleDoors => Scp956Pinata.CompatibleDoors.Select(x => Door.Get(x));
    }
}
