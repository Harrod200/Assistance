using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;

namespace MoreRealisticNukes
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(SolarSystemBootstrap), "Initialize")]
	internal static class DisarmNukesMissionBootstrapPatch
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000025F4 File Offset: 0x000007F4
		public static void Postfix()
		{
			if (Main.enabled && Main.settings != null && Main.settings.EnableDisarmNukesMission)
			{
				try
				{
					DisarmNukesMissionBootstrapPatch.RegisterMissionTemplate();
					int num = DisarmNukesMissionBootstrapPatch.GrantToCouncilorTypes();
					int num2 = DisarmNukesMissionBootstrapPatch.GrantToOrgs();
					int num3 = DisarmNukesMissionBootstrapPatch.GrantToTraits();
					if (Main.mod != null)
					{
						Main.mod.Logger.Log(string.Concat(new object[] { "Disarm Nukes mission registered. Grants: councilorTypes=", num, ", orgs=", num2, ", traits=", num3, "." }));
					}
				}
				catch (Exception ex)
				{
					if (Main.mod != null)
					{
						Main.mod.Logger.Error("Failed to register Disarm Nukes mission: " + ex);
					}
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000026F8 File Offset: 0x000008F8
		private static void RegisterMissionTemplate()
		{
			TemplateManager.Add(new TIMissionTemplate_DisarmNukes(), typeof(TIMissionTemplate), true);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002714 File Offset: 0x00000914
		private static int GrantToCouncilorTypes()
		{
			int num = 0;
			foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate in TemplateManager.GetAllTemplates<TICouncilorTypeTemplate>(true))
			{
				if (ticouncilorTypeTemplate != null && DisarmNukesMissionBootstrapPatch.Contains(ticouncilorTypeTemplate.missionNames, "SabotageFacilities") && !DisarmNukesMissionBootstrapPatch.Contains(ticouncilorTypeTemplate.missionNames, "DisarmNukes"))
				{
					ticouncilorTypeTemplate.missionNames = DisarmNukesMissionBootstrapPatch.Append(ticouncilorTypeTemplate.missionNames, "DisarmNukes");
					ticouncilorTypeTemplate._missions = null;
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000027A8 File Offset: 0x000009A8
		private static int GrantToOrgs()
		{
			int num = 0;
			foreach (TIOrgTemplate tiorgTemplate in TemplateManager.GetAllTemplates<TIOrgTemplate>(true))
			{
				if (tiorgTemplate != null && DisarmNukesMissionBootstrapPatch.Contains(tiorgTemplate.missionsGrantedNames, "SabotageFacilities") && !DisarmNukesMissionBootstrapPatch.Contains(tiorgTemplate.missionsGrantedNames, "DisarmNukes"))
				{
					tiorgTemplate.missionsGrantedNames = DisarmNukesMissionBootstrapPatch.Append(tiorgTemplate.missionsGrantedNames, "DisarmNukes");
					tiorgTemplate.grantsMarked = false;
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000283C File Offset: 0x00000A3C
		private static int GrantToTraits()
		{
			int num = 0;
			foreach (TITraitTemplate titraitTemplate in TemplateManager.GetAllTemplates<TITraitTemplate>(true))
			{
				if (titraitTemplate != null && titraitTemplate.missionsGrantedNames != null && titraitTemplate.missionsGrantedNames.Contains("SabotageFacilities") && !titraitTemplate.missionsGrantedNames.Contains("DisarmNukes"))
				{
					titraitTemplate.missionsGrantedNames.Add("DisarmNukes");
					DisarmNukesMissionBootstrapPatch.ClearPrivateCache(titraitTemplate, "_missionsGranted");
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000028D8 File Offset: 0x00000AD8
		private static bool Contains(string[] values, string value)
		{
			return values != null && values.Contains(value);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000028F8 File Offset: 0x00000AF8
		private static string[] Append(string[] values, string value)
		{
			string[] array;
			if (values == null)
			{
				array = new string[] { value };
			}
			else
			{
				string[] array2 = new string[values.Length + 1];
				Array.Copy(values, array2, values.Length);
				array2[array2.Length - 1] = value;
				array = array2;
			}
			return array;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002944 File Offset: 0x00000B44
		private static void ClearPrivateCache(object target, string fieldName)
		{
			FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(target, null);
			}
		}

		// Token: 0x04000018 RID: 24
		private const string SourceMissionName = "SabotageFacilities";

		// Token: 0x04000019 RID: 25
		private const string DisarmMissionName = "DisarmNukes";
	}
}
