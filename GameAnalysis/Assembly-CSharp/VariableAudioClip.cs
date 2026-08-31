using System;
using UnityEngine;

// Token: 0x02000423 RID: 1059
[CreateAssetMenu(menuName = "TerraInvicta/Sounds/Variable Audio Clip")]
public class VariableAudioClip : AbstractAudioClip
{
	// Token: 0x06001615 RID: 5653 RVA: 0x000705D0 File Offset: 0x0006E7D0
	public override void InitSourceWithClip(AudioSource source)
	{
		source.clip = this.clip;
		source.volume = this.volume + global::UnityEngine.Random.Range(-this.volumeVariance, this.volumeVariance);
		source.pitch = this.pitch + global::UnityEngine.Random.Range(-this.pitchVariance, this.pitchVariance);
		if (this.lowPassFilter)
		{
			AudioLowPassFilter audioLowPassFilter = source.gameObject.AddComponent<AudioLowPassFilter>();
			if (audioLowPassFilter != null)
			{
				audioLowPassFilter.cutoffFrequency = this.lowPassFrequency + global::UnityEngine.Random.Range(-this.lowPassVariance, this.lowPassVariance);
			}
		}
	}

	// Token: 0x0400141D RID: 5149
	public AudioClip clip;

	// Token: 0x0400141E RID: 5150
	[Range(0.01f, 3f)]
	public float pitch = 1f;

	// Token: 0x0400141F RID: 5151
	[Range(0f, 3f)]
	public float pitchVariance;

	// Token: 0x04001420 RID: 5152
	[Range(0f, 1f)]
	public float volume = 1f;

	// Token: 0x04001421 RID: 5153
	[Range(0f, 1f)]
	public float volumeVariance;

	// Token: 0x04001422 RID: 5154
	public bool lowPassFilter;

	// Token: 0x04001423 RID: 5155
	[Range(0f, 22000f)]
	public float lowPassFrequency = 5000f;

	// Token: 0x04001424 RID: 5156
	[Range(0f, 22000f)]
	public float lowPassVariance;
}
