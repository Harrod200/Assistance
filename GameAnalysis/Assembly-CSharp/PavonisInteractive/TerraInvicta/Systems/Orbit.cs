using System;
using Unity.Entities;
using UnityEngine;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x0200098F RID: 2447
	[Serializable]
	public struct Orbit : IComponentData
	{
		// Token: 0x17000FED RID: 4077
		// (get) Token: 0x06005CD6 RID: 23766 RVA: 0x002C3B14 File Offset: 0x002C1D14
		// (set) Token: 0x06005CD7 RID: 23767 RVA: 0x002C3B65 File Offset: 0x002C1D65
		public DateTime PeriapsisEpoch
		{
			get
			{
				if (this._PeriapsisEpoch != null)
				{
					return this._PeriapsisEpoch.Value;
				}
				this._PeriapsisEpoch = new DateTime?(this.PrevPeriapsisTime(TITimeState.Now().ExportTime()));
				return this._PeriapsisEpoch.Value;
			}
			set
			{
				this._PeriapsisEpoch = new DateTime?(value);
			}
		}

		// Token: 0x17000FEE RID: 4078
		// (get) Token: 0x06005CD8 RID: 23768 RVA: 0x002C3B73 File Offset: 0x002C1D73
		public bool IsElliptical
		{
			get
			{
				return this.Eccentricity < 1.0;
			}
		}

		// Token: 0x17000FEF RID: 4079
		// (get) Token: 0x06005CD9 RID: 23769 RVA: 0x002C3B86 File Offset: 0x002C1D86
		public bool IsHyperbolic
		{
			get
			{
				return this.Eccentricity >= 1.0;
			}
		}

		// Token: 0x0400422F RID: 16943
		public double Eccentricity;

		// Token: 0x04004230 RID: 16944
		public double SemimajorAxis_m;

		// Token: 0x04004231 RID: 16945
		public double Inclination_Rad;

		// Token: 0x04004232 RID: 16946
		public double LongitudeAscendingNode_Rad;

		// Token: 0x04004233 RID: 16947
		public double ArgumentPeriapsis_Rad;

		// Token: 0x04004234 RID: 16948
		public double MeanAnomalyAtEpoch_Rad;

		// Token: 0x04004235 RID: 16949
		public Vector3d PositionAtEpoch;

		// Token: 0x04004236 RID: 16950
		public Vector3d VelocityAtEpoch;

		// Token: 0x04004237 RID: 16951
		public DateTime Epoch;

		// Token: 0x04004238 RID: 16952
		public Entity Barycenter;

		// Token: 0x04004239 RID: 16953
		private DateTime? _PeriapsisEpoch;

		// Token: 0x0400423A RID: 16954
		public double Period;

		// Token: 0x0400423B RID: 16955
		public double MeanMotion;

		// Token: 0x0400423C RID: 16956
		public Vector3d Apoapsis;

		// Token: 0x0400423D RID: 16957
		public Vector3d Periapsis;

		// Token: 0x0400423E RID: 16958
		public Vector3d Normal;

		// Token: 0x0400423F RID: 16959
		public VectorLine OrbitTrail;

		// Token: 0x04004240 RID: 16960
		public Vector3d[] WorldPoints;

		// Token: 0x04004241 RID: 16961
		public Vector3[] ScaledPoints;

		// Token: 0x04004242 RID: 16962
		public double[] TimeAtPoint_s;
	}
}
