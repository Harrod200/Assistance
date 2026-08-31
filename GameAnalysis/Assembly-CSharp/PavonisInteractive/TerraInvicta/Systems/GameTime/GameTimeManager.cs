using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.GameTime
{
	// Token: 0x020009AC RID: 2476
	[UpdateInGroup(typeof(PipelineStages.InputProcessStage))]
	public class GameTimeManager : ManagerSystem
	{
		// Token: 0x17000FF4 RID: 4084
		// (get) Token: 0x06005D37 RID: 23863 RVA: 0x002C7A31 File Offset: 0x002C5C31
		// (set) Token: 0x06005D38 RID: 23864 RVA: 0x002C7A38 File Offset: 0x002C5C38
		public static GameTimeManager Singleton { get; private set; }

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06005D39 RID: 23865 RVA: 0x002C7A40 File Offset: 0x002C5C40
		// (remove) Token: 0x06005D3A RID: 23866 RVA: 0x002C7A78 File Offset: 0x002C5C78
		public event GameTimeManager.SpeedChangeCallback SpeedChanged;

		// Token: 0x17000FF5 RID: 4085
		// (get) Token: 0x06005D3B RID: 23867 RVA: 0x002C7AAD File Offset: 0x002C5CAD
		// (set) Token: 0x06005D3C RID: 23868 RVA: 0x002C7AB5 File Offset: 0x002C5CB5
		public DateTime Now { get; private set; }

		// Token: 0x17000FF6 RID: 4086
		// (get) Token: 0x06005D3D RID: 23869 RVA: 0x002C7ABE File Offset: 0x002C5CBE
		public TIDateTime currentTime
		{
			get
			{
				return this.timeState.Time_Now();
			}
		}

		// Token: 0x17000FF7 RID: 4087
		// (get) Token: 0x06005D3E RID: 23870 RVA: 0x002C7ACB File Offset: 0x002C5CCB
		public bool IsTimeFlowing
		{
			get
			{
				return this.currentSpeed > 0f;
			}
		}

		// Token: 0x17000FF8 RID: 4088
		// (get) Token: 0x06005D3F RID: 23871 RVA: 0x002C7ADA File Offset: 0x002C5CDA
		public string SpaceCombatPausedText
		{
			get
			{
				return this.combatSpeeds[0].description;
			}
		}

		// Token: 0x17000FF9 RID: 4089
		// (get) Token: 0x06005D40 RID: 23872 RVA: 0x002C7AED File Offset: 0x002C5CED
		public bool IsBlocked
		{
			get
			{
				return this.blocked || this.promptQueue.anyBlocking;
			}
		}

		// Token: 0x17000FFA RID: 4090
		// (get) Token: 0x06005D41 RID: 23873 RVA: 0x002C7B04 File Offset: 0x002C5D04
		public bool isBlockedByPrompt
		{
			get
			{
				return this.promptQueue.anyBlocking;
			}
		}

		// Token: 0x17000FFB RID: 4091
		// (get) Token: 0x06005D42 RID: 23874 RVA: 0x002C7B11 File Offset: 0x002C5D11
		// (set) Token: 0x06005D43 RID: 23875 RVA: 0x002C7B19 File Offset: 0x002C5D19
		public TITimeQueue timeQueue { get; private set; }

		// Token: 0x17000FFC RID: 4092
		// (get) Token: 0x06005D44 RID: 23876 RVA: 0x002C7B22 File Offset: 0x002C5D22
		// (set) Token: 0x06005D45 RID: 23877 RVA: 0x002C7B2A File Offset: 0x002C5D2A
		public List<SpeedSetting> currentSpeeds { get; private set; }

		// Token: 0x17000FFD RID: 4093
		// (get) Token: 0x06005D46 RID: 23878 RVA: 0x002C7B33 File Offset: 0x002C5D33
		// (set) Token: 0x06005D47 RID: 23879 RVA: 0x002C7B3B File Offset: 0x002C5D3B
		public int currentSpeedIndex { get; private set; }

		// Token: 0x17000FFE RID: 4094
		// (get) Token: 0x06005D48 RID: 23880 RVA: 0x002C7B44 File Offset: 0x002C5D44
		// (set) Token: 0x06005D49 RID: 23881 RVA: 0x002C7B4C File Offset: 0x002C5D4C
		public float currentSpeed { get; private set; }

		// Token: 0x17000FFF RID: 4095
		// (get) Token: 0x06005D4A RID: 23882 RVA: 0x002C7B55 File Offset: 0x002C5D55
		public float lastSpeed
		{
			get
			{
				return this.currentSpeeds[this.lastSpeedIndex].multiplier;
			}
		}

		// Token: 0x06005D4B RID: 23883 RVA: 0x002C7B6D File Offset: 0x002C5D6D
		private void OnSpeedChanged(SpeedSetting speed)
		{
			GameTimeManager.SpeedChangeCallback speedChanged = this.SpeedChanged;
			if (speedChanged != null)
			{
				speedChanged(speed);
			}
			GameControl.eventManager.TriggerEvent(new GameTimeSpeedChanged(), null, Array.Empty<object>());
		}

		// Token: 0x06005D4C RID: 23884 RVA: 0x002C7B96 File Offset: 0x002C5D96
		internal float GetDeltaTime(float deltaTime)
		{
			return this.timeQueue.GetDeltaTime(deltaTime, this.Now);
		}

		// Token: 0x06005D4D RID: 23885 RVA: 0x002C7BAA File Offset: 0x002C5DAA
		internal void UpdateTime(float deltaTime)
		{
			this.timeState.UpdateCurrentDateTime((double)deltaTime);
			this.Now = this.timeState.Time_SystemNow();
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x002C7BCC File Offset: 0x002C5DCC
		internal void SetTime(TIDateTime time)
		{
			this.timeState.SetCurrentDateTime(time.year, time.month, time.day, time.hour, time.minute, time.second, time.millisecond);
			this.Now = this.timeState.Time_SystemNow();
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x002C7C1F File Offset: 0x002C5E1F
		internal void UpdateEvents()
		{
			this.timeQueue.UpdateToTime(this.Now);
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x002C7C32 File Offset: 0x002C5E32
		internal void UpdateCombatEvents()
		{
			this.combatTimeQueue.UpdateToTime(this.Now);
		}

		// Token: 0x06005D51 RID: 23889 RVA: 0x002C7C45 File Offset: 0x002C5E45
		public void Initialize()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> Initialize GameTime", delegate
			{
				GameTimeManager.Singleton = this;
				this.timeState = GameStateManager.Time();
				this.Now = this.timeState.Time_SystemNow();
				this.timeQueue = new TITimeQueue();
				this.timeQueue.Initialize();
				this.combatTimeQueue = new TITimeQueue();
				this.combatTimeQueue.Initialize();
				this.promptQueue = GameStateManager.FindGameState<TIPromptQueueState>();
				TITimeEvent[] allGameStates = GameStateManager.GetAllGameStates<TITimeEvent>(true);
				List<TITimeEvent> list = new List<TITimeEvent>();
				foreach (TITimeEvent titimeEvent in allGameStates)
				{
					if (!titimeEvent.isComplete)
					{
						this.timeQueue.AddEvent(titimeEvent);
					}
					else
					{
						list.Add(titimeEvent);
					}
					if (titimeEvent.eventName == "NarrativeEvent" && titimeEvent.eventDataTemplate == null && !list.Contains(titimeEvent))
					{
						list.Add(titimeEvent);
					}
				}
				foreach (TITimeEvent titimeEvent2 in list)
				{
					this.timeQueue.CancelEvent(titimeEvent2.eventName, titimeEvent2.eventObject, titimeEvent2.eventObject2, titimeEvent2.eventDataTemplateName, titimeEvent2.time);
					if (!titimeEvent2.deleted)
					{
						GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent2.ID, false);
					}
				}
				List<int> strategyLayerSpeedSettings = TemplateManager.global.strategyLayerSpeedSettings;
				List<int> combatLayerSpeedSettings = TemplateManager.global.combatLayerSpeedSettings;
				this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
				this.strategySpeeds.Clear();
				for (int j = 0; j < strategyLayerSpeedSettings.Count; j++)
				{
					SpeedSetting speedSetting = default(SpeedSetting);
					speedSetting.multiplier = (float)strategyLayerSpeedSettings[j];
					speedSetting.description = Loc.T("UI.GeneralControls.StrategySpeed" + j.ToString());
					this.strategySpeeds.Add(speedSetting);
				}
				this.combatSpeeds.Clear();
				for (int k = 0; k < combatLayerSpeedSettings.Count; k++)
				{
					SpeedSetting speedSetting2 = default(SpeedSetting);
					speedSetting2.multiplier = (float)combatLayerSpeedSettings[k];
					speedSetting2.description = Loc.T("UI.SpaceCombat.CombatSpeed" + k.ToString());
					this.combatSpeeds.Add(speedSetting2);
				}
				this.UpdateCurrentSpeedState(SpeedSettingState.Strategy);
			}, true, true);
		}

		// Token: 0x06005D52 RID: 23890 RVA: 0x002C7C5F File Offset: 0x002C5E5F
		public void AddTimeEvent(TITimeEvent timeEvent)
		{
			this.timeQueue.AddEvent(timeEvent);
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x002C7C6D File Offset: 0x002C5E6D
		public TIDateTime GetTimeForPendingEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate)
		{
			TITimeEvent titimeEvent = this.timeQueue.FindEvent(eventName, eventObject, eventObject2, eventTemplate);
			return ((titimeEvent != null) ? titimeEvent.time : null) ?? null;
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x002C7C90 File Offset: 0x002C5E90
		public TIDateTime ExtendTimeEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate, int value, TITimeQueueRepeatType unit)
		{
			return this.timeQueue.ExtendEvent(eventName, eventObject, eventObject2, eventTemplate, value, unit);
		}

		// Token: 0x06005D55 RID: 23893 RVA: 0x002C7CA6 File Offset: 0x002C5EA6
		public void CancelTimeEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate, TIDateTime eventDateTime)
		{
			this.timeQueue.CancelEvent(eventName, eventObject, eventObject2, eventTemplate, eventDateTime);
		}

		// Token: 0x06005D56 RID: 23894 RVA: 0x002C7CBA File Offset: 0x002C5EBA
		public void CancelTimeEvents(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate)
		{
			this.timeQueue.CancelEvents(eventName, eventObject, eventObject2, eventTemplate);
		}

		// Token: 0x06005D57 RID: 23895 RVA: 0x002C7CCC File Offset: 0x002C5ECC
		public void AddCombatTimeEvent(TITimeEvent timeEvent)
		{
			this.combatTimeQueue.AddEvent(timeEvent);
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x002C7CDA File Offset: 0x002C5EDA
		public TIDateTime ExtendCombatTimeEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate, int value, TITimeQueueRepeatType unit)
		{
			return this.combatTimeQueue.ExtendEvent(eventName, eventObject, eventObject2, eventTemplate, value, unit);
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x002C7CF0 File Offset: 0x002C5EF0
		public void CancelCombatTimeEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate, TIDateTime eventDateTime)
		{
			this.combatTimeQueue.CancelEvent(eventName, eventObject, eventObject2, eventTemplate, eventDateTime);
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x002C7D04 File Offset: 0x002C5F04
		public void ClearCombatTimeEvents()
		{
			this.combatTimeQueue.ClearQueue();
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x002C7D11 File Offset: 0x002C5F11
		public void SubstituteStatesInTimeQueue(TIGameState oldState, TIGameState newState)
		{
			this.timeQueue.SubstituteStateInEvents(oldState, newState);
		}

		// Token: 0x06005D5C RID: 23900 RVA: 0x002C7D20 File Offset: 0x002C5F20
		public void CancelAllTimeEventsForObject(TIGameState eventObject)
		{
			this.timeQueue.CancelAllTimeEventsForObject(eventObject);
		}

		// Token: 0x06005D5D RID: 23901 RVA: 0x002C7D2E File Offset: 0x002C5F2E
		public void CancelAllTimeEventsByName(string eventName)
		{
			this.timeQueue.CancelAllTimeEventsByName(eventName);
		}

		// Token: 0x06005D5E RID: 23902 RVA: 0x002C7D3C File Offset: 0x002C5F3C
		public void UpdateCurrentSpeedState(SpeedSettingState state)
		{
			if (state != SpeedSettingState.Strategy)
			{
				if (state != SpeedSettingState.SpaceCombat)
				{
					Error.Log(string.Format("Unknown speed setting: {0}", state), Array.Empty<object>());
					return;
				}
				this.currentSpeeds = this.combatSpeeds;
				this.lastSpeedIndex = this.combatStartingSpeedIndex;
			}
			else
			{
				this.currentSpeeds = this.strategySpeeds;
				this.lastSpeedIndex = this.strategyStartingSpeedIndex;
			}
			this.currentSpeedIndex = 0;
			this.currentSpeed = 0f;
			this.OnSpeedChanged(this.CurrentSpeedSetting);
		}

		// Token: 0x06005D5F RID: 23903 RVA: 0x002C7DC0 File Offset: 0x002C5FC0
		public void Play()
		{
			if (this.currentSpeeds == null)
			{
				return;
			}
			if (this.lastSpeedIndex == 0 && this.currentSpeedIndex == 0)
			{
				this.currentSpeedIndex = (this.lastSpeedIndex = 1);
				this.currentSpeed = this.currentSpeeds[this.currentSpeedIndex].multiplier;
				this.OnSpeedChanged(this.CurrentSpeedSetting);
				return;
			}
			if (this.lastSpeedIndex != this.currentSpeedIndex)
			{
				if (this.lastSpeedIndex > this.MaxSpeedIdx())
				{
					this.SetSpeed(this.lastSpeedIndex, true);
					return;
				}
				this.currentSpeedIndex = this.lastSpeedIndex;
				this.currentSpeed = this.currentSpeeds[this.currentSpeedIndex].multiplier;
				this.OnSpeedChanged(this.CurrentSpeedSetting);
			}
		}

		// Token: 0x17001000 RID: 4096
		// (get) Token: 0x06005D60 RID: 23904 RVA: 0x002C7E7D File Offset: 0x002C607D
		public bool Paused
		{
			get
			{
				return this.currentSpeedIndex == 0;
			}
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x002C7E88 File Offset: 0x002C6088
		public void Pause()
		{
			if (this.currentSpeeds == null)
			{
				return;
			}
			if (this.currentSpeeds.Count > 0 && this.currentSpeedIndex != 0)
			{
				this.lastSpeedIndex = this.currentSpeedIndex;
				this.currentSpeedIndex = 0;
				this.currentSpeed = this.currentSpeeds[this.currentSpeedIndex].multiplier;
				this.OnSpeedChanged(this.CurrentSpeedSetting);
			}
			this.pausedFrame = TIFrameCounter.FrameCount;
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x002C7EFA File Offset: 0x002C60FA
		public void PauseAndBlock()
		{
			this.Pause();
			this.blocked = true;
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x002C7F09 File Offset: 0x002C6109
		public void UnPauseAndUnBlock()
		{
			this.blocked = false;
			this.Play();
		}

		// Token: 0x06005D64 RID: 23908 RVA: 0x002C7F18 File Offset: 0x002C6118
		public void UnBlock()
		{
			this.blocked = false;
			GameControl.eventManager.TriggerEvent(new GameTimeSpeedChanged(), null, Array.Empty<object>());
		}

		// Token: 0x06005D65 RID: 23909 RVA: 0x002C7F38 File Offset: 0x002C6138
		public bool TogglePause()
		{
			int currentSpeedIndex = this.currentSpeedIndex;
			if (this.currentSpeedIndex != 0)
			{
				this.Pause();
			}
			else if (!this.IsBlocked)
			{
				this.Play();
			}
			return this.currentSpeedIndex != currentSpeedIndex;
		}

		// Token: 0x06005D66 RID: 23910 RVA: 0x002C7F76 File Offset: 0x002C6176
		public bool PausedThisFrame()
		{
			return this.pausedFrame == TIFrameCounter.FrameCount;
		}

		// Token: 0x17001001 RID: 4097
		// (get) Token: 0x06005D67 RID: 23911 RVA: 0x002C7F88 File Offset: 0x002C6188
		public bool DontCapSpeed
		{
			get
			{
				return TIGlobalValuesState.isSpaceCombatEnabled || !this.spaceObjectSelection.HasSelection || this.spaceObjectSelection.spaceObjectStateSelected == null || this.spaceObjectSelection.spaceObjectStateSelected.semiMajorAxis_m == 0.0 || this.spaceObjectSelection.spaceObjectStateSelected.barycenter.isSun || (this.spaceObjectSelection.spaceObjectStateSelected.isSpaceFleetState && this.spaceObjectSelection.spaceObjectStateSelected.ref_fleet.landed);
			}
		}

		// Token: 0x06005D68 RID: 23912 RVA: 0x002C801C File Offset: 0x002C621C
		public int MaxSpeedIdx()
		{
			if (this.DontCapSpeed)
			{
				return this.currentSpeeds.Count;
			}
			TISpaceObjectState spaceObjectStateSelected = this.spaceObjectSelection.spaceObjectStateSelected;
			if (spaceObjectStateSelected != null)
			{
				double num;
				if (spaceObjectStateSelected.isSpaceFleetState && spaceObjectStateSelected.ref_fleet.inTransfer && spaceObjectStateSelected.ref_fleet.trajectory.launched)
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState;
					num = spaceObjectStateSelected.ref_fleet.trajectory.getDistFromBarycenterAtTime_m(TITimeState.Now(), out tinaturalSpaceObjectState) / 1000.0;
				}
				else
				{
					num = spaceObjectStateSelected.semiMajorAxis_km;
				}
				return Mathf.Clamp((int)(1L + (long)(num / 11500.0)), 2, this.currentSpeeds.Count);
			}
			return this.currentSpeeds.Count;
		}

		// Token: 0x06005D69 RID: 23913 RVA: 0x002C80D4 File Offset: 0x002C62D4
		public bool ResetSpeed(bool barycenterFallback = false)
		{
			if (this.DontCapSpeed)
			{
				return false;
			}
			TISpaceObjectState spaceObjectStateSelected = this.spaceObjectSelection.spaceObjectStateSelected;
			if (((spaceObjectStateSelected != null) ? spaceObjectStateSelected.barycenter : null) != null && barycenterFallback)
			{
				this.spaceObjectSelection.SelectObject(this.spaceObjectSelection.spaceObjectStateSelected.barycenter.gameObjectLink, false, false);
				return false;
			}
			int currentSpeedIndex = this.currentSpeedIndex;
			this.SetSpeed(Math.Min(this.currentSpeedIndex, this.MaxSpeedIdx()), false);
			return this.currentSpeedIndex != currentSpeedIndex;
		}

		// Token: 0x06005D6A RID: 23914 RVA: 0x002C815C File Offset: 0x002C635C
		public bool IncreaseSpeed()
		{
			int currentSpeedIndex = this.currentSpeedIndex;
			if (this.currentSpeedIndex == 0)
			{
				int num = this.lastSpeedIndex;
				this.lastSpeedIndex = Math.Min(this.lastSpeedIndex + 1, this.currentSpeeds.Count - 1);
				if (num != this.lastSpeedIndex)
				{
					return true;
				}
			}
			else
			{
				this.SetSpeed(this.currentSpeedIndex + 1, true);
			}
			return this.currentSpeedIndex != currentSpeedIndex;
		}

		// Token: 0x06005D6B RID: 23915 RVA: 0x002C81C4 File Offset: 0x002C63C4
		public bool DecreaseSpeed()
		{
			int currentSpeedIndex = this.currentSpeedIndex;
			if (this.currentSpeedIndex >= 2)
			{
				this.SetSpeed(this.currentSpeedIndex - 1, false);
			}
			if (this.currentSpeedIndex == 0)
			{
				int num = this.lastSpeedIndex;
				this.lastSpeedIndex = Math.Max(this.lastSpeedIndex - 1, 1);
				if (num != this.lastSpeedIndex)
				{
					return true;
				}
			}
			return this.currentSpeedIndex != currentSpeedIndex;
		}

		// Token: 0x17001002 RID: 4098
		// (get) Token: 0x06005D6C RID: 23916 RVA: 0x002C8228 File Offset: 0x002C6428
		public SpeedSetting CurrentSpeedSetting
		{
			get
			{
				return this.currentSpeeds[this.currentSpeedIndex];
			}
		}

		// Token: 0x06005D6D RID: 23917 RVA: 0x002C823C File Offset: 0x002C643C
		public void SetSpeed(int idx, bool pushBeyondCap)
		{
			int num = this.MaxSpeedIdx();
			if (pushBeyondCap && idx > num && num < this.currentSpeeds.Count)
			{
				TISpaceObjectState spaceObjectStateSelected = this.spaceObjectSelection.spaceObjectStateSelected;
				if (((spaceObjectStateSelected != null) ? spaceObjectStateSelected.barycenter : null) != null)
				{
					this.spaceObjectSelection.SelectObject(this.spaceObjectSelection.spaceObjectStateSelected.barycenter.gameObjectLink, false, false);
					int num2 = num;
					num = this.MaxSpeedIdx();
					if (num > num2)
					{
						idx = Math.Min(idx, num);
					}
					else
					{
						TISpaceObjectState spaceObjectStateSelected2 = this.spaceObjectSelection.spaceObjectStateSelected;
						if (((spaceObjectStateSelected2 != null) ? spaceObjectStateSelected2.barycenter : null) != null)
						{
							this.spaceObjectSelection.SelectObject(this.spaceObjectSelection.spaceObjectStateSelected.barycenter.gameObjectLink, false, false);
							num = this.MaxSpeedIdx();
							idx = Math.Min(idx, num);
						}
					}
				}
			}
			else
			{
				idx = Math.Min(idx, num);
			}
			if (idx > 0 && idx < this.currentSpeeds.Count)
			{
				this.lastSpeedIndex = this.currentSpeedIndex;
				this.currentSpeedIndex = idx;
				this.currentSpeed = this.currentSpeeds[this.currentSpeedIndex].multiplier;
				this.OnSpeedChanged(this.CurrentSpeedSetting);
			}
		}

		// Token: 0x06005D6E RID: 23918 RVA: 0x002C8373 File Offset: 0x002C6573
		public void PreserveStrategySpeed()
		{
			this.strategyStartingSpeedIndex = this.currentSpeedIndex;
		}

		// Token: 0x06005D6F RID: 23919 RVA: 0x002C8381 File Offset: 0x002C6581
		public void Reset()
		{
			this.currentSpeeds = null;
			this.currentSpeedIndex = 0;
			this.currentSpeed = 0f;
		}

		// Token: 0x040042C4 RID: 17092
		private TIPromptQueueState promptQueue;

		// Token: 0x040042C6 RID: 17094
		private TITimeQueue combatTimeQueue;

		// Token: 0x040042C7 RID: 17095
		private TITimeState timeState;

		// Token: 0x040042CB RID: 17099
		public int lastSpeedIndex;

		// Token: 0x040042CC RID: 17100
		public bool blocked;

		// Token: 0x040042CD RID: 17101
		private SpaceObjectSelection spaceObjectSelection;

		// Token: 0x040042CE RID: 17102
		private List<SpeedSetting> strategySpeeds = new List<SpeedSetting>();

		// Token: 0x040042CF RID: 17103
		private List<SpeedSetting> combatSpeeds = new List<SpeedSetting>();

		// Token: 0x040042D0 RID: 17104
		private int strategyStartingSpeedIndex = 1;

		// Token: 0x040042D1 RID: 17105
		private int combatStartingSpeedIndex = 3;

		// Token: 0x040042D2 RID: 17106
		private int pausedFrame;

		// Token: 0x040042D3 RID: 17107
		private const float antiSeizureControl_km = 11500f;

		// Token: 0x02001356 RID: 4950
		// (Invoke) Token: 0x060090BA RID: 37050
		public delegate void SpeedChangeCallback(SpeedSetting speed);
	}
}
