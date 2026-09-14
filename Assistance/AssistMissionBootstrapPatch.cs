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

                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("Assist mission template registered successfully.");
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
    }
}

