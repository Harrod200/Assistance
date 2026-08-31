using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009D3 RID: 2515
	public class AudioInitializer : MonoBehaviour
	{
		// Token: 0x17001038 RID: 4152
		// (get) Token: 0x06005E61 RID: 24161 RVA: 0x002CD799 File Offset: 0x002CB999
		public static AudioInitializer Instance
		{
			get
			{
				return AudioInitializer._instance;
			}
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x002CD7A0 File Offset: 0x002CB9A0
		private void Awake()
		{
			if (AudioInitializer._instance != null && AudioInitializer._instance != this)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				AudioInitializer._instance = this;
				global::UnityEngine.Object.DontDestroyOnLoad(this);
			}
			ModTemplateManager.LoadFMODBankMods();
			this.LoadAllFMODBanks();
			BusManager.Initialize();
			AudioManager.Initialize();
		}

		// Token: 0x06005E63 RID: 24163 RVA: 0x002CD7F8 File Offset: 0x002CB9F8
		private void LoadAllFMODBanks()
		{
			string[] array = Directory.EnumerateDirectories(Application.streamingAssetsPath + "/Audio/").ToArray<string>();
			List<string> list = new List<string>();
			foreach (string text in array)
			{
				list.AddRange(Directory.GetFiles(text, "*.*", SearchOption.AllDirectories).ToList<string>());
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = list[j].Replace("\\", "/");
			}
			List<string> list2 = list.Where<string>((string s) => s.Contains(".bank") && !s.Contains(".meta")).ToList<string>();
			new List<string>();
			foreach (string text2 in list2)
			{
				Debug.Log("Loading FMOD bank " + text2);
				Bank bank;
				RuntimeManager.StudioSystem.loadBankFile(text2, LOAD_BANK_FLAGS.NORMAL, out bank);
			}
		}

		// Token: 0x04004377 RID: 17271
		private static AudioInitializer _instance;
	}
}
