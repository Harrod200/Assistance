using System;

namespace Poly2Tri
{
	// Token: 0x020004E4 RID: 1252
	public class TriangulationUtil
	{
		// Token: 0x06001D44 RID: 7492 RVA: 0x0009AF2C File Offset: 0x0009912C
		public static bool SmartIncircle(Point2D pa, Point2D pb, Point2D pc, Point2D pd)
		{
			double x = pd.X;
			double y = pd.Y;
			double num = pa.X - x;
			double num2 = pa.Y - y;
			double num3 = pb.X - x;
			double num4 = pb.Y - y;
			double num5 = num * num4;
			double num6 = num3 * num2;
			double num7 = num5 - num6;
			if (num7 <= 0.0)
			{
				return false;
			}
			double num8 = pc.X - x;
			double num9 = pc.Y - y;
			double num10 = num8 * num2;
			double num11 = num * num9;
			double num12 = num10 - num11;
			if (num12 <= 0.0)
			{
				return false;
			}
			double num13 = num3 * num9;
			double num14 = num8 * num4;
			double num15 = num * num + num2 * num2;
			double num16 = num3 * num3 + num4 * num4;
			double num17 = num8 * num8 + num9 * num9;
			return num15 * (num13 - num14) + num16 * num12 + num17 * num7 > 0.0;
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x0009B00C File Offset: 0x0009920C
		public static bool InScanArea(Point2D pa, Point2D pb, Point2D pc, Point2D pd)
		{
			double x = pd.X;
			double y = pd.Y;
			double num = pa.X - x;
			double num2 = pa.Y - y;
			double num3 = pb.X - x;
			double num4 = pb.Y - y;
			double num5 = num * num4;
			double num6 = num3 * num2;
			if (num5 - num6 <= 0.0)
			{
				return false;
			}
			double num7 = pc.X - x;
			double num8 = pc.Y - y;
			double num9 = num7 * num2;
			double num10 = num * num8;
			return num9 - num10 > 0.0;
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x0009B094 File Offset: 0x00099294
		public static Orientation Orient2d(Point2D pa, Point2D pb, Point2D pc)
		{
			double num = (pa.X - pc.X) * (pb.Y - pc.Y);
			double num2 = (pa.Y - pc.Y) * (pb.X - pc.X);
			double num3 = num - num2;
			if (num3 > -MathUtil.EPSILON && num3 < MathUtil.EPSILON)
			{
				return Orientation.Collinear;
			}
			if (num3 > 0.0)
			{
				return Orientation.CCW;
			}
			return Orientation.CW;
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x0009B0FD File Offset: 0x000992FD
		public static bool PointInBoundingBox(double xmin, double xmax, double ymin, double ymax, Point2D p)
		{
			return p.X > xmin && p.X < xmax && p.Y > ymin && p.Y < ymax;
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x0009B129 File Offset: 0x00099329
		public static bool PointOnLineSegment2D(Point2D lineStart, Point2D lineEnd, Point2D p, double epsilon)
		{
			return TriangulationUtil.PointOnLineSegment2D(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, p.X, p.Y, epsilon);
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x0009B158 File Offset: 0x00099358
		public static bool PointOnLineSegment2D(double x1, double y1, double x2, double y2, double x, double y, double epsilon)
		{
			if (!MathUtil.IsValueBetween(x, x1, x2, epsilon) || !MathUtil.IsValueBetween(y, y1, y2, epsilon))
			{
				return false;
			}
			if (MathUtil.AreValuesEqual(x2 - x1, 0.0, epsilon))
			{
				return true;
			}
			double num = (y2 - y1) / (x2 - x1);
			double num2 = -(num * x1) + y1;
			return MathUtil.AreValuesEqual(y - (num * x + num2), 0.0, epsilon);
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x0009B1C0 File Offset: 0x000993C0
		public static bool RectsIntersect(Rect2D r1, Rect2D r2)
		{
			return r1.Right > r2.Left && r1.Left < r2.Right && r1.Bottom > r2.Top && r1.Top < r2.Bottom;
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x0009B1FC File Offset: 0x000993FC
		public static bool LinesIntersect2D(Point2D ptStart0, Point2D ptEnd0, Point2D ptStart1, Point2D ptEnd1, bool firstIsSegment, bool secondIsSegment, bool coincidentEndPointCollisions, ref Point2D pIntersectionPt, double epsilon)
		{
			double num = (ptEnd0.X - ptStart0.X) * (ptStart1.Y - ptEnd1.Y) - (ptStart1.X - ptEnd1.X) * (ptEnd0.Y - ptStart0.Y);
			if (Math.Abs(num) < epsilon)
			{
				return false;
			}
			double num2 = (ptStart1.X - ptStart0.X) * (ptStart1.Y - ptEnd1.Y) - (ptStart1.X - ptEnd1.X) * (ptStart1.Y - ptStart0.Y);
			double num3 = (ptEnd0.X - ptStart0.X) * (ptStart1.Y - ptStart0.Y) - (ptStart1.X - ptStart0.X) * (ptEnd0.Y - ptStart0.Y);
			double num4 = 1.0 / num;
			double num5 = num2 * num4;
			double num6 = num3 * num4;
			if ((!firstIsSegment || (num5 >= 0.0 && num5 <= 1.0)) && (!secondIsSegment || (num6 >= 0.0 && num6 <= 1.0)) && (coincidentEndPointCollisions || (!MathUtil.AreValuesEqual(0.0, num5, epsilon) && !MathUtil.AreValuesEqual(0.0, num6, epsilon))))
			{
				if (pIntersectionPt != null)
				{
					pIntersectionPt.X = ptStart0.X + num5 * (ptEnd0.X - ptStart0.X);
					pIntersectionPt.Y = ptStart0.Y + num5 * (ptEnd0.Y - ptStart0.Y);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x0009B380 File Offset: 0x00099580
		public static bool LinesIntersect2D(Point2D ptStart0, Point2D ptEnd0, Point2D ptStart1, Point2D ptEnd1, ref Point2D pIntersectionPt, double epsilon)
		{
			return TriangulationUtil.LinesIntersect2D(ptStart0, ptEnd0, ptStart1, ptEnd1, true, true, false, ref pIntersectionPt, epsilon);
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x0009B39D File Offset: 0x0009959D
		public static double LI2DDotProduct(Point2D v0, Point2D v1)
		{
			return v0.X * v1.X + v0.Y * v1.Y;
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x0009B3BC File Offset: 0x000995BC
		public static bool RaysIntersect2D(Point2D ptRayOrigin0, Point2D ptRayVector0, Point2D ptRayOrigin1, Point2D ptRayVector1, ref Point2D ptIntersection)
		{
			double num = 0.01;
			if (ptIntersection == null)
			{
				return Math.Abs(ptRayVector1.X - ptRayVector0.X) > num && Math.Abs(ptRayVector1.Y - ptRayVector0.Y) > num;
			}
			Point2D point2D = new Point2D(ptRayOrigin1.X - ptRayOrigin0.X, ptRayOrigin1.Y - ptRayOrigin0.Y);
			Point2D point2D2 = new Point2D(-ptRayVector1.Y, ptRayVector1.X);
			double num2 = TriangulationUtil.LI2DDotProduct(ptRayVector0, point2D2);
			if (Math.Abs(num2) < num)
			{
				return false;
			}
			double num3 = TriangulationUtil.LI2DDotProduct(point2D, point2D2) / num2;
			ptIntersection.X = ptRayOrigin0.X + ptRayVector0.X * num3;
			ptIntersection.Y = ptRayOrigin0.Y + ptRayVector0.Y * num3;
			return true;
		}
	}
}
