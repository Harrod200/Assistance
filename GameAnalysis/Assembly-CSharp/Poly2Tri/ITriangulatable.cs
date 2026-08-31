using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004CD RID: 1229
	public interface ITriangulatable
	{
		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06001C03 RID: 7171
		IList<DelaunayTriangle> Triangles { get; }

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06001C04 RID: 7172
		TriangulationMode TriangulationMode { get; }

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06001C05 RID: 7173
		// (set) Token: 0x06001C06 RID: 7174
		string FileName { get; set; }

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06001C07 RID: 7175
		// (set) Token: 0x06001C08 RID: 7176
		bool DisplayFlipX { get; set; }

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06001C09 RID: 7177
		// (set) Token: 0x06001C0A RID: 7178
		bool DisplayFlipY { get; set; }

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06001C0B RID: 7179
		// (set) Token: 0x06001C0C RID: 7180
		float DisplayRotate { get; set; }

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06001C0D RID: 7181
		// (set) Token: 0x06001C0E RID: 7182
		double Precision { get; set; }

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06001C0F RID: 7183
		double MinX { get; }

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06001C10 RID: 7184
		double MaxX { get; }

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06001C11 RID: 7185
		double MinY { get; }

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06001C12 RID: 7186
		double MaxY { get; }

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06001C13 RID: 7187
		Rect2D Bounds { get; }

		// Token: 0x06001C14 RID: 7188
		void Prepare(TriangulationContext tcx);

		// Token: 0x06001C15 RID: 7189
		void AddTriangle(DelaunayTriangle t);

		// Token: 0x06001C16 RID: 7190
		void AddTriangles(IEnumerable<DelaunayTriangle> list);

		// Token: 0x06001C17 RID: 7191
		void ClearTriangles();
	}
}
