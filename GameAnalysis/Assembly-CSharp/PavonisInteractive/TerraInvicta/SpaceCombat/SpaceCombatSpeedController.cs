using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x02000A01 RID: 2561
	public class SpaceCombatSpeedController : MonoBehaviour
	{
		// Token: 0x170010FA RID: 4346
		// (get) Token: 0x06006260 RID: 25184 RVA: 0x002E20AE File Offset: 0x002E02AE
		public bool IsPaused
		{
			get
			{
				return this.gameTime.Paused;
			}
		}

		// Token: 0x170010FB RID: 4347
		// (get) Token: 0x06006261 RID: 25185 RVA: 0x002E20BB File Offset: 0x002E02BB
		public string combatTimeString
		{
			get
			{
				return this.combatTimeText.text;
			}
		}

		// Token: 0x06006262 RID: 25186 RVA: 0x002E20C8 File Offset: 0x002E02C8
		private void OnEnable()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.Pause();
			GameControl.eventManager.AddListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.OnCombatSecond), null, null, true, false);
			this.combatTimeText.SetText("00:00");
			this.TimePipsList.SetListSize<PipListItemController>(TemplateManager.global.combatLayerSpeedSettings.Count - 1, false, false);
		}

		// Token: 0x06006263 RID: 25187 RVA: 0x002E2133 File Offset: 0x002E0333
		private void OnDisable()
		{
			GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.OnCombatSecond), null);
		}

		// Token: 0x06006264 RID: 25188 RVA: 0x002E214C File Offset: 0x002E034C
		private void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.OnCombatSecond), null);
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x002E2165 File Offset: 0x002E0365
		public void Play()
		{
			this.gameTime.Play();
			this.SetSpeedString(this.gameTime.CurrentSpeedSetting);
			this.pauseButton.SetActive(true);
			this.playButton.SetActive(false);
		}

		// Token: 0x06006266 RID: 25190 RVA: 0x002E219B File Offset: 0x002E039B
		public void Pause()
		{
			this.gameTime.Pause();
			this.SetSpeedString(this.gameTime.CurrentSpeedSetting);
			this.pauseButton.SetActive(false);
			this.playButton.SetActive(true);
		}

		// Token: 0x06006267 RID: 25191 RVA: 0x002E21D1 File Offset: 0x002E03D1
		public void TogglePause()
		{
			if (this.gameTime.Paused)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_UnPause", false, false);
				this.Play();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_Pause", false, false);
			this.Pause();
		}

		// Token: 0x06006268 RID: 25192 RVA: 0x002E2205 File Offset: 0x002E0405
		public void PauseNoToggle()
		{
			if (!this.gameTime.Paused)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_Pause", false, false);
				this.Pause();
			}
		}

		// Token: 0x06006269 RID: 25193 RVA: 0x002E2226 File Offset: 0x002E0426
		public void IncreaseSpeed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SpeedUp", false, false);
			this.gameTime.IncreaseSpeed();
			this.SetSpeedString(this.gameTime.CurrentSpeedSetting);
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x002E2251 File Offset: 0x002E0451
		public void DecreaseSpeed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SlowDown", false, false);
			this.gameTime.DecreaseSpeed();
			this.SetSpeedString(this.gameTime.CurrentSpeedSetting);
		}

		// Token: 0x0600626B RID: 25195 RVA: 0x002E227C File Offset: 0x002E047C
		public void SetSpeed(int speedIndex)
		{
			this.gameTime.SetSpeed(speedIndex, false);
			this.SetSpeedString(this.gameTime.CurrentSpeedSetting);
		}

		// Token: 0x0600626C RID: 25196 RVA: 0x002E229C File Offset: 0x002E049C
		private void SetSpeedString(SpeedSetting speed)
		{
			this.speedText.SetText(speed.description);
			int num = 1;
			using (IEnumerator<object> enumerator = this.TimePipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatSpeedController.<>o__21.<>p__0 == null)
					{
						SpaceCombatSpeedController.<>o__21.<>p__0 = CallSite<Func<CallSite, object, PipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PipListItemController), typeof(SpaceCombatSpeedController)));
					}
					PipListItemController pipListItemController = SpaceCombatSpeedController.<>o__21.<>p__0.Target(SpaceCombatSpeedController.<>o__21.<>p__0, enumerator.Current);
					if (speed.multiplier != 0f)
					{
						if (this.gameTime.currentSpeedIndex < num)
						{
							pipListItemController.SetPipStatus(false, false);
						}
						else
						{
							pipListItemController.SetPipStatus(true, false);
						}
					}
					else if (this.gameTime.lastSpeedIndex < num)
					{
						pipListItemController.SetPipStatus(false, false);
					}
					else
					{
						pipListItemController.SetPipStatus(true, false);
					}
					num++;
				}
			}
		}

		// Token: 0x0600626D RID: 25197 RVA: 0x002E2390 File Offset: 0x002E0590
		private void OnCombatSecond(CombatSecond e)
		{
			this.UpdateClock();
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x002E2398 File Offset: 0x002E0598
		private void UpdateClock()
		{
			this.combatTimeText.SetText(this.ToTimeString((int)GameControl.spaceCombat.combatDuration_s));
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x002E23B6 File Offset: 0x002E05B6
		public void UpdateClockDisplay()
		{
			this.SetSpeedString(this.gameTime.CurrentSpeedSetting);
		}

		// Token: 0x06006270 RID: 25200 RVA: 0x002E23CC File Offset: 0x002E05CC
		private string ToTimeString(int seconds)
		{
			int num = seconds / 60;
			seconds %= 60;
			if (num >= 60)
			{
				int num2 = num / 60;
				num %= 60;
				return string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", num2, num, seconds);
			}
			return string.Format("{0,2:D2}:{1,2:D2}", num, seconds);
		}

		// Token: 0x04004524 RID: 17700
		public GameObject pauseButton;

		// Token: 0x04004525 RID: 17701
		public GameObject playButton;

		// Token: 0x04004526 RID: 17702
		[SerializeField]
		private TMP_Text combatTimeText;

		// Token: 0x04004527 RID: 17703
		[SerializeField]
		private TMP_Text speedText;

		// Token: 0x04004528 RID: 17704
		[SerializeField]
		private TMP_Text speedPausedText;

		// Token: 0x04004529 RID: 17705
		public ListManagerBase TimePipsList;

		// Token: 0x0400452A RID: 17706
		private GameTimeManager gameTime;
	}
}
