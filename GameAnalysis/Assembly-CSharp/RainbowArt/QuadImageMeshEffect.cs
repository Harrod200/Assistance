using System;
using UnityEngine;
using UnityEngine.UI;

namespace RainbowArt
{
	// Token: 0x0200054B RID: 1355
	public class QuadImageMeshEffect : BaseMeshEffect
	{
		// Token: 0x060022BE RID: 8894 RVA: 0x000B425C File Offset: 0x000B245C
		protected QuadImageMeshEffect()
		{
			this.from = new Vector2[]
			{
				new Vector2(1f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 1f)
			};
			this.to = new Vector2[]
			{
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f)
			};
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x000B4338 File Offset: 0x000B2538
		public override void ModifyMesh(VertexHelper vh)
		{
			if (vh.currentVertCount != 4)
			{
				return;
			}
			UIVertex uivertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref uivertex, i);
				Vector3 position = uivertex.position;
				Vector2 vector = Vector2.Lerp(this.from[i], this.to[i], this.curValue);
				uivertex.position = new Vector3(position.x * vector.x, position.y * vector.y, position.z);
				vh.SetUIVertex(uivertex, i);
			}
		}

		// Token: 0x04001A5C RID: 6748
		public Vector2[] from;

		// Token: 0x04001A5D RID: 6749
		public Vector2[] to;

		// Token: 0x04001A5E RID: 6750
		public float curValue;
	}
}
