
// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using Barotrauma;

namespace DummySimulator
{
    internal static class PrefabExt
    {
        public static string Name(this Prefab prefab)
        {
            return prefab switch
            {
                ItemPrefab item => item.Name.ToString(),
                AfflictionPrefab affliction => affliction.Name.ToString(),
                _ => prefab.Identifier.ToString()
            };
        }
        public static Sprite? Icon(this Prefab? prefab)
        {
            return prefab switch
            {
                ItemPrefab item => item.InventoryIcon ?? item.sprite,
                AfflictionPrefab affliction => affliction.Icon,
                _ => null
            };
        }
    }
}