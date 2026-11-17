// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Runtime.CompilerServices;
using Barotrauma;
using Barotrauma.LuaCs;
using SOS;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("Neurotrauma")]
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
                Logger.LogError("Failed to find package!");
                return;
            }

            Instance = this;

            InitClient();
        }

        public void PreInitPatching() { }

        public void OnLoadCompleted() { }

        public void Dispose()
        {
            DisposeClient();

            Logger.LogDebug("[SOS-DummySim] Mod Unloaded.", level: LogLevel.Trace);
            GC.SuppressFinalize(this);
        }
    }
}
