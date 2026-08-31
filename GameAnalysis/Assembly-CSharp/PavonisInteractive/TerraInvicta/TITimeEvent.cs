using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D9 RID: 2009
	public class TITimeEvent : TIGameState
	{
		// Token: 0x17000E73 RID: 3699
		// (get) Token: 0x0600488F RID: 18575 RVA: 0x001DD537 File Offset: 0x001DB737
		public TITimeEventTemplate template
		{
			get
			{
				return this.GetMyTemplate<TITimeEventTemplate>();
			}
		}

		// Token: 0x17000E74 RID: 3700
		// (get) Token: 0x06004890 RID: 18576 RVA: 0x001DD53F File Offset: 0x001DB73F
		public TIDateTime time
		{
			get
			{
				return this.triggerTime;
			}
		}

		// Token: 0x17000E75 RID: 3701
		// (get) Token: 0x06004891 RID: 18577 RVA: 0x001DD547 File Offset: 0x001DB747
		public TIGameState eventObject
		{
			get
			{
				return this.eventObjectID.GetState();
			}
		}

		// Token: 0x17000E76 RID: 3702
		// (get) Token: 0x06004892 RID: 18578 RVA: 0x001DD554 File Offset: 0x001DB754
		public TIGameState eventObject2
		{
			get
			{
				return this.eventObject2ID.GetState();
			}
		}

		// Token: 0x06004893 RID: 18579 RVA: 0x001DD564 File Offset: 0x001DB764
		public static TITimeEvent CreateNewTimeEvent(TIDateTime triggerTime, TIGameState eventObject = null, TIGameState eventObject2 = null, TIDataTemplate eventDataTemplate = null, string eventName = "", bool stopClock = true, bool pauseTime = false, TITimeQueueRepeatType repeatType = TITimeQueueRepeatType.None, int timeStep = 1, bool addToQueue = true, bool combat = false)
		{
			TITimeEventTemplate titimeEventTemplate = TemplateManager.Find<TITimeEventTemplate>("GenericTimeEvent", false);
			TITimeEvent titimeEvent = titimeEventTemplate.CreateGameState() as TITimeEvent;
			if (string.IsNullOrEmpty(eventName))
			{
				Log.Error("Time event created with no eventName. It will never trigger.", Array.Empty<object>());
			}
			titimeEvent.InitWithTemplate(titimeEventTemplate);
			titimeEvent.triggerTime = triggerTime;
			titimeEvent.eventObjectID = ((eventObject != null) ? eventObject.ID : default(GameStateID));
			titimeEvent.eventObject2ID = ((eventObject2 != null) ? eventObject2.ID : default(GameStateID));
			titimeEvent._eventDataTemplate = eventDataTemplate;
			titimeEvent.eventDataTemplateName = ((eventDataTemplate != null) ? eventDataTemplate.dataName : null) ?? string.Empty;
			titimeEvent.eventName = eventName;
			titimeEvent.stopClock = stopClock || pauseTime;
			titimeEvent.pauseTime = pauseTime;
			titimeEvent.repeatType = repeatType;
			titimeEvent.timeStep = timeStep;
			titimeEvent.combatEvent = combat;
			if (addToQueue)
			{
				if (!combat)
				{
					titimeEvent.gameTime.AddTimeEvent(titimeEvent);
				}
				else
				{
					titimeEvent.gameTime.AddCombatTimeEvent(titimeEvent);
				}
			}
			return titimeEvent;
		}

		// Token: 0x06004894 RID: 18580 RVA: 0x001DD65B File Offset: 0x001DB85B
		public override bool Initialize()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			return true;
		}

		// Token: 0x17000E77 RID: 3703
		// (get) Token: 0x06004895 RID: 18581 RVA: 0x001DD66E File Offset: 0x001DB86E
		public TIDataTemplate eventDataTemplate
		{
			get
			{
				if (this._eventDataTemplate == null && !string.IsNullOrEmpty(this.eventDataTemplateName))
				{
					this._eventDataTemplate = TemplateManager.Find<TIDataTemplate>(this.eventDataTemplateName, true);
				}
				return this._eventDataTemplate;
			}
		}

		// Token: 0x06004896 RID: 18582 RVA: 0x001DD6A0 File Offset: 0x001DB8A0
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TITimeEventTemplate titimeEventTemplate = template as TITimeEventTemplate;
			if (titimeEventTemplate == null)
			{
				return;
			}
			this.templateName = titimeEventTemplate.dataName;
			this.eventName = titimeEventTemplate.eventName;
			this.repeatType = titimeEventTemplate.eventType;
			this.timeStep = titimeEventTemplate.timeStep ?? 1;
			this.pauseTime = titimeEventTemplate.pauseTime;
			this.stopClock = titimeEventTemplate.stopClock || titimeEventTemplate.pauseTime;
			this.isComplete = false;
		}

		// Token: 0x06004897 RID: 18583 RVA: 0x001DD730 File Offset: 0x001DB930
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (this.repeatType != TITimeQueueRepeatType.None)
			{
				this.triggerTime = this.GetNextEventTime(TITimeState.Now());
			}
			TITimeEventTemplate template = this.template;
			bool flag;
			if (template == null)
			{
				flag = false;
			}
			else
			{
				List<TITimeEventTemplate.RepeatChange> repeatChanges = template.repeatChanges;
				int? num = ((repeatChanges != null) ? new int?(repeatChanges.Count) : null);
				int num2 = 0;
				flag = (num.GetValueOrDefault() > num2) & (num != null);
			}
			if (flag)
			{
				this.repeatChangeTriggered = new List<bool>();
				for (int i = 0; i < this.template.repeatChanges.Count; i++)
				{
					this.repeatChangeTriggered.Add(false);
				}
			}
		}

		// Token: 0x06004898 RID: 18584 RVA: 0x001DD7CC File Offset: 0x001DB9CC
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (this.template.dataName == "CouncilorMissionUpdate" && TemplateManager.global.dontStopBimonthlyMissions)
			{
				for (int i = 0; i < this.template.repeatChanges.Count; i++)
				{
					if (this.template.repeatChanges[i].updateEventType == TITimeQueueRepeatType.EveryThreeWeeksToMonth || this.template.repeatChanges[i].updateEventType == TITimeQueueRepeatType.Month)
					{
						this.repeatChangeTriggered[i] = true;
					}
				}
			}
		}

		// Token: 0x06004899 RID: 18585 RVA: 0x001DD867 File Offset: 0x001DBA67
		public void StartEvent()
		{
			if (this.eventName != string.Empty && TIGlobalValuesState.isSpaceCombatEnabled == this.combatEvent)
			{
				GameControl.eventManager.TriggerEvent(new TimeEventStart(this), this.eventName, Array.Empty<object>());
			}
		}

		// Token: 0x0600489A RID: 18586 RVA: 0x001DD8A4 File Offset: 0x001DBAA4
		public void EndEvent()
		{
			if (this.repeatType == TITimeQueueRepeatType.None)
			{
				this.isComplete = true;
				base.ArchiveState(true);
				GameStateManager.RemoveGameState<TITimeEvent>(base.ID, false);
				return;
			}
			this.CheckChangeRepeatTime();
			TIDateTime nextEventTime = this.GetNextEventTime(this.gameTime.currentTime);
			if (nextEventTime <= this.gameTime.currentTime)
			{
				Debug.LogWarning("Attempting to add event " + this.eventName + " before current time. Danger of unbounded loop. Did not add event.");
				return;
			}
			this.triggerTime = nextEventTime;
			this.isComplete = false;
			if (this.combatEvent)
			{
				this.gameTime.AddCombatTimeEvent(this);
				return;
			}
			this.gameTime.AddTimeEvent(this);
		}

		// Token: 0x0600489B RID: 18587 RVA: 0x001DD94C File Offset: 0x001DBB4C
		public void CheckChangeRepeatTime()
		{
			if (this.template != null && this.template.repeatChanges != null)
			{
				for (int i = 0; i < this.template.repeatChanges.Count; i++)
				{
					if (!this.repeatChangeTriggered[i])
					{
						TITimeEventTemplate.RepeatChange repeatChange = this.template.repeatChanges[i];
						if (repeatChange.triggerCondition != null && repeatChange.ConditionMet)
						{
							this.repeatType = repeatChange.updateEventType;
							this.repeatChangeTriggered[i] = true;
							if (this.template.dataName == "CouncilorMissionUpdate")
							{
								this.startMonth = TITimeState.Now().month;
								TINotificationQueueState.LogTimeChangeUpdate(this.repeatType);
								TIMissionPhaseState.UpdatePerMonthTurnFrequency();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600489C RID: 18588 RVA: 0x001DDA18 File Offset: 0x001DBC18
		public TIDateTime GetNextEventTime(TIDateTime dt)
		{
			if (this.repeatType == TITimeQueueRepeatType.None)
			{
				return null;
			}
			TITimeQueueRepeatType updateEventType = this.repeatType;
			if (this.template != null && this.template.repeatChanges != null)
			{
				for (int i = 0; i < this.template.repeatChanges.Count; i++)
				{
					TITimeEventTemplate.RepeatChange repeatChange = this.template.repeatChanges[i];
					TIGlobalCondition_fCampaignDuration_years tiglobalCondition_fCampaignDuration_years = repeatChange.triggerCondition as TIGlobalCondition_fCampaignDuration_years;
					if (tiglobalCondition_fCampaignDuration_years != null && (dt.DifferenceInDays(TITimeState.Now()) + (double)TITimeState.CampaignDuration_days()) / 365.2421875 >= (double)TIUtilities.GetFloatValue(tiglobalCondition_fCampaignDuration_years.strValue))
					{
						updateEventType = repeatChange.updateEventType;
					}
				}
			}
			TIDateTime tidateTime = new TIDateTime();
			tidateTime.CopyDateTime(dt);
			switch (updateEventType)
			{
			case TITimeQueueRepeatType.HalfSecond:
				if (dt.millisecond < 500)
				{
					tidateTime.millisecond = 500;
					goto IL_0245;
				}
				tidateTime.millisecond = 0;
				goto IL_0245;
			case TITimeQueueRepeatType.Second:
				goto IL_021B;
			case TITimeQueueRepeatType.Minute:
				goto IL_0214;
			case TITimeQueueRepeatType.Hour:
				goto IL_020D;
			case TITimeQueueRepeatType.Day:
				goto IL_0206;
			case TITimeQueueRepeatType.WeekToMonth:
				if (dt.day <= 7)
				{
					tidateTime.day = 8;
					tidateTime.hour = 6;
					goto IL_0206;
				}
				if (dt.day <= 14)
				{
					tidateTime.day = 15;
					tidateTime.hour = 12;
					goto IL_0206;
				}
				if (dt.day <= 21)
				{
					tidateTime.day = 22;
					tidateTime.hour = 18;
					goto IL_0206;
				}
				tidateTime.day = 1;
				tidateTime.hour = 0;
				goto IL_0206;
			case TITimeQueueRepeatType.Semimonthly:
				if (dt.day >= 1 && dt.day <= 15)
				{
					tidateTime.day = 16;
					tidateTime.hour = 12;
					goto IL_0206;
				}
				tidateTime.day = 1;
				tidateTime.hour = 0;
				goto IL_0206;
			case TITimeQueueRepeatType.EveryThreeWeeksToMonth:
				switch (IntExtensions.EuclidianModulo(tidateTime.month - this.startMonth, 3))
				{
				case 0:
					if (dt.day >= 1 && dt.day <= 21)
					{
						tidateTime.day = 22;
						goto IL_0206;
					}
					tidateTime.AddMonths(1);
					tidateTime.day = 16;
					goto IL_0206;
				case 1:
					tidateTime.AddMonths(1);
					tidateTime.day = 8;
					goto IL_0206;
				case 2:
					tidateTime.AddMonths(1);
					tidateTime.day = 1;
					goto IL_0206;
				default:
					goto IL_0206;
				}
				break;
			case TITimeQueueRepeatType.Month:
				break;
			case TITimeQueueRepeatType.Year:
				tidateTime.month = 1;
				break;
			default:
				goto IL_0245;
			}
			tidateTime.day = 1;
			IL_0206:
			tidateTime.hour = 0;
			IL_020D:
			tidateTime.minute = 0;
			IL_0214:
			tidateTime.second = 0;
			IL_021B:
			tidateTime.millisecond = 0;
			IL_0245:
			switch (updateEventType)
			{
			case TITimeQueueRepeatType.HalfSecond:
				tidateTime.AddSeconds((double)((float)this.timeStep / 2f));
				break;
			case TITimeQueueRepeatType.Second:
				tidateTime.AddSeconds((double)this.timeStep);
				break;
			case TITimeQueueRepeatType.Minute:
				tidateTime.AddSeconds((double)this.timeStep * 60.0);
				break;
			case TITimeQueueRepeatType.Hour:
				tidateTime.AddSeconds((double)this.timeStep * 3600.0);
				break;
			case TITimeQueueRepeatType.Day:
				tidateTime.AddDays((float)this.timeStep);
				break;
			case TITimeQueueRepeatType.WeekToMonth:
			{
				int day = dt.day;
				if (day >= 22)
				{
					tidateTime.AddMonths(1);
				}
				else if (day >= 15)
				{
					tidateTime.AddHours(18.0);
				}
				else if (day >= 8)
				{
					tidateTime.AddHours(12.0);
				}
				else
				{
					tidateTime.AddHours(6.0);
				}
				break;
			}
			case TITimeQueueRepeatType.Semimonthly:
				if (dt.day == 1)
				{
					tidateTime.AddHours(12.0);
				}
				else
				{
					tidateTime.AddMonths(1);
				}
				break;
			case TITimeQueueRepeatType.Month:
				tidateTime.AddMonths(this.timeStep);
				break;
			case TITimeQueueRepeatType.Year:
				tidateTime.AddYears(this.timeStep);
				break;
			}
			return tidateTime;
		}

		// Token: 0x0600489D RID: 18589 RVA: 0x001DDDAA File Offset: 0x001DBFAA
		public override string ToString()
		{
			return base.ToString() + ":" + this.eventName;
		}

		// Token: 0x040029BB RID: 10683
		[SerializeField]
		private TIDateTime triggerTime;

		// Token: 0x040029BC RID: 10684
		public GameStateID eventObjectID;

		// Token: 0x040029BD RID: 10685
		public GameStateID eventObject2ID;

		// Token: 0x040029BE RID: 10686
		public string eventDataTemplateName;

		// Token: 0x040029BF RID: 10687
		private TIDataTemplate _eventDataTemplate;

		// Token: 0x040029C0 RID: 10688
		public string eventName;

		// Token: 0x040029C1 RID: 10689
		public TITimeQueueRepeatType repeatType;

		// Token: 0x040029C2 RID: 10690
		public int timeStep;

		// Token: 0x040029C3 RID: 10691
		public bool stopClock;

		// Token: 0x040029C4 RID: 10692
		public bool pauseTime;

		// Token: 0x040029C5 RID: 10693
		public bool combatEvent;

		// Token: 0x040029C6 RID: 10694
		public bool isComplete;

		// Token: 0x040029C7 RID: 10695
		public List<bool> repeatChangeTriggered;

		// Token: 0x040029C8 RID: 10696
		public int startMonth;

		// Token: 0x040029C9 RID: 10697
		private GameTimeManager gameTime;
	}
}
