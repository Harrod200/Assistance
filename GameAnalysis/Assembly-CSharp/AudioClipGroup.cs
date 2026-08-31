using System;
using UnityEngine;

// Token: 0x02000422 RID: 1058
[CreateAssetMenu(menuName = "TerraInvicta/Sounds/Audio Clip Group")]
public class AudioClipGroup : AbstractAudioClip
{
	// Token: 0x06001613 RID: 5651 RVA: 0x00070574 File Offset: 0x0006E774
	public override void InitSourceWithClip(AudioSource source)
	{
		int num = this.clips.Length;
		int num2 = this.variableClips.Length;
		int num3 = Mathf.FloorToInt((float)global::UnityEngine.Random.Range(0, num + num2));
		if (num3 < num)
		{
			source.clip = this.clips[num3];
			return;
		}
		this.variableClips[num3 - num].InitSourceWithClip(source);
	}

	// Token: 0x0400141B RID: 5147
	public AudioClip[] clips;

	// Token: 0x0400141C RID: 5148
	public VariableAudioClip[] variableClips;
}
