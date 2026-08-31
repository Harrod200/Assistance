using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Modding
{
	// Token: 0x02000959 RID: 2393
	public class ModMapper
	{
		// Token: 0x06005B1D RID: 23325 RVA: 0x002BE174 File Offset: 0x002BC374
		public ModMapper()
		{
			this.SetupDirectories();
			this.SetupFiles();
			this.GetModMap();
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x002BE1F4 File Offset: 0x002BC3F4
		private void SetupDirectories()
		{
			Debug.Log("Setting Up Directories");
			foreach (string text in this.requiredDirectories)
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
			}
		}

		// Token: 0x06005B1F RID: 23327 RVA: 0x002BE25C File Offset: 0x002BC45C
		private void SetupFiles()
		{
			Debug.Log("Setting Up Files");
			foreach (string text in this.requiredFiles)
			{
				if (!File.Exists(text))
				{
					StreamWriter streamWriter = new StreamWriter(text, false);
					streamWriter.Write("[]");
					streamWriter.Close();
				}
			}
		}

		// Token: 0x06005B20 RID: 23328 RVA: 0x002BE2D4 File Offset: 0x002BC4D4
		public List<ModMap> GetModMap()
		{
			Debug.Log("Getting Mod Map");
			Debug.Log("Loading mod map.");
			string text = File.ReadAllText("Mods/Mods");
			try
			{
				this.cachedModMapList = JsonConvert.DeserializeObject<List<ModMap>>(text, new JsonConverter[]
				{
					new ExpandoObjectConverter()
				});
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
				this.cachedModMapList = new List<ModMap>();
			}
			foreach (ModMap modMap in new List<ModMap>(this.cachedModMapList))
			{
				if (modMap.FilePath.Contains("ModInfo.json"))
				{
					this.cachedModMapList.Remove(modMap);
				}
			}
			return this.cachedModMapList;
		}

		// Token: 0x06005B21 RID: 23329 RVA: 0x002BE3A4 File Offset: 0x002BC5A4
		public List<ModMap> ScanMods()
		{
			Debug.Log("Scanning Mods");
			Debug.Log("Scanning for new mods.");
			List<string> list = Directory.GetFiles("Mods/Enabled", "*", SearchOption.AllDirectories).ToList<string>();
			for (int i = 0; i < list.Count<string>(); i++)
			{
				list[i] = list[i].Replace("\\", "/");
			}
			foreach (string text in new List<string>(list))
			{
				if (text.Contains("ModInfo.json"))
				{
					list.Remove(text);
				}
			}
			foreach (string text2 in list)
			{
				HashAlgorithm hashAlgorithm = HashAlgorithm.Create();
				using (FileStream fileStream = new FileStream(text2, FileMode.Open, FileAccess.Read))
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (byte b in hashAlgorithm.ComputeHash(fileStream))
					{
						stringBuilder.Append(b.ToString("X2"));
					}
					this.modMapList.Add(new ModMap
					{
						FilePath = text2,
						Hash = stringBuilder.ToString()
					});
				}
			}
			return this.modMapList;
		}

		// Token: 0x06005B22 RID: 23330 RVA: 0x002BE534 File Offset: 0x002BC734
		public bool IsModListCurrent()
		{
			Debug.Log("Checking If Mod Map Is Current");
			this.ScanMods();
			if (this.cachedModMapList == null || this.modMapList == null)
			{
				Debug.Log("Error loading mod list.");
				return false;
			}
			if (this.cachedModMapList.Count<ModMap>() == this.modMapList.Count<ModMap>())
			{
				using (List<ModMap>.Enumerator enumerator = this.cachedModMapList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ModMap cachedModMap = enumerator.Current;
						try
						{
							if (this.modMapList.Single<ModMap>((ModMap x) => x.FilePath == cachedModMap.FilePath).Hash != cachedModMap.Hash)
							{
								Debug.Log("New mods detected.");
								return false;
							}
						}
						catch (Exception ex)
						{
							Debug.Log("Error loading mods. Assuming change happened." + ex.Message);
							return false;
						}
					}
				}
				Debug.Log("No new mods detected.");
				return true;
			}
			Debug.Log("New mods detected.");
			return false;
		}

		// Token: 0x06005B23 RID: 23331 RVA: 0x002BE650 File Offset: 0x002BC850
		public void UpdateModMap(List<ModMap> newModMap)
		{
			Debug.Log("Updating Mod Map");
			Debug.Log("Updating mod map.");
			this.modMapList = newModMap;
		}

		// Token: 0x06005B24 RID: 23332 RVA: 0x002BE66D File Offset: 0x002BC86D
		public void UpdateModMap()
		{
			Debug.Log("Updating Mod Map");
			if (this.cachedModMapList == null)
			{
				this.ScanMods();
			}
			this.UpdateModMap(this.cachedModMapList);
		}

		// Token: 0x06005B25 RID: 23333 RVA: 0x002BE694 File Offset: 0x002BC894
		public void SaveModMap()
		{
			Debug.Log("Saving Mod Map");
			Debug.Log("Saving mod map.");
			string text = JsonConvert.SerializeObject(this.modMapList, Formatting.Indented);
			File.WriteAllText("Mods/Mods", text);
		}

		// Token: 0x06005B26 RID: 23334 RVA: 0x002BE6D0 File Offset: 0x002BC8D0
		public void MarkModAsValid(string modFilePath, bool valid = true)
		{
			Debug.Log("Marking Mods As Valid");
			foreach (ModMap modMap in this.modMapList)
			{
				if (modMap.FilePath == modFilePath)
				{
					modMap.Valid = valid;
				}
			}
		}

		// Token: 0x06005B27 RID: 23335 RVA: 0x002BE73C File Offset: 0x002BC93C
		public void AddErrors(string modFilePath, List<string> errorMessages)
		{
			Debug.Log("Taking Note Of Mod Errors");
			foreach (ModMap modMap in this.modMapList)
			{
				if (modMap.FilePath == modFilePath)
				{
					modMap.errorMessages.AddRange(errorMessages);
				}
			}
		}

		// Token: 0x04004178 RID: 16760
		private List<ModMap> modMapList = new List<ModMap>();

		// Token: 0x04004179 RID: 16761
		private List<ModMap> cachedModMapList = new List<ModMap>();

		// Token: 0x0400417A RID: 16762
		private List<string> requiredDirectories = new List<string> { "Mods", "Mods/Enabled", "Mods/Disabled" };

		// Token: 0x0400417B RID: 16763
		private List<string> requiredFiles = new List<string> { "Mods/Mods" };
	}
}
