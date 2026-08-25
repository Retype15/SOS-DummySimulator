// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Barotrauma;
using Barotrauma.LuaCs;
using Microsoft.Xna.Framework;
[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace DummySimulator
{
    public partial class Plugin : IAssemblyPlugin
    {

#pragma warning disable CS8618
        public IConfigService ConfigService { get; set; } = null!;
        public IPluginManagementService PluginManagementService { get; set; } = null!;
        public IConsoleCommandsService ConsoleCommandsService { get; set; } = null!;
#pragma warning restore CS8618

        internal ContentPackage Package = null!;

        internal static Plugin Instance = null!;

        public void Initialize()
        {
            if (!PluginManagementService.TryGetPackageForPlugin<Plugin>(out Package))
            {
                RLogger.LogError("Failed to find package!");
                return;
            }

            Instance = this;

            InitClient();
        }

        public void OnLoadCompleted()
        {
            RLogger.Log(TextSOS.Get("schema.shared.loaded", "[SCHEMA] Loaded Successfully.").Value);
        }

        public void Dispose()
        {
            DisposeClient();

            RLogger.Log(TextSOS.Get("schema.shared.unloaded", "[SCHEMA] Mod Unloaded.").Value);
            GC.SuppressFinalize(this);
        }

        public void PreInitPatching() { }
    }

    internal static class TextSOS
    {
        private static readonly Dictionary<string, Dictionary<Identifier, string>> prefixCache = [];
        public static LocalizedString Get(string key, string fallback = "")
        {
            var text = TextManager.Get(key);

            if (!string.IsNullOrEmpty(fallback))
            {
#if DEBUG
                return text.Fallback("[NT]" + fallback); // NT=NOT-TRANSLATED
#else
                return text.Fallback(fallback);
#endif
            }
            return text;
        }

        public static Dictionary<Identifier, string> GetTranslationsByPrefix(string prefix)
        {
            if (prefixCache.TryGetValue(prefix, out var cached)) return cached;

            var allTranslations = TextManager.GetAllTagTextPairs();
            var filtered = allTranslations
                .Where(kvp => kvp.Key.StartsWith(prefix))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            prefixCache[prefix] = filtered;
            return filtered;
        }
    }

    internal static class RLogger
    {
        [Conditional("DEBUG")]
        public static void LogDebug(string message, Color? color = null) => LuaCsLogger.LogMessage(message, color);

        [Conditional("DEBUG")]
        public static void LogDebugError(string message) => LuaCsLogger.LogError(message);

        [Conditional("RELEASE")]
        public static void LogRelease(string message, Color? color = null) => LuaCsLogger.LogMessage(message, color);

        [Conditional("RELEASE")]
        public static void LogReleaseError(string message) => LuaCsLogger.LogError(message);

        public static void Log(string message, Color? color = null) => LuaCsLogger.LogMessage(message, color);

        public static void LogError(string message) => LuaCsLogger.LogError(message);

        public static void LogWarning(string message, Color? color = null) => LuaCsLogger.LogMessage(message, color ?? Color.Yellow);
    }
}
