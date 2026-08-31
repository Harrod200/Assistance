using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000883 RID: 2179
	public class IntelScreenSpacebodyListItem_Data
	{
		// Token: 0x06005195 RID: 20885 RVA: 0x0023E0B0 File Offset: 0x0023C2B0
		public void SetData(TISpaceBodyState spaceBody)
		{
			this.spacebodyState = spaceBody;
			this.sizeValue = spaceBody.dimensionX_km;
			this.orbitValue = spaceBody.semiMajorAxis_km;
			this.orbitSortWeight = this.spacebodyState.semiMajorAxis_AU;
			if (spaceBody.isaMoon)
			{
				this.orbitSortWeight = spaceBody.semiMajorAxis_AU + spaceBody.barycenter.semiMajorAxis_AU;
			}
			this.DescSortWeight = this.orbitSortWeight;
			if (spaceBody.objectType == SpaceObjectType.Asteroid || spaceBody.objectType == SpaceObjectType.Comet)
			{
				this.DescSortWeight *= 1000.0;
			}
			if (spaceBody.habSites.Length != 0)
			{
				bool flag = GameControl.control.activePlayer.Prospected(spaceBody);
				this.sumWater = spaceBody.GetSiteProfileRating(FactionResource.Water, flag);
				this.sumVolatiles = spaceBody.GetSiteProfileRating(FactionResource.Volatiles, flag);
				this.sumMetals = spaceBody.GetSiteProfileRating(FactionResource.Metals, flag);
				this.sumNobles = spaceBody.GetSiteProfileRating(FactionResource.NobleMetals, flag);
				this.sumFissiles = spaceBody.GetSiteProfileRating(FactionResource.Fissiles, flag);
			}
			this.sumSolar = TIHabModuleState.NaturalSolarPowerMultiplier(spaceBody.orbits.MaxBy<TIOrbitState, float>((TIOrbitState x) => x.solarMultiplier));
			if (spaceBody.barycenter.isEarth || spaceBody.isEarth)
			{
				this.hasLaunchWindow = false;
				return;
			}
			this.hasLaunchWindow = true;
			double num;
			TIDateTime nextHohmannLaunchWindowDate = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(this.controller.activePlayer, GameStateManager.Earth(), spaceBody, TITimeState.Now(), out num);
			bool flag2;
			double hohmannTimePenaltyFraction = TISpaceObjectState.GetHohmannTimePenaltyFraction(this.controller.activePlayer, nextHohmannLaunchWindowDate, num, out flag2);
			this.launchWindowCloserToPrior = flag2;
			this.launchWindowPenalty = hohmannTimePenaltyFraction.ToPercent("P0");
			this.launchWindowSort = hohmannTimePenaltyFraction * (double)(flag2 ? (-1) : 1);
		}

		// Token: 0x06005196 RID: 20886 RVA: 0x0023E25C File Offset: 0x0023C45C
		public bool HasFactionHab(TIFactionState faction)
		{
			if (this.spacebodyState.habs.Count == 0)
			{
				return false;
			}
			using (List<TIHabState>.Enumerator enumerator = this.spacebodyState.habs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.faction.Equals(faction))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005197 RID: 20887 RVA: 0x0023E2D4 File Offset: 0x0023C4D4
		public bool HasHumanHab()
		{
			if (this.spacebodyState.habs.Count == 0)
			{
				return false;
			}
			using (List<TIHabState>.Enumerator enumerator = this.spacebodyState.habs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsAlien())
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005198 RID: 20888 RVA: 0x0023E348 File Offset: 0x0023C548
		public bool HasHab()
		{
			return this.spacebodyState.habs.Count > 0;
		}

		// Token: 0x04003612 RID: 13842
		public bool showInList;

		// Token: 0x04003613 RID: 13843
		public IntelScreenController controller;

		// Token: 0x04003614 RID: 13844
		public TISpaceBodyState spacebodyState;

		// Token: 0x04003615 RID: 13845
		public double orbitSortWeight;

		// Token: 0x04003616 RID: 13846
		public double DescSortWeight;

		// Token: 0x04003617 RID: 13847
		public double orbitValue;

		// Token: 0x04003618 RID: 13848
		public double sizeValue;

		// Token: 0x04003619 RID: 13849
		public SiteProfileRating sumWater;

		// Token: 0x0400361A RID: 13850
		public SiteProfileRating sumVolatiles;

		// Token: 0x0400361B RID: 13851
		public SiteProfileRating sumMetals;

		// Token: 0x0400361C RID: 13852
		public SiteProfileRating sumNobles;

		// Token: 0x0400361D RID: 13853
		public SiteProfileRating sumFissiles;

		// Token: 0x0400361E RID: 13854
		public float sumSolar;

		// Token: 0x0400361F RID: 13855
		public bool prospectedRecorded;

		// Token: 0x04003620 RID: 13856
		public string launchWindowPenalty;

		// Token: 0x04003621 RID: 13857
		public double launchWindowSort;

		// Token: 0x04003622 RID: 13858
		public bool launchWindowCloserToPrior;

		// Token: 0x04003623 RID: 13859
		public bool hasLaunchWindow;
	}
}
