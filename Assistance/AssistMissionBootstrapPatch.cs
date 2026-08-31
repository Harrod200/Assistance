using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;

namespace Assistance
{
    [HarmonyPatch(typeof(SolarSystemBootstrap), "Initialize")]
    internal static class AssistMissionBootstrapPatch
    {
        public static void Postfix()
        {
            if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
            {
                try
                {
                    RegisterMissionTemplate();
                    int councilorTypes = GrantToAllCouncilors();

                    if (Main.mod != null)
                    {
                        Main.mod.Logger.Log(string.Concat(new object[] { "Assist mission registered. Grants: councilorTypes=", councilorTypes, "." }));
                    }
                }
                catch (Exception ex)
                {
                    if (Main.mod != null)
                    {
                        Main.mod.Logger.Error("Failed to register Assist mission: " + ex);
                    }
                }
            }
        }

        private static void RegisterMissionTemplate()
        {
            try
            {
                var assistMission = new TIMissionTemplate_Assist();
                if (Main.mod != null)
                {
                    Main.mod.Logger.Log("Created TIMissionTemplate_Assist instance.");
                    Main.mod.Logger.Log("  - dataName: " + assistMission.dataName);
                    Main.mod.Logger.Log("  - friendlyName: " + assistMission.friendlyName);
                    Main.mod.Logger.Log("  - resolutionMethod: " + (assistMission.resolutionMethod != null ? assistMission.resolutionMethod.GetType().Name : "NULL"));
                    Main.mod.Logger.Log("  - attackingModifiers count: " + (assistMission.resolutionMethod != null ? assistMission.resolutionMethod.attackingModifiers.Count : -1));
                    Main.mod.Logger.Log("  - defendingModifiers count: " + (assistMission.resolutionMethod != null ? assistMission.resolutionMethod.defendingModifiers.Count : -1));
                    Main.mod.Logger.Log("  - attackerContexts count: " + assistMission.attackerContexts.Count);
                    Main.mod.Logger.Log("  - defenderContexts count: " + assistMission.defenderContexts.Count);
                    Main.mod.Logger.Log("  - conditions count: " + assistMission.conditions.Count);
                }

                TemplateManager.Add(assistMission, typeof(TIMissionTemplate), true);

                if (Main.mod != null)
                {
                    Main.mod.Logger.Log("Successfully registered Assist mission template with TemplateManager.");
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error("Error registering mission template: " + ex);
                }
            }
        }

        private static int GrantToAllCouncilors()
        {
            int count = 0;

            // Grant to all councilor types that don't have it yet
            // BUT: Only grant to player-controlled faction councilor types
            // This prevents AI factions from ever attempting to use this mission
            foreach (TICouncilorTypeTemplate councilorType in TemplateManager.GetAllTemplates<TICouncilorTypeTemplate>(true))
            {
                if (councilorType != null && !Contains(councilorType.missionNames, "Assist"))
                {
                    // Check if this councilor type belongs to a player faction
                    // If it's part of AI faction setup, skip it
                    // This is a conservative approach - we'll grant it to all types at registration
                    // and rely on TIMissionCondition_PlayerFactionOnly to filter at runtime
                    councilorType.missionNames = Append(councilorType.missionNames, "Assist");
                    councilorType._missions = null;
                    count++;
                }
            }

            return count;
        }

        private static bool Contains(string[] values, string value)
        {
            return values != null && values.Contains(value);
        }

        private static string[] Append(string[] values, string value)
        {
            string[] result;
            if (values == null)
            {
                result = new string[] { value };
            }
            else
            {
                string[] newArray = new string[values.Length + 1];
                Array.Copy(values, newArray, values.Length);
                newArray[newArray.Length - 1] = value;
                result = newArray;
            }
            return result;
        }

        private static void ClearPrivateCache(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, null);
            }
        }

        private const string MissionName = "Assist";
    }
}
