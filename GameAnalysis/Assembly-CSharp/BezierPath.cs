using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200044E RID: 1102
public class BezierPath
{
	// Token: 0x0600171D RID: 5917 RVA: 0x00077BDC File Offset: 0x00075DDC
	public BezierPath(IEnumerable<BezierCurve> curves = null)
	{
		if (curves != null)
		{
			this.Segments.AddRange(curves);
		}
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x00077C00 File Offset: 0x00075E00
	public int GetIndex(double t, out double lerpFactor)
	{
		double num = this.Segments.Sum<BezierCurve>((BezierCurve x) => x.FastLength) * t;
		for (int i = 0; i < this.Segments.Count; i++)
		{
			if (num <= this.Segments[i].FastLength)
			{
				lerpFactor = num / this.Segments[i].FastLength;
				return i;
			}
			num -= this.Segments[i].FastLength;
		}
		lerpFactor = 1.0;
		return this.Segments.Count - 1;
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x00077CA8 File Offset: 0x00075EA8
	public Vector3d GetPosition(double t)
	{
		double num;
		int index = this.GetIndex(t, out num);
		return this.Segments[index].GetPosition(num);
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x00077CD4 File Offset: 0x00075ED4
	public double GetValue(double t, Func<int, double> GetValueAtIndex)
	{
		double num;
		int index = this.GetIndex(t, out num);
		return Mathd.Lerp(GetValueAtIndex(index), GetValueAtIndex(index + 1), num);
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x00077D04 File Offset: 0x00075F04
	public double GetValue<T>(double t, IEnumerable<T> elements, Func<T, double> GetElementValue)
	{
		List<T> elementList = elements as List<T>;
		if (elementList == null)
		{
			elementList = elements.ToList<T>();
		}
		if (elementList.Count != this.Segments.Count + 1)
		{
			throw new NotSupportedException();
		}
		return this.GetValue(t, (int i) => GetElementValue(elementList[i]));
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x00077D74 File Offset: 0x00075F74
	public double GetPartialVisualLength(Vector3d eye, double t0, double t1, Func<Vector3d, Vector3d> GetTransformedPosition = null, int resolution = 5)
	{
		if (GetTransformedPosition == null)
		{
			GetTransformedPosition = (Vector3d position) => position;
		}
		double num = 0.0;
		Vector3d vector3d = GetTransformedPosition(this.GetPosition(t0));
		for (int i = 0; i < resolution; i++)
		{
			double num2 = t0 + (t1 - t0) * (double)(i + 1) / (double)resolution;
			Vector3d vector3d2 = GetTransformedPosition(this.GetPosition(num2));
			Vector3d vector3d3 = vector3d;
			vector3d = vector3d2;
			double num3 = num;
			Vector3d vector3d4 = vector3d3 - eye;
			Vector3d vector3d5 = vector3d2 - eye;
			num = num3 + Vector3d.Angle(in vector3d4, in vector3d5);
		}
		return num;
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x00077E12 File Offset: 0x00076012
	public double GetVisualLength(Vector3d eye, Func<Vector3d, Vector3d> GetTransformedPosition = null, int resolution = 5)
	{
		return this.GetPartialVisualLength(eye, 0.0, 1.0, GetTransformedPosition, resolution);
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x00077E30 File Offset: 0x00076030
	public double GetMiddleX(Func<double, double, double> Function, double x0, double x1, double toleranceFraction = 0.05000000074505806)
	{
		double num = Function(0.0, x0);
		double num2 = num + Function(x0, x1);
		double num3 = num + (num2 - num) * 0.5;
		double num4 = (num2 - num) * toleranceFraction;
		double num5 = x0 + (x1 - x0) * 0.5;
		int num6 = 1;
		for (;;)
		{
			double num7 = num + Function(x0, num5);
			if (num7 > num3 - num4 && num7 < num3 + num4)
			{
				return num5;
			}
			if (num6 > 30)
			{
				break;
			}
			double num8 = (x1 - x0) / (double)Mathf.Pow(2f, (float)(++num6));
			if (num7 < num3 - num4)
			{
				num5 += num8;
			}
			else
			{
				num5 -= num8;
			}
		}
		return x0 + (x1 - x0) * 0.5;
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x00077EE9 File Offset: 0x000760E9
	public List<double> Subdivide(Func<double, double, double> Function, int steps, bool fast = true)
	{
		return this.Subdivide(Function, 0.0, 1.0, steps, fast).ToList<double>();
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x00077F0C File Offset: 0x0007610C
	public IEnumerable<double> Subdivide(Func<double, double, double> Function, double x0, double x1, int steps, bool fast = true)
	{
		double middleX = this.GetMiddleX(Function, x0, x1, 0.05000000074505806);
		if (steps == 1)
		{
			return Enumerable.Empty<double>().Append(middleX);
		}
		double num = (middleX - x0) / (x1 - x0);
		bool flag = num < 0.20000000298023224 || num > 0.800000011920929;
		if (fast && !flag)
		{
			Function = (double x0_, double x1_) => x0;
		}
		return this.Subdivide(Function, x0, middleX, steps - 1, true).Append(middleX).Concat<double>(this.Subdivide(Function, middleX, x1, steps - 1, true));
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x00077FC0 File Offset: 0x000761C0
	public List<double> GetEqualVisualLengthSudvision(Vector3d eye, Func<Vector3d, Vector3d> GetTransformedPosition, int subdivisionCount = 7)
	{
		double totalLength = this.GetPartialVisualLength(eye, 0.0, 1.0, GetTransformedPosition, 20);
		return this.Subdivide((double x0, double x1) => this.GetPartialVisualLength(eye, x0, x1, GetTransformedPosition, 5) / totalLength, subdivisionCount, true);
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x00078028 File Offset: 0x00076228
	public double GetVisualLengthOfSubdivisionSegment(List<double> subdivision, int index, Vector3d eye, Func<Vector3d, Vector3d> GetTransformedPosition)
	{
		double num = 0.0;
		if (index > 0)
		{
			num = subdivision[index - 1];
		}
		double num2 = 1.0;
		if (index < subdivision.Count)
		{
			num2 = subdivision[index];
		}
		Vector3d vector3d = GetTransformedPosition(this.GetPosition(num)) - eye;
		Vector3d vector3d2 = GetTransformedPosition(this.GetPosition(num2)) - eye;
		return Vector3d.Angle(in vector3d, in vector3d2);
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x0007809C File Offset: 0x0007629C
	public List<double> GetTransformedLengthOfSubdivisionSegments(List<double> subdivision, Vector3d eye, Func<Vector3d, Vector3d> GetTransformedPosition = null)
	{
		if (GetTransformedPosition == null)
		{
			GetTransformedPosition = (Vector3d position) => position;
		}
		List<double> list = new List<double>();
		Vector3d vector3d = GetTransformedPosition(this.GetPosition(subdivision.First<double>()));
		for (int i = 0; i < subdivision.Count - 1; i++)
		{
			Vector3d vector3d2 = GetTransformedPosition(this.GetPosition(subdivision[i + 1]));
			Vector3d vector3d3 = vector3d;
			vector3d = vector3d2;
			List<double> list2 = list;
			Vector3d vector3d4 = vector3d3 - eye;
			Vector3d vector3d5 = vector3d2 - eye;
			list2.Add(Vector3d.Angle(in vector3d4, in vector3d5));
		}
		return list;
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x00078138 File Offset: 0x00076338
	public Func<double, double> GetBakedVisualTFunction(Vector3d eye, Func<Vector3d, Vector3d> GetTransformedPosition = null, int resolution = 10)
	{
		if (resolution < 2)
		{
			resolution = 2;
		}
		List<double> equalVisualLengthSudvision = this.GetEqualVisualLengthSudvision(eye, GetTransformedPosition, 7);
		List<double> transformedLengthOfSubdivisionSegments = this.GetTransformedLengthOfSubdivisionSegments(equalVisualLengthSudvision, eye, GetTransformedPosition);
		List<double> visualTs = new List<double>();
		for (int i = 0; i <= resolution; i++)
		{
			visualTs.Add(this.GetVisualT((double)i / (double)resolution, eye, GetTransformedPosition, equalVisualLengthSudvision, transformedLengthOfSubdivisionSegments));
		}
		return delegate(double t)
		{
			if (t >= 1.0)
			{
				return 1.0;
			}
			int num = (int)(t * (double)resolution);
			double num2 = (t - (double)num / (double)resolution) / ((double)(num + 1) / (double)resolution - (double)num / (double)resolution);
			return Mathd.Lerp(visualTs[num], visualTs[num + 1], num2);
		};
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x000781C0 File Offset: 0x000763C0
	public double GetVisualT(double t, Vector3d eye, Func<Vector3d, Vector3d> GetTransformedPosition = null, List<double> subdivision = null, List<double> transformedLengths = null)
	{
		if (t == 0.0)
		{
			return 0.0;
		}
		if (t == 1.0)
		{
			return 1.0;
		}
		if (GetTransformedPosition == null)
		{
			GetTransformedPosition = (Vector3d position) => position;
		}
		if (subdivision == null)
		{
			subdivision = this.GetEqualVisualLengthSudvision(eye, GetTransformedPosition, 7);
		}
		if (transformedLengths == null)
		{
			transformedLengths = this.GetTransformedLengthOfSubdivisionSegments(subdivision, eye, GetTransformedPosition);
		}
		double num = transformedLengths.Sum();
		double num2 = 0.0;
		for (int i = 0; i < transformedLengths.Count; i++)
		{
			double num3 = transformedLengths[i] / num;
			if (num3 + num2 > t)
			{
				double num4 = 0.0;
				if (i > 0)
				{
					num4 = subdivision[i - 1];
				}
				double num5 = (t - num2) / num3;
				num2 = num4 + (subdivision[i] - num4) * num5;
				break;
			}
			num2 += num3;
		}
		return num2;
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x000782AC File Offset: 0x000764AC
	public void Smooth()
	{
		for (int i = 0; i < this.Segments.Count - 1; i++)
		{
			Vector3d a = this.Segments[i].A;
			Vector3d b = this.Segments[i].B;
			Vector3d b2 = this.Segments[i + 1].B;
			Vector3d vector3d = (b - a).normalized + (b - b2).normalized;
			Vector3d normalized = Vector3d.Cross(Vector3d.Cross(vector3d, b - a), vector3d).normalized;
			Vector3d vector3d2 = b - normalized * Vector3d.Distance(in b, in a) * 0.30000001192092896;
			Vector3d vector3d3 = b + normalized * Vector3d.Distance(in b, in b2) * 0.30000001192092896;
			if (i == 0)
			{
				this.Segments[i].ControlPointA = a + (vector3d2 - a) * 0.5;
			}
			this.Segments[i].ControlPointB = vector3d2;
			this.Segments[i].Type = BezierCurveType.Cubic;
			this.Segments[i + 1].A = b;
			this.Segments[i + 1].B = b2;
			this.Segments[i + 1].ControlPointA = vector3d3;
			if (i == this.Segments.Count - 2)
			{
				this.Segments[i + 1].ControlPointB = b2 + (vector3d3 - b2) * 0.5;
			}
			this.Segments[i + 1].Type = BezierCurveType.Cubic;
		}
	}

	// Token: 0x040015A3 RID: 5539
	public List<BezierCurve> Segments = new List<BezierCurve>();
}
