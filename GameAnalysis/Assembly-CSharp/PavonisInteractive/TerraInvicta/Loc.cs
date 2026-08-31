using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F7 RID: 1783
	public static class Loc
	{
		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06002A44 RID: 10820 RVA: 0x000E5574 File Offset: 0x000E3774
		// (remove) Token: 0x06002A45 RID: 10821 RVA: 0x000E55A8 File Offset: 0x000E37A8
		public static event Action OnLanguageChangedEvent;

		// Token: 0x06002A46 RID: 10822 RVA: 0x000E55DB File Offset: 0x000E37DB
		static Loc()
		{
			if (Application.isPlaying)
			{
				Loc.localizationManager = GlobalInstaller.container.Resolve<LocalizationManager>();
			}
		}

		// Token: 0x06002A47 RID: 10823 RVA: 0x000E55F3 File Offset: 0x000E37F3
		public static string T(string key)
		{
			return Loc.localizationManager.Find(key);
		}

		// Token: 0x06002A48 RID: 10824 RVA: 0x000E5600 File Offset: 0x000E3800
		public static string T(string key, params object[] args)
		{
			return Loc.localizationManager.Find(key, args);
		}

		// Token: 0x06002A49 RID: 10825 RVA: 0x000E5610 File Offset: 0x000E3810
		public static string T_Scenario(string key)
		{
			TIMetaTemplate scenarioTemplate = GameControl.control.scenarioTemplate;
			string text = ((scenarioTemplate != null) ? scenarioTemplate.scenarioLocalizationPostfix : null);
			if (string.IsNullOrEmpty(text))
			{
				return Loc.localizationManager.Find(key);
			}
			return Loc.localizationManager.Find_Fallback(key + text, key);
		}

		// Token: 0x06002A4A RID: 10826 RVA: 0x000E565C File Offset: 0x000E385C
		public static string T_Scenario(string key, params object[] args)
		{
			string scenarioLocalizationPostfix = GameControl.control.scenarioTemplate.scenarioLocalizationPostfix;
			if (string.IsNullOrEmpty(scenarioLocalizationPostfix))
			{
				return Loc.localizationManager.Find(key, args);
			}
			return Loc.localizationManager.Find_Fallback(key + scenarioLocalizationPostfix, key, args);
		}

		// Token: 0x06002A4B RID: 10827 RVA: 0x000E56A1 File Offset: 0x000E38A1
		public static ICollection<string> Languages()
		{
			return Loc.localizationManager.Languages();
		}

		// Token: 0x06002A4C RID: 10828 RVA: 0x000E56AD File Offset: 0x000E38AD
		public static List<string> FindAllKeys(string substring)
		{
			return Loc.localizationManager.FindAllKeys(substring);
		}

		// Token: 0x06002A4D RID: 10829 RVA: 0x000E56BA File Offset: 0x000E38BA
		public static void SetLanguage(string name)
		{
			if (Application.isPlaying)
			{
				Loc.localizationManager.SetLanguage(name);
				Action onLanguageChangedEvent = Loc.OnLanguageChangedEvent;
				if (onLanguageChangedEvent == null)
				{
					return;
				}
				onLanguageChangedEvent();
			}
		}

		// Token: 0x06002A4E RID: 10830 RVA: 0x000E56E0 File Offset: 0x000E38E0
		public static string defaultLanguage()
		{
			SystemLanguage systemLanguage = Application.systemLanguage;
			if (systemLanguage <= SystemLanguage.French)
			{
				if (systemLanguage == SystemLanguage.Czech)
				{
					return "cze";
				}
				if (systemLanguage == SystemLanguage.English)
				{
					return "en";
				}
				if (systemLanguage == SystemLanguage.French)
				{
					return "fr";
				}
			}
			else
			{
				if (systemLanguage == SystemLanguage.German)
				{
					return "deu";
				}
				switch (systemLanguage)
				{
				case SystemLanguage.Italian:
					return "ita";
				case SystemLanguage.Japanese:
					return "jpn";
				case SystemLanguage.Korean:
					return "kor";
				case SystemLanguage.Latvian:
				case SystemLanguage.Lithuanian:
				case SystemLanguage.Norwegian:
				case SystemLanguage.Romanian:
					break;
				case SystemLanguage.Polish:
					return "pol";
				case SystemLanguage.Portuguese:
					return "por";
				case SystemLanguage.Russian:
					return "rus";
				default:
					switch (systemLanguage)
					{
					case SystemLanguage.Spanish:
						return "esp";
					case SystemLanguage.Ukrainian:
						return "ukr";
					case SystemLanguage.ChineseSimplified:
						return "chs";
					case SystemLanguage.ChineseTraditional:
						return "cht";
					case SystemLanguage.Unknown:
						return "en";
					}
					break;
				}
			}
			return "en";
		}

		// Token: 0x06002A4F RID: 10831 RVA: 0x000E57D4 File Offset: 0x000E39D4
		public static string GetDefaultLanguageKey()
		{
			string text = Loc.defaultLanguage();
			if (Loc.localizationManager.IsLanguageActive(text))
			{
				return text;
			}
			return "en";
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06002A50 RID: 10832 RVA: 0x000E57FB File Offset: 0x000E39FB
		public static string CurrentLanguage
		{
			get
			{
				return Loc.localizationManager.currentLanguageKey;
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x06002A51 RID: 10833 RVA: 0x000E5807 File Offset: 0x000E3A07
		public static TILocalizationTemplate currentLocalizationTemplate
		{
			get
			{
				return Loc.localizationManager.currentLocalizationTemplate;
			}
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06002A52 RID: 10834 RVA: 0x000E5813 File Offset: 0x000E3A13
		public static TILocalizationTemplate priorLocalizationTemplate
		{
			get
			{
				return Loc.localizationManager.priorLocalizationTemplate;
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06002A53 RID: 10835 RVA: 0x000E581F File Offset: 0x000E3A1F
		public static TMP_FontAsset CurrentHeaderFont
		{
			get
			{
				return Loc.localizationManager.GetHeaderFontAsset();
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06002A54 RID: 10836 RVA: 0x000E582B File Offset: 0x000E3A2B
		public static TMP_FontAsset CurrentBodyFont
		{
			get
			{
				return Loc.localizationManager.GetBodyFontAsset();
			}
		}

		// Token: 0x06002A55 RID: 10837 RVA: 0x000E5838 File Offset: 0x000E3A38
		public static void SwapFonts(GameObject gameObject)
		{
			if (!Loc.currentLocalizationTemplate.requiresFontChange)
			{
				TILocalizationTemplate priorLocalizationTemplate = Loc.priorLocalizationTemplate;
				if (priorLocalizationTemplate == null || !priorLocalizationTemplate.requiresFontChange)
				{
					return;
				}
			}
			TextMeshProUGUI[] componentsInChildren = gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].font.name.ToUpperInvariant().Contains("-HEADER"))
				{
					componentsInChildren[i].font = Loc.CurrentHeaderFont;
				}
				else
				{
					componentsInChildren[i].font = Loc.CurrentBodyFont;
				}
			}
		}

		// Token: 0x0400208C RID: 8332
		private static readonly LocalizationManager localizationManager;
	}
}
