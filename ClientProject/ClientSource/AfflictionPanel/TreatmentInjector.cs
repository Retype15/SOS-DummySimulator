// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using Barotrauma;
using Microsoft.Xna.Framework;
using SOS;

namespace DummySimulator
{
    /// <summary>
    /// Makes every treatment button of the health window usable against the simulated subject.
    /// Discovery is structural: the shared <c>recommendedTreatmentContainer</c> is only one of the
    /// possible hosts, since UI mods may hide it and build per-limb lists of their own, with slots
    /// nested inside intermediate layout groups.
    /// </summary>
    internal static class TreatmentInjector
    {
        private const int MaxAncestorDepth = 16;

        private static readonly GUIButton.OnClickedHandler ClickHandler = OnTreatmentClicked;
        private static readonly List<GUIComponent> walkBuffer = [];

        private static bool idleLogged;

        public static void Inject(CharacterHealth? health)
        {
            if (health == null) return;

            GUIComponent? root = health.healthWindow;
            if (root == null) root = health.recommendedTreatmentContainer;
            if (root == null) return;

            if (PatchButtons(root))
            {
                idleLogged = false;
                return;
            }

            if (idleLogged) return;

            idleLogged = true;
            Logger.LogDebug("[SOS] No treatment buttons found in the health window; treatment injection is idle.", level: LogLevel.Trace);
        }

        private static bool PatchButtons(GUIComponent root)
        {
            bool found = false;

            walkBuffer.Clear();
            walkBuffer.Add(root);

            while (walkBuffer.Count > 0)
            {
                int index = walkBuffer.Count - 1;
                GUIComponent current = walkBuffer[index];
                walkBuffer.RemoveAt(index);

                if (current is GUIButton button)
                {
                    ItemPrefab? prefab = button.UserData as ItemPrefab ?? button.Parent?.UserData as ItemPrefab;
                    if (prefab != null)
                    {
                        PatchButton(button, prefab);
                        found = true;
                    }
                }

                for (int i = 0; i < current.CountChildren; i++)
                {
                    GUIComponent? child = current.GetChild(i);
                    if (child != null) walkBuffer.Add(child);
                }
            }

            return found;
        }

        private static void PatchButton(GUIButton button, ItemPrefab prefab)
        {
            bool wasDisabled = !button.Enabled;
            if (wasDisabled)
            {
                button.Enabled = true;
                EnableChildren(button);
            }

            if (ReferenceEquals(button.OnClicked, ClickHandler)) return;

            button.OnClicked = ClickHandler;

            if (wasDisabled)
            {
                string colorHex = Color.LightSkyBlue.ToStringHex();
                button.ToolTip = RichString.Rich($"‖color:{colorHex}‖[SIMULATION MODE]‖color:end‖\n‖color:255,255,255,255‖{prefab.Name.Value}‖color:end‖\n{prefab.Description}");
            }
        }

        private static void EnableChildren(GUIComponent parent)
        {
            for (int i = 0; i < parent.CountChildren; i++)
            {
                GUIComponent? child = parent.GetChild(i);
                if (child != null) child.Enabled = true;
            }
        }

        private static bool OnTreatmentClicked(GUIButton button, object userData)
        {
            if (userData is not ItemPrefab prefab) return true;

            ClinicalSimulatorManager.ApplyMockItem(prefab, ResolveTargetLimb(button));
            return true;
        }

        private static Limb? ResolveTargetLimb(GUIComponent? source)
        {
            Character? patient = ClinicalSimulatorManager.Patient;
            if (patient == null || patient.Removed || patient.AnimController == null) return null;

            if (FindLimbType(source) is LimbType limbType)
            {
                foreach (Limb limb in patient.AnimController.Limbs)
                {
                    if (limb.type == limbType) return limb;
                }
            }

            return ClinicalSimulatorManager.SelectedLimb;
        }

        private static LimbType? FindLimbType(GUIComponent? source)
        {
            RectTransform? rect = source?.RectTransform;

            for (int depth = 0; rect != null && depth < MaxAncestorDepth; depth++)
            {
                if (rect.GUIComponent?.UserData is LimbType limbType) return limbType;
                rect = rect.Parent;
            }

            return null;
        }
    }
}
