using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200073F RID: 1855
	public class FactionGoal_FoundPlatform : FactionGoal_FoundStation
	{
		// Token: 0x06002F40 RID: 12096 RVA: 0x00102DDE File Offset: 0x00100FDE
		public FactionGoal_FoundPlatform()
		{
		}

		// Token: 0x06002F41 RID: 12097 RVA: 0x00102DE8 File Offset: 0x00100FE8
		public FactionGoal_FoundPlatform(TIFactionState faction, int importance, TIOrbitState orbit, GoalType buildStationGoal, List<TIHabModuleTemplate> requiredModules, GoalType defendGoal)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.orbit = orbit;
			List<string> list;
			if (requiredModules == null)
			{
				list = null;
			}
			else
			{
				list = requiredModules.Select<TIHabModuleTemplate, string>((TIHabModuleTemplate x) => x.dataName).ToList<string>();
			}
			this.requiredModuleNames = list ?? new List<string>();
			this.subsequentGoals = new List<GoalType> { buildStationGoal, defendGoal };
		}

		// Token: 0x06002F42 RID: 12098 RVA: 0x00102E6C File Offset: 0x0010106C
		public static FactionGoal_FoundPlatform CreateGoal(FactionGoal_FoundPlatform p)
		{
			FactionGoal_FoundPlatform factionGoal_FoundPlatform = GameStateManager.CreateNewGameState<FactionGoal_FoundPlatform>();
			factionGoal_FoundPlatform.orbit = p.orbit;
			factionGoal_FoundPlatform.requiredModuleNames = new List<string>(p.requiredModuleNames);
			return factionGoal_FoundPlatform;
		}

		// Token: 0x06002F43 RID: 12099 RVA: 0x00102E90 File Offset: 0x00101090
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_FoundPlatform>(base.ID, false);
		}

		// Token: 0x06002F44 RID: 12100 RVA: 0x00102E9F File Offset: 0x0010109F
		public override GoalType GetGoalType()
		{
			return GoalType.FoundPlatform;
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x06002F45 RID: 12101 RVA: 0x00102EA2 File Offset: 0x001010A2
		public override List<Type> spaceOperations
		{
			get
			{
				return FactionGoal_FoundPlatform.spaceOps;
			}
		}

		// Token: 0x0400223B RID: 8763
		private static readonly List<Type> spaceOps = new List<Type> { typeof(FoundPlatformOperation) };
	}
}
