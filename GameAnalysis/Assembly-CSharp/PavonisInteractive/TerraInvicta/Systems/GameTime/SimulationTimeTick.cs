using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.GameTime
{
	// Token: 0x020009AE RID: 2478
	[UpdateInGroup(typeof(PipelineStages.InputProcessStage))]
	public class SimulationTimeTick : ComponentSystem
	{
		// Token: 0x06005D74 RID: 23924 RVA: 0x002C862C File Offset: 0x002C682C
		protected override void OnStartRunning()
		{
			this.promptQueue = GameStateManager.PromptQueue();
			this.missionPhase = GameStateManager.MissionPhase();
			this.factionPlanner = GameObject.Find("AIObject").GetComponent<AIDailyFactionPlanner>();
			this.spaceCombat = GameControl.spaceCombat;
			this.precombatController = World.Active.GetExistingManager<CanvasManager>().PrecombatControllerCanvas as PrecombatController;
		}

		// Token: 0x06005D75 RID: 23925 RVA: 0x002C868C File Offset: 0x002C688C
		protected override void OnUpdate()
		{
			if (GameControl.solarSystem.enabled)
			{
				this.promptQueue.HandlePrompts();
				if (Input.anyKey)
				{
					this.anyKeyTimer = 0f;
				}
				else if (this.gameTime.Paused && !this.missionPhase.phaseActive)
				{
					this.anyKeyTimer += Time.deltaTime;
					if (this.anyKeyTimer >= 15f)
					{
						this.factionPlanner.IdleAIPlanning();
						this.anyKeyTimer = 0f;
					}
				}
				if (this.spaceCombat.HasActiveState())
				{
					return;
				}
				if (this.precombatController.Canvas.enabled)
				{
					return;
				}
				using (IEnumerator<TISpaceCombatState> enumerator = GameStateManager.IterateByClass<TISpaceCombatState>(false).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TISpaceCombatState tispaceCombatState = enumerator.Current;
						this.spaceCombat.SetCombat(tispaceCombatState);
						tispaceCombatState.StartCombatFromStrategyLayer();
						return;
					}
				}
				if (TISpaceFleetState.fleetsWaitingToInitiateCombat.Count > 0)
				{
					TISpaceFleetState.fleetsWaitingToInitiateCombat.First<TISpaceFleetState>().FinishWaitingToInitiateCombat();
					return;
				}
				if (this.promptQueue.anyActivePlayerBlocking)
				{
					this.gameTime.Pause();
					return;
				}
				float num = this.gameTime.currentSpeed * Time.deltaTime;
				if ((double)Math.Abs(num) > 0.001)
				{
					float deltaTime = this.gameTime.GetDeltaTime(num);
					this.gameTime.UpdateTime(deltaTime);
				}
				this.gameTime.UpdateEvents();
				return;
			}
			else if (TIGlobalValuesState.isSpaceCombatEnabled)
			{
				float num2 = this.gameTime.currentSpeed * Time.deltaTime;
				if ((double)Math.Abs(num2) > 0.001)
				{
					this.gameTime.UpdateTime(num2);
				}
				this.gameTime.UpdateCombatEvents();
			}
		}

		// Token: 0x040042D5 RID: 17109
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x040042D6 RID: 17110
		private float anyKeyTimer;

		// Token: 0x040042D7 RID: 17111
		private AIDailyFactionPlanner factionPlanner;

		// Token: 0x040042D8 RID: 17112
		private TIPromptQueueState promptQueue;

		// Token: 0x040042D9 RID: 17113
		private TIMissionPhaseState missionPhase;

		// Token: 0x040042DA RID: 17114
		private SpaceCombatManager spaceCombat;

		// Token: 0x040042DB RID: 17115
		private PrecombatController precombatController;
	}
}
