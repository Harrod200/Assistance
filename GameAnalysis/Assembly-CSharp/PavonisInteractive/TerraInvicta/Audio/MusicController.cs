using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009D8 RID: 2520
	public class MusicController : MonoBehaviour
	{
		// Token: 0x17001040 RID: 4160
		// (get) Token: 0x06005EA4 RID: 24228 RVA: 0x002CE1AB File Offset: 0x002CC3AB
		public static MusicController Instance
		{
			get
			{
				return MusicController._instance;
			}
		}

		// Token: 0x06005EA5 RID: 24229 RVA: 0x002CE1B2 File Offset: 0x002CC3B2
		private void Awake()
		{
			if (MusicController._instance != null && MusicController._instance != this)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			MusicController._instance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(this);
		}

		// Token: 0x06005EA6 RID: 24230 RVA: 0x002CE1E6 File Offset: 0x002CC3E6
		private void Start()
		{
			if (this.currentMusicInstance.isValid())
			{
				this.currentMusicInstance.SetVolume(1f);
			}
			this.PlaySceneMusic();
			SceneManager.sceneLoaded += this.ChangedActiveScene;
		}

		// Token: 0x06005EA7 RID: 24231 RVA: 0x002CE21D File Offset: 0x002CC41D
		public void ChangeMusicScene()
		{
			base.StartCoroutine(this.ChangeMusic());
		}

		// Token: 0x06005EA8 RID: 24232 RVA: 0x002CE22C File Offset: 0x002CC42C
		public void VolumeUpdated()
		{
			base.StartCoroutine(AudioManager.FadeAudio(this.currentMusicInstance, this.musicFadeTime, TIPlayerProfileManager.musicVolumeModifier()));
		}

		// Token: 0x06005EA9 RID: 24233 RVA: 0x002CE24C File Offset: 0x002CC44C
		private void PlaySceneMusic()
		{
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
			{
				this.StopFanfare();
				AudioManager.CreateFMODObjects("event:/Music/Menu/trig_Music_MainMenu", out this.currentMusicDescription, out this.currentMusicInstance);
			}
			else if (TIGlobalValuesState.isSpaceCombatEnabled)
			{
				AudioManager.CreateFMODObjects("event:/Music/Battle/trig_Music_BattleLayer_Shuffle", out this.currentMusicDescription, out this.currentMusicInstance);
			}
			else if (GameControl.control.activePlayer != null)
			{
				this.musicProgression = GameControl.control.activePlayer.GetDesiredMusicProgression();
				switch (this.musicProgression)
				{
				case CampaignMusicProgression.EarlyGame:
					AudioManager.CreateFMODObjects("event:/Music/trig_Music_All_Random", out this.currentMusicDescription, out this.currentMusicInstance);
					break;
				case CampaignMusicProgression.MidGame:
					AudioManager.CreateFMODObjects("event:/Music/trig_Music_All_Midgame", out this.currentMusicDescription, out this.currentMusicInstance);
					break;
				case CampaignMusicProgression.LateGame:
					AudioManager.CreateFMODObjects("event:/Music/trig_Music_All_Lategame", out this.currentMusicDescription, out this.currentMusicInstance);
					break;
				}
			}
			this.currentMusicInstance.Play();
			if (!this.playingFanfare)
			{
				base.StartCoroutine(AudioManager.FadeAudio(this.currentMusicInstance, this.musicFadeTime, 1f));
			}
		}

		// Token: 0x06005EAA RID: 24234 RVA: 0x002CE367 File Offset: 0x002CC567
		private void ChangedActiveScene(Scene scene, LoadSceneMode loadSceneMode)
		{
			base.StartCoroutine(this.WaitForScene(scene));
		}

		// Token: 0x06005EAB RID: 24235 RVA: 0x002CE377 File Offset: 0x002CC577
		private IEnumerator WaitForScene(Scene scene)
		{
			while (!GameControl.bootstrapFinished && scene.buildIndex != 0)
			{
				yield return new WaitForSeconds(1f);
			}
			base.StartCoroutine(this.ChangeMusic());
			yield break;
		}

		// Token: 0x06005EAC RID: 24236 RVA: 0x002CE38D File Offset: 0x002CC58D
		private IEnumerator ChangeMusic()
		{
			this.currentMusicInstance.Stop(STOP_MODE.ALLOWFADEOUT);
			while (!this.currentMusicInstance.IsStopped())
			{
				yield return new WaitForSeconds(1f);
			}
			this.currentMusicInstance.SetVolume(0f);
			this.PlaySceneMusic();
			yield break;
		}

		// Token: 0x06005EAD RID: 24237 RVA: 0x002CE39C File Offset: 0x002CC59C
		public void PlayFanfare(string path)
		{
			if (this.playingFanfare)
			{
				return;
			}
			this.playingFanfare = true;
			base.StartCoroutine(this.StartFanfare(path));
		}

		// Token: 0x06005EAE RID: 24238 RVA: 0x002CE3BC File Offset: 0x002CC5BC
		private IEnumerator StartFanfare(string path)
		{
			yield return new WaitForSeconds(this.musicFadeTime);
			if (!string.IsNullOrEmpty(path))
			{
				if (AudioManager.VerifyPath(path, false))
				{
					this.currentFanfareInstance = AudioManager.CreateFMODInstance(path);
					if (this.currentFanfareInstance.isValid() && !this.currentFanfareInstance.IsPlaying())
					{
						this.currentFanfareInstance.Play();
					}
					else
					{
						Debug.LogError("Fanfare instance not valid, " + path);
						this.playingFanfare = false;
					}
				}
				else
				{
					this.playingFanfare = false;
					Debug.Log("Failed to Find Fanfare Path, + " + path);
				}
			}
			base.StartCoroutine(AudioManager.FadeAudio(this.currentMusicInstance, this.musicFadeTime / 2f, 0f));
			base.StartCoroutine(this.UpdateFanFare());
			yield break;
		}

		// Token: 0x06005EAF RID: 24239 RVA: 0x002CE3D2 File Offset: 0x002CC5D2
		private IEnumerator UpdateFanFare()
		{
			while (this.playingFanfare)
			{
				yield return null;
				if (this.currentFanfareInstance.IsStopped())
				{
					yield return null;
					this.playingFanfare = false;
					this.currentFanfareInstance.Release();
					base.StartCoroutine(AudioManager.FadeAudio(this.currentMusicInstance, this.musicFadeTime, 1f));
				}
			}
			yield break;
		}

		// Token: 0x06005EB0 RID: 24240 RVA: 0x002CE3E1 File Offset: 0x002CC5E1
		private void StopFanfare()
		{
			if (this.currentFanfareInstance.isValid())
			{
				this.currentFanfareInstance.Stop(STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x06005EB1 RID: 24241 RVA: 0x002CE3FD File Offset: 0x002CC5FD
		private void Update()
		{
		}

		// Token: 0x04004388 RID: 17288
		private EventDescription currentMusicDescription;

		// Token: 0x04004389 RID: 17289
		private EventInstance currentMusicInstance;

		// Token: 0x0400438A RID: 17290
		private EventInstance currentFanfareInstance;

		// Token: 0x0400438B RID: 17291
		[SerializeField]
		private float musicFadeTime = 2f;

		// Token: 0x0400438C RID: 17292
		private static MusicController _instance;

		// Token: 0x0400438D RID: 17293
		private CampaignMusicProgression musicProgression;

		// Token: 0x0400438E RID: 17294
		private bool playingFanfare;
	}
}
