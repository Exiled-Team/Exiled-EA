// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Interfaces;

    /// <summary>
    /// A set of tools to print messages on the server console.
    /// </summary>
    public static class Log
    {
        private static readonly Dictionary<Assembly, bool> KnownDebugValues = new();

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Debug(object message) => Debug(message.ToString());

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <typeparam name="T">The inputted object's type.</typeparam>
        /// <param name="object">The object to be logged and returned.</param>
        /// <returns>Returns the <typeparamref name="T"/> object inputted in <paramref name="object"/>.</returns>
        public static T DebugObject<T>(T @object)
        {
            Debug(@object);

            return @object;
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Debug(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
#if DEBUG
            if (callingAssembly.GetName().Name is "Exiled.API")
            {
                Send($"[{callingAssembly.GetName().Name}] {message}", Discord.LogLevel.Debug, ConsoleColor.Green);
                return;
            }
#endif
            if (!KnownDebugValues.ContainsKey(callingAssembly))
            {
                if (!Server.PluginAssemblies.ContainsKey(callingAssembly))
                    SetDebugThroughReflection(callingAssembly);
                else
                    KnownDebugValues.Add(callingAssembly, Server.PluginAssemblies[callingAssembly].Config.Debug);
            }

            if (KnownDebugValues[callingAssembly])
                Send($"[{callingAssembly.GetName().Name}] {message}", Discord.LogLevel.Debug, ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Warn"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Warn, ConsoleColor.Magenta);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Warn"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Warn, ConsoleColor.Magenta);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        public static void Send(object message, Discord.LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        public static void Send(string message, Discord.LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> of the message.</param>
        public static void SendRaw(object message, ConsoleColor color) => ServerConsole.AddLog(message.ToString(), color);

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> of the message.</param>
        public static void SendRaw(string message, ConsoleColor color) => ServerConsole.AddLog(message, color);

        /// <summary>
        /// Sends an <see cref="Error(object)"/> with the provided message if the condition is false and stops the execution.
        /// <example> For example:
        /// <code>
        ///     Player ply = Player.Get(2);
        ///     Log.Assert(ply is not null, "The player with the id 2 is null");
        /// </code>
        /// results in it logging an error if the player is null and not continuing.
        /// </example>
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate. If the condition is true it will continue.</param>
        /// <param name="message">The information message. The error and exception will show this message.</param>
        /// <exception cref="Exception">If the condition is false. It throws an exception stopping the execution.</exception>
        public static void Assert(bool condition, object message)
        {
            if (condition)
                return;

            Error(message);

            throw new Exception(message.ToString());
        }

        private static void SetDebugThroughReflection(Assembly assembly)
        {
            try
            {
                IPlugin<IConfig> eventsPlugin = Server.PluginAssemblies.Values.FirstOrDefault(p => p.Name == "Exiled.Events");
                KnownDebugValues.Add(assembly, eventsPlugin?.Config.Debug ?? false);
            }
            catch (Exception e)
            {
                Error(e);
                KnownDebugValues.Add(assembly, false);
            }
        }
    }
}