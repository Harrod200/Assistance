using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200075C RID: 1884
	public class LearnedPerformanceRequirements
	{
		// Token: 0x06003166 RID: 12646 RVA: 0x00109C0C File Offset: 0x00107E0C
		public void GiveChaseAccelerationLowerBound(float newChaseAccelerationLowerBound_mps2)
		{
			this.minimumChaseAcceleration_mps2 = Mathf.Max(this.minimumChaseAcceleration_mps2, newChaseAccelerationLowerBound_mps2);
		}

		// Token: 0x06003167 RID: 12647 RVA: 0x00109C20 File Offset: 0x00107E20
		public void GiveChaseDVLowerBound(float newChaseDVLowerBound_kps)
		{
			this.minimumChaseDV_kps = Mathf.Max(this.minimumChaseDV_kps, newChaseDVLowerBound_kps);
		}

		// Token: 0x06003168 RID: 12648 RVA: 0x00109C34 File Offset: 0x00107E34
		public void ClearChaseRequirements()
		{
			this.minimumChaseAcceleration_mps2 = 0f;
			this.minimumChaseDV_kps = 0f;
		}

		// Token: 0x06003169 RID: 12649 RVA: 0x00109C4C File Offset: 0x00107E4C
		public bool MeetsRequirements(float dv_kps, float acceleration_mps2, TISpaceGameState location)
		{
			location = this.GetAdjustedLocation(location);
			float num;
			float num2;
			if (this.minimumDVByLocation_kps.TryGetValue(location, out num))
			{
				if (dv_kps < num)
				{
					return false;
				}
				num2 = dv_kps - num;
			}
			else
			{
				num2 = dv_kps * 0.33f;
			}
			bool flag = acceleration_mps2 >= this.minimumChaseAcceleration_mps2;
			bool flag2 = num2 >= this.minimumChaseDV_kps;
			return flag || flag2;
		}

		// Token: 0x0600316A RID: 12650 RVA: 0x00109CA7 File Offset: 0x00107EA7
		public bool MeetsRequirements(TISpaceShipTemplate shipTemplate, TISpaceGameState location)
		{
			return this.MeetsRequirements(shipTemplate.baseCruiseDeltaV_kps(false), shipTemplate.basePursuitAcceleration_mps2(false), location);
		}

		// Token: 0x0600316B RID: 12651 RVA: 0x00109CBE File Offset: 0x00107EBE
		public bool MeetsRequirements(TISpaceShipState ship, TISpaceGameState location = null)
		{
			if (location == null)
			{
				location = ship.fleet.location;
			}
			return this.MeetsRequirements(ship.currentDeltaV_kps, ship.pursuitAcceleration_mps2, location);
		}

		// Token: 0x0600316C RID: 12652 RVA: 0x00109CE9 File Offset: 0x00107EE9
		public bool MeetsRequirements(TISpaceFleetState fleet, TISpaceGameState location = null)
		{
			if (location == null)
			{
				location = fleet.location;
			}
			return this.MeetsRequirements(fleet.currentDeltaV_kps, fleet.pursuitAcceleration_mps2, location);
		}

		// Token: 0x0600316D RID: 12653 RVA: 0x00109D10 File Offset: 0x00107F10
		private TISpaceGameState GetAdjustedLocation(TISpaceGameState location)
		{
			if (location.ref_habSite != null || (location.isSpaceBodyState && location.ref_spaceBody.orbits.Count > 0))
			{
				return location.ref_spaceBody.orbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km);
			}
			return location;
		}

		// Token: 0x0600316E RID: 12654 RVA: 0x00109D77 File Offset: 0x00107F77
		public void RegisterDVRequirement(TISpaceGameState location, float requiredDV_kps)
		{
			location = this.GetAdjustedLocation(location);
			this.minimumDVByLocation_kps[location] = Mathf.Max(this.GetMinimumDV_kps(location), requiredDV_kps);
		}

		// Token: 0x0600316F RID: 12655 RVA: 0x00109D9C File Offset: 0x00107F9C
		public float GetMinimumDV_kps(TISpaceGameState location)
		{
			location = this.GetAdjustedLocation(location);
			float num;
			if (this.minimumDVByLocation_kps.TryGetValue(location, out num))
			{
				return num;
			}
			return 0f;
		}

		// Token: 0x0400227B RID: 8827
		private Dictionary<TISpaceGameState, float> minimumDVByLocation_kps = new Dictionary<TISpaceGameState, float>();

		// Token: 0x0400227C RID: 8828
		private float minimumChaseAcceleration_mps2;

		// Token: 0x0400227D RID: 8829
		private float minimumChaseDV_kps;
	}
}
