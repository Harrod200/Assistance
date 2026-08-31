using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004D3 RID: 1235
	public class PolygonUtil
	{
		// Token: 0x06001C84 RID: 7300 RVA: 0x000979F4 File Offset: 0x00095BF4
		public static Point2DList.WindingOrderType CalculateWindingOrder(IList<Point2D> l)
		{
			double num = 0.0;
			for (int i = 0; i < l.Count; i++)
			{
				int num2 = (i + 1) % l.Count;
				num += l[i].X * l[num2].Y;
				num -= l[i].Y * l[num2].X;
			}
			num /= 2.0;
			if (num < 0.0)
			{
				return Point2DList.WindingOrderType.CW;
			}
			if (num > 0.0)
			{
				return Point2DList.WindingOrderType.CCW;
			}
			return Point2DList.WindingOrderType.Unknown;
		}

		// Token: 0x06001C85 RID: 7301 RVA: 0x00097A88 File Offset: 0x00095C88
		public static bool PolygonsAreSame2D(IList<Point2D> poly1, IList<Point2D> poly2)
		{
			int count = poly1.Count;
			int count2 = poly2.Count;
			if (count != count2)
			{
				return false;
			}
			Point2D point2D = new Point2D(0.0, 0.0);
			for (int i = 0; i < count2; i++)
			{
				point2D.Set(poly1[0]);
				point2D.Subtract(poly2[i]);
				if (point2D.MagnitudeSquared() < 0.0001)
				{
					int num = i;
					bool flag = false;
					for (;;)
					{
						bool flag2 = true;
						int j = 1;
						while (j < count)
						{
							if (!flag)
							{
								i++;
							}
							else
							{
								i--;
								if (i < 0)
								{
									i = count2 - 1;
								}
							}
							point2D.Set(poly1[j]);
							point2D.Subtract(poly2[i % count2]);
							if (point2D.MagnitudeSquared() >= 0.0001)
							{
								if (flag)
								{
									return false;
								}
								i = num;
								flag = true;
								flag2 = false;
								break;
							}
							else
							{
								j++;
							}
						}
						if (flag2)
						{
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x00097B74 File Offset: 0x00095D74
		public static bool PointInPolygon2D(IList<Point2D> polygon, Point2D p)
		{
			if (polygon == null || polygon.Count < 3)
			{
				return false;
			}
			int count = polygon.Count;
			Point2D point2D = polygon[count - 1];
			bool flag = point2D.Y >= p.Y;
			bool flag2 = false;
			for (int i = 0; i < count; i++)
			{
				Point2D point2D2 = polygon[i];
				bool flag3 = point2D2.Y >= p.Y;
				if (flag != flag3 && (point2D2.Y - p.Y) * (point2D.X - point2D2.X) >= (point2D2.X - p.X) * (point2D.Y - point2D2.Y) == flag3)
				{
					flag2 = !flag2;
				}
				flag = flag3;
				point2D = point2D2;
			}
			return flag2;
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x00097C38 File Offset: 0x00095E38
		public static bool PolygonsIntersect2D(IList<Point2D> poly1, Rect2D boundRect1, IList<Point2D> poly2, Rect2D boundRect2)
		{
			if (poly1 == null || poly1.Count < 3 || boundRect1 == null || poly2 == null || poly2.Count < 3 || boundRect2 == null)
			{
				return false;
			}
			if (!boundRect1.Intersects(boundRect2))
			{
				return false;
			}
			double num = Math.Max(Math.Min(boundRect1.Width, boundRect2.Width) * 0.0010000000474974513, MathUtil.EPSILON);
			int count = poly1.Count;
			int count2 = poly2.Count;
			for (int i = 0; i < count; i++)
			{
				int num2 = i + 1;
				if (num2 == count)
				{
					num2 = 0;
				}
				for (int j = 0; j < count2; j++)
				{
					int num3 = j + 1;
					if (num3 == count2)
					{
						num3 = 0;
					}
					Point2D point2D = null;
					if (TriangulationUtil.LinesIntersect2D(poly1[i], poly1[num2], poly2[j], poly2[num3], ref point2D, num))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x00097D0A File Offset: 0x00095F0A
		public bool PolygonContainsPolygon(IList<Point2D> poly1, Rect2D boundRect1, IList<Point2D> poly2, Rect2D boundRect2)
		{
			return PolygonUtil.PolygonContainsPolygon(poly1, boundRect1, poly2, boundRect2, true);
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x00097D18 File Offset: 0x00095F18
		public static bool PolygonContainsPolygon(IList<Point2D> poly1, Rect2D boundRect1, IList<Point2D> poly2, Rect2D boundRect2, bool runIntersectionTest)
		{
			if (poly1 == null || poly1.Count < 3 || poly2 == null || poly2.Count < 3)
			{
				return false;
			}
			if (runIntersectionTest)
			{
				if (poly1.Count == poly2.Count && PolygonUtil.PolygonsAreSame2D(poly1, poly2))
				{
					return false;
				}
				if (PolygonUtil.PolygonsIntersect2D(poly1, boundRect1, poly2, boundRect2))
				{
					return false;
				}
			}
			return PolygonUtil.PointInPolygon2D(poly1, poly2[0]);
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x00097D7C File Offset: 0x00095F7C
		public static void ClipPolygonToEdge2D(Point2D edgeBegin, Point2D edgeEnd, IList<Point2D> poly, out List<Point2D> outPoly)
		{
			outPoly = null;
			if (edgeBegin == null || edgeEnd == null || poly == null || poly.Count < 3)
			{
				return;
			}
			outPoly = new List<Point2D>();
			int num = poly.Count - 1;
			Point2D point2D = new Point2D(edgeEnd.X - edgeBegin.X, edgeEnd.Y - edgeBegin.Y);
			bool flag = TriangulationUtil.Orient2d(edgeBegin, edgeEnd, poly[num]) == Orientation.CW;
			Point2D point2D2 = new Point2D(0.0, 0.0);
			for (int i = 0; i < poly.Count; i++)
			{
				int num2 = ((TriangulationUtil.Orient2d(edgeBegin, edgeEnd, poly[i]) == Orientation.CW) ? 1 : 0);
				if (num2 != 0)
				{
					if (flag)
					{
						outPoly.Add(poly[i]);
					}
					else
					{
						point2D2.Set(poly[i].X - poly[num].X, poly[i].Y - poly[num].Y);
						Point2D point2D3 = new Point2D(0.0, 0.0);
						if (TriangulationUtil.RaysIntersect2D(poly[num], point2D2, edgeBegin, point2D, ref point2D3))
						{
							outPoly.Add(point2D3);
							outPoly.Add(poly[i]);
						}
					}
				}
				else if (flag)
				{
					point2D2.Set(poly[i].X - poly[num].X, poly[i].Y - poly[num].Y);
					Point2D point2D4 = new Point2D(0.0, 0.0);
					if (TriangulationUtil.RaysIntersect2D(poly[num], point2D2, edgeBegin, point2D, ref point2D4))
					{
						outPoly.Add(point2D4);
					}
				}
				num = i;
				flag = num2 != 0;
			}
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x00097F44 File Offset: 0x00096144
		public static void ClipPolygonToPolygon(IList<Point2D> poly, IList<Point2D> clipPoly, out List<Point2D> outPoly)
		{
			outPoly = null;
			if (poly == null || poly.Count < 3 || clipPoly == null || clipPoly.Count < 3)
			{
				return;
			}
			outPoly = new List<Point2D>(poly);
			int count = clipPoly.Count;
			int num = count - 1;
			for (int i = 0; i < count; i++)
			{
				List<Point2D> list = null;
				Point2D point2D = clipPoly[num];
				Point2D point2D2 = clipPoly[i];
				PolygonUtil.ClipPolygonToEdge2D(point2D, point2D2, outPoly, out list);
				outPoly.Clear();
				outPoly.AddRange(list);
				num = i;
			}
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x00097FBC File Offset: 0x000961BC
		public static PolygonUtil.PolyUnionError PolygonUnion(Point2DList polygon1, Point2DList polygon2, out Point2DList union)
		{
			PolygonOperationContext polygonOperationContext = new PolygonOperationContext();
			polygonOperationContext.Init(PolygonUtil.PolyOperation.Union, polygon1, polygon2);
			PolygonUtil.PolygonUnionInternal(polygonOperationContext);
			union = polygonOperationContext.Union;
			return polygonOperationContext.mError;
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x00097FF0 File Offset: 0x000961F0
		protected static void PolygonUnionInternal(PolygonOperationContext ctx)
		{
			Point2DList union = ctx.Union;
			if (ctx.mStartingIndex == -1)
			{
				switch (ctx.mError)
				{
				case PolygonUtil.PolyUnionError.NoIntersections:
				case PolygonUtil.PolyUnionError.InfiniteLoop:
					return;
				case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
					union.AddRange(ctx.mOriginalPolygon2);
					return;
				}
			}
			Point2DList point2DList = ctx.mPoly1;
			Point2DList point2DList2 = ctx.mPoly2;
			List<int> list = ctx.mPoly1VectorAngles;
			Point2D point2D = ctx.mPoly1[ctx.mStartingIndex];
			int num = ctx.mStartingIndex;
			int num2 = -1;
			union.Clear();
			do
			{
				union.Add(point2DList[num]);
				foreach (EdgeIntersectInfo edgeIntersectInfo in ctx.mIntersections)
				{
					if (point2DList[num].Equals(edgeIntersectInfo.IntersectionPoint, point2DList.Epsilon))
					{
						int num3 = point2DList2.IndexOf(edgeIntersectInfo.IntersectionPoint);
						int num4 = point2DList2.NextIndex(num3);
						Point2D point2D2 = point2DList2[num4];
						bool flag;
						if (list[num4] == -1)
						{
							flag = ctx.PointInPolygonAngle(point2D2, point2DList);
							list[num4] = (flag ? 1 : 0);
						}
						else
						{
							flag = list[num4] == 1;
						}
						if (!flag)
						{
							if (point2DList == ctx.mPoly1)
							{
								point2DList = ctx.mPoly2;
								list = ctx.mPoly2VectorAngles;
								point2DList2 = ctx.mPoly1;
								if (num2 < 0)
								{
									num2 = num3;
								}
							}
							else
							{
								point2DList = ctx.mPoly1;
								list = ctx.mPoly1VectorAngles;
								point2DList2 = ctx.mPoly2;
							}
							num = num3;
							break;
						}
					}
				}
				num = point2DList.NextIndex(num);
				if (point2DList == ctx.mPoly1)
				{
					if (num == 0)
					{
						break;
					}
				}
				else if (num2 >= 0 && num == num2)
				{
					break;
				}
			}
			while (point2DList[num] != point2D && union.Count <= ctx.mPoly1.Count + ctx.mPoly2.Count);
			if (union.Count > ctx.mPoly1.Count + ctx.mPoly2.Count)
			{
				ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
			}
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x00098208 File Offset: 0x00096408
		public static PolygonUtil.PolyUnionError PolygonIntersect(Point2DList polygon1, Point2DList polygon2, out Point2DList intersectOut)
		{
			PolygonOperationContext polygonOperationContext = new PolygonOperationContext();
			polygonOperationContext.Init(PolygonUtil.PolyOperation.Intersect, polygon1, polygon2);
			PolygonUtil.PolygonIntersectInternal(polygonOperationContext);
			intersectOut = polygonOperationContext.Intersect;
			return polygonOperationContext.mError;
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x0009823C File Offset: 0x0009643C
		protected static void PolygonIntersectInternal(PolygonOperationContext ctx)
		{
			Point2DList intersect = ctx.Intersect;
			if (ctx.mStartingIndex == -1)
			{
				switch (ctx.mError)
				{
				case PolygonUtil.PolyUnionError.NoIntersections:
				case PolygonUtil.PolyUnionError.InfiniteLoop:
					return;
				case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
					intersect.AddRange(ctx.mOriginalPolygon2);
					return;
				}
			}
			Point2DList point2DList = ctx.mPoly1;
			Point2DList point2DList2 = ctx.mPoly2;
			List<int> list = ctx.mPoly1VectorAngles;
			int num = ctx.mPoly1.IndexOf(ctx.mIntersections[0].IntersectionPoint);
			Point2D point2D = ctx.mPoly1[num];
			int num2 = num;
			int num3 = -1;
			intersect.Clear();
			while (!intersect.Contains(point2DList[num]))
			{
				intersect.Add(point2DList[num]);
				foreach (EdgeIntersectInfo edgeIntersectInfo in ctx.mIntersections)
				{
					if (point2DList[num].Equals(edgeIntersectInfo.IntersectionPoint, point2DList.Epsilon))
					{
						int num4 = point2DList2.IndexOf(edgeIntersectInfo.IntersectionPoint);
						int num5 = point2DList2.NextIndex(num4);
						Point2D point2D2 = point2DList2[num5];
						bool flag;
						if (list[num5] == -1)
						{
							flag = ctx.PointInPolygonAngle(point2D2, point2DList);
							list[num5] = (flag ? 1 : 0);
						}
						else
						{
							flag = list[num5] == 1;
						}
						if (flag)
						{
							if (point2DList == ctx.mPoly1)
							{
								point2DList = ctx.mPoly2;
								list = ctx.mPoly2VectorAngles;
								point2DList2 = ctx.mPoly1;
								if (num3 < 0)
								{
									num3 = num4;
								}
							}
							else
							{
								point2DList = ctx.mPoly1;
								list = ctx.mPoly1VectorAngles;
								point2DList2 = ctx.mPoly2;
							}
							num = num4;
							break;
						}
					}
				}
				num = point2DList.NextIndex(num);
				if (point2DList == ctx.mPoly1)
				{
					if (num == num2)
					{
						break;
					}
				}
				else if (num3 >= 0 && num == num3)
				{
					break;
				}
				if (point2DList[num] == point2D || intersect.Count > ctx.mPoly1.Count + ctx.mPoly2.Count)
				{
					break;
				}
			}
			if (intersect.Count > ctx.mPoly1.Count + ctx.mPoly2.Count)
			{
				ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
			}
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x00098480 File Offset: 0x00096680
		public static PolygonUtil.PolyUnionError PolygonSubtract(Point2DList polygon1, Point2DList polygon2, out Point2DList subtract)
		{
			PolygonOperationContext polygonOperationContext = new PolygonOperationContext();
			polygonOperationContext.Init(PolygonUtil.PolyOperation.Subtract, polygon1, polygon2);
			PolygonUtil.PolygonSubtractInternal(polygonOperationContext);
			subtract = polygonOperationContext.Subtract;
			return polygonOperationContext.mError;
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x000984B4 File Offset: 0x000966B4
		public static void PolygonSubtractInternal(PolygonOperationContext ctx)
		{
			Point2DList subtract = ctx.Subtract;
			if (ctx.mStartingIndex == -1)
			{
				PolygonUtil.PolyUnionError mError = ctx.mError;
				if (mError - PolygonUtil.PolyUnionError.NoIntersections <= 2)
				{
					return;
				}
			}
			Point2DList point2DList = ctx.mPoly1;
			Point2DList point2DList2 = ctx.mPoly2;
			List<int> list = ctx.mPoly1VectorAngles;
			Point2D point2D = ctx.mPoly1[ctx.mStartingIndex];
			int num = ctx.mStartingIndex;
			subtract.Clear();
			bool flag = true;
			do
			{
				subtract.Add(point2DList[num]);
				foreach (EdgeIntersectInfo edgeIntersectInfo in ctx.mIntersections)
				{
					if (point2DList[num].Equals(edgeIntersectInfo.IntersectionPoint, point2DList.Epsilon))
					{
						int num2 = point2DList2.IndexOf(edgeIntersectInfo.IntersectionPoint);
						if (flag)
						{
							int num3 = point2DList2.PreviousIndex(num2);
							Point2D point2D2 = point2DList2[num3];
							bool flag2;
							if (list[num3] == -1)
							{
								flag2 = ctx.PointInPolygonAngle(point2D2, point2DList);
								list[num3] = (flag2 ? 1 : 0);
							}
							else
							{
								flag2 = list[num3] == 1;
							}
							if (flag2)
							{
								if (point2DList == ctx.mPoly1)
								{
									point2DList = ctx.mPoly2;
									list = ctx.mPoly2VectorAngles;
									point2DList2 = ctx.mPoly1;
								}
								else
								{
									point2DList = ctx.mPoly1;
									list = ctx.mPoly1VectorAngles;
									point2DList2 = ctx.mPoly2;
								}
								num = num2;
								flag = !flag;
								break;
							}
						}
						else
						{
							int num4 = point2DList2.NextIndex(num2);
							Point2D point2D3 = point2DList2[num4];
							bool flag3;
							if (list[num4] == -1)
							{
								flag3 = ctx.PointInPolygonAngle(point2D3, point2DList);
								list[num4] = (flag3 ? 1 : 0);
							}
							else
							{
								flag3 = list[num4] == 1;
							}
							if (!flag3)
							{
								if (point2DList == ctx.mPoly1)
								{
									point2DList = ctx.mPoly2;
									list = ctx.mPoly2VectorAngles;
									point2DList2 = ctx.mPoly1;
								}
								else
								{
									point2DList = ctx.mPoly1;
									list = ctx.mPoly1VectorAngles;
									point2DList2 = ctx.mPoly2;
								}
								num = num2;
								flag = !flag;
								break;
							}
						}
					}
				}
				if (flag)
				{
					num = point2DList.NextIndex(num);
				}
				else
				{
					num = point2DList.PreviousIndex(num);
				}
			}
			while (point2DList[num] != point2D && subtract.Count <= ctx.mPoly1.Count + ctx.mPoly2.Count);
			if (subtract.Count > ctx.mPoly1.Count + ctx.mPoly2.Count)
			{
				ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
			}
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x00098750 File Offset: 0x00096950
		public static PolygonUtil.PolyUnionError PolygonOperation(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2, out Dictionary<uint, Point2DList> results)
		{
			PolygonOperationContext polygonOperationContext = new PolygonOperationContext();
			polygonOperationContext.Init(operations, polygon1, polygon2);
			results = polygonOperationContext.mOutput;
			return PolygonUtil.PolygonOperation(polygonOperationContext);
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x0009877B File Offset: 0x0009697B
		public static PolygonUtil.PolyUnionError PolygonOperation(PolygonOperationContext ctx)
		{
			if ((ctx.mOperations & PolygonUtil.PolyOperation.Union) == PolygonUtil.PolyOperation.Union)
			{
				PolygonUtil.PolygonUnionInternal(ctx);
			}
			if ((ctx.mOperations & PolygonUtil.PolyOperation.Intersect) == PolygonUtil.PolyOperation.Intersect)
			{
				PolygonUtil.PolygonIntersectInternal(ctx);
			}
			if ((ctx.mOperations & PolygonUtil.PolyOperation.Subtract) == PolygonUtil.PolyOperation.Subtract)
			{
				PolygonUtil.PolygonSubtractInternal(ctx);
			}
			return ctx.mError;
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x000987B8 File Offset: 0x000969B8
		public static List<Point2DList> SplitComplexPolygon(Point2DList verts, double epsilon)
		{
			int count = verts.Count;
			List<SplitComplexPolygonNode> list = new List<SplitComplexPolygonNode>();
			for (int i = 0; i < verts.Count; i++)
			{
				SplitComplexPolygonNode splitComplexPolygonNode = new SplitComplexPolygonNode(new Point2D(verts[i].X, verts[i].Y));
				list.Add(splitComplexPolygonNode);
			}
			for (int j = 0; j < verts.Count; j++)
			{
				int num = ((j == count - 1) ? 0 : (j + 1));
				int num2 = ((j == 0) ? (count - 1) : (j - 1));
				list[j].AddConnection(list[num]);
				list[j].AddConnection(list[num2]);
			}
			int num3 = list.Count;
			bool flag = true;
			int num4 = 0;
			while (flag)
			{
				flag = false;
				int num5 = 0;
				while (!flag && num5 < num3)
				{
					int num6 = 0;
					while (!flag && num6 < list[num5].NumConnected)
					{
						int num7 = 0;
						while (!flag && num7 < num3)
						{
							if (num7 != num5 && !(list[num7] == list[num5][num6]))
							{
								int num8 = 0;
								while (!flag && num8 < list[num7].NumConnected)
								{
									if (!(list[num7][num8] == list[num5][num6]) && !(list[num7][num8] == list[num5]))
									{
										Point2D point2D = new Point2D();
										if (TriangulationUtil.LinesIntersect2D(list[num5].Position, list[num5][num6].Position, list[num7].Position, list[num7][num8].Position, true, true, true, ref point2D, epsilon))
										{
											flag = true;
											SplitComplexPolygonNode splitComplexPolygonNode2 = new SplitComplexPolygonNode(point2D);
											int num9 = list.IndexOf(splitComplexPolygonNode2);
											if (num9 >= 0 && num9 < list.Count)
											{
												splitComplexPolygonNode2 = list[num9];
											}
											else
											{
												list.Add(splitComplexPolygonNode2);
												num3 = list.Count;
											}
											SplitComplexPolygonNode splitComplexPolygonNode3 = list[num5];
											SplitComplexPolygonNode splitComplexPolygonNode4 = list[num5][num6];
											SplitComplexPolygonNode splitComplexPolygonNode5 = list[num7];
											SplitComplexPolygonNode splitComplexPolygonNode6 = list[num7][num8];
											splitComplexPolygonNode4.RemoveConnection(splitComplexPolygonNode3);
											splitComplexPolygonNode3.RemoveConnection(splitComplexPolygonNode4);
											splitComplexPolygonNode6.RemoveConnection(splitComplexPolygonNode5);
											splitComplexPolygonNode5.RemoveConnection(splitComplexPolygonNode6);
											if (!splitComplexPolygonNode2.Position.Equals(splitComplexPolygonNode3.Position, epsilon))
											{
												splitComplexPolygonNode2.AddConnection(splitComplexPolygonNode3);
												splitComplexPolygonNode3.AddConnection(splitComplexPolygonNode2);
											}
											if (!splitComplexPolygonNode2.Position.Equals(splitComplexPolygonNode5.Position, epsilon))
											{
												splitComplexPolygonNode2.AddConnection(splitComplexPolygonNode5);
												splitComplexPolygonNode5.AddConnection(splitComplexPolygonNode2);
											}
											if (!splitComplexPolygonNode2.Position.Equals(splitComplexPolygonNode4.Position, epsilon))
											{
												splitComplexPolygonNode2.AddConnection(splitComplexPolygonNode4);
												splitComplexPolygonNode4.AddConnection(splitComplexPolygonNode2);
											}
											if (!splitComplexPolygonNode2.Position.Equals(splitComplexPolygonNode6.Position, epsilon))
											{
												splitComplexPolygonNode2.AddConnection(splitComplexPolygonNode6);
												splitComplexPolygonNode6.AddConnection(splitComplexPolygonNode2);
											}
										}
									}
									num8++;
								}
							}
							num7++;
						}
						num6++;
					}
					num5++;
				}
				num4++;
			}
			bool flag2 = true;
			int num10 = num3;
			double num11 = epsilon * epsilon;
			while (flag2)
			{
				flag2 = false;
				for (int k = 0; k < num3; k++)
				{
					if (list[k].NumConnected != 0)
					{
						for (int l = k + 1; l < num3; l++)
						{
							if (list[l].NumConnected != 0 && (list[k].Position - list[l].Position).MagnitudeSquared() <= num11)
							{
								if (num10 <= 3)
								{
									throw new Exception("Eliminated so many duplicate points that resulting polygon has < 3 vertices!");
								}
								num10--;
								flag2 = true;
								SplitComplexPolygonNode splitComplexPolygonNode7 = list[k];
								SplitComplexPolygonNode splitComplexPolygonNode8 = list[l];
								int numConnected = splitComplexPolygonNode8.NumConnected;
								for (int m = 0; m < numConnected; m++)
								{
									SplitComplexPolygonNode splitComplexPolygonNode9 = splitComplexPolygonNode8[m];
									if (splitComplexPolygonNode9 != splitComplexPolygonNode7)
									{
										splitComplexPolygonNode7.AddConnection(splitComplexPolygonNode9);
										splitComplexPolygonNode9.AddConnection(splitComplexPolygonNode7);
									}
									splitComplexPolygonNode9.RemoveConnection(splitComplexPolygonNode8);
								}
								splitComplexPolygonNode8.ClearConnections();
								list.RemoveAt(l);
								num3--;
							}
						}
					}
				}
			}
			double num12 = double.MaxValue;
			double num13 = double.MinValue;
			int num14 = -1;
			for (int n = 0; n < num3; n++)
			{
				if (list[n].Position.Y < num12 && list[n].NumConnected > 1)
				{
					num12 = list[n].Position.Y;
					num14 = n;
					num13 = list[n].Position.X;
				}
				else if (list[n].Position.Y == num12 && list[n].Position.X > num13 && list[n].NumConnected > 1)
				{
					num14 = n;
					num13 = list[n].Position.X;
				}
			}
			Point2D point2D2 = new Point2D(1.0, 0.0);
			List<Point2D> list2 = new List<Point2D>();
			SplitComplexPolygonNode splitComplexPolygonNode10 = list[num14];
			SplitComplexPolygonNode splitComplexPolygonNode11 = splitComplexPolygonNode10;
			SplitComplexPolygonNode splitComplexPolygonNode12 = splitComplexPolygonNode10.GetRightestConnection(point2D2);
			if (splitComplexPolygonNode12 == null)
			{
				return PolygonUtil.SplitComplexPolygonCleanup(verts);
			}
			list2.Add(splitComplexPolygonNode11.Position);
			while (splitComplexPolygonNode12 != splitComplexPolygonNode11)
			{
				if (list2.Count > 4 * num3)
				{
					throw new Exception("nodes should never be visited four times apiece (proof?), so we've probably hit a loop...crap");
				}
				list2.Add(splitComplexPolygonNode12.Position);
				SplitComplexPolygonNode splitComplexPolygonNode13 = splitComplexPolygonNode10;
				splitComplexPolygonNode10 = splitComplexPolygonNode12;
				splitComplexPolygonNode12 = splitComplexPolygonNode10.GetRightestConnection(splitComplexPolygonNode13);
				if (splitComplexPolygonNode12 == null)
				{
					return PolygonUtil.SplitComplexPolygonCleanup(list2);
				}
			}
			if (list2.Count < 1)
			{
				return PolygonUtil.SplitComplexPolygonCleanup(verts);
			}
			return PolygonUtil.SplitComplexPolygonCleanup(list2);
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x00098DD8 File Offset: 0x00096FD8
		private static List<Point2DList> SplitComplexPolygonCleanup(IList<Point2D> orig)
		{
			List<Point2DList> list = new List<Point2DList>();
			Point2DList point2DList = new Point2DList(orig);
			list.Add(point2DList);
			int i = 0;
			int num = list.Count;
			while (i < num)
			{
				int num2 = list[i].Count;
				for (int j = 0; j < num2; j++)
				{
					for (int k = j + 1; k < num2; k++)
					{
						if (list[i][j].Equals(list[i][k], point2DList.Epsilon))
						{
							int num3 = k - j;
							Point2DList point2DList2 = new Point2DList();
							for (int l = j + 1; l <= k; l++)
							{
								point2DList2.Add(list[i][l]);
							}
							list[i].RemoveRange(j + 1, num3);
							list.Add(point2DList2);
							num++;
							num2 -= num3;
							k = j + 1;
						}
					}
				}
				list[i].Simplify();
				i++;
			}
			return list;
		}

		// Token: 0x02000C69 RID: 3177
		public enum PolyUnionError
		{
			// Token: 0x04004E45 RID: 20037
			None,
			// Token: 0x04004E46 RID: 20038
			NoIntersections,
			// Token: 0x04004E47 RID: 20039
			Poly1InsidePoly2,
			// Token: 0x04004E48 RID: 20040
			InfiniteLoop
		}

		// Token: 0x02000C6A RID: 3178
		[Flags]
		public enum PolyOperation : uint
		{
			// Token: 0x04004E4A RID: 20042
			None = 0U,
			// Token: 0x04004E4B RID: 20043
			Union = 1U,
			// Token: 0x04004E4C RID: 20044
			Intersect = 2U,
			// Token: 0x04004E4D RID: 20045
			Subtract = 4U
		}
	}
}
