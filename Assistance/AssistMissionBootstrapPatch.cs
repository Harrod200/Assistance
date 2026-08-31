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
            TemplateManager.Add(new TIMissionTemplate_Assist(), typeof(TIMissionTemplate), true);
        }

        private static int GrantToAllCouncilors()
        {
            int count = 0;

            // Grant to all councilor types that don't have it yet
            foreach (TICouncilorTypeTemplate councilorType in TemplateManager.GetAllTemplates<TICouncilorTypeTemplate>(true))
            {
                if (councilorType != null && !Contains(councilorType.missionNames, "Assist"))
                {
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
