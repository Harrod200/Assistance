using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AssetBundles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class TIPlayerProfileManager : MonoBehaviour
{
	// Token: 0x17000024 RID: 36
	// (get) Token: 0x0600016A RID: 362 RVA: 0x0000B63C File Offset: 0x0000983C
	// (set) Token: 0x0600016B RID: 363 RVA: 0x0000B643 File Offset: 0x00009843
	public static TIInputManager inputManager { get; set; }

	// Token: 0x0600016C RID: 364 RVA: 0x0000B64B File Offset: 0x0000984B
	public static bool canUseMods()
	{
		return !Application.isEditor;
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000B655 File Offset: 0x00009855
	public static float masterVolumeModifier()
	{
		return TIPlayerProfileManager.requestedMasterVolume / 100f;
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000B662 File Offset: 0x00009862
	public static float musicVolumeModifier()
	{
		return TIPlayerProfileManager.requestedMusicVolume / 100f;
	}

	// Token: 0x0600016F RID: 367 RVA: 0x0000B66F File Offset: 0x0000986F
	public static float effectsVolumeModifier()
	{
		return TIPlayerProfileManager.requestedEffectsVolume / 100f * TIPlayerProfileManager.masterVolumeModifier();
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000B682 File Offset: 0x00009882
	public static float ambienceVolumeModifier()
	{
		return TIPlayerProfileManager.requestedAmbienceVolume / 100f * TIPlayerProfileManager.masterVolumeModifier();
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000B695 File Offset: 0x00009895
	public static float voiceVolumeModifier()
	{
		return TIPlayerProfileManager.requestedVoiceVolume / 100f * TIPlayerProfileManager.masterVolumeModifier();
	}

	// Token: 0x06000172 RID: 370 RVA: 0x0000B6A8 File Offset: 0x000098A8
	public static float uiVolumeModifier()
	{
		return TIPlayerProfileManager.requestedUIVolume / 100f * TIPlayerProfileManager.masterVolumeModifier();
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000B6BC File Offset: 0x000098BC
	public static void CheckConfigVersion()
	{
		if (!TIPlayerProfileManager.checkedV && !TIPlayerProfileManager.fatalError)
		{
			foreach (string text in TIPlayerProfileManager.lines)
			{
				if (text.Split(new char[] { ':' })[0].Contains("ConfigVersion"))
				{
					if (int.Parse(text.Split(new char[] { ':' })[1]) < TIPlayerProfileManager.configversion)
					{
						Debug.Log("Outdated Profile, creating new one");
						TIPlayerProfileManager.CreateDefaultConfigFile();
					}
					return;
				}
				Debug.Log("CreateDefaultConfig");
				TIPlayerProfileManager.CreateDefaultConfigFile();
			}
		}
	}

	// Token: 0x06000174 RID: 372 RVA: 0x0000B74C File Offset: 0x0000994C
	public static string savedLanguage()
	{
		if (!TIPlayerProfileManager.fatalError)
		{
			foreach (string text in TIPlayerProfileManager.lines)
			{
				if (text.Split(new char[] { ':' })[0].Contains("Language"))
				{
					string text2 = text.Split(new char[] { ':' })[1];
					if (text2 != null)
					{
						return text2;
					}
				}
			}
		}
		Debug.Log("Setting Default Language");
		return Loc.GetDefaultLanguageKey();
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000B7C0 File Offset: 0x000099C0
	public static void GetKBList()
	{
		TIPlayerProfileManager.KBList.Clear();
		TIPlayerProfileManager.KBModifierList.Clear();
		if (!TIPlayerProfileManager.fatalError)
		{
			foreach (string text in TIPlayerProfileManager.lines)
			{
				if (text.Split(new char[] { ':' })[0].Contains("KB"))
				{
					TIPlayerProfileManager.KBList.Add(text.Split(new char[] { ':' })[1]);
				}
				if (text.Split(new char[] { ':' })[0].Contains("KBModifier"))
				{
					TIPlayerProfileManager.KBModifierList.Add(text.Split(new char[] { ':' })[1]);
				}
			}
		}
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0000B881 File Offset: 0x00009A81
	public static KeyCode savedKeybind(int index)
	{
		if (index < TIPlayerProfileManager.KBList.Count)
		{
			return (KeyCode)Enum.Parse(typeof(KeyCode), TIPlayerProfileManager.KBList[index]);
		}
		return KeyCode.None;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000B8B1 File Offset: 0x00009AB1
	public static KeyCode savedKeybindModifier(int index)
	{
		if (index < TIPlayerProfileManager.KBModifierList.Count)
		{
			return (KeyCode)Enum.Parse(typeof(KeyCode), TIPlayerProfileManager.KBModifierList[index]);
		}
		return KeyCode.None;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000B8E1 File Offset: 0x00009AE1
	public static bool savedEmptyKeybind(int index)
	{
		return index < TIPlayerProfileManager.KBList.Count && (KeyCode)Enum.Parse(typeof(KeyCode), TIPlayerProfileManager.KBList[index]) == KeyCode.None;
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000B914 File Offset: 0x00009B14
	public static int savedQualitySetting()
	{
		if (!TIPlayerProfileManager.fatalError)
		{
			foreach (string text in TIPlayerProfileManager.lines)
			{
				if (text.Split(new char[] { ':' })[0].Contains("QualitySetting"))
				{
					int num = int.Parse(text.Split(new char[] { ':' })[1]);
					Debug.Log("Quality Setting Found: " + int.Parse(text.Split(new char[] { ':' })[1]).ToString());
					return num;
				}
			}
		}
		Debug.Log("No valid quality Setting Found, reverting to default 1");
		return 1;
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000B9B8 File Offset: 0x00009BB8
	public static string GetValue(string searchKey)
	{
		if (!TIPlayerProfileManager.fatalError)
		{
			foreach (string text in TIPlayerProfileManager.lines)
			{
				if (text.Split(new char[] { ':' })[0].Contains(searchKey))
				{
					string text2 = text.Split(new char[] { ':' })[1];
					if (searchKey == "Timestamp")
					{
						text2 = text.Replace("Timestamp:", "");
					}
					return text2;
				}
			}
		}
		return null;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000BA34 File Offset: 0x00009C34
	public static string GetValueJson(string searchKey)
	{
		if (!TIPlayerProfileManager.fatalError)
		{
			foreach (string text in TIPlayerProfileManager.lines)
			{
				if (text.Split(new char[] { ':' })[0].Contains(searchKey))
				{
					return text.Replace(TIUtilities.CombineStrings(new string[] { searchKey, ":" }), "");
				}
			}
		}
		return null;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000BAA0 File Offset: 0x00009CA0
	public static TINotificationTemplateOverride GetNotificationOverride(string dataName)
	{
		string value = TIPlayerProfileManager.GetValue(TIUtilities.CombineStrings(new string[] { "NotificationOverride.", dataName }));
		TINotificationTemplateOverride tinotificationTemplateOverride = new TINotificationTemplateOverride();
		if (value == null)
		{
			return tinotificationTemplateOverride;
		}
		NotificationOverrideBehavior notificationOverrideBehavior;
		Enum.TryParse<NotificationOverrideBehavior>(value.Split(new char[] { ',' })[0], out notificationOverrideBehavior);
		tinotificationTemplateOverride.alert = notificationOverrideBehavior;
		NotificationOverrideBehavior notificationOverrideBehavior2;
		Enum.TryParse<NotificationOverrideBehavior>(value.Split(new char[] { ',' })[1], out notificationOverrideBehavior2);
		tinotificationTemplateOverride.newsFeed = notificationOverrideBehavior2;
		NotificationOverrideBehavior notificationOverrideBehavior3;
		Enum.TryParse<NotificationOverrideBehavior>(value.Split(new char[] { ',' })[2], out notificationOverrideBehavior3);
		tinotificationTemplateOverride.timerFeed = notificationOverrideBehavior3;
		NotificationOverrideBehavior notificationOverrideBehavior4;
		Enum.TryParse<NotificationOverrideBehavior>(value.Split(new char[] { ',' })[3], out notificationOverrideBehavior4);
		tinotificationTemplateOverride.summaryFeed = notificationOverrideBehavior4;
		return tinotificationTemplateOverride;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000BB60 File Offset: 0x00009D60
	public static float GetFloatByKey(string key, float defaultValue)
	{
		float num;
		if (float.TryParse(TIPlayerProfileManager.GetValue(key), out num))
		{
			return num;
		}
		return defaultValue;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000BB80 File Offset: 0x00009D80
	public static int GetIntByKey(string key, int defaultValue)
	{
		int num;
		if (int.TryParse(TIPlayerProfileManager.GetValue(key), out num))
		{
			return num;
		}
		return defaultValue;
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000BBA0 File Offset: 0x00009DA0
	public static bool GetBoolByKey(string key, bool defaultValue)
	{
		bool flag;
		if (bool.TryParse(TIPlayerProfileManager.GetValue(key), out flag))
		{
			return flag;
		}
		return defaultValue;
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000BBC0 File Offset: 0x00009DC0
	public static string GetAltSavePathString(string searchKey, string[] textToSearch)
	{
		foreach (string text in textToSearch)
		{
			if (text.Split(new char[] { ':' })[0].Contains(searchKey))
			{
				string text2 = "\t\t\"savePath\": \"";
				string text3 = text.Substring(text2.Length);
				text3 = text3.Replace("\\\\", "\\");
				text3 = text3.Replace("\\\\", "\\");
				if (text3.Substring(text3.Length - 2).Contains(','))
				{
					text3 = text3.Remove(text3.Length - 1);
				}
				text3 = text3.Remove(text3.Length - 2);
				Debug.Log("AltSavePath = " + text3);
				return text3;
			}
		}
		return null;
	}

	// Token: 0x06000181 RID: 385 RVA: 0x0000BC94 File Offset: 0x00009E94
	public static void GetLastLaunch()
	{
		DateTime dateTime = default(DateTime);
		DateTime dateTime2;
		if (TIPlayerProfileManager.lastLaunch == dateTime && TIPlayerProfileManager.GetValue("Timestamp") != null && DateTime.TryParse(TIPlayerProfileManager.GetValue("Timestamp"), out dateTime2))
		{
			TIPlayerProfileManager.lastLaunch = dateTime2;
			Debug.Log("LastLaunch" + TIPlayerProfileManager.lastLaunch.ToString());
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x0000BCF4 File Offset: 0x00009EF4
	public static void Init()
	{
		Debug.Log("Init Profile");
		TIPlayerProfileManager.CheckAlternateSavePath();
		TIInputManager.InitBindingArray();
		TIPlayerProfileManager.notificationTemplates = new List<TINotificationTemplate>(from o in TemplateManager.GetAllTemplates<TINotificationTemplate>(true)
			where o.allowAnyChanges
			select o);
		TIPlayerProfileManager.notificationTemplates = (from x in TIPlayerProfileManager.notificationTemplates
			orderby x.summaryAudience.category == SummaryCategory.None descending, x.summaryAudience.category.ToString()
			select x).ToList<TINotificationTemplate>();
		TIPlayerProfileManager.LoadPlayerConfig(false);
		TIPlayerProfileManager.CheckModsToUninstall();
		Loc.SetLanguage(TIPlayerProfileManager.savedLanguage());
		TIInputManager.Init();
		Debug.Log(string.Concat(new string[]
		{
			SystemInfo.operatingSystem,
			"\nProcessor: ",
			SystemInfo.processorType,
			", ",
			SystemInfo.processorFrequency.ToString(),
			", ",
			SystemInfo.processorCount.ToString(),
			"\nGPU Vendor: ",
			SystemInfo.graphicsDeviceVendor,
			"\nGPU Name: ",
			SystemInfo.graphicsDeviceName,
			"\nGPU ID: ",
			SystemInfo.graphicsDeviceID.ToString(),
			"\nSystem RAM: ",
			SystemInfo.systemMemorySize.ToString(),
			" MB\nGPU VRAM: ",
			SystemInfo.graphicsMemorySize.ToString(),
			" MB\n",
			RegionInfo.CurrentRegion.ToString(),
			"\n",
			CultureInfo.CurrentCulture.ToString(),
			"\n",
			CultureInfo.CurrentCulture.DisplayName,
			"\n",
			Application.systemLanguage.ToString()
		}));
		try
		{
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				Debug.Log("Drive " + driveInfo.Name + "  Drive type: " + driveInfo.DriveType.ToString());
				if (driveInfo.IsReady)
				{
					Debug.Log("Volume label: " + driveInfo.VolumeLabel + "  File system: " + driveInfo.DriveFormat);
					Debug.Log("Available space to current user: " + (driveInfo.AvailableFreeSpace / 1000000000L).ToString() + " GB");
					Debug.Log("Total available space: " + (driveInfo.TotalFreeSpace / 1000000000L).ToString() + " GB");
					Debug.Log("Total size of drive: " + (driveInfo.TotalSize / 1000000000L).ToString() + " GB");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
		if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBA32))
		{
			Debug.Log("Warning, system does not support RGBA32 cursor texture format");
		}
	}

	// Token: 0x06000183 RID: 387 RVA: 0x0000C010 File Offset: 0x0000A210
	public static void CheckAlternateSavePath()
	{
		if (TIPlayerProfileManager.checkedAltSavePath || SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
		{
			return;
		}
		StreamReader streamReader = new StreamReader(Application.streamingAssetsPath + "/Templates/TIGlobalConfig.json", true);
		string[] array = streamReader.ReadToEnd().Split(new char[] { '\n' });
		streamReader.Close();
		string altSavePathString = TIPlayerProfileManager.GetAltSavePathString("savePath", array);
		if (!string.IsNullOrEmpty(altSavePathString))
		{
			TIPlayerProfileManager.useAlternateSavePath = true;
			TIPlayerProfileManager.alternateSavePath = altSavePathString;
		}
		TIPlayerProfileManager.checkedAltSavePath = true;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000C088 File Offset: 0x0000A288
	public static void LoadPlayerConfig(bool modCheck = false)
	{
		if (modCheck)
		{
			TIPlayerProfileManager.CheckAlternateSavePath();
		}
		TIPlayerProfileManager.loadpath = CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile";
		if (File.Exists(TIPlayerProfileManager.loadpath))
		{
			Debug.Log("Found Profile Config File");
			TIPlayerProfileManager.ReadPlayerConfig(modCheck);
			return;
		}
		if (!modCheck)
		{
			TIPlayerProfileManager.CreateDefaultConfigFile();
		}
	}

	// Token: 0x06000185 RID: 389 RVA: 0x0000C0D8 File Offset: 0x0000A2D8
	public static void ReadPlayerConfig(bool modCheck = false)
	{
		try
		{
			TIPlayerProfileManager.lines = null;
			StreamReader streamReader = new StreamReader(TIPlayerProfileManager.loadpath, true);
			TIPlayerProfileManager.lines = streamReader.ReadToEnd().Split(new char[] { '\n' });
			streamReader.Close();
			if (!modCheck)
			{
				if (!TIPlayerProfileManager.checkedV)
				{
					TIPlayerProfileManager.CheckConfigVersion();
				}
				TIPlayerProfileManager.LoadDefaultGraphics();
				TIPlayerProfileManager.LoadDefaultAudio();
				TIPlayerProfileManager.LoadDefaultGameplaySettings();
				TIPlayerProfileManager.LoadNotificationOverrides();
				TIPlayerProfileManager.LoadPreviousCampaignLaunchOptions();
				TIPlayerProfileManager.LoadPreviouslySubscribedWorkshopMods();
				TIPlayerProfileManager.GetLastLaunch();
			}
			TIPlayerProfileManager.LoadModSettings();
			Debug.Log("Use Mods:" + TIPlayerProfileManager.useMods.ToString());
			Debug.Log("Previous launch failed due to bad mod templates: " + TIPlayerProfileManager.loadingFailureDueToMods.ToString());
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			Debug.LogError(ex.StackTrace);
			StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
			if (startMenuController != null)
			{
				TIPlayerProfileManager.fatalError = true;
				startMenuController.fatalStartupError = true;
				startMenuController.BankModFailureWarning("UI.StartScreen.StartupErrorHeader", "UI.StartScreen.StartupErrorSavePath", CreateSaveFileScrollList.GetSaveFolderPath(), "");
			}
		}
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000C1E0 File Offset: 0x0000A3E0
	public static void LoadModSettings()
	{
		if (TIPlayerProfileManager.GetValue("LoadingFailureDueToMods") == "True")
		{
			TIPlayerProfileManager.loadingFailureDueToMods = true;
		}
		if (TIPlayerProfileManager.GetValue("UseMods") == "True" && !TIPlayerProfileManager.loadingFailureDueToMods)
		{
			TIPlayerProfileManager.useMods = true;
		}
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000C22C File Offset: 0x0000A42C
	public static void LoadDefaultGameplaySettings()
	{
		if (TIPlayerProfileManager.GetValue("WaypointSnapAngleIndex") != null)
		{
			TIPlayerProfileManager.waypointAngleSnapIndex = int.Parse(TIPlayerProfileManager.GetValue("WaypointSnapAngleIndex"));
		}
		if (TIPlayerProfileManager.GetValue("MaxShipsInCombat") != null)
		{
			TIPlayerProfileManager.maxShipsInCombat = int.Parse(TIPlayerProfileManager.GetValue("MaxShipsInCombat"));
		}
		float num;
		if (TIPlayerProfileManager.GetValue("TooltipDelayPrimary") != null && float.TryParse(TIPlayerProfileManager.GetValue("TooltipDelayPrimary"), out num))
		{
			TIPlayerProfileManager.tooltipDelayPrimary = num;
		}
		float num2;
		if (TIPlayerProfileManager.GetValue("TooltipDelaySupplemental") != null && float.TryParse(TIPlayerProfileManager.GetValue("TooltipDelaySupplemental"), out num2))
		{
			TIPlayerProfileManager.tooltipDelaySupplemental = num2;
		}
		if (TIPlayerProfileManager.GetValue("MissionPhaseReportStartOpen") == "True")
		{
			TIPlayerProfileManager.missionPhaseReportStartOpen = true;
		}
		if (TIPlayerProfileManager.GetValue("UnpauseAfterMissionAssignment") == "True")
		{
			TIPlayerProfileManager.unpauseAfterMissionAssignment = true;
		}
		if (TIPlayerProfileManager.GetValue("FirstGame") == "False")
		{
			TIPlayerProfileManager.firstGame = false;
		}
		if (TIPlayerProfileManager.GetValue("AlertSpaceTimerNotifications") == "True")
		{
			TIPlayerProfileManager.alertSpaceTimerNotifications = true;
		}
		if (TIPlayerProfileManager.GetValue("ShowMonthlyIncomes") == "True")
		{
			TIPlayerProfileManager.showMonthlyIncomes = true;
		}
		if (TIPlayerProfileManager.GetValue("MuteInBackground") == "False")
		{
			TIPlayerProfileManager.muteInBackground = false;
		}
		if (TIPlayerProfileManager.GetValue("CompressSaves") == "False")
		{
			TIPlayerProfileManager.compressSaves = false;
		}
		if (TIPlayerProfileManager.GetValue("DisplaySystemClock") == "True")
		{
			TIPlayerProfileManager.displaySystemClock = true;
		}
		if (TIPlayerProfileManager.GetValue("AssignmentPhaseCouncilorCameraFocus") == "False")
		{
			TIPlayerProfileManager.assignmentPhaseCouncilorCameraFocus = false;
		}
		if (TIPlayerProfileManager.GetValue("CycleNextCouncilorWhenAssigningMissions") == "False")
		{
			TIPlayerProfileManager.cycleNextCouncilorWhenAssigningMissions = false;
		}
		if (TIPlayerProfileManager.GetValue("ShowHighSpeedOrbitTrails") == "False")
		{
			TIPlayerProfileManager.showHighSpeedOrbitTrails = false;
		}
		if (TIPlayerProfileManager.GetValue("ShowEarthLights") == "False")
		{
			TIPlayerProfileManager.showEarthLights = false;
		}
		if (TIPlayerProfileManager.GetValue("TextureStreaming") == "False")
		{
			TIPlayerProfileManager.useTextureStreaming = false;
		}
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000C430 File Offset: 0x0000A630
	public static void LoadDefaultGraphics()
	{
		if (TIPlayerProfileManager.GetValue("UseWindowsCursor") == "True")
		{
			TIPlayerProfileManager.usingWindowsCursor = true;
		}
		int num = int.Parse(TIPlayerProfileManager.GetValue("QualitySetting"));
		int.TryParse(TIPlayerProfileManager.GetValue("AntiAliasingMode"), out TIPlayerProfileManager.antiAliasingMode);
		if (num == 99)
		{
			TIPlayerProfileManager.isCustomQuality = true;
		}
		if (!TIPlayerProfileManager.isCustomQuality)
		{
			QualitySettings.SetQualityLevel(num);
		}
		if (TIPlayerProfileManager.isCustomQuality)
		{
			int num2 = int.Parse(TIPlayerProfileManager.GetValue("TextureQuality"));
			int num3 = int.Parse(TIPlayerProfileManager.GetValue("AntiAliasing"));
			QualitySettings.masterTextureLimit = num2;
			QualitySettings.antiAliasing = num3;
		}
		Debug.Log("AA Mode: " + TIPlayerProfileManager.antiAliasingMode.ToString());
		Debug.Log("AA Level: " + int.Parse(TIPlayerProfileManager.GetValue("AntiAliasing")).ToString());
		if (TIPlayerProfileManager.antiAliasingMode != 0)
		{
			QualitySettings.antiAliasing = 0;
		}
		if (TIPlayerProfileManager.GetValue("TextureStreaming") == "True")
		{
			TIPlayerProfileManager.useTextureStreaming = true;
		}
		if (TIPlayerProfileManager.GetValue("EnableAccessibilityMagnifier") == "True")
		{
			TIPlayerProfileManager.enableAccessibilityMagnifier = true;
		}
		else
		{
			TIPlayerProfileManager.enableAccessibilityMagnifier = false;
		}
		TIPlayerProfileManager.SetMipmapMemoryBudget();
		bool flag = true;
		TIPlayerProfileManager.storedResolution.x = (float)int.Parse(TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[0]);
		TIPlayerProfileManager.storedResolution.y = (float)int.Parse(TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[1]);
		if (TIPlayerProfileManager.GetValue("Fullscreen") != null)
		{
			if (TIPlayerProfileManager.GetValue("Fullscreen") == "True")
			{
				flag = true;
			}
			if (TIPlayerProfileManager.GetValue("Fullscreen") == "False")
			{
				flag = false;
			}
			Debug.Log("FullscreenMode:" + flag.ToString());
		}
		TIPlayerProfileManager.storedFullscreenMode = flag;
		if (TIPlayerProfileManager.GetValue("Resolution") != null && TIPlayerProfileManager.GetValue("RefreshRate") == null)
		{
			Debug.Log("Set Resolution: " + TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[0] + "x" + TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[1]);
			Screen.SetResolution(int.Parse(TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[0]), int.Parse(TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[1]), flag);
		}
		if (TIPlayerProfileManager.GetValue("Resolution") != null && TIPlayerProfileManager.GetValue("RefreshRate") != null)
		{
			TIPlayerProfileManager.storedRefreshRate = int.Parse(TIPlayerProfileManager.GetValue("RefreshRate"));
			Debug.Log("Set Resolution: " + TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[0] + "x" + TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[1]);
			Screen.SetResolution(int.Parse(TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[0]), int.Parse(TIPlayerProfileManager.GetValue("Resolution").Split(new char[] { 'x' })[1]), flag, TIPlayerProfileManager.storedRefreshRate);
		}
		int.TryParse(TIPlayerProfileManager.GetValue("UIScaleSetting"), out TIPlayerProfileManager.uiScaleSetting);
		Debug.Log("UI Scale Setting: " + TIPlayerProfileManager.uiScaleSetting.ToString());
		if (TIPlayerProfileManager.GetValue("ConfineCursor") == "True")
		{
			TIPlayerProfileManager.SetCursorConfineMode(true);
		}
		if (TIPlayerProfileManager.GetValue("VSyncEnabled") == "True")
		{
			TIPlayerProfileManager.vsyncEnabled = true;
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			TIPlayerProfileManager.vsyncEnabled = false;
			QualitySettings.vSyncCount = 0;
		}
		int num4;
		if (int.TryParse(TIPlayerProfileManager.GetValue("SkyboxVariant"), out num4))
		{
			TIPlayerProfileManager.skyboxVariant = num4;
		}
		if (TIPlayerProfileManager.GetValue("UseCouncilorVideo") == "False")
		{
			TIPlayerProfileManager.useCouncilorVideo = false;
		}
		RenderSettings.skybox = AssetBundleManager.LoadAsset<Material>(TemplateManager.global.skyboxes[TIPlayerProfileManager.skyboxVariant]);
		TIPlayerProfileManager.SetCouncilorVideoSetting(false);
		Debug.Log("Use Councilor and Leader Videos: " + TIPlayerProfileManager.useCouncilorVideo.ToString());
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000C874 File Offset: 0x0000AA74
	public static void LoadNotificationOverrides()
	{
		TIPlayerProfileManager.notificationOverrides.Clear();
		foreach (TINotificationTemplate tinotificationTemplate in TIPlayerProfileManager.notificationTemplates)
		{
			TIPlayerProfileManager.notificationOverrides.Add(TIPlayerProfileManager.GetNotificationOverride(tinotificationTemplate.dataName));
		}
	}

	// Token: 0x0600018A RID: 394 RVA: 0x0000C8E0 File Offset: 0x0000AAE0
	public static void LoadPreviousCampaignLaunchOptions()
	{
		string valueJson = TIPlayerProfileManager.GetValueJson("PreviousCampaignLaunchOptions");
		if (!string.IsNullOrEmpty(valueJson))
		{
			TIPlayerProfileManager.storedCampaignOptions = JsonConvert.DeserializeObject<TIPlayerProfileManager.StoredCampaignOptions>(valueJson, new JsonConverter[]
			{
				new ExpandoObjectConverter()
			});
		}
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000C91C File Offset: 0x0000AB1C
	public static void LoadPreviouslySubscribedWorkshopMods()
	{
		string valueJson = TIPlayerProfileManager.GetValueJson("SubscribedWorkshopMods");
		if (!string.IsNullOrEmpty(valueJson))
		{
			TIPlayerProfileManager.subscribedMods = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueJson, new JsonConverter[]
			{
				new ExpandoObjectConverter()
			});
		}
	}

	// Token: 0x0600018C RID: 396 RVA: 0x0000C958 File Offset: 0x0000AB58
	public static void CheckModsToUninstall()
	{
		string valueJson = TIPlayerProfileManager.GetValueJson("ModsToUninstall");
		if (!string.IsNullOrEmpty(valueJson))
		{
			TIPlayerProfileManager.modsToUninstall = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueJson, new JsonConverter[]
			{
				new ExpandoObjectConverter()
			});
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> keyValuePair in TIPlayerProfileManager.modsToUninstall)
		{
			if (keyValuePair.Value.Contains("Mods/Disabled/") && Directory.Exists(keyValuePair.Value))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(keyValuePair.Value);
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
						Directory.Delete(keyValuePair.Value, true);
						Debug.Log("finished uninstalling mod" + keyValuePair.Value);
					}
					catch (Exception)
					{
						Debug.LogError("Fail to delete mod: " + keyValuePair.Value);
					}
				}
			}
			list.Add(keyValuePair.Key);
		}
		foreach (string text in list)
		{
			TIPlayerProfileManager.modsToUninstall.Remove(text);
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000CB1C File Offset: 0x0000AD1C
	public static string GetPreviousCampaignLaunchOptionsString()
	{
		return JsonConvert.SerializeObject(TIPlayerProfileManager.storedCampaignOptions);
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000CB28 File Offset: 0x0000AD28
	public static string GetSubscribedModsString()
	{
		return JsonConvert.SerializeObject(TIPlayerProfileManager.subscribedMods);
	}

	// Token: 0x0600018F RID: 399 RVA: 0x0000CB34 File Offset: 0x0000AD34
	public static string GetModsToUninstallString()
	{
		return JsonConvert.SerializeObject(TIPlayerProfileManager.modsToUninstall);
	}

	// Token: 0x06000190 RID: 400 RVA: 0x0000CB40 File Offset: 0x0000AD40
	public static void ClearSubscribedMods()
	{
		TIPlayerProfileManager.subscribedMods.Clear();
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000CB4C File Offset: 0x0000AD4C
	public static void SetMipmapMemoryBudget()
	{
		if (!TIPlayerProfileManager.useTextureStreaming)
		{
			QualitySettings.streamingMipmapsActive = false;
			return;
		}
		QualitySettings.streamingMipmapsMemoryBudget = Mathf.Clamp((float)SystemInfo.graphicsMemorySize * 0.75f, 1024f, (float)SystemInfo.graphicsMemorySize * 0.75f);
		Debug.Log("TextureStreaming: " + TIPlayerProfileManager.useTextureStreaming.ToString() + ", " + QualitySettings.streamingMipmapsMemoryBudget.ToString());
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000CBB9 File Offset: 0x0000ADB9
	public static void SetCursorConfineMode(bool confine)
	{
		if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer) && confine)
		{
			Cursor.lockState = CursorLockMode.Confined;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
		TIPlayerProfileManager.confineCursor = confine;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000CBE6 File Offset: 0x0000ADE6
	public static void VerifyModDirectories()
	{
		if (!Directory.Exists("Mods/Enabled"))
		{
			Directory.CreateDirectory("Mods/Enabled");
		}
		if (!Directory.Exists("Mods/Disabled"))
		{
			Directory.CreateDirectory("Mods/Disabled");
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000CC16 File Offset: 0x0000AE16
	public static void SetCouncilorVideoSetting(bool newConfig = true)
	{
		if (newConfig && (TIUtilities.IsSteamDeck() || TIUtilities.IsLinux()))
		{
			TIPlayerProfileManager.useCouncilorVideo = false;
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000CC30 File Offset: 0x0000AE30
	public static void LoadDefaultAudio()
	{
		if (TIPlayerProfileManager.GetValue("VolumeMaster") != null)
		{
			TIPlayerProfileManager.requestedMasterVolume = float.Parse(TIPlayerProfileManager.GetValue("VolumeMaster"));
		}
		if (TIPlayerProfileManager.GetValue("VolumeMusic") != null)
		{
			TIPlayerProfileManager.requestedMusicVolume = float.Parse(TIPlayerProfileManager.GetValue("VolumeMusic"));
		}
		if (TIPlayerProfileManager.GetValue("VolumeUI") != null)
		{
			TIPlayerProfileManager.requestedUIVolume = float.Parse(TIPlayerProfileManager.GetValue("VolumeUI"));
		}
		if (TIPlayerProfileManager.GetValue("VolumeEffects") != null)
		{
			TIPlayerProfileManager.requestedEffectsVolume = float.Parse(TIPlayerProfileManager.GetValue("VolumeEffects"));
		}
		if (TIPlayerProfileManager.GetValue("VolumeVoice") != null)
		{
			TIPlayerProfileManager.requestedVoiceVolume = float.Parse(TIPlayerProfileManager.GetValue("VolumeVoice"));
		}
		if (TIPlayerProfileManager.GetValue("VolumeAmbience") != null)
		{
			TIPlayerProfileManager.requestedAmbienceVolume = float.Parse(TIPlayerProfileManager.GetValue("VolumeAmbience"));
		}
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000CD00 File Offset: 0x0000AF00
	public static void CreateDefaultConfigFile()
	{
		Debug.Log("Creating Default Config");
		TIPlayerProfileManager.checkedV = true;
		if (File.Exists(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile"))
		{
			File.Delete(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile");
		}
		else
		{
			Directory.CreateDirectory(CreateSaveFileScrollList.GetSaveFolderPath());
		}
		StreamWriter streamWriter = new StreamWriter(TIPlayerProfileManager.loadpath, true);
		streamWriter.Write("ConfigVersion:" + TIPlayerProfileManager.configversion.ToString());
		streamWriter.Write("\nLanguage:" + Loc.GetDefaultLanguageKey());
		TIPlayerProfileManager.SetCouncilorVideoSetting(true);
		int graphicsMemorySize = SystemInfo.graphicsMemorySize;
		if (graphicsMemorySize <= 3000)
		{
			QualitySettings.SetQualityLevel(2);
		}
		else if (graphicsMemorySize < 5000 || (TIUtilities.IsSteamDeck() && SystemInfo.graphicsMemorySize < 8000))
		{
			QualitySettings.SetQualityLevel(1);
		}
		TIPlayerProfileManager.SetMipmapMemoryBudget();
		if (TIUtilities.HasRadeonGPU())
		{
			TIPlayerProfileManager.antiAliasingMode = 1;
			QualitySettings.antiAliasing = 0;
		}
		if (!TIPlayerProfileManager.isCustomQuality)
		{
			streamWriter.Write("\nQualitySetting:" + QualitySettings.GetQualityLevel().ToString());
		}
		if (TIPlayerProfileManager.isCustomQuality)
		{
			streamWriter.Write("\nQualitySetting:" + 99.ToString());
		}
		streamWriter.Write("\nTextureQuality:" + QualitySettings.masterTextureLimit.ToString());
		streamWriter.Write("\nAntiAliasing:" + QualitySettings.antiAliasing.ToString());
		streamWriter.Write("\nAntiAliasingMode:" + TIPlayerProfileManager.antiAliasingMode.ToString());
		streamWriter.Write("\nResolution:" + Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString());
		streamWriter.Write("\nRefreshRate:" + Screen.currentResolution.refreshRate.ToString());
		streamWriter.Write("\nFullscreen:" + Screen.fullScreen.ToString());
		streamWriter.Write("\nVolumeMaster:" + TIPlayerProfileManager.requestedMasterVolume.ToString());
		streamWriter.Write("\nVolumeMusic:" + TIPlayerProfileManager.requestedMusicVolume.ToString());
		streamWriter.Write("\nVolumeUI:" + TIPlayerProfileManager.requestedUIVolume.ToString());
		streamWriter.Write("\nVolumeEffects:" + TIPlayerProfileManager.requestedEffectsVolume.ToString());
		streamWriter.Write("\nVolumeVoice:" + TIPlayerProfileManager.requestedVoiceVolume.ToString());
		streamWriter.Write("\nVolumeAmbience:" + TIPlayerProfileManager.requestedAmbienceVolume.ToString());
		streamWriter.Write("\nWaypointSnapAngleIndex:" + TIPlayerProfileManager.waypointAngleSnapIndex.ToString());
		streamWriter.Write("\nUseWindowsCursor:" + TIPlayerProfileManager.usingWindowsCursor.ToString());
		streamWriter.Write("\nMissionPhaseReportStartOpen:" + TIPlayerProfileManager.missionPhaseReportStartOpen.ToString());
		streamWriter.Write("\nConfineCursor:" + TIPlayerProfileManager.confineCursor.ToString());
		streamWriter.Write("\nVSyncEnabled:" + TIPlayerProfileManager.vsyncEnabled.ToString());
		streamWriter.Write("\nUnpauseAfterMissionAssignment:" + TIPlayerProfileManager.unpauseAfterMissionAssignment.ToString());
		streamWriter.Write("\nMaxShipsInCombat:" + TIPlayerProfileManager.maxShipsInCombat.ToString());
		streamWriter.Write("\nUseMods:" + TIPlayerProfileManager.useMods.ToString());
		streamWriter.Write("\nFirstGame:" + TIPlayerProfileManager.firstGame.ToString());
		streamWriter.Write("\nAlertSpaceTimerNotifications:" + TIPlayerProfileManager.alertSpaceTimerNotifications.ToString());
		streamWriter.Write("\nShowMonthlyIncomes:" + TIPlayerProfileManager.showMonthlyIncomes.ToString());
		streamWriter.Write("\nMuteInBackground:" + TIPlayerProfileManager.muteInBackground.ToString());
		streamWriter.Write("\nCompressSaves:" + TIPlayerProfileManager.compressSaves.ToString());
		streamWriter.Write("\nDisplaySystemClock:" + TIPlayerProfileManager.displaySystemClock.ToString());
		streamWriter.Write("\nTextureStreaming:" + TIPlayerProfileManager.useTextureStreaming.ToString());
		streamWriter.Write("\nAssignmentPhaseCouncilorCameraFocus:" + TIPlayerProfileManager.assignmentPhaseCouncilorCameraFocus.ToString());
		streamWriter.Write("\nCycleNextCouncilorWhenAssigningMissions:" + TIPlayerProfileManager.cycleNextCouncilorWhenAssigningMissions.ToString());
		streamWriter.Write("\nShowHighSpeedOrbitTrails:" + TIPlayerProfileManager.showHighSpeedOrbitTrails.ToString());
		streamWriter.Write("\nShowEarthLights:" + TIPlayerProfileManager.showEarthLights.ToString());
		streamWriter.Write("\nUIScaleSetting:" + TIPlayerProfileManager.uiScaleSetting.ToString());
		streamWriter.Write("\nEnableAccessibilityMagnifier:" + TIPlayerProfileManager.enableAccessibilityMagnifier.ToString());
		streamWriter.Write("\nUseCouncilorVideo:" + TIPlayerProfileManager.useCouncilorVideo.ToString());
		streamWriter.Write("\nSkyboxVariant:" + TIPlayerProfileManager.skyboxVariant.ToString());
		streamWriter.Write("\nTooltipDelayPrimary:" + TIPlayerProfileManager.tooltipDelayPrimary.ToString());
		streamWriter.Write("\nTooltipDelaySupplemental:" + TIPlayerProfileManager.tooltipDelaySupplemental.ToString());
		streamWriter.Write("\nTimestamp:" + DateTime.UtcNow.ToString());
		for (int i = 0; i < TIInputManager.keyBindings.Count; i++)
		{
			streamWriter.Write("\nKB" + i.ToString() + ":" + TIInputManager.keyBindings[i].ToString());
		}
		for (int j = 0; j < TIInputManager.keyBindingModifiers.Count; j++)
		{
			streamWriter.Write("\nKBModifier" + j.ToString() + ":" + TIInputManager.keyBindingModifiers[j].ToString());
		}
		for (int k = 0; k < TIPlayerProfileManager.notificationTemplates.Count; k++)
		{
			streamWriter.Write("\n" + TIUtilities.CombineStrings(new string[]
			{
				"NotificationOverride.",
				TIPlayerProfileManager.notificationTemplates[k].dataName,
				":",
				"0,0,0,0"
			}));
		}
		streamWriter.Write("\nPreviousCampaignLaunchOptions:" + TIPlayerProfileManager.GetPreviousCampaignLaunchOptionsString());
		streamWriter.Close();
		TIPlayerProfileManager.ReadPlayerConfig(false);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000D368 File Offset: 0x0000B568
	public static void SavePlayerConfig()
	{
		try
		{
			if (File.Exists(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile"))
			{
				File.Delete(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile");
			}
			if (!TIPlayerProfileManager.storedFullscreenMode)
			{
				TIPlayerProfileManager.storedResolution = new Vector2((float)Screen.width, (float)Screen.height);
			}
			StreamWriter streamWriter = new StreamWriter(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile", true);
			streamWriter.Write("ConfigVersion:" + TIPlayerProfileManager.configversion.ToString());
			streamWriter.Write("\nLanguage:" + Loc.CurrentLanguage);
			if (!TIPlayerProfileManager.isCustomQuality)
			{
				streamWriter.Write("\nQualitySetting:" + QualitySettings.GetQualityLevel().ToString());
			}
			if (TIPlayerProfileManager.isCustomQuality)
			{
				streamWriter.Write("\nQualitySetting:" + 99.ToString());
			}
			streamWriter.Write("\nTextureQuality:" + QualitySettings.masterTextureLimit.ToString());
			streamWriter.Write("\nAntiAliasing:" + QualitySettings.antiAliasing.ToString());
			streamWriter.Write("\nAntiAliasingMode:" + TIPlayerProfileManager.antiAliasingMode.ToString());
			streamWriter.Write("\nResolution:" + TIPlayerProfileManager.storedResolution.x.ToString() + "x" + TIPlayerProfileManager.storedResolution.y.ToString());
			streamWriter.Write("\nRefreshRate:" + TIPlayerProfileManager.storedRefreshRate.ToString());
			streamWriter.Write("\nFullscreen:" + TIPlayerProfileManager.storedFullscreenMode.ToString());
			streamWriter.Write("\nVolumeMaster:" + TIPlayerProfileManager.requestedMasterVolume.ToString());
			streamWriter.Write("\nVolumeMusic:" + TIPlayerProfileManager.requestedMusicVolume.ToString());
			streamWriter.Write("\nVolumeUI:" + TIPlayerProfileManager.requestedUIVolume.ToString());
			streamWriter.Write("\nVolumeEffects:" + TIPlayerProfileManager.requestedEffectsVolume.ToString());
			streamWriter.Write("\nVolumeVoice:" + TIPlayerProfileManager.requestedVoiceVolume.ToString());
			streamWriter.Write("\nVolumeAmbience:" + TIPlayerProfileManager.requestedAmbienceVolume.ToString());
			streamWriter.Write("\nWaypointSnapAngleIndex:" + TIPlayerProfileManager.waypointAngleSnapIndex.ToString());
			streamWriter.Write("\nUseWindowsCursor:" + TIPlayerProfileManager.usingWindowsCursor.ToString());
			streamWriter.Write("\nMissionPhaseReportStartOpen:" + TIPlayerProfileManager.missionPhaseReportStartOpen.ToString());
			streamWriter.Write("\nConfineCursor:" + TIPlayerProfileManager.confineCursor.ToString());
			streamWriter.Write("\nVSyncEnabled:" + TIPlayerProfileManager.vsyncEnabled.ToString());
			streamWriter.Write("\nUnpauseAfterMissionAssignment:" + TIPlayerProfileManager.unpauseAfterMissionAssignment.ToString());
			streamWriter.Write("\nMaxShipsInCombat:" + TIPlayerProfileManager.maxShipsInCombat.ToString());
			streamWriter.Write("\nUseMods:" + TIPlayerProfileManager.useMods.ToString());
			streamWriter.Write("\nFirstGame:" + TIPlayerProfileManager.firstGame.ToString());
			streamWriter.Write("\nAlertSpaceTimerNotifications:" + TIPlayerProfileManager.alertSpaceTimerNotifications.ToString());
			streamWriter.Write("\nShowMonthlyIncomes:" + TIPlayerProfileManager.showMonthlyIncomes.ToString());
			streamWriter.Write("\nMuteInBackground:" + TIPlayerProfileManager.muteInBackground.ToString());
			streamWriter.Write("\nCompressSaves:" + TIPlayerProfileManager.compressSaves.ToString());
			streamWriter.Write("\nTextureStreaming:" + TIPlayerProfileManager.useTextureStreaming.ToString());
			streamWriter.Write("\nDisplaySystemClock:" + TIPlayerProfileManager.displaySystemClock.ToString());
			streamWriter.Write("\nAssignmentPhaseCouncilorCameraFocus:" + TIPlayerProfileManager.assignmentPhaseCouncilorCameraFocus.ToString());
			streamWriter.Write("\nCycleNextCouncilorWhenAssigningMissions:" + TIPlayerProfileManager.cycleNextCouncilorWhenAssigningMissions.ToString());
			streamWriter.Write("\nShowHighSpeedOrbitTrails:" + TIPlayerProfileManager.showHighSpeedOrbitTrails.ToString());
			streamWriter.Write("\nShowEarthLights:" + TIPlayerProfileManager.showEarthLights.ToString());
			streamWriter.Write("\nUIScaleSetting:" + TIPlayerProfileManager.uiScaleSetting.ToString());
			streamWriter.Write("\nEnableAccessibilityMagnifier:" + TIPlayerProfileManager.enableAccessibilityMagnifier.ToString());
			streamWriter.Write("\nUseCouncilorVideo:" + TIPlayerProfileManager.useCouncilorVideo.ToString());
			streamWriter.Write("\nSkyboxVariant:" + TIPlayerProfileManager.skyboxVariant.ToString());
			streamWriter.Write("\nTooltipDelayPrimary:" + TIPlayerProfileManager.tooltipDelayPrimary.ToString());
			streamWriter.Write("\nTooltipDelaySupplemental:" + TIPlayerProfileManager.tooltipDelaySupplemental.ToString());
			streamWriter.Write("\nTimestamp:" + DateTime.UtcNow.ToString());
			for (int i = 0; i < TIInputManager.keyBindings.Count; i++)
			{
				streamWriter.Write("\nKB" + i.ToString() + ":" + TIInputManager.keyBindings[i].ToString());
			}
			for (int j = 0; j < TIInputManager.keyBindingModifiers.Count; j++)
			{
				streamWriter.Write("\nKBModifier" + j.ToString() + ":" + TIInputManager.keyBindingModifiers[j].ToString());
			}
			for (int k = 0; k < TIPlayerProfileManager.notificationOverrides.Count; k++)
			{
				TextWriter textWriter = streamWriter;
				string text = "\n";
				string[] array = new string[10];
				array[0] = "NotificationOverride.";
				array[1] = TIPlayerProfileManager.notificationTemplates[k].dataName;
				array[2] = ":";
				int num = 3;
				int num2 = (int)TIPlayerProfileManager.notificationOverrides[k].alert;
				array[num] = num2.ToString();
				array[4] = ",";
				int num3 = 5;
				num2 = (int)TIPlayerProfileManager.notificationOverrides[k].newsFeed;
				array[num3] = num2.ToString();
				array[6] = ",";
				int num4 = 7;
				num2 = (int)TIPlayerProfileManager.notificationOverrides[k].timerFeed;
				array[num4] = num2.ToString();
				array[8] = ",";
				int num5 = 9;
				num2 = (int)TIPlayerProfileManager.notificationOverrides[k].summaryFeed;
				array[num5] = num2.ToString();
				textWriter.Write(text + TIUtilities.CombineStrings(array));
			}
			streamWriter.Write("\nSubscribedWorkshopMods:" + TIPlayerProfileManager.GetSubscribedModsString());
			streamWriter.Write("\nModsToUninstall:" + TIPlayerProfileManager.GetModsToUninstallString());
			streamWriter.Write("\nPreviousCampaignLaunchOptions:" + TIPlayerProfileManager.GetPreviousCampaignLaunchOptionsString());
			streamWriter.Close();
			TIPlayerProfileManager.ReadPlayerConfig(false);
			Debug.Log("Player Config Saved");
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			Debug.LogError(ex.StackTrace);
			StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
			if (startMenuController != null)
			{
				TIPlayerProfileManager.fatalError = true;
				startMenuController.fatalStartupError = true;
				startMenuController.BankModFailureWarning("UI.StartScreen.StartupErrorHeader", "UI.StartScreen.StartupErrorSavePath", CreateSaveFileScrollList.GetSaveFolderPath(), "");
			}
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000DA90 File Offset: 0x0000BC90
	public static void HandleModFailure()
	{
		if (File.Exists(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile"))
		{
			StreamWriter streamWriter = new StreamWriter(CreateSaveFileScrollList.GetSaveFolderPath() + "PlayerOptions.TIProfile", true);
			streamWriter.Write("\nLoadingFailureDueToMods:True");
			streamWriter.Close();
		}
	}

	// Token: 0x040001A7 RID: 423
	public static string[] lines;

	// Token: 0x040001A8 RID: 424
	public static string[] cloudConfigLines;

	// Token: 0x040001A9 RID: 425
	public static string loadpath;

	// Token: 0x040001AA RID: 426
	private static readonly int configversion = 2;

	// Token: 0x040001AB RID: 427
	private static bool checkedV = false;

	// Token: 0x040001AC RID: 428
	public static bool isCustomQuality = false;

	// Token: 0x040001AD RID: 429
	public static bool useAlternateSavePath = false;

	// Token: 0x040001AE RID: 430
	public static string alternateSavePath;

	// Token: 0x040001AF RID: 431
	public static bool checkedAltSavePath = false;

	// Token: 0x040001B0 RID: 432
	public static DateTime lastLaunch;

	// Token: 0x040001B1 RID: 433
	public static DateTime lastCloudLaunch;

	// Token: 0x040001B2 RID: 434
	[Range(0f, 100f)]
	public static float requestedMasterVolume = 100f;

	// Token: 0x040001B3 RID: 435
	[Range(0f, 100f)]
	public static float requestedMusicVolume = 50f;

	// Token: 0x040001B4 RID: 436
	[Range(0f, 100f)]
	public static float requestedUIVolume = 75f;

	// Token: 0x040001B5 RID: 437
	[Range(0f, 100f)]
	public static float requestedEffectsVolume = 50f;

	// Token: 0x040001B6 RID: 438
	[Range(0f, 100f)]
	public static float requestedVoiceVolume = 100f;

	// Token: 0x040001B7 RID: 439
	[Range(0f, 100f)]
	public static float requestedAmbienceVolume = 100f;

	// Token: 0x040001B8 RID: 440
	public static int waypointAngleSnap = 30;

	// Token: 0x040001B9 RID: 441
	public static int waypointAngleSnapIndex = 5;

	// Token: 0x040001BA RID: 442
	public static int maxShipsInCombat = 30;

	// Token: 0x040001BB RID: 443
	public static bool usingWindowsCursor = false;

	// Token: 0x040001BC RID: 444
	public static bool missionPhaseReportStartOpen = false;

	// Token: 0x040001BD RID: 445
	public static bool unpauseAfterMissionAssignment = false;

	// Token: 0x040001BE RID: 446
	public static bool assignmentPhaseCouncilorCameraFocus = true;

	// Token: 0x040001BF RID: 447
	public static bool alertSpaceTimerNotifications = false;

	// Token: 0x040001C0 RID: 448
	public static bool cycleNextCouncilorWhenAssigningMissions = true;

	// Token: 0x040001C1 RID: 449
	public static bool showMonthlyIncomes = false;

	// Token: 0x040001C2 RID: 450
	public static bool muteInBackground = true;

	// Token: 0x040001C3 RID: 451
	public static bool compressSaves = true;

	// Token: 0x040001C4 RID: 452
	public static Vector2 storedResolution = new Vector2(1920f, 1080f);

	// Token: 0x040001C5 RID: 453
	public static int storedRefreshRate = 60;

	// Token: 0x040001C6 RID: 454
	public static int antiAliasingMode = 0;

	// Token: 0x040001C7 RID: 455
	public static bool confineCursor = false;

	// Token: 0x040001C8 RID: 456
	public static bool vsyncEnabled = false;

	// Token: 0x040001C9 RID: 457
	public static bool displaySystemClock = false;

	// Token: 0x040001CA RID: 458
	public static bool showHighSpeedOrbitTrails = true;

	// Token: 0x040001CB RID: 459
	public static bool showEarthLights = true;

	// Token: 0x040001CC RID: 460
	public static bool useTextureStreaming = true;

	// Token: 0x040001CD RID: 461
	public static int uiScaleSetting = 0;

	// Token: 0x040001CE RID: 462
	public static bool enableAccessibilityMagnifier = false;

	// Token: 0x040001CF RID: 463
	public static bool useCouncilorVideo = true;

	// Token: 0x040001D0 RID: 464
	public static int skyboxVariant = 0;

	// Token: 0x040001D1 RID: 465
	public static float tooltipDelayPrimary = 0f;

	// Token: 0x040001D2 RID: 466
	public static float tooltipDelaySupplemental = 0.4f;

	// Token: 0x040001D3 RID: 467
	public static List<TINotificationTemplateOverride> notificationOverrides = new List<TINotificationTemplateOverride>();

	// Token: 0x040001D4 RID: 468
	public static List<TINotificationTemplate> notificationTemplates = new List<TINotificationTemplate>();

	// Token: 0x040001D5 RID: 469
	public static TIPlayerProfileManager.StoredCampaignOptions storedCampaignOptions = new TIPlayerProfileManager.StoredCampaignOptions();

	// Token: 0x040001D6 RID: 470
	public static Dictionary<string, string> subscribedMods = new Dictionary<string, string>();

	// Token: 0x040001D7 RID: 471
	public static Dictionary<string, string> modsToUninstall = new Dictionary<string, string>();

	// Token: 0x040001D8 RID: 472
	public static bool storedFullscreenMode = true;

	// Token: 0x040001D9 RID: 473
	public static bool firstGame = true;

	// Token: 0x040001DA RID: 474
	public static bool useMods = false;

	// Token: 0x040001DB RID: 475
	public static bool loadingFailureDueToMods = false;

	// Token: 0x040001DC RID: 476
	public static bool showFPS = false;

	// Token: 0x040001DD RID: 477
	public static List<string> KBList = new List<string>();

	// Token: 0x040001DE RID: 478
	public static List<string> KBModifierList = new List<string>();

	// Token: 0x040001DF RID: 479
	public static bool fatalError;

	// Token: 0x02000AC6 RID: 2758
	[Serializable]
	public class StoredCampaignOptions
	{
		// Token: 0x0400486D RID: 18541
		public bool isValid;

		// Token: 0x0400486E RID: 18542
		public int customFactionStartingNationGroup;

		// Token: 0x0400486F RID: 18543
		public List<int> startingCouncilorProfessions = new List<int>();

		// Token: 0x04004870 RID: 18544
		public bool usePlayerCountryForStartingCouncilor = true;

		// Token: 0x04004871 RID: 18545
		public bool variableProjectUnlocks = true;

		// Token: 0x04004872 RID: 18546
		public bool showTriggeredProjects;

		// Token: 0x04004873 RID: 18547
		public bool addAlienAssaultCarrierFleet;

		// Token: 0x04004874 RID: 18548
		public bool cinematicCombatRealismDV;

		// Token: 0x04004875 RID: 18549
		public bool cinematicCombatRealismScale;

		// Token: 0x04004876 RID: 18550
		public bool otherFactionStartingNations;

		// Token: 0x04004877 RID: 18551
		public bool canDisableFactions;

		// Token: 0x04004878 RID: 18552
		public int researchSpeedMultiplier = 99;

		// Token: 0x04004879 RID: 18553
		public int controlPointMaintenanceFreebieBonus;

		// Token: 0x0400487A RID: 18554
		public int controlPointMaintenanceFreebieBonusAI;

		// Token: 0x0400487B RID: 18555
		public int missionControlBonus;

		// Token: 0x0400487C RID: 18556
		public int missionControlBonusAI;

		// Token: 0x0400487D RID: 18557
		public int alienProgressionSpeed = 20;

		// Token: 0x0400487E RID: 18558
		public int miningProductivityMultiplier = 20;

		// Token: 0x0400487F RID: 18559
		public int nationalIPMultiplier = 4;

		// Token: 0x04004880 RID: 18560
		public int averageMonthlyEvents = 5;

		// Token: 0x04004881 RID: 18561
		public int miningRatePlayer = 20;

		// Token: 0x04004882 RID: 18562
		public int miningRateHumanAI = 20;

		// Token: 0x04004883 RID: 18563
		public int miningRateAlien = 20;

		// Token: 0x04004884 RID: 18564
		public int habConstructionSpeedPlayer = 20;

		// Token: 0x04004885 RID: 18565
		public int habConstructionSpeedHumanAI = 20;

		// Token: 0x04004886 RID: 18566
		public int habConstructionSpeedAlien = 20;

		// Token: 0x04004887 RID: 18567
		public int shipConstructionSpeedPlayer = 20;

		// Token: 0x04004888 RID: 18568
		public int shipConstructionSpeedHumanAI = 20;

		// Token: 0x04004889 RID: 18569
		public int shipConstructionSpeedAlien = 20;
	}
}
