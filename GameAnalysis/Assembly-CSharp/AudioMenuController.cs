using System;
using FMOD.Studio;
using FMODUnity;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000441 RID: 1089
public class AudioMenuController : MonoBehaviour
{
	// Token: 0x0600169F RID: 5791 RVA: 0x000738BC File Offset: 0x00071ABC
	private void Start()
	{
		this.isInitializing = true;
		this.Init();
		this.LoadLocalizedText();
		this.isInitializing = false;
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x000738D8 File Offset: 0x00071AD8
	public void Init()
	{
		this.volumeMasterSlider.value = (float)TIPlayerProfileManager.GetIntByKey("VolumeMaster", (int)this.volumeMasterSlider.value);
		this.volumeMusicSlider.value = (float)TIPlayerProfileManager.GetIntByKey("VolumeMusic", (int)this.volumeMusicSlider.value);
		this.volumeUISlider.value = (float)TIPlayerProfileManager.GetIntByKey("VolumeUI", (int)this.volumeUISlider.value);
		this.volumeEffectsSlider.value = (float)TIPlayerProfileManager.GetIntByKey("VolumeEffects", (int)this.volumeEffectsSlider.value);
		this.volumeVoiceSlider.value = (float)TIPlayerProfileManager.GetIntByKey("VolumeVoice", (int)this.volumeVoiceSlider.value);
		this.volumeAmbienceSlider.value = (float)TIPlayerProfileManager.GetIntByKey("VolumeAmbience", (int)this.volumeAmbienceSlider.value);
		this.toggleMuteInBackgroundToggle.isOn = TIPlayerProfileManager.muteInBackground;
		this.volumeMasterValueText.text = this.volumeMasterSlider.value.ToString() + "%";
		this.volumeMusicValueText.text = this.volumeMusicSlider.value.ToString() + "%";
		this.volumeUIValueText.text = this.volumeUISlider.value.ToString() + "%";
		this.volumeEffectsValueText.text = this.volumeEffectsSlider.value.ToString() + "%";
		this.volumeVoiceValueText.text = this.volumeVoiceSlider.value.ToString() + "%";
		this.volumeAmbienceValueText.text = this.volumeAmbienceSlider.value.ToString() + "%";
		this.UpdateMasterVolume();
		this.UpdateMusicVolume();
		this.UpdateEffectsVolume();
		this.UpdateVoiceVolume();
		this.UpdateAmbienceVolume();
		this.UpdateUIVolume();
		this.initEventSFX = true;
		this.initEventVO = true;
		this.initEventAMB = true;
		this.initEventUI = true;
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x00073AF4 File Offset: 0x00071CF4
	public void UpdateMasterVolume()
	{
		this.volumeMasterValueText.text = this.volumeMasterSlider.value.ToString() + "%";
		TIPlayerProfileManager.requestedMasterVolume = this.volumeMasterSlider.value;
		BusManager.SetVolume(BusManager.Master, this.volumeMasterSlider.value / 100f);
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x00073B54 File Offset: 0x00071D54
	public void UpdateMusicVolume()
	{
		this.volumeMusicValueText.text = this.volumeMusicSlider.value.ToString() + "%";
		TIPlayerProfileManager.requestedMusicVolume = this.volumeMusicSlider.value;
		BusManager.SetVolume(BusManager.Music, this.volumeMusicSlider.value / 100f);
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x00073BB4 File Offset: 0x00071DB4
	public void UpdateEffectsVolume()
	{
		this.volumeEffectsValueText.text = this.volumeEffectsSlider.value.ToString() + "%";
		TIPlayerProfileManager.requestedEffectsVolume = this.volumeEffectsSlider.value;
		BusManager.SetVolume(BusManager.SFX, this.volumeEffectsSlider.value / 100f);
		if (!this.eventInstanceSFX.isValid())
		{
			this.eventInstanceSFX = AudioManager.CreateFMODInstance("event:/SFX/Game_SFX/Ship_Fire/trig_SFX_BlasterMediumLoudHigh005MonoRR");
			this.eventInstanceSFX.set3DAttributes(base.transform.position.To3DAttributes());
		}
		if (this.initEventSFX && this.audioEffectDelay <= 0f)
		{
			this.PlaySFXEvent();
		}
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x00073C68 File Offset: 0x00071E68
	public void UpdateVoiceVolume()
	{
		this.volumeVoiceValueText.text = this.volumeVoiceSlider.value.ToString() + "%";
		TIPlayerProfileManager.requestedVoiceVolume = this.volumeVoiceSlider.value;
		BusManager.SetVolume(BusManager.Voice, this.volumeVoiceSlider.value / 100f);
		if (!this.eventInstanceVO.isValid())
		{
			this.eventInstanceVO = AudioManager.CreateFMODInstance("event:/VO/ENG/AMER/M/0/Selection/MissionAssigned");
		}
		if (this.initEventVO)
		{
			this.eventInstanceVO.Play(Camera.main.gameObject);
		}
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x00073D04 File Offset: 0x00071F04
	public void UpdateAmbienceVolume()
	{
		this.volumeAmbienceValueText.text = this.volumeAmbienceSlider.value.ToString() + "%";
		TIPlayerProfileManager.requestedAmbienceVolume = this.volumeAmbienceSlider.value;
		BusManager.SetVolume(BusManager.Ambient, this.volumeAmbienceSlider.value / 100f);
		if (!this.eventInstanceAMB.isValid())
		{
			this.eventInstanceAMB = AudioManager.CreateFMODInstance("event:/SFX/Environment/trig_SFX_AmbientPlanet");
		}
		this.ambientPlayTimer = 4f;
		if (this.initEventAMB)
		{
			this.eventInstanceAMB.Play(Camera.main.gameObject);
		}
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x00073DAC File Offset: 0x00071FAC
	public void UpdateUIVolume()
	{
		this.volumeUIValueText.text = this.volumeUISlider.value.ToString() + "%";
		TIPlayerProfileManager.requestedUIVolume = this.volumeUISlider.value;
		BusManager.SetVolume(BusManager.UI, this.volumeUISlider.value / 100f);
		BusManager.SetVolume(BusManager.UI_Special_UI_Reverb, this.volumeUISlider.value / 100f);
		if (this.initEventUI && this.audioEffectDelay <= 0f)
		{
			this.PlayUIEvent();
		}
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x00073E42 File Offset: 0x00072042
	public void UpdateOnToggleMuteInBackground()
	{
		TIPlayerProfileManager.muteInBackground = this.toggleMuteInBackgroundToggle.isOn;
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x00073E68 File Offset: 0x00072068
	public void ApplyAudioSettings()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}
		TIPlayerProfileManager.SavePlayerConfig();
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x00073E84 File Offset: 0x00072084
	private void Update()
	{
		if (this.ambientPlayTimer > 0f)
		{
			this.ambientPlayTimer -= Time.deltaTime;
			if (this.ambientPlayTimer <= 0f)
			{
				this.eventInstanceAMB.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
		}
		if (this.audioEffectDelay > 0f)
		{
			this.audioEffectDelay -= Time.deltaTime;
		}
		if (Input.GetMouseButtonUp(0) && this.audioEffectDelay > 0f && this.playUIEffect)
		{
			this.PlayUIEvent();
		}
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x00073F0C File Offset: 0x0007210C
	private void PlayUIEvent()
	{
		if (!this.isInitializing)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatWeaponCycle", false, false);
		}
		this.audioEffectDelay = 0.35f;
		this.playUIEffect = true;
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x00073F34 File Offset: 0x00072134
	private void PlaySFXEvent()
	{
		this.eventInstanceSFX.Play(Camera.main.gameObject);
		this.audioEffectDelay = 0.5f;
		this.playUIEffect = false;
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x00073F60 File Offset: 0x00072160
	private void OnDisable()
	{
		if (this.eventInstanceAMB.isValid())
		{
			this.eventInstanceAMB.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (this.eventInstanceVO.isValid())
		{
			this.eventInstanceVO.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (this.eventInstanceSFX.isValid())
		{
			this.eventInstanceSFX.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		this.playUIEffect = false;
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x00073FC4 File Offset: 0x000721C4
	public void LoadLocalizedText()
	{
		this.volumeMasterTitle.text = Loc.T("UI.Options.AudioMaster");
		this.volumeMusicTitle.text = Loc.T("UI.Options.AudioMusic");
		this.volumeUITitle.text = Loc.T("UI.Options.AudioUI");
		this.volumeEffectsTitle.text = Loc.T("UI.Options.AudioEffects");
		this.volumeVoiceTitle.text = Loc.T("UI.Options.AudioVoice");
		this.volumeAmbienceTitle.text = Loc.T("UI.Options.AudioAmbience");
		this.toggleMuteInBackgroundTitle.text = Loc.T("UI.Options.MuteInBackground");
		this.applyChangesTitle.text = Loc.T("UI.Options.ApplyChanges");
	}

	// Token: 0x040014E6 RID: 5350
	[Header("Option Titles")]
	public TextMeshProUGUI volumeMasterTitle;

	// Token: 0x040014E7 RID: 5351
	public TextMeshProUGUI volumeMusicTitle;

	// Token: 0x040014E8 RID: 5352
	public TextMeshProUGUI volumeUITitle;

	// Token: 0x040014E9 RID: 5353
	public TextMeshProUGUI volumeEffectsTitle;

	// Token: 0x040014EA RID: 5354
	public TextMeshProUGUI volumeVoiceTitle;

	// Token: 0x040014EB RID: 5355
	public TextMeshProUGUI volumeAmbienceTitle;

	// Token: 0x040014EC RID: 5356
	public TextMeshProUGUI toggleMuteInBackgroundTitle;

	// Token: 0x040014ED RID: 5357
	public TextMeshProUGUI applyChangesTitle;

	// Token: 0x040014EE RID: 5358
	[Header("Volume Values")]
	public TextMeshProUGUI volumeMasterValueText;

	// Token: 0x040014EF RID: 5359
	public TextMeshProUGUI volumeMusicValueText;

	// Token: 0x040014F0 RID: 5360
	public TextMeshProUGUI volumeUIValueText;

	// Token: 0x040014F1 RID: 5361
	public TextMeshProUGUI volumeEffectsValueText;

	// Token: 0x040014F2 RID: 5362
	public TextMeshProUGUI volumeVoiceValueText;

	// Token: 0x040014F3 RID: 5363
	public TextMeshProUGUI volumeAmbienceValueText;

	// Token: 0x040014F4 RID: 5364
	[Header("Sliders(Not the burgers)")]
	public Slider volumeMasterSlider;

	// Token: 0x040014F5 RID: 5365
	public Slider volumeMusicSlider;

	// Token: 0x040014F6 RID: 5366
	public Slider volumeUISlider;

	// Token: 0x040014F7 RID: 5367
	public Slider volumeEffectsSlider;

	// Token: 0x040014F8 RID: 5368
	public Slider volumeVoiceSlider;

	// Token: 0x040014F9 RID: 5369
	public Slider volumeAmbienceSlider;

	// Token: 0x040014FA RID: 5370
	[Header("Toggles")]
	public Toggle toggleMuteInBackgroundToggle;

	// Token: 0x040014FB RID: 5371
	private EventInstance eventInstanceSFX;

	// Token: 0x040014FC RID: 5372
	private EventInstance eventInstanceVO;

	// Token: 0x040014FD RID: 5373
	private EventInstance eventInstanceAMB;

	// Token: 0x040014FE RID: 5374
	private bool initEventSFX;

	// Token: 0x040014FF RID: 5375
	private bool initEventVO;

	// Token: 0x04001500 RID: 5376
	private bool initEventAMB;

	// Token: 0x04001501 RID: 5377
	private bool initEventUI;

	// Token: 0x04001502 RID: 5378
	private bool playUIEffect;

	// Token: 0x04001503 RID: 5379
	private float audioEffectDelay;

	// Token: 0x04001504 RID: 5380
	private float ambientPlayTimer = 0.1f;

	// Token: 0x04001505 RID: 5381
	protected CanvasManager canvasManager;

	// Token: 0x04001506 RID: 5382
	private bool isInitializing = true;
}
