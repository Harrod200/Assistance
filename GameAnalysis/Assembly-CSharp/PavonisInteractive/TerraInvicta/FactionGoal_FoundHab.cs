using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200073D RID: 1853
	public abstract class FactionGoal_FoundHab : FactionGoal_Space
	{
		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06002F15 RID: 12053 RVA: 0x001023FB File Offset: 0x001005FB
		// (set) Token: 0x06002F16 RID: 12054 RVA: 0x00102403 File Offset: 0x00100603
		public bool setAsPrimaryHab { get; protected set; }

		// Token: 0x06002F17 RID: 12055 RVA: 0x0010240C File Offset: 0x0010060C
		public override bool RequiresFleet()
		{
			if (this.faction.habs.Any<TIHabState>((TIHabState x) => x.ref_system == this.target().ref_system))
			{
				return false;
			}
			if (!this.faction.IsAlienFaction)
			{
				if (FoundHabOperation.GetCostFromEarth(this.target(), this.faction, true).completionTime_days <= TemplateManager.global.maxHabBoostFromEarthDuration_days)
				{
					return false;
				}
			}
			else if (this.faction.habs.Any<TIHabState>((TIHabState x) => x.ref_system == this.target().ref_system))
			{
				return false;
			}
			return true;
		}

		// Token: 0x06002F18 RID: 12056 RVA: 0x0010248C File Offset: 0x0010068C
		public override TIGameState actor()
		{
			if (!(base.assignedFleet != null))
			{
				return this.faction.ref_gameState;
			}
			return base.assignedFleet.ref_gameState;
		}

		// Token: 0x06002F19 RID: 12057 RVA: 0x001024B3 File Offset: 0x001006B3
		public override bool InProgress()
		{
			return base.assignedFleet != null && base.assignedFleet.inTransfer;
		}

		// Token: 0x06002F1A RID: 12058 RVA: 0x001024D0 File Offset: 0x001006D0
		public override bool FoundHabGoal()
		{
			return true;
		}

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06002F1B RID: 12059 RVA: 0x001024D4 File Offset: 0x001006D4
		public override bool GrantMissionControlIndulgence
		{
			get
			{
				if (!base.objectiveGoal)
				{
					TIGameState tigameState = this.target();
					int? num;
					if (tigameState == null)
					{
						num = null;
					}
					else
					{
						TISpaceBodyState ref_system = tigameState.ref_system;
						num = ((ref_system != null) ? new int?(ref_system.habSitesInSystem.Count) : null);
					}
					int? num2 = num;
					return num2.GetValueOrDefault() > 4;
				}
				return true;
			}
		}

		// Token: 0x06002F1C RID: 12060 RVA: 0x0010252E File Offset: 0x0010072E
		public List<TIHabModuleTemplate> RequiredModules()
		{
			return this.specialModules;
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06002F1D RID: 12061 RVA: 0x00102538 File Offset: 0x00100738
		protected List<TIHabModuleTemplate> specialModules
		{
			get
			{
				List<TIHabModuleTemplate> list = new List<TIHabModuleTemplate>();
				foreach (string text in this.requiredModuleNames)
				{
					TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(text, false);
					if (tihabModuleTemplate != null)
					{
						list.Add(tihabModuleTemplate);
					}
				}
				return list;
			}
		}

		// Token: 0x06002F1E RID: 12062 RVA: 0x0010259C File Offset: 0x0010079C
		public override bool NeedsShipsOrdered()
		{
			return this.RequiresFleet() && base.NeedsShipsOrdered();
		}

		// Token: 0x06002F1F RID: 12063 RVA: 0x001025B0 File Offset: 0x001007B0
		public override ShipRole GetPrimaryShipRole()
		{
			if (!this.target().ref_spaceObject.isSun)
			{
				if (this.target().ref_spaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU > 1.25)
				{
					return ShipRole.OuterSystemColonyShip;
				}
				return ShipRole.InnerSystemColonyShip;
			}
			else
			{
				if (this.target().ref_orbit.semiMajorAxis_AU > 1.25)
				{
					return ShipRole.OuterSystemColonyShip;
				}
				return ShipRole.InnerSystemColonyShip;
			}
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x00102611 File Offset: 0x00100811
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_FoundHab.preferredShipRoles;
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x00102618 File Offset: 0x00100818
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x06002F22 RID: 12066 RVA: 0x0010261F File Offset: 0x0010081F
		public void SetHab(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x06002F23 RID: 12067 RVA: 0x00102628 File Offset: 0x00100828
		public override TIGameState goalProduct()
		{
			return this.hab;
		}

		// Token: 0x06002F24 RID: 12068 RVA: 0x00102630 File Offset: 0x00100830
		public override bool GoalFulfilled()
		{
			return TIGameState.Valid(this.hab);
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x00102640 File Offset: 0x00100840
		public override bool ShouldDiscardGoal()
		{
			if (this.target().ref_spaceBody == null || this.faction == null)
			{
				return true;
			}
			if (base.importance <= 0)
			{
				return true;
			}
			if (this.faction.IsAlienFaction)
			{
				if (this is FactionGoal_FoundSurveillanceStation)
				{
					return false;
				}
				if (base.Age_years > 3f)
				{
					return true;
				}
			}
			else
			{
				if (base.Age_years > 5f)
				{
					return true;
				}
				if (!this.target().ref_spaceBody.IsSafeForColonization(this.faction, HabType.Any))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x001026CC File Offset: 0x001008CC
		public override bool ShouldPauseGoal()
		{
			return this.faction.IsAlienFaction && !this.LeaveMyFleetAlone() && !this.target().ref_spaceBody.IsSafeForColonization(this.faction, HabType.Any);
		}

		// Token: 0x06002F27 RID: 12071 RVA: 0x00102700 File Offset: 0x00100900
		public override bool LeaveMyFleetAlone()
		{
			if (base.LeaveMyFleetAlone())
			{
				return true;
			}
			TIGameState tigameState = this.target();
			TISpaceBodyState tispaceBodyState = ((tigameState != null) ? tigameState.ref_system : null);
			if (tispaceBodyState != null)
			{
				TISpaceFleetState assignedFleet = base.assignedFleet;
				if (((assignedFleet != null) ? assignedFleet.ref_system : null) == tispaceBodyState)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002234 RID: 8756
		public List<string> requiredModuleNames;

		// Token: 0x04002236 RID: 8758
		private static readonly Dictionary<ShipRole, float> preferredShipRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LL_Intruder,
				1f
			}
		};

		// Token: 0x04002237 RID: 8759
		private TIHabState hab;
	}
}
