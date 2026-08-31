using System;

namespace Poly2Tri
{
	// Token: 0x020004C1 RID: 1217
	public static class P2T
	{
		// Token: 0x06001B79 RID: 7033 RVA: 0x00094180 File Offset: 0x00092380
		public static void Triangulate(PolygonSet ps)
		{
			foreach (Polygon polygon in ps.Polygons)
			{
				P2T.Triangulate(polygon);
			}
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x000941CC File Offset: 0x000923CC
		public static void Triangulate(Polygon p)
		{
			P2T.Triangulate(P2T._defaultAlgorithm, p);
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x000941D9 File Offset: 0x000923D9
		public static void Triangulate(ConstrainedPointSet cps)
		{
			P2T.Triangulate(P2T._defaultAlgorithm, cps);
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x000941E6 File Offset: 0x000923E6
		public static void Triangulate(PointSet ps)
		{
			P2T.Triangulate(P2T._defaultAlgorithm, ps);
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x000941F3 File Offset: 0x000923F3
		public static TriangulationContext CreateContext(TriangulationAlgorithm algorithm)
		{
			return new DTSweepContext();
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x000941FC File Offset: 0x000923FC
		public static void Triangulate(TriangulationAlgorithm algorithm, ITriangulatable t)
		{
			TriangulationContext triangulationContext = P2T.CreateContext(algorithm);
			triangulationContext.PrepareTriangulation(t);
			P2T.Triangulate(triangulationContext);
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x00094210 File Offset: 0x00092410
		public static void Triangulate(TriangulationContext tcx)
		{
			TriangulationAlgorithm algorithm = tcx.Algorithm;
			DTSweep.Triangulate((DTSweepContext)tcx);
		}

		// Token: 0x04001759 RID: 5977
		private static TriangulationAlgorithm _defaultAlgorithm;
	}
}
