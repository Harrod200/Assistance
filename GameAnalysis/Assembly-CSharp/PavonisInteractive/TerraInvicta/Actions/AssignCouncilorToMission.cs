using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A4E RID: 2638
	public class AssignCouncilorToMission : PlayerAction
	{
		// Token: 0x060064E9 RID: 25833 RVA: 0x002FA4F4 File Offset: 0x002F86F4
		public AssignCouncilorToMission(TICouncilorState councilor, TIMissionTemplate missionTemplate, TIGameState target, float resourcesSpend, bool forceMission = false)
		{
			this.councilorID = councilor.ID;
			if (target != null)
			{
				this.targetID = target.ID;
			}
			if (forceMission)
			{
				missionTemplate.debugForced = true;
				if (target == null)
				{
					target = councilor;
					this.targetID = councilor.ID;
				}
			}
			this.missionTemplate = missionTemplate;
			this.resourcesSpend = resourcesSpend;
		}

		// Token: 0x060064EA RID: 25834 RVA: 0x002FA55C File Offset: 0x002F875C
		public override void Execute()
		{
			TICouncilorState state = this.councilorID.GetState<TICouncilorState>(false);
			TIFactionState faction = state.faction;
			TIGameState state2 = this.targetID.GetState();
			TIMissionState activeMission = state.activeMission;
			if (activeMission != null && TIMissionPhaseState.InMissionPhase() && activeMission.missionTemplate.hasCost)
			{
				faction.AddToCurrentResource(activeMission.resources, activeMission.missionTemplate.cost.resourceType, false, null);
			}
			TIMissionState timissionState = this.missionTemplate.CreateGameState() as TIMissionState;
			timissionState.InitWithTemplate(this.missionTemplate);
			timissionState.target = state2;
			timissionState.councilor = state;
			timissionState.resources = this.resourcesSpend;
			if (this.missionTemplate.hasCost)
			{
				if (this.missionTemplate.cost is TIMissionCost_Bonus)
				{
					faction.AddToCurrentResource(-this.resourcesSpend, this.missionTemplate.cost.resourceType, false, this.missionTemplate.dataName);
				}
				else if (this.missionTemplate.cost is TIMissionCost_Flat)
				{
					faction.AddToCurrentResource(-this.resourcesSpend, this.missionTemplate.cost.resourceType, false, this.missionTemplate.dataName);
				}
			}
			state.SetActiveMission(timissionState);
			if (this.missionTemplate != null)
			{
				GameControl.eventManager.TriggerEvent(new CouncilorMissionAssigned(state, timissionState), null, new object[] { state.faction });
				state.ChangeLocation(timissionState.GetInitialMissionLocation());
			}
			if (TIGlobalValuesState.isTutorialActive)
			{
				string dataName = timissionState.missionTemplate.dataName;
				if (dataName != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(dataName);
					if (num <= 674146076U)
					{
						if (num != 182146424U)
						{
							if (num != 429902756U)
							{
								if (num != 674146076U)
								{
									return;
								}
								if (!(dataName == "Propaganda"))
								{
									return;
								}
								faction.CompleteMilestone(CampaignMilestone.TutorialAssignPublicCampaign);
								return;
							}
							else
							{
								if (!(dataName == "Purge"))
								{
									return;
								}
								faction.CompleteMilestone(CampaignMilestone.TutorialAssignPurge);
								return;
							}
						}
						else
						{
							if (!(dataName == "Unrest"))
							{
								return;
							}
							faction.CompleteMilestone(CampaignMilestone.TutorialAssignIncreaseUnrest);
							return;
						}
					}
					else if (num <= 1386603441U)
					{
						if (num != 752956076U)
						{
							if (num != 1386603441U)
							{
								return;
							}
							if (!(dataName == "Crackdown"))
							{
								return;
							}
							faction.CompleteMilestone(CampaignMilestone.TutorialAssignCrackdown);
							return;
						}
						else
						{
							if (!(dataName == "DefendInterests"))
							{
								return;
							}
							faction.CompleteMilestone(CampaignMilestone.TutorialAssignDefendInterests);
						}
					}
					else if (num != 1619260818U)
					{
						if (num == 3574394417U)
						{
							if (!(dataName == "GainInfluence"))
							{
								return;
							}
							faction.CompleteMilestone(CampaignMilestone.TutorialAssignControlNationMission);
							return;
						}
					}
					else
					{
						if (!(dataName == "Coup"))
						{
							return;
						}
						faction.CompleteMilestone(CampaignMilestone.TutorialAssignCoup);
						return;
					}
				}
			}
		}

		// Token: 0x04004704 RID: 18180
		private GameStateID councilorID;

		// Token: 0x04004705 RID: 18181
		private GameStateID targetID;

		// Token: 0x04004706 RID: 18182
		private readonly float resourcesSpend;

		// Token: 0x04004707 RID: 18183
		private TIMissionTemplate missionTemplate;
	}
}
