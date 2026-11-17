// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Reflection;
using Barotrauma;
using HarmonyLib;
using SOS;

namespace DummySimulator
{
    /// <summary>
    /// Vanilla <see cref="Ragdoll.GetLimb"/> iterates the private <c>limbs</c> field, which is set to
    /// null when the ragdoll is removed, while the public <see cref="Ragdoll.Limbs"/> property already
    /// returns an empty array in that state. Mods that keep a reference to a removed character
    /// (delayed timers, spawn callbacks) crash on that null dereference. Returning null mirrors the
    /// rest of the API, and both known call sites already handle a null limb.
    /// </summary>
    internal static class RagdollLifecycleGuard
    {
        private const string HarmonyId = "retype.sos.dummysimulator.ragdollguard";

        private static Harmony? harmony;

        public static void Install()
        {
            if (harmony != null) return;

            try
            {
                MethodInfo? getLimb = AccessTools.Method(typeof(Ragdoll), nameof(Ragdoll.GetLimb));
                if (getLimb == null) throw new MissingMethodException(typeof(Ragdoll).FullName, nameof(Ragdoll.GetLimb));

                harmony = new Harmony(HarmonyId);
                harmony.Patch(getLimb, prefix: new HarmonyMethod(typeof(RagdollLifecycleGuard), nameof(GetLimbPrefix)));
                Logger.LogDebug("[SOS] Ragdoll lifecycle guard installed.", level: LogLevel.Trace);
            }
            catch (Exception e)
            {
                harmony = null;
                LuaCsLogger.LogError($"[SOS] Failed to install the ragdoll lifecycle guard: {e.Message}");
            }
        }

        public static void Uninstall()
        {
            if (harmony == null) return;

            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception e)
            {
                LuaCsLogger.LogError($"[SOS] Failed to remove the ragdoll lifecycle guard: {e.Message}");
            }
            finally
            {
                harmony = null;
            }
        }

        private static bool GetLimbPrefix(Ragdoll __instance, ref Limb __result)
        {
            if (__instance.limbs != null) return true;

            __result = null!;
            return false;
        }
    }
}
