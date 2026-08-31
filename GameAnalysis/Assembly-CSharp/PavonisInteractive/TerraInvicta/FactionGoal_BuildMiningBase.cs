using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200074A RID: 1866
	public class FactionGoal_BuildMiningBase : FactionGoal_BuildBase
	{
		// Token: 0x06002FBD RID: 12221 RVA: 0x00104654 File Offset: 0x00102854
		public FactionGoal_BuildMiningBase()
		{
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x0010465C File Offset: 0x0010285C
		public FactionGoal_BuildMiningBase(TIFactionState faction, int importance, TIHabState hab)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.hab = hab;
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x00104679 File Offset: 0x00102879
		public static FactionGoal_BuildMiningBase CreateGoal(FactionGoal_BuildMiningBase p)
		{
			FactionGoal_BuildMiningBase factionGoal_BuildMiningBase = GameStateManager.CreateNewGameState<FactionGoal_BuildMiningBase>();
			factionGoal_BuildMiningBase.hab = p.hab;
			return factionGoal_BuildMiningBase;
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x0010468C File Offset: 0x0010288C
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_BuildMiningBase>(base.ID, false);
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x06002FC1 RID: 12225 RVA: 0x0010469B File Offset: 0x0010289B
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_BuildMiningBase.incompatibleHabGoals;
			}
		}

		// Token: 0x06002FC2 RID: 12226 RVA: 0x001046A2 File Offset: 0x001028A2
		public override GoalType GetGoalType()
		{
			return GoalType.BuildMiningBase;
		}

		// Token: 0x06002FC3 RID: 12227 RVA: 0x001046A8 File Offset: 0x001028A8
		public override bool GoalFulfilled()
		{
			if (base.hab != null && base.hab.HasMine)
			{
				return base.hab.ActiveModules().FirstOrDefault<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.mine && x.tier == 3) != null;
			}
			return false;
		}

		// Token: 0x06002FC4 RID: 12228 RVA: 0x00104707 File Offset: 0x00102907
		public override List<TIHabModuleTemplate> RequiredModules()
		{
			return (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
				where x.mine && x.EverAllowedForFaction(this.faction)
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x06002FC5 RID: 12229 RVA: 0x00104725 File Offset: 0x00102925
		public override List<TIHabModuleTemplate> allowedModules()
		{
			return (from x in base.hab.AllowedModules(this.faction)
				where x.coreModule || x.mine || x.powerSource || (x.allowsResupply && !x.allowsShipConstruction) || x.spaceCombatModule
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x0400224A RID: 8778
		private static readonly List<GoalType> incompatibleHabGoals = new List<GoalType>
		{
			GoalType.BuildFullBase,
			GoalType.BuildSpecialtyBase
		};
	}
}
