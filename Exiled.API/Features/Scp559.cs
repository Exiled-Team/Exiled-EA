// -----------------------------------------------------------------------
// <copyright file="Scp559.cs" company="Exiled Team">
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

    using MapGeneration;
    using UnityEngine;

    /// <summary>
    /// A set of tools to modify SCP-559's behaviour.
    /// </summary>
    public static class Scp559
    {
        /// <summary>
        /// Gets the cached <see cref="global::Scp559Cake"/>.
        /// </summary>
        public static Scp559Cake Scp559Cake => Scp559Cake._singleton;

        /// <summary>
        /// Gets the different.
        /// </summary>
        public static IEnumerable<Vector3> Spawnpoints => Scp559Cake.PossiblePositions.Select(x => new Vector3(x.x, x.y, x.z));
    }
}
