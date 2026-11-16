// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using Barotrauma;
using Barotrauma.Items.Components;
using DummySimulator.Configs;
using Microsoft.Xna.Framework;

namespace DummySimulator
{
    internal sealed class ContainedSlot
    {
        public ContainedSlot(int index, IReadOnlyList<ItemPrefab> candidates, Sprite? icon)
        {
            Index = index;
            Candidates = candidates;
            Icon = icon;
        }

        public int Index { get; }

        public IReadOnlyList<ItemPrefab> Candidates { get; }

        public Sprite? Icon { get; }
    }

    internal static class ItemContents
    {
        private const char EntrySeparator = ';';
        private const char SlotSeparator = '|';
        private const char IndexSeparator = ':';
        private const char AssignmentSeparator = '=';

        private static readonly Identifier[] GenericTags =
        [
            "tinyitem",
            "smallitem",
            "mediumitem",
            "largeitem",
            "bigitem",
            "hugeitem"
        ];

        private static readonly Dictionary<Identifier, IReadOnlyList<ContainedSlot>> slotCache = [];
        private static readonly Dictionary<Identifier, ItemPrefab> prefabCache = [];
        private static readonly IReadOnlyDictionary<int, Identifier> EmptySelections = new Dictionary<int, Identifier>();

        private static Dictionary<Identifier, Dictionary<int, Identifier>>? selections;

        private static Dictionary<Identifier, Dictionary<int, Identifier>> Selections
            => selections ??= Parse(DummyConfig.Instance.DummyItemContents);

        public static IReadOnlyList<ContainedSlot> GetSlots(ItemPrefab? prefab)
        {
            if (prefab == null) return [];
            if (slotCache.TryGetValue(prefab.Identifier, out IReadOnlyList<ContainedSlot>? cached)) return cached;

            IReadOnlyList<ContainedSlot> slots = Discover(prefab);
            slotCache[prefab.Identifier] = slots;

            return slots;
        }

        public static bool HasSelection(Identifier item) => Selections.TryGetValue(item, out Dictionary<int, Identifier>? slots) && slots.Count > 0;

        public static IReadOnlyDictionary<int, Identifier> GetSelections(Identifier item)
            => Selections.TryGetValue(item, out Dictionary<int, Identifier>? slots) ? slots : EmptySelections;

        public static Identifier? GetSelection(Identifier item, int slotIndex)
        {
            if (!Selections.TryGetValue(item, out Dictionary<int, Identifier>? slots)) return null;
            return slots.TryGetValue(slotIndex, out Identifier prefab) ? prefab : (Identifier?)null;
        }

        public static ItemPrefab? GetSelectedPrefab(Identifier item, int slotIndex)
            => GetSelection(item, slotIndex) is { } selection ? FindPrefab(selection) : null;

        public static ItemPrefab? FindPrefab(Identifier identifier)
        {
            if (identifier.IsEmpty) return null;
            if (prefabCache.TryGetValue(identifier, out ItemPrefab? cached)) return cached;

            ItemPrefab? prefab = ItemPrefab.Prefabs.Find(candidate => candidate.Identifier == identifier);

            if (prefab != null) prefabCache[identifier] = prefab;

            return prefab;
        }

        public static void SetSelection(Identifier item, int slotIndex, Identifier? prefab)
        {
            if (prefab is not { } selected || selected.IsEmpty)
            {
                if (!Selections.TryGetValue(item, out Dictionary<int, Identifier>? existing)) return;
                if (!existing.Remove(slotIndex) || existing.Count > 0) return;

                Selections.Remove(item);
                return;
            }

            if (!Selections.TryGetValue(item, out Dictionary<int, Identifier>? slots))
            {
                slots = [];
                Selections[item] = slots;
            }

            slots[slotIndex] = selected;
        }

        public static string Serialize()
        {
            var entries = new List<string>();

            foreach (var entry in Selections.Where(entry => entry.Value.Count > 0).OrderBy(entry => entry.Key.Value, StringComparer.OrdinalIgnoreCase))
            {
                string slots = string.Join(SlotSeparator, entry.Value
                    .OrderBy(slot => slot.Key)
                    .Select(slot => $"{slot.Key}{IndexSeparator}{slot.Value.Value}"));

                entries.Add($"{entry.Key.Value}{AssignmentSeparator}{slots}");
            }

            return string.Join(EntrySeparator, entries);
        }

