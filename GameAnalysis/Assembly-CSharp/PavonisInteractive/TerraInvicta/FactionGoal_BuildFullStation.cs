using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000745 RID: 1861
	public class FactionGoal_BuildFullStation : FactionGoal_BuildStation
	{
		// Token: 0x06002F8D RID: 12173 RVA: 0x00103FF3 File Offset: 0x001021F3
		public FactionGoal_BuildFullStation()
		{
		}

		// Token: 0x06002F8E RID: 12174 RVA: 0x00103FFB File Offset: 0x001021FB
		public FactionGoal_BuildFullStation(TIFactionState faction, int importance, TIHabState hab)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.hab = hab;
		}

		// Token: 0x06002F8F RID: 12175 RVA: 0x00104018 File Offset: 0x00102218
		public static FactionGoal_BuildFullStation CreateGoal(FactionGoal_BuildFullStation p)
		{
			FactionGoal_BuildFullStation factionGoal_BuildFullStation = GameStateManager.CreateNewGameState<FactionGoal_BuildFullStation>();
			factionGoal_BuildFullStation.hab = p.hab;
			return factionGoal_BuildFullStation;
		}

		// Token: 0x06002F90 RID: 12176 RVA: 0x0010402B File Offset: 0x0010222B
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_BuildFullStation>(base.ID, false);
		}

		// Token: 0x06002F91 RID: 12177 RVA: 0x0010403A File Offset: 0x0010223A
		public override List<TIHabModuleTemplate> RequiredModules()
		{
			return (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
				where x.allowsShipConstruction && x.EverAllowedForFaction(this.faction) && x.tier == base.hab.tier
				select x).ToList<TIHabModuleTemplate>();
		}

		// Token: 0x06002F92 RID: 12178 RVA: 0x00104058 File Offset: 0x00102258
		public override List<TIHabModuleTemplate> allowedModules()
		{
			return base.hab.AllowedModules(this.faction);
		}

		// Token: 0x04002245 RID: 8773
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.BuildRefuellingStation,
			GoalType.BuildSpecialtyStation
		};
	}
}
