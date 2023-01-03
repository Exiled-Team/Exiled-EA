// -----------------------------------------------------------------------
// <copyright file="ComponentsEqualityComparer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// Represents the equality comparer for unity components.
    /// </summary>
    internal class ComponentsEqualityComparer : IEqualityComparer<Component>
    {
        /// <inheritdoc/>
        public bool Equals(Component x, Component y)
        {
            return x == y;
        }

        /// <inheritdoc/>
        public int GetHashCode(Component obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
