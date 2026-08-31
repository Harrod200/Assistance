using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Steamworks;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Modding
{
	// Token: 0x02000957 RID: 2391
	public class ModManager : MonoBehaviour
	{
		// Token: 0x06005B05 RID: 23301 RVA: 0x002BD268 File Offset: 0x002BB468
		private void Start()
		{
			this.modMenuController = base.GetComponent<ModMenuController>();
			if (!TIPlayerProfileManager.useMods)
			{
				return;
			}
			Debug.Log("Starting Mod Manager");
			this.modMapper = new ModMapper();
			this.jsonController = new JsonController();
		}

		// Token: 0x06005B06 RID: 23302 RVA: 0x002BD29E File Offset: 0x002BB49E
		public List<string> GetDisabledModFiles()
		{
			return this.disabledModFiles;
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x002BD2A8 File Offset: 0x002BB4A8
		public List<string> GetEnabledModFiles()
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
				Debug.Log("Adding Enabled Mod Directory: " + text);
			}
			string[] array4 = Directory.EnumerateDirectories("Mods/Disabled/").ToArray<string>();
			List<string> list2 = new List<string>();
			foreach (string text2 in array4)
			{
				list2.AddRange(Directory.GetFiles(text2, "*.*", SearchOption.AllDirectories).ToList<string>());
				ModManager.DisabledModDirectories.Add(text2);
				string[] array5 = new string[] { "Mods/Disabled/" };
				ModManager.DisabledModNames.Add(text2.Split(array5, StringSplitOptions.None)[1]);
				if (!Application.isEditor)
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
				Debug.Log(list[j]);
			}
			for (int k = 0; k < list2.Count; k++)
			{
				list2[k] = list2[k].Replace("\\", "/");
			}
			this.disabledModFiles = list2;
			return list;
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x002BD518 File Offset: 0x002BB718
		public void DisableMod(string mod)
		{
			Debug.Log("Disabling: " + mod);
			if (!Directory.Exists(mod.Replace("Enabled", "Disabled")))
			{
				Directory.Move(mod, mod.Replace("Enabled", "Disabled"));
				Debug.Log("Successfully Moved Files");
				this.modMenuController.RefreshInstalledMods();
				return;
			}
			this.modMenuController.ShowModWarningDialog(Loc.T("UI.StartScreen.Mods.ModWarningHeaderBadState"), Loc.T("UI.StartScreen.Mods.ModWarningDescriptionBadState", new object[] { mod.Replace("Mods/Enabled/", "") }));
		}

		// Token: 0x06005B09 RID: 23305 RVA: 0x002BD5B0 File Offset: 0x002BB7B0
		public void EnableMod(string mod)
		{
			Debug.Log("Enabling: " + mod);
			if (!Directory.Exists(mod.Replace("Disabled", "Enabled")))
			{
				Directory.Move(mod, mod.Replace("Disabled", "Enabled"));
				Debug.Log("Successfully Moved Files");
				this.modMenuController.RefreshInstalledMods();
				return;
			}
			this.modMenuController.ShowModWarningDialog(Loc.T("UI.StartScreen.Mods.ModWarningHeaderBadState"), Loc.T("UI.StartScreen.Mods.ModWarningDescriptionBadState", new object[] { mod.Replace("Mods/Enabled/", "") }));
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x002BD648 File Offset: 0x002BB848
		public void DeleteMod(string mod)
		{
			Debug.Log("Deleting: " + mod);
			string[] array = mod.Split(new char[] { '/' });
			string text = array[array.Length - 1];
			if (TIPlayerProfileManager.subscribedMods.ContainsKey(text))
			{
				string[] array2 = TIPlayerProfileManager.subscribedMods[text].Split(new char[] { '\\' });
				uint num;
				uint.TryParse(array2[array2.Length - 1], out num);
				SteamUGC.UnsubscribeItem(new PublishedFileId_t((ulong)num));
				TIPlayerProfileManager.subscribedMods.Remove(text);
				TIPlayerProfileManager.SavePlayerConfig();
			}
			if (mod.Contains("Mods/Disabled/"))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(mod);
				if (Utilities.CanDeleteDirectory(directoryInfo))
				{
					try
					{
						foreach (FileInfo fileInfo in from x in directoryInfo.GetFiles()
							orderby x.Name == "ModInfo.json"
							select x)
						{
							fileInfo.Delete();
						}
						Directory.Delete(mod, true);
						Debug.Log("Successfully Deleted: " + mod);
						goto IL_01C4;
					}
					catch (Exception)
					{
						this.modMenuController.ShowModWarningDialog(Loc.T("UI.StartScreen.Mods.ModWarningHeaderDeleteFail"), Loc.T("UI.StartScreen.Mods.ModWarningDescriptionDeleteFail"));
						if (!TIPlayerProfileManager.modsToUninstall.ContainsKey(mod.Replace("Mods/Disabled/", "")))
						{
							TIPlayerProfileManager.modsToUninstall.Add(mod.Replace("Mods/Disabled/", ""), mod);
							TIPlayerProfileManager.SavePlayerConfig();
						}
						goto IL_01C4;
					}
				}
				this.modMenuController.ShowModWarningDialog(Loc.T("UI.StartScreen.Mods.ModWarningHeaderDeleteFail"), Loc.T("UI.StartScreen.Mods.ModWarningDescriptionDeleteFail"));
				if (!TIPlayerProfileManager.modsToUninstall.ContainsKey(mod.Replace("Mods/Disabled/", "")))
				{
					TIPlayerProfileManager.modsToUninstall.Add(mod.Replace("Mods/Disabled/", ""), mod);
					TIPlayerProfileManager.SavePlayerConfig();
				}
				IL_01C4:
				this.modMenuController.RefreshInstalledMods();
			}
		}

		// Token: 0x06005B0B RID: 23307 RVA: 0x002BD840 File Offset: 0x002BBA40
		public static void TryRemoveMod(string mod)
		{
			Debug.Log("Disabling: " + mod);
			if (!Directory.Exists(mod.Replace("Enabled", "Disabled")))
			{
				Directory.Move(mod, mod.Replace("Enabled", "Disabled"));
				Debug.Log("Successfully Moved Files");
			}
		}

		// Token: 0x06005B0C RID: 23308 RVA: 0x002BD894 File Offset: 0x002BBA94
		private void ResetOldMods()
		{
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x002BD898 File Offset: 0x002BBA98
		public void LoadJsonMods()
		{
			this.jsonMods.Clear();
			Debug.Log("Setting Up Directories");
			foreach (string text in (from s in this.GetEnabledModFiles()
				where s.Contains(".json") && !s.Contains("ModInfo.json")
				select s).ToList<string>())
			{
				if (this.jsonController == null)
				{
					this.jsonController = new JsonController();
				}
				JsonMod jsonMod = this.jsonController.LoadJson(text);
				if (jsonMod == null)
				{
					Debug.LogWarning("Mod Manager Failed on: " + text);
					this.hitFailure = true;
					break;
				}
				this.jsonMods.Add(jsonMod);
				Debug.Log("Mod Template Found: " + jsonMod.ModFilePath);
			}
		}

		// Token: 0x06005B0E RID: 23310 RVA: 0x002BD984 File Offset: 0x002BBB84
		public List<JsonMod> GetModsForTemplate(string fileName)
		{
			List<JsonMod> list = new List<JsonMod>();
			foreach (JsonMod jsonMod in this.jsonMods)
			{
				if (jsonMod.ModFileName == fileName + ".json")
				{
					list.Add(jsonMod);
				}
			}
			list = list.OrderBy<JsonMod, int>((JsonMod o) => o.LoadOrder).ToList<JsonMod>();
			return list;
		}

		// Token: 0x06005B0F RID: 23311 RVA: 0x002BDA24 File Offset: 0x002BBC24
		private void ActivateMods()
		{
			this.ActivateJsonMods();
		}

		// Token: 0x06005B10 RID: 23312 RVA: 0x002BDA2C File Offset: 0x002BBC2C
		private void ActivateJsonMods()
		{
		}

		// Token: 0x06005B11 RID: 23313 RVA: 0x002BDA30 File Offset: 0x002BBC30
		private void GetModsConflict(ModType modType)
		{
			Debug.Log("Getting Mod Conflicts");
			if (modType == ModType.Json)
			{
				HashSet<string> modsUsingSameFile = new HashSet<string>((from x in this.jsonMods
					group x by x.ModFileName into g
					where g.Count<JsonMod>() > 1
					select g.Key).ToList<string>());
				List<JsonMod> list = (from x in this.jsonMods
					where modsUsingSameFile.Contains(x.ModFileName)
					select (x)).ToList<JsonMod>();
				using (List<JsonMod>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JsonMod outterJsonMod = enumerator.Current;
						using (List<JsonMod>.Enumerator enumerator2 = list.GetEnumerator())
						{
							Func<Mod, bool> <>9__11;
							while (enumerator2.MoveNext())
							{
								JsonMod innerJsonMod = enumerator2.Current;
								if (outterJsonMod != innerJsonMod)
								{
									HashSet<string> dataNames = outterJsonMod.GetDataNames();
									HashSet<string> innerJsonDataNames = innerJsonMod.GetDataNames();
									if (dataNames.Any<string>((string x) => innerJsonDataNames.Contains(x)))
									{
										Func<Mod, bool> <>9__12;
										foreach (string text in (from x in dataNames
											select (x) into g
											where innerJsonDataNames.Contains(g)
											select g).ToList<string>())
										{
											JObject jobject = outterJsonMod.GetJObject(text);
											JObject innerJsonModJObject = innerJsonMod.GetJObject(text);
											List<string> list2 = (from x in jobject.Properties()
												select x.Name into g
												where (from y in innerJsonModJObject.Properties()
													select y.Name).Contains(g)
												select g).ToList<string>();
											list2.Remove("dataName");
											foreach (string text2 in list2)
											{
												IEnumerable<Mod> enumerable = this.conflictingMods;
												Func<Mod, bool> func;
												if ((func = <>9__11) == null)
												{
													func = (<>9__11 = (Mod mod) => mod != outterJsonMod);
												}
												if (enumerable.Any<Mod>(func) || this.conflictingMods.Count<Mod>() == 0)
												{
													this.conflictingMods.Add(outterJsonMod);
												}
												IEnumerable<Mod> enumerable2 = this.conflictingMods;
												Func<Mod, bool> func2;
												if ((func2 = <>9__12) == null)
												{
													func2 = (<>9__12 = (Mod mod) => mod != innerJsonMod || this.conflictingMods.Count<Mod>() == 0);
												}
												if (enumerable2.Any<Mod>(func2))
												{
													this.conflictingMods.Add(innerJsonMod);
												}
												outterJsonMod.errorList.Add(string.Concat(new string[] { "Duplicate keys: ", text2, " in file: ", outterJsonMod.ModFilePath, ", ", innerJsonMod.ModFilePath }));
												innerJsonMod.errorList.Add(string.Concat(new string[] { "Duplicate keys: ", text2, " in file: ", innerJsonMod.ModFilePath, ", ", outterJsonMod.ModFilePath }));
												this.modMapper.AddErrors(outterJsonMod.ModFilePath, new List<string> { string.Concat(new string[] { "Duplicate keys: \"", text2, "\" in file: \"", outterJsonMod.ModFilePath, "\", \"", innerJsonMod.ModFilePath, "\"" }) });
												this.modMapper.AddErrors(innerJsonMod.ModFilePath, new List<string> { string.Concat(new string[] { "Duplicate keys: \"", text2, "\" in file: \"", innerJsonMod.ModFilePath, "\", \"", outterJsonMod.ModFilePath, "\"" }) });
												this.modMapper.MarkModAsValid(outterJsonMod.ModFilePath, false);
												this.modMapper.MarkModAsValid(innerJsonMod.ModFilePath, false);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04004160 RID: 16736
		private JsonController jsonController;

		// Token: 0x04004161 RID: 16737
		private ModMenuController modMenuController;

		// Token: 0x04004162 RID: 16738
		private ModMapper modMapper;

		// Token: 0x04004163 RID: 16739
		public List<JsonMod> jsonMods = new List<JsonMod>();

		// Token: 0x04004164 RID: 16740
		private HashSet<Mod> conflictingMods = new HashSet<Mod>();

		// Token: 0x04004165 RID: 16741
		private string localizationPath = Application.streamingAssetsPath + "/Localization/";

		// Token: 0x04004166 RID: 16742
		private List<string> disabledModFiles = new List<string>();

		// Token: 0x04004167 RID: 16743
		public static List<string> ModDirectories = new List<string>();

		// Token: 0x04004168 RID: 16744
		public static List<string> DisabledModDirectories = new List<string>();

		// Token: 0x04004169 RID: 16745
		public static List<string> ModAssetBundles = new List<string>();

		// Token: 0x0400416A RID: 16746
		public static List<string> ModAssetBundleManifestFiles = new List<string>();

		// Token: 0x0400416B RID: 16747
		public static List<string> ModNames = new List<string>();

		// Token: 0x0400416C RID: 16748
		public static List<string> DisabledModNames = new List<string>();

		// Token: 0x0400416D RID: 16749
		public static bool checkedForModUpdates;

		// Token: 0x0400416E RID: 16750
		public static List<string> dlcDirectories = new List<string>();

		// Token: 0x0400416F RID: 16751
		public static List<string> dlcAssetbundles = new List<string>();

		// Token: 0x04004170 RID: 16752
		public static List<string> dlcAssetbundleManifestFiles = new List<string>();

		// Token: 0x04004171 RID: 16753
		public static List<string> dlcNames = new List<string>();

		// Token: 0x04004172 RID: 16754
		private bool hitFailure;

		// Token: 0x04004173 RID: 16755
		public static readonly string[] WorkshopTags = new string[] { "None", "Balance", "Gameplay", "Total Conversion", "Translation", "UI", "Councilors", "Ships", "Utilities" };
	}
}
