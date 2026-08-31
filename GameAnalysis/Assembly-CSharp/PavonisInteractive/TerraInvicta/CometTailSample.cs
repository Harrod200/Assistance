using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000597 RID: 1431
	public abstract class CometTailSample
	{
		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x0600261F RID: 9759 RVA: 0x000CE9DF File Offset: 0x000CCBDF
		// (set) Token: 0x06002620 RID: 9760 RVA: 0x000CE9E7 File Offset: 0x000CCBE7
		public TIDateTime SpawnDate { get; private set; }

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06002621 RID: 9761 RVA: 0x000CE9F0 File Offset: 0x000CCBF0
		// (set) Token: 0x06002622 RID: 9762 RVA: 0x000CE9F8 File Offset: 0x000CCBF8
		public Vector3d SpawnPosition { get; private set; }

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06002623 RID: 9763 RVA: 0x000CEA01 File Offset: 0x000CCC01
		// (set) Token: 0x06002624 RID: 9764 RVA: 0x000CEA09 File Offset: 0x000CCC09
		public Vector3d SpawnVelocity { get; private set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06002625 RID: 9765 RVA: 0x000CEA12 File Offset: 0x000CCC12
		// (set) Token: 0x06002626 RID: 9766 RVA: 0x000CEA1A File Offset: 0x000CCC1A
		public double SpawnRadius_m { get; private set; }

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06002627 RID: 9767 RVA: 0x000CEA23 File Offset: 0x000CCC23
		// (set) Token: 0x06002628 RID: 9768 RVA: 0x000CEA2B File Offset: 0x000CCC2B
		public double SpawnOpacity { get; private set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06002629 RID: 9769 RVA: 0x000CEA34 File Offset: 0x000CCC34
		// (set) Token: 0x0600262A RID: 9770 RVA: 0x000CEA3C File Offset: 0x000CCC3C
		public double ExpansionVelocity_mps { get; private set; }

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x0600262B RID: 9771 RVA: 0x000CEA48 File Offset: 0x000CCC48
		public double Age_s
		{
			get
			{
				return (TITimeState.Now() - this.SpawnDate).TotalSeconds;
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x0600262C RID: 9772 RVA: 0x000CEA6D File Offset: 0x000CCC6D
		public double Radius_m
		{
			get
			{
				return this.SpawnRadius_m + this.ExpansionVelocity_mps * this.Age_s;
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x0600262D RID: 9773 RVA: 0x000CEA83 File Offset: 0x000CCC83
		public double RelativeDensity
		{
			get
			{
				return Mathd.Pow(this.SpawnRadius_m / this.Radius_m, 3.0);
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x0600262E RID: 9774 RVA: 0x000CEAA0 File Offset: 0x000CCCA0
		public double Opacity
		{
			get
			{
				return this.SpawnOpacity * this.SpawnRadius_m / this.Radius_m;
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x0600262F RID: 9775 RVA: 0x000CEAB8 File Offset: 0x000CCCB8
		public Vector3d Position_m
		{
			get
			{
				TIDateTime tidateTime = TITimeState.Now();
				if (this.cachedNow == null || this.cachedNow != tidateTime)
				{
					this.CalculatePositionAndVelocity(tidateTime, out this.cachedPosition_m, out this.cachedVelocity_mps, 10);
					this.cachedNow = tidateTime;
				}
				return this.cachedPosition_m;
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06002630 RID: 9776 RVA: 0x000CEB0C File Offset: 0x000CCD0C
		public Vector3d Velocity_mps
		{
			get
			{
				TIDateTime tidateTime = TITimeState.Now();
				if (this.cachedNow == null || this.cachedNow != tidateTime)
				{
					this.CalculatePositionAndVelocity(tidateTime, out this.cachedPosition_m, out this.cachedVelocity_mps, 10);
					this.cachedNow = tidateTime;
				}
				return this.cachedVelocity_mps;
			}
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06002631 RID: 9777
		public abstract Color Color { get; }

		// Token: 0x06002632 RID: 9778 RVA: 0x000CEB60 File Offset: 0x000CCD60
		public virtual void CalculatePositionAndVelocity(TIDateTime time, out Vector3d position_m, out Vector3d velocity_mps, int resolution = 10)
		{
			position_m = this.SpawnPosition;
			velocity_mps = this.SpawnVelocity;
			double num = (time - this.SpawnDate).TotalSeconds / (double)resolution;
			for (int i = 0; i < resolution; i++)
			{
				position_m += velocity_mps * num;
				velocity_mps += this.GetAccelerationVector_mps2(position_m);
			}
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x000CEBE5 File Offset: 0x000CCDE5
		public virtual Vector3d GetAccelerationVector_mps2(Vector3d position_m)
		{
			return Vector3d.zero;
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x000CEBEC File Offset: 0x000CCDEC
		public CometTailSample(TIDateTime date, Vector3d position, Vector3d velocity, double radius_m, double opacity, double expansionVelocity_mps)
		{
			this.SpawnDate = date;
			this.SpawnPosition = position;
			this.SpawnVelocity = velocity;
			this.SpawnRadius_m = radius_m;
			this.SpawnOpacity = opacity;
			this.ExpansionVelocity_mps = expansionVelocity_mps;
		}

		// Token: 0x04001C6B RID: 7275
		private TIDateTime cachedNow;

		// Token: 0x04001C6C RID: 7276
		private Vector3d cachedPosition_m;

		// Token: 0x04001C6D RID: 7277
		private Vector3d cachedVelocity_mps;
	}
}
