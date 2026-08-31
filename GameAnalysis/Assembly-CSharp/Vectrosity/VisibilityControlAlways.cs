using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x020004A8 RID: 1192
	[AddComponentMenu("Vectrosity/VisibilityControlAlways")]
	public class VisibilityControlAlways : MonoBehaviour
	{
		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001ACC RID: 6860 RVA: 0x000914EA File Offset: 0x0008F6EA
		public RefInt objectNumber
		{
			get
			{
				return this.m_objectNumber;
			}
		}

		// Token: 0x06001ACD RID: 6861 RVA: 0x000914F2 File Offset: 0x0008F6F2
		public void Setup(VectorLine line)
		{
			VectorManager.VisibilitySetup(base.transform, line, out this.m_objectNumber);
			VectorManager.DrawArrayLine2(this.m_objectNumber.i);
			this.m_vectorLine = line;
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x0009151D File Offset: 0x0008F71D
		private void OnDestroy()
		{
			if (this.m_destroyed)
			{
				return;
			}
			this.m_destroyed = true;
			VectorManager.VisibilityRemove(this.m_objectNumber.i);
			if (this.m_dontDestroyLine)
			{
				return;
			}
			VectorLine.Destroy(ref this.m_vectorLine);
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x00091553 File Offset: 0x0008F753
		public void DontDestroyLine()
		{
			this.m_dontDestroyLine = true;
		}

		// Token: 0x040016D7 RID: 5847
		private RefInt m_objectNumber;

		// Token: 0x040016D8 RID: 5848
		private VectorLine m_vectorLine;

		// Token: 0x040016D9 RID: 5849
		private bool m_destroyed;

		// Token: 0x040016DA RID: 5850
		private bool m_dontDestroyLine;
	}
}
