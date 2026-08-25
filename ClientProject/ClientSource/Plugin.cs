// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Diagnostics;
using System.Xml.Linq;
using DummySimulator.Configs;
using SOS;

namespace DummySimulator
{
    // Client-specific code
    public partial class Plugin
    {
        [Conditional("CLIENT")]
        public static void InitClient()
        {
            Logger.LogDebug("DummySimulator.InitClient Executed...", level: LogLevel.Trace);
            API.On(CommKeys.CloseWindow, ClinicalSimulatorManager.Destroy);

            API.RegisterConfig(() => DummyConfig.Instance);

            API.On<int>("SOS.Migration.DummyDeathCount", DummyConfig.Instance.MigrateDummyDeathCount);
            API.On<bool>("SOS.Migration.DummySimulated", DummyConfig.Instance.MigrateDummySimulated);
            API.On<XElement>("SOS.Migration.DummyCharacterXML", DummyConfig.Instance.MigrateDummyCharacterXML);
        }

        [Conditional("CLIENT")]
        public static void DisposeClient()
        {
            API.Off(CommKeys.CloseWindow, ClinicalSimulatorManager.Destroy);

            API.Off<int>("SOS.Migration.DummyDeathCount", DummyConfig.Instance.MigrateDummyDeathCount);
            API.Off<bool>("SOS.Migration.DummySimulated", DummyConfig.Instance.MigrateDummySimulated);
            API.Off<XElement>("SOS.Migration.DummyCharacterXML", DummyConfig.Instance.MigrateDummyCharacterXML);
        }
    }
}