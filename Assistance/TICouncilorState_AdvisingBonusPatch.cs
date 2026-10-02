using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Boosts TICouncilorState.AdvisingBonus() to include assist bonuses.
    ///
    /// AdvisingBonus(attribute) = councilor's raw stat / 100. It is the single building block behind
    /// everything Advise does: TINationState.GetAdvisingScore() and TIHabState.GetAdvisingAttribute()
    /// both sum AdvisingBonus(attribute)/rank across all current advisors (diminishing returns), and
    /// the vanilla TIMissionEffect_Advise.ApplyEffect() Special3 fallback (for target types that are
    /// neither a nation nor a hab) reads AdvisingBonus() directly for its own message. Those are the
    /// only three call sites in the game.
    ///
    /// This patch adds accumulated assist bonuses to the AdvisingBonus calculation, scaled by the
    /// assistPercentage setting (simulating skill overlap/efficiency loss). The base attribute
    /// continues to be handled by vanilla's own calculation (not modified here).
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

            if (assistBonus > 0)
            {
                // Apply assist bonus scaled by the efficiency setting (assistPercentage).
                // This simulates loss of effectiveness due to skill overlap, conflicting priorities, etc.
                // Formula: (assist bonus * assistPercentage) / 100 / 100
                // Example: 20 point bonus * 50% efficiency = 10 effective points, /100 for advising scale = 0.1
                float assistPercentage = Main.settings.assistPercentage / 100f;
                float additional = (assistBonus * assistPercentage) / 100f;
                __result += additional;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[AdviseMission] AdvisingBonus({0}) for '{1}': base {2:F3} + assist {3:F3} (bonus={4}, efficiency={5}%)",
                        attribute, __instance.displayName, __result - additional, additional, assistBonus, Main.settings.assistPercentage));
                }
            }
        }
    }
}
