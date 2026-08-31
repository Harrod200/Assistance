using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using PavonisInteractive.TerraInvicta.Modding;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000708 RID: 1800
	public class TemplateManager
	{
		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06002A8E RID: 10894 RVA: 0x000E6C49 File Offset: 0x000E4E49
		public bool Initialized
		{
			get
			{
				return this.initialized;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06002A8F RID: 10895 RVA: 0x000E6C54 File Offset: 0x000E4E54
		public static TIGlobalConfig global
		{
			get
			{
				if (TemplateManager.self._global != null)
				{
					return TemplateManager.self._global;
				}
				foreach (TIGlobalConfig tiglobalConfig in TemplateManager.IterateByClass<TIGlobalConfig>(false))
				{
					TemplateManager.self._global = tiglobalConfig;
				}
				return TemplateManager.self._global;
			}
		}

		// Token: 0x06002A90 RID: 10896 RVA: 0x000E6CC8 File Offset: 0x000E4EC8
		public void LoadMetaOnly(string templatePath)
		{
			TemplateManager.RegisterFileBasedTemplate(string.Join("/", new string[] { templatePath, "TIMetaTemplate.json" }), false);
		}

		// Token: 0x06002A91 RID: 10897 RVA: 0x000E6CEC File Offset: 0x000E4EEC
		public void Initialize(string templatePath)
		{
			if (this.initialized)
			{
				return;
			}
			Log.Time("<color=#00cc00>LoadTime:</color> Load All Templates", delegate
			{
				if (Directory.Exists("DLC_Content"))
				{
					ModTemplateManager.GetDLCContent(true);
				}
				TIPlayerProfileManager.LoadPlayerConfig(true);
				if (TIPlayerProfileManager.useMods && !TIPlayerProfileManager.loadingFailureDueToMods)
				{
					TemplateManager.StageModTemplates();
				}
				TemplateManager.RegisterFileBasedTemplates(templatePath, false);
				if (TIPlayerProfileManager.useMods)
				{
					this.LoadNonVanillaModTemplates();
				}
				if (!TemplateManager.self.foundGlobal)
				{
					TemplateManager.Add<TIGlobalConfig>(new TIGlobalConfig(TIGlobalConfig.globalName), false);
				}
				TemplateManager.ValidateAllTemplates();
			}, true, true);
			this.initialized = true;
		}

		// Token: 0x06002A92 RID: 10898 RVA: 0x000E6D35 File Offset: 0x000E4F35
		public static void InitializeStaticManagers()
		{
			PolicyManager.Initialize();
			OperationsManager.Initalize();
			ShipCommandsManager.Initialize();
		}

		// Token: 0x06002A93 RID: 10899 RVA: 0x000E6D48 File Offset: 0x000E4F48
		public static void ClearAllTemplates()
		{
			TemplateManager.self.templatesByName.Clear();
			TemplateManager.self.templatesByType.Clear();
			TemplateManager.self.duplicateTemplatesByType.Clear();
			TemplateManager.cachedHabModuleTemplates = null;
			TemplateManager.self._global = null;
			TemplateManager.self.initialized = false;
		}

		// Token: 0x06002A94 RID: 10900 RVA: 0x000E6D9E File Offset: 0x000E4F9E
		private static void StageModTemplates()
		{
			TIPlayerProfileManager.VerifyModDirectories();
			ModTemplateManager.LoadJsonMods();
			Debug.Log("Done Staging Mod Templates");
		}

		// Token: 0x06002A95 RID: 10901 RVA: 0x000E6DB4 File Offset: 0x000E4FB4
		private void LoadNonVanillaModTemplates()
		{
			foreach (JsonMod jsonMod in ModTemplateManager.jsonMods)
			{
				if (!jsonMod.foundVanillaMatch)
				{
					Debug.Log("Adding non vanilla template: " + jsonMod.ModFileName);
					Type type = TemplateManager.FindDataTemplateType(jsonMod.ModFileName.Replace(".json", ""), null);
					if (type != null)
					{
						TIDataTemplate[] array = FSSaveLoad.LoadDataTemplates(jsonMod.ModFilePath, type.MakeArrayType());
						if (array != null)
						{
							foreach (TIDataTemplate tidataTemplate in array)
							{
								if (tidataTemplate.dataName != null)
								{
									TemplateManager.Add(tidataTemplate, type, false);
								}
								else
								{
									Debug.Log("Attempting to add template of type " + type.ToString() + " with null dataName;");
								}
							}
						}
						else
						{
							Debug.LogError("Failed to deserialize a modded non-vanilla template: " + jsonMod.ModFilePath + ", Type : " + type.Name);
						}
					}
					else
					{
						Debug.LogError("Failed to find Type for a modded non-vanilla template: " + jsonMod.ModFilePath);
					}
				}
			}
		}

		// Token: 0x06002A96 RID: 10902 RVA: 0x000E6EE8 File Offset: 0x000E50E8
		public static IEnumerable<T> IterateByClass<T>(bool allowChild = true) where T : TIDataTemplate
		{
			Dictionary<string, TIDataTemplate> dictionary;
			if (allowChild)
			{
				Type tType = typeof(T);
				foreach (KeyValuePair<Type, Dictionary<string, TIDataTemplate>> keyValuePair in TemplateManager.self.templatesByType)
				{
					if (tType.IsAssignableFrom(keyValuePair.Key))
					{
						dictionary = TemplateManager.self.templatesByType[keyValuePair.Key];
						foreach (KeyValuePair<string, TIDataTemplate> keyValuePair2 in dictionary)
						{
							yield return keyValuePair2.Value as T;
						}
						Dictionary<string, TIDataTemplate>.Enumerator enumerator2 = default(Dictionary<string, TIDataTemplate>.Enumerator);
					}
				}
				IEnumerator<KeyValuePair<Type, Dictionary<string, TIDataTemplate>>> enumerator = null;
				tType = null;
			}
			else if (TemplateManager.self.templatesByType.TryGetValue(typeof(T), out dictionary))
			{
				foreach (KeyValuePair<string, TIDataTemplate> keyValuePair3 in dictionary)
				{
					yield return keyValuePair3.Value as T;
				}
				Dictionary<string, TIDataTemplate>.Enumerator enumerator2 = default(Dictionary<string, TIDataTemplate>.Enumerator);
			}
			yield break;
			yield break;
		}

		// Token: 0x06002A97 RID: 10903 RVA: 0x000E6EF8 File Offset: 0x000E50F8
		private static IEnumerable<string> GetScenarioTagsCollection(TIDataTemplate template)
		{
			IEnumerable<string> scenarioTags = template.scenarioTags;
			return scenarioTags ?? Enumerable.Empty<string>();
		}

		// Token: 0x06002A98 RID: 10904 RVA: 0x000E6F16 File Offset: 0x000E5116
		public static void Add<T>(T newTemplate, bool replaceDuplicate = false) where T : TIDataTemplate
		{
			TemplateManager.Add(newTemplate, typeof(T), replaceDuplicate);
		}

		// Token: 0x06002A99 RID: 10905 RVA: 0x000E6F30 File Offset: 0x000E5130
		public static void Add(TIDataTemplate newTemplate, Type type, bool replaceDuplicate = false)
		{
			string text;
			if (!newTemplate.IsValid(out text))
			{
				if (TIPlayerProfileManager.useMods)
				{
					TIPlayerProfileManager.HandleModFailure();
				}
				throw new Exception("Cannot add invalid template: " + text);
			}
			if (newTemplate.disable)
			{
				Dictionary<string, TIDataTemplate> dictionary = TemplateManager.self.templatesByType[typeof(TIMetaTemplate)];
				bool flag = false;
				using (Dictionary<string, TIDataTemplate>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((TIMetaTemplate)enumerator.Current).templateNames.Contains(newTemplate.dataName))
						{
							flag = true;
							Log.Error("Can't disable " + newTemplate.dataName + " because it's in a metatemplate", Array.Empty<object>());
							break;
						}
					}
				}
				if (!flag)
				{
					return;
				}
			}
			Dictionary<string, TIDataTemplate> dictionary2;
			if (TemplateManager.self.templatesByType.TryGetValue(type, out dictionary2))
			{
				TIDataTemplate tidataTemplate;
				if (dictionary2.TryGetValue(newTemplate.dataName, out tidataTemplate))
				{
					if (!replaceDuplicate)
					{
						HashSet<string> newTemplateTagsSet = new HashSet<string>(TemplateManager.GetScenarioTagsCollection(newTemplate));
						bool flag2 = newTemplateTagsSet.SetEquals(TemplateManager.GetScenarioTagsCollection(tidataTemplate));
						if (!flag2)
						{
							Dictionary<string, List<TIDataTemplate>> dictionary3;
							TemplateManager.self.duplicateTemplatesByType.TryGetValue(type, out dictionary3);
							string dataName = newTemplate.dataName;
							if (dictionary3 == null)
							{
								dictionary3 = (TemplateManager.self.duplicateTemplatesByType[type] = new Dictionary<string, List<TIDataTemplate>>());
							}
							List<TIDataTemplate> list;
							if (!dictionary3.TryGetValue(dataName, out list))
							{
								Dictionary<string, List<TIDataTemplate>> dictionary4 = dictionary3;
								string text2 = dataName;
								List<TIDataTemplate> list2 = new List<TIDataTemplate>();
								list2.Add(tidataTemplate);
								list = list2;
								dictionary4[text2] = list2;
							}
							flag2 = list.Any<TIDataTemplate>((TIDataTemplate x) => newTemplateTagsSet.SetEquals(TemplateManager.GetScenarioTagsCollection(x)));
							if (!flag2)
							{
								dictionary3[dataName].Add(newTemplate);
							}
						}
						if (flag2 && !GameControl.loadcycle100)
						{
							Log.Debug("Found template with the same dataName AND same scenarioTags : {0}, [{1}]", new object[]
							{
								newTemplate.dataName,
								newTemplateTagsSet.ToCommaSeparatedString<string>(null)
							});
						}
						return;
					}
					dictionary2.Remove(newTemplate.dataName);
				}
				dictionary2.Add(newTemplate.dataName, newTemplate);
			}
			else
			{
				TemplateManager.self.templatesByType[type] = new Dictionary<string, TIDataTemplate> { { newTemplate.dataName, newTemplate } };
			}
			Dictionary<Type, TIDataTemplate> dictionary5;
			if (TemplateManager.self.templatesByName.TryGetValue(newTemplate.dataName, out dictionary5))
			{
				if (dictionary5.ContainsKey(type))
				{
					if (!replaceDuplicate)
					{
						return;
					}
					dictionary5.Remove(type);
				}
				dictionary5.Add(type, newTemplate);
				return;
			}
			TemplateManager.self.templatesByName[newTemplate.dataName] = new Dictionary<Type, TIDataTemplate> { { type, newTemplate } };
		}

		// Token: 0x06002A9A RID: 10906 RVA: 0x000E71C0 File Offset: 0x000E53C0
		public static void Remove<T>(TIDataTemplate templateToRemove) where T : TIDataTemplate
		{
			TemplateManager.Remove(templateToRemove, typeof(T));
		}

		// Token: 0x06002A9B RID: 10907 RVA: 0x000E71D4 File Offset: 0x000E53D4
		public static void Remove(TIDataTemplate templateToRemove, Type type)
		{
			Dictionary<string, TIDataTemplate> dictionary;
			if (TemplateManager.self.templatesByType.TryGetValue(type, out dictionary))
			{
				dictionary.Remove(templateToRemove.dataName);
			}
			Dictionary<Type, TIDataTemplate> dictionary2;
			if (TemplateManager.self.templatesByName.TryGetValue(templateToRemove.dataName, out dictionary2))
			{
				dictionary2.Remove(type);
			}
		}

		// Token: 0x06002A9C RID: 10908 RVA: 0x000E7223 File Offset: 0x000E5423
		public static void ResolveScenarioTemplates(TIMetaTemplate scenarioTemplate)
		{
			TemplateManager.ResolveTaggedTemplates(scenarioTemplate);
			TemplateManager.ResolveDuplicateTemplates(scenarioTemplate);
			TemplateManager.self._global = null;
		}

		// Token: 0x06002A9D RID: 10909 RVA: 0x000E723C File Offset: 0x000E543C
		private static void ResolveTaggedTemplates(TIMetaTemplate scenarioTemplate)
		{
			IEnumerable<string> scenarioTags = TemplateManager.GetScenarioTagsCollection(scenarioTemplate);
			Func<TIDataTemplate, bool> <>9__0;
			foreach (KeyValuePair<Type, Dictionary<string, TIDataTemplate>> keyValuePair in TemplateManager.self.templatesByType)
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, TIDataTemplate> keyValuePair2 in keyValuePair.Value)
				{
					IEnumerable<string> scenarioTagsCollection = TemplateManager.GetScenarioTagsCollection(keyValuePair2.Value);
					if (scenarioTagsCollection.Intersect<string>(scenarioTags).Count<string>() != scenarioTagsCollection.Count<string>())
					{
						list.Add(keyValuePair2.Key);
					}
				}
				foreach (string text in list)
				{
					Dictionary<string, List<TIDataTemplate>> dictionary;
					List<TIDataTemplate> list2;
					if (TemplateManager.self.duplicateTemplatesByType.TryGetValue(keyValuePair.Key, out dictionary) && dictionary.TryGetValue(text, out list2))
					{
						IEnumerable<TIDataTemplate> enumerable = list2;
						Func<TIDataTemplate, bool> func;
						if ((func = <>9__0) == null)
						{
							func = (<>9__0 = delegate(TIDataTemplate x)
							{
								IEnumerable<string> scenarioTagsCollection2 = TemplateManager.GetScenarioTagsCollection(x);
								return scenarioTagsCollection2.Intersect<string>(scenarioTags).Count<string>() == scenarioTagsCollection2.Count<string>();
							});
						}
						List<TIDataTemplate> list3 = enumerable.Where<TIDataTemplate>(func).ToList<TIDataTemplate>();
						TIDataTemplate tidataTemplate = list3.FirstOrDefault<TIDataTemplate>();
						if (tidataTemplate != null)
						{
							foreach (TIDataTemplate tidataTemplate2 in list2.Except<TIDataTemplate>(list3).ToList<TIDataTemplate>())
							{
								TemplateManager.self.duplicateTemplatesByType[keyValuePair.Key][text].Remove(tidataTemplate2);
							}
							keyValuePair.Value[text] = tidataTemplate;
							TemplateManager.self.templatesByName[text][keyValuePair.Key] = tidataTemplate;
							continue;
						}
						keyValuePair.Value.Remove(text);
						TemplateManager.self.templatesByName[text].Remove(keyValuePair.Key);
						TemplateManager.self.duplicateTemplatesByType[keyValuePair.Key].Remove(text);
					}
					keyValuePair.Value.Remove(text);
					TemplateManager.self.templatesByName[text].Remove(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06002A9E RID: 10910 RVA: 0x000E7508 File Offset: 0x000E5708
		public static void ResolveDuplicateTemplates(TIMetaTemplate scenarioTemplate)
		{
			IEnumerable<string> scenarioTags = TemplateManager.GetScenarioTagsCollection(scenarioTemplate);
			Func<TIDataTemplate, int> <>9__0;
			Func<string, int> <>9__2;
			Func<TIDataTemplate, int> <>9__1;
			foreach (KeyValuePair<Type, Dictionary<string, List<TIDataTemplate>>> keyValuePair in TemplateManager.self.duplicateTemplatesByType)
			{
				foreach (KeyValuePair<string, List<TIDataTemplate>> keyValuePair2 in keyValuePair.Value.ToList<KeyValuePair<string, List<TIDataTemplate>>>())
				{
					IEnumerable<TIDataTemplate> value = keyValuePair2.Value;
					Func<TIDataTemplate, int> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (TIDataTemplate x) => TemplateManager.GetScenarioTagsCollection(x).Intersect<string>(scenarioTags).Count<string>());
					}
					IOrderedEnumerable<TIDataTemplate> orderedEnumerable = value.OrderByDescending<TIDataTemplate, int>(func);
					Func<TIDataTemplate, int> func2;
					if ((func2 = <>9__1) == null)
					{
						func2 = (<>9__1 = delegate(TIDataTemplate x)
						{
							IEnumerable<string> scenarioTagsCollection = TemplateManager.GetScenarioTagsCollection(x);
							Func<string, int> func3;
							if ((func3 = <>9__2) == null)
							{
								func3 = (<>9__2 = (string x) => scenarioTags.IndexOf(x));
							}
							IEnumerable<int> enumerable = from i in scenarioTagsCollection.Select<string, int>(func3)
								where i >= 0
								select i;
							if (!enumerable.Any<int>())
							{
								return int.MaxValue;
							}
							return enumerable.Min();
						});
					}
					TIDataTemplate tidataTemplate = orderedEnumerable.ThenBy<TIDataTemplate, int>(func2).First<TIDataTemplate>();
					TemplateManager.self.templatesByType[keyValuePair.Key][tidataTemplate.dataName] = tidataTemplate;
					TemplateManager.self.templatesByName[tidataTemplate.dataName][keyValuePair.Key] = tidataTemplate;
					TemplateManager.self.duplicateTemplatesByType[keyValuePair.Key].Remove(tidataTemplate.dataName);
				}
			}
		}

		// Token: 0x06002A9F RID: 10911 RVA: 0x000E7690 File Offset: 0x000E5890
		public static void RegisterEmptyParentTemplates(Type type)
		{
			TemplateManager.self.templatesByType[type] = new Dictionary<string, TIDataTemplate>();
		}

		// Token: 0x06002AA0 RID: 10912 RVA: 0x000E76A7 File Offset: 0x000E58A7
		public static T[] GetAllTemplates<T>(bool allowChild = true) where T : TIDataTemplate
		{
			return TemplateManager.IterateByClass<T>(allowChild).ToArray<T>();
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06002AA1 RID: 10913 RVA: 0x000E76B4 File Offset: 0x000E58B4
		public static IEnumerable<TIHabModuleTemplate> HabModuleTemplates
		{
			get
			{
				if (TemplateManager.cachedHabModuleTemplates == null)
				{
					TemplateManager.cachedHabModuleTemplates = TemplateManager.IterateByClass<TIHabModuleTemplate>(true).ToList<TIHabModuleTemplate>();
				}
				return TemplateManager.cachedHabModuleTemplates;
			}
		}

		// Token: 0x06002AA2 RID: 10914 RVA: 0x000E76D2 File Offset: 0x000E58D2
		public static T Find<T>(string templateName, bool allowChild = false) where T : TIDataTemplate
		{
			return TemplateManager.Find(templateName, typeof(T), allowChild) as T;
		}

		// Token: 0x06002AA3 RID: 10915 RVA: 0x000E76F0 File Offset: 0x000E58F0
		internal static TIDataTemplate Find(string templateName, Type T, bool allowChild = false)
		{
			if (string.IsNullOrEmpty(templateName))
			{
				return null;
			}
			TIDataTemplate tidataTemplate = null;
			Dictionary<Type, TIDataTemplate> dictionary;
			TemplateManager.self.templatesByName.TryGetValue(templateName, out dictionary);
			if (!allowChild)
			{
				if (T == null)
				{
					Debug.Log(templateName);
				}
				if (dictionary != null)
				{
					dictionary.TryGetValue(T, out tidataTemplate);
				}
				return tidataTemplate;
			}
			tidataTemplate = TemplateManager.Find(templateName, T, false);
			if (tidataTemplate != null)
			{
				return tidataTemplate;
			}
			if (dictionary == null)
			{
				return null;
			}
			foreach (TIDataTemplate tidataTemplate2 in dictionary.Values)
			{
				if (T.IsInstanceOfType(tidataTemplate2))
				{
					return tidataTemplate2;
				}
			}
			return null;
		}

		// Token: 0x06002AA4 RID: 10916 RVA: 0x000E77A4 File Offset: 0x000E59A4
		private static void ValidateAllTemplates()
		{
			using (IEnumerator<TIDataTemplate> enumerator = TemplateManager.IterateByClass<TIDataTemplate>(true).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text;
					if (!enumerator.Current.IsValid(out text))
					{
						Debug.Log(text);
					}
				}
			}
		}

		// Token: 0x06002AA5 RID: 10917 RVA: 0x000E77F8 File Offset: 0x000E59F8
		private static void RegisterFileBasedTemplates(string templatePath, bool replaceDuplicates = false)
		{
			if (!Directory.Exists(templatePath))
			{
				Log.Error("TemplateManager.InitTemplates -- could not find template path " + templatePath, Array.Empty<object>());
				return;
			}
			IEnumerable<string> files = Directory.GetFiles(templatePath, "*.json", SearchOption.AllDirectories);
			IEnumerable<string> enumerable = Enumerable.Empty<string>();
			if (Directory.Exists("DLC_Content"))
			{
				enumerable = Directory.GetFiles("DLC_Content", "*.json", SearchOption.AllDirectories);
			}
			string[] array = (from x in files.Concat<string>(enumerable).ToArray<string>()
				orderby x.Contains("TIMetaTemplate.json") descending
				select x).ToArray<string>();
			for (int i = 0; i < array.Length; i++)
			{
				TemplateManager.RegisterFileBasedTemplate(array[i], replaceDuplicates);
			}
		}

		// Token: 0x06002AA6 RID: 10918 RVA: 0x000E78A0 File Offset: 0x000E5AA0
		private static void RegisterFileBasedTemplate(string templateFile, bool replaceDuplicates = false)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(templateFile);
			Type type = TemplateManager.FindDataTemplateType(fileNameWithoutExtension, null);
			if (type == typeof(TIGlobalConfig))
			{
				TemplateManager.self.foundGlobal = true;
			}
			if (type == null)
			{
				Debug.LogError("templateType is null for " + templateFile);
				return;
			}
			TIDataTemplate[] array;
			if (TIPlayerProfileManager.useMods)
			{
				List<JsonMod> modsForTemplate = ModTemplateManager.GetModsForTemplate(fileNameWithoutExtension);
				if (modsForTemplate.Count > 0)
				{
					bool flag = templateFile.Contains("DLC_Content");
					List<JObject> list = TemplateManager.jController.LoadJson(templateFile).FileContents;
					foreach (JsonMod jsonMod in modsForTemplate)
					{
						MergeArrayHandling mergeArrayHandling = MergeArrayHandling.Merge;
						if (jsonMod.TemplatesToConcatArrays != null && jsonMod.TemplatesToConcatArrays.Contains(fileNameWithoutExtension + ".json"))
						{
							mergeArrayHandling = MergeArrayHandling.Concat;
						}
						if (jsonMod.TemplatesToReplaceArrays != null && jsonMod.TemplatesToReplaceArrays.Contains(fileNameWithoutExtension + ".json"))
						{
							mergeArrayHandling = MergeArrayHandling.Replace;
						}
						if (jsonMod.TemplatesToReplace != null && jsonMod.TemplatesToReplace.Contains(fileNameWithoutExtension + ".json"))
						{
							list = jsonMod.FileContents;
						}
						else
						{
							list = TemplateManager.jController.CombineJson(list, jsonMod.FileContents, flag, mergeArrayHandling);
						}
						Debug.Log("Successfully Merged Mod Template: " + jsonMod.ModFilePath);
						jsonMod.SetFoundMatch();
					}
					array = FSSaveLoad.LoadDataTemplatesFromString(TemplateManager.jController.jObjectListToString(list), type.MakeArrayType());
				}
				else
				{
					array = FSSaveLoad.LoadDataTemplates(templateFile, type.MakeArrayType());
				}
			}
			else
			{
				array = FSSaveLoad.LoadDataTemplates(templateFile, type.MakeArrayType());
			}
			foreach (TIDataTemplate tidataTemplate in array)
			{
				if (tidataTemplate.dataName != null)
				{
					TemplateManager.Add(tidataTemplate, type, replaceDuplicates);
				}
				else
				{
					Debug.Log("Attempting to add template of type " + type.ToString() + " with null dataName;");
				}
			}
		}

		// Token: 0x06002AA7 RID: 10919 RVA: 0x000E7AA4 File Offset: 0x000E5CA4
		private static void RegisterClassBasedTemplates()
		{
			Type builderType = typeof(ITemplateBuilder);
			foreach (Type type in from p in AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>((Assembly s) => s.GetTypes())
				where builderType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract
				select p)
			{
				foreach (TIDataTemplate tidataTemplate in (Activator.CreateInstance(type) as ITemplateBuilder).BuildTemplates())
				{
					Type type2 = tidataTemplate.GetType();
					if (tidataTemplate.dataName != null && type2 != null)
					{
						TemplateManager.Add(tidataTemplate, type2, false);
					}
					else
					{
						Debug.Log("Attempting to add template of type " + type2.ToString() + " with null dataName;");
					}
				}
			}
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x000E7BC0 File Offset: 0x000E5DC0
		private static Type FindDataTemplateType(string templateName, Assembly assembly = null)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetExecutingAssembly();
			}
			Type type = assembly.GetType(templateName);
			if (type == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					type = assemblies[i].GetType(templateName);
					if (type != null)
					{
						return type;
					}
				}
				if (type == null)
				{
					Debug.LogWarning("Did not find Type for: " + templateName + " in extended assembly search");
				}
			}
			if (type == null)
			{
				Debug.LogWarning("Did not find Type for: " + templateName);
			}
			return type;
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x000E7C54 File Offset: 0x000E5E54
		public static string GenerateDataName(string coreString = "generatedDataTemplate")
		{
			List<string> list = new List<string>(TemplateManager.self.templatesByName.Keys);
			bool flag = true;
			string text = string.Empty;
			while (flag)
			{
				flag = false;
				text = new StringBuilder(coreString).Append(TemplateManager.self.newTemplateValue).ToString();
				if (list.Contains(text))
				{
					flag = true;
					TemplateManager.self.newTemplateValue++;
				}
			}
			return text;
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x000E7CBE File Offset: 0x000E5EBE
		public static void AddSkirmishModeTemplate(TISpaceShipTemplate template)
		{
			TemplateManager.self.skirmishModeTemplates.Add(template);
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x000E7CD0 File Offset: 0x000E5ED0
		public static void ClearSkirmishModeTemplates()
		{
			foreach (TISpaceShipTemplate tispaceShipTemplate in TemplateManager.self.skirmishModeTemplates)
			{
				TemplateManager.self.skirmishModeTemplates.ForEach(delegate(TISpaceShipTemplate x)
				{
					x.factionName = null;
				});
				if (TemplateManager.self.templatesByName.ContainsKey(tispaceShipTemplate.dataName))
				{
					TemplateManager.self.templatesByName[tispaceShipTemplate.dataName].Remove(typeof(TISpaceShipTemplate));
				}
				if (TemplateManager.self.templatesByType.ContainsKey(typeof(TISpaceShipTemplate)))
				{
					TemplateManager.self.templatesByType[typeof(TISpaceShipTemplate)].Remove(tispaceShipTemplate.dataName);
				}
			}
			TemplateManager.self.skirmishModeTemplates.Clear();
		}

		// Token: 0x040020AB RID: 8363
		public static readonly TemplateManager self = new TemplateManager();

		// Token: 0x040020AC RID: 8364
		private readonly IDictionary<Type, Dictionary<string, TIDataTemplate>> templatesByType = new Dictionary<Type, Dictionary<string, TIDataTemplate>>();

		// Token: 0x040020AD RID: 8365
		private readonly IDictionary<string, Dictionary<Type, TIDataTemplate>> templatesByName = new Dictionary<string, Dictionary<Type, TIDataTemplate>>();

		// Token: 0x040020AE RID: 8366
		private readonly IDictionary<Type, Dictionary<string, List<TIDataTemplate>>> duplicateTemplatesByType = new Dictionary<Type, Dictionary<string, List<TIDataTemplate>>>();

		// Token: 0x040020AF RID: 8367
		private List<TISpaceShipTemplate> skirmishModeTemplates = new List<TISpaceShipTemplate>();

		// Token: 0x040020B0 RID: 8368
		private bool foundGlobal;

		// Token: 0x040020B1 RID: 8369
		private bool initialized;

		// Token: 0x040020B2 RID: 8370
		private TIGlobalConfig _global;

		// Token: 0x040020B3 RID: 8371
		private static JsonController jController = new JsonController();

		// Token: 0x040020B4 RID: 8372
		private static List<TIHabModuleTemplate> cachedHabModuleTemplates = null;

		// Token: 0x040020B5 RID: 8373
		private int newTemplateValue;
	}
}
