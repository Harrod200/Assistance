using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E9 RID: 1769
	[RequireComponent(typeof(Image))]
	public class ImageBlinker : MonoBehaviour
	{
		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x0600292E RID: 10542 RVA: 0x000DBB40 File Offset: 0x000D9D40
		private Color EffectiveRestColor
		{
			get
			{
				Color color = Color.Lerp(this.restColor, this.blinkColor, 1f - this.restColor.a);
				color.a = this.restColor.a;
				return color;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x0600292F RID: 10543 RVA: 0x000DBB83 File Offset: 0x000D9D83
		public Image Image
		{
			get
			{
				return base.GetComponent<Image>();
			}
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x000DBB8C File Offset: 0x000D9D8C
		public void Update()
		{
			this.secondsElapsed += Time.deltaTime;
			if (this.secondsElapsed >= this.blinkingDuration)
			{
				this.Image.color = this.restColor;
				global::UnityEngine.Object.Destroy(this);
				return;
			}
			float num = this.secondsElapsed / this.blinkingDuration * (float)this.blinkCount * 2f;
			float num2 = num - (float)((int)num);
			Color effectiveRestColor = this.EffectiveRestColor;
			Color effectiveRestColor2 = this.blinkColor;
			if ((int)num % 2 == 1)
			{
				effectiveRestColor = this.blinkColor;
				effectiveRestColor2 = this.EffectiveRestColor;
			}
			this.Image.color = Color.Lerp(effectiveRestColor, effectiveRestColor2, num2);
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x000DBC27 File Offset: 0x000D9E27
		public static void Blink(Image image, float blinkingDuration, int blinkCount, Color blinkColor)
		{
			if (image.HasComponent<ImageBlinker>())
			{
				return;
			}
			ImageBlinker imageBlinker = image.gameObject.AddComponent<ImageBlinker>();
			imageBlinker.blinkingDuration = blinkingDuration;
			imageBlinker.blinkCount = blinkCount;
			imageBlinker.restColor = image.color;
			imageBlinker.blinkColor = blinkColor;
		}

		// Token: 0x04001F8D RID: 8077
		private float secondsElapsed;

		// Token: 0x04001F8E RID: 8078
		private float blinkingDuration = 1f;

		// Token: 0x04001F8F RID: 8079
		private int blinkCount = 2;

		// Token: 0x04001F90 RID: 8080
		private Color restColor;

		// Token: 0x04001F91 RID: 8081
		private Color blinkColor;
	}
}
