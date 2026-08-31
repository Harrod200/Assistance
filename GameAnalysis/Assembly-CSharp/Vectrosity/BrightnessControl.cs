using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x02000496 RID: 1174
	[AddComponentMenu("Vectrosity/BrightnessControl")]
	public class BrightnessControl : MonoBehaviour
	{
		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001922 RID: 6434 RVA: 0x000818AF File Offset: 0x0007FAAF
		public RefInt objectNumber
		{
			get
			{
				return this.m_objectNumber;
			}
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x000818B8 File Offset: 0x0007FAB8
		public void Setup(VectorLine line, bool m_useLine)
		{
			this.m_objectNumber = new RefInt(0);
			VectorManager.CheckDistanceSetup(base.transform, line, line.color, this.m_objectNumber);
			VectorManager.SetDistanceColor(this.m_objectNumber.i);
			if (m_useLine)
			{
				this.m_useLine = true;
				this.m_vectorLine = line;
			}
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x0008190A File Offset: 0x0007FB0A
		public void SetUseLine(bool useLine)
		{
			this.m_useLine = useLine;
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x00081913 File Offset: 0x0007FB13
		private void OnBecameVisible()
		{
			VectorManager.SetOldDistance(this.m_objectNumber.i, -1);
			VectorManager.SetDistanceColor(this.m_objectNumber.i);
			if (!this.m_useLine)
			{
				return;
			}
			this.m_vectorLine.active = true;
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x0008194B File Offset: 0x0007FB4B
		public void OnBecameInvisible()
		{
			if (!this.m_useLine)
			{
				return;
			}
			this.m_vectorLine.active = false;
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x00081962 File Offset: 0x0007FB62
		private void OnDestroy()
		{
			if (this.m_destroyed)
			{
				return;
			}
			this.m_destroyed = true;
			VectorManager.DistanceRemove(this.m_objectNumber.i);
			if (this.m_useLine)
			{
				VectorLine.Destroy(ref this.m_vectorLine);
			}
		}

		// Token: 0x04001630 RID: 5680
		private RefInt m_objectNumber;

		// Token: 0x04001631 RID: 5681
		private VectorLine m_vectorLine;

		// Token: 0x04001632 RID: 5682
		private bool m_useLine;

		// Token: 0x04001633 RID: 5683
		private bool m_destroyed;
	}
}
