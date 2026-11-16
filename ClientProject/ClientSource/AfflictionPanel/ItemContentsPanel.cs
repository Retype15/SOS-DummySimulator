// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using Barotrauma;
using DummySimulator.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SOS;

namespace DummySimulator
{
    /// AAA
    internal static class ItemContentsPanel
    {
        private const int HeaderHeight = 20;
        private const int SlotSize = 44;
        private const int SlotSpacing = 4;
        private const int SectionPadding = 6;

        private const int SelectorWidth = 220;
        private const int SelectorRowHeight = 28;
        private const int SelectorSpacing = 2;
        private const int SelectorMaxRows = 8;
        private const int SelectorPadding = 4;

        private const int SelectorIconSize = 20;
        private const int SelectorIconOffset = 6;
        private const int SelectorIconSpacing = 4;
        private const int SelectorTextPadding = 6;

        private static GUIFrame? section;
        private static GUIFrame? popupHost;
        private static GUIFrame? selector;
        private static GUIButton? selectorAnchor;
        private static ItemPrefab? target;
        private static IReadOnlyList<ContainedSlot> slots = [];

        private static int ReservedSlots => Math.Max(1, DummyConfig.Instance.DummyContentsSlots);

        public static int Height { get; private set; }

        public static void Attach(GUIFrame hostSection, GUIFrame host)
        {
            section = hostSection;
            popupHost = host;
        }

        public static void Detach()
        {
            CloseSelector();
            section?.ClearChildren();
            section = null;
            popupHost = null;
            target = null;
            slots = [];
            Height = 0;
        }

        public static void SetTarget(ItemPrefab? prefab)
        {
            if (target == prefab) return;

            target = prefab;
            CloseSelector();
            Build();
        }

        public static void Update()
        {
            if (selector == null || !PlayerInput.PrimaryMouseButtonClicked()) return;

            GUIComponent? mouseOn = GUI.MouseOn;
            if (IsInside(mouseOn, selector) || IsInside(mouseOn, selectorAnchor)) return;

            CloseSelector();
        }

        private static void Build()
        {
            if (section == null) return;

            section.ClearChildren();
            slots = ItemContents.GetSlots(target);

            int headerHeight = slots.Count > 0 ? BuildContent() : 0;
            Height = CalculateHeight(Math.Min(slots.Count, ReservedSlots), headerHeight);

            SimulatorColumn.Reflow();
        }

        private static int BuildContent()
        {
            var layout = new GUILayoutGroup(new RectTransform(Vector2.One, section!.RectTransform))
            {
                Stretch = true,
                AbsoluteSpacing = SlotSpacing
            };

            LocalizedString caption = Texts.Get("sos.sim.contents_header", "Contents");

            var header = new GUITextBlock(
                new RectTransform(new Vector2(1f, 0f), layout.RectTransform),
                caption,
                font: GUIStyle.SmallFont,
                textAlignment: Alignment.Center,
                wrap: true);

            SimulatorColumn.FitRowToWrappedText(header.RectTransform, header, caption, HeaderHeight);

            var list = new GUIListBox(new RectTransform(Vector2.One, layout.RectTransform), style: null)
            {
                Spacing = SlotSpacing
            };

            foreach (ContainedSlot slot in slots)
            {
                BuildSlotRow(list.Content, slot);
            }

            return header.RectTransform.MinSize.Y;
        }

        private static void BuildSlotRow(GUIComponent parent, ContainedSlot slot)
        {
            var button = new GUIButton(
                new RectTransform(new Vector2(1f, 0f), parent.RectTransform) { MinSize = new Point(0, SlotSize) },
                style: null)
            {
                CanBeFocused = true,
                OnDrawToolTip = component => component.ToolTip = SlotTooltip(slot),
                OnClicked = (button, _) => { ToggleSelector(slot, button); return true; }
            };

            _ = new GUICustomComponent(
                new RectTransform(Vector2.One, button.RectTransform),
                onDraw: (spriteBatch, component) => DrawSlot(spriteBatch, component.Rect, slot, GUI.IsMouseOn(button)))
            {
                CanBeFocused = false
            };
        }

        private static void DrawSlot(SpriteBatch spriteBatch, Rectangle rect, ContainedSlot slot, bool hovered)
        {
            int size = Math.Min(rect.Width, rect.Height);
            var slotRect = new Rectangle(rect.Center.X - (size / 2), rect.Center.Y - (size / 2), size, size);

            Sprite background = Inventory.SlotSpriteSmall;
            spriteBatch.Draw(background.Texture, slotRect, background.SourceRect, Color.White);

            Sprite? selected = target == null ? null : ItemContents.GetSelectedPrefab(target.Identifier, slot.Index)?.Icon();

            if (selected != null)
            {
                float scale = Math.Min(Math.Min((slotRect.Width - 10) / selected.size.X, (slotRect.Height - 10) / selected.size.Y), 2.0f);
                selected.Draw(spriteBatch, slotRect.Center.ToVector2(), Color.White, 0f, scale);
            }
            else
            {
                slot.Icon?.Draw(
                    spriteBatch,
                    slotRect.Center.ToVector2(),
                    GUIStyle.EquipmentSlotIconColor,
                    0f,
                    Math.Min(slotRect.Width / slot.Icon.size.X, slotRect.Height / slot.Icon.size.Y) * 0.8f);
            }

            if (hovered)
            {
                GUI.DrawRectangle(spriteBatch, slotRect, Color.White * 0.35f, isFilled: false, thickness: 2);
            }
        }

