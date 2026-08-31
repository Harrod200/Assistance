using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F8 RID: 1784
	public class LocalizationManager
	{
		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06002A56 RID: 10838 RVA: 0x000E58B4 File Offset: 0x000E3AB4
		// (set) Token: 0x06002A57 RID: 10839 RVA: 0x000E58BC File Offset: 0x000E3ABC
		public TILocalizationTemplate currentLocalizationTemplate { get; private set; }

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000E58C5 File Offset: 0x000E3AC5
		// (set) Token: 0x06002A59 RID: 10841 RVA: 0x000E58CD File Offset: 0x000E3ACD
		public TILocalizationTemplate priorLocalizationTemplate { get; private set; }

		// Token: 0x06002A5A RID: 10842 RVA: 0x000E58D8 File Offset: 0x000E3AD8
		public LocalizationManager()
		{
			this.languages = new Dictionary<string, IDictionary<string, string>>();
			Log.Time("<color=#00cc00>LoadTime:</color> Initialize Localization", delegate
			{
				string text = Application.streamingAssetsPath + "/Localization";
				if (!Directory.Exists("Mods/Enabled"))
				{
					Directory.CreateDirectory("Mods/Enabled");
				}
				if (!Directory.Exists("Mods/Disabled"))
				{
					Directory.CreateDirectory("Mods/Disabled");
				}
				string text2 = "Mods/Enabled";
				string text3 = "DLC_Content";
				if (!Directory.Exists(text))
				{
					Log.Warn("No localization path found at: " + text, Array.Empty<object>());
					return;
				}
				for (int i = 0; i < 22; i++)
				{
					this.testArgs.Add(i.ToString());
				}
				TILocalizationTemplate[] allTemplates = TemplateManager.GetAllTemplates<TILocalizationTemplate>(true);
				List<string> list = new List<string>();
				foreach (string text4 in Directory.EnumerateFiles(text2, "*", SearchOption.AllDirectories))
				{
					string[] array = Path.GetFileName(text4).Split(new char[] { '.' });
					if (array.Length >= 2)
					{
						string newExtension = array[1];
						if (!list.Contains(newExtension) && allTemplates.Any<TILocalizationTemplate>((TILocalizationTemplate x) => x.dataName == newExtension))
						{
							list.Add(newExtension);
						}
					}
				}
				foreach (TILocalizationTemplate tilocalizationTemplate in allTemplates)
				{
					if (!Directory.Exists(TIUtilities.CombineStrings(new string[] { text, "/", tilocalizationTemplate.dataName })) && list.Contains(tilocalizationTemplate.dataName))
					{
						Directory.CreateDirectory(TIUtilities.CombineStrings(new string[] { text, "/", tilocalizationTemplate.dataName }));
					}
				}
				foreach (string text5 in Directory.EnumerateDirectories(text))
				{
					this.locs = new Dictionary<string, string>();
					foreach (string text6 in Directory.EnumerateFiles(text5))
					{
						if (!text6.EndsWith("meta"))
						{
							this.ProcessLocEntry(text6, false);
						}
					}
					if (Directory.Exists(text3))
					{
						foreach (string text7 in Directory.EnumerateFiles(text3, "*." + Path.GetFileName(text5), SearchOption.AllDirectories))
						{
							if (!text7.EndsWith("meta"))
							{
								this.ProcessLocEntry(text7, true);
							}
						}
					}
					foreach (string text8 in Directory.EnumerateFiles(text2, "*." + Path.GetFileName(text5), SearchOption.AllDirectories))
					{
						if (!text8.EndsWith("meta"))
						{
							this.ProcessLocEntry(text8, true);
						}
					}
					if (this.locs.Any<KeyValuePair<string, string>>())
					{
						this.languages.Add(Path.GetFileName(text5), this.locs);
					}
				}
				Log.Info(string.Format("Loaded {0} languages", this.languages.Count), Array.Empty<object>());
			}, true, true);
			if (this.logInvalidEntryError)
			{
				Log.Error(this.badStringErrors.ToString(), Array.Empty<object>());
			}
			if (this.logInvalidEntryWarning)
			{
				Log.Warn(this.badStringWarnings.ToString(), Array.Empty<object>());
			}
		}

		// Token: 0x06002A5B RID: 10843 RVA: 0x000E5988 File Offset: 0x000E3B88
		private void ProcessLocEntry(string filename, bool moddedLoc = false)
		{
			int num = 0;
			foreach (string text in File.ReadLines(filename))
			{
				num++;
				if (!string.IsNullOrWhiteSpace(text))
				{
					int num2 = text.IndexOf("=", StringComparison.Ordinal);
					if (num2 < 1)
					{
						this.LogInvalidEntry(filename, num, text, 1, false);
					}
					else
					{
						string text2 = text.Substring(0, num2);
						string text3 = text.Substring(num2 + 1, text.Length - num2 - 1);
						text3 = Regex.Replace(text3, "\\t", "");
						text3 = new StringBuilder(text3).Replace("<br/>", Environment.NewLine).Replace("\n", Environment.NewLine).Replace("<h>", "<color=#EC9933>")
							.Replace("</h>", "</color>")
							.Replace("<rcol>", new StringBuilder("<line-height=0.01%>").Append(Environment.NewLine).Append("<align=\"right\">").ToString())
							.Replace("</rcol>", "</align></line-height>")
							.ToString();
						if (text3.Contains("//"))
						{
							int num3 = text3.IndexOf("//", StringComparison.Ordinal);
							text3 = text3.Substring(0, num3);
						}
						text3 = text3.TrimEnd(new char[] { ' ' });
						text3 = text3.Replace("<sp>", " ").ToString();
						if (string.IsNullOrWhiteSpace(text2))
						{
							this.LogInvalidEntry(filename, num, text, 2, false);
						}
						else if (string.IsNullOrWhiteSpace(text3))
						{
							this.LogInvalidEntry(filename, num, text, 3, false);
						}
						else
						{
							text3 = text3.Replace("<skip/>", string.Empty);
							if (this.locs.ContainsKey(text2))
							{
								if (moddedLoc)
								{
									this.locs.Remove(text2);
									this.locs.Add(text2, text3);
								}
								else
								{
									this.LogInvalidEntry(filename, num, text, 4, true);
								}
							}
							else
							{
								this.locs.Add(text2, text3);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002A5C RID: 10844 RVA: 0x000E5BAC File Offset: 0x000E3DAC
		public void SetLanguage(string name)
		{
			if (this.currentLanguageKey == name)
			{
				return;
			}
			IDictionary<string, string> dictionary;
			if (this.languages.TryGetValue(name, out dictionary))
			{
				this.currentLanguage = dictionary;
				this.currentLanguageKey = name;
				this.priorLocalizationTemplate = this.currentLocalizationTemplate;
				this.currentLocalizationTemplate = TemplateManager.Find<TILocalizationTemplate>(name, false);
				if (this.currentLocalizationTemplate == null)
				{
					Log.Error("Can't find loc template: " + name, Array.Empty<object>());
					return;
				}
			}
			else
			{
				Log.Error("Unknown language: " + name, Array.Empty<object>());
			}
		}

		// Token: 0x06002A5D RID: 10845 RVA: 0x000E5C34 File Offset: 0x000E3E34
		public bool IsLanguageActive(string name)
		{
			TILocalizationTemplate tilocalizationTemplate = TemplateManager.Find<TILocalizationTemplate>(name, false);
			return tilocalizationTemplate != null && tilocalizationTemplate.active;
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x000E5C54 File Offset: 0x000E3E54
		public ICollection<string> Languages()
		{
			return this.languages.Keys;
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x000E5C61 File Offset: 0x000E3E61
		public string Find(string key)
		{
			return this.GetKeyValue(key);
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x000E5C6C File Offset: 0x000E3E6C
		public string Find(string key, params object[] args)
		{
			string keyValue = this.GetKeyValue(key);
			string[] array = keyValue.Split(new char[] { '{' });
			int num = 0;
			for (int i = 1; i < array.Length; i++)
			{
				int num2;
				if (array[i].Length > 1 && int.TryParse(array[i].Substring(0, 1), out num2) && num2 > num)
				{
					num = num2;
				}
			}
			if (num + 1 <= args.Length || args.Length == 0)
			{
				if (keyValue == key)
				{
					return key;
				}
				try
				{
					return string.Format(keyValue, args);
				}
				catch
				{
					Debug.LogError("Bad string formatting in " + key);
					return key;
				}
			}
			Debug.LogWarning("Too many arg tags in loc string: : " + key);
			return key;
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x000E5D28 File Offset: 0x000E3F28
		public string Find_Fallback(string key, string fallbackKey)
		{
			string text = this.Find(key);
			if (text == key)
			{
				text = this.Find(fallbackKey);
			}
			return text;
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x000E5D50 File Offset: 0x000E3F50
		public string Find_Fallback(string key, string fallbackKey, params object[] args)
		{
			string text = this.Find(key, args);
			if (text == key)
			{
				text = this.Find(fallbackKey, args);
			}
			return text;
		}

		// Token: 0x06002A63 RID: 10851 RVA: 0x000E5D7C File Offset: 0x000E3F7C
		public string Test(string value, params object[] args)
		{
			string[] array = value.Split(new char[] { '{' });
			int num = 0;
			for (int i = 1; i < array.Length; i++)
			{
				int num2;
				if (array[i].Length > 1 && int.TryParse(array[i].Substring(0, 1), out num2) && num2 > num)
				{
					num = num2;
				}
			}
			if (num + 1 <= args.Length || args.Length == 0)
			{
				return string.Format(value, args);
			}
			Debug.LogWarning("Too many arg tags in loc string: : " + value);
			return value;
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x000E5DF4 File Offset: 0x000E3FF4
		public List<string> FindAllKeys(string substring)
		{
			return this.currentLanguage.Keys.Where<string>((string x) => x.Contains(substring)).ToList<string>();
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x000E5E2F File Offset: 0x000E402F
		public TMP_FontAsset GetHeaderFontAsset()
		{
			if (this.currentFontKey != this.currentLanguageKey)
			{
				this.LoadRequiredLanguageFonts(this.currentLanguageKey);
				this.currentFontKey = this.currentLanguageKey;
			}
			return this.languageHeaderFont;
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x000E5E62 File Offset: 0x000E4062
		public TMP_FontAsset GetBodyFontAsset()
		{
			if (this.currentFontKey != this.currentLanguageKey)
			{
				this.LoadRequiredLanguageFonts(this.currentLanguageKey);
				this.currentFontKey = this.currentLanguageKey;
			}
			return this.languageBodyFont;
		}

		// Token: 0x06002A67 RID: 10855 RVA: 0x000E5E98 File Offset: 0x000E4098
		public void LoadRequiredLanguageFonts(string name)
		{
			if (this.currentLocalizationTemplate == null)
			{
				Log.Error("Unknown language: " + name, Array.Empty<object>());
				return;
			}
			this.languageHeaderFont = Resources.Load<TMP_FontAsset>("All Fonts/" + this.currentLocalizationTemplate.headlineFontPath);
			if (this.languageHeaderFont == null)
			{
				this.languageHeaderFont = Resources.Load<TMP_FontAsset>("All Fonts/" + TemplateManager.Find<TILocalizationTemplate>("en", false).headlineFontPath);
			}
			if (this.languageHeaderFont == null)
			{
				Log.Error("No header font asset found: " + name, Array.Empty<object>());
				return;
			}
			this.languageBodyFont = Resources.Load<TMP_FontAsset>("All Fonts/" + this.currentLocalizationTemplate.bodyTextFontPath);
			if (this.languageBodyFont == null)
			{
				this.languageBodyFont = Resources.Load<TMP_FontAsset>("All Fonts/" + TemplateManager.Find<TILocalizationTemplate>("en", false).bodyTextFontPath);
			}
			if (this.languageBodyFont == null)
			{
				Log.Error("No body font asset found: " + name, Array.Empty<object>());
			}
		}

		// Token: 0x06002A68 RID: 10856 RVA: 0x000E5FB4 File Offset: 0x000E41B4
		private void LogInvalidEntry(string filename, int lineNumber, string line, int errorCode, bool error = false)
		{
			if (error && this.logInvalidEntryError)
			{
				this.badStringErrors.Append("Invalid localization entry ({0}:{1}) : {2}").Append(" ").Append(Path.GetFileName(filename))
					.Append(" ")
					.Append(lineNumber)
					.Append(" ")
					.Append(line)
					.Append(" EC: ")
					.Append(errorCode)
					.AppendLine();
				return;
			}
			if (this.logInvalidEntryWarning)
			{
				this.badStringWarnings.Append("Invalid localization entry ({0}:{1}) : {2}").Append(" ").Append(Path.GetFileName(filename))
					.Append(" ")
					.Append(lineNumber)
					.Append(" ")
					.Append(line)
					.Append(" EC: ")
					.Append(errorCode)
					.AppendLine();
			}
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x000E6090 File Offset: 0x000E4290
		private string GetKeyValue(string key)
		{
			if (this.currentLanguage == null)
			{
				Log.Error("No Language Set", Array.Empty<object>());
				return key;
			}
			string text;
			if (this.currentLanguage.TryGetValue(key, out text))
			{
				return text;
			}
			string text2;
			if (!this.currentLocalizationTemplate.core && this.languages["en"].TryGetValue(key, out text2))
			{
				return text2;
			}
			return key;
		}

		// Token: 0x0400208D RID: 8333
		public string currentLanguageKey;

		// Token: 0x0400208E RID: 8334
		public string currentFontKey;

		// Token: 0x0400208F RID: 8335
		private TMP_FontAsset languageHeaderFont;

		// Token: 0x04002090 RID: 8336
		private TMP_FontAsset languageBodyFont;

		// Token: 0x04002093 RID: 8339
		private const string InvalidEntry = "Invalid localization entry ({0}:{1}) : {2}";

		// Token: 0x04002094 RID: 8340
		private readonly IDictionary<string, IDictionary<string, string>> languages;

		// Token: 0x04002095 RID: 8341
		private IDictionary<string, string> currentLanguage;

		// Token: 0x04002096 RID: 8342
		private bool logInvalidEntryError;

		// Token: 0x04002097 RID: 8343
		private bool logInvalidEntryWarning;

		// Token: 0x04002098 RID: 8344
		private List<string> testArgs = new List<string>();

		// Token: 0x04002099 RID: 8345
		private Dictionary<string, string> locs = new Dictionary<string, string>();

		// Token: 0x0400209A RID: 8346
		private StringBuilder badStringErrors = new StringBuilder("Loc Errors").AppendLine();

		// Token: 0x0400209B RID: 8347
		private StringBuilder badStringWarnings = new StringBuilder("Loc Warnings").AppendLine();
	}
}
