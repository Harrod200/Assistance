using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000799 RID: 1945
	public class MicrothrustTransferLERPvalues
	{
		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06003E6E RID: 15982 RVA: 0x0019504E File Offset: 0x0019324E
		// (set) Token: 0x06003E6F RID: 15983 RVA: 0x00195056 File Offset: 0x00193256
		public double eccentricity
		{
			get
			{
				return this._eccentricity;
			}
			set
			{
				if (value >= 1.0)
				{
					Debug.LogError("Attempted to create a hyperbolic microthrust.  Forcing to circular to avoid crashes.");
					this._eccentricity = 0.0;
					return;
				}
				this._eccentricity = value;
			}
		}

		// Token: 0x06003E70 RID: 15984 RVA: 0x00195088 File Offset: 0x00193288
		public MicrothrustTransferLERPvalues(double radius_m, double meanAnomaly_Rad, double eccentricity, double ascendingNode_Rad, double inclination_Rad, double argPeriapsis_Rad, double meanAnomalyCorrection_Rad, double radiusCorrection_m, double meanAnomalySpeedCorrection_RadPerSec)
		{
			if (eccentricity < 0.0 || eccentricity >= 1.0)
			{
				Log.Warn("Attempting to make hyperbolic microthrust spiral (LERPed).  Forcing circular.", Array.Empty<object>());
				eccentricity = 0.0;
				radius_m = Mathd.Abs(radius_m);
			}
			if (radius_m <= 0.0)
			{
				Log.Warn("Attempted to create a MicrothrustTransferLerp with a negative radius.  Forcing positive.", Array.Empty<object>());
				radius_m = Mathd.Max(Mathd.Abs(radius_m), 1000.0);
			}
			if (radius_m + radiusCorrection_m <= 0.0)
			{
				Log.Warn("Attempted to create a MicrothrustTransferLERP with negative radius after correction.  Discarding radius correction.", Array.Empty<object>());
				radiusCorrection_m = 0.0;
			}
			this.radius_m = radius_m;
			this.meanAnomaly_Rad = meanAnomaly_Rad;
			this.eccentricity = eccentricity;
			this.ascendingNode_Rad = ascendingNode_Rad;
			this.inclination_Rad = inclination_Rad;
			this.argPeriapsis_Rad = argPeriapsis_Rad;
			this.meanAnomalyCorrection_Rad = meanAnomalyCorrection_Rad;
			this.radiusCorrection_m = radiusCorrection_m;
			this.meanAnomalySpeedCorrection_RadPerSec = meanAnomalySpeedCorrection_RadPerSec;
		}

		// Token: 0x06003E71 RID: 15985 RVA: 0x00195174 File Offset: 0x00193374
		public MicrothrustTransferLERPvalues Clone()
		{
			return (MicrothrustTransferLERPvalues)base.MemberwiseClone();
		}

		// Token: 0x040026E0 RID: 9952
		public double radius_m;

		// Token: 0x040026E1 RID: 9953
		public double meanAnomaly_Rad;

		// Token: 0x040026E2 RID: 9954
		private double _eccentricity;

		// Token: 0x040026E3 RID: 9955
		public double ascendingNode_Rad;

		// Token: 0x040026E4 RID: 9956
		public double inclination_Rad;

		// Token: 0x040026E5 RID: 9957
		public double argPeriapsis_Rad;

		// Token: 0x040026E6 RID: 9958
		public double meanAnomalyCorrection_Rad;

		// Token: 0x040026E7 RID: 9959
		public double radiusCorrection_m;

		// Token: 0x040026E8 RID: 9960
		public double meanAnomalySpeedCorrection_RadPerSec;
	}
}
