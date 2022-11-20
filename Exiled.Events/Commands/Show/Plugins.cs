// -----------------------------------------------------------------------
// <copyright file="Plugins.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Show
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using API.Interfaces;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    /// <summary>
    /// The command to show all plugins.
    /// </summary>
    public sealed class Plugins : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "plugins";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "sp", "showplugins" };

        /// <inheritdoc/>
        public string Description { get; } = "Get all plugins, names, authors and versions";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            const string perm = "ee.showplugins";
            if (!sender.CheckPermission(perm) && sender is PlayerCommandSender playerSender && !playerSender.QueryProcessor._roles.RaEverywhere)
            {
                response = $"You can't get a list of all plugins, you don't have \"{perm}\" permissions.";
                return false;
            }

            StringBuilder sb = StringBuilderPool.Shared.Rent();

            // Append a new line to start the response on a new line
            sb.AppendLine();

            SortedSet<IPlugin<IConfig>> plugins = Loader.Loader.Plugins;
            int enabledPluginCount = plugins.Where(plugin => plugin.Config.IsEnabled).Count();

            // Append two new lines before the list
            sb.Append("Total number of plugins: ").Append(plugins.Count).AppendLine()
                .Append("Enabled plugins: ").Append(enabledPluginCount).AppendLine()
                .Append("Disabled plugins: ").Append(plugins.Count - enabledPluginCount)
                .AppendLine().AppendLine();

            StringBuilder AppendNewRow() => sb.AppendLine().Append("\t");

            for (int z = 0; z < plugins.Count; z++)
            {
                IPlugin<IConfig> plugin = plugins.ElementAt(z);

                sb.Append(string.IsNullOrEmpty(plugin.Name) ? "(Unknown)" : plugin.Name).Append(":");

                if (!plugin.Config.IsEnabled)
                {
                    AppendNewRow().Append("- Disabled");
                }

                AppendNewRow().Append("- Author: ").Append(plugin.Author);
                AppendNewRow().Append("- Version: ").Append(plugin.Version);
                AppendNewRow().Append("- Required Exiled Version: ").Append(plugin.RequiredExiledVersion);
                AppendNewRow().Append("- Prefix: ").Append(plugin.Prefix);
                AppendNewRow().Append("- Priority: ").Append(plugin.Priority);

                // Finalize a plugin row if it's not the end
                if (z + 1 != plugins.Count)
                    sb.AppendLine();
            }

            response = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return true;
        }
    }
}