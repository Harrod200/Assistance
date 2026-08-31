using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200082F RID: 2095
	public class CalendarDayGridItemController : MonoBehaviour
	{
		// Token: 0x06004B4A RID: 19274 RVA: 0x001F6584 File Offset: 0x001F4784
		public void ClearGridItem()
		{
			this.dayNumber.SetText(string.Empty);
			this.alarmButton.gameObject.SetActive(false);
			this.tip.enabled = false;
			this.todaysEventList.SetListSize<CalendarItemListItemController>(0, false, false);
		}

		// Token: 0x06004B4B RID: 19275 RVA: 0x001F65C4 File Offset: 0x001F47C4
		private string SetTip(List<CalendarDayGridItemController.CalendarItem> todaysEvents)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.date.ToCustomDateString());
			for (int i = 0; i < todaysEvents.Count; i++)
			{
				stringBuilder.AppendLine(Loc.T("UI.Council.Calendar.FullEvent", new object[]
				{
					todaysEvents[i].dateTime.ToCustomTimeString(),
					todaysEvents[i].description
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004B4C RID: 19276 RVA: 0x001F663C File Offset: 0x001F483C
		public void UpdateGridItem(TIDateTime date, List<CalendarDayGridItemController.CalendarItem> todaysEvents)
		{
			this.dayNumber.SetText(date.day.ToString("N0"));
			this.date = date;
			this.todaysEventList.SetListSize<CalendarItemListItemController>(todaysEvents.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.todaysEventList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CalendarDayGridItemController.<>o__7.<>p__0 == null)
					{
						CalendarDayGridItemController.<>o__7.<>p__0 = CallSite<Func<CallSite, object, CalendarItemListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CalendarItemListItemController), typeof(CalendarDayGridItemController)));
					}
					CalendarDayGridItemController.<>o__7.<>p__0.Target(CalendarDayGridItemController.<>o__7.<>p__0, enumerator.Current).UpdateListItem(this, todaysEvents[num].description, todaysEvents[num].dateTime, todaysEvents[num].alarm, todaysEvents[num].flag1, todaysEvents[num].flag2, todaysEvents[num].icon);
					num++;
				}
			}
			if (todaysEvents.Count > 0)
			{
				this.tip.SetDelegate("BodyText", () => this.SetTip(todaysEvents));
				this.tip.enabled = true;
			}
			else
			{
				this.tip.enabled = false;
			}
			this.alarmButton.gameObject.SetActive(date >= TITimeState.Now());
		}

		// Token: 0x06004B4D RID: 19277 RVA: 0x001F67E8 File Offset: 0x001F49E8
		public void OnAlarmClicked()
		{
			GeneralControlsController.Singleton.OpenAlarmPanel(this.date, "");
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x001F680B File Offset: 0x001F4A0B
		public void OnItemButtonClicked(TIDateTime dateTime, string description)
		{
			GeneralControlsController.Singleton.OpenAlarmPanel(dateTime, description);
		}

		// Token: 0x06004B4F RID: 19279 RVA: 0x001F681C File Offset: 0x001F4A1C
		public static SortedList<int, List<CalendarDayGridItemController.CalendarItem>> GetMonthlyEvents(TIFactionState faction, TIDateTime date)
		{
			int month = date.month;
			int year = date.year;
			SortedList<int, List<CalendarDayGridItemController.CalendarItem>> sortedList = new SortedList<int, List<CalendarDayGridItemController.CalendarItem>>();
			for (int i = 1; i < 32; i++)
			{
				sortedList[i] = new List<CalendarDayGridItemController.CalendarItem>();
			}
			List<CalendarDayGridItemController.CooldownPair> list = new List<CalendarDayGridItemController.CooldownPair>();
			using (IEnumerator<TINationState> enumerator = GameStateManager.AllExtantNations().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TINationState nation = enumerator.Current;
					foreach (TIControlPoint ticontrolPoint in nation.controlPoints)
					{
						if (ticontrolPoint.faction == faction)
						{
							if (ticontrolPoint.benefitsDisabled && ticontrolPoint.crackdownExpiration.year == year && ticontrolPoint.crackdownExpiration.month == month)
							{
								sortedList[ticontrolPoint.crackdownExpiration.day].Add(new CalendarDayGridItemController.CalendarItem(ticontrolPoint.crackdownExpiration, Loc.T("UI.Council.Calendar.CrackdownExpires", new object[] { ticontrolPoint.displayName }), false, "", ticontrolPoint.nation.flagResource, ""));
							}
							else if (ticontrolPoint.defended && ticontrolPoint.defendExpiration.year == year && ticontrolPoint.defendExpiration.month == month)
							{
								sortedList[ticontrolPoint.defendExpiration.day].Add(new CalendarDayGridItemController.CalendarItem(ticontrolPoint.defendExpiration, Loc.T("UI.Council.Calendar.DefendInterestsExpires", new object[] { ticontrolPoint.displayName }), false, "", ticontrolPoint.nation.flagResource, ""));
							}
						}
					}
					if (nation.executiveFaction == faction)
					{
						TIDateTime tidateTime = nation.ExecutivePowerConsolidationDate();
						if (tidateTime != null && tidateTime.year == year && tidateTime.month == month)
						{
							sortedList[tidateTime.day].Add(new CalendarDayGridItemController.CalendarItem(tidateTime, Loc.T("UI.Council.Calendar.ExecutivePowerConsolidated", new object[] { nation.displayName }), false, "", nation.flagResource, ""));
						}
						using (Dictionary<TINationState, TIDateTime>.Enumerator enumerator3 = nation.improveRelationsCooldowns.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<TINationState, TIDateTime> cooldown = enumerator3.Current;
								if (cooldown.Value.year == year && cooldown.Value.month == month && list.None<CalendarDayGridItemController.CooldownPair>((CalendarDayGridItemController.CooldownPair x) => x.nation1 == cooldown.Key && x.nation2 == nation))
								{
									sortedList[cooldown.Value.day].Add(new CalendarDayGridItemController.CalendarItem(cooldown.Value, Loc.T("UI.Council.Calendar.ImproveRelationsCooldownEnds", new object[]
									{
										nation.displayName,
										cooldown.Key.displayName
									}), false, "", nation.flagResource, cooldown.Key.flagResource));
									list.Add(new CalendarDayGridItemController.CooldownPair
									{
										nation1 = nation,
										nation2 = cooldown.Key
									});
								}
							}
						}
					}
				}
			}
			foreach (TIHabState tihabState in faction.habs)
			{
				foreach (TIHabModuleState tihabModuleState in tihabState.AllModules())
				{
					if (((!tihabModuleState.completed && new TIDateTime(tihabModuleState.completionDate) >= TITimeState.Now()) || (tihabModuleState.completed && new TIDateTime(tihabModuleState.completionDate) < TITimeState.Now())) && tihabModuleState.completionDate.Year == year && tihabModuleState.completionDate.Month == month)
					{
						sortedList[tihabModuleState.completionDate.Day].Add(new CalendarDayGridItemController.CalendarItem(new TIDateTime(tihabModuleState.completionDate), Loc.T("UI.Council.Calendar.HabModuleCompleted", new object[] { tihabState.displayName, tihabModuleState.displayName }), false, tihabState.iconResource, "", ""));
					}
					else if (tihabModuleState.decommissioning && tihabModuleState.decommissionDate.Year == year && tihabModuleState.decommissionDate.Month == month)
					{
						sortedList[tihabModuleState.decommissionDate.Day].Add(new CalendarDayGridItemController.CalendarItem(new TIDateTime(tihabModuleState.decommissionDate), Loc.T("UI.Council.Calendar.HabModuleDecommissioned", new object[] { tihabState.displayName, tihabModuleState.displayName }), false, tihabState.iconResource, "", ""));
					}
				}
			}
			foreach (TISpaceBodyState tispaceBodyState in GameStateManager.AllSpaceBodies())
			{
				TIDateTime tidateTime2 = faction.ProspectorArrival(tispaceBodyState);
				if (tidateTime2 != null && tidateTime2.year == year && tidateTime2.month == month)
				{
					sortedList[tidateTime2.day].Add(new CalendarDayGridItemController.CalendarItem(tidateTime2, Loc.T("UI.Council.Calendar.SpaceBodyProspected", new object[] { tispaceBodyState.displayName }), false, tispaceBodyState.iconResource, "", ""));
				}
			}
			foreach (TICouncilorState ticouncilorState in faction.councilors)
			{
				if (ticouncilorState.HasMission)
				{
					TIDateTime resolveTime = ticouncilorState.activeMission.resolveTime;
					if (resolveTime != null && resolveTime.year == year)
					{
						TIDateTime resolveTime2 = ticouncilorState.activeMission.resolveTime;
						if (resolveTime2 != null && resolveTime2.month == month)
						{
							List<CalendarDayGridItemController.CalendarItem> list2 = sortedList[ticouncilorState.activeMission.resolveTime.day];
							TIDateTime resolveTime3 = ticouncilorState.activeMission.resolveTime;
							string text = Loc.T("UI.Council.Calendar.MissionResolution", new object[]
							{
								ticouncilorState.activeMission.displayName,
								ticouncilorState.activeMission.target.GetDisplayName(ticouncilorState.faction),
								ticouncilorState.displayName
							});
							bool flag = false;
							string iconResource = ticouncilorState.iconResource;
							TINationState ref_nation = ticouncilorState.activeMission.target.ref_nation;
							list2.Add(new CalendarDayGridItemController.CalendarItem(resolveTime3, text, flag, iconResource, ((ref_nation != null) ? ref_nation.flagResource : null) ?? "", ""));
						}
					}
				}
			}
			foreach (TISpaceFleetState tispaceFleetState in faction.KnownFleets)
			{
				if (tispaceFleetState.inTransfer && tispaceFleetState.trajectory.arrivalTime.year == year && tispaceFleetState.trajectory.arrivalTime.month == month)
				{
					TIOrbitState ref_orbit = tispaceFleetState.trajectory.destination.ref_orbit;
					if (ref_orbit != null && ref_orbit.OrbitOfInterest(faction, 3))
					{
						sortedList[tispaceFleetState.trajectory.arrivalTime.day].Add(new CalendarDayGridItemController.CalendarItem(tispaceFleetState.trajectory.arrivalTime, Loc.T("UI.Council.Calendar.FleetArrival", new object[]
						{
							tispaceFleetState.GetDisplayName(faction),
							TIUtilities.GetLocationString(tispaceFleetState.trajectory.destination, true, true)
						}), false, tispaceFleetState.iconResource, "", ""));
					}
				}
			}
			sortedList[1].Add(new CalendarDayGridItemController.CalendarItem(new TIDateTime(year, month, 1), Loc.T("UI.Council.Calendar.NewCouncilors"), false, "", "", ""));
			sortedList[15].Add(new CalendarDayGridItemController.CalendarItem(new TIDateTime(year, month, 15), Loc.T("UI.Council.Calendar.NewOrgs"), false, "", "", ""));
			TIDateTime tidateTime3 = new TIDateTime(year, month, 1);
			tidateTime3.AddDays(-1f);
			bool flag2 = false;
			int num = 0;
			while (!flag2 && num < 32)
			{
				TIDateTime tidateTime4 = TIControlPoint.FindMissionPhaseAfter(tidateTime3);
				if (tidateTime4.month == month && tidateTime4.year == year)
				{
					sortedList[tidateTime4.day].Add(new CalendarDayGridItemController.CalendarItem(new TIDateTime(year, month, tidateTime4.day, tidateTime4.hour, tidateTime4.minute), Loc.T("UI.Council.Calendar.MissionAssignments"), false, "", "", ""));
					tidateTime3 = tidateTime4;
				}
				else
				{
					flag2 = true;
				}
				num++;
			}
			foreach (Alarm alarm in faction.alarms)
			{
				if (alarm.time != null && alarm.time.year == year && alarm.time.month == month && (alarm.alarmType != AlarmType.FleetApproaching || TIGameState.Valid(alarm.associatedGameState)))
				{
					sortedList[alarm.time.day].Add(new CalendarDayGridItemController.CalendarItem(alarm.time, TINotificationQueueState.AlarmString(GameControl.control.activePlayer, alarm.alarmEvent.eventObject2, alarm), true, TemplateManager.global.pathWarningIcon, "", ""));
				}
			}
			return sortedList;
		}

		// Token: 0x04002BF4 RID: 11252
		public TMP_Text dayNumber;

		// Token: 0x04002BF5 RID: 11253
		public ListManagerBase todaysEventList;

		// Token: 0x04002BF6 RID: 11254
		public Button alarmButton;

		// Token: 0x04002BF7 RID: 11255
		public TooltipTrigger tip;

		// Token: 0x04002BF8 RID: 11256
		private TIDateTime date;

		// Token: 0x0200101E RID: 4126
		public struct CooldownPair
		{
			// Token: 0x040061BA RID: 25018
			public TINationState nation1;

			// Token: 0x040061BB RID: 25019
			public TINationState nation2;
		}

		// Token: 0x0200101F RID: 4127
		public struct CalendarItem
		{
			// Token: 0x06008241 RID: 33345 RVA: 0x0032C08E File Offset: 0x0032A28E
			public CalendarItem(TIDateTime dateTime, string description, bool alarm = false, string icon = "", string flag1 = "", string flag2 = "")
			{
				this.dateTime = dateTime;
				this.description = description;
				this.flag1 = flag1;
				this.flag2 = flag2;
				this.icon = icon;
				this.alarm = alarm;
			}

			// Token: 0x040061BC RID: 25020
			public TIDateTime dateTime;

			// Token: 0x040061BD RID: 25021
			public string description;

			// Token: 0x040061BE RID: 25022
			public string icon;

			// Token: 0x040061BF RID: 25023
			public string flag1;

			// Token: 0x040061C0 RID: 25024
			public string flag2;

			// Token: 0x040061C1 RID: 25025
			public bool alarm;
		}
	}
}
