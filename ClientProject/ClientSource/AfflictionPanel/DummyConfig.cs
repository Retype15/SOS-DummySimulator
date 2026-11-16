// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Xml.Linq;
using Barotrauma;
using Barotrauma.LuaCs.Data;
using Microsoft.Xna.Framework;

using SOS;
using SOS.Configs;
using SOS.Extensions;

namespace DummySimulator.Configs
{
    public sealed class DummyConfig : ConfigDirtySaver, ISOSConfig
    {
        public const string ID = "SOS.DummyConfig";

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
            _dummyItemContents.SetIfNotEqual(ItemContents.Serialize());

            SaveChanges(Plugin.Instance.ConfigService);
        }

        public void Draw(RectTransform rectT)
        {
            var l = new SOS.GUI.GUILayoutBuilder(rectT);
            l.Header("Dummy Manager Settings", Color.Gold);

            l.NumberInput(
                Texts.Get("sos.sim.contents_slots", "Contents slots").Value,
                DummyContentsSlots,
                value => DummyContentsSlots = (int)value,
                min: 1,
                max: 8,
                step: 1,
                tooltip: Texts.Get("sos.sim.contents_slots_tooltip", "Slot rows the Contents editor reserves room for.").Value);

            l.ButtonToResetSection(this);
            l.Separator();
        }

        public void Reset()
        {
            DummyDeathCount = 0;
            DummyMedicalSkill = _dummyMedicalSkill.DefaultValue;
            DummySimulated = false;
            DummyCharacterXML = XElement.Parse("<Character />");
            DummyItemContents = string.Empty;
            DummyContentsSlots = _dummyContentsSlots.DefaultValue;
            ItemContents.Reset();
            if (ClinicalSimulatorManager.Patient != null) ClinicalSimulatorManager.DiscardPatient();
        }

        private readonly ISettingBase<int> _dummyDeathCount;
        public int DummyDeathCount
        {
            get => _dummyDeathCount.Value;
            set => _dummyDeathCount.SetIfNotEqual(value);
        }

        private readonly ISettingBase<int> _dummyMedicalSkill;
        public int DummyMedicalSkill
        {
            get => _dummyMedicalSkill.Value;
            set => _dummyMedicalSkill.SetIfNotEqual(value);
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

        private readonly ISettingBase<string> _dummyItemContents;
        public string DummyItemContents
        {
            get => _dummyItemContents.Value;
            set => _dummyItemContents.SetIfNotEqual(value);
        }

        private readonly ISettingBase<int> _dummyContentsSlots;

        public int DummyContentsSlots
        {
            get => _dummyContentsSlots.Value;
            set => _dummyContentsSlots.SetIfNotEqual(value);
        }

        public DummyConfig()
        {
            Logger.LogDebugWarning("EXECUTING DummyConfig CONSTRUCTOR...", level: LogLevel.Trace);
            _instance = this;

            void TryInitConfig<T>(string name, out T setting) where T : ISettingBase
                => base.TryInitConfig(name, out setting, Plugin.Instance.ConfigService, Plugin.Instance.Package);

            TryInitConfig("DummyDeathCount", out _dummyDeathCount);
            TryInitConfig("DummyMedicalSkill", out _dummyMedicalSkill);
            TryInitConfig("DummySimulated", out _dummySimulated);
            TryInitConfig("DummyCharacterXML", out _dummyCharacterXMLRaw);
            TryInitConfig("DummyItemContents", out _dummyItemContents);
            TryInitConfig("DummyContentsSlots", out _dummyContentsSlots);
        }

        #region Migration methods

        internal void MigrateDummyDeathCount(int value) => DummyDeathCount = value;
        internal void MigrateDummySimulated(bool value) => DummySimulated = value;
        internal void MigrateDummyCharacterXML(XElement value) => DummyCharacterXML = value;

        #endregion
    }
}