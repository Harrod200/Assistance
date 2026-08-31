using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000450 RID: 1104
public static class Math3d
{
	// Token: 0x0600172E RID: 5934 RVA: 0x000785A0 File Offset: 0x000767A0
	public static void Init()
	{
		Math3d.tempChild = new GameObject("Math3d_TempChild").transform;
		Math3d.tempParent = new GameObject("Math3d_TempParent").transform;
		Math3d.tempChild.gameObject.hideFlags = HideFlags.HideAndDontSave;
		global::UnityEngine.Object.DontDestroyOnLoad(Math3d.tempChild.gameObject);
		Math3d.tempParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
		global::UnityEngine.Object.DontDestroyOnLoad(Math3d.tempParent.gameObject);
		Math3d.tempChild.parent = Math3d.tempParent;
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x00078624 File Offset: 0x00076824
	public static Vector2 GetPointOnSpline(float percentage, Vector2[] cPoints)
	{
		if (cPoints.Length >= 4)
		{
			int num = cPoints.Length - 3;
			int num2 = Mathf.Min(Mathf.FloorToInt(percentage * (float)num), num - 1);
			float num3 = percentage * (float)num - (float)num2;
			Vector2 vector = cPoints[num2];
			Vector2 vector2 = cPoints[num2 + 1];
			Vector2 vector3 = cPoints[num2 + 2];
			Vector2 vector4 = cPoints[num2 + 3];
			Vector2 vector5 = 0.5f * (2f * vector2 + (-vector + vector3) * num3 + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3));
			return new Vector2(vector5.x, vector5.y);
		}
		return new Vector2(0f, 0f);
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x00078758 File Offset: 0x00076958
	public static float[] GetLineSplineIntersections(Vector2[] linePoints, Vector2[] cPoints)
	{
		List<float> list = new List<float>();
		int num = cPoints.Length - 3;
		for (int i = 0; i < num; i++)
		{
			Vector2 vector = cPoints[i];
			Vector2 vector2 = cPoints[i + 1];
			Vector2 vector3 = cPoints[i + 2];
			Vector2 vector4 = cPoints[i + 3];
			float num2 = 0.5f * (-vector.x + 3f * vector2.x - 3f * vector3.x + vector4.x);
			float num3 = 0.5f * (-vector.y + 3f * vector2.y - 3f * vector3.y + vector4.y);
			float num4 = 0.5f * (2f * vector.x - 5f * vector2.x + 4f * vector3.x - vector4.x);
			float num5 = 0.5f * (2f * vector.y - 5f * vector2.y + 4f * vector3.y - vector4.y);
			float num6 = 0.5f * (-vector.x + vector3.x);
			float num7 = 0.5f * (-vector.y + vector3.y);
			float num8 = 0.5f * (2f * vector2.x);
			float num9 = 0.5f * (2f * vector2.y);
			float num10 = linePoints[0].y - linePoints[1].y;
			float num11 = linePoints[1].x - linePoints[0].x;
			float num12 = (linePoints[0].x - linePoints[1].x) * linePoints[0].y + (linePoints[1].y - linePoints[0].y) * linePoints[0].x;
			float num13 = num10 * num2 + num11 * num3;
			float num14 = num10 * num4 + num11 * num5;
			float num15 = num10 * num6 + num11 * num7;
			float num16 = num10 * num8 + num11 * num9 + num12;
			int num17;
			float num18;
			float num19;
			float num20;
			Math3d.SolveCubic(out num17, out num18, out num19, out num20, num13, num14, num15, num16);
			float num21 = (float)i / (float)num;
			float num22 = ((float)i + 1f) / (float)num - num21;
			if (num18 >= 0f && num18 <= 1f)
			{
				float num23 = num18 * num22 + num21;
				list.Add(num23);
			}
			if (num19 >= 0f && num19 <= 1f)
			{
				float num23 = num19 * num22 + num21;
				list.Add(num23);
			}
			if (num20 >= 0f && num20 <= 1f)
			{
				float num23 = num20 * num22 + num21;
				list.Add(num23);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x00078A30 File Offset: 0x00076C30
	private static void SolveCubic(out int nRoots, out float x1, out float x2, out float x3, float a, float b, float c, float d)
	{
		float num = 6.2831855f;
		float num2 = 12.566371f;
		float num3 = a;
		a = b / num3;
		b = c / num3;
		c = d / num3;
		float num4 = a / 3f;
		float num5 = (3f * b - a * a) / 9f;
		float num6 = num5 * num5 * num5;
		float num7 = (9f * a * b - 27f * c - 2f * a * a * a) / 54f;
		float num8 = num7 * num7;
		float num9 = num6 + num8;
		if (num9 < 0f)
		{
			nRoots = 3;
			float num10 = Mathf.Acos(num7 / Mathf.Sqrt(-num6));
			float num11 = Mathf.Sqrt(-num5);
			x1 = 2f * num11 * Mathf.Cos(num10 / 3f) - num4;
			x2 = 2f * num11 * Mathf.Cos((num10 + num) / 3f) - num4;
			x3 = 2f * num11 * Mathf.Cos((num10 + num2) / 3f) - num4;
			return;
		}
		if (num9 > 0f)
		{
			nRoots = 1;
			float num12 = Mathf.Sqrt(num9);
			float num13 = Math3d.CubeRoot(num7 + num12);
			float num14 = Math3d.CubeRoot(num7 - num12);
			x1 = num13 + num14 - num4;
			x2 = float.NaN;
			x3 = float.NaN;
			return;
		}
		nRoots = 3;
		float num15 = Math3d.CubeRoot(num7);
		x1 = 2f * num15 - num4;
		x2 = num15 - num4;
		x3 = x2;
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x00078BA2 File Offset: 0x00076DA2
	private static float CubeRoot(float d)
	{
		if (d < 0f)
		{
			return -Mathf.Pow(-d, 0.33333334f);
		}
		return Mathf.Pow(d, 0.33333334f);
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x00078BC8 File Offset: 0x00076DC8
	public static Vector3 AddVectorLength(Vector3 vector, float size)
	{
		float num = Vector3.Magnitude(vector);
		float num2 = (num + size) / num;
		return vector * num2;
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x00078BE9 File Offset: 0x00076DE9
	public static Vector3 SetVectorLength(Vector3 vector, float size)
	{
		return Vector3.Normalize(vector) * size;
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x00078BF7 File Offset: 0x00076DF7
	public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
	{
		return Quaternion.Inverse(A) * B;
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x00078C05 File Offset: 0x00076E05
	public static Quaternion AddRotation(Quaternion A, Quaternion B)
	{
		return A * B;
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00078C0E File Offset: 0x00076E0E
	public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
	{
		return rotation * vector;
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00078C17 File Offset: 0x00076E17
	public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
	{
		return Quaternion.Inverse(rotation) * vector;
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00078C28 File Offset: 0x00076E28
	public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
	{
		Quaternion quaternion = Math3d.SubtractRotation(to, from);
		Vector3 vector2 = Math3d.InverseTransformDirectionMath(from, vector);
		Vector3 vector3 = quaternion * vector2;
		return Math3d.TransformDirectionMath(from, vector3);
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x00078C54 File Offset: 0x00076E54
	public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
	{
		linePoint = Vector3.zero;
		lineVec = Vector3.zero;
		lineVec = Vector3.Cross(plane1Normal, plane2Normal);
		Vector3 vector = Vector3.Cross(plane2Normal, lineVec);
		float num = Vector3.Dot(plane1Normal, vector);
		if (Mathf.Abs(num) > 0.006f)
		{
			Vector3 vector2 = plane1Position - plane2Position;
			float num2 = Vector3.Dot(plane1Normal, vector2) / num;
			linePoint = plane2Position + num2 * vector;
			return true;
		}
		return false;
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x00078CD4 File Offset: 0x00076ED4
	public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
	{
		intersection = Vector3.zero;
		float num = Vector3.Dot(planePoint - linePoint, planeNormal);
		float num2 = Vector3.Dot(lineVec, planeNormal);
		if (num2 != 0f)
		{
			float num3 = num / num2;
			Vector3 vector = Math3d.SetVectorLength(lineVec, num3);
			intersection = linePoint + vector;
			return true;
		}
		return false;
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x00078D28 File Offset: 0x00076F28
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		Vector3 vector = linePoint2 - linePoint1;
		Vector3 vector2 = Vector3.Cross(lineVec1, lineVec2);
		Vector3 vector3 = Vector3.Cross(vector, lineVec2);
		if (Mathf.Abs(Vector3.Dot(vector, vector2)) < 0.0001f && vector2.sqrMagnitude > 0.0001f)
		{
			float num = Vector3.Dot(vector3, vector2) / vector2.sqrMagnitude;
			intersection = linePoint1 + lineVec1 * num;
			return true;
		}
		intersection = Vector3.zero;
		return false;
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x00078DA0 File Offset: 0x00076FA0
	public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;
		float num = Vector3.Dot(lineVec1, lineVec1);
		float num2 = Vector3.Dot(lineVec1, lineVec2);
		float num3 = Vector3.Dot(lineVec2, lineVec2);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 vector = linePoint1 - linePoint2;
			float num5 = Vector3.Dot(lineVec1, vector);
			float num6 = Vector3.Dot(lineVec2, vector);
			float num7 = (num2 * num6 - num5 * num3) / num4;
			float num8 = (num * num6 - num5 * num2) / num4;
			closestPointLine1 = linePoint1 + lineVec1 * num7;
			closestPointLine2 = linePoint2 + lineVec2 * num8;
			return true;
		}
		return false;
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x00078E54 File Offset: 0x00077054
	public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{
		float num = Vector3.Dot(point - linePoint, lineVec);
		return linePoint + lineVec * num;
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x00078E7C File Offset: 0x0007707C
	public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{
		Vector3 vector = Math3d.ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
		int num = Math3d.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector);
		if (num == 0)
		{
			return vector;
		}
		if (num == 1)
		{
			return linePoint1;
		}
		if (num == 2)
		{
			return linePoint2;
		}
		return Vector3.zero;
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x00078EC0 File Offset: 0x000770C0
	public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		float num = Math3d.SignedDistancePlanePoint(planeNormal, planePoint, point);
		num *= -1f;
		Vector3 vector = Math3d.SetVectorLength(planeNormal, num);
		return point + vector;
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x00078EED File Offset: 0x000770ED
	public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
	{
		return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x00078F02 File Offset: 0x00077102
	public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		return Vector3.Dot(planeNormal, point - planePoint);
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00078F11 File Offset: 0x00077111
	public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
	{
		return Vector3.Dot(Vector3.Cross(normal, vectorA), vectorB);
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x00078F20 File Offset: 0x00077120
	public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
	{
		Vector3 vector = Vector3.Cross(normal, referenceVector);
		return Vector3.Angle(referenceVector, otherVector) * Mathf.Sign(Vector3.Dot(vector, otherVector));
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x00078F4C File Offset: 0x0007714C
	public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
	{
		float num = (float)Math.Acos((double)Vector3.Dot(vector, normal));
		return 1.5707964f - num;
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x00078F70 File Offset: 0x00077170
	public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
	{
		double num = (double)Vector3.Dot(vec1, vec2);
		if (num < -1.0)
		{
			num = -1.0;
		}
		if (num > 1.0)
		{
			num = 1.0;
		}
		return (float)Math.Acos(num);
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x00078FBC File Offset: 0x000771BC
	public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
	{
		planeNormal = Vector3.zero;
		planePoint = Vector3.zero;
		Vector3 vector = pointB - pointA;
		Vector3 vector2 = pointC - pointA;
		planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
		Vector3 vector3 = pointA + vector / 2f;
		Vector3 vector4 = pointA + vector2 / 2f;
		Vector3 vector5 = pointC - vector3;
		Vector3 vector6 = pointB - vector4;
		Vector3 vector7;
		Math3d.ClosestPointsOnTwoLines(out planePoint, out vector7, vector3, vector5, vector4, vector6);
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x00079048 File Offset: 0x00077248
	public static Vector3 GetForwardVector(Quaternion q)
	{
		return q * Vector3.forward;
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x00079055 File Offset: 0x00077255
	public static Vector3 GetUpVector(Quaternion q)
	{
		return q * Vector3.up;
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x00079062 File Offset: 0x00077262
	public static Vector3 GetRightVector(Quaternion q)
	{
		return q * Vector3.right;
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x0007906F File Offset: 0x0007726F
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x00079090 File Offset: 0x00077290
	public static Vector3 PositionFromMatrix(Matrix4x4 m)
	{
		Vector4 column = m.GetColumn(3);
		return new Vector3(column.x, column.y, column.z);
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x000790C0 File Offset: 0x000772C0
	public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
	{
		Quaternion quaternion = Quaternion.LookRotation(alignWithVector, alignWithNormal);
		Quaternion quaternion2 = Quaternion.LookRotation(customForward, customUp);
		gameObjectInOut.transform.rotation = quaternion * Quaternion.Inverse(quaternion2);
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000790F8 File Offset: 0x000772F8
	public static void TransformWithParent(out Quaternion childRotation, out Vector3 childPosition, Quaternion parentRotation, Vector3 parentPosition, Quaternion startParentRotation, Vector3 startParentPosition, Quaternion startChildRotation, Vector3 startChildPosition)
	{
		childRotation = Quaternion.identity;
		childPosition = Vector3.zero;
		Math3d.tempParent.rotation = startParentRotation;
		Math3d.tempParent.position = startParentPosition;
		Math3d.tempParent.localScale = Vector3.one;
		Math3d.tempChild.rotation = startChildRotation;
		Math3d.tempChild.position = startChildPosition;
		Math3d.tempChild.localScale = Vector3.one;
		Math3d.tempParent.rotation = parentRotation;
		Math3d.tempParent.position = parentPosition;
		childRotation = Math3d.tempChild.rotation;
		childPosition = Math3d.tempChild.position;
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x000791A0 File Offset: 0x000773A0
	public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
	{
		Math3d.LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);
		Vector3 vector = gameObjectInOut.transform.TransformPoint(trianglePosition);
		Vector3 vector2 = alignWithPosition - vector;
		gameObjectInOut.transform.Translate(vector2, Space.World);
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x000791DE File Offset: 0x000773DE
	public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
	{
		gameObjectInOut.transform.position = positionVector;
		gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x00079200 File Offset: 0x00077400
	public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{
		Vector3 vector = linePoint2 - linePoint1;
		Vector3 vector2 = point - linePoint1;
		if (Vector3.Dot(vector2, vector) <= 0f)
		{
			return 1;
		}
		if (vector2.magnitude <= vector.magnitude)
		{
			return 0;
		}
		return 2;
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x00079240 File Offset: 0x00077440
	public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
	{
		Camera main = Camera.main;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 vector = main.WorldToScreenPoint(linePoint1);
		Vector3 vector2 = main.WorldToScreenPoint(linePoint2);
		Vector3 vector3 = Math3d.ProjectPointOnLineSegment(vector, vector2, mousePosition);
		vector3 = new Vector3(vector3.x, vector3.y, 0f);
		return (vector3 - mousePosition).magnitude;
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x00079298 File Offset: 0x00077498
	public static float MouseDistanceToCircle(Vector3 point, float radius)
	{
		Camera main = Camera.main;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 vector = main.WorldToScreenPoint(point);
		vector = new Vector3(vector.x, vector.y, 0f);
		return (vector - mousePosition).magnitude - radius;
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x000792E0 File Offset: 0x000774E0
	public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
	{
		bool flag = false;
		bool flag2 = Math3d.IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
		if (!flag2)
		{
			flag = Math3d.IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
		}
		if (!flag2 && !flag)
		{
			bool flag3 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
			bool flag4 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
			bool flag5 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
			bool flag6 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);
			return flag3 || flag4 || flag5 || flag6;
		}
		return true;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x00079348 File Offset: 0x00077548
	public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
	{
		Vector3 vector = rectC - rectA;
		float num = -(vector.magnitude / 2f);
		vector = Math3d.AddVectorLength(vector, num);
		Vector3 vector2 = rectA + vector;
		Vector3 vector3 = rectB - rectA;
		float num2 = vector3.magnitude / 2f;
		Vector3 vector4 = rectD - rectA;
		float num3 = vector4.magnitude / 2f;
		float magnitude = (Math3d.ProjectPointOnLine(vector2, vector3.normalized, point) - point).magnitude;
		return (Math3d.ProjectPointOnLine(vector2, vector4.normalized, point) - point).magnitude <= num2 && magnitude <= num3;
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x000793F0 File Offset: 0x000775F0
	public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
	{
		Vector3 vector = pointA2 - pointA1;
		Vector3 vector2 = pointB2 - pointB1;
		Vector3 vector3;
		Vector3 vector4;
		if (Math3d.ClosestPointsOnTwoLines(out vector3, out vector4, pointA1, vector.normalized, pointB1, vector2.normalized))
		{
			bool flag = Math3d.PointOnWhichSideOfLineSegment(pointA1, pointA2, vector3) != 0;
			int num = Math3d.PointOnWhichSideOfLineSegment(pointB1, pointB2, vector4);
			return !flag && num == 0;
		}
		return false;
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x00079444 File Offset: 0x00077644
	public static bool LinearAcceleration(out Vector3 vector, Vector3 position, int samples)
	{
		Vector3 vector2 = Vector3.zero;
		vector = Vector3.zero;
		if (samples < 3)
		{
			samples = 3;
		}
		if (Math3d.positionRegister == null)
		{
			Math3d.positionRegister = new Vector3[samples];
			Math3d.posTimeRegister = new float[samples];
		}
		for (int i = 0; i < Math3d.positionRegister.Length - 1; i++)
		{
			Math3d.positionRegister[i] = Math3d.positionRegister[i + 1];
			Math3d.posTimeRegister[i] = Math3d.posTimeRegister[i + 1];
		}
		Math3d.positionRegister[Math3d.positionRegister.Length - 1] = position;
		Math3d.posTimeRegister[Math3d.posTimeRegister.Length - 1] = Time.time;
		Math3d.positionSamplesTaken++;
		if (Math3d.positionSamplesTaken >= samples)
		{
			for (int j = 0; j < Math3d.positionRegister.Length - 2; j++)
			{
				Vector3 vector3 = Math3d.positionRegister[j + 1] - Math3d.positionRegister[j];
				float num = Math3d.posTimeRegister[j + 1] - Math3d.posTimeRegister[j];
				if (num == 0f)
				{
					return false;
				}
				Vector3 vector4 = vector3 / num;
				vector3 = Math3d.positionRegister[j + 2] - Math3d.positionRegister[j + 1];
				num = Math3d.posTimeRegister[j + 2] - Math3d.posTimeRegister[j + 1];
				if (num == 0f)
				{
					return false;
				}
				Vector3 vector5 = vector3 / num;
				vector2 += vector5 - vector4;
			}
			vector2 /= (float)(Math3d.positionRegister.Length - 2);
			float num2 = Math3d.posTimeRegister[Math3d.posTimeRegister.Length - 1] - Math3d.posTimeRegister[0];
			vector = vector2 / num2;
			return true;
		}
		return false;
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x00079604 File Offset: 0x00077804
	public static bool AngularAcceleration(out Vector3 vector, Quaternion rotation, int samples)
	{
		Vector3 vector2 = Vector3.zero;
		vector = Vector3.zero;
		if (samples < 3)
		{
			samples = 3;
		}
		if (Math3d.rotationRegister == null)
		{
			Math3d.rotationRegister = new Quaternion[samples];
			Math3d.rotTimeRegister = new float[samples];
		}
		for (int i = 0; i < Math3d.rotationRegister.Length - 1; i++)
		{
			Math3d.rotationRegister[i] = Math3d.rotationRegister[i + 1];
			Math3d.rotTimeRegister[i] = Math3d.rotTimeRegister[i + 1];
		}
		Math3d.rotationRegister[Math3d.rotationRegister.Length - 1] = rotation;
		Math3d.rotTimeRegister[Math3d.rotTimeRegister.Length - 1] = Time.time;
		Math3d.rotationSamplesTaken++;
		if (Math3d.rotationSamplesTaken >= samples)
		{
			for (int j = 0; j < Math3d.rotationRegister.Length - 2; j++)
			{
				Quaternion quaternion = Math3d.SubtractRotation(Math3d.rotationRegister[j + 1], Math3d.rotationRegister[j]);
				float num = Math3d.rotTimeRegister[j + 1] - Math3d.rotTimeRegister[j];
				if (num == 0f)
				{
					return false;
				}
				Vector3 vector3 = Math3d.RotDiffToSpeedVec(quaternion, num);
				quaternion = Math3d.SubtractRotation(Math3d.rotationRegister[j + 2], Math3d.rotationRegister[j + 1]);
				num = Math3d.rotTimeRegister[j + 2] - Math3d.rotTimeRegister[j + 1];
				if (num == 0f)
				{
					return false;
				}
				Vector3 vector4 = Math3d.RotDiffToSpeedVec(quaternion, num);
				vector2 += vector4 - vector3;
			}
			vector2 /= (float)(Math3d.rotationRegister.Length - 2);
			float num2 = Math3d.rotTimeRegister[Math3d.rotTimeRegister.Length - 1] - Math3d.rotTimeRegister[0];
			vector = vector2 / num2;
			return true;
		}
		return false;
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x000797C2 File Offset: 0x000779C2
	public static float LinearFunction2DBasic(float x, float Qx, float Qy)
	{
		return x * (Qy / Qx);
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x000797CC File Offset: 0x000779CC
	public static float LinearFunction2DFull(float x, float Px, float Py, float Qx, float Qy)
	{
		float num = Qy - Py;
		float num2 = Qx - Px;
		float num3 = num / num2;
		return Py + num3 * (x - Px);
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x000797EC File Offset: 0x000779EC
	private static Vector3 RotDiffToSpeedVec(Quaternion rotation, float deltaTime)
	{
		float num;
		if (rotation.eulerAngles.x <= 180f)
		{
			num = rotation.eulerAngles.x;
		}
		else
		{
			num = rotation.eulerAngles.x - 360f;
		}
		float num2;
		if (rotation.eulerAngles.y <= 180f)
		{
			num2 = rotation.eulerAngles.y;
		}
		else
		{
			num2 = rotation.eulerAngles.y - 360f;
		}
		float num3;
		if (rotation.eulerAngles.z <= 180f)
		{
			num3 = rotation.eulerAngles.z;
		}
		else
		{
			num3 = rotation.eulerAngles.z - 360f;
		}
		return new Vector3(num / deltaTime, num2 / deltaTime, num3 / deltaTime);
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x000798A8 File Offset: 0x00077AA8
	public static float PointLineDistance(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x000798D8 File Offset: 0x00077AD8
	public static float Distance(this Vector3 point, Math3d.Line line)
	{
		Vector3 vector = line.Point - point;
		return Mathf.Abs(vector.Crossed(line.Direction).Crossed(line.Direction).normalized.Dot(vector));
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x0007991C File Offset: 0x00077B1C
	public static float Distance(this Math3d.Line line, Vector3 point)
	{
		return point.Distance(line);
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x00079928 File Offset: 0x00077B28
	public static float Distance(this Vector3 point, Math3d.LineSegment line_segment)
	{
		Vector3 vector = line_segment.B - line_segment.A;
		float num = (point - line_segment.A).Dot(vector.normalized);
		if (num <= 0f || num >= vector.magnitude)
		{
			return Mathf.Min(point.Distance(line_segment.A), point.Distance(line_segment.B));
		}
		return point.Distance(new Math3d.Line(line_segment.A, vector));
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x000799A2 File Offset: 0x00077BA2
	public static float Distance(this Math3d.LineSegment line_segment, Vector3 point)
	{
		return point.Distance(line_segment);
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x000799AB File Offset: 0x00077BAB
	public static float Distance(this Vector3 a, Vector3 b)
	{
		return Vector3.Distance(a, b);
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x000799B4 File Offset: 0x00077BB4
	public static Vector3 Crossed(this Vector3 a, Vector3 b)
	{
		return Vector3.Cross(a, b);
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x000799BD File Offset: 0x00077BBD
	public static float Dot(this Vector3 a, Vector3 b)
	{
		return Vector3.Dot(a, b);
	}

	// Token: 0x040015AE RID: 5550
	private static Transform tempChild;

	// Token: 0x040015AF RID: 5551
	private static Transform tempParent;

	// Token: 0x040015B0 RID: 5552
	private static Vector3[] positionRegister;

	// Token: 0x040015B1 RID: 5553
	private static float[] posTimeRegister;

	// Token: 0x040015B2 RID: 5554
	private static int positionSamplesTaken;

	// Token: 0x040015B3 RID: 5555
	private static Quaternion[] rotationRegister;

	// Token: 0x040015B4 RID: 5556
	private static float[] rotTimeRegister;

	// Token: 0x040015B5 RID: 5557
	private static int rotationSamplesTaken;

	// Token: 0x02000C4A RID: 3146
	[Serializable]
	public struct LineSegment
	{
		// Token: 0x06006C42 RID: 27714 RVA: 0x0030654B File Offset: 0x0030474B
		public LineSegment(Vector3 a, Vector3 b)
		{
			this.A = a;
			this.B = b;
		}

		// Token: 0x04004DF7 RID: 19959
		public Vector3 A;

		// Token: 0x04004DF8 RID: 19960
		public Vector3 B;
	}

	// Token: 0x02000C4B RID: 3147
	[Serializable]
	public struct Line
	{
		// Token: 0x06006C43 RID: 27715 RVA: 0x0030655B File Offset: 0x0030475B
		public Line(Vector3 point, Vector3 direction)
		{
			this.Point = point;
			this.Direction = direction;
		}

		// Token: 0x04004DF9 RID: 19961
		public Vector3 Point;

		// Token: 0x04004DFA RID: 19962
		public Vector3 Direction;
	}
}
