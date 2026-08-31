using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x020004A5 RID: 1189
	public class VectorObject3D : MonoBehaviour, IVectorObject
	{
		// Token: 0x06001AB0 RID: 6832 RVA: 0x000910D8 File Offset: 0x0008F2D8
		public void SetVectorLine(VectorLine vectorLine, Texture tex, Material mat, bool useCustomMaterial)
		{
			base.gameObject.AddComponent<MeshRenderer>();
			base.gameObject.AddComponent<MeshFilter>();
			this.m_vectorLine = vectorLine;
			this.m_material = mat;
			this.m_material.mainTexture = tex;
			base.GetComponent<MeshRenderer>().sharedMaterial = this.m_material;
			this.m_useCustomMaterial = useCustomMaterial;
			this.SetupMesh();
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x00091136 File Offset: 0x0008F336
		public void Destroy()
		{
			global::UnityEngine.Object.Destroy(this.m_mesh);
			if (!this.m_useCustomMaterial)
			{
				global::UnityEngine.Object.Destroy(this.m_material);
			}
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x00091156 File Offset: 0x0008F356
		public void Enable(bool enable)
		{
			if (this == null)
			{
				return;
			}
			base.GetComponent<MeshRenderer>().enabled = enable;
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x0009116E File Offset: 0x0008F36E
		public void SetTexture(Texture tex)
		{
			base.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x00091181 File Offset: 0x0008F381
		public void SetMaterial(Material mat)
		{
			this.m_material = mat;
			this.m_useCustomMaterial = true;
			base.GetComponent<MeshRenderer>().sharedMaterial = mat;
			if (mat != null)
			{
				base.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = this.m_vectorLine.texture;
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x000911C4 File Offset: 0x0008F3C4
		private void SetupMesh()
		{
			this.m_mesh = new Mesh();
			this.m_mesh.name = this.m_vectorLine.name;
			this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
			base.GetComponent<MeshFilter>().mesh = this.m_mesh;
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x00091210 File Offset: 0x0008F410
		private void LateUpdate()
		{
			if (this.m_updateVerts)
			{
				this.SetVerts();
			}
			if (this.m_updateUVs)
			{
				if (this.m_vectorLine.lineUVs.Length == this.m_mesh.vertexCount)
				{
					this.m_mesh.uv = this.m_vectorLine.lineUVs;
				}
				this.m_updateUVs = false;
			}
			if (this.m_updateColors)
			{
				if (this.m_vectorLine.lineColors.Length == this.m_mesh.vertexCount)
				{
					this.m_mesh.colors = this.m_vectorLine.lineColors;
				}
				this.m_updateColors = false;
			}
			if (this.m_updateTris)
			{
				this.m_mesh.SetTriangles(this.m_vectorLine.lineTriangles, 0);
				this.m_updateTris = false;
			}
			if (this.m_updateNormals)
			{
				this.m_mesh.RecalculateNormals();
				this.m_updateNormals = false;
			}
			if (this.m_updateTangents)
			{
				this.m_mesh.tangents = this.m_vectorLine.CalculateTangents(this.m_mesh.normals);
				this.m_updateTangents = false;
			}
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x00091319 File Offset: 0x0008F519
		private void SetVerts()
		{
			this.m_mesh.vertices = this.m_vectorLine.lineVertices;
			this.m_updateVerts = false;
			this.m_mesh.RecalculateBounds();
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x00091343 File Offset: 0x0008F543
		public void SetName(string name)
		{
			if (this.m_mesh == null)
			{
				return;
			}
			this.m_mesh.name = name;
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x00091360 File Offset: 0x0008F560
		public void UpdateVerts()
		{
			this.m_updateVerts = true;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x00091369 File Offset: 0x0008F569
		public void UpdateUVs()
		{
			this.m_updateUVs = true;
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x00091372 File Offset: 0x0008F572
		public void UpdateColors()
		{
			this.m_updateColors = true;
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x0009137B File Offset: 0x0008F57B
		public void UpdateNormals()
		{
			this.m_updateNormals = true;
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x00091384 File Offset: 0x0008F584
		public void UpdateTangents()
		{
			this.m_updateTangents = true;
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x0009138D File Offset: 0x0008F58D
		public void UpdateTris()
		{
			this.m_updateTris = true;
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00091396 File Offset: 0x0008F596
		public void UpdateMeshAttributes()
		{
			this.m_mesh.Clear();
			this.m_updateVerts = true;
			this.m_updateUVs = true;
			this.m_updateColors = true;
			this.m_updateTris = true;
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x000913BF File Offset: 0x0008F5BF
		public void ClearMesh()
		{
			if (this.m_mesh == null)
			{
				return;
			}
			this.m_mesh.Clear();
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x000913DB File Offset: 0x0008F5DB
		public int VertexCount()
		{
			return this.m_mesh.vertexCount;
		}

		// Token: 0x040016C7 RID: 5831
		private bool m_updateVerts = true;

		// Token: 0x040016C8 RID: 5832
		private bool m_updateUVs = true;

		// Token: 0x040016C9 RID: 5833
		private bool m_updateColors = true;

		// Token: 0x040016CA RID: 5834
		private bool m_updateNormals;

		// Token: 0x040016CB RID: 5835
		private bool m_updateTangents;

		// Token: 0x040016CC RID: 5836
		private bool m_updateTris = true;

		// Token: 0x040016CD RID: 5837
		private Mesh m_mesh;

		// Token: 0x040016CE RID: 5838
		private VectorLine m_vectorLine;

		// Token: 0x040016CF RID: 5839
		private Material m_material;

		// Token: 0x040016D0 RID: 5840
		private bool m_useCustomMaterial;
	}
}
