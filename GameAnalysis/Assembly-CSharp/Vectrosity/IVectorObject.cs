using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x02000498 RID: 1176
	internal interface IVectorObject
	{
		// Token: 0x0600192A RID: 6442
		void SetName(string name);

		// Token: 0x0600192B RID: 6443
		void UpdateVerts();

		// Token: 0x0600192C RID: 6444
		void UpdateUVs();

		// Token: 0x0600192D RID: 6445
		void UpdateColors();

		// Token: 0x0600192E RID: 6446
		void UpdateTris();

		// Token: 0x0600192F RID: 6447
		void UpdateNormals();

		// Token: 0x06001930 RID: 6448
		void UpdateTangents();

		// Token: 0x06001931 RID: 6449
		void UpdateMeshAttributes();

		// Token: 0x06001932 RID: 6450
		void ClearMesh();

		// Token: 0x06001933 RID: 6451
		void SetMaterial(Material material);

		// Token: 0x06001934 RID: 6452
		void SetTexture(Texture texture);

		// Token: 0x06001935 RID: 6453
		void Enable(bool enable);

		// Token: 0x06001936 RID: 6454
		void SetVectorLine(VectorLine vectorLine, Texture texture, Material material, bool useCustomMaterial);

		// Token: 0x06001937 RID: 6455
		void Destroy();

		// Token: 0x06001938 RID: 6456
		int VertexCount();
	}
}
