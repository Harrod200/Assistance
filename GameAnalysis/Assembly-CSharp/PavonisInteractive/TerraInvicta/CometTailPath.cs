using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000596 RID: 1430
	public class CometTailPath : BezierPath
	{
		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06002617 RID: 9751 RVA: 0x000CE89F File Offset: 0x000CCA9F
		// (set) Token: 0x06002618 RID: 9752 RVA: 0x000CE8A7 File Offset: 0x000CCAA7
		public List<CometTailSample> Samples { get; private set; }

		// Token: 0x06002619 RID: 9753 RVA: 0x000CE8B0 File Offset: 0x000CCAB0
		public CometTailPath(List<CometTailSample> samples)
			: base(null)
		{
			this.Samples = samples;
			for (int i = 1; i < samples.Count; i++)
			{
				Vector3d position_m = samples[i - 1].Position_m;
				Vector3d position_m2 = samples[i].Position_m;
				this.Segments.Add(new BezierCurve
				{
					A = position_m,
					B = position_m2,
					ControlPointA = Vector3d.Lerp(position_m, position_m2, 0.30000001192092896),
					ControlPointB = Vector3d.Lerp(position_m, position_m2, 0.699999988079071)
				});
			}
			base.Smooth();
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x000CE948 File Offset: 0x000CCB48
		public double GetRadius(float t)
		{
			return (double)((float)base.GetValue((double)t, (int i) => this.Samples[i].Radius_m));
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x000CE960 File Offset: 0x000CCB60
		public float GetOpacity(float t)
		{
			return (float)base.GetValue((double)t, (int i) => this.Samples[i].Opacity);
		}

		// Token: 0x0600261C RID: 9756 RVA: 0x000CE978 File Offset: 0x000CCB78
		public Color GetColor(float t)
		{
			double num;
			int index = base.GetIndex((double)t, out num);
			return Color.Lerp(this.Samples[index].Color, this.Samples[index].Color, (float)num);
		}
	}
}
