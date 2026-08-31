using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000746 RID: 1862
	public class FactionGoal_BuildRefuellingStation : FactionGoal_BuildStation
	{
		// Token: 0x06002F95 RID: 12181 RVA: 0x001040B3 File Offset: 0x001022B3
		public FactionGoal_BuildRefuellingStation()
		{
		}

		// Token: 0x06002F96 RID: 12182 RVA: 0x001040BB File Offset: 0x001022BB
		public FactionGoal_BuildRefuellingStation(TIFactionState faction, int importance, TIHabState hab)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.hab = hab;
		}

		// Token: 0x06002F97 RID: 12183 RVA: 0x001040D8 File Offset: 0x001022D8
		public static FactionGoal_BuildRefuellingStation CreateGoal(FactionGoal_BuildRefuellingStation p)
		{
			FactionGoal_BuildRefuellingStation factionGoal_BuildRefuellingStation = GameStateManager.CreateNewGameState<FactionGoal_BuildRefuellingStation>();
			factionGoal_BuildRefuellingStation.hab = p.hab;
			return factionGoal_BuildRefuellingStation;
		}

		// Token: 0x06002F98 RID: 12184 RVA: 0x001040EB File Offset: 0x001022EB
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_BuildRefuellingStation>(base.ID, false);
		}

		// Token: 0x06002F99 RID: 12185 RVA: 0x001040FA File Offset: 0x001022FA
		public override GoalType GetGoalType()
		{
			return GoalType.BuildRefuellingStation;
		}

		// Token: 0x06002F9A RID: 12186 RVA: 0x00104100 File Offset: 0x00102300
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.hab == null || base.hab.archived || base.hab.faction != this.faction || this.faction.habs.Any<TIHabState>((TIHabState x) => x.location == this.location() && x.AllowsResupply(this.faction, true, true));
		}

		// Token: 0x06002F9B RID: 12187 RVA: 0x00104167 File Offset: 0x00102367
		public override bool GoalFulfilled()
		{
			return base.hab != null && base.hab.AllowsResupply(this.faction, true, false) && base.hab.numCompletedModules == TIHabState.maxModules(1);
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06002F9C RID: 12188 RVA: 0x001041A1 File Offset: 0x001023A1
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_BuildRefuellingStation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002F9D RID: 12189 RVA: 0x001041A8 File Offset: 0x001023A8
		public override List<TIHabModuleTemplate> RequiredModules()
		{
			List<TIHabModuleTemplate> list = new List<TIHabModuleTemplate>();
			list.Add((from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
				where x.allowsResupply && !x.allowsShipConstruction && x.EverAllowedForFaction(this.faction)
				select x).MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier));
			return list;
		}

		// Token: 0x06002F9E RID: 12190 RVA: 0x001041FB File Offset: 0x001023FB
		public override List<TIHabModuleTemplate> allowedModules()
		{
			return (from x in base.hab.AllowedModules(this.faction)
				where x.powerSource || (x.allowsResupply && !x.allowsShipConstruction) || x.spaceCombatModule
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x04002246 RID: 8774
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.BuildFullStation,
			GoalType.BuildSpecialtyStation
		};
	}
}
