using System;
using UnityEngine;

// Token: 0x0200044B RID: 1099
[Serializable]
public class BezierCurve
{
	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001712 RID: 5906 RVA: 0x000776E2 File Offset: 0x000758E2
	public double Length
	{
		get
		{
			this.RefreshCache();
			return this.cachedLength;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001713 RID: 5907 RVA: 0x000776F0 File Offset: 0x000758F0
	public double FastLength
	{
		get
		{
			if (this.cachedLength < 0.0)
			{
				this.cachedLength = this.ComputeLength(10);
			}
			return this.cachedLength;
		}
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x00077718 File Offset: 0x00075918
	public void RefreshCache()
	{
		if (this.cacheType == this.Type && (in this.cacheA) == (in this.A) && (in this.cacheB) == (in this.B) && (in this.cacheControlPointA) == (in this.ControlPointA) && (in this.cacheControlPointB) == (in this.ControlPointB))
		{
			return;
		}
		this.cachedLength = this.ComputeLength(10);
		this.cacheType = this.Type;
		this.cacheA = this.A;
		this.cacheB = this.B;
		this.cacheControlPointA = this.ControlPointA;
		this.cacheControlPointB = this.ControlPointB;
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x000777CC File Offset: 0x000759CC
	public double ComputeLength(int resolution = 10)
	{
		double num = 0.0;
		for (int i = 0; i < resolution; i++)
		{
			double num2 = (double)i / (double)resolution;
			double num3 = (double)(i + 1) / (double)resolution;
			double num4 = num;
			Vector3d position = this.GetPosition(num2);
			Vector3d position2 = this.GetPosition(num3);
			num = num4 + Vector3d.Distance(in position, in position2);
		}
		return num;
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x0007781C File Offset: 0x00075A1C
	public Vector3d GetPosition(double t)
	{
		switch (this.Type)
		{
		case BezierCurveType.Linear:
			return Vector3d.Lerp(this.A, this.B, t);
		case BezierCurveType.Quadratic:
			return Mathd.Pow(1.0 - t, 2.0) * this.A + 2.0 * (1.0 - t) * t * this.ControlPointA + t * t * this.B;
		case BezierCurveType.Cubic:
			return Mathd.Pow(1.0 - t, 3.0) * this.A + 3.0 * Mathd.Pow(1.0 - t, 2.0) * t * this.ControlPointA + 3.0 * (1.0 - t) * t * t * this.ControlPointB + t * t * t * this.B;
		default:
			return Vector3d.zero;
		}
	}

	// Token: 0x0400158D RID: 5517
	public Vector3d A;

	// Token: 0x0400158E RID: 5518
	public Vector3d B;

	// Token: 0x0400158F RID: 5519
	public Vector3d ControlPointA;

	// Token: 0x04001590 RID: 5520
	public Vector3d ControlPointB;

	// Token: 0x04001591 RID: 5521
	public BezierCurveType Type;

	// Token: 0x04001592 RID: 5522
	private Vector3d cacheA;

	// Token: 0x04001593 RID: 5523
	private Vector3d cacheB;

	// Token: 0x04001594 RID: 5524
	private Vector3d cacheControlPointA;

	// Token: 0x04001595 RID: 5525
	private Vector3d cacheControlPointB;

	// Token: 0x04001596 RID: 5526
	private BezierCurveType cacheType;

	// Token: 0x04001597 RID: 5527
	private double cachedLength = -1.0;
}
