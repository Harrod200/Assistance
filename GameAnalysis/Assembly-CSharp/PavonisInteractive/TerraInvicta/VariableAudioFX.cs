using System;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B3 RID: 1459
	public class VariableAudioFX : MonoBehaviour
	{
		// Token: 0x0600278E RID: 10126 RVA: 0x000D84EC File Offset: 0x000D66EC
		private void Start()
		{
			if (this.playOnAwake)
			{
				if (this.variableSFXGroup.variableSFX.Length != 0)
				{
					this.index = global::UnityEngine.Random.Range(0, this.variableSFXGroup.variableSFX.Length - 1);
					this.eventPath = this.variableSFXGroup.variableSFX[this.index].variableEventPath;
				}
				if (this.eventPath != null)
				{
					if (!this.eventInstance.isValid())
					{
						this.eventInstance = AudioManager.CreateFMODInstance(this.eventPath);
					}
					if (TIGlobalValuesState.isSpaceCombatEnabled)
					{
						this.eventInstance.SetDistance(AudioManager.GetCombatAudioMaxDistance(this.eventInstance), 1f);
					}
					this.eventInstance.setPitch(this.variableSFXGroup.variableSFX[this.index].eventPitch + global::UnityEngine.Random.Range(-this.variableSFXGroup.variableSFX[this.index].eventPitchVariance, this.variableSFXGroup.variableSFX[this.index].eventPitchVariance));
					this.eventInstance.SetVolume(this.variableSFXGroup.variableSFX[this.index].eventVolume + global::UnityEngine.Random.Range(-this.variableSFXGroup.variableSFX[this.index].eventVolumeVariance, this.variableSFXGroup.variableSFX[this.index].eventVolumeVariance));
					this.eventInstance.Play(base.gameObject);
					this.eventInstance.Release();
				}
			}
		}

		// Token: 0x04001D69 RID: 7529
		[Header("Variable Group SFX")]
		public VariableSFXGroup variableSFXGroup;

		// Token: 0x04001D6A RID: 7530
		private int index;

		// Token: 0x04001D6B RID: 7531
		public bool playOnAwake;

		// Token: 0x04001D6C RID: 7532
		public string eventPath;

		// Token: 0x04001D6D RID: 7533
		public EventInstance eventInstance;

		// Token: 0x02000D09 RID: 3337
		[Serializable]
		public class VariableSFX
		{
			// Token: 0x04005043 RID: 20547
			public string variableEventPath;

			// Token: 0x04005044 RID: 20548
			[Range(0f, 1f)]
			public float eventVolume = 1f;

			// Token: 0x04005045 RID: 20549
			[Range(0f, 1f)]
			public float eventVolumeVariance;

			// Token: 0x04005046 RID: 20550
			[Range(0.01f, 3f)]
			public float eventPitch = 1f;

			// Token: 0x04005047 RID: 20551
			[Range(0f, 3f)]
			public float eventPitchVariance;
		}
	}
}
