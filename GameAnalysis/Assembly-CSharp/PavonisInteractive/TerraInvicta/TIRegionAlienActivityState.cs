using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200076E RID: 1902
	public class TIRegionAlienActivityState : TIRegionAlienEntityState
	{
		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x060039B5 RID: 14773 RVA: 0x001554F4 File Offset: 0x001536F4
		public override TIRegionAlienActivityState ref_regionAlienActivity
		{
			get
			{
				return this;
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x060039B6 RID: 14774 RVA: 0x001554F7 File Offset: 0x001536F7
		public override bool isRegionAlienActivity
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060039B7 RID: 14775 RVA: 0x001554FA File Offset: 0x001536FA
		public void InitWithRegionState(TIRegionState region)
		{
			if (!this.gameStateSubjectCreated)
			{
				if (region.template == null)
				{
					return;
				}
				this.templateName = region.template.dataName;
				base.region = region;
				this.alienMissionsDetected = new Dictionary<TIFactionState, List<string>>();
			}
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x060039B8 RID: 14776 RVA: 0x00155538 File Offset: 0x00153738
		public override void PostAllStartUpInit_5()
		{
			foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
			{
				if (this.alienMissionsDetected.ContainsKey(tifactionState))
				{
					foreach (string text in this.alienMissionsDetected[tifactionState])
					{
						GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledActivityExpiration), this.timeEventName(tifactionState, text), null, true, false);
					}
				}
			}
		}

		// Token: 0x060039B9 RID: 14777 RVA: 0x001555D4 File Offset: 0x001537D4
		public override bool Extant()
		{
			return this.alienMissionsDetected.Count > 0;
		}

		// Token: 0x060039BA RID: 14778 RVA: 0x001555E4 File Offset: 0x001537E4
		public override string GetIconResourcePath(TIFactionState faction)
		{
			if (this.MissionDetectedByFaction(faction, "TerrorizeRegion"))
			{
				return TemplateManager.global.pathGeoscapeTerrorize;
			}
			if (this.MissionDetectedByFaction(faction, "EnthrallElites") || this.MissionDetectedByFaction(faction, "EnthrallUnalignedElites") || this.MissionDetectedByFaction(faction, "EnthrallOrg"))
			{
				return TemplateManager.global.pathGeoscapeEnthrallElites;
			}
			if (this.MissionDetectedByFaction(faction, "EnthrallPublic"))
			{
				return TemplateManager.global.pathGeoscapeEnthrallPublic;
			}
			if (this.MissionDetectedByFaction(faction, "Abductions"))
			{
				return TemplateManager.global.pathGeoscapeAbductions;
			}
			return TemplateManager.global.pathGeoscapeAlienActivity;
		}

		// Token: 0x060039BB RID: 14779 RVA: 0x0015567C File Offset: 0x0015387C
		public override string GetIllustrationPath(TIFactionState faction)
		{
			if (this.MissionDetectedByFaction(faction, "TerrorizeRegion"))
			{
				return TemplateManager.global.illus_terrorize;
			}
			if (this.MissionDetectedByFaction(faction, "EnthrallElites") || this.MissionDetectedByFaction(faction, "EnthrallUnalignedElites") || this.MissionDetectedByFaction(faction, "EnthrallOrg"))
			{
				return TemplateManager.global.illus_enthrallElites;
			}
			if (this.MissionDetectedByFaction(faction, "EnthrallPublic"))
			{
				return TemplateManager.global.illus_enthrallPublic;
			}
			if (this.MissionDetectedByFaction(faction, "Abductions"))
			{
				return TemplateManager.global.illus_abductions;
			}
			return TemplateManager.global.illus_alienActivity;
		}

		// Token: 0x060039BC RID: 14780 RVA: 0x00155713 File Offset: 0x00153913
		private string timeEventName(TIFactionState faction, string mission)
		{
			return new StringBuilder("ExpireAlienActivity").Append(faction.template.dataName).Append(base.region.template.dataName).Append(mission)
				.ToString();
		}

		// Token: 0x060039BD RID: 14781 RVA: 0x00155750 File Offset: 0x00153950
		public void ActivitySightedByFaction(TIFactionState faction, TIMissionTemplate missionTemplate, TICouncilorState targetCouncilor, TIFactionState targetFaction, TIMissionState mission = null)
		{
			faction.SetIntel(this, 1f, null, false);
			if (!this.alienMissionsDetected.ContainsKey(faction))
			{
				this.alienMissionsDetected.Add(faction, new List<string>());
			}
			this.alienMissionsDetected[faction].Add(missionTemplate.dataName);
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddMonths(2);
			DateTime dateTime = tidateTime.ExportTime();
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59, 59);
			tidateTime.ImportTime(dateTime2);
			TITimeEvent.CreateNewTimeEvent(tidateTime, faction, null, missionTemplate, this.timeEventName(faction, missionTemplate.dataName), true, false, TITimeQueueRepeatType.None, 1, true, false);
			if (this.alienMissionsDetected[faction].Count == 1)
			{
				GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledActivityExpiration), this.timeEventName(faction, missionTemplate.dataName), null, true, false);
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
			string dataName = missionTemplate.dataName;
			if (dataName != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(dataName);
				if (num <= 2276976907U)
				{
					if (num <= 457615351U)
					{
						if (num != 431564165U)
						{
							if (num != 457615351U)
							{
								return;
							}
							if (!(dataName == "Xenoform"))
							{
								return;
							}
							TINotificationQueueState.LogXenoformMission(faction, base.region, false);
							faction.SetIntel(base.region.xenoforming, 1f, (mission != null) ? mission.councilor : null, false);
							return;
						}
						else
						{
							if (!(dataName == "Abductions"))
							{
								return;
							}
							TINotificationQueueState.LogAbductions(faction, base.region);
							return;
						}
					}
					else if (num != 1412701134U)
					{
						if (num != 2276976907U)
						{
							return;
						}
						if (!(dataName == "TerrorizeRegion"))
						{
							return;
						}
						TINotificationQueueState.LogTerrorize(faction, base.region);
						return;
					}
					else
					{
						if (!(dataName == "BuildFacility"))
						{
							return;
						}
						faction.SetIntel(base.region.alienFacility, 1f, null, false);
					}
				}
				else
				{
					if (num > 2592082955U)
					{
						if (num != 3176398323U)
						{
							if (num != 3917615220U)
							{
								return;
							}
							if (!(dataName == "EnthrallUnalignedElites"))
							{
								return;
							}
						}
						else if (!(dataName == "EnthrallElites"))
						{
							return;
						}
						TINotificationQueueState.LogEnthrallElites(faction, base.region);
						return;
					}
					if (num != 2349948754U)
					{
						if (num != 2592082955U)
						{
							return;
						}
						if (!(dataName == "EnthrallOrg"))
						{
							return;
						}
						if (mission != null)
						{
							TINotificationQueueState.LogEnthrallOrg(faction, base.region, mission.target.ref_org, targetCouncilor, targetFaction);
							return;
						}
					}
					else
					{
						if (!(dataName == "EnthrallPublic"))
						{
							return;
						}
						TINotificationQueueState.LogEnthrallPublic(faction, base.region);
						return;
					}
				}
			}
		}

		// Token: 0x060039BE RID: 14782 RVA: 0x001559F3 File Offset: 0x00153BF3
		public void ScheduledActivityExpiration(TimeEventStart e)
		{
			this.RemoveMissionFromActivity(e.eventObject as TIFactionState, (e.eventDataTemplate as TIMissionTemplate).dataName);
		}

		// Token: 0x060039BF RID: 14783 RVA: 0x00155A18 File Offset: 0x00153C18
		public void RemoveMissionFromActivity(TIFactionState faction, string mission)
		{
			this.alienMissionsDetected[faction].Remove(mission);
			if (!this.alienMissionsDetected[faction].Contains(mission))
			{
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledActivityExpiration), this.timeEventName(faction, mission));
			}
			if (this.alienMissionsDetected[faction].Count == 0)
			{
				faction.ExpireIntel(this, true);
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
		}

		// Token: 0x060039C0 RID: 14784 RVA: 0x00155AAC File Offset: 0x00153CAC
		public void RemoveActivity(TIFactionState faction)
		{
			foreach (string text in this.alienMissionsDetected[faction])
			{
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledActivityExpiration), this.timeEventName(faction, text));
			}
			this.alienMissionsDetected[faction].Clear();
			faction.ExpireIntel(this, true);
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
		}

		// Token: 0x060039C1 RID: 14785 RVA: 0x00155B5C File Offset: 0x00153D5C
		public override bool VisibleToFaction(TIFactionState faction)
		{
			return this.alienMissionsDetected.ContainsKey(faction) && faction.GetIntel(this) > 0f && this.alienMissionsDetected[faction].Count > 0;
		}

		// Token: 0x060039C2 RID: 14786 RVA: 0x00155B90 File Offset: 0x00153D90
		public List<string> GetMissionList(TIFactionState faction)
		{
			if (!this.alienMissionsDetected.ContainsKey(faction))
			{
				return new List<string>();
			}
			return this.alienMissionsDetected[faction];
		}

		// Token: 0x060039C3 RID: 14787 RVA: 0x00155BB2 File Offset: 0x00153DB2
		public bool MissionDetectedByFaction(TIFactionState faction, string missionDataName)
		{
			return this.GetMissionList(faction).Contains(missionDataName);
		}

		// Token: 0x0400256E RID: 9582
		[SerializeField]
		protected Dictionary<TIFactionState, List<string>> alienMissionsDetected;
	}
}
