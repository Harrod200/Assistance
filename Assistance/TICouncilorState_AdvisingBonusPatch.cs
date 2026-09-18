using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

namespace Assistance
{
    /// <summary>
    /// Boosts TICouncilorState.AdvisingBonus() rather than replacing the Advise mission effect.
    ///
    /// AdvisingBonus(attribute) = councilor's raw stat / 100. It is the single building block behind
    /// everything Advise does: TINationState.GetAdvisingScore() and TIHabState.GetAdvisingAttribute()
    /// both sum AdvisingBonus(attribute)/rank across all current advisors (diminishing returns), and
    /// the vanilla TIMissionEffect_Advise.ApplyEffect() Special3 fallback (for target types that are
    /// neither a nation nor a hab) reads AdvisingBonus() directly for its own message. Those are the
    /// only three call sites in the game.
    ///
    /// Boosting this one method means nation, hab, and that third target type all pick up the mod's
    /// advisePercentage setting and the councilor's accumulated Assist bonus automatically - with
    /// vanilla's own ranking, stacking, message formatting (including the nation-Command-is-flat vs
    /// everything-else-is-percentage distinction), and outcome handling left completely untouched.
    /// No need to duplicate any of that here, and no target-type branch is ever silently dropped.
    ///
    /// This supersedes the previous approach of Harmony-prefixing TIMissionEffect_Advise.ApplyEffect
    /// and fully reimplementing it (see the now-inert TIMissionEffect_Advise.cs).
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "AdvisingBonus")]
    internal static class TICouncilorState_AdvisingBonusPatch
    {
        [HarmonyPostfix]
        public static void Postfix(TICouncilorState __instance, CouncilorAttribute attribute, ref float __result)
        {
            if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission || __instance == null)
                return;

            // Non-destructive read - clearing is handled solely by TICouncilorState_CompleteMissionPatch.
            int assistBonus = AssistBonusTracker.GetStatBonus(__instance, attribute);
            float assistPercentage = Main.settings.assistPercentage / 100f;

            float additional = 0f;

            if (assistPercentage > 0f)
            {
                // Same effective-value shape as the rest of the mod: the councilor's raw stat, boosted
                // by any accumulated Assist bonus, scaled by the configured assist percentage.
                int rawAttribute = __instance.GetAttribute(attribute, true, true, true, false, false, false);
                int totalStat = rawAttribute + assistBonus;

                // Apply stat cap if enabled, but ensure the natural stat is never reduced
                if (Main.settings.statCapEnabled)
                {
                    totalStat = Mathf.Max(rawAttribute, Mathf.Min(totalStat, Main.settings.statCapLimit));
                }

                additional += totalStat * assistPercentage / 100f;
            }

            if (assistBonus > 0)
            {
                // Matches vanilla's own AdvisingBonus scale (raw stat / 100), so the accumulated Assist
                // bonus is folded in on the same footing as the councilor's own stat.
                additional += assistBonus / 100f;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[AdviseMission] AdvisingBonus({0}) for '{1}': base {2:F3} + assist/percentage {3:F3}",
                        attribute, __instance.displayName, __result, additional));
                }
            }

            __result += additional;
        }
    }
}
