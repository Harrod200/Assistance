using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Modding;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F9 RID: 1785
	public class NamelistManager
	{
		// Token: 0x06002A6B RID: 10859 RVA: 0x000E64B0 File Offset: 0x000E46B0
		public NamelistManager()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> Load Namelists", delegate
			{
				this.LoadNamelists(true);
			}, true, true);
			Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
		}

		// Token: 0x06002A6C RID: 10860 RVA: 0x000E64E4 File Offset: 0x000E46E4
		public void LoadNamelists(bool startup)
		{
			if (startup)
			{
				TIPlayerProfileManager.LoadPlayerConfig(true);
			}
			if (TIPlayerProfileManager.useMods && !TIPlayerProfileManager.loadingFailureDueToMods)
			{
				this.StageModNameLists();
			}
			this.namelists = new Dictionary<Type, INamelist>();
			try
			{
				this.Load<CouncilorName>(new CouncilorNameParser());
				this.Load<OrgName>(new OrgNameParser());
				this.Load<SpaceAssetName>(new SpaceAssetNameParser());
			}
			catch (Exception ex) when (TIPlayerProfileManager.useMods && !TIPlayerProfileManager.loadingFailureDueToMods)
			{
				TIPlayerProfileManager.HandleModFailure();
				Debug.LogError("Mod Manager failed to parse Namelists: " + ex.Message);
				StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
				if (startMenuController != null)
				{
					startMenuController.BankModFailureWarning("UI.StartScreen.Mods.ModWarningHeaderFailGeneral", "UI.StartScreen.Mods.ModWarningDescriptionFailGeneral", ex.Message, string.Join("\n", ModTemplateManager.nameListModPaths));
				}
			}
		}

		// Token: 0x06002A6D RID: 10861 RVA: 0x000E65C4 File Offset: 0x000E47C4
		private void OnLanguageChangedEvent()
		{
			this.LoadNamelists(false);
		}

		// Token: 0x06002A6E RID: 10862 RVA: 0x000E65CD File Offset: 0x000E47CD
		public bool TryGetName<TKey>(TKey key, out string name) where TKey : INamelistKey<TKey>
		{
			name = this.GetName<TKey>(key);
			return !string.IsNullOrEmpty(name);
		}

		// Token: 0x06002A6F RID: 10863 RVA: 0x000E65E4 File Offset: 0x000E47E4
		public string GetName<TKey>(TKey key) where TKey : INamelistKey<TKey>
		{
			INamelist namelist;
			if (this.namelists.TryGetValue(typeof(TKey), out namelist))
			{
				return ((INamelist<TKey>)namelist).GetName(key);
			}
			return "NoNameList";
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x000E661C File Offset: 0x000E481C
		private void Load<TKey>(INamelistParser<TKey> parser) where TKey : INamelistKey<TKey>
		{
			string text = Application.streamingAssetsPath + "/Namelists";
			if (Error.IsDirectoryMissing(text))
			{
				return;
			}
			Type typeFromHandle = typeof(TKey);
			string text2 = text + "/" + typeFromHandle.Name + ".csv";
			List<string> list = new List<string>();
			if (TIPlayerProfileManager.useMods)
			{
				foreach (string text3 in ModTemplateManager.nameListModPaths)
				{
					if (text3.Contains(typeFromHandle.Name + ".csv"))
					{
						if (JsonController.IsReplaceableModFile(text3.Split(new string[] { typeFromHandle.Name + ".csv" }, StringSplitOptions.None)[0], text3))
						{
							text2 = text3;
						}
						else
						{
							list.Add(text3);
						}
					}
				}
			}
			if (Error.IsFileMissing(text2))
			{
				return;
			}
			Namelist<TKey> namelist = new Namelist<TKey>(text2, parser, list);
			this.namelists.Add(typeFromHandle, namelist);
		}

		// Token: 0x06002A71 RID: 10865 RVA: 0x000E6728 File Offset: 0x000E4928
		private void StageModNameLists()
		{
			Debug.Log("Staging Mod Namelists");
			TIPlayerProfileManager.VerifyModDirectories();
			ModTemplateManager.LoadNameListMods();
			Debug.Log("Done Staging Mod Namelists");
		}

		// Token: 0x0400209C RID: 8348
		private Dictionary<Type, INamelist> namelists;
	}
}
