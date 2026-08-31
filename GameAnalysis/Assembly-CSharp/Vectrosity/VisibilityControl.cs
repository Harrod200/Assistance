using System;
using System.Collections;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x020004A7 RID: 1191
	[AddComponentMenu("Vectrosity/VisibilityControl")]
	public class VisibilityControl : MonoBehaviour
	{
		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x0009141C File Offset: 0x0008F61C
		public RefInt objectNumber
		{
			get
			{
				return this.m_objectNumber;
			}
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x00091424 File Offset: 0x0008F624
		public void Setup(VectorLine line, bool makeBounds)
		{
			if (makeBounds)
			{
				VectorManager.SetupBoundsMesh(base.gameObject, line);
			}
			VectorManager.VisibilitySetup(base.transform, line, out this.m_objectNumber);
			this.m_vectorLine = line;
			VectorManager.DrawArrayLine2(this.m_objectNumber.i);
			base.StartCoroutine(this.VisibilityTest());
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x00091476 File Offset: 0x0008F676
		private IEnumerator VisibilityTest()
		{
			yield return null;
			yield return null;
			if (!base.GetComponent<Renderer>().isVisible)
			{
				this.m_vectorLine.active = false;
			}
			yield break;
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00091485 File Offset: 0x0008F685
		private IEnumerator OnBecameVisible()
		{
			yield return new WaitForEndOfFrame();
			this.m_vectorLine.active = true;
			yield break;
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x00091494 File Offset: 0x0008F694
		private IEnumerator OnBecameInvisible()
		{
			yield return new WaitForEndOfFrame();
			this.m_vectorLine.active = false;
			yield break;
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x000914A3 File Offset: 0x0008F6A3
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

		// Token: 0x06001ACA RID: 6858 RVA: 0x000914D9 File Offset: 0x0008F6D9
		public void DontDestroyLine()
		{
			this.m_dontDestroyLine = true;
		}

		// Token: 0x040016D3 RID: 5843
		private RefInt m_objectNumber;

		// Token: 0x040016D4 RID: 5844
		private VectorLine m_vectorLine;

		// Token: 0x040016D5 RID: 5845
		private bool m_destroyed;

		// Token: 0x040016D6 RID: 5846
		private bool m_dontDestroyLine;
	}
}
