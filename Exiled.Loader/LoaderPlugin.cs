// -----------------------------------------------------------------------
// <copyright file="LoaderPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.IO;
    using System.Reflection;

    using NorthwoodLib;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;

    /// <summary>
    /// The PluginAPI Plugin class for the EXILED Loader.
    /// </summary>
    public class LoaderPlugin
    {
#pragma warning disable SA1401
        /// <summary>
        /// The config for the EXILED Loader.
        /// </summary>
        [PluginConfig]
        public static Config Config;
#pragma warning restore SA1401

        private static Loader loader;

        /// <summary>
        /// Called by PluginAPI when the plugin is enabled.
        /// </summary>
        [PluginEntryPoint("Exiled Loader", null, "Loads the EXILED Plugin Framework.", "Exiled-Team")]
        public void Enable()
        {
            if (!Config.IsEnabled)
            {
                Log.Info("EXILED is disabled on this server via config.");
                return;
            }

            /* TODO: Implement this with a dictionary of Game versions and the exiled versions it is compatible with.
            if (GameCore.Version.VersionString != "your mom")
            {
                Log.Error("EXILED is not compatible with this version of the game, aborting...");
                return;
            }*/

            Log.Info($"Loading EXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

            string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED");

            if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
            {
                Log.Warning("Switching root patch to EXILED-Testing.");
                rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");
            }

            string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            if (!File.Exists(Path.Combine(dependenciesPath, "Exiled.API.dll")))
            {
                Log.Error($"[Exiled.Loader] Exiled.API.dll was not found, Exiled won't be loaded!", "Exiled.Loader");
                return;
            }

            if (!File.Exists(Path.Combine(dependenciesPath, "YamlDotNet.dll")))
            {
                ServerConsole.AddLog($"[Exiled.Loader] YamlDotNet.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                return;
            }

            loader = new Loader();

            Log.Info("Calling run");

            loader.Run();
        }
    }
}