using System;
using UnityEngine;

// Token: 0x02000424 RID: 1060
[RequireComponent(typeof(AudioSource))]
public class VariableAudioSource : MonoBehaviour
{
	// Token: 0x06001617 RID: 5655 RVA: 0x0007068C File Offset: 0x0006E88C
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.InitSource();
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x000706A0 File Offset: 0x0006E8A0
	private void Start()
	{
		if (this.playOnAwake)
		{
			this.source.Play();
		}
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000706B5 File Offset: 0x0006E8B5
	public void InitSource()
	{
		if (this.clip != null)
		{
			this.clip.InitSourceWithClip(base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x04001425 RID: 5157
	[SerializeField]
	private AbstractAudioClip clip;

	// Token: 0x04001426 RID: 5158
	[SerializeField]
	private bool playOnAwake;

	// Token: 0x04001427 RID: 5159
	private AudioSource source;
}
