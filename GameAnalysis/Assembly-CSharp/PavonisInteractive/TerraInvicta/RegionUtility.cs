using System;
using System.Collections.Generic;
using Poly2Tri;
using UnityEngine;
using UnityEngine.Rendering;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000566 RID: 1382
	public static class RegionUtility
	{
		// Token: 0x06002490 RID: 9360 RVA: 0x000C3B9C File Offset: 0x000C1D9C
		public static List<List<Vector3>> CreateSegmentedPolysAsVector3(TIRegionOutline region, Transform parent = null, float? overrideQuality = null)
		{
			List<List<Vector3>> list = new List<List<Vector3>>(region.poly2DList.Count);
			foreach (CurvedPolygon curvedPolygon in region.poly2DList)
			{
				List<Vector3> list2 = RegionUtility.DrawRegionSpline(RegionUtility.Scale2DPoly(curvedPolygon.data, 1f, 1f, 0f), overrideQuality);
				list.Add(list2);
			}
			return list;
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x000C3C20 File Offset: 0x000C1E20
		public static List<VectorLine> CreateSegmentedPolysAsVectorLine(TIRegionOutline region, float xScale = 6.2831855f, float yscale = 3.1415927f, Transform parent = null, float? overrideQuality = null)
		{
			int num = 0;
			List<VectorLine> list = new List<VectorLine>(region.poly2DList.Count);
			foreach (CurvedPolygon curvedPolygon in region.poly2DList)
			{
				List<CurvedPolyPoint> list2 = RegionUtility.Scale2DPoly(curvedPolygon.data, xScale, yscale, 0f);
				VectorLine vectorLine = new VectorLine(region.regionName + num++.ToString(), new List<Vector3>(RegionUtility.GetNumRegionPoints(list2, null)), 1f, LineType.Continuous, Joins.Fill);
				RegionUtility.DrawRegionSpline(vectorLine, list2, overrideQuality);
				if (parent != null)
				{
					vectorLine.rectTransform.SetParent(parent, false);
				}
				list.Add(vectorLine);
			}
			return list;
		}

		// Token: 0x06002492 RID: 9362 RVA: 0x000C3CFC File Offset: 0x000C1EFC
		public static List<CurvedPolyPoint> Scale2DPoly(CurvedPolyPoint[] inPoints, float xScale = 6.2831855f, float yScale = 3.1415927f, float zOffset = 0f)
		{
			List<CurvedPolyPoint> list = new List<CurvedPolyPoint>();
			foreach (CurvedPolyPoint curvedPolyPoint in inPoints)
			{
				list.Add(curvedPolyPoint.Scale(xScale, yScale, 1f));
			}
			return list;
		}

		// Token: 0x06002493 RID: 9363 RVA: 0x000C3D3C File Offset: 0x000C1F3C
		public static List<Vector3> DrawRegionSpline(List<CurvedPolyPoint> points, float? overrideQuality = null)
		{
			int num = 0;
			int numRegionPoints = RegionUtility.GetNumRegionPoints(points, overrideQuality);
			List<Vector3> list = new List<Vector3>(numRegionPoints);
			for (int i = 0; i < numRegionPoints; i++)
			{
				list.Add(Vector3.zero);
			}
			for (int j = 0; j < points.Count - 1; j++)
			{
				CurvedPolyPoint curvedPolyPoint = points[j];
				CurvedPolyPoint curvedPolyPoint2 = points[j + 1];
				int numRegionSegments = RegionUtility.GetNumRegionSegments(curvedPolyPoint, curvedPolyPoint2, overrideQuality);
				Vector3 vector = curvedPolyPoint.anchor;
				Vector3 vector2 = curvedPolyPoint2.anchor;
				Vector3 vector3 = (curvedPolyPoint2.bezier ? curvedPolyPoint2.bezier1 : curvedPolyPoint.anchor);
				Vector3 vector4 = (curvedPolyPoint2.bezier ? curvedPolyPoint2.bezier2 : curvedPolyPoint2.anchor);
				RegionUtility.MakeCurve(list, vector, vector3, vector2, vector4, numRegionSegments, num);
				num += numRegionSegments;
			}
			list[list.Count - 1] = list[0];
			return list;
		}

		// Token: 0x06002494 RID: 9364 RVA: 0x000C3E38 File Offset: 0x000C2038
		public static void DrawRegionSpline(VectorLine line, List<CurvedPolyPoint> points, float? overrideQuality = null)
		{
			int num = 0;
			for (int i = 0; i < points.Count - 1; i++)
			{
				CurvedPolyPoint curvedPolyPoint = points[i];
				CurvedPolyPoint curvedPolyPoint2 = points[i + 1];
				int numRegionSegments = RegionUtility.GetNumRegionSegments(curvedPolyPoint, curvedPolyPoint2, overrideQuality);
				Vector3 vector = curvedPolyPoint.anchor;
				Vector3 vector2 = curvedPolyPoint2.anchor;
				Vector3 vector3 = (curvedPolyPoint2.bezier ? curvedPolyPoint2.bezier1 : curvedPolyPoint.anchor);
				Vector3 vector4 = (curvedPolyPoint2.bezier ? curvedPolyPoint2.bezier2 : curvedPolyPoint2.anchor);
				line.MakeCurve(vector, vector3, vector2, vector4, numRegionSegments, num);
				num += numRegionSegments;
			}
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x000C3EE8 File Offset: 0x000C20E8
		public static int GetNumRegionPoints(List<CurvedPolyPoint> points, float? overrideQuality = null)
		{
			int num = 1;
			for (int i = 0; i < points.Count - 1; i++)
			{
				num += RegionUtility.GetNumRegionSegments(points[i], points[i + 1], overrideQuality);
			}
			return num;
		}

		// Token: 0x06002496 RID: 9366 RVA: 0x000C3F24 File Offset: 0x000C2124
		public static int GetNumRegionSegments(CurvedPolyPoint p1, CurvedPolyPoint p2, float? overrideQuality = null)
		{
			float value = RegionUtility.segmentationQuality;
			float num = RegionUtility.BezierCurveLength(p1, p2);
			if (overrideQuality != null)
			{
				value = overrideQuality.Value;
			}
			float num2 = RegionUtility.maxSegLength - value * (RegionUtility.maxSegLength - RegionUtility.minSegLength) / 10f;
			return Mathf.Max(Mathf.RoundToInt(num / num2), p2.bezier ? 2 : 1);
		}

		// Token: 0x06002497 RID: 9367 RVA: 0x000C3F84 File Offset: 0x000C2184
		public static float BezierCurveLength(CurvedPolyPoint point1, CurvedPolyPoint point2)
		{
			Vector2 anchor = point1.anchor;
			Vector2 anchor2 = point2.anchor;
			float magnitude = (anchor2 - anchor).magnitude;
			if (point2.bezier)
			{
				Vector2 bezier = point2.bezier1;
				Vector2 bezier2 = point2.bezier2;
				return ((anchor - bezier).magnitude + (bezier2 - bezier).magnitude + (anchor2 - bezier2).magnitude + magnitude) / 2f;
			}
			return magnitude;
		}

		// Token: 0x06002498 RID: 9368 RVA: 0x000C4004 File Offset: 0x000C2204
		public static void MakeCurve(List<Vector3> m_points3, Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index)
		{
			for (int i = 0; i < segments; i++)
			{
				m_points3[index + i] = RegionUtility.GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)i / (float)segments);
			}
		}

		// Token: 0x06002499 RID: 9369 RVA: 0x000C403C File Offset: 0x000C223C
		private static Vector3 GetBezierPoint3D(ref Vector3 anchor1, ref Vector3 control1, ref Vector3 anchor2, ref Vector3 control2, float t)
		{
			float num = 3f * (control1.x - anchor1.x);
			float num2 = 3f * (control2.x - control1.x) - num;
			float num3 = anchor2.x - anchor1.x - num - num2;
			float num4 = 3f * (control1.y - anchor1.y);
			float num5 = 3f * (control2.y - control1.y) - num4;
			float num6 = anchor2.y - anchor1.y - num4 - num5;
			float num7 = 3f * (control1.z - anchor1.z);
			float num8 = 3f * (control2.z - control1.z) - num7;
			float num9 = anchor2.z - anchor1.z - num7 - num8;
			return new Vector3(num3 * (t * t * t) + num2 * (t * t) + num * t + anchor1.x, num6 * (t * t * t) + num5 * (t * t) + num4 * t + anchor1.y, num9 * (t * t * t) + num8 * (t * t) + num7 * t + anchor1.z);
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x000C4168 File Offset: 0x000C2368
		public static List<Polygon> VectorLineListToPolygonList(List<VectorLine> vlList)
		{
			List<Polygon> list = new List<Polygon>();
			foreach (VectorLine vectorLine in vlList)
			{
				list.Add(RegionUtility.VectorLineToPolygon(vectorLine));
			}
			return list;
		}

		// Token: 0x0600249B RID: 9371 RVA: 0x000C41C4 File Offset: 0x000C23C4
		public static List<Polygon> VectorListToPolygonList(List<List<Vector3>> vList, string polyName = "foo")
		{
			List<Polygon> list = new List<Polygon>();
			int num = 0;
			foreach (List<Vector3> list2 in vList)
			{
				list.Add(RegionUtility.VectorListToPolygon(list2, polyName + num++.ToString()));
			}
			return list;
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x000C4234 File Offset: 0x000C2434
		public static Polygon VectorLineToPolygon(VectorLine vl)
		{
			int num = 0;
			PolygonPoint[] array = new PolygonPoint[vl.points3.Count];
			foreach (Vector3 vector in vl.points3)
			{
				array[num++] = new PolygonPoint((double)vector.x, (double)vector.y);
			}
			return new Polygon(array)
			{
				FileName = vl.name
			};
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x000C42C0 File Offset: 0x000C24C0
		public static Polygon VectorListToPolygon(List<Vector3> vl, string polyName = "")
		{
			int num = 0;
			PolygonPoint[] array = new PolygonPoint[vl.Count];
			foreach (Vector3 vector in vl)
			{
				array[num++] = new PolygonPoint((double)vector.x, (double)vector.y);
			}
			return new Polygon(array)
			{
				FileName = polyName
			};
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x000C433C File Offset: 0x000C253C
		public static void TriangulatePolygon(Polygon poly, float? overrideQuality = null)
		{
			if (poly == null)
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			List<Vector2> list = RegionUtility.Create2DFibonacciSpherePoints(RegionUtility.NumberFibonacciSpherePointsFromQuality(overrideQuality), poly.MinY, poly.MaxY, poly.MinX, poly.MaxX);
			List<TriangulationPoint> list2 = new List<TriangulationPoint>();
			foreach (Vector2 vector in list)
			{
				if (poly.IsPointInside(new TriangulationPoint((double)vector.x, (double)vector.y)))
				{
					list2.Add(new TriangulationPoint((double)vector.x, (double)vector.y));
				}
			}
			poly.AddSteinerPoints(list2);
			RegionController.timeAddInteriorPoints += Time.realtimeSinceStartup - realtimeSinceStartup;
			P2T.Triangulate(poly);
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x000C4408 File Offset: 0x000C2608
		private static int NumberFibonacciSpherePointsFromQuality(float? overrideQuality = null)
		{
			float value = RegionUtility.segmentationQuality;
			if (overrideQuality != null)
			{
				value = overrideQuality.Value;
			}
			return (int)((float)RegionUtility.minFibonacciPoints + value * (float)(RegionUtility.maxFibonacciPoints - RegionUtility.minFibonacciPoints) / 10f);
		}

		// Token: 0x060024A0 RID: 9376 RVA: 0x000C4448 File Offset: 0x000C2648
		public static List<Vector2> Create2DFibonacciSpherePoints(int numPoints, double minLat, double maxLat, double minLon, double maxLon)
		{
			float num = 6.2831855f;
			int num2 = (int)((double)numPoints / 2.0 * (1.0 + Mathd.Sin(minLat)));
			int num3 = (int)((double)numPoints / 2.0 * (1.0 + Mathd.Sin(maxLat))) + 1;
			List<Vector2> list = new List<Vector2>(Mathf.Abs(num3 - num2) + 1);
			for (int i = num2; i < num3; i++)
			{
				float num4 = (float)i;
				float num5 = Mathf.Asin(-1f + 2f * num4 / (float)numPoints);
				float num6;
				for (num6 = num * 0.618034f * num4 % num; num6 > 3.1415927f; num6 -= num)
				{
				}
				if ((double)num5 >= minLat && (double)num5 <= maxLat && (double)num6 >= minLon && (double)num6 <= maxLon)
				{
					list.Add(new Vector2(num6, num5));
				}
			}
			return list;
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x000C4520 File Offset: 0x000C2720
		public static List<Vector2> Create2DFibonacciSpherePoints(int numPoints, bool display = false)
		{
			float num = 6.2831855f;
			List<Vector2> list = new List<Vector2>(numPoints);
			for (int i = 0; i < numPoints; i++)
			{
				float num2 = (float)i;
				float num3 = Mathf.Asin(-1f + 2f * num2 / (float)numPoints);
				float num4;
				for (num4 = num * 0.618034f * num2 % num; num4 > 3.1415927f; num4 -= num)
				{
				}
				list.Add(new Vector2(num4, num3));
			}
			if (display)
			{
				GameObject gameObject = new GameObject("Fibonacci Container");
				gameObject.transform.position = GameObject.Find("FlatMap").transform.position;
				foreach (Vector2 vector in list)
				{
					GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
					gameObject2.transform.SetParent(gameObject.transform, false);
					Vector3 vector2 = new Vector3(vector.x * 60f / num, vector.y * 30f / 3.1415927f, 0f);
					gameObject2.transform.localPosition = vector2;
					gameObject2.transform.localScale = 0.2f * Vector3.one;
				}
			}
			return list;
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x000C466C File Offset: 0x000C286C
		public static void ConvertVectorLineTo3DInPlace(VectorLine vl, float radius = 20.005f)
		{
			for (int i = 0; i < vl.points3.Count; i++)
			{
				vl.points3[i] = RegionUtility.ThreeDimFromTwoDimCartesian((double)vl.points3[i].x, (double)vl.points3[i].y, radius);
			}
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x000C46C8 File Offset: 0x000C28C8
		public static VectorLine ConvertVectorLineTo3D(VectorLine vl, float radius = 20.005f)
		{
			VectorLine vectorLine = new VectorLine(vl.name, new List<Vector3>(vl.points3.Count), 1f, LineType.Continuous, Joins.Fill);
			int num = 0;
			foreach (Vector3 vector in vl.points3)
			{
				vectorLine.points3[num++] = RegionUtility.ThreeDimFromTwoDimCartesian((double)vector.x, (double)vector.y, radius);
			}
			return vectorLine;
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x000C4760 File Offset: 0x000C2960
		public static void ConvertVectorListTo3DInPlace(List<Vector3> vList, float radius = 20.005f)
		{
			for (int i = 0; i < vList.Count; i++)
			{
				vList[i] = RegionUtility.ThreeDimFromTwoDimCartesian((double)vList[i].x, (double)vList[i].y, radius);
			}
		}

		// Token: 0x060024A5 RID: 9381 RVA: 0x000C47A8 File Offset: 0x000C29A8
		public static List<Vector3List> ConvertVector3List(List<List<Vector3>> list)
		{
			List<Vector3List> list2 = new List<Vector3List>(list.Count);
			foreach (List<Vector3> list3 in list)
			{
				Vector3List vector3List = new Vector3List
				{
					data = list3
				};
				list2.Add(vector3List);
			}
			return list2;
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x000C4818 File Offset: 0x000C2A18
		public static List<List<Vector3>> ConvertVector3List(List<Vector3List> list)
		{
			List<List<Vector3>> list2 = new List<List<Vector3>>(list.Count);
			foreach (Vector3List vector3List in list)
			{
				List<Vector3> list3 = new List<Vector3>(vector3List.data.Count);
				foreach (Vector3 vector in vector3List.data)
				{
					list3.Add(vector);
				}
				list2.Add(list3);
			}
			return list2;
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x000C48C8 File Offset: 0x000C2AC8
		public static List<Vector3Array> ConvertVector3Array(List<Vector3[]> list)
		{
			List<Vector3Array> list2 = new List<Vector3Array>(list.Count);
			foreach (Vector3[] array in list)
			{
				Vector3Array vector3Array = new Vector3Array
				{
					data = array
				};
				list2.Add(vector3Array);
			}
			return list2;
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x000C4938 File Offset: 0x000C2B38
		public static List<Vector3[]> ConvertVector3Array(List<Vector3Array> list)
		{
			List<Vector3[]> list2 = new List<Vector3[]>(list.Count);
			foreach (Vector3Array vector3Array in list)
			{
				list2.Add(vector3Array.data);
			}
			return list2;
		}

		// Token: 0x060024A9 RID: 9385 RVA: 0x000C4998 File Offset: 0x000C2B98
		public static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
		{
			float num = angleInDegrees * 0.017453292f;
			float num2 = Mathf.Cos(num);
			float num3 = Mathf.Sin(num);
			return new Vector2(num2 * (pointToRotate.x - centerPoint.x) - num3 * (pointToRotate.y - centerPoint.y) + centerPoint.x, num3 * (pointToRotate.x - centerPoint.x) + num2 * (pointToRotate.y - centerPoint.y) + centerPoint.y);
		}

		// Token: 0x060024AA RID: 9386 RVA: 0x000C4A0C File Offset: 0x000C2C0C
		public static Vector2 TwoDimFromThreeDimCartesian(Vector3 p)
		{
			float num = 1.25f - (Mathf.Atan2(p.z, -p.x) / 6.2831855f + 0.5f);
			if (num > 1f)
			{
				num -= 1f;
			}
			float num2 = Mathf.Asin(p.y * 2f) / 3.1415927f;
			return new Vector2(num * 2f - 1f, num2) * 100f;
		}

		// Token: 0x060024AB RID: 9387 RVA: 0x000C4A84 File Offset: 0x000C2C84
		public static PolygonPoint LatLonFromSpherePoint(Vector3 pIn)
		{
			Vector3 normalized = pIn.normalized;
			double num = (double)Mathf.Asin(normalized.y);
			double num2 = (double)Mathf.Atan2(normalized.x, normalized.z);
			return new PolygonPoint(num, -num2);
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x000C4AC0 File Offset: 0x000C2CC0
		public static Vector2d LatLonVector2FromSpherePoint(Vector3 pIn)
		{
			Vector3 normalized = pIn.normalized;
			double num = (double)Mathf.Asin(normalized.y);
			double num2 = (double)Mathf.Atan2(normalized.x, normalized.z);
			return new Vector2d(num, -num2);
		}

		// Token: 0x060024AD RID: 9389 RVA: 0x000C4AFB File Offset: 0x000C2CFB
		public static Vector3 ScaledTwoDimCartesian(double x, double y, float xScale, float yScale)
		{
			return new Vector3(xScale * (float)x, yScale * (float)y, 0f);
		}

		// Token: 0x060024AE RID: 9390 RVA: 0x000C4B10 File Offset: 0x000C2D10
		public static void ThreeDimFromTwoDimCartesian(Vector3 v, float radius = 20.005f)
		{
			float num = Mathf.Cos(v.y);
			float num2 = -Mathf.Sin(v.x) * num;
			float num3 = Mathf.Cos(v.x) * num;
			float num4 = Mathf.Sin(v.y);
			v.x = num2 * radius;
			v.y = num4 * radius;
			v.z = num3 * radius;
		}

		// Token: 0x060024AF RID: 9391 RVA: 0x000C4B70 File Offset: 0x000C2D70
		public static Vector3 ThreeDimFromTwoDimCartesian(double x, double y, float radius = 20.005f)
		{
			double num = Mathd.Cos(y);
			float num2 = (float)(-(float)Mathd.Sin(x) * num);
			double num3 = Mathd.Cos(x) * num;
			double num4 = Mathd.Sin(y);
			return new Vector3(num2, (float)num4, (float)num3) * radius;
		}

		// Token: 0x060024B0 RID: 9392 RVA: 0x000C4BAD File Offset: 0x000C2DAD
		public static bool ContainsPoint2D(PolygonPoint[] polyPoints, double x, double y)
		{
			return PolygonUtil.PointInPolygon2D(polyPoints, new Point2D(x, y));
		}

		// Token: 0x060024B1 RID: 9393 RVA: 0x000C4BBC File Offset: 0x000C2DBC
		public static void Draw2DRegionPolyOutlines(List<VectorLine> polys, float mapOffset = -0.01f, Transform parent = null)
		{
			foreach (VectorLine vectorLine in polys)
			{
				vectorLine.SetWidth(0.1f);
				vectorLine.SetColor(Color.black);
				vectorLine.Draw3D();
				if (parent != null)
				{
					vectorLine.rectTransform.SetParent(parent, false);
				}
				vectorLine.rectTransform.localPosition = new Vector3(0f, 0f, mapOffset);
			}
		}

		// Token: 0x060024B2 RID: 9394 RVA: 0x000C4C50 File Offset: 0x000C2E50
		public static void DisplayRegionOutline3D(List<VectorLine> polys, Transform parent = null)
		{
			foreach (VectorLine vectorLine in polys)
			{
				vectorLine.SetWidth(10f);
				vectorLine.SetColor(Color.black);
				vectorLine.Draw3D();
				vectorLine.rectTransform.position = new Vector3(0f, 0f, 0f);
				if (parent != null)
				{
					vectorLine.rectTransform.SetParent(parent, false);
				}
			}
		}

		// Token: 0x060024B3 RID: 9395 RVA: 0x000C4CE8 File Offset: 0x000C2EE8
		public static List<VectorLine> DisplayRegionOutline3D(List<List<Vector3>> polys, string polyName, Transform parent = null)
		{
			List<VectorLine> list = new List<VectorLine>(polys.Count);
			foreach (List<Vector3> list2 in polys)
			{
				VectorLine vectorLine = new VectorLine(polyName, list2, 1f, LineType.Continuous, Joins.Fill);
				vectorLine.SetWidth(10f);
				vectorLine.SetColor(Color.black);
				vectorLine.Draw3D();
				vectorLine.rectTransform.position = new Vector3(0f, 0f, 0f);
				if (parent != null)
				{
					vectorLine.rectTransform.SetParent(parent, false);
				}
				list.Add(vectorLine);
			}
			return list;
		}

		// Token: 0x060024B4 RID: 9396 RVA: 0x000C4DA4 File Offset: 0x000C2FA4
		public static Vector3[] MeshFromPolygon(Polygon poly, bool is3D = false, float offset = -0.0025f)
		{
			int num = 2;
			int num2 = 1;
			Vector3[] array = new Vector3[poly.Triangles.Count * 3];
			for (int i = 0; i < poly.Triangles.Count; i++)
			{
				DelaunayTriangle delaunayTriangle = poly.Triangles[i];
				if (is3D)
				{
					array[i * 3] = RegionUtility.ThreeDimFromTwoDimCartesian(delaunayTriangle.Points[0].X, delaunayTriangle.Points[0].Y, 20.005f);
					array[i * 3 + num] = RegionUtility.ThreeDimFromTwoDimCartesian(delaunayTriangle.Points[1].X, delaunayTriangle.Points[1].Y, 20.005f);
					array[i * 3 + num2] = RegionUtility.ThreeDimFromTwoDimCartesian(delaunayTriangle.Points[2].X, delaunayTriangle.Points[2].Y, 20.005f);
				}
				else
				{
					array[i * 3] = RegionUtility.ScaledTwoDimCartesian(delaunayTriangle.Points[0].X, delaunayTriangle.Points[0].Y, 1f, 1f);
					array[i * 3 + num] = RegionUtility.ScaledTwoDimCartesian(delaunayTriangle.Points[1].X, delaunayTriangle.Points[1].Y, 1f, 1f);
					array[i * 3 + num2] = RegionUtility.ScaledTwoDimCartesian(delaunayTriangle.Points[2].X, delaunayTriangle.Points[2].Y, 1f, 1f);
				}
			}
			return array;
		}

		// Token: 0x060024B5 RID: 9397 RVA: 0x000C4F60 File Offset: 0x000C3160
		public static List<Vector3[]> MeshFromPolygon(List<Polygon> polyList, bool is3D = false, float offset = -0.0025f)
		{
			List<Vector3[]> list = new List<Vector3[]>(polyList.Count);
			foreach (Polygon polygon in polyList)
			{
				list.Add(RegionUtility.MeshFromPolygon(polygon, is3D, offset));
			}
			return list;
		}

		// Token: 0x060024B6 RID: 9398 RVA: 0x000C4FC4 File Offset: 0x000C31C4
		public static Mesh CreateSurfaceMesh(Vector3[] surfPoints)
		{
			int num = surfPoints.Length - 1;
			int num2 = num + 1;
			int[] array = new int[num2];
			List<Vector3> list = new List<Vector3>(num2);
			List<Vector3> list2 = new List<Vector3>(num2);
			int num3 = -1;
			if (RegionUtility.s_vectorCache == null)
			{
				RegionUtility.s_vectorCache = new Dictionary<Vector3, int>(2000);
			}
			else
			{
				RegionUtility.s_vectorCache.Clear();
			}
			for (int i = 0; i <= num; i++)
			{
				Vector3 vector = surfPoints[i];
				if (RegionUtility.s_vectorCache.ContainsKey(vector))
				{
					array[i] = RegionUtility.s_vectorCache[vector];
				}
				else
				{
					list.Add(vector);
					list2.Add(vector.normalized);
					RegionUtility.s_vectorCache.Add(vector, ++num3);
					array[i] = num3;
				}
			}
			return new Mesh
			{
				vertices = list.ToArray(),
				normals = list2.ToArray(),
				triangles = array
			};
		}

		// Token: 0x060024B7 RID: 9399 RVA: 0x000C50A0 File Offset: 0x000C32A0
		public static GameObject CreateSurface(string name, Vector3[] surfPoints, Material material)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(MeshRenderer),
				typeof(MeshFilter)
			});
			gameObject.hideFlags = HideFlags.DontSave;
			Mesh mesh = RegionUtility.CreateSurfaceMesh(surfPoints);
			mesh.name = "Mesh - " + name;
			mesh.hideFlags = HideFlags.DontSave;
			gameObject.GetComponent<MeshFilter>().mesh = mesh;
			Renderer component = gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				component.sharedMaterial = material;
				component.receiveShadows = false;
				component.shadowCastingMode = ShadowCastingMode.Off;
				component.lightProbeUsage = LightProbeUsage.Off;
				component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			}
			return gameObject;
		}

		// Token: 0x04001B92 RID: 7058
		private const float PI = 3.1415927f;

		// Token: 0x04001B93 RID: 7059
		private const float TWOPI = 6.2831855f;

		// Token: 0x04001B94 RID: 7060
		public static float segmentationQuality = 0f;

		// Token: 0x04001B95 RID: 7061
		public static float maxSegLength = 0.02f;

		// Token: 0x04001B96 RID: 7062
		public static float minSegLength = 0.002f;

		// Token: 0x04001B97 RID: 7063
		public static int minFibonacciPoints = 40000;

		// Token: 0x04001B98 RID: 7064
		public static int maxFibonacciPoints = 200000;

		// Token: 0x04001B99 RID: 7065
		private const float SQRT_5 = 2.236068f;

		// Token: 0x04001B9A RID: 7066
		private const float phi = 0.618034f;

		// Token: 0x04001B9B RID: 7067
		private static Dictionary<Vector3, int> s_vectorCache;
	}
}
