using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x02000A00 RID: 2560
	public class FleetFormationPositionMarkerController : MonoBehaviour
	{
		// Token: 0x0600625D RID: 25181 RVA: 0x002E1F9C File Offset: 0x002E019C
		public void Initialize(TISpaceFleetState fleetState)
		{
			this.renderer.sprite = fleetState.icon;
		}

		// Token: 0x0600625E RID: 25182 RVA: 0x002E1FB0 File Offset: 0x002E01B0
		private void Update()
		{
			if (this.mainCamT == null)
			{
				this.mainCamT = GameControl.spaceCombat.mainCameraTransform;
			}
			base.transform.LookAt(this.mainCamT.position);
			this.minSizeDisSqr = this.minSizeDistance * this.minSizeDistance;
			this.maxSizeDisSqr = this.maxSizeDistance * this.maxSizeDistance;
			float num = ((this.mainCamT.position - base.transform.position).sqrMagnitude - this.minSizeDisSqr) / (this.maxSizeDisSqr - this.minSizeDisSqr);
			float num2 = Mathf.Lerp(this.minDisplaySize, this.maxDisplaySize, num);
			base.transform.localScale = new Vector3(num2, num2, 1f);
		}

		// Token: 0x0400451C RID: 17692
		public SpriteRenderer renderer;

		// Token: 0x0400451D RID: 17693
		public float minSizeDistance = 5f;

		// Token: 0x0400451E RID: 17694
		public float maxSizeDistance = 100f;

		// Token: 0x0400451F RID: 17695
		public float minDisplaySize = 1f;

		// Token: 0x04004520 RID: 17696
		public float maxDisplaySize = 3f;

		// Token: 0x04004521 RID: 17697
		private Transform mainCamT;

		// Token: 0x04004522 RID: 17698
		private float minSizeDisSqr;

		// Token: 0x04004523 RID: 17699
		private float maxSizeDisSqr;
	}
}
