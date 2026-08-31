using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x020004A9 RID: 1193
	[AddComponentMenu("Vectrosity/VisibilityControlStatic")]
	public class VisibilityControlStatic : MonoBehaviour
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001AD1 RID: 6865 RVA: 0x00091564 File Offset: 0x0008F764
		public RefInt objectNumber
		{
			get
			{
				return this.m_objectNumber;
			}
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x0009156C File Offset: 0x0008F76C
		public void Setup(VectorLine line, bool makeBounds)
		{
			if (makeBounds)
			{
				VectorManager.SetupBoundsMesh(base.gameObject, line);
			}
			this.m_originalMatrix = base.transform.localToWorldMatrix;
			List<Vector3> list = new List<Vector3>(line.points3);
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = this.m_originalMatrix.MultiplyPoint3x4(list[i]);
			}
			line.points3 = list;
			this.m_vectorLine = line;
			VectorManager.VisibilityStaticSetup(line, out this.m_objectNumber);
			base.StartCoroutine(this.WaitCheck());
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x000915F6 File Offset: 0x0008F7F6
		private IEnumerator WaitCheck()
		{
			VectorManager.DrawArrayLine(this.m_objectNumber.i);
			yield return null;
			yield return null;
			if (!base.GetComponent<Renderer>().isVisible)
			{
				this.m_vectorLine.active = false;
			}
			yield break;
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00091605 File Offset: 0x0008F805
		private void OnBecameVisible()
		{
			this.m_vectorLine.active = true;
			VectorManager.DrawArrayLine(this.m_objectNumber.i);
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00091623 File Offset: 0x0008F823
		private void OnBecameInvisible()
		{
			this.m_vectorLine.active = false;
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00091631 File Offset: 0x0008F831
		private void OnDestroy()
		{
			if (this.m_destroyed)
			{
				return;
			}
			this.m_destroyed = true;
			VectorManager.VisibilityStaticRemove(this.m_objectNumber.i);
			if (this.m_dontDestroyLine)
			{
				return;
			}
			VectorLine.Destroy(ref this.m_vectorLine);
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x00091667 File Offset: 0x0008F867
		public void DontDestroyLine()
		{
			this.m_dontDestroyLine = true;
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x00091670 File Offset: 0x0008F870
		public Matrix4x4 GetMatrix()
		{
			return this.m_originalMatrix;
		}

		// Token: 0x040016DB RID: 5851
		private RefInt m_objectNumber;

		// Token: 0x040016DC RID: 5852
		private VectorLine m_vectorLine;

		// Token: 0x040016DD RID: 5853
		private bool m_destroyed;

		// Token: 0x040016DE RID: 5854
		private bool m_dontDestroyLine;

		// Token: 0x040016DF RID: 5855
		private Matrix4x4 m_originalMatrix;
	}
}
