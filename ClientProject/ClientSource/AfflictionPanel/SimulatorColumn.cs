// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using Barotrauma;
using Microsoft.Xna.Framework;

namespace DummySimulator
{
    internal static class SimulatorColumn
    {
        public const int ColumnWidth = 68;

        private const int SectionSpacing = 6;

        private static GUIFrame? frame;
        private static GUIFrame? surgerySection;
        private static GUIFrame? contentsSection;

        public static void Attach(GUIFrame parent, GUIFrame host)
        {
            if (frame != null) return;

            frame = new GUIFrame(
                new RectTransform(
                    new Vector2(0f, 1f),
                    parent.RectTransform,
                    Anchor.CenterRight,
                    minSize: new Point(ColumnWidth, 0),
                    maxSize: new Point(ColumnWidth, int.MaxValue)),
                style: null)
            {
                CanBeFocused = false
            };

            frame.RectTransform.SizeChanged += Reflow;

            surgerySection = CreateSection(Anchor.TopCenter);
            contentsSection = CreateSection(Anchor.BottomCenter);

            NtSurgeryPanel.Attach(surgerySection);
            ItemContentsPanel.Attach(contentsSection, host);

            Reflow();
        }

        public static void Detach()
        {
            NtSurgeryPanel.Detach();
            ItemContentsPanel.Detach();

            if (frame != null)
            {
                frame.RectTransform.SizeChanged -= Reflow;
                frame.RectTransform.Parent = null;
            }

            frame = null;
            surgerySection = null;
            contentsSection = null;
        }

        public static void Update()
        {
            NtSurgeryPanel.Update();
            ItemContentsPanel.Update();
        }

        public static void Reflow()
        {
            if (frame == null || surgerySection == null || contentsSection == null) return;

            int available = frame.RectTransform.NonScaledSize.Y;
            if (available <= 0) return;

            int contentsHeight = Math.Clamp(ItemContentsPanel.Height, 0, Math.Max(0, available - SectionSpacing));
            int surgeryHeight = Math.Max(0, available - contentsHeight - SectionSpacing);

            SetHeight(surgerySection, surgeryHeight);
            SetHeight(contentsSection, contentsHeight);
        }

        public static void FitRowToWrappedText(RectTransform row, GUITextBlock block, LocalizedString text, int minHeight)
        {
            int width = block.RectTransform.NonScaledSize.X - (int)(block.Padding.X + block.Padding.Z);
            string wrapped = width > 0 ? block.Font.WrapText(text.Value, width) : text.Value;

            int height = Math.Max(minHeight, (int)MathF.Ceiling(block.Font.MeasureString(wrapped).Y));

            row.MinSize = new Point(0, height);
            row.MaxSize = new Point(int.MaxValue, height);
        }

        private static GUIFrame CreateSection(Anchor anchor)
        {
            return new GUIFrame(
                new RectTransform(new Vector2(1f, 0f), frame!.RectTransform, anchor)
                {
                    MinSize = new Point(0, 0),
                    MaxSize = new Point(int.MaxValue, 0)
                },
                style: null)
            {
                CanBeFocused = false
            };
        }

        private static void SetHeight(GUIFrame section, int height)
        {
            section.RectTransform.MinSize = new Point(0, height);
            section.RectTransform.MaxSize = new Point(int.MaxValue, height);
        }
    }
}
