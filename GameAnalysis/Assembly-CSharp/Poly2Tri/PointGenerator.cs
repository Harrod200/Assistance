using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004E2 RID: 1250
	public class PointGenerator
	{
		// Token: 0x06001D3C RID: 7484 RVA: 0x0009AC08 File Offset: 0x00098E08
		public static List<TriangulationPoint> UniformDistribution(int n, double scale)
		{
			List<TriangulationPoint> list = new List<TriangulationPoint>();
			for (int i = 0; i < n; i++)
			{
				list.Add(new TriangulationPoint(scale * (0.5 - PointGenerator.RNG.NextDouble()), scale * (0.5 - PointGenerator.RNG.NextDouble())));
			}
			return list;
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x0009AC60 File Offset: 0x00098E60
		public static List<TriangulationPoint> UniformGrid(int n, double scale)
		{
			double num = scale / (double)n;
			double num2 = 0.5 * scale;
			List<TriangulationPoint> list = new List<TriangulationPoint>();
			for (int i = 0; i < n + 1; i++)
			{
				double num3 = num2 - (double)i * num;
				for (int j = 0; j < n + 1; j++)
				{
					list.Add(new TriangulationPoint(num3, num2 - (double)j * num));
				}
			}
			return list;
		}

		// Token: 0x040017C2 RID: 6082
		private static readonly Random RNG = new Random();
	}
}
