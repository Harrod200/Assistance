using System;
using System.Linq;
using PavonisInteractive.TerraInvicta.GamePlayScript.Systems;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems.PeriodicUpdates
{
	// Token: 0x020009A6 RID: 2470
	[UpdateInGroup(typeof(PipelineStages.SimulationStage))]
	public class FactionPeriodicUpdate : StrategyLayerComponentSystem
	{
		// Token: 0x17000FF3 RID: 4083
		// (get) Token: 0x06005D15 RID: 23829 RVA: 0x002C639E File Offset: 0x002C459E
		// (set) Token: 0x06005D16 RID: 23830 RVA: 0x002C63A6 File Offset: 0x002C45A6
		public AIDailyFactionPlanner factionPlanner { get; private set; }

		// Token: 0x06005D17 RID: 23831 RVA: 0x002C63B0 File Offset: 0x002C45B0
		public override void Initialize()
		{
			this.daily0000Condition = GameTimeCondition.Daily0000(this.gameTime.Now);
			this.daily0300Condition = GameTimeCondition.Daily0300(this.gameTime.Now);
			this.daily0600Condition = GameTimeCondition.Daily0600(this.gameTime.Now);
			this.daily0900Condition = GameTimeCondition.Daily0900(this.gameTime.Now);
			this.daily1500Condition = GameTimeCondition.Daily1500(this.gameTime.Now);
			this.daily1800Condition = GameTimeCondition.Daily1800(this.gameTime.Now);
			this.daily2100Condition = GameTimeCondition.Daily2100(this.gameTime.Now);
			this.daily2300Condition = GameTimeCondition.Daily2300(this.gameTime.Now);
			this.monthlyCondition = GameTimeCondition.Monthly(this.gameTime.Now);
			this.midMonthlyCondition = GameTimeCondition.MidMonthly(this.gameTime.Now);
			this.global = GameStateManager.GlobalValues();
			this.factionStates = GameStateManager.AllFactions();
			this.factionPlanner = AIDailyFactionPlanner.singleton;
			this.factionPlanner.Initialize();
		}

		// Token: 0x06005D18 RID: 23832 RVA: 0x002C64C8 File Offset: 0x002C46C8
		protected override void OnUpdate()
		{
			if (this.daily0000Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily0000Update();
			}
			if (this.daily0300Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily0300Update();
			}
			if (this.daily0600Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily0600Update();
			}
			if (this.daily0900Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily0900Update();
			}
			if (this.daily1500Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily1500Update();
			}
			if (this.daily1800Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily1800Update();
			}
			if (this.daily2100Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily2100Update();
			}
			if (this.daily2300Condition.Satisfied(this.gameTime.Now))
			{
				this.OnDaily2300Update();
			}
			if (this.monthlyCondition.Satisfied(this.gameTime.Now))
			{
				this.OnMonthlyUpdate();
				return;
			}
			if (this.midMonthlyCondition.Satisfied(this.gameTime.Now))
			{
				this.OnMidMonthlyUpdate();
			}
		}

		// Token: 0x06005D19 RID: 23833 RVA: 0x002C6604 File Offset: 0x002C4804
		private void OnDaily0000Update()
		{
			GameStateManager.Time().AddDayToCampaign();
			for (int i = 0; i < this.factionStates.Length; i++)
			{
				this.factionStates[i].Daily0000FactionUpdate();
			}
			this.factionPlanner.FactionOperations0000();
		}

		// Token: 0x06005D1A RID: 23834 RVA: 0x002C6646 File Offset: 0x002C4846
		private void OnDaily0300Update()
		{
			this.factionPlanner.FactionOperations(true);
		}

		// Token: 0x06005D1B RID: 23835 RVA: 0x002C6654 File Offset: 0x002C4854
		private void OnDaily0600Update()
		{
			this.factionPlanner.FactionOperations(true);
		}

		// Token: 0x06005D1C RID: 23836 RVA: 0x002C6662 File Offset: 0x002C4862
		private void OnDaily0900Update()
		{
			this.factionPlanner.FactionOperations(true);
		}

		// Token: 0x06005D1D RID: 23837 RVA: 0x002C6670 File Offset: 0x002C4870
		private void OnDaily1500Update()
		{
			this.factionPlanner.FactionOperations(false);
		}

		// Token: 0x06005D1E RID: 23838 RVA: 0x002C667E File Offset: 0x002C487E
		private void OnDaily1800Update()
		{
			this.factionPlanner.FactionOperations(false);
		}

		// Token: 0x06005D1F RID: 23839 RVA: 0x002C668C File Offset: 0x002C488C
		private void OnDaily2100Update()
		{
			this.factionPlanner.FactionOperations(false);
		}

		// Token: 0x06005D20 RID: 23840 RVA: 0x002C669A File Offset: 0x002C489A
		private void OnDaily2300Update()
		{
			this.factionPlanner.FactionOperations(false);
		}

		// Token: 0x06005D21 RID: 23841 RVA: 0x002C66A8 File Offset: 0x002C48A8
		private void OnAnnualUpdate()
		{
			foreach (TIOrbitState tiorbitState in GameStateManager.AllOrbits())
			{
				if (tiorbitState.destroyedAssets > 0)
				{
					if (tiorbitState.destroyedAssets < 5)
					{
						if (TIUtilities.RandomFloatValue() < 0.25f)
						{
							tiorbitState.destroyedAssets--;
						}
					}
					else
					{
						tiorbitState.destroyedAssets *= (int)TIUtilities.RandomRange(0.85f, 1f);
					}
				}
			}
		}

		// Token: 0x06005D22 RID: 23842 RVA: 0x002C6718 File Offset: 0x002C4918
		private void OnMonthlyUpdate()
		{
			this.global.MonthlyGlobalEnvironmentalChanges();
			GameStateManager.Effects().GlobalCheckForRemoveEffects();
			foreach (TIFactionState tifactionState in this.factionStates.ToList<TIFactionState>().Shuffle<TIFactionState>())
			{
				tifactionState.MonthlyFactionUpdate();
			}
			GameControl.eventManager.TriggerEvent(new RecruitListsUpdated(), null, Array.Empty<object>());
			this.global.NarrativeEventsMonthlyUpdate();
			if (this.gameTime.currentTime.month == 1)
			{
				this.OnAnnualUpdate();
			}
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x002C67C0 File Offset: 0x002C49C0
		private void OnMidMonthlyUpdate()
		{
			GameStateManager.Effects().GlobalCheckForRemoveEffects();
			foreach (TIFactionState tifactionState in this.factionStates.ToList<TIFactionState>().Shuffle<TIFactionState>())
			{
				tifactionState.MidMonthlyUpdate();
			}
			this.global.CleanUpOrgs();
		}

		// Token: 0x040042A4 RID: 17060
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x040042A5 RID: 17061
		private GameTimeCondition daily0000Condition;

		// Token: 0x040042A6 RID: 17062
		private GameTimeCondition daily0300Condition;

		// Token: 0x040042A7 RID: 17063
		private GameTimeCondition daily0600Condition;

		// Token: 0x040042A8 RID: 17064
		private GameTimeCondition daily0900Condition;

		// Token: 0x040042A9 RID: 17065
		private GameTimeCondition daily1500Condition;

		// Token: 0x040042AA RID: 17066
		private GameTimeCondition daily1800Condition;

		// Token: 0x040042AB RID: 17067
		private GameTimeCondition daily2100Condition;

		// Token: 0x040042AC RID: 17068
		private GameTimeCondition daily2300Condition;

		// Token: 0x040042AD RID: 17069
		private GameTimeCondition monthlyCondition;

		// Token: 0x040042AE RID: 17070
		private GameTimeCondition midMonthlyCondition;

		// Token: 0x040042AF RID: 17071
		private TIGlobalValuesState global;

		// Token: 0x040042B0 RID: 17072
		private TIFactionState[] factionStates;

		// Token: 0x02001351 RID: 4945
		public struct FactionGroup
		{
			// Token: 0x04006FBD RID: 28605
			public readonly int Length;

			// Token: 0x04006FBE RID: 28606
			public ComponentDataArray<Faction> Faction;
		}
	}
}
