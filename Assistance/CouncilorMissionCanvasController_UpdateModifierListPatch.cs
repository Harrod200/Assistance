using System;
using System.Collections.Generic;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

namespace Assistance
{
    /// <summary>
    /// Harmony patches for UI methods to prevent crashes when displaying Assist mission modifiers.
    /// </summary>

    /// <summary>
    /// Patch for Loc.T to add null-safety and prevent crashes from missing localization strings.
    /// </summary>
    [HarmonyPatch(typeof(Loc), "T", new System.Type[] { typeof(string) })]
    public static class Loc_T_NullSafetyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, string key)
        {
            try
            {
                // If Loc.T returns null, replace with the key itself as fallback
                if (__result == null)
                {
                    __result = key ?? "MISSING_LOCALIZATION";
                    Debug.LogWarning("[AssistMission] Loc.T returned null for key: " + key + ", using fallback");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[AssistMission] Error in Loc.T postfix: " + e.Message);
            }
        }
    }

    /// <summary>
    /// Patch for CouncilorMissionCanvasController.UpdateModifierList to add exception handling.
    /// </summary>
    [HarmonyPatch]
    public static class CouncilorMissionCanvasController_UpdateModifierListPatch
    {
        [HarmonyTargetMethod]
        public static System.Reflection.MethodBase TargetMethod()
        {
            // Try to find the UpdateModifierList method by reflection
            var controllerType = Type.GetType("PavonisInteractive.TerraInvicta.CouncilorMissionCanvasController, Assembly-CSharp");
            if (controllerType != null)
            {
                var method = controllerType.GetMethod("UpdateModifierList",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new Type[] { typeof(List<TIMissionModifier>), typeof(ListManagerBase), typeof(bool), typeof(float) },
                    null);

                if (method != null)
                {
                    Debug.Log("[AssistMission] Successfully found UpdateModifierList method for patching");
                    return method;
                }
            }

            Debug.LogWarning("[AssistMission] Could not find UpdateModifierList method for patching");
            return null;
        }

        [HarmonyPrefix]
        public static bool Prefix(object __instance, List<TIMissionModifier> modifierList, object uiList, bool hidden, float total)
        {
            try
            {
                // Validate inputs before proceeding
                if (modifierList == null)
                {
                    Debug.LogWarning("[AssistMission] UpdateModifierList: modifierList is null, skipping");
                    return false;
                }

                if (uiList == null)
                {
                    Debug.LogWarning("[AssistMission] UpdateModifierList: uiList is null, skipping");
                    return false;
                }

                // Proceed with original method
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("[AssistMission] UpdateModifierList prefix error: " + e.Message + "\n" + e.StackTrace);
                return false;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(object __instance, List<TIMissionModifier> modifierList, object uiList, bool hidden, float total)
        {
            try
            {
                // Post-process to ensure no issues
                Debug.Log("[AssistMission] UpdateModifierList completed successfully");
            }
            catch (Exception e)
            {
                Debug.LogError("[AssistMission] UpdateModifierList postfix error: " + e.Message);
            }
        }
    }
}

