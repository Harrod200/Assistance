using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F8 RID: 2296
	public class UITextStyle : MonoBehaviour
	{
		// Token: 0x060057F7 RID: 22519 RVA: 0x00285994 File Offset: 0x00283B94
		private void OnValidate()
		{
		}

		// Token: 0x04003F7B RID: 16251
		[Tooltip("Very Light Heading: Main UI panel headers\r\n\r\nStandard Heading: Less prominent headers and active buttons\r\n\r\nBody Text: Standard body text\r\n\r\nDimmed Text: ")]
		public UITextStyle.TextStyle textStyle = UITextStyle.TextStyle.BodyText;

		// Token: 0x04003F7C RID: 16252
		public static readonly Color32[] UITextColors = new Color32[]
		{
			new Color32(207, 231, 232, byte.MaxValue),
			new Color32(175, 209, 224, byte.MaxValue),
			new Color32(155, 185, 199, byte.MaxValue),
			new Color32(108, 129, 139, byte.MaxValue)
		};

		// Token: 0x020011E7 RID: 4583
		public enum TextStyle
		{
			// Token: 0x04006889 RID: 26761
			VeryLightHeading,
			// Token: 0x0400688A RID: 26762
			StandardHeading,
			// Token: 0x0400688B RID: 26763
			BodyText,
			// Token: 0x0400688C RID: 26764
			DimmedText
		}
	}
}
