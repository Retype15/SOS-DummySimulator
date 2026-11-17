// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Reflection;
using System.Runtime.CompilerServices;
using Barotrauma;
using Microsoft.Xna.Framework;
using Neurotrauma.ClientSource;

using SOS;

namespace DummySimulator
{
    internal static class NtSurgeryPanel
    {
        private const int OutlineSize = 44;
        private const int ButtonSize = 36;
        private const int EntrySpacing = 4;
        private const int MinHeaderHeight = 20;
        private const int RowSpacing = 6;
        private const int MinBackHeight = 24;

        private static GUIFrame? frame;

        private static readonly List<(GUIButton Button, LimbType RequiredLimb)> gated = [];

        public static bool IsAvailable => NeurotraumaBridge.IsNeurotraumaLoaded;

        public static void Attach(GUIFrame section)
        {
            if (!IsAvailable || frame != null) return;
            AttachCore(section);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AttachCore(GUIFrame section)
        {
            if (NtData.Entries.Count == 0) return;

            frame = section;
            BuildColumn();
        }

        public static void Update()
        {
            if (frame == null || gated.Count == 0) return;

            LimbType? selectedLimb = ClinicalSimulatorManager.SelectedLimbType;

            foreach (var (button, requiredLimb) in gated)
            {
                bool matches = selectedLimb == requiredLimb;
                if (button.Enabled == matches) continue;

                button.Enabled = matches;
                button.ToolTip = LimbTooltip(matches);
            }
        }

        public static void Detach()
        {
            if (frame == null) return;
            DetachCore();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DetachCore()
        {
            frame?.ClearChildren();
            frame = null;
            gated.Clear();
            NtData.OpenMenu = null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void BuildColumn()
        {
            if (frame == null) return;

            frame.ClearChildren();
            gated.Clear();

            ButtonData? openMenu = NtData.OpenMenu;
            if (openMenu?.SubButtons is not { Count: > 0 }) openMenu = null;
            IReadOnlyList<ButtonData> source = openMenu?.SubButtons ?? NtData.Entries;

            var column = new GUILayoutGroup(new RectTransform(Vector2.One, frame.RectTransform, Anchor.Center))
            {
                Stretch = true,
                AbsoluteSpacing = RowSpacing
            };

            LocalizedString caption = openMenu == null
                ? Texts.Get("sos.sim.surgery_tools", "Surgery Tools")
                : Texts.Get("sos.sim.surgery_organs", "Organs");

            var header = new GUITextBlock(
                new RectTransform(new Vector2(1f, 0f), column.RectTransform),
                caption,
                font: GUIStyle.SmallFont,
                textAlignment: Alignment.Center,
                wrap: true);

            SimulatorColumn.FitRowToWrappedText(header.RectTransform, header, caption, MinHeaderHeight);

            if (openMenu != null)
            {
                LocalizedString backText = Texts.Get("sos.sim.surgery_back", "Back");

                var back = new GUIButton(
                    new RectTransform(new Vector2(1f, 0f), column.RectTransform),
                    backText,
                    style: "GUIButtonSmall")
                {
                    ToolTip = RichString.Rich(Texts.Get("sos.sim.surgery_back_tooltip", "Returns to the full tool list.")),
                    OnClicked = (_, _) => { NtData.OpenMenu = null; BuildColumn(); return true; }
                };

                back.TextBlock.Wrap = true;
                back.TextBlock.SetTextPos();

                SimulatorColumn.FitRowToWrappedText(back.RectTransform, back.TextBlock, backText, MinBackHeight);
            }

            var list = new GUIListBox(new RectTransform(Vector2.One, column.RectTransform), style: null)
            {
                Spacing = EntrySpacing
            };

            LimbType? selectedLimb = ClinicalSimulatorManager.SelectedLimbType;

            foreach (ButtonData entry in source)
            {
                var outline = new GUIFrame(
                    new RectTransform(new Point(OutlineSize, OutlineSize), list.Content.RectTransform, Anchor.TopCenter, isFixedSize: true),
                    style: "GUIFrameListBox")
                {
                    CanBeFocused = false
                };

                var button = new GUIButton(
                    new RectTransform(new Point(ButtonSize, ButtonSize), outline.RectTransform, Anchor.Center, isFixedSize: true),
                    text: "",
                    style: entry.StyleName)
                {

                    UserData = entry.Identifier,

                    ToolTip = RichString.Rich(entry.Tooltip),
                    CanBeFocused = true,
                    OnClicked = (_, _) => { OnEntryPressed(entry); return true; }

                };

                if (entry.RequiredLimb.HasValue)
                {
                    bool matches = selectedLimb == entry.RequiredLimb.Value;
                    button.Enabled = matches;
                    button.ToolTip = LimbTooltip(matches);

                    gated.Add((button, entry.RequiredLimb.Value));
                }
            }
        }

        private static RichString LimbTooltip(bool limbMatches) => RichString.Rich(limbMatches
            ? Texts.Get("sos.sim.surgery_organ_ready", "Operates on the selected limb.")
            : Texts.Get("sos.sim.surgery_organ_limb", "Select the matching limb first."));

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void OnEntryPressed(ButtonData entry)
        {
            if (entry.SubButtons is { Count: > 0 })
            {
                NtData.OpenMenu = entry;
                BuildColumn();
                return;
            }

            if (string.IsNullOrEmpty(entry.ItemIdentifier)) return;

            ClinicalSimulatorManager.ApplySurgery(entry.ItemIdentifier!);
        }

        private static class NtData
        {
            private const string DefinitionsTypeName = "ButtonDefinitions";

            public static readonly List<ButtonData> Entries = ReadEntries();

            public static ButtonData? OpenMenu;

            private static List<ButtonData> ReadEntries()
            {
                try
                {
                    Type? definitions = typeof(ButtonData).Assembly.GetType(DefinitionsTypeName);
                    FieldInfo? field = definitions?.GetField("Entries", BindingFlags.Public | BindingFlags.Static);

                    if (field?.GetValue(null) is List<ButtonData> entries) return entries;

                    Logger.LogError($"[SOS] Could not find '{DefinitionsTypeName}.Entries'; the surgery column stays empty.");
                }
                catch (Exception e)
                {
                    Logger.LogError($"[SOS] Failed to read the Neurotrauma surgery buttons: {e.Message}");
                }

                return [];
            }
        }
    }
}
