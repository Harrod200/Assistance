using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Modding
{
	// Token: 0x02000956 RID: 2390
	public class JsonController
	{
		// Token: 0x06005AFC RID: 23292 RVA: 0x002BCC84 File Offset: 0x002BAE84
		public JsonMod LoadJson(string jsonPath)
		{
			string text = File.ReadAllText(jsonPath);
			JsonMod jsonMod = new JsonMod();
			jsonMod.ModFilePath = jsonPath;
			jsonMod.ModFileName = Path.GetFileName(jsonPath);
			this.GetModSettings(jsonPath, jsonMod);
			try
			{
				jsonMod.FileContents = JsonConvert.DeserializeObject<List<JObject>>(text, new JsonConverter[]
				{
					new ExpandoObjectConverter()
				});
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Mod Manager failed to parse Json: " + jsonPath + " " + ex.Message);
				StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
				if (startMenuController != null)
				{
					startMenuController.BankModFailureWarning("UI.StartScreen.Mods.ModWarningHeaderFailLoadJson", "UI.StartScreen.Mods.ModWarningDescriptionFailLoadJson", jsonPath, ex.Message);
				}
				return null;
			}
			return jsonMod;
		}

		// Token: 0x06005AFD RID: 23293 RVA: 0x002BCD30 File Offset: 0x002BAF30
		public JsonMod LoadJsonString(string jString)
		{
			JsonMod jsonMod = new JsonMod();
			try
			{
				jsonMod.FileContents = JsonConvert.DeserializeObject<List<JObject>>(jString, new JsonConverter[]
				{
					new ExpandoObjectConverter()
				});
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Mod Manager failed to parse Json:  " + ex.Message);
				return null;
			}
			return jsonMod;
		}

		// Token: 0x06005AFE RID: 23294 RVA: 0x002BCD90 File Offset: 0x002BAF90
		public void GetModSettings(string modPath, JsonMod mod)
		{
			string text = Path.GetDirectoryName(modPath) + "\\ModInfo.json";
			string text2 = "";
			if (File.Exists(text))
			{
				text2 = File.ReadAllText(text);
			}
			JsonMod jsonMod;
			try
			{
				jsonMod = JsonConvert.DeserializeObject<JsonMod>(text2, new JsonConverter[]
				{
					new ExpandoObjectConverter()
				});
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Mod Manager failed to parse Json: " + modPath + " " + ex.Message);
				StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
				if (startMenuController != null)
				{
					startMenuController.BankModFailureWarning("UI.StartScreen.Mods.ModWarningHeaderFailLoadJson", "UI.StartScreen.Mods.ModWarningDescriptionFailLoadJson", text, ex.Message);
				}
				return;
			}
			if (jsonMod != null)
			{
				mod.TemplatesToConcatArrays = jsonMod.TemplatesToConcatArrays;
				mod.TemplatesToReplaceArrays = jsonMod.TemplatesToReplaceArrays;
				mod.LoadOrder = jsonMod.LoadOrder;
				mod.TemplatesToReplace = jsonMod.TemplatesToReplace;
			}
		}

		// Token: 0x06005AFF RID: 23295 RVA: 0x002BCE68 File Offset: 0x002BB068
		public static bool IsReplaceableModFile(string modPath, string modFile)
		{
			string text = Path.GetDirectoryName(modPath) + "\\ModInfo.json";
			string text2 = "";
			if (File.Exists(text))
			{
				text2 = File.ReadAllText(text);
			}
			JsonMod jsonMod;
			try
			{
				jsonMod = JsonConvert.DeserializeObject<JsonMod>(text2, new JsonConverter[]
				{
					new ExpandoObjectConverter()
				});
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Mod Manager failed to parse Json: " + modPath + " " + ex.Message);
				StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
				if (startMenuController != null)
				{
					startMenuController.BankModFailureWarning("UI.StartScreen.Mods.ModWarningHeaderFailLoadJson", "UI.StartScreen.Mods.ModWarningDescriptionFailLoadJson", text, ex.Message);
				}
				return false;
			}
			return jsonMod != null && jsonMod.TemplatesToReplace != null && jsonMod.TemplatesToReplace.Contains(modFile.Split(new string[] { modPath }, StringSplitOptions.None)[1]);
		}

		// Token: 0x06005B00 RID: 23296 RVA: 0x002BCF40 File Offset: 0x002BB140
		public static int GetModLoadOrder(string modPath)
		{
			string text = Path.GetDirectoryName(modPath) + "\\ModInfo.json";
			string text2 = "";
			if (File.Exists(text))
			{
				text2 = File.ReadAllText(text);
			}
			JsonMod jsonMod;
			try
			{
				jsonMod = JsonConvert.DeserializeObject<JsonMod>(text2, new JsonConverter[]
				{
					new ExpandoObjectConverter()
				});
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Mod Manager failed to parse Json: " + modPath + " " + ex.Message);
				StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
				if (startMenuController != null)
				{
					startMenuController.BankModFailureWarning("UI.StartScreen.Mods.ModWarningHeaderFailLoadJson", "UI.StartScreen.Mods.ModWarningDescriptionFailLoadJson", text, ex.Message);
				}
				return 0;
			}
			if (jsonMod != null)
			{
				return jsonMod.LoadOrder;
			}
			return 0;
		}

		// Token: 0x06005B01 RID: 23297 RVA: 0x002BCFF4 File Offset: 0x002BB1F4
		public bool WriteJson(dynamic jsonContents, string jsonPath)
		{
			if (JsonController.<>o__5.<>p__1 == null)
			{
				JsonController.<>o__5.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(JsonController)));
			}
			Func<CallSite, object, string> target = JsonController.<>o__5.<>p__1.Target;
			CallSite <>p__ = JsonController.<>o__5.<>p__1;
			if (JsonController.<>o__5.<>p__0 == null)
			{
				JsonController.<>o__5.<>p__0 = CallSite<Func<CallSite, Type, object, Formatting, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SerializeObject", null, typeof(JsonController), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			string text = target(<>p__, JsonController.<>o__5.<>p__0.Target(JsonController.<>o__5.<>p__0, typeof(JsonConvert), jsonContents, Formatting.Indented));
			File.WriteAllText(jsonPath, text);
			return true;
		}

		// Token: 0x06005B02 RID: 23298 RVA: 0x002BD0B8 File Offset: 0x002BB2B8
		public string jObjectListToString(List<JObject> jObjectList)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			foreach (JObject jobject in jObjectList)
			{
				stringBuilder.Append(jobject.ToString());
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		// Token: 0x06005B03 RID: 23299 RVA: 0x002BD12C File Offset: 0x002BB32C
		public List<JObject> CombineJson(List<JObject> originalJson, List<JObject> replaceJson, bool dlcFile, MergeArrayHandling mergeArrayMode = MergeArrayHandling.Merge)
		{
			try
			{
				using (List<JObject>.Enumerator enumerator = replaceJson.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JObject replaceJsonContent = enumerator.Current;
						List<JObject> list = (from x in originalJson
							select (x) into x
							where x["dataName"].ToString() == replaceJsonContent["dataName"].ToString()
							select x).ToList<JObject>();
						foreach (JObject jobject in list)
						{
							int num = originalJson.IndexOf(jobject);
							originalJson[num].Merge(replaceJsonContent, new JsonMergeSettings
							{
								MergeArrayHandling = mergeArrayMode,
								PropertyNameComparison = StringComparison.Ordinal,
								MergeNullValueHandling = MergeNullValueHandling.Ignore
							});
						}
						if (list.Count<JObject>() == 0 && !dlcFile)
						{
							originalJson.Add(replaceJsonContent);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				throw;
			}
			return originalJson;
		}
	}
}
