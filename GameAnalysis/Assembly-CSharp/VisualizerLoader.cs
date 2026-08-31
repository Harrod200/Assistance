using System;
using System.Collections;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class VisualizerLoader : MonoBehaviour
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060001DD RID: 477 RVA: 0x0000F352 File Offset: 0x0000D552
	private GameControl control
	{
		get
		{
			return GameControl.control;
		}
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000F359 File Offset: 0x0000D559
	public void Initialize(bool loadingSave, IScenario scenario)
	{
		this.loadingSave = loadingSave;
		this.scenario = scenario;
		if (this.control.skirmishMode)
		{
			this.InitVisualizersSkirmish();
			return;
		}
		base.StartCoroutine(this.InitVisualizersCampaign());
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000F38C File Offset: 0x0000D58C
	private void Update()
	{
		if (GameControl.loadcycle100)
		{
			foreach (ScriptBehaviourManager scriptBehaviourManager in World.Active.BehaviourManagers)
			{
				ComponentSystemBase componentSystemBase = scriptBehaviourManager as ComponentSystemBase;
				if (componentSystemBase != null)
				{
					componentSystemBase.Enabled = !GameControl.control.skirmishMode || !(scriptBehaviourManager is StrategyLayerComponentSystem);
				}
			}
			base.StartCoroutine(this.CatchUp());
			if (!this.loadingSave && !this.control.skirmishMode)
			{
				World.Active.GetExistingManager<SpaceObjectPositioning>().TriggerForceUpdate();
			}
			base.StartCoroutine(this.CatchUp());
			this.control.CompleteInit(this.loadingSave, this.scenario);
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000F460 File Offset: 0x0000D660
	private IEnumerator CatchUp()
	{
		int num;
		for (int i = 0; i < 5; i = num + 1)
		{
			yield return this.frameWait;
			num = i;
		}
		yield break;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000F470 File Offset: 0x0000D670
	private void InitVisualizersCampaign_Editor()
	{
		foreach (TIGameState tigameState in GameStateManager.IterateByClass<TIGameState>(true))
		{
			IGameStateVisualizer gameStateVisualizer = tigameState as IGameStateVisualizer;
			if (gameStateVisualizer != null && gameStateVisualizer != null)
			{
				gameStateVisualizer.CreateVisualizer(tigameState.GetMyTemplate<TIDataTemplate>());
			}
		}
		this.CompleteInitVisualizer();
		Debug.Log("Initial Loading Cycle Completed");
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000F4E0 File Offset: 0x0000D6E0
	private IEnumerator InitVisualizersCampaign()
	{
		foreach (TIGameState gameState in GameStateManager.IterateByClass<TIGameState>(true))
		{
			IGameStateVisualizer gameStateVis = gameState as IGameStateVisualizer;
			if (gameStateVis != null)
			{
				yield return this.smallWait;
				IGameStateVisualizer gameStateVisualizer = gameStateVis;
				if (gameStateVisualizer != null)
				{
					gameStateVisualizer.CreateVisualizer(gameState.GetMyTemplate<TIDataTemplate>());
				}
			}
			gameStateVis = null;
			gameState = null;
		}
		IEnumerator<TIGameState> enumerator = null;
		this.CompleteInitVisualizer();
		Debug.Log("Initial Loading Cycle Completed");
		yield break;
		yield break;
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000F4F0 File Offset: 0x0000D6F0
	private void InitVisualizersSkirmish()
	{
		foreach (TIGameState tigameState in GameStateManager.IterateByClass<TIGameState>(true))
		{
			IGameStateVisualizer gameStateVisualizer = tigameState as IGameStateVisualizer;
			if (gameStateVisualizer != null && gameStateVisualizer != null)
			{
				gameStateVisualizer.CreateVisualizer(tigameState.GetMyTemplate<TIDataTemplate>());
			}
		}
		this.CompleteInitVisualizer();
		Debug.Log("Initial Skirmish Loading Cycle Completed");
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000F560 File Offset: 0x0000D760
	private void CompleteInitVisualizer()
	{
		Shader.WarmupAllShaders();
		Log.Time("<color=#00cc00>LoadTime:</color> PostVisualizerInit6", delegate
		{
			foreach (TIGameState tigameState in GameStateManager.IterateByClass<TIGameState>(true))
			{
				tigameState.PostVisualizerCreationInit_6();
			}
		}, true, true);
		Log.Time("<color=#00cc00>LoadTime:</color> PostVisualizerInit7", delegate
		{
			foreach (TIGameState tigameState2 in GameStateManager.IterateByClass<TIGameState>(true))
			{
				tigameState2.PostVisualizerCreationInit_7();
			}
		}, true, true);
		GameControl.CreateAndDestroyStartupGameStates();
		Log.Info("Number of GameStates: " + GameStateManager.GetAllGameStates<TIGameState>(true).Length.ToString(), Array.Empty<object>());
		this.control.UpdateLoading(100f);
		GameControl.loadcycle100 = true;
	}

	// Token: 0x040001FA RID: 506
	private bool loadingSave;

	// Token: 0x040001FB RID: 507
	private IScenario scenario;

	// Token: 0x040001FC RID: 508
	private readonly WaitForSeconds smallWait = new WaitForSeconds(0.001f);

	// Token: 0x040001FD RID: 509
	private readonly WaitForEndOfFrame frameWait = new WaitForEndOfFrame();
}
