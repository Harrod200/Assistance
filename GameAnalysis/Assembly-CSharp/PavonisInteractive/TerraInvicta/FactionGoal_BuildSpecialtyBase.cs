using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200074B RID: 1867
	public class FactionGoal_BuildSpecialtyBase : FactionGoal_BuildBase
	{
		// Token: 0x06002FC8 RID: 12232 RVA: 0x00104794 File Offset: 0x00102994
		public FactionGoal_BuildSpecialtyBase()
		{
		}

		// Token: 0x06002FC9 RID: 12233 RVA: 0x0010479C File Offset: 0x0010299C
		public FactionGoal_BuildSpecialtyBase(TIFactionState faction, int importance, TIHabState hab, List<TIHabModuleTemplate> specialtyModules, bool setAsPrimaryHab, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.hab = hab;
			this.specialtyModuleDataNames = specialtyModules.Select<TIHabModuleTemplate, string>((TIHabModuleTemplate x) => x.dataName).ToList<string>();
			this.setAsPrimaryHab = setAsPrimaryHab;
			this.objective = objective;
		}

		// Token: 0x06002FCA RID: 12234 RVA: 0x00104808 File Offset: 0x00102A08
		public static FactionGoal_BuildSpecialtyBase CreateGoal(FactionGoal_BuildSpecialtyBase p)
		{
			FactionGoal_BuildSpecialtyBase factionGoal_BuildSpecialtyBase = GameStateManager.CreateNewGameState<FactionGoal_BuildSpecialtyBase>();
			factionGoal_BuildSpecialtyBase.hab = p.hab;
			factionGoal_BuildSpecialtyBase.specialtyModuleDataNames = new List<string>(p.specialtyModuleDataNames);
			factionGoal_BuildSpecialtyBase.setAsPrimaryHab = p.setAsPrimaryHab;
			if (factionGoal_BuildSpecialtyBase.setAsPrimaryHab)
			{
				p.faction.primaryHab = factionGoal_BuildSpecialtyBase.hab;
			}
			factionGoal_BuildSpecialtyBase.objective = p.objective;
			return factionGoal_BuildSpecialtyBase;
		}

		// Token: 0x06002FCB RID: 12235 RVA: 0x0010486A File Offset: 0x00102A6A
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_BuildSpecialtyBase>(base.ID, false);
		}

		// Token: 0x06002FCC RID: 12236 RVA: 0x00104879 File Offset: 0x00102A79
		public override GoalType GetGoalType()
		{
			return GoalType.BuildSpecialtyBase;
		}

		// Token: 0x06002FCD RID: 12237 RVA: 0x0010487D File Offset: 0x00102A7D
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.hab == null || base.hab.archived || base.hab.faction != this.faction;
		}

		// Token: 0x06002FCE RID: 12238 RVA: 0x001048BC File Offset: 0x00102ABC
		public override bool GoalFulfilled()
		{
			if (base.objectiveGoal)
			{
				return false;
			}
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

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x06002FCF RID: 12239 RVA: 0x00104946 File Offset: 0x00102B46
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_BuildSpecialtyBase.incompatibleHabGoals;
			}
		}

		// Token: 0x06002FD0 RID: 12240 RVA: 0x0010494D File Offset: 0x00102B4D
		public override List<TIHabModuleTemplate> RequiredModules()
		{
			return base.specialModules;
		}

		// Token: 0x06002FD1 RID: 12241 RVA: 0x00104955 File Offset: 0x00102B55
		public override List<TIHabModuleTemplate> allowedModules()
		{
			return (from x in base.hab.AllowedModules(this.faction)
				where x.coreModule || x.powerSource || x.mine || x.spaceCombatModule || x.CombatTroops() || base.specialModules.Contains(x)
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x0400224B RID: 8779
		public bool setAsPrimaryHab;

		// Token: 0x0400224C RID: 8780
		private static readonly List<GoalType> incompatibleHabGoals = new List<GoalType>
		{
			GoalType.BuildFullBase,
			GoalType.BuildMiningBase
		};
	}
}