        public static void Reset()
        {
            selections = null;
        }

        private static Dictionary<Identifier, Dictionary<int, Identifier>> Parse(string? raw)
        {
            var result = new Dictionary<Identifier, Dictionary<int, Identifier>>();
            if (string.IsNullOrWhiteSpace(raw)) return result;

            string[] entries = raw.Split(EntrySeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (string entry in entries)
            {
                int assignment = entry.IndexOf(AssignmentSeparator);
                if (assignment <= 0) continue;

                Identifier item = entry[..assignment].Trim().ToIdentifier();
                if (item.IsEmpty) continue;

                var slots = new Dictionary<int, Identifier>();

                foreach (string pair in entry[(assignment + 1)..].Split(SlotSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    int separator = pair.IndexOf(IndexSeparator);
                    if (separator <= 0) continue;
                    if (!int.TryParse(pair[..separator].Trim(), out int slotIndex) || slotIndex < 0) continue;

                    Identifier prefab = pair[(separator + 1)..].Trim().ToIdentifier();
                    if (prefab.IsEmpty) continue;

                    slots[slotIndex] = prefab;
                }

                if (slots.Count > 0) result[item] = slots;
            }

            return result;
        }

        private static List<ContainedSlot> Discover(ItemPrefab prefab)
        {
            Item? probe = null;

            try
            {
                probe = new Item(prefab, Vector2.Zero, null, callOnItemLoaded: false);

                ItemContainer? container = probe.GetComponent<ItemContainer>();
                if (container?.Inventory == null || container.Capacity <= 0) return [];

                container.AllowAccessWhenDropped = true;

                List<Identifier>[]? restrictions = GetSlotRestrictions(container);
                if (restrictions == null)
                {
                    LuaCsLogger.Log($"[SOS] Could not read the inventory restrictions of '{prefab.Identifier}'. Every slot will be shown.", Color.Orange);
                }

                var slots = new List<ContainedSlot>();

                for (int index = 0; index < container.Capacity; index++)
                {
                    if (restrictions != null && restrictions[index].Count == 0) continue;

                    var candidates = ItemPrefab.Prefabs
                        .Where(candidate => container.CanBeContained(candidate, index))
                        .OrderBy(candidate => candidate.Name.Value, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (candidates.Count == 0)
                    {
                        if (restrictions != null)
                        {
                            LuaCsLogger.Log($"[SOS] Slot {index} of '{prefab.Identifier}' is restricted to [{string.Join(", ", restrictions[index])}], but no loaded prefab matches it.", Color.Orange);
                        }
                        continue;
                    }

                    slots.Add(new ContainedSlot(index, candidates, container.GetSlotIcon(index)));
                }

                return slots;
            }
            catch (Exception e)
            {
                LuaCsLogger.LogError($"[SOS] Failed to read the inventory slots of '{prefab.Identifier}': {e}");
                return [];
            }
            finally
            {
                if (probe != null && !probe.Removed) Entity.Spawner?.AddItemToRemoveQueue(probe);
            }
        }

        private static List<Identifier>[]? GetSlotRestrictions(ItemContainer container)
        {
            var restrictions = container.slotRestrictions;
            if (restrictions.Length != container.Capacity) return null;

            var result = new List<List<Identifier>>(restrictions.Length);

            foreach (var restriction in restrictions)
            {
                var identifiers = new List<Identifier>();

                if (restriction.ContainableItems != null)
                {
                    foreach (RelatedItem related in restriction.ContainableItems)
                    {
                        foreach (Identifier identifier in related.Identifiers)
                        {
                            if (IsGenericTag(identifier) || identifiers.Contains(identifier)) continue;
                            identifiers.Add(identifier);
                        }
                    }
                }

                result.Add(identifiers);
            }

            return [.. result];
        }

        private static bool IsGenericTag(Identifier identifier) => Array.IndexOf(GenericTags, identifier) >= 0;
    }
}
