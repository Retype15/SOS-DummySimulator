// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Runtime.CompilerServices;
using Barotrauma;
using Neurotrauma;
using SOS;

namespace DummySimulator
{
    internal static class NeurotraumaBridge
    {
        private const string NeurotraumaPackage = "Neurotrauma CS";
        public const string SimulatedMarker = "luabotomy";
        private const float SimulatedMarkerStrength = 0.1f;
        private const float MaxSimulatedDeltaTime = 1f;
        private const int MaxStepsPerFrame = 32;

        private static bool? isNeurotraumaLoaded;

        private static Character? stepOwner;
        private static float stepAccumulator;
        private static int stepFailureCount;

        public static bool IsNeurotraumaLoaded
        {
            get
            {
                isNeurotraumaLoaded ??= ContentPackageManager.EnabledPackages.All
                    .Any(package => package.NameMatches(NeurotraumaPackage));
                return isNeurotraumaLoaded.Value;
            }
        }

        public static void Attach(Character? character)
        {
            if (character == null || !IsNeurotraumaLoaded) return;
            AttachCore(character);
        }

        public static void Tick(Character? character, float simulatedDeltaTime)
        {
            if (character == null || simulatedDeltaTime <= 0f || !IsNeurotraumaLoaded) return;
            TickCore(character, simulatedDeltaTime);
        }

        public static void Detach(Character? character)
        {
            if (character == null || !IsNeurotraumaLoaded) return;
            DetachCore(character);
        }

        public static void ApplyItemTreatment(Item? item, Character? user, Character? target, Limb? targetLimb)
        {
            if (item == null || user == null || target == null || targetLimb == null || !IsNeurotraumaLoaded) return;
            ApplyItemTreatmentCore(item, user, target, targetLimb);
        }

        public static void ApplySurgeryFunction(string? itemIdentifier, Character? user, Character? target, Limb? targetLimb)
        {
            if (string.IsNullOrEmpty(itemIdentifier) || user == null || target == null || targetLimb == null || !IsNeurotraumaLoaded) return;
            ApplySurgeryFunctionCore(itemIdentifier, user, target, targetLimb);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ApplyItemTreatmentCore(Item item, Character user, Character target, Limb targetLimb)
        {
            try
            {
                string itemIdentifier = item.Prefab.Identifier.Value;

                Action<NTItems.ItemUpdateFunctionInfos>? function = NeurotraumaInit.NTItemsLoader.Get(itemIdentifier);
                if (function == null) return;

                NTHuman? userHuman = NTHuman.getNTHumanFromCharacter(user);
                NTHuman? targetHuman = NTHuman.getNTHumanFromCharacter(target);
                if (userHuman == null || targetHuman == null)
                {
                    Logger.LogError($"[SOS] No Neurotrauma state for the dummy; skipping '{itemIdentifier}'.");
                    return;
                }

                function(new NTItems.ItemUpdateFunctionInfos(item, userHuman, targetHuman, targetLimb));
            }
            catch (Exception e)
            {
                Logger.LogError($"[SOS] Neurotrauma item treatment failed for '{item.Prefab.Identifier}': {e.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ApplySurgeryFunctionCore(string itemIdentifier, Character user, Character target, Limb targetLimb)
        {
            try
            {
                Action<NTItems.ItemUpdateFunctionInfos>? function = NeurotraumaInit.NTItemsLoader.Get(itemIdentifier);
                if (function == null) return;

                NTHuman? userHuman = NTHuman.getNTHumanFromCharacter(user);
                NTHuman? targetHuman = NTHuman.getNTHumanFromCharacter(target);
                if (userHuman == null || targetHuman == null)
                {
                    Logger.LogError($"[SOS] No Neurotrauma state for the dummy; skipping '{itemIdentifier}'.");
                    return;
                }

                function(new NTItems.ItemUpdateFunctionInfos(null!, userHuman, targetHuman, targetLimb));
            }
            catch (Exception e)
            {
                Logger.LogError($"[SOS] Neurotrauma surgery function failed for '{itemIdentifier}': {e.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AttachCore(Character character)
        {
            NTHuman ntHuman = NTHuman.getNTHumanFromCharacter(character) ?? new NTHuman(character);
            ntHuman.SetAffliction(SimulatedMarker, SimulatedMarkerStrength);
            ResetSteps(character);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DetachCore(Character character)
        {
            NTHuman? ntHuman = NTHuman.getNTHumanFromCharacter(character);
            if (ntHuman != null) NTHuman.RemoveNTHuman(ntHuman);
            if (stepOwner == character) ResetSteps(null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void TickCore(Character character, float simulatedDeltaTime)
        {
            if (!NTConfig.Get("NT_Calculations", true)) return;
            if (!HF.IsCharacterValid(character) || character.IsDead) return;

            NTHuman? ntHuman = NTHuman.getNTHumanFromCharacter(character);
            if (ntHuman == null)
            {
                ntHuman = new NTHuman(character);
                ntHuman.SetAffliction(SimulatedMarker, SimulatedMarkerStrength);
            }

            float step = NTHumanUpdate.GetUpdateInterval(NTAfflictions.AfflictionPriority.HIGH) / 60f;
            if (step <= 0f) return;

            if (stepOwner != character) ResetSteps(character);

            stepAccumulator += Math.Min(simulatedDeltaTime, MaxSimulatedDeltaTime);

            int steps = 0;
            while (stepAccumulator >= step && steps < MaxStepsPerFrame)
            {
                stepAccumulator -= step;
                steps++;

                try
                {
                    RunStep(ntHuman, step);
                }
                catch (Exception e)
                {
                    LogStepFailure(e);
                    stepAccumulator = 0f;
                    break;
                }
            }

            if (steps == MaxStepsPerFrame) stepAccumulator = 0f;
        }

        private static void RunStep(NTHuman ntHuman, float step)
        {
            if (!HF.IsCharacterValid(ntHuman.Human) || ntHuman.Human.IsDead) return;

            ntHuman.PreHook(step);
            ntHuman.UpdateStats(step);
            ntHuman.UpdateAfflictions(ntHuman.FetchAfflictions(Priorities.All), step);
            ntHuman.PostHook(step);
        }

        private static void LogStepFailure(Exception e)
        {
            stepFailureCount++;

            if (stepFailureCount <= 3 || stepFailureCount % 300 == 0)
                Logger.LogError($"[SOS] Neurotrauma step failed ({stepFailureCount} times): {e}");
        }

        private static void ResetSteps(Character? character)
        {
            stepOwner = character;
            stepAccumulator = 0f;
            stepFailureCount = 0;
        }

        private static class Priorities
        {
            public static readonly List<NTAfflictions.AfflictionPriority> All =
            [
                NTAfflictions.AfflictionPriority.HIGH,
                NTAfflictions.AfflictionPriority.MEDIUM,
                NTAfflictions.AfflictionPriority.LOW,
            ];
        }
    }
}