        private static void ToggleSelector(ContainedSlot slot, GUIButton anchor)
        {
            if (selector != null && selectorAnchor == anchor)
            {
                CloseSelector();
                return;
            }

            OpenSelector(slot, anchor);
        }

        private static void OpenSelector(ContainedSlot slot, GUIButton anchor)
        {
            CloseSelector();

            if (popupHost == null) return;

            int rows = Math.Min(slot.Candidates.Count + 1, SelectorMaxRows);
            int height = (rows * SelectorRowHeight) + ((rows - 1) * SelectorSpacing) + (SelectorPadding * 2);

            float scale = GUI.Scale <= 0f ? 1f : GUI.Scale;
            int hostWidth = (int)(popupHost.Rect.Width / scale);
            int hostHeight = (int)(popupHost.Rect.Height / scale);
            if (hostWidth <= 0 || hostHeight <= 0) return;

            int width = Math.Min(SelectorWidth, hostWidth);
            int anchorX = (int)((anchor.Rect.X - popupHost.Rect.X) / scale);
            int anchorY = (int)((anchor.Rect.Y - popupHost.Rect.Y) / scale);

            int x = Math.Clamp(anchorX - width - SelectorSpacing, 0, Math.Max(0, hostWidth - width));
            int y = Math.Clamp(anchorY, 0, Math.Max(0, hostHeight - height));

            selector = new GUIFrame(
                new RectTransform(new Point(width, height), popupHost.RectTransform) { AbsoluteOffset = new Point(x, y) },
                style: "GUIFrame")
            {
                CanBeFocused = true,
                UserData = "ContentsSelector"
            };

            selectorAnchor = anchor;

            var list = new GUIListBox(new RectTransform(Vector2.One, selector.RectTransform, Anchor.Center), style: null)
            {
                Spacing = SelectorSpacing
            };

            AddSelectorRow(list, slot, prefab: null);

            foreach (ItemPrefab candidate in slot.Candidates)
            {
                AddSelectorRow(list, slot, candidate);
            }
        }

        private static void AddSelectorRow(GUIListBox list, ContainedSlot slot, ItemPrefab? prefab)
        {
            var row = new GUIButton(
                new RectTransform(new Vector2(1f, 0f), list.Content.RectTransform) { MinSize = new Point(0, SelectorRowHeight) },
                style: "ListBoxElement")
            {
                OnClicked = (_, _) =>
                {
                    if (target != null) ItemContents.SetSelection(target.Identifier, slot.Index, prefab?.Identifier);
                    CloseSelector();
                    return true;
                }
            };

            var iconFrame = new GUIFrame(
                new RectTransform(new Point(SelectorIconSize, SelectorIconSize), row.RectTransform, Anchor.CenterLeft)
                {
                    AbsoluteOffset = new Point(SelectorIconOffset, 0)
                },
                style: null)
            {
                CanBeFocused = false
            };

            if (prefab?.Icon() is { } icon)
            {
                _ = new GUIImage(new RectTransform(Vector2.One, iconFrame.RectTransform), icon, scaleToFit: true)
                {
                    CanBeFocused = false
                };
            }

            LocalizedString name = prefab == null
                ? Texts.Get("sos.sim.contents_none", "None")
                : prefab.Name();

            _ = new GUITextBlock(
                new RectTransform(Vector2.One, row.RectTransform),
                name,
                font: GUIStyle.SmallFont,
                textColor: prefab == null ? Color.Gray : Color.White,
                textAlignment: Alignment.CenterLeft)
            {
                CanBeFocused = false,
                OverflowClip = true,
                Padding = new Vector4(SelectorIconOffset + SelectorIconSize + SelectorIconSpacing, 0f, SelectorTextPadding, 0f)
            };
        }

        private static RichString SlotTooltip(ContainedSlot slot)
        {
            ItemPrefab? selected = target == null ? null : ItemContents.GetSelectedPrefab(target.Identifier, slot.Index);
            LocalizedString name = selected == null ? Texts.Get("sos.sim.contents_none", "None") : selected.Name();

            return RichString.Rich($"{Texts.Get("sos.sim.contents_slot_tooltip", "Click to choose what this slot holds.").Value}\n{name.Value}");
        }

        private static void CloseSelector()
        {
            if (selector == null) return;

            selector.RectTransform.Parent = null;
            selector = null;
            selectorAnchor = null;
        }

        private static bool IsInside(GUIComponent? component, GUIComponent? ancestor)
        {
            if (component == null || ancestor == null) return false;

            for (GUIComponent? current = component; current != null; current = current.Parent)
            {
                if (current == ancestor) return true;
            }

            return false;
        }

        private static int CalculateHeight(int visibleSlots, int headerHeight)
        {
            if (visibleSlots <= 0) return 0;

            int listHeight = visibleSlots * (SlotSize + SlotSpacing);

            return headerHeight + SlotSpacing + listHeight + SectionPadding;
        }
    }
}
