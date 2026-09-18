using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    [HarmonyPatch]
    public static class UIMissionDetailBreakdown_RowInjectionPatch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.TypeByName("PavonisInteractive.TerraInvicta.UIMissionContestedDetailBreakdown")
                ?.GetMethod("RefreshWindow", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPostfix]
        public static void Postfix(
            object __instance,
            object mission,
            object councilor,
            object target)
        {
            var councilorState = councilor as TICouncilorState;
            if (councilorState == null || !AssistBonusTracker.IsPlayerControlled(councilorState))
            {
                return;
            }

            var missionTemplate = mission as TIMissionTemplate;
            if (missionTemplate == null)
            {
                return;
            }

            CouncilorAttribute activeAttribute = missionTemplate.primaryAttackerStat;
            if (activeAttribute != CouncilorAttribute.None)
            {
                int totalStatSheetValue = councilorState.GetAttribute(activeAttribute, true, true, true, false, false, false);
                int rawNakedStatValue = councilorState.GetAttribute(activeAttribute, false, false, false, false, false, false);
                int hiddenOrgContribution = totalStatSheetValue - rawNakedStatValue;

                if (hiddenOrgContribution > 0)
                {
                    __instance.GetType().GetMethod("AddModifierRow", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.Invoke(__instance, new object[] { $"Attached Orgs & Assets ({activeAttribute})", (float)hiddenOrgContribution, false });
                }
            }

            int activeAssistanceBonus = AssistBonusTracker.GetStatBonus(councilorState, activeAttribute);
            if (activeAssistanceBonus > 0)
            {
                __instance.GetType().GetMethod("AddModifierRow", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(__instance, new object[] { "Adjacent Squad Assistance Pool", (float)activeAssistanceBonus, false });
            }
        }
    }
}
