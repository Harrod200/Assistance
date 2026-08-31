using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

// Token: 0x02000443 RID: 1091
public class GraphicsMenuController : MonoBehaviour
{
	// Token: 0x060016B4 RID: 5812 RVA: 0x00074200 File Offset: 0x00072400
	private void Start()
	{
		this.isInitializing = true;
		this.mainCameraPostProcessingLayer = Camera.main.GetComponent<PostProcessLayer>();
		this.LoadLocalizedText();
		this.DisplayDefaults();
		this.LoadValidResolutions();
		Debug.Log("QualityLevel" + QualitySettings.GetQualityLevel().ToString());
		Debug.Log("Master Texture Limit" + QualitySettings.masterTextureLimit.ToString());
		Debug.Log("AA Mode: " + TIPlayerProfileManager.antiAliasingMode.ToString());
		Debug.Log("AntiAliasing:" + QualitySettings.antiAliasing.ToString());
		Debug.Log("VSYNC: " + QualitySettings.vSyncCount.ToString());
		Debug.Log("TextureStreaming: " + QualitySettings.streamingMipmapsActive.ToString());
		this.isInitializing = false;
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x000742E4 File Offset: 0x000724E4
	private void DisplayDefaults()
	{
		if (TIPlayerProfileManager.fatalError)
		{
			return;
		}
		this.qualitySettingsDropdown.value = int.Parse(TIPlayerProfileManager.GetValue("QualitySetting"));
		this.textureSettingsDropdown.value = int.Parse(TIPlayerProfileManager.GetValue("TextureQuality"));
		this.skyboxVariantDropdown.value = int.Parse(TIPlayerProfileManager.GetValue("SkyboxVariant"));
		this.skyboxVariantDropdown.captionText.text = this.skyboxVariantDropdown.options[this.skyboxVariantDropdown.value].text;
		this.antiAliasingSettingsDropdownLabel.text = TIPlayerProfileManager.GetValue("AntiAliasing");
		this.UpdateAntiAliasingMode(int.Parse(TIPlayerProfileManager.GetValue("AntiAliasingMode")));
		this.fullscreenSettingToggle.isOn = bool.Parse(TIPlayerProfileManager.GetValue("Fullscreen"));
		this.enableVSyncToggle.isOn = bool.Parse(TIPlayerProfileManager.GetValue("VSync"));
		this.textureStreamingToggle.isOn = bool.Parse(TIPlayerProfileManager.GetValue("TextureStreaming"));
		this.uiScaleSlider.SetValueWithoutNotify((float)int.Parse(TIPlayerProfileManager.GetValue("UIScaleSetting")));
		this.UIScaleValue.SetText((1080f / (float)TemplateManager.global.uiScaleValues[(int)this.uiScaleSlider.value]).ToPercent("P0"));
		this.accessibilityMagnifierToggle.isOn = bool.Parse(TIPlayerProfileManager.GetValue("EnableAccessibilityMagnifier"));
		this.confineCursorToggle.isOn = TIPlayerProfileManager.confineCursor;
		this.useCouncilorVideoToggle.isOn = TIPlayerProfileManager.useCouncilorVideo;
		if (TIUtilities.IsSteamDeck())
		{
			this.useCouncilorVideoToggle.gameObject.SetActive(false);
		}
		this.qualityDirty = false;
		this.textureDirty = false;
		this.antiAliasingDirty = false;
		this.antiAliasingModeDirty = false;
		this.resolutionDirty = false;
		this.fullscreenDirty = false;
		this.confineCursorDirty = false;
		this.textureStreamingDirty = false;
		this.useLargeUIScaleDirty = false;
		this.useAccessibilityMagnifierDirty = false;
		this.skyboxVariantDirty = false;
		this.useCouncilorVideoDirty = false;
		if (int.Parse(TIPlayerProfileManager.GetValue("QualitySetting")) == 99)
		{
			TIPlayerProfileManager.isCustomQuality = true;
			this.qualityLevelLabel.text = Loc.T("UI.Options.GraphicsCustom");
			return;
		}
		TIPlayerProfileManager.isCustomQuality = false;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x00074518 File Offset: 0x00072718
	private void LoadValidResolutions()
	{
		this.resolutionSettingsDropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = Screen.resolutions.Length - 1; i > -1; i--)
		{
			list.Add(Screen.resolutions[i].ToString());
		}
		this.resolutionSettingsDropdown.AddOptions(list);
		for (int j = Screen.resolutions.Length - 1; j > -1; j--)
		{
			if (TIPlayerProfileManager.storedResolution.x == (float)Screen.resolutions[j].width && TIPlayerProfileManager.storedResolution.y == (float)Screen.resolutions[j].height && TIPlayerProfileManager.storedRefreshRate == Screen.resolutions[j].refreshRate)
			{
				this.resolutionSettingsDropdown.SetValueWithoutNotify(this.resolutionSettingsDropdown.options.Count - 1 - j);
			}
		}
		this.resolutionSettingsDropdownLabel.text = new StringBuilder(TIPlayerProfileManager.storedResolution.x.ToString()).Append(" x ").Append(TIPlayerProfileManager.storedResolution.y).Append(" @ ")
			.Append(TIPlayerProfileManager.storedRefreshRate)
			.Append("Hz")
			.ToString();
		this.UpdateCanIncreaseUIScale();
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x00074658 File Offset: 0x00072858
	public void ChangeQualitySettings()
	{
		if (this.qualitySettingsDropdown.value == 0 && SystemInfo.graphicsMemorySize < 5000 && SystemInfo.graphicsMemorySize > 3000)
		{
			StartMenuController componentInParent = base.GetComponentInParent<StartMenuController>();
			if (componentInParent != null)
			{
				componentInParent.ToggleHardwareWarning(true);
			}
		}
		else if (this.qualitySettingsDropdown.value == 1 && SystemInfo.graphicsMemorySize < 3000)
		{
			StartMenuController componentInParent2 = base.GetComponentInParent<StartMenuController>();
			if (componentInParent2 != null)
			{
				componentInParent2.ToggleHardwareWarning(true);
			}
		}
		this.qualityDirty = true;
		TIPlayerProfileManager.isCustomQuality = false;
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x000746D8 File Offset: 0x000728D8
	public void ChangeTextureSettings()
	{
		if (this.textureSettingsDropdown.value == 0 && SystemInfo.graphicsMemorySize < 5000 && SystemInfo.graphicsMemorySize > 3000)
		{
			StartMenuController componentInParent = base.GetComponentInParent<StartMenuController>();
			if (componentInParent != null)
			{
				componentInParent.ToggleHardwareWarning(true);
			}
		}
		else if (this.textureSettingsDropdown.value == 1 && SystemInfo.graphicsMemorySize < 3000)
		{
			StartMenuController componentInParent2 = base.GetComponentInParent<StartMenuController>();
			if (componentInParent2 != null)
			{
				componentInParent2.ToggleHardwareWarning(true);
			}
		}
		this.textureDirty = true;
		TIPlayerProfileManager.isCustomQuality = true;
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x00074757 File Offset: 0x00072957
	public void ChangeAntiAliasingSettings()
	{
		this.antiAliasingDirty = true;
		TIPlayerProfileManager.isCustomQuality = true;
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x00074768 File Offset: 0x00072968
	public void ChangeAntiAliasingMode()
	{
		if (this.antiAliasingModeDropdown.value == 0)
		{
			this.antiAliasingTitle.gameObject.SetActive(true);
			this.RadeonAAWarningObject.SetActive(TIUtilities.HasRadeonGPU());
		}
		else
		{
			this.antiAliasingTitle.gameObject.SetActive(false);
			this.RadeonAAWarningObject.SetActive(false);
		}
		this.antiAliasingModeDirty = true;
		TIPlayerProfileManager.isCustomQuality = true;
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x000747D0 File Offset: 0x000729D0
	public void ChangeSkyboxVariant()
	{
		this.skyboxVariantDirty = true;
		RenderSettings.skybox = TIUtilities.assetLoader.LoadAsset<Material>(TemplateManager.global.skyboxes[this.skyboxVariantDropdown.value]);
		if (GameControl.loadcycle100)
		{
			CameraManager.Singleton.SetSkybox(this.skyboxVariantDropdown.value);
		}
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x00074829 File Offset: 0x00072A29
	public void ChangeCouncilorVideoToggle()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.useCouncilorVideoDirty = true;
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x00074846 File Offset: 0x00072A46
	public void ChangeResolutionSettings()
	{
		this.resolutionDirty = true;
		this.UpdateCanIncreaseUIScale();
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x00074855 File Offset: 0x00072A55
	public void ToggleFullscreenMode()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.fullscreenModeSetting = this.fullscreenSettingToggle.isOn;
		this.fullscreenDirty = true;
		this.resolutionDirty = true;
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x0007488A File Offset: 0x00072A8A
	public void ToggleCursorConfineMode()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.confineCursorDirty = true;
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x000748A7 File Offset: 0x00072AA7
	public void ToggleVSyncMode()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.vsyncDirty = true;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x000748C4 File Offset: 0x00072AC4
	public void ToggleTextureStreaming()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.textureStreamingDirty = true;
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x000748E4 File Offset: 0x00072AE4
	public void ChangedLargeUIScale()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.UIScaleValue.SetText((1080f / (float)TemplateManager.global.uiScaleValues[(int)this.uiScaleSlider.value]).ToPercent("P0"));
		this.useLargeUIScaleDirty = true;
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x0007493F File Offset: 0x00072B3F
	public void ToggleAccessibilityMagnifier()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
		this.useAccessibilityMagnifierDirty = true;
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x0007495C File Offset: 0x00072B5C
	public void UpdateAntiAliasingText()
	{
		if (this.antiAliasingModeDropdown.value == 0)
		{
			if (QualitySettings.antiAliasing == 0)
			{
				this.antiAliasingSettingsDropdown.value = 0;
			}
			if (QualitySettings.antiAliasing == 2)
			{
				this.antiAliasingSettingsDropdown.value = 1;
			}
			if (QualitySettings.antiAliasing == 4)
			{
				this.antiAliasingSettingsDropdown.value = 2;
			}
			if (QualitySettings.antiAliasing == 8)
			{
				this.antiAliasingSettingsDropdown.value = 3;
				return;
			}
		}
		else
		{
			this.antiAliasingSettingsDropdown.value = 0;
		}
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x000749D4 File Offset: 0x00072BD4
	public void UpdateAntiAliasingMode(int mode)
	{
		if (this.antiAliasingModeDropdown.value != mode)
		{
			this.antiAliasingModeDropdown.value = mode;
		}
		TIPlayerProfileManager.antiAliasingMode = this.antiAliasingModeDropdown.value;
		if (TIPlayerProfileManager.antiAliasingMode != 0)
		{
			this.antiAliasingSettingsDropdown.value = 0;
			if (this.antiAliasingModeDropdown.value == 1)
			{
				this.mainCameraPostProcessingLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
			}
			QualitySettings.antiAliasing = 0;
			this.antiAliasingTitle.gameObject.SetActive(false);
			this.RadeonAAWarningObject.SetActive(false);
			return;
		}
		this.mainCameraPostProcessingLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
		this.antiAliasingTitle.gameObject.SetActive(true);
		this.RadeonAAWarningObject.SetActive(TIUtilities.HasRadeonGPU());
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x00074A8C File Offset: 0x00072C8C
	public void LoadLocalizedText()
	{
		this.qualitySettingsDropdown.options[0].text = Loc.T("UI.Options.GraphicsQuality0");
		this.qualitySettingsDropdown.options[1].text = Loc.T("UI.Options.GraphicsQuality1");
		this.qualitySettingsDropdown.options[2].text = Loc.T("UI.Options.GraphicsQuality2");
		this.textureSettingsDropdown.options[0].text = Loc.T("UI.Options.TextureQuality0");
		this.textureSettingsDropdown.options[1].text = Loc.T("UI.Options.TextureQuality1");
		this.textureSettingsDropdown.options[2].text = Loc.T("UI.Options.TextureQuality2");
		this.textureSettingsDropdown.options[3].text = Loc.T("UI.Options.TextureQuality3");
		this.antiAliasingModeDropdown.options[0].text = Loc.T("UI.Options.AntiAliasingMode0");
		this.antiAliasingModeDropdown.options[1].text = Loc.T("UI.Options.AntiAliasingMode1");
		this.antiAliasingModeDropdown.options[2].text = Loc.T("UI.Options.AntiAliasingMode2");
		this.skyboxVariantDropdown.options.Clear();
		foreach (string text in TemplateManager.global.skyboxes)
		{
			this.skyboxVariantDropdown.options.Add(new TMP_Dropdown.OptionData(text.Split(new char[] { '/' })[1]));
		}
		this.qualityTitle.text = Loc.T("UI.Options.GraphicsQuality");
		this.textureTitle.text = Loc.T("UI.Options.TextureQuality");
		this.antiAliasingTitle.text = Loc.T("UI.Options.AntiAliasingQuality");
		this.antiAliasingModeTitle.text = Loc.T("UI.Options.AntiAliasingMode");
		this.resolutionTitle.text = Loc.T("UI.Options.Resolution");
		this.fullscreenTitle.text = Loc.T("UI.Options.Fullscreen");
		this.confineCursorTitle.text = Loc.T("UI.Options.CursorConfine");
		this.enableVSyncTitle.text = Loc.T("UI.Options.VSync");
		this.textureStreamingTitle.text = Loc.T("UI.Options.TextureStreaming");
		this.UIScaleTitle.text = Loc.T("UI.Options.LargeUIScale");
		this.accessibilityMagnifierTitle.text = Loc.T("UI.Options.EnableAccessibilityMagnifier");
		this.skyboxVariantTitle.text = Loc.T("UI.Options.Skybox");
		this.applyChangesTitle.text = Loc.T("UI.Options.ApplyChanges");
		this.useCouncilorVideoTitle.text = Loc.T("UI.Options.UseCouncilorVideo");
		this.textureStreamingTT.SetDelegate("BodyText", () => Loc.T("UI.Options.TextureStreamingDescription"));
		this.uiScaleTT.SetDelegate("BodyText", () => Loc.T("UI.Options.UIScaleDescription"));
		this.accessibilityMagnifierTT.SetDelegate("BodyText", () => Loc.T("UI.Options.AccessibilityMagnifierDescription"));
		this.councilorVideoTT.SetDelegate("BodyText", () => Loc.T("UI.Options.UseCouncilorVideoTooltip"));
		this.RadeonAAWarningTT.SetDelegate("BodyText", () => Loc.T("UI.Options.RadeonAAWarning"));
		this.qualitySettingsDropdownLabel.text = (TIPlayerProfileManager.isCustomQuality ? Loc.T("UI.Options.GraphicsCustom") : this.qualitySettingsDropdown.options[this.qualitySettingsDropdown.value].text);
		this.textureSettingsDropdownLabel.text = this.textureSettingsDropdown.options[this.textureSettingsDropdown.value].text;
		this.antiAliasingModeDropdownLabel.text = Loc.T("UI.Options.AntiAliasingMode" + this.antiAliasingModeDropdown.value.ToString());
		this.antiAliasingSettingsDropdownLabel.text = this.antiAliasingSettingsDropdown.options[this.antiAliasingSettingsDropdown.value].text;
		this.UpdateAntiAliasingText();
		this.resolutionSettingsDropdownLabel.text = this.resolutionSettingsDropdown.options[this.resolutionSettingsDropdown.value].text;
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x00074F58 File Offset: 0x00073158
	public void UpdateQualitySettings()
	{
		bool flag = true;
		if (!TIPlayerProfileManager.isCustomQuality && this.qualityDirty)
		{
			QualitySettings.SetQualityLevel(this.qualitySettingsDropdown.value, true);
			TIPlayerProfileManager.SetMipmapMemoryBudget();
			this.textureSettingsDropdown.value = QualitySettings.GetQualityLevel();
			this.UpdateAntiAliasingText();
			this.textureDirty = false;
			this.antiAliasingDirty = false;
			this.qualitySettingsDropdownLabel.text = this.qualitySettingsDropdown.options[this.qualitySettingsDropdown.value].text;
			TIPlayerProfileManager.isCustomQuality = false;
			this.qualityDirty = false;
			this.vsyncDirty = true;
		}
		if (TIPlayerProfileManager.isCustomQuality)
		{
			if (this.textureDirty)
			{
				QualitySettings.masterTextureLimit = this.textureSettingsDropdown.value;
				this.textureDirty = false;
			}
			if (this.antiAliasingDirty)
			{
				int num = 0;
				if (this.antiAliasingSettingsDropdown.value == 1)
				{
					num = 2;
				}
				if (this.antiAliasingSettingsDropdown.value == 2)
				{
					num = 4;
				}
				if (this.antiAliasingSettingsDropdown.value == 3)
				{
					num = 8;
				}
				if (this.antiAliasingModeDropdown.value == 0)
				{
					QualitySettings.antiAliasing = num;
					this.RadeonAAWarningObject.SetActive(TIUtilities.HasRadeonGPU());
				}
				else
				{
					this.RadeonAAWarningObject.SetActive(false);
				}
				this.antiAliasingDirty = false;
			}
			this.qualityLevelLabel.text = Loc.T("UI.Options.GraphicsCustom");
		}
		if (this.resolutionDirty)
		{
			int num2 = int.Parse(this.resolutionSettingsDropdown.options[this.resolutionSettingsDropdown.value].text.Split(new char[] { 'x' })[0]);
			int num3 = int.Parse(this.resolutionSettingsDropdown.options[this.resolutionSettingsDropdown.value].text.Split(new char[] { 'x' })[1].Split(new char[] { '@' })[0]);
			int num4 = int.Parse(Regex.Replace(this.resolutionSettingsDropdown.options[this.resolutionSettingsDropdown.value].text.Split(new char[] { '@' })[1], "[^0-9]", ""));
			if (!this.fullscreenModeSetting)
			{
				flag = false;
				base.StartCoroutine(this.HandleWindowedFixedResolutionChange(num2, num3, num4));
			}
			else
			{
				Screen.SetResolution(num2, num3, this.fullscreenModeSetting, num4);
			}
			TIPlayerProfileManager.SetCursorConfineMode(TIPlayerProfileManager.confineCursor);
			TIPlayerProfileManager.storedResolution = new Vector2((float)num2, (float)num3);
			TIPlayerProfileManager.storedRefreshRate = num4;
			this.resolutionDirty = false;
			if (GameControl.canvasStack != null)
			{
				GameControl.canvasStack.RefreshUltraWideScaling();
			}
		}
		if (this.fullscreenDirty)
		{
			this.fullscreenDirty = false;
			TIPlayerProfileManager.storedFullscreenMode = this.fullscreenSettingToggle.isOn;
		}
		if (this.confineCursorDirty)
		{
			this.confineCursorDirty = false;
			TIPlayerProfileManager.SetCursorConfineMode(this.confineCursorToggle.isOn);
		}
		if (this.vsyncDirty)
		{
			this.vsyncDirty = false;
			TIPlayerProfileManager.vsyncEnabled = this.enableVSyncToggle.isOn;
			QualitySettings.vSyncCount = (this.enableVSyncToggle.isOn ? 1 : 0);
		}
		if (this.antiAliasingModeDirty)
		{
			this.UpdateAntiAliasingMode(this.antiAliasingModeDropdown.value);
			this.antiAliasingModeDirty = false;
		}
		if (this.textureStreamingDirty)
		{
			this.textureStreamingDirty = false;
			TIPlayerProfileManager.useTextureStreaming = this.textureStreamingToggle.isOn;
		}
		if (this.useLargeUIScaleDirty)
		{
			this.useLargeUIScaleDirty = false;
			this.UpdateUIScaleSetting();
		}
		if (this.useAccessibilityMagnifierDirty)
		{
			this.useAccessibilityMagnifierDirty = false;
			this.UpdateAccessibilityMagnifierSetting();
		}
		if (this.skyboxVariantDirty)
		{
			this.skyboxVariantDirty = false;
			TIPlayerProfileManager.skyboxVariant = this.skyboxVariantDropdown.value;
			RenderSettings.skybox = TIUtilities.assetLoader.LoadAsset<Material>(TemplateManager.global.skyboxes[TIPlayerProfileManager.skyboxVariant]);
			if (GameControl.loadcycle100)
			{
				CameraManager.Singleton.SetSkybox(this.skyboxVariantDropdown.value);
			}
		}
		if (this.useCouncilorVideoDirty)
		{
			this.useCouncilorVideoDirty = false;
			TIPlayerProfileManager.useCouncilorVideo = this.useCouncilorVideoToggle.isOn;
		}
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}
		if (flag)
		{
			TIPlayerProfileManager.SavePlayerConfig();
		}
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x00075350 File Offset: 0x00073550
	private void UpdateUIScaleSetting()
	{
		TIPlayerProfileManager.uiScaleSetting = (int)this.uiScaleSlider.value;
		if (GameControl.control.viewMgr.currentView == ViewType.MainMenu)
		{
			global::UnityEngine.Object.FindAnyObjectByType<StartMenuController>().UpdateUIScaling();
			return;
		}
		GameControl.canvasStack.RefreshUIScaling();
		if (GameControl.control.activePlayer != null)
		{
			GameControl.eventManager.TriggerEvent(new UIScaleSettingChange(), null, Array.Empty<object>());
		}
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x000753BC File Offset: 0x000735BC
	private void UpdateAccessibilityMagnifierSetting()
	{
		TIPlayerProfileManager.enableAccessibilityMagnifier = this.accessibilityMagnifierToggle.isOn;
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x000753CE File Offset: 0x000735CE
	private void UpdateCanIncreaseUIScale()
	{
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x000753D0 File Offset: 0x000735D0
	private IEnumerator HandleWindowedFixedResolutionChange(int x, int y, int refreshRate)
	{
		Screen.SetResolution(x, y, true, refreshRate);
		yield return null;
		Screen.SetResolution(x, y, false, refreshRate);
		TIPlayerProfileManager.SavePlayerConfig();
		yield break;
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x000753ED File Offset: 0x000735ED
	private void OnLanguageChangedEvent()
	{
		this.LoadLocalizedText();
		Loc.SwapFonts(base.gameObject);
	}

	// Token: 0x0400150B RID: 5387
	public TMP_Dropdown qualitySettingsDropdown;

	// Token: 0x0400150C RID: 5388
	public TMP_Dropdown textureSettingsDropdown;

	// Token: 0x0400150D RID: 5389
	public TMP_Dropdown antiAliasingSettingsDropdown;

	// Token: 0x0400150E RID: 5390
	public TMP_Dropdown antiAliasingModeDropdown;

	// Token: 0x0400150F RID: 5391
	public TMP_Dropdown resolutionSettingsDropdown;

	// Token: 0x04001510 RID: 5392
	public TMP_Dropdown skyboxVariantDropdown;

	// Token: 0x04001511 RID: 5393
	public TextMeshProUGUI qualitySettingsDropdownLabel;

	// Token: 0x04001512 RID: 5394
	public TextMeshProUGUI textureSettingsDropdownLabel;

	// Token: 0x04001513 RID: 5395
	public TextMeshProUGUI antiAliasingSettingsDropdownLabel;

	// Token: 0x04001514 RID: 5396
	public TextMeshProUGUI antiAliasingModeDropdownLabel;

	// Token: 0x04001515 RID: 5397
	public TextMeshProUGUI resolutionSettingsDropdownLabel;

	// Token: 0x04001516 RID: 5398
	public TextMeshProUGUI qualityTitle;

	// Token: 0x04001517 RID: 5399
	public TextMeshProUGUI textureTitle;

	// Token: 0x04001518 RID: 5400
	public TextMeshProUGUI antiAliasingTitle;

	// Token: 0x04001519 RID: 5401
	public TextMeshProUGUI antiAliasingModeTitle;

	// Token: 0x0400151A RID: 5402
	public TextMeshProUGUI resolutionTitle;

	// Token: 0x0400151B RID: 5403
	public TextMeshProUGUI fullscreenTitle;

	// Token: 0x0400151C RID: 5404
	public TextMeshProUGUI confineCursorTitle;

	// Token: 0x0400151D RID: 5405
	public TextMeshProUGUI applyChangesTitle;

	// Token: 0x0400151E RID: 5406
	public TextMeshProUGUI enableVSyncTitle;

	// Token: 0x0400151F RID: 5407
	public TextMeshProUGUI textureStreamingTitle;

	// Token: 0x04001520 RID: 5408
	public TextMeshProUGUI accessibilityMagnifierTitle;

	// Token: 0x04001521 RID: 5409
	public TextMeshProUGUI skyboxVariantTitle;

	// Token: 0x04001522 RID: 5410
	public TextMeshProUGUI useCouncilorVideoTitle;

	// Token: 0x04001523 RID: 5411
	public TextMeshProUGUI UIScaleTitle;

	// Token: 0x04001524 RID: 5412
	public TextMeshProUGUI UIScaleValue;

	// Token: 0x04001525 RID: 5413
	public TextMeshProUGUI qualityLevelLabel;

	// Token: 0x04001526 RID: 5414
	public Toggle enableVSyncToggle;

	// Token: 0x04001527 RID: 5415
	public Toggle fullscreenSettingToggle;

	// Token: 0x04001528 RID: 5416
	public Toggle confineCursorToggle;

	// Token: 0x04001529 RID: 5417
	public Toggle textureStreamingToggle;

	// Token: 0x0400152A RID: 5418
	public Toggle accessibilityMagnifierToggle;

	// Token: 0x0400152B RID: 5419
	public Toggle useCouncilorVideoToggle;

	// Token: 0x0400152C RID: 5420
	public Slider uiScaleSlider;

	// Token: 0x0400152D RID: 5421
	public TooltipTrigger textureStreamingTT;

	// Token: 0x0400152E RID: 5422
	public TooltipTrigger uiScaleTT;

	// Token: 0x0400152F RID: 5423
	public TooltipTrigger accessibilityMagnifierTT;

	// Token: 0x04001530 RID: 5424
	public TooltipTrigger councilorVideoTT;

	// Token: 0x04001531 RID: 5425
	public TooltipTrigger RadeonAAWarningTT;

	// Token: 0x04001532 RID: 5426
	public GameObject RadeonAAWarningObject;

	// Token: 0x04001533 RID: 5427
	private PostProcessLayer mainCameraPostProcessingLayer;

	// Token: 0x04001534 RID: 5428
	private bool fullscreenModeSetting = true;

	// Token: 0x04001535 RID: 5429
	private bool qualityDirty;

	// Token: 0x04001536 RID: 5430
	private bool textureDirty;

	// Token: 0x04001537 RID: 5431
	private bool antiAliasingDirty;

	// Token: 0x04001538 RID: 5432
	private bool antiAliasingModeDirty;

	// Token: 0x04001539 RID: 5433
	private bool resolutionDirty;

	// Token: 0x0400153A RID: 5434
	private bool fullscreenDirty;

	// Token: 0x0400153B RID: 5435
	private bool confineCursorDirty;

	// Token: 0x0400153C RID: 5436
	private bool vsyncDirty;

	// Token: 0x0400153D RID: 5437
	private bool textureStreamingDirty;

	// Token: 0x0400153E RID: 5438
	private bool useLargeUIScaleDirty;

	// Token: 0x0400153F RID: 5439
	private bool useAccessibilityMagnifierDirty;

	// Token: 0x04001540 RID: 5440
	private bool skyboxVariantDirty;

	// Token: 0x04001541 RID: 5441
	private bool useCouncilorVideoDirty;

	// Token: 0x04001542 RID: 5442
	private bool isInitializing = true;
}
