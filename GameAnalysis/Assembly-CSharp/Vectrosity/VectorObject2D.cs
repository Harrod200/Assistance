using System;
using UnityEngine;
using UnityEngine.UI;

namespace Vectrosity
{
	// Token: 0x020004A4 RID: 1188
	[Serializable]
	public class VectorObject2D : RawImage, IVectorObject
	{
		// Token: 0x06001A9A RID: 6810 RVA: 0x00090D0A File Offset: 0x0008EF0A
		public void SetVectorLine(VectorLine vectorLine, Texture tex, Material mat, bool useCustomMaterial)
		{
			this.vectorLine = vectorLine;
			this.SetTexture(tex);
			this.SetMaterial(mat);
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x00090D21 File Offset: 0x0008EF21
		public void Destroy()
		{
			global::UnityEngine.Object.Destroy(this.m_mesh);
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x00090D2E File Offset: 0x0008EF2E
		public void DestroyNow()
		{
			global::UnityEngine.Object.DestroyImmediate(this.m_mesh);
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x00090D3B File Offset: 0x0008EF3B
		public void Enable(bool enable)
		{
			if (this == null)
			{
				return;
			}
			base.enabled = enable;
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x00090D4E File Offset: 0x0008EF4E
		public void SetTexture(Texture tex)
		{
			base.texture = tex;
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x00090D57 File Offset: 0x0008EF57
		public void SetMaterial(Material mat)
		{
			this.material = mat;
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x00090D60 File Offset: 0x0008EF60
		protected override void UpdateGeometry()
		{
			if (this.m_mesh == null)
			{
				this.SetupMesh();
			}
			if (base.rectTransform != null && base.rectTransform.rect.width >= 0f && base.rectTransform.rect.height >= 0f)
			{
				this.OnPopulateMesh(VectorObject2D.vertexHelper);
			}
			base.canvasRenderer.SetMesh(this.m_mesh);
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x00090DDF File Offset: 0x0008EFDF
		private void SetupMesh()
		{
			this.m_mesh = new Mesh();
			this.m_mesh.name = this.vectorLine.name;
			this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
			this.SetMeshBounds();
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x00090E18 File Offset: 0x0008F018
		private void SetMeshBounds()
		{
			if (this.m_mesh != null)
			{
				this.m_mesh.bounds = new Bounds(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f), new Vector3((float)Screen.width, (float)Screen.height, 0f));
			}
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x00090E74 File Offset: 0x0008F074
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_updateVerts)
			{
				this.m_mesh.vertices = this.vectorLine.lineVertices;
				this.m_updateVerts = false;
			}
			if (this.m_updateUVs)
			{
				if (this.vectorLine.lineUVs.Length == this.m_mesh.vertexCount)
				{
					this.m_mesh.uv = this.vectorLine.lineUVs;
				}
				this.m_updateUVs = false;
			}
			if (this.m_updateColors)
			{
				if (this.vectorLine.lineColors.Length == this.m_mesh.vertexCount)
				{
					this.m_mesh.colors = this.vectorLine.lineColors;
				}
				this.m_updateColors = false;
			}
			if (this.m_updateTris)
			{
				this.m_mesh.SetTriangles(this.vectorLine.lineTriangles, 0);
				this.m_updateTris = false;
				this.SetMeshBounds();
			}
			if (this.m_updateNormals && this.m_mesh != null)
			{
				this.m_mesh.RecalculateNormals();
				this.m_updateNormals = false;
				this.UpdateGeometry();
			}
			if (this.m_updateTangents && this.m_mesh != null)
			{
				this.m_mesh.tangents = this.vectorLine.CalculateTangents(this.m_mesh.normals);
				this.m_updateTangents = false;
			}
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x00090FBC File Offset: 0x0008F1BC
		public void SetName(string name)
		{
			if (this.m_mesh == null)
			{
				return;
			}
			this.m_mesh.name = name;
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x00090FD9 File Offset: 0x0008F1D9
		public void UpdateVerts()
		{
			this.m_updateVerts = true;
			this.SetVerticesDirty();
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x00090FE8 File Offset: 0x0008F1E8
		public void UpdateUVs()
		{
			this.m_updateUVs = true;
			this.SetVerticesDirty();
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x00090FF7 File Offset: 0x0008F1F7
		public void UpdateColors()
		{
			this.m_updateColors = true;
			this.SetVerticesDirty();
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x00091006 File Offset: 0x0008F206
		public void UpdateNormals()
		{
			this.m_updateNormals = true;
			this.SetVerticesDirty();
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x00091015 File Offset: 0x0008F215
		public void UpdateTangents()
		{
			this.m_updateTangents = true;
			this.SetVerticesDirty();
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x00091024 File Offset: 0x0008F224
		public void UpdateTris()
		{
			this.m_updateTris = true;
			this.SetVerticesDirty();
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x00091034 File Offset: 0x0008F234
		public void UpdateMeshAttributes()
		{
			if (this.m_mesh != null)
			{
				this.m_mesh.Clear();
			}
			this.m_updateVerts = true;
			this.m_updateUVs = true;
			this.m_updateColors = true;
			this.m_updateTris = true;
			this.SetVerticesDirty();
			this.SetMeshBounds();
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x00091082 File Offset: 0x0008F282
		public void ClearMesh()
		{
			if (this.m_mesh == null)
			{
				return;
			}
			this.m_mesh.Clear();
			this.UpdateGeometry();
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x000910A4 File Offset: 0x0008F2A4
		public int VertexCount()
		{
			return this.m_mesh.vertexCount;
		}

		// Token: 0x040016BE RID: 5822
		private bool m_updateVerts = true;

		// Token: 0x040016BF RID: 5823
		private bool m_updateUVs = true;

		// Token: 0x040016C0 RID: 5824
		private bool m_updateColors = true;

		// Token: 0x040016C1 RID: 5825
		private bool m_updateNormals;

		// Token: 0x040016C2 RID: 5826
		private bool m_updateTangents;

		// Token: 0x040016C3 RID: 5827
		private bool m_updateTris = true;

		// Token: 0x040016C4 RID: 5828
		private Mesh m_mesh;

		// Token: 0x040016C5 RID: 5829
		public VectorLine vectorLine;

		// Token: 0x040016C6 RID: 5830
		private static VertexHelper vertexHelper;
	}
}
