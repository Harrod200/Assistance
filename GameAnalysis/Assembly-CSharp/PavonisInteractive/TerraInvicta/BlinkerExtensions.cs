using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006EA RID: 1770
	public static class BlinkerExtensions
	{
		// Token: 0x06002933 RID: 10547 RVA: 0x000DBC77 File Offset: 0x000D9E77
		public static void Blink(this Image image, float blinkingDuration, int blinkCount, Color blinkColor)
		{
			ImageBlinker.Blink(image, blinkingDuration, blinkCount, blinkColor);
		}
	}
}
