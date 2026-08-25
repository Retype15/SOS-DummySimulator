// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Xml.Linq;
using Barotrauma;
using Barotrauma.LuaCs.Data;
using SOS;
using SOS.Configs;

namespace DummySimulator.Configs
{
    public sealed class DummyConfig : ConfigDirtySaver, ISOSConfig
    {
        public string Id => "SOS.DummyConfig";
        public double Order => 0;

        private static DummyConfig? _instance;
        public static DummyConfig Instance => _instance ??= new();

        public void Load()
        {
            Logger.LogDebug("EJECUTING LOAD FROM DUMMY CONFIG...", level: LogLevel.Information);
        }

        public void Save()
        {
            Logger.LogDebug("EJECUTING SAVE FROM DUMMY CONFIG...", level: LogLevel.Information);
            _dummyCharacterXMLRaw.SetIfNotEqual(DummyCharacterXML.ToString());

            SaveChanges(Plugin.Instance.ConfigService);
        }

        public bool DrawSettings(GUIListBox container)
        {
            return true;
        }

        public void Reset()
        {
            DummyDeathCount = 0;
            DummySimulated = false;
            DummyCharacterXML = XElement.Parse("<Character />");

            _dummyCharacterXML = null;
        }

        private readonly ISettingBase<int> _dummyDeathCount;
        public int DummyDeathCount
        {
            get => _dummyDeathCount.Value;
            set => _dummyDeathCount.SetIfNotEqual(value);
        }

        private readonly ISettingBase<bool> _dummySimulated;
        public bool DummySimulated
        {
            get => _dummySimulated.Value;
            set => _dummySimulated.SetIfNotEqual(value);
        }

        private readonly ISettingBase<string> _dummyCharacterXMLRaw;
        private XElement? _dummyCharacterXML = null;
        public XElement DummyCharacterXML
        {
            get => _dummyCharacterXML ??= XElement.Parse(_dummyCharacterXMLRaw.Value ?? "<Character />");
            set => _dummyCharacterXML = value;
        }

        public DummyConfig()
        {
            Logger.LogDebugWarning("EXECUTING DummyConfig CONSTRUCTOR...", level: LogLevel.Trace);
            _instance = this;

            void TryInitConfig<T>(string name, out T setting) where T : ISettingBase
                => base.TryInitConfig(name, out setting, Plugin.Instance.ConfigService, Plugin.Instance.Package);

            TryInitConfig("DummyDeathCount", out _dummyDeathCount);
            TryInitConfig("DummySimulated", out _dummySimulated);
            TryInitConfig("DummyCharacterXML", out _dummyCharacterXMLRaw);
        }
    }
}