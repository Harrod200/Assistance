using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005AE RID: 1454
	[CreateAssetMenu(menuName = "TerraInvicta/VFX/TextureArray")]
	public class TextureArray : ScriptableObject
	{
		// Token: 0x04001D5F RID: 7519
		public List<Texture2D> Images = new List<Texture2D>();

		// Token: 0x04001D60 RID: 7520
		public bool isLinear;
	}
}
