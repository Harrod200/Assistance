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

                    if (Main.mod != null && Main.settings.debugLogging)
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
                if (Main.mod != null && Main.settings.debugLogging)
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

                if (Main.mod != null && Main.settings.debugLogging)
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

            try
            {
                // Get all councilor types
                var councilTypes = TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true).ToList();

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("Found {0} councilor types to grant Assist mission to.", councilTypes.Count));
                }

                // Grant Assist mission to all councilor types
                // This adds the mission to each councilor type's available missions
                foreach (TICouncilorTypeTemplate councilType in councilTypes)
                {
                    if (councilType != null)
                    {
                        if (Main.mod != null && Main.settings.debugLogging)
                        {
                            Main.mod.Logger.Log(string.Format("Processing councilor type: {0}, current missions: {1}", councilType.dataName, string.Join(", ", councilType.missionNames ?? new string[0])));
                        }

                        if (!Contains(councilType.missionNames, MissionName))
                        {
                            councilType.missionNames = Append(councilType.missionNames, MissionName);

                            // Clear the cached missions list so it gets repopulated with the new mission
                            ClearPrivateCache(councilType, "_missions");

                            count++;

                            if (Main.mod != null && Main.settings.debugLogging)
                            {
                                Main.mod.Logger.Log(string.Format("Granted Assist mission to councilor type: {0}", councilType.dataName));
                            }
                        }
                        else if (Main.mod != null && Main.settings.debugLogging)
                        {
                            Main.mod.Logger.Log(string.Format("Assist mission already present in councilor type: {0}", councilType.dataName));
                        }
                    }
                }

                if (Main.mod != null && Main.settings.debugLogging && count > 0)
                {
                    Main.mod.Logger.Log(string.Format("Successfully granted Assist mission to {0} councilor types.", count));
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error("Error granting Assist mission to councilor types: " + ex);
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
