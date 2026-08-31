using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000862 RID: 2146
	public class PipListItemController : MonoBehaviour
	{
		// Token: 0x06004FA0 RID: 20384 RVA: 0x00226670 File Offset: 0x00224870
		public void SetPipStatus(bool filled, bool secondaryColor = false)
		{
			Color color = (secondaryColor ? this.filledSecondaryColor : this.filledColor);
			this.pipImage.color = (filled ? color : this.unfilledColor);
		}

		// Token: 0x04003303 RID: 13059
		public Image pipImage;

		// Token: 0x04003304 RID: 13060
		public Color unfilledColor = new Color(0.2627451f, 0.35686275f, 0.43137255f);

		// Token: 0x04003305 RID: 13061
		public Color filledColor = new Color(0.8f, 0.69803923f, 0.43137255f);

		// Token: 0x04003306 RID: 13062
		public Color filledSecondaryColor = new Color(0.8f, 0.69803923f, 0.43137255f);
	}
}
