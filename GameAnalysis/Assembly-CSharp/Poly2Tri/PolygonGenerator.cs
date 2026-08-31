using System;

namespace Poly2Tri
{
	// Token: 0x020004E3 RID: 1251
	public class PolygonGenerator
	{
		// Token: 0x06001D40 RID: 7488 RVA: 0x0009ACE4 File Offset: 0x00098EE4
		public static Polygon RandomCircleSweep(double scale, int vertexCount)
		{
			double num = scale / 4.0;
			PolygonPoint[] array = new PolygonPoint[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				do
				{
					if (i % 250 == 0)
					{
						num += scale / 2.0 * (0.5 - PolygonGenerator.RNG.NextDouble());
					}
					else if (i % 50 == 0)
					{
						num += scale / 5.0 * (0.5 - PolygonGenerator.RNG.NextDouble());
					}
					else
					{
						num += 25.0 * scale / (double)vertexCount * (0.5 - PolygonGenerator.RNG.NextDouble());
					}
					num = ((num > scale / 2.0) ? (scale / 2.0) : num);
					num = ((num < scale / 10.0) ? (scale / 10.0) : num);
				}
				while (num < scale / 10.0 || num > scale / 2.0);
				PolygonPoint polygonPoint = new PolygonPoint(num * Math.Cos(PolygonGenerator.PI_2 * (double)i / (double)vertexCount), num * Math.Sin(PolygonGenerator.PI_2 * (double)i / (double)vertexCount));
				array[i] = polygonPoint;
			}
			return new Polygon(array);
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x0009AE28 File Offset: 0x00099028
		public static Polygon RandomCircleSweep2(double scale, int vertexCount)
		{
			double num = scale / 4.0;
			PolygonPoint[] array = new PolygonPoint[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				do
				{
					num += scale / 5.0 * (0.5 - PolygonGenerator.RNG.NextDouble());
					num = ((num > scale / 2.0) ? (scale / 2.0) : num);
					num = ((num < scale / 10.0) ? (scale / 10.0) : num);
				}
				while (num < scale / 10.0 || num > scale / 2.0);
				PolygonPoint polygonPoint = new PolygonPoint(num * Math.Cos(PolygonGenerator.PI_2 * (double)i / (double)vertexCount), num * Math.Sin(PolygonGenerator.PI_2 * (double)i / (double)vertexCount));
				array[i] = polygonPoint;
			}
			return new Polygon(array);
		}

		// Token: 0x040017C3 RID: 6083
		private static readonly Random RNG = new Random();

		// Token: 0x040017C4 RID: 6084
		private static double PI_2 = 6.283185307179586;
	}
}
