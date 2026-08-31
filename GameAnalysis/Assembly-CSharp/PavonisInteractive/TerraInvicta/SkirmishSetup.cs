using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007E1 RID: 2017
	public class SkirmishSetup : MonoBehaviour
	{
		// Token: 0x060048D7 RID: 18647 RVA: 0x001DF03E File Offset: 0x001DD23E
		public void Awake()
		{
			if (GameControl.control.skirmishMode)
			{
				GameControl.eventManager.AddListener<StartupComplete>(new EventManager.EventDelegate<StartupComplete>(this.OnStartupComplete), null, null, true, false);
				return;
			}
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060048D8 RID: 18648 RVA: 0x001DF072 File Offset: 0x001DD272
		public void OnStartupComplete(StartupComplete e)
		{
			base.StartCoroutine(this.StartCombatSetup(e.scenario));
		}

		// Token: 0x060048D9 RID: 18649 RVA: 0x001DF087 File Offset: 0x001DD287
		private IEnumerator StartCombatSetup(IScenario scenario)
		{
			List<TISpaceFleetState> list = GameStateManager.IterateByClass<TISpaceFleetState>(false).ToList<TISpaceFleetState>();
			TISpaceCombatState combatState = GameStateManager.CreateNewGameState<TISpaceCombatState>();
			SkirmishModeScenario skirmishModeScenario = scenario as SkirmishModeScenario;
			TIHabState tihabState = null;
			if (skirmishModeScenario.habTemplate != null)
			{
				tihabState = GameStateManager.FindByTemplate<TIHabState>(skirmishModeScenario.habTemplate.dataName, false);
			}
			GameControl.spaceCombat.SetCombat(combatState);
			combatState.InitializeCombat(list[0], list[1], tihabState);
			while (GameControl.control == null || GameControl.control.viewMgr == null || !GameControl.loadcycle100)
			{
				yield return this.wait;
			}
			foreach (TISpaceFleetState tispaceFleetState in combatState.fleets)
			{
				if (combatState.stances.ContainsKey(tispaceFleetState.faction))
				{
					combatState.stances[tispaceFleetState.faction] = CombatStance.Pursue;
				}
				else
				{
					combatState.stances.Add(tispaceFleetState.faction, CombatStance.Pursue);
				}
			}
			combatState.active = true;
			GameControl.spaceCombat.initialized = false;
			GameControl.control.viewMgr.DisableSolarSystemForSkirmishMode(scenario);
			GameControl.control.viewMgr.GotoView(ViewType.SpaceCombat);
			global::UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}

		// Token: 0x04002A66 RID: 10854
		private readonly WaitForEndOfFrame wait = new WaitForEndOfFrame();
	}
}
