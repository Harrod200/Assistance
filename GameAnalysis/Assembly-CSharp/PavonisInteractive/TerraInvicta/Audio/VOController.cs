using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009DC RID: 2524
	public class VOController : MonoBehaviour
	{
		// Token: 0x17001041 RID: 4161
		// (get) Token: 0x06005EBD RID: 24253 RVA: 0x002CE77F File Offset: 0x002CC97F
		public static VOController Instance
		{
			get
			{
				return VOController._instance;
			}
		}

		// Token: 0x06005EBE RID: 24254 RVA: 0x002CE786 File Offset: 0x002CC986
		private void Awake()
		{
			if (VOController._instance != null && VOController._instance != this)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			VOController._instance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(this);
		}

		// Token: 0x06005EBF RID: 24255 RVA: 0x002CE7BC File Offset: 0x002CC9BC
		public void AddVOToQueue(EventInstance eventInstance, bool onEarth)
		{
			this.eventInstanceList.Add(eventInstance);
			if (this.VOQueue != null)
			{
				if (this.eventInstanceList.Count == 1)
				{
					base.StopCoroutine(this.VOQueue);
					this.VOQueue = base.StartCoroutine(this.AddToQueue(eventInstance, onEarth));
					return;
				}
			}
			else
			{
				this.VOQueue = base.StartCoroutine(this.AddToQueue(eventInstance, onEarth));
			}
		}

		// Token: 0x06005EC0 RID: 24256 RVA: 0x002CE820 File Offset: 0x002CCA20
		public void AddVOToQueue(string eventPath, bool onEarth)
		{
			if (AudioManager.VerifyPath(eventPath, true))
			{
				this.AddVOToQueue(AudioManager.CreateFMODInstance(eventPath), onEarth);
			}
		}

		// Token: 0x06005EC1 RID: 24257 RVA: 0x002CE838 File Offset: 0x002CCA38
		private IEnumerator AddToQueue(EventInstance eventInstance, bool onEarth)
		{
			while (this.eventInstanceList.Count > 0)
			{
				if (!this.eventInstanceList[0].IsStopped())
				{
					while (!this.eventInstanceList[0].IsPlaying())
					{
						yield return new WaitForSeconds(0.1f);
					}
					yield return new WaitForSeconds((float)this.eventInstanceList[0].GetLength() / 1000f);
					this.instanceToRelease = this.eventInstanceList[0];
					this.eventInstanceList.Remove(this.eventInstanceList[0]);
					this.instanceToRelease.Release();
					if (this.radioProcessingEarth.isValid())
					{
						this.radioProcessingEarth.Stop(STOP_MODE.IMMEDIATE);
					}
					if (this.radioProcessingSpace.isValid())
					{
						this.radioProcessingSpace.Stop(STOP_MODE.IMMEDIATE);
					}
				}
				else
				{
					if (!this.radioProcessingEarth.isValid())
					{
						this.radioProcessingEarth = AudioManager.CreateFMODInstance("snapshot:/Voice_Processsing_EARTH_ON");
					}
					if (!this.radioProcessingSpace.isValid())
					{
						this.radioProcessingSpace = AudioManager.CreateFMODInstance("snapshot:/Voice_Processsing_SPACE_ON");
					}
					if (onEarth)
					{
						if (this.radioProcessingEarth.isValid())
						{
							this.radioProcessingEarth.Play();
						}
					}
					else if (this.radioProcessingSpace.isValid())
					{
						this.radioProcessingSpace.Play();
					}
					this.eventInstanceList[0].Play();
				}
			}
			yield break;
		}

		// Token: 0x04004393 RID: 17299
		private List<EventInstance> eventInstanceList = new List<EventInstance>();

		// Token: 0x04004394 RID: 17300
		private EventInstance radioProcessingEarth;

		// Token: 0x04004395 RID: 17301
		private EventInstance radioProcessingSpace;

		// Token: 0x04004396 RID: 17302
		private Coroutine VOQueue;

		// Token: 0x04004397 RID: 17303
		private static VOController _instance;

		// Token: 0x04004398 RID: 17304
		private EventInstance instanceToRelease;
	}
}
