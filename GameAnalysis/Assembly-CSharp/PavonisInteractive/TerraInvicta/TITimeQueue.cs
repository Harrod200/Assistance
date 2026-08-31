using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007EE RID: 2030
	public class TITimeQueue
	{
		// Token: 0x17000E7F RID: 3711
		// (get) Token: 0x060048DC RID: 18652 RVA: 0x001DF22A File Offset: 0x001DD42A
		// (set) Token: 0x060048DD RID: 18653 RVA: 0x001DF232 File Offset: 0x001DD432
		public List<TITimeEvent> events { get; private set; }

		// Token: 0x060048DE RID: 18654 RVA: 0x001DF23B File Offset: 0x001DD43B
		public TITimeQueue()
		{
			this.events = new List<TITimeEvent>();
			this.stopClockEvents = new List<TITimeEvent>();
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}

		// Token: 0x060048DF RID: 18655 RVA: 0x001DF26C File Offset: 0x001DD46C
		public void Initialize()
		{
			if (this.events == null)
			{
				this.events = new List<TITimeEvent>();
			}
			if (this.stopClockEvents == null)
			{
				this.stopClockEvents = new List<TITimeEvent>();
			}
		}

		// Token: 0x060048E0 RID: 18656 RVA: 0x001DF2A4 File Offset: 0x001DD4A4
		public float GetDeltaTime(float deltaTime, DateTime now)
		{
			DateTime dateTime = now.AddSeconds((double)deltaTime);
			TITimeEvent nextStopClockEvent = this.GetNextStopClockEvent();
			if (nextStopClockEvent != null && nextStopClockEvent.time.ExportTime() <= dateTime && nextStopClockEvent.stopClock)
			{
				TimeSpan timeSpan = nextStopClockEvent.time.ExportTime() - now;
				if (timeSpan < TimeSpan.Zero)
				{
					timeSpan = TimeSpan.Zero;
					Log.Error("Unprocessed event had trigger time before earlier time: " + nextStopClockEvent.eventName, Array.Empty<object>());
				}
				return (float)timeSpan.TotalSeconds;
			}
			return deltaTime;
		}

		// Token: 0x060048E1 RID: 18657 RVA: 0x001DF330 File Offset: 0x001DD530
		public void UpdateToTime(DateTime dateTime)
		{
			int num = 0;
			TITimeEvent titimeEvent = this.GetNextEvent();
			while (titimeEvent != null && titimeEvent.time.ExportTime() <= dateTime)
			{
				titimeEvent.StartEvent();
				if (!titimeEvent.isComplete && titimeEvent.pauseTime)
				{
					this.gameTime.Pause();
				}
				this.RemoveEvent(titimeEvent);
				titimeEvent.EndEvent();
				titimeEvent = this.GetNextEvent();
				if (++num > 10000)
				{
					if (!this.gameTime.Paused)
					{
						Log.Warn("Event counter overload, pausing to catch up.", Array.Empty<object>());
					}
					this.gameTime.Pause();
					return;
				}
			}
		}

		// Token: 0x060048E2 RID: 18658 RVA: 0x001DF3D0 File Offset: 0x001DD5D0
		public void AddEvent(TITimeEvent newEvent)
		{
			this.AddSorted(newEvent);
		}

		// Token: 0x060048E3 RID: 18659 RVA: 0x001DF3D9 File Offset: 0x001DD5D9
		public void RemoveEvent(TITimeEvent timeEvent)
		{
			this.events.Remove(timeEvent);
			if (timeEvent.stopClock)
			{
				this.stopClockEvents.Remove(timeEvent);
			}
		}

		// Token: 0x060048E4 RID: 18660 RVA: 0x001DF400 File Offset: 0x001DD600
		public TITimeEvent FindEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventDataTemplate)
		{
			foreach (TITimeEvent titimeEvent in this.events)
			{
				if (eventDataTemplate == null)
				{
					if (titimeEvent.eventDataTemplate != null)
					{
						continue;
					}
				}
				else if (titimeEvent.eventDataTemplateName != eventDataTemplate.dataName)
				{
					continue;
				}
				if (titimeEvent.eventName == eventName && titimeEvent.eventObject == eventObject && titimeEvent.eventObject2 == eventObject2)
				{
					return titimeEvent;
				}
			}
			return null;
		}

		// Token: 0x060048E5 RID: 18661 RVA: 0x001DF4A0 File Offset: 0x001DD6A0
		public TIDateTime ExtendEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate, int value, TITimeQueueRepeatType unit)
		{
			TITimeEvent titimeEvent = this.FindEvent(eventName, eventObject, eventObject2, eventTemplate);
			if (titimeEvent != null)
			{
				TIDateTime tidateTime = new TIDateTime(titimeEvent.time);
				switch (unit)
				{
				case TITimeQueueRepeatType.Second:
					tidateTime.AddSeconds((double)value);
					goto IL_00A5;
				case TITimeQueueRepeatType.Minute:
					tidateTime.AddSeconds((double)(value * 60));
					goto IL_00A5;
				case TITimeQueueRepeatType.Hour:
					tidateTime.AddHours((double)value);
					goto IL_00A5;
				case TITimeQueueRepeatType.Day:
					tidateTime.AddDays((float)value);
					goto IL_00A5;
				case TITimeQueueRepeatType.Month:
					tidateTime.AddMonths(value);
					goto IL_00A5;
				case TITimeQueueRepeatType.Year:
					tidateTime.AddYears(value);
					goto IL_00A5;
				}
				Error.Log("Bad unit passed to TITimeQueue.ExtendEvent", Array.Empty<object>());
				IL_00A5:
				this.RemoveEvent(titimeEvent);
				TITimeEvent.CreateNewTimeEvent(tidateTime, eventObject, eventObject2, titimeEvent.eventDataTemplate, eventName, titimeEvent.stopClock, titimeEvent.pauseTime, titimeEvent.repeatType, titimeEvent.timeStep, true, titimeEvent.combatEvent);
				GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent.ID, false);
				return tidateTime;
			}
			return null;
		}

		// Token: 0x060048E6 RID: 18662 RVA: 0x001DF598 File Offset: 0x001DD798
		public void SubstituteStateInEvents(TIGameState oldState, TIGameState newState)
		{
			foreach (TITimeEvent titimeEvent in this.events)
			{
				if (titimeEvent.eventObject == oldState)
				{
					titimeEvent.eventObjectID = newState.ID;
				}
				if (titimeEvent.eventObject2 == oldState)
				{
					titimeEvent.eventObject2ID = newState.ID;
				}
			}
		}

		// Token: 0x060048E7 RID: 18663 RVA: 0x001DF618 File Offset: 0x001DD818
		public void CancelEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate, TIDateTime eventTime)
		{
			List<TITimeEvent> list = new List<TITimeEvent>();
			foreach (TITimeEvent titimeEvent in this.events)
			{
				if (titimeEvent.eventName == eventName && titimeEvent.eventObject == eventObject && titimeEvent.eventObject2 == eventObject2 && titimeEvent.eventDataTemplate == eventTemplate && titimeEvent.time == eventTime)
				{
					list.Add(titimeEvent);
				}
			}
			foreach (TITimeEvent titimeEvent2 in list)
			{
				this.RemoveEvent(titimeEvent2);
				GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent2.ID, false);
			}
		}

		// Token: 0x060048E8 RID: 18664 RVA: 0x001DF700 File Offset: 0x001DD900
		public void CancelEvent(string eventName, TIGameState eventObject, TIGameState eventObject2, string eventTemplateName, TIDateTime eventTime)
		{
			List<TITimeEvent> list = new List<TITimeEvent>();
			foreach (TITimeEvent titimeEvent in this.events)
			{
				if (titimeEvent.eventName == eventName && titimeEvent.eventObject == eventObject && titimeEvent.eventObject2 == eventObject2 && titimeEvent.eventDataTemplateName == eventTemplateName && titimeEvent.time == eventTime)
				{
					list.Add(titimeEvent);
				}
			}
			foreach (TITimeEvent titimeEvent2 in list)
			{
				this.RemoveEvent(titimeEvent2);
				GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent2.ID, false);
			}
		}

		// Token: 0x060048E9 RID: 18665 RVA: 0x001DF7EC File Offset: 0x001DD9EC
		public void CancelEvents(string eventName, TIGameState eventObject, TIGameState eventObject2, TIDataTemplate eventTemplate)
		{
			List<TITimeEvent> list = new List<TITimeEvent>();
			foreach (TITimeEvent titimeEvent in this.events)
			{
				if (titimeEvent.eventName == eventName && titimeEvent.eventObject == eventObject && titimeEvent.eventObject2 == eventObject2 && titimeEvent.eventDataTemplate == eventTemplate)
				{
					list.Add(titimeEvent);
				}
			}
			foreach (TITimeEvent titimeEvent2 in list)
			{
				this.RemoveEvent(titimeEvent2);
				GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent2.ID, false);
			}
		}

		// Token: 0x060048EA RID: 18666 RVA: 0x001DF8C4 File Offset: 0x001DDAC4
		public void CancelAllTimeEventsForObject(TIGameState eventObject)
		{
			if (eventObject != null)
			{
				List<TITimeEvent> list = new List<TITimeEvent>();
				foreach (TITimeEvent titimeEvent in this.events)
				{
					if (titimeEvent.eventObjectID == eventObject.ID || titimeEvent.eventObject2ID == eventObject.ID)
					{
						list.Add(titimeEvent);
					}
				}
				foreach (TITimeEvent titimeEvent2 in list)
				{
					this.RemoveEvent(titimeEvent2);
					GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent2.ID, false);
				}
			}
		}

		// Token: 0x060048EB RID: 18667 RVA: 0x001DF99C File Offset: 0x001DDB9C
		public void CancelAllTimeEventsByName(string eventName)
		{
			List<TITimeEvent> list = new List<TITimeEvent>();
			foreach (TITimeEvent titimeEvent in this.events)
			{
				if (titimeEvent.eventName == eventName)
				{
					list.Add(titimeEvent);
				}
			}
			foreach (TITimeEvent titimeEvent2 in list)
			{
				this.RemoveEvent(titimeEvent2);
				GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent2.ID, false);
			}
		}

		// Token: 0x060048EC RID: 18668 RVA: 0x001DFA50 File Offset: 0x001DDC50
		public void ClearQueue()
		{
			foreach (TITimeEvent titimeEvent in this.events)
			{
				GameStateManager.RemoveGameState<TITimeEvent>(titimeEvent.ID, false);
			}
			this.events.Clear();
			this.stopClockEvents.Clear();
		}

		// Token: 0x060048ED RID: 18669 RVA: 0x001DFAC0 File Offset: 0x001DDCC0
		private void AddSorted(TITimeEvent evt)
		{
			if (this.events.Count == 0)
			{
				this.events.Add(evt);
				if (evt.stopClock)
				{
					this.stopClockEvents.Add(evt);
					return;
				}
			}
			else
			{
				TITimeQueue.TITimeEventComparer titimeEventComparer = new TITimeQueue.TITimeEventComparer();
				int num = this.events.BinarySearch(evt, titimeEventComparer);
				if (num < 0)
				{
					this.events.Insert(-num - 1, evt);
				}
				else
				{
					this.events.Insert(num, evt);
				}
				if (evt.stopClock)
				{
					int num2 = this.stopClockEvents.BinarySearch(evt, titimeEventComparer);
					if (num2 < 0)
					{
						this.stopClockEvents.Insert(-num2 - 1, evt);
						return;
					}
					this.stopClockEvents.Insert(num2, evt);
				}
			}
		}

		// Token: 0x060048EE RID: 18670 RVA: 0x001DFB6A File Offset: 0x001DDD6A
		private TITimeEvent GetNextEvent()
		{
			if (this.events.Count <= 0)
			{
				return null;
			}
			return this.events[0];
		}

		// Token: 0x060048EF RID: 18671 RVA: 0x001DFB88 File Offset: 0x001DDD88
		private TITimeEvent GetNextStopClockEvent()
		{
			if (this.stopClockEvents.Count <= 0)
			{
				return null;
			}
			return this.stopClockEvents[0];
		}

		// Token: 0x04002ACD RID: 10957
		private GameTimeManager gameTime;

		// Token: 0x04002ACE RID: 10958
		private List<TITimeEvent> stopClockEvents;

		// Token: 0x02000F9B RID: 3995
		private class TITimeEventComparer : IComparer<TITimeEvent>
		{
			// Token: 0x06007F57 RID: 32599 RVA: 0x00327BA8 File Offset: 0x00325DA8
			public int Compare(TITimeEvent evt1, TITimeEvent evt2)
			{
				int num = evt1.time.ExportTime().CompareTo(evt2.time.ExportTime());
				if (num == 0)
				{
					return evt1.eventName.CompareTo(evt2.eventName);
				}
				return num;
			}
		}
	}
}
