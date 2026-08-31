using System;
using System.Collections;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x02000428 RID: 1064
public class Cinematic2DController : MonoBehaviour
{
	// Token: 0x0600162B RID: 5675 RVA: 0x00070AC0 File Offset: 0x0006ECC0
	public IEnumerator BeginWhenPrepared(bool playAudio = true, bool intro = false)
	{
		this.hasBeenPrepared = false;
		this.Init();
		TIInputManager.acceptingInput = false;
		BusManager.SetVolume(BusManager.UI, 0f);
		while (!this.videoPlayer.isPrepared)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		this.hasBeenPrepared = true;
		this.Begin(playAudio, intro);
		yield break;
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x00070AE0 File Offset: 0x0006ECE0
	public void Begin(bool playAudio = true, bool intro = false)
	{
		TIUtilities.TryPlayVideo(this.videoPlayer);
		this.playingIntroText = intro;
		this.playingIntroCinematic = intro;
		string text = this.cinematicPathString.Split(new char[] { '/' })[1];
		string text2 = this.generalIntroString.Split(new char[] { '/' })[1];
		this.commonCinematicPathString = text2;
		if (text.Contains("general"))
		{
			text = text.Replace("general", "general_" + this.ideologyString);
		}
		this.template = TemplateManager.Find<TI2DCinematicTemplate>(text, false);
		this.generalIntroTemplate = TemplateManager.Find<TI2DCinematicTemplate>(text2, false);
		this.Init();
		if (intro)
		{
			this.cinematicText.text = Loc.T("TICinematicsTemplate.cinematics/" + text2 + "1");
		}
		else
		{
			this.cinematicText.text = Loc.T("TICinematicsTemplate." + this.cinematicPathString + "1");
		}
		if (playAudio)
		{
			BusManager.SetVolume(BusManager.Music, 0f);
		}
		this.active = true;
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x00070BEC File Offset: 0x0006EDEC
	public void Init()
	{
		this.cineCanvasGroup = base.GetComponent<CanvasGroup>();
		this.cineCanvasGroup.alpha = 1f;
		this.cinematicText.text = "";
		this.GetTextTimeStamps();
		this.textTimer = 0f;
		this.currentTextSequence = 0;
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00070C3D File Offset: 0x0006EE3D
	public void ShowTestCinematic()
	{
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x00070C40 File Offset: 0x0006EE40
	private void Update()
	{
		if (!this.hasBeenPrepared)
		{
			return;
		}
		if (base.gameObject.activeSelf && !this.videoPlayer.isPlaying && this.active && this.hasBeenPrepared)
		{
			this.OnClickCloseCinematic();
		}
		this.textTimer += Time.deltaTime;
		if (this.playingIntroText)
		{
			if (this.currentTextSequence < this.generalTextStrings && this.textTimer >= this.textTimeStamps[this.currentTextSequence])
			{
				this.cinematicText.text = Loc.T("TICinematicsTemplate." + this.generalIntroString + (this.currentTextSequence + 1).ToString());
				this.currentTextSequence++;
				if (this.currentTextSequence == this.generalTextStrings)
				{
					this.currentTextSequence = 0;
					this.playingIntroText = false;
					return;
				}
			}
		}
		else if (this.playingIntroCinematic)
		{
			if (this.currentTextSequence + this.generalTextStrings < this.template.textSequences && this.textTimer >= this.textTimeStamps[this.currentTextSequence + this.generalTextStrings])
			{
				this.cinematicText.text = Loc.T("TICinematicsTemplate." + this.cinematicPathString + (this.currentTextSequence + 1).ToString());
				this.currentTextSequence++;
				Debug.Log("Seq:" + this.currentTextSequence.ToString());
			}
		}
		else if (this.currentTextSequence < this.template.textSequences && this.textTimer >= this.textTimeStamps[this.currentTextSequence])
		{
			this.cinematicText.text = Loc.T("TICinematicsTemplate." + this.cinematicPathString + (this.currentTextSequence + 1).ToString());
			this.currentTextSequence++;
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			this.introQueued = false;
			this.OnClickCloseCinematic();
		}
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x00070E48 File Offset: 0x0006F048
	private void GetTextTimeStamps()
	{
		if (this.template != null)
		{
			this.textTimeStamps[0] = this.template.textTimeStamp1;
			this.textTimeStamps[1] = this.template.textTimeStamp2;
			this.textTimeStamps[2] = this.template.textTimeStamp3;
			this.textTimeStamps[3] = this.template.textTimeStamp4;
			this.textTimeStamps[4] = this.template.textTimeStamp5;
			this.textTimeStamps[5] = this.template.textTimeStamp6;
			this.textTimeStamps[6] = this.template.textTimeStamp7;
			this.textTimeStamps[7] = this.template.textTimeStamp8;
			this.textTimeStamps[8] = this.template.textTimeStamp9;
			this.textTimeStamps[9] = this.template.textTimeStamp10;
			this.textTimeStamps[10] = this.template.textTimeStamp11;
			this.textTimeStamps[11] = this.template.textTimeStamp12;
			this.textTimeStamps[12] = this.template.textTimeStamp13;
			this.textTimeStamps[13] = this.template.textTimeStamp14;
			this.textTimeStamps[14] = this.template.textTimeStamp15;
			this.textTimeStamps[15] = this.template.textTimeStamp16;
			this.textTimeStamps[16] = this.template.textTimeStamp17;
			this.textTimeStamps[17] = this.template.textTimeStamp18;
			this.textTimeStamps[18] = this.template.textTimeStamp19;
			this.textTimeStamps[19] = this.template.textTimeStamp20;
		}
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x00070FE8 File Offset: 0x0006F1E8
	public void OnClickCloseCinematic()
	{
		if (this.introQueued)
		{
			this.StartQueuedCinematic();
			return;
		}
		TIInputManager.acceptingInput = true;
		BusManager.SetVolume(BusManager.UI, TIPlayerProfileManager.uiVolumeModifier());
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
		this.cinemaObject.SetActive(false);
		this.videoPlayer.targetTexture.Release();
		BusManager.SetVolume(BusManager.Music, TIPlayerProfileManager.musicVolumeModifier());
		AudioManager.StopCinematicAudio();
		MusicController.Instance.ChangeMusicScene();
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x00071060 File Offset: 0x0006F260
	public void StartQueuedCinematic()
	{
		this.cinematicPathString = this.queuedCinematicPathString;
		this.videoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.cinematicPathString);
		this.videoPlayer.SetDirectAudioVolume(0, TIPlayerProfileManager.masterVolumeModifier());
		TIUtilities.TryPlayVideo(this.videoPlayer);
		this.introQueued = false;
		this.Begin(false, false);
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x000710BF File Offset: 0x0006F2BF
	private void OnEnable()
	{
		this.videoPlayer.SetDirectAudioVolume(0, TIPlayerProfileManager.masterVolumeModifier());
	}

	// Token: 0x0400143A RID: 5178
	public TI2DCinematicTemplate template;

	// Token: 0x0400143B RID: 5179
	private TI2DCinematicTemplate generalIntroTemplate;

	// Token: 0x0400143C RID: 5180
	public GameObject cinemaObject;

	// Token: 0x0400143D RID: 5181
	public Image cinematicIllustration;

	// Token: 0x0400143E RID: 5182
	public TMP_Text cinematicText;

	// Token: 0x0400143F RID: 5183
	private CanvasGroup cineCanvasGroup;

	// Token: 0x04001440 RID: 5184
	public CanvasGroup textboxCanvasGroup;

	// Token: 0x04001441 RID: 5185
	public Button continueButton;

	// Token: 0x04001442 RID: 5186
	public VideoPlayer videoPlayer;

	// Token: 0x04001443 RID: 5187
	public string audioPath;

	// Token: 0x04001444 RID: 5188
	public string ideologyString;

	// Token: 0x04001445 RID: 5189
	[Tooltip("How long the fade lasts")]
	public float fadeTimer;

	// Token: 0x04001446 RID: 5190
	[Tooltip("Time between letters being typed")]
	public float typingSpeed = 0.1f;

	// Token: 0x04001447 RID: 5191
	[TextArea]
	public string cinematicDisplayText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

	// Token: 0x04001448 RID: 5192
	public string cinematicPathString;

	// Token: 0x04001449 RID: 5193
	private string commonCinematicPathString;

	// Token: 0x0400144A RID: 5194
	public string queuedCinematicPathString;

	// Token: 0x0400144B RID: 5195
	public bool introQueued;

	// Token: 0x0400144C RID: 5196
	private string generalIntroString = "cinematics/_intro_general";

	// Token: 0x0400144D RID: 5197
	private int currentTextSequence;

	// Token: 0x0400144E RID: 5198
	private float[] textTimeStamps = new float[20];

	// Token: 0x0400144F RID: 5199
	private float[] textTimeStampsCommon = new float[20];

	// Token: 0x04001450 RID: 5200
	private float textTimer;

	// Token: 0x04001451 RID: 5201
	private int generalTextStrings = 5;

	// Token: 0x04001452 RID: 5202
	private bool playingIntroText;

	// Token: 0x04001453 RID: 5203
	private bool playingIntroCinematic;

	// Token: 0x04001454 RID: 5204
	private bool active;

	// Token: 0x04001455 RID: 5205
	private bool hasBeenPrepared;
}
