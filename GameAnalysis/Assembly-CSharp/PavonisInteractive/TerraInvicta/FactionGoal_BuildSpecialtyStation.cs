using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000747 RID: 1863
	public class FactionGoal_BuildSpecialtyStation : FactionGoal_BuildStation
	{
		// Token: 0x06002FA2 RID: 12194 RVA: 0x00104297 File Offset: 0x00102497
		public FactionGoal_BuildSpecialtyStation()
		{
		}

		// Token: 0x06002FA3 RID: 12195 RVA: 0x001042A0 File Offset: 0x001024A0
		public FactionGoal_BuildSpecialtyStation(TIFactionState faction, int importance, TIHabState hab, List<TIHabModuleTemplate> specialtyModules, bool setAsPrimaryHab, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.hab = hab;
			this.setAsPrimaryHab = setAsPrimaryHab;
			this.specialtyModuleDataNames = specialtyModules.Select<TIHabModuleTemplate, string>((TIHabModuleTemplate x) => x.dataName).ToList<string>();
			this.objective = objective;
		}

		// Token: 0x06002FA4 RID: 12196 RVA: 0x0010430C File Offset: 0x0010250C
		public static FactionGoal_BuildSpecialtyStation CreateGoal(FactionGoal_BuildSpecialtyStation p)
		{
			FactionGoal_BuildSpecialtyStation factionGoal_BuildSpecialtyStation = GameStateManager.CreateNewGameState<FactionGoal_BuildSpecialtyStation>();
			factionGoal_BuildSpecialtyStation.hab = p.hab;
			factionGoal_BuildSpecialtyStation.specialtyModuleDataNames = new List<string>(p.specialtyModuleDataNames);
			factionGoal_BuildSpecialtyStation.setAsPrimaryHab = p.setAsPrimaryHab;
			if (factionGoal_BuildSpecialtyStation.setAsPrimaryHab)
			{
				p.faction.primaryHab = factionGoal_BuildSpecialtyStation.hab;
			}
			factionGoal_BuildSpecialtyStation.objective = p.objective;
			return factionGoal_BuildSpecialtyStation;
		}

		// Token: 0x06002FA5 RID: 12197 RVA: 0x0010436E File Offset: 0x0010256E
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_BuildSpecialtyStation>(base.ID, false);
		}

		// Token: 0x06002FA6 RID: 12198 RVA: 0x0010437D File Offset: 0x0010257D
		public override GoalType GetGoalType()
		{
			return GoalType.BuildSpecialtyStation;
		}

		// Token: 0x06002FA7 RID: 12199 RVA: 0x00104381 File Offset: 0x00102581
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.hab == null || base.hab.archived || base.hab.faction != this.faction;
		}

		// Token: 0x06002FA8 RID: 12200 RVA: 0x001043C0 File Offset: 0x001025C0
		public override bool GoalFulfilled()
		{
			if (base.hab != null)
			{
				List<TIHabModuleState> list = base.hab.CompletedModules();
				if (base.hab.numActiveSectors == 5 && list.Count == TIHabState.maxModules(3))
				{
					if (list.TrueForAll((TIHabModuleState x) => x.tier == 3))
					{
						return list.Any<TIHabModuleState>((TIHabModuleState x) => base.specialModules.Contains(x.moduleTemplate));
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06002FA9 RID: 12201 RVA: 0x00104440 File Offset: 0x00102640
		public override List<TIHabModuleTemplate> RequiredModules()
		{
			return base.specialModules;
		}

		// Token: 0x06002FAA RID: 12202 RVA: 0x00104448 File Offset: 0x00102648
		public override List<TIHabModuleTemplate> allowedModules()
		{
			return (from x in base.hab.AllowedModules(this.faction)
				where x.coreModule || x.powerSource || x.spaceCombatModule || x.CombatTroops() || base.specialModules.Contains(x)
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x06002FAB RID: 12203 RVA: 0x00104471 File Offset: 0x00102671
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_BuildSpecialtyStation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x04002247 RID: 8775
		public bool setAsPrimaryHab;

		// Token: 0x04002248 RID: 8776
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.BuildFullStation,
			GoalType.BuildRefuellingStation
		};
	}
}
