using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using PavonisInteractive.TerraInvicta.Modding;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007DE RID: 2014
	public class ModTemplateManager
	{
		// Token: 0x060048CB RID: 18635 RVA: 0x001DE614 File Offset: 0x001DC814
		public static void LoadJsonFromDLC()
		{
			ModTemplateManager.jsonDLCs.Clear();
			foreach (string text in (from s in ModTemplateManager.GetDLCContent(true)
				where s.Contains(".json") && !s.Contains("ModInfo.json")
				select s).ToList<string>())
			{
				if (ModTemplateManager.jsonController == null)
				{
					ModTemplateManager.jsonController = new JsonController();
				}
				JsonMod jsonMod = ModTemplateManager.jsonController.LoadJson(text);
				if (jsonMod == null)
				{
					Debug.LogWarning("Mod Manager Failed loading DLC content on: " + text);
					break;
				}
				ModTemplateManager.jsonDLCs.Add(jsonMod);
				Debug.Log("DLC Template Found: " + jsonMod.ModFilePath);
			}
		}

		// Token: 0x060048CC RID: 18636 RVA: 0x001DE6E8 File Offset: 0x001DC8E8
		public static void LoadJsonMods()
		{
			ModTemplateManager.jsonMods.Clear();
			Debug.Log("Setting Up Directories");
			foreach (string text in (from s in ModTemplateManager.GetEnabledModFiles(true)
				where s.Contains(".json") && !s.Contains("ModInfo.json")
				select s).ToList<string>())
			{
				if (ModTemplateManager.jsonController == null)
				{
					ModTemplateManager.jsonController = new JsonController();
				}
				JsonMod jsonMod = ModTemplateManager.jsonController.LoadJson(text);
				if (jsonMod == null)
				{
					Debug.LogWarning("Mod Manager Failed on: " + text);
					break;
				}
				ModTemplateManager.jsonMods.Add(jsonMod);
				Debug.Log("Mod Template Found: " + jsonMod.ModFilePath);
			}
		}

		// Token: 0x060048CD RID: 18637 RVA: 0x001DE7C8 File Offset: 0x001DC9C8
		public static void LoadNameListMods()
		{
			ModTemplateManager.nameListModPaths.Clear();
			List<string> list = (from s in ModTemplateManager.GetEnabledModFiles(false)
				where s.Contains("CouncilorName.csv") || s.Contains("OrgName.csv") || s.Contains("SpaceAssetName.csv")
				select s).ToList<string>();
			List<ModTemplateManager.NameListModInfo> list2 = new List<ModTemplateManager.NameListModInfo>();
			new List<int>();
			new List<string>();
			foreach (string text in list)
			{
				if (text == null)
				{
					Debug.LogWarning("Mod Manager Failed on: " + text);
					return;
				}
				list2.Add(new ModTemplateManager.NameListModInfo
				{
					path = text,
					loadOrder = JsonController.GetModLoadOrder(text)
				});
				Debug.Log("NameList Mod Template Found: " + text);
			}
			list2 = list2.OrderBy<ModTemplateManager.NameListModInfo, int>((ModTemplateManager.NameListModInfo o) => o.loadOrder).ToList<ModTemplateManager.NameListModInfo>();
			foreach (ModTemplateManager.NameListModInfo nameListModInfo in list2)
			{
				ModTemplateManager.nameListModPaths.Add(nameListModInfo.path);
			}
		}

		// Token: 0x060048CE RID: 18638 RVA: 0x001DE918 File Offset: 0x001DCB18
		public static void LoadFMODBankMods()
		{
			ModTemplateManager.FMODBankModPaths.Clear();
			List<string> list = (from s in ModTemplateManager.GetEnabledModFiles(false)
				where s.Contains(".bank") && !s.Contains(".guids")
				select s).ToList<string>();
			List<ModTemplateManager.FmodBankModInfo> list2 = new List<ModTemplateManager.FmodBankModInfo>();
			new List<int>();
			new List<string>();
			foreach (string text in list)
			{
				if (text == null)
				{
					Debug.LogWarning("Mod Manager Failed on: " + text);
					return;
				}
				list2.Add(new ModTemplateManager.FmodBankModInfo
				{
					path = text,
					loadOrder = JsonController.GetModLoadOrder(text)
				});
				Debug.Log("Audio Bank Mod Found: " + text);
			}
			list2 = list2.OrderBy<ModTemplateManager.FmodBankModInfo, int>((ModTemplateManager.FmodBankModInfo o) => o.loadOrder).ToList<ModTemplateManager.FmodBankModInfo>();
			foreach (ModTemplateManager.FmodBankModInfo fmodBankModInfo in list2)
			{
				ModTemplateManager.FMODBankModPaths.Add(fmodBankModInfo.path);
			}
			foreach (string text2 in ModTemplateManager.FMODBankModPaths)
			{
				Debug.Log("Loading mod audio bank " + text2);
				Bank bank;
				RuntimeManager.StudioSystem.loadBankFile(text2, LOAD_BANK_FLAGS.NORMAL, out bank);
			}
		}

		// Token: 0x060048CF RID: 18639 RVA: 0x001DEAC8 File Offset: 0x001DCCC8
		public static List<JsonMod> GetModsForTemplate(string fileName)
		{
			List<JsonMod> list = new List<JsonMod>();
			foreach (JsonMod jsonMod in ModTemplateManager.jsonMods)
			{
				if (jsonMod.ModFileName == fileName + ".json")
				{
					list.Add(jsonMod);
				}
			}
			list = list.OrderBy<JsonMod, int>((JsonMod o) => o.LoadOrder).ToList<JsonMod>();
			return list;
		}

		// Token: 0x060048D0 RID: 18640 RVA: 0x001DEB64 File Offset: 0x001DCD64
		public static List<JsonMod> GetDLCForTemplate(string fileName)
		{
			List<JsonMod> list = new List<JsonMod>();
			foreach (JsonMod jsonMod in ModTemplateManager.jsonDLCs)
			{
				if (jsonMod.ModFileName == fileName + ".json")
				{
					list.Add(jsonMod);
				}
			}
			list = list.OrderBy<JsonMod, int>((JsonMod o) => o.LoadOrder).ToList<JsonMod>();
			return list;
		}

		// Token: 0x060048D1 RID: 18641 RVA: 0x001DEC00 File Offset: 0x001DCE00
		public static List<string> GetDLCContent(bool logging = true)
		{
			ModManager.dlcDirectories.Clear();
			ModManager.dlcNames.Clear();
			ModManager.dlcAssetbundles.Clear();
			ModManager.dlcAssetbundleManifestFiles.Clear();
			if (!Directory.Exists("DLC_Content"))
			{
				return null;
			}
			if (logging)
			{
				Debug.Log("Getting All DLC Content");
			}
			new List<string>();
			string[] array = Directory.EnumerateDirectories("DLC_Content/").ToArray<string>();
			List<string> list = new List<string>();
			foreach (string text in array)
			{
				list.AddRange(Directory.GetFiles(text, "*.*", SearchOption.AllDirectories).ToList<string>());
				ModManager.dlcDirectories.Add(text);
				string[] array3 = new string[] { "DLC_Content/" };
				ModManager.dlcNames.Add(text.Split(array3, StringSplitOptions.None)[1]);
				if (logging)
				{
					Debug.Log("Adding DLC Directory: " + text);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = list[j].Replace("\\", "/");
				if (list[j].Contains(".manifest") && !list[j].Contains(".meta"))
				{
					ModManager.dlcAssetbundleManifestFiles.Add(list[j]);
					ModManager.dlcAssetbundles.Add(list[j].Replace(".manifest", ""));
				}
			}
			return list;
		}

		// Token: 0x060048D2 RID: 18642 RVA: 0x001DED70 File Offset: 0x001DCF70
		public static List<string> GetEnabledModFiles(bool logging = true)
		{
			Debug.Log("Getting All Mod Files");
			ModManager.ModDirectories.Clear();
			ModManager.ModAssetBundleManifestFiles.Clear();
			ModManager.ModAssetBundles.Clear();
			ModManager.ModNames.Clear();
			ModManager.DisabledModDirectories.Clear();
			ModManager.DisabledModNames.Clear();
			if (!Directory.Exists("Mods/Enabled"))
			{
				Directory.CreateDirectory("Mods/Enabled");
			}
			if (!Directory.Exists("Mods/Disabled"))
			{
				Directory.CreateDirectory("Mods/Disabled");
			}
			string[] array = Directory.EnumerateDirectories("Mods/Enabled/").ToArray<string>();
			List<string> list = new List<string>();
			foreach (string text in array)
			{
				list.AddRange(Directory.GetFiles(text, "*.*", SearchOption.AllDirectories).ToList<string>());
				ModManager.ModDirectories.Add(text);
				string[] array3 = new string[] { "Mods/Enabled/" };
				ModManager.ModNames.Add(text.Split(array3, StringSplitOptions.None)[1]);
				if (logging)
				{
					Debug.Log("Adding Enabled Mod Directory: " + text);
				}
			}
			string[] array4 = Directory.EnumerateDirectories("Mods/Disabled/").ToArray<string>();
			List<string> list2 = new List<string>();
			foreach (string text2 in array4)
			{
				list2.AddRange(Directory.GetFiles(text2, "*.*", SearchOption.AllDirectories).ToList<string>());
				ModManager.DisabledModDirectories.Add(text2);
				string[] array5 = new string[] { "Mods/Disabled/" };
				ModManager.DisabledModNames.Add(text2.Split(array5, StringSplitOptions.None)[1]);
				if (!Application.isEditor && logging)
				{
					Debug.Log("Adding Disabled Mod Directory: " + text2);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = list[j].Replace("\\", "/");
				if (list[j].Contains(".manifest") && !list[j].Contains(".meta"))
				{
					ModManager.ModAssetBundleManifestFiles.Add(list[j]);
					ModManager.ModAssetBundles.Add(list[j].Replace(".manifest", ""));
				}
				if (logging)
				{
					Debug.Log(list[j]);
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				list2[k] = list2[k].Replace("\\", "/");
			}
			ModTemplateManager.disabledModFiles = list2;
			return list;
		}

		// Token: 0x040029FA RID: 10746
		public static List<JsonMod> jsonMods = new List<JsonMod>();

		// Token: 0x040029FB RID: 10747
		public static List<JsonMod> jsonDLCs = new List<JsonMod>();

		// Token: 0x040029FC RID: 10748
		private static JsonController jsonController;

		// Token: 0x040029FD RID: 10749
		private static List<string> disabledModFiles = new List<string>();

		// Token: 0x040029FE RID: 10750
		public static List<string> nameListModPaths = new List<string>();

		// Token: 0x040029FF RID: 10751
		public static List<string> FMODBankModPaths = new List<string>();

		// Token: 0x02000F96 RID: 3990
		public class NameListModInfo
		{
			// Token: 0x04005ED2 RID: 24274
			public string path;

			// Token: 0x04005ED3 RID: 24275
			public int loadOrder;
		}

		// Token: 0x02000F97 RID: 3991
		public class FmodBankModInfo
		{
			// Token: 0x04005ED4 RID: 24276
			public string path;

			// Token: 0x04005ED5 RID: 24277
			public int loadOrder;
		}
	}
}
