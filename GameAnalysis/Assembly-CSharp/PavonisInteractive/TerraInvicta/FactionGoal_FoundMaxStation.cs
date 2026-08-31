using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000740 RID: 1856
	public class FactionGoal_FoundMaxStation : FactionGoal_FoundStation
	{
		// Token: 0x06002F47 RID: 12103 RVA: 0x00102EC5 File Offset: 0x001010C5
		public FactionGoal_FoundMaxStation()
		{
		}

		// Token: 0x06002F48 RID: 12104 RVA: 0x00102ED0 File Offset: 0x001010D0
		public FactionGoal_FoundMaxStation(TIFactionState faction, int importance, TIOrbitState orbit, GoalType buildStationGoal, List<TIHabModuleTemplate> requiredModules, GoalType defendGoal, bool setAsPrimaryHab = false, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.orbit = orbit;
			this.subsequentGoals = new List<GoalType> { buildStationGoal, defendGoal };
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
			base.setAsPrimaryHab = setAsPrimaryHab;
			this.objective = objective;
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x00102F64 File Offset: 0x00101164
		public static FactionGoal_FoundMaxStation CreateGoal(FactionGoal_FoundMaxStation p)
		{
			FactionGoal_FoundMaxStation factionGoal_FoundMaxStation = GameStateManager.CreateNewGameState<FactionGoal_FoundMaxStation>();
			factionGoal_FoundMaxStation.orbit = p.orbit;
			factionGoal_FoundMaxStation.requiredModuleNames = new List<string>(p.requiredModuleNames);
			factionGoal_FoundMaxStation.setAsPrimaryHab = p.setAsPrimaryHab;
			factionGoal_FoundMaxStation.objective = p.objective;
			return factionGoal_FoundMaxStation;
		}

		// Token: 0x06002F4A RID: 12106 RVA: 0x00102FA0 File Offset: 0x001011A0
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_FoundMaxStation>(base.ID, false);
		}

		// Token: 0x06002F4B RID: 12107 RVA: 0x00102FAF File Offset: 0x001011AF
		public override GoalType GetGoalType()
		{
			return GoalType.FoundMaxStation;
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06002F4C RID: 12108 RVA: 0x00102FB2 File Offset: 0x001011B2
		public override List<Type> spaceOperations
		{
			get
			{
				return FactionGoal_FoundMaxStation.spaceOps;
			}
		}

		// Token: 0x0400223C RID: 8764
		private static readonly List<Type> spaceOps = new List<Type>
		{
			typeof(FoundPlatformOperation),
			typeof(FoundOrbitalOperation),
			typeof(FoundRingOperation)
		};
	}
}
