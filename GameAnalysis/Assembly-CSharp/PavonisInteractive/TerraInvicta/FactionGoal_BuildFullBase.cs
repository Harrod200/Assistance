using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000749 RID: 1865
	public class FactionGoal_BuildFullBase : FactionGoal_BuildBase
	{
		// Token: 0x06002FB2 RID: 12210 RVA: 0x00104530 File Offset: 0x00102730
		public FactionGoal_BuildFullBase()
		{
		}

		// Token: 0x06002FB3 RID: 12211 RVA: 0x00104538 File Offset: 0x00102738
		public FactionGoal_BuildFullBase(TIFactionState faction, int importance, TIHabState hab)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.hab = hab;
		}

		// Token: 0x06002FB4 RID: 12212 RVA: 0x00104555 File Offset: 0x00102755
		public static FactionGoal_BuildFullBase CreateGoal(FactionGoal_BuildFullBase p)
		{
			FactionGoal_BuildFullBase factionGoal_BuildFullBase = GameStateManager.CreateNewGameState<FactionGoal_BuildFullBase>();
			factionGoal_BuildFullBase.hab = p.hab;
			return factionGoal_BuildFullBase;
		}

		// Token: 0x06002FB5 RID: 12213 RVA: 0x00104568 File Offset: 0x00102768
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_BuildFullBase>(base.ID, false);
		}

		// Token: 0x06002FB6 RID: 12214 RVA: 0x00104577 File Offset: 0x00102777
		public override GoalType GetGoalType()
		{
			return GoalType.BuildFullBase;
		}

		// Token: 0x06002FB7 RID: 12215 RVA: 0x0010457C File Offset: 0x0010277C
		public override bool GoalFulfilled()
		{
			if (base.hab != null && base.hab.numActiveSectors == 5 && base.hab.numCompletedModules == TIHabState.maxModules(3))
			{
				return base.hab.CompletedModules().TrueForAll((TIHabModuleState x) => x.tier == 3);
			}
			return false;
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x06002FB8 RID: 12216 RVA: 0x001045E9 File Offset: 0x001027E9
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_BuildFullBase.incompatibleHabGoals;
			}
		}

		// Token: 0x06002FB9 RID: 12217 RVA: 0x001045F0 File Offset: 0x001027F0
		public override List<TIHabModuleTemplate> RequiredModules()
		{
			return (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
				where x.mine && x.EverAllowedForFaction(this.faction)
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x06002FBA RID: 12218 RVA: 0x0010460E File Offset: 0x0010280E
		public override List<TIHabModuleTemplate> allowedModules()
		{
			return base.hab.AllowedModules(this.faction);
		}

		// Token: 0x04002249 RID: 8777
		private static readonly List<GoalType> incompatibleHabGoals = new List<GoalType>
		{
			GoalType.BuildMiningBase,
			GoalType.BuildSpecialtyBase
		};
	}
}
