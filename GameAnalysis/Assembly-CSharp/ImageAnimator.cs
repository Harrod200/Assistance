using System;
using System.Collections;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000440 RID: 1088
public class ImageAnimator : MonoBehaviour
{
	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001697 RID: 5783 RVA: 0x00073831 File Offset: 0x00071A31
	// (set) Token: 0x06001698 RID: 5784 RVA: 0x00073839 File Offset: 0x00071A39
	public bool isPlaying { get; private set; }

	// Token: 0x06001699 RID: 5785 RVA: 0x00073842 File Offset: 0x00071A42
	public void SetSpriteSheet(string path, float animationSpeed = 0.1f)
	{
		this.sprites = Resources.LoadAll<Sprite>(path);
		this.animationSpeed = animationSpeed;
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x00073857 File Offset: 0x00071A57
	private IEnumerator LoopAnimation()
	{
		if (this.sprites.Length == 0)
		{
			this.stopFlag = true;
			this.isPlaying = false;
			base.StopCoroutine(this.LoopAnimation());
			Log.Debug("Animation missing sprite sheet", Array.Empty<object>());
			yield return null;
		}
		while (!this.stopFlag)
		{
			int num;
			for (int i = 0; i < this.sprites.Length; i = num + 1)
			{
				this.image.sprite = this.sprites[i];
				yield return new WaitForSecondsRealtime(this.animationSpeed);
				num = i;
			}
		}
		yield break;
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x00073866 File Offset: 0x00071A66
	public int SpriteCount()
	{
		return this.sprites.Length;
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x00073870 File Offset: 0x00071A70
	public void Play()
	{
		this.stopFlag = false;
		this.isPlaying = true;
		base.StartCoroutine(this.LoopAnimation());
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x0007388D File Offset: 0x00071A8D
	public void Stop()
	{
		this.stopFlag = true;
		base.StopCoroutine(this.LoopAnimation());
		this.isPlaying = false;
	}

	// Token: 0x040014E1 RID: 5345
	public Image image;

	// Token: 0x040014E2 RID: 5346
	private Sprite[] sprites;

	// Token: 0x040014E3 RID: 5347
	public float animationSpeed = 0.1f;

	// Token: 0x040014E4 RID: 5348
	private bool stopFlag;
}
