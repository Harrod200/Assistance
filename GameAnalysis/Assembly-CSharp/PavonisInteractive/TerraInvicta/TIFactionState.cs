using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FullSerializer;
using Newtonsoft.Json;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.Tasks;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using Steamworks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000760 RID: 1888
	public class TIFactionState : TIGameState, IGameStateVisualizer, IOperationCapableState
	{
		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06003173 RID: 12659 RVA: 0x00109E08 File Offset: 0x00108008
		// (set) Token: 0x06003174 RID: 12660 RVA: 0x00109E10 File Offset: 0x00108010
		public Dictionary<TIHabModuleState, List<ShipConstructionQueueItem>> nShipyardQueues { get; private set; } = new Dictionary<TIHabModuleState, List<ShipConstructionQueueItem>>();

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06003175 RID: 12661 RVA: 0x00109E19 File Offset: 0x00108019
		// (set) Token: 0x06003176 RID: 12662 RVA: 0x00109E21 File Offset: 0x00108021
		public Dictionary<string, float> techNameContributionHistory { get; private set; }

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06003177 RID: 12663 RVA: 0x00109E2A File Offset: 0x0010802A
		// (set) Token: 0x06003178 RID: 12664 RVA: 0x00109E32 File Offset: 0x00108032
		public bool unlockedVictoryObjective { get; private set; }

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06003179 RID: 12665 RVA: 0x00109E3B File Offset: 0x0010803B
		// (set) Token: 0x0600317A RID: 12666 RVA: 0x00109E43 File Offset: 0x00108043
		public List<string> finishedProjectNames { get; private set; }

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x0600317B RID: 12667 RVA: 0x00109E4C File Offset: 0x0010804C
		// (set) Token: 0x0600317C RID: 12668 RVA: 0x00109E54 File Offset: 0x00108054
		public bool orgProjectSlotUnlocked { get; private set; }

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x0600317D RID: 12669 RVA: 0x00109E5D File Offset: 0x0010805D
		// (set) Token: 0x0600317E RID: 12670 RVA: 0x00109E65 File Offset: 0x00108065
		public bool habProjectSlotUnlocked { get; private set; }

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x0600317F RID: 12671 RVA: 0x00109E6E File Offset: 0x0010806E
		// (set) Token: 0x06003180 RID: 12672 RVA: 0x00109E76 File Offset: 0x00108076
		public int atrocities { get; private set; }

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06003181 RID: 12673 RVA: 0x00109E7F File Offset: 0x0010807F
		// (set) Token: 0x06003182 RID: 12674 RVA: 0x00109E87 File Offset: 0x00108087
		public List<CampaignMilestone> milestones { get; private set; }

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06003183 RID: 12675 RVA: 0x00109E90 File Offset: 0x00108090
		// (set) Token: 0x06003184 RID: 12676 RVA: 0x00109E98 File Offset: 0x00108098
		public string factionOperationCompleteName { get; private set; }

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06003185 RID: 12677 RVA: 0x00109EA1 File Offset: 0x001080A1
		// (set) Token: 0x06003186 RID: 12678 RVA: 0x00109EA9 File Offset: 0x001080A9
		public List<PolicyOptionWithTarget> plannedPolicies { get; private set; }

		// Token: 0x06003187 RID: 12679 RVA: 0x00109EB2 File Offset: 0x001080B2
		public bool permanentAlly(TIFactionState faction)
		{
			return faction != null && (faction == this || (faction.IsAlienFaction && this.IsAlienProxy) || (faction.IsAlienProxy && this.IsAlienFaction));
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06003188 RID: 12680 RVA: 0x00109EEA File Offset: 0x001080EA
		public override bool isFactionState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06003189 RID: 12681 RVA: 0x00109EED File Offset: 0x001080ED
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x0600318A RID: 12682 RVA: 0x00109EF0 File Offset: 0x001080F0
		public override TIFactionState ref_faction
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x0600318B RID: 12683 RVA: 0x00109EF3 File Offset: 0x001080F3
		public TIFactionTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIFactionTemplate>();
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x0600318C RID: 12684 RVA: 0x00109EFB File Offset: 0x001080FB
		public Player playerControl
		{
			get
			{
				if (this._playerControl == null)
				{
					this._playerControl = GameControl.playerManager.FindPlayer(this.player.ID).GetComponent<Player>();
				}
				return this._playerControl;
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x0600318D RID: 12685 RVA: 0x00109F31 File Offset: 0x00108131
		public Vector3 ideologyCoordinates
		{
			get
			{
				return this.ideology.ideologyCoordinates;
			}
		}

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x0600318E RID: 12686 RVA: 0x00109F3E File Offset: 0x0010813E
		public List<TICouncilorState> activeCouncilors
		{
			get
			{
				return this.councilors.Where<TICouncilorState>((TICouncilorState x) => x.active).ToList<TICouncilorState>();
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x0600318F RID: 12687 RVA: 0x00109F6F File Offset: 0x0010816F
		public List<TISpaceShipState> ships
		{
			get
			{
				return this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).ToList<TISpaceShipState>();
			}
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06003190 RID: 12688 RVA: 0x00109FA0 File Offset: 0x001081A0
		public List<TISpaceShipState> knownShips
		{
			get
			{
				return this.KnownFleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).ToList<TISpaceShipState>();
			}
		}

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06003191 RID: 12689 RVA: 0x00109FD1 File Offset: 0x001081D1
		public int numActiveCouncilors
		{
			get
			{
				return this.activeCouncilors.Count;
			}
		}

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06003192 RID: 12690 RVA: 0x00109FDE File Offset: 0x001081DE
		public bool IsActiveHumanFaction
		{
			get
			{
				return !this.template.isAlien;
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06003193 RID: 12691 RVA: 0x00109FEE File Offset: 0x001081EE
		public bool IsAlienFaction
		{
			get
			{
				return this.template.isAlien;
			}
		}

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06003194 RID: 12692 RVA: 0x00109FFB File Offset: 0x001081FB
		public bool IsAlienProxy
		{
			get
			{
				return this == GameStateManager.AlienProxy();
			}
		}

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06003195 RID: 12693 RVA: 0x0010A008 File Offset: 0x00108208
		public bool isAlienAppeaser
		{
			get
			{
				return this == GameStateManager.AlienAppeaser();
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06003196 RID: 12694 RVA: 0x0010A015 File Offset: 0x00108215
		public bool isActivePlayer
		{
			get
			{
				return GameControl.control.activePlayer == this;
			}
		}

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06003197 RID: 12695 RVA: 0x0010A027 File Offset: 0x00108227
		public bool veryProAlien
		{
			get
			{
				return this.ideologyCoordinates.x <= -1f;
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06003198 RID: 12696 RVA: 0x0010A03E File Offset: 0x0010823E
		public bool proAlien
		{
			get
			{
				return this.ideologyCoordinates.x < 0f;
			}
		}

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06003199 RID: 12697 RVA: 0x0010A052 File Offset: 0x00108252
		public bool antiAlien
		{
			get
			{
				return this.ideologyCoordinates.x > 0f;
			}
		}

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x0600319A RID: 12698 RVA: 0x0010A066 File Offset: 0x00108266
		public bool veryAntiAlien
		{
			get
			{
				return this.ideologyCoordinates.x >= 1f;
			}
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x0600319B RID: 12699 RVA: 0x0010A07D File Offset: 0x0010827D
		public bool malleable
		{
			get
			{
				return this.ideologyCoordinates.y >= Mathf.Abs(this.ideologyCoordinates.x);
			}
		}

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x0600319C RID: 12700 RVA: 0x0010A09F File Offset: 0x0010829F
		public bool extremist
		{
			get
			{
				return Mathf.Abs(this.ideologyCoordinates.x) >= 2f;
			}
		}

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x0600319D RID: 12701 RVA: 0x0010A0BB File Offset: 0x001082BB
		public bool shouldNeverAttackAliens
		{
			get
			{
				return this.ideologyCoordinates.x < -1f && this.aiValues.protectAlienLife >= 0.9f;
			}
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x0600319E RID: 12702 RVA: 0x0010A0E6 File Offset: 0x001082E6
		public bool cynical
		{
			get
			{
				return this.ideologyCoordinates.y < 0f;
			}
		}

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x0600319F RID: 12703 RVA: 0x0010A0FA File Offset: 0x001082FA
		public bool believers
		{
			get
			{
				return this.ideologyCoordinates.y > 0f;
			}
		}

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x060031A0 RID: 12704 RVA: 0x0010A10E File Offset: 0x0010830E
		public bool currentlyDetectingHydra
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.Abductions || x.targetMissionTarget == ObjectiveMissionTargetType.EnthrallMission || x.targetMissionTarget == ObjectiveMissionTargetType.HydraCouncilor || x.targetMilestone == CampaignMilestone.AccessHydraCorpus || x.targetMilestone == CampaignMilestone.AccessLiveHydra);
			}
		}

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x060031A1 RID: 12705 RVA: 0x0010A13C File Offset: 0x0010833C
		public bool currentlySearchingForHydraCouncilor
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.HydraCouncilor || x.targetMilestone == CampaignMilestone.AccessHydraCorpus || x.targetMilestone == CampaignMilestone.AccessLiveHydra);
			}
		}

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x060031A2 RID: 12706 RVA: 0x0010A16A File Offset: 0x0010836A
		public bool currentlyHuntingHydraMissions
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.EnthrallMission || x.targetMissionTarget == ObjectiveMissionTargetType.Abductions);
			}
		}

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x060031A3 RID: 12707 RVA: 0x0010A198 File Offset: 0x00108398
		public bool currentlyTryingToContactHydra
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTemplateName == TIFactionState.contactMission.dataName && x.targetMissionTarget == ObjectiveMissionTargetType.HydraCouncilor);
			}
		}

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x060031A4 RID: 12708 RVA: 0x0010A1C6 File Offset: 0x001083C6
		public bool currentlyHuntingHydraToKill
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMilestone == CampaignMilestone.AccessHydraCorpus);
			}
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x060031A5 RID: 12709 RVA: 0x0010A1F4 File Offset: 0x001083F4
		public bool currentlyCapturingHydra
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMilestone == CampaignMilestone.AccessLiveHydra);
			}
		}

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x060031A6 RID: 12710 RVA: 0x0010A222 File Offset: 0x00108422
		public bool huntingAlienWarship
		{
			get
			{
				return this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMilestone == CampaignMilestone.AccessAlienShip || x.targetMilestone == CampaignMilestone.AccessAlienTech);
			}
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x060031A7 RID: 12711 RVA: 0x0010A250 File Offset: 0x00108450
		public bool CanSellSpaceResourcesOnEarth
		{
			get
			{
				return this.habs.Any<TIHabState>((TIHabState x) => x.CanSellResources(this));
			}
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x060031A8 RID: 12712 RVA: 0x0010A269 File Offset: 0x00108469
		public string displayNameWithColor
		{
			get
			{
				return new StringBuilder(this.template.inlineColorString).Append(this.displayName).Append("</color>").ToString();
			}
		}

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x060031A9 RID: 12713 RVA: 0x0010A295 File Offset: 0x00108495
		public string displayNameCapitalized
		{
			get
			{
				if (!this.scenarioCustomizations.usingCustomizations || !this.scenarioCustomizations.customFactionText.ContainsKey(this.templateName))
				{
					return this.template.capitalizedFactionName;
				}
				return Utilities.Capitalize(this.displayName);
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x060031AA RID: 12714 RVA: 0x0010A2D3 File Offset: 0x001084D3
		public string displayNameCapitalizedWithColor
		{
			get
			{
				return new StringBuilder(this.template.inlineColorString).Append(this.displayNameCapitalized).Append("</color>").ToString();
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x060031AB RID: 12715 RVA: 0x0010A300 File Offset: 0x00108500
		public string adjective
		{
			get
			{
				if (!this.scenarioCustomizations.usingCustomizations || !this.scenarioCustomizations.customFactionText.ContainsKey(this.templateName))
				{
					return this.template.adjective;
				}
				return this.scenarioCustomizations.customFactionText[this.templateName].customAdjective;
			}
		}

		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x060031AC RID: 12716 RVA: 0x0010A359 File Offset: 0x00108559
		public string adjectiveWithColor
		{
			get
			{
				return new StringBuilder(this.template.inlineColorString).Append(this.adjective).Append("</color>").ToString();
			}
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x060031AD RID: 12717 RVA: 0x0010A385 File Offset: 0x00108585
		public string inlineControlPointCapIcon
		{
			get
			{
				return new StringBuilder(this.template.inlineColorString).Append(TIGlobalConfig.globalConfig.controlPointInlineSpritePath_color).Append("</color>").ToString();
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x060031AE RID: 12718 RVA: 0x0010A3B5 File Offset: 0x001085B5
		public string leaderName
		{
			get
			{
				return this.template.leaderName;
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x060031AF RID: 12719 RVA: 0x0010A3C4 File Offset: 0x001085C4
		public string leaderAddress
		{
			get
			{
				if (!this.scenarioCustomizations.usingCustomizations || !this.scenarioCustomizations.customFactionText.ContainsKey(this.templateName))
				{
					return this.template.leaderAddress;
				}
				return this.scenarioCustomizations.customFactionText[this.templateName].customLeaderAddress;
			}
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x060031B0 RID: 12720 RVA: 0x0010A41D File Offset: 0x0010861D
		public string leaderNameWithAddress
		{
			get
			{
				return Loc.T("UI.Council.LeaderFullName", new object[] { this.leaderAddress, this.leaderName });
			}
		}

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x060031B1 RID: 12721 RVA: 0x0010A444 File Offset: 0x00108644
		public string fleetNameBase
		{
			get
			{
				if (!this.scenarioCustomizations.usingCustomizations || !this.scenarioCustomizations.customFactionText.ContainsKey(this.templateName))
				{
					return this.template.fleetNameBase;
				}
				return this.scenarioCustomizations.customFactionText[this.templateName].customFleetNameBase;
			}
		}

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x060031B2 RID: 12722 RVA: 0x0010A49D File Offset: 0x0010869D
		public string introduction
		{
			get
			{
				return this.template.introduction;
			}
		}

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x060031B3 RID: 12723 RVA: 0x0010A4AA File Offset: 0x001086AA
		public string goal
		{
			get
			{
				return this.template.goal;
			}
		}

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x060031B4 RID: 12724 RVA: 0x0010A4B7 File Offset: 0x001086B7
		public TIOrgTemplate winningOrgTemplate
		{
			get
			{
				return TemplateManager.Find<TIOrgTemplate>(this.template.winningOrg, false);
			}
		}

		// Token: 0x060031B5 RID: 12725 RVA: 0x0010A4CC File Offset: 0x001086CC
		public bool CanBeDisabled()
		{
			if (!TIGlobalValuesState.CanDisableFactions || TIMissionPhaseState.phasesPerMonth >= 2f || this.IsAlienProxy || this.isAlienAppeaser || this.IsAlienFaction || this.isActivePlayer)
			{
				return false;
			}
			if (!GameControl.control.activePlayer.veryAntiAlien)
			{
				return this != GameStateManager.AllFactions().MaxBy<TIFactionState, float>((TIFactionState x) => x.ideologyCoordinates.x);
			}
			return true;
		}

		// Token: 0x060031B6 RID: 12726 RVA: 0x0010A550 File Offset: 0x00108750
		public bool Defeated()
		{
			return this.CanBeDisabled() && this.fleets.Count == 0 && this.habs.Count == 0 && this.councilors.Count == 0 && this.controlPoints.Count == 0;
		}

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x060031B7 RID: 12727 RVA: 0x0010A59C File Offset: 0x0010879C
		public string factionIcon64path
		{
			get
			{
				return this.template.councilIcon64;
			}
		}

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x060031B8 RID: 12728 RVA: 0x0010A5A9 File Offset: 0x001087A9
		public string factionIcon128path
		{
			get
			{
				return this.template.councilIcon128;
			}
		}

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x060031B9 RID: 12729 RVA: 0x0010A5B6 File Offset: 0x001087B6
		public string factionIcon256path
		{
			get
			{
				return this.template.councilIcon256;
			}
		}

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x060031BA RID: 12730 RVA: 0x0010A5C3 File Offset: 0x001087C3
		public string factionIcon64UIpath
		{
			get
			{
				return this.template.councilIcon64_ui;
			}
		}

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x060031BB RID: 12731 RVA: 0x0010A5D0 File Offset: 0x001087D0
		public string factionIcon128UIpath
		{
			get
			{
				return this.template.councilIcon128_ui;
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x060031BC RID: 12732 RVA: 0x0010A5DD File Offset: 0x001087DD
		public string factionIcon256UIpath
		{
			get
			{
				return this.template.councilIcon256_ui;
			}
		}

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x060031BD RID: 12733 RVA: 0x0010A5EA File Offset: 0x001087EA
		public string cursorPath
		{
			get
			{
				return this.template.cursorPath;
			}
		}

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x060031BE RID: 12734 RVA: 0x0010A5F7 File Offset: 0x001087F7
		public string cinematicsPath
		{
			get
			{
				return this.template.cinematicsPath;
			}
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x060031BF RID: 12735 RVA: 0x0010A604 File Offset: 0x00108804
		public float recentDailySpoilsIncome
		{
			get
			{
				return this.lastWeeksSpoils / 7f;
			}
		}

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x060031C0 RID: 12736 RVA: 0x0010A612 File Offset: 0x00108812
		public float mediumTermDailySpoilsIncome
		{
			get
			{
				return this.lastMonthsSpoils / 30.436874f;
			}
		}

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x060031C1 RID: 12737 RVA: 0x0010A620 File Offset: 0x00108820
		public ScenarioCustomizations scenarioCustomizations
		{
			get
			{
				return TIGlobalValuesState.Customizations;
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x060031C2 RID: 12738 RVA: 0x0010A628 File Offset: 0x00108828
		public Dictionary<FactionResource, float> copyResources
		{
			get
			{
				return this.resources.ToDictionary<KeyValuePair<FactionResource, float>, FactionResource, float>((KeyValuePair<FactionResource, float> x) => x.Key, (KeyValuePair<FactionResource, float> x) => x.Value);
			}
		}

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x060031C3 RID: 12739 RVA: 0x0010A680 File Offset: 0x00108880
		public List<TISpaceAssetState> spaceAssets
		{
			get
			{
				return this.habs.ConvertAll<TISpaceAssetState>((TIHabState x) => x.ref_spaceAsset).Union<TISpaceAssetState>(this.fleets.ConvertAll<TISpaceAssetState>((TISpaceFleetState x) => x.ref_spaceAsset)).ToList<TISpaceAssetState>();
			}
		}

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x060031C4 RID: 12740 RVA: 0x0010A6EC File Offset: 0x001088EC
		public TIHabState primaryStation
		{
			get
			{
				if (this.primaryHab == null)
				{
					return null;
				}
				if (this.primaryHab.IsStation)
				{
					return this.primaryHab;
				}
				if (this.IsAlienFaction)
				{
					return this.stations.FirstOrDefault<TIHabState>((TIHabState x) => x.ref_system == this.primaryHab.ref_system);
				}
				return null;
			}
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x060031C5 RID: 12741 RVA: 0x0010A73E File Offset: 0x0010893E
		public TISpaceBodyState primarySystem
		{
			get
			{
				TIHabState tihabState = this.primaryHab;
				if (tihabState == null)
				{
					return null;
				}
				return tihabState.ref_system;
			}
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x0010A754 File Offset: 0x00108954
		public void ResetPrimaryHab()
		{
			if (this.IsActiveHumanFaction)
			{
				using (List<TIObjectiveTemplate>.Enumerator enumerator = this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIObjectiveTemplate objective = enumerator.Current;
						if (!string.IsNullOrEmpty(objective.targetHabModuleName))
						{
							Func<TIHabModuleState, bool> <>9__0;
							foreach (TIHabState tihabState in this.habs)
							{
								IEnumerable<TIHabModuleState> enumerable = tihabState.AllModules();
								Func<TIHabModuleState, bool> func;
								if ((func = <>9__0) == null)
								{
									func = (<>9__0 = (TIHabModuleState x) => x.moduleTemplate == objective.targetHabModuleTemplate);
								}
								if (enumerable.Any<TIHabModuleState>(func))
								{
									this.primaryHab = tihabState;
									return;
								}
							}
						}
					}
				}
				this.primaryHab = null;
			}
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x0010A84C File Offset: 0x00108A4C
		public override bool Initialize()
		{
			this.fleets = new List<TISpaceFleetState>();
			this.councilors = new List<TICouncilorState>();
			this.resources = new Dictionary<FactionResource, float>(14);
			this.baseIncomes_year = new Dictionary<FactionResource, float>();
			this.unassignedOrgs = new List<TIOrgState>();
			this.habSectors = new List<TISectorState>();
			this.availableCouncilors = new List<TICouncilorState>();
			this.availableOrgs = new List<TIOrgState>();
			this.availableProjectNames = new List<string>();
			this.activeProjectTriggers = new List<ProjectTrigger>();
			this.currentProjectProgress = new List<ProjectProgress>();
			this.finishedProjectNames = new List<string>();
			this.objectiveNames = new Dictionary<string, ObjectiveStatus>();
			this.intel = new Dictionary<TIGameState, float>();
			this.highestIntel = new Dictionary<TIGameState, float>();
			this.turnedCouncilors = new List<TICouncilorState>();
			this.knownSpies = new List<TICouncilorState>();
			this.factionHate = new Dictionary<TIFactionState, float>();
			this.internalCouncilorSuspicion = new Dictionary<TICouncilorState, float>();
			this.milestones = new List<CampaignMilestone>();
			this.knownAlienSites = new Dictionary<TIGameState, TIDateTime>();
			this.factionGoals = new Dictionary<GoalType, List<TIFactionGoalState>>();
			this.shipDesigns = new List<TISpaceShipTemplate>();
			this.shipRefitDesignNames = new List<string>();
			this.obsoleteShipDesigns = new List<string>();
			this.nShipyardQueues = new Dictionary<TIHabModuleState, List<ShipConstructionQueueItem>>();
			this.controlPoints = new List<TIControlPoint>();
			this.plannedPolicies = new List<PolicyOptionWithTarget>();
			this.armies = new List<TIArmyState>();
			this.customPresets = new List<TIPriorityPresetTemplate>();
			this.habDesigns = new List<TIHabTemplate>();
			this.techNameContributionHistory = new Dictionary<string, float>();
			this.lostControlPoints = new Dictionary<TIControlPoint, TIDateTime>();
			this.availableProjects = new List<TIProjectTemplate>();
			this.completedProjects = new List<TIProjectTemplate>();
			ArmyType[] array = (ArmyType[])Enum.GetValues(typeof(ArmyType));
			this.armiesLost = array.ToDictionary<ArmyType, ArmyType, int>((ArmyType x) => x, (ArmyType y) => 0);
			this.factionAssassinations = new Dictionary<TIFactionState, int>();
			this.intelSharingFactions = new List<TIFactionState>();
			return base.Initialize();
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x0010AA58 File Offset: 0x00108C58
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TIFactionTemplate tifactionTemplate = template as TIFactionTemplate;
			if (tifactionTemplate == null)
			{
				return;
			}
			this.templateName = tifactionTemplate.dataName;
			this.displayName = tifactionTemplate.displayName;
			this.defaultPriorityPresetTemplateName = tifactionTemplate.defaultPresetName;
			this.updateShipDesignsFlag = this.IsAlienFaction || TemplateManager.global.debug_shipDesignAI || GameStateManager.Time().template.globalTechsCompleted.Contains("OrbitalShipbuilding");
		}

		// Token: 0x060031C9 RID: 12745 RVA: 0x0010AAD1 File Offset: 0x00108CD1
		public static TIFactionState CreateDummy(TIFactionTemplate template)
		{
			return new TIFactionState
			{
				templateName = template.dataName,
				displayName = template.displayName,
				isDummy = true
			};
		}

		// Token: 0x060031CA RID: 12746 RVA: 0x0010AAF8 File Offset: 0x00108CF8
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (!this.gameStateSubjectCreated)
			{
				if (this.scenarioCustomizations.usingCustomizations && this.scenarioCustomizations.customFactionText.ContainsKey(this.templateName))
				{
					this.SetDisplayName(this.scenarioCustomizations.customFactionText[this.templateName].customDisplayName);
				}
				this.aiValues = default(AIValues);
				this.aiValues = this.template.AIValues[0];
				foreach (FactionResource factionResource in Enums.FactionResources)
				{
					if (factionResource != FactionResource.None)
					{
						this.resources.Add(factionResource, 0f);
						this.baseIncomes_year.Add(factionResource, 0f);
					}
				}
				foreach (ResourceValue resourceValue in this.template.startingResources)
				{
					if (resourceValue.resource != FactionResource.None)
					{
						this.AddToCurrentResource(resourceValue.value, resourceValue.resource, true, null);
					}
				}
				foreach (ResourceValue resourceValue2 in this.template.baseAnnualIncomes)
				{
					this.ChangeBaseResourceIncome(resourceValue2.resource, resourceValue2.value);
				}
				this.cachedTechTooltipStrings = new Dictionary<TIGenericTechTemplate, string>();
				this.researchWeights = new int[6];
				this.SetResearchPriority(3, 1);
				this.currentProjectProgress.Add(new ProjectProgress("Project_AudienceResearch", 3, 0f));
				this.AddAvailableProject("Project_AudienceResearch");
				this.AddAvailableProject("Project_CommercialResearch");
				this.AddAvailableProject("Project_OperationsResearch");
				this.AddAvailableProject("Project_ManagementResearch");
				if (this.IsAlienFaction)
				{
					this.knowsWinCondition = true;
					this.AddCompletedProject(TemplateManager.global.alienMasterProject);
					using (IEnumerator<TISpaceBodyState> enumerator2 = GameStateManager.IterateByClass<TISpaceBodyState>(false).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TISpaceBodyState tispaceBodyState = enumerator2.Current;
							this.SetIntel(tispaceBodyState, 1f, null, false);
						}
						goto IL_026C;
					}
				}
				foreach (string text in GameStateManager.Time().template.startingSurveyedSpaceBodies)
				{
					TISpaceBodyState tispaceBodyState2 = GameStateManager.FindByTemplate<TISpaceBodyState>(text, false);
					this.SetIntel(tispaceBodyState2, 1f, null, false);
				}
				this.updateHabPlanningFlag = true;
				IL_026C:
				this.resourceIncomeDeficiencies = new List<FactionResource>();
				foreach (string text2 in this.template.firstTechNames.Union<string>(this.template.winnerTechNames).ToList<string>())
				{
					if (!string.IsNullOrEmpty(text2) && TemplateManager.Find<TIGenericTechTemplate>(text2, true) == null)
					{
						Log.Error("Bad tech dataName: " + text2 + " " + this.template.dataName, Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x060031CB RID: 12747 RVA: 0x0010AE40 File Offset: 0x00109040
		public override void PostGlobalGameStateCreateInit_2()
		{
			foreach (GoalType goalType in Enums.GoalTypes)
			{
				if (!this.factionGoals.ContainsKey(goalType))
				{
					this.factionGoals.Add(goalType, new List<TIFactionGoalState>());
				}
			}
			this.objectives = new Dictionary<TIObjectiveTemplate, ObjectiveStatus>();
			this.techContributionHistory = new Dictionary<TITechTemplate, float>();
			this.cachedPriorityBonuses = new Dictionary<PriorityType, float>();
			this.cachedTechTooltipStrings = new Dictionary<TIGenericTechTemplate, string>();
			this.ideology = TemplateManager.Find<TIFactionIdeologyTemplate>(this.template.ideologyName, false);
			if (this.lastRecordedLoyalty == null)
			{
				this.lastRecordedLoyalty = new Dictionary<TICouncilorState, int>();
			}
			if (this.lastTimeSecretsWereSeen == null)
			{
				this.lastTimeSecretsWereSeen = new Dictionary<TICouncilorState, TIDateTime>();
			}
			this.fleetGoalTracker = new Dictionary<TISpaceFleetState, FactionGoal_Fleet>();
			if (this.shipRefitDesignNames == null)
			{
				this.shipRefitDesignNames = new List<string>();
			}
			if (this.intelSharingFactions == null)
			{
				this.intelSharingFactions = new List<TIFactionState>();
			}
			if (this.numAtrocitiesByCause == null)
			{
				this.numAtrocitiesByCause = new Dictionary<TIFactionState.AtrocityCause, int>();
			}
			if (this.permaAbandonedNations == null)
			{
				this.permaAbandonedNations = new List<TINationState>();
			}
			if (this.newAvailableCouncilors == null)
			{
				this.newAvailableCouncilors = new List<TICouncilorState>();
			}
			if (this.newAvailableOrgs == null)
			{
				this.newAvailableOrgs = new List<TIOrgState>();
			}
			if (this.shipRefitDesigns != null)
			{
				this.shipRefitDesignNames.AddRange(this.shipRefitDesigns.Select<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.dataName));
				this.shipRefitDesigns = null;
			}
			if (this.habDesigns == null)
			{
				this.habDesigns = new List<TIHabTemplate>();
			}
			else
			{
				this.habDesigns.ForEach(delegate(TIHabTemplate x)
				{
					TemplateManager.Add(x, typeof(TIHabTemplate), true);
				});
			}
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				this.dirtyResourcesTracker.SetResourceDirty(factionResource);
				this.annualResourceIncomes.Add(factionResource, 0f);
			}
			if (this.notificationOverrides == null)
			{
				this.notificationOverrides = new Dictionary<string, TINotificationTemplateOverride>();
			}
			if (this.dailyResourceTransfers == null)
			{
				this.dailyResourceTransfers = new List<DailyResourceTransfer>();
			}
			if (this.factionAssassinations == null)
			{
				this.factionAssassinations = new Dictionary<TIFactionState, int>();
			}
			if (this.currentRiskAversion == 0f)
			{
				this.currentRiskAversion = 0.5f;
			}
			if (!GameControl.control.skirmishMode)
			{
				if (!this.gameStateSubjectCreated)
				{
					GameStateManager.MissionPhase().newCampaignStart = true;
					using (IEnumerator<TIObjectiveTemplate> enumerator = TemplateManager.IterateByClass<TIObjectiveTemplate>(true).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIObjectiveTemplate tiobjectiveTemplate = enumerator.Current;
							if (tiobjectiveTemplate.factions.Contains(this))
							{
								ObjectiveStatus objectiveStatus = (tiobjectiveTemplate.starter ? ObjectiveStatus.Unlocked : ObjectiveStatus.Locked);
								if (TIGlobalValuesState.GlobalValues.tutorialMode || tiobjectiveTemplate.objectiveType != ObjectiveType.Tutorial)
								{
									this.objectives.Add(tiobjectiveTemplate, objectiveStatus);
									this.objectiveNames.Add(tiobjectiveTemplate.dataName, objectiveStatus);
								}
							}
						}
						goto IL_0895;
					}
				}
				this.availableProjects = new List<TIProjectTemplate>();
				this.completedProjects = new List<TIProjectTemplate>();
				foreach (KeyValuePair<string, ObjectiveStatus> keyValuePair in this.objectiveNames)
				{
					TIObjectiveTemplate tiobjectiveTemplate2 = TemplateManager.Find<TIObjectiveTemplate>(keyValuePair.Key, false);
					if (tiobjectiveTemplate2 != null)
					{
						this.objectives.Add(tiobjectiveTemplate2, keyValuePair.Value);
					}
				}
				foreach (string text in this.availableProjectNames)
				{
					TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(text, false);
					if (tiprojectTemplate != null)
					{
						this.availableProjects.Add(tiprojectTemplate);
					}
				}
				foreach (string text2 in this.finishedProjectNames)
				{
					TIProjectTemplate tiprojectTemplate2 = TemplateManager.Find<TIProjectTemplate>(text2, false);
					if (tiprojectTemplate2 != null)
					{
						this.completedProjects.Add(tiprojectTemplate2);
					}
				}
				foreach (TISpaceShipTemplate tispaceShipTemplate in this.shipDesigns)
				{
					tispaceShipTemplate.factionName = this.templateName;
					TemplateManager.Add(tispaceShipTemplate, typeof(TISpaceShipTemplate), true);
				}
				foreach (TIPriorityPresetTemplate tipriorityPresetTemplate in this.customPresets)
				{
					TemplateManager.Add(tipriorityPresetTemplate, typeof(TIPriorityPresetTemplate), true);
				}
				foreach (string text3 in this.techNameContributionHistory.Keys)
				{
					if (TemplateManager.Find<TITechTemplate>(text3, false) != null)
					{
						this.techContributionHistory.Add(TemplateManager.Find<TITechTemplate>(text3, false), this.techNameContributionHistory[text3]);
					}
				}
				using (List<List<TIFactionGoalState>>.Enumerator enumerator7 = this.factionGoals.Values.ToList<List<TIFactionGoalState>>().GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						foreach (TIFactionGoalState tifactionGoalState in enumerator7.Current.OrderByDescending<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance).ToList<TIFactionGoalState>())
						{
							if (tifactionGoalState.isFleetGoal && tifactionGoalState.ref_fleetGoal.assignedFleet != null)
							{
								if (tifactionGoalState.ref_fleetGoal.assignedFleet.faction != this)
								{
									Log.Error(this.displayNameCapitalized + " has goals with assigned fleet in another faction. Deleting: " + tifactionGoalState.ref_fleetGoal.ToString(), Array.Empty<object>());
									this.RemoveGoal(tifactionGoalState);
								}
								else if (!this.fleetGoalTracker.ContainsKey(tifactionGoalState.ref_fleetGoal.assignedFleet))
								{
									this.fleetGoalTracker.Add(tifactionGoalState.ref_fleetGoal.assignedFleet, tifactionGoalState.ref_fleetGoal);
								}
								else
								{
									Log.Error(string.Concat(new string[]
									{
										this.displayNameCapitalized,
										" has duplicate goals with same assigned fleet: ",
										tifactionGoalState.ref_fleetGoal.assignedFleet.ToString(),
										". Keeping: ",
										this.fleetGoalTracker[tifactionGoalState.ref_fleetGoal.assignedFleet].ToString(),
										". Deleting: ",
										tifactionGoalState.ToString()
									}), Array.Empty<object>());
									this.RemoveGoal(tifactionGoalState);
								}
							}
						}
					}
				}
				foreach (TISpaceFleetState tispaceFleetState in this.fleets.ToList<TISpaceFleetState>())
				{
					if (tispaceFleetState.deleted)
					{
						Log.Error("Cleaning up deleted " + this.displayName + " fleet " + tispaceFleetState.ID.ToString(), Array.Empty<object>());
						this.fleets.Remove(tispaceFleetState);
					}
					else if (!this.fleetGoalTracker.ContainsKey(tispaceFleetState))
					{
						this.fleetGoalTracker.Add(tispaceFleetState, null);
					}
				}
				foreach (TISpaceShipTemplate tispaceShipTemplate2 in this.shipDesigns)
				{
					if (tispaceShipTemplate2.utilityModules.Any<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate == null))
					{
						tispaceShipTemplate2.moduleTemplateEntries.RemoveAll((ModuleDataTemplateEntry x) => x.moduleTemplate == null);
						tispaceShipTemplate2.ReCacheUtilityModules();
					}
				}
				foreach (ProjectProgress projectProgress in this.currentProjectProgress.ToList<ProjectProgress>())
				{
					if (projectProgress.projectTemplate == null)
					{
						int slot2 = projectProgress.slot;
						this.currentProjectProgress.Remove(projectProgress);
						if (this.AllowedProjectSlots().Contains(slot2))
						{
							TIPromptQueueState.AddPromptStatic(new Prompt(this, this, null, "PromptSelectProject", slot2));
						}
					}
				}
				using (List<int>.Enumerator enumerator11 = this.AllowedProjectSlots().GetEnumerator())
				{
					while (enumerator11.MoveNext())
					{
						int slot = enumerator11.Current;
						if (this.currentProjectProgress.None<ProjectProgress>((ProjectProgress x) => x.slot == slot))
						{
							TIPromptQueueState.AddPromptStatic(new Prompt(this, this, null, "PromptSelectProject", slot));
						}
					}
				}
				IL_0895:
				this.defaultPriorityPreset = TemplateManager.Find<TIPriorityPresetTemplate>(this.defaultPriorityPresetTemplateName, false);
			}
			if (this.factionFleetsEncountered == null)
			{
				this.factionFleetsEncountered = new Dictionary<TIFactionState, int>();
			}
			if (this.factionEarlyToDoList == null)
			{
				this.factionEarlyToDoList = new List<AITaskCategory>();
			}
			if (this.factionLateToDoList == null)
			{
				this.factionLateToDoList = new List<AITaskCategory>();
			}
			if (this.AISavingTarget == null)
			{
				this.AISavingTarget = new AISavingData(this, null, null, null, 0f);
				this.AISavingTarget.ClearPurchaseData();
			}
			if (this.AISavingTarget != null && this.AISavingTarget.active && (this.AISavingTarget.bankedResources == null || this.AISavingTarget.desiredPurchase == null || this.AISavingTarget.location == null || this.AISavingTarget.location.deleted))
			{
				this.AIClearSavingTarget("AI Saving target cleanup");
			}
			if (this.shipsBuiltInClass == null)
			{
				this.shipsBuiltInClass = new Dictionary<string, int>();
			}
			foreach (ProjectProgress projectProgress2 in this.currentProjectProgress)
			{
				if (projectProgress2.accumulatedResearch < 0f)
				{
					projectProgress2.accumulatedResearch = 0f;
				}
			}
			if (this.lastTechRaceDate == null)
			{
				this.lastTechRaceDate = TITimeState.Now();
			}
			if (this.LastObjectiveProjectCompletionDate == null)
			{
				this.LastObjectiveProjectCompletionDate = TITimeState.Now();
			}
			if (!this.boostAccounts.ContainsKey(TIFactionState.BoostAccountName.Base))
			{
				this.boostAccounts[TIFactionState.BoostAccountName.Base] = null;
			}
			if (!this.boostAccounts.ContainsKey(TIFactionState.BoostAccountName.Station))
			{
				this.boostAccounts[TIFactionState.BoostAccountName.Station] = null;
			}
			if (!this.boostAccounts.ContainsKey(TIFactionState.BoostAccountName.Probe))
			{
				this.boostAccounts[TIFactionState.BoostAccountName.Probe] = null;
			}
			if (!this.boostAccounts.ContainsKey(TIFactionState.BoostAccountName.Org))
			{
				this.boostAccounts[TIFactionState.BoostAccountName.Org] = null;
			}
			foreach (TIGameState tigameState in this.intel.Keys.ToList<TIGameState>())
			{
				if (tigameState.deleted)
				{
					this.ExpireIntel(tigameState, true);
				}
				else if (!this.highestIntel.ContainsKey(tigameState))
				{
					Log.Error("Save Repair: Save missing HighestIntel key for " + tigameState.displayName, Array.Empty<object>());
					this.highestIntel.Add(tigameState, this.intel[tigameState]);
				}
			}
			foreach (TIOrgState tiorgState in this.unassignedOrgs.ToList<TIOrgState>())
			{
				if (!TIGameState.Valid(tiorgState))
				{
					this.unassignedOrgs.Remove(tiorgState);
					Log.Debug(this.displayName + " had invalid org in unassigned orgs. This would cause a crash. Deleting.", Array.Empty<object>());
				}
			}
			if (this.hiddenProjects == null)
			{
				this.hiddenProjects = new List<string>();
			}
			if (this.favoredProjects == null)
			{
				this.favoredProjects = new List<string>();
			}
			if (this.obsoletedShipParts == null)
			{
				this.obsoletedShipParts = new List<string>();
			}
			if (this.missedProjects == null)
			{
				this.missedProjects = new List<string>();
			}
			if (this.sabotagedProjects == null)
			{
				this.sabotagedProjects = new List<string>();
			}
			if (this.ignoreContacts == null)
			{
				this.ignoreContacts = new List<TIFactionState>();
			}
			if (this.ignoreInterstateDiplomacy == null)
			{
				this.ignoreInterstateDiplomacy = new List<TIFactionState>();
			}
			if (this.alarms == null)
			{
				this.alarms = new List<Alarm>();
			}
			foreach (TISectorState tisectorState in this.habSectors.ToList<TISectorState>())
			{
				if (tisectorState.faction != this)
				{
					this.habSectors.Remove(tisectorState);
				}
			}
			foreach (KeyValuePair<string, List<TIFactionState.Transaction>> keyValuePair2 in this.Transactions.ToList<KeyValuePair<string, List<TIFactionState.Transaction>>>())
			{
				if (keyValuePair2.Key.Contains('\n'))
				{
					string text4 = JsonConvert.ToString(keyValuePair2.Key).Trim(new char[] { '"' });
					this.Transactions.Remove(keyValuePair2.Key);
					this.Transactions.Add(text4, keyValuePair2.Value);
				}
			}
		}

		// Token: 0x060031CC RID: 12748 RVA: 0x0010BCD4 File Offset: 0x00109ED4
		public override void PostCanvasManagerCreateInit_3()
		{
			this.UpdateAllowedShipParts(null);
			if (!this.gameStateSubjectCreated)
			{
				this.factionOperationCompleteName = new StringBuilder("FactionOperationComplete").Append(base.ID.ToString()).ToString();
				this.assessedAlienHateOfMe = GameStateManager.AlienFaction().MinimumFactionHate(this);
				this.updateShipDesignsFlag = this.allowedShipHulls.Count<TIShipHullTemplate>() > 0;
			}
			else
			{
				if (this.nShipyardQueues == null)
				{
					this.nShipyardQueues = new Dictionary<TIHabModuleState, List<ShipConstructionQueueItem>>();
				}
				if (this.nShipyardQueues.Count == 0)
				{
					List<TIHabModuleState> list = (from x in this.habSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.CompletedModules())
						where x.moduleTemplate.allowsShipConstruction && !x.decommissioning
						select x).ToList<TIHabModuleState>();
					if (list.Count <= 0)
					{
						goto IL_01A9;
					}
					using (List<TIHabModuleState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIHabModuleState tihabModuleState = enumerator.Current;
							this.AddShipyardToFaction(tihabModuleState, false);
						}
						goto IL_01A9;
					}
				}
				foreach (TIHabModuleState tihabModuleState2 in this.nShipyardQueues.Keys.ToList<TIHabModuleState>())
				{
					if (tihabModuleState2.hab == null || tihabModuleState2.hab.deleted || tihabModuleState2.sector == null || tihabModuleState2.sector.deleted || tihabModuleState2.moduleTemplate == null)
					{
						this.RemoveShipyardFromFaction(tihabModuleState2, false);
					}
				}
				IL_01A9:
				foreach (List<ShipConstructionQueueItem> list2 in this.nShipyardQueues.Values.ToList<List<ShipConstructionQueueItem>>())
				{
					for (int i = list2.Count - 1; i >= 0; i--)
					{
						if (list2[i].shipDesign == null)
						{
							global::UnityEngine.Debug.LogWarning("Save Repair: removing null ship template from shipyard " + list2[i].shipyard.ID.ToString());
							this.RemoveShipFromShipyardQueue(list2[i].shipyard, list2[i]);
						}
						else
						{
							if (list2[i].isRefit && list2[i].originalShipDesign != null)
							{
								list2[i].refit_originalShipDesignTemplateName = list2[i].originalShipDesign.dataName;
								list2[i].originalShipDesign = null;
							}
							if (list2[i].isRefit && list2[i].originalSpaceShipState.deleted)
							{
								global::UnityEngine.Debug.LogWarning("Save Repair: removing null shipstate from shipyard " + list2[i].shipyard.ID.ToString());
								this.RemoveShipFromShipyardQueue(list2[i].shipyard, list2[i]);
							}
							else if (!list2[i].costPaid && list2[i].shipDesign.Obsolete(this))
							{
								global::UnityEngine.Debug.LogWarning("Save Repair: removing unpaid obsolete ship from shipyard " + list2[i].shipyard.ID.ToString());
								this.RemoveShipFromShipyardQueue(list2[i].shipyard, list2[i]);
							}
						}
					}
				}
				if (this.assessedAlienHateOfMe == -1f)
				{
					this.assessedAlienHateOfMe = GameStateManager.AlienFaction().GetFactionHate(this);
				}
			}
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnTimedOperationComplete), this.factionOperationCompleteName, null, true, false);
			if (this.IsAlienFaction && this.specialRegionAdjacencies == null)
			{
				this.specialRegionAdjacencies = new List<SpecialRegionAdjacencies>();
				foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
				{
					if (tibilateralTemplate.relationType == BilateralRelationType.PhysicalAdjacency && !string.IsNullOrEmpty(tibilateralTemplate.projectUnlockName) && tibilateralTemplate.BilateralIsInScenario())
					{
						this.specialRegionAdjacencies.Add(new SpecialRegionAdjacencies
						{
							region1 = tibilateralTemplate.regionState1,
							region2 = tibilateralTemplate.regionState2
						});
					}
				}
			}
			if (this.isActivePlayer)
			{
				this.alertSpaceTimerNotifications = TIPlayerProfileManager.alertSpaceTimerNotifications;
				this.showMonthlyIncomesInTopBarAndIntel = TIPlayerProfileManager.showMonthlyIncomes;
			}
			this.SetHe3Access();
			this.fullSpaceVisibility = this.FullSystemVisibility;
			if (this.history_CPCapOverageByDay == null)
			{
				float oneDayControlPointCapMissionPenalty = this.GetOneDayControlPointCapMissionPenalty();
				this.history_CPCapOverageByDay = new List<float>();
				this.history_CPCapOverageByDay.AddRange(Enumerable.Repeat<float>(oneDayControlPointCapMissionPenalty, 32));
			}
			if (this.history_MCCapOverageByDay == null)
			{
				int missionControlShortage = this.MissionControlShortage;
				this.history_MCCapOverageByDay = new List<int>();
				this.history_MCCapOverageByDay.AddRange(Enumerable.Repeat<int>(missionControlShortage, 32));
			}
		}

		// Token: 0x060031CD RID: 12749 RVA: 0x0010C254 File Offset: 0x0010A454
		public override void PostAllStartUpInit_5()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (!this.IsAlienFaction)
			{
				using (List<TIControlPoint>.Enumerator enumerator = this.controlPoints.ToList<TIControlPoint>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIControlPoint ticontrolPoint = enumerator.Current;
						bool? flag;
						if (ticontrolPoint == null)
						{
							flag = null;
						}
						else
						{
							TINationState nation = ticontrolPoint.nation;
							flag = ((nation != null) ? new bool?(!nation.extant) : null);
						}
						if (flag ?? true)
						{
							if (ticontrolPoint.deleted)
							{
								Log.Debug(this.displayName + " had control point in non-extant nation. Repairing.", Array.Empty<object>());
								this.controlPoints.Remove(ticontrolPoint);
							}
							else
							{
								Log.Debug(this.displayName + " had control point in non-extant nation " + ticontrolPoint.nation.displayName + ". Repairing.", Array.Empty<object>());
								ticontrolPoint.SetFaction(null, false);
								this.controlPoints.Remove(ticontrolPoint);
							}
						}
					}
					goto IL_0257;
				}
			}
			bool flag2 = false;
			List<TISpaceShipTemplate> list = (from x in this.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template).Distinct<TISpaceShipTemplate>()
				where x.UnnormalizedTemplateSpaceCombatValue(false, 1f) > 0f
				select x).Take_Random<TISpaceShipTemplate>(5).ToList<TISpaceShipTemplate>();
			if (list.Count == 5)
			{
				float num = list.Average<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.UnnormalizedTemplateSpaceCombatValue(false, 1f));
				float num2 = list.Average<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.UnnormalizedTemplateSpaceCombatValue(true, 1f));
				if (Mathf.Abs(num - num2) / num > 0.3f)
				{
					flag2 = true;
				}
			}
			else
			{
				flag2 = true;
			}
			if (flag2)
			{
				foreach (TISpaceShipTemplate tispaceShipTemplate in TemplateManager.IterateByClass<TISpaceShipTemplate>(true))
				{
					tispaceShipTemplate.UnnormalizedTemplateSpaceCombatValue(true, 1f);
				}
				foreach (TISpaceShipState tispaceShipState in GameStateManager.IterateByClass<TISpaceShipState>(false))
				{
					tispaceShipState.spaceCombatValueDataDirty = true;
				}
			}
			IL_0257:
			if (!this.gameStateSubjectCreated && this.checkNotificationOverrides)
			{
				this.notificationOverrides = new Dictionary<string, TINotificationTemplateOverride>();
				for (int i = 0; i < TIPlayerProfileManager.notificationTemplates.Count; i++)
				{
					this.notificationOverrides.Add(TIPlayerProfileManager.notificationTemplates[i].dataName, TIPlayerProfileManager.GetNotificationOverride(TIPlayerProfileManager.notificationTemplates[i].dataName));
				}
			}
		}

		// Token: 0x060031CE RID: 12750 RVA: 0x0010C548 File Offset: 0x0010A748
		public override void PostVisualizerCreationInit_6()
		{
			if (!this.gameStateSubjectCreated && this.IsAlienFaction)
			{
				this.CacheSTOFighterMass();
				foreach (TIRegionUFOCrashdownState tiregionUFOCrashdownState in GameStateManager.IterateByClass<TIRegionUFOCrashdownState>(false))
				{
					if (tiregionUFOCrashdownState.crashdownPresent)
					{
						tiregionUFOCrashdownState.TriggerCrashdown(true);
						break;
					}
				}
			}
			if (!this.gameStateSubjectCreated && TIGlobalValuesState.GlobalValues.tutorialMode && GameControl.control.activePlayer == this)
			{
				TINotificationQueueState.LogTutorialStart(this);
			}
		}

		// Token: 0x060031CF RID: 12751 RVA: 0x0010C5E4 File Offset: 0x0010A7E4
		public override void PostVisualizerCreationInit_7()
		{
			if (!this.gameStateSubjectCreated)
			{
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					this.GainFactionHate(tifactionState, 0f, false, "Initialization", true);
				}
				TIRegionUFOCrashdownState tiregionUFOCrashdownState = null;
				foreach (TIRegionUFOCrashdownState tiregionUFOCrashdownState2 in GameStateManager.IterateByClass<TIRegionUFOCrashdownState>(false))
				{
					if (tiregionUFOCrashdownState2.crashdownPresent)
					{
						tiregionUFOCrashdownState = tiregionUFOCrashdownState2;
						break;
					}
				}
				if (!GameControl.control.skirmishMode)
				{
					if (tiregionUFOCrashdownState != null)
					{
						TINotificationQueueState.LogCampaignStart(this, tiregionUFOCrashdownState.region);
					}
					foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByStatus(ObjectiveStatus.Unlocked))
					{
						if (tiobjectiveTemplate.objectiveType != ObjectiveType.General && !tiobjectiveTemplate.isChildObjective)
						{
							TINotificationQueueState.LogObjectiveUnlocked(this, tiobjectiveTemplate);
						}
					}
					if (this.IsActiveHumanFaction)
					{
						TIUtilities.InitRandom(478154);
						this.GenerateOrgsForAcquisition(true);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusMoney, FactionResource.Money, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusInfluence, FactionResource.Influence, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusOps, FactionResource.Operations, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusBoost, FactionResource.Boost, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusWater, FactionResource.Water, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusVolatiles, FactionResource.Volatiles, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusMetals, FactionResource.Metals, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusNobles, FactionResource.NobleMetals, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusFissiles, FactionResource.Fissiles, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusAntimatter, FactionResource.Antimatter, false, null);
						this.AddToCurrentResource(GameStateManager.Time().template.bonusExotics, FactionResource.Exotics, false, null);
						foreach (TIProjectTemplate tiprojectTemplate in TemplateManager.IterateByClass<TIProjectTemplate>(true))
						{
							if (tiprojectTemplate.FactionPrereqsSatisfied(this) && tiprojectTemplate.GetResearchCost(this) <= 0f)
							{
								this.OnProjectComplete(tiprojectTemplate, -1, true, false);
							}
						}
						foreach (string text in GameStateManager.Time().template.projectsCompleted.Where<string>((string x) => !string.IsNullOrEmpty(x)))
						{
							TIProjectTemplate tiprojectTemplate2 = TemplateManager.Find<TIProjectTemplate>(text, false);
							if (tiprojectTemplate2.FactionPrereqsSatisfied(this))
							{
								this.OnProjectComplete(tiprojectTemplate2, -1, true, false);
							}
						}
						if (!this.IsAlienFaction)
						{
							foreach (TIProjectTemplate tiprojectTemplate3 in TemplateManager.IterateByClass<TIProjectTemplate>(true))
							{
								List<string> prereqs = tiprojectTemplate3.prereqs;
								if (prereqs != null && prereqs.Count == 0 && !this.ProjectAlreadyTriggered(tiprojectTemplate3) && tiprojectTemplate3.FactionPrereqsSatisfied(this))
								{
									this.RollToAddProjectTrigger(tiprojectTemplate3, null);
								}
							}
						}
					}
					if (this.player.isAI && this.updateShipDesignsFlag)
					{
						AIDailyFactionPlanner.DesignShips(this, null);
					}
				}
			}
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.shipDesigns)
			{
				tispaceShipTemplate.CacheTemplateValues(false);
			}
			this.unassignedOrgs = this.unassignedOrgs.Distinct<TIOrgState>().ToList<TIOrgState>();
			this.CheckForOrgProjectStatusChange();
			this.CheckforHabProjectUnlock();
			this.CachePriorityBonuses_Day();
			this.CheckForMissedProjectProject();
			this.gameStateSubjectCreated = true;
			if (!GameControl.control.skirmishMode && !this.IsAlienFaction)
			{
				string text2 = this.displayName + " Missing Projects: ";
				foreach (TIProjectTemplate tiprojectTemplate4 in TIGlobalResearchState.GetAllProjects())
				{
					if (!this.ProjectAlreadyTriggered(tiprojectTemplate4) && tiprojectTemplate4.factionAvailableChance >= 100f && tiprojectTemplate4.PrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), this.completedProjects, this))
					{
						text2 = text2 + tiprojectTemplate4.dataName + "\n";
						this.AddAvailableProject(tiprojectTemplate4, null);
					}
				}
				if (text2.Contains("\n"))
				{
					Log.Error(text2, Array.Empty<object>());
				}
				foreach (KeyValuePair<TIObjectiveTemplate, ObjectiveStatus> keyValuePair in this.objectives.Where<KeyValuePair<TIObjectiveTemplate, ObjectiveStatus>>((KeyValuePair<TIObjectiveTemplate, ObjectiveStatus> x) => x.Value == ObjectiveStatus.Unlocked).ToList<KeyValuePair<TIObjectiveTemplate, ObjectiveStatus>>())
				{
					if (this.completedProjectsDistinct.Contains(keyValuePair.Key.targetProjectTemplate))
					{
						this.CheckForObjectivesCompleteViaProject(keyValuePair.Key.targetProjectTemplate);
						Log.Error(string.Concat(new string[]
						{
							"REPAIR: Objective ",
							keyValuePair.Key.displayName(this),
							" not completed despite project ",
							keyValuePair.Key.targetProjectTemplate.displayName,
							" completed by ",
							this.displayName
						}), Array.Empty<object>());
					}
				}
				if (this.isActivePlayer)
				{
					Log.Time("<color=#00cc00>LoadTime:</color> CacheAllTechTooltipStrings", delegate
					{
						this.CacheAllTechTooltipStrings();
					}, true, true);
				}
				if (!this.isActivePlayer)
				{
					HumanHabPlanner.ManageMineNetwork(this);
				}
			}
			if (!GameControl.control.skirmishMode)
			{
				foreach (TIHabModuleState tihabModuleState in this.nShipyardQueues.Keys.ToList<TIHabModuleState>())
				{
					if (tihabModuleState.active && this.nShipyardQueues[tihabModuleState].Count > 0 && this.nShipyardQueues[tihabModuleState][0].costPaid && this.nShipyardQueues[tihabModuleState][0].daysToCompletion > 0f)
					{
						GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, tihabModuleState, this.nShipyardQueues[tihabModuleState][0]), null, new object[] { this, tihabModuleState });
					}
				}
			}
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x0010CCF0 File Offset: 0x0010AEF0
		public void SetAlarm(TIGameState targetGameState, TIDataTemplate targetTemplate, TIDateTime triggerTime, AlarmType alarm, string customString = "")
		{
			switch (alarm)
			{
			case AlarmType.None:
				return;
			case AlarmType.FleetApproaching:
				targetGameState.ref_fleet.alwaysShowOrbitTrailDuringTransfer = true;
				break;
			case AlarmType.PlayerAlarm:
				break;
			default:
				return;
			}
			TITimeEvent titimeEvent = TITimeEvent.CreateNewTimeEvent(triggerTime, this, targetGameState, targetTemplate, "PlayerAlarm", true, false, TITimeQueueRepeatType.None, 1, true, false);
			this.alarms.Add(new Alarm
			{
				alarmType = alarm,
				time = triggerTime,
				customPlayerString = customString,
				alarmEvent = titimeEvent,
				associatedGameState = targetGameState
			});
			GameControl.eventManager.TriggerEvent(new AlarmAdded(this, triggerTime), null, new object[] { this });
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x0010CD90 File Offset: 0x0010AF90
		public void NewCampaign()
		{
			if (!this.gameStateSubjectCreated)
			{
				bool flag = GameControl.control.skirmishMode || TemplateManager.global.debug_advancedFactionStart;
				List<string> list = (from y in GameStateManager.IterateByClass<TISpaceFleetState>(false).SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
					select y.templateName).Distinct<string>().ToList<string>();
				foreach (TISpaceShipTemplate tispaceShipTemplate in TemplateManager.IterateByClass<TISpaceShipTemplate>(true))
				{
					if (tispaceShipTemplate.designingFaction == this)
					{
						if (!flag)
						{
							List<string> startingShipDesigns = GameStateManager.Time().template.startingShipDesigns;
							if ((startingShipDesigns == null || !startingShipDesigns.Contains(tispaceShipTemplate.dataName)) && !list.Contains(tispaceShipTemplate.dataName))
							{
								continue;
							}
						}
						tispaceShipTemplate.SetClassDisplayName(false);
						this.shipDesigns.Add(tispaceShipTemplate);
					}
				}
				if (this.IsActiveHumanFaction)
				{
					this.RecruitInitialCouncilors();
					if (!GameControl.control.skirmishMode)
					{
						this.GenerateRecruitableCouncilors(true);
					}
					if (this.player.isAI)
					{
						Dictionary<TICouncilorState, Dictionary<FactionResource, float>> dictionary = this.councilors.ToDictionary<TICouncilorState, TICouncilorState, Dictionary<FactionResource, float>>((TICouncilorState x) => x, (TICouncilorState y) => TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource z) => z, (FactionResource z) => y.GetMonthlyIncome(z)));
						AIDailyFactionPlanner.RecruitCouncilors(this, new List<TIMissionTemplate>(), new List<TIMissionTemplate>(), ref dictionary, false, 0, true);
					}
				}
				else
				{
					Dictionary<TIHabSiteState, float> bestNoblesOptions = (from x in (from x in GameStateManager.KuiperBeltObjects(false)
							where x.semiMajorAxis_AU > 38.0 && x.apoapsis_AU < 55.0 && x.objectType == SpaceObjectType.DwarfPlanet
							select x).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites)
						where x.water_day > 0f && x.volatiles_day > 0f && x.metals_day > 0f && x.fissiles_day > 0f
						select x).ToDictionary<TIHabSiteState, TIHabSiteState, float>((TIHabSiteState x) => x, (TIHabSiteState y) => y.GetDailyProduction(FactionResource.NobleMetals));
					float maxNobles = bestNoblesOptions.Max<KeyValuePair<TIHabSiteState, float>>((KeyValuePair<TIHabSiteState, float> x) => x.Value);
					TIHabSiteState tihabSiteState = bestNoblesOptions.Keys.Where<TIHabSiteState>((TIHabSiteState x) => bestNoblesOptions[x] >= maxNobles * 0.75f).MaxBy<TIHabSiteState, float>((TIHabSiteState x) => AIEvaluators.EvaluateHabSite(this, x, false, false, true));
					this.primaryHab = GameStateManager.FindByTemplate<TIHabState>("AlienHQ", false);
					this.primaryHab.habSite = tihabSiteState;
					this.primaryHab.habSite.hab = this.primaryHab;
					this.primaryHab.barycenter = tihabSiteState.parentBody;
					TIHabState tihabState = GameStateManager.FindByTemplate<TIHabState>("AlienHQStation", false);
					if (tihabState != null)
					{
						TIOrbitState tiorbitState;
						if (this.primaryHab.barycenter.ref_spaceBody.interfaceOrbits.Count > 0)
						{
							tiorbitState = this.primaryHab.barycenter.ref_spaceBody.interfaceOrbits[0];
						}
						else
						{
							tiorbitState = GameStateManager.FindByTemplate<TIOrbitState>("LowNeptuneOrbit", false);
						}
						tihabState.SetRandomizedOrbitFromState(tiorbitState, true);
					}
					List<TIRegionState> list2 = new List<TIRegionState>();
					TIRegionState tiregionState = GameStateManager.Time().template.InitialCrashdownRegion() ?? AIEvaluators.SelectAlienCrashdownRegion(true, false);
					list2.Add(tiregionState);
					this.SetIntialPlanetaryConquestGoals(tihabSiteState);
					tiregionState.alienCrashdown.SetAsInitialCrashdownRegion();
					int aliensPreferredCouncilorCount = AIEvaluators.GetAliensPreferredCouncilorCount();
					for (int i = 0; i < aliensPreferredCouncilorCount; i++)
					{
						TICouncilorState ticouncilorState = GameStateManager.CreateNewGameState<TICouncilorState>();
						ticouncilorState.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>("randomizedAlienCouncilor1", false));
						ticouncilorState.NewCharacterGeneration(null, null, this, false, true);
						ticouncilorState.location.ref_hab.DepartCouncilor(ticouncilorState);
						ticouncilorState.SetFaction(this);
						ticouncilorState.SetRecruitDate();
						this.councilors.Add(ticouncilorState);
						TISpaceShipState tispaceShipState = null;
						if (i != 0)
						{
							if (i - 1 > 4)
							{
								ticouncilorState.SetLocation(this.primaryHab);
							}
							else
							{
								if (GameStateManager.Time().template.startingAlienCouncilorFleets.Where<string>((string x) => !string.IsNullOrEmpty(x)).Count<string>() >= i)
								{
									TISpaceFleetState tispaceFleetState = GameStateManager.FindByTemplate<TISpaceFleetState>(GameStateManager.Time().template.startingAlienCouncilorFleets[i - 1], false);
									if (tispaceFleetState != null && tispaceFleetState.ships.Count > 0)
									{
										if (tispaceFleetState.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.HasSpecialModuleRule(SpecialModuleRule.Crashdown, false)))
										{
											tispaceShipState = tispaceFleetState.ships.First<TISpaceShipState>((TISpaceShipState x) => x.HasSpecialModuleRule(SpecialModuleRule.Crashdown, false));
										}
									}
								}
								if (tispaceShipState != null)
								{
									ticouncilorState.SetLocation(tispaceShipState);
									TIRegionState tiregionState2 = AIEvaluators.SelectAlienCrashdownRegion(true, false);
									this.AddGoal(new FactionGoal_TransportCouncilorsWithFleet(this, 15, new List<TICouncilorState> { ticouncilorState }, tiregionState2), HandleDuplicateGoalRule.Ignore, tispaceShipState.ref_fleet);
									list2.Add(tiregionState2);
								}
								else
								{
									ticouncilorState.SetLocation(this.primaryHab);
									FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = new FactionGoal_TransportCouncilorsWithFleet(this, 17, new List<TICouncilorState> { ticouncilorState }, AIEvaluators.SelectAlienCrashdownRegion(true, false));
									this.AddGoal(factionGoal_TransportCouncilorsWithFleet, HandleDuplicateGoalRule.Ignore, null);
								}
							}
						}
						else
						{
							ticouncilorState.SetLocation(tiregionState);
						}
						if (i == 5)
						{
							this.GrantNewOrgToCouncilor(ticouncilorState, TemplateManager.global.alienShockTroopOrgDataName);
						}
						if (ticouncilorState.location == null)
						{
							ticouncilorState.SetLocation(this.primaryHab);
						}
						this.SetIntel(ticouncilorState, TemplateManager.global.intelToSeeCouncilorSecrets, null, false);
					}
					foreach (TISpaceFleetState tispaceFleetState2 in this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.HasSpecialModuleCapability(SpecialModuleRule.LandArmy)))
					{
						this.AddGoal(new FactionGoal_InvadeEarth(this, 19), HandleDuplicateGoalRule.ResetImportanceIfHigher, tispaceFleetState2);
					}
					if (!this.GoalsOfType(GoalType.InvadeEarth, false, true).Any<TIFactionGoalState>() && (TIGlobalValuesState.IsQuietAlienCampaign() || TIGlobalValuesState.IsInvasionFocusedAlienCampaign()))
					{
						this.AddGoal(new FactionGoal_InvadeEarth(this, 19), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					}
					foreach (TIHabState tihabState2 in this.habs)
					{
						if (tihabState2.IsBase)
						{
							this.AddGoal(new FactionGoal_BuildFullBase(this, 19, tihabState2), HandleDuplicateGoalRule.Ignore, null);
						}
						else
						{
							this.AddGoal(new FactionGoal_BuildFullStation(this, 18, tihabState2), HandleDuplicateGoalRule.Ignore, null);
						}
					}
				}
				foreach (TISpaceShipState tispaceShipState2 in this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships))
				{
					if (tispaceShipState2.template.designingFaction == this && !this.shipDesigns.Contains(tispaceShipState2.template))
					{
						this.SaveShipDesign(tispaceShipState2.template);
					}
				}
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x060031D2 RID: 12754 RVA: 0x0010D53C File Offset: 0x0010B73C
		// (set) Token: 0x060031D3 RID: 12755 RVA: 0x0010D544 File Offset: 0x0010B744
		[fsIgnore]
		public GameObject factionObject { get; private set; }

		// Token: 0x060031D4 RID: 12756 RVA: 0x0010D54D File Offset: 0x0010B74D
		public void CreateVisualizer(TIDataTemplate myTemplate)
		{
			this.factionObject = ECSUtils.CreateEntity("Faction", this.templateName);
			this.factionObject.GetComponent<FactionStateComponent>().State = this;
		}

		// Token: 0x060031D5 RID: 12757 RVA: 0x0010D578 File Offset: 0x0010B778
		public TICouncilorTypeTemplate GetRandomJobWithMissions(List<string> missionNames, List<TICouncilorTypeTemplate> takenJobs = null)
		{
			Dictionary<TICouncilorTypeTemplate, float> dictionary = new Dictionary<TICouncilorTypeTemplate, float>();
			missionNames = missionNames.Where<string>((string x) => TemplateManager.Find<TIMissionTemplate>(x, false) != null).ToList<string>();
			foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate in TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true))
			{
				if (ticouncilorTypeTemplate.unlocked && ticouncilorTypeTemplate.missionNames.Intersect<string>(missionNames).Count<string>() == missionNames.Count)
				{
					dictionary.Add(ticouncilorTypeTemplate, ticouncilorTypeTemplate.weight);
				}
			}
			Dictionary<TICouncilorTypeTemplate, float> dictionary2 = new Dictionary<TICouncilorTypeTemplate, float>(dictionary);
			if (takenJobs != null && takenJobs.Count > 0)
			{
				foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate2 in takenJobs)
				{
					dictionary.Remove(ticouncilorTypeTemplate2);
				}
			}
			if (TemplateManager.global.diff_initialCouncilorsFavoredStat)
			{
				foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate3 in dictionary.Keys.ToList<TICouncilorTypeTemplate>())
				{
					if (!ticouncilorTypeTemplate3.keyStat.Contains(TemplateManager.Find<TIMissionTemplate>(missionNames[0], false).primaryAttackerStat))
					{
						dictionary.Remove(ticouncilorTypeTemplate3);
					}
				}
			}
			if (dictionary.Count < 1)
			{
				dictionary = dictionary2;
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<TICouncilorTypeTemplate, float>>((KeyValuePair<TICouncilorTypeTemplate, float> j) => j.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x0010D72C File Offset: 0x0010B92C
		public void RecruitInitialCouncilors()
		{
			int num = (GameControl.control.skirmishMode ? 0 : (2 - this.councilors.Count));
			List<TINationState> list = new List<TINationState>();
			List<TICouncilorTypeTemplate> list2 = new List<TICouncilorTypeTemplate>();
			for (int i = 0; i < TIGlobalValuesState.GlobalValues.scenarioCustomizations.skipStartingCouncilors.Count; i++)
			{
				if (TIGlobalValuesState.GlobalValues.scenarioCustomizations.skipStartingCouncilors[i])
				{
					this.AddToCurrentResource((float)TemplateManager.global.skipCouncilorInfluenceBonus, FactionResource.Influence, false, null);
					num--;
				}
			}
			if (num > 0)
			{
				for (int j = 0; j < num; j++)
				{
					TICouncilorState ticouncilorState = GameStateManager.CreateNewGameState<TICouncilorState>();
					ticouncilorState.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>("randomizedCouncilor1", false));
					TICouncilorTypeTemplate ticouncilorTypeTemplate = null;
					TIRegionState tiregionState = null;
					if (j == 0)
					{
						if (TIGlobalValuesState.usingCustomizations && TIGlobalValuesState.GlobalValues.scenarioCustomizations.startingCouncilorProfessions != null && TIGlobalValuesState.GlobalValues.scenarioCustomizations.startingCouncilorProfessions.Count > j)
						{
							if (GameControl.control.activePlayer == this)
							{
								ticouncilorTypeTemplate = TIGlobalValuesState.GlobalValues.scenarioCustomizations.startingCouncilorProfessions[j];
							}
							else
							{
								this.AddToCurrentResource(TemplateManager.global.AI_BonusInfluenceOnPlayerCouncilorSelect(), FactionResource.Influence, false, null);
								ticouncilorTypeTemplate = this.GetRandomJobWithMissions(this.template.guaranteedMissions[0], list2);
							}
						}
						else
						{
							ticouncilorTypeTemplate = this.GetRandomJobWithMissions(this.template.guaranteedMissions[0], list2);
						}
						list2.Add(ticouncilorTypeTemplate);
						if (GameControl.control.activePlayer == this && TIGlobalValuesState.GlobalValues.scenarioCustomizations.usePlayerCountryForStartingCouncilor)
						{
							TINationState tinationState;
							string playerNationName;
							if (string.IsNullOrEmpty(TIGlobalConfig.globalConfig.homeCountryThreeLetterISOCodeOverride))
							{
								playerNationName = Utilities.PlayerCountryCode();
								tinationState = GameStateManager.AllExtantHumanNations().FirstOrDefault<TINationState>((TINationState x) => x.template.ISOCodes.Contains(playerNationName));
							}
							else
							{
								playerNationName = TIGlobalConfig.globalConfig.homeCountryThreeLetterISOCodeOverride;
								tinationState = GameStateManager.AllExtantHumanNations().FirstOrDefault<TINationState>((TINationState x) => x.template.ISOCodes.Contains(playerNationName));
								if (tinationState == null)
								{
									playerNationName = Utilities.PlayerCountryCode();
									tinationState = GameStateManager.AllExtantHumanNations().FirstOrDefault<TINationState>((TINationState x) => x.template.ISOCodes.Contains(playerNationName));
								}
							}
							tinationState = GameStateManager.AllExtantHumanNations().FirstOrDefault<TINationState>((TINationState x) => x.template.ISOCodes.Contains(playerNationName));
							if (tinationState != null)
							{
								TINationState tinationState2 = GameStateManager.FindByTemplate<TINationState>(tinationState.templateName, false);
								if (tinationState2 != null)
								{
									if (tinationState2.extant)
									{
										tiregionState = tinationState2.RandomRegionWeightedByPopulation();
									}
									else
									{
										tiregionState = tinationState2.capital;
									}
								}
							}
						}
					}
					if (j == 1)
					{
						if (TIGlobalValuesState.usingCustomizations && TIGlobalValuesState.GlobalValues.scenarioCustomizations.startingCouncilorProfessions != null && TIGlobalValuesState.GlobalValues.scenarioCustomizations.startingCouncilorProfessions.Count > j)
						{
							if (GameControl.control.activePlayer == this)
							{
								ticouncilorTypeTemplate = TIGlobalValuesState.GlobalValues.scenarioCustomizations.startingCouncilorProfessions[j];
							}
							else
							{
								this.AddToCurrentResource(TemplateManager.global.AI_BonusInfluenceOnPlayerCouncilorSelect(), FactionResource.Influence, false, null);
								ticouncilorTypeTemplate = this.GetRandomJobWithMissions(this.template.guaranteedMissions[1], list2);
							}
						}
						else
						{
							ticouncilorTypeTemplate = this.GetRandomJobWithMissions(this.template.guaranteedMissions[1], list2);
						}
					}
					ticouncilorState.NewCharacterGeneration(ticouncilorTypeTemplate, tiregionState, this, GameControl.control.activePlayer == this || TIGlobalValuesState.GlobalValues.difficulty >= 2, true);
					if (j > 0)
					{
						bool flag = false;
						int num2 = 0;
						while (!flag)
						{
							if (list.Contains(ticouncilorState.homeNation))
							{
								ticouncilorState.NewCharacterGeneration(ticouncilorTypeTemplate, tiregionState, this, false, false);
								num2++;
							}
							else
							{
								flag = true;
							}
							if (num2 > 100)
							{
								flag = true;
							}
						}
					}
					list.Add(ticouncilorState.homeNation);
					this.councilors.Add(ticouncilorState);
					ticouncilorState.SetFaction(this);
					ticouncilorState.SetRecruitDate();
					ticouncilorState.SelectVoice();
					List<TITraitTemplate> list3 = new List<TITraitTemplate>();
					foreach (TITraitTemplate titraitTemplate in ticouncilorState.traits)
					{
						if (titraitTemplate.restrictedLocations != RestrictedLocations.None && titraitTemplate.restrictedLocations != RestrictedLocations.Space)
						{
							list3.Add(titraitTemplate);
						}
					}
					foreach (TITraitTemplate titraitTemplate2 in list3)
					{
						ticouncilorState.RemoveTrait(titraitTemplate2);
					}
					ticouncilorState.ChangeXP(Mathf.Max(0, TemplateManager.global.initialXPPerYearAge * (ticouncilorState.age - TemplateManager.global.minAgeForXPBonus)));
					this.SetIntel(ticouncilorState, TemplateManager.global.intelToSeeCouncilorMission, null, false);
				}
			}
		}

		// Token: 0x060031D7 RID: 12759 RVA: 0x0010DBFC File Offset: 0x0010BDFC
		private void AgeCouncilors()
		{
			if (this.IsActiveHumanFaction)
			{
				foreach (TICouncilorState ticouncilorState in this.councilors)
				{
					float num = TIEffectsState.SumEffectsModifiers(Context.HumanLifespan, this, 65f, null);
					float num2 = 65f + num + (float)((ticouncilorState.gender == CouncilorGender.Female) ? 6 : 0);
					float num3 = (float)ticouncilorState.age - num2;
					if ((float)ticouncilorState.age > num2)
					{
						float num4 = Mathf.Pow((float)ticouncilorState.age - num2, 1.2f);
						if (num <= 0f && TIUtilities.RandomFloatValue() * 1200f < num4 && !ticouncilorState.traits.Contains(this.declining))
						{
							ticouncilorState.AddTrait(this.declining, true);
						}
						float num5 = 0.016f + 0.001f * num3 + 5.001E-06f * num3 * num3 * num3;
						if (TIUtilities.RandomFloatValue() < num5 / 12f)
						{
							TINotificationQueueState.LogCouncilorPassesAway(ticouncilorState);
							ticouncilorState.KillCouncilor(false, null);
							if (this.isActivePlayer)
							{
								this.UnlockAchievement("councilorDeathNatural");
								break;
							}
							break;
						}
					}
				}
			}
		}

		// Token: 0x060031D8 RID: 12760 RVA: 0x0010DD34 File Offset: 0x0010BF34
		internal void MonthlyFactionUpdate()
		{
			if (TITimeState.CampaignDuration_days() > 14 && this.GenerateRecruitableCouncilors(false) && this.councilors.Count < this.maxCouncilSize)
			{
				TINotificationQueueState.AddCouncilorMessage((this.councilors.Count > 0) ? this.councilors.SelectRandomItem<TICouncilorState>().ref_gameState : base.ref_gameState, CouncilorChatType.RecruitPoolUpdated, this);
			}
			this.MonthlyProjectTriggerChanceChange();
			this.UpdateEstimatedAlienHate(0f, true);
			if (this.IsAlienFaction && !this.finishedProjectNames.Contains(TIGlobalConfig.globalConfig.alienAdvancedMasterProject) && (TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact() > TIGlobalConfig.globalConfig.GetCampaignDurationBeforeAlienAdvancedTech() || TIFactionState.<MonthlyFactionUpdate>g__AlternateTriggersForAdvancedAlienTechPassed|337_0()))
			{
				TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(TIGlobalConfig.globalConfig.alienAdvancedMasterProject, false);
				if (tiprojectTemplate != null)
				{
					this.OnProjectComplete(tiprojectTemplate, -1, true, false);
					this.updateShipDesignsFlag = true;
				}
			}
			if (TITimeState.Now().month == 1 && this.IsAlienFaction)
			{
				List<FactionGoal_DefendWithFleet> list = AIEvaluators.GetBossDefenseGoals(this).ToList<FactionGoal_DefendWithFleet>();
				float typicalShipStrength = this.GetTypicalShipSpaceCombatValue();
				float typicalShipMC = this.GetTypicalShipMissionControlConsumption();
				if (list.Where<FactionGoal_DefendWithFleet>((FactionGoal_DefendWithFleet x) => x.MayIncreaseFleetSize()).Sum<FactionGoal_DefendWithFleet>(delegate(FactionGoal_DefendWithFleet x)
				{
					float num2 = 0f;
					if (x.assignedFleet != null)
					{
						num2 = x.assignedFleet.SpaceCombatValue() / typicalShipStrength / typicalShipMC;
					}
					return Mathf.Max((float)x.EarmarkedFleetMC - num2, 0f);
				}) < 40f)
				{
					int num = ((float)(from x in this.nShipyardQueues.SelectMany<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>, ShipConstructionQueueItem>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value)
						where !x.isRefit
						where x.costPaid
						select x).Sum<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => 365.2422f / x.durationInDays).RoundUp() * 0.15f).RoundUp() + 3;
					if (TITimeState.CampaignDuration_years_Exact() < 4f)
					{
						num = 0;
					}
					if ((from x in GameStateManager.AllFactions()
						where !x.veryProAlien
						select x).Any<TIFactionState>((TIFactionState x) => x.unlockedVictoryObjective))
					{
						num *= 2;
					}
					for (int i = 0; i < num; i++)
					{
						FactionGoal_DefendWithFleet nextBossDefenseGoalToFortify = AIEvaluators.GetNextBossDefenseGoalToFortify(this, list);
						int earmarkedFleetMC = nextBossDefenseGoalToFortify.EarmarkedFleetMC;
						nextBossDefenseGoalToFortify.EarmarkedFleetMC = earmarkedFleetMC + 1;
					}
				}
			}
		}

		// Token: 0x060031D9 RID: 12761 RVA: 0x0010DFBC File Offset: 0x0010C1BC
		internal void MidMonthlyUpdate()
		{
			if (this.GenerateOrgsForAcquisition(false) && this.councilors.Count > 0)
			{
				if (this.councilors.Max<TICouncilorState>((TICouncilorState x) => x.availableAdministration) > 0)
				{
					TINotificationQueueState.AddCouncilorMessage(this.councilors.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.availableAdministration), CouncilorChatType.NewOrgsAvailable, this);
				}
			}
			this.AgeCouncilors();
			this.UpdateEstimatedAlienHate(0f, true);
		}

		// Token: 0x060031DA RID: 12762 RVA: 0x0010E050 File Offset: 0x0010C250
		public void CheckForDefeated()
		{
			if (!this.defeated && this.Defeated())
			{
				this.defeated = true;
				foreach (TIOrgState tiorgState in this.unassignedOrgs.ToList<TIOrgState>())
				{
					if (tiorgState.template.allowedOnMarket && tiorgState.orgType != OrgType.Faction)
					{
						this.RemoveOrgFromUnassignedPool(tiorgState);
					}
				}
				TINationState[] array = GameStateManager.AllNations();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].RemoveCouncilUnrestAttempts(this);
				}
				foreach (FactionResource factionResource in Enums.FactionResources)
				{
					if (this.baseIncomes_year.ContainsKey(factionResource) && this.baseIncomes_year[factionResource] > 0f)
					{
						this.ChangeBaseResourceIncome(factionResource, -this.baseIncomes_year[factionResource]);
					}
					if (this.resources.ContainsKey(factionResource) && this.GetCurrentResourceAmount(factionResource) > 0f)
					{
						this.SubtractFromCurrentResource(this.GetCurrentResourceAmount(factionResource), factionResource, true, null);
					}
				}
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					foreach (DailyResourceTransfer dailyResourceTransfer in tifactionState.dailyResourceTransfers.ToList<DailyResourceTransfer>())
					{
						if (dailyResourceTransfer.targetFaction == this)
						{
							this.RemoveDailyResourceTransfer(dailyResourceTransfer);
						}
					}
					foreach (GoalType goalType in tifactionState.factionGoals.Keys.ToList<GoalType>())
					{
						if (this.factionGoals[goalType] != null)
						{
							foreach (TIFactionGoalState tifactionGoalState in this.factionGoals[goalType].ToList<TIFactionGoalState>())
							{
								TIGameState tigameState = tifactionGoalState.target();
								if (((tigameState != null) ? tigameState.ref_faction : null) == this)
								{
									this.RemoveGoal(tifactionGoalState);
								}
							}
						}
					}
				}
				foreach (DailyResourceTransfer dailyResourceTransfer2 in this.dailyResourceTransfers.ToList<DailyResourceTransfer>())
				{
					this.RemoveDailyResourceTransfer(dailyResourceTransfer2);
				}
				foreach (GoalType goalType2 in this.factionGoals.Keys.ToList<GoalType>())
				{
					if (this.factionGoals[goalType2] != null)
					{
						foreach (TIFactionGoalState tifactionGoalState2 in this.factionGoals[goalType2].ToList<TIFactionGoalState>())
						{
							this.RemoveGoal(tifactionGoalState2);
						}
					}
				}
				TINotificationQueueState.LogFactionDefeated(this);
			}
		}

		// Token: 0x060031DB RID: 12763 RVA: 0x0010E3B8 File Offset: 0x0010C5B8
		internal void Daily0000FactionUpdate()
		{
			this.CheckForDefeated();
			if (this.defeated)
			{
				return;
			}
			foreach (TIHabState tihabState in this.habs)
			{
				if (tihabState.MayHaveFluctuatingIncomes)
				{
					tihabState.UpdateCurrentAnnualNetResourceIncomes(false);
				}
			}
			this.CachePriorityBonuses_Day();
			this.UpdateDailyResourceTransfers();
			for (int i = 0; i < Enums.FactionResources.Length; i++)
			{
				FactionResource factionResource = Enums.FactionResources[i];
				float dailyIncome = this.GetDailyIncome(factionResource, false, true);
				this.AddToCurrentResource(dailyIncome, factionResource, true, "Daily Income");
				TIHistoricalData.Record(this, "Total " + factionResource.ToString(), this.GetCurrentResourceAmount(factionResource), 14f, true);
				TIHistoricalData.Record(this, factionResource.ToString() + " income (per day)", dailyIncome, 14f, true);
			}
			TIHistoricalData.Record(this, "Global research fraction", (float)this.researchWeights.Where<int>((int x) => x < 3).Sum() / (float)this.researchWeights.Sum(), 14f, true);
			GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
			if (TITimeState.Now().ExportTime().DayOfWeek == DayOfWeek.Monday)
			{
				this.lastWeeksSpoils = this.thisWeeksCumulativeSpoils;
				this.thisWeeksCumulativeSpoils = 0f;
			}
			if (TITimeState.Now().day == 1)
			{
				this.lastMonthsSpoils = this.thisMonthsCumulativeSpoils;
				this.thisMonthsCumulativeSpoils = 0f;
			}
			this.CheckForResourceShortages();
			this.history_CPCapOverageByDay.Insert(0, this.GetOneDayControlPointCapMissionPenalty());
			this.history_CPCapOverageByDay.RemoveRange(32, this.history_CPCapOverageByDay.Count - 32);
			this.history_MCCapOverageByDay.Insert(0, this.MissionControlShortage);
			this.history_MCCapOverageByDay.RemoveRange(32, this.history_MCCapOverageByDay.Count - 32);
			this.PingForAlienSpaceAssetDetection();
			this.DailyProjectTriggerCheck();
			this.ShipConstructionQueueDailyUpdate();
			this.CheckForNewObjectives();
			this.DailyFleetsUpdate();
			this.highestSpaceStrengthSinceLastAlienKnockdown = Mathf.Max(this.GetFactionStrengthEstimate_SpaceOnly(), this.highestSpaceStrengthSinceLastAlienKnockdown);
			if (TISpaceShipTemplate.AllowDynamicTemplateSpaceCombatValue())
			{
				bool flag = TITimeState.CampaignDuration_days() != 0;
				int num = GameStateManager.AllFactions().IndexOf(this) * 15;
				int num2 = 120;
				if (((flag ? 1 : 0) + num) % num2 == 0)
				{
					TIHabModuleTemplate.InvalidateHabDefenseNumbers(this);
					CoroutineDummy.Singleton.StartCoroutine(this.UpdateShipDesignStrengths());
				}
			}
		}

		// Token: 0x060031DC RID: 12764 RVA: 0x0010E640 File Offset: 0x0010C840
		public IEnumerator UpdateShipDesignStrengths()
		{
			List<TISpaceShipTemplate> list = this.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template).Distinct<TISpaceShipTemplate>().ToList<TISpaceShipTemplate>();
			List<TISpaceShipTemplate> shipDesignsToFullyUpdate = list.ToList<TISpaceShipTemplate>();
			if (this.isActivePlayer)
			{
				int num = Mathf.Max(20, (this.shipDesigns.Count - list.Count) / 3);
				shipDesignsToFullyUpdate.AddRange(this.shipDesigns.Except<TISpaceShipTemplate>(list).Take_Random<TISpaceShipTemplate>(num));
			}
			int num2;
			for (int i = 0; i < shipDesignsToFullyUpdate.Count; i = num2 + 1)
			{
				shipDesignsToFullyUpdate[i].UnnormalizedTemplateSpaceCombatValue(true, 1f);
				if (i % 2 == 0)
				{
					yield return null;
				}
				num2 = i;
			}
			yield return null;
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.shipDesigns)
			{
				tispaceShipTemplate.TemplateSpaceCombatValue(false, 0.5f, 1f, false);
			}
			for (int i = 0; i < this.ships.Count; i = num2 + 1)
			{
				if (i % 50 == 0)
				{
					yield return null;
				}
				this.ships[i].SpaceCombatValue(true, 0f);
				num2 = i;
			}
			yield break;
		}

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x060031DD RID: 12765 RVA: 0x0010E64F File Offset: 0x0010C84F
		public Sprite leaderIcon
		{
			get
			{
				if (this._leaderIcon == null)
				{
					this._leaderIcon = GameControl.assetLoader.LoadAsset<Sprite>(this.pathLeaderIcon);
				}
				return this._leaderIcon;
			}
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x060031DE RID: 12766 RVA: 0x0010E67B File Offset: 0x0010C87B
		public Sprite factionIcon64
		{
			get
			{
				if (this._factionIcon64 == null)
				{
					this._factionIcon64 = GameControl.assetLoader.LoadAsset<Sprite>(this.factionIcon64path);
				}
				return this._factionIcon64;
			}
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x060031DF RID: 12767 RVA: 0x0010E6A7 File Offset: 0x0010C8A7
		public Sprite factionIcon128
		{
			get
			{
				if (this._factionIcon128 == null)
				{
					this._factionIcon128 = GameControl.assetLoader.LoadAsset<Sprite>(this.factionIcon128path);
				}
				return this._factionIcon128;
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x060031E0 RID: 12768 RVA: 0x0010E6D3 File Offset: 0x0010C8D3
		public Sprite factionIcon256
		{
			get
			{
				if (this._factionIcon256 == null)
				{
					this._factionIcon256 = GameControl.assetLoader.LoadAsset<Sprite>(this.factionIcon256path);
				}
				return this._factionIcon256;
			}
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x060031E1 RID: 12769 RVA: 0x0010E6FF File Offset: 0x0010C8FF
		public Sprite factionIcon64UI
		{
			get
			{
				if (this._factionIcon64UI == null)
				{
					this._factionIcon64UI = GameControl.assetLoader.LoadAsset<Sprite>(this.factionIcon64UIpath);
				}
				return this._factionIcon64UI;
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x060031E2 RID: 12770 RVA: 0x0010E72B File Offset: 0x0010C92B
		public Sprite factionIcon128UI
		{
			get
			{
				if (this._factionIcon128UI == null)
				{
					this._factionIcon128UI = GameControl.assetLoader.LoadAsset<Sprite>(this.factionIcon128UIpath);
				}
				return this._factionIcon128UI;
			}
		}

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x060031E3 RID: 12771 RVA: 0x0010E757 File Offset: 0x0010C957
		public Sprite factionIcon256UI
		{
			get
			{
				if (this._factionIcon256UI == null)
				{
					this._factionIcon256UI = GameControl.assetLoader.LoadAsset<Sprite>(this.factionIcon256UIpath);
				}
				return this._factionIcon256UI;
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x060031E4 RID: 12772 RVA: 0x0010E783 File Offset: 0x0010C983
		public Sprite fleetIcon
		{
			get
			{
				if (this._fleetIcon == null)
				{
					this._fleetIcon = GameControl.assetLoader.LoadAsset<Sprite>(this.template.fleetIcon);
				}
				return this._fleetIcon;
			}
		}

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x060031E5 RID: 12773 RVA: 0x0010E7B4 File Offset: 0x0010C9B4
		public Sprite fleetIcon1
		{
			get
			{
				if (this._fleetIcon1 == null)
				{
					this._fleetIcon1 = GameControl.assetLoader.LoadAsset<Sprite>(this.template.fleetIcon1Resource);
				}
				return this._fleetIcon1;
			}
		}

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x060031E6 RID: 12774 RVA: 0x0010E7E5 File Offset: 0x0010C9E5
		public Sprite fleetIcon2
		{
			get
			{
				if (this._fleetIcon2 == null)
				{
					this._fleetIcon2 = GameControl.assetLoader.LoadAsset<Sprite>(this.template.fleetIcon2Resource);
				}
				return this._fleetIcon2;
			}
		}

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x060031E7 RID: 12775 RVA: 0x0010E816 File Offset: 0x0010CA16
		public Sprite fleetIcon3
		{
			get
			{
				if (this._fleetIcon3 == null)
				{
					this._fleetIcon3 = GameControl.assetLoader.LoadAsset<Sprite>(this.template.fleetIcon3Resource);
				}
				return this._fleetIcon3;
			}
		}

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x060031E8 RID: 12776 RVA: 0x0010E847 File Offset: 0x0010CA47
		public Sprite stationIcon
		{
			get
			{
				if (this._stationIcon == null)
				{
					this._stationIcon = GameControl.assetLoader.LoadAsset<Sprite>(this.template.stationIcon);
				}
				return this._stationIcon;
			}
		}

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x060031E9 RID: 12777 RVA: 0x0010E878 File Offset: 0x0010CA78
		public Sprite baseIcon
		{
			get
			{
				if (this._baseIcon == null)
				{
					this._baseIcon = GameControl.assetLoader.LoadAsset<Sprite>(this.template.baseIcon);
				}
				return this._baseIcon;
			}
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x060031EA RID: 12778 RVA: 0x0010E8A9 File Offset: 0x0010CAA9
		private TICouncilorAppearanceTemplate leaderAppearance
		{
			get
			{
				if (this._leaderAppearance == null)
				{
					this._leaderAppearance = this.template.leaderAppearance;
				}
				return this._leaderAppearance;
			}
		}

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x060031EB RID: 12779 RVA: 0x0010E8CC File Offset: 0x0010CACC
		public TIVictoryTemplate victoryTemplate
		{
			get
			{
				if (this._victoryTemplate == null)
				{
					this._victoryTemplate = TemplateManager.Find<TIVictoryTemplate>(this.template.victoryTemplateName, false);
					if (this._victoryTemplate == null)
					{
						Log.Error("No Victory Template Found for " + this.templateName, Array.Empty<object>());
					}
				}
				return this._victoryTemplate;
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x060031EC RID: 12780 RVA: 0x0010E920 File Offset: 0x0010CB20
		public string pathLeaderIcon
		{
			get
			{
				return this.leaderAppearance.iconYoung;
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x060031ED RID: 12781 RVA: 0x0010E92D File Offset: 0x0010CB2D
		public string pathLeaderHeadVideo
		{
			get
			{
				return this.leaderAppearance.idleVideoYoung;
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x060031EE RID: 12782 RVA: 0x0010E93A File Offset: 0x0010CB3A
		public string pathLeaderTorsoVideo
		{
			get
			{
				return this.leaderAppearance.idleVideoOld;
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x060031EF RID: 12783 RVA: 0x0010E947 File Offset: 0x0010CB47
		public string pathLeaderHeadPortrait
		{
			get
			{
				return this.leaderAppearance.portraitYoung;
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x060031F0 RID: 12784 RVA: 0x0010E954 File Offset: 0x0010CB54
		public string pathLeaderTorsoPortration
		{
			get
			{
				return this.leaderAppearance.portraitOld;
			}
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x0010E961 File Offset: 0x0010CB61
		public static bool DontAccumulateResource(FactionResource resourceType)
		{
			return TIResourcesCost.unAccumulatableResources.Contains(resourceType);
		}

		// Token: 0x060031F2 RID: 12786 RVA: 0x0010E96E File Offset: 0x0010CB6E
		public static bool ResourceCanGoNegative(FactionResource resourceType)
		{
			return TIResourcesCost.resourcesAllowedToGoNegative.Contains(resourceType);
		}

		// Token: 0x060031F3 RID: 12787 RVA: 0x0010E97B File Offset: 0x0010CB7B
		public static bool TradeableResource(FactionResource resourceType)
		{
			return !TIResourcesCost.unTradeableResources.Contains(resourceType);
		}

		// Token: 0x060031F4 RID: 12788 RVA: 0x0010E98C File Offset: 0x0010CB8C
		public void ChangeBaseResourceIncome(FactionResource resourceType, float amount)
		{
			if (resourceType != FactionResource.None)
			{
				Dictionary<FactionResource, float> dictionary = this.baseIncomes_year;
				dictionary[resourceType] += amount;
				if ((resourceType != FactionResource.Influence || !TIGlobalConfig.globalConfig.allowNegativeInfluenceBaseIncome) && !TIFactionState.ResourceCanGoNegative(resourceType))
				{
					this.baseIncomes_year[resourceType] = Mathf.Max(0f, this.baseIncomes_year[resourceType]);
				}
				this.SetResourceIncomeDataDirty(resourceType);
			}
		}

		// Token: 0x060031F5 RID: 12789 RVA: 0x0010E9F6 File Offset: 0x0010CBF6
		public float GetCurrentResourceAmount(FactionResource resourceType)
		{
			if (!this.resources.ContainsKey(resourceType))
			{
				return 0f;
			}
			return this.resources[resourceType];
		}

		// Token: 0x060031F6 RID: 12790 RVA: 0x0010EA18 File Offset: 0x0010CC18
		private void RecordTransaction(float amountToAdd, FactionResource resourceType, string label = null)
		{
			if (label == null)
			{
				label = new StackTrace().ToString().GetHashCode().ToString();
			}
			List<TIFactionState.Transaction> list;
			if (!this.Transactions.TryGetValue(label, out list))
			{
				list = (this.Transactions[label] = new List<TIFactionState.Transaction>());
			}
			list.Add(new TIFactionState.Transaction
			{
				Resource = resourceType,
				Amount = amountToAdd,
				Date = TITimeState.Now()
			});
			int num = 12;
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddMonths(-num);
			while (list.Count > 0 && list.First<TIFactionState.Transaction>().Date < tidateTime)
			{
				list.RemoveAt(0);
			}
		}

		// Token: 0x060031F7 RID: 12791 RVA: 0x0010EAC8 File Offset: 0x0010CCC8
		public float AddToCurrentResource(float amountToAdd, FactionResource resourceType, bool suppressFactionResourcesUpdatedEvent = false, string label = null)
		{
			if (amountToAdd == 0f)
			{
				return this.GetCurrentResourceAmount(resourceType);
			}
			this.RecordTransaction(amountToAdd, resourceType, label);
			if (resourceType == FactionResource.Research)
			{
				if (amountToAdd < 0f)
				{
					List<ProjectProgress> list = this.currentProjectProgress.ToList<ProjectProgress>();
					float num = -amountToAdd;
					float num3;
					for (float num2 = 0f; num2 < num; num2 += num3)
					{
						if (list.Count<ProjectProgress>() <= 0)
						{
							break;
						}
						ProjectProgress projectProgress = list.SelectRandomItem<ProjectProgress>();
						list.Remove(projectProgress);
						num3 = Mathf.Min(num - num2, projectProgress.accumulatedResearch);
						projectProgress.accumulatedResearch -= num3;
					}
				}
				else
				{
					this.DistributeResearchToSlots(amountToAdd);
				}
				return 0f;
			}
			if (TIFactionState.DontAccumulateResource(resourceType))
			{
				return 0f;
			}
			Dictionary<FactionResource, float> dictionary = this.resources;
			dictionary[resourceType] += amountToAdd;
			if (this.resources[resourceType] < 0f && !TIFactionState.ResourceCanGoNegative(resourceType))
			{
				this.resources[resourceType] = 0f;
			}
			if (!suppressFactionResourcesUpdatedEvent)
			{
				GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
			}
			return this.resources[resourceType];
		}

		// Token: 0x060031F8 RID: 12792 RVA: 0x0010EBE4 File Offset: 0x0010CDE4
		public IEnumerable<TIFactionState.Transaction> GetFilteredTransactions(ref float window_days, string label = null, FactionResource resource = FactionResource.None, Func<string, bool> LabelPredicate = null)
		{
			if (!this.Transactions.Any<KeyValuePair<string, List<TIFactionState.Transaction>>>((KeyValuePair<string, List<TIFactionState.Transaction>> x) => x.Value.Count > 0))
			{
				return Enumerable.Empty<TIFactionState.Transaction>();
			}
			IEnumerable<TIFactionState.Transaction> enumerable;
			if (label != null)
			{
				List<TIFactionState.Transaction> list;
				if (!this.Transactions.TryGetValue(label, out list))
				{
					return Enumerable.Empty<TIFactionState.Transaction>();
				}
				enumerable = list;
			}
			else
			{
				enumerable = this.Transactions.Where<KeyValuePair<string, List<TIFactionState.Transaction>>>((KeyValuePair<string, List<TIFactionState.Transaction>> x) => LabelPredicate == null || LabelPredicate(x.Key)).SelectMany<KeyValuePair<string, List<TIFactionState.Transaction>>, TIFactionState.Transaction>((KeyValuePair<string, List<TIFactionState.Transaction>> x) => x.Value);
			}
			if (resource != FactionResource.None)
			{
				enumerable = enumerable.Where<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Resource == resource);
			}
			List<TIFactionState.Transaction> list2;
			if (this.Transactions.TryGetValue("Daily Income", out list2) && list2.Count > 0)
			{
				float num = (float)(TITimeState.Now() - list2[0].Date).TotalDays;
				window_days = Mathf.Min(window_days, num);
			}
			TIDateTime cutoffDate = TITimeState.Now();
			cutoffDate.AddDays(-window_days);
			return enumerable.Where<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Date > cutoffDate);
		}

		// Token: 0x060031F9 RID: 12793 RVA: 0x0010ED23 File Offset: 0x0010CF23
		public IEnumerable<TIFactionState.Transaction> GetFilteredTransactions(float window_days, string label = null, FactionResource resource = FactionResource.None, Func<string, bool> LabelPredicate = null)
		{
			return this.GetFilteredTransactions(ref window_days, label, resource, LabelPredicate);
		}

		// Token: 0x060031FA RID: 12794 RVA: 0x0010ED31 File Offset: 0x0010CF31
		public float SubtractFromCurrentResource(float amountToSubtract, FactionResource resourceType, bool suppressFactionResourcesUpdatedEvent = false, string label = null)
		{
			return this.AddToCurrentResource(-amountToSubtract, resourceType, suppressFactionResourcesUpdatedEvent, label);
		}

		// Token: 0x060031FB RID: 12795 RVA: 0x0010ED40 File Offset: 0x0010CF40
		public float TransferResourceToFaction(float amountToTransfer, FactionResource resource, TIFactionState receivingFaction)
		{
			if (TIFactionState.DontAccumulateResource(resource))
			{
				return 0f;
			}
			float num = this.GetCurrentResourceAmount(resource);
			if (resource == FactionResource.Research)
			{
				num = this.GetLoseableResearch();
			}
			if (!TIFactionState.ResourceCanGoNegative(resource) && amountToTransfer > num)
			{
				amountToTransfer = num;
			}
			this.SubtractFromCurrentResource(amountToTransfer, resource, false, "Transfer to " + receivingFaction.templateName);
			receivingFaction.AddToCurrentResource(amountToTransfer, resource, false, "Transfer from " + this.templateName);
			return amountToTransfer;
		}

		// Token: 0x060031FC RID: 12796 RVA: 0x0010EDB2 File Offset: 0x0010CFB2
		public float GetLoseableResearch()
		{
			return this.currentProjectProgress.Sum<ProjectProgress>((ProjectProgress x) => x.accumulatedResearch);
		}

		// Token: 0x060031FD RID: 12797 RVA: 0x0010EDDE File Offset: 0x0010CFDE
		public float GetMonthlyIncome(FactionResource resourceType, bool dontRecalculate = false, bool suppressFactionResourcesUpdatedEvent = false)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncome(resourceType, dontRecalculate, suppressFactionResourcesUpdatedEvent, false);
			}
			return this.GetYearlyIncome(resourceType, dontRecalculate, suppressFactionResourcesUpdatedEvent, false) / 12f;
		}

		// Token: 0x060031FE RID: 12798 RVA: 0x0010EE03 File Offset: 0x0010D003
		public float GetNetDailyIncome(FactionResource resourceType, bool suppressFactionResourcesUpdatedEvent = false)
		{
			if (resourceType == FactionResource.MissionControl)
			{
				return (float)this.AvailableMissionControl;
			}
			return this.GetDailyIncome(resourceType, false, suppressFactionResourcesUpdatedEvent);
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x0010EE1A File Offset: 0x0010D01A
		public float GetDailyIncome(FactionResource resourceType, bool dontRecalculate = false, bool suppressFactionResourcesUpdatedEvent = false)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncome(resourceType, dontRecalculate, suppressFactionResourcesUpdatedEvent, false);
			}
			return this.GetYearlyIncome(resourceType, dontRecalculate, suppressFactionResourcesUpdatedEvent, false) / 365.2422f;
		}

		// Token: 0x06003200 RID: 12800 RVA: 0x0010EE3F File Offset: 0x0010D03F
		public float GetSpoilsAdjustedMonthlyIncome()
		{
			return this.GetMonthlyIncome(FactionResource.Money, true, false) + 30.436874f * this.mediumTermDailySpoilsIncome;
		}

		// Token: 0x06003201 RID: 12801 RVA: 0x0010EE57 File Offset: 0x0010D057
		public int GetMaxSimultaneousProjects()
		{
			if (!this.IsAlienFaction)
			{
				return 1 + (this.OrgProjectAllowed() ? 1 : 0) + (this.HabProjectAllowed() ? 1 : 0);
			}
			return 0;
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x0010EE7E File Offset: 0x0010D07E
		public float GetTotalProjects()
		{
			return this.GetYearlyIncomeFromHQ(FactionResource.Projects) + (float)this.TraitProjectCount() + (float)this.OrgProjectCount() + (float)this.HabProjectCount();
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x0010EEA0 File Offset: 0x0010D0A0
		public int GetMissionControlFromCouncilors()
		{
			int num = 0;
			foreach (TICouncilorState ticouncilorState in this.activeCouncilors)
			{
				num += (int)ticouncilorState.GetYearlyIncome(FactionResource.MissionControl);
			}
			return num;
		}

		// Token: 0x06003204 RID: 12804 RVA: 0x0010EEFC File Offset: 0x0010D0FC
		public int GetMissionControlFromNations()
		{
			int num = 0;
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				num += (int)tinationState.GetMonthlyCouncilResourceShare(this, FactionResource.MissionControl, false);
			}
			return num;
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x0010EF54 File Offset: 0x0010D154
		public int GetMissionControlContributionFromHabs()
		{
			int num = 0;
			foreach (TISectorState tisectorState in this.habSectors)
			{
				foreach (TIHabModuleState tihabModuleState in tisectorState.habModules)
				{
					if (tihabModuleState.moduleTemplate != null && tihabModuleState.active && tihabModuleState.moduleTemplate.missionControl > 0)
					{
						num += tihabModuleState.moduleTemplate.missionControl;
					}
				}
			}
			return num;
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x0010F008 File Offset: 0x0010D208
		public List<ResourceValue> AvailableSpaceResources(float fraction = 1f)
		{
			List<ResourceValue> list = new List<ResourceValue>();
			foreach (FactionResource factionResource in TIResourcesCost.spaceResources)
			{
				list.Add(new ResourceValue
				{
					resource = factionResource,
					value = this.GetCurrentResourceAmount(factionResource) * fraction
				});
			}
			return list;
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x0010F084 File Offset: 0x0010D284
		public List<ResourceValue> AvailableSpaceResourcesExcept(float fraction, TIResourcesCost committedSpending)
		{
			List<ResourceValue> list = new List<ResourceValue>();
			foreach (FactionResource factionResource in TIResourcesCost.spaceResources)
			{
				list.Add(new ResourceValue
				{
					resource = factionResource,
					value = Mathf.Max(0f, this.GetCurrentResourceAmount(factionResource) * fraction - committedSpending.GetSingleCostValue(factionResource))
				});
			}
			return list;
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x0010F110 File Offset: 0x0010D310
		public int GetMaxMissionControl()
		{
			float num = this.GetYearlyIncomeFromHQ(FactionResource.MissionControl) + (float)this.GetMissionControlFromCouncilors() + (float)this.GetMissionControlFromNations() + (float)this.GetMissionControlContributionFromHabs();
			num += TIEffectsState.SumEffectsModifiers(Context.MissionControlDisruption_PCT, this, num, null);
			return (int)num;
		}

		// Token: 0x06003209 RID: 12809 RVA: 0x0010F150 File Offset: 0x0010D350
		public int GetMaxMissionControlFromBuildableSources()
		{
			float num = (float)(this.GetMissionControlFromCouncilors() + this.GetMissionControlFromNations() + this.GetMissionControlContributionFromHabs());
			num += TIEffectsState.SumEffectsModifiers(Context.MissionControlDisruption_PCT, this, num, null);
			return (int)num;
		}

		// Token: 0x0600320A RID: 12810 RVA: 0x0010F188 File Offset: 0x0010D388
		public float GetBaselineControlPointMaintenanceCost(bool includeDisabled = false)
		{
			float num = 0f;
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				if (!ticontrolPoint.benefitsDisabled || includeDisabled)
				{
					num += ticontrolPoint.BaselineMaintenanceCost;
				}
			}
			return num;
		}

		// Token: 0x0600320B RID: 12811 RVA: 0x0010F1F4 File Offset: 0x0010D3F4
		public float GetControlPointMaintenanceFreebieCap()
		{
			if (!this.IsAlienFaction)
			{
				return (float)(TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies + (this.isActivePlayer ? 0 : TIGlobalValuesState.GlobalValues.scenarioCustomizations.controlPointMaintenanceFreebieBonusAI) + this.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.controlPointCapacity) + this.habs.Sum<TIHabState>((TIHabState x) => x.controlPointCapacityValue)) - TIEffectsState.SumEffectsModifiers(Context.ControlPointMaintenance, this, (float)TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies, null);
			}
			return 20000f;
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x0010F2A4 File Offset: 0x0010D4A4
		public float GetOneDayControlPointCapMissionPenalty()
		{
			float num = this.GetBaselineControlPointMaintenanceCost(false) - this.GetControlPointMaintenanceFreebieCap();
			if (num > 0f)
			{
				return num * TemplateManager.global.TIMissionModifier_ControlPointOverage_Multiplier;
			}
			return 0f;
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x0010F2DA File Offset: 0x0010D4DA
		public float GetAveragedControlPointCapPenaltyToMissions()
		{
			return this.history_CPCapOverageByDay.Average();
		}

		// Token: 0x0600320E RID: 12814 RVA: 0x0010F2E7 File Offset: 0x0010D4E7
		public float GetAveragedMissionControlShortage()
		{
			return (float)this.history_MCCapOverageByDay.Average();
		}

		// Token: 0x0600320F RID: 12815 RVA: 0x0010F2F8 File Offset: 0x0010D4F8
		public float GetAnnualInfluenceCostOfNextControlPoint(TINationState nation)
		{
			float num = this.GetBaselineControlPointMaintenanceCost(false) - this.GetControlPointMaintenanceFreebieCap() + nation.ControlPointMaintenanceCost;
			if (num > 0f)
			{
				return num * num;
			}
			return 0f;
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x0010F32C File Offset: 0x0010D52C
		public float GetAnnualControlPointMaintenanceCost()
		{
			if (this.IsAlienFaction)
			{
				return 0f;
			}
			float num = this.GetBaselineControlPointMaintenanceCost(false) - this.GetControlPointMaintenanceFreebieCap();
			if (num > 0f)
			{
				return num * num;
			}
			return 0f;
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x0010F367 File Offset: 0x0010D567
		public void SetPermaAbandonNationStatus(TINationState nation, bool setAbandoned)
		{
			if (setAbandoned)
			{
				this.permaAbandonedNations.AddUnique(nation);
				return;
			}
			this.permaAbandonedNations.Remove(nation);
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x0010F388 File Offset: 0x0010D588
		public float GetYearlyIncomeFromHQ(FactionResource resourceType)
		{
			if (resourceType == FactionResource.MissionControl)
			{
				return this.baseIncomes_year[FactionResource.MissionControl] + (float)GameStateManager.Time().template.bonusMissionControl + TIGlobalValuesState.GlobalValues.scenarioCustomizations.missionControlBonus + (this.isActivePlayer ? 0f : TIGlobalValuesState.GlobalValues.scenarioCustomizations.missionControlBonusAI);
			}
			return this.baseIncomes_year[resourceType];
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x0010F3F4 File Offset: 0x0010D5F4
		public float GetMonthlyIncomeFromHQ(FactionResource resource)
		{
			if (resource == FactionResource.Projects)
			{
				return this.baseIncomes_year[resource];
			}
			if (resource == FactionResource.MissionControl)
			{
				return this.baseIncomes_year[FactionResource.MissionControl] + (float)GameStateManager.Time().template.bonusMissionControl + TIGlobalValuesState.GlobalValues.scenarioCustomizations.missionControlBonus + (this.isActivePlayer ? 0f : TIGlobalValuesState.GlobalValues.scenarioCustomizations.missionControlBonusAI);
			}
			return this.baseIncomes_year[resource] / 12f;
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x0010F478 File Offset: 0x0010D678
		public float GetDailyIncomeFromHQ(FactionResource resourceType)
		{
			if (resourceType == FactionResource.Projects)
			{
				return this.baseIncomes_year[resourceType];
			}
			if (resourceType == FactionResource.MissionControl)
			{
				return this.baseIncomes_year[FactionResource.MissionControl] + (float)GameStateManager.Time().template.bonusMissionControl + TIGlobalValuesState.GlobalValues.scenarioCustomizations.missionControlBonus + (this.isActivePlayer ? 0f : TIGlobalValuesState.GlobalValues.scenarioCustomizations.missionControlBonusAI);
			}
			return this.baseIncomes_year[resourceType] / 365.2422f;
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x0010F4F9 File Offset: 0x0010D6F9
		public float GetDailyIncomeFromCouncilors(FactionResource resourceType)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncomeFromCouncilors(resourceType);
			}
			return this.GetYearlyIncomeFromCouncilors(resourceType) / 365.2422f;
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x0010F518 File Offset: 0x0010D718
		public float GetDailyIncomeFromNations(FactionResource resourceType, bool includeDeficit = true)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncomeFromNations(resourceType, includeDeficit);
			}
			return this.GetYearlyIncomeFromNations(resourceType, includeDeficit) / 365.2422f;
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x0010F539 File Offset: 0x0010D739
		public float GetMonthlyIncomeFromNations(FactionResource resourceType, bool includeDeficit = true)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncomeFromNations(resourceType, includeDeficit);
			}
			return this.GetYearlyIncomeFromNations(resourceType, includeDeficit) / 12f;
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x0010F55A File Offset: 0x0010D75A
		public float GetDailyIncomeFromHabs(FactionResource resourceType)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncomeFromHabs(resourceType);
			}
			return this.GetYearlyIncomeFromHabs(resourceType) / 365.2422f;
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x0010F57C File Offset: 0x0010D77C
		public float GetYearlyIncomeFromCouncilors(FactionResource resourceType)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.activeCouncilors)
			{
				num += ticouncilorState.GetYearlyIncome(resourceType);
			}
			return num;
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x0010F5DC File Offset: 0x0010D7DC
		public float GetMonthlyIncomeFromCouncilors(FactionResource resourceType)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.activeCouncilors)
			{
				num += ticouncilorState.GetMonthlyIncome(resourceType);
			}
			return num;
		}

		// Token: 0x0600321B RID: 12827 RVA: 0x0010F63C File Offset: 0x0010D83C
		public float GetYearlyIncomeFromNations(FactionResource resourceType, bool includeDeficit = true)
		{
			float num = 0f;
			switch (resourceType)
			{
			case FactionResource.Money:
			{
				using (List<TIControlPoint>.Enumerator enumerator = this.controlPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIControlPoint ticontrolPoint = enumerator.Current;
						if (!ticontrolPoint.benefitsDisabled)
						{
							num += 12f * ticontrolPoint.nation.GetMonthlyMoneyIncomeFromControlPoint(this);
						}
					}
					return num;
				}
				break;
			}
			case FactionResource.Influence:
				goto IL_01E2;
			case FactionResource.Operations:
			case FactionResource.Projects:
				return num;
			case FactionResource.Research:
				goto IL_012D;
			case FactionResource.Boost:
				break;
			default:
				return num;
			}
			if (this.IsActiveHumanFaction)
			{
				foreach (TIControlPoint ticontrolPoint2 in this.controlPoints)
				{
					if (!ticontrolPoint2.benefitsDisabled)
					{
						num += 12f * ticontrolPoint2.nation.GetMonthlyBoostIncomeFromControlPoint();
					}
				}
			}
			if (!this.IsAlienProxy)
			{
				return num;
			}
			using (List<TIControlPoint>.Enumerator enumerator = GameStateManager.AlienFaction().controlPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIControlPoint ticontrolPoint3 = enumerator.Current;
					if (!ticontrolPoint3.benefitsDisabled)
					{
						num += 12f * ticontrolPoint3.nation.GetMonthlyBoostIncomeFromControlPoint();
					}
				}
				return num;
			}
			IL_012D:
			if (this.IsActiveHumanFaction)
			{
				foreach (TIControlPoint ticontrolPoint4 in this.controlPoints)
				{
					if (!ticontrolPoint4.benefitsDisabled)
					{
						num += 12f * ticontrolPoint4.nation.GetMonthlyResearchFromControlPoint(this);
					}
				}
			}
			if (!this.IsAlienProxy)
			{
				return num;
			}
			using (List<TIControlPoint>.Enumerator enumerator = GameStateManager.AlienFaction().controlPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIControlPoint ticontrolPoint5 = enumerator.Current;
					if (!ticontrolPoint5.benefitsDisabled)
					{
						num += 12f * ticontrolPoint5.nation.GetMonthlyResearchFromControlPoint(this);
					}
				}
				return num;
			}
			IL_01E2:
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				num += 12f * tinationState.GetMonthlyCouncilResourceShare(this, resourceType, false);
			}
			if (!this.IsAlienFaction && includeDeficit)
			{
				num -= this.GetAnnualControlPointMaintenanceCost();
			}
			return num;
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x0010F8D0 File Offset: 0x0010DAD0
		public float GetYearlyNetIncomeFromShips(FactionResource resource)
		{
			return this.GetMonthlyNetIncomeFromShips(resource) * 12f;
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x0010F8E0 File Offset: 0x0010DAE0
		public float GetMonthlyNetIncomeFromShips(FactionResource resource)
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.GetMonthlyNetIncome(resource));
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x0010F911 File Offset: 0x0010DB11
		public float GetDailyNetIncomeFromShips(FactionResource resource)
		{
			return this.GetYearlyNetIncomeFromShips(resource) / 365.2422f;
		}

		// Token: 0x0600321F RID: 12831 RVA: 0x0010F920 File Offset: 0x0010DB20
		public float GetYearlyGrossRevenueFromShips(FactionResource resource)
		{
			return this.GetMonthlyGrossRevenueFromShips(resource) * 12f;
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x0010F930 File Offset: 0x0010DB30
		public float GetMonthlyGrossRevenueFromShips(FactionResource resource)
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.GetMonthlyGrossRevenue(resource));
		}

		// Token: 0x06003221 RID: 12833 RVA: 0x0010F961 File Offset: 0x0010DB61
		public float GetDailyGrossRevenueFromShips(FactionResource resource)
		{
			return this.GetYearlyGrossRevenueFromShips(resource) / 365.2422f;
		}

		// Token: 0x06003222 RID: 12834 RVA: 0x0010F970 File Offset: 0x0010DB70
		public float GetYearlyExpensesFromShips(FactionResource resource)
		{
			return this.GetMonthlyExpensesFromShips(resource) * 12f;
		}

		// Token: 0x06003223 RID: 12835 RVA: 0x0010F980 File Offset: 0x0010DB80
		public float GetMonthlyExpensesFromShips(FactionResource resource)
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.GetMonthlyExpenses(resource));
		}

		// Token: 0x06003224 RID: 12836 RVA: 0x0010F9B1 File Offset: 0x0010DBB1
		public float GetDailyExpensesFromShips(FactionResource resource)
		{
			return this.GetYearlyExpensesFromShips(resource) / 365.2422f;
		}

		// Token: 0x06003225 RID: 12837 RVA: 0x0010F9C0 File Offset: 0x0010DBC0
		public static bool IsASpaceResource(FactionResource resource)
		{
			return TIResourcesCost.spaceResources.Contains(resource);
		}

		// Token: 0x06003226 RID: 12838 RVA: 0x0010F9D0 File Offset: 0x0010DBD0
		public float GetYearlyIncomeFromHabs(FactionResource resourceType)
		{
			float num = 0f;
			foreach (TIHabState tihabState in this.habs)
			{
				num += tihabState.GetAnnualNetResourceIncome(this, resourceType);
			}
			if (resourceType != FactionResource.Research)
			{
				if (resourceType == FactionResource.Boost)
				{
					num -= this.DailySpaceResourceShortage() * 365.2422f;
				}
			}
			else
			{
				num += TIEffectsState.SumEffectsModifiers(Context.HabResearchProduction, this, num, null);
			}
			return num;
		}

		// Token: 0x06003227 RID: 12839 RVA: 0x0010FA58 File Offset: 0x0010DC58
		public float GetNetYearlyIncomeFromDiplomacy(FactionResource resourceType)
		{
			return this.GetNetDailyIncomeFromDiplomacy(resourceType) * 365.2422f;
		}

		// Token: 0x06003228 RID: 12840 RVA: 0x0010FA67 File Offset: 0x0010DC67
		public float GetNetMonthlyIncomeFromDiplomacy(FactionResource resourceType)
		{
			return this.GetNetDailyIncomeFromDiplomacy(resourceType) * 30.436874f;
		}

		// Token: 0x06003229 RID: 12841 RVA: 0x0010FA78 File Offset: 0x0010DC78
		public float GetNetDailyIncomeFromDiplomacy(FactionResource resourceType)
		{
			float num = 0f;
			foreach (DailyResourceTransfer dailyResourceTransfer in this.dailyResourceTransfers)
			{
				if (dailyResourceTransfer.transfer.resource == resourceType && !this.AI_AtWarWithFaction(dailyResourceTransfer.targetFaction))
				{
					float num2 = dailyResourceTransfer.transfer.value;
					float currentResourceAmount = this.GetCurrentResourceAmount(dailyResourceTransfer.transfer.resource);
					if (num2 > currentResourceAmount)
					{
						num2 = currentResourceAmount;
					}
					num -= num2;
				}
			}
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState != this)
				{
					foreach (DailyResourceTransfer dailyResourceTransfer2 in tifactionState.dailyResourceTransfers)
					{
						if (this == dailyResourceTransfer2.targetFaction && dailyResourceTransfer2.transfer.resource == resourceType && !tifactionState.AI_AtWarWithFaction(this))
						{
							float num3 = dailyResourceTransfer2.transfer.value;
							float currentResourceAmount2 = tifactionState.GetCurrentResourceAmount(dailyResourceTransfer2.transfer.resource);
							if (num3 > currentResourceAmount2)
							{
								num3 = currentResourceAmount2;
							}
							num += num3;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x0600322A RID: 12842 RVA: 0x0010FBE0 File Offset: 0x0010DDE0
		public void AddDailyResourceTransfer(TIFactionState targetFaction, FactionResource resource, float value, TIDateTime expiry, bool fixedValue)
		{
			if (value != 0f)
			{
				DailyResourceTransfer dailyResourceTransfer = null;
				foreach (DailyResourceTransfer dailyResourceTransfer2 in this.dailyResourceTransfers)
				{
					if (dailyResourceTransfer2.targetFaction == targetFaction && dailyResourceTransfer2.transfer.resource == resource && dailyResourceTransfer2.expiry == expiry)
					{
						dailyResourceTransfer = dailyResourceTransfer2;
						break;
					}
				}
				if (dailyResourceTransfer != null)
				{
					dailyResourceTransfer.transfer = new ResourceValue
					{
						resource = dailyResourceTransfer.transfer.resource,
						value = Mathf.Max(dailyResourceTransfer.transfer.value + value, 0f)
					};
				}
				else
				{
					this.dailyResourceTransfers.Add(new DailyResourceTransfer(targetFaction, expiry, resource, value));
				}
				this.SetResourceIncomeDataDirty(resource);
				targetFaction.SetResourceIncomeDataDirty(resource);
			}
		}

		// Token: 0x0600322B RID: 12843 RVA: 0x0010FCD0 File Offset: 0x0010DED0
		public void RemoveDailyResourceTransfer(DailyResourceTransfer transfer)
		{
			this.dailyResourceTransfers.Remove(transfer);
		}

		// Token: 0x0600322C RID: 12844 RVA: 0x0010FCE0 File Offset: 0x0010DEE0
		public void UpdateDailyResourceTransfers()
		{
			TIDateTime tidateTime = TITimeState.Now();
			List<DailyResourceTransfer> list = new List<DailyResourceTransfer>();
			foreach (DailyResourceTransfer dailyResourceTransfer in this.dailyResourceTransfers)
			{
				if (dailyResourceTransfer.expiry != null && dailyResourceTransfer.expiry < tidateTime)
				{
					list.Add(dailyResourceTransfer);
				}
			}
			foreach (DailyResourceTransfer dailyResourceTransfer2 in list)
			{
				dailyResourceTransfer2.targetFaction.SetResourceIncomeDataDirty(dailyResourceTransfer2.transfer.resource);
				this.SetResourceIncomeDataDirty(dailyResourceTransfer2.transfer.resource);
				this.dailyResourceTransfers.Remove(dailyResourceTransfer2);
			}
		}

		// Token: 0x0600322D RID: 12845 RVA: 0x0010FDCC File Offset: 0x0010DFCC
		public float GetMonthlyTransferOutFromResourceTransfers(FactionResource resource, TIFactionState targetFaction, bool includeInactives)
		{
			float num = 0f;
			foreach (DailyResourceTransfer dailyResourceTransfer in this.dailyResourceTransfers)
			{
				if (dailyResourceTransfer.transfer.resource == resource && (targetFaction == null || (dailyResourceTransfer.targetFaction == targetFaction && (includeInactives || !this.AI_AtWarWithFaction(targetFaction)))))
				{
					num += dailyResourceTransfer.transfer.value;
				}
			}
			return num * 30.436874f;
		}

		// Token: 0x0600322E RID: 12846 RVA: 0x0010FE64 File Offset: 0x0010E064
		public float GetMonthlyTransferInFromResourceTransfers(FactionResource resource, TIFactionState originFaction, bool includeInactives)
		{
			float num = 0f;
			List<TIFactionState> list = new List<TIFactionState>();
			if (originFaction == null)
			{
				list = GameStateManager.AllFactions().ToList<TIFactionState>();
				list.Remove(this);
			}
			else
			{
				list.Add(originFaction);
			}
			foreach (TIFactionState tifactionState in list)
			{
				foreach (DailyResourceTransfer dailyResourceTransfer in tifactionState.dailyResourceTransfers)
				{
					if (dailyResourceTransfer.transfer.resource == resource && dailyResourceTransfer.targetFaction == this && (includeInactives || !tifactionState.AI_AtWarWithFaction(this)))
					{
						num += dailyResourceTransfer.transfer.value;
					}
				}
			}
			return num * 30.436874f;
		}

		// Token: 0x0600322F RID: 12847 RVA: 0x0010FF5C File Offset: 0x0010E15C
		public float GetYearlyIncomeFromExcessMissionControl(FactionResource resourceType)
		{
			if (this.IsActiveHumanFaction)
			{
				if (resourceType == FactionResource.Money)
				{
					return (float)Mathf.Min(this.GetMaxMissionControlFromBuildableSources(), this.AvailableMissionControl) * 365.2422f * TemplateManager.global.ExcessMCToMoneyConversion_Day;
				}
				if (resourceType == FactionResource.Research)
				{
					return (float)Mathf.Min(this.GetMaxMissionControlFromBuildableSources(), this.AvailableMissionControl) * 365.2422f * TemplateManager.global.ExcessMCToResearchConversion_Day;
				}
			}
			return 0f;
		}

		// Token: 0x06003230 RID: 12848 RVA: 0x0010FFC8 File Offset: 0x0010E1C8
		public float GetMonthlyIncomeFromExcessMissionControl(FactionResource resourceType)
		{
			return this.GetDailyIncomeFromExcessMissionControl(resourceType) * 30.436874f;
		}

		// Token: 0x06003231 RID: 12849 RVA: 0x0010FFD7 File Offset: 0x0010E1D7
		public float GetDailyIncomeFromExcessMissionControl(FactionResource resourceType)
		{
			return this.GetYearlyIncomeFromExcessMissionControl(resourceType) / 365.2422f;
		}

		// Token: 0x06003232 RID: 12850 RVA: 0x0010FFE6 File Offset: 0x0010E1E6
		public void TriggerFactionResourceUpdateEvent()
		{
			GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
		}

		// Token: 0x06003233 RID: 12851 RVA: 0x00110003 File Offset: 0x0010E203
		public void SetResourceIncomeDataDirty(FactionResource resource)
		{
			this.dirtyResourcesTracker.SetResourceDirty(resource);
			GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
		}

		// Token: 0x06003234 RID: 12852 RVA: 0x0011002C File Offset: 0x0010E22C
		public void SetResourceIncomeDataDirty(FactionResource[] resources)
		{
			for (int i = 0; i < resources.Length; i++)
			{
				this.dirtyResourcesTracker.SetResourceDirty(resources[i]);
			}
			GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
		}

		// Token: 0x06003235 RID: 12853 RVA: 0x00110070 File Offset: 0x0010E270
		public void SetResourceIncomeDataDirty()
		{
			this.dirtyResourcesTracker.SetAllResourcesDirty();
			GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
		}

		// Token: 0x06003236 RID: 12854 RVA: 0x00110098 File Offset: 0x0010E298
		public float GetMonthlyIncomeWithoutDiplomacy(FactionResource resourceType)
		{
			if (resourceType == FactionResource.Projects || resourceType == FactionResource.MissionControl)
			{
				return this.GetYearlyIncomeWithoutDiplomacy(resourceType);
			}
			return this.GetYearlyIncomeWithoutDiplomacy(resourceType) / 12f;
		}

		// Token: 0x06003237 RID: 12855 RVA: 0x001100B7 File Offset: 0x0010E2B7
		public float GetYearlyIncomeWithoutDiplomacy(FactionResource resourceType)
		{
			return this.GetYearlyIncome(resourceType, false, false, false) - this.GetNetYearlyIncomeFromDiplomacy(resourceType);
		}

		// Token: 0x06003238 RID: 12856 RVA: 0x001100CC File Offset: 0x0010E2CC
		public float GetYearlyIncome(FactionResource resourceType, bool dontRecalculate = false, bool suppressFactionResourcesUpdatedEvent = false, bool forceRecalculate = false)
		{
			if (forceRecalculate || (!dontRecalculate && this.dirtyResourcesTracker.IsResourceIncomeDirty(resourceType)))
			{
				float num = this.annualResourceIncomes[resourceType];
				switch (resourceType)
				{
				case FactionResource.Research:
					if (this.IsAlienFaction)
					{
						return 0f;
					}
					this.annualResourceIncomes[resourceType] = this.GetYearlyIncomeFromHQ(resourceType) + this.GetYearlyIncomeFromNations(resourceType, true) + this.GetYearlyIncomeFromCouncilors(resourceType) + this.GetYearlyIncomeFromHabs(resourceType) + this.GetYearlyNetIncomeFromShips(resourceType) + this.GetNetYearlyIncomeFromDiplomacy(resourceType) + this.GetNegativeYearlyIncomeFromUnassignedOrgs(resourceType) + this.GetYearlyIncomeFromExcessMissionControl(resourceType);
					goto IL_011C;
				case FactionResource.Projects:
					this.annualResourceIncomes[FactionResource.Projects] = this.GetTotalProjects();
					goto IL_011C;
				case FactionResource.MissionControl:
					this.annualResourceIncomes[FactionResource.MissionControl] = (float)this.GetMaxMissionControl();
					goto IL_011C;
				}
				this.annualResourceIncomes[resourceType] = this.GetYearlyIncomeFromHQ(resourceType) + this.GetYearlyIncomeFromNations(resourceType, true) + this.GetYearlyIncomeFromCouncilors(resourceType) + this.GetYearlyIncomeFromHabs(resourceType) + this.GetYearlyNetIncomeFromShips(resourceType) + this.GetNetYearlyIncomeFromDiplomacy(resourceType) + this.GetNegativeYearlyIncomeFromUnassignedOrgs(resourceType) + this.GetYearlyIncomeFromExcessMissionControl(resourceType);
				IL_011C:
				if (!suppressFactionResourcesUpdatedEvent && num != this.annualResourceIncomes[resourceType])
				{
					GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
				}
				if (resourceType != FactionResource.Research)
				{
					this.dirtyResourcesTracker.MarkResourceIncomeUpdated(resourceType);
				}
			}
			return this.annualResourceIncomes[resourceType];
		}

		// Token: 0x06003239 RID: 12857 RVA: 0x00110240 File Offset: 0x0010E440
		public float GetUnderConstructionMiningIncomePerDay(FactionResource resource)
		{
			if (!TIResourcesCost.basicSpaceResources.Contains(resource))
			{
				return 0f;
			}
			float num = 0f;
			using (IEnumerator<TIHabModuleState> enumerator = (from x in this.bases
				select x.mine into x
				where x.underConstruction
				select x).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIHabModuleState mine = enumerator.Current;
					TIHabModuleTemplate tihabModuleTemplate = mine.moduleTemplate;
					if (this.IsAlienFaction)
					{
						tihabModuleTemplate = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining)
							where x.alienModule && x.automated == mine.hab.coreModule.moduleTemplate.automated
							where x.tier <= mine.hab.maxTier
							select x).MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
					}
					num += tihabModuleTemplate.DailyResourceIncome(resource, mine.hab, this);
				}
			}
			return num;
		}

		// Token: 0x0600323A RID: 12858 RVA: 0x00110374 File Offset: 0x0010E574
		public float GetUnderConstructionMiningAdjustedNetIncomePerDay(FactionResource resource)
		{
			if (!TIResourcesCost.basicSpaceResources.Contains(resource))
			{
				return 0f;
			}
			return this.GetDailyIncome(resource, false, false) + this.GetUnderConstructionMiningIncomePerDay(resource);
		}

		// Token: 0x0600323B RID: 12859 RVA: 0x0011039A File Offset: 0x0010E59A
		public float GetUnderConstructionMiningAdjustedNetIncomePerYear(FactionResource resource)
		{
			return this.GetUnderConstructionMiningAdjustedNetIncomePerDay(resource) * 365.2422f;
		}

		// Token: 0x0600323C RID: 12860 RVA: 0x001103AC File Offset: 0x0010E5AC
		public void RecalculateIncomes()
		{
			foreach (FactionResource factionResource in this.annualResourceIncomes.Keys.ToList<FactionResource>())
			{
				this.GetYearlyIncome(factionResource, false, true, true);
				this.GetYearlyRevenue(factionResource, true, false);
			}
		}

		// Token: 0x0600323D RID: 12861 RVA: 0x00110418 File Offset: 0x0010E618
		public float GetYearlyRevenue(FactionResource resource, bool forceRecalculate = false, bool forceUseCache = false)
		{
			bool flag = this.dirtyResourcesTracker.IsResourceRevenueDirty(resource);
			float num;
			if ((forceUseCache || (!forceRecalculate && !flag)) && this.cachedYearlyRevenue.TryGetValue(resource, out num))
			{
				return num;
			}
			if (resource == FactionResource.MissionControl)
			{
				this.cachedYearlyRevenue[resource] = (float)this.GetMaxMissionControl();
			}
			else if (resource == FactionResource.Projects)
			{
				this.cachedYearlyRevenue[resource] = this.GetTotalProjects();
			}
			else
			{
				float num2 = this.habs.Sum<TIHabState>((TIHabState x) => x.GetYearlyRevenue(resource, false));
				List<float> list = new List<float>();
				list.Add(this.GetYearlyIncomeFromHQ(resource));
				list.Add(this.GetYearlyIncomeFromNations(resource, true));
				list.Add(this.GetYearlyIncomeFromCouncilors(resource));
				list.Add(this.GetYearlyNetIncomeFromShips(resource));
				list.Add(this.GetNetYearlyIncomeFromDiplomacy(resource));
				list.Add(this.GetYearlyIncomeFromExcessMissionControl(resource));
				float num3 = list.Sum<float>(delegate(float x)
				{
					if (x <= 0f)
					{
						return 0f;
					}
					return x;
				});
				this.cachedYearlyRevenue[resource] = num2 + num3;
			}
			this.dirtyResourcesTracker.MarkResourceRevenueUpdated(resource);
			return this.cachedYearlyRevenue[resource];
		}

		// Token: 0x0600323E RID: 12862 RVA: 0x00110596 File Offset: 0x0010E796
		public float GetMonthlyRevenue(FactionResource resource, bool forceUseCache = false)
		{
			if (resource == FactionResource.MissionControl || resource == FactionResource.Projects)
			{
				return this.GetYearlyRevenue(resource, false, forceUseCache);
			}
			return this.GetYearlyRevenue(resource, false, forceUseCache) / 12f;
		}

		// Token: 0x0600323F RID: 12863 RVA: 0x001105B9 File Offset: 0x0010E7B9
		public float GetDailyRevenue(FactionResource resource, bool forceUseCache = false)
		{
			if (resource == FactionResource.MissionControl || resource == FactionResource.Projects)
			{
				return this.GetYearlyRevenue(resource, false, forceUseCache);
			}
			return this.GetYearlyRevenue(resource, false, forceUseCache) / 365.2422f;
		}

		// Token: 0x06003240 RID: 12864 RVA: 0x001105DC File Offset: 0x0010E7DC
		public float GetYearlyExpenditure(FactionResource resource, bool forceUseCache = false)
		{
			return this.GetYearlyRevenue(resource, false, forceUseCache) - this.GetYearlyIncome(resource, forceUseCache, false, false);
		}

		// Token: 0x06003241 RID: 12865 RVA: 0x001105F2 File Offset: 0x0010E7F2
		public float GetMonthlyRevenue_AI(FactionResource resource)
		{
			return this.GetMonthlyRevenue(resource, true);
		}

		// Token: 0x06003242 RID: 12866 RVA: 0x001105FC File Offset: 0x0010E7FC
		public float GetDailyRevenue_AI(FactionResource resource)
		{
			return this.GetDailyRevenue(resource, true);
		}

		// Token: 0x06003243 RID: 12867 RVA: 0x00110606 File Offset: 0x0010E806
		public float GetYearlyExpenditure_AI(FactionResource resource, bool forceUseCache = false)
		{
			return this.GetYearlyExpenditure(resource, true);
		}

		// Token: 0x06003244 RID: 12868 RVA: 0x00110610 File Offset: 0x0010E810
		public float GetMonthlyGrossRevenue(FactionResource resource)
		{
			if (resource == FactionResource.MissionControl)
			{
				return (float)this.MissionControlIncome;
			}
			float num = this.councilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetMonthlyIncome_PositiveOnly(resource));
			float num2 = this.habs.Sum<TIHabState>((TIHabState x) => x.GetMonthlyRevenue_WithAdviser(resource, false));
			return this.GetMonthlyIncomeFromHQ(resource) + this.GetMonthlyIncomeFromNations(resource, false) + num + this.GetMonthlyGrossRevenueFromShips(resource) + this.GetMonthlyTransferInFromResourceTransfers(resource, null, false) + this.GetYearlyIncomeFromExcessMissionControl(resource) / 12f + num2;
		}

		// Token: 0x06003245 RID: 12869 RVA: 0x001106B8 File Offset: 0x0010E8B8
		public float GetMonthlyGrossExpenses(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Influence:
				return this.councilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetMonthlyIncome_NegativeOnly(resource, true)) + this.GetMonthlyExpensesFromShips(resource) + this.GetMonthlyTransferOutFromResourceTransfers(resource, null, false) + this.habs.Sum<TIHabState>((TIHabState x) => x.GetMonthlySupportCost(resource, false)) + this.GetNegativeYearlyIncomeFromUnassignedOrgs(resource) / 12f + this.GetAnnualControlPointMaintenanceCost() / 12f;
			case FactionResource.Research:
			case FactionResource.Projects:
				return 0f;
			case FactionResource.MissionControl:
				return (float)this.missionControlUsage;
			}
			return this.councilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetMonthlyIncome_NegativeOnly(resource, true)) + this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.GetMonthlyExpenses(resource)) + this.GetMonthlyTransferOutFromResourceTransfers(resource, null, false) + this.GetNegativeYearlyIncomeFromUnassignedOrgs(resource) / 12f + this.habs.Sum<TIHabState>((TIHabState x) => x.GetMonthlySupportCost(resource, false));
		}

		// Token: 0x06003246 RID: 12870 RVA: 0x001107E0 File Offset: 0x0010E9E0
		public float DailySpaceResourceShortage()
		{
			float num = 0f;
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				if (TIFactionState.IsASpaceResource(factionResource))
				{
					float num2 = this.GetCurrentResourceAmount(factionResource);
					float dailyIncome = this.GetDailyIncome(factionResource, false, false);
					num2 += dailyIncome;
					if (num2 < 0f)
					{
						num += Mathf.Abs(num2);
					}
				}
			}
			return num;
		}

		// Token: 0x06003247 RID: 12871 RVA: 0x00110842 File Offset: 0x0010EA42
		public void CheckForResourceShortages()
		{
			if (this.SubstitutingBoostForSpaceResource())
			{
				this.SetResourceIncomeDataDirty(TIFactionState.habSupportResources);
			}
			this.InsufficientBoostToSupportHabs();
		}

		// Token: 0x06003248 RID: 12872 RVA: 0x0011085E File Offset: 0x0010EA5E
		public bool SubstitutingBoostForSpaceResource()
		{
			return this.DailySpaceResourceShortage() > 0f;
		}

		// Token: 0x06003249 RID: 12873 RVA: 0x0011086D File Offset: 0x0010EA6D
		public float DailyHabBoostShortage()
		{
			return Mathf.Abs(Mathf.Min(this.GetCurrentResourceAmount(FactionResource.Boost) + this.GetDailyIncome(FactionResource.Boost, false, false) - this.DailySpaceResourceShortage(), 0f));
		}

		// Token: 0x0600324A RID: 12874 RVA: 0x00110896 File Offset: 0x0010EA96
		public bool InsufficientBoostToSupportHabs()
		{
			return this.DailyHabBoostShortage() > 0f;
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x0600324B RID: 12875 RVA: 0x001108A5 File Offset: 0x0010EAA5
		public bool Insolvent
		{
			get
			{
				return this.IsActiveHumanFaction && this.GetCurrentResourceAmount(FactionResource.Money) <= 0f && this.GetMonthlyIncome(FactionResource.Money, false, false) < 0f;
			}
		}

		// Token: 0x0600324C RID: 12876 RVA: 0x001108CF File Offset: 0x0010EACF
		public bool ResourceShortageOfType(FactionResource resource)
		{
			return this.GetCurrentResourceAmount(resource) <= 0f && this.GetMonthlyIncome(resource, false, false) < 0f;
		}

		// Token: 0x0600324D RID: 12877 RVA: 0x001108F4 File Offset: 0x0010EAF4
		public int GetMissionControlRequirementFromShips()
		{
			int num = this.fleets.Sum<TISpaceFleetState>((TISpaceFleetState y) => y.MissionControlConsumption());
			return num + (int)TIEffectsState.SumEffectsModifiers(Context.ShipMissionControlReduction, this, (float)num, null);
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x0600324E RID: 12878 RVA: 0x0011093C File Offset: 0x0010EB3C
		public int MineNetworkSize
		{
			get
			{
				return this.habs.Count<TIHabState>((TIHabState x) => x.HasActiveMine);
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x0600324F RID: 12879 RVA: 0x00110968 File Offset: 0x0010EB68
		public int SafeMineNextworkSize
		{
			get
			{
				return TemplateManager.global.spaceMineFreebies + (int)TIEffectsState.SumEffectsModifiers(Context.MCFreeSpaceMineNetwork, this, (float)TemplateManager.global.spaceMineFreebies, null);
			}
		}

		// Token: 0x06003250 RID: 12880 RVA: 0x0011098C File Offset: 0x0010EB8C
		public int GetMissionControlRequirementFromNextMine(TISpaceBodyState body = null)
		{
			int mineNetworkSize = this.MineNetworkSize;
			return this.GetMissionControlRequirementFromMineNetwork(mineNetworkSize + 1) - this.GetMissionControlRequirementFromMineNetwork(mineNetworkSize);
		}

		// Token: 0x06003251 RID: 12881 RVA: 0x001109B4 File Offset: 0x0010EBB4
		public int GetMissionControlGainedFromTurningOffMine(TIHabModuleState mine)
		{
			if (!mine.active)
			{
				return 0;
			}
			int mineNetworkSize = this.MineNetworkSize;
			return this.GetMissionControlRequirementFromMineNetwork(mineNetworkSize) - this.GetMissionControlRequirementFromMineNetwork(mineNetworkSize - 1);
		}

		// Token: 0x06003252 RID: 12882 RVA: 0x001109E3 File Offset: 0x0010EBE3
		public int GetMissionControlRequirementFromMineNetwork(int mineNetworkSize = -1)
		{
			if (mineNetworkSize < 0)
			{
				mineNetworkSize = this.MineNetworkSize;
			}
			mineNetworkSize -= this.SafeMineNextworkSize;
			if (mineNetworkSize > 0)
			{
				return Mathf.Max(1, mineNetworkSize * mineNetworkSize / 2);
			}
			return 0;
		}

		// Token: 0x06003253 RID: 12883 RVA: 0x00110A0C File Offset: 0x0010EC0C
		public int GetMissionControlRequirementFromHabs(bool includeMineNetwork = true)
		{
			int num = 0;
			for (int i = 0; i < this.habSectors.Count; i++)
			{
				for (int j = 0; j < this.habSectors[i].habModules.Count; j++)
				{
					TIHabModuleTemplate moduleTemplate = this.habSectors[i].habModules[j].moduleTemplate;
					if (moduleTemplate != null && moduleTemplate.missionControl < 0 && (this.habSectors[i].habModules[j].powered || moduleTemplate.coreModule || moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.ConsumesMCWhenUnpowered)))
					{
						num -= this.habSectors[i].habModules[j].moduleTemplate.missionControl;
					}
				}
			}
			num += (int)TIEffectsState.SumEffectsModifiers(Context.HabMissionControlReduction, this, (float)num, null);
			if (includeMineNetwork)
			{
				num += this.GetMissionControlRequirementFromMineNetwork(-1);
			}
			return num;
		}

		// Token: 0x06003254 RID: 12884 RVA: 0x00110B00 File Offset: 0x0010ED00
		public int GetMissionControlFromRefits()
		{
			int num = 0;
			foreach (List<ShipConstructionQueueItem> list in this.nShipyardQueues.Values.ToList<List<ShipConstructionQueueItem>>())
			{
				foreach (ShipConstructionQueueItem shipConstructionQueueItem in list)
				{
					if (shipConstructionQueueItem.isRefit && shipConstructionQueueItem.originalSpaceShipState != null)
					{
						num += shipConstructionQueueItem.originalSpaceShipState.missionControlConsumption;
					}
				}
			}
			return num;
		}

		// Token: 0x06003255 RID: 12885 RVA: 0x00110BB4 File Offset: 0x0010EDB4
		public int GetFutureMissionControlfromUnderConstructionShips(bool onlyPaidShips = false)
		{
			int num = 0;
			foreach (List<ShipConstructionQueueItem> list in this.nShipyardQueues.Values.ToList<List<ShipConstructionQueueItem>>())
			{
				foreach (ShipConstructionQueueItem shipConstructionQueueItem in list)
				{
					if ((!onlyPaidShips || shipConstructionQueueItem.costPaid) && (!shipConstructionQueueItem.isRefit || shipConstructionQueueItem.originalSpaceShipState == null))
					{
						num += shipConstructionQueueItem.shipDesign.hullTemplate.missionControl;
					}
				}
			}
			return num;
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06003256 RID: 12886 RVA: 0x00110C78 File Offset: 0x0010EE78
		// (set) Token: 0x06003257 RID: 12887 RVA: 0x00110C80 File Offset: 0x0010EE80
		public int missionControlUsage { get; private set; }

		// Token: 0x06003258 RID: 12888 RVA: 0x00110C89 File Offset: 0x0010EE89
		public void SetMissionControlUsageDataDirty()
		{
			this.missionControlUsageDataDirty = true;
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x00110C94 File Offset: 0x0010EE94
		public int GetMissionControlUsage()
		{
			if (this.missionControlUsageDataDirty)
			{
				int missionControlUsage = this.missionControlUsage;
				this.missionControlUsage = this.GetMissionControlRequirementFromShips() + this.GetMissionControlRequirementFromHabs(true) + this.GetMissionControlFromRefits();
				if (missionControlUsage != this.missionControlUsage)
				{
					GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
				}
				this.missionControlUsageDataDirty = false;
			}
			return this.missionControlUsage;
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x00110CFC File Offset: 0x0010EEFC
		public int GetMissionControlUsageUnderConstruction()
		{
			int futureMissionControlfromUnderConstructionShips = this.GetFutureMissionControlfromUnderConstructionShips(true);
			int num = (from x in this.habs.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.UnderConstructionModules())
				where !x.moduleTemplate.coreModule && x.moduleTemplate.missionControl < 0
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => -x.moduleTemplate.missionControl);
			return futureMissionControlfromUnderConstructionShips + num;
		}

		// Token: 0x0600325B RID: 12891 RVA: 0x00110D88 File Offset: 0x0010EF88
		public int GetFutureAdditionalMissionControlUsage()
		{
			return (from x in this.GoalsOfType(TIFactionGoalState.FoundHabGoals, false, true)
				select x as FactionGoal_FoundHab).Count<FactionGoal_FoundHab>(delegate(FactionGoal_FoundHab x)
			{
				TISpaceFleetState assignedFleet = x.assignedFleet;
				return assignedFleet != null && assignedFleet.inTransfer;
			}) + this.GetMissionControlUsageUnderConstruction();
		}

		// Token: 0x0600325C RID: 12892 RVA: 0x00110DF4 File Offset: 0x0010EFF4
		public float GetCurrentMiningMultiplierFromOrgsAndEffects(FactionResource resource)
		{
			if (this.miningMultiplierCachedFrame[resource] != TIFrameCounter.FrameCount)
			{
				float num = 1f;
				if (!this.IsAlienFaction)
				{
					num += this.activeCouncilors.SelectMany<TICouncilorState, TIOrgState>((TICouncilorState x) => x.activeOrgs).Sum<TIOrgState>((TIOrgState z) => z.miningBonus);
					if (this.IsAlienProxy && TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, this))
					{
						num += GameStateManager.AlienFaction().activeCouncilors.SelectMany<TICouncilorState, TIOrgState>((TICouncilorState x) => x.activeOrgs).Sum<TIOrgState>((TIOrgState z) => z.miningBonus);
					}
				}
				num += TIEffectsState.SumEffectsModifiers(Context.SpaceMiningBonus, this, num, null);
				switch (resource)
				{
				case FactionResource.Water:
					num += TIEffectsState.SumEffectsModifiers(Context.MiningWaterBonus, this, num, null);
					break;
				case FactionResource.Volatiles:
					num += TIEffectsState.SumEffectsModifiers(Context.MiningVolatilesBonus, this, num, null);
					break;
				case FactionResource.Metals:
					num += TIEffectsState.SumEffectsModifiers(Context.MiningMetalsBonus, this, num, null);
					break;
				case FactionResource.NobleMetals:
					num += TIEffectsState.SumEffectsModifiers(Context.MiningNoblesBonus, this, num, null);
					break;
				case FactionResource.Fissiles:
					num += TIEffectsState.SumEffectsModifiers(Context.MiningFissilesBonus, this, num, null);
					break;
				}
				this.cachedMiningMultiplier[resource] = num;
				this.miningMultiplierCachedFrame[resource] = TIFrameCounter.FrameCount;
			}
			return this.cachedMiningMultiplier[resource];
		}

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x0600325D RID: 12893 RVA: 0x00110F8F File Offset: 0x0010F18F
		public int MissionControlIncome
		{
			get
			{
				return this.GetYearlyIncome(FactionResource.MissionControl, false, false, false).Round();
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x0600325E RID: 12894 RVA: 0x00110FA0 File Offset: 0x0010F1A0
		public int MissionControlIncomeSansHabIncome
		{
			get
			{
				return this.MissionControlIncome - this.GetMissionControlContributionFromHabs();
			}
		}

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x0600325F RID: 12895 RVA: 0x00110FAF File Offset: 0x0010F1AF
		public bool AnyAvailableMissionControl
		{
			get
			{
				return this.GetYearlyIncome(FactionResource.MissionControl, false, false, false) > (float)this.GetMissionControlUsage();
			}
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06003260 RID: 12896 RVA: 0x00110FC4 File Offset: 0x0010F1C4
		public int AvailableMissionControl
		{
			get
			{
				return Mathf.Max(this.MissionControlBalance, 0);
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06003261 RID: 12897 RVA: 0x00110FD2 File Offset: 0x0010F1D2
		public int MissionControlBalance
		{
			get
			{
				return this.MissionControlIncome - this.GetMissionControlUsage();
			}
		}

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06003262 RID: 12898 RVA: 0x00110FE1 File Offset: 0x0010F1E1
		public int AvailableMissionControlMinusFutureUsage
		{
			get
			{
				return this.AvailableMissionControl - this.GetFutureAdditionalMissionControlUsage();
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06003263 RID: 12899 RVA: 0x00110FF0 File Offset: 0x0010F1F0
		public int MissionControlShortage
		{
			get
			{
				if (!this.IsAlienFaction)
				{
					return -1 * Mathf.Min((int)this.GetYearlyIncome(FactionResource.MissionControl, false, false, false) - this.GetMissionControlUsage(), 0);
				}
				return 0;
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06003264 RID: 12900 RVA: 0x00111016 File Offset: 0x0010F216
		public bool UnlockedSpaceResources
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06003265 RID: 12901 RVA: 0x0011101C File Offset: 0x0010F21C
		public int AI_GenericMissionControlAvailable
		{
			get
			{
				if (this.genericMissionControlAvailableCachedFrame != TIFrameCounter.FrameCount)
				{
					int num = ((float)(this.MissionControlIncome - this.GetMissionControlContributionFromHabs()) * 0.075f).RoundUp();
					this.cachedGenericMissionControlAvailable = Mathf.Max(0, this.MissionControlIncome - (this.GetMissionControlUsage() - this.GetMissionControlRequirementFromMineNetwork(-1)) - this.GetFutureAdditionalMissionControlUsage() - num);
					this.genericMissionControlAvailableCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedGenericMissionControlAvailable;
			}
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06003266 RID: 12902 RVA: 0x0011108B File Offset: 0x0010F28B
		public bool AI_AnyAvailabeGenericMissionControl
		{
			get
			{
				return this.AI_GenericMissionControlAvailable > 0;
			}
		}

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06003267 RID: 12903 RVA: 0x00111096 File Offset: 0x0010F296
		public bool HasAnySpaceResources
		{
			get
			{
				return this.resources.Any<KeyValuePair<FactionResource, float>>((KeyValuePair<FactionResource, float> x) => TIResourcesCost.spaceResources.Contains(x.Key) && x.Value > 0f);
			}
		}

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x06003268 RID: 12904 RVA: 0x001110C2 File Offset: 0x0010F2C2
		public bool UnlockedAntimatter
		{
			get
			{
				return TIEffectsState.CheckForAnyEffectInContext(Context.CanAmassAntimatter, this);
			}
		}

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x06003269 RID: 12905 RVA: 0x001110CF File Offset: 0x0010F2CF
		public bool UnlockedExotics
		{
			get
			{
				return TIEffectsState.CheckForAnyEffectInContext(Context.CanAmassExotics, this);
			}
		}

		// Token: 0x0600326A RID: 12906 RVA: 0x001110DC File Offset: 0x0010F2DC
		public bool UnlockedResource(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Money:
			case FactionResource.Influence:
			case FactionResource.Operations:
			case FactionResource.Research:
			case FactionResource.Boost:
			case FactionResource.MissionControl:
				return true;
			case FactionResource.Water:
			case FactionResource.Volatiles:
			case FactionResource.Metals:
			case FactionResource.NobleMetals:
			case FactionResource.Fissiles:
				return this.UnlockedSpaceResources;
			case FactionResource.Antimatter:
				return this.UnlockedAntimatter;
			case FactionResource.Exotics:
				return this.UnlockedExotics;
			}
			return false;
		}

		// Token: 0x0600326B RID: 12907 RVA: 0x00111144 File Offset: 0x0010F344
		public List<FactionResource> SellableResourcesOnEarth()
		{
			List<FactionResource> list = new List<FactionResource>
			{
				FactionResource.Metals,
				FactionResource.NobleMetals,
				FactionResource.Fissiles
			};
			if (this.UnlockedAntimatter)
			{
				list.Add(FactionResource.Antimatter);
			}
			if (this.UnlockedExotics)
			{
				list.Add(FactionResource.Exotics);
			}
			return list;
		}

		// Token: 0x0600326C RID: 12908 RVA: 0x00111190 File Offset: 0x0010F390
		public TIRegionSpaceFacilityState SelectRandomLaunchSite()
		{
			Dictionary<TILaunchFacilityState, float> dictionary = new Dictionary<TILaunchFacilityState, float>();
			Dictionary<TILaunchFacilityState, float> dictionary2 = new Dictionary<TILaunchFacilityState, float>();
			foreach (TILaunchFacilityState tilaunchFacilityState in GameStateManager.IterateByClass<TILaunchFacilityState>(false))
			{
				float num = tilaunchFacilityState.region.boostPerMonth_dekatons * tilaunchFacilityState.region.boostPerMonth_dekatons;
				if (num > 0f)
				{
					dictionary.Add(tilaunchFacilityState, num * (float)tilaunchFacilityState.region.nation.CountFactionControlPoints(this, true, true, true));
					dictionary2.Add(tilaunchFacilityState, num);
				}
			}
			if (dictionary.Count > 0)
			{
				return dictionary.SelectRandomWeightedItem<KeyValuePair<TILaunchFacilityState, float>>((KeyValuePair<TILaunchFacilityState, float> y) => y.Value, -1f, 1E-37f).Key;
			}
			if (dictionary2.Count > 0)
			{
				return dictionary2.SelectRandomWeightedItem<KeyValuePair<TILaunchFacilityState, float>>((KeyValuePair<TILaunchFacilityState, float> y) => y.Value, -1f, 1E-37f).Key;
			}
			return GameStateManager.AllRegions().SelectRandomItem<TIRegionState>().GetRegionSpaceFacility(SpaceFacilityType.launchFacility);
		}

		// Token: 0x0600326D RID: 12909 RVA: 0x001112C0 File Offset: 0x0010F4C0
		private string GetExpenditureLabel(TIFactionState.Expenditure expenditure, FactionResource resource)
		{
			StringBuilder stringBuilder = new StringBuilder(resource.ToString());
			stringBuilder.Append(" for ").Append(expenditure.ToString());
			return stringBuilder.ToString();
		}

		// Token: 0x0600326E RID: 12910 RVA: 0x001112F8 File Offset: 0x0010F4F8
		public void RecordExpenditure(TIFactionState.Expenditure expenditure, FactionResource resource, float quantity)
		{
			string expenditureLabel = this.GetExpenditureLabel(expenditure, resource);
			TIHistoricalData.Record_Sum(this, expenditureLabel, quantity / 28f, 28f, false);
		}

		// Token: 0x0600326F RID: 12911 RVA: 0x00111324 File Offset: 0x0010F524
		public void RecordExpenditure(TIFactionState.Expenditure expenditure, TIResourcesCost resourcesCost)
		{
			foreach (ResourceValue resourceValue in resourcesCost.resourceCosts)
			{
				this.RecordExpenditure(expenditure, resourceValue.resource, resourceValue.value);
			}
		}

		// Token: 0x06003270 RID: 12912 RVA: 0x00111384 File Offset: 0x0010F584
		public float GetHighestRecordedExpenditurePerDay(TIFactionState.Expenditure expenditure, FactionResource resource, bool update = false)
		{
			if (update)
			{
				this.EstimateExpenditurePerDay(expenditure, resource);
			}
			Dictionary<FactionResource, ValueTuple<TIDateTime, float>> dictionary;
			if (!this.highestRecordedExpenditurePerDay.TryGetValue(expenditure, out dictionary))
			{
				dictionary = (this.highestRecordedExpenditurePerDay[expenditure] = new Dictionary<FactionResource, ValueTuple<TIDateTime, float>>());
			}
			ValueTuple<TIDateTime, float> valueTuple;
			if (!dictionary.TryGetValue(resource, out valueTuple))
			{
				return 0f;
			}
			return valueTuple.Item2;
		}

		// Token: 0x06003271 RID: 12913 RVA: 0x001113D8 File Offset: 0x0010F5D8
		public void RecordExpenditurePerDay(TIFactionState.Expenditure expenditure, FactionResource resource, float expenditurePerDay)
		{
			float num = this.GetHighestRecordedExpenditurePerDay(expenditure, resource, false);
			if (expenditurePerDay > num)
			{
				this.highestRecordedExpenditurePerDay[expenditure][resource] = new ValueTuple<TIDateTime, float>(TITimeState.Now(), expenditurePerDay);
				if (expenditure == TIFactionState.Expenditure.ShipMaintainence)
				{
					this.fleetWetMassDuringHighestShipMaintainence[resource] = this.FleetWetMass_tons;
				}
			}
		}

		// Token: 0x06003272 RID: 12914 RVA: 0x00111428 File Offset: 0x0010F628
		public void ClearHighestRecordedExpenditure(TIFactionState.Expenditure expenditure, FactionResource resource)
		{
			Dictionary<FactionResource, ValueTuple<TIDateTime, float>> dictionary;
			if (!this.highestRecordedExpenditurePerDay.TryGetValue(expenditure, out dictionary))
			{
				return;
			}
			dictionary.Remove(resource);
			if (expenditure == TIFactionState.Expenditure.ShipMaintainence)
			{
				this.fleetWetMassDuringHighestShipMaintainence.Remove(resource);
			}
		}

		// Token: 0x06003273 RID: 12915 RVA: 0x00111460 File Offset: 0x0010F660
		public float EstimateExpenditurePerDay(TIFactionState.Expenditure expenditure, FactionResource resource)
		{
			string expenditureLabel = this.GetExpenditureLabel(expenditure, resource);
			float num = 2f;
			bool flag;
			float num2 = TIHistoricalData.GuessNextReading(this, expenditureLabel, 365.2422f * num, 28f, 100, out flag, TIHistoricalData.EstimateType.High);
			if (!flag)
			{
				this.RecordExpenditurePerDay(expenditure, resource, num2);
			}
			else
			{
				this.ClearHighestRecordedExpenditure(expenditure, resource);
			}
			return num2;
		}

		// Token: 0x06003274 RID: 12916 RVA: 0x001114AC File Offset: 0x0010F6AC
		public float EstimateTotalExpenditurePerDay(FactionResource resource)
		{
			return ((TIFactionState.Expenditure[])Enum.GetValues(typeof(TIFactionState.Expenditure))).Sum<TIFactionState.Expenditure>((TIFactionState.Expenditure x) => this.EstimateExpenditurePerDay(x, resource));
		}

		// Token: 0x06003275 RID: 12917 RVA: 0x001114F2 File Offset: 0x0010F6F2
		public float GetDVPerDayEstimate()
		{
			return 0f;
		}

		// Token: 0x06003276 RID: 12918 RVA: 0x001114FC File Offset: 0x0010F6FC
		public float PredictMaximumMaintainenceCostsPerDay(FactionResource resource)
		{
			float num = this.GetHighestRecordedExpenditurePerDay(TIFactionState.Expenditure.ShipMaintainence, resource, false);
			if (num == 0f)
			{
				return this.FutureFleetWetMass_tons / this.FleetWetMass_tons * this.EstimateExpenditurePerDay(TIFactionState.Expenditure.ShipMaintainence, resource);
			}
			float num2 = num / this.fleetWetMassDuringHighestShipMaintainence[resource];
			return this.FutureFleetWetMass_tons * num2;
		}

		// Token: 0x06003277 RID: 12919 RVA: 0x0011154C File Offset: 0x0010F74C
		public void LogTransfer(Trajectory trajectory)
		{
			TIOrbitState originOrbit = trajectory.originOrbit;
			if (!(((originOrbit != null) ? originOrbit.ref_system : null) == null) && !(trajectory.originOrbit.ref_system == GameStateManager.Sol()))
			{
				TISpaceGameState destination = trajectory.destination;
				if (!(((destination != null) ? destination.ref_system : null) == null) && !(trajectory.destination.ref_system == GameStateManager.Sol()))
				{
					TIFactionState.<>c__DisplayClass552_0 CS$<>8__locals1;
					CS$<>8__locals1.dvPerDay = (float)(trajectory.DV_kps / trajectory.duration_d);
					if (float.IsNaN(CS$<>8__locals1.dvPerDay) || float.IsInfinity(CS$<>8__locals1.dvPerDay))
					{
						return;
					}
					if (trajectory.originOrbit.ref_system == trajectory.destination.ref_system)
					{
						this.LocalTransferDVLog.Add(new ValueTuple<float, float>((float)trajectory.DV_kps, (float)trajectory.duration_d));
						return;
					}
					this.SolarTransferDVLog.Add(new ValueTuple<float, float>((float)trajectory.DV_kps, (float)trajectory.duration_d));
					return;
				}
			}
		}

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x06003278 RID: 12920 RVA: 0x00111646 File Offset: 0x0010F846
		public List<TINationState> totalControlNations
		{
			get
			{
				return (from x in GameStateManager.AllExtantNations()
					where x.TotalOwningFaction == this
					select x).ToList<TINationState>();
			}
		}

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06003279 RID: 12921 RVA: 0x00111663 File Offset: 0x0010F863
		public List<TINationState> majorityControlNations
		{
			get
			{
				return (from x in GameStateManager.AllExtantNations()
					where x.MajorityControlFaction == this
					select x).ToList<TINationState>();
			}
		}

		// Token: 0x0600327A RID: 12922 RVA: 0x00111680 File Offset: 0x0010F880
		public List<TINationState> nationsWithInterest(bool includeAlienProxies)
		{
			return (from x in GameStateManager.AllNations()
				where this.NationWithFactionInterest(x, includeAlienProxies)
				select x).ToList<TINationState>();
		}

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x0600327B RID: 12923 RVA: 0x001116BC File Offset: 0x0010F8BC
		public List<TINationState> executiveNations
		{
			get
			{
				return (from x in this.controlPoints
					where x.executive
					select x.nation).ToList<TINationState>();
			}
		}

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x0600327C RID: 12924 RVA: 0x0011171C File Offset: 0x0010F91C
		public List<TINationState> nationsWithMyControlPoints
		{
			get
			{
				return this.controlPoints.Select<TIControlPoint, TINationState>((TIControlPoint x) => x.nation).Distinct<TINationState>().ToList<TINationState>();
			}
		}

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x0600327D RID: 12925 RVA: 0x00111754 File Offset: 0x0010F954
		public List<PolicyOptionWithTarget> AllSetPolicyMissionOptionsWithTargets
		{
			get
			{
				List<PolicyOptionWithTarget> list = new List<PolicyOptionWithTarget>();
				foreach (TINationState tinationState in this.executiveNations)
				{
					list.AddRange(tinationState.AvailableSetPolicyOptionsWithTargets(false));
				}
				return list;
			}
		}

		// Token: 0x0600327E RID: 12926 RVA: 0x001117B4 File Offset: 0x0010F9B4
		public void AddPlannedPolicy(PolicyOptionWithTarget policy)
		{
			this.plannedPolicies.Add(policy);
		}

		// Token: 0x0600327F RID: 12927 RVA: 0x001117C2 File Offset: 0x0010F9C2
		public void RemovePlannedPolicy(PolicyOptionWithTarget policy)
		{
			this.plannedPolicies.Remove(policy);
		}

		// Token: 0x06003280 RID: 12928 RVA: 0x001117D1 File Offset: 0x0010F9D1
		public void ClearPlannedPolicies()
		{
			this.plannedPolicies.Clear();
		}

		// Token: 0x06003281 RID: 12929 RVA: 0x001117DE File Offset: 0x0010F9DE
		public List<TIPriorityPresetTemplate> ValidPresetsForFaction()
		{
			return (from x in TemplateManager.IterateByClass<TIPriorityPresetTemplate>(true)
				where x.ValidPresetForFaction(this)
				select x).ToList<TIPriorityPresetTemplate>();
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x001117FC File Offset: 0x0010F9FC
		public void SetDefaultPreset(string presetName)
		{
			this.defaultPriorityPresetTemplateName = presetName;
			this.defaultPriorityPreset = TemplateManager.Find<TIPriorityPresetTemplate>(presetName, false);
		}

		// Token: 0x06003283 RID: 12931 RVA: 0x00111812 File Offset: 0x0010FA12
		public void SaveCustomPresetDesign(TIPriorityPresetTemplate priorityPreset)
		{
			TemplateManager.Add(priorityPreset, typeof(TIPriorityPresetTemplate), false);
			this.customPresets.Add(priorityPreset);
			GameControl.eventManager.TriggerEvent(new CustomPriorityPresetsChanged(this), null, Array.Empty<object>());
		}

		// Token: 0x06003284 RID: 12932 RVA: 0x00111847 File Offset: 0x0010FA47
		public void DeleteCustomPresetDesign(TIPriorityPresetTemplate priorityPreset)
		{
			if (priorityPreset.customDesign)
			{
				priorityPreset.factionName = string.Empty;
				this.customPresets.Remove(priorityPreset);
				priorityPreset.deleted = true;
			}
		}

		// Token: 0x06003285 RID: 12933 RVA: 0x00111870 File Offset: 0x0010FA70
		public float GetAverageNationPriorityFraction(PriorityType nationPriority)
		{
			if (this.averagePriorityFractionsCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedAverageNationPriorityFractions.Clear();
				this.averagePriorityFractionsCachedFrame = TIFrameCounter.FrameCount;
			}
			float num;
			if (this.cachedAverageNationPriorityFractions.TryGetValue(nationPriority, out num))
			{
				return num;
			}
			float num2 = this.controlPoints.Sum<TIControlPoint>((TIControlPoint x) => x.nation.GetInvestmentFromControlPoint());
			float num3 = this.controlPoints.Sum<TIControlPoint>((TIControlPoint x) => x.nation.GetInvestmentFromControlPoint() * (float)x.GetControlPointPriority(nationPriority, true) / (float)x.totalWeightsForControlPoint);
			num = (this.cachedAverageNationPriorityFractions[nationPriority] = num3 / num2);
			return num;
		}

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06003286 RID: 12934 RVA: 0x0011191E File Offset: 0x0010FB1E
		public int maxCouncilSize
		{
			get
			{
				return Mathf.Min(6, 4 + (int)TIEffectsState.SumEffectsModifiers(Context.CouncilSize, this, 4f, null));
			}
		}

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06003287 RID: 12935 RVA: 0x00111937 File Offset: 0x0010FB37
		public int emptyCouncilorSlots
		{
			get
			{
				return this.maxCouncilSize - this.councilors.Count;
			}
		}

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06003288 RID: 12936 RVA: 0x0011194B File Offset: 0x0010FB4B
		public List<TICouncilorState> CouncilorsOnEarth
		{
			get
			{
				return this.councilors.Where<TICouncilorState>((TICouncilorState x) => x.OnEarth).ToList<TICouncilorState>();
			}
		}

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06003289 RID: 12937 RVA: 0x0011197C File Offset: 0x0010FB7C
		public TICouncilorState FirstCouncilorAvailableForMissionAssignment
		{
			get
			{
				foreach (TICouncilorState ticouncilorState in this.councilors)
				{
					if (ticouncilorState.active && !ticouncilorState.HasMission && ticouncilorState.GetPossibleMissionList(false, false, true, null, false).Count > 0)
					{
						return ticouncilorState;
					}
				}
				return null;
			}
		}

		// Token: 0x0600328A RID: 12938 RVA: 0x001119F4 File Offset: 0x0010FBF4
		public List<TICouncilorState> AvailableCouncilorsWithMission(TIMissionTemplate mission)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				if (ticouncilorState.active && !ticouncilorState.HasMission && ticouncilorState.GetPossibleMissionList(false, false, true, null, false).Contains(mission))
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x0600328B RID: 12939 RVA: 0x00111A74 File Offset: 0x0010FC74
		public TICouncilorState GetBestCouncilorForJob(TIMissionTemplate mission, List<TICouncilorState> availableCouncilors)
		{
			TICouncilorState ticouncilorState = null;
			List<TICouncilorState> list = this.AvailableCouncilorsWithMission(mission).Intersect<TICouncilorState>(availableCouncilors).ToList<TICouncilorState>();
			CouncilorAttribute primaryAttackerStat = mission.primaryAttackerStat;
			if (primaryAttackerStat != CouncilorAttribute.None)
			{
				float num = 0f;
				using (List<TICouncilorState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TICouncilorState ticouncilorState2 = enumerator.Current;
						float num2 = (float)ticouncilorState2.GetAttribute(primaryAttackerStat, true, true, true, false, false, false);
						if (num2 > num)
						{
							ticouncilorState = ticouncilorState2;
							num = num2;
						}
					}
					return ticouncilorState;
				}
			}
			ticouncilorState = list.SelectRandomItem<TICouncilorState>();
			return ticouncilorState;
		}

		// Token: 0x0600328C RID: 12940 RVA: 0x00111B08 File Offset: 0x0010FD08
		public void BeginIntelSharingWith(TIFactionState faction)
		{
			faction.intelSharingFactions.AddUnique(this);
			this.GiveIntelToFaction(faction, false);
		}

		// Token: 0x0600328D RID: 12941 RVA: 0x00111B1F File Offset: 0x0010FD1F
		public void EndIntelSharingWith(TIFactionState faction)
		{
			faction.intelSharingFactions.Remove(this);
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x0600328E RID: 12942 RVA: 0x00111B30 File Offset: 0x0010FD30
		public List<TIFactionState> factionsCompromisingThisFaction
		{
			get
			{
				List<TIFactionState> list = new List<TIFactionState>(from x in GameStateManager.AllFactions()
					where x.intelSharingFactions.Contains(this)
					select x);
				foreach (TICouncilorState ticouncilorState in this.councilors)
				{
					if (ticouncilorState.turned)
					{
						list.AddUnique(ticouncilorState.agentForFaction);
					}
				}
				return list;
			}
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x0600328F RID: 12943 RVA: 0x00111BB0 File Offset: 0x0010FDB0
		public List<TIFactionState> factionsCompromised
		{
			get
			{
				List<TIFactionState> list = new List<TIFactionState>(this.intelSharingFactions);
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					if (tifactionState.factionsCompromisingThisFaction.Contains(this))
					{
						list.AddUnique(tifactionState);
					}
				}
				return list;
			}
		}

		// Token: 0x06003290 RID: 12944 RVA: 0x00111BF8 File Offset: 0x0010FDF8
		public float Suspicion(TICouncilorState councilor)
		{
			if (this.internalCouncilorSuspicion.ContainsKey(councilor))
			{
				return this.internalCouncilorSuspicion[councilor];
			}
			return 0f;
		}

		// Token: 0x06003291 RID: 12945 RVA: 0x00111C1C File Offset: 0x0010FE1C
		public bool AI_SuspectTurned(TICouncilorState councilor)
		{
			if (this.GetViewofCouncilor(councilor).turned)
			{
				return true;
			}
			if (this.HasIntelOnCouncilorSecrets(councilor))
			{
				return false;
			}
			double num = TITimeState.Now().DifferenceInDays(councilor.recruitDate) / 365.2421875;
			return (double)this.Suspicion(councilor) >= 30.0 + num;
		}

		// Token: 0x06003292 RID: 12946 RVA: 0x00111C7A File Offset: 0x0010FE7A
		public void SetSuspicion(TICouncilorState councilor, float value)
		{
			if (!this.IsAlienFaction)
			{
				if (!this.internalCouncilorSuspicion.ContainsKey(councilor))
				{
					this.internalCouncilorSuspicion.Add(councilor, value);
					return;
				}
				this.internalCouncilorSuspicion[councilor] = value;
			}
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x00111CB0 File Offset: 0x0010FEB0
		public void ChangeSuspicion(TICouncilorState councilor, float delta)
		{
			if (!this.IsAlienFaction)
			{
				if (!this.internalCouncilorSuspicion.ContainsKey(councilor))
				{
					this.internalCouncilorSuspicion.Add(councilor, 0f);
				}
				Dictionary<TICouncilorState, float> dictionary = this.internalCouncilorSuspicion;
				dictionary[councilor] += delta;
				if (this.internalCouncilorSuspicion[councilor] < 0f)
				{
					this.internalCouncilorSuspicion[councilor] = 0f;
				}
			}
		}

		// Token: 0x06003294 RID: 12948 RVA: 0x00111D24 File Offset: 0x0010FF24
		public void AddSuspicionForFailure(MissionResult missionResult)
		{
			if (!missionResult.councilor.faction.HasIntelOnCouncilorSecrets(missionResult.councilor))
			{
				CouncilorView viewofCouncilor = missionResult.councilor.faction.GetViewofCouncilor(missionResult.councilor);
				this.ChangeSuspicion(missionResult.councilor, 1f / Mathf.Max(1f - missionResult.successChance, 1E-05f) / Mathf.Max(1f, viewofCouncilor.GetAttribute(CouncilorAttribute.Loyalty)));
			}
		}

		// Token: 0x06003295 RID: 12949 RVA: 0x00111D9C File Offset: 0x0010FF9C
		public void AddSuspicionForMajorReversal(float multiplier, TICouncilorState targetedCouncilor)
		{
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				if (!this.HasIntelOnCouncilorSecrets(ticouncilorState))
				{
					this.ChangeSuspicion(ticouncilorState, multiplier / Mathf.Max(1f, this.GetViewofCouncilor(ticouncilorState).GetAttribute(CouncilorAttribute.Loyalty)));
				}
			}
			this.thisTurnsReveralScore += multiplier;
			if (this.thisTurnsReveralScore > 25f * this.aiValues.riskAversion)
			{
				using (List<TICouncilorState>.Enumerator enumerator = this.councilors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TICouncilorState ticouncilorState2 = enumerator.Current;
						ticouncilorState2.imBeingTargeted = true;
					}
					goto IL_00C6;
				}
			}
			if (targetedCouncilor != null)
			{
				targetedCouncilor.imBeingTargeted = true;
			}
			IL_00C6:
			if (!this.crazyIvan && !this.IsAlienFaction)
			{
				bool flag = false;
				foreach (TICouncilorState ticouncilorState3 in this.councilors)
				{
					if (ticouncilorState3.imBeingTargeted && ticouncilorState3.targetedLastTurn)
					{
						if (flag)
						{
							this.crazyIvan = true;
						}
						else
						{
							flag = true;
						}
					}
				}
			}
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x00111EFC File Offset: 0x001100FC
		public void ClearScrambleValues()
		{
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				ticouncilorState.targetedLastTurn = ticouncilorState.imBeingTargeted;
				ticouncilorState.imBeingTargeted = false;
			}
			this.thisTurnsReveralScore = 0f;
			this.crazyIvan = false;
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x00111F6C File Offset: 0x0011016C
		public bool ShouldTryToRestoreCouncilorLoyalty(TICouncilorState turnedCouncilor)
		{
			return this.councilors.Where<TICouncilorState>((TICouncilorState x) => x != turnedCouncilor).Any<TICouncilorState>((TICouncilorState x) => this.WorthTryingToUnturnCouncilor(turnedCouncilor, x, -1));
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x00111FB8 File Offset: 0x001101B8
		public bool WorthTryingToUnturnCouncilor(TICouncilorState turnedCouncilor, TICouncilorState inspirer, int sliderSteps = -1)
		{
			if (Enums.CouncilorAttributes.None<CouncilorAttribute>((CouncilorAttribute x) => turnedCouncilor.GetAttribute(x, true, true, true, false, false, false) >= 10))
			{
				return false;
			}
			float num = 0f;
			if (TIFactionState.inspireMission.hasCost)
			{
				if (sliderSteps < 0)
				{
					sliderSteps = Mathf.Min(7, inspirer.CurrentMaxSliderSteps(TIFactionState.inspireMission, 1f));
				}
				num = TIFactionState.inspireMission.cost.GetCost((float)sliderSteps, inspirer, null);
			}
			return TIFactionState.inspireMission.resolutionMethod.GetSuccessChance(TIFactionState.inspireMission, inspirer, turnedCouncilor, num, false) > 0.2f;
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x00112054 File Offset: 0x00110254
		public float GetAggregateStat(CouncilorAttribute attribute, bool includeDetained, TIGameState requiredSupraLocation = null)
		{
			float num = (float)this.GetTotalStat(attribute, includeDetained, requiredSupraLocation);
			if (num > 0f)
			{
				return num / 6f;
			}
			if (this.numActiveCouncilors <= 0)
			{
				return 0f;
			}
			return num / (float)this.numActiveCouncilors;
		}

		// Token: 0x0600329A RID: 12954 RVA: 0x00112094 File Offset: 0x00110294
		public void SetCouncilStatsDirty()
		{
			this.cachedTotalStats.Clear();
		}

		// Token: 0x0600329B RID: 12955 RVA: 0x001120A4 File Offset: 0x001102A4
		public int GetTotalStat(CouncilorAttribute attribute, bool includeDetained, TIGameState requiredSupraLocation = null)
		{
			bool flag = !includeDetained && requiredSupraLocation == null;
			int num;
			if (flag && this.cachedTotalStats.TryGetValue(attribute, out num))
			{
				return num;
			}
			int num2 = 0;
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				if (ticouncilorState.status == CouncilorStatus.Active && (includeDetained || !ticouncilorState.detained) && (requiredSupraLocation == null || TIUtilities.ObjectToSupraLocation(ticouncilorState) == requiredSupraLocation))
				{
					num2 += ticouncilorState.GetAttribute(attribute, true, true, true, false, false, false);
				}
			}
			int num3 = ((num2 > 0) ? num2 : 0);
			if (flag)
			{
				this.cachedTotalStats[attribute] = num3;
			}
			return num3;
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x00112170 File Offset: 0x00110370
		public float GetMaxCouncilorStat(CouncilorAttribute attribute, bool includeDetained, TIGameState requiredSupraLocation = null)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				if (ticouncilorState.status == CouncilorStatus.Active && (includeDetained || !ticouncilorState.detained) && (requiredSupraLocation == null || TIUtilities.ObjectToSupraLocation(ticouncilorState) == requiredSupraLocation))
				{
					int attribute2 = ticouncilorState.GetAttribute(attribute, true, true, true, false, false, false);
					if ((float)attribute2 > num)
					{
						num = (float)attribute2;
					}
				}
			}
			return num;
		}

		// Token: 0x0600329D RID: 12957 RVA: 0x00112204 File Offset: 0x00110404
		public bool CouncilHasTrait(TITraitTemplate trait, bool includeDetained, TIGameState requiredSupraLocation = null)
		{
			foreach (TICouncilorState ticouncilorState in this.activeCouncilors)
			{
				if ((includeDetained || !ticouncilorState.detained) && (requiredSupraLocation == null || TIUtilities.ObjectToSupraLocation(ticouncilorState) == requiredSupraLocation) && ticouncilorState.traits.Contains(trait))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600329E RID: 12958 RVA: 0x00112288 File Offset: 0x00110488
		public void GrantNewOrgToCouncilor(TICouncilorState councilor, string orgDataName)
		{
			TIOrgState tiorgState = TIFactionState.CreateNewOrg(orgDataName);
			tiorgState.SetFactionOrbit(this);
			if (councilor != null)
			{
				councilor.AddOrg(tiorgState);
			}
		}

		// Token: 0x0600329F RID: 12959 RVA: 0x001122B0 File Offset: 0x001104B0
		public void AddAvailableCouncilor(TICouncilorState councilor, bool forced = false)
		{
			councilor.SetFaction(this);
			TIDateTime tidateTime = null;
			if (councilor.recruitDate != null)
			{
				tidateTime = new TIDateTime(councilor.recruitDate);
			}
			councilor.SetRecruitDate();
			this.councilors.Add(councilor);
			this.availableCouncilors.Remove(councilor);
			if (this.IsAlienFaction)
			{
				this.SetIntel(councilor, TemplateManager.global.intelToSeeCouncilorSecrets, null, false);
				if (!councilor.OnEarth || TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, this))
				{
					this.GrantNewOrgToCouncilor(councilor, TemplateManager.global.alienShockTroopOrgDataName);
					int num = this.councilors.Count<TICouncilorState>((TICouncilorState x) => x.OnEarth);
					if (!councilor.OnEarth)
					{
						int num2 = 18;
						int num3 = Mathf.Min(20 - num, num2);
						FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = new FactionGoal_TransportCouncilorsWithFleet(this, num3, new List<TICouncilorState> { councilor }, AIEvaluators.SelectAlienCrashdownRegion(true, false));
						this.AddGoal(factionGoal_TransportCouncilorsWithFleet, HandleDuplicateGoalRule.Ignore, null);
					}
				}
			}
			else
			{
				this.SetIntel(councilor, TemplateManager.global.intelToSeeCouncilorMission, null, false);
			}
			councilor.SelectVoice();
			if (!forced)
			{
				councilor.HireRecruitCost(this).PayCost(this, "Hire Councilor");
				switch (this.councilors.Count)
				{
				case 3:
					this.CompleteMilestone(CampaignMilestone.TutorialRecruitCouncilor3);
					break;
				case 4:
					this.CompleteMilestone(CampaignMilestone.TutorialRecruitCouncilor4);
					break;
				case 5:
					this.CompleteMilestone(CampaignMilestone.TutorialBuildCouncil);
					break;
				}
			}
			TINotificationQueueState.AddCouncilorMessage(councilor, CouncilorChatType.NewCouncilor, councilor.faction);
			this.SetResourceIncomeDataDirty(TIFactionState.councilorResources);
			councilor.RecordLocation();
			if (councilor.XP == 0 && !forced && tidateTime == null)
			{
				if (this.IsActiveHumanFaction)
				{
					councilor.ChangeXP(Mathf.Max(0, TemplateManager.global.initialXPPerYearAge * (councilor.age - TemplateManager.global.minAgeForXPBonus)));
				}
				councilor.ChangeXP((int)TIEffectsState.SumEffectsModifiers(Context.NewCouncilorRecruitXP, this, (float)councilor.XP, null));
			}
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(this, councilor, councilor.location, true), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new CouncilorPositionUpdated(councilor, councilor.location), null, (from x in new object[]
				{
					this,
					councilor,
					councilor.location,
					councilor.location.ref_nation,
					councilor.location.ref_fleet,
					councilor.location.ref_spaceBody
				}.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			if (this.isActivePlayer && this.councilors.Count >= 6)
			{
				this.UnlockAchievement("recruitFullCouncil");
				if (this.turnedCouncilors.Count == 2)
				{
					this.UnlockAchievement("controlFullCouncilTurned");
				}
			}
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x00112574 File Offset: 0x00110774
		public void DismissCouncilor(TICouncilorState councilor, TIFactionState dismissingFaction)
		{
			if (!councilor.turned)
			{
				if (councilor.detained && councilor.detainingFaction != councilor.faction)
				{
					if (TIUtilities.RandomFloatValue() * 100f > (float)(councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false) * 5))
					{
						councilor.detainingFaction.GainIntel(councilor.faction, TIUtilities.RandomFloatValue() / 2f, null, false);
					}
					TINotificationQueueState.LogDetainedCouncilorDismissed(councilor.detainingFaction, councilor);
				}
				councilor.Retire();
				return;
			}
			TIMissionState activeMission = councilor.activeMission;
			if (activeMission != null)
			{
				activeMission.ResolveMission(TIMissionState.AbortReason.TurnedCouncilorQuit, "");
			}
			councilor.RemoveFromGoals();
			this.councilors.Remove(councilor);
			TIHabState ref_hab = councilor.ref_hab;
			if (ref_hab != null)
			{
				ref_hab.RemoveAdvisingCouncilor(councilor);
			}
			TINationState ref_nation = councilor.ref_nation;
			if (ref_nation != null)
			{
				ref_nation.RemoveAdvisingCouncilor(councilor);
			}
			councilor.EndProtectionOfTarget();
			councilor.GetProtectors().ToList<TICouncilorState>().ForEach(delegate(TICouncilorState x)
			{
				x.EndProtectionOfTarget();
			});
			this.ValidateAllOrgs(false);
			if (councilor.agentForFaction != GameStateManager.AlienFaction())
			{
				if (dismissingFaction == councilor.agentForFaction)
				{
					EventManager eventManager = GameControl.eventManager;
					GameEvent gameEvent = new CouncilorDepartsRegion(councilor, councilor.location.ref_region);
					string text = null;
					object[] array = new TIGameState[] { councilor, councilor.ref_region, councilor.ref_nation };
					eventManager.TriggerEvent(gameEvent, text, array);
					EventManager eventManager2 = GameControl.eventManager;
					GameEvent gameEvent2 = new CouncilorDepartsRegion(councilor, councilor.priorLocation.ref_region);
					string text2 = null;
					array = new TIGameState[]
					{
						councilor,
						councilor.priorLocation.ref_region,
						councilor.priorLocation.ref_nation
					};
					eventManager2.TriggerEvent(gameEvent2, text2, array);
				}
				councilor.agentForFaction.availableCouncilors.Insert(0, councilor);
				councilor.agentForFaction.newAvailableCouncilors.Add(councilor);
				councilor.RemoveTrait(TemplateManager.Find<TITraitTemplate>("Extorted", false));
				councilor.UnTurnCouncilor(dismissingFaction == councilor.agentForFaction, false);
				if (dismissingFaction == councilor.agentForFaction)
				{
					TINotificationQueueState.LogInvoluntaryCouncilorDismissal(councilor, councilor.faction);
				}
			}
			if (councilor.detained)
			{
				World.Active.GetExistingManager<GameTimeManager>().CancelTimeEvent(councilor.ReleaseDetailedCouncilorEventName, null, null, null, councilor.detainedReleaseDate);
				if (councilor.detainingFaction != councilor.faction)
				{
					if (TIUtilities.RandomFloatValue() * 100f > (float)(councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false) * 5))
					{
						councilor.detainingFaction.GainIntel(councilor.faction, TIUtilities.RandomFloatValue() / 2f, null, false);
					}
					if (councilor.isHuman)
					{
						TINotificationQueueState.LogDetainedCouncilorDismissed(councilor.detainingFaction, councilor);
					}
				}
				councilor.detainingFaction = null;
			}
			TIGameState location = councilor.location;
			councilor.RemoveFromCurrentLocation();
			councilor.SetFaction(null);
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(this, councilor, location, false), null, Array.Empty<object>());
			this.SetResourceIncomeDataDirty(TIFactionState.councilorResources);
		}

		// Token: 0x060032A1 RID: 12961 RVA: 0x0011284C File Offset: 0x00110A4C
		public TICouncilorState GetNextCouncilor(TICouncilorState councilor, bool useFinderSortIndex = false)
		{
			int num;
			if (useFinderSortIndex)
			{
				new List<TICouncilorState>();
				List<TICouncilorState> list = this.councilors.OrderBy<TICouncilorState, int>((TICouncilorState x) => x.finderSortOverride).ToList<TICouncilorState>();
				num = list.IndexOf(councilor);
				if (num >= this.councilors.Count - 1)
				{
					num = 0;
				}
				else
				{
					num++;
				}
				return list[num];
			}
			num = this.councilors.FindIndex((TICouncilorState c) => c.ID == councilor.ID);
			if (num >= this.councilors.Count - 1)
			{
				num = 0;
			}
			else
			{
				num++;
			}
			return this.councilors[num];
		}

		// Token: 0x060032A2 RID: 12962 RVA: 0x00112908 File Offset: 0x00110B08
		public TICouncilorState GetPreviousCouncilor(TICouncilorState councilor)
		{
			int num = this.councilors.FindIndex((TICouncilorState c) => c.ID == councilor.ID);
			if (num == 0)
			{
				num = this.councilors.Count - 1;
			}
			else
			{
				num--;
			}
			return this.councilors[num];
		}

		// Token: 0x060032A3 RID: 12963 RVA: 0x00112960 File Offset: 0x00110B60
		public bool GenerateRecruitableCouncilors(bool campaignStart = false)
		{
			this.newAvailableCouncilors.Clear();
			bool flag = false;
			if (this.availableCouncilors.Count > 1 && !campaignStart && this.IsActiveHumanFaction)
			{
				for (int i = this.availableCouncilors.Count - 1; i >= 0; i--)
				{
					if (TIUtilities.RandomFloatValue() * 100f < (float)this.availableCouncilors[i].age)
					{
						TICouncilorState ticouncilorState = this.availableCouncilors[i];
						this.availableCouncilors.Remove(ticouncilorState);
						if (ticouncilorState.template.randomized)
						{
							TIGlobalValuesState.GlobalValues.councilorAppearanceTemplatesInUse.Remove(ticouncilorState.appearanceTemplateName);
							ticouncilorState.ArchiveState(true);
							GameStateManager.RemoveGameState<TICouncilorState>(ticouncilorState.ID, false);
						}
					}
				}
			}
			if (TemplateManager.global.maxFactionCouncilorCandidatePool > 0)
			{
				int num = (this.IsActiveHumanFaction ? TIUtilities.RandomRange(-TemplateManager.global.maxFactionCouncilorCandidatePoolVariance, TemplateManager.global.maxFactionCouncilorCandidatePoolVariance) : 0);
				for (int j = this.availableCouncilors.Count; j <= TemplateManager.global.maxFactionCouncilorCandidatePool + num; j++)
				{
					List<TICouncilorState> list = new List<TICouncilorState>();
					foreach (TICouncilorState ticouncilorState2 in GameStateManager.IterateByClass<TICouncilorState>(false))
					{
						if (!ticouncilorState2.everBeenAvailable && !ticouncilorState2.template.debugOnly && string.IsNullOrEmpty(ticouncilorState2.template.debugStartingCouncil) && !ticouncilorState2.template.randomized && ticouncilorState2.age >= 18 && ticouncilorState2.age <= 85 && ticouncilorState2.template.allowedIdeologies.Contains(this.ideology.ideology))
						{
							list.Add(ticouncilorState2);
						}
					}
					if (TIUtilities.RandomFloatValue() > TemplateManager.global.chanceCouncilorTemplate || list.Count == 0)
					{
						TICouncilorState ticouncilorState3 = GameStateManager.CreateNewGameState<TICouncilorState>();
						if (this.IsAlienFaction)
						{
							ticouncilorState3.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>("randomizedAlienCouncilor2", false));
						}
						else
						{
							ticouncilorState3.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>("randomizedCouncilor1", false));
						}
						if (this.availableCouncilors.None<TICouncilorState>((TICouncilorState x) => x.HireRecruitCost(this).CanAfford(this, 1f, null, float.PositiveInfinity)) && this.availableCouncilors.Count<TICouncilorState>((TICouncilorState x) => x.typeTemplate.affinities.Contains(this.ideology.ideology)) < 2)
						{
							IEnumerable<TICouncilorTypeTemplate> enumerable = from x in TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)
								where x.affinities.Contains(this.ideology.ideology)
								select x;
							if (enumerable.Count<TICouncilorTypeTemplate>() > 0)
							{
								ticouncilorState3.NewCharacterGeneration(enumerable.SelectRandomItem<TICouncilorTypeTemplate>(), null, this.IsAlienFaction ? null : this, false, false);
							}
						}
						else
						{
							ticouncilorState3.NewCharacterGeneration(null, null, this.IsAlienFaction ? null : this, false, false);
						}
						this.availableCouncilors.Insert(0, ticouncilorState3);
						this.newAvailableCouncilors.Add(ticouncilorState3);
						flag = true;
					}
					else
					{
						int num2 = TIUtilities.RandomRange(0, list.Count);
						this.availableCouncilors.Insert(0, list[num2]);
						this.newAvailableCouncilors.Add(list[num2]);
						flag = true;
						list[num2].everBeenAvailable = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x060032A4 RID: 12964 RVA: 0x00112C8C File Offset: 0x00110E8C
		public List<MissionOption> GetMissionOptionsForTarget(TIGameState target)
		{
			List<MissionOption> list = new List<MissionOption>();
			foreach (TICouncilorState ticouncilorState in this.activeCouncilors)
			{
				list.AddRange(ticouncilorState.MissionOptionsForTarget(target));
			}
			list = (from x in list
				orderby x.mission.sortOrder, x.baseChance descending
				select x).ToList<MissionOption>();
			return list;
		}

		// Token: 0x060032A5 RID: 12965 RVA: 0x00112D3C File Offset: 0x00110F3C
		public IEnumerable<TIMissionTemplate> GetAllPossibleMissions()
		{
			return this.councilors.SelectMany<TICouncilorState, TIMissionTemplate>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false)).Distinct<TIMissionTemplate>();
		}

		// Token: 0x060032A6 RID: 12966 RVA: 0x00112D6D File Offset: 0x00110F6D
		public static TIOrgState CreateNewOrg(TIOrgTemplate newOrgTemplate)
		{
			TIOrgState tiorgState = GameStateManager.CreateNewGameState<TIOrgState>();
			tiorgState.InitWithTemplate(newOrgTemplate);
			tiorgState.PostGameStateCreateInit_OnCreationOnly_1();
			tiorgState.InitRunTimeValues();
			return tiorgState;
		}

		// Token: 0x060032A7 RID: 12967 RVA: 0x00112D88 File Offset: 0x00110F88
		public static TIOrgState CreateNewOrg(string orgDataName)
		{
			TIOrgTemplate tiorgTemplate = TemplateManager.Find<TIOrgTemplate>(orgDataName, false);
			if (tiorgTemplate == null)
			{
				Log.Error("Bad orgDataName " + orgDataName + " in CreateNewOrg", Array.Empty<object>());
				return null;
			}
			return TIFactionState.CreateNewOrg(tiorgTemplate);
		}

		// Token: 0x060032A8 RID: 12968 RVA: 0x00112DC4 File Offset: 0x00110FC4
		public bool CouncilHasOrg(TIOrgTemplate orgTemplate, bool includeDetained)
		{
			foreach (TICouncilorState ticouncilorState in this.activeCouncilors)
			{
				if ((includeDetained || !ticouncilorState.detained) && ticouncilorState.HasOrg(orgTemplate))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060032A9 RID: 12969 RVA: 0x00112E2C File Offset: 0x0011102C
		public List<TIOrgState> ValidateAllOrgs(bool suppressReporting)
		{
			TIFactionState.<>c__DisplayClass619_0 CS$<>8__locals1;
			CS$<>8__locals1.badOrgs = new List<TIOrgState>();
			new List<int>();
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				TIFactionState.<>c__DisplayClass619_1 CS$<>8__locals2;
				CS$<>8__locals2.availableAdministration = ticouncilorState.availableAdministration;
				using (List<TIOrgState>.Enumerator enumerator2 = ticouncilorState.orgs.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIOrgState tiorgState = enumerator2.Current;
						if (!tiorgState.IsEligibleForCouncilor(ticouncilorState))
						{
							TIFactionState.<ValidateAllOrgs>g__AddBadOrg|619_3(tiorgState, ref CS$<>8__locals1, ref CS$<>8__locals2);
						}
					}
					goto IL_008F;
				}
				goto IL_007E;
				IL_008F:
				if (CS$<>8__locals2.availableAdministration >= 0 || !ticouncilorState.orgs.Except<TIOrgState>(CS$<>8__locals1.badOrgs).Any<TIOrgState>())
				{
					continue;
				}
				IL_007E:
				TIFactionState.<ValidateAllOrgs>g__AddBadOrg|619_3(TIFactionState.<ValidateAllOrgs>g__GetOrgToRemoveToRelieveAdministrationDeficit|619_0(ticouncilorState, ref CS$<>8__locals1), ref CS$<>8__locals1, ref CS$<>8__locals2);
				goto IL_008F;
			}
			foreach (TIOrgState tiorgState2 in CS$<>8__locals1.badOrgs)
			{
				this.AddOrgToFactionPool(tiorgState2, tiorgState2.assignedCouncilor, tiorgState2 != CS$<>8__locals1.badOrgs.Last<TIOrgState>());
			}
			if (!suppressReporting && CS$<>8__locals1.badOrgs.Count > 0)
			{
				TINotificationQueueState.LogOrgsForcedToPool(this, CS$<>8__locals1.badOrgs);
			}
			return CS$<>8__locals1.badOrgs;
		}

		// Token: 0x060032AA RID: 12970 RVA: 0x00112FA4 File Offset: 0x001111A4
		public void AddAvailableOrg(TIOrgState newOrg, bool newToFaction = true)
		{
			if (newToFaction)
			{
				this.availableOrgs.Insert(0, newOrg);
				this.newAvailableOrgs.AddUnique(newOrg);
				newOrg.SetFactionOrbit(this);
				return;
			}
			this.availableOrgs.Add(newOrg);
		}

		// Token: 0x060032AB RID: 12971 RVA: 0x00112FD8 File Offset: 0x001111D8
		public bool GenerateOrgsForAcquisition(bool campaignStart = false)
		{
			TIFactionState.<>c__DisplayClass621_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.newAvailable = false;
			this.newAvailableOrgs.Clear();
			if (campaignStart)
			{
				this.<GenerateOrgsForAcquisition>g__CreateNewOrgs|621_0(50 / GameStateManager.AllHumanFactions().Length, ref CS$<>8__locals1);
			}
			List<TIOrgState> list = new List<TIOrgState>();
			if (this.availableOrgs.Count > 1 && !campaignStart)
			{
				for (int i = this.availableOrgs.Count - 1; i >= 0; i--)
				{
					if ((float)Mathd.d100() < 100f - (float)this.availableOrgs[i].tier * 25f)
					{
						this.availableOrgs[i].ClearFactionOrbit();
						list.Add(this.availableOrgs[i]);
						this.availableOrgs.Remove(this.availableOrgs[i]);
					}
				}
			}
			int num = TemplateManager.global.initialMaxOrgsAvailableToCouncil + (int)TIEffectsState.SumEffectsModifiers(Context.MaxAvailableOrgs, this, (float)TemplateManager.global.initialMaxOrgsAvailableToCouncil, null) + TIUtilities.RandomRange(0, 6);
			foreach (TIOrgState tiorgState in GameStateManager.IterateByClass<TIOrgState>(false).ToList<TIOrgState>().Shuffle<TIOrgState>())
			{
				if (tiorgState.AllowedOnFactionMarket(this) && tiorgState.factionOrbit == null && !list.Contains(tiorgState))
				{
					if (tiorgState.restrictiveOwnership)
					{
						if (this.availableOrgs.Count<TIOrgState>((TIOrgState x) => x.restrictiveOwnership) > num / 3)
						{
							continue;
						}
					}
					if (!campaignStart || tiorgState.tier <= 2)
					{
						float num2 = 5f * tiorgState.AvailabilityModifier(this);
						if ((float)Mathd.d100() <= num2)
						{
							this.AddAvailableOrg(tiorgState, true);
							CS$<>8__locals1.newAvailable = true;
						}
						if (this.availableOrgs.Count > num)
						{
							break;
						}
					}
				}
			}
			if (this.availableOrgs.Count < num)
			{
				this.<GenerateOrgsForAcquisition>g__CreateNewOrgs|621_0(num - this.availableOrgs.Count, ref CS$<>8__locals1);
			}
			GameControl.eventManager.TriggerEvent(new CouncilOrgsChanged(this), null, new object[] { this });
			return CS$<>8__locals1.newAvailable;
		}

		// Token: 0x060032AC RID: 12972 RVA: 0x00113210 File Offset: 0x00111410
		public void CachePriorityBonuses_Day()
		{
			foreach (PriorityType priorityType in Enums.PriorityTypes)
			{
				this.cachedPriorityBonuses[priorityType] = this.SumPriorityBonuses(priorityType, false);
			}
		}

		// Token: 0x060032AD RID: 12973 RVA: 0x0011324C File Offset: 0x0011144C
		public float SumLEOHabPriorityBonuses(PriorityType priority, bool includeNonActive = false, float extra = 0f)
		{
			if (this.LEOHabPriorityBonusesCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedLEOHabPriorityBonuses.Clear();
				this.cachedLEOHabPriorityBonuses_IncludeNonActive.Clear();
				this.LEOHabPriorityBonusesCachedFrame = TIFrameCounter.FrameCount;
			}
			float num;
			bool flag;
			if (includeNonActive)
			{
				flag = this.cachedLEOHabPriorityBonuses_IncludeNonActive.TryGetValue(priority, out num);
			}
			else
			{
				flag = this.cachedLEOHabPriorityBonuses.TryGetValue(priority, out num);
			}
			if (!flag)
			{
				foreach (TIHabState tihabState in this.LEOStations)
				{
					switch (priority)
					{
					case PriorityType.Economy:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusEconomy, includeNonActive);
						break;
					case PriorityType.Welfare:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusWelfare, includeNonActive);
						break;
					case PriorityType.Environment:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusEnvironment, includeNonActive);
						break;
					case PriorityType.Knowledge:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusKnowledge, includeNonActive);
						break;
					case PriorityType.Government:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusGovernment, includeNonActive);
						break;
					case PriorityType.Unity:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusUnity, includeNonActive);
						break;
					case PriorityType.Oppression:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusOppression, includeNonActive);
						break;
					case PriorityType.LaunchFacilities:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusLaunchFacilities, includeNonActive);
						break;
					case PriorityType.MissionControl:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusMissionControl, includeNonActive);
						break;
					case PriorityType.Military:
						num += tihabState.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusMiltech, includeNonActive);
						break;
					}
				}
				if (includeNonActive)
				{
					this.cachedLEOHabPriorityBonuses_IncludeNonActive[priority] = num;
				}
				else
				{
					this.cachedLEOHabPriorityBonuses[priority] = num;
				}
			}
			return Mathf.Min(num + extra, TemplateManager.global.LEOHabModulePriorityBonusCap);
		}

		// Token: 0x060032AE RID: 12974 RVA: 0x001133F4 File Offset: 0x001115F4
		public float SumPriorityBonuses(PriorityType priority, bool skipLEO = false)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				foreach (TIOrgState tiorgState in ticouncilorState.activeOrgs)
				{
					switch (priority)
					{
					case PriorityType.Economy:
						num += tiorgState.economyBonus;
						break;
					case PriorityType.Welfare:
						num += tiorgState.welfareBonus;
						break;
					case PriorityType.Environment:
						num += tiorgState.environmentBonus;
						break;
					case PriorityType.Knowledge:
						num += tiorgState.knowledgeBonus;
						break;
					case PriorityType.Government:
						num += tiorgState.governmentBonus;
						break;
					case PriorityType.Unity:
						num += tiorgState.unityBonus;
						break;
					case PriorityType.Oppression:
						num += tiorgState.oppressionBonus;
						break;
					case PriorityType.Funding:
						num += tiorgState.spaceDevBonus;
						break;
					case PriorityType.Spoils:
						num += tiorgState.spoilsBonus;
						break;
					case PriorityType.Civilian_InitiateSpaceflightProgram:
					case PriorityType.LaunchFacilities:
					case PriorityType.Military_BuildSTOSquadron:
						num += tiorgState.spaceflightBonus;
						break;
					case PriorityType.MissionControl:
						num += tiorgState.MCBonus;
						break;
					case PriorityType.Military_FoundMilitary:
					case PriorityType.Military:
					case PriorityType.Military_BuildArmy:
					case PriorityType.Military_BuildNavy:
					case PriorityType.Military_BuildSpaceDefenses:
						num += tiorgState.militaryBonus;
						break;
					}
				}
				foreach (TITraitTemplate titraitTemplate in ticouncilorState.traits)
				{
					foreach (PriorityBonus priorityBonus in titraitTemplate.priorityBonuses)
					{
						if (priorityBonus.priority == priority)
						{
							num += priorityBonus.bonus;
						}
					}
				}
			}
			if (!skipLEO)
			{
				num += this.SumLEOHabPriorityBonuses(priority, false, 0f);
			}
			switch (priority)
			{
			case PriorityType.Economy:
				num += TIEffectsState.SumEffectsModifiers(Context.EconomyPriority, this, num, null);
				break;
			case PriorityType.Welfare:
				num += TIEffectsState.SumEffectsModifiers(Context.WelfarePriority, this, num, null);
				break;
			case PriorityType.Environment:
				num += TIEffectsState.SumEffectsModifiers(Context.EnvironmentPriority, this, num, null);
				break;
			case PriorityType.Knowledge:
				num += TIEffectsState.SumEffectsModifiers(Context.KnowledgePriority, this, num, null);
				break;
			case PriorityType.Government:
				num += TIEffectsState.SumEffectsModifiers(Context.GovernmentPriority, this, num, null);
				break;
			case PriorityType.Unity:
				num += TIEffectsState.SumEffectsModifiers(Context.UnityPriority, this, num, null);
				break;
			case PriorityType.Oppression:
				num += TIEffectsState.SumEffectsModifiers(Context.OppressionPriority, this, num, null);
				break;
			case PriorityType.Funding:
				num += TIEffectsState.SumEffectsModifiers(Context.SpaceDevPriority, this, num, null);
				break;
			case PriorityType.Spoils:
				num += TIEffectsState.SumEffectsModifiers(Context.SpoilsPriority, this, num, null);
				break;
			case PriorityType.Civilian_InitiateSpaceflightProgram:
				num += TIEffectsState.SumEffectsModifiers(Context.SpaceflightPriority, this, num, null);
				break;
			case PriorityType.LaunchFacilities:
				num += TIEffectsState.SumEffectsModifiers(Context.LaunchFacilitiesPriority, this, num, null);
				break;
			case PriorityType.MissionControl:
				num += TIEffectsState.SumEffectsModifiers(Context.MissionControlPriority, this, num, null);
				break;
			case PriorityType.Military:
				num += TIEffectsState.SumEffectsModifiers(Context.MilitaryPriority, this, num, null);
				break;
			case PriorityType.Military_BuildArmy:
				num += TIEffectsState.SumEffectsModifiers(Context.BuildArmyPriority, this, num, null);
				break;
			case PriorityType.Military_BuildNavy:
				num += TIEffectsState.SumEffectsModifiers(Context.UpgradeArmyPriority, this, num, null);
				break;
			case PriorityType.Military_InitiateNuclearProgram:
			case PriorityType.Military_BuildNuclearWeapons:
				num += TIEffectsState.SumEffectsModifiers(Context.BuildNuclearWeaponsPriority, this, num, null);
				break;
			case PriorityType.Military_BuildSpaceDefenses:
				num += TIEffectsState.SumEffectsModifiers(Context.BuildSpaceDefensesPriority, this, num, null);
				break;
			case PriorityType.Military_BuildSTOSquadron:
				num += TIEffectsState.SumEffectsModifiers(Context.BuildSTOSquadronPriority, this, num, null);
				break;
			}
			return num;
		}

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x060032AF RID: 12975 RVA: 0x001137CC File Offset: 0x001119CC
		public int AlienDetectionBonus
		{
			get
			{
				if (this.alienDetectionBonusCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedAlienDetectionBonus = this.LEOStations.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.ActiveModules()).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.AlienDetectionBonus).Round();
					this.cachedAlienDetectionBonus = Mathf.Min(this.cachedAlienDetectionBonus, TemplateManager.global.alienDetectionBonusCapFromLEOHabs.Round());
					this.alienDetectionBonusCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedAlienDetectionBonus;
			}
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x060032B0 RID: 12976 RVA: 0x00113874 File Offset: 0x00111A74
		public int HumanDetectionBonus
		{
			get
			{
				if (this.HumanDetectionBonusCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedHumanDetectionBonus = this.LEOStations.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.ActiveModules()).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.HumanDetectionBonus).Round();
					this.cachedHumanDetectionBonus = Mathf.Min(this.cachedHumanDetectionBonus, TemplateManager.global.humanDetectionBonusCapFromLEOHabs.Round());
					this.HumanDetectionBonusCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedHumanDetectionBonus;
			}
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x060032B1 RID: 12977 RVA: 0x0011391C File Offset: 0x00111B1C
		public float ArmyCombatBonus
		{
			get
			{
				if (this.armyCombatBonusCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedArmyCombatBonus = this.LEOStations.Sum<TIHabState>((TIHabState x) => x.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusArmyCombatValue, false));
					this.cachedArmyCombatBonus = Mathf.Min(this.cachedArmyCombatBonus, TemplateManager.global.maxArmyCombatBonusFromLEOHabs);
					this.armyCombatBonusCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedArmyCombatBonus;
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x060032B2 RID: 12978 RVA: 0x00113994 File Offset: 0x00111B94
		public float PropagandaBonus
		{
			get
			{
				if (this.propagandaBonusCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedPropagandaBonus = this.LEOStations.Sum<TIHabState>((TIHabState x) => x.GetLEOLabBonus(HabModuleSpecialRule.LEOBonusPropagandaStrength, false));
					this.cachedPropagandaBonus = Mathf.Min(this.cachedPropagandaBonus, TemplateManager.global.maxLEOHabPropagandaStrengthBonus);
					this.propagandaBonusCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedPropagandaBonus;
			}
		}

		// Token: 0x060032B3 RID: 12979 RVA: 0x00113A0A File Offset: 0x00111C0A
		public bool OwnsOrgInUnassignedPool(TIOrgState org)
		{
			return org.hasFactionbutNoCouncilor && org.unassignedCouncil == this;
		}

		// Token: 0x060032B4 RID: 12980 RVA: 0x00113A22 File Offset: 0x00111C22
		public bool CanPurchaseOrg(TIOrgState org)
		{
			if (this.OwnsOrgInUnassignedPool(org))
			{
				return org.GetTransferCost().CanAfford(this, 1f, null, float.PositiveInfinity);
			}
			return org.GetPurchaseCost(this).CanAfford(this, 1f, null, float.PositiveInfinity);
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x00113A60 File Offset: 0x00111C60
		public void PurchaseOrg(bool unassignedInCouncil, TIOrgState org, TICouncilorState councilor = null, bool straightToPool = false)
		{
			TIResourcesCost tiresourcesCost;
			if (unassignedInCouncil)
			{
				if (councilor == null)
				{
					return;
				}
				tiresourcesCost = org.GetTransferCost();
			}
			else
			{
				tiresourcesCost = org.GetPurchaseCost(this);
			}
			tiresourcesCost.PayCost(this, "Purchase Org");
			if (!straightToPool)
			{
				this.AssignOrgToCouncilor(org, councilor);
			}
			else
			{
				this.AddOrgToFactionPool(org, null, false);
			}
			this.CompleteMilestone(CampaignMilestone.TutorialPurchaseOrg);
		}

		// Token: 0x060032B6 RID: 12982 RVA: 0x00113AB8 File Offset: 0x00111CB8
		public bool CanTransferOrgFromCouncilorToCouncilor(TIOrgState org, TICouncilorState receivingCouncilor, bool checkCost = true)
		{
			return org.hasCouncilor && org.assignedCouncilor.faction == receivingCouncilor.faction && receivingCouncilor.SufficientCapacityForOrg(org) && org.CouncilorCanAcquire(receivingCouncilor) && (!checkCost || org.GetTransferCost().CanAfford(this, 1f, null, float.PositiveInfinity));
		}

		// Token: 0x060032B7 RID: 12983 RVA: 0x00113B15 File Offset: 0x00111D15
		public void TransferOrgToCouncilor(TIOrgState org, TICouncilorState councilorReceiving, TICouncilorState councilorGiving)
		{
			org.GetTransferCost().PayCost(this, "Transfer Org");
			councilorGiving.RemoveOrg(org);
			this.AssignOrgToCouncilor(org, councilorReceiving);
		}

		// Token: 0x060032B8 RID: 12984 RVA: 0x00113B38 File Offset: 0x00111D38
		public void SellOrg(TIOrgState org, TICouncilorState councilor = null)
		{
			if (councilor == null)
			{
				this.RemoveOrgFromUnassignedPool(org);
				this.AddAvailableOrg(org, false);
			}
			else
			{
				councilor.RemoveOrg(org);
				if (org.assignedCouncilor != null)
				{
					global::UnityEngine.Debug.LogWarning("OrgState " + org.ID.ToString() + " exists on another councilor");
				}
				else
				{
					this.AddAvailableOrg(org, false);
				}
			}
			org.GetSalePrice(false).RefundCost(this, "Sell Org");
			this.SetResourceIncomeDataDirty(TIOrgState.orgNegativeResources);
		}

		// Token: 0x060032B9 RID: 12985 RVA: 0x00113BC4 File Offset: 0x00111DC4
		public void LoseOrg(TIOrgState org)
		{
			if (org.hasCouncilor)
			{
				if (org.assignedCouncilor.OrgProvidingActiveMission(org))
				{
					org.assignedCouncilor.activeMission.ResolveMission(TIMissionState.AbortReason.MissionOrgLost, "");
				}
				org.assignedCouncilor.RemoveOrg(org);
				return;
			}
			this.RemoveOrgFromUnassignedPool(org);
		}

		// Token: 0x060032BA RID: 12986 RVA: 0x00113C14 File Offset: 0x00111E14
		public TIOrgState CreateOrTransferOrgToFactionPool(TIOrgTemplate orgTemplate, bool allowTheft = true)
		{
			TIOrgState tiorgState = null;
			if (orgTemplate.randomized)
			{
				tiorgState = TIFactionState.CreateNewOrg(orgTemplate);
				this.AddOrgToFactionPool(tiorgState, null, false);
			}
			else
			{
				TIOrgState tiorgState2 = GameStateManager.FindByTemplate<TIOrgState>(orgTemplate.dataName, false);
				if (tiorgState2 == null)
				{
					tiorgState = TIFactionState.CreateNewOrg(orgTemplate);
					this.AddOrgToFactionPool(tiorgState, null, false);
				}
				else if (tiorgState2.IsEligibleForFaction(this) && tiorgState2.factionOrbit != this)
				{
					if (tiorgState2.factionOrbit != null)
					{
						if (allowTheft)
						{
							TIFactionState factionOrbit = tiorgState2.factionOrbit;
							TICouncilorState assignedCouncilor = tiorgState2.assignedCouncilor;
							factionOrbit.LoseOrg(tiorgState2);
							List<TIOrgState> list = factionOrbit.ValidateAllOrgs(true);
							TINotificationQueueState.LogFactionOrgStolen(this, factionOrbit, tiorgState2, list, assignedCouncilor);
							this.AddOrgToFactionPool(tiorgState2, null, false);
						}
					}
					else
					{
						this.AddOrgToFactionPool(tiorgState2, null, false);
					}
				}
			}
			return tiorgState;
		}

		// Token: 0x060032BB RID: 12987 RVA: 0x00113CD0 File Offset: 0x00111ED0
		public void AddOrgToFactionPool(TIOrgState org, TICouncilorState councilor = null, bool skipOverageCheck = false)
		{
			TIFactionState ref_faction = org.ref_faction;
			TICouncilorState assignedCouncilor = org.assignedCouncilor;
			if (assignedCouncilor != null)
			{
				assignedCouncilor.RemoveOrg(org);
			}
			if (councilor != null && councilor.orgs.Contains(org))
			{
				councilor.RemoveOrg(org);
			}
			else if (this.availableOrgs.Contains(org))
			{
				this.availableOrgs.Remove(org);
			}
			if (ref_faction != null && ref_faction != this && ref_faction.unassignedOrgs.Contains(org))
			{
				ref_faction.RemoveOrgFromUnassignedPool(org);
				org.ClearFactionOrbit();
				Log.Error("Code attempted to add org " + org.displayName + " to another faction pool without removing it first.", Array.Empty<object>());
			}
			if (!this.unassignedOrgs.Contains(org))
			{
				this.unassignedOrgs.Add(org);
				org.SetFactionOrbit(this);
				this.SetResourceIncomeDataDirty(TIOrgState.orgNegativeResources);
				if (!skipOverageCheck && this.UnassignedPoolOverage() > 0 && !TIPromptQueueState.HasPromptStatic(this, this, null, "PromptDropOrgs", 0))
				{
					TINotificationQueueState.LogOrgPoolOverfull(this);
					TIPromptQueueState.AddPromptStatic(this, this, null, "PromptDropOrgs", 0);
				}
			}
		}

		// Token: 0x060032BC RID: 12988 RVA: 0x00113DD3 File Offset: 0x00111FD3
		public void AssignOrgToCouncilor(TIOrgState org, TICouncilorState councilor)
		{
			if (this.availableOrgs.Contains(org))
			{
				this.availableOrgs.Remove(org);
			}
			else if (this.unassignedOrgs.Contains(org))
			{
				this.RemoveOrgFromUnassignedPool(org);
			}
			councilor.AddOrg(org);
		}

		// Token: 0x060032BD RID: 12989 RVA: 0x00113E10 File Offset: 0x00112010
		public int UnassignedPoolOverage()
		{
			return Mathf.Max(this.unassignedOrgs.Count - this.unassignedOrgs.Where<TIOrgState>((TIOrgState x) => !x.template.allowedOnMarket).Count<TIOrgState>() - TemplateManager.global.maxFactionOrgPoolSize, 0);
		}

		// Token: 0x060032BE RID: 12990 RVA: 0x00113E6C File Offset: 0x0011206C
		public void RemoveOrgFromUnassignedPool(TIOrgState org)
		{
			this.unassignedOrgs.Remove(org);
			this.SetResourceIncomeDataDirty(TIOrgState.orgNegativeResources);
			if (TIPromptQueueState.HasPromptStatic(this, this, null, "PromptDropOrgs", 0) && this.UnassignedPoolOverage() <= 0)
			{
				TIPromptQueueState.RemovePromptStatic(this, this, null, "PromptDropOrgs", 0);
			}
		}

		// Token: 0x060032BF RID: 12991 RVA: 0x00113EB8 File Offset: 0x001120B8
		public void ActivateCouncilorOrgs()
		{
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				ticouncilorState.ActivateAllOrgs();
			}
		}

		// Token: 0x060032C0 RID: 12992 RVA: 0x00113F08 File Offset: 0x00112108
		public void DeactivateAllCouncilorOrgs()
		{
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				ticouncilorState.DeactivateAllOrgs();
			}
		}

		// Token: 0x060032C1 RID: 12993 RVA: 0x00113F58 File Offset: 0x00112158
		public List<TIOrgState> GetAllOrgs()
		{
			List<TIOrgState> list = new List<TIOrgState>();
			list.AddRange(this.unassignedOrgs);
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				list.AddRange(ticouncilorState.orgs);
			}
			return list;
		}

		// Token: 0x060032C2 RID: 12994 RVA: 0x00113FC4 File Offset: 0x001121C4
		public List<TIOrgState> GetStealableOrgs(TICouncilorState councilor)
		{
			return this.councilors.SelectMany<TICouncilorState, TIOrgState>((TICouncilorState x) => x.GetStealableOrgs(councilor)).ToList<TIOrgState>();
		}

		// Token: 0x060032C3 RID: 12995 RVA: 0x00113FFA File Offset: 0x001121FA
		public float GetNegativeDailyIncomeFromUnassignedOrgs(FactionResource resource)
		{
			if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
			{
				return this.GetNegativeMonthlyIncomeFromUnassignedOrgs(resource);
			}
			return this.GetNegativeYearlyIncomeFromUnassignedOrgs(resource) / 365.2422f;
		}

		// Token: 0x060032C4 RID: 12996 RVA: 0x00114019 File Offset: 0x00112219
		public float GetNegativeYearlyIncomeFromUnassignedOrgs(FactionResource resource)
		{
			if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
			{
				return this.GetNegativeMonthlyIncomeFromUnassignedOrgs(resource);
			}
			return this.GetNegativeMonthlyIncomeFromUnassignedOrgs(resource) * 12f;
		}

		// Token: 0x060032C5 RID: 12997 RVA: 0x00114038 File Offset: 0x00112238
		public float GetNegativeMonthlyIncomeFromUnassignedOrgs(FactionResource resource)
		{
			float num = 0f;
			foreach (TIOrgState tiorgState in this.unassignedOrgs)
			{
				switch (resource)
				{
				case FactionResource.Money:
					num += ((tiorgState.adjustedIncomeMoney_month < 0f) ? tiorgState.adjustedIncomeMoney_month : 0f);
					break;
				case FactionResource.Influence:
					num += ((tiorgState.adjustedIncomeInfluence_month < 0f) ? tiorgState.adjustedIncomeInfluence_month : 0f);
					break;
				case FactionResource.Operations:
					num += ((tiorgState.adjustedIncomeOps_month < 0f) ? tiorgState.adjustedIncomeOps_month : 0f);
					break;
				case FactionResource.Boost:
					num += ((tiorgState.adjustedIncomeBoost_month < 0f) ? tiorgState.adjustedIncomeBoost_month : 0f);
					break;
				case FactionResource.MissionControl:
					num += ((tiorgState.incomeMissionControl < 0f) ? tiorgState.incomeMissionControl : 0f);
					break;
				}
			}
			return num;
		}

		// Token: 0x060032C6 RID: 12998 RVA: 0x00114154 File Offset: 0x00112354
		public int TraitProjectCount()
		{
			if (this.traitProjectCountCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedTraitProjectCount = this.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.traits.Sum<TITraitTemplate>(new Func<TITraitTemplate, int>(TIFactionState.<TraitProjectCount>g__selector|665_1)));
				this.traitProjectCountCachedFrame = TIFrameCounter.FrameCount;
			}
			return this.cachedTraitProjectCount;
		}

		// Token: 0x060032C7 RID: 12999 RVA: 0x001141B0 File Offset: 0x001123B0
		public int OrgProjectCount()
		{
			if (this.orgProjectCountCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedOrgProjectCount = this.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.activeOrgs.Sum<TIOrgState>(new Func<TIOrgState, int>(TIFactionState.<OrgProjectCount>g__selector|668_1)));
				this.orgProjectCountCachedFrame = TIFrameCounter.FrameCount;
			}
			return this.cachedOrgProjectCount;
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x0011420B File Offset: 0x0011240B
		public bool OrgProjectAllowed()
		{
			return this.orgProjectSlotUnlocked;
		}

		// Token: 0x060032C9 RID: 13001 RVA: 0x00114214 File Offset: 0x00112414
		private TIProjectTemplate GetDefaultProjectForSlot(int slot)
		{
			List<int> list = new List<int> { 3 };
			if (this.orgProjectSlotUnlocked)
			{
				list.Add(4);
			}
			if (this.habProjectSlotUnlocked)
			{
				list.Add(5);
			}
			list.Remove(slot);
			List<string> list2 = new List<string> { "Project_AudienceResearch", "Project_CommercialResearch", "Project_OperationsResearch" };
			foreach (int num in list)
			{
				list2.Remove(this.GetProjectProgressInSlot(num).projectTemplateName);
			}
			return TemplateManager.Find<TIProjectTemplate>(list2[0], false);
		}

		// Token: 0x060032CA RID: 13002 RVA: 0x001142D8 File Offset: 0x001124D8
		public void CheckForOrgProjectStatusChange()
		{
			bool orgProjectSlotUnlocked = this.orgProjectSlotUnlocked;
			this.orgProjectSlotUnlocked = this.activeCouncilors.Any<TICouncilorState>((TICouncilorState x) => x.activeOrgs.Any<TIOrgState>((TIOrgState y) => y.projectCapacityGranted > 0));
			if (this.orgProjectSlotUnlocked != orgProjectSlotUnlocked)
			{
				if (this.orgProjectSlotUnlocked)
				{
					TIProjectTemplate projectInSlot = this.GetProjectInSlot(4);
					if (projectInSlot == null || this.completedProjects.Contains(projectInSlot) || !this.availableProjects.Contains(projectInSlot) || projectInSlot == this.GetProjectInSlot(3) || (this.HabProjectAllowed() && projectInSlot == this.GetProjectInSlot(5)))
					{
						this.SetProjectInSlot(4, this.SelectableProjects(4)[0]);
					}
				}
				else
				{
					this.SetResearchPriority(4, 0);
					if (TIPromptQueueState.anyBlockingPrompt)
					{
						Prompt prompt = new Prompt(this, this, null, "PromptSelectProject", 4);
						if (TIPromptQueueState.HasPromptStatic(prompt))
						{
							this.SetProjectInSlot(4, this.GetDefaultProjectForSlot(4));
							TIPromptQueueState.RemovePromptStatic(prompt);
						}
					}
				}
				GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x001143E0 File Offset: 0x001125E0
		public int HabProjectCount()
		{
			if (this.habProjectCountCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedHabProjectCount = this.habSectors.Sum<TISectorState>((TISectorState x) => x.habModules.Where<TIHabModuleState>((TIHabModuleState z) => z.active).Sum<TIHabModuleState>(new Func<TIHabModuleState, int>(TIFactionState.<HabProjectCount>g__selector|674_1)));
				this.habProjectCountCachedFrame = TIFrameCounter.FrameCount;
			}
			return this.cachedHabProjectCount;
		}

		// Token: 0x060032CC RID: 13004 RVA: 0x0011443B File Offset: 0x0011263B
		public bool HabProjectAllowed()
		{
			return this.habProjectSlotUnlocked;
		}

		// Token: 0x060032CD RID: 13005 RVA: 0x00114444 File Offset: 0x00112644
		public void CheckforHabProjectUnlock()
		{
			bool habProjectSlotUnlocked = this.habProjectSlotUnlocked;
			this.habProjectSlotUnlocked = this.habSectors.Any<TISectorState>(delegate(TISectorState x)
			{
				List<TIHabModuleState> habModules = x.habModules;
				bool? flag;
				if (habModules == null)
				{
					flag = null;
				}
				else
				{
					IEnumerable<TIHabModuleState> enumerable = habModules.Where<TIHabModuleState>((TIHabModuleState z) => z.active);
					if (enumerable == null)
					{
						flag = null;
					}
					else
					{
						flag = new bool?(enumerable.Any<TIHabModuleState>((TIHabModuleState y) => y.moduleTemplate.incomeProjects > 0));
					}
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			});
			if (this.habProjectSlotUnlocked != habProjectSlotUnlocked)
			{
				if (this.habProjectSlotUnlocked)
				{
					TIProjectTemplate projectInSlot = this.GetProjectInSlot(5);
					if (projectInSlot == null || projectInSlot == this.GetProjectInSlot(3) || this.completedProjects.Contains(projectInSlot) || !this.availableProjects.Contains(projectInSlot) || (this.orgProjectSlotUnlocked && projectInSlot == this.GetProjectInSlot(4)))
					{
						this.SetProjectInSlot(5, this.SelectableProjects(5)[0]);
					}
				}
				else
				{
					this.SetResearchPriority(5, 0);
					if (TIPromptQueueState.anyBlockingPrompt)
					{
						Prompt prompt = new Prompt(this, this, null, "PromptSelectProject", 5);
						if (TIPromptQueueState.HasPromptStatic(prompt))
						{
							this.SetProjectInSlot(4, this.GetDefaultProjectForSlot(4));
							TIPromptQueueState.RemovePromptStatic(prompt);
						}
					}
				}
				GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x060032CE RID: 13006 RVA: 0x0011454C File Offset: 0x0011274C
		public TIProjectTemplate GetProjectInSlot(int slot)
		{
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				if (projectProgress.slot == slot)
				{
					return projectProgress.projectTemplate;
				}
			}
			return null;
		}

		// Token: 0x060032CF RID: 13007 RVA: 0x001145B0 File Offset: 0x001127B0
		public ProjectProgress GetProjectProgressInSlot(int slot)
		{
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				if (projectProgress.slot == slot)
				{
					return projectProgress;
				}
			}
			return null;
		}

		// Token: 0x060032D0 RID: 13008 RVA: 0x0011460C File Offset: 0x0011280C
		public int GetSlotForProject(TIProjectTemplate projectTemplate)
		{
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				if (projectProgress.projectTemplate == projectTemplate)
				{
					return projectProgress.slot;
				}
			}
			return -1;
		}

		// Token: 0x060032D1 RID: 13009 RVA: 0x00114670 File Offset: 0x00112870
		public int GetResearchPriority(int slot)
		{
			return this.researchWeights[slot];
		}

		// Token: 0x060032D2 RID: 13010 RVA: 0x0011467A File Offset: 0x0011287A
		public void SetResearchPriority(int slot, int value)
		{
			this.researchWeights[slot] = value;
			if (this.TotalResearchWeights(this.OrgProjectAllowed(), this.HabProjectAllowed()) == 0)
			{
				this.SetResearchPriority(3, 1);
			}
			this.SetResourceIncomeDataDirty(FactionResource.Research);
		}

		// Token: 0x060032D3 RID: 13011 RVA: 0x001146A8 File Offset: 0x001128A8
		public void IncrementResearchPriority(int slot)
		{
			this.researchWeights[slot]++;
			if (this.researchWeights[slot] > 3)
			{
				this.researchWeights[slot] = 0;
				if (this.TotalResearchWeights(this.OrgProjectAllowed(), this.HabProjectAllowed()) == 0)
				{
					this.researchWeights[slot] = 1;
				}
			}
			this.SetResourceIncomeDataDirty(FactionResource.Research);
		}

		// Token: 0x060032D4 RID: 13012 RVA: 0x00114700 File Offset: 0x00112900
		public void DecrementResearchPriority(int slot)
		{
			this.researchWeights[slot]--;
			if (this.researchWeights[slot] < 0)
			{
				this.researchWeights[slot] = 3;
			}
			if (this.TotalResearchWeights(this.OrgProjectAllowed(), this.HabProjectAllowed()) == 0)
			{
				this.researchWeights[slot] = 3;
			}
			this.SetResourceIncomeDataDirty(FactionResource.Research);
		}

		// Token: 0x060032D5 RID: 13013 RVA: 0x00114758 File Offset: 0x00112958
		public string ProjectCompletionDate(int slot)
		{
			TIDateTime tidateTime = TITimeState.Now();
			ProjectProgress projectProgressInSlot = this.GetProjectProgressInSlot(slot);
			float num = projectProgressInSlot.projectTemplate.GetResearchCost(this) - projectProgressInSlot.accumulatedResearch;
			if (num > 0f)
			{
				float num2 = this.PointsToSlot(slot, this.GetDailyIncome(FactionResource.Research, false, false), (float)this.TotalResearchWeights(this.OrgProjectAllowed(), this.HabProjectAllowed()));
				if (num2 <= 0f)
				{
					return string.Empty;
				}
				float num3 = num / num2;
				tidateTime.AddDays(num3);
			}
			return tidateTime.ToCustomDateString();
		}

		// Token: 0x060032D6 RID: 13014 RVA: 0x001147D8 File Offset: 0x001129D8
		public int TotalResearchWeights(bool orgProject, bool habProject)
		{
			return this.researchWeights[0] + this.researchWeights[1] + this.researchWeights[2] + this.researchWeights[3] + (orgProject ? this.researchWeights[4] : 0) + (habProject ? this.researchWeights[5] : 0);
		}

		// Token: 0x060032D7 RID: 13015 RVA: 0x00114828 File Offset: 0x00112A28
		public float FractionWeightInSlot(int slot, bool orgProject, bool habProject)
		{
			float num = (float)this.researchWeights[slot];
			float num2 = (float)this.TotalResearchWeights(orgProject, habProject);
			return num / num2;
		}

		// Token: 0x060032D8 RID: 13016 RVA: 0x0011484A File Offset: 0x00112A4A
		public float FractionWeightInSlot(int slot)
		{
			return this.FractionWeightInSlot(slot, this.OrgProjectAllowed(), this.HabProjectAllowed());
		}

		// Token: 0x060032D9 RID: 13017 RVA: 0x00114860 File Offset: 0x00112A60
		public void AddResearchToProject(int slot, float researchValue)
		{
			TIHistoricalData.Record_Sum(this, "Effective research per day", researchValue / 60f, 60f, true);
			TIHistoricalData.Record_Sum(this, "Effective project research per day", researchValue / 60f, 60f, true);
			ProjectProgress projectProgressInSlot = this.GetProjectProgressInSlot(slot);
			if (projectProgressInSlot != null)
			{
				projectProgressInSlot.accumulatedResearch += researchValue;
			}
		}

		// Token: 0x060032DA RID: 13018 RVA: 0x001148B8 File Offset: 0x00112AB8
		public int ContributingToSlots(bool orgProject, bool habProject)
		{
			return ((this.researchWeights[0] > 0) ? 1 : 0) + ((this.researchWeights[1] > 0) ? 1 : 0) + ((this.researchWeights[2] > 0) ? 1 : 0) + ((this.researchWeights[3] > 0) ? 1 : 0) + ((orgProject && this.researchWeights[4] > 0) ? 1 : 0) + ((habProject && this.researchWeights[5] > 0) ? 1 : 0);
		}

		// Token: 0x060032DB RID: 13019 RVA: 0x0011492C File Offset: 0x00112B2C
		public int ActiveSlotsWithTechCategory(TechCategory category, bool orgProject, bool habProject)
		{
			int num = 0;
			for (int i = 0; i <= 5; i++)
			{
				if (this.researchWeights[i] > 0)
				{
					switch (i)
					{
					case 0:
					case 1:
					case 2:
					{
						TechProgress techProgress = GameStateManager.GlobalResearch().GetTechProgress(i);
						if (((techProgress != null) ? techProgress.techTemplate : null) != null && techProgress.TechCategory == category)
						{
							num++;
						}
						break;
					}
					case 3:
					{
						ProjectProgress projectProgressInSlot = this.GetProjectProgressInSlot(i);
						if (projectProgressInSlot != null && projectProgressInSlot.projectTemplate != null && projectProgressInSlot.projectCategory == category)
						{
							num++;
						}
						break;
					}
					case 4:
						if (orgProject)
						{
							ProjectProgress projectProgressInSlot2 = this.GetProjectProgressInSlot(i);
							if (projectProgressInSlot2 != null && projectProgressInSlot2.projectTemplate != null && projectProgressInSlot2.projectCategory == category)
							{
								num++;
							}
						}
						break;
					case 5:
						if (habProject)
						{
							ProjectProgress projectProgressInSlot3 = this.GetProjectProgressInSlot(i);
							if (projectProgressInSlot3 != null && projectProgressInSlot3.projectTemplate != null && projectProgressInSlot3.projectCategory == category)
							{
								num++;
							}
						}
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x060032DC RID: 13020 RVA: 0x00114A1D File Offset: 0x00112C1D
		public bool ResearchProjectCompleted(int slot)
		{
			ProjectProgress projectProgressInSlot = this.GetProjectProgressInSlot(slot);
			return projectProgressInSlot != null && projectProgressInSlot.SufficientResearchAccumulated(this);
		}

		// Token: 0x060032DD RID: 13021 RVA: 0x00114A32 File Offset: 0x00112C32
		public bool NewProjectRequired(int slot)
		{
			return this.GetProjectInSlot(slot) == null || this.GetProjectProgressInSlot(slot).completed;
		}

		// Token: 0x060032DE RID: 13022 RVA: 0x00114A4C File Offset: 0x00112C4C
		public float TechContributionBonus(TIProjectTemplate project)
		{
			float num = 0f;
			float num2 = 0f;
			foreach (TIGenericTechTemplate tigenericTechTemplate in project.TechPrereqs)
			{
				TITechTemplate titechTemplate = tigenericTechTemplate as TITechTemplate;
				if (titechTemplate != null)
				{
					num += (this.techContributionHistory.Keys.Contains(tigenericTechTemplate) ? this.techContributionHistory[titechTemplate] : 0f);
					num2 += 1f;
				}
			}
			if (project.AltTechPrereq0 != null)
			{
				TITechTemplate titechTemplate2 = project.AltTechPrereq0 as TITechTemplate;
				if (titechTemplate2 != null)
				{
					num += (this.techContributionHistory.Keys.Contains(titechTemplate2) ? this.techContributionHistory[titechTemplate2] : 0f);
				}
			}
			if (project.AltTechPrereq1 != null)
			{
				TITechTemplate titechTemplate3 = project.AltTechPrereq1 as TITechTemplate;
				if (titechTemplate3 != null)
				{
					num += (this.techContributionHistory.Keys.Contains(titechTemplate3) ? this.techContributionHistory[titechTemplate3] : 0f);
				}
			}
			return num / Mathf.Max(num2, 1f);
		}

		// Token: 0x060032DF RID: 13023 RVA: 0x00114B74 File Offset: 0x00112D74
		public string GetCachedTechTooltipString(TIGenericTechTemplate tech)
		{
			string text = "";
			if (this.cachedTechTooltipStrings != null && this.cachedTechTooltipStrings.ContainsKey(tech))
			{
				text = this.cachedTechTooltipStrings[tech];
			}
			return text;
		}

		// Token: 0x060032E0 RID: 13024 RVA: 0x00114BAC File Offset: 0x00112DAC
		public void SetCachedTechTooltipString(TIGenericTechTemplate tech, bool recursive = true)
		{
			if (this.cachedTechTooltipStrings.ContainsKey(tech))
			{
				this.cachedTechTooltipStrings[tech] = ResearchScreenController.TechTreeTooltip(this, tech, false);
				if (!recursive)
				{
					return;
				}
				using (List<TIGenericTechTemplate>.Enumerator enumerator = tech.AllPrereqFor(this, true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGenericTechTemplate tigenericTechTemplate = enumerator.Current;
						this.SetCachedTechTooltipString(tigenericTechTemplate, false);
					}
					return;
				}
			}
			this.cachedTechTooltipStrings.Add(tech, ResearchScreenController.TechTreeTooltip(this, tech, false));
			if (recursive)
			{
				foreach (TIGenericTechTemplate tigenericTechTemplate2 in tech.AllPrereqFor(this, true))
				{
					this.SetCachedTechTooltipString(tigenericTechTemplate2, false);
				}
			}
		}

		// Token: 0x060032E1 RID: 13025 RVA: 0x00114C88 File Offset: 0x00112E88
		public void CacheAllTechTooltipStrings()
		{
			List<TITechTemplate> allTechs = TIGlobalResearchState.GetAllTechs();
			List<TIProjectTemplate> allProjects = TIGlobalResearchState.GetAllProjects();
			foreach (TITechTemplate titechTemplate in allTechs)
			{
				this.SetCachedTechTooltipString(titechTemplate, false);
			}
			foreach (TIProjectTemplate tiprojectTemplate in allProjects)
			{
				this.SetCachedTechTooltipString(tiprojectTemplate, false);
			}
		}

		// Token: 0x060032E2 RID: 13026 RVA: 0x00114D24 File Offset: 0x00112F24
		public void OnProjectComplete(TIProjectTemplate project, int slot, bool suppressLogging = false, bool startup = false)
		{
			if (slot >= 3)
			{
				if (this.GetProjectProgressInSlot(slot).completed)
				{
					return;
				}
				this.GetProjectProgressInSlot(slot).completed = true;
				this.LastObjectiveProjectCompletionDate = TITimeState.Now();
			}
			List<TIHabModuleTemplate> list = (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
				where x.RequiredProject == project
				select x).ToList<TIHabModuleTemplate>();
			List<TIShipPartTemplate> list2 = (from x in TemplateManager.IterateByClass<TIShipPartTemplate>(true)
				where x.requiredProject == project
				select x).ToList<TIShipPartTemplate>();
			this.AddCompletedProject(project);
			string text = this.longtermTechTarget;
			TIGlobalResearchState.globalResearch.CheckForAutoPickTech(this);
			TIProjectTemplate tiprojectTemplate = null;
			if (!string.IsNullOrEmpty(this.longtermTechTarget) && TemplateManager.Find<TIGenericTechTemplate>(text, true).isProject())
			{
				tiprojectTemplate = (TIProjectTemplate)TIGlobalResearchState.globalResearch.nextPrereqTechToTarget(this.longtermTechTarget, this, false);
			}
			if (this.activeProjectTriggers.Any<ProjectTrigger>((ProjectTrigger x) => x.projectTemplate == project))
			{
				this.activeProjectTriggers.Remove(this.activeProjectTriggers.First<ProjectTrigger>((ProjectTrigger x) => x.projectTemplate == project));
			}
			if (!project.repeatable)
			{
				this.availableProjectNames.Remove(project.dataName);
				this.availableProjects.Remove(project);
			}
			IEnumerable<TIProjectTemplate> allProjects = TIGlobalResearchState.GetAllProjects();
			Func<TIProjectTemplate, bool> <>9__8;
			Func<TIProjectTemplate, bool> func;
			if ((func = <>9__8) == null)
			{
				func = (<>9__8 = (TIProjectTemplate x) => x.TechPrereqs.Contains(project) || x.AltTechPrereq0 == project || x.AltTechPrereq1 == project);
			}
			foreach (TIProjectTemplate tiprojectTemplate2 in allProjects.Where<TIProjectTemplate>(func))
			{
				this.RollToAddProjectTrigger(tiprojectTemplate2.ref_project, null);
			}
			foreach (TIEffectTemplate tieffectTemplate in project.Effects)
			{
				TIEffectsState.AddEffect(tieffectTemplate, this, null, null, "");
			}
			foreach (ResourceValue resourceValue in project.resourcesGranted)
			{
				FactionResource resource = resourceValue.resource;
				if (resource != FactionResource.None)
				{
					if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
					{
						this.ChangeBaseResourceIncome(resourceValue.resource, resourceValue.value);
					}
					else
					{
						this.AddToCurrentResource(resourceValue.value, resourceValue.resource, false, "Project Completion");
					}
				}
			}
			TIOrgTemplate orgGranted = project.OrgGranted;
			TIOrgState tiorgState = null;
			if (orgGranted != null)
			{
				tiorgState = this.CreateOrTransferOrgToFactionPool(orgGranted, true);
			}
			if (!suppressLogging)
			{
				TIFactionState.LogAI(this.displayName + " completes " + project.displayName, false);
				TIProjectTemplate project2 = project;
				TIOrgState tiorgState2 = tiorgState;
				string text2 = ((tiprojectTemplate != null) ? tiprojectTemplate.displayName : null);
				TIGenericTechTemplate tigenericTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(text, true);
				TINotificationQueueState.LogProjectComplete(this, project2, slot, tiorgState2, text2, (tigenericTechTemplate != null) ? tigenericTechTemplate.displayName : null);
			}
			this.CheckForObjectivesCompleteViaProject(project);
			if (list2.Count > 0)
			{
				this.UpdateAllowedShipParts(list2);
				if (this.ShipConstructionHabs(true, true).Count > 0)
				{
					this.updateShipDesignsFlag = this.allowedShipHulls.Count<TIShipHullTemplate>() > 0;
				}
				TIHabModuleTemplate.InvalidateHabDefenseNumbers(this);
				foreach (TIHabState tihabState in this.habs)
				{
					foreach (TIHabModuleState tihabModuleState in tihabState.CompletedModules())
					{
						if (tihabModuleState.isCombatModule)
						{
							tihabModuleState.SetSpaceCombatWeapons(this);
						}
					}
				}
				if (list2.Any<TIShipPartTemplate>((TIShipPartTemplate x) => x.exoFighterPart) && (TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron) || this.IsAlienFaction))
				{
					this.CacheSTOFighterMass();
				}
				GameControl.eventManager.TriggerEvent(new ShipPartUnlocked(this), null, new object[] { this });
			}
			if (list.Count > 0)
			{
				GameControl.eventManager.TriggerEvent(new HabModuleUnlocked(this), null, new object[] { this });
				if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule))
				{
					this.updateHabPlanningFlag = true;
				}
			}
			if (project.Effects.SelectMany<TIEffectTemplate, Context>((TIEffectTemplate x) => x.GetContexts()).Intersect<Context>(TIFactionState.spaceRangeContexts).Count<Context>() > 0)
			{
				this.updateHabPlanningFlag = true;
			}
			if (project.oneTimeGlobally)
			{
				HashSet<TIFactionState> hashSet = new HashSet<TIFactionState>();
				IEnumerable<TIFactionState> enumerable = GameStateManager.AllHumanFactions();
				Func<TIFactionState, bool> <>9__9;
				Func<TIFactionState, bool> func2;
				if ((func2 = <>9__9) == null)
				{
					func2 = (<>9__9 = (TIFactionState x) => x != this);
				}
				foreach (TIFactionState tifactionState in enumerable.Where<TIFactionState>(func2).ToList<TIFactionState>().Shuffle<TIFactionState>())
				{
					tifactionState.availableProjects.Remove(project);
					tifactionState.availableProjectNames.Remove(project.dataName);
					int slotForProject = tifactionState.GetSlotForProject(project);
					if (slotForProject >= 3)
					{
						ProjectProgress projectProgressInSlot = tifactionState.GetProjectProgressInSlot(slotForProject);
						tifactionState.currentProjectProgress.Remove(projectProgressInSlot);
						if (tifactionState.ProjectAllowedInSlot(slotForProject))
						{
							tifactionState.SetProjectInSlot(slotForProject, tifactionState.GetDefaultProjectForSlot(slotForProject));
						}
						TINotificationQueueState.LogUniqueProjectSnipedByAnotherFaction(this, tifactionState, project, slotForProject);
						hashSet.Add(tifactionState);
					}
					foreach (TIProjectTemplate tiprojectTemplate3 in TIGlobalResearchState.GetAllProjects())
					{
						if (tiprojectTemplate3.TechPrereqs.Contains(project) || tiprojectTemplate3.AltTechPrereq0 == project || tiprojectTemplate3.AltTechPrereq1 == project)
						{
							tifactionState.RollToAddProjectTrigger(tiprojectTemplate3, project);
						}
					}
				}
				TINotificationQueueState.LogUniqueProjectCompleteByAnotherFaction(this, project, hashSet);
				List<TIBilateralTemplate> list3 = new List<TIBilateralTemplate>();
				foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
				{
					if (tibilateralTemplate.projectUnlockName == project.dataName && tibilateralTemplate.BilateralIsInScenario())
					{
						BilateralRelationType relationType = tibilateralTemplate.relationType;
						if (relationType != BilateralRelationType.PhysicalAdjacency)
						{
							if (relationType == BilateralRelationType.Claim)
							{
								tibilateralTemplate.nationState1.SetClaim(tibilateralTemplate.regionState1, tibilateralTemplate.hostileClaim, false);
								if (tibilateralTemplate.capitalClaim && !tibilateralTemplate.nationState1.extant)
								{
									tibilateralTemplate.nationState1.SetCapital(tibilateralTemplate.regionState1);
								}
								list3.Add(tibilateralTemplate);
							}
						}
						else
						{
							tibilateralTemplate.regionState1.ChangeAdjacency(tibilateralTemplate.regionState2, (tibilateralTemplate.friendlyOnly && tibilateralTemplate.regionState1.GetAdjacencyType(tibilateralTemplate.regionState2) != TerrestrialAdjacencyType.FullAdjacency) ? TerrestrialAdjacencyType.FriendlyCrossingOnly : TerrestrialAdjacencyType.FullAdjacency);
						}
					}
				}
			}
			string dataName = project.dataName;
			if (dataName != null && dataName == "Project_ClandestineCells")
			{
				this.CompleteMilestone(CampaignMilestone.TutorialResearchClandestineCells);
			}
			if (project.Effects.Count > 0)
			{
				this.SetResourceIncomeDataDirty();
			}
			if (GameControl.loadcycle100 && this.isActivePlayer)
			{
				this.SetCachedTechTooltipString(project, true);
			}
			if (tiprojectTemplate != null)
			{
				PlayerAction playerAction = new SelectProjectForDevelopmentAction(this, slot, tiprojectTemplate);
				this.playerControl.StartAction(playerAction);
			}
			TIHistoricalData.Record(this, "Completed projects", (float)this.completedProjects.Count, 0f, true);
			TIHistoricalData.Record(this, "Total project investment", this.completedProjects.Sum<TIProjectTemplate>((TIProjectTemplate x) => x.GetResearchCost(this)), 0f, true);
		}

		// Token: 0x060032E3 RID: 13027 RVA: 0x001155C0 File Offset: 0x001137C0
		public void OnProjectCompleteInSlot(int slot)
		{
			this.OnProjectComplete(this.GetProjectProgressInSlot(slot).projectTemplate, slot, false, false);
		}

		// Token: 0x060032E4 RID: 13028 RVA: 0x001155D8 File Offset: 0x001137D8
		public bool ProjectPaused(TIProjectTemplate template)
		{
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				if (projectProgress.projectTemplate == template)
				{
					if (projectProgress.slot >= 6 || (!this.orgProjectSlotUnlocked && projectProgress.slot == 4) || (!this.habProjectSlotUnlocked && projectProgress.slot == 5))
					{
						return true;
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x060032E5 RID: 13029 RVA: 0x00115664 File Offset: 0x00113864
		public ProjectProgress GetProjectProgressByTemplate(TIProjectTemplate template)
		{
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				if (projectProgress.projectTemplate == template)
				{
					return projectProgress;
				}
			}
			return new ProjectProgress();
		}

		// Token: 0x060032E6 RID: 13030 RVA: 0x001156C4 File Offset: 0x001138C4
		public float GetProjectProgressValueByTemplate(TIProjectTemplate template)
		{
			float num = 0f;
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				if (projectProgress.projectTemplate != null && projectProgress.projectTemplate == template)
				{
					if (projectProgress.completed)
					{
						num = (projectProgress.projectTemplate.repeatable ? 0f : projectProgress.projectTemplate.GetResearchCost(this));
						break;
					}
					num = projectProgress.accumulatedResearch;
					break;
				}
			}
			return num;
		}

		// Token: 0x060032E7 RID: 13031 RVA: 0x0011575C File Offset: 0x0011395C
		public float GetProjectProgressValueByTemplateFraction(TIProjectTemplate template)
		{
			if (template.GetResearchCost(this) > 0f)
			{
				return this.GetProjectProgressValueByTemplate(template) / template.GetResearchCost(this);
			}
			return -1f;
		}

		// Token: 0x060032E8 RID: 13032 RVA: 0x00115784 File Offset: 0x00113984
		public void SetProjectInSlot(int slot, TIProjectTemplate newProjectTemplate)
		{
			if (newProjectTemplate == null)
			{
				Log.Error(this.displayName + " trying to add null project to slot " + slot.ToString(), Array.Empty<object>());
			}
			if (!this.ProjectAllowedInSlot(slot) && this.player.isAI)
			{
				Log.Error(string.Concat(new string[]
				{
					this.displayName,
					" trying to add project ",
					newProjectTemplate.dataName,
					" to locked slot ",
					slot.ToString()
				}), Array.Empty<object>());
			}
			string text = string.Empty;
			if (this.GetProjectInSlot(slot) == null)
			{
				if (this.ProjectPaused(newProjectTemplate))
				{
					this.GetProjectProgressInSlot(this.GetSlotForProject(newProjectTemplate)).slot = slot;
					text += "Case 1:";
				}
				else
				{
					this.currentProjectProgress.Add(new ProjectProgress(newProjectTemplate, slot, 0f));
					text += "Case 2:";
				}
			}
			else if (this.NewProjectRequired(slot))
			{
				ProjectProgress projectProgressInSlot = this.GetProjectProgressInSlot(slot);
				this.currentProjectProgress.Remove(projectProgressInSlot);
				if (this.ProjectPaused(newProjectTemplate))
				{
					this.GetProjectProgressInSlot(this.GetSlotForProject(newProjectTemplate)).slot = slot;
					text += "Case 3:";
				}
				else
				{
					projectProgressInSlot.accumulatedResearch = 0f;
					projectProgressInSlot.projectTemplateName = newProjectTemplate.dataName;
					projectProgressInSlot.completed = false;
					this.currentProjectProgress.Add(projectProgressInSlot);
					text += "Case 4:";
				}
			}
			else if (this.ProjectPaused(newProjectTemplate))
			{
				ProjectProgress projectProgressInSlot2 = this.GetProjectProgressInSlot(this.GetSlotForProject(newProjectTemplate));
				ProjectProgress projectProgressInSlot3 = this.GetProjectProgressInSlot(slot);
				ProjectProgress projectProgress = projectProgressInSlot2;
				ProjectProgress projectProgress2 = projectProgressInSlot3;
				int slot2 = projectProgressInSlot3.slot;
				int slot3 = projectProgressInSlot2.slot;
				projectProgress.slot = slot2;
				projectProgress2.slot = slot3;
				text += "Case 5:";
			}
			else
			{
				ProjectProgress projectProgressInSlot4 = this.GetProjectProgressInSlot(slot);
				if (projectProgressInSlot4.projectTemplate != newProjectTemplate)
				{
					if (projectProgressInSlot4.accumulatedResearch > 0f)
					{
						List<int> list = new List<int>();
						foreach (ProjectProgress projectProgress3 in this.currentProjectProgress)
						{
							list.Add(projectProgress3.slot);
						}
						bool flag = false;
						int num = 6;
						while (!flag)
						{
							if (list.Contains(num))
							{
								num++;
							}
							else
							{
								flag = true;
							}
						}
						projectProgressInSlot4.slot = num;
						text += "A ";
					}
					else
					{
						this.currentProjectProgress.Remove(projectProgressInSlot4);
					}
					ProjectProgress projectProgress4 = new ProjectProgress(newProjectTemplate, slot, 0f);
					this.currentProjectProgress.Add(projectProgress4);
					text += "Case 6:";
				}
			}
			if ((from x in this.currentProjectProgress
				group x by x.projectTemplateName).Any<IGrouping<string, ProjectProgress>>((IGrouping<string, ProjectProgress> y) => y.Count<ProjectProgress>() > 1))
			{
				text = string.Concat(new string[]
				{
					text,
					"Duplicate project templates selected for ",
					this.displayName,
					" Slot: ",
					slot.ToString(),
					" Project: ",
					newProjectTemplate.dataName
				});
				foreach (ProjectProgress projectProgress5 in this.currentProjectProgress)
				{
					text = string.Concat(new string[]
					{
						text,
						"\nSlot",
						projectProgress5.slot.ToString(),
						": ",
						projectProgress5.projectTemplateName
					});
				}
				Log.Error(text, Array.Empty<object>());
			}
			TIFactionState.LogAI(string.Concat(new string[]
			{
				this.displayName,
				" picked ",
				newProjectTemplate.displayName,
				" for slot ",
				slot.ToString()
			}), false);
		}

		// Token: 0x060032E9 RID: 13033 RVA: 0x00115B88 File Offset: 0x00113D88
		public List<int> AllowedProjectSlots()
		{
			List<int> list = new List<int>();
			for (int i = 3; i <= 5; i++)
			{
				if (this.ProjectAllowedInSlot(i))
				{
					list.Add(i);
				}
			}
			return list;
		}

		// Token: 0x060032EA RID: 13034 RVA: 0x00115BB8 File Offset: 0x00113DB8
		public bool ProjectAllowedInSlot(int slot)
		{
			return slot == 3 || (slot == 4 && this.OrgProjectAllowed()) || (slot == 5 && this.HabProjectAllowed());
		}

		// Token: 0x060032EB RID: 13035 RVA: 0x00115BE0 File Offset: 0x00113DE0
		public int BestAvailableEmptySlot()
		{
			float num = (float)((this.ProjectAllowedInSlot(4) && this.NewProjectRequired(4)) ? 1 : (-1));
			float num2 = (float)((this.NewProjectRequired(5) && this.ProjectAllowedInSlot(5)) ? 1 : (-1));
			if (num > 1f && num >= num2)
			{
				return 4;
			}
			if (num2 > 1f && num2 > num)
			{
				return 5;
			}
			return 3;
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x060032EC RID: 13036 RVA: 0x00115C3C File Offset: 0x00113E3C
		public bool shipBuilding
		{
			get
			{
				return this.nShipyardQueues.Keys.Count > 0 && this.GetMonthlyIncome(FactionResource.Water, false, false) > 5f && this.GetMonthlyIncome(FactionResource.Volatiles, false, false) > 5f && this.GetMonthlyIncome(FactionResource.Metals, false, false) > 5f;
			}
		}

		// Token: 0x060032ED RID: 13037 RVA: 0x00115C90 File Offset: 0x00113E90
		public int MostReplaceableProjectSlot()
		{
			for (int i = 3; i <= 5; i++)
			{
				if (this.ProjectAllowedInSlot(i) && this.NewProjectRequired(i))
				{
					return i;
				}
			}
			IEnumerable<TIProjectTemplate> enumerable = (from x in this.CurrentlyActiveProjects()
				group x by AIEvaluators.GetTechTier(x, this)).MinBy<IGrouping<int, TIProjectTemplate>, int>((IGrouping<int, TIProjectTemplate> x) => x.Key);
			string nextFactionTech = this.cheapestForcedTechName;
			bool shipBuilding = this.shipBuilding;
			List<TIMissionTemplate> availableMissions = this.GetAllPossibleMissions().ToList<TIMissionTemplate>();
			TIProjectTemplate tiprojectTemplate = enumerable.MinBy<TIProjectTemplate, float>((TIProjectTemplate x) => AIEvaluators.ScoreTech(this, x, true, nextFactionTech == x.dataName, shipBuilding, availableMissions));
			return this.GetSlotForProject(tiprojectTemplate);
		}

		// Token: 0x060032EE RID: 13038 RVA: 0x00115D48 File Offset: 0x00113F48
		public float MultipleFacilitiesMultiplier(int traitProjects, int orgFacilities, int habFacilities)
		{
			int num = 0;
			num += (int)this.baseIncomes_year[FactionResource.Projects];
			num += traitProjects;
			num += Mathf.Max(0, orgFacilities - 1);
			num += Mathf.Max(0, habFacilities - 1);
			if (num > 0)
			{
				int num2 = num - 20;
				int num3 = num2 - 20;
				return (float)Mathf.Min(num, 20) * TemplateManager.global.first20ExtraProjectBonusPct + (float)Mathf.Clamp(num2, 0, 20) * TemplateManager.global.second20ExtraProjectBonusPct + (float)Mathf.Max(num3, 0) * TemplateManager.global.overageExtraProjectBonusPct;
			}
			return 0f;
		}

		// Token: 0x060032EF RID: 13039 RVA: 0x00115DD8 File Offset: 0x00113FD8
		public float BaseHabsMultiplier(TechCategory techCategory, float extra = 0f)
		{
			if (this.baseHabsMultiplierCachedFrames == null)
			{
				this.baseHabsMultiplierCachedFrames = ((TechCategory[])Enum.GetValues(typeof(TechCategory))).ToDictionary<TechCategory, TechCategory, int>((TechCategory x) => x, (TechCategory x) => -1);
			}
			if (this.baseHabsMultiplierCachedFrames[techCategory] != TIFrameCounter.FrameCount)
			{
				float num = 0f;
				foreach (TISectorState tisectorState in this.habSectors)
				{
					for (int i = 0; i < tisectorState.habModules.Count; i++)
					{
						TIHabModuleState tihabModuleState = tisectorState.habModules[i];
						if (!tihabModuleState.empty && tihabModuleState.powered)
						{
							for (int j = 0; j < tihabModuleState.moduleTemplate.techBonuses.Length; j++)
							{
								if (tihabModuleState.moduleTemplate.techBonuses[j].category == techCategory)
								{
									num += tihabModuleState.moduleTemplate.techBonuses[j].bonus;
								}
							}
						}
					}
				}
				this.cachedBaseHabsMultipliers[techCategory] = num;
				this.baseHabsMultiplierCachedFrames[techCategory] = TIFrameCounter.FrameCount;
			}
			return this.cachedBaseHabsMultipliers[techCategory] + extra;
		}

		// Token: 0x060032F0 RID: 13040 RVA: 0x00115F64 File Offset: 0x00114164
		public float AdjustedHabsMultiplier(TechCategory techCategory, float extra = 0f)
		{
			float num = this.BaseHabsMultiplier(techCategory, extra);
			if (num > 0.5f)
			{
				float num2 = num - 0.5f;
				num = 0.5f + 0.5f * (num2 / (num2 + 2f));
			}
			return num;
		}

		// Token: 0x060032F1 RID: 13041 RVA: 0x00115FA4 File Offset: 0x001141A4
		public float HabsMultiplier(TechCategory techCategory)
		{
			float num = 0f;
			foreach (TISectorState tisectorState in this.habSectors)
			{
				for (int i = 0; i < tisectorState.habModules.Count; i++)
				{
					TIHabModuleState tihabModuleState = tisectorState.habModules[i];
					if (!tihabModuleState.empty && tihabModuleState.powered)
					{
						for (int j = 0; j < tihabModuleState.moduleTemplate.techBonuses.Length; j++)
						{
							if (tihabModuleState.moduleTemplate.techBonuses[j].category == techCategory)
							{
								num += tihabModuleState.moduleTemplate.techBonuses[j].bonus;
							}
						}
					}
				}
			}
			if (num > 0.5f)
			{
				float num2 = num - 0.5f;
				num = 0.5f + 0.5f * (num2 / (num2 + 2f));
			}
			return num;
		}

		// Token: 0x060032F2 RID: 13042 RVA: 0x001160B4 File Offset: 0x001142B4
		public float TraitsMultiplier(TechCategory techCategory)
		{
			if (this.traitsMultiplierCachedFrame[techCategory] != TIFrameCounter.FrameCount)
			{
				float num = 0f;
				foreach (TICouncilorState ticouncilorState in this.councilors.Where<TICouncilorState>((TICouncilorState x) => x.active))
				{
					foreach (TITraitTemplate titraitTemplate in ticouncilorState.traits)
					{
						for (int i = 0; i < titraitTemplate.techBonuses.Count; i++)
						{
							if (titraitTemplate.techBonuses[i].category == techCategory)
							{
								num += titraitTemplate.techBonuses[i].bonus;
							}
						}
					}
				}
				if (num > 0.5f)
				{
					float num2 = num - 0.5f;
					num = 0.5f + 0.5f * (num2 / (num2 + 2f));
				}
				this.cachedTraitsMultiplier[techCategory] = num;
				this.traitsMultiplierCachedFrame[techCategory] = TIFrameCounter.FrameCount;
			}
			return this.cachedTraitsMultiplier[techCategory];
		}

		// Token: 0x060032F3 RID: 13043 RVA: 0x0011620C File Offset: 0x0011440C
		public float OrgsMultiplier(TechCategory techCategory)
		{
			if (this.orgsMultiplierCachedFrame[techCategory] != TIFrameCounter.FrameCount)
			{
				this.cachedOrgsMultiplier[techCategory] = 0f;
				foreach (TICouncilorState ticouncilorState in this.councilors.Where<TICouncilorState>((TICouncilorState x) => x.active))
				{
					foreach (TIOrgState tiorgState in ticouncilorState.activeOrgs)
					{
						for (int i = 0; i < tiorgState.techBonuses.Length; i++)
						{
							if (tiorgState.techBonuses[i].category == techCategory)
							{
								Dictionary<TechCategory, float> dictionary = this.cachedOrgsMultiplier;
								dictionary[techCategory] += tiorgState.techBonuses[i].bonus;
							}
						}
					}
				}
				if (this.cachedOrgsMultiplier[techCategory] > 0.5f)
				{
					float num = this.cachedOrgsMultiplier[techCategory] - 0.5f;
					this.cachedOrgsMultiplier[techCategory] = 0.5f + 0.5f * (num / (num + 2f));
				}
				this.orgsMultiplierCachedFrame[techCategory] = TIFrameCounter.FrameCount;
			}
			return this.cachedOrgsMultiplier[techCategory];
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x0011639C File Offset: 0x0011459C
		public float FleetsModifier(TechCategory techCategory)
		{
			if (this.fleetsModifierCachedFrame[techCategory] != TIFrameCounter.FrameCount)
			{
				this.cachedFleetsModifier[techCategory] = 0f;
				if (techCategory == TechCategory.SpaceScience)
				{
					float num = this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Sum<TISpaceShipState>((TISpaceShipState y) => y.spaceScienceResearchBonus);
					if (num > 0.5f)
					{
						float num2 = num - 0.5f;
						num = 0.5f + 0.5f * (num2 / (num2 + 2f));
					}
					this.cachedFleetsModifier[techCategory] = num;
				}
				this.fleetsModifierCachedFrame[techCategory] = TIFrameCounter.FrameCount;
			}
			return this.cachedFleetsModifier[techCategory];
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x00116476 File Offset: 0x00114676
		public float InvestigationsModifier(TechCategory techCategory)
		{
			if (techCategory != TechCategory.Xenology)
			{
				return 0f;
			}
			return (float)this.alienInvestigations / 100f;
		}

		// Token: 0x060032F6 RID: 13046 RVA: 0x0011648F File Offset: 0x0011468F
		public float SumCategoryModifiers(TechCategory category)
		{
			return this.HabsMultiplier(category) + this.OrgsMultiplier(category) + this.TraitsMultiplier(category) + this.InvestigationsModifier(category) + this.FleetsModifier(category) + this.EffectsModifier(category);
		}

		// Token: 0x060032F7 RID: 13047 RVA: 0x001164C0 File Offset: 0x001146C0
		public float DistributedCategoryModifierValue(TechCategory category)
		{
			float num = this.SumCategoryModifiers(category);
			int num2 = this.ActiveSlotsWithTechCategory(category, this.orgProjectSlotUnlocked, this.habProjectSlotUnlocked) - 1;
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					num *= TemplateManager.global.categoryBonusPenaltyPerExtraSlot;
				}
			}
			return num;
		}

		// Token: 0x060032F8 RID: 13048 RVA: 0x0011650C File Offset: 0x0011470C
		public float EffectsModifier(TechCategory techCategory)
		{
			Context context;
			switch (techCategory)
			{
			case TechCategory.Materials:
				context = Context.MaterialScience;
				break;
			case TechCategory.SpaceScience:
				context = Context.SpaceScience;
				break;
			case TechCategory.Energy:
				context = Context.EnergyScience;
				break;
			case TechCategory.LifeScience:
				context = Context.LifeScience;
				break;
			case TechCategory.MilitaryScience:
				context = Context.MilitaryScience;
				break;
			case TechCategory.InformationScience:
				context = Context.InformationScience;
				break;
			case TechCategory.SocialScience:
				context = Context.SocialScience;
				break;
			case TechCategory.Xenology:
				context = Context.Xenology;
				break;
			default:
				return 0f;
			}
			return TIEffectsState.SumEffectsModifiers(context, this, 0f, null);
		}

		// Token: 0x060032F9 RID: 13049 RVA: 0x00116594 File Offset: 0x00114794
		public float GetEffectiveResearch(float points, TechCategory category, bool isProject)
		{
			float num = this.DistributedCategoryModifierValue(category);
			if (isProject)
			{
				num += this.MultipleFacilitiesMultiplier(this.TraitProjectCount(), this.OrgProjectCount(), this.HabProjectCount());
			}
			return points * (1f + num);
		}

		// Token: 0x060032FA RID: 13050 RVA: 0x001165D0 File Offset: 0x001147D0
		public float GetEffectiveResearchPerDay(TechCategory category, bool isProject, bool fast = false)
		{
			return this.GetEffectiveResearch(this.GetDailyIncome(FactionResource.Research, fast, false), category, isProject);
		}

		// Token: 0x060032FB RID: 13051 RVA: 0x001165E4 File Offset: 0x001147E4
		public float PointsToSlot(int slot, float points, float totalWeights)
		{
			float num = points;
			if (slot >= 3)
			{
				TIProjectTemplate projectInSlot = this.GetProjectInSlot(slot);
				if (projectInSlot != null)
				{
					num = this.GetEffectiveResearch(points, projectInSlot.techCategory, true);
				}
			}
			else if (slot >= 0 && slot <= 2)
			{
				num = this.GetEffectiveResearch(points, GameStateManager.GlobalResearch().GetTechProgress(slot).TechCategory, false);
			}
			return num * ((float)this.researchWeights[slot] / totalWeights);
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x060032FC RID: 13052 RVA: 0x00116641 File Offset: 0x00114841
		public float BonusPctFromDistribution
		{
			get
			{
				return TemplateManager.global.researchBonusPerSlotInUse * (float)this.ContributingToSlots(this.OrgProjectAllowed(), this.HabProjectAllowed());
			}
		}

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x060032FD RID: 13053 RVA: 0x00116661 File Offset: 0x00114861
		// (set) Token: 0x060032FE RID: 13054 RVA: 0x00116669 File Offset: 0x00114869
		public float GlobalResearchPurse
		{
			get
			{
				return this.globalResearchPurse;
			}
			set
			{
				this.globalResearchPurse = value;
			}
		}

		// Token: 0x060032FF RID: 13055 RVA: 0x00116674 File Offset: 0x00114874
		public void DistributeResearchToSlots(float basePointsToDistribute)
		{
			float num = TemplateManager.global.GetPassiveTechInvestmentDifficultyScaling() + TemplateManager.global.GetActiveTechInvestmentDifficultyScaling();
			if (this.player.isAI && this.ShouldFocusOnGlobalResearch())
			{
				num = Mathf.Clamp(num * 2f, 0f, 0.8f);
			}
			this.GlobalResearchPurse += basePointsToDistribute * num;
			int num2 = this.TotalResearchWeights(this.OrgProjectAllowed(), this.HabProjectAllowed());
			TIGlobalResearchState tiglobalResearchState = GameStateManager.GlobalResearch();
			float num3 = basePointsToDistribute * (1f + this.BonusPctFromDistribution);
			for (int i = 0; i <= 2; i++)
			{
				if (this.IsActiveHumanFaction)
				{
					tiglobalResearchState.AddResearchToTech(i, this.PointsToSlot(i, num3, (float)num2), this);
					this.GlobalResearchPurse -= basePointsToDistribute * (float)this.GetResearchPriority(i) / (float)num2;
				}
			}
			this.AddResearchToProject(3, this.PointsToSlot(3, num3, (float)num2));
			if (this.ResearchProjectCompleted(3))
			{
				this.OnProjectCompleteInSlot(3);
			}
			if (this.OrgProjectAllowed())
			{
				this.AddResearchToProject(4, this.PointsToSlot(4, num3, (float)num2));
				if (this.ResearchProjectCompleted(4))
				{
					this.OnProjectCompleteInSlot(4);
				}
			}
			if (this.HabProjectAllowed())
			{
				this.AddResearchToProject(5, this.PointsToSlot(5, num3, (float)num2));
				if (this.ResearchProjectCompleted(5))
				{
					this.OnProjectCompleteInSlot(5);
				}
			}
			GameControl.eventManager.TriggerEvent(new ResearchUpdated(this), null, new object[] { this });
		}

		// Token: 0x06003300 RID: 13056 RVA: 0x001167D0 File Offset: 0x001149D0
		public List<TIProjectTemplate> CurrentlyActiveProjects()
		{
			List<TIProjectTemplate> list = new List<TIProjectTemplate> { this.GetProjectInSlot(3) };
			if (this.OrgProjectAllowed())
			{
				list.Add(this.GetProjectInSlot(4));
			}
			if (this.HabProjectAllowed())
			{
				list.Add(this.GetProjectInSlot(5));
			}
			return list;
		}

		// Token: 0x06003301 RID: 13057 RVA: 0x0011681C File Offset: 0x00114A1C
		public List<TIProjectTemplate> StartedProjects()
		{
			List<TIProjectTemplate> list = new List<TIProjectTemplate>();
			foreach (ProjectProgress projectProgress in this.currentProjectProgress)
			{
				list.Add(projectProgress.projectTemplate);
			}
			return list;
		}

		// Token: 0x06003302 RID: 13058 RVA: 0x0011687C File Offset: 0x00114A7C
		public List<TIProjectTemplate> SelectableProjects(int slot = -1)
		{
			if (slot < 0)
			{
				slot = this.BestAvailableEmptySlot();
			}
			List<TIProjectTemplate> list = new List<TIProjectTemplate>();
			list.AddRange(this.availableProjects.Except<TIProjectTemplate>(this.CurrentlyActiveProjects()).Distinct<TIProjectTemplate>());
			TIProjectTemplate projectInSlot = this.GetProjectInSlot(slot);
			if (projectInSlot != null && this.availableProjects.Contains(projectInSlot) && !list.Contains(projectInSlot) && (projectInSlot.repeatable || !this.NewProjectRequired(slot)))
			{
				list.Add(projectInSlot);
			}
			return list;
		}

		// Token: 0x06003303 RID: 13059 RVA: 0x001168F2 File Offset: 0x00114AF2
		public IEnumerable<TIProjectTemplate> GetFutureProjects(int layerCount)
		{
			return this.GetDescendentProjects(this.availableProjects, layerCount);
		}

		// Token: 0x06003304 RID: 13060 RVA: 0x00116904 File Offset: 0x00114B04
		public IEnumerable<TIProjectTemplate> GetDescendentProjects(IEnumerable<TIProjectTemplate> ancestors, int generationCount)
		{
			if (generationCount == 0)
			{
				return Enumerable.Empty<TIProjectTemplate>();
			}
			IEnumerable<TIProjectTemplate> enumerable = from x in ancestors.SelectMany<TIProjectTemplate, TIGenericTechTemplate>((TIProjectTemplate x) => x.AllPrereqFor(this, false))
				where x.isProject()
				select x as TIProjectTemplate;
			if (generationCount == 1)
			{
				return enumerable;
			}
			return enumerable.Union<TIProjectTemplate>(this.GetDescendentProjects(enumerable, generationCount - 1));
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06003305 RID: 13061 RVA: 0x0011698B File Offset: 0x00114B8B
		public List<TIProjectTemplate> completedProjectsDistinct
		{
			get
			{
				return this.completedProjects.Distinct<TIProjectTemplate>().ToList<TIProjectTemplate>();
			}
		}

		// Token: 0x06003306 RID: 13062 RVA: 0x001169A0 File Offset: 0x00114BA0
		public List<TIProjectTemplate> StealableProjects(TIFactionState stealingFaction)
		{
			List<TIProjectTemplate> list = new List<TIProjectTemplate>();
			if (!this.IsAlienFaction)
			{
				list.AddRange(stealingFaction.GetViewofFaction(this).completedProjectsDistinct);
				list = list.Except<TIProjectTemplate>(stealingFaction.availableProjects).ToList<TIProjectTemplate>();
				list = list.Except<TIProjectTemplate>(stealingFaction.completedProjectsDistinct).ToList<TIProjectTemplate>();
				list.RemoveAll((TIProjectTemplate x) => x.oneTimeGlobally || x.repeatable || x.AI_techRole == TechRole.FactionObjective || x.AI_projectRole == ProjectRole.Objective || !x.FactionPrereqsSatisfied(stealingFaction) || !x.TechPrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), stealingFaction.completedProjects));
			}
			return list;
		}

		// Token: 0x06003307 RID: 13063 RVA: 0x00116A24 File Offset: 0x00114C24
		public List<TIProjectTemplate> ProjectsVulnerableToSabotage(TIFactionState sabotagingFaction)
		{
			List<TIProjectTemplate> list = new List<TIProjectTemplate>();
			if (!this.IsAlienFaction)
			{
				foreach (ProjectProgress projectProgress in sabotagingFaction.GetViewofFaction(this).currentProjectProgress)
				{
					if (projectProgress.accumulatedResearch > 0f && projectProgress.progressFrac(this) < 1f && !this.sabotagedProjects.Contains(projectProgress.projectTemplateName))
					{
						list.Add(projectProgress.projectTemplate);
					}
				}
			}
			return list;
		}

		// Token: 0x06003308 RID: 13064 RVA: 0x00116AC4 File Offset: 0x00114CC4
		public void SufferProjectSabotage(TIProjectTemplate project)
		{
			int slotForProject = this.GetSlotForProject(project);
			this.AddResearchToProject(slotForProject, -Mathf.Min(TemplateManager.global.MaxSabotageProjectRPDamage, this.GetProjectProgressInSlot(slotForProject).accumulatedResearch * TemplateManager.global.MaxSabotageProjectAccumulatedHit));
			this.AddSuspicionForMajorReversal(5f, null);
			this.sabotagedProjects.Add(project.dataName);
		}

		// Token: 0x06003309 RID: 13065 RVA: 0x00116B24 File Offset: 0x00114D24
		public bool AddAvailableProject(string dataName)
		{
			TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(dataName, false);
			return tiprojectTemplate != null && this.AddAvailableProject(tiprojectTemplate, null);
		}

		// Token: 0x0600330A RID: 13066 RVA: 0x00116B48 File Offset: 0x00114D48
		public bool AddAvailableProject(TIProjectTemplate project, ProjectTrigger triggerToRemove = null)
		{
			bool flag = true;
			if (!this.availableProjects.Contains(project))
			{
				this.availableProjects.Add(project);
				this.availableProjectNames.Add(project.dataName);
				if (GameControl.loadcycle100 && this.isActivePlayer)
				{
					this.SetCachedTechTooltipString(project, true);
				}
				if (this.missedProjects != null)
				{
					this.RemoveMissedProjectFromList(project.dataName);
				}
				this.AIReviewProjects = true;
			}
			else
			{
				flag = false;
			}
			if (triggerToRemove == null)
			{
				using (List<ProjectTrigger>.Enumerator enumerator = this.activeProjectTriggers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.projectTemplate == project)
						{
							this.activeProjectTriggers.Remove(triggerToRemove);
						}
					}
					return flag;
				}
			}
			this.activeProjectTriggers.Remove(triggerToRemove);
			return flag;
		}

		// Token: 0x0600330B RID: 13067 RVA: 0x00116C1C File Offset: 0x00114E1C
		public void AddCompletedProject(string dataName)
		{
			TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(dataName, false);
			if (tiprojectTemplate != null)
			{
				this.AddCompletedProject(tiprojectTemplate);
			}
		}

		// Token: 0x0600330C RID: 13068 RVA: 0x00116C3B File Offset: 0x00114E3B
		private void AddCompletedProject(TIProjectTemplate project)
		{
			this.AIReviewProjects = true;
			this.completedProjects.Add(project);
			this.finishedProjectNames.Add(project.dataName);
			if (project.oneTimeGlobally)
			{
				TIGlobalResearchState.globalResearch.AddFinishedOneTimeOnlyProject(project);
			}
		}

		// Token: 0x0600330D RID: 13069 RVA: 0x00116C74 File Offset: 0x00114E74
		public void SetLongTermTechTarget(string techDataName)
		{
			this.longtermTechTarget = techDataName;
		}

		// Token: 0x0600330E RID: 13070 RVA: 0x00116C7D File Offset: 0x00114E7D
		public bool SetProjectHidden(string projectDataName)
		{
			return this.hiddenProjects.AddUnique(projectDataName);
		}

		// Token: 0x0600330F RID: 13071 RVA: 0x00116C8B File Offset: 0x00114E8B
		public bool SetProjectUnhidden(string projectDataName)
		{
			return this.hiddenProjects.Remove(projectDataName);
		}

		// Token: 0x06003310 RID: 13072 RVA: 0x00116C99 File Offset: 0x00114E99
		public bool SetProjectFavored(string projectDataName)
		{
			return this.favoredProjects.AddUnique(projectDataName);
		}

		// Token: 0x06003311 RID: 13073 RVA: 0x00116CA7 File Offset: 0x00114EA7
		public bool SetProjectUnfavored(string projectDataName)
		{
			return this.favoredProjects.Remove(projectDataName);
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x06003312 RID: 13074 RVA: 0x00116CB8 File Offset: 0x00114EB8
		public List<TIProjectTemplate> TriggeredProjects
		{
			get
			{
				if (this.triggeredProjectsCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedTriggeredProjects.Clear();
					this.triggeredProjectsCachedFrame = TIFrameCounter.FrameCount;
					foreach (TIProjectTemplate tiprojectTemplate in TIGlobalResearchState.GetAllProjects())
					{
						if (this.ProjectAlreadyTriggered(tiprojectTemplate))
						{
							this.cachedTriggeredProjects.Add(tiprojectTemplate);
						}
					}
					return this.cachedTriggeredProjects;
				}
				return this.cachedTriggeredProjects;
			}
		}

		// Token: 0x06003313 RID: 13075 RVA: 0x00116D48 File Offset: 0x00114F48
		public bool ProjectAlreadyTriggered(TIProjectTemplate projectTemplate)
		{
			using (List<ProjectTrigger>.Enumerator enumerator = this.activeProjectTriggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.projectTemplate == projectTemplate)
					{
						return true;
					}
				}
			}
			using (List<ProjectProgress>.Enumerator enumerator2 = this.currentProjectProgress.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.projectTemplate == projectTemplate)
					{
						return true;
					}
				}
			}
			return this.availableProjects.Contains(projectTemplate) || this.completedProjects.Contains(projectTemplate) || (projectTemplate.GetResearchCost(this) <= 0f && projectTemplate.PrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), this.completedProjects, this));
		}

		// Token: 0x06003314 RID: 13076 RVA: 0x00116E30 File Offset: 0x00115030
		public void OnPublicTechCompleted(TITechTemplate completedTechTemplate, float myContributionFraction)
		{
			this.techNameContributionHistory[completedTechTemplate.dataName] = myContributionFraction;
			this.techContributionHistory[completedTechTemplate] = myContributionFraction;
			if (completedTechTemplate.endGameTech)
			{
				IEnumerable<TIProjectTemplate> allProjects = TIGlobalResearchState.GetAllProjects();
				Func<TIProjectTemplate, bool> <>9__0;
				Func<TIProjectTemplate, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (TIProjectTemplate x) => x.techCategory == completedTechTemplate.techCategory || x.techCategory == TechCategory.Xenology);
				}
				using (IEnumerator<TIProjectTemplate> enumerator = allProjects.Where<TIProjectTemplate>(func).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIProjectTemplate tiprojectTemplate = enumerator.Current;
						this.RollToAddProjectTrigger(tiprojectTemplate, null);
					}
					goto IL_00EF;
				}
			}
			IEnumerable<TIProjectTemplate> allProjects2 = TIGlobalResearchState.GetAllProjects();
			Func<TIProjectTemplate, bool> <>9__1;
			Func<TIProjectTemplate, bool> func2;
			if ((func2 = <>9__1) == null)
			{
				func2 = (<>9__1 = (TIProjectTemplate x) => x.TechPrereqs.Contains(completedTechTemplate) || x.AltTechPrereq0 == completedTechTemplate || x.AltTechPrereq1 == completedTechTemplate);
			}
			foreach (TIProjectTemplate tiprojectTemplate2 in allProjects2.Where<TIProjectTemplate>(func2))
			{
				this.RollToAddProjectTrigger(tiprojectTemplate2, null);
			}
			IL_00EF:
			this.CheckForObjectivesCompleteViaTech(completedTechTemplate);
			bool flag = false;
			foreach (string text in completedTechTemplate.orgTypeUnlocks)
			{
				if (TemplateManager.Find<TIOrgTemplate>(text, false).randomized)
				{
					for (int i = 0; i < 3; i++)
					{
						TIOrgState tiorgState = TIFactionState.CreateNewOrg(text);
						if (i == 0 && tiorgState.factionOrbit == null && tiorgState.AllowedOnFactionMarket(this))
						{
							this.AddAvailableOrg(tiorgState, true);
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				TINotificationQueueState.AddCouncilorMessage(this, CouncilorChatType.NewOrgsAvailable, this);
			}
			if (GameControl.loadcycle100 && this.isActivePlayer)
			{
				this.SetCachedTechTooltipString(completedTechTemplate, true);
			}
		}

		// Token: 0x06003315 RID: 13077 RVA: 0x00117010 File Offset: 0x00115210
		public void OnPublicTechCompleted_PostEffectsApplied(TITechTemplate completedTechTemplate, bool startup)
		{
			if (!startup)
			{
				if (completedTechTemplate.Effects.SelectMany<TIEffectTemplate, Context>((TIEffectTemplate x) => x.GetContexts()).Intersect<Context>(TIFactionState.spaceRangeContexts).Count<Context>() > 0)
				{
					this.updateHabPlanningFlag = true;
					AIDailyFactionPlanner.AIReaction(AIReactionEvent.ColonizationTechCompleted, this, null);
				}
			}
			if (completedTechTemplate.Effects.Any<TIEffectTemplate>((TIEffectTemplate x) => x.GetContexts().Contains(Context.BuildSpaceDefensesPriority)))
			{
				foreach (TINationState tinationState in GameStateManager.AllExtantNations())
				{
					tinationState.PossiblePriorityValidationChange(false);
				}
			}
		}

		// Token: 0x06003316 RID: 13078 RVA: 0x001170D4 File Offset: 0x001152D4
		public void OnMilestoneCompleted(CampaignMilestone milestone)
		{
			IEnumerable<TIProjectTemplate> allProjects = TIGlobalResearchState.GetAllProjects();
			Func<TIProjectTemplate, bool> <>9__0;
			Func<TIProjectTemplate, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TIProjectTemplate x) => x.requiredMilestone == milestone);
			}
			foreach (TIProjectTemplate tiprojectTemplate in allProjects.Where<TIProjectTemplate>(func))
			{
				this.RollToAddProjectTrigger(tiprojectTemplate, null);
			}
		}

		// Token: 0x06003317 RID: 13079 RVA: 0x00117154 File Offset: 0x00115354
		public float GetProjectUnlockChance(TIProjectTemplate project, float bonus)
		{
			if (project.factionAlways == this.templateName || !TIGlobalValuesState.GlobalValues.scenarioCustomizations.variableProjectUnlocks || project.factionAvailableChance >= 100f)
			{
				return 100f;
			}
			float num = Mathf.Max(project.factionAvailableChance, project.factionAvailableChance * (7f / (float)GameStateManager.AllHumanFactions().Length));
			num += Mathf.Max(0f, bonus);
			num += Mathf.Max(0f, TIEffectsState.SumEffectsModifiers(Context.ProjectUnlockChance, this, num, null));
			num += Mathf.Max(0f, this.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.ProjectUnlockBonus()));
			num += (float)this.GetTotalStat(CouncilorAttribute.Science, true, null) / 5f;
			return Mathf.Clamp(num, 0f, 100f);
		}

		// Token: 0x06003318 RID: 13080 RVA: 0x0011723C File Offset: 0x0011543C
		public void RollToAddProjectTrigger(TIProjectTemplate project, TIProjectTemplate oneTimeProjectCompleted = null)
		{
			List<TIProjectTemplate> list = new List<TIProjectTemplate>(this.completedProjects);
			if (oneTimeProjectCompleted != null)
			{
				list.Add(oneTimeProjectCompleted);
			}
			if (!this.ProjectAlreadyTriggered(project) && project.PrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), list, this))
			{
				float num = this.TechContributionBonus(project) * 100f;
				float num2 = this.GetProjectUnlockChance(project, num);
				if (num2 < 100f && project.factionAlways == "Random" && GameStateManager.AllHumanFactions().None<TIFactionState>((TIFactionState x) => x.ProjectAlreadyTriggered(project)))
				{
					num2 = 100f;
				}
				if (100f * TIUtilities.RandomFloatValue() <= num2)
				{
					num += (float)this.GetTotalStat(CouncilorAttribute.Science, true, null) / 5f;
					ProjectTrigger projectTrigger = new ProjectTrigger
					{
						projectTemplateName = project.dataName,
						monthlyTriggerValue = project.initialUnlockChance + num
					};
					this.activeProjectTriggers.Add(projectTrigger);
					this.RemoveMissedProjectFromList(project.dataName);
					return;
				}
				this.AddMissedProjectToList(project.dataName);
			}
		}

		// Token: 0x06003319 RID: 13081 RVA: 0x0011736E File Offset: 0x0011556E
		public bool EligibleForMissedProjectProject()
		{
			return this.missedProjects.Count > 0 && this.specialReinvestigateProject.PrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), this.completedProjects, this);
		}

		// Token: 0x0600331A RID: 13082 RVA: 0x00117397 File Offset: 0x00115597
		public void CheckForMissedProjectProject()
		{
			if (!this.availableProjects.Contains(this.specialReinvestigateProject) && this.EligibleForMissedProjectProject())
			{
				this.availableProjects.Add(this.specialReinvestigateProject);
				this.availableProjectNames.Add("Project_ReviewFailedProjects");
			}
		}

		// Token: 0x0600331B RID: 13083 RVA: 0x001173D5 File Offset: 0x001155D5
		public void AddMissedProjectToList(string missedProject)
		{
			if (missedProject != "Project_ReviewFailedProjects")
			{
				this.missedProjects.AddUnique(missedProject);
			}
			this.CheckForMissedProjectProject();
		}

		// Token: 0x0600331C RID: 13084 RVA: 0x001173F8 File Offset: 0x001155F8
		public void RemoveMissedProjectFromList(string missedProject)
		{
			this.missedProjects.Remove(missedProject);
			if (this.missedProjects.Count == 0 && this.availableProjects.Contains(this.specialReinvestigateProject))
			{
				this.availableProjects.Remove(this.specialReinvestigateProject);
				this.availableProjectNames.Remove("Project_ReviewFailedProjects");
			}
		}

		// Token: 0x0600331D RID: 13085 RVA: 0x00117458 File Offset: 0x00115658
		public void DailyProjectTriggerCheck()
		{
			List<ProjectTrigger> list = new List<ProjectTrigger>();
			List<TITechTemplate> list2 = TIGlobalResearchState.FinishedTechs();
			foreach (ProjectTrigger projectTrigger in this.activeProjectTriggers)
			{
				TIProjectTemplate projectTemplate = projectTrigger.projectTemplate;
				if (projectTemplate != null && projectTemplate.PrereqsSatisfied(list2, this.completedProjects, this))
				{
					float num = Mathf.Clamp(projectTrigger.monthlyTriggerValue / 100f, 0f, 1f);
					if (num > 0f)
					{
						float num2 = Mathf.Clamp(-(Mathf.Pow(1f - num, 0.032854885f) - 1f), 0f, 1f);
						if (TIUtilities.RandomFloatValue() <= num2)
						{
							list.Add(projectTrigger);
						}
					}
				}
			}
			foreach (ProjectTrigger projectTrigger2 in list)
			{
				if (this.AddAvailableProject(projectTrigger2.projectTemplate, projectTrigger2))
				{
					TINotificationQueueState.LogProjectTriggered(this, projectTrigger2.projectTemplate, projectTrigger2.projectTemplate.factionAvailableChance < 100f && projectTrigger2.projectTemplate.factionAlways != this.templateName);
				}
			}
		}

		// Token: 0x0600331E RID: 13086 RVA: 0x001175B4 File Offset: 0x001157B4
		public void MonthlyProjectTriggerChanceChange()
		{
			List<TITechTemplate> list = TIGlobalResearchState.FinishedTechs();
			List<ProjectTrigger> list2 = new List<ProjectTrigger>();
			foreach (ProjectTrigger projectTrigger in this.activeProjectTriggers)
			{
				ProjectTrigger projectTrigger2 = projectTrigger;
				TIProjectTemplate projectTemplate = projectTrigger.projectTemplate;
				if (projectTemplate != null && projectTemplate.PrereqsSatisfied(list, this.completedProjects, this) && projectTrigger.monthlyTriggerValue < projectTrigger.projectTemplate.maxUnlockChance)
				{
					float num = projectTrigger2.projectTemplate.deltaUnlockChance * TIGlobalValuesState.GetResearchSpeedModifier();
					num += TIEffectsState.SumEffectsModifiers(Context.MonthlyProjectTriggerChance, this, num, null);
					projectTrigger2.monthlyTriggerValue = Mathf.Clamp(projectTrigger2.monthlyTriggerValue + projectTrigger2.projectTemplate.deltaUnlockChance + num, projectTrigger2.projectTemplate.initialUnlockChance, projectTrigger2.projectTemplate.maxUnlockChance);
				}
				if (projectTrigger2.monthlyTriggerValue > 0f)
				{
					list2.Add(projectTrigger2);
				}
			}
			this.activeProjectTriggers = list2;
		}

		// Token: 0x0600331F RID: 13087 RVA: 0x001176C0 File Offset: 0x001158C0
		public bool HasObjectiveProjectAvailable()
		{
			return this.availableProjects.Any<TIProjectTemplate>((TIProjectTemplate x) => x.AI_projectRole == ProjectRole.Objective);
		}

		// Token: 0x06003320 RID: 13088 RVA: 0x001176EC File Offset: 0x001158EC
		public void AddFleet(TISpaceFleetState fleet)
		{
			TIFactionState faction = fleet.faction;
			this.fleets.Add(fleet);
			fleet.SetFaction(this);
			this.fleetGoalTracker.Add(fleet, null);
			GameControl.eventManager.TriggerEvent(new FleetCoreStatusChange(fleet), null, new object[] { this, fleet, faction, fleet.location, fleet.barycenter }.Where<object>((object x) => x != null).ToArray<object>());
		}

		// Token: 0x06003321 RID: 13089 RVA: 0x00117780 File Offset: 0x00115980
		public void RemoveFleet(TISpaceFleetState fleet)
		{
			if (this.fleets.Contains(fleet))
			{
				if (!GameControl.control.skirmishMode)
				{
					this.fleetGoalsDirty = true;
					foreach (FactionGoal_Fleet factionGoal_Fleet in this.AllFleetGoals(false))
					{
						if (factionGoal_Fleet.pendingFleets.Contains(fleet))
						{
							factionGoal_Fleet.RemovePendingFleet(fleet);
						}
					}
					if (this.fleetGoalTracker.ContainsKey(fleet))
					{
						FactionGoal_Fleet factionGoal_Fleet2 = this.fleetGoalTracker[fleet];
						if (factionGoal_Fleet2 != null)
						{
							factionGoal_Fleet2.UnassignFleet();
						}
						this.fleetGoalTracker.Remove(fleet);
					}
				}
				this.fleets.Remove(fleet);
				GameControl.eventManager.TriggerEvent(new FleetCoreStatusChange(fleet), null, new object[] { this, fleet, fleet.location, fleet.barycenter }.Where<object>((object x) => x != null).ToArray<object>());
				fleet.SetFaction(null);
				return;
			}
			Throw.CollectionItemMissing<TISpaceFleetState>(this, fleet, this.fleets, "fleets");
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06003322 RID: 13090 RVA: 0x001178B4 File Offset: 0x00115AB4
		public IEnumerable<TISpaceShipState> combatShips
		{
			get
			{
				return from x in this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
					where x.combatant
					select x;
			}
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06003323 RID: 13091 RVA: 0x00117910 File Offset: 0x00115B10
		public IEnumerable<TISpaceShipState> noncombatShips
		{
			get
			{
				return from x in this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
					where x.nonCombatant
					select x;
			}
		}

		// Token: 0x06003324 RID: 13092 RVA: 0x0011796C File Offset: 0x00115B6C
		public void DailyFleetsUpdate()
		{
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddDays(-1f);
			for (int i = 0; i < this.fleets.Count; i++)
			{
				TISpaceFleetState tispaceFleetState = this.fleets[i];
				if (tispaceFleetState.exists && tispaceFleetState.currentOperations.Count > 0)
				{
					foreach (OperationData operationData in tispaceFleetState.CurrentOperations())
					{
						if (operationData.completionDate < tidateTime)
						{
							OperationTiming operationTiming = operationData.operation.GetOperationTiming();
							if (operationTiming != OperationTiming.InstantExecution)
							{
								if (operationTiming - OperationTiming.DelayedExecutionOfInstantEffect <= 1)
								{
									tispaceFleetState.CompleteFleetOperation(operationData.operation, operationData.target);
									Log.Info("Executing overdue delayed operation " + operationData.operationDataName + " for " + tispaceFleetState.ID.ToString(), Array.Empty<object>());
								}
							}
							else
							{
								Log.Info("Cancelling overdue instant operation " + operationData.operationDataName + " for " + tispaceFleetState.ID.ToString(), Array.Empty<object>());
								tispaceFleetState.CancelOperation(operationData);
							}
						}
					}
				}
				for (int j = 0; j < tispaceFleetState.ships.Count; j++)
				{
					if (this.fleets[i].ships[j].propulsionValuesDataDirty)
					{
						this.fleets[i].ships[j].SetPropulsionValuesDirty(true, false);
					}
				}
				if (TITimeState.Now().day == 1)
				{
					if (tispaceFleetState.inTransfer)
					{
						tispaceFleetState.unreachableLocations.Clear();
					}
					FactionGoal_Fleet factionGoal_Fleet = tispaceFleetState.AssignedGoal();
					if (TITimeState.Now().month % 3 == 0 && factionGoal_Fleet != null && !(factionGoal_Fleet is FactionGoal_FixUpFleet))
					{
						TIOrbitState ref_orbit = tispaceFleetState.ref_orbit;
						if (ref_orbit != null && ref_orbit.isAdHocOrbit)
						{
							tispaceFleetState.AssignedGoal().UnassignFleet();
						}
					}
				}
			}
			float targetDesiredStaticFleetFraction = AIEvaluators.GetTargetDesiredStaticFleetFraction(this);
			float num = 0.0075f;
			if (this.desiredStaticFleetFraction < targetDesiredStaticFleetFraction)
			{
				num *= 3f;
			}
			this.desiredStaticFleetFraction = Mathf.Lerp(this.desiredStaticFleetFraction, targetDesiredStaticFleetFraction, num);
		}

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06003325 RID: 13093 RVA: 0x00117BD0 File Offset: 0x00115DD0
		public float FleetDryMass_tons
		{
			get
			{
				return this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Sum<TISpaceShipState>((TISpaceShipState x) => x.template.dryMass_tons(false));
			}
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06003326 RID: 13094 RVA: 0x00117C2C File Offset: 0x00115E2C
		public float FleetWetMass_tons
		{
			get
			{
				return this.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Sum<TISpaceShipState>((TISpaceShipState x) => x.template.wetMass_tons);
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06003327 RID: 13095 RVA: 0x00117C88 File Offset: 0x00115E88
		public float FutureFleetWetMass_tons
		{
			get
			{
				return this.FleetWetMass_tons + this.nShipyardQueues.SelectMany<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>, ShipConstructionQueueItem>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value).Sum<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => x.shipDesign.wetMass_tons);
			}
		}

		// Token: 0x06003328 RID: 13096 RVA: 0x00117CEC File Offset: 0x00115EEC
		public TIResourcesCost GetAverageShipBuildCost()
		{
			IEnumerable<TISpaceShipTemplate> enumerable = this.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template);
			if (!enumerable.Any<TISpaceShipTemplate>())
			{
				enumerable = this.shipDesigns;
			}
			float num = -1f;
			if (this.averageShipBuildCostCachedDate != null)
			{
				num = (float)(TITimeState.Now() - this.averageShipBuildCostCachedDate).TotalDays;
			}
			if (num < 0f || num > 30f || this.cachedAverageShipBuildCost == null || this.cachedAverageShipBuildCost.resourceCosts.Count == 0)
			{
				if (enumerable.Any<TISpaceShipTemplate>())
				{
					this.cachedAverageShipBuildCost = new TIResourcesCost();
					using (IEnumerator<TISpaceShipTemplate> enumerator = enumerable.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceShipTemplate tispaceShipTemplate = enumerator.Current;
							this.cachedAverageShipBuildCost.SumCosts_NoDuration(tispaceShipTemplate.spaceResourceConstructionCost(false, null, true, false, false).MultiplyCost(1f / (float)enumerable.Count<TISpaceShipTemplate>()));
						}
						goto IL_015A;
					}
				}
				this.cachedAverageShipBuildCost = new TIResourcesCost();
				if (this.IsAlienFaction)
				{
					throw new NotImplementedException();
				}
				this.cachedAverageShipBuildCost.AddCost(FactionResource.Water, 180f, true);
				this.cachedAverageShipBuildCost.AddCost(FactionResource.Volatiles, 70f, true);
				this.cachedAverageShipBuildCost.AddCost(FactionResource.Metals, 70f, true);
				this.cachedAverageShipBuildCost.AddCost(FactionResource.NobleMetals, 20f, true);
				IL_015A:
				this.averageShipBuildCostCachedDate = TITimeState.Now();
			}
			return this.cachedAverageShipBuildCost;
		}

		// Token: 0x06003329 RID: 13097 RVA: 0x00117E74 File Offset: 0x00116074
		public TIResourcesCost GetTypicalShipBuildCost()
		{
			return this.GetAverageShipBuildCost();
		}

		// Token: 0x0600332A RID: 13098 RVA: 0x00117E7C File Offset: 0x0011607C
		public TIResourcesCost GetTypicalShipBuildCostSansRareMaterials()
		{
			TIResourcesCost averageShipBuildCost = this.GetAverageShipBuildCost();
			averageShipBuildCost.RemoveCost(FactionResource.Fissiles);
			averageShipBuildCost.RemoveCost(FactionResource.Antimatter);
			averageShipBuildCost.RemoveCost(FactionResource.Exotics);
			return averageShipBuildCost;
		}

		// Token: 0x0600332B RID: 13099 RVA: 0x00117E9C File Offset: 0x0011609C
		public TIResourcesCost GetTypicalShipFuelCostsPerKps()
		{
			IEnumerable<TISpaceShipTemplate> enumerable = this.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template);
			if (!enumerable.Any<TISpaceShipTemplate>())
			{
				enumerable = this.shipDesigns;
			}
			float num = -1f;
			if (this.averageShipFuelCostCachedDate != null)
			{
				num = (float)(TITimeState.Now() - this.averageShipFuelCostCachedDate).TotalDays;
			}
			if (num < 0f || num > 30f || this.cachedAverageShipFuelCost == null || this.cachedAverageShipFuelCost.resourceCosts.Count == 0)
			{
				if (enumerable.Any<TISpaceShipTemplate>())
				{
					this.cachedAverageShipFuelCost = new TIResourcesCost();
					using (IEnumerator<TISpaceShipTemplate> enumerator = enumerable.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceShipTemplate tispaceShipTemplate = enumerator.Current;
							TIResourcesCost tiresourcesCost = tispaceShipTemplate.propellantTanksBuildCost(this).MultiplyCost(1f / tispaceShipTemplate.baseCruiseDeltaV_kps(false));
							this.cachedAverageShipFuelCost.SumCosts_NoDuration(tiresourcesCost.MultiplyCost(1f / (float)enumerable.Count<TISpaceShipTemplate>()));
						}
						goto IL_0142;
					}
				}
				this.cachedAverageShipFuelCost = new TIResourcesCost();
				if (this.IsAlienFaction)
				{
					this.cachedAverageShipFuelCost.AddCost(FactionResource.Water, 0.6f, true);
				}
				else
				{
					this.cachedAverageShipFuelCost.AddCost(FactionResource.Water, 30f, true);
				}
				IL_0142:
				this.averageShipFuelCostCachedDate = TITimeState.Now();
			}
			return this.cachedAverageShipFuelCost;
		}

		// Token: 0x0600332C RID: 13100 RVA: 0x0011800C File Offset: 0x0011620C
		public TIResourcesCost GetTypicalShipFuelCostPerKPSSansRareMaterials()
		{
			TIResourcesCost typicalShipFuelCostsPerKps = this.GetTypicalShipFuelCostsPerKps();
			typicalShipFuelCostsPerKps.RemoveCost(FactionResource.Fissiles);
			typicalShipFuelCostsPerKps.RemoveCost(FactionResource.Antimatter);
			typicalShipFuelCostsPerKps.RemoveCost(FactionResource.Exotics);
			return typicalShipFuelCostsPerKps;
		}

		// Token: 0x0600332D RID: 13101 RVA: 0x0011802C File Offset: 0x0011622C
		public float GetTypicalShipSpaceCombatValue()
		{
			IEnumerable<TISpaceShipState> enumerable;
			if (!this.combatShips.Any<TISpaceShipState>())
			{
				IEnumerable<TISpaceShipState> ships = this.ships;
				enumerable = ships;
			}
			else
			{
				enumerable = this.combatShips;
			}
			IEnumerable<TISpaceShipState> enumerable2 = enumerable;
			if (enumerable2.Any<TISpaceShipState>())
			{
				return enumerable2.Average<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
			}
			return 0f;
		}

		// Token: 0x0600332E RID: 13102 RVA: 0x0011808C File Offset: 0x0011628C
		public float GetTypicalShipMissionControlConsumption()
		{
			IEnumerable<TISpaceShipState> enumerable;
			if (!this.combatShips.Any<TISpaceShipState>())
			{
				IEnumerable<TISpaceShipState> ships = this.ships;
				enumerable = ships;
			}
			else
			{
				enumerable = this.combatShips;
			}
			IEnumerable<TISpaceShipState> enumerable2 = enumerable;
			if (enumerable2.Any<TISpaceShipState>())
			{
				return enumerable2.Average<TISpaceShipState>((TISpaceShipState x) => (float)x.missionControlConsumption);
			}
			return 1f;
		}

		// Token: 0x0600332F RID: 13103 RVA: 0x001180EC File Offset: 0x001162EC
		public float GetTypicalShipBombardmentValue(TISpaceBodyState spaceBody)
		{
			IEnumerable<TISpaceShipState> enumerable;
			if (!this.combatShips.Any<TISpaceShipState>())
			{
				IEnumerable<TISpaceShipState> ships = this.ships;
				enumerable = ships;
			}
			else
			{
				enumerable = this.combatShips;
			}
			IEnumerable<TISpaceShipState> enumerable2 = enumerable;
			if (enumerable2.Any<TISpaceShipState>())
			{
				return enumerable2.Average<TISpaceShipState>((TISpaceShipState x) => x.BombardmentValue(spaceBody));
			}
			return 0f;
		}

		// Token: 0x06003330 RID: 13104 RVA: 0x00118144 File Offset: 0x00116344
		public static IEnumerable<TISpaceFleetState> GetDefenders(TISpaceObjectState primaryDefender)
		{
			IEnumerable<TISpaceFleetState> enumerable = Enumerable.Empty<TISpaceFleetState>();
			if (primaryDefender.isHabState)
			{
				enumerable = enumerable.Concat<TISpaceFleetState>(primaryDefender.ref_hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(primaryDefender.ref_hab.faction)));
			}
			if (primaryDefender.isSpaceFleetState)
			{
				enumerable = enumerable.Append(primaryDefender.ref_fleet);
			}
			if (primaryDefender.ref_orbit != null)
			{
				enumerable = enumerable.Concat<TISpaceFleetState>(from x in primaryDefender.ref_orbit.fleetsInOrbit
					where !x.dockedAtHab
					where x.faction == primaryDefender.ref_faction || (x.faction.permanentAlly(primaryDefender.ref_faction) && x.faction.player.isAI && primaryDefender.ref_faction.player.isAI)
					select x);
			}
			return (from x in enumerable.Distinct<TISpaceFleetState>()
				where !x.landed
				where TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(primaryDefender, x) < 1000000.0
				select x).ToList<TISpaceFleetState>();
		}

		// Token: 0x06003331 RID: 13105 RVA: 0x00118259 File Offset: 0x00116459
		public IEnumerable<TISpaceFleetState> GetAttackers(TISpaceFleetState primaryAttacker)
		{
			return Enumerable.Repeat<TISpaceFleetState>(primaryAttacker, 1);
		}

		// Token: 0x06003332 RID: 13106 RVA: 0x00118262 File Offset: 0x00116462
		public float GetDesiredStaticFleetFraction()
		{
			return this.desiredStaticFleetFraction;
		}

		// Token: 0x06003333 RID: 13107 RVA: 0x0011826C File Offset: 0x0011646C
		public void AddCombatLog(TIFactionState.CombatLog combatLog)
		{
			List<TIFactionState.CombatLog.Attack> list = combatLog.Attacks.OrderBy<TIFactionState.CombatLog.Attack, float>((TIFactionState.CombatLog.Attack x) => TIUtilities.RandomFloatValue()).ToList<TIFactionState.CombatLog.Attack>();
			if (list.Count > 100)
			{
				list = list.GetRange(0, 100);
			}
			combatLog.SetAttacks(list);
			this.CombatLogs.Add(combatLog);
			for (;;)
			{
				if (this.CombatLogs.Sum<TIFactionState.CombatLog>((TIFactionState.CombatLog x) => x.Attacks.Count<TIFactionState.CombatLog.Attack>()) <= 5000)
				{
					break;
				}
				this.CombatLogs.RemoveAt(0);
			}
		}

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06003334 RID: 13108 RVA: 0x0011830F File Offset: 0x0011650F
		public List<TIHabState> habs
		{
			get
			{
				return this.habSectors.Select<TISectorState, TIHabState>((TISectorState x) => x.hab).Distinct<TIHabState>().ToList<TIHabState>();
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06003335 RID: 13109 RVA: 0x00118348 File Offset: 0x00116548
		public List<TIHabState> stations
		{
			get
			{
				return (from x in this.habSectors.Select<TISectorState, TIHabState>((TISectorState x) => x.hab).Distinct<TIHabState>()
					where x.IsStation
					select x).ToList<TIHabState>();
			}
		}

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06003336 RID: 13110 RVA: 0x001183B0 File Offset: 0x001165B0
		public List<TIHabState> bases
		{
			get
			{
				return (from x in this.habSectors.Select<TISectorState, TIHabState>((TISectorState x) => x.hab).Distinct<TIHabState>()
					where x.IsBase
					select x).ToList<TIHabState>();
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06003337 RID: 13111 RVA: 0x00118415 File Offset: 0x00116615
		public List<TIHabModuleState> habModules
		{
			get
			{
				return this.habSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules).ToList<TIHabModuleState>();
			}
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06003338 RID: 13112 RVA: 0x00118446 File Offset: 0x00116646
		public List<TIHabModuleState> activeHabModules
		{
			get
			{
				return this.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.active).ToList<TIHabModuleState>();
			}
		}

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06003339 RID: 13113 RVA: 0x00118478 File Offset: 0x00116678
		public bool needsPrimaryHab
		{
			get
			{
				TIObjectiveTemplate tiobjectiveTemplate = (from x in this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked)
					where x.targetHabModuleTemplate != null
					select x).FirstOrDefault<TIObjectiveTemplate>();
				return tiobjectiveTemplate != null && (this.primaryHab == null || this.primaryHab.location != tiobjectiveTemplate.targetHabLocationState);
			}
		}

		// Token: 0x0600333A RID: 13114 RVA: 0x001184E4 File Offset: 0x001166E4
		public List<TIHabState> ShipConstructionHabs(bool includeInactives, bool includeUnderConstruction = false)
		{
			return this.habs.Where<TIHabState>((TIHabState x) => x.AllowsShipConstruction(this, includeInactives, includeUnderConstruction)).ToList<TIHabState>();
		}

		// Token: 0x0600333B RID: 13115 RVA: 0x00118528 File Offset: 0x00116728
		public IEnumerable<TIHabState> ResupplyHabs(bool includeInactives, bool includeTheft = false)
		{
			IEnumerable<TIHabState> enumerable = this.habs.Where<TIHabState>((TIHabState x) => x.AllowsResupply(this, false, includeInactives));
			if (includeTheft)
			{
				enumerable = enumerable.Union<TIHabState>(from x in GameStateManager.IterateByClass<TIHabState>(false)
					where x.faction != this && x.AllowsResupply(this, true, false)
					select x);
			}
			return enumerable;
		}

		// Token: 0x0600333C RID: 13116 RVA: 0x00118584 File Offset: 0x00116784
		public bool CanBuildShipsAtLocation(TIGameState location, bool includeInactives, bool includeUnderConstruction)
		{
			return this.ShipConstructionHabs(includeInactives, includeUnderConstruction).Count<TIHabState>((TIHabState x) => x == location || x.location == location) > 0;
		}

		// Token: 0x0600333D RID: 13117 RVA: 0x001185BC File Offset: 0x001167BC
		public bool CanResupplyShipsAtLocation(TIGameState location, bool includeInactives)
		{
			return this.ResupplyHabs(includeInactives, false).Count<TIHabState>((TIHabState x) => x == location || x.location == location) > 0;
		}

		// Token: 0x0600333E RID: 13118 RVA: 0x001185F4 File Offset: 0x001167F4
		public bool CanFoundHabFromHabAtLocation(TIGameState location, bool includeInactives = false, bool includeUnderConstruction = false)
		{
			List<TIHabState> habs = this.habs;
			TISpaceObjectState getSunOrbitingRelatedObject = location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
			foreach (TIHabState tihabState in habs)
			{
				TISpaceObjectState getSunOrbitingRelatedObject2 = tihabState.GetSunOrbitingRelatedObject;
				if (getSunOrbitingRelatedObject == getSunOrbitingRelatedObject2 && tihabState.HabConstructHabOptions(this, includeInactives, includeUnderConstruction).Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600333F RID: 13119 RVA: 0x00118678 File Offset: 0x00116878
		public int MaxTierCanFoundAtLocation(TIGameState location, bool includeInactives = false, bool includeUnderConstruction = false)
		{
			List<TIHabState> habs = this.habs;
			TISpaceObjectState getSunOrbitingRelatedObject = location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
			int num = 0;
			foreach (TIHabState tihabState in habs)
			{
				TISpaceObjectState getSunOrbitingRelatedObject2 = tihabState.GetSunOrbitingRelatedObject;
				if (getSunOrbitingRelatedObject == getSunOrbitingRelatedObject2)
				{
					List<HabModuleSpecialRule> list = tihabState.HabConstructHabOptions(this, includeInactives, includeUnderConstruction);
					if (list.Count > 0)
					{
						if (list.Contains(HabModuleSpecialRule.CanFoundTier3Habs))
						{
							return 3;
						}
						if (list.Contains(HabModuleSpecialRule.CanFoundTier2Habs))
						{
							num = 2;
						}
						else if (num == 0 && list.Contains(HabModuleSpecialRule.CanFoundTier1Habs))
						{
							num = 1;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06003340 RID: 13120 RVA: 0x0011872C File Offset: 0x0011692C
		public void SetHe3Access()
		{
			bool he3Access = this.He3Access;
			this.He3Access = this.activeHabModules.Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.HarvestHelium3));
			if (he3Access != this.He3Access)
			{
				this.shipDesigns.ForEach(delegate(TISpaceShipTemplate x)
				{
					x.spaceResourceConstructionCost(true, null, true, false, false);
				});
			}
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x06003341 RID: 13121 RVA: 0x001187A4 File Offset: 0x001169A4
		public List<TIHabModuleState> shipConstructionModules
		{
			get
			{
				List<TIHabModuleState> list = new List<TIHabModuleState>();
				foreach (TISectorState tisectorState in this.habSectors)
				{
					foreach (TIHabModuleState tihabModuleState in tisectorState.habModules)
					{
						if (tihabModuleState.moduleTemplate != null && tihabModuleState.moduleTemplate.allowsShipConstruction && tihabModuleState.constructionCompleted)
						{
							list.Add(tihabModuleState);
						}
					}
				}
				return list;
			}
		}

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06003342 RID: 13122 RVA: 0x00118854 File Offset: 0x00116A54
		public List<TIHabState> LEOStations
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TIHabState tihabState in this.habs)
				{
					if (tihabState.inEarthLEO)
					{
						list.Add(tihabState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06003343 RID: 13123 RVA: 0x001188B8 File Offset: 0x00116AB8
		public List<TIHabState> EarthSystemStations
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TIHabState tihabState in this.habs)
				{
					if (!list.Contains(tihabState) && tihabState.IsStation && (tihabState.orbitState.barycenter.isEarth || (tihabState.orbitState.barycenter.barycenter != null && tihabState.orbitState.barycenter.barycenter.isEarth)))
					{
						list.Add(tihabState);
					}
				}
				return list;
			}
		}

		// Token: 0x06003344 RID: 13124 RVA: 0x00118964 File Offset: 0x00116B64
		public List<TIHabState> MyHabsAtLocation(TIGameState location)
		{
			List<TIHabState> list = new List<TIHabState>();
			if (location.isHabSiteState)
			{
				TIHabState ref_hab = location.ref_habSite.ref_hab;
				if (ref_hab != null && ref_hab.ref_factions.Contains(this))
				{
					list.Add(location.ref_habSite.ref_hab);
				}
			}
			else
			{
				if (location.isOrbitState)
				{
					using (List<TIHabState>.Enumerator enumerator = location.ref_orbit.stationsInOrbit.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIHabState tihabState = enumerator.Current;
							if (tihabState.ref_factions.Contains(this))
							{
								list.Add(tihabState);
							}
						}
						return list;
					}
				}
				if (location.isNaturalSpaceObjectState)
				{
					if (location.isSpaceBodyState)
					{
						list.AddRange(location.ref_spaceBody.surfaceBases.Where<TIHabState>((TIHabState x) => x.ref_factions.Contains(this)));
					}
					list.AddRange(location.ref_naturalSpaceObject.orbits.SelectMany<TIOrbitState, TIHabState>((TIOrbitState x) => x.stationsInOrbit.Where<TIHabState>((TIHabState y) => y.ref_factions.Contains(this))));
				}
			}
			return list;
		}

		// Token: 0x06003345 RID: 13125 RVA: 0x00118A70 File Offset: 0x00116C70
		public TIHabState GetMainBaseInSystem(TISpaceBodyState system)
		{
			if (system == null)
			{
				return null;
			}
			if (this.primaryHab != null && this.primaryHab.IsBase && this.primaryHab.ref_system == system)
			{
				return this.primaryHab;
			}
			return this.bases.Where<TIHabState>((TIHabState x) => x.ref_system == system).FirstOrDefault<TIHabState>();
		}

		// Token: 0x06003346 RID: 13126 RVA: 0x00118AF0 File Offset: 0x00116CF0
		public TISpaceBodyState GetInnermostColonizedPlanet()
		{
			return (from x in this.bases
				group x by x.ref_system into x
				select x.Key into x
				where x.objectType == SpaceObjectType.Planet
				orderby x.semiMajorAxis_AU
				select x).FirstOrDefault<TISpaceBodyState>();
		}

		// Token: 0x06003347 RID: 13127 RVA: 0x00118B98 File Offset: 0x00116D98
		public void NeverForget(TIHabState destroyedHab, TIFactionState destroyer = null)
		{
			this.HabDestructionLog.Add(new TIFactionState.HabDestructionLogEntry
			{
				HabType = destroyedHab.habType,
				SpaceBody = destroyedHab.ref_spaceBody,
				Date = TITimeState.Now(),
				Destroyer = destroyer
			});
		}

		// Token: 0x06003348 RID: 13128 RVA: 0x00118BE8 File Offset: 0x00116DE8
		public IEnumerable<TIFactionState.HabDestructionLogEntry> GetHabDestructions(TISpaceBodyState spaceBody, HabType habType = HabType.Any)
		{
			return this.HabDestructionLog.Where<TIFactionState.HabDestructionLogEntry>((TIFactionState.HabDestructionLogEntry x) => x.SpaceBody == spaceBody);
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06003349 RID: 13129 RVA: 0x00118C1C File Offset: 0x00116E1C
		public List<TIHabState> HabCores
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TISectorState tisectorState in this.habSectors)
				{
					if (tisectorState.coreSector)
					{
						list.Add(tisectorState.hab);
					}
				}
				return list;
			}
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x0600334A RID: 13130 RVA: 0x00118C84 File Offset: 0x00116E84
		public int MaxStationTier
		{
			get
			{
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanFoundTier3Station, this))
				{
					return 3;
				}
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanFoundTier2Station, this))
				{
					return 2;
				}
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanFoundTier1Station, this))
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x0600334B RID: 13131 RVA: 0x00118CB4 File Offset: 0x00116EB4
		public int MaxBaseTier
		{
			get
			{
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanFoundTier3Base, this))
				{
					return 3;
				}
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanFoundTier2Base, this))
				{
					return 2;
				}
				if (TIEffectsState.CheckForAnyEffectInContext(Context.CanFoundTier1Base, this))
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x0600334C RID: 13132 RVA: 0x00118CE4 File Offset: 0x00116EE4
		public bool MaxedOutHabForFaction(TIHabState hab)
		{
			TIFactionGoalState tifactionGoalState = this.FindGoals(TIFactionGoalState.BuildHabGoals, this, hab, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>();
			GoalType? goalType = ((tifactionGoalState != null) ? new GoalType?(tifactionGoalState.GetGoalType()) : null);
			if (goalType != null)
			{
				GoalType valueOrDefault = goalType.GetValueOrDefault();
				if (valueOrDefault == GoalType.BuildMiningBase)
				{
					TIHabModuleState tihabModuleState = hab.sectors[0].habModules[1];
					return tihabModuleState.hasModule && tihabModuleState.powered && !tihabModuleState.moduleTemplate.CanUpgrade(this);
				}
				if (valueOrDefault == GoalType.BuildRefuellingStation)
				{
					return hab.AllowsResupply(hab.coreFaction, false, false);
				}
			}
			foreach (TIHabModuleState tihabModuleState2 in hab.AllModules())
			{
				if (tihabModuleState2.empty || tihabModuleState2.destroyed || tihabModuleState2.moduleTemplate.CanUpgrade(this))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600334D RID: 13133 RVA: 0x00118DF0 File Offset: 0x00116FF0
		public List<TIHabState> MaxedOutHabsForFaction(HabType habType)
		{
			List<TIHabState> list = new List<TIHabState>();
			IEnumerable<TIHabState> habs = this.habs;
			Func<TIHabState, bool> <>9__0;
			Func<TIHabState, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TIHabState x) => x.habType == habType || habType == HabType.Any);
			}
			foreach (TIHabState tihabState in habs.Where<TIHabState>(func))
			{
				if (this.MaxedOutHabForFaction(tihabState))
				{
					list.Add(tihabState);
				}
			}
			return list;
		}

		// Token: 0x0600334E RID: 13134 RVA: 0x00118E84 File Offset: 0x00117084
		public void SaveHabDesign(TIHabTemplate habDesign)
		{
			TemplateManager.Add(habDesign, typeof(TIHabTemplate), true);
			this.habDesigns.Add(habDesign);
			this.savedHabDesigns++;
		}

		// Token: 0x0600334F RID: 13135 RVA: 0x00118EB4 File Offset: 0x001170B4
		public void DeleteHabDesign(string dataName)
		{
			TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(dataName, false);
			if (this.habDesigns.Contains(tihabTemplate))
			{
				this.habDesigns.Remove(tihabTemplate);
				TemplateManager.Remove<TIHabTemplate>(tihabTemplate);
			}
		}

		// Token: 0x06003350 RID: 13136 RVA: 0x00118EEC File Offset: 0x001170EC
		public bool IsDuplicateHabDesign(TIHabTemplate designToTest)
		{
			foreach (TIHabTemplate tihabTemplate in this.habDesigns)
			{
				if (tihabTemplate.dataName == designToTest.dataName)
				{
					return true;
				}
				bool flag = false;
				for (int i = 0; i < tihabTemplate.sectors.Length; i++)
				{
					for (int j = 0; j < tihabTemplate.sectors[i].habModuleNames.Length; j++)
					{
						if (tihabTemplate.sectors[i].habModuleNames[j] != designToTest.sectors[i].habModuleNames[j])
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003351 RID: 13137 RVA: 0x00118FD8 File Offset: 0x001171D8
		public List<ShipConstructionQueueItem> GetShipyardQueue(TIHabModuleState shipyard)
		{
			return this.nShipyardQueues[shipyard];
		}

		// Token: 0x06003352 RID: 13138 RVA: 0x00118FE8 File Offset: 0x001171E8
		public void AddShipyardToFaction(TIHabModuleState candidateShipyardModule, bool startup = false)
		{
			if (candidateShipyardModule.moduleTemplate.allowsShipConstruction && !this.nShipyardQueues.ContainsKey(candidateShipyardModule))
			{
				this.nShipyardQueues.Add(candidateShipyardModule, new List<ShipConstructionQueueItem>());
				candidateShipyardModule.shipyardAllowPayFromEarth = false;
				foreach (TISpaceFleetState tispaceFleetState in this.fleets)
				{
					if (!tispaceFleetState.inTransfer && tispaceFleetState.ref_system != null && tispaceFleetState.ref_system == candidateShipyardModule.ref_system)
					{
						FactionGoal_Fleet factionGoal_Fleet = tispaceFleetState.AssignedGoal();
						if (factionGoal_Fleet is FactionGoal_FixUpFleet)
						{
							TIGameState tigameState = factionGoal_Fleet.target();
							if (((tigameState != null) ? tigameState.ref_system : null) != tispaceFleetState.ref_system)
							{
								factionGoal_Fleet.UnassignFleet();
								factionGoal_Fleet.SetImportance(0);
							}
						}
					}
				}
			}
			if (TIGlobalValuesState.isTutorialActive && !startup)
			{
				this.CompleteMilestone(CampaignMilestone.TutorialSpaceDock);
			}
		}

		// Token: 0x06003353 RID: 13139 RVA: 0x001190E4 File Offset: 0x001172E4
		public void RemoveShipyardFromFaction(TIHabModuleState shipyard, bool peaceful)
		{
			if (this.nShipyardQueues.ContainsKey(shipyard))
			{
				if (this.nShipyardQueues[shipyard].Count > 0)
				{
					foreach (ShipConstructionQueueItem shipConstructionQueueItem in new List<ShipConstructionQueueItem>(this.nShipyardQueues[shipyard]))
					{
						if (peaceful && shipConstructionQueueItem.costPaid)
						{
							shipConstructionQueueItem.resourcesCost.RefundCost(this, "Shipyard Removal Refund");
							shipConstructionQueueItem.costPaid = false;
						}
						if (shipConstructionQueueItem.isRefit && shipConstructionQueueItem.originalSpaceShipState != null && !shipConstructionQueueItem.originalSpaceShipState.deleted)
						{
							if (peaceful)
							{
								this.CompleteShipConstruction(shipyard, true, shipConstructionQueueItem);
							}
							else
							{
								shipConstructionQueueItem.originalSpaceShipState.DestroyShip(true, null);
							}
						}
						this.nShipyardQueues[shipyard].Remove(shipConstructionQueueItem);
						GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, shipyard, shipConstructionQueueItem), null, new object[] { this, shipyard });
					}
					this.nShipyardQueues[shipyard].Clear();
				}
				if (this.AISavingTarget.active && this.AISavingTarget.desiredPurchase is TISpaceShipTemplate && this.AISavingTarget.location == shipyard)
				{
					this.AIClearSavingTarget("Lost Shipyard");
				}
				this.nShipyardQueues.Remove(shipyard);
			}
		}

		// Token: 0x06003354 RID: 13140 RVA: 0x0011925C File Offset: 0x0011745C
		public bool AddShipToShipyardQueue(TIHabModuleState shipyard, TISpaceShipTemplate shipClass, bool allowPayFromEarth, float fractionWillingToSpend = 1f, FactionGoal_Fleet intendedGoal = null, bool isRefit = false, TISpaceShipTemplate originalShipDesign = null, TISpaceShipState originalShipState = null)
		{
			TIResourcesCost tiresourcesCost = null;
			TIResourcesCost tiresourcesCost2;
			if (isRefit)
			{
				tiresourcesCost2 = shipClass.RefitResourceCost(shipyard, originalShipDesign, true, true, originalShipState);
				tiresourcesCost2.GetRefundCost(out tiresourcesCost);
			}
			else
			{
				tiresourcesCost2 = shipClass.spaceResourceConstructionCost(false, shipyard, true, false, false);
			}
			if (!tiresourcesCost2.CanAfford(shipyard.sector.faction, fractionWillingToSpend, null, float.PositiveInfinity) && allowPayFromEarth)
			{
				tiresourcesCost2 = TISpaceShipTemplate.MixedResourceConstructionCost(shipyard.sector.faction, shipyard.hab, tiresourcesCost2, this.AvailableSpaceResources(fractionWillingToSpend), false);
			}
			ShipConstructionQueueItem shipConstructionQueueItem = new ShipConstructionQueueItem(shipClass, shipyard, new TIDateTime(), tiresourcesCost2, intendedGoal, isRefit, originalShipDesign, originalShipState, tiresourcesCost);
			this.nShipyardQueues[shipyard].Add(shipConstructionQueueItem);
			return this.nShipyardQueues[shipyard].Count != 1 || !this.isActivePlayer || this.AttemptInitiateShipConstruction(shipyard);
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x00119324 File Offset: 0x00117524
		public void RemoveShipFromShipyardQueue(TIHabModuleState shipyard, ShipConstructionQueueItem item)
		{
			if (item.costPaid)
			{
				item.resourcesCost.RefundCost(this, "Cancel Ship");
				item.costPaid = false;
			}
			this.nShipyardQueues[shipyard].Remove(item);
			GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, shipyard, item), null, new object[] { this, shipyard });
		}

		// Token: 0x06003356 RID: 13142 RVA: 0x00119388 File Offset: 0x00117588
		public void RepositionShipinShipyardQueue(TIHabModuleState shipyard, ShipConstructionQueueItem item, int newIndex)
		{
			int num = this.nShipyardQueues[shipyard].IndexOf(item);
			this.nShipyardQueues[shipyard].RemoveAt(num);
			this.nShipyardQueues[shipyard].Insert(newIndex, item);
			GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, shipyard, item), null, new object[] { this, shipyard });
		}

		// Token: 0x06003357 RID: 13143 RVA: 0x001193F0 File Offset: 0x001175F0
		public bool AttemptInitiateShipConstruction(TIHabModuleState shipyard)
		{
			bool flag = true;
			if (this.player.isAI)
			{
				if (!this.IsAlienFaction)
				{
					FactionGoal_Fleet aifactionGoal = this.nShipyardQueues[shipyard][0].AIFactionGoal;
					flag = (aifactionGoal != null && aifactionGoal.GrantMissionControlIndulgence) || this.AI_AnyAvailabeGenericMissionControl;
				}
				bool flag2;
				if (flag)
				{
					TIResourcesCost resourcesCost = this.nShipyardQueues[shipyard][0].resourcesCost;
					TIDataTemplate shipDesign = this.nShipyardQueues[shipyard][0].shipDesign;
					FactionGoal_Fleet aifactionGoal2 = this.nShipyardQueues[shipyard][0].AIFactionGoal;
					flag2 = resourcesCost.CanAfford_AI(this, shipDesign, shipyard, (aifactionGoal2 != null) ? aifactionGoal2.importance : 10, true, false, 1f, null, float.PositiveInfinity);
				}
				else
				{
					flag2 = false;
				}
				flag = flag2;
			}
			else
			{
				flag = this.nShipyardQueues[shipyard][0].resourcesCost.CanAfford(this, 1f, null, float.PositiveInfinity);
			}
			if (flag)
			{
				this.nShipyardQueues[shipyard][0].startDate = TITimeState.Now();
				this.nShipyardQueues[shipyard][0].daysToCompletion = this.nShipyardQueues[shipyard][0].resourcesCost.completionTime_days;
				this.nShipyardQueues[shipyard][0].resourcesCost.PayCost(this, "Ship Construction");
				this.nShipyardQueues[shipyard][0].costPaid = true;
				this.RecordExpenditure(TIFactionState.Expenditure.ShipConstruction, this.nShipyardQueues[shipyard][0].resourcesCost);
				GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, shipyard, this.nShipyardQueues[shipyard][0]), null, new object[] { this, shipyard });
				if (this.AISavingTarget.active && this.nShipyardQueues[shipyard][0].shipDesign.dataName == this.AISavingTarget.desiredPurchase.dataName && shipyard == this.AISavingTarget.location.ref_habModule)
				{
					this.AIClearSavingTarget("Building Ship");
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003358 RID: 13144 RVA: 0x00119624 File Offset: 0x00117824
		public void ShipConstructionQueueDailyUpdate()
		{
			List<TIHabModuleState> list = this.nShipyardQueues.Select<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>, TIHabModuleState>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Key).ToList<TIHabModuleState>();
			if (this.player.isAI)
			{
				list = AIEvaluators.GetOrderedShipyards(this).ToList<TIHabModuleState>();
			}
			this.lastUnaffordableShipShipyard = null;
			bool flag = false;
			foreach (TIHabModuleState tihabModuleState in list)
			{
				if (tihabModuleState.active && this.nShipyardQueues[tihabModuleState].Count > 0)
				{
					ShipConstructionQueueItem shipConstructionQueueItem = this.nShipyardQueues[tihabModuleState][0];
					if (shipConstructionQueueItem.costPaid)
					{
						shipConstructionQueueItem.daysToCompletion -= 1f;
						if (shipConstructionQueueItem.daysToCompletion <= 0f)
						{
							this.CompleteShipConstruction(tihabModuleState, false, null);
						}
						else
						{
							GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, tihabModuleState, this.nShipyardQueues[tihabModuleState][0]), null, new object[] { this, tihabModuleState });
						}
					}
					else
					{
						bool flag2 = shipConstructionQueueItem.resourcesCost.GetSingleCostValue(FactionResource.Boost) > 0f;
						TIResourcesCost tiresourcesCost = (shipConstructionQueueItem.isRefit ? shipConstructionQueueItem.shipDesign.RefitResourceCost(tihabModuleState, shipConstructionQueueItem.refit_originalShipDesign, true, true, shipConstructionQueueItem.originalSpaceShipState) : shipConstructionQueueItem.shipDesign.spaceResourceConstructionCost(false, tihabModuleState, true, false, false));
						shipConstructionQueueItem.UpdateResourcesCost(flag2 ? TISpaceShipTemplate.MixedResourceConstructionCost(this, tihabModuleState.hab, tiresourcesCost, this.AvailableSpaceResources(1f), false) : tiresourcesCost);
						if (this.player.isAI)
						{
							if (this.lastUnaffordableShipShipyard != null)
							{
								continue;
							}
							float singleCostValue = tiresourcesCost.GetSingleCostValue(FactionResource.Exotics);
							if (flag && singleCostValue > 0f && (!this.AISavingTarget.active || !(shipConstructionQueueItem.AIFactionGoal == this.AISavingTarget.relatedGoal)))
							{
								continue;
							}
						}
						bool flag3 = this.AttemptInitiateShipConstruction(tihabModuleState);
						if (this.player.isAI && !flag3)
						{
							FactionGoal_Fleet aifactionGoal = shipConstructionQueueItem.AIFactionGoal;
							int num = ((aifactionGoal != null) ? aifactionGoal.importance : 10);
							List<ResourceValue> list2 = shipConstructionQueueItem.resourcesCost.GetShortfall(this, shipConstructionQueueItem.shipDesign, tihabModuleState, num, false).resourceCosts.Where<ResourceValue>((ResourceValue x) => x.value > 0f).ToList<ResourceValue>();
							if (list2.Count > 0)
							{
								if (list2.Count<ResourceValue>((ResourceValue x) => x.resource == FactionResource.Exotics) == list2.Count)
								{
									flag = true;
								}
								else
								{
									this.lastUnaffordableShipShipyard = tihabModuleState;
								}
							}
						}
					}
				}
			}
			if (this.nShipyardQueues.Keys.Count > 0)
			{
				GameControl.eventManager.TriggerEvent(new ShipConstructionUpdated(this, null, null), null, new object[] { this });
			}
		}

		// Token: 0x06003359 RID: 13145 RVA: 0x00119944 File Offset: 0x00117B44
		private Vector3 GetHabOffsetDirection(Transform root, int index, int count)
		{
			if (count < 5 && index >= 1)
			{
				index++;
			}
			switch (index)
			{
			case 1:
				return root.up;
			case 2:
				return root.right;
			case 3:
				return -root.up;
			case 4:
				return -root.right;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x0600335A RID: 13146 RVA: 0x001199A4 File Offset: 0x00117BA4
		public void RecordShipBuilt(TISpaceShipTemplate template)
		{
			if (!this.shipsBuiltInClass.ContainsKey(template.dataName))
			{
				this.shipsBuiltInClass.Add(template.dataName, 0);
			}
			Dictionary<string, int> dictionary = this.shipsBuiltInClass;
			string dataName = template.dataName;
			dictionary[dataName]++;
		}

		// Token: 0x0600335B RID: 13147 RVA: 0x001199F4 File Offset: 0x00117BF4
		public void CompleteShipConstruction(TIHabModuleState shipyardIdx, bool refitCancel = false, ShipConstructionQueueItem item = null)
		{
			if (this.nShipyardQueues[shipyardIdx].Count == 0)
			{
				return;
			}
			TIHabState hab = shipyardIdx.sector.hab;
			if (item == null)
			{
				item = this.nShipyardQueues[shipyardIdx][0];
			}
			TISpaceShipTemplate tispaceShipTemplate = item.shipDesign;
			if (refitCancel)
			{
				tispaceShipTemplate = item.refit_originalShipDesign;
			}
			TISpaceShipState tispaceShipState;
			if (!refitCancel)
			{
				tispaceShipState = (TISpaceShipState)tispaceShipTemplate.CreateGameState();
				tispaceShipState.InitWithTemplate(tispaceShipTemplate);
			}
			else
			{
				tispaceShipState = item.originalSpaceShipState;
			}
			bool flag = false;
			if (item.isRefit)
			{
				tispaceShipState.CopyDataForRefit(item.originalSpaceShipState);
				shipyardIdx.hab.CompleteShipConstruction(tispaceShipState, item.originalSpaceShipState);
				if (refitCancel)
				{
					flag = true;
				}
				else
				{
					TIResourcesCost resourcesRefund = item.resourcesRefund;
					if (resourcesRefund != null)
					{
						resourcesRefund.PayCost(this, null);
					}
					TISpaceShipState originalSpaceShipState = item.originalSpaceShipState;
					if (originalSpaceShipState != null)
					{
						originalSpaceShipState.DestroyShip(false, null);
					}
				}
			}
			else
			{
				tispaceShipState.CompleteShipInitialization();
				shipyardIdx.hab.CompleteShipConstruction(tispaceShipState, null);
				if (this.shipsBuiltInClass[tispaceShipTemplate.dataName] == 1 && tispaceShipTemplate.refitIteration == 0)
				{
					tispaceShipState.SetDisplayName(tispaceShipTemplate.displayName);
				}
				else
				{
					tispaceShipState.SetDisplayName(TISpaceAssetState.GetRandomAssetName(tispaceShipState, this));
				}
			}
			TISpaceFleetState tispaceFleetState;
			if (!refitCancel || flag)
			{
				List<TISpaceShipState> list = new List<TISpaceShipState> { tispaceShipState };
				tispaceFleetState = TISpaceFleetState.CreateAtRunTime(this, list, shipyardIdx.sector.hab, null, item.AIFactionGoal, false, false, null);
			}
			else
			{
				tispaceFleetState = tispaceShipState.fleet;
			}
			this.SetResourceIncomeDataDirty(TISpaceShipState.relevantIncomeResources);
			if (shipyardIdx.sector.hab.IsStation && shipyardIdx.sector.hab.controller != null)
			{
				Vector3d vector3d = this.GetHabOffsetDirection(shipyardIdx.sector.hab.controller.transform, shipyardIdx.sector.sectorNum, shipyardIdx.sector.hab.sectors.Count) * 1600f;
				Vector3d vector3d2 = this.GetHabOffsetDirection(shipyardIdx.sector.hab.controller.transform, shipyardIdx.slot, shipyardIdx.sector.slots) * 600f;
				Vector3d vector3d3 = shipyardIdx.sector.hab.GetGlobalPosition() - tispaceFleetState.GetGlobalPosition();
				tispaceShipState.currentFleetOffset = vector3d3 + (vector3d + vector3d2);
				tispaceShipState.currentRotation = Quaternion.LookRotation((Vector3)(shipyardIdx.sector.hab.SpatialRotation * Vector3d.up));
				Vector3d vector3d4 = shipyardIdx.sector.hab.SpatialRotation * Vector3d.forward * 600.0;
				tispaceShipState.InitiateManeuverSequence(tispaceShipState.currentFleetOffset + vector3d4, tispaceShipState.fleetFormationOffset + vector3d4 * 0.5, tispaceShipState.fleetFormationOffset, tispaceFleetState.RotationNow);
			}
			GameControl.eventManager.TriggerEvent(new ShipConstructionCompleted(tispaceShipState), null, new object[] { tispaceShipState, this, tispaceFleetState, shipyardIdx });
			if (!refitCancel)
			{
				if (tispaceShipState.allWeaponTemplates.Count > 0)
				{
					TIGlobalValuesState.GlobalValues.CheckGlobalMilestone(GlobalMilestone.FirstWarship, this, shipyardIdx.hab);
				}
				if (this.isActivePlayer)
				{
					if (tispaceShipTemplate.hullName == "Titan")
					{
						this.UnlockAchievement("buildTitan");
					}
					if (tispaceShipTemplate.baseCruiseDeltaV_kps(false) >= 1000f)
					{
						this.UnlockAchievement("buildShipHighDV");
					}
					if (tispaceShipTemplate.baseCombatAcceleration_gs >= 4f)
					{
						this.UnlockAchievement("buildShipHighAccel");
					}
					if (this.ships.Count == 1)
					{
						this.UnlockAchievement("buildFirstShip");
					}
					else if (item.isRefit)
					{
						this.UnlockAchievement("refitShip");
					}
				}
				TIHistoricalData.Record(this, "Ship count", (float)this.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Count), 14f, true);
				if (item.isRefit)
				{
					TIHistoricalData.Record_Sum(this, "Refits per month", 0.16758242f, 182f, true);
				}
				else
				{
					TIHistoricalData.Record_Sum(this, "Ships per month", 0.16758242f, 182f, true);
				}
			}
			TINotificationQueueState.LogShipComplete(tispaceShipState, hab, refitCancel);
			if (!refitCancel)
			{
				this.nShipyardQueues[shipyardIdx].RemoveAt(0);
				if (shipyardIdx.sector.hab.IsStation && shipyardIdx.sector.hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(this)))
				{
					shipyardIdx.sector.hab.dockedFleets.First<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(this)).InitiateCombat(tispaceFleetState, shipyardIdx.sector.hab, false);
				}
				if (this.nShipyardQueues[shipyardIdx].Count > 0)
				{
					this.AttemptInitiateShipConstruction(shipyardIdx);
				}
			}
			else
			{
				this.nShipyardQueues[shipyardIdx].Remove(item);
			}
			this.SetMissionControlUsageDataDirty();
		}

		// Token: 0x0600335C RID: 13148 RVA: 0x00119EDA File Offset: 0x001180DA
		public bool UnlockedShipPart(TIShipPartTemplate part)
		{
			return part.requiredProject == null || this.completedProjects.Contains(part.requiredProject);
		}

		// Token: 0x0600335D RID: 13149 RVA: 0x00119EFC File Offset: 0x001180FC
		public float DaysToShipyardAvailability(TIHabModuleState shipyard)
		{
			float num = 0f;
			if (shipyard.underConstruction)
			{
				TIDateTime tidateTime = TITimeState.Now();
				num += (float)tidateTime.DifferenceInDays(new TIDateTime(shipyard.completionDate));
			}
			foreach (ShipConstructionQueueItem shipConstructionQueueItem in this.nShipyardQueues[shipyard])
			{
				num += shipConstructionQueueItem.daysToCompletion;
			}
			return num;
		}

		// Token: 0x0600335E RID: 13150 RVA: 0x00119F84 File Offset: 0x00118184
		public bool AI_ShipyardCanServeGoal(TIHabModuleState shipyard, TIFactionGoalState goal, TISpaceShipTemplate ship, bool respectLocalGoals = true)
		{
			if (goal.importance == 20)
			{
				return true;
			}
			if (this.AISavingTarget.active && this.AISavingTarget.location.ref_habModule == shipyard && this.AISavingTarget.relatedGoal == goal)
			{
				return true;
			}
			TISpaceObjectState getSunOrbitingRelatedObject = shipyard.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
			if (goal.location().ref_naturalSpaceObject.GetSunOrbitingRelatedObject == getSunOrbitingRelatedObject)
			{
				return true;
			}
			if (TISpaceShipTemplate.shortRangeStrategic(ship.role))
			{
				return false;
			}
			foreach (FactionGoal_Fleet factionGoal_Fleet in this.AllFleetGoals(true))
			{
				if (!(factionGoal_Fleet == goal) && TIGameState.Valid(factionGoal_Fleet.location()) && (respectLocalGoals || factionGoal_Fleet.importance >= goal.importance) && factionGoal_Fleet.GetGoalType() != GoalType.AssembleFleet && factionGoal_Fleet.location().ref_naturalSpaceObject.GetSunOrbitingRelatedObject == getSunOrbitingRelatedObject && factionGoal_Fleet.NeedsShipsOrdered())
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x0011A0A0 File Offset: 0x001182A0
		public TIHabModuleState AI_GetBestShipyardForBuild(TISpaceShipTemplate ship, TIGameState destination, TIFactionGoalState goal, out TIFactionState.ShipyardAISearchResult result, out bool tapBoost, bool tapSavings = false, float fraction = 1f, bool emergency = false, bool ignoreCanAfford = false)
		{
			fraction = Mathf.Clamp(fraction, 0f, 1f);
			Dictionary<TIHabModuleState, float> dictionary = new Dictionary<TIHabModuleState, float>();
			TIResourcesCost tiresourcesCost = ship.spaceResourceConstructionCost(false, null, true, false, false);
			result = TIFactionState.ShipyardAISearchResult.Failure_Generic;
			TIHabState tihabState = null;
			tapBoost = false;
			if (emergency && goal.target() == destination)
			{
				tihabState = goal.target().ref_hab;
			}
			if (goal.faction.IsAlienFaction)
			{
				if (goal.GetGoalType() == GoalType.TransportCouncilorsViaFleet)
				{
					tihabState = goal.faction.primaryHab;
				}
				else if (goal.GetGoalType() == GoalType.InvadeEarth)
				{
					List<TIHabState> list = new List<TIHabState>();
					if (ship.combatant)
					{
						list.AddRange(goal.faction.primaryHab.ref_system.habsInSystem.Where<TIHabState>((TIHabState x) => x.CompletedShipyards().Count > 0 && x.faction == goal.faction));
					}
					else
					{
						list.Add(goal.faction.primaryHab);
						if (this.primaryStation != null && this.primaryStation.CompletedShipyards().Count > 0)
						{
							list.Add(this.primaryStation);
						}
					}
					Func<ShipConstructionQueueItem, bool> <>9__2;
					tihabState = list.MinBy<TIHabState, float>(delegate(TIHabState hab)
					{
						IEnumerable<ShipConstructionQueueItem> enumerable2 = hab.AllShipConstructionQueueItems(goal.faction);
						Func<ShipConstructionQueueItem, bool> func2;
						if ((func2 = <>9__2) == null)
						{
							func2 = (<>9__2 = (ShipConstructionQueueItem x) => x.shipDesign.nonCombatant == ship.nonCombatant);
						}
						float num7 = (float)enumerable2.Where<ShipConstructionQueueItem>(func2).Count<ShipConstructionQueueItem>() / (float)hab.CompletedShipyards().Count;
						if (hab == goal.faction.primaryHab)
						{
							num7 -= 0.001f;
						}
						return num7;
					});
				}
			}
			List<TIHabModuleState> list2 = this.nShipyardQueues.Keys.ToList<TIHabModuleState>();
			if (!this.IsAlienFaction)
			{
				float minSystemAU = 0f;
				FactionGoal_FoundHab factionGoal_FoundHab = goal as FactionGoal_FoundHab;
				if (factionGoal_FoundHab != null && factionGoal_FoundHab.target().ref_system.semiMajorAxis_AU >= GameStateManager.Jupiter().semiMajorAxis_AU)
				{
					minSystemAU = (float)GameStateManager.Mars().semiMajorAxis_AU;
				}
				List<TIHabModuleState> list3 = this.nShipyardQueues.Keys.Where<TIHabModuleState>((TIHabModuleState x) => x.ref_system.semiMajorAxis_AU > (double)minSystemAU).ToList<TIHabModuleState>();
				if (list3.Count > 0)
				{
					list2 = list3;
				}
			}
			List<bool> list4 = new List<bool> { true };
			if (TIFactionGoalState.FoundHabGoals.Contains(goal.GetGoalType()))
			{
				list4.Add(false);
			}
			List<TIHabState> list5 = new List<TIHabState>();
			Func<ShipConstructionQueueItem, bool> <>9__4;
			foreach (bool flag in list4)
			{
				foreach (TIHabModuleState tihabModuleState in list2)
				{
					if (!(tihabState != null) || !(tihabModuleState.hab != tihabState))
					{
						int num;
						if (!(goal != null))
						{
							num = 0;
						}
						else
						{
							IEnumerable<ShipConstructionQueueItem> enumerable = this.nShipyardQueues[tihabModuleState];
							Func<ShipConstructionQueueItem, bool> func;
							if ((func = <>9__4) == null)
							{
								func = (<>9__4 = (ShipConstructionQueueItem x) => x.AIFactionGoal == goal);
							}
							num = enumerable.Count<ShipConstructionQueueItem>(func);
						}
						int num2 = num;
						if (((emergency || tihabState != null) && num2 < 2) || (this.nShipyardQueues[tihabModuleState].Count <= tihabModuleState.moduleTemplate.tier && this.AI_ShipyardCanServeGoal(tihabModuleState, goal, ship, flag)))
						{
							float num3 = fraction / Mathf.Max(1f, Mathf.Pow((float)this.nShipyardQueues[tihabModuleState].Count, 2f));
							if (ship.CanBuildAtShipyard(tihabModuleState))
							{
								bool flag2 = false;
								TIResourcesCost tiresourcesCost2 = tiresourcesCost;
								if (tiresourcesCost.CanAfford_AI(this, ship, tihabModuleState, goal.importance, false, false, num3, null, float.PositiveInfinity))
								{
									flag2 = true;
								}
								else if (this.IsActiveHumanFaction || GameStateManager.AlienNation().extant)
								{
									if (AIEvaluators.ShouldPayTodaysBoostCost(ship, this, tihabModuleState, fraction, goal))
									{
										flag2 = true;
										list5.AddUnique(tihabModuleState.hab);
									}
									else
									{
										result = TIFactionState.ShipyardAISearchResult.Failure_CantAfford;
									}
								}
								else
								{
									result = TIFactionState.ShipyardAISearchResult.Failure_CantAfford;
								}
								if (flag2 || ignoreCanAfford)
								{
									float num4;
									bool flag3;
									if (destination == tihabModuleState.hab)
									{
										num4 = 0f;
										flag3 = false;
									}
									else
									{
										num4 = (float)MasterTransferPlanner.GetEstimatedTransferTime_s(tihabModuleState.hab, destination.ref_spaceObject, (double)ship.baseCruiseAcceleration_mps2(false), (double)ship.baseCruiseDeltaV_mps(false), out flag3) / 86400f;
									}
									if (!flag3)
									{
										result = TIFactionState.ShipyardAISearchResult.Success;
										Dictionary<TIHabModuleState, float> dictionary2 = dictionary;
										TIHabModuleState tihabModuleState2 = tihabModuleState;
										float num5 = tiresourcesCost2.completionTime_days + num4;
										float num6;
										if (!emergency)
										{
											num6 = this.nShipyardQueues[tihabModuleState].Sum<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => x.daysToCompletion);
										}
										else
										{
											num6 = 0f;
										}
										dictionary2.Add(tihabModuleState2, num5 + num6);
									}
								}
							}
						}
					}
				}
				if (dictionary.Count > 0)
				{
					result = TIFactionState.ShipyardAISearchResult.Success;
					TIHabModuleState key = dictionary.MinBy<KeyValuePair<TIHabModuleState, float>, float>((KeyValuePair<TIHabModuleState, float> x) => x.Value).Key;
					tapBoost = list5.Contains(key.hab) && AIEvaluators.ValidShipyardToSpendBoost(this, key);
					return key;
				}
				dictionary.Clear();
			}
			return null;
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06003360 RID: 13152 RVA: 0x0011A604 File Offset: 0x00118804
		public IEnumerable<HabSchematic> HabSchematics
		{
			get
			{
				if (this.habSchematics == null)
				{
					this.habSchematics = (from x in TemplateManager.IterateByClass<TIHabSchematicTemplate>(true)
						where x.AvailableToFaction(this)
						select x.HabSchematic).ToList<HabSchematic>();
				}
				return this.habSchematics;
			}
		}

		// Token: 0x06003361 RID: 13153 RVA: 0x0011A665 File Offset: 0x00118865
		public float GetMineSizeModifier()
		{
			return 1f + TIEffectsState.SumEffectsModifiers(Context.MineSizeModifier, this, 1f, null);
		}

		// Token: 0x06003362 RID: 13154 RVA: 0x0011A67E File Offset: 0x0011887E
		public float GetHabConstructionDurationModifier()
		{
			return 1f / (1f + TIEffectsState.SumEffectsModifiers(Context.HabConstructionSpeed, this, 1f, null));
		}

		// Token: 0x06003363 RID: 13155 RVA: 0x0011A6A0 File Offset: 0x001188A0
		public void OnTimedOperationComplete(TimeEventStart e)
		{
			if (e.eventObject == this)
			{
				IOperation operation = e.eventDataTemplate as IOperation;
				if (operation != null)
				{
					operation.OnOperationExecute(this, e.eventObject2);
				}
				GameControl.eventManager.TriggerEvent(new TimeEventComplete(this, null), this.factionOperationCompleteName, Array.Empty<object>());
			}
		}

		// Token: 0x06003364 RID: 13156 RVA: 0x0011A6F4 File Offset: 0x001188F4
		public List<IOperation> VisibleOperationList(TINaturalSpaceObjectState naturalSpaceObject)
		{
			return OperationsManager.spaceOperations.Where<IOperation>((IOperation x) => x.OpVisibleToActor(this, naturalSpaceObject)).ToList<IOperation>();
		}

		// Token: 0x06003365 RID: 13157 RVA: 0x0011A730 File Offset: 0x00118930
		public List<IOperation> AvailableOperationList(TINaturalSpaceObjectState naturalSpaceObject)
		{
			List<IOperation> list = new List<IOperation>();
			foreach (IOperation operation in OperationsManager.spaceOperations)
			{
				if (operation.OpVisibleToActor(this, naturalSpaceObject) && operation.ActorCanPerformOperation(this, naturalSpaceObject))
				{
					list.Add(operation);
				}
			}
			return list;
		}

		// Token: 0x06003366 RID: 13158 RVA: 0x0011A7A0 File Offset: 0x001189A0
		public List<OperationData> CurrentOperations()
		{
			return null;
		}

		// Token: 0x06003367 RID: 13159 RVA: 0x0011A7A3 File Offset: 0x001189A3
		public bool CanProspectFromShip(TISpaceBodyState spaceBody)
		{
			return spaceBody.habSites.Length != 0 && this.EligibleforColonization(spaceBody) && !this.Prospected(spaceBody);
		}

		// Token: 0x06003368 RID: 13160 RVA: 0x0011A7C3 File Offset: 0x001189C3
		public bool CandidateForProspecting(TISpaceBodyState spaceBody)
		{
			return !this.ProspectingSpaceBody(spaceBody) && !this.Prospected(spaceBody) && spaceBody.habSites.Length != 0 && this.CanExplore(spaceBody);
		}

		// Token: 0x06003369 RID: 13161 RVA: 0x0011A7EC File Offset: 0x001189EC
		public List<TIResourcesCost> CanOvertakeProbeWithProbe(TISpaceBodyState spaceBody)
		{
			List<TIResourcesCost> list = new List<TIResourcesCost>();
			TIDateTime tidateTime = this.ProspectorArrival(spaceBody);
			if (tidateTime != null && !this.FleetSurveyingPlanet(spaceBody))
			{
				double num = tidateTime.DifferenceInDays(TITimeState.Now());
				LaunchProbeOperation launchProbeOperation = new LaunchProbeOperation();
				TIResourcesCost tiresourcesCost = launchProbeOperation.SpaceCost(this, spaceBody);
				if (tiresourcesCost.anyDebit && tiresourcesCost.CanAfford(this, 1f, null, float.PositiveInfinity) && (double)tiresourcesCost.completionTime_days < num * 0.949999988079071)
				{
					list.Add(tiresourcesCost);
				}
				TIResourcesCost tiresourcesCost2 = launchProbeOperation.EarthCost(this, spaceBody);
				if (tiresourcesCost2.anyDebit && tiresourcesCost2.CanAfford(this, 1f, null, float.PositiveInfinity) && (double)tiresourcesCost2.completionTime_days < num * 0.949999988079071)
				{
					list.Add(tiresourcesCost2);
				}
			}
			return list;
		}

		// Token: 0x0600336A RID: 13162 RVA: 0x0011A8B5 File Offset: 0x00118AB5
		public bool CanProspectWithProbe(TISpaceBodyState spaceBody, bool checkIfCanOvertake)
		{
			if (this.CanProspectFromShip(spaceBody) && !this.FleetSurveyingPlanet(spaceBody))
			{
				if (!this.ProspectorEnRoute(spaceBody))
				{
					return true;
				}
				if (checkIfCanOvertake)
				{
					return this.CanOvertakeProbeWithProbe(spaceBody).Count > 0;
				}
			}
			return false;
		}

		// Token: 0x0600336B RID: 13163 RVA: 0x0011A8E8 File Offset: 0x00118AE8
		public bool EligibleForFoundingBase(TISpaceBodyState spaceBody)
		{
			return this.Prospected(spaceBody) && this.EligibleforColonization(spaceBody) && spaceBody.vacantHabSites.Count > 0;
		}

		// Token: 0x0600336C RID: 13164 RVA: 0x0011A90C File Offset: 0x00118B0C
		public bool AlienTerritoryToAvoid(TISpaceBodyState spaceBody)
		{
			return !this.IsAlienFaction && !this.IsAlienProxy && spaceBody.alienTerritory;
		}

		// Token: 0x0600336D RID: 13165 RVA: 0x0011A926 File Offset: 0x00118B26
		public bool EligibleforColonization(TISpaceGameState spaceGameState)
		{
			return (!(spaceGameState.ref_spaceBody != null) || !this.AlienTerritoryToAvoid(spaceGameState.ref_spaceBody)) && this.CanExplore(spaceGameState);
		}

		// Token: 0x0600336E RID: 13166 RVA: 0x0011A950 File Offset: 0x00118B50
		public bool CanExplore(TISpaceGameState spaceGameState)
		{
			if (TIFactionState.<CanExplore>g__IsCacheable|895_0(spaceGameState) && this.cachedCanExplore.Contains(spaceGameState))
			{
				return true;
			}
			if (!spaceGameState.isSpaceObjectState && !spaceGameState.isOrbitState && !spaceGameState.isHabSiteState)
			{
				Log.Error("Wrongly Passed " + spaceGameState.displayName + " to CanExplore", Array.Empty<object>());
				return false;
			}
			if (this.IsAlienFaction || spaceGameState.ref_faction == this || (spaceGameState.isSpaceBodyState && spaceGameState.ref_spaceBody.isEarth))
			{
				return true;
			}
			if (spaceGameState.isSpaceFleetState && spaceGameState.ref_fleet.inTransfer)
			{
				return spaceGameState.ref_fleet.trajectory.destination != null && this.CanExplore(spaceGameState.ref_fleet.trajectory.destination);
			}
			IEnumerable<TIEffectTemplate> enumerable;
			if (spaceGameState.isOrbitState)
			{
				enumerable = spaceGameState.ref_orbit.GetExplorationEffectOptions();
			}
			else
			{
				enumerable = spaceGameState.ref_naturalSpaceObject.GetExplorationEffectOptions();
			}
			enumerable = enumerable.Where<TIEffectTemplate>((TIEffectTemplate x) => x != null);
			bool flag = !enumerable.Any<TIEffectTemplate>() || enumerable.Any<TIEffectTemplate>((TIEffectTemplate x) => TIEffectsState.CheckForEffectInAnyContext(this, x));
			bool flag2 = false;
			if (TIEffectsState.CheckForAnyEffectInContext(Context.SemiMajorAxisExplorationRange_AU, this))
			{
				float num = 1f + TIEffectsState.SumEffectsModifiers(Context.SemiMajorAxisExplorationRange_AU, this, 1f, null);
				flag2 = spaceGameState.ref_system.semiMajorAxis_AU > (double)num;
			}
			bool flag3 = !flag2 && flag;
			if (flag3 && TIFactionState.<CanExplore>g__IsCacheable|895_0(spaceGameState))
			{
				this.cachedCanExplore.Add(spaceGameState);
			}
			return flag3;
		}

		// Token: 0x0600336F RID: 13167 RVA: 0x0011AAE0 File Offset: 0x00118CE0
		public float DesiredStrategicRange_AU()
		{
			if (this.IsAlienFaction)
			{
				return 60f;
			}
			float num = TIEffectsState.SumEffectsModifiers(Context.OuterExplorationRange_AU, this, 1f, null);
			if (this.isActivePlayer)
			{
				return num;
			}
			List<TIObjectiveTemplate> objectivesByTypeAndStatus = this.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked);
			if (objectivesByTypeAndStatus.Count > 0 && objectivesByTypeAndStatus[0].targetMissionTarget == ObjectiveMissionTargetType.AlienHQ)
			{
				return Mathf.Clamp((float)GameStateManager.AlienFaction().primaryHab.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.apoapsis_AU, 1.02f, num);
			}
			List<TISpaceObjectState> list = this.habs.Select<TIHabState, TISpaceObjectState>((TIHabState x) => x.ref_naturalSpaceObject.GetSunOrbitingRelatedObject).Distinct<TISpaceObjectState>().ToList<TISpaceObjectState>();
			List<TISpaceObjectState> list2 = (from x in (from x in this.GoalsOfType(TIFactionGoalState.FoundHabGoals, false, true)
					select x.target().ref_naturalSpaceObject.GetSunOrbitingRelatedObject).Distinct<TISpaceObjectState>().ToList<TISpaceObjectState>().Except<TISpaceObjectState>(list)
				where x.semiMajorAxis_AU >= 0.98
				select x).ToList<TISpaceObjectState>();
			float num2;
			if (list2.Count <= 0)
			{
				num2 = (float)1.0199999809265137;
			}
			else
			{
				num2 = (float)list2.Min<TISpaceObjectState>((TISpaceObjectState x) => x.semiMajorAxis_AU);
			}
			return Mathf.Clamp(num2, 1.02f, num);
		}

		// Token: 0x06003370 RID: 13168 RVA: 0x0011AC48 File Offset: 0x00118E48
		private void CheckForNewObjectives()
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate in TemplateManager.IterateByClass<TIObjectiveTemplate>(true))
			{
				if (tiobjectiveTemplate.factions.Contains(this))
				{
					ObjectiveStatus objectiveStatus = (tiobjectiveTemplate.starter ? ObjectiveStatus.Unlocked : ObjectiveStatus.Locked);
					if ((TIGlobalValuesState.GlobalValues.tutorialMode || tiobjectiveTemplate.objectiveType != ObjectiveType.Tutorial) && !this.objectives.Keys.Contains(tiobjectiveTemplate))
					{
						this.objectives.Add(tiobjectiveTemplate, objectiveStatus);
						this.objectiveNames.Add(tiobjectiveTemplate.dataName, objectiveStatus);
					}
				}
			}
		}

		// Token: 0x06003371 RID: 13169 RVA: 0x0011ACF0 File Offset: 0x00118EF0
		public void CheckForMilestonesCompleteViaOperation(TIOperationTemplate operation, TIGameState target)
		{
			if (TIGlobalValuesState.isTutorialActive)
			{
				if (operation is LaunchProbeOperation)
				{
					this.CompleteMilestone(CampaignMilestone.TutorialProbeSpaceBody);
					return;
				}
				if (operation is FoundStationOperation)
				{
					this.CompleteMilestone(CampaignMilestone.TutorialFoundStation);
					return;
				}
				if (operation is FoundBaseOperation)
				{
					this.CompleteMilestone(CampaignMilestone.TutorialFoundBase);
				}
			}
		}

		// Token: 0x06003372 RID: 13170 RVA: 0x0011AD2B File Offset: 0x00118F2B
		public bool CanCompleteAccessLiveAliensMilestones()
		{
			return TIEffectsState.CheckForAnyEffectInContext(Context.CanCaptureHydra, this);
		}

		// Token: 0x06003373 RID: 13171 RVA: 0x0011AD34 File Offset: 0x00118F34
		public void CompleteMilestone(CampaignMilestone milestone)
		{
			bool flag = TIObjectiveTemplate.IsTutorialMilestone(milestone);
			if (!TIGlobalValuesState.isTutorialActive && flag)
			{
				return;
			}
			if (!this.milestones.Contains(milestone))
			{
				if (flag && !this.isActivePlayer)
				{
					return;
				}
				GameControl.eventManager.TriggerEvent(new MilestoneComplete(milestone, this), null, new object[] { this });
				if ((milestone == CampaignMilestone.AlienInfrastructureExists && this.proAlien) || ((!TIObjectiveTemplate.MilestoneRequiresLiveAlienAccess(milestone) || this.CanCompleteAccessLiveAliensMilestones()) && (!TIObjectiveTemplate.MilestoneRequiresDeadHydraAccess(milestone) || this.milestones.Contains(CampaignMilestone.AccessHydraCorpus))))
				{
					this.milestones.Add(milestone);
					if (!TIObjectiveTemplate.SuppressMilestoneReporting(milestone))
					{
						TINotificationQueueState.LogMilestoneComplete(this, milestone);
					}
					TIFactionState.LogAI(this.displayName + " completes " + milestone.ToString(), false);
					this.CheckForObjectivesCompleteViaMilestone(milestone);
					this.OnMilestoneCompleted(milestone);
				}
				if (milestone <= CampaignMilestone.AlienMegafaunaSpawns)
				{
					if (milestone == CampaignMilestone.AccessAlienShip)
					{
						this.CompleteMilestone(CampaignMilestone.AccessAlienTech);
						this.CompleteMilestone(CampaignMilestone.AccessGriffinCorpus);
						return;
					}
					switch (milestone)
					{
					case CampaignMilestone.AliensLandArmy:
						if (this.ideology.ideology != FactionIdeology.Cooperate && this.ideology.ideology != FactionIdeology.Alien)
						{
							TINationState.AllFactionNationsPropaganda_PerOwnedCP(this, 30f);
						}
						this.CompleteMilestone(CampaignMilestone.AlienInvasionPlanDiscovered);
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienOvertAggression);
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
						return;
					case CampaignMilestone.AlienArmyDestroyed:
						break;
					case CampaignMilestone.AliensBombardEarth:
						if (this.ideology.ideology != FactionIdeology.Cooperate && this.ideology.ideology != FactionIdeology.Alien)
						{
							TINationState.AllFactionNationsPropaganda_PerOwnedCP(this, 30f);
						}
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienOvertAggression);
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
						return;
					case CampaignMilestone.AliensAttackInSpace:
						if (this.ideology.ideology != FactionIdeology.Cooperate && this.ideology.ideology != FactionIdeology.Alien)
						{
							TINationState.AllFactionNationsPropaganda_PerOwnedCP(this, 15f);
						}
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienOvertAggression);
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
						return;
					case CampaignMilestone.AlienMegafaunaSpawns:
						if (this.ideology.ideology != FactionIdeology.Cooperate && this.ideology.ideology != FactionIdeology.Alien)
						{
							TINationState.AllFactionNationsPropaganda_PerOwnedCP(this, 5f);
						}
						TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
						return;
					default:
						return;
					}
				}
				else
				{
					if (milestone == CampaignMilestone.AlienWarshipSighted)
					{
						this.CompleteMilestone(CampaignMilestone.AlienSpaceshipSighted);
						return;
					}
					if (milestone == CampaignMilestone.AlienInvasionShipSighted)
					{
						this.CompleteMilestone(CampaignMilestone.AlienWarshipSighted);
						this.CompleteMilestone(CampaignMilestone.AlienInvasionPlanDiscovered);
						return;
					}
					if (milestone != CampaignMilestone.AlienNationWasFounded)
					{
						return;
					}
					this.CompleteMilestone(CampaignMilestone.AlienInvasionPlanDiscovered);
					TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
				}
			}
		}

		// Token: 0x06003374 RID: 13172 RVA: 0x0011AF6C File Offset: 0x0011916C
		public static void CompleteMilestoneForAllHumanFactions(CampaignMilestone milestone)
		{
			TIFactionState[] array = GameStateManager.AllHumanFactions();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CompleteMilestone(milestone);
			}
		}

		// Token: 0x06003375 RID: 13173 RVA: 0x0011AF96 File Offset: 0x00119196
		public void ResetMilestone(CampaignMilestone milestone)
		{
			if (!TIGlobalValuesState.isTutorialActive && TIObjectiveTemplate.IsTutorialMilestone(milestone))
			{
				return;
			}
			if (this.milestones.Contains(milestone))
			{
				this.milestones.Remove(milestone);
			}
		}

		// Token: 0x06003376 RID: 13174 RVA: 0x0011AFC4 File Offset: 0x001191C4
		public void ResetAllTutorialMilestones()
		{
			List<CampaignMilestone> list = new List<CampaignMilestone>();
			List<CampaignMilestone> list2 = new List<CampaignMilestone>();
			for (int i = 0; i < GameControl.control.activePlayer.milestones.Count; i++)
			{
				if (GameControl.control.activePlayer.milestones[i] < CampaignMilestone.UITutorial_GeneralControlsCanvas || GameControl.control.activePlayer.milestones[i] > CampaignMilestone.UITutorial_END || GameControl.control.activePlayer.milestones[i] == CampaignMilestone.UITutorial_Intro)
				{
					list.Add(GameControl.control.activePlayer.milestones[i]);
				}
				else
				{
					list2.Add(GameControl.control.activePlayer.milestones[i]);
				}
			}
			GameControl.control.activePlayer.milestones.Clear();
			GameControl.control.activePlayer.milestones = list;
			foreach (CampaignMilestone campaignMilestone in list2)
			{
				this.ResetMilestone(campaignMilestone);
			}
			UITutorialController[] array = global::UnityEngine.Object.FindObjectsOfType<UITutorialController>(true);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].ResetTutorial(false);
			}
			if (!GameControl.control.skirmishMode)
			{
				(World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController).mainHUDTutorialController.HoldTutorial(CampaignMilestone.UITutorial_GeneralControlsCanvas, false, true);
			}
		}

		// Token: 0x06003377 RID: 13175 RVA: 0x0011B14C File Offset: 0x0011934C
		public bool MilestoneCompleted(CampaignMilestone milestone)
		{
			return this.milestones.Contains(milestone);
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x0011B15A File Offset: 0x0011935A
		public void InitializeAchievements()
		{
			this.InitializeSteamAchievements();
		}

		// Token: 0x06003379 RID: 13177 RVA: 0x0011B162 File Offset: 0x00119362
		public void InitializeSteamAchievements()
		{
			if (!SteamManager.Initialized)
			{
				global::UnityEngine.Debug.LogWarning("Cannot init Steam Achievements Steamworks has not been initialized!");
				return;
			}
		}

		// Token: 0x0600337A RID: 13178 RVA: 0x0011B176 File Offset: 0x00119376
		public void UnlockAchievement(string apiName)
		{
			if (!this.isActivePlayer || GameControl.control.skirmishMode || TemplateManager.global.debug_ConsoleActive)
			{
				return;
			}
			if (SteamManager.Initialized)
			{
				this.UnlockSteamAchievement(apiName);
			}
		}

		// Token: 0x0600337B RID: 13179 RVA: 0x0011B1A7 File Offset: 0x001193A7
		public void ResetAchievement(string apiName)
		{
			this.ResetSteamAchievement(apiName);
			SteamUserStats.StoreStats();
		}

		// Token: 0x0600337C RID: 13180 RVA: 0x0011B1B6 File Offset: 0x001193B6
		private void UnlockSteamAchievement(string apiName)
		{
			if (!SteamManager.Initialized)
			{
				global::UnityEngine.Debug.LogWarning("Steam not Initialized, cannot unlock Steam Achievement");
				return;
			}
			SteamUserStats.SetAchievement(apiName);
			SteamUserStats.StoreStats();
		}

		// Token: 0x0600337D RID: 13181 RVA: 0x0011B1D7 File Offset: 0x001193D7
		private void ResetSteamAchievement(string apiName)
		{
			SteamUserStats.ClearAchievement(apiName);
			SteamUserStats.StoreStats();
		}

		// Token: 0x0600337E RID: 13182 RVA: 0x0011B1E6 File Offset: 0x001193E6
		public void ResetAllSteamUserStats(bool resetAchievementsToo)
		{
			SteamUserStats.ResetAllStats(resetAchievementsToo);
			SteamUserStats.StoreStats();
		}

		// Token: 0x0600337F RID: 13183 RVA: 0x0011B1F8 File Offset: 0x001193F8
		public void ProcessBuildHabAchievements(TIHabState hab)
		{
			if (this != GameControl.control.activePlayer)
			{
				return;
			}
			if (this.habs.Count >= 42)
			{
				this.UnlockAchievement("controlManyHabs");
			}
			if (hab.IsStation)
			{
				this.UnlockAchievement("buildHabSpace");
				if (hab.barycenter.isLagrangePointState)
				{
					this.UnlockAchievement("colonizeLPoint");
				}
			}
			else
			{
				this.UnlockAchievement("buildHabBase");
				if (hab.barycenter.objectType == SpaceObjectType.Comet)
				{
					this.UnlockAchievement("cometBase");
				}
			}
			string text = hab.barycenter.templateName;
			if (text != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1289753133U)
				{
					if (num <= 921805286U)
					{
						if (num != 593356674U)
						{
							if (num != 820825766U)
							{
								if (num == 921805286U)
								{
									if (text == "Jupiter")
									{
										this.UnlockAchievement("colonizeJupiter");
									}
								}
							}
							else if (text == "Mimas")
							{
								this.UnlockAchievement("colonizeMimas");
							}
						}
						else if (text == "Haumea")
						{
							this.UnlockAchievement("colonizeHaumea");
						}
					}
					else if (num <= 1180344712U)
					{
						if (num != 1105775185U)
						{
							if (num == 1180344712U)
							{
								if (text == "Mars")
								{
									this.UnlockAchievement("colonizeMars");
								}
							}
						}
						else if (text == "Io")
						{
							this.UnlockAchievement("colonizeIo");
						}
					}
					else if (num != 1196921158U)
					{
						if (num == 1289753133U)
						{
							if (text == "Uranus")
							{
								this.UnlockAchievement("colonizeUranus");
							}
						}
					}
					else if (text == "Venus")
					{
						this.UnlockAchievement("colonizeVenus");
					}
				}
				else if (num <= 2677462550U)
				{
					if (num <= 2296796206U)
					{
						if (num != 2234485159U)
						{
							if (num == 2296796206U)
							{
								if (text == "Mercury")
								{
									this.UnlockAchievement("colonizeMercury");
								}
							}
						}
						else if (text == "Luna")
						{
							this.UnlockAchievement("colonizeLuna");
						}
					}
					else if (num != 2306451309U)
					{
						if (num == 2677462550U)
						{
							if (text == "Saturn")
							{
								this.UnlockAchievement("colonizeSaturn");
							}
						}
					}
					else if (text == "Pluto")
					{
						this.UnlockAchievement("colonizePluto");
					}
				}
				else if (num <= 3490196713U)
				{
					if (num != 3268195154U)
					{
						if (num == 3490196713U)
						{
							if (text == "Ceres")
							{
								this.UnlockAchievement("colonizeCeres");
							}
						}
					}
					else if (text == "Neptune")
					{
						this.UnlockAchievement("colonizeNeptune");
					}
				}
				else if (num != 3638345371U)
				{
					if (num == 3726054639U)
					{
						if (text == "Miranda")
						{
							this.UnlockAchievement("colonizeMiranda");
						}
					}
				}
				else if (text == "Europa")
				{
					this.UnlockAchievement("colonizeEuropa");
				}
			}
			TINaturalSpaceObjectState barycenter = hab.barycenter.barycenter;
			if (hab.barycenter.isaMoon && barycenter != null)
			{
				text = barycenter.templateName;
				if (text != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
					if (num <= 1289753133U)
					{
						if (num <= 1180344712U)
						{
							if (num != 921805286U)
							{
								if (num == 1180344712U)
								{
									if (text == "Mars")
									{
										this.UnlockAchievement("colonizeMars");
									}
								}
							}
							else if (text == "Jupiter")
							{
								this.UnlockAchievement("colonizeJupiter");
							}
						}
						else if (num != 1196921158U)
						{
							if (num == 1289753133U)
							{
								if (text == "Uranus")
								{
									this.UnlockAchievement("colonizeUranus");
								}
							}
						}
						else if (text == "Venus")
						{
							this.UnlockAchievement("colonizeVenus");
						}
					}
					else if (num <= 2306451309U)
					{
						if (num != 2296796206U)
						{
							if (num == 2306451309U)
							{
								if (text == "Pluto")
								{
									this.UnlockAchievement("colonizePluto");
								}
							}
						}
						else if (text == "Mercury")
						{
							this.UnlockAchievement("colonizeMercury");
						}
					}
					else if (num != 2677462550U)
					{
						if (num == 3268195154U)
						{
							if (text == "Neptune")
							{
								this.UnlockAchievement("colonizeNeptune");
							}
						}
					}
					else if (text == "Saturn")
					{
						this.UnlockAchievement("colonizeSaturn");
					}
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			foreach (TIHabState tihabState in this.habs)
			{
				text = tihabState.barycenter.templateName;
				if (text != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
					if (num <= 1289753133U)
					{
						if (num <= 1180344712U)
						{
							if (num != 921805286U)
							{
								if (num == 1180344712U)
								{
									if (text == "Mars")
									{
										flag4 = true;
									}
								}
							}
							else if (text == "Jupiter")
							{
								flag5 = true;
							}
						}
						else if (num != 1196921158U)
						{
							if (num == 1289753133U)
							{
								if (text == "Uranus")
								{
									flag7 = true;
								}
							}
						}
						else if (text == "Venus")
						{
							flag2 = true;
						}
					}
					else if (num <= 2677462550U)
					{
						if (num != 2296796206U)
						{
							if (num == 2677462550U)
							{
								if (text == "Saturn")
								{
									flag6 = true;
								}
							}
						}
						else if (text == "Mercury")
						{
							flag = true;
						}
					}
					else if (num != 3268195154U)
					{
						if (num == 4159608695U)
						{
							if (text == "Earth")
							{
								flag3 = true;
							}
						}
					}
					else if (text == "Neptune")
					{
						flag8 = true;
					}
				}
				TINaturalSpaceObjectState barycenter2 = tihabState.barycenter.barycenter;
				if (tihabState.barycenter.isaMoon && barycenter2 != null)
				{
					text = barycenter2.templateName;
					if (text != null)
					{
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
						if (num <= 1289753133U)
						{
							if (num <= 1180344712U)
							{
								if (num != 921805286U)
								{
									if (num == 1180344712U)
									{
										if (text == "Mars")
										{
											flag4 = true;
										}
									}
								}
								else if (text == "Jupiter")
								{
									flag5 = true;
								}
							}
							else if (num != 1196921158U)
							{
								if (num == 1289753133U)
								{
									if (text == "Uranus")
									{
										flag7 = true;
									}
								}
							}
							else if (text == "Venus")
							{
								flag2 = true;
							}
						}
						else if (num <= 2677462550U)
						{
							if (num != 2296796206U)
							{
								if (num == 2677462550U)
								{
									if (text == "Saturn")
									{
										flag6 = true;
									}
								}
							}
							else if (text == "Mercury")
							{
								flag = true;
							}
						}
						else if (num != 3268195154U)
						{
							if (num == 4159608695U)
							{
								if (text == "Earth")
								{
									flag3 = true;
								}
							}
						}
						else if (text == "Neptune")
						{
							flag8 = true;
						}
					}
				}
			}
			if (flag && flag2 && flag3 && flag4 && flag5 && flag6 && flag7 && flag8)
			{
				this.UnlockAchievement("colonizeAllMajorPlanets");
			}
		}

		// Token: 0x06003380 RID: 13184 RVA: 0x0011BAC0 File Offset: 0x00119CC0
		public bool WonWithAllFactions()
		{
			int num = 0;
			if (!SteamManager.Initialized)
			{
				return false;
			}
			bool flag;
			if (SteamUserStats.GetAchievement("resistWin", out flag) && flag)
			{
				num++;
			}
			if (SteamUserStats.GetAchievement("appeaseWin", out flag) && flag)
			{
				num++;
			}
			if (SteamUserStats.GetAchievement("submitWin", out flag) && flag)
			{
				num++;
			}
			if (SteamUserStats.GetAchievement("exploitWin", out flag) && flag)
			{
				num++;
			}
			if (SteamUserStats.GetAchievement("escapeWin", out flag) && flag)
			{
				num++;
			}
			if (SteamUserStats.GetAchievement("destroyWin", out flag) && flag)
			{
				num++;
			}
			if (SteamUserStats.GetAchievement("cooperateWin", out flag) && flag)
			{
				num++;
			}
			return num >= 7;
		}

		// Token: 0x06003381 RID: 13185 RVA: 0x0011BB74 File Offset: 0x00119D74
		public void CheckForObjectivesCompleteViaMilestone(CampaignMilestone milestone)
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByStatus(ObjectiveStatus.Unlocked))
			{
				if (tiobjectiveTemplate.targetMilestone == milestone)
				{
					this.CompleteObjective(tiobjectiveTemplate);
				}
			}
		}

		// Token: 0x06003382 RID: 13186 RVA: 0x0011BBD4 File Offset: 0x00119DD4
		public void CheckForObjectivesCompleteViaProject(TIProjectTemplate projectTemplate)
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByStatus(ObjectiveStatus.Unlocked))
			{
				if (tiobjectiveTemplate.targetProjectTemplate == projectTemplate)
				{
					this.CompleteObjective(tiobjectiveTemplate);
				}
			}
			if (TIGlobalValuesState.isTutorialActive)
			{
				foreach (TIObjectiveTemplate tiobjectiveTemplate2 in this.GetObjectivesAndChildByType(ObjectiveType.Tutorial))
				{
					if (tiobjectiveTemplate2.targetProjectTemplate == projectTemplate)
					{
						string text = tiobjectiveTemplate2.targetProjectTemplateName;
						if (text != null)
						{
							if (!(text == "Project_ClandestineCells"))
							{
								if (!(text == "Project_OutpostCore"))
								{
									if (!(text == "Project_SpaceDock"))
									{
										if (!(text == "Project_Warships"))
										{
											if (text == "Project_OutpostMiningComplex")
											{
												this.CompleteMilestone(CampaignMilestone.TutorialResearchOutpostMining);
											}
										}
										else
										{
											this.CompleteMilestone(CampaignMilestone.TutorialResearchInterplanetaryWarships);
										}
									}
									else
									{
										this.CompleteMilestone(CampaignMilestone.TutorialResearchSpaceDock);
									}
								}
								else
								{
									this.CompleteMilestone(CampaignMilestone.TutorialResearchOutpostCore);
								}
							}
							else
							{
								this.CompleteMilestone(CampaignMilestone.TutorialResearchClandestineCells);
							}
						}
					}
				}
			}
			if (this.isActivePlayer)
			{
				string text = projectTemplate.dataName;
				if (text != null)
				{
					if (text == "Project_TheirMovements")
					{
						this.UnlockAchievement("researchAlienMovements");
						return;
					}
					if (text == "Project_Pherocytes")
					{
						this.UnlockAchievement("researchPherocytes");
						return;
					}
					if (!(text == "Project_Exotics"))
					{
						return;
					}
					this.UnlockAchievement("researchExotics");
				}
			}
		}

		// Token: 0x06003383 RID: 13187 RVA: 0x0011BD60 File Offset: 0x00119F60
		public void CheckForObjectivesCompleteViaTech(TITechTemplate techTemplate)
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByStatus(ObjectiveStatus.Unlocked))
			{
				if (tiobjectiveTemplate.targetTechTemplate == techTemplate)
				{
					this.CompleteObjective(tiobjectiveTemplate);
				}
			}
			if (TIGlobalValuesState.isTutorialActive)
			{
				foreach (TIObjectiveTemplate tiobjectiveTemplate2 in this.GetObjectivesAndChildByType(ObjectiveType.Tutorial))
				{
					if (tiobjectiveTemplate2.targetTechTemplate == techTemplate)
					{
						string targetTechTemplateName = tiobjectiveTemplate2.targetTechTemplateName;
						if (targetTechTemplateName != null)
						{
							uint num = <PrivateImplementationDetails>.ComputeStringHash(targetTechTemplateName);
							if (num <= 1587511089U)
							{
								if (num != 377260942U)
								{
									if (num != 1006853753U)
									{
										if (num == 1587511089U)
										{
											if (targetTechTemplateName == "MissiontoMars")
											{
												this.CompleteMilestone(CampaignMilestone.TutorialResearchMissionToMoonMars);
											}
										}
									}
									else if (targetTechTemplateName == "WeAreNotAlone")
									{
										this.CompleteMilestone(CampaignMilestone.TutorialResearchWeAreNotAlone);
									}
								}
								else if (targetTechTemplateName == "MissionToSpace")
								{
									this.CompleteMilestone(CampaignMilestone.TutorialResearchMissionToSpace);
								}
							}
							else if (num <= 2666515544U)
							{
								if (num != 2331365941U)
								{
									if (num == 2666515544U)
									{
										if (targetTechTemplateName == "SpaceMiningandRefining")
										{
											this.CompleteMilestone(CampaignMilestone.TutorialResearchSpaceMining);
										}
									}
								}
								else if (targetTechTemplateName == "OutpostHabs")
								{
									this.CompleteMilestone(CampaignMilestone.TutorialResearchOutpostHabs);
								}
							}
							else if (num != 3723051642U)
							{
								if (num == 3912042944U)
								{
									if (targetTechTemplateName == "OrbitalShipbuilding")
									{
										this.CompleteMilestone(CampaignMilestone.TutorialResearchOrbitalShipbuilding);
									}
								}
							}
							else if (targetTechTemplateName == "MissiontotheMoon")
							{
								this.CompleteMilestone(CampaignMilestone.TutorialResearchMissionToMoonMars);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003384 RID: 13188 RVA: 0x0011BF5C File Offset: 0x0011A15C
		public void CheckForObjectivesCompleteViaMission(TIMissionState missionState, MissionResult result)
		{
			if (result.Success)
			{
				foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByStatus(ObjectiveStatus.Unlocked))
				{
					if (tiobjectiveTemplate.objectiveType != ObjectiveType.General && tiobjectiveTemplate.targetMissionTemplate == missionState.missionTemplate && tiobjectiveTemplate.ValidObjectiveTarget(missionState.target, this))
					{
						this.CompleteObjective(tiobjectiveTemplate);
					}
				}
				if (result.Success && missionState.target.isHabModuleState && missionState.target.ref_habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.InterstellarLaunchModule) && missionState.missionTemplate.IsVictoryMission && this.victoryTemplate.dataName == "vc_escapeVictory")
				{
					missionState.target.ref_habModule.SetCompletedModule(TemplateManager.IterateByClass<TIHabModuleTemplate>(true).First<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.tier == 3 && x.allowsShipConstruction).dataName, false);
				}
			}
		}

		// Token: 0x06003385 RID: 13189 RVA: 0x0011C080 File Offset: 0x0011A280
		public void CheckForObjectivesCompleteViaHabModuleActivated(TIHabModuleState habModule)
		{
			using (List<TIObjectiveTemplate>.Enumerator enumerator = this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIObjectiveTemplate objective = enumerator.Current;
					if (!string.IsNullOrEmpty(objective.targetHabModuleName) && habModule.moduleTemplate == objective.targetHabModuleTemplate && this.activeHabModules.Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate == objective.targetHabModuleTemplate) >= objective.targetCount)
					{
						this.CompleteObjective(objective);
					}
				}
			}
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x0011C130 File Offset: 0x0011A330
		public List<CampaignMilestone> DesiredMilestones()
		{
			List<CampaignMilestone> list = new List<CampaignMilestone>();
			foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked))
			{
				if (tiobjectiveTemplate.targetMilestone != CampaignMilestone.None && !list.Contains(tiobjectiveTemplate.targetMilestone))
				{
					list.Add(tiobjectiveTemplate.targetMilestone);
				}
			}
			return list;
		}

		// Token: 0x06003387 RID: 13191 RVA: 0x0011C1A8 File Offset: 0x0011A3A8
		public void CompleteObjective(TIObjectiveTemplate objective)
		{
			this.objectives[objective] = ObjectiveStatus.Completed;
			this.objectiveNames[objective.dataName] = ObjectiveStatus.Completed;
			GameControl.eventManager.TriggerEvent(new ObjectiveComplete(objective, this), null, new object[] { this });
			List<TIObjectiveTemplate> list = this.CheckForNewObjectiveUnlocksViaObjective();
			foreach (ResourceValue resourceValue in objective.resourcesGranted)
			{
				this.AddToCurrentResource(resourceValue.value, resourceValue.resource, false, "Objective Completed");
			}
			if (objective.setsWinConditionForFaction)
			{
				this.knowsWinCondition = true;
			}
			if (objective.AIValuesIndex >= 0)
			{
				this.aiValues = this.template.AIValues[objective.AIValuesIndex];
			}
			if (!objective.isChildObjective)
			{
				if (this.isActivePlayer && objective.dataName == "InvestigateAlienCrashdown")
				{
					this.UnlockAchievement("investigateCrash");
				}
				if (objective.dataName.Contains("ResearchTheirLanguage"))
				{
					this.CompleteMilestone(CampaignMilestone.AccessAlienLanguage);
				}
				TINotificationQueueState.LogObjectiveComplete(this, objective, list);
			}
			TIFactionState.LogAI(this.displayName + " completes " + objective.displayName(this), false);
			foreach (TIProjectTemplate tiprojectTemplate in TIGlobalResearchState.GetAllProjects())
			{
				if (!string.IsNullOrEmpty(tiprojectTemplate.requiredObjectiveName) && (tiprojectTemplate.requiredObjectiveName == objective.dataName || tiprojectTemplate.altRequiredObjectiveName == objective.dataName))
				{
					this.RollToAddProjectTrigger(tiprojectTemplate, null);
				}
			}
			foreach (TIObjectiveTemplate tiobjectiveTemplate in list)
			{
				if (tiobjectiveTemplate.targetMilestone != CampaignMilestone.None && this.MilestoneCompleted(tiobjectiveTemplate.targetMilestone) && !this.GetObjectivesByStatus(ObjectiveStatus.Completed).Contains(tiobjectiveTemplate))
				{
					this.CompleteObjective(tiobjectiveTemplate);
				}
			}
		}

		// Token: 0x06003388 RID: 13192 RVA: 0x0011C3B4 File Offset: 0x0011A5B4
		public void UnlockObjective(TIObjectiveTemplate objective)
		{
			this.objectives[objective] = ObjectiveStatus.Unlocked;
			this.objectiveNames[objective.dataName] = ObjectiveStatus.Unlocked;
			if (objective.objectiveType == ObjectiveType.Victory)
			{
				TINotificationQueueState.LogOtherFactionUnlocksVictoryCondition(this);
				if (this.isActivePlayer)
				{
					this.UnlockAchievement("discoverVictory");
				}
				this.unlockedVictoryObjective = true;
			}
			if (objective.targetProjectTemplate != null && this.completedProjects.Contains(objective.targetProjectTemplate))
			{
				this.CompleteObjective(objective);
				return;
			}
			if (objective.targetMilestone != CampaignMilestone.None && this.MilestoneCompleted(objective.targetMilestone))
			{
				this.CompleteObjective(objective);
				return;
			}
			if (objective.targetHabModuleTemplate != null && this.activeHabModules.Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate == objective.targetHabModuleTemplate) >= objective.targetCount)
			{
				this.CompleteObjective(objective);
				return;
			}
			if (objective.targetTechTemplate != null && TIGlobalResearchState.TechFinished(objective.targetTechTemplate))
			{
				this.CompleteObjective(objective);
			}
		}

		// Token: 0x06003389 RID: 13193 RVA: 0x0011C4F0 File Offset: 0x0011A6F0
		public List<TIObjectiveTemplate> CheckForNewObjectiveUnlocksViaObjective()
		{
			List<TIObjectiveTemplate> list = new List<TIObjectiveTemplate>();
			foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetObjectivesByStatus(ObjectiveStatus.Locked))
			{
				if (tiobjectiveTemplate.passedUnlockingObjectives(this) && this.GetObjectivesByStatus(ObjectiveStatus.Locked).Contains(tiobjectiveTemplate))
				{
					this.UnlockObjective(tiobjectiveTemplate);
					list.Add(tiobjectiveTemplate);
				}
			}
			return list;
		}

		// Token: 0x0600338A RID: 13194 RVA: 0x0011C56C File Offset: 0x0011A76C
		public List<TIObjectiveTemplate> GetObjectives()
		{
			return this.objectives.Keys.ToList<TIObjectiveTemplate>();
		}

		// Token: 0x0600338B RID: 13195 RVA: 0x0011C580 File Offset: 0x0011A780
		public List<TIObjectiveTemplate> GetObjectivesByStatus(ObjectiveStatus status)
		{
			return this.objectives.Keys.Where<TIObjectiveTemplate>((TIObjectiveTemplate x) => this.objectives[x] == status).ToList<TIObjectiveTemplate>();
		}

		// Token: 0x0600338C RID: 13196 RVA: 0x0011C5C4 File Offset: 0x0011A7C4
		public List<TIObjectiveTemplate> GetObjectivesByType(ObjectiveType objectiveType)
		{
			return this.objectives.Keys.Where<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.objectiveType == objectiveType && !x.isChildObjective).ToList<TIObjectiveTemplate>();
		}

		// Token: 0x0600338D RID: 13197 RVA: 0x0011C600 File Offset: 0x0011A800
		public List<TIObjectiveTemplate> GetObjectivesAndChildByType(ObjectiveType objectiveType)
		{
			return this.objectives.Keys.Where<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.objectiveType == objectiveType).ToList<TIObjectiveTemplate>();
		}

		// Token: 0x0600338E RID: 13198 RVA: 0x0011C63C File Offset: 0x0011A83C
		public List<TIObjectiveTemplate> GetObjectivesByTypeAndStatus(ObjectiveType objectiveType, ObjectiveStatus status)
		{
			return this.objectives.Keys.Where<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.objectiveType == objectiveType && this.objectives[x] == status).ToList<TIObjectiveTemplate>();
		}

		// Token: 0x0600338F RID: 13199 RVA: 0x0011C685 File Offset: 0x0011A885
		public ObjectiveStatus GetObjectiveStatus(TIObjectiveTemplate objectiveTemplate)
		{
			if (this.objectives.Keys.Contains(objectiveTemplate))
			{
				return this.objectives[objectiveTemplate];
			}
			return ObjectiveStatus.Locked;
		}

		// Token: 0x06003390 RID: 13200 RVA: 0x0011C6A8 File Offset: 0x0011A8A8
		public bool IsObjectiveComplete(TIObjectiveTemplate objectiveTemplate)
		{
			return this.GetObjectiveStatus(objectiveTemplate) == ObjectiveStatus.Completed;
		}

		// Token: 0x06003391 RID: 13201 RVA: 0x0011C6B4 File Offset: 0x0011A8B4
		public CampaignMilestone GetMileStoneFromObjective(TIObjectiveTemplate objectiveTemplate)
		{
			CampaignMilestone campaignMilestone = CampaignMilestone.None;
			foreach (CampaignMilestone campaignMilestone2 in this.milestones)
			{
				if (campaignMilestone2.ToString() == objectiveTemplate.dataName)
				{
					campaignMilestone = campaignMilestone2;
				}
			}
			return campaignMilestone;
		}

		// Token: 0x06003392 RID: 13202 RVA: 0x0011C720 File Offset: 0x0011A920
		public string GetObjectiveCompletedVoicePath(TIObjectiveTemplate finishedObjective)
		{
			switch (this.ideology.ideology)
			{
			case FactionIdeology.Destroy:
				return finishedObjective.completedVoicePathDestroy;
			case FactionIdeology.Resist:
				return finishedObjective.completedVoicePathResist;
			case FactionIdeology.Escape:
				return finishedObjective.completedVoicePathEscape;
			case FactionIdeology.Exploit:
				return finishedObjective.completedVoicePathExploit;
			case FactionIdeology.Cooperate:
				return finishedObjective.completedVoicePathCooperate;
			case FactionIdeology.Appease:
				return finishedObjective.completedVoicePathAppease;
			case FactionIdeology.Submit:
				return finishedObjective.completedVoicePathSubmit;
			case FactionIdeology.Alien:
				return finishedObjective.completedVoicePathAlien;
			case FactionIdeology.Mod_1:
				return finishedObjective.completedVoicePathMod1;
			case FactionIdeology.Mod_2:
				return finishedObjective.completedVoicePathMod2;
			case FactionIdeology.Mod_3:
				return finishedObjective.completedVoicePathMod3;
			case FactionIdeology.Mod_4:
				return finishedObjective.completedVoicePathMod4;
			case FactionIdeology.Mod_5:
				return finishedObjective.completedVoicePathMod5;
			case FactionIdeology.Mod_6:
				return finishedObjective.completedVoicePathMod6;
			case FactionIdeology.Mod_7:
				return finishedObjective.completedVoicePathMod7;
			case FactionIdeology.Mod_8:
				return finishedObjective.completedVoicePathMod8;
			}
			return "";
		}

		// Token: 0x06003393 RID: 13203 RVA: 0x0011C800 File Offset: 0x0011AA00
		public CampaignMusicProgression GetDesiredMusicProgression()
		{
			if (this.unlockedVictoryObjective)
			{
				return CampaignMusicProgression.LateGame;
			}
			if (this.MilestoneCompleted(CampaignMilestone.AccessLiveHydra) || this.MilestoneCompleted(CampaignMilestone.AccessAlienLanguage))
			{
				return CampaignMusicProgression.MidGame;
			}
			return CampaignMusicProgression.EarlyGame;
		}

		// Token: 0x06003394 RID: 13204 RVA: 0x0011C821 File Offset: 0x0011AA21
		public void LaunchProspector(TISpaceBodyState spaceBody)
		{
			this.SetIntel(spaceBody, 0.1f, null, false);
		}

		// Token: 0x06003395 RID: 13205 RVA: 0x0011C831 File Offset: 0x0011AA31
		public void ProspectSpaceBody(TISpaceBodyState spaceBody)
		{
			this.SetIntel(spaceBody, 1f, null, false);
			AIDailyFactionPlanner.AIReaction(AIReactionEvent.SpaceBodyProspected, this, spaceBody);
		}

		// Token: 0x06003396 RID: 13206 RVA: 0x0011C849 File Offset: 0x0011AA49
		public bool ProspectingSpaceBody(TISpaceBodyState spaceBody)
		{
			return !this.Prospected(spaceBody) && (this.ProspectorEnRoute(spaceBody) || this.FleetSurveyingPlanet(spaceBody));
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x0011C868 File Offset: 0x0011AA68
		public bool ProspectorEnRoute(TISpaceBodyState spaceBody)
		{
			return this.GetIntel(spaceBody) >= 0.1f && !this.Prospected(spaceBody);
		}

		// Token: 0x06003398 RID: 13208 RVA: 0x0011C884 File Offset: 0x0011AA84
		public bool FleetSurveyingPlanet(TISpaceBodyState spaceBody)
		{
			Func<OperationData, bool> <>9__1;
			return this.fleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				IEnumerable<OperationData> enumerable = x.CurrentOperations();
				Func<OperationData, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (OperationData y) => y.target == spaceBody && y.operationDataName == typeof(SurveyPlanetFromFleetOperation).ToString());
				}
				return enumerable.Any<OperationData>(func);
			});
		}

		// Token: 0x06003399 RID: 13209 RVA: 0x0011C8B8 File Offset: 0x0011AAB8
		public TIDateTime ProspectorArrival(TISpaceBodyState spaceBody)
		{
			if (!this.ProspectorEnRoute(spaceBody))
			{
				return null;
			}
			return this.gameTime.GetTimeForPendingEvent(this.factionOperationCompleteName, this, spaceBody, OperationsManager.operationsLookup[typeof(LaunchProbeOperation)].GetTemplate()) ?? this.gameTime.GetTimeForPendingEvent(this.factionOperationCompleteName, this, spaceBody, OperationsManager.operationsLookup[typeof(LaunchOverrideProbeOperation)].GetTemplate());
		}

		// Token: 0x0600339A RID: 13210 RVA: 0x0011C92C File Offset: 0x0011AB2C
		public bool Prospected(TISpaceBodyState spaceBody)
		{
			return this.GetIntel(spaceBody) >= 1f;
		}

		// Token: 0x0600339B RID: 13211 RVA: 0x0011C93F File Offset: 0x0011AB3F
		public bool Prospected(TIHabSiteState habSite)
		{
			return this.Prospected(habSite.parentBody);
		}

		// Token: 0x0600339C RID: 13212 RVA: 0x0011C950 File Offset: 0x0011AB50
		public bool CanShareIntelItemWithFaction(TIFactionState receivingFaction, TIGameState gameState)
		{
			return !gameState.isRegionUFOCrashdown && !gameState.isRegionAlienActivity && (!gameState.isSpaceBodyState || (!this.IsAlienFaction && this.intel[gameState] != 0.1f)) && (!gameState.isCouncilorState || !gameState.ref_councilor.isAlien || receivingFaction.CanDetectAlien) && (!gameState.isFactionState || !gameState.ref_faction.IsAlienFaction || receivingFaction.CanDetectAlien);
		}

		// Token: 0x0600339D RID: 13213 RVA: 0x0011C9D4 File Offset: 0x0011ABD4
		public void GiveIntelToFaction(TIFactionState intelGainingFaction, bool fromSpy)
		{
			foreach (TIGameState tigameState in this.intel.Keys.ToList<TIGameState>())
			{
				if (this.CanShareIntelItemWithFaction(intelGainingFaction, tigameState))
				{
					intelGainingFaction.SetIntelIfValueHigher(tigameState, tigameState.isCouncilorState ? Mathf.Min(this.GetIntel(tigameState), TemplateManager.global.intelToSeeCouncilorMission) : this.GetIntel(tigameState), fromSpy ? intelGainingFaction.turnedCouncilors.FirstOrDefault<TICouncilorState>((TICouncilorState x) => x.faction == this) : base.ref_gameState);
				}
			}
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x0011CA84 File Offset: 0x0011AC84
		private void ProcessIntelChange(TIGameState intelTarget, bool simultaneous)
		{
			this.intel[intelTarget] = Mathf.Clamp(this.intel[intelTarget], 0f, 1f);
			if (this.intel[intelTarget] > this.highestIntel[intelTarget])
			{
				this.highestIntel[intelTarget] = this.intel[intelTarget];
			}
			if (intelTarget.deleted || intelTarget.archived)
			{
				return;
			}
			if (intelTarget.isCouncilorState && intelTarget.ref_councilor.status == CouncilorStatus.Active)
			{
				TICouncilorState ref_councilor = intelTarget.ref_councilor;
				bool flag = this.councilors.Contains(ref_councilor);
				bool flag2 = ref_councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.LoyaltyMonitor);
				if (flag || ref_councilor.detainingFaction == this || this.factionsCompromised.Contains(ref_councilor.faction) || (this.permanentAlly(ref_councilor.faction) && this.intel[ref_councilor] > TemplateManager.global.intelToSeeCouncilorBasicData))
				{
					this.intel[ref_councilor] = Mathf.Max(this.intel[ref_councilor], TemplateManager.global.myCouncilorBaselineIntel);
				}
				if (ref_councilor.agentForFaction == this || (flag && flag2))
				{
					this.intel[ref_councilor] = Mathf.Max(this.intel[ref_councilor], TemplateManager.global.intelToSeeCouncilorSecrets);
				}
				if (ref_councilor.ref_faction != null && this.GetIntel(ref_councilor.ref_faction) < this.intel[ref_councilor])
				{
					this.SetIntel(ref_councilor.ref_faction, this.intel[ref_councilor], null, false);
				}
			}
			else if (intelTarget.isSpaceAssetState)
			{
				float num;
				if (intelTarget.ref_faction == this)
				{
					num = (this.IsAlienFaction ? TemplateManager.global.alienMySpaceAssetBaselineIntel : TemplateManager.global.humanMySpaceAssetBaselineIntel);
				}
				else
				{
					TIFactionState ref_faction = intelTarget.ref_faction;
					if (ref_faction != null && ref_faction.IsAlienFaction)
					{
						num = (this.IsAlienFaction ? TemplateManager.global.alienMySpaceAssetBaselineIntel : intelTarget.ref_spaceAsset.BaselineIntelOnAlienAsset(this));
					}
					else
					{
						num = TemplateManager.global.humanSpaceAssetBaselineIntel;
					}
				}
				if (this.intel[intelTarget] < num)
				{
					this.intel[intelTarget] = Mathf.Max(this.intel[intelTarget], num);
				}
			}
			if (!simultaneous)
			{
				foreach (TIFactionState tifactionState in this.factionsCompromisingThisFaction)
				{
					if (this.CanShareIntelItemWithFaction(tifactionState, intelTarget))
					{
						tifactionState.SetIntelIfValueHigher(intelTarget, this.GetIntel(intelTarget), null);
					}
				}
				foreach (TIFactionState tifactionState2 in this.factionsCompromised)
				{
					if (tifactionState2.intel.ContainsKey(intelTarget) && (!this.intel.ContainsKey(intelTarget) || this.intel[intelTarget] < tifactionState2.intel[intelTarget]) && tifactionState2.CanShareIntelItemWithFaction(this, intelTarget))
					{
						this.intel[intelTarget] = tifactionState2.intel[intelTarget];
					}
				}
			}
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x0011CDF4 File Offset: 0x0011AFF4
		public float GainIntelToMinimum(TIGameState intelTarget, float gain, float minimum, TIGameState changeSource = null, float maxFromThisGain = 1f)
		{
			float num = this.GetIntel(intelTarget);
			float num2 = num + gain;
			if (num2 < minimum)
			{
				num2 = minimum;
			}
			if (num2 > maxFromThisGain)
			{
				num2 = Mathf.Max(num, maxFromThisGain);
			}
			this.SetIntel(intelTarget, num2, changeSource, false);
			return num2;
		}

		// Token: 0x060033A0 RID: 13216 RVA: 0x0011CE2D File Offset: 0x0011B02D
		public void SetIntelIfValueHigher(TIGameState intelTarget, float value, TIGameState changeSource = null)
		{
			if (this.intel.ContainsKey(intelTarget))
			{
				if (this.GetIntel(intelTarget) < value)
				{
					this.SetIntel(intelTarget, value, changeSource, false);
					return;
				}
			}
			else
			{
				this.SetIntel(intelTarget, value, changeSource, false);
			}
		}

		// Token: 0x060033A1 RID: 13217 RVA: 0x0011CE5C File Offset: 0x0011B05C
		public void SetIntelIfValueLower(TIGameState intelTarget, float value, TIGameState changeSource = null, bool simultaneous = false)
		{
			if (this.intel.ContainsKey(intelTarget) && this.GetIntel(intelTarget) > value)
			{
				this.SetIntel(intelTarget, value, changeSource, simultaneous);
			}
		}

		// Token: 0x060033A2 RID: 13218 RVA: 0x0011CE84 File Offset: 0x0011B084
		public void SetIntel(TIGameState intelTarget, float value, TIGameState changeSource = null, bool simultaneous = false)
		{
			if (intelTarget == null)
			{
				return;
			}
			if (intelTarget.deleted)
			{
				if (this.intel.ContainsKey(intelTarget))
				{
					this.intel.Remove(intelTarget);
				}
				if (this.highestIntel.ContainsKey(intelTarget))
				{
					this.highestIntel.Remove(intelTarget);
				}
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = intelTarget.isRegionXenoformingState && !intelTarget.ref_xenoforming.VisibleToFaction(this);
			float num = 0f;
			if (!this.intel.ContainsKey(intelTarget))
			{
				this.intel.Add(intelTarget, value);
				if (!this.highestIntel.ContainsKey(intelTarget))
				{
					this.highestIntel.Add(intelTarget, value);
				}
				if (value != 0f)
				{
					flag = true;
					if (value > 0f)
					{
						flag2 = true;
					}
				}
				this.ProcessIntelChange(intelTarget, simultaneous);
			}
			else
			{
				num = this.intel[intelTarget];
				this.intel[intelTarget] = value;
				this.ProcessIntelChange(intelTarget, simultaneous);
				if (this.intel[intelTarget] != num)
				{
					flag = true;
					if (this.intel[intelTarget] > num && !intelTarget.archived)
					{
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				if (intelTarget.isCouncilorState)
				{
					TICouncilorState ref_councilor = intelTarget.ref_councilor;
					if (ref_councilor.status != CouncilorStatus.None)
					{
						GameControl.eventManager.TriggerEvent(new CouncilorVisibilityChanged(ref_councilor, this), null, (from x in new object[] { this, ref_councilor, ref_councilor.location, ref_councilor.priorLocation, ref_councilor.ref_nation, ref_councilor.ref_naturalSpaceObject, ref_councilor.ref_fleet }.Distinct<object>()
							where x != null
							select x).ToArray<object>());
						if (ref_councilor.isAlien && this.HasIntelOnCouncilorBasicData(ref_councilor))
						{
							this.MarkAlienSite(ref_councilor.location, null);
						}
						if (value >= TemplateManager.global.intelToSeeCouncilorSecrets)
						{
							this.lastRecordedLoyalty[ref_councilor] = ref_councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false);
							this.lastTimeSecretsWereSeen[ref_councilor] = TITimeState.Now();
							if (ref_councilor.faction == this && ref_councilor.turned && !this.knownSpies.Contains(ref_councilor))
							{
								this.knownSpies.Add(ref_councilor);
								TINotificationQueueState.LogSpyDiscovered(ref_councilor);
								return;
							}
						}
					}
				}
				else if (intelTarget.isFactionState)
				{
					if (flag2 && value >= TemplateManager.global.intelToSeeFactionBasicData)
					{
						TINotificationQueueState.LogFirstFactionEncounter(this, intelTarget.ref_faction, changeSource ?? intelTarget.ref_faction, changeSource ?? intelTarget.ref_faction);
						return;
					}
				}
				else if (intelTarget.isSpaceAssetState && flag2)
				{
					TISpaceAssetState ref_spaceAsset = intelTarget.ref_spaceAsset;
					if (num < TemplateManager.global.intelToSeeSpaceAssetLocationandComposition && value >= TemplateManager.global.intelToSeeSpaceAssetLocationandComposition)
					{
						if (ref_spaceAsset.isSpaceFleetState)
						{
							if (!(ref_spaceAsset.ref_faction != null))
							{
								Log.Error("Fleet will null faction passed to SetIntel for " + this.displayName, Array.Empty<object>());
								return;
							}
							if (!this.factionFleetsEncountered.ContainsKey(ref_spaceAsset.ref_faction))
							{
								this.factionFleetsEncountered.Add(ref_spaceAsset.ref_faction, 0);
							}
							Dictionary<TIFactionState, int> dictionary = this.factionFleetsEncountered;
							TIFactionState ref_faction = ref_spaceAsset.ref_faction;
							dictionary[ref_faction]++;
							ref_spaceAsset.ref_fleet.SetDisplayName(this, null, false);
							TINotificationQueueState.LogFleetDetected(this, ref_spaceAsset.ref_fleet);
							if (ref_spaceAsset.ref_faction.IsAlienFaction)
							{
								if (ref_spaceAsset.ref_fleet.InvasionFleet())
								{
									this.CompleteMilestone(CampaignMilestone.AlienWarshipSighted);
									this.CompleteMilestone(CampaignMilestone.AlienInvasionShipSighted);
								}
								else if (ref_spaceAsset.ref_fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.combatant))
								{
									this.CompleteMilestone(CampaignMilestone.AlienWarshipSighted);
								}
								else
								{
									this.CompleteMilestone(CampaignMilestone.AlienSpaceshipSighted);
								}
							}
						}
						else
						{
							if (ref_spaceAsset.ref_hab.IsAlien())
							{
								TINotificationQueueState.LogAlienHabDetected(this, ref_spaceAsset.ref_hab);
								this.CompleteMilestone(CampaignMilestone.AlienHabSighted);
								TISpaceBodyState ref_system = ref_spaceAsset.ref_hab.ref_system;
								if (ref_system != null && ref_system.isEarth)
								{
									TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
								}
							}
							else
							{
								TINotificationQueueState.LogHumanHabDetected(this, ref_spaceAsset.ref_hab);
							}
							AIDailyFactionPlanner.AIReaction(AIReactionEvent.NewHabDetected, ref_spaceAsset.ref_hab, null);
						}
						GameControl.eventManager.TriggerEvent(new SpaceAssetDetected(this, ref_spaceAsset), null, new object[] { this, ref_spaceAsset, ref_spaceAsset.barycenter }.Where<object>((object x) => x != null).ToArray<object>());
						return;
					}
				}
				else if (intelTarget.isRegionAlienEntity)
				{
					if (intelTarget.isRegionXenoformingState)
					{
						TIDateTime tidateTime = TITimeState.Now();
						tidateTime.AddDays(-intelTarget.ref_xenoforming.xenoformingLevel * 12f);
						if (intelTarget.ref_xenoforming.Extant())
						{
							this.MarkAlienSite(intelTarget.ref_region, tidateTime);
							if (changeSource != intelTarget && flag3 && intelTarget.ref_xenoforming.VisibleToFaction(this))
							{
								intelTarget.ref_xenoforming.SightedByFaction(this, true);
								return;
							}
						}
					}
					else
					{
						TIRegionAlienEntityState ref_regionAlienEntity = intelTarget.ref_regionAlienEntity;
						if (ref_regionAlienEntity.Extant())
						{
							this.MarkAlienSite(intelTarget.ref_region, null);
							if (ref_regionAlienEntity.isRegionAlienFacility && num == 0f)
							{
								ref_regionAlienEntity.ref_alienFacility.SightedByFaction(this);
								return;
							}
							GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(ref_regionAlienEntity, ref_regionAlienEntity.region), null, new object[] { ref_regionAlienEntity, ref_regionAlienEntity.region });
							return;
						}
					}
				}
				else if (intelTarget.isSpaceBodyState && flag2 && this.intel[intelTarget] >= 1f)
				{
					this.updateHabPlanningFlag = true;
					GameControl.eventManager.TriggerEvent(new SpaceBodyProspected(this, intelTarget.ref_spaceBody), null, new object[] { this, intelTarget.ref_spaceBody });
				}
			}
		}

		// Token: 0x060033A3 RID: 13219 RVA: 0x0011D46E File Offset: 0x0011B66E
		public void GainIntel(TIGameState intelTarget, float value, TIGameState source = null, bool simultaneous = false)
		{
			if (this.intel.ContainsKey(intelTarget))
			{
				this.SetIntel(intelTarget, this.intel[intelTarget] + value, source, simultaneous);
				return;
			}
			this.SetIntel(intelTarget, value, source, simultaneous);
		}

		// Token: 0x060033A4 RID: 13220 RVA: 0x0011D4A2 File Offset: 0x0011B6A2
		public float GetIntel(TIGameState prospectiveTarget)
		{
			if (!TIGameState.Valid(prospectiveTarget) || prospectiveTarget.archived)
			{
				return 0f;
			}
			if (this.intel.ContainsKey(prospectiveTarget))
			{
				return this.intel[prospectiveTarget];
			}
			return 0f;
		}

		// Token: 0x060033A5 RID: 13221 RVA: 0x0011D4DA File Offset: 0x0011B6DA
		public float GetHighestIntel(TIGameState target)
		{
			if (target == null)
			{
				return 0f;
			}
			if (this.highestIntel.ContainsKey(target))
			{
				return this.highestIntel[target];
			}
			return 0f;
		}

		// Token: 0x060033A6 RID: 13222 RVA: 0x0011D50C File Offset: 0x0011B70C
		public List<TICouncilorState> EnemyCouncilorsIHaveIntelOn(TIFactionState faction, bool allHuman = false)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TIGameState tigameState in this.intel.Keys)
			{
				if (tigameState.isCouncilorState && this.intel[tigameState] > 0f && tigameState.ref_councilor.faction != this && (faction == null || tigameState.ref_councilor.faction == faction || (allHuman && tigameState.ref_councilor.faction.IsAlienFaction)))
				{
					list.Add(tigameState.ref_councilor);
				}
			}
			return list;
		}

		// Token: 0x060033A7 RID: 13223 RVA: 0x0011D5D0 File Offset: 0x0011B7D0
		public List<TISpaceBodyState> ProspectedSpaceBodies()
		{
			List<TISpaceBodyState> list = new List<TISpaceBodyState>();
			foreach (TIGameState tigameState in this.intel.Keys)
			{
				if (tigameState.isSpaceBodyState && this.intel[tigameState] >= 1f)
				{
					list.Add(tigameState.ref_spaceBody);
				}
			}
			return list;
		}

		// Token: 0x060033A8 RID: 13224 RVA: 0x0011D650 File Offset: 0x0011B850
		public List<TISpaceBodyState> SpaceBodiesWithProspectorEnRoute()
		{
			List<TISpaceBodyState> list = new List<TISpaceBodyState>();
			foreach (TIGameState tigameState in this.intel.Keys)
			{
				if (tigameState.isSpaceBodyState && this.intel[tigameState] == 0.1f)
				{
					list.Add(tigameState.ref_spaceBody);
				}
			}
			return list;
		}

		// Token: 0x060033A9 RID: 13225 RVA: 0x0011D6D0 File Offset: 0x0011B8D0
		public IEnumerable<TISpaceBodyState> ProspectedAndSoonToBeProspectedSpaceBodies()
		{
			return this.SpaceBodiesWithProspectorEnRoute().Concat<TISpaceBodyState>(this.ProspectedSpaceBodies());
		}

		// Token: 0x060033AA RID: 13226 RVA: 0x0011D6E3 File Offset: 0x0011B8E3
		public bool SufficientIntel(TIGameState target, float thresholdValue)
		{
			return this.GetIntel(target) >= thresholdValue;
		}

		// Token: 0x060033AB RID: 13227 RVA: 0x0011D6F2 File Offset: 0x0011B8F2
		public bool SufficientMemory(TIGameState target, float thresholdValue)
		{
			return this.GetHighestIntel(target) >= thresholdValue;
		}

		// Token: 0x060033AC RID: 13228 RVA: 0x0011D701 File Offset: 0x0011B901
		public bool HasMemoryOnCouncilorBasicData(TICouncilorState councilor)
		{
			return this.SufficientMemory(councilor, TemplateManager.global.intelToSeeCouncilorBasicData);
		}

		// Token: 0x060033AD RID: 13229 RVA: 0x0011D714 File Offset: 0x0011B914
		public bool HasMemoryOnCouncilorDetails(TICouncilorState councilor)
		{
			return this.SufficientMemory(councilor, TemplateManager.global.intelToSeeCouncilorDetails);
		}

		// Token: 0x060033AE RID: 13230 RVA: 0x0011D727 File Offset: 0x0011B927
		public bool HasMemoryOnCouncilorSecrets(TICouncilorState councilor)
		{
			return this.SufficientMemory(councilor, TemplateManager.global.intelToSeeCouncilorSecrets);
		}

		// Token: 0x060033AF RID: 13231 RVA: 0x0011D73A File Offset: 0x0011B93A
		public bool HasIntelOnCouncilorLocation(TICouncilorState councilor)
		{
			return this.SufficientIntel(councilor, TemplateManager.global.intelToSeeNeutralPawn);
		}

		// Token: 0x060033B0 RID: 13232 RVA: 0x0011D74D File Offset: 0x0011B94D
		public bool HasIntelOnCouncilorBasicData(TICouncilorState councilor)
		{
			return this.SufficientIntel(councilor, TemplateManager.global.intelToSeeCouncilorBasicData);
		}

		// Token: 0x060033B1 RID: 13233 RVA: 0x0011D760 File Offset: 0x0011B960
		public bool HasIntelOnCouncilorDetails(TICouncilorState councilor)
		{
			return this.SufficientIntel(councilor, TemplateManager.global.intelToSeeCouncilorDetails);
		}

		// Token: 0x060033B2 RID: 13234 RVA: 0x0011D773 File Offset: 0x0011B973
		public bool HasIntelOnCouncilorMission(TICouncilorState councilor)
		{
			return this.SufficientIntel(councilor, TemplateManager.global.intelToSeeCouncilorMission);
		}

		// Token: 0x060033B3 RID: 13235 RVA: 0x0011D786 File Offset: 0x0011B986
		public bool HasIntelOnCouncilorSecrets(TICouncilorState councilor)
		{
			return this.SufficientIntel(councilor, TemplateManager.global.intelToSeeCouncilorSecrets);
		}

		// Token: 0x060033B4 RID: 13236 RVA: 0x0011D799 File Offset: 0x0011B999
		public bool HasIntelOnSpaceAssetLocation(TISpaceAssetState asset)
		{
			return this.SufficientIntel(asset, TemplateManager.global.intelToSeeSpaceAssetLocationandComposition);
		}

		// Token: 0x060033B5 RID: 13237 RVA: 0x0011D7AC File Offset: 0x0011B9AC
		public bool HasIntelOnFleetShipDetails(TISpaceFleetState fleet)
		{
			return this.SufficientIntel(fleet, TemplateManager.global.intelToSeeFleetShipDetails);
		}

		// Token: 0x060033B6 RID: 13238 RVA: 0x0011D7BF File Offset: 0x0011B9BF
		public bool HasIntelOnUndercoverCouncilorsInSpaceAsset(TISpaceAssetState asset)
		{
			return this.SufficientIntel(asset, TemplateManager.global.intelToSeeSpaceAssetUndercoverEnemyCouncilors);
		}

		// Token: 0x060033B7 RID: 13239 RVA: 0x0011D7D4 File Offset: 0x0011B9D4
		public void ExpireIntel(TIGameState gameState, bool alsoFromHighest)
		{
			if (this.intel.ContainsKey(gameState))
			{
				this.SetIntel(gameState, 0f, null, false);
				this.intel.Remove(gameState);
				if (alsoFromHighest && this.highestIntel.ContainsKey(gameState))
				{
					this.highestIntel.Remove(gameState);
				}
			}
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x0011D828 File Offset: 0x0011BA28
		public void PingForAlienSpaceAssetDetection()
		{
			if (!this.IsAlienFaction)
			{
				TIFactionState tifactionState = GameStateManager.AlienFaction();
				foreach (TISpaceFleetState tispaceFleetState in tifactionState.fleets)
				{
					float num = tispaceFleetState.BaselineIntelOnAlienAsset(this);
					if (this.GetIntel(tispaceFleetState) < num)
					{
						this.SetIntel(tispaceFleetState, num, null, false);
					}
				}
				foreach (TIHabState tihabState in tifactionState.habs)
				{
					float num2 = tihabState.BaselineIntelOnAlienAsset(this);
					if (this.GetIntel(tihabState) < num2)
					{
						this.SetIntel(tihabState, num2, null, false);
					}
				}
			}
		}

		// Token: 0x060033B9 RID: 13241 RVA: 0x0011D900 File Offset: 0x0011BB00
		public void DegradeIntelOnVariousThings()
		{
			HashSet<TIGameState> hashSet = new HashSet<TIGameState>();
			float phasesPerMonth = TIMissionPhaseState.phasesPerMonth;
			foreach (TIGameState tigameState in this.intel.Keys.ToList<TIGameState>())
			{
				if (tigameState.isRegionState)
				{
					hashSet.Add(tigameState.ref_region);
				}
				else if (tigameState.isCouncilorState && tigameState.ref_faction != null)
				{
					TICouncilorState ref_councilor = tigameState.ref_councilor;
					if (ref_councilor.status != CouncilorStatus.None && !ref_councilor.enemyFactionsTargetingMe.Contains(this))
					{
						float num = this.GetIntel(ref_councilor);
						bool flag = ref_councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.LoyaltyMonitor);
						float num2 = 0f;
						if (ref_councilor.agentForFaction == this || (ref_councilor.faction == this && (ref_councilor.faction.knownSpies.Contains(ref_councilor) || flag)))
						{
							num2 = TemplateManager.global.intelToSeeCouncilorSecrets;
						}
						else if (ref_councilor.faction == this || ref_councilor.detainingFaction == this || (ref_councilor.faction.permanentAlly(this) && num >= TemplateManager.global.intelToSeeCouncilorBasicData) || this.factionsCompromised.Contains(ref_councilor.faction))
						{
							num2 = TemplateManager.global.myCouncilorBaselineIntel;
						}
						float num3 = (ref_councilor.isAlien ? 1f : (1f - num / 2f));
						if (num2 == 0f)
						{
							if (TIUtilities.RandomFloatValue() <= num3)
							{
								hashSet.Add(ref_councilor);
							}
							else
							{
								float num4 = Mathf.Min((float)TIUtilities.RandomRange(1, 3) * 0.25f, num);
								this.GainIntel(ref_councilor, -num4, null, true);
							}
						}
						else
						{
							this.SetIntelIfValueLower(ref_councilor, num2, null, true);
						}
					}
				}
				else if (tigameState.isSpaceFleetState || tigameState.isHabState)
				{
					this.GainIntel(tigameState, -0.2f / phasesPerMonth, null, false);
				}
				else if (tigameState.isFactionState && tigameState != this && (!tigameState.ref_faction.IsAlienFaction || this.highestIntel[tigameState] >= TemplateManager.global.intelToSeeFactionBasicData))
				{
					this.GainIntelToMinimum(tigameState, -0.1f, TemplateManager.global.intelToSeeFactionBasicData, null, 1f);
				}
				if (this.GetIntel(tigameState) <= 0f)
				{
					hashSet.Add(tigameState);
				}
			}
			foreach (TIGameState tigameState2 in hashSet)
			{
				this.ExpireIntel(tigameState2, true);
			}
		}

		// Token: 0x060033BA RID: 13242 RVA: 0x0011DBFC File Offset: 0x0011BDFC
		public CouncilorView GetViewofCouncilor(TICouncilorState targetCouncilor)
		{
			return new CouncilorView(targetCouncilor, this);
		}

		// Token: 0x060033BB RID: 13243 RVA: 0x0011DC05 File Offset: 0x0011BE05
		public FactionView GetViewofFaction(TIFactionState targetFaction)
		{
			return new FactionView(targetFaction, this);
		}

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x060033BC RID: 13244 RVA: 0x0011DC10 File Offset: 0x0011BE10
		public List<TISpaceFleetState> KnownFleets
		{
			get
			{
				List<TISpaceFleetState> list = new List<TISpaceFleetState>();
				foreach (TISpaceFleetState tispaceFleetState in GameStateManager.IterateByClass<TISpaceFleetState>(false))
				{
					if (tispaceFleetState.VisibleToFaction(this))
					{
						list.Add(tispaceFleetState);
					}
				}
				return list;
			}
		}

		// Token: 0x060033BD RID: 13245 RVA: 0x0011DC70 File Offset: 0x0011BE70
		public bool CanTargetFleet(TISpaceFleetState targetFleet)
		{
			if (!targetFleet.VisibleToFaction(this) || targetFleet.landed)
			{
				return false;
			}
			if (!targetFleet.inTransfer)
			{
				return this.CanExplore(targetFleet.orbitState);
			}
			return targetFleet.trajectory.destinationOrbit == null || this.CanExplore(targetFleet.trajectory.destinationOrbit);
		}

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x060033BE RID: 13246 RVA: 0x0011DCCC File Offset: 0x0011BECC
		public List<TISpaceFleetState> TargetableFleets
		{
			get
			{
				List<TISpaceFleetState> list = new List<TISpaceFleetState>();
				foreach (TISpaceFleetState tispaceFleetState in GameStateManager.IterateByClass<TISpaceFleetState>(false))
				{
					if (this.CanTargetFleet(tispaceFleetState))
					{
						list.Add(tispaceFleetState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x060033BF RID: 13247 RVA: 0x0011DD2C File Offset: 0x0011BF2C
		public List<TIHabState> KnownHabs
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
				{
					if (tihabState.VisibleToFaction(this))
					{
						list.Add(tihabState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x060033C0 RID: 13248 RVA: 0x0011DD8C File Offset: 0x0011BF8C
		public List<TIHabState> KnownStations
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
				{
					if (tihabState.VisibleToFaction(this) && tihabState.IsStation)
					{
						list.Add(tihabState);
					}
				}
				return list;
			}
		}

		// Token: 0x060033C1 RID: 13249 RVA: 0x0011DDF4 File Offset: 0x0011BFF4
		public bool CanTargetStation(TIHabState hab)
		{
			return hab.IsStation && hab.VisibleToFaction(this) && this.CanExplore(hab);
		}

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x060033C2 RID: 13250 RVA: 0x0011DE10 File Offset: 0x0011C010
		public List<TIHabState> TargetableStations
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
				{
					if (this.CanTargetStation(tihabState))
					{
						list.Add(tihabState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x060033C3 RID: 13251 RVA: 0x0011DE70 File Offset: 0x0011C070
		public List<TIHabState> KnownBases
		{
			get
			{
				List<TIHabState> list = new List<TIHabState>();
				foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
				{
					if (tihabState.VisibleToFaction(this) && tihabState.habType == HabType.Base)
					{
						list.Add(tihabState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x060033C4 RID: 13252 RVA: 0x0011DED8 File Offset: 0x0011C0D8
		public List<TIOrbitState> TargetableOrbitsForBuilding
		{
			get
			{
				List<TIOrbitState> list = new List<TIOrbitState>();
				foreach (TIOrbitState tiorbitState in GameStateManager.IterateByClass<TIOrbitState>(false))
				{
					if (this.CanExplore(tiorbitState))
					{
						list.Add(tiorbitState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x060033C5 RID: 13253 RVA: 0x0011DF38 File Offset: 0x0011C138
		public bool FullSystemVisibility
		{
			get
			{
				return this.IsAlienFaction || this.GetAlienDetectionRange_AU >= (double)TemplateManager.global.totalSystemDetection_AU;
			}
		}

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x060033C6 RID: 13254 RVA: 0x0011DF5A File Offset: 0x0011C15A
		public double GetAlienDetectionRange_AU
		{
			get
			{
				return (double)((this.IsAlienFaction || TemplateManager.global.debug_spaceDetection) ? 500f : (TemplateManager.global.baselineAlienAssetDetectionRange_AU + TIEffectsState.SumEffectsModifiers(Context.DetectAlienSpaceAssetsRange, this, TemplateManager.global.baselineAlienAssetDetectionRange_AU, null)));
			}
		}

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x060033C7 RID: 13255 RVA: 0x0011DF95 File Offset: 0x0011C195
		public double GetAlienDetectionRange_m
		{
			get
			{
				return this.GetAlienDetectionRange_AU * 149597870700.0;
			}
		}

		// Token: 0x060033C8 RID: 13256 RVA: 0x0011DFA7 File Offset: 0x0011C1A7
		public bool CanTargetOrbit(TIOrbitState orbit)
		{
			return this.IsAlienFaction || this.CanExplore(orbit);
		}

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x060033C9 RID: 13257 RVA: 0x0011DFBC File Offset: 0x0011C1BC
		public List<TIOrbitState> TargetableOrbitsForNavigation
		{
			get
			{
				if (this.IsAlienFaction)
				{
					return GameStateManager.AllOrbits().ToList<TIOrbitState>();
				}
				List<TIOrbitState> list = new List<TIOrbitState>();
				foreach (TIOrbitState tiorbitState in GameStateManager.AllOrbits())
				{
					if (this.CanExplore(tiorbitState))
					{
						list.Add(tiorbitState);
					}
				}
				return list;
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x060033CA RID: 13258 RVA: 0x0011E00B File Offset: 0x0011C20B
		public List<TIRegionAlienFacilityState> KnownAlienFacilities
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TIRegionAlienFacilityState>(false)
					where x.Extant() && x.VisibleToFaction(this)
					select x).ToList<TIRegionAlienFacilityState>();
			}
		}

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x060033CB RID: 13259 RVA: 0x0011E029 File Offset: 0x0011C229
		public List<TIRegionUFOLandingState> KnownUFOLandings
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TIRegionUFOLandingState>(false)
					where x.Extant() && x.VisibleToFaction(this)
					select x).ToList<TIRegionUFOLandingState>();
			}
		}

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x060033CC RID: 13260 RVA: 0x0011E047 File Offset: 0x0011C247
		public List<TIRegionAlienActivityState> KnownAbductions
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TIRegionAlienActivityState>(false)
					where x.MissionDetectedByFaction(this, "Abductions")
					select x).ToList<TIRegionAlienActivityState>();
			}
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x060033CD RID: 13261 RVA: 0x0011E065 File Offset: 0x0011C265
		public List<TIRegionAlienActivityState> KnownXenoformMissions
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TIRegionAlienActivityState>(false)
					where x.MissionDetectedByFaction(this, "Xenoform")
					select x).ToList<TIRegionAlienActivityState>();
			}
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x060033CE RID: 13262 RVA: 0x0011E083 File Offset: 0x0011C283
		public List<TIRegionXenoformingState> KnownXenoforming
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TIRegionXenoformingState>(false)
					where x.VisibleToFaction(this)
					select x).ToList<TIRegionXenoformingState>();
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x060033CF RID: 13263 RVA: 0x0011E0A1 File Offset: 0x0011C2A1
		public List<TIRegionAlienActivityState> KnownAlienActivities
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TIRegionAlienActivityState>(false)
					where x.VisibleToFaction(this)
					select x).ToList<TIRegionAlienActivityState>();
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x060033D0 RID: 13264 RVA: 0x0011E0C0 File Offset: 0x0011C2C0
		public List<TIRegionAlienEntityState> KnownAlienEntities
		{
			get
			{
				if (this.cachedKnownAlienEntitiesFrame != TIFrameCounter.FrameCount)
				{
					this.cachedKnownAlienEntities = (from x in GameStateManager.IterateByClass<TIRegionAlienEntityState>(true).ToList<TIRegionAlienEntityState>()
						where x.VisibleToFaction(this)
						select x).ToList<TIRegionAlienEntityState>();
					this.cachedKnownAlienEntitiesFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedKnownAlienEntities;
			}
		}

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x060033D1 RID: 13265 RVA: 0x0011E114 File Offset: 0x0011C314
		public List<TIRegionAlienActivityState> KnownEnthralls
		{
			get
			{
				List<TIRegionAlienActivityState> list = new List<TIRegionAlienActivityState>();
				foreach (TIRegionAlienActivityState tiregionAlienActivityState in GameStateManager.IterateByClass<TIRegionAlienActivityState>(false))
				{
					if (tiregionAlienActivityState.MissionDetectedByFaction(this, "EnthrallPublic") || tiregionAlienActivityState.MissionDetectedByFaction(this, "EnthrallElites") || tiregionAlienActivityState.MissionDetectedByFaction(this, "EnthrallOrg") || tiregionAlienActivityState.MissionDetectedByFaction(this, "EnthrallUnalignedElites"))
					{
						list.Add(tiregionAlienActivityState);
					}
				}
				return list;
			}
		}

		// Token: 0x060033D2 RID: 13266 RVA: 0x0011E1A0 File Offset: 0x0011C3A0
		public List<CouncilorView> EverKnownCouncilors(TIFactionState faction)
		{
			List<CouncilorView> list = new List<CouncilorView>();
			foreach (TICouncilorState ticouncilorState in faction.councilors)
			{
				CouncilorView viewofCouncilor = this.GetViewofCouncilor(ticouncilorState);
				if (viewofCouncilor.factionMemory == faction)
				{
					list.Add(viewofCouncilor);
				}
			}
			return list;
		}

		// Token: 0x060033D3 RID: 13267 RVA: 0x0011E214 File Offset: 0x0011C414
		public List<TICouncilorState> CurrentKnownUnidentifiedCouncilors()
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors))
			{
				if (ticouncilorState.faction != this)
				{
					CouncilorView viewofCouncilor = this.GetViewofCouncilor(ticouncilorState);
					if (viewofCouncilor.factionCurrent == null && viewofCouncilor.location != null)
					{
						list.Add(ticouncilorState);
					}
				}
			}
			return list;
		}

		// Token: 0x060033D4 RID: 13268 RVA: 0x0011E2C0 File Offset: 0x0011C4C0
		public List<TICouncilorState> CurrentKnownCouncilors(bool requireFactionIdentified, List<TIFactionState> limitToFactions = null, bool justExcludeMine = false, bool includeDetained = true)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			if (requireFactionIdentified && limitToFactions != null)
			{
				using (List<TICouncilorState>.Enumerator enumerator = limitToFactions.SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors).ToList<TICouncilorState>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TICouncilorState ticouncilorState = enumerator.Current;
						CouncilorView viewofCouncilor = this.GetViewofCouncilor(ticouncilorState);
						if (viewofCouncilor.factionCurrent != null && viewofCouncilor.location != null)
						{
							list.Add(ticouncilorState);
						}
					}
					return list;
				}
			}
			IEnumerable<TICouncilorState> enumerable = GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors);
			Func<TICouncilorState, bool> <>9__2;
			Func<TICouncilorState, bool> func;
			if ((func = <>9__2) == null)
			{
				func = (<>9__2 = (TICouncilorState x) => x.active || (x.detained & includeDetained));
			}
			foreach (TICouncilorState ticouncilorState2 in enumerable.Where<TICouncilorState>(func))
			{
				if (ticouncilorState2.faction != this || !justExcludeMine)
				{
					CouncilorView viewofCouncilor2 = this.GetViewofCouncilor(ticouncilorState2);
					if (viewofCouncilor2.location != null && (!requireFactionIdentified || viewofCouncilor2.factionCurrent != null))
					{
						list.Add(ticouncilorState2);
					}
				}
			}
			return list;
		}

		// Token: 0x060033D5 RID: 13269 RVA: 0x0011E44C File Offset: 0x0011C64C
		public void MarkAlienSite(TIGameState site, TIDateTime time = null)
		{
			TIDateTime tidateTime;
			if (time == null)
			{
				tidateTime = TITimeState.Now();
			}
			else
			{
				tidateTime = new TIDateTime(time);
			}
			if (this.knownAlienSites.ContainsKey(site))
			{
				this.knownAlienSites[site] = tidateTime;
				return;
			}
			this.knownAlienSites.Add(site, tidateTime);
		}

		// Token: 0x060033D6 RID: 13270 RVA: 0x0011E49C File Offset: 0x0011C69C
		public TIGameState MostRecentAlienSite(bool EarthOnly = true)
		{
			TIGameState tigameState = null;
			if (this.knownAlienSites.Count > 0)
			{
				TIDateTime tidateTime = this.knownAlienSites.FirstOrDefault<KeyValuePair<TIGameState, TIDateTime>>().Value;
				foreach (TIGameState tigameState2 in this.knownAlienSites.Keys)
				{
					if ((tigameState2.isRegionState || !EarthOnly) && (tidateTime == null || tidateTime <= this.knownAlienSites[tigameState2]))
					{
						tidateTime = this.knownAlienSites[tigameState2];
						tigameState = tigameState2;
					}
				}
			}
			return tigameState;
		}

		// Token: 0x060033D7 RID: 13271 RVA: 0x0011E554 File Offset: 0x0011C754
		public float MostRecentAlienSiteAge_days(bool EarthOnly = true)
		{
			TIGameState tigameState = this.MostRecentAlienSite(EarthOnly);
			if (tigameState != null)
			{
				return (float)TITimeState.Now().DifferenceInDays(this.knownAlienSites[tigameState]);
			}
			return -1f;
		}

		// Token: 0x060033D8 RID: 13272 RVA: 0x0011E58F File Offset: 0x0011C78F
		public void RegisterControlPointRecievedFromAliens(TIControlPoint controlPoint)
		{
			this.AlienControlPointGiftHistory.Add(new ValueTuple<TINationState, TIDateTime>(controlPoint.nation, TITimeState.Now()));
		}

		// Token: 0x060033D9 RID: 13273 RVA: 0x0011E5AC File Offset: 0x0011C7AC
		public float GetFactionHate(TIFactionState enemyCouncil)
		{
			if (enemyCouncil != null && this.factionHate.ContainsKey(enemyCouncil))
			{
				return this.factionHate[enemyCouncil];
			}
			return 0f;
		}

		// Token: 0x060033DA RID: 13274 RVA: 0x0011E5D7 File Offset: 0x0011C7D7
		public float GetEstimatedAlienHate()
		{
			return this.assessedAlienHateOfMe;
		}

		// Token: 0x060033DB RID: 13275 RVA: 0x0011E5DF File Offset: 0x0011C7DF
		public TIDateTime GetLastDateofFixedAlienHate()
		{
			return this.lastDateOfFixedAlienHate;
		}

		// Token: 0x060033DC RID: 13276 RVA: 0x0011E5E8 File Offset: 0x0011C7E8
		public void FixAssessedAlienHateToActualValue()
		{
			float num = this.assessedAlienHateOfMe;
			this.assessedAlienHateOfMe = GameStateManager.AlienFaction().GetFactionHate(this);
			this.lastDateOfFixedAlienHate = TITimeState.Now();
			if (this.assessedAlienHateOfMe != num)
			{
				GameControl.eventManager.TriggerEvent(new AlienThreatUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x060033DD RID: 13277 RVA: 0x0011E63C File Offset: 0x0011C83C
		public void UpdateEstimatedAlienHate(float value, bool force = false)
		{
			float num = this.assessedAlienHateOfMe;
			this.assessedAlienHateOfMe += value;
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			float num2 = tifactionState.MinimumFactionHate(this);
			float num3 = tifactionState.MaximumFactionHate(this);
			this.assessedAlienHateOfMe = Mathf.Clamp(this.assessedAlienHateOfMe, num2, num3);
			if (this.assessedAlienHateOfMe != num || force)
			{
				GameControl.eventManager.TriggerEvent(new AlienThreatUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x060033DE RID: 13278 RVA: 0x0011E6B0 File Offset: 0x0011C8B0
		public void SetFactionHate(TIFactionState enemyCouncil, float value, bool cantConflagrate = false, string cause = "")
		{
			float num = value - this.GetFactionHate(enemyCouncil);
			this.GainFactionHate(enemyCouncil, num, cantConflagrate, (cause == string.Empty) ? "Hard Set" : cause, false);
		}

		// Token: 0x060033DF RID: 13279 RVA: 0x0011E6E8 File Offset: 0x0011C8E8
		public void GainFactionHate(TIFactionState enemyCouncil, float value, bool cantConflagrate = false, string cause = "", bool randomize = true)
		{
			if (enemyCouncil == null || this.permanentAlly(enemyCouncil))
			{
				return;
			}
			if (Mathf.Abs(value) >= 1f && randomize)
			{
				value *= TIUtilities.RandomRange(1f - TemplateManager.global.hateVariance, 1f + TemplateManager.global.hateVariance);
			}
			if (value > 0f && this.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud())
			{
				value *= 0.6f;
			}
			float num = this.MinimumFactionHate(enemyCouncil);
			float num2 = this.MaximumFactionHate(enemyCouncil);
			if (!this.factionHate.ContainsKey(enemyCouncil))
			{
				this.factionHate.Add(enemyCouncil, value);
				this.factionHate[enemyCouncil] = Mathf.Clamp(this.factionHate[enemyCouncil], num, num2);
			}
			else
			{
				Dictionary<TIFactionState, float> dictionary = this.factionHate;
				dictionary[enemyCouncil] += value;
				this.factionHate[enemyCouncil] = Mathf.Clamp(this.factionHate[enemyCouncil], num, num2);
			}
			if (this.IsAlienFaction)
			{
				enemyCouncil.UpdateEstimatedAlienHate(value, false);
			}
			if (!cantConflagrate && value > 0f)
			{
				if (this.IsAlienFaction)
				{
					float num3 = 1f;
					if (enemyCouncil == GameStateManager.AlienProxy())
					{
						num3 = 0f;
					}
					if (enemyCouncil == GameStateManager.AlienAppeaser())
					{
						num3 *= 0.3f;
						if (GameStateManager.AlienAppeaser().unlockedVictoryObjective)
						{
							num3 = 0f;
						}
					}
					if (num3 > 0f)
					{
						if (GameStateManager.AlienProxy().CanContactAlien)
						{
							GameStateManager.AlienProxy().GainFactionHate(enemyCouncil, value / 2f, true, "Alien Proxy supports contacted aliens", true);
						}
						else
						{
							GameStateManager.AlienProxy().GainFactionHate(enemyCouncil, value / 4f, true, "Alien Proxy supports aliens", true);
						}
						if (GameStateManager.AlienAppeaser() != GameStateManager.AlienProxy() && GameStateManager.AlienAppeaser().CanContactAlien)
						{
							GameStateManager.AlienAppeaser().GainFactionHate(enemyCouncil, value / 3f, true, "Alien Appeaser supports aliens", true);
							return;
						}
					}
				}
				else if (this.IsAlienProxy && (!enemyCouncil.isAlienAppeaser || !enemyCouncil.unlockedVictoryObjective))
				{
					if (GameStateManager.AlienProxy().CanContactAlien)
					{
						GameStateManager.AlienFaction().GainFactionHate(enemyCouncil, value / 4f, true, "Aliens support contacted Proxy", true);
						return;
					}
					GameStateManager.AlienFaction().GainFactionHate(enemyCouncil, value / 8f, true, "Aliens support Proxy", true);
					return;
				}
				else if (this.isAlienAppeaser && !this.IsAlienProxy && this.CanContactAlien)
				{
					GameStateManager.AlienFaction().GainFactionHate(enemyCouncil, value / 10f, true, "Aliens support contacted Appeasers", true);
				}
			}
		}

		// Token: 0x060033E0 RID: 13280 RVA: 0x0011E96D File Offset: 0x0011CB6D
		public bool ShouldWorryAboutMCBasedAlienHate()
		{
			return !this.veryProAlien && GameStateManager.AlienFaction().MCBasedAlienHate(this) * 1.2f >= TemplateManager.global.alienFactionHateWarValue;
		}

		// Token: 0x060033E1 RID: 13281 RVA: 0x0011E99C File Offset: 0x0011CB9C
		public float MCBasedAlienHate(TIFactionState enemyFaction)
		{
			if (this.IsAlienFaction && !enemyFaction.veryProAlien)
			{
				float num = (float)enemyFaction.missionControlUsage * TemplateManager.global.AI_AlienHatePerMCUtilitizedMultiplier();
				return num + TIEffectsState.SumEffectsModifiers(Context.AlienHateFromMCUsage, enemyFaction, num, null);
			}
			return 0f;
		}

		// Token: 0x060033E2 RID: 13282 RVA: 0x0011E9E0 File Offset: 0x0011CBE0
		public float MinimumFactionHate(TIFactionState enemyFaction)
		{
			float num = this.MCBasedAlienHate(enemyFaction);
			if (this.veryProAlien && enemyFaction.veryAntiAlien)
			{
				return Mathf.Max(num, TemplateManager.global.factionHateConflictThreshold);
			}
			if (this.veryAntiAlien && enemyFaction.veryProAlien)
			{
				return Mathf.Max(num, TemplateManager.global.factionHateConflictThreshold);
			}
			return num;
		}

		// Token: 0x060033E3 RID: 13283 RVA: 0x0011EA38 File Offset: 0x0011CC38
		public float MaximumFactionHate(TIFactionState enemyFaction)
		{
			if (this.IsAlienFaction)
			{
				return Mathf.Max(TemplateManager.global.GetAlienHateMaximum(), this.MinimumFactionHate(enemyFaction));
			}
			return float.PositiveInfinity;
		}

		// Token: 0x060033E4 RID: 13284 RVA: 0x0011EA60 File Offset: 0x0011CC60
		public void SaveShipDesign(TISpaceShipTemplate shipDesign)
		{
			shipDesign.factionName = this.templateName;
			TemplateManager.Add(shipDesign, typeof(TISpaceShipTemplate), false);
			object obj = this.shipDesignsLock;
			lock (obj)
			{
				this.shipDesigns.Add(shipDesign);
			}
			this.shipDesignCount++;
			shipDesign.FinishDesigningShip();
		}

		// Token: 0x060033E5 RID: 13285 RVA: 0x0011EAD8 File Offset: 0x0011CCD8
		public void DeleteShipDesign(TISpaceShipTemplate shipDesign)
		{
			if (shipDesign.CanDeleteDesign)
			{
				shipDesign.factionName = string.Empty;
				object obj = this.shipDesignsLock;
				lock (obj)
				{
					this.shipDesigns.Remove(shipDesign);
				}
				this.shipRefitDesignNames.Remove(shipDesign.dataName);
				this.obsoleteShipDesigns.Remove(shipDesign.dataName);
				TemplateManager.Remove<TISpaceShipTemplate>(shipDesign);
			}
		}

		// Token: 0x060033E6 RID: 13286 RVA: 0x0011EB5C File Offset: 0x0011CD5C
		public IReadOnlyList<TISpaceShipTemplate> GetShipDesignsThreadSafe()
		{
			IReadOnlyList<TISpaceShipTemplate> readOnlyList = null;
			object obj = this.shipDesignsLock;
			lock (obj)
			{
				readOnlyList = new List<TISpaceShipTemplate>(this.shipDesigns);
			}
			return readOnlyList;
		}

		// Token: 0x060033E7 RID: 13287 RVA: 0x0011EBA8 File Offset: 0x0011CDA8
		private IEnumerable<TIShipPartTemplate> GetPartVariations(TIShipPartTemplate part)
		{
			IEnumerable<TIShipPartTemplate> enumerable = Enumerable.Empty<TIShipPartTemplate>().Append(part);
			TIDriveTemplate tidriveTemplate = part as TIDriveTemplate;
			if (tidriveTemplate != null)
			{
				enumerable = tidriveTemplate.Variations;
			}
			return enumerable;
		}

		// Token: 0x060033E8 RID: 13288 RVA: 0x0011EBD4 File Offset: 0x0011CDD4
		public void SetShipPartObsolete(TIShipPartTemplate part, bool includeVariations = true)
		{
			this.shipDesigner_CachedDriveStats.Clear();
			if (!includeVariations)
			{
				this.obsoletedShipParts.AddUnique(part.dataName);
				return;
			}
			foreach (TIShipPartTemplate tishipPartTemplate in this.GetPartVariations(part))
			{
				this.obsoletedShipParts.AddUnique(tishipPartTemplate.dataName);
			}
		}

		// Token: 0x060033E9 RID: 13289 RVA: 0x0011EC50 File Offset: 0x0011CE50
		public void SetShipPartNotObsolete(TIShipPartTemplate part, bool includeVariations = true)
		{
			this.shipDesigner_CachedDriveStats.Clear();
			if (!includeVariations)
			{
				this.obsoletedShipParts.Remove(part.dataName);
				return;
			}
			foreach (TIShipPartTemplate tishipPartTemplate in this.GetPartVariations(part))
			{
				this.obsoletedShipParts.Remove(tishipPartTemplate.dataName);
			}
		}

		// Token: 0x060033EA RID: 13290 RVA: 0x0011ECCC File Offset: 0x0011CECC
		public void SetDesignerShowObsoletePartsSetting(bool show)
		{
			this.showObsoleteParts = show;
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x060033EB RID: 13291 RVA: 0x0011ECD5 File Offset: 0x0011CED5
		public IEnumerable<TIShipHullTemplate> allowedShipHulls
		{
			get
			{
				return this.cachedAllowedShipHulls;
			}
		}

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x060033EC RID: 13292 RVA: 0x0011ECDD File Offset: 0x0011CEDD
		public IEnumerable<TIRadiatorTemplate> allowedRadiators
		{
			get
			{
				return this.cachedAllowedRadiators;
			}
		}

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x060033ED RID: 13293 RVA: 0x0011ECE5 File Offset: 0x0011CEE5
		public IEnumerable<TIDriveTemplate> allowedDrives
		{
			get
			{
				return this.cachedAllowedDrives;
			}
		}

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x060033EE RID: 13294 RVA: 0x0011ECED File Offset: 0x0011CEED
		public IEnumerable<TIBatteryTemplate> allowedBatteries
		{
			get
			{
				return this.cachedAllowedBatteries;
			}
		}

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x060033EF RID: 13295 RVA: 0x0011ECF5 File Offset: 0x0011CEF5
		public IEnumerable<TIShipArmorTemplate> allowedArmors
		{
			get
			{
				return this.cachedAllowedArmors;
			}
		}

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x060033F0 RID: 13296 RVA: 0x0011ECFD File Offset: 0x0011CEFD
		public IEnumerable<TIPowerPlantTemplate> allowedPowerPlants
		{
			get
			{
				return this.cachedAllowedPowerPlants;
			}
		}

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x060033F1 RID: 13297 RVA: 0x0011ED05 File Offset: 0x0011CF05
		public IEnumerable<TIShipWeaponTemplate> allowedNoseWeapons
		{
			get
			{
				return this.cachedAllowedNoseWeapons;
			}
		}

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x060033F2 RID: 13298 RVA: 0x0011ED0D File Offset: 0x0011CF0D
		public IEnumerable<TIShipWeaponTemplate> allowedHullWeapons
		{
			get
			{
				return this.cachedAllowedHullWeapons;
			}
		}

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x060033F3 RID: 13299 RVA: 0x0011ED15 File Offset: 0x0011CF15
		public IEnumerable<TIHeatSinkTemplate> allowedHeatSinks
		{
			get
			{
				return this.cachedAllowedHeatSinks;
			}
		}

		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x060033F4 RID: 13300 RVA: 0x0011ED1D File Offset: 0x0011CF1D
		public IEnumerable<TIUtilityModuleTemplate> allowedUtilityModules
		{
			get
			{
				return this.cachedAllowedUtilityModules;
			}
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x0011ED28 File Offset: 0x0011CF28
		public void UpdateAllowedShipParts(List<TIShipPartTemplate> newParts = null)
		{
			if (TemplateManager.global.debug_showAllShipPartsIncludingAlien)
			{
				if (this.IsAlienFaction)
				{
					this.cachedAllowedShipHulls = (from x in TemplateManager.IterateByClass<TIShipHullTemplate>(true)
						where x.isAlien && !x.noShipyardBuild
						select x).ToList<TIShipHullTemplate>();
					this.cachedAllowedRadiators = (from x in TemplateManager.IterateByClass<TIRadiatorTemplate>(true)
						where x.isAlien
						select x).ToList<TIRadiatorTemplate>();
					this.cachedAllowedDrives = (from x in TemplateManager.IterateByClass<TIDriveTemplate>(true)
						where x.isAlien
						select x).ToList<TIDriveTemplate>();
					this.cachedAllowedBatteries = (from x in TemplateManager.IterateByClass<TIBatteryTemplate>(true)
						where x.isAlien
						select x).ToList<TIBatteryTemplate>();
					this.cachedAllowedArmors = (from x in TemplateManager.IterateByClass<TIShipArmorTemplate>(true)
						where x.isAlien
						select x).ToList<TIShipArmorTemplate>();
					this.cachedAllowedPowerPlants = (from x in TemplateManager.IterateByClass<TIPowerPlantTemplate>(true)
						where x.isAlien
						select x).ToList<TIPowerPlantTemplate>();
					this.cachedAllowedNoseWeapons = (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
						where x.isAlien
						select x into module
						where module.noseWeapon && module.mount != Mount.HalfNose
						select module).ToList<TIShipWeaponTemplate>();
					this.cachedAllowedHullWeapons = (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
						where x.isAlien
						select x into module
						where module.hullWeapon && module.mount != Mount.HalfHull
						select module).ToList<TIShipWeaponTemplate>();
					this.cachedAllowedHeatSinks = (from x in TemplateManager.IterateByClass<TIHeatSinkTemplate>(true)
						where x.isAlien
						select x).ToList<TIHeatSinkTemplate>();
					this.cachedAllowedUtilityModules = (from x in TemplateManager.IterateByClass<TIUtilityModuleTemplate>(true)
						where x.isAlien
						select x).ToList<TIUtilityModuleTemplate>();
					return;
				}
				this.cachedAllowedShipHulls = (from x in TemplateManager.IterateByClass<TIShipHullTemplate>(true)
					where !x.isAlien && !x.noShipyardBuild
					select x).ToList<TIShipHullTemplate>();
				this.cachedAllowedRadiators = (from x in TemplateManager.IterateByClass<TIRadiatorTemplate>(true)
					where !x.isAlien
					select x).ToList<TIRadiatorTemplate>();
				this.cachedAllowedDrives = (from x in TemplateManager.IterateByClass<TIDriveTemplate>(true)
					where !x.isAlien
					select x).ToList<TIDriveTemplate>();
				this.cachedAllowedBatteries = (from x in TemplateManager.IterateByClass<TIBatteryTemplate>(true)
					where !x.isAlien
					select x).ToList<TIBatteryTemplate>();
				this.cachedAllowedArmors = (from x in TemplateManager.IterateByClass<TIShipArmorTemplate>(true)
					where !x.isAlien
					select x).ToList<TIShipArmorTemplate>();
				this.cachedAllowedPowerPlants = (from x in TemplateManager.IterateByClass<TIPowerPlantTemplate>(true)
					where !x.isAlien
					select x).ToList<TIPowerPlantTemplate>();
				this.cachedAllowedNoseWeapons = (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
					where !x.isAlien
					select x into module
					where module.noseWeapon && module.mount != Mount.HalfNose
					select module).ToList<TIShipWeaponTemplate>();
				this.cachedAllowedHullWeapons = (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
					where !x.isAlien
					select x into module
					where module.hullWeapon && module.mount != Mount.HalfHull
					select module).ToList<TIShipWeaponTemplate>();
				this.cachedAllowedHeatSinks = (from x in TemplateManager.IterateByClass<TIHeatSinkTemplate>(true)
					where !x.isAlien
					select x).ToList<TIHeatSinkTemplate>();
				this.cachedAllowedUtilityModules = (from x in TemplateManager.IterateByClass<TIUtilityModuleTemplate>(true)
					where !x.isAlien
					select x).ToList<TIUtilityModuleTemplate>();
				return;
			}
			else
			{
				if (newParts == null)
				{
					this.cachedAllowedShipHulls = (from hull in TemplateManager.IterateByClass<TIShipHullTemplate>(true)
						where hull.FactionCanBuild(this) && !hull.noShipyardBuild
						select hull).ToList<TIShipHullTemplate>();
					this.cachedAllowedRadiators = (from radiator in TemplateManager.IterateByClass<TIRadiatorTemplate>(true)
						where radiator.FactionCanBuild(this)
						select radiator).ToList<TIRadiatorTemplate>();
					this.cachedAllowedDrives = (from drive in TemplateManager.IterateByClass<TIDriveTemplate>(true)
						where drive.FactionCanBuild(this)
						select drive).ToList<TIDriveTemplate>();
					this.cachedAllowedBatteries = (from battery in TemplateManager.IterateByClass<TIBatteryTemplate>(true)
						where battery.FactionCanBuild(this)
						select battery).ToList<TIBatteryTemplate>();
					this.cachedAllowedArmors = (from armor in TemplateManager.IterateByClass<TIShipArmorTemplate>(true)
						where armor.FactionCanBuild(this)
						select armor).ToList<TIShipArmorTemplate>();
					this.cachedAllowedPowerPlants = (from module in TemplateManager.IterateByClass<TIPowerPlantTemplate>(true)
						where module.FactionCanBuild(this)
						select module).ToList<TIPowerPlantTemplate>();
					this.cachedAllowedNoseWeapons = (from module in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
						where module.noseWeapon && module.mount != Mount.HalfNose && module.FactionCanBuild(this)
						select module).ToList<TIShipWeaponTemplate>();
					this.cachedAllowedHullWeapons = (from module in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
						where module.hullWeapon && module.mount != Mount.HalfHull && module.FactionCanBuild(this)
						select module).ToList<TIShipWeaponTemplate>();
					this.cachedAllowedHeatSinks = (from module in TemplateManager.IterateByClass<TIHeatSinkTemplate>(true)
						where module.FactionCanBuild(this)
						select module).ToList<TIHeatSinkTemplate>();
					this.cachedAllowedUtilityModules = (from module in TemplateManager.IterateByClass<TIUtilityModuleTemplate>(true)
						where module.FactionCanBuild(this)
						select module).ToList<TIUtilityModuleTemplate>();
					return;
				}
				foreach (TIShipPartTemplate tishipPartTemplate in newParts)
				{
					if (tishipPartTemplate is TIShipHullTemplate)
					{
						this.cachedAllowedShipHulls = (from hull in TemplateManager.IterateByClass<TIShipHullTemplate>(true)
							where hull.FactionCanBuild(this) && !hull.noShipyardBuild
							select hull).ToList<TIShipHullTemplate>();
					}
					else if (tishipPartTemplate.isRadiator)
					{
						this.cachedAllowedRadiators = (from radiator in TemplateManager.IterateByClass<TIRadiatorTemplate>(true)
							where radiator.FactionCanBuild(this)
							select radiator).ToList<TIRadiatorTemplate>();
					}
					else if (tishipPartTemplate.isDrive)
					{
						this.cachedAllowedDrives = (from drive in TemplateManager.IterateByClass<TIDriveTemplate>(true)
							where drive.FactionCanBuild(this)
							select drive).ToList<TIDriveTemplate>();
					}
					else if (tishipPartTemplate.isBattery)
					{
						this.cachedAllowedBatteries = (from battery in TemplateManager.IterateByClass<TIBatteryTemplate>(true)
							where battery.FactionCanBuild(this)
							select battery).ToList<TIBatteryTemplate>();
					}
					else if (tishipPartTemplate.isArmor)
					{
						this.cachedAllowedArmors = (from armor in TemplateManager.IterateByClass<TIShipArmorTemplate>(true)
							where armor.FactionCanBuild(this)
							select armor).ToList<TIShipArmorTemplate>();
					}
					else if (tishipPartTemplate.isPowerPlant)
					{
						this.cachedAllowedPowerPlants = (from module in TemplateManager.IterateByClass<TIPowerPlantTemplate>(true)
							where module.FactionCanBuild(this)
							select module).ToList<TIPowerPlantTemplate>();
					}
					else if (tishipPartTemplate.isWeapon)
					{
						this.cachedAllowedNoseWeapons = (from module in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
							where module.noseWeapon && module.mount != Mount.HalfNose && module.FactionCanBuild(this)
							select module).ToList<TIShipWeaponTemplate>();
						this.cachedAllowedHullWeapons = (from module in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
							where module.hullWeapon && module.mount != Mount.HalfHull && module.FactionCanBuild(this)
							select module).ToList<TIShipWeaponTemplate>();
					}
					else if (tishipPartTemplate.isHeatSink)
					{
						this.cachedAllowedHeatSinks = (from module in TemplateManager.IterateByClass<TIHeatSinkTemplate>(true)
							where module.FactionCanBuild(this)
							select module).ToList<TIHeatSinkTemplate>();
					}
					else if (tishipPartTemplate.isUtilityModule)
					{
						this.cachedAllowedUtilityModules = (from module in TemplateManager.IterateByClass<TIUtilityModuleTemplate>(true)
							where module.FactionCanBuild(this)
							select module).ToList<TIUtilityModuleTemplate>();
					}
				}
				return;
			}
		}

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x060033F6 RID: 13302 RVA: 0x0011F56C File Offset: 0x0011D76C
		public List<TIShipPartTemplate> allowedShipParts
		{
			get
			{
				List<TIShipPartTemplate> list = new List<TIShipPartTemplate>();
				list.AddRange(this.allowedShipHulls);
				list.AddRange(this.allowedRadiators);
				list.AddRange(this.allowedDrives);
				list.AddRange(this.allowedBatteries);
				list.AddRange(this.allowedPowerPlants);
				list.AddRange(this.allowedNoseWeapons);
				list.AddRange(this.allowedHullWeapons);
				list.AddRange(this.allowedHeatSinks);
				list.AddRange(this.allowedUtilityModules);
				return list;
			}
		}

		// Token: 0x060033F7 RID: 13303 RVA: 0x0011F5EC File Offset: 0x0011D7EC
		public TIRadiatorTemplate GetBestRadiatorRaw()
		{
			return this.allowedRadiators.Where<TIRadiatorTemplate>((TIRadiatorTemplate x) => x.buildCost(1f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f).MaxBy<TIRadiatorTemplate, float>((TIRadiatorTemplate x) => x.AIScoringValueForResearch());
		}

		// Token: 0x060033F8 RID: 13304 RVA: 0x0011F648 File Offset: 0x0011D848
		public TIRadiatorTemplate GetBestRadiator(TISpaceShipTemplate design, bool allowExotics)
		{
			List<TIRadiatorTemplate> list = new List<TIRadiatorTemplate>();
			foreach (TIRadiatorTemplate tiradiatorTemplate in this.allowedRadiators)
			{
				TIResourcesCost tiresourcesCost = tiradiatorTemplate.buildCost(tiradiatorTemplate.buildMass_tons(design.wasteHeat_GW, 0f, 0f, 0f, false), 0f);
				if ((!allowExotics || tiresourcesCost.GetSingleCostValue(FactionResource.Exotics) <= this.GetCurrentResourceAmount(FactionResource.Exotics)) && tiresourcesCost.GetSingleCostValue(FactionResource.Exotics) <= 0f)
				{
					list.Add(tiradiatorTemplate);
				}
			}
			list = this.GetObsoleteFilteredParts<TIRadiatorTemplate>(list).ToList<TIRadiatorTemplate>();
			return list.MaxBy<TIRadiatorTemplate, float>((TIRadiatorTemplate radiator) => radiator.AIScoringValueForResearch());
		}

		// Token: 0x060033F9 RID: 13305 RVA: 0x0011F71C File Offset: 0x0011D91C
		public TIBatteryTemplate GetBestBattery(TISpaceShipTemplate design, bool allowExotics)
		{
			List<TIBatteryTemplate> list = ((design != null) ? design.batteryTemplates : null);
			if (design == null || (list != null && list.Count == 0))
			{
				IEnumerable<TIBatteryTemplate> enumerable = this.allowedBatteries.Where<TIBatteryTemplate>((TIBatteryTemplate x) => allowExotics || x.buildCost(0f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f);
				enumerable = this.GetObsoleteFilteredParts<TIBatteryTemplate>(enumerable).ToList<TIBatteryTemplate>();
				return enumerable.MaxBy<TIBatteryTemplate, float>((TIBatteryTemplate battery) => battery.AIScoringValueForResearch());
			}
			return list[0];
		}

		// Token: 0x060033FA RID: 13306 RVA: 0x0011F7A4 File Offset: 0x0011D9A4
		public TIShipArmorTemplate GetBestArmor(bool allowExotics)
		{
			IEnumerable<TIShipArmorTemplate> enumerable = this.allowedArmors.Where<TIShipArmorTemplate>((TIShipArmorTemplate x) => allowExotics || x.buildCost(1f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f);
			enumerable = this.GetObsoleteFilteredParts<TIShipArmorTemplate>(enumerable).ToList<TIShipArmorTemplate>();
			return enumerable.MinBy<TIShipArmorTemplate, float>((TIShipArmorTemplate armor) => armor.mass_damagePoint_kg);
		}

		// Token: 0x060033FB RID: 13307 RVA: 0x0011F808 File Offset: 0x0011DA08
		public TIDriveTemplate GetBestDrive(ShipRole role, int thrusters, bool allowAntimatter, bool allowExotics, float desiredStrategicRange_AU)
		{
			Dictionary<TIDriveTemplate, float> dictionary = new Dictionary<TIDriveTemplate, float>();
			List<TIDriveTemplate> list = this.allowedDrives.Where<TIDriveTemplate>((TIDriveTemplate x) => x.thrusters == thrusters).ToList<TIDriveTemplate>();
			list = TISpaceShipTemplate.ValidDrivesForPowerPlants(list, this.allowedPowerPlants);
			list = this.GetObsoleteFilteredParts<TIDriveTemplate>(list).ToList<TIDriveTemplate>();
			if (this.IsActiveHumanFaction)
			{
				if (list.Any<TIDriveTemplate>((TIDriveTemplate x) => x.thrust_N >= 100000f && x.EV_kps >= 8f))
				{
					list.RemoveAll((TIDriveTemplate x) => x.thrust_N < 100000f || x.EV_kps < 8f);
				}
			}
			if (desiredStrategicRange_AU > 1.02f)
			{
				List<TIDriveTemplate> list2 = list.Where<TIDriveTemplate>((TIDriveTemplate x) => (double)x.EV_kps > 2.5 * (double)desiredStrategicRange_AU).ToList<TIDriveTemplate>();
				if (list2.Count<TIDriveTemplate>() > 0 && desiredStrategicRange_AU > 6f)
				{
					list2 = list2.Where<TIDriveTemplate>((TIDriveTemplate x) => x.thrust_N >= 20000f).ToList<TIDriveTemplate>();
				}
				if (list2.Count<TIDriveTemplate>() > 0)
				{
					list = list2;
				}
			}
			foreach (TIDriveTemplate tidriveTemplate in list)
			{
				float num;
				switch (role)
				{
				case ShipRole.TroopCarrier:
				case ShipRole.ArmyCarrier:
				case ShipRole.EarthSurveillance:
				case ShipRole.CouncilorTransport:
					num = tidriveTemplate.thrustRating / 2f + tidriveTemplate.EVRating * 36f;
					break;
				default:
					num = tidriveTemplate.EVRating;
					break;
				case ShipRole.LS_Penetrator:
					num = tidriveTemplate.thrustRating * 4f + tidriveTemplate.EVRating * 36f;
					break;
				case ShipRole.LM_Interdictor:
					num = tidriveTemplate.thrustRating * 2f + tidriveTemplate.EVRating * 36f;
					break;
				case ShipRole.LL_Intruder:
					num = tidriveTemplate.thrustRating + tidriveTemplate.EVRating * 36f;
					break;
				case ShipRole.MS_Strike:
					num = tidriveTemplate.thrustRating * 4f + tidriveTemplate.EVRating * 24f;
					break;
				case ShipRole.MM_SpaceSuperiority:
					num = tidriveTemplate.thrustRating * 2f + tidriveTemplate.EVRating * 24f;
					break;
				case ShipRole.ML_Standoff:
					num = tidriveTemplate.thrustRating + tidriveTemplate.EVRating * 24f;
					break;
				case ShipRole.SS_Interceptor:
					num = tidriveTemplate.thrustRating * 4f + tidriveTemplate.EVRating * 12f;
					break;
				case ShipRole.SM_Patrol:
					num = tidriveTemplate.thrustRating * 2f + tidriveTemplate.EVRating * 12f;
					break;
				case ShipRole.SL_Defender:
					num = tidriveTemplate.thrustRating + tidriveTemplate.EVRating * 12f;
					break;
				}
				num *= (float)(tidriveTemplate.openCycleCooling ? 2 : 1);
				if (this.IsAlienFaction)
				{
					if ((allowExotics && (tidriveTemplate.GetPerTankPropellantMaterials(this).exotics > 0f || tidriveTemplate.weightedBuildMaterials.exotics > 0f)) || (!allowExotics && tidriveTemplate.GetPerTankPropellantMaterials(this).exotics == 0f && tidriveTemplate.weightedBuildMaterials.exotics == 0f))
					{
						dictionary.Add(tidriveTemplate, num);
					}
				}
				else if ((allowAntimatter || (tidriveTemplate.GetPerTankPropellantMaterials(this).antimatter == 0f && tidriveTemplate.weightedBuildMaterials.exotics == 0f)) && (allowExotics || (tidriveTemplate.GetPerTankPropellantMaterials(this).exotics == 0f && tidriveTemplate.weightedBuildMaterials.exotics == 0f)))
				{
					dictionary.Add(tidriveTemplate, num);
				}
			}
			return dictionary.MaxBy<KeyValuePair<TIDriveTemplate, float>, float>((KeyValuePair<TIDriveTemplate, float> x) => x.Value).Key;
		}

		// Token: 0x060033FC RID: 13308 RVA: 0x0011FC20 File Offset: 0x0011DE20
		public void SetShipDesignNoseWeapons(bool playerAutodesign, ref TISpaceShipTemplate design, bool allowExotics, IEnumerable<TIShipWeaponTemplate> choices = null)
		{
			if (choices == null)
			{
				choices = this.allowedNoseWeapons;
			}
			if (!allowExotics)
			{
				choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.buildCost(0f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f);
			}
			choices = this.GetObsoleteFilteredParts<TIShipWeaponTemplate>(choices);
			if (!choices.Any<TIShipWeaponTemplate>())
			{
				return;
			}
			design.noseWeaponTemplateEntries.Clear();
			int num = design.hullTemplate.noseHardpoints;
			if (num > 0)
			{
				Dictionary<TIShipWeaponTemplate, float> scores = new Dictionary<TIShipWeaponTemplate, float>();
				foreach (TIShipWeaponTemplate tishipWeaponTemplate in choices)
				{
					float curatedDesignScore = tishipWeaponTemplate.GetCuratedDesignScore(design.role, choices, !playerAutodesign);
					scores.Add(tishipWeaponTemplate, curatedDesignScore);
				}
				choices = choices.OrderByDescending<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => scores[x]).ToList<TIShipWeaponTemplate>();
				bool flag = design.role == ShipRole.LM_Protector;
				List<List<TIShipHullTemplate.ShipModuleSlot>> list = design.hullTemplate.ValidBigWeaponSlotSets(Mount.FourNose);
				List<List<TIShipHullTemplate.ShipModuleSlot>> list2 = design.hullTemplate.ValidBigWeaponSlotSets(Mount.ThreeNoseAngle);
				List<List<TIShipHullTemplate.ShipModuleSlot>> list3 = design.hullTemplate.ValidBigWeaponSlotSets(Mount.TwoNoseVert);
				List<List<TIShipHullTemplate.ShipModuleSlot>> list4 = design.hullTemplate.ValidBigWeaponSlotSets(Mount.TwoNoseHoriz);
				List<TIShipHullTemplate.ShipModuleSlot> allSlotsOfType = design.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.NoseHardPoint);
				if (list.Count == 0 || flag)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.FourNose).ToList<TIShipWeaponTemplate>();
				}
				if (list2.Count == 0 || flag)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.ThreeNoseAngle).ToList<TIShipWeaponTemplate>();
				}
				if (list3.Count == 0)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.TwoNoseVert).ToList<TIShipWeaponTemplate>();
				}
				if (list4.Count == 0)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.TwoNoseHoriz).ToList<TIShipWeaponTemplate>();
				}
				if (choices.Any<TIShipWeaponTemplate>())
				{
					IEnumerable<TIShipWeaponTemplate> enumerable = choices;
					if (choices.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode))
					{
						enumerable = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode);
					}
					TIShipWeaponTemplate tishipWeaponTemplate2 = this.ChooseWeapon(enumerable, scores, playerAutodesign);
					int num2 = 0;
					switch (tishipWeaponTemplate2.mount)
					{
					case Mount.OneNose:
						num2 = design.hullTemplate.slotIndex(allSlotsOfType[0]);
						break;
					case Mount.TwoNoseHoriz:
						num2 = design.hullTemplate.slotIndex(list4[0][0]);
						break;
					case Mount.TwoNoseVert:
						num2 = design.hullTemplate.slotIndex(list3[0][0]);
						break;
					case Mount.ThreeNoseAngle:
						num2 = design.hullTemplate.slotIndex(list2[0][0]);
						break;
					case Mount.FourNose:
						num2 = design.hullTemplate.slotIndex(list[0][0]);
						break;
					}
					num -= tishipWeaponTemplate2.internalSize;
					design.noseWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(tishipWeaponTemplate2, num2));
				}
				List<TIShipWeaponTemplate> list5 = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount == Mount.OneNose).ToList<TIShipWeaponTemplate>();
				if (num > 0 && list5.Count > 0)
				{
					foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in allSlotsOfType)
					{
						int num3 = design.hullTemplate.slotIndex(shipModuleSlot);
						if (!design.SlotIndexOccupied(num3, true))
						{
							TIShipWeaponTemplate tishipWeaponTemplate3 = this.ChooseWeapon(list5, scores, playerAutodesign);
							design.noseWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(tishipWeaponTemplate3, num3));
							num--;
						}
					}
				}
			}
		}

		// Token: 0x060033FD RID: 13309 RVA: 0x00120068 File Offset: 0x0011E268
		public string GetBestPointDefenseWeaponTemplateName()
		{
			string text = "30mmAutocannon";
			List<TIShipWeaponTemplate> list = this.allowedHullWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.defenseMode && !x.attackMode && !x.isMissileWeapon).ToList<TIShipWeaponTemplate>();
			if (list.Count > 0)
			{
				List<TIShipWeaponTemplate> list2 = list.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isLaserWeapon).ToList<TIShipWeaponTemplate>();
				if (list2.Count > 0)
				{
					text = list2.MinBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => x.cooldown_s).dataName;
				}
				else
				{
					text = list.MinBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => x.cooldown_s).dataName;
				}
			}
			return text;
		}

		// Token: 0x060033FE RID: 13310 RVA: 0x00120144 File Offset: 0x0011E344
		public string GetBestHabWeapon(bool isBase, int tier, WeaponClass preferredClass, TISpaceBodyState parentSpaceBody, List<TIShipWeaponTemplate> notionalAdditions = null)
		{
			if (isBase)
			{
				return TILaserWeaponTemplate.GetBestHeavyDefenseLaser(this, parentSpaceBody ?? GameStateManager.Luna(), tier).dataName;
			}
			string text = string.Empty;
			if (GameControl.control.skirmishMode)
			{
				if (preferredClass == WeaponClass.Laser)
				{
					switch (tier)
					{
					case 1:
						text = (this.IsAlienFaction ? "Alien64cmOrangeLaserBattery" : "60cmGreenArcLaserBattery");
						break;
					case 2:
						text = (this.IsAlienFaction ? "Alien128cmOrangeLaserBattery" : "120cmGreenArcLaserBattery");
						break;
					case 3:
						text = (this.IsAlienFaction ? "Alien384cmOrangeLaserBattery" : "360cmGreenArcLaserBattery");
						break;
					}
				}
				else if (preferredClass == WeaponClass.Plasma)
				{
					switch (tier)
					{
					case 1:
						text = (this.IsAlienFaction ? "AlienPlasmaBattery" : "PlasmaBatteryMk2");
						break;
					case 2:
						text = (this.IsAlienFaction ? "AlienPlasmaBattery" : "PlasmaBatteryMk2");
						break;
					case 3:
						text = (this.IsAlienFaction ? "AlienHeavyPlasmaBattery" : "HeavyPlasmaBatteryMk2");
						break;
					}
				}
				else
				{
					switch (tier)
					{
					case 1:
						text = (this.IsAlienFaction ? "AdvancedAlienLightMagBattery" : "LightRailgunBatteryMk2");
						break;
					case 2:
						text = (this.IsAlienFaction ? "AdvancedAlienMagBattery" : "CoilgunBatteryMk2");
						break;
					case 3:
						text = (this.IsAlienFaction ? "AdvancedAlienHeavyMagBattery" : "HeavyCoilgunBatteryMk2");
						break;
					}
				}
			}
			else
			{
				text = (this.IsAlienFaction ? "AlienLightMagBattery" : "8-inchCannon");
				List<Mount> allowedMounts = new List<Mount> { Mount.OneHull };
				if (tier != 2)
				{
					if (tier == 3)
					{
						allowedMounts.Add(Mount.TwoHullHoriz);
						allowedMounts.Add(Mount.TwoHullVert);
						allowedMounts.Add(Mount.ThreeHullHoriz);
						allowedMounts.Add(Mount.FourHull);
					}
				}
				else
				{
					allowedMounts.Add(Mount.TwoHullHoriz);
					allowedMounts.Add(Mount.TwoHullVert);
				}
				List<TIShipWeaponTemplate> options;
				if (!this.IsAlienFaction)
				{
					options = this.allowedHullWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => this.validWeaponClassesForHumanHabs.Contains(x.weaponClass) && x.attackMode && allowedMounts.Contains(x.mount)).ToList<TIShipWeaponTemplate>();
				}
				else
				{
					options = this.allowedHullWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => this.validWeaponClassesForAlienHabs.Contains(x.weaponClass) && x.attackMode && allowedMounts.Contains(x.mount)).ToList<TIShipWeaponTemplate>();
				}
				if (notionalAdditions != null)
				{
					foreach (TIShipWeaponTemplate tishipWeaponTemplate in notionalAdditions)
					{
						if (allowedMounts.Contains(tishipWeaponTemplate.mount) && this.validWeaponClassesForHumanHabs.Contains(tishipWeaponTemplate.weaponClass) && (tishipWeaponTemplate.weaponClass != WeaponClass.Plasma || tier >= 3))
						{
							options.Add(tishipWeaponTemplate);
						}
					}
				}
				List<TIShipWeaponTemplate> list = options.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.weaponClass == preferredClass).ToList<TIShipWeaponTemplate>();
				if (list.Any<TIShipWeaponTemplate>())
				{
					options = list;
				}
				else if (this.IsAlienFaction)
				{
					options = options.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isLaserWeapon).ToList<TIShipWeaponTemplate>();
				}
				else
				{
					options = options.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isNavalGunWeapon).ToList<TIShipWeaponTemplate>();
				}
				if (options.Count > 0)
				{
					options = (from x in options
						orderby x.internalSize descending, x.targetingRange_km descending, x.GetCuratedDesignScore(ShipRole.SL_Defender, options, false) descending, x.GenericScore() descending
						select x).ToList<TIShipWeaponTemplate>();
					text = options[0].dataName;
				}
			}
			return text;
		}

		// Token: 0x060033FF RID: 13311 RVA: 0x00120590 File Offset: 0x0011E790
		private IEnumerable<TIShipWeaponTemplate> GetWeaponBasket(IEnumerable<TIShipWeaponTemplate> candidates, Dictionary<TIShipWeaponTemplate, float> scores)
		{
			Func<TIShipWeaponTemplate, float> <>9__2;
			return candidates.GroupBy<TIShipWeaponTemplate, WeaponClass>(delegate(TIShipWeaponTemplate x)
			{
				if (x.weaponClass != WeaponClass.Magnetic)
				{
					return x.weaponClass;
				}
				return WeaponClass.NavalGun;
			}).Select<IGrouping<WeaponClass, TIShipWeaponTemplate>, TIShipWeaponTemplate>(delegate(IGrouping<WeaponClass, TIShipWeaponTemplate> x)
			{
				Func<TIShipWeaponTemplate, float> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (TIShipWeaponTemplate y) => scores[y]);
				}
				return x.MaxBy<TIShipWeaponTemplate, float>(func);
			});
		}

		// Token: 0x06003400 RID: 13312 RVA: 0x001205E0 File Offset: 0x0011E7E0
		private TIShipWeaponTemplate ChooseWeapon(IEnumerable<TIShipWeaponTemplate> candidates, Dictionary<TIShipWeaponTemplate, float> scores, bool forceChooseBest)
		{
			if (forceChooseBest)
			{
				return this.GetWeaponBasket(candidates, scores).MaxBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => scores[x]);
			}
			return this.GetWeaponBasket(candidates, scores).SelectRandomWeightedItem<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => Mathf.Pow(scores[x], 1f), -1f, 1E-37f);
		}

		// Token: 0x06003401 RID: 13313 RVA: 0x00120644 File Offset: 0x0011E844
		private void SetShipDesignHullWeapons(bool playerAutoDesign, ref TISpaceShipTemplate design, bool allowExotics, IEnumerable<TIShipWeaponTemplate> choices = null)
		{
			if (choices == null)
			{
				choices = this.allowedHullWeapons;
			}
			if (!allowExotics)
			{
				choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.buildCost(0f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f);
			}
			choices = this.GetObsoleteFilteredParts<TIShipWeaponTemplate>(choices);
			if (!choices.Any<TIShipWeaponTemplate>())
			{
				return;
			}
			design.hullWeaponTemplateEntries.Clear();
			int num = design.hullTemplate.hullHardpoints;
			if (num > 0)
			{
				Dictionary<TIShipWeaponTemplate, float> scores = new Dictionary<TIShipWeaponTemplate, float>();
				ShipRole role = design.role;
				foreach (TIShipWeaponTemplate tishipWeaponTemplate in choices)
				{
					float curatedDesignScore = tishipWeaponTemplate.GetCuratedDesignScore(design.role, choices, !playerAutoDesign);
					scores.Add(tishipWeaponTemplate, curatedDesignScore);
				}
				choices = choices.OrderByDescending<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => scores[x]).ToList<TIShipWeaponTemplate>();
				bool flag = design.role == ShipRole.LM_Protector || num <= 2;
				List<List<TIShipHullTemplate.ShipModuleSlot>> list = design.hullTemplate.ValidBigWeaponSlotSets(Mount.FourHull);
				List<List<TIShipHullTemplate.ShipModuleSlot>> list2 = design.hullTemplate.ValidBigWeaponSlotSets(Mount.ThreeHullHoriz);
				List<List<TIShipHullTemplate.ShipModuleSlot>> list3 = design.hullTemplate.ValidBigWeaponSlotSets(Mount.TwoHullVert);
				List<List<TIShipHullTemplate.ShipModuleSlot>> list4 = design.hullTemplate.ValidBigWeaponSlotSets(Mount.TwoHullHoriz);
				List<TIShipHullTemplate.ShipModuleSlot> allSlotsOfType = design.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.HullHardPoint);
				if (num <= 4 || list.Count == 0 || flag)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.FourHull).ToList<TIShipWeaponTemplate>();
				}
				if (num <= 3 || list2.Count == 0 || flag)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.ThreeHullHoriz).ToList<TIShipWeaponTemplate>();
				}
				if (list3.Count == 0 || flag)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.TwoHullVert).ToList<TIShipWeaponTemplate>();
				}
				if (list4.Count == 0 || flag)
				{
					choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount != Mount.TwoHullHoriz).ToList<TIShipWeaponTemplate>();
				}
				if (choices.Any<TIShipWeaponTemplate>())
				{
					IEnumerable<TIShipWeaponTemplate> enumerable = choices;
					if (choices.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode))
					{
						enumerable = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode);
					}
					TIShipWeaponTemplate tishipWeaponTemplate2 = this.ChooseWeapon(enumerable, scores, playerAutoDesign);
					int num2 = 0;
					switch (tishipWeaponTemplate2.mount)
					{
					case Mount.OneHull:
						num2 = design.hullTemplate.slotIndex(allSlotsOfType[0]);
						break;
					case Mount.TwoHullHoriz:
						num2 = design.hullTemplate.slotIndex(list4[0][0]);
						break;
					case Mount.TwoHullVert:
						num2 = design.hullTemplate.slotIndex(list3[0][0]);
						break;
					case Mount.ThreeHullHoriz:
						num2 = design.hullTemplate.slotIndex(list2[0][0]);
						break;
					case Mount.FourHull:
						num2 = design.hullTemplate.slotIndex(list[0][0]);
						break;
					}
					num -= tishipWeaponTemplate2.internalSize;
					design.hullWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(tishipWeaponTemplate2, num2));
					if (this.IsAlienFaction && !TISpaceShipTemplate.longRangeCombatant(role))
					{
						choices = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => !x.isMissileWeapon);
					}
				}
				List<TIShipWeaponTemplate> list5 = choices.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.mount == Mount.OneHull).ToList<TIShipWeaponTemplate>();
				List<TIShipWeaponTemplate> list6 = new List<TIShipWeaponTemplate>();
				if (role == ShipRole.LM_Protector)
				{
					if (list5.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.guardianMode))
					{
						list5 = list5.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.guardianMode).ToList<TIShipWeaponTemplate>();
						if (design.hullTemplate.hullHardpoints <= 5)
						{
							if (list5.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => !x.isMissileWeapon))
							{
								list5.RemoveAll((TIShipWeaponTemplate x) => x.isMissileWeapon);
							}
						}
						list6 = (from x in list5
							orderby x.isProjectileWeapon, x.isParticleWeapon
							select x).ToList<TIShipWeaponTemplate>();
						goto IL_058F;
					}
				}
				list6 = (from x in list5
					where x.defenseMode && !x.attackMode
					orderby x.isProjectileWeapon, x.isParticleWeapon
					select x).ToList<TIShipWeaponTemplate>();
				IL_058F:
				List<TIShipWeaponTemplate> list7 = list5.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode).ToList<TIShipWeaponTemplate>();
				bool flag2 = TISpaceShipTemplate.longRangeCombatant(role) && design.hullTemplate.hullHardpoints <= 2 && !TISpaceShipTemplate.SoloOperator(role);
				bool flag3 = TISpaceShipTemplate.longRangeCombatant(role) && design.hullTemplate.hullHardpoints <= 8;
				bool flag4 = TISpaceShipTemplate.longRangeCombatant(role);
				bool flag5 = false;
				if (num > 0 && list5.Count > 0)
				{
					foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in allSlotsOfType)
					{
						int num3 = design.hullTemplate.slotIndex(shipModuleSlot);
						if (!design.SlotIndexOccupied(num3, true))
						{
							TIShipWeaponTemplate tishipWeaponTemplate3 = list5.FirstOrDefault<TIShipWeaponTemplate>();
							if (flag2 && flag3 && flag4 && list7.Count > 0)
							{
								tishipWeaponTemplate3 = this.ChooseWeapon(list7, scores, playerAutoDesign);
								flag5 = true;
							}
							else if (list6.Count > 0)
							{
								if (!flag2)
								{
									tishipWeaponTemplate3 = list6.FirstOrDefault<TIShipWeaponTemplate>();
									flag2 = true;
								}
								else if (!flag3)
								{
									if (design.hullTemplate.hullHardpoints >= 4)
									{
										tishipWeaponTemplate3 = list5.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.defenseMode && !x.attackMode && x.isParticleWeapon).FirstOrDefault<TIShipWeaponTemplate>();
										if (tishipWeaponTemplate3 == null)
										{
											tishipWeaponTemplate3 = list6.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isParticleWeapon).FirstOrDefault<TIShipWeaponTemplate>();
										}
										if (tishipWeaponTemplate3 == null)
										{
											tishipWeaponTemplate3 = list6.FirstOrDefault<TIShipWeaponTemplate>();
										}
									}
									flag3 = true;
								}
								else if (!flag4)
								{
									if (design.hullTemplate.hullHardpoints >= 6)
									{
										tishipWeaponTemplate3 = list5.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.defenseMode && x.isGunTypeWeapon).FirstOrDefault<TIShipWeaponTemplate>();
										if (tishipWeaponTemplate3 == null)
										{
											tishipWeaponTemplate3 = list6.FirstOrDefault<TIShipWeaponTemplate>();
										}
									}
									flag4 = true;
								}
								else if (flag5 && flag2 && flag3 && flag4 && design.hullTemplate.hullHardpoints > 8 && !TISpaceShipTemplate.longRangeCombatant(role))
								{
									flag2 = false;
									flag3 = false;
									flag4 = false;
								}
							}
							design.hullWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(tishipWeaponTemplate3, num3));
							num--;
							if (num <= 0)
							{
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06003402 RID: 13314 RVA: 0x00120E70 File Offset: 0x0011F070
		public TIPowerPlantTemplate GetBestPowerPlant(TISpaceShipTemplate design, bool allowExotics, bool allowAntimatter, IEnumerable<TIPowerPlantTemplate> choices = null)
		{
			if (choices == null)
			{
				choices = this.allowedPowerPlants;
			}
			choices = from x in choices.Intersect<TIPowerPlantTemplate>(design.validPowerPlantsForDrive)
				where allowExotics || x.buildCost(1f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f
				where allowAntimatter || x.buildCost(1f, 0f).GetSingleCostValue(FactionResource.Antimatter) == 0f
				select x;
			choices = this.GetObsoleteFilteredParts<TIPowerPlantTemplate>(choices).ToList<TIPowerPlantTemplate>();
			return choices.MaxBy<TIPowerPlantTemplate, float>((TIPowerPlantTemplate x) => x.AIScoringValueForResearch());
		}

		// Token: 0x06003403 RID: 13315 RVA: 0x00120F04 File Offset: 0x0011F104
		public TIHeatSinkTemplate GetBestHeatSink(bool allowExotics)
		{
			IEnumerable<TIHeatSinkTemplate> enumerable = this.allowedHeatSinks.Where<TIHeatSinkTemplate>((TIHeatSinkTemplate x) => allowExotics || x.buildCost(0f, 0f).GetSingleCostValue(FactionResource.Exotics) == 0f);
			enumerable = this.GetObsoleteFilteredParts<TIHeatSinkTemplate>(enumerable).ToList<TIHeatSinkTemplate>();
			return enumerable.MaxBy<TIHeatSinkTemplate, float>((TIHeatSinkTemplate x) => x.AIScoringValueForResearch());
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x00120F68 File Offset: 0x0011F168
		public TIUtilityModuleTemplate GetBestAssaultModule(List<TIUtilityModuleTemplate> allowedModules)
		{
			IEnumerable<TIUtilityModuleTemplate> enumerable = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.Assault));
			if (enumerable.Any<TIUtilityModuleTemplate>())
			{
				return enumerable.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.marineOpsValue);
			}
			return null;
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x00120FCC File Offset: 0x0011F1CC
		public TIUtilityModuleTemplate GetBestECMModule(List<TIUtilityModuleTemplate> allowedModules)
		{
			IEnumerable<TIUtilityModuleTemplate> enumerable = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.ECM));
			if (enumerable.Any<TIUtilityModuleTemplate>())
			{
				return enumerable.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.ECMValue);
			}
			return null;
		}

		// Token: 0x06003406 RID: 13318 RVA: 0x00121030 File Offset: 0x0011F230
		public TIShipModuleTemplate GetNextBestUtilityModule(TISpaceShipTemplate design, ref List<TIUtilityModuleTemplate> allowedModules, bool allowExotics, ref List<int> skipList, List<TIShipModuleTemplate> selectedModules)
		{
			List<int> list = new List<int>();
			int num;
			switch (design.role)
			{
			case ShipRole.TroopCarrier:
				list = new List<int>
				{
					0, 9, 3, 8, 4, 1, 10, 11, 6, 7,
					12, 5
				};
				num = design.hullTemplate.internalModules;
				goto IL_0838;
			case ShipRole.ArmyCarrier:
			case ShipRole.CouncilorTransport:
				list = new List<int>
				{
					0, 9, 3, 4, 1, 10, 11, 12, 8, 6,
					7, 5
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.Explorer:
				list = new List<int>
				{
					0, 11, 10, 4, 3, 9, 1, 12, 5, 2,
					6, 7, 8
				};
				num = 0;
				goto IL_0838;
			case ShipRole.InnerSystemColonyShip:
			case ShipRole.OuterSystemColonyShip:
				list = new List<int>
				{
					0, 4, 10, 11, 9, 3, 1, 6, 12, 13,
					5, 2, 8, 7
				};
				num = 0;
				goto IL_0838;
			case ShipRole.LS_Penetrator:
				list = new List<int>
				{
					0, 12, 1, 2, 9, 3, 7, 5, 4, 14,
					10, 11, 6, 8
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.LM_Protector:
				list = new List<int>
				{
					2, 9, 12, 3, 0, 7, 1, 5, 6, 8,
					4, 14, 10, 11
				};
				num = 1;
				goto IL_0838;
			case ShipRole.LM_Interdictor:
				list = new List<int>
				{
					12, 0, 9, 3, 1, 4, 7, 14, 10, 5,
					2, 8, 11, 6
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.LL_Intruder:
				list = new List<int>
				{
					5, 12, 0, 9, 3, 4, 14, 10, 1, 11,
					6, 7, 8, 2
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.LL_Bomber:
				list = new List<int>
				{
					2, 3, 4, 5, 9, 0, 7, 1, 12, 6,
					14, 10, 11
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.MS_Strike:
				list = new List<int>
				{
					1, 12, 0, 2, 9, 3, 6, 13, 7, 5,
					10, 4, 8, 11
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.ML_Standoff:
				list = new List<int>
				{
					5, 12, 0, 9, 1, 3, 4, 13, 6, 10,
					7, 8, 11, 2
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.SS_Interceptor:
				list = new List<int>
				{
					1, 12, 0, 2, 9, 3, 7, 6, 8, 4,
					5, 10, 11
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.SM_Patrol:
				list = new List<int>
				{
					12, 1, 2, 3, 9, 0, 6, 7, 8, 4,
					5, 10, 11
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			case ShipRole.SL_Defender:
				list = new List<int>
				{
					5, 12, 9, 1, 3, 0, 6, 7, 8, 6,
					10, 11, 2
				};
				num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
				goto IL_0838;
			}
			list = new List<int>
			{
				12, 9, 0, 1, 3, 2, 7, 4, 13, 6,
				10, 7, 8, 11
			};
			num = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
			IL_0838:
			list = list.Except<int>(skipList).ToList<int>();
			for (int i = 0; i < list.Count; i++)
			{
				switch (list[i])
				{
				case 0:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.EVMultiplier > 1f);
					if (enumerable.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate = enumerable.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.EVMultiplier);
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate;
					}
					break;
				}
				case 1:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable2 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.thrustMultiplier > 1f);
					if (enumerable2.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate2 = enumerable2.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.thrustMultiplier);
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable2).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate2;
					}
					break;
				}
				case 2:
				{
					int num2 = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
					IEnumerable<TIUtilityModuleTemplate> enumerable3 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.laserPowerBonus_MW > 0f);
					if (enumerable3.Any<TIUtilityModuleTemplate>())
					{
						if (!design.noseWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isLaserWeapon))
						{
							if (!design.hullWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isLaserWeapon && x.internalSize >= 2))
							{
								goto IL_0AA9;
							}
						}
						if (selectedModules.Count<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
						{
							TIUtilityModuleTemplate ref_utilityModule2 = x.ref_utilityModule;
							return ref_utilityModule2 != null && ref_utilityModule2.laserPowerBonus_MW > 0f;
						}) < num2)
						{
							TIShipModuleTemplate tishipModuleTemplate3 = enumerable3.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.laserPowerBonus_MW);
							allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable3).ToList<TIUtilityModuleTemplate>();
							return tishipModuleTemplate3;
						}
					}
					IL_0AA9:
					skipList.Add(list[i]);
					break;
				}
				case 3:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable4 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.ComponentArmor));
					if (enumerable4.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate4 = enumerable4.First<TIUtilityModuleTemplate>();
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable4).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate4;
					}
					break;
				}
				case 4:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable5 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.Repair));
					if (enumerable5.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate5 = enumerable5.First<TIUtilityModuleTemplate>();
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable5).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate5;
					}
					break;
				}
				case 5:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable6 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.Magazine));
					int[] array = new int[3];
					array[0] = 1;
					array[1] = (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f));
					array[2] = design.allWeaponTemplates.Count<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.hasMagazine());
					Mathf.Max(array);
					if (enumerable6.Any<TIUtilityModuleTemplate>())
					{
						if (this.IsAlienFaction)
						{
							if (design.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.hasMagazine()))
							{
								goto IL_0C72;
							}
						}
						if (!design.allWeaponTemplates.Any<TIShipWeaponTemplate>(delegate(TIShipWeaponTemplate x)
						{
							TIProjectileWeaponTemplate ref_projectileWeapon = x.ref_projectileWeapon;
							return ref_projectileWeapon != null && ref_projectileWeapon.magazine < 100;
						}))
						{
							break;
						}
						IL_0C72:
						TIShipModuleTemplate tishipModuleTemplate6 = enumerable6.First<TIUtilityModuleTemplate>();
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable6).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate6;
					}
					break;
				}
				case 6:
				{
					int num3 = (design.driveTemplate.openCycleCooling ? 1 : Mathf.Max(1, design.hullTemplate.consTier - 1));
					if (selectedModules.Count<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.isHeatSink) < num3)
					{
						TIHeatSinkTemplate bestHeatSink = this.GetBestHeatSink(allowExotics);
						if (bestHeatSink != null)
						{
							return bestHeatSink;
						}
					}
					break;
				}
				case 7:
				{
					int num4 = 1;
					if (selectedModules.Count<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.isBattery) < num4)
					{
						TIUtilityModuleTemplate ref_utilityModule = this.GetBestBattery(design, allowExotics).ref_utilityModule;
						if (ref_utilityModule != null)
						{
							return ref_utilityModule;
						}
					}
					else
					{
						skipList.Add(list[i]);
					}
					break;
				}
				case 8:
					if (selectedModules.Where<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.ref_utilityModule != null).Count<TIShipModuleTemplate>((TIShipModuleTemplate x) => x.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Assault)) < num)
					{
						TIUtilityModuleTemplate bestAssaultModule = this.GetBestAssaultModule(allowedModules);
						if (bestAssaultModule != null)
						{
							return bestAssaultModule;
						}
						skipList.Add(list[i]);
					}
					else
					{
						skipList.Add(list[i]);
					}
					break;
				case 9:
				{
					skipList.Add(list[i]);
					TIUtilityModuleTemplate bestECMModule = this.GetBestECMModule(allowedModules);
					if (bestECMModule != null)
					{
						return bestECMModule;
					}
					break;
				}
				case 10:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable7 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.ImmunetoAerobrakingDamage) && !x.specialModuleRules.Contains(SpecialModuleRule.Crashdown) && !x.specialModuleRules.Contains(SpecialModuleRule.LandArmy));
					if (enumerable7.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate7 = enumerable7.First<TIUtilityModuleTemplate>();
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable7).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate7;
					}
					break;
				}
				case 11:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable8 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.RefuelFromUnimprovedSites));
					if (enumerable8.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate8 = enumerable8.First<TIUtilityModuleTemplate>();
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable8).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate8;
					}
					break;
				}
				case 12:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable9 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.TargetingComputer));
					if (enumerable9.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate9 = enumerable9.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.specialModuleValue);
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable9).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate9;
					}
					break;
				}
				case 13:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable10 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.SalvageBonus));
					if (enumerable10.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate10 = enumerable10.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.specialModuleValue);
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable10).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate10;
					}
					break;
				}
				case 14:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable11 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.ReduceFleetMCConsumption));
					if (enumerable11.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate11 = enumerable11.MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.specialModuleValue);
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable11).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate11;
					}
					break;
				}
				case 15:
				{
					int num5 = Mathf.Max(1, (int)Math.Truncate((double)((float)design.hullTemplate.internalModules / 2f)));
					IEnumerable<TIUtilityModuleTemplate> enumerable12 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.particleBeamPowerBonus_MW > 0f);
					if (enumerable12.Any<TIUtilityModuleTemplate>())
					{
						if (!design.noseWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isParticleWeapon))
						{
							if (!design.hullWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isParticleWeapon && x.internalSize >= 2))
							{
								goto IL_1124;
							}
						}
						if (selectedModules.Count<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
						{
							TIUtilityModuleTemplate ref_utilityModule3 = x.ref_utilityModule;
							return ref_utilityModule3 != null && ref_utilityModule3.particleBeamPowerBonus_MW > 0f;
						}) < num5)
						{
							TIShipModuleTemplate tishipModuleTemplate12 = enumerable12.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.particleBeamPowerBonus_MW);
							allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable12).ToList<TIUtilityModuleTemplate>();
							return tishipModuleTemplate12;
						}
					}
					IL_1124:
					skipList.Add(list[i]);
					break;
				}
				case 16:
				{
					skipList.Add(list[i]);
					IEnumerable<TIUtilityModuleTemplate> enumerable13 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.vectorThrustBonus > 0f);
					if (enumerable13.Any<TIUtilityModuleTemplate>())
					{
						TIShipModuleTemplate tishipModuleTemplate13 = enumerable13.MaxBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.vectorThrustBonus);
						allowedModules = allowedModules.Except<TIUtilityModuleTemplate>(enumerable13).ToList<TIUtilityModuleTemplate>();
						return tishipModuleTemplate13;
					}
					break;
				}
				}
			}
			return null;
		}

		// Token: 0x06003407 RID: 13319 RVA: 0x001221FC File Offset: 0x001203FC
		private void AddUtilityModuleToDesign(ref List<TIShipModuleTemplate> modules, ref List<TIUtilityModuleTemplate> allowedModules, TIShipModuleTemplate module)
		{
			modules.Add(module);
			if (module.isUtilityModule && module.ref_utilityModule.grouping != -1)
			{
				allowedModules.RemoveAll((TIUtilityModuleTemplate x) => x.grouping == module.ref_utilityModule.grouping);
			}
		}

		// Token: 0x06003408 RID: 13320 RVA: 0x00122258 File Offset: 0x00120458
		protected List<ModuleDataTemplateEntry> GetBestUtilityModules(TISpaceShipTemplate design, bool allowExotics, bool allowAntimatter, IEnumerable<IEnumerable<SpecialModuleRule>> forcedSpecialModuleRules = null, List<ModuleDataTemplateEntry> cachedBestUtilityModules = null)
		{
			if (forcedSpecialModuleRules == null)
			{
				forcedSpecialModuleRules = Enumerable.Empty<IEnumerable<SpecialModuleRule>>();
			}
			List<TIShipModuleTemplate> list = new List<TIShipModuleTemplate>();
			List<TIUtilityModuleTemplate> allowedModules = this.allowedUtilityModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => design.ValidPartForDesign(x)).ToList<TIUtilityModuleTemplate>();
			if (!allowExotics)
			{
				allowedModules = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.weightedBuildMaterials.exotics <= 0f).ToList<TIUtilityModuleTemplate>();
			}
			if (!allowAntimatter)
			{
				allowedModules = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.weightedBuildMaterials.antimatter <= 0f).ToList<TIUtilityModuleTemplate>();
			}
			allowedModules = this.GetObsoleteFilteredParts<TIUtilityModuleTemplate>(allowedModules).ToList<TIUtilityModuleTemplate>();
			if (cachedBestUtilityModules != null)
			{
				List<TIHeatSinkTemplate> list2 = this.GetObsoleteFilteredParts<TIHeatSinkTemplate>(this.allowedHeatSinks.Where<TIHeatSinkTemplate>((TIHeatSinkTemplate x) => (allowExotics || x.weightedBuildMaterials.exotics <= 0f) && (allowAntimatter || x.weightedBuildMaterials.antimatter <= 0f))).ToList<TIHeatSinkTemplate>();
				IEnumerable<TIShipPartTemplate> allowedUtilitySlotModules = allowedModules.Select<TIUtilityModuleTemplate, TIShipPartTemplate>((TIUtilityModuleTemplate x) => x).Union<TIShipPartTemplate>(list2);
				if (cachedBestUtilityModules.Distinct<ModuleDataTemplateEntry>().All<ModuleDataTemplateEntry>((ModuleDataTemplateEntry x) => allowedUtilitySlotModules.Any<TIShipPartTemplate>((TIShipPartTemplate y) => y.dataName == x.moduleName)))
				{
					return cachedBestUtilityModules;
				}
			}
			int num = design.hullTemplate.internalModules;
			Func<SpecialModuleRule, bool> <>9__7;
			foreach (IEnumerable<SpecialModuleRule> enumerable in forcedSpecialModuleRules)
			{
				if (num <= 0)
				{
					break;
				}
				IEnumerable<SpecialModuleRule> enumerable2 = enumerable;
				Func<SpecialModuleRule, bool> func;
				if ((func = <>9__7) == null)
				{
					func = (<>9__7 = (SpecialModuleRule x) => allowedModules.Any<TIUtilityModuleTemplate>((TIUtilityModuleTemplate y) => y.specialModuleRules.Contains(x)));
				}
				IEnumerable<SpecialModuleRule> enumerable3 = enumerable2.Where<SpecialModuleRule>(func);
				if (enumerable3.Any<SpecialModuleRule>())
				{
					SpecialModuleRule selectedRule = enumerable3.First<SpecialModuleRule>();
					IEnumerable<TIUtilityModuleTemplate> enumerable4 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(selectedRule));
					this.AddUtilityModuleToDesign(ref list, ref allowedModules, enumerable4.MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons));
					num--;
				}
			}
			bool flag = false;
			List<int> list3 = new List<int>();
			int i = 0;
			Func<TIUtilityModuleTemplate, bool> <>9__35;
			while (i < num)
			{
				switch (design.role)
				{
				case ShipRole.TroopCarrier:
					if (i != 0)
					{
						if (i != 1)
						{
							if (i % 2 != 0)
							{
								goto IL_0B4C;
							}
							TIUtilityModuleTemplate bestAssaultModule = this.GetBestAssaultModule(allowedModules);
							if (bestAssaultModule != null)
							{
								this.AddUtilityModuleToDesign(ref list, ref allowedModules, bestAssaultModule);
							}
						}
						else
						{
							TIHeatSinkTemplate bestHeatSink = this.GetBestHeatSink(false);
							if (bestHeatSink == null)
							{
								goto IL_0B4C;
							}
							list.Add(bestHeatSink);
							flag = true;
						}
					}
					else
					{
						TIUtilityModuleTemplate bestAssaultModule2 = this.GetBestAssaultModule(allowedModules);
						if (bestAssaultModule2 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, bestAssaultModule2);
					}
					break;
				case ShipRole.ArmyCarrier:
				{
					if (i != 0)
					{
						goto IL_0B4C;
					}
					TIUtilityModuleTemplate tiutilityModuleTemplate = null;
					if (this.IsAlienFaction && design.hullTemplate.dataName == "AlienAssaultCarrier")
					{
						tiutilityModuleTemplate = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.LandArmy)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
					}
					if (tiutilityModuleTemplate == null)
					{
						goto IL_0B4C;
					}
					this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate);
					break;
				}
				case ShipRole.Explorer:
					switch (i)
					{
					case 0:
					{
						TIUtilityModuleTemplate tiutilityModuleTemplate2 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.Prospector)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate2 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate2);
						break;
					}
					case 1:
					{
						TIUtilityModuleTemplate tiutilityModuleTemplate3 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.RefuelFromUnimprovedSites)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate3 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate3);
						break;
					}
					case 2:
					{
						TIUtilityModuleTemplate tiutilityModuleTemplate4 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.RefuelFromAtmospheres)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate4 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate4);
						break;
					}
					default:
						goto IL_0B4C;
					}
					break;
				case ShipRole.InnerSystemColonyShip:
					if (!this.IsAlienFaction)
					{
						TIUtilityModuleTemplate tiutilityModuleTemplate5 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundSolarPlatform)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						TIUtilityModuleTemplate tiutilityModuleTemplate6 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundSolarOutpost)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate5 == null)
						{
							tiutilityModuleTemplate5 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFusionPlatform)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
							if (tiutilityModuleTemplate5 == null)
							{
								tiutilityModuleTemplate5 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFissionPlatform)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
							}
						}
						if (tiutilityModuleTemplate6 == null)
						{
							tiutilityModuleTemplate6 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFusionOutpost)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
							if (tiutilityModuleTemplate6 == null)
							{
								tiutilityModuleTemplate6 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFissionOutpost)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
							}
						}
						if (i % 2 == 0 && tiutilityModuleTemplate6 != null)
						{
							this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate6);
						}
						else
						{
							if (tiutilityModuleTemplate5 == null)
							{
								goto IL_0B4C;
							}
							this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate5);
						}
					}
					else
					{
						IEnumerable<TIUtilityModuleTemplate> allowedModules2 = allowedModules;
						Func<TIUtilityModuleTemplate, bool> func2;
						if ((func2 = <>9__35) == null)
						{
							func2 = (<>9__35 = (TIUtilityModuleTemplate x) => (x.specialModuleRules.Contains(SpecialModuleRule.FoundSurveillancePlatform) || x.specialModuleRules.Contains(SpecialModuleRule.FoundSurveillanceOrbital) || x.specialModuleRules.Contains(SpecialModuleRule.FoundSurveillanceRing)) && x.minConsTier == design.hullTemplate.consTier);
						}
						TIUtilityModuleTemplate tiutilityModuleTemplate7 = allowedModules2.Where<TIUtilityModuleTemplate>(func2).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate7 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate7);
					}
					break;
				case ShipRole.OuterSystemColonyShip:
				{
					TIUtilityModuleTemplate tiutilityModuleTemplate8 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFusionPlatform)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
					TIUtilityModuleTemplate tiutilityModuleTemplate9 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFusionOutpost)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
					if (tiutilityModuleTemplate8 == null)
					{
						tiutilityModuleTemplate8 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFissionPlatform)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate8 == null)
						{
							tiutilityModuleTemplate8 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundSolarPlatform)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						}
					}
					if (tiutilityModuleTemplate9 == null)
					{
						tiutilityModuleTemplate9 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundFissionOutpost)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate9 == null)
						{
							tiutilityModuleTemplate9 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.FoundSolarOutpost)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						}
					}
					if (i % 2 == 0 && tiutilityModuleTemplate9 != null)
					{
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate9);
					}
					else
					{
						if (tiutilityModuleTemplate8 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate8);
					}
					break;
				}
				case ShipRole.EarthSurveillance:
				{
					if (i != 0)
					{
						goto IL_0B4C;
					}
					TIUtilityModuleTemplate tiutilityModuleTemplate10 = null;
					if (this.IsAlienFaction)
					{
						tiutilityModuleTemplate10 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.Surveillance)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
					}
					if (tiutilityModuleTemplate10 == null)
					{
						goto IL_0B4C;
					}
					this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate10);
					break;
				}
				case ShipRole.CouncilorTransport:
					if (!this.IsAlienFaction)
					{
						goto IL_0B4C;
					}
					if (i != 0)
					{
						if (i != 1)
						{
							goto IL_0B4C;
						}
						TIUtilityModuleTemplate tiutilityModuleTemplate11 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.Salamanders)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate11 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate11);
					}
					else
					{
						TIUtilityModuleTemplate tiutilityModuleTemplate12 = allowedModules.Where<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleRules.Contains(SpecialModuleRule.LandHydra)).MinBy<TIUtilityModuleTemplate, float>((TIUtilityModuleTemplate x) => x.mass_tons);
						if (tiutilityModuleTemplate12 == null)
						{
							goto IL_0B4C;
						}
						this.AddUtilityModuleToDesign(ref list, ref allowedModules, tiutilityModuleTemplate12);
					}
					break;
				default:
					goto IL_0B4C;
				}
				IL_0BB5:
				i++;
				continue;
				IL_0B4C:
				if (!flag || (i < 6 && design.hullName == "AlienMothership"))
				{
					TIHeatSinkTemplate bestHeatSink2 = this.GetBestHeatSink(false);
					if (bestHeatSink2 != null)
					{
						list.Add(bestHeatSink2);
						flag = true;
						goto IL_0BB5;
					}
				}
				TIShipModuleTemplate nextBestUtilityModule = this.GetNextBestUtilityModule(design, ref allowedModules, allowExotics, ref list3, list);
				if (nextBestUtilityModule != null)
				{
					this.AddUtilityModuleToDesign(ref list, ref allowedModules, nextBestUtilityModule);
					goto IL_0BB5;
				}
				break;
			}
			List<ModuleDataTemplateEntry> list4 = new List<ModuleDataTemplateEntry>();
			List<TIShipHullTemplate.ShipModuleSlot> allSlotsOfType = design.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.Utility);
			for (int j = 0; j < list.Count; j++)
			{
				if (j < design.hullTemplate.internalModules)
				{
					int num2 = design.hullTemplate.slotIndex(allSlotsOfType[j]);
					list4.Add(new ModuleDataTemplateEntry(list[j], num2));
				}
			}
			return list4;
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x00122EB0 File Offset: 0x001210B0
		public IEnumerable<TIDriveTemplate> GetDriveCatalogue(ShipRole role, TIShipHullTemplate hull, float randomness = 0f)
		{
			IEnumerable<TIDriveTemplate> obsoleteFilteredParts = this.GetObsoleteFilteredParts<TIDriveTemplate>(this.allowedDrives);
			ValueTuple<ShipRole, TIShipHullTemplate> valueTuple = new ValueTuple<ShipRole, TIShipHullTemplate>(role, hull);
			Dictionary<TIDriveTemplate, ValueTuple<float, float>> cachedDriveDVs;
			if (!this.shipDesigner_CachedDriveStats.TryGetValue(valueTuple, out cachedDriveDVs) || cachedDriveDVs.Count == 0)
			{
				return obsoleteFilteredParts;
			}
			float minimumDV = cachedDriveDVs.Values.Max<ValueTuple<float, float>>(([TupleElementNames(new string[] { "Acceleration_gs", "DV_kps" })] ValueTuple<float, float> x) => x.Item2) * 0.15f;
			return obsoleteFilteredParts.Except<TIDriveTemplate>(cachedDriveDVs.Keys.Where<TIDriveTemplate>((TIDriveTemplate x) => cachedDriveDVs[x].Item2 < minimumDV)).ToList<TIDriveTemplate>();
		}

		// Token: 0x0600340A RID: 13322 RVA: 0x00122F60 File Offset: 0x00121160
		private IEnumerable<T> GetObsoleteFilteredParts<T>(IEnumerable<T> parts) where T : TIShipPartTemplate
		{
			IEnumerable<T> enumerable = parts.Where<T>((T x) => !this.obsoletedShipParts.Contains(x.dataName));
			if (!enumerable.Any<T>())
			{
				enumerable = parts;
			}
			return enumerable;
		}

		// Token: 0x0600340B RID: 13323 RVA: 0x00122F8C File Offset: 0x0012118C
		public TIFactionState.ShipDesignerOutcome DesignShip(bool playerAutodesign, ShipRole role, out TISpaceShipTemplate design, float desiredStrategicRange_AU, bool allowExotics = false, bool allowAntimatter = false, TIShipHullTemplate forceHull = null, IEnumerable<IEnumerable<SpecialModuleRule>> forcedSpecialModuleRules = null, bool heavy = false, TIOrbitState exampleOrigin = null, TIOrbitState exampleDestination = null, float desiredMaxTransferDuration = float.PositiveInfinity, float hardMaxTransferDuration = float.PositiveInfinity)
		{
			TIFactionState.<>c__DisplayClass1100_0 CS$<>8__locals1 = new TIFactionState.<>c__DisplayClass1100_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.role = role;
			CS$<>8__locals1.allowExotics = allowExotics;
			CS$<>8__locals1.allowAntimatter = allowAntimatter;
			CS$<>8__locals1.playerAutodesign = playerAutodesign;
			CS$<>8__locals1.forcedSpecialModuleRules = forcedSpecialModuleRules;
			CS$<>8__locals1.exampleOrigin = exampleOrigin;
			CS$<>8__locals1.exampleDestination = exampleDestination;
			CS$<>8__locals1.desiredMaxTransferDuration = desiredMaxTransferDuration;
			CS$<>8__locals1.hardMaxTransferDuration = hardMaxTransferDuration;
			if (this.IsAlienFaction)
			{
				return this.DesignAlienShip(CS$<>8__locals1.role, out design, desiredStrategicRange_AU, CS$<>8__locals1.allowExotics, CS$<>8__locals1.allowAntimatter, forceHull, heavy, 250);
			}
			CS$<>8__locals1.colonyRole = CS$<>8__locals1.role == ShipRole.InnerSystemColonyShip || CS$<>8__locals1.role == ShipRole.OuterSystemColonyShip;
			CS$<>8__locals1.armor = this.GetBestArmor(CS$<>8__locals1.allowExotics);
			List<TIDriveTemplate> list = this.allowedDrives.Where<TIDriveTemplate>((TIDriveTemplate x) => x.thrust_N / (float)x.thrusters >= 100000f && x.EV_kps >= 8f).ToList<TIDriveTemplate>();
			List<TIShipHullTemplate> list2 = this.allowedShipHulls.ToList<TIShipHullTemplate>();
			design = null;
			if (list2.Count == 0)
			{
				return TIFactionState.ShipDesignerOutcome.NoAvailableHulls;
			}
			CS$<>8__locals1.hull = null;
			if (forceHull == null)
			{
				TIFactionState.<>c__DisplayClass1100_1 CS$<>8__locals2 = new TIFactionState.<>c__DisplayClass1100_1();
				CS$<>8__locals2.minSlots = 0;
				CS$<>8__locals2.minHullSlots = 0;
				CS$<>8__locals2.minNoseSlots = 0;
				switch (CS$<>8__locals1.role)
				{
				case ShipRole.TroopCarrier:
				{
					TIFactionState.<>c__DisplayClass1100_1 CS$<>8__locals3 = CS$<>8__locals2;
					int num;
					if (!heavy)
					{
						num = list2.Where<TIShipHullTemplate>((TIShipHullTemplate x) => x.internalModules <= 4).MaxBy<TIShipHullTemplate, int>((TIShipHullTemplate x) => x.internalModules).internalModules;
					}
					else
					{
						num = list2.MaxBy<TIShipHullTemplate, int>((TIShipHullTemplate x) => x.internalModules).internalModules;
					}
					CS$<>8__locals3.minSlots = num;
					break;
				}
				case ShipRole.Explorer:
					CS$<>8__locals2.minSlots = 3;
					break;
				case ShipRole.InnerSystemColonyShip:
				case ShipRole.OuterSystemColonyShip:
					CS$<>8__locals2.minSlots = 1;
					break;
				case ShipRole.CouncilorTransport:
					CS$<>8__locals2.minSlots = 1;
					break;
				case ShipRole.LS_Penetrator:
					CS$<>8__locals2.minSlots = 3;
					break;
				case ShipRole.LM_Protector:
					CS$<>8__locals2.minSlots = 1;
					CS$<>8__locals2.minHullSlots = 4;
					break;
				case ShipRole.LM_Interdictor:
					CS$<>8__locals2.minSlots = 1;
					CS$<>8__locals2.minHullSlots = 1;
					break;
				case ShipRole.LL_Intruder:
				case ShipRole.LL_Bomber:
					CS$<>8__locals2.minSlots = 3;
					CS$<>8__locals2.minNoseSlots = 2;
					CS$<>8__locals2.minHullSlots = 2;
					break;
				case ShipRole.MS_Strike:
					CS$<>8__locals2.minSlots = 1;
					break;
				case ShipRole.MM_SpaceSuperiority:
					CS$<>8__locals2.minSlots = 4;
					CS$<>8__locals2.minHullSlots = 1;
					break;
				case ShipRole.ML_Standoff:
					CS$<>8__locals2.minSlots = 3;
					CS$<>8__locals2.minHullSlots = 2;
					break;
				case ShipRole.SS_Interceptor:
					CS$<>8__locals2.minSlots = 1;
					break;
				case ShipRole.SM_Patrol:
					CS$<>8__locals2.minSlots = 2;
					CS$<>8__locals2.minHullSlots = 1;
					break;
				case ShipRole.SL_Defender:
					CS$<>8__locals2.minSlots = 3;
					CS$<>8__locals2.minHullSlots = 2;
					break;
				}
				if (heavy)
				{
					int i;
					int k;
					for (k = CS$<>8__locals2.minSlots + 4; k > CS$<>8__locals2.minSlots; k = i - 1)
					{
						list2 = list2.Where<TIShipHullTemplate>((TIShipHullTemplate x) => x.internalModules >= k && x.hullHardpoints >= CS$<>8__locals2.minHullSlots && x.noseHardpoints >= CS$<>8__locals2.minNoseSlots).ToList<TIShipHullTemplate>();
						if (list2.Count > 0)
						{
							break;
						}
						i = k;
					}
				}
				else
				{
					list2 = list2.Where<TIShipHullTemplate>((TIShipHullTemplate x) => x.internalModules >= CS$<>8__locals2.minSlots && x.hullHardpoints >= CS$<>8__locals2.minHullSlots && x.noseHardpoints >= CS$<>8__locals2.minNoseSlots).ToList<TIShipHullTemplate>();
				}
				if (list2.Count == 0)
				{
					return TIFactionState.ShipDesignerOutcome.NoHullsForRole;
				}
				list2 = this.GetObsoleteFilteredParts<TIShipHullTemplate>(list2).ToList<TIShipHullTemplate>();
				if (list.Count == 0)
				{
					list2.RemoveAll((TIShipHullTemplate x) => !x.smallHull);
				}
				list2.RemoveAll((TIShipHullTemplate hull) => !hull.smallHull && CS$<>8__locals1.armor.armor_section_volume(1f, hull.length_m, hull.width_m, CS$<>8__locals1.armor.armor_section_thickness_m(1f), true) * CS$<>8__locals1.armor.density_kgm3 / 1000f / hull.mass_tons > 3f);
				if (list2.Count == 0)
				{
					return TIFactionState.ShipDesignerOutcome.NoHullsForRole;
				}
				CS$<>8__locals1.hull = list2.SelectRandomWeightedItem<TIShipHullTemplate>(delegate(TIShipHullTemplate x)
				{
					float num6 = 16f;
					if (CS$<>8__locals1.colonyRole)
					{
						num6 /= Mathf.Pow(x.length_m, 2f);
					}
					num6 += Mathf.Pow((float)x.GetAllSlotsOfType(ShipModuleSlotType.NoseHardPoint).Count, 1.7f) * 2f;
					num6 += Mathf.Pow((float)x.GetAllSlotsOfType(ShipModuleSlotType.HullHardPoint).Count, 1.35f) * 1f;
					return Mathf.Pow(num6, 2f);
				}, -1f, 1E-37f);
			}
			else
			{
				if (!list2.Contains(forceHull))
				{
					return TIFactionState.ShipDesignerOutcome.ForcedHullNotAvailable;
				}
				CS$<>8__locals1.hull = forceHull;
			}
			CS$<>8__locals1.desiredArmor = 3;
			CS$<>8__locals1.desiredCruiseAccel_gs = 0.02f;
			CS$<>8__locals1.desiredDV_kps = 10f;
			switch (CS$<>8__locals1.role)
			{
			case ShipRole.TroopCarrier:
				CS$<>8__locals1.desiredDV_kps = 30f;
				CS$<>8__locals1.desiredArmor = 10;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.005f;
				break;
			case ShipRole.Explorer:
				CS$<>8__locals1.desiredDV_kps = 60f;
				CS$<>8__locals1.desiredArmor = 4;
				break;
			case ShipRole.InnerSystemColonyShip:
				CS$<>8__locals1.desiredDV_kps = 45f;
				CS$<>8__locals1.desiredArmor = 3;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.005f;
				break;
			case ShipRole.OuterSystemColonyShip:
				CS$<>8__locals1.desiredDV_kps = 60f;
				CS$<>8__locals1.desiredArmor = 4;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.005f;
				break;
			case ShipRole.CouncilorTransport:
				CS$<>8__locals1.desiredDV_kps = 45f;
				CS$<>8__locals1.desiredArmor = 10;
				break;
			case ShipRole.LS_Penetrator:
				CS$<>8__locals1.desiredDV_kps = 40f;
				CS$<>8__locals1.desiredArmor = 36;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.02f;
				break;
			case ShipRole.LM_Protector:
			case ShipRole.LM_Interdictor:
				CS$<>8__locals1.desiredDV_kps = 40f;
				CS$<>8__locals1.desiredArmor = 30;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.02f;
				break;
			case ShipRole.LL_Intruder:
				CS$<>8__locals1.desiredDV_kps = 40f;
				CS$<>8__locals1.desiredArmor = 16;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.01f;
				break;
			case ShipRole.LL_Bomber:
				CS$<>8__locals1.desiredDV_kps = 40f;
				CS$<>8__locals1.desiredArmor = 40;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.01f;
				break;
			case ShipRole.MS_Strike:
				CS$<>8__locals1.desiredDV_kps = 20f;
				CS$<>8__locals1.desiredArmor = 36;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.04f;
				break;
			case ShipRole.MM_SpaceSuperiority:
				CS$<>8__locals1.desiredDV_kps = 20f;
				CS$<>8__locals1.desiredArmor = 32;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.03f;
				break;
			case ShipRole.ML_Standoff:
				CS$<>8__locals1.desiredDV_kps = 20f;
				CS$<>8__locals1.desiredArmor = 16;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.02f;
				break;
			case ShipRole.SS_Interceptor:
				CS$<>8__locals1.desiredDV_kps = 8f;
				CS$<>8__locals1.desiredArmor = 36;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.05f;
				break;
			case ShipRole.SM_Patrol:
				CS$<>8__locals1.desiredDV_kps = 8f;
				CS$<>8__locals1.desiredArmor = 32;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.05f;
				break;
			case ShipRole.SL_Defender:
				CS$<>8__locals1.desiredDV_kps = 8f;
				CS$<>8__locals1.desiredArmor = 25;
				CS$<>8__locals1.desiredCruiseAccel_gs = 0.04f;
				break;
			}
			if (TISpaceShipTemplate.shortRangeStrategic(CS$<>8__locals1.role))
			{
				CS$<>8__locals1.desiredDV_kps *= Mathf.Clamp(desiredStrategicRange_AU / 30f, 1f, 5f);
				CS$<>8__locals1.desiredCruiseAccel_gs *= Mathf.Max(1f, desiredStrategicRange_AU / 20f);
			}
			if (TISpaceShipTemplate.mediumRangeStrategic(CS$<>8__locals1.role))
			{
				CS$<>8__locals1.desiredDV_kps *= Mathf.Clamp(desiredStrategicRange_AU / 5f, 1f, 10f);
				CS$<>8__locals1.desiredCruiseAccel_gs *= Mathf.Max(1f, desiredStrategicRange_AU / 10f);
			}
			else if (TISpaceShipTemplate.longRangeStrategic(CS$<>8__locals1.role))
			{
				CS$<>8__locals1.desiredDV_kps *= Mathf.Clamp(desiredStrategicRange_AU, 1f, 15f);
				CS$<>8__locals1.desiredCruiseAccel_gs *= Mathf.Max(1f, desiredStrategicRange_AU / 5f);
			}
			CS$<>8__locals1.designDataName = TemplateManager.GenerateDataName(new StringBuilder(this.templateName).Append("ShipTemplate").ToString());
			CS$<>8__locals1.cachedDriveTypeName = "";
			CS$<>8__locals1.utilityModules = null;
			CS$<>8__locals1.noseWeapons = null;
			CS$<>8__locals1.hullWeapons = null;
			CS$<>8__locals1.designKey = new ValueTuple<ShipRole, TIShipHullTemplate>(CS$<>8__locals1.role, CS$<>8__locals1.hull);
			if (!this.shipDesigner_CachedDriveStats.ContainsKey(CS$<>8__locals1.designKey))
			{
				this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey] = new Dictionary<TIDriveTemplate, ValueTuple<float, float>>();
			}
			CS$<>8__locals1.unfilteredPowerPlants = this.allowedPowerPlants.ToList<TIPowerPlantTemplate>();
			CS$<>8__locals1.filteredPowerPlants = this.GetObsoleteFilteredParts<TIPowerPlantTemplate>(CS$<>8__locals1.unfilteredPowerPlants);
			List<TIDriveTemplate> list3 = this.allowedDrives.ToList<TIDriveTemplate>();
			if (!CS$<>8__locals1.hull.smallHull && (!this.isActivePlayer || list.Count > 0))
			{
				list3 = list;
			}
			if (!this.isActivePlayer && !this.IsAlienFaction && !CS$<>8__locals1.colonyRole)
			{
				list3 = list3.Where<TIDriveTemplate>(delegate(TIDriveTemplate drive)
				{
					TIResourcesCost costEstimate = drive.buildCost(0f, 0f);
					costEstimate.SumCosts_NoDuration(drive.GetPerTankPropellantMaterials(CS$<>8__locals1.<>4__this).ToResourcesCost(20f));
					return Enumerable.Empty<FactionResource>().Append(FactionResource.Fissiles).None<FactionResource>((FactionResource resource) => costEstimate.GetSingleCostValue(resource) / Mathf.Max(0f, CS$<>8__locals1.<>4__this.GetDailyIncome(resource, true, false)) > 180f);
				}).ToList<TIDriveTemplate>();
			}
			if (list3.Count == 0)
			{
				return TIFactionState.ShipDesignerOutcome.NoDrives;
			}
			CS$<>8__locals1.statBasedMinimumDV_kps = CS$<>8__locals1.<DesignShip>g__GetStatBasedMinimumDV_kps|2();
			CS$<>8__locals1.statBasedMinimumAcceleration_gs = CS$<>8__locals1.<DesignShip>g__GetStatBasedMinimumAcceleration_gs|3();
			list3 = list3.Except<TIDriveTemplate>(this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey].Keys.Where<TIDriveTemplate>(delegate(TIDriveTemplate drive)
			{
				ValueTuple<float, float> valueTuple = CS$<>8__locals1.<>4__this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey][drive];
				float item = valueTuple.Item1;
				return valueTuple.Item2 < CS$<>8__locals1.statBasedMinimumDV_kps || item < CS$<>8__locals1.statBasedMinimumAcceleration_gs;
			})).ToList<TIDriveTemplate>();
			if (!this.isActivePlayer)
			{
				list3 = (from x in list3
					group x by x.driveTypeName).SelectMany<IGrouping<string, TIDriveTemplate>, TIDriveTemplate>(delegate(IGrouping<string, TIDriveTemplate> x)
				{
					TIDriveTemplate min = x.MinBy<TIDriveTemplate, int>((TIDriveTemplate y) => y.thrusters);
					TIDriveTemplate max = x.MaxBy<TIDriveTemplate, int>((TIDriveTemplate y) => y.thrusters);
					return x.Where<TIDriveTemplate>((TIDriveTemplate x) => x == min || x == max || TIUtilities.RandomFloatValue() < 0.25f);
				}).ToList<TIDriveTemplate>();
			}
			List<TIDriveTemplate> list4 = list3.Where<TIDriveTemplate>((TIDriveTemplate x) => CS$<>8__locals1.unfilteredPowerPlants.Any<TIPowerPlantTemplate>((TIPowerPlantTemplate y) => y.IsCompatible(x))).ToList<TIDriveTemplate>();
			List<TIDriveTemplate> list5 = list3.Where<TIDriveTemplate>((TIDriveTemplate x) => CS$<>8__locals1.filteredPowerPlants.Any<TIPowerPlantTemplate>((TIPowerPlantTemplate y) => y.IsCompatible(x))).ToList<TIDriveTemplate>();
			List<TIDriveTemplate> list6 = (from x in this.GetObsoleteFilteredParts<TIDriveTemplate>(list3)
				where CS$<>8__locals1.unfilteredPowerPlants.Any<TIPowerPlantTemplate>((TIPowerPlantTemplate y) => y.IsCompatible(x))
				select x).ToList<TIDriveTemplate>();
			List<TIDriveTemplate> list7 = (from x in this.GetObsoleteFilteredParts<TIDriveTemplate>(list3)
				where CS$<>8__locals1.filteredPowerPlants.Any<TIPowerPlantTemplate>((TIPowerPlantTemplate y) => y.IsCompatible(x))
				select x).ToList<TIDriveTemplate>();
			TIFactionState.<>c__DisplayClass1100_0 CS$<>8__locals5 = CS$<>8__locals1;
			IEnumerable<TIDriveTemplate> enumerable = Enumerable.Empty<IEnumerable<TIDriveTemplate>>().Append(list7).Append(list6)
				.Append(list5)
				.Append(list4)
				.FirstOrDefault<IEnumerable<TIDriveTemplate>>((IEnumerable<TIDriveTemplate> x) => x.Any<TIDriveTemplate>());
			List<TIDriveTemplate> list8;
			if (enumerable == null)
			{
				list8 = null;
			}
			else
			{
				list8 = enumerable.OrderBy<TIDriveTemplate, string>((TIDriveTemplate x) => x.dataName).ToList<TIDriveTemplate>();
			}
			CS$<>8__locals5.candidateDrives = list8 ?? new List<TIDriveTemplate>();
			if (CS$<>8__locals1.candidateDrives == null)
			{
				return TIFactionState.ShipDesignerOutcome.NoDrives;
			}
			List<TISpaceShipTemplate> list9 = CS$<>8__locals1.<DesignShip>g__GetCandidateDesigns|14();
			CS$<>8__locals1.statBasedMinimumDV_kps = CS$<>8__locals1.<DesignShip>g__GetStatBasedMinimumDV_kps|2();
			CS$<>8__locals1.statBasedMinimumAcceleration_gs = CS$<>8__locals1.<DesignShip>g__GetStatBasedMinimumAcceleration_gs|3();
			list9 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false) >= CS$<>8__locals1.statBasedMinimumDV_kps && x.baseCruiseAcceleration_gs(false) >= CS$<>8__locals1.statBasedMinimumAcceleration_gs).ToList<TISpaceShipTemplate>();
			if (CS$<>8__locals1.forcedSpecialModuleRules != null)
			{
				list9 = list9.Where<TISpaceShipTemplate>(delegate(TISpaceShipTemplate x)
				{
					Func<SpecialModuleRule, bool> <>9__49;
					return CS$<>8__locals1.forcedSpecialModuleRules.All<IEnumerable<SpecialModuleRule>>(delegate(IEnumerable<SpecialModuleRule> y)
					{
						Func<SpecialModuleRule, bool> func;
						if ((func = <>9__49) == null)
						{
							func = (<>9__49 = (SpecialModuleRule z) => x.HasSpecialModuleCapability(z));
						}
						return y.Any<SpecialModuleRule>(func);
					});
				}).ToList<TISpaceShipTemplate>();
			}
			if (list9.Count == 0)
			{
				return TIFactionState.ShipDesignerOutcome.NoCandidateDesigns;
			}
			bool flag = CS$<>8__locals1.role == ShipRole.InnerSystemColonyShip || CS$<>8__locals1.role == ShipRole.OuterSystemColonyShip;
			if (flag || CS$<>8__locals1.role == ShipRole.Explorer)
			{
				CS$<>8__locals1.minimumCruiseAcceleration = 0.001f;
			}
			else
			{
				CS$<>8__locals1.minimumCruiseAcceleration = 0.01f;
			}
			List<float> list10 = new List<float> { 0.7f, 0.5f, 0.3f };
			foreach (TISpaceShipTemplate tispaceShipTemplate in list9)
			{
				TIFactionState.<>c__DisplayClass1100_11 CS$<>8__locals6;
				CS$<>8__locals6.candidateDesign = tispaceShipTemplate;
				if (CS$<>8__locals1.<DesignShip>g__ShouldCutArmor|51(ref CS$<>8__locals6))
				{
					int armorValue = CS$<>8__locals6.candidateDesign.noseArmor.armorValue;
					int armorValue2 = CS$<>8__locals6.candidateDesign.lateralArmor.armorValue;
					int armorValue3 = CS$<>8__locals6.candidateDesign.tailArmor.armorValue;
					int num2 = Mathf.Min(armorValue, (4f * (float)CS$<>8__locals6.candidateDesign.hullTemplate.consTier).Round());
					int num3 = Mathf.Min(armorValue2, (1.01f * (float)CS$<>8__locals6.candidateDesign.hullTemplate.consTier).Round());
					int num4 = Mathf.Min(armorValue3, (1.01f * (float)CS$<>8__locals6.candidateDesign.hullTemplate.consTier).Round());
					foreach (float num5 in list10)
					{
						float totalArmorMass_tons = CS$<>8__locals6.candidateDesign.totalArmorMass_tons;
						CS$<>8__locals6.candidateDesign.TrySetArmor(ShipModuleSlotType.NoseArmor, Mathf.Max(num2, ((float)armorValue * num5).RoundUp()));
						CS$<>8__locals6.candidateDesign.TrySetArmor(ShipModuleSlotType.LateralArmor, Mathf.Max(num3, ((float)armorValue2 * Mathf.Pow(num5, 1.4f)).RoundUp()));
						CS$<>8__locals6.candidateDesign.TrySetArmor(ShipModuleSlotType.TailArmor, Mathf.Max(num4, ((float)armorValue3 * num5).RoundUp()));
						CS$<>8__locals1.<DesignShip>g__UpdateDesignAfterCuttingArmor|50(totalArmorMass_tons, ref CS$<>8__locals6);
						if (!CS$<>8__locals1.<DesignShip>g__ShouldCutArmor|51(ref CS$<>8__locals6))
						{
							break;
						}
					}
					if (((CS$<>8__locals1.hull.smallHull || flag) && CS$<>8__locals1.<DesignShip>g__ShouldCutArmor|51(ref CS$<>8__locals6)) || (flag && CS$<>8__locals6.candidateDesign.baseCruiseDeltaV_kps(false) < CS$<>8__locals1.desiredDV_kps))
					{
						float totalArmorMass_tons2 = CS$<>8__locals6.candidateDesign.totalArmorMass_tons;
						CS$<>8__locals6.candidateDesign.TrySetArmor(ShipModuleSlotType.TailArmor, (CS$<>8__locals1.hull.smallHull && !TISpaceShipTemplate.longRangeCombatant(CS$<>8__locals1.role)) ? 1 : 0);
						CS$<>8__locals1.<DesignShip>g__UpdateDesignAfterCuttingArmor|50(totalArmorMass_tons2, ref CS$<>8__locals6);
					}
					if (((CS$<>8__locals1.hull.smallHull || flag) && CS$<>8__locals1.<DesignShip>g__ShouldCutArmor|51(ref CS$<>8__locals6)) || (flag && CS$<>8__locals6.candidateDesign.baseCruiseDeltaV_kps(false) < CS$<>8__locals1.desiredDV_kps))
					{
						float totalArmorMass_tons3 = CS$<>8__locals6.candidateDesign.totalArmorMass_tons;
						CS$<>8__locals6.candidateDesign.TrySetArmor(ShipModuleSlotType.LateralArmor, (CS$<>8__locals1.hull.smallHull && !TISpaceShipTemplate.longRangeCombatant(CS$<>8__locals1.role)) ? 1 : 0);
						CS$<>8__locals1.<DesignShip>g__UpdateDesignAfterCuttingArmor|50(totalArmorMass_tons3, ref CS$<>8__locals6);
					}
				}
			}
			if (CS$<>8__locals1.exampleDestination != null)
			{
				if (CS$<>8__locals1.exampleOrigin == null)
				{
					CS$<>8__locals1.exampleOrigin = GameStateManager.LEOStates().First<TIOrbitState>();
				}
				Dictionary<TISpaceShipTemplate, float> dictionary = list9.ToDictionary<TISpaceShipTemplate, TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x, delegate(TISpaceShipTemplate x)
				{
					TIVirtualSpaceFleet tivirtualSpaceFleet = new TIVirtualSpaceFleet(CS$<>8__locals1.exampleOrigin, x.baseCruiseAcceleration_mps2(false), x.baseCruiseDeltaV_mps(false), CS$<>8__locals1.<>4__this, null, 0.0);
					IEnumerable<Trajectory> trajectories = Enumerable.Empty<Trajectory>();
					try
					{
						double num7;
						MasterTransferPlanner.RequestTrajectories(tivirtualSpaceFleet, CS$<>8__locals1.exampleDestination, 64, delegate(Trajectory[] t)
						{
							trajectories = t.ToList<Trajectory>();
						}, out num7, false, false, 1.0);
					}
					catch (Exception ex)
					{
						Log.Error(ex.Message + "\n" + ex.StackTrace, Array.Empty<object>());
					}
					if (!trajectories.Any<Trajectory>())
					{
						return float.PositiveInfinity;
					}
					return (float)trajectories.Min<Trajectory>((Trajectory y) => y.duration_d);
				});
				IEnumerable<KeyValuePair<TISpaceShipTemplate, float>> enumerable2 = dictionary.Where<KeyValuePair<TISpaceShipTemplate, float>>((KeyValuePair<TISpaceShipTemplate, float> x) => x.Value < CS$<>8__locals1.desiredMaxTransferDuration);
				if (!enumerable2.Any<KeyValuePair<TISpaceShipTemplate, float>>())
				{
					enumerable2 = dictionary.Where<KeyValuePair<TISpaceShipTemplate, float>>((KeyValuePair<TISpaceShipTemplate, float> x) => x.Value < CS$<>8__locals1.hardMaxTransferDuration);
				}
				list9 = enumerable2.Select<KeyValuePair<TISpaceShipTemplate, float>, TISpaceShipTemplate>((KeyValuePair<TISpaceShipTemplate, float> x) => x.Key).ToList<TISpaceShipTemplate>();
				if (list9.Count == 0)
				{
					return TIFactionState.ShipDesignerOutcome.NoCandidateDesigns;
				}
			}
			List<TISpaceShipTemplate> list11 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false) >= CS$<>8__locals1.desiredDV_kps && x.baseCruiseAcceleration_gs(false) >= CS$<>8__locals1.desiredCruiseAccel_gs).ToList<TISpaceShipTemplate>();
			if (list11.Count > 0)
			{
				list9 = list11;
			}
			else
			{
				if (TISpaceShipTemplate.shortRangeStrategic(CS$<>8__locals1.role) || (TISpaceShipTemplate.mediumRangeStrategic(CS$<>8__locals1.role) && list9.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false) >= CS$<>8__locals1.minimumCruiseAcceleration && x.baseCruiseDeltaV_kps(false) >= 10f)))
				{
					List<TISpaceShipTemplate> list12 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false) >= CS$<>8__locals1.minimumCruiseAcceleration).ToList<TISpaceShipTemplate>();
					if (list12.Count > 0)
					{
						list9 = list12;
					}
					if (list9.Count > 0)
					{
						IEnumerable<TISpaceShipTemplate> enumerable3 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => CS$<>8__locals1.<>4__this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey][x.driveTemplate].Item2 >= CS$<>8__locals1.desiredDV_kps);
						if (enumerable3.Any<TISpaceShipTemplate>())
						{
							list9 = enumerable3.ToList<TISpaceShipTemplate>();
						}
						else
						{
							List<TISpaceShipTemplate> list13 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => CS$<>8__locals1.<>4__this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey][x.driveTemplate].Item2 >= base.<DesignShip>g__GetStatBasedMinimumDV_kps|2()).ToList<TISpaceShipTemplate>();
							if (list13.Count > 0)
							{
								list9 = list13.ToList<TISpaceShipTemplate>();
							}
							else
							{
								float best2 = list9.Max<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false));
								list9 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false) >= best2 * 0.95f).ToList<TISpaceShipTemplate>();
							}
						}
					}
				}
				else
				{
					IEnumerable<TISpaceShipTemplate> enumerable4 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => CS$<>8__locals1.<>4__this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey][x.driveTemplate].Item2 >= CS$<>8__locals1.desiredDV_kps);
					if (enumerable4.Any<TISpaceShipTemplate>())
					{
						list9 = enumerable4.ToList<TISpaceShipTemplate>();
					}
					else
					{
						list9 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => CS$<>8__locals1.<>4__this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey][x.driveTemplate].Item2 >= base.<DesignShip>g__GetStatBasedMinimumDV_kps|2()).ToList<TISpaceShipTemplate>();
					}
					if (list9.Count > 0)
					{
						List<TISpaceShipTemplate> list14 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false) >= CS$<>8__locals1.minimumCruiseAcceleration).ToList<TISpaceShipTemplate>();
						if (list14.Count > 0)
						{
							list9 = list14;
						}
						else
						{
							float best = list9.Max<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false));
							list9 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false) >= best * 0.95f).ToList<TISpaceShipTemplate>();
						}
					}
				}
				list9 = list9.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false) > 0.001f || (x.hullTemplate.consTier == 1 && x.thrusterCount == 6)).ToList<TISpaceShipTemplate>();
			}
			if (list9.Count == 0)
			{
				return TIFactionState.ShipDesignerOutcome.MinimumPropulsionRequirementsNotMet;
			}
			Dictionary<TISpaceShipTemplate, float> dictionary2 = list9.ToDictionary<TISpaceShipTemplate, TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x, delegate(TISpaceShipTemplate candidateDesign)
			{
				TIResourcesCost tiresourcesCost = candidateDesign.spaceResourceConstructionCost(true, null, true, false, false);
				float num8 = 0f;
				foreach (FactionResource factionResource in TIResourcesCost.habResources)
				{
					float monthlyIncome = CS$<>8__locals1.<>4__this.GetMonthlyIncome(factionResource, true, false);
					float singleCostValue = tiresourcesCost.GetSingleCostValue(factionResource);
					float num9 = 0f;
					if (singleCostValue > 0f)
					{
						num9 = Mathf.Min(10f, singleCostValue / monthlyIncome);
					}
					num8 += num9;
				}
				float num10 = 1f / num8;
				float item2 = CS$<>8__locals1.<>4__this.shipDesigner_CachedDriveStats[CS$<>8__locals1.designKey][candidateDesign.driveTemplate].Item2;
				if (item2 < CS$<>8__locals1.desiredDV_kps)
				{
					num10 *= item2 / CS$<>8__locals1.desiredDV_kps;
				}
				num10 *= Mathf.Log10(Mathf.Max(item2, 1f));
				float num11 = candidateDesign.baseCruiseAcceleration_gs(false);
				if (num11 < CS$<>8__locals1.desiredCruiseAccel_gs)
				{
					num10 *= num11 / CS$<>8__locals1.desiredCruiseAccel_gs;
				}
				float num12 = Mathf.Log10(num11) + 3f;
				if (num12 >= 0f)
				{
					num12 += 1f;
				}
				else
				{
					num12 = 1f / (-num12 + 1f);
				}
				num10 *= num12;
				int num13 = base.<DesignShip>g__GetDesiredNoseArmorValue|1(candidateDesign.noseArmor.materialTemplate);
				return num10 * (0.5f + (float)candidateDesign.noseArmor.armorValue / (float)num13 * 0.5f);
			});
			design = dictionary2.MaxBy<KeyValuePair<TISpaceShipTemplate, float>, float>((KeyValuePair<TISpaceShipTemplate, float> x) => x.Value).Key;
			if (design == null)
			{
				return TIFactionState.ShipDesignerOutcome.NoScoredDesigns;
			}
			design.SetClassDisplayName(false);
			if (this.player.isAI)
			{
				if (!CS$<>8__locals1.allowAntimatter && design.requiresAntimatter)
				{
					return TIFactionState.ShipDesignerOutcome.AntimatterRequired;
				}
				if (!CS$<>8__locals1.allowExotics && design.requiresExotics)
				{
					return TIFactionState.ShipDesignerOutcome.ExoticsRequired;
				}
				switch (design.driveTemplate.driveClassification)
				{
				case DriveClassification.Chemical:
					design.hullAppearanceIndex = TIUtilities.GetHullAppearanceIndex(this.template.hullIndex_chem);
					break;
				case DriveClassification.Electrothermal:
				case DriveClassification.Electromagnetic:
				case DriveClassification.Electrostatic:
					design.hullAppearanceIndex = TIUtilities.GetHullAppearanceIndex(this.template.hullIndex_electric);
					break;
				case DriveClassification.Fission_Thermal:
				case DriveClassification.Fission_Pulse:
				case DriveClassification.NuclearSaltWater:
					design.hullAppearanceIndex = TIUtilities.GetHullAppearanceIndex(this.template.hullIndex_fission);
					break;
				case DriveClassification.Fusion_Thermal:
				case DriveClassification.Fusion_Pulse:
					design.hullAppearanceIndex = TIUtilities.GetHullAppearanceIndex((design.driveTemplate.powerRequirement_GW <= 100f) ? this.template.hullIndex_fusion : this.template.hullIndex_fusion_adv);
					break;
				case DriveClassification.Antimatter:
					design.hullAppearanceIndex = TIUtilities.GetHullAppearanceIndex(this.template.hullIndex_amat);
					break;
				default:
					design.hullAppearanceIndex = this.template.hullIndex_default;
					break;
				}
			}
			if (design.AllowedRole(design.role))
			{
				return TIFactionState.ShipDesignerOutcome.Success;
			}
			return TIFactionState.ShipDesignerOutcome.DesignNotAllowedForRole;
		}

		// Token: 0x0600340C RID: 13324 RVA: 0x001241FC File Offset: 0x001223FC
		public TIFactionState.ShipDesignerOutcome DesignAlienShip(ShipRole role, out TISpaceShipTemplate design, float desiredStrategicRange_AU, bool allowExotics = false, bool allowAntimatter = false, TIShipHullTemplate forceHull = null, bool heavy = false, int designPasses = 122)
		{
			List<TIShipHullTemplate> list = this.allowedShipHulls.ToList<TIShipHullTemplate>();
			design = null;
			string text = string.Empty;
			if (forceHull == null)
			{
				int minSlots = 0;
				int minNoseSlots = 0;
				int minHullSlots = 0;
				switch (role)
				{
				case ShipRole.TroopCarrier:
				case ShipRole.ArmyCarrier:
					minSlots = 4;
					break;
				case ShipRole.Explorer:
					minSlots = 3;
					break;
				case ShipRole.InnerSystemColonyShip:
					minSlots = 2;
					break;
				case ShipRole.OuterSystemColonyShip:
					minSlots = 2;
					break;
				case ShipRole.EarthSurveillance:
					minSlots = 3;
					minHullSlots = 2;
					break;
				case ShipRole.CouncilorTransport:
					minSlots = 1;
					break;
				case ShipRole.LS_Penetrator:
					minSlots = 3;
					break;
				case ShipRole.LM_Protector:
				case ShipRole.SL_Defender:
					minSlots = 3;
					minHullSlots = 2;
					break;
				case ShipRole.LM_Interdictor:
					minSlots = 1;
					minHullSlots = 1;
					break;
				case ShipRole.LL_Intruder:
					minSlots = 3;
					minHullSlots = 2;
					break;
				case ShipRole.LL_Bomber:
					minSlots = 1;
					minNoseSlots = 2;
					break;
				case ShipRole.MS_Strike:
					minSlots = 1;
					break;
				case ShipRole.MM_SpaceSuperiority:
					minSlots = 4;
					minHullSlots = 1;
					break;
				case ShipRole.ML_Standoff:
					minSlots = 3;
					minHullSlots = 2;
					break;
				case ShipRole.SS_Interceptor:
					minSlots = 1;
					break;
				case ShipRole.SM_Patrol:
					minSlots = 2;
					minHullSlots = 1;
					break;
				}
				if (heavy)
				{
					int i;
					int k;
					for (k = minSlots + 4; k > minSlots; k = i - 1)
					{
						list = list.Where<TIShipHullTemplate>((TIShipHullTemplate x) => x.internalModules >= k && x.hullHardpoints >= minHullSlots && x.noseHardpoints >= minNoseSlots).ToList<TIShipHullTemplate>();
						if (list.Count > 0)
						{
							break;
						}
						i = k;
					}
				}
				else
				{
					list = list.Where<TIShipHullTemplate>((TIShipHullTemplate x) => x.internalModules >= minSlots && x.hullHardpoints >= minHullSlots && x.noseHardpoints >= minNoseSlots).ToList<TIShipHullTemplate>();
				}
				if (list.Count == 0)
				{
					return TIFactionState.ShipDesignerOutcome.NoAvailableHulls;
				}
				text = list.MinBy<TIShipHullTemplate, float>((TIShipHullTemplate x) => x.mass_tons).dataName;
			}
			else
			{
				if (!list.Contains(forceHull))
				{
					return TIFactionState.ShipDesignerOutcome.ForcedHullNotAvailable;
				}
				text = forceHull.dataName;
			}
			string text2 = text;
			TIShipHullTemplate flagshipHull = this.FlagshipHull;
			text2 == (((flagshipHull != null) ? flagshipHull.dataName : null) ?? "");
			float num;
			int num2;
			float num3;
			switch (role)
			{
			case ShipRole.TroopCarrier:
			case ShipRole.ArmyCarrier:
				num = 900f;
				num2 = 20;
				num3 = 0.01f;
				goto IL_03F7;
			case ShipRole.InnerSystemColonyShip:
			case ShipRole.OuterSystemColonyShip:
				num = 800f;
				num2 = 5;
				num3 = 0.01f;
				goto IL_03F7;
			case ShipRole.EarthSurveillance:
				num = 800f;
				num2 = 8;
				num3 = 0.02f;
				goto IL_03F7;
			case ShipRole.CouncilorTransport:
				num = 800f;
				num2 = 12;
				num3 = 0.05f;
				goto IL_03F7;
			case ShipRole.LS_Penetrator:
				num = 900f;
				num2 = 20;
				num3 = 0.05f;
				goto IL_03F7;
			case ShipRole.LM_Protector:
			case ShipRole.LM_Interdictor:
				num = 900f;
				num2 = 14;
				num3 = 0.04f;
				goto IL_03F7;
			case ShipRole.LL_Intruder:
				num = 900f;
				num2 = 12;
				num3 = 0.03f;
				goto IL_03F7;
			case ShipRole.LL_Bomber:
				num = 900f;
				num2 = 30;
				num3 = 0.05f;
				goto IL_03F7;
			case ShipRole.MS_Strike:
				num = 600f;
				num2 = 20;
				num3 = 0.07f;
				goto IL_03F7;
			case ShipRole.MM_SpaceSuperiority:
				num = 600f;
				num2 = 20;
				num3 = 0.05f;
				goto IL_03F7;
			case ShipRole.ML_Standoff:
				num = 600f;
				num2 = 16;
				num3 = 0.04f;
				goto IL_03F7;
			case ShipRole.SS_Interceptor:
				num = 200f;
				num2 = 22;
				num3 = 0.07f;
				goto IL_03F7;
			case ShipRole.SM_Patrol:
				num = 200f;
				num2 = 22;
				num3 = 0.06f;
				goto IL_03F7;
			case ShipRole.SL_Defender:
				num = 200f;
				num2 = 20;
				num3 = 0.05f;
				goto IL_03F7;
			}
			num3 = 0.04f;
			num = 300f;
			num2 = 10;
			IL_03F7:
			if (allowExotics)
			{
				num2 = (int)Mathf.Round((float)num2 * 1.25f);
				num3 *= 1.25f;
			}
			design = new TISpaceShipTemplate(TemplateManager.GenerateDataName(new StringBuilder(this.templateName).Append("ShipTemplate").ToString()))
			{
				factionName = this.templateName,
				hullName = text
			};
			design.InitAtRunTime(false);
			design.role = role;
			int num4 = ((design.combatant || design.role == ShipRole.CouncilorTransport || design.role == ShipRole.TroopCarrier || design.role == ShipRole.ArmyCarrier) ? 10 : 3);
			int num5 = ((design.combatant || design.role == ShipRole.CouncilorTransport || design.role == ShipRole.TroopCarrier || design.role == ShipRole.ArmyCarrier) ? 3 : 1);
			TIDriveTemplate tidriveTemplate = this.GetBestDrive(role, 1, allowAntimatter, allowExotics, desiredStrategicRange_AU);
			if (tidriveTemplate == null)
			{
				return TIFactionState.ShipDesignerOutcome.NoDrives;
			}
			string maxThrustersTemplateName = tidriveTemplate.maxThrustersTemplateName;
			if (design.hullTemplate.consTier >= 3)
			{
				tidriveTemplate = TemplateManager.Find<TIDriveTemplate>(maxThrustersTemplateName, false);
			}
			design.SetDriveTemplate(tidriveTemplate.dataName);
			TIPowerPlantTemplate bestPowerPlant = this.GetBestPowerPlant(design, allowExotics, allowAntimatter, null);
			if (bestPowerPlant == null)
			{
				return TIFactionState.ShipDesignerOutcome.NoPowerPlants;
			}
			design.SetPowerPlantTemplate(bestPowerPlant.dataName);
			TIShipArmorTemplate bestArmor = this.GetBestArmor(allowExotics);
			bool flag = bestArmor.buildCost(1f, 0f).GetSingleCostValue(FactionResource.Exotics) > 0f;
			float num6 = Mathf.Max(1f, design.hullTemplate.length_m / design.hullTemplate.width_m);
			if (design.hullTemplate.smallHull)
			{
				num6 += 0.5f;
			}
			else if (design.hullTemplate.mediumHull)
			{
				num2 *= 2;
				num6 += 1f;
				num5 += 2;
			}
			else if (design.hullTemplate.largeHull)
			{
				num2 = ((float)(num2 + 3) * 1.1f).Round();
				num3 *= 0.7f;
				num4 += 5;
				num5 += 5;
				num6 += 1.4f;
			}
			else if (design.hullTemplate.hugeHull)
			{
				num2 = ((float)(num2 + 5) * 2.5f).Round();
				num4 += 5;
				num5 += 10;
				num3 = 0.01f;
				num *= 0.9f;
				num6 += 2f;
			}
			num2 = Mathf.RoundToInt((float)num2 * 3500f / bestArmor.density_kgm3);
			design.noseArmor.materialName = bestArmor.dataName;
			design.TrySetArmor(ShipModuleSlotType.NoseArmor, num2);
			design.lateralArmor.materialName = bestArmor.dataName;
			design.TrySetArmor(ShipModuleSlotType.LateralArmor, ((float)num2 / num6).Round());
			design.tailArmor.materialName = bestArmor.dataName;
			int num7;
			switch (role)
			{
			case ShipRole.TroopCarrier:
			case ShipRole.ArmyCarrier:
			case ShipRole.InnerSystemColonyShip:
			case ShipRole.OuterSystemColonyShip:
			case ShipRole.EarthSurveillance:
			case ShipRole.CouncilorTransport:
				num7 = num2;
				goto IL_071B;
			case ShipRole.LS_Penetrator:
			case ShipRole.MS_Strike:
			case ShipRole.SS_Interceptor:
				num7 = Mathf.RoundToInt((float)num2 * 0.75f);
				goto IL_071B;
			}
			num7 = Mathf.Max((float)num2 * 0.25f, (float)num2 / num6).Round();
			IL_071B:
			design.TrySetArmor(ShipModuleSlotType.TailArmor, num7);
			this.SetShipDesignNoseWeapons(false, ref design, allowExotics, null);
			this.SetShipDesignHullWeapons(false, ref design, allowExotics, null);
			design.moduleTemplateEntries = this.GetBestUtilityModules(design, allowExotics, allowAntimatter, null, null);
			if (role - ShipRole.TroopCarrier <= 6)
			{
				if (!design.AllowedRole(role))
				{
					return TIFactionState.ShipDesignerOutcome.DesignNotAllowedForRole;
				}
			}
			else if (design.noseWeaponTemplateEntries.Count == 0 && design.hullWeaponTemplateEntries.Count == 0)
			{
				return TIFactionState.ShipDesignerOutcome.NoWeapons;
			}
			design.SetRadiatorTemplate(this.GetBestRadiator(design, allowExotics).dataName);
			design.propellantTanks = 1;
			bool flag2 = false;
			int num8 = 0;
			bool flag3 = false;
			float num9;
			if (design.combatant && !design.hullTemplate.hugeHull)
			{
				if (flag)
				{
					num9 = 5.25f - (float)design.hullTemplate.consTier * 1.25f;
				}
				else
				{
					num9 = 4f - (float)design.hullTemplate.consTier;
				}
			}
			else
			{
				num9 = 1f - (float)design.hullTemplate.consTier * 0.25f;
			}
			while (!flag2)
			{
				num8++;
				float num10 = design.baseCruiseDeltaV_kps(true);
				float num11 = design.baseCruiseAcceleration_gs(true);
				if (num11 >= num3 && num10 >= num)
				{
					flag2 = true;
				}
				else
				{
					if (num8 % 25 == 0)
					{
						num = Mathf.Max(250f, num * 0.9f);
					}
					if (num8 % 40 == 0)
					{
						num3 *= 0.9f;
					}
					if (num8 > designPasses)
					{
						if (!this.player.isAI)
						{
							break;
						}
						if (num11 < num3)
						{
							if (design.hullTemplate.hugeHull || (num11 >= 0.01f && (num11 >= num3 * 0.75f || design.hullTemplate.largeHull)))
							{
								return TIFactionState.ShipDesignerOutcome.Success;
							}
							TIFactionState.LogAI("Accel fail: " + design.DebugSummary(), false);
							return TIFactionState.ShipDesignerOutcome.AITooManyPasses_InsufficientAcceleration;
						}
						else
						{
							if (num10 < num)
							{
								return TIFactionState.ShipDesignerOutcome.AITooManyPasses_InsufficientDeltaV;
							}
							return TIFactionState.ShipDesignerOutcome.AITooManyPasses_Generic;
						}
					}
					else
					{
						bool flag4 = false;
						if (num10 < num)
						{
							int num12 = ((num8 > 25) ? ((num8 > 50) ? ((num8 > 75) ? 10 : 3) : 2) : 1);
							design.propellantTanks += num12;
							if (design.propellantTanks % 10 == 0 || num12 == 10)
							{
								flag4 = true;
							}
							num10 = design.baseCruiseDeltaV_kps(true);
							num11 = design.baseCruiseAcceleration_gs(true);
						}
						if ((num11 < num3 || design.driveTemplate.selfPowered || num10 > num * 2f) && design.driveName != maxThrustersTemplateName)
						{
							TIDriveTemplate tidriveTemplate2 = design.driveTemplate.AddThruster(design, 1);
							if (tidriveTemplate2 != null)
							{
								design.SetDriveTemplate(tidriveTemplate2.dataName);
								num11 = design.baseCruiseAcceleration_gs(true);
							}
						}
						else if (num11 < num3)
						{
							if (!flag4)
							{
								if (num10 > num * 1.25f)
								{
									design.propellantTanks = Math.Max(1, (int)((float)design.propellantTanks * 0.95f));
								}
								num11 = design.baseCruiseAcceleration_gs(true);
							}
							if (num11 < num3)
							{
								flag4 = true;
							}
						}
						if (flag4)
						{
							flag3 = design.driveTemplate.thrusters == 6;
							int num13 = ((num8 > designPasses - 10) ? ((designPasses - num8) * -1) : (-1));
							if ((float)design.noseArmor.armorValue / num6 > (float)design.lateralArmor.armorValue && design.noseArmor.armorValue > num4)
							{
								design.TryAddArmorPoints(ShipModuleSlotType.NoseArmor, num13);
							}
							else
							{
								if (design.lateralArmor.armorValue > num5)
								{
									design.TryAddArmorPoints(ShipModuleSlotType.LateralArmor, -1);
								}
								if (design.tailArmor.armorValue > num5)
								{
									design.TryAddArmorPoints(ShipModuleSlotType.TailArmor, num13 * 2);
								}
							}
						}
						else
						{
							bool flag5 = !flag3;
							bool flag6 = false;
							bool flag7 = false;
							bool flag8 = false;
							while (flag5)
							{
								num11 = design.baseCruiseAcceleration_gs(true);
								num10 = design.baseCruiseDeltaV_kps(true);
								float baseCombatAcceleration_gs = design.baseCombatAcceleration_gs;
								if (num10 > 1.02f * num && num11 > num3 * 1.02f && baseCombatAcceleration_gs >= num9)
								{
									if (!flag6 && (float)design.noseArmor.armorValue / num6 < (float)design.lateralArmor.armorValue)
									{
										if (design.TryAddArmorPoints(ShipModuleSlotType.NoseArmor, 1) < 1)
										{
											flag6 = true;
										}
										else if (flag && (float)design.noseArmor.armorValue >= 3f * design.hullTemplate.width_m)
										{
											flag6 = true;
										}
									}
									else
									{
										if (!flag7)
										{
											if (design.TryAddArmorPoints(ShipModuleSlotType.LateralArmor, 1) < 1)
											{
												flag7 = true;
											}
											else if (flag && (float)design.lateralArmor.armorValue >= design.hullTemplate.length_m / 5f)
											{
												flag7 = true;
											}
										}
										if (!flag8)
										{
											if (design.TryAddArmorPoints(ShipModuleSlotType.TailArmor, 1) < 1)
											{
												flag8 = true;
											}
											else if (flag && (float)design.tailArmor.armorValue >= 2f * design.hullTemplate.width_m)
											{
												flag8 = true;
											}
										}
									}
									flag5 = !flag6 && !flag7 && !flag8;
								}
								else
								{
									flag5 = false;
								}
							}
						}
					}
				}
			}
			design.spaceResourceConstructionCost(true, null, true, false, false);
			design.dryMass_tons(true);
			if (this.player.isAI)
			{
				if (!allowAntimatter && design.requiresAntimatter)
				{
					return TIFactionState.ShipDesignerOutcome.AntimatterRequired;
				}
				if (!allowExotics && design.requiresExotics)
				{
					return TIFactionState.ShipDesignerOutcome.ExoticsRequired;
				}
				design.hullAppearanceIndex = this.template.hullIndex_default;
			}
			if (design.AllowedRole(design.role))
			{
				return TIFactionState.ShipDesignerOutcome.Success;
			}
			return TIFactionState.ShipDesignerOutcome.DesignNotAllowedForRole;
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x00124E6C File Offset: 0x0012306C
		public TISpaceShipTemplate DesignRefit(TISpaceShipTemplate original)
		{
			TISpaceShipTemplate tispaceShipTemplate = new TISpaceShipTemplate(original.dataName + " Refit " + this.nextRefitNumber.ToString())
			{
				factionName = this.templateName,
				hullName = original.hullName
			};
			tispaceShipTemplate.InitAtRunTime(false);
			tispaceShipTemplate.role = original.role;
			TIResourcesCost tiresourcesCost = original.spaceResourceConstructionCost(false, null, true, true, false);
			bool flag = tiresourcesCost.GetSingleCostValue(FactionResource.Exotics) > 0f;
			bool flag2 = tiresourcesCost.GetSingleCostValue(FactionResource.Antimatter) > 0f;
			List<TIPowerPlantTemplate> list = this.allowedPowerPlants.Where<TIPowerPlantTemplate>((TIPowerPlantTemplate x) => x.IsValidRefitPart(original)).ToList<TIPowerPlantTemplate>();
			TIDriveTemplate tidriveTemplate = (from x in this.allowedDrives
				where x.IsValidRefitPart(original)
				where x.EV_kps >= original.driveTemplate.EV_kps
				select x).MaxBy<TIDriveTemplate, float>((TIDriveTemplate x) => Mathf.Pow(x.EV_kps / original.driveTemplate.EV_kps, 1.5f) * Mathf.Pow(x.thrust_N / original.driveTemplate.thrust_N, 1f));
			if (tidriveTemplate == null)
			{
				return null;
			}
			if (tidriveTemplate.IsSameDriveWithDifferentThrusterCount(original.driveTemplate))
			{
				tidriveTemplate = original.driveTemplate;
			}
			tispaceShipTemplate.SetDriveTemplate(tidriveTemplate.dataName);
			TIPowerPlantTemplate bestPowerPlant = this.GetBestPowerPlant(tispaceShipTemplate, flag, flag2, list);
			if (bestPowerPlant == null)
			{
				return null;
			}
			tispaceShipTemplate.SetPowerPlantTemplate(bestPowerPlant.dataName);
			IEnumerable<TIShipWeaponTemplate> factionWeapons = this.allowedNoseWeapons.Union<TIShipWeaponTemplate>(this.allowedHullWeapons);
			using (List<ModuleDataEntry>.Enumerator enumerator = original.allWeapons.GetEnumerator())
			{
				Func<TIShipWeaponTemplate, float> <>9__5;
				while (enumerator.MoveNext())
				{
					ModuleDataEntry weapon = enumerator.Current;
					IEnumerable<TIShipWeaponTemplate> enumerable = factionWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.IsValidRefitPart(weapon.weaponTemplate));
					Func<TIShipWeaponTemplate, float> func;
					if ((func = <>9__5) == null)
					{
						func = (<>9__5 = (TIShipWeaponTemplate x) => x.GetCuratedDesignScore(original.role, factionWeapons, false));
					}
					TIShipWeaponTemplate tishipWeaponTemplate = enumerable.MaxBy<TIShipWeaponTemplate, float>(func);
					if (tishipWeaponTemplate != null)
					{
						ModuleDataTemplateEntry moduleDataTemplateEntry = new ModuleDataTemplateEntry(tishipWeaponTemplate, weapon.slotIndex);
						if (tishipWeaponTemplate.noseWeapon)
						{
							tispaceShipTemplate.noseWeaponTemplateEntries.Add(moduleDataTemplateEntry);
						}
						else
						{
							tispaceShipTemplate.hullWeaponTemplateEntries.Add(moduleDataTemplateEntry);
						}
					}
				}
			}
			TIRadiatorTemplate bestRadiator = this.GetBestRadiator(tispaceShipTemplate, flag);
			if (bestRadiator == null)
			{
				return null;
			}
			tispaceShipTemplate.SetRadiatorTemplate(bestRadiator.dataName);
			foreach (ModuleDataEntry moduleDataEntry in original.utilityModules)
			{
				TIShipPartTemplate tishipPartTemplate = moduleDataEntry.moduleTemplate;
				if (moduleDataEntry.moduleTemplate.isBattery)
				{
					tishipPartTemplate = this.GetBestBattery(tispaceShipTemplate, flag);
				}
				else if (moduleDataEntry.moduleTemplate.isHeatSink)
				{
					tishipPartTemplate = this.GetBestHeatSink(flag);
				}
				tispaceShipTemplate.moduleTemplateEntries.Add(new ModuleDataTemplateEntry(tishipPartTemplate, moduleDataEntry.slotIndex));
			}
			if (!tispaceShipTemplate.AllowedRole(original.role))
			{
				return null;
			}
			TIShipArmorTemplate bestArmor = this.GetBestArmor(flag);
			if (bestArmor == null)
			{
				return null;
			}
			tispaceShipTemplate.SetTailArmorTemplate(bestArmor.dataName);
			tispaceShipTemplate.TrySetArmor(ShipModuleSlotType.TailArmor, original.tailArmor.armorValue);
			tispaceShipTemplate.SetLateralArmorTemplate(bestArmor.dataName);
			tispaceShipTemplate.TrySetArmor(ShipModuleSlotType.LateralArmor, original.lateralArmor.armorValue);
			tispaceShipTemplate.SetNoseArmorTemplate(bestArmor.dataName);
			tispaceShipTemplate.TrySetArmor(ShipModuleSlotType.NoseArmor, original.noseArmor.armorValue);
			tispaceShipTemplate.propellantTanks = original.propellantTanks;
			if (tispaceShipTemplate.IsDuplicateOf(original))
			{
				return null;
			}
			float num = original.baseCruiseDeltaV_kps(false);
			tispaceShipTemplate.dryMass_tons(true);
			float num2;
			tispaceShipTemplate.propellantTanks = tispaceShipTemplate.GetIdealPropellentTankCount(num, out num2);
			if (num2 < num)
			{
				return null;
			}
			this.nextRefitNumber++;
			string text;
			if (!tispaceShipTemplate.IsAValidRefitFor(original, out text, false))
			{
				global::UnityEngine.Debug.LogError("AI code passed illegal refit, please make a bug report with save file");
				return null;
			}
			return tispaceShipTemplate;
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x00125268 File Offset: 0x00123468
		public bool HasRefitForTemplate(TISpaceShipTemplate originalTemplate)
		{
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.shipDesigns)
			{
				string text;
				if (originalTemplate != tispaceShipTemplate && !this.obsoleteShipDesigns.Contains(tispaceShipTemplate.dataName) && tispaceShipTemplate.IsAValidRefitFor(originalTemplate, out text, false))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x001252E0 File Offset: 0x001234E0
		public bool HasRefitForTemplate(TISpaceShipTemplate originalTemplate, out TISpaceShipTemplate refitTemplate)
		{
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.shipDesigns)
			{
				string text;
				if (originalTemplate != tispaceShipTemplate && !this.obsoleteShipDesigns.Contains(tispaceShipTemplate.dataName) && tispaceShipTemplate.IsAValidRefitFor(originalTemplate, out text, false))
				{
					refitTemplate = tispaceShipTemplate;
					return true;
				}
			}
			refitTemplate = null;
			return false;
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x0012535C File Offset: 0x0012355C
		public List<TIShipWeaponTemplate> AllowedHumanFighterNoseWeapons()
		{
			return (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
				where !x.isAlien
				select x into module
				where module.mount == Mount.HalfNose && (module.requiredProject == null || this.completedProjects.Contains(module.requiredProject))
				select module).ToList<TIShipWeaponTemplate>();
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x001253AC File Offset: 0x001235AC
		public List<TIShipWeaponTemplate> AllowedFighterHullWeapons()
		{
			List<TIShipWeaponTemplate> list;
			if (this.IsAlienFaction)
			{
				list = (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
					where x.isAlien
					select x into module
					where module.mount == Mount.HalfHull
					select module).ToList<TIShipWeaponTemplate>();
			}
			else
			{
				list = (from x in TemplateManager.IterateByClass<TIShipWeaponTemplate>(true)
					where !x.isAlien
					select x into module
					where module.mount == Mount.HalfHull && (module.requiredProject == null || this.completedProjects.Contains(module.requiredProject))
					select module).ToList<TIShipWeaponTemplate>();
			}
			return list;
		}

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x06003412 RID: 13330 RVA: 0x00125459 File Offset: 0x00123659
		public int EarthSTOFightersAvailable
		{
			get
			{
				return this.executiveNations.Sum<TINationState>((TINationState x) => x.availableSTOFighters);
			}
		}

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x06003413 RID: 13331 RVA: 0x00125485 File Offset: 0x00123685
		public int TotalEarthSTOFighters
		{
			get
			{
				return this.executiveNations.Sum<TINationState>((TINationState x) => x.numSTOFighters);
			}
		}

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x06003414 RID: 13332 RVA: 0x001254B4 File Offset: 0x001236B4
		public List<TISpaceAssetState> TargetsForSTOFighters
		{
			get
			{
				List<TISpaceAssetState> list = new List<TISpaceAssetState>();
				using (List<TISpaceAssetState>.Enumerator enumerator = GameStateManager.Earth().assetsInInterfaceOrbits.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceAssetState asset = enumerator.Current;
						if (!asset.faction.permanentAlly(this) && (!asset.isHabState || !asset.ref_hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction != asset.faction)))
						{
							list.Add(asset);
						}
					}
				}
				return list;
			}
		}

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x06003415 RID: 13333 RVA: 0x00125568 File Offset: 0x00123768
		public bool CanLaunchSTOFighters
		{
			get
			{
				return this.EarthSTOFightersAvailable > 0 && (this.IsAlienFaction || TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron)) && this.TargetsForSTOFighters.Count > 0;
			}
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x00125594 File Offset: 0x00123794
		public float STOFighterLaunchCost(TISpaceShipTemplate fighterTemplate)
		{
			return fighterTemplate.wetMass_tons * TIGlobalConfig.globalConfig.spaceResourceToTons;
		}

		// Token: 0x06003417 RID: 13335 RVA: 0x001255A8 File Offset: 0x001237A8
		[return: TupleElementNames(new string[] { "Strength", "BoostCost" })]
		public ValueTuple<float, float> GetAverageSTOFighterStats()
		{
			List<PlannedFighters> list = this.executiveNations.Select<TINationState, PlannedFighters>(delegate(TINationState x)
			{
				List<TIShipWeaponTemplate> list2 = this.AllowedFighterHullWeapons();
				return new PlannedFighters(this.DesignSTOFighter(x, list2.MaxBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate y) => y.GenericScore())), x.availableSTOFighters);
			}).ToList<PlannedFighters>();
			int num = list.Sum<PlannedFighters>((PlannedFighters x) => x.count);
			return new ValueTuple<float, float>(list.Sum<PlannedFighters>((PlannedFighters x) => x.fighter.TemplateSpaceCombatValue(false, -1f, 1f, false)) / (float)num, list.Sum<PlannedFighters>((PlannedFighters x) => x.boostCost) / (float)num);
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x06003418 RID: 13336 RVA: 0x0012564D File Offset: 0x0012384D
		private float desiredSTOWetMass_tons
		{
			get
			{
				return TemplateManager.global.desiredSTOFighterWetMass_tons;
			}
		}

		// Token: 0x06003419 RID: 13337 RVA: 0x0012565C File Offset: 0x0012385C
		public TISpaceShipTemplate DesignSTOFighter(TINationState homeNation, TIShipWeaponTemplate primaryArmament = null)
		{
			TIFactionState.<>c__DisplayClass1123_0 CS$<>8__locals1 = new TIFactionState.<>c__DisplayClass1123_0();
			if (primaryArmament == null)
			{
				primaryArmament = this.AllowedFighterHullWeapons().MaxBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => x.GenericScore());
			}
			CS$<>8__locals1.fighterTemplate = new TISpaceShipTemplate(new StringBuilder(this.templateName).Append("FighterDesign").Append(((homeNation != null) ? homeNation.templateName : null) ?? "TestDesign").ToString());
			CS$<>8__locals1.fighterTemplate.InitAtRunTime(true);
			bool flag = false;
			CS$<>8__locals1.fighterTemplate.nation = homeNation;
			if (homeNation != null)
			{
				flag = homeNation.alienNation;
				IEnumerable<TIRegionState> enumerable = homeNation.regions.Where<TIRegionState>((TIRegionState x) => x.boostPerMonth_dekatons > 0f);
				if (enumerable.Count<TIRegionState>() == 0)
				{
					enumerable = homeNation.regions;
					if (enumerable.Count<TIRegionState>() == 0)
					{
						enumerable = GameStateManager.AllRegions();
					}
				}
				CS$<>8__locals1.fighterTemplate.SetDisplayName(enumerable.SelectRandomItem<TIRegionState>().fighterSquadronName);
			}
			CS$<>8__locals1.fighterTemplate.factionName = this.templateName;
			CS$<>8__locals1.fighterTemplate.role = ShipRole.SS_Interceptor;
			CS$<>8__locals1.fighterTemplate.hullName = (flag ? "SalamanderGunship" : "STOFighter");
			CS$<>8__locals1.fighterTemplate.SetRadiatorTemplate(this.GetBestRadiatorRaw().dataName);
			foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in CS$<>8__locals1.fighterTemplate.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.NoseHardPoint))
			{
				if (homeNation != null && homeNation.alienNation)
				{
					CS$<>8__locals1.fighterTemplate.noseWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(TemplateManager.Find<TIShipWeaponTemplate>("AlienMiniLightMagCannon", true), CS$<>8__locals1.fighterTemplate.hullTemplate.slotIndex(shipModuleSlot)));
				}
				else
				{
					CS$<>8__locals1.fighterTemplate.noseWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(this.AllowedHumanFighterNoseWeapons().MaxBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => x.BaseDamageAtRange_points(200f, true)), CS$<>8__locals1.fighterTemplate.hullTemplate.slotIndex(shipModuleSlot)));
				}
			}
			if (primaryArmament == null)
			{
				primaryArmament = (flag ? TemplateManager.Find<TIShipWeaponTemplate>("GlitteringJewelMissilePod", true) : TemplateManager.Find<TIShipWeaponTemplate>("KraitMissilePod", true));
			}
			foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 in CS$<>8__locals1.fighterTemplate.hullTemplate.GetAllSlotsOfType(ShipModuleSlotType.HullHardPoint))
			{
				CS$<>8__locals1.fighterTemplate.hullWeaponTemplateEntries.Add(new ModuleDataTemplateEntry(primaryArmament, CS$<>8__locals1.fighterTemplate.hullTemplate.slotIndex(shipModuleSlot2)));
			}
			TIShipArmorTemplate bestArmor = this.GetBestArmor(false);
			CS$<>8__locals1.fighterTemplate.SetNoseArmorTemplate(bestArmor.dataName);
			CS$<>8__locals1.fighterTemplate.SetLateralArmorTemplate(bestArmor.dataName);
			CS$<>8__locals1.fighterTemplate.SetTailArmorTemplate(bestArmor.dataName);
			if (flag)
			{
				TIFactionState.<>c__DisplayClass1123_0 CS$<>8__locals2 = CS$<>8__locals1;
				List<TIPowerPlantTemplate> list = new List<TIPowerPlantTemplate>();
				list.Add(this.allowedPowerPlants.MinBy<TIPowerPlantTemplate, float>((TIPowerPlantTemplate x) => x.maxOutput_GW));
				CS$<>8__locals2.powerPlants = list;
				CS$<>8__locals1.drives = new List<TIDriveTemplate> { TemplateManager.Find<TIDriveTemplate>("SuperKronosLiquidRocketx1", false) };
			}
			else
			{
				CS$<>8__locals1.powerPlants = this.allowedPowerPlants.Where<TIPowerPlantTemplate>((TIPowerPlantTemplate x) => x.exoFighterPart).ToList<TIPowerPlantTemplate>();
				CS$<>8__locals1.drives = this.allowedDrives.Where<TIDriveTemplate>((TIDriveTemplate x) => x.exoFighterPart).ToList<TIDriveTemplate>();
				if (CS$<>8__locals1.drives.Count == 0)
				{
					CS$<>8__locals1.drives.Add(TemplateManager.Find<TIDriveTemplate>("ApexSolidRocketx1", false));
				}
				if (CS$<>8__locals1.powerPlants.None<TIPowerPlantTemplate>((TIPowerPlantTemplate x) => x.powerPlantClass == PowerPlantRequirement.Solid_Core_Fission))
				{
					CS$<>8__locals1.drives.RemoveAll((TIDriveTemplate x) => x.requiredPowerPlant == PowerPlantRequirement.Solid_Core_Fission);
				}
				if (CS$<>8__locals1.powerPlants.Count == 0)
				{
					CS$<>8__locals1.powerPlants.Add(TemplateManager.Find<TIPowerPlantTemplate>("FuelCellI", false));
				}
			}
			bool flag2 = false;
			int num = 100;
			int num2 = 0;
			CS$<>8__locals1.fighterTemplate.TrySetArmor(ShipModuleSlotType.NoseArmor, 1);
			CS$<>8__locals1.<DesignSTOFighter>g__SetPropulsion|5();
			bool flag3 = false;
			while (!flag2 && num2 < num)
			{
				if (CS$<>8__locals1.fighterTemplate.baseCombatAcceleration_gs < 1f && CS$<>8__locals1.drives.Count > 1)
				{
					CS$<>8__locals1.drives.Remove(CS$<>8__locals1.drive);
					CS$<>8__locals1.fighterTemplate.TrySetArmor(ShipModuleSlotType.NoseArmor, 1);
					CS$<>8__locals1.fighterTemplate.TrySetArmor(ShipModuleSlotType.LateralArmor, 0);
					CS$<>8__locals1.<DesignSTOFighter>g__SetPropulsion|5();
					flag3 = false;
				}
				float num3 = CS$<>8__locals1.fighterTemplate.baseCruiseDeltaV_kps(true);
				if (CS$<>8__locals1.fighterTemplate.wetMass_tons <= this.desiredSTOWetMass_tons)
				{
					if (!flag3 && num3 > 6f && (float)(CS$<>8__locals1.fighterTemplate.noseArmorValue / (CS$<>8__locals1.fighterTemplate.lateralArmorValue + 1)) >= 5f)
					{
						flag3 = CS$<>8__locals1.fighterTemplate.TryAddArmorPoints(ShipModuleSlotType.LateralArmor, 1) <= 0;
						if (CS$<>8__locals1.fighterTemplate.baseCruiseDeltaV_kps(true) < 2f || CS$<>8__locals1.fighterTemplate.baseCombatAcceleration_gs < 1f)
						{
							CS$<>8__locals1.fighterTemplate.TryAddArmorPoints(ShipModuleSlotType.LateralArmor, -1);
							flag3 = true;
						}
					}
					else if (num3 > 3f)
					{
						flag2 = CS$<>8__locals1.fighterTemplate.TryAddArmorPoints(ShipModuleSlotType.NoseArmor, 1) <= 0;
					}
					else if (num3 < 2f)
					{
						CS$<>8__locals1.fighterTemplate.propellantTanks++;
					}
					else
					{
						flag2 = true;
					}
				}
				else if (CS$<>8__locals1.fighterTemplate.lateralArmor.armorValue > 0)
				{
					CS$<>8__locals1.fighterTemplate.TryAddArmorPoints(ShipModuleSlotType.LateralArmor, -1);
				}
				else if (CS$<>8__locals1.fighterTemplate.noseArmor.armorValue > 1)
				{
					CS$<>8__locals1.fighterTemplate.TryAddArmorPoints(ShipModuleSlotType.NoseArmor, -1);
				}
				else if (CS$<>8__locals1.drives.Count == 1 || CS$<>8__locals1.fighterTemplate.baseCombatAcceleration_gs >= 1f)
				{
					flag2 = true;
				}
				num2++;
			}
			return CS$<>8__locals1.fighterTemplate;
		}

		// Token: 0x0600341A RID: 13338 RVA: 0x00125CB0 File Offset: 0x00123EB0
		public void CacheSTOFighterMass()
		{
			this.cachedSTOFighterMinimumBoost = float.PositiveInfinity;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.AllowedFighterHullWeapons())
			{
				float num = this.DesignSTOFighter(this.IsAlienFaction ? GameStateManager.AlienNation() : null, tishipWeaponTemplate).wetMass_tons * TemplateManager.global.spaceResourceToTons;
				if (num < this.cachedSTOFighterMinimumBoost)
				{
					this.cachedSTOFighterMinimumBoost = num;
				}
			}
		}

		// Token: 0x0600341B RID: 13339 RVA: 0x00125D40 File Offset: 0x00123F40
		public bool CanTradeAwayResource(FactionResource resource, TIFactionState otherFaction)
		{
			return this.UnlockedResource(resource) && otherFaction.UnlockedResource(resource);
		}

		// Token: 0x0600341C RID: 13340 RVA: 0x00125D54 File Offset: 0x00123F54
		public TradeOffer InitializeTradingOptions(TIFactionState otherFaction)
		{
			TradeOffer tradeOffer = new TradeOffer(this);
			foreach (FactionResource factionResource in TIResourcesCost.tradeableResources)
			{
				if (this.CanTradeAwayResource(factionResource, otherFaction))
				{
					tradeOffer.resourceValues.Add(new ResourceValue(factionResource, 0f));
				}
			}
			foreach (TIOrgState tiorgState in this.GetAllOrgs())
			{
				if (this.CanTradeOrg(tiorgState, otherFaction))
				{
					tradeOffer.orgs.Add(tiorgState);
				}
			}
			foreach (TIProjectTemplate tiprojectTemplate in this.completedProjects)
			{
				if (this.CanTradeProject(tiprojectTemplate, otherFaction))
				{
					tradeOffer.projects.Add(tiprojectTemplate);
				}
			}
			return tradeOffer;
		}

		// Token: 0x0600341D RID: 13341 RVA: 0x00125E70 File Offset: 0x00124070
		public void ProcessTrade(TradeOffer acceptedOffer, float tradeHateModifier, TIFactionState otherFaction, bool originalContactingFaction)
		{
			StringBuilder stringBuilder = new StringBuilder("Processed Trade Values: ");
			stringBuilder.AppendLine("Trade Offer from: " + otherFaction.displayName + " to " + this.displayName);
			List<ResourceValue> resourceValues = acceptedOffer.resourceValues;
			if (resourceValues != null && resourceValues.Count > 0)
			{
				foreach (ResourceValue resourceValue in acceptedOffer.resourceValues)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string text = resourceValue.resource.ToString();
					string text2 = ",";
					float value = resourceValue.value;
					stringBuilder2.AppendLine(text + text2 + value.ToString());
				}
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				tiresourcesCost.ConstructCost(acceptedOffer.resourceValues.ToArray());
				tiresourcesCost.PayCost(acceptedOffer.offeringFaction, "Trade Debit");
				tiresourcesCost.RefundCost(this, "Trade Credit");
			}
			foreach (TIProjectTemplate tiprojectTemplate in acceptedOffer.projects)
			{
				if (this.AddAvailableProject(tiprojectTemplate, null))
				{
					stringBuilder.AppendLine("Project-" + tiprojectTemplate.dataName);
				}
			}
			foreach (TIOrgState tiorgState in acceptedOffer.orgs)
			{
				acceptedOffer.offeringFaction.LoseOrg(tiorgState);
				this.AddOrgToFactionPool(tiorgState, null, true);
				stringBuilder.AppendLine("Org-" + tiorgState.displayName);
			}
			foreach (TIControlPoint ticontrolPoint in acceptedOffer.controlPoints)
			{
				ticontrolPoint.nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Trade, this);
				stringBuilder.AppendLine("Control Point-" + ticontrolPoint.displayName);
			}
			foreach (TISectorState tisectorState in acceptedOffer.habSectors)
			{
				tisectorState.SetFaction(this);
				stringBuilder.AppendLine("HabSector-" + tisectorState.displayName);
			}
			foreach (TIHabState tihabState in acceptedOffer.habs)
			{
				tihabState.CaptureHab(this, 5, true, false, null, null);
				stringBuilder.AppendLine("Hab-" + tihabState.displayName);
			}
			if (this.player.isAI && acceptedOffer.treatyType == TradeOffer.TreatyType.NAP)
			{
				this.AddGoal(new FactionGoal_NonAggressionPact(this, 4, acceptedOffer.offeringFaction), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
			else if (this.player.isAI && acceptedOffer.treatyType == TradeOffer.TreatyType.Truce)
			{
				this.AddGoal(new FactionGoal_TruceWithFaction(this, 3, acceptedOffer.offeringFaction), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
			if (acceptedOffer.treatyType == TradeOffer.TreatyType.Intel)
			{
				this.BeginIntelSharingWith(otherFaction);
			}
			if (acceptedOffer.intelExchange)
			{
				this.GiveIntelToFaction(otherFaction, false);
			}
			this.GainFactionHate(otherFaction, (TemplateManager.global.factionHateForTradeTreaty + TemplateManager.global.factionHateForTrade) * tradeHateModifier, false, "Trade", true);
			if (this.IsAlienFaction)
			{
				otherFaction.FixAssessedAlienHateToActualValue();
			}
			else if (otherFaction.IsAlienFaction)
			{
				this.FixAssessedAlienHateToActualValue();
			}
			TIFactionState.LogAI(stringBuilder.ToString(), false);
		}

		// Token: 0x0600341E RID: 13342 RVA: 0x00126228 File Offset: 0x00124428
		public bool WillingToTrade(TIFactionState otherFaction)
		{
			return Mathf.Abs(this.ideologyCoordinates.x - otherFaction.ideologyCoordinates.x) < 3f || this.malleable;
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x00126258 File Offset: 0x00124458
		public bool MayTradeAwayHab(TIHabState hab, TIFactionState receivingFaction)
		{
			return !this.IsAlienFaction && !receivingFaction.IsAlienFaction && receivingFaction.CanExplore(hab) && !hab.decommissioning && hab.ShipsBeingBuiltAtHab(this).Count <= 0 && hab.CompletedModules().Count != 0 && hab.dockedFleets.Count <= 0;
		}

		// Token: 0x06003420 RID: 13344 RVA: 0x001262B3 File Offset: 0x001244B3
		public bool AI_ShouldNotAcquireHabInTrade(TIHabState hab)
		{
			return !this.isActivePlayer && hab.NetPower(true, true) < 0;
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x001262CD File Offset: 0x001244CD
		public bool AI_ShouldNotTradeAwayHab(TIHabState hab)
		{
			bool isActivePlayer = this.isActivePlayer;
			return false;
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x001262D8 File Offset: 0x001244D8
		public bool CanTradeProject(TIProjectTemplate project, TIFactionState factionToTradeTo)
		{
			return !this.IsAlienFaction && !factionToTradeTo.IsAlienFaction && (project.PrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), factionToTradeTo.completedProjects, factionToTradeTo) && project.techCategory != TechCategory.Xenology && !factionToTradeTo.availableProjects.Contains(project) && !factionToTradeTo.completedProjects.Contains(project) && !project.repeatable);
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x0012633B File Offset: 0x0012453B
		public bool CanTradeOrg(TIOrgState org, TIFactionState factionToTradeTo)
		{
			if (org.IsEligibleForFaction(factionToTradeTo))
			{
				TICouncilorState assignedCouncilor = org.assignedCouncilor;
				return assignedCouncilor == null || assignedCouncilor.CanRemoveOrg(org);
			}
			return false;
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x0012635C File Offset: 0x0012455C
		public bool HasNAP(TIFactionState otherFaction, bool includeToBeDiscarded = true)
		{
			if (this == otherFaction || otherFaction == null)
			{
				return false;
			}
			if (includeToBeDiscarded)
			{
				return otherFaction.FindGoals(GoalType.NonAggressionPact, otherFaction, this, TIFactionState.GoalFilter.none, true).Count > 0 || this.FindGoals(GoalType.NonAggressionPact, this, otherFaction, TIFactionState.GoalFilter.none, true).Count > 0;
			}
			if (otherFaction.FindGoals(GoalType.NonAggressionPact, otherFaction, this, TIFactionState.GoalFilter.none, true).Count<TIFactionGoalState>((TIFactionGoalState x) => x.importance > 0) <= 0)
			{
				return this.FindGoals(GoalType.NonAggressionPact, this, otherFaction, TIFactionState.GoalFilter.none, true).Count<TIFactionGoalState>((TIFactionGoalState x) => x.importance > 0) > 0;
			}
			return true;
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x00126414 File Offset: 0x00124614
		public bool HasTruce(TIFactionState otherFaction, bool includeToBeDiscarded = true)
		{
			if (this == otherFaction || otherFaction == null)
			{
				return false;
			}
			if (includeToBeDiscarded)
			{
				return otherFaction.FindGoals(GoalType.TruceWithFaction, otherFaction, this, TIFactionState.GoalFilter.none, true).Count > 0 || this.FindGoals(GoalType.TruceWithFaction, this, otherFaction, TIFactionState.GoalFilter.none, true).Count > 0;
			}
			if (otherFaction.FindGoals(GoalType.TruceWithFaction, otherFaction, this, TIFactionState.GoalFilter.none, true).Count<TIFactionGoalState>((TIFactionGoalState x) => x.importance > 0) <= 0)
			{
				return this.FindGoals(GoalType.TruceWithFaction, this, otherFaction, TIFactionState.GoalFilter.none, true).Count<TIFactionGoalState>((TIFactionGoalState x) => x.importance > 0) > 0;
			}
			return true;
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x001264CC File Offset: 0x001246CC
		public bool CanTradeNAP(TIFactionState otherFaction)
		{
			bool flag = this.HasNAP(otherFaction, true);
			return !otherFaction.permanentAlly(this) && !otherFaction.AI_AtWarWithFaction(this) && !this.AI_AtWarWithFaction(otherFaction) && this.mostPowerfulHumanEnemy != otherFaction && otherFaction.mostPowerfulHumanEnemy != this && !flag && (!this.IsAlienFaction || otherFaction.veryProAlien) && (!otherFaction.IsAlienFaction || this.veryProAlien) && new FactionGoal_NonAggressionPact(this, 4, otherFaction).ValidNewGoal() && AIDailyFactionPlanner.JealousyAndDeescalation(this, otherFaction, false, false) <= 0f;
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x00126560 File Offset: 0x00124760
		public string NoNAPTradeFeedback(TIFactionState otherFaction, bool includeAILogic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (otherFaction.AI_AtWarWithFaction(this))
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoNAP_War"));
			}
			if (this.mostPowerfulHumanEnemy == this)
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoNAP_MostPowerfulEnemy"));
			}
			if (otherFaction.IsAlienFaction && !this.veryProAlien)
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoNAP_NotProAlien"));
			}
			if (otherFaction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked).Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoNAP_PursuingVictory"));
			}
			if (AIDailyFactionPlanner.JealousyAndDeescalation(this, otherFaction, false, false) > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoNAP_Jealousy"));
			}
			if (includeAILogic && AIEvaluators.GetWillingnessToTradeNAP(this, otherFaction, false) <= 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoNAP_WontPropose"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003428 RID: 13352 RVA: 0x00126634 File Offset: 0x00124834
		public string NoIntelFeedback(TIFactionState otherFaction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (TINationState.GetIdeologicalDistance(this, otherFaction) > 1.12f || (!this.malleable && ((this.proAlien && !otherFaction.proAlien) || (this.antiAlien && !otherFaction.antiAlien))))
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoIntel_Never"));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoIntel_ForNow"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003429 RID: 13353 RVA: 0x001266A8 File Offset: 0x001248A8
		public string NoTruceFeedback(TIFactionState otherFaction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (AIDailyFactionPlanner.JealousyAndDeescalation(this, otherFaction, false, false) > 0f && !AIEvaluators.HumanFactionTooBeatDownToContinue(this, otherFaction))
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoTruce_Jealousy"));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.Diplomacy.NoTruce_WontPropose"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600342A RID: 13354 RVA: 0x001266FE File Offset: 0x001248FE
		public bool CanTradeTruce(TIFactionState otherFaction)
		{
			return !otherFaction.permanentAlly(this) && (this.FindGoals(GoalType.WarOnFaction, this, otherFaction, TIFactionState.GoalFilter.none, true).Count > 0 || otherFaction.FindGoals(GoalType.WarOnFaction, otherFaction, this, TIFactionState.GoalFilter.none, true).Count > 0);
		}

		// Token: 0x0600342B RID: 13355 RVA: 0x00126738 File Offset: 0x00124938
		public bool CanTradeIntelSharing(TIFactionState otherFaction, bool ignoreExistingAgreement = false)
		{
			bool flag = this.intelSharingFactions.Contains(otherFaction);
			bool flag2 = this.turnedCouncilors.None<TICouncilorState>((TICouncilorState x) => x.faction == otherFaction);
			bool flag3 = this.HasNAP(otherFaction, false);
			bool flag4 = otherFaction.IsAlienFaction && !this.veryProAlien;
			bool flag5 = this.GetFactionHate(otherFaction) < TemplateManager.global.factionHateConflictThreshold;
			return (ignoreExistingAgreement || !flag) && flag5 && flag2 && !flag4 && (flag3 || this.permanentAlly(otherFaction));
		}

		// Token: 0x0600342C RID: 13356 RVA: 0x001267E7 File Offset: 0x001249E7
		public bool CanTradeTreaty(TIFactionState otherFaction, TradeOffer.TreatyType treatyType)
		{
			switch (treatyType)
			{
			case TradeOffer.TreatyType.Truce:
				return this.CanTradeTruce(otherFaction);
			case TradeOffer.TreatyType.NAP:
				return this.CanTradeNAP(otherFaction);
			case TradeOffer.TreatyType.Intel:
				return this.CanTradeIntelSharing(otherFaction, false);
			default:
				return false;
			}
		}

		// Token: 0x0600342D RID: 13357 RVA: 0x0012681C File Offset: 0x00124A1C
		public string DiplomacyGreetingMessage(TIFactionState otherFaction, bool forceWar)
		{
			string text = "War";
			float num = this.GetFactionHate(otherFaction);
			bool flag = this.WillingToTrade(otherFaction);
			if (!forceWar)
			{
				if (flag && num < TemplateManager.global.factionHateConflictThreshold)
				{
					text = "Tolerance";
				}
				else if (flag && num >= TemplateManager.global.factionHateConflictThreshold && num <= TemplateManager.global.factionHateWarThreshold)
				{
					text = "Conflict";
				}
				else if (!flag || num > TemplateManager.global.factionHateWarThreshold)
				{
					text = "War";
				}
			}
			string text2 = new StringBuilder(this.ideology.ideology.ToString()).Append(".").Append(otherFaction.ideology.ideology.ToString()).Append(".")
				.Append(text)
				.ToString();
			string text3 = Loc.T("TIFactionTemplate.Diplomacy." + text2.ToString());
			return new StringBuilder(this.leaderNameWithAddress).Append(": ").Append("\"").Append(text3)
				.Append("\"")
				.ToString();
		}

		// Token: 0x0600342E RID: 13358 RVA: 0x00126938 File Offset: 0x00124B38
		public string GetDiplomacyMood(TIFactionState otherFaction)
		{
			string text = "War";
			float num = this.GetFactionHate(otherFaction);
			bool flag = this.WillingToTrade(otherFaction);
			if (flag && num < TemplateManager.global.factionHateConflictThreshold)
			{
				text = "Tolerance";
			}
			else if (flag && num >= TemplateManager.global.factionHateConflictThreshold && num <= TemplateManager.global.factionHateWarThreshold)
			{
				text = "Conflicted";
			}
			else if (!flag || num > TemplateManager.global.factionHateWarThreshold)
			{
				text = "War";
			}
			return text;
		}

		// Token: 0x0600342F RID: 13359 RVA: 0x001269B0 File Offset: 0x00124BB0
		public void CommitAtrocity(int numAtrocities, TIFactionState.AtrocityCause cause, bool propagandaHitWhenZero = false, float multiplier = 0.333f)
		{
			this.atrocities += numAtrocities;
			if (!this.numAtrocitiesByCause.ContainsKey(cause))
			{
				this.numAtrocitiesByCause.Add(cause, 0);
			}
			Dictionary<TIFactionState.AtrocityCause, int> dictionary = this.numAtrocitiesByCause;
			dictionary[cause] += numAtrocities;
			if (numAtrocities > 0 || (numAtrocities == 0 && propagandaHitWhenZero))
			{
				float num = (float)(-(float)this.atrocities) * multiplier * TIGlobalConfig.globalConfig.atrocityPOMultiplier;
				if (num != 0f)
				{
					TINationState.GlobalPropaganda(this.ideology, num + TIEffectsState.SumEffectsModifiers(Context.AtrocityMitigation, this, num, null));
				}
			}
			if (numAtrocities > 0)
			{
				TITraitTemplate.ProcessLoyaltyChangeFromTraits(this, SpecialTraitRule.LoyaltyLossOnFactionAtrocity, 1);
			}
		}

		// Token: 0x06003430 RID: 13360 RVA: 0x00126A50 File Offset: 0x00124C50
		public string AtrocityCauseTable()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.Intel.Atrocity.Header"));
			foreach (TIFactionState.AtrocityCause atrocityCause in this.numAtrocitiesByCause.Keys.OrderByDescending<TIFactionState.AtrocityCause, int>((TIFactionState.AtrocityCause x) => this.numAtrocitiesByCause[x]))
			{
				stringBuilder.AppendLine(Loc.T(new StringBuilder("UI.Intel.Atrocity.").Append(atrocityCause).ToString(), new object[] { this.numAtrocitiesByCause[atrocityCause].ToString("N0") }));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003431 RID: 13361 RVA: 0x00126B14 File Offset: 0x00124D14
		public void SetNotificationPreference(string notificationTemplateDataName, int notificationType, NotificationOverrideBehavior overrideBehavior)
		{
			if (!this.notificationOverrides.ContainsKey(notificationTemplateDataName))
			{
				this.notificationOverrides.Add(notificationTemplateDataName, new TINotificationTemplateOverride());
			}
			switch (notificationType)
			{
			case 0:
				this.notificationOverrides[notificationTemplateDataName].alert = overrideBehavior;
				return;
			case 1:
				this.notificationOverrides[notificationTemplateDataName].timerFeed = overrideBehavior;
				return;
			case 2:
				this.notificationOverrides[notificationTemplateDataName].newsFeed = overrideBehavior;
				return;
			case 3:
				this.notificationOverrides[notificationTemplateDataName].summaryFeed = overrideBehavior;
				return;
			default:
				return;
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x06003432 RID: 13362 RVA: 0x00126BA2 File Offset: 0x00124DA2
		public static TIMissionTemplate assassinateMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Assassinate", false);
			}
		}

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x06003433 RID: 13363 RVA: 0x00126BAF File Offset: 0x00124DAF
		public static TIMissionTemplate detainMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Detain", false);
			}
		}

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x06003434 RID: 13364 RVA: 0x00126BBC File Offset: 0x00124DBC
		public static TIMissionTemplate assaultAlienAssetMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("AssaultAlienAsset", false);
			}
		}

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x06003435 RID: 13365 RVA: 0x00126BC9 File Offset: 0x00124DC9
		public static TIMissionTemplate surveilMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("DetectCouncilActivity", false);
			}
		}

		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x06003436 RID: 13366 RVA: 0x00126BD6 File Offset: 0x00124DD6
		public static TIMissionTemplate investigateMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("InvestigateCouncilor", false);
			}
		}

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x06003437 RID: 13367 RVA: 0x00126BE3 File Offset: 0x00124DE3
		public static TIMissionTemplate setPolicyMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("SetNationalPolicy", false);
			}
		}

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x06003438 RID: 13368 RVA: 0x00126BF0 File Offset: 0x00124DF0
		public static TIMissionTemplate orbitMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Orbit", false);
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x06003439 RID: 13369 RVA: 0x00126BFD File Offset: 0x00124DFD
		public static TIMissionTemplate transferMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Transfer", false);
			}
		}

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x0600343A RID: 13370 RVA: 0x00126C0A File Offset: 0x00124E0A
		public static TIMissionTemplate deorbitMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Deorbit", false);
			}
		}

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x0600343B RID: 13371 RVA: 0x00126C17 File Offset: 0x00124E17
		public static TIMissionTemplate defendInterestsMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("DefendInterests", false);
			}
		}

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x0600343C RID: 13372 RVA: 0x00126C24 File Offset: 0x00124E24
		public static TIMissionTemplate seizeHabMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("SeizeSpaceAsset", false);
			}
		}

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x0600343D RID: 13373 RVA: 0x00126C31 File Offset: 0x00124E31
		public static TIMissionTemplate protectMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Protect", false);
			}
		}

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x0600343E RID: 13374 RVA: 0x00126C3E File Offset: 0x00124E3E
		public static TIMissionTemplate controlHabMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("ControlSpaceAsset", false);
			}
		}

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x0600343F RID: 13375 RVA: 0x00126C4B File Offset: 0x00124E4B
		public static TIMissionTemplate abductionsMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Abductions", false);
			}
		}

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x06003440 RID: 13376 RVA: 0x00126C58 File Offset: 0x00124E58
		public static TIMissionTemplate terrorizeMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("TerrorizeRegion", false);
			}
		}

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x06003441 RID: 13377 RVA: 0x00126C65 File Offset: 0x00124E65
		public static TIMissionTemplate enthrallElitesMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("EnthrallElites", false);
			}
		}

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x06003442 RID: 13378 RVA: 0x00126C72 File Offset: 0x00124E72
		public static TIMissionTemplate enthrallPublicMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("EnthrallPublic", false);
			}
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x06003443 RID: 13379 RVA: 0x00126C7F File Offset: 0x00124E7F
		public static TIMissionTemplate enthrallNonAlignedElitesMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("EnthrallUnalignedElites", false);
			}
		}

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x06003444 RID: 13380 RVA: 0x00126C8C File Offset: 0x00124E8C
		public static TIMissionTemplate enthrallOrgMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("EnthrallOrg", false);
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x06003445 RID: 13381 RVA: 0x00126C99 File Offset: 0x00124E99
		public static TIMissionTemplate xenoformMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Xenoform", false);
			}
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x06003446 RID: 13382 RVA: 0x00126CA6 File Offset: 0x00124EA6
		public static TIMissionTemplate purgeMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Purge", false);
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x06003447 RID: 13383 RVA: 0x00126CB3 File Offset: 0x00124EB3
		public static TIMissionTemplate crackdownMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Crackdown", false);
			}
		}

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x06003448 RID: 13384 RVA: 0x00126CC0 File Offset: 0x00124EC0
		public static TIMissionTemplate hostileTakeoverMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("HostileTakeover", false);
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x06003449 RID: 13385 RVA: 0x00126CCD File Offset: 0x00124ECD
		public static TIMissionTemplate stealProjectMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("StealProject", false);
			}
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x0600344A RID: 13386 RVA: 0x00126CDA File Offset: 0x00124EDA
		public static TIMissionTemplate sabotageProjectMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("SabotageProject", false);
			}
		}

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x0600344B RID: 13387 RVA: 0x00126CE7 File Offset: 0x00124EE7
		public static TIMissionTemplate grantNationMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("AssumeControl", false);
			}
		}

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x0600344C RID: 13388 RVA: 0x00126CF4 File Offset: 0x00124EF4
		public static TIMissionTemplate publicCampaignMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Propaganda", false);
			}
		}

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x0600344D RID: 13389 RVA: 0x00126D01 File Offset: 0x00124F01
		public static TIMissionTemplate contactMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Contact", false);
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x0600344E RID: 13390 RVA: 0x00126D0E File Offset: 0x00124F0E
		public static TIMissionTemplate adviseMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Advise", false);
			}
		}

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x0600344F RID: 13391 RVA: 0x00126D1B File Offset: 0x00124F1B
		public static TIMissionTemplate goToGroundMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("GoToGround", false);
			}
		}

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x06003450 RID: 13392 RVA: 0x00126D28 File Offset: 0x00124F28
		public static TIMissionTemplate controlNationMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("GainInfluence", false);
			}
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x06003451 RID: 13393 RVA: 0x00126D35 File Offset: 0x00124F35
		public static TIMissionTemplate coupMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Coup", false);
			}
		}

		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06003452 RID: 13394 RVA: 0x00126D42 File Offset: 0x00124F42
		public static TIMissionTemplate inspireMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Inspire", false);
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06003453 RID: 13395 RVA: 0x00126D4F File Offset: 0x00124F4F
		public static TIMissionTemplate turnMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Turn", false);
			}
		}

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x06003454 RID: 13396 RVA: 0x00126D5C File Offset: 0x00124F5C
		public static TIMissionTemplate sabotageSpaceFacilityMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("SabotageFacilities", false);
			}
		}

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x06003455 RID: 13397 RVA: 0x00126D69 File Offset: 0x00124F69
		public static TIMissionTemplate sabotageHabModuleMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("SabotageHabModule", false);
			}
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06003456 RID: 13398 RVA: 0x00126D76 File Offset: 0x00124F76
		public static TIMissionTemplate unrestMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Unrest", false);
			}
		}

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x06003457 RID: 13399 RVA: 0x00126D83 File Offset: 0x00124F83
		public static TIMissionTemplate stabilizeMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("Stabilize", false);
			}
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x06003458 RID: 13400 RVA: 0x00126D90 File Offset: 0x00124F90
		public static TIMissionTemplate passTechnologyMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("PassTechnology", false);
			}
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x06003459 RID: 13401 RVA: 0x00126D9D File Offset: 0x00124F9D
		public static TIMissionTemplate buildFacilityMission
		{
			get
			{
				return TemplateManager.Find<TIMissionTemplate>("BuildFacility", false);
			}
		}

		// Token: 0x0600345A RID: 13402 RVA: 0x00126DAC File Offset: 0x00124FAC
		public Dictionary<TIOrgState, TICouncilorState> ProposeOptimizedCriticalOrgMissions(out List<TIMissionTemplate> missionsNotFound, List<TIMissionTemplate> criticalMissions = null)
		{
			if (criticalMissions == null)
			{
				criticalMissions = this.ObjectiveCriticalMissions();
			}
			missionsNotFound = new List<TIMissionTemplate>(criticalMissions);
			Dictionary<TIOrgState, TICouncilorState> dictionary = new Dictionary<TIOrgState, TICouncilorState>();
			using (IEnumerator<TIMissionTemplate> enumerator = criticalMissions.Distinct<TIMissionTemplate>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionTemplate mission = enumerator.Current;
					List<TIMissionTemplate> list = new List<TIMissionTemplate>(criticalMissions);
					list.Remove(mission);
					CouncilorAttribute stat = mission.primaryAttackerStat;
					if (stat != CouncilorAttribute.None)
					{
						IEnumerable<TICouncilorState> enumerable = this.councilors.Where<TICouncilorState>((TICouncilorState x) => !x.RestrictedMissions().Contains(mission));
						TICouncilorState bestCouncilor = null;
						if (enumerable.Any<TICouncilorState>())
						{
							bestCouncilor = enumerable.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(stat, true, true, true, false, false, false));
							if (!bestCouncilor.GetPossibleMissionList(false, false, true, null, false).Contains(mission))
							{
								List<TIOrgState> list2 = new List<TIOrgState>();
								list2.AddRange(this.unassignedOrgs);
								list2.AddRange(this.availableOrgs);
								if (list2.None<TIOrgState>((TIOrgState x) => x.missionsGranted.Contains(mission)))
								{
									list2 = (from x in this.councilors.Where<TICouncilorState>((TICouncilorState x) => x != bestCouncilor).SelectMany<TICouncilorState, TIOrgState>((TICouncilorState x) => x.orgs).Except<TIOrgState>(dictionary.Keys)
										where x.missionsGranted.Contains(mission)
										select x).ToList<TIOrgState>();
								}
								else
								{
									list2 = (from x in list2.Except<TIOrgState>(dictionary.Keys)
										where x.missionsGranted.Contains(mission)
										select x).ToList<TIOrgState>();
								}
								list2 = (from x in list2
									orderby x.GetPurchaseOrTransferCost(this).CanAfford(this, 1f, null, float.PositiveInfinity) descending, x.GetStatBonus(stat) descending, x.tier
									select x).ToList<TIOrgState>();
								using (List<TIOrgState>.Enumerator enumerator2 = list2.GetEnumerator())
								{
									if (enumerator2.MoveNext())
									{
										TIOrgState org = enumerator2.Current;
										bool flag = true;
										if (org.hasCouncilor)
										{
											List<TIMissionTemplate> list3 = (from x in list.Intersect<TIMissionTemplate>(org.missionsGranted)
												where org.assignedCouncilor.OrgGrantingMission(x, true) == org && x.primaryAttackerStat > CouncilorAttribute.None
												select x).ToList<TIMissionTemplate>();
											if (list3.Any<TIMissionTemplate>())
											{
												using (List<TIMissionTemplate>.Enumerator enumerator3 = list3.GetEnumerator())
												{
													while (enumerator3.MoveNext())
													{
														TIMissionTemplate confoundingMission = enumerator3.Current;
														IEnumerable<TICouncilorState> enumerable2 = this.councilors.Where<TICouncilorState>((TICouncilorState x) => !x.RestrictedMissions().Contains(confoundingMission));
														if (enumerable2.Any<TICouncilorState>() && enumerable2.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(confoundingMission.primaryAttackerStat, true, true, true, false, false, false)) == org.assignedCouncilor)
														{
															flag = false;
														}
													}
												}
											}
										}
										if (flag)
										{
											dictionary.Add(org, bestCouncilor);
											missionsNotFound.Remove(mission);
										}
									}
								}
							}
						}
					}
				}
			}
			return dictionary;
		}

		// Token: 0x0600345B RID: 13403 RVA: 0x00127144 File Offset: 0x00125344
		public List<TIMissionTemplate> ObjectiveCriticalMissions()
		{
			List<TIMissionTemplate> list = (from x in this.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked)
				where !string.IsNullOrEmpty(x.targetMissionTemplateName)
				select x.targetMissionTemplate).ToList<TIMissionTemplate>();
			List<CampaignMilestone> list2 = this.DesiredMilestones();
			if (list2.Contains(CampaignMilestone.AccessHydraCorpus))
			{
				list.Add(TIFactionState.surveilMission);
				list.Add(TIFactionState.investigateMission);
				list.Add(TIFactionState.assassinateMission);
			}
			if (list2.Contains(CampaignMilestone.AccessLiveHydra))
			{
				list.Add(TIFactionState.surveilMission);
				list.Add(TIFactionState.investigateMission);
				list.Add(TIFactionState.detainMission);
			}
			if (list2.Contains(CampaignMilestone.AlienDiplomacy))
			{
				list.Add(TIFactionState.surveilMission);
				list.Add(TIFactionState.investigateMission);
			}
			if (list2.Contains(CampaignMilestone.AccessAlienTech) || list2.Contains(CampaignMilestone.AccessAlienShip))
			{
				list.Add(TIFactionState.surveilMission);
				list.Add(TIFactionState.assaultAlienAssetMission);
			}
			return list.Distinct<TIMissionTemplate>().ToList<TIMissionTemplate>();
		}

		// Token: 0x0600345C RID: 13404 RVA: 0x00127258 File Offset: 0x00125458
		public List<TIMissionTemplate> RequiredMissions(bool includeCriticals = true)
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>
			{
				TIFactionState.controlNationMission,
				TIFactionState.crackdownMission,
				TIFactionState.purgeMission,
				TIFactionState.defendInterestsMission,
				TIFactionState.defendInterestsMission,
				TIFactionState.publicCampaignMission,
				TIFactionState.surveilMission,
				TIFactionState.investigateMission,
				TIFactionState.inspireMission
			};
			if (includeCriticals)
			{
				list.AddRange(this.ObjectiveCriticalMissions());
			}
			return list;
		}

		// Token: 0x0600345D RID: 13405 RVA: 0x001272E0 File Offset: 0x001254E0
		public List<TIMissionTemplate> MissingRequiredMissions(List<TIMissionTemplate> requiredMissions = null)
		{
			if (requiredMissions == null)
			{
				requiredMissions = this.RequiredMissions(true);
			}
			List<TIMissionTemplate> list = new List<TIMissionTemplate>(requiredMissions);
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				foreach (TIMissionTemplate timissionTemplate in ticouncilorState.GetPossibleMissionList(false, false, true, null, false))
				{
					list.Remove(timissionTemplate);
				}
			}
			return list.ToList<TIMissionTemplate>();
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x0600345E RID: 13406 RVA: 0x0012738C File Offset: 0x0012558C
		public List<TIFactionState> enemyWarFactions
		{
			get
			{
				return (from x in this.GoalsOfType(GoalType.WarOnFaction, false, true)
					select x.target().ref_faction).ToList<TIFactionState>();
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x0600345F RID: 13407 RVA: 0x001273C4 File Offset: 0x001255C4
		public List<TIFactionState> enemyTotalWarFactions
		{
			get
			{
				return (from x in this.GoalsOfType(GoalType.WarOnFaction, false, true)
					where (x as FactionGoal_WarOnFaction).IsTotalWar
					select x.target().ref_faction).ToList<TIFactionState>();
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06003460 RID: 13408 RVA: 0x00127428 File Offset: 0x00125628
		public List<TIFactionState> factionsAtWarWithMe
		{
			get
			{
				return (from x in GameStateManager.AllFactions()
					where x.GoalsOfType(GoalType.WarOnFaction, false, true).Any<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_faction == this)
					select x).ToList<TIFactionState>();
			}
		}

		// Token: 0x06003461 RID: 13409 RVA: 0x00127445 File Offset: 0x00125645
		public bool IsInTotalWarWithFaction(TIFactionState enemy)
		{
			return this.enemyTotalWarFactions.Contains(enemy);
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06003462 RID: 13410 RVA: 0x00127453 File Offset: 0x00125653
		public static bool AIFullDump
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003463 RID: 13411 RVA: 0x00127456 File Offset: 0x00125656
		public static void LogAI(string logEntry, bool fullDumpOnly = false)
		{
		}

		// Token: 0x06003464 RID: 13412 RVA: 0x00127458 File Offset: 0x00125658
		public static void DumpGoals(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder(faction.displayName + " Objectives and Goals\n");
			foreach (TIObjectiveTemplate tiobjectiveTemplate in faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked))
			{
				stringBuilder.AppendLine(tiobjectiveTemplate.displayName(faction));
			}
			foreach (List<TIFactionGoalState> list in faction.factionGoals.Values)
			{
				foreach (TIFactionGoalState tifactionGoalState in list)
				{
					stringBuilder.AppendLine(tifactionGoalState.description);
				}
			}
			TIFactionState.LogAI(stringBuilder.ToString(), false);
		}

		// Token: 0x06003465 RID: 13413 RVA: 0x0012755C File Offset: 0x0012575C
		public static void DumpShipyards(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder(TITimeState.Now().ToCustomDateString() + " " + faction.displayName + " Shipyard Queues\n");
			foreach (TIHabModuleState tihabModuleState in faction.nShipyardQueues.Keys)
			{
				stringBuilder.AppendLine(tihabModuleState.hab.displayName + " (" + tihabModuleState.hab.ref_naturalSpaceObject.displayName + "):");
				int num = 1;
				foreach (ShipConstructionQueueItem shipConstructionQueueItem in faction.nShipyardQueues[tihabModuleState])
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string[] array = new string[18];
					array[0] = " ";
					array[1] = num.ToString();
					array[2] = " ";
					array[3] = shipConstructionQueueItem.shipDesign.fullClassName;
					array[4] = "/";
					array[5] = shipConstructionQueueItem.shipDesign.roleStr;
					array[6] = "  PAID: ";
					array[7] = shipConstructionQueueItem.costPaid.ToString();
					array[8] = " D2C: ";
					array[9] = shipConstructionQueueItem.daysToCompletion.ToString();
					array[10] = " GOAL: ";
					int num2 = 11;
					FactionGoal_Fleet aifactionGoal = shipConstructionQueueItem.AIFactionGoal;
					array[num2] = ((aifactionGoal != null) ? new GameStateID?(aifactionGoal.ID) : null).ToString();
					array[12] = " ";
					int num3 = 13;
					FactionGoal_Fleet aifactionGoal2 = shipConstructionQueueItem.AIFactionGoal;
					array[num3] = ((aifactionGoal2 != null) ? aifactionGoal2.description : null) ?? "None";
					array[14] = " STRENGTH:";
					int num4 = 15;
					FactionGoal_Fleet aifactionGoal3 = shipConstructionQueueItem.AIFactionGoal;
					string text;
					if (aifactionGoal3 == null)
					{
						text = null;
					}
					else
					{
						TISpaceFleetState assignedFleet = aifactionGoal3.assignedFleet;
						text = ((assignedFleet != null) ? assignedFleet.SpaceCombatValue().ToString() : null);
					}
					array[num4] = text ?? "0";
					array[16] = "/";
					int num5 = 17;
					FactionGoal_Fleet aifactionGoal4 = shipConstructionQueueItem.AIFactionGoal;
					array[num5] = ((aifactionGoal4 != null) ? aifactionGoal4.desiredFleetCombatValue.ToString() : null) ?? "0";
					stringBuilder2.AppendLine(string.Concat(array));
					num++;
				}
			}
			TIFactionState.LogAI(stringBuilder.ToString(), false);
		}

		// Token: 0x06003466 RID: 13414 RVA: 0x001277D4 File Offset: 0x001259D4
		public static void DumpMissions(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder(faction.displayName + " assigned missions\n");
			foreach (TICouncilorState ticouncilorState in faction.activeCouncilors)
			{
				if (ticouncilorState.HasMission)
				{
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"PEND: ",
						ticouncilorState.displayName,
						" ",
						ticouncilorState.activeMission.missionTemplate.displayName,
						" ",
						ticouncilorState.activeMission.target.displayName,
						" ",
						ticouncilorState.activeMission.GetSuccessChance().ToString()
					}));
				}
				else if (ticouncilorState.completedMission != null)
				{
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"DONE: ",
						ticouncilorState.displayName,
						" ",
						ticouncilorState.completedMission.missionTemplate.displayName,
						" ",
						ticouncilorState.completedMission.target.displayName
					}));
				}
			}
			TIFactionState.LogAI(stringBuilder.ToString(), false);
		}

		// Token: 0x06003467 RID: 13415 RVA: 0x0012793C File Offset: 0x00125B3C
		public List<TIFactionGoalState> GoalsOfType(GoalType goalType, bool orderByImportance = false, bool skipResolved = true)
		{
			if (this.factionGoals.ContainsKey(goalType))
			{
				List<TIFactionGoalState> list = this.factionGoals[goalType].ToList<TIFactionGoalState>();
				if (skipResolved)
				{
					list = list.Where<TIFactionGoalState>((TIFactionGoalState x) => !x.skipGoal).ToList<TIFactionGoalState>();
				}
				if (orderByImportance)
				{
					list = list.OrderByDescending<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance).ToList<TIFactionGoalState>();
				}
				return list;
			}
			return new List<TIFactionGoalState>();
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x001279CC File Offset: 0x00125BCC
		public List<TIFactionGoalState> GoalsOfType(List<GoalType> goalTypes, bool orderByImportance = false, bool skipResolved = true)
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			foreach (GoalType goalType in goalTypes)
			{
				if (this.factionGoals.ContainsKey(goalType))
				{
					list.AddRange(this.factionGoals[goalType]);
				}
			}
			if (skipResolved)
			{
				list = list.Where<TIFactionGoalState>((TIFactionGoalState x) => x != null && !x.skipGoal).ToList<TIFactionGoalState>();
			}
			if (orderByImportance)
			{
				list = list.OrderByDescending<TIFactionGoalState, int?>(delegate(TIFactionGoalState x)
				{
					if (x == null)
					{
						return null;
					}
					return new int?(x.importance);
				}).ToList<TIFactionGoalState>();
			}
			return list;
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x00127A98 File Offset: 0x00125C98
		public TIFactionGoalState AddGoal(TIFactionGoalState prospectiveGoal, HandleDuplicateGoalRule duplicationRule = HandleDuplicateGoalRule.ResetImportance, TISpaceFleetState fleet = null)
		{
			this.fleetGoalsDirty = true;
			if (prospectiveGoal.ValidNewGoal() && this.player.isAI)
			{
				if (prospectiveGoal.incompatibleGoals != null)
				{
					Func<TIFactionGoalState, bool> <>9__0;
					foreach (GoalType goalType in prospectiveGoal.incompatibleGoals)
					{
						if (this.factionGoals.ContainsKey(goalType))
						{
							IEnumerable<TIFactionGoalState> enumerable = this.factionGoals[goalType];
							Func<TIFactionGoalState, bool> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (TIFactionGoalState x) => x.target() == prospectiveGoal.target());
							}
							TIFactionGoalState tifactionGoalState = enumerable.FirstOrDefault<TIFactionGoalState>(func);
							if (tifactionGoalState != null)
							{
								if (prospectiveGoal.GetGoalType() == GoalType.TruceWithFaction && tifactionGoalState.GetGoalType() == GoalType.WarOnFaction)
								{
									this.RemoveGoal(tifactionGoalState);
								}
								else
								{
									if (tifactionGoalState.importance >= prospectiveGoal.importance)
									{
										return tifactionGoalState;
									}
									this.RemoveGoal(tifactionGoalState);
								}
							}
						}
					}
				}
				GoalType goalType2 = prospectiveGoal.GetGoalType();
				foreach (TIFactionGoalState tifactionGoalState2 in this.factionGoals[goalType2])
				{
					if (tifactionGoalState2.IsDuplicate(prospectiveGoal, null))
					{
						switch (duplicationRule)
						{
						case HandleDuplicateGoalRule.Ignore:
							return tifactionGoalState2;
						case HandleDuplicateGoalRule.ResetImportance:
							tifactionGoalState2.SetImportance(prospectiveGoal.importance);
							break;
						case HandleDuplicateGoalRule.ResetImportanceIfHigher:
							if (prospectiveGoal.importance > tifactionGoalState2.importance)
							{
								tifactionGoalState2.SetImportance(prospectiveGoal.importance);
							}
							break;
						}
						tifactionGoalState2.assignedDate = TITimeState.Now();
						return tifactionGoalState2;
					}
				}
				TIFactionGoalState tifactionGoalState3 = null;
				switch (prospectiveGoal.GetGoalType())
				{
				case GoalType.ProspectSites:
					tifactionGoalState3 = FactionGoal_ProspectSites.CreateGoal(prospectiveGoal as FactionGoal_ProspectSites);
					break;
				case GoalType.FoundPlatform:
					tifactionGoalState3 = FactionGoal_FoundPlatform.CreateGoal(prospectiveGoal as FactionGoal_FoundPlatform);
					break;
				case GoalType.FoundBase:
					tifactionGoalState3 = FactionGoal_FoundBase.CreateGoal(prospectiveGoal as FactionGoal_FoundBase);
					break;
				case GoalType.FoundMaxStation:
					tifactionGoalState3 = FactionGoal_FoundMaxStation.CreateGoal(prospectiveGoal as FactionGoal_FoundMaxStation);
					break;
				case GoalType.BuildFullStation:
					tifactionGoalState3 = FactionGoal_BuildFullStation.CreateGoal(prospectiveGoal as FactionGoal_BuildFullStation);
					break;
				case GoalType.BuildFullBase:
					tifactionGoalState3 = FactionGoal_BuildFullBase.CreateGoal(prospectiveGoal as FactionGoal_BuildFullBase);
					break;
				case GoalType.BuildMiningBase:
					tifactionGoalState3 = FactionGoal_BuildMiningBase.CreateGoal(prospectiveGoal as FactionGoal_BuildMiningBase);
					break;
				case GoalType.BuildRefuellingStation:
					tifactionGoalState3 = FactionGoal_BuildRefuellingStation.CreateGoal(prospectiveGoal as FactionGoal_BuildRefuellingStation);
					break;
				case GoalType.BuildSpecialtyStation:
					tifactionGoalState3 = FactionGoal_BuildSpecialtyStation.CreateGoal(prospectiveGoal as FactionGoal_BuildSpecialtyStation);
					break;
				case GoalType.BuildSpecialtyBase:
					tifactionGoalState3 = FactionGoal_BuildSpecialtyBase.CreateGoal(prospectiveGoal as FactionGoal_BuildSpecialtyBase);
					break;
				case GoalType.CaptureNationClean:
					tifactionGoalState3 = FactionGoal_CaptureNation_Clean.CreateGoal(prospectiveGoal as FactionGoal_CaptureNation_Clean);
					break;
				case GoalType.CaptureNationDirty:
					tifactionGoalState3 = FactionGoal_CaptureNation_Dirty.CreateGoal(prospectiveGoal as FactionGoal_CaptureNation_Dirty);
					break;
				case GoalType.ExpandNation:
					tifactionGoalState3 = FactionGoal_ExpandNation.CreateGoal(prospectiveGoal as FactionGoal_ExpandNation);
					break;
				case GoalType.DevelopNation:
					tifactionGoalState3 = FactionGoal_DevelopNation.CreateGoal(prospectiveGoal as FactionGoal_DevelopNation);
					break;
				case GoalType.MilitarizeNation:
					tifactionGoalState3 = FactionGoal_MilitarizeNation.CreateGoal(prospectiveGoal as FactionGoal_MilitarizeNation);
					break;
				case GoalType.NeutralizeNation:
					tifactionGoalState3 = FactionGoal_NeutralizeNation.CreateGoal(prospectiveGoal as FactionGoal_NeutralizeNation);
					break;
				case GoalType.SupportNation:
					tifactionGoalState3 = FactionGoal_SupportNation.CreateGoal(prospectiveGoal as FactionGoal_SupportNation);
					break;
				case GoalType.PillageNation:
					tifactionGoalState3 = FactionGoal_PillageNation.CreateGoal(prospectiveGoal as FactionGoal_PillageNation);
					break;
				case GoalType.SpaceifyNation:
					tifactionGoalState3 = FactionGoal_SpaceifyNation.CreateGoal(prospectiveGoal as FactionGoal_SpaceifyNation);
					break;
				case GoalType.WarOnFaction:
					tifactionGoalState3 = FactionGoal_WarOnFaction.CreateGoal(prospectiveGoal as FactionGoal_WarOnFaction);
					break;
				case GoalType.TruceWithFaction:
					tifactionGoalState3 = FactionGoal_TruceWithFaction.CreateGoal(prospectiveGoal as FactionGoal_TruceWithFaction);
					break;
				case GoalType.NonAggressionPact:
					tifactionGoalState3 = FactionGoal_NonAggressionPact.CreateGoal(prospectiveGoal as FactionGoal_NonAggressionPact);
					break;
				case GoalType.AssembleFleet:
					tifactionGoalState3 = FactionGoal_AssembleFleet.CreateGoal(prospectiveGoal as FactionGoal_AssembleFleet);
					break;
				case GoalType.JoinFleet:
					tifactionGoalState3 = FactionGoal_JoinFleet.CreateGoal(prospectiveGoal as FactionGoal_JoinFleet);
					break;
				case GoalType.DefendWithFleet:
					tifactionGoalState3 = FactionGoal_DefendWithFleet.CreateGoal(prospectiveGoal as FactionGoal_DefendWithFleet);
					break;
				case GoalType.SecureEarthSpace:
					tifactionGoalState3 = FactionGoal_SecureEarthSpace.CreateGoal(prospectiveGoal as FactionGoal_SecureEarthSpace);
					break;
				case GoalType.AttackWithFleet:
					tifactionGoalState3 = FactionGoal_AttackWithFleet.CreateGoal(prospectiveGoal as FactionGoal_AttackWithFleet);
					break;
				case GoalType.CaptureHab:
					tifactionGoalState3 = FactionGoal_CaptureHab.CreateGoal(prospectiveGoal as FactionGoal_CaptureHab);
					break;
				case GoalType.ResupplyFleet:
					tifactionGoalState3 = FactionGoal_ResupplyFleet.CreateGoal(prospectiveGoal as FactionGoal_ResupplyFleet);
					break;
				case GoalType.RepairFleet:
					tifactionGoalState3 = FactionGoal_RepairFleet.CreateGoal(prospectiveGoal as FactionGoal_RepairFleet);
					break;
				case GoalType.RefitFleet:
					tifactionGoalState3 = FactionGoal_RefitFleet.CreateGoal(prospectiveGoal as FactionGoal_RefitFleet);
					break;
				case GoalType.TransportCouncilorsViaFleet:
					tifactionGoalState3 = FactionGoal_TransportCouncilorsWithFleet.CreateGoal(prospectiveGoal as FactionGoal_TransportCouncilorsWithFleet);
					break;
				case GoalType.InvadeEarth:
					tifactionGoalState3 = FactionGoal_InvadeEarth.CreateGoal(prospectiveGoal as FactionGoal_InvadeEarth);
					break;
				case GoalType.SurveilEarth:
					tifactionGoalState3 = FactionGoal_SurveilEarth.CreateGoal(prospectiveGoal as FactionGoal_SurveilEarth);
					break;
				case GoalType.PursueVictory:
					tifactionGoalState3 = FactionGoal_Victory.CreateGoal(prospectiveGoal as FactionGoal_Victory);
					break;
				case GoalType.FoundStation:
					tifactionGoalState3 = FactionGoal_FoundStation.CreateGoal(prospectiveGoal as FactionGoal_FoundStation);
					break;
				case GoalType.FoundSurveillanceStation:
					tifactionGoalState3 = FactionGoal_FoundSurveillanceStation.CreateGoal(prospectiveGoal as FactionGoal_FoundSurveillanceStation);
					break;
				case GoalType.SendFleet:
					tifactionGoalState3 = FactionGoal_SendFleet.CreateGoal(prospectiveGoal as FactionGoal_SendFleet);
					break;
				}
				if (tifactionGoalState3 == null)
				{
					return null;
				}
				tifactionGoalState3.faction = this;
				tifactionGoalState3.assignedDate = TITimeState.Now();
				this.factionGoals[goalType2].Add(tifactionGoalState3);
				if (tifactionGoalState3.isFleetGoal && fleet != null)
				{
					tifactionGoalState3.ref_fleetGoal.AssignFleet(fleet);
				}
				tifactionGoalState3.SetImportance(prospectiveGoal.importance);
				tifactionGoalState3.objective = prospectiveGoal.objective;
				if (prospectiveGoal.subsequentGoals != null && prospectiveGoal.subsequentGoals.Count > 0)
				{
					tifactionGoalState3.subsequentGoals = new List<GoalType>(prospectiveGoal.subsequentGoals);
				}
				tifactionGoalState3.OnGoalAssigned();
				return tifactionGoalState3;
			}
			return null;
		}

		// Token: 0x0600346A RID: 13418 RVA: 0x00128138 File Offset: 0x00126338
		public void RemoveGoal(TIFactionGoalState goal)
		{
			this.fleetGoalsDirty = true;
			if (goal == goal.faction.focusGoal)
			{
				goal.faction.focusGoal = null;
			}
			goal.OnGoalRemoved();
			if (goal.isFleetGoal && goal.ref_fleetGoal.assignedFleet != null)
			{
				goal.ref_fleetGoal.UnassignFleet();
			}
			if (this.player.isAI)
			{
				if (this.AISavingTarget.active && goal == this.AISavingTarget.relatedGoal)
				{
					this.AIClearSavingTarget("RemoveGoal on AISavingTarget related goal");
				}
				foreach (TIHabModuleState tihabModuleState in this.nShipyardQueues.Keys)
				{
					List<ShipConstructionQueueItem> list = new List<ShipConstructionQueueItem>();
					foreach (ShipConstructionQueueItem shipConstructionQueueItem in this.nShipyardQueues[tihabModuleState])
					{
						if (shipConstructionQueueItem.AIFactionGoal == goal)
						{
							shipConstructionQueueItem.AIFactionGoal = null;
							if (!shipConstructionQueueItem.costPaid || shipConstructionQueueItem.daysToCompletion > 30f || !shipConstructionQueueItem.shipDesign.combatant)
							{
								list.Add(shipConstructionQueueItem);
							}
						}
					}
					foreach (ShipConstructionQueueItem shipConstructionQueueItem2 in list)
					{
						this.playerControl.StartAction(new RemoveShipFromShipyardQueueAction(tihabModuleState, shipConstructionQueueItem2));
					}
				}
			}
			this.factionGoals[goal.GetGoalType()].Remove(goal);
			goal.ArchiveState(true);
			goal.RemoveState();
		}

		// Token: 0x0600346B RID: 13419 RVA: 0x00128318 File Offset: 0x00126518
		public List<TIFactionGoalState> FindGoals(GoalType goaltype, TIGameState actor, TIGameState target, TIFactionState.GoalFilter filter = TIFactionState.GoalFilter.none, bool skipResolved = true)
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			if (this.isActivePlayer)
			{
				return list;
			}
			foreach (TIFactionGoalState tifactionGoalState in this.factionGoals[goaltype])
			{
				if ((!skipResolved || !tifactionGoalState.skipGoal) && tifactionGoalState.actor() == actor && tifactionGoalState.target() == target)
				{
					switch (filter)
					{
					case TIFactionState.GoalFilter.InProgressOnly:
						if (tifactionGoalState.InProgress())
						{
							list.Add(tifactionGoalState);
							continue;
						}
						continue;
					case TIFactionState.GoalFilter.NotInProgressOnly:
						if (!tifactionGoalState.InProgress())
						{
							list.Add(tifactionGoalState);
							continue;
						}
						continue;
					}
					list.Add(tifactionGoalState);
				}
			}
			return list;
		}

		// Token: 0x0600346C RID: 13420 RVA: 0x001283E4 File Offset: 0x001265E4
		public List<TIFactionGoalState> FindGoals(List<GoalType> goalTypes, TIGameState actor, TIGameState target, TIFactionState.GoalFilter filter = TIFactionState.GoalFilter.none, bool skipResolved = true)
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			foreach (GoalType goalType in goalTypes)
			{
				list.AddRange(this.FindGoals(goalType, actor, target, filter, skipResolved));
			}
			return list;
		}

		// Token: 0x0600346D RID: 13421 RVA: 0x00128448 File Offset: 0x00126648
		public List<TIFactionGoalState> FindGoals(List<GoalType> goalTypes, List<TIGameState> actors, List<TIGameState> targets, TIFactionState.GoalFilter filter = TIFactionState.GoalFilter.none, bool skipResolved = true)
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			foreach (GoalType goalType in goalTypes)
			{
				foreach (TIFactionGoalState tifactionGoalState in this.factionGoals[goalType])
				{
					if ((!skipResolved || !tifactionGoalState.skipGoal) && actors.Contains(tifactionGoalState.actor()) && targets.Contains(tifactionGoalState.target()))
					{
						switch (filter)
						{
						case TIFactionState.GoalFilter.InProgressOnly:
							if (tifactionGoalState.InProgress())
							{
								list.Add(tifactionGoalState);
								continue;
							}
							continue;
						case TIFactionState.GoalFilter.NotInProgressOnly:
							if (!tifactionGoalState.InProgress())
							{
								list.Add(tifactionGoalState);
								continue;
							}
							continue;
						}
						list.Add(tifactionGoalState);
					}
				}
			}
			return list;
		}

		// Token: 0x0600346E RID: 13422 RVA: 0x0012854C File Offset: 0x0012674C
		public List<TIFactionGoalState> GoalsWithTarget(TIGameState target, GoalType goalTypeFilter, bool skipResolved = true)
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			foreach (TIFactionGoalState tifactionGoalState in this.factionGoals[goalTypeFilter])
			{
				if ((!skipResolved || !tifactionGoalState.skipGoal) && tifactionGoalState.target() == target)
				{
					list.Add(tifactionGoalState);
				}
			}
			return list;
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x001285C8 File Offset: 0x001267C8
		public List<TIFactionGoalState> GoalsWithTarget(TIGameState target, List<GoalType> goalTypeFilter = null, bool skipResolved = true)
		{
			List<GoalType> list = ((goalTypeFilter == null) ? this.factionGoals.Keys.ToList<GoalType>() : this.factionGoals.Keys.Intersect<GoalType>(goalTypeFilter).ToList<GoalType>());
			List<TIFactionGoalState> list2 = new List<TIFactionGoalState>();
			foreach (GoalType goalType in list)
			{
				foreach (TIFactionGoalState tifactionGoalState in this.factionGoals[goalType])
				{
					if ((!skipResolved || !tifactionGoalState.skipGoal) && tifactionGoalState.target() == target)
					{
						list2.Add(tifactionGoalState);
					}
				}
			}
			return list2;
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x001286A8 File Offset: 0x001268A8
		public IEnumerable<FactionGoal_Fleet> AllFleetGoals(bool skipResolved)
		{
			if (this.fleetGoalsDirty)
			{
				this.cachedFleetGoals = (from x in this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value)
					where x.isFleetGoal
					select x.ref_fleetGoal).ToList<FactionGoal_Fleet>();
				this.fleetGoalsDirty = false;
				this.unresolvedFleetGoalsDirty = true;
			}
			if (skipResolved)
			{
				if (this.unresolvedFleetGoalsDirty)
				{
					this.cachedUnresolvedFleetGoals = this.cachedFleetGoals.Where<FactionGoal_Fleet>((FactionGoal_Fleet x) => !x.skipGoal).ToList<FactionGoal_Fleet>();
					this.unresolvedFleetGoalsDirty = false;
				}
				return this.cachedUnresolvedFleetGoals;
			}
			return this.cachedFleetGoals;
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x001287A0 File Offset: 0x001269A0
		public List<TIFactionGoalState> AllFoundHabGoals(bool skipResolved)
		{
			if (!skipResolved)
			{
				return (from x in this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value)
					where x.FoundHabGoal()
					select x).ToList<TIFactionGoalState>();
			}
			return (from x in this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value)
				where x.FoundHabGoal()
				where !x.skipGoal
				select x).ToList<TIFactionGoalState>();
		}

		// Token: 0x06003472 RID: 13426 RVA: 0x0012887C File Offset: 0x00126A7C
		public List<TIFactionGoalState> AllCaptureNationGoals(bool skipResolved)
		{
			if (!skipResolved)
			{
				return this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value.Where<TIFactionGoalState>((TIFactionGoalState x) => x is FactionGoal_CaptureNation)).ToList<TIFactionGoalState>();
			}
			return this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value.Where<TIFactionGoalState>((TIFactionGoalState x) => x is FactionGoal_CaptureNation && !x.skipGoal)).ToList<TIFactionGoalState>();
		}

		// Token: 0x06003473 RID: 13427 RVA: 0x001288EC File Offset: 0x00126AEC
		public void CleanStateFromGoalTargets(TIGameState state)
		{
			foreach (TIFactionGoalState tifactionGoalState in this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value).ToList<TIFactionGoalState>())
			{
				if (tifactionGoalState.target() == state)
				{
					tifactionGoalState.ChangeTarget(null);
					this.unresolvedFleetGoalsDirty = true;
				}
			}
		}

		// Token: 0x06003474 RID: 13428 RVA: 0x00128980 File Offset: 0x00126B80
		public void SubstituteFleetAsGoalTarget(TISpaceFleetState oldState, TISpaceFleetState newState)
		{
			foreach (KeyValuePair<GoalType, List<TIFactionGoalState>> keyValuePair in this.factionGoals.ToList<KeyValuePair<GoalType, List<TIFactionGoalState>>>())
			{
				using (List<TIFactionGoalState>.Enumerator enumerator2 = keyValuePair.Value.ToList<TIFactionGoalState>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIFactionGoalState goal = enumerator2.Current;
						if (!(goal.target() != oldState) && !keyValuePair.Value.Where<TIFactionGoalState>((TIFactionGoalState x) => goal != x && x.IsDuplicate(goal, newState)).ToList<TIFactionGoalState>().Any<TIFactionGoalState>())
						{
							goal.ChangeTarget(newState);
							if (newState == null)
							{
								this.unresolvedFleetGoalsDirty = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x06003475 RID: 13429 RVA: 0x00128AAC File Offset: 0x00126CAC
		public void RegisterKill(TIGameState destroyedTarget, float valueMultiplier)
		{
			if (destroyedTarget.ref_faction != null)
			{
				List<string> list;
				if (!this.Kills.TryGetValue(destroyedTarget.ref_faction, out list))
				{
					list = (this.Kills[destroyedTarget.ref_faction] = new List<string>());
				}
				list.Add(destroyedTarget.GetDisplayName(this));
			}
			if (this.IsAlienFaction)
			{
				TIFactionState ref_faction = destroyedTarget.ref_faction;
				if (destroyedTarget.isHabModuleState || destroyedTarget.isSpaceShipState)
				{
					destroyedTarget = destroyedTarget.ref_spaceAsset;
				}
				if (ref_faction != null)
				{
					if (destroyedTarget.isCouncilorState)
					{
						float num = 0f;
						foreach (CouncilorAttribute councilorAttribute in Enums.CouncilorAttributes)
						{
							num += (float)destroyedTarget.ref_councilor.GetAttribute(councilorAttribute, false, false, true, false, false, false);
						}
						num /= (float)Enums.CouncilorAttributes.Length;
						this.GainFactionHate(ref_faction, -num * valueMultiplier, false, "Alien venting: Killed Councilor", true);
						return;
					}
					if ((from x in this.GoalsOfType(GoalType.AttackWithFleet, false, true)
						where x.target() == destroyedTarget
						select x).Union<TIFactionGoalState>(this.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, TIFactionGoalState>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value).Where<TIFactionGoalState>(delegate(TIFactionGoalState x)
					{
						FactionGoal_Fleet factionGoal_Fleet = x as FactionGoal_Fleet;
						return factionGoal_Fleet != null && factionGoal_Fleet.dynamicAttackTarget == destroyedTarget;
					})).Any<TIFactionGoalState>() && !this.IsTrespassing(destroyedTarget))
					{
						FactionGoal_WarOnFaction factionGoal_WarOnFaction = (from x in this.FindGoals(GoalType.WarOnFaction, this, ref_faction, TIFactionState.GoalFilter.none, true)
							select x as FactionGoal_WarOnFaction).FirstOrDefault<FactionGoal_WarOnFaction>();
						if (factionGoal_WarOnFaction == null || !factionGoal_WarOnFaction.IsTotalWar)
						{
							this.GainFactionHate(ref_faction, -valueMultiplier, false, "Alien venting: Fleet Destroyed Target during retaliation or limited war", true);
						}
					}
					if (TemplateManager.global.DoAliensGiveHateReprieveAfterKnockdown())
					{
						float factionStrengthEstimate_SpaceOnly = ref_faction.GetFactionStrengthEstimate_SpaceOnly();
						if (1f - factionStrengthEstimate_SpaceOnly / ref_faction.highestSpaceStrengthSinceLastAlienKnockdown > 0.35f)
						{
							float num2 = TemplateManager.global.AlienHateReprieveAfterKnockdown();
							this.GainFactionHate(ref_faction, this.GetFactionHate(ref_faction) * -num2, false, "Reprieve after Knockdown", true);
							ref_faction.highestSpaceStrengthSinceLastAlienKnockdown = factionStrengthEstimate_SpaceOnly;
						}
					}
				}
			}
		}

		// Token: 0x06003476 RID: 13430 RVA: 0x00128D00 File Offset: 0x00126F00
		public float GetPerceivedEnemyFleetStrengthFactor(TIFactionState enemy)
		{
			if (enemy == this)
			{
				return 1f;
			}
			float num;
			if (!this.perceivedEnemyFleetStrengthFactors.TryGetValue(enemy, out num))
			{
				num = (this.perceivedEnemyFleetStrengthFactors[enemy] = 1f);
			}
			return num;
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x00128D40 File Offset: 0x00126F40
		public float GetPerceivedEnemyFleetStrength(TISpaceFleetState enemyFleet)
		{
			return this.GetPerceivedEnemyFleetStrengthFactor(enemyFleet.faction) * enemyFleet.SpaceCombatValue();
		}

		// Token: 0x06003478 RID: 13432 RVA: 0x00128D55 File Offset: 0x00126F55
		public float GetPerceivedEnemySpaceAssetStrength(TISpaceAssetState spaceAsset)
		{
			if (spaceAsset.isHabState && spaceAsset.ref_hab.IsStation)
			{
				return spaceAsset.ref_hab.PerceivedAggregateDefensiveScore_Station(this);
			}
			if (spaceAsset.isSpaceFleetState)
			{
				return this.GetPerceivedEnemyFleetStrength(spaceAsset.ref_fleet);
			}
			return 0f;
		}

		// Token: 0x06003479 RID: 13433 RVA: 0x00128D94 File Offset: 0x00126F94
		public float GetPerceivedEnemySpaceAssetStrength_AndItsDefenders(TISpaceAssetState spaceAsset)
		{
			if (spaceAsset.isHabState && spaceAsset.ref_hab.IsBase)
			{
				return 0f;
			}
			if (!spaceAsset.isSpaceFleetState)
			{
				return this.GetPerceivedEnemySpaceAssetStrength(spaceAsset);
			}
			if (spaceAsset.ref_fleet.dockedAtStation && spaceAsset.ref_faction.permanentAlly(spaceAsset.ref_hab.ref_faction))
			{
				return this.GetPerceivedEnemySpaceAssetStrength(spaceAsset.ref_hab);
			}
			return TIFactionState.GetDefenders(spaceAsset).Sum<TISpaceFleetState>((TISpaceFleetState x) => this.GetPerceivedEnemyFleetStrength(x));
		}

		// Token: 0x0600347A RID: 13434 RVA: 0x00128E15 File Offset: 0x00127015
		public void AdjustPerceivedEnemyFleetStrengthFactor(TIFactionState enemy, float adjustmentFactor)
		{
			if (enemy == this)
			{
				return;
			}
			this.perceivedEnemyFleetStrengthFactors[enemy] = Mathf.Clamp(this.GetPerceivedEnemyFleetStrengthFactor(enemy) * adjustmentFactor, 0.6666667f, 1.5f);
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600347B RID: 13435 RVA: 0x00128E45 File Offset: 0x00127045
		public int TechRaceSlot
		{
			get
			{
				return this.techRaceSlot;
			}
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x0600347C RID: 13436 RVA: 0x00128E4D File Offset: 0x0012704D
		public bool IsInTechRace
		{
			get
			{
				return this.techRaceSlot >= 0;
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x0600347D RID: 13437 RVA: 0x00128E5B File Offset: 0x0012705B
		public TIDateTime LastTechRaceDate
		{
			get
			{
				return this.lastTechRaceDate;
			}
		}

		// Token: 0x0600347E RID: 13438 RVA: 0x00128E63 File Offset: 0x00127063
		public void BeginTechRace(int techSlot)
		{
			this.techRaceSlot = techSlot;
		}

		// Token: 0x0600347F RID: 13439 RVA: 0x00128E6C File Offset: 0x0012706C
		public void EndTechRace()
		{
			if (!this.IsInTechRace)
			{
				return;
			}
			this.techRaceSlot = -1;
			this.lastTechRaceDate = TITimeState.Now();
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06003480 RID: 13440 RVA: 0x00128E89 File Offset: 0x00127089
		// (set) Token: 0x06003481 RID: 13441 RVA: 0x00128E91 File Offset: 0x00127091
		public int PassiveTechSlot { get; private set; } = -1;

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06003482 RID: 13442 RVA: 0x00128E9A File Offset: 0x0012709A
		public bool HasChosenPassiveTechSlot
		{
			get
			{
				return this.PassiveTechSlot >= 0;
			}
		}

		// Token: 0x06003483 RID: 13443 RVA: 0x00128EA8 File Offset: 0x001270A8
		public void ClearPassiveTechSlot()
		{
			this.PassiveTechSlot = -1;
		}

		// Token: 0x06003484 RID: 13444 RVA: 0x00128EB1 File Offset: 0x001270B1
		public void SetPassiveTechSlot(int passiveTechSlot)
		{
			this.PassiveTechSlot = passiveTechSlot;
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06003485 RID: 13445 RVA: 0x00128EBA File Offset: 0x001270BA
		// (set) Token: 0x06003486 RID: 13446 RVA: 0x00128EC2 File Offset: 0x001270C2
		[fsProperty]
		public TIDateTime LastObjectiveProjectCompletionDate { get; private set; }

		// Token: 0x06003487 RID: 13447 RVA: 0x00128ECC File Offset: 0x001270CC
		public float TechCategoryValuation(TechCategory category)
		{
			switch (category)
			{
			case TechCategory.Materials:
				return this.aiValues.materialsTechs;
			case TechCategory.SpaceScience:
				return this.aiValues.spaceTechs;
			case TechCategory.Energy:
				return this.aiValues.energyTechs;
			case TechCategory.LifeScience:
				return this.aiValues.lifeTechs;
			case TechCategory.MilitaryScience:
				return this.aiValues.militaryTechs;
			case TechCategory.InformationScience:
				return this.aiValues.informationTechs;
			case TechCategory.SocialScience:
				return this.aiValues.socialTechs;
			case TechCategory.Xenology:
				return 2f;
			default:
				return 1f;
			}
		}

		// Token: 0x06003488 RID: 13448 RVA: 0x00128F60 File Offset: 0x00127160
		public float TechRoleValuation(TechRole role)
		{
			switch (role)
			{
			case TechRole.SpaceDevelopment:
				return this.aiValues.wantSpaceFacilities;
			case TechRole.SpaceExpansion:
				if (this.GetCurrentResourceAmount(FactionResource.Boost) <= 50f && this.bases.Count <= 1)
				{
					return 0.25f;
				}
				return 1.25f;
			case TechRole.SpaceWar:
				return this.aiValues.wantSpaceWarCapability * (float)(this.huntingAlienWarship ? 10 : 1);
			case TechRole.EarthPolitics:
				return this.aiValues.wantEarthWarCapability * this.aiValues.wantPopularity;
			case TechRole.Efficiency:
				return this.aiValues.gatherMoney * this.aiValues.gatherScience * this.aiValues.riskAversion;
			case TechRole.Income:
				return this.aiValues.gatherInfluence * this.aiValues.gatherMoney * this.aiValues.gatherOps * this.aiValues.gatherScience;
			case TechRole.FactionObjective:
				return 10f;
			default:
				return 1f;
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06003489 RID: 13449 RVA: 0x0012905C File Offset: 0x0012725C
		public List<string> forcedTechNames
		{
			get
			{
				List<string> list = new List<string>(this.template.firstTechNames);
				if (this.unlockedVictoryObjective)
				{
					list.AddRangeUnique<string>(this.template.winnerTechNames);
				}
				return list;
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x0600348A RID: 13450 RVA: 0x00129094 File Offset: 0x00127294
		public string cheapestForcedTechName
		{
			get
			{
				List<TIGenericTechTemplate> list = (from x in (from x in TIGlobalResearchState.AvailableTechs()
						select x.dataName).Union<string>(this.forcedTechNames)
					select TemplateManager.Find<TIGenericTechTemplate>(x, true)).ToList<TIGenericTechTemplate>();
				if (list.Count > 0)
				{
					return list.MinBy<TIGenericTechTemplate, float>((TIGenericTechTemplate x) => x.GetResearchCost(this)).dataName;
				}
				return string.Empty;
			}
		}

		// Token: 0x0600348B RID: 13451 RVA: 0x00129128 File Offset: 0x00127328
		public float AI_ModifiedRiskAversion()
		{
			float num = this.aiValues.riskAversion;
			switch (this.selfAssessement)
			{
			case FactionSelfAssessment.LosingBig:
				num *= 0.7f;
				break;
			case FactionSelfAssessment.Losing:
				num *= 0.9f;
				break;
			case FactionSelfAssessment.WayAhead:
				num *= 1.15f;
				break;
			}
			return Mathf.Clamp(num, 0.5f, 1f);
		}

		// Token: 0x0600348C RID: 13452 RVA: 0x00129194 File Offset: 0x00127394
		public TIFactionGoalState GetManagementGoalForNation(TINationState nation, bool beneficialOnly)
		{
			List<TIFactionGoalState> list = this.FindGoals(TIFactionGoalState.NationManagementGoals, this, nation, TIFactionState.GoalFilter.none, true);
			if (beneficialOnly)
			{
				list.RemoveAll((TIFactionGoalState x) => x.GetGoalType() == GoalType.PillageNation);
			}
			if (list.Count > 0)
			{
				return list.MaxBy<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance);
			}
			return null;
		}

		// Token: 0x0600348D RID: 13453 RVA: 0x0012920C File Offset: 0x0012740C
		public TIFactionGoalState SetManagementGoalForNation(TINationState nation)
		{
			List<TIFactionGoalState> list = this.FindGoals(TIFactionGoalState.NationManagementGoals, this, nation, TIFactionState.GoalFilter.none, true);
			list.AddRange(this.FindGoals(TIFactionGoalState.NationManagementGoals, nation, nation, TIFactionState.GoalFilter.none, true));
			GoalType goalType = this.AI_GetPreferredManagementGoalForNation(nation);
			if (list.Count == 0 || (goalType != list[0].GetGoalType() && goalType != GoalType.None))
			{
				list.ForEach(delegate(TIFactionGoalState x)
				{
					this.RemoveGoal(x);
				});
				switch (goalType)
				{
				case GoalType.ExpandNation:
					return this.AddGoal(new FactionGoal_ExpandNation(this, 5 + nation.numControlPoints_unclamped * 2, nation), HandleDuplicateGoalRule.Ignore, null);
				case GoalType.DevelopNation:
					return this.AddGoal(new FactionGoal_DevelopNation(this, 5 + nation.numControlPoints_unclamped * 2, nation), HandleDuplicateGoalRule.Ignore, null);
				case GoalType.MilitarizeNation:
					return this.AddGoal(new FactionGoal_MilitarizeNation(this, 5 + nation.numControlPoints_unclamped * 2, nation), HandleDuplicateGoalRule.Ignore, null);
				case GoalType.PillageNation:
					return this.AddGoal(new FactionGoal_PillageNation(this, 5 + nation.numControlPoints_unclamped * 2, nation), HandleDuplicateGoalRule.Ignore, null);
				case GoalType.SpaceifyNation:
					return this.AddGoal(new FactionGoal_SpaceifyNation(this, 5 + nation.numControlPoints_unclamped * 2, nation), HandleDuplicateGoalRule.Ignore, null);
				}
			}
			if (list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x00129338 File Offset: 0x00127538
		public GoalType AI_GetPreferredManagementGoalForNation(TINationState nation)
		{
			if (nation.alienNation)
			{
				return GoalType.ExpandNation;
			}
			if (this.FindGoals(GoalType.NeutralizeNation, this, nation, TIFactionState.GoalFilter.none, true).Count > 0 || (this.GetDailyIncome(FactionResource.Money, false, false) < 0f && (float)nation.numControlPoints <= Mathf.Round(3f - this.aiValues.protectHumanLife) && nation.numStandardArmies == 0 && !nation.spaceFlightProgram && nation.spoilsPriorityMoneyPerControlPoint > 50f * this.aiValues.gatherMoney))
			{
				return GoalType.PillageNation;
			}
			bool flag = nation.numControlPoints + (nation.military ? 1 : 0) < 4;
			bool flag2 = this.NeedsSpaceBootstrap();
			bool flag3 = this.resourceIncomeDeficiencies.Contains(FactionResource.Boost) || flag2;
			bool flag4 = this.LackingBasicMissionControl();
			bool flag5 = flag4 || !AIEvaluators.Abundant(this, FactionResource.MissionControl, 1f);
			int num = nation.maxMissionControl - nation.currentMissionControl;
			bool flag6 = flag3 && nation.IsUsefulForBoost();
			bool flag7 = flag5 && num >= (nation.spaceFlightProgram ? 1 : 2);
			bool flag8 = (flag6 || flag7) && (flag || !nation.spaceFlightProgram || flag4 || flag3);
			bool flag9 = false;
			if (!flag)
			{
				flag9 = nation.wars.Count > 0 && !nation.HasExternalClaims();
				if (!flag9 && !flag8)
				{
					float num2 = 0f;
					if (nation.wars.Count > 0)
					{
						num2 = nation.wars.Sum<TINationState>((TINationState x) => (float)x.numStandardArmies * x.militaryTechLevel);
					}
					else
					{
						foreach (TINationState tinationState in nation.rivals)
						{
							float num3 = (float)tinationState.numStandardArmies * tinationState.militaryTechLevel + tinationState.allies.Sum<TINationState>((TINationState x) => (float)x.numStandardArmies * x.militaryTechLevel);
							if (num3 > num2)
							{
								num2 = num3;
							}
						}
					}
					flag9 = num2 > (((float)nation.NumNuclearWeaponsDefendingMe() > 0f) ? 2f : 1.25f) * ((float)nation.numStandardArmies * nation.militaryTechLevel) + nation.allies.Sum<TINationState>((TINationState x) => (float)x.numStandardArmies * x.militaryTechLevel);
				}
			}
			if (flag9)
			{
				return GoalType.MilitarizeNation;
			}
			if (flag8)
			{
				return GoalType.SpaceifyNation;
			}
			Func<TIBilateralTemplate, bool> <>9__4;
			if ((nation.HasExternalClaims() || this.availableProjects.Any<TIProjectTemplate>(delegate(TIProjectTemplate x)
			{
				IEnumerable<TIBilateralTemplate> associatedClaims = x.associatedClaims;
				Func<TIBilateralTemplate, bool> func;
				if ((func = <>9__4) == null)
				{
					func = (<>9__4 = (TIBilateralTemplate x) => x.nationState1 == nation);
				}
				return associatedClaims.Any<TIBilateralTemplate>(func);
			})) && nation.MajorityControlFaction == this)
			{
				return GoalType.ExpandNation;
			}
			return GoalType.DevelopNation;
		}

		// Token: 0x0600348F RID: 13455 RVA: 0x00129684 File Offset: 0x00127884
		public bool AI_AtWarWithOtherFactions()
		{
			return this.enemyWarFactions.Count > 0;
		}

		// Token: 0x06003490 RID: 13456 RVA: 0x00129694 File Offset: 0x00127894
		public bool AI_AtWarWithFaction(TIFactionState faction)
		{
			if (faction == null)
			{
				return false;
			}
			if (this.factionWarStatusCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedFactionWarStatus.Clear();
				this.factionWarStatusCachedFrame = TIFrameCounter.FrameCount;
			}
			bool flag;
			if (this.cachedFactionWarStatus.TryGetValue(faction, out flag))
			{
				return flag;
			}
			bool flag2 = this.FindGoals(GoalType.WarOnFaction, this, faction, TIFactionState.GoalFilter.none, true).Count > 0 || AIEvaluators.FactionsGoToWar(this, faction);
			this.cachedFactionWarStatus[faction] = flag2;
			return flag2;
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x0012970E File Offset: 0x0012790E
		public int AI_WarWithFactionImportance(TIFactionState otherFaction)
		{
			if (!(otherFaction != null))
			{
				return 0;
			}
			TIFactionGoalState tifactionGoalState = this.FindGoals(GoalType.WarOnFaction, this, otherFaction, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>();
			if (tifactionGoalState == null)
			{
				return 0;
			}
			return tifactionGoalState.importance;
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x00129737 File Offset: 0x00127937
		public float AvailableCPCapSpace()
		{
			return this.GetControlPointMaintenanceFreebieCap() - this.controlPoints.Sum<TIControlPoint>((TIControlPoint x) => x.CurrentMaintenanceCost);
		}

		// Token: 0x06003493 RID: 13459 RVA: 0x0012976A File Offset: 0x0012796A
		public bool MinorCPTrouble()
		{
			return this.GetAnnualControlPointMaintenanceCost() > 0f;
		}

		// Token: 0x06003494 RID: 13460 RVA: 0x00129779 File Offset: 0x00127979
		public bool MajorCPTrouble()
		{
			return this.MinorCPTrouble() && this.GetDailyIncome(FactionResource.Influence, false, false) < 0f;
		}

		// Token: 0x06003495 RID: 13461 RVA: 0x00129795 File Offset: 0x00127995
		public bool NationWithFactionInterest(TINationState nation, bool includeAlienProxyRelations)
		{
			return nation.extant && (this == nation.executiveFaction || nation.CouncilControlPointFraction(this, true, includeAlienProxyRelations) > 0.25f);
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x06003496 RID: 13462 RVA: 0x001297C1 File Offset: 0x001279C1
		public TIShipHullTemplate FlagshipHull
		{
			get
			{
				return this.allowedShipHulls.MaxBy<TIShipHullTemplate, int>((TIShipHullTemplate x) => x.hullHardpoints);
			}
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x001297F0 File Offset: 0x001279F0
		public TISpaceShipTemplate GetDesiredShipToBuild(FactionGoal_Fleet factionGoal, bool needNow = false)
		{
			List<TISpaceFleetState> list = factionGoal.pendingFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.inTransfer).ToList<TISpaceFleetState>();
			if (factionGoal.assignedFleet != null)
			{
				list.Add(factionGoal.assignedFleet);
			}
			ShipRole role = ShipRole.MM_SpaceSuperiority;
			ShipSize size = ShipSize.Medium;
			List<TISpaceShipTemplate> list2 = new List<TISpaceShipTemplate>();
			TIShipHullTemplate forceHull = null;
			ShipRole primaryRole = factionGoal.GetPrimaryShipRole();
			TIShipHullTemplate desiredFlagshipHull = factionGoal.desiredFlagshipHull;
			IEnumerable<TISpaceShipTemplate> enumerable = factionGoal.PendingShipTemplates().Concat<TISpaceShipTemplate>(list.SelectMany<TISpaceFleetState, TISpaceShipTemplate>((TISpaceFleetState x) => x.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState y) => y.template)));
			bool flag = primaryRole != ShipRole.NoRole && enumerable.None<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == primaryRole && x.CanFulfillGoal(factionGoal));
			bool flag2 = desiredFlagshipHull != null && enumerable.None<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.hullName == desiredFlagshipHull.dataName);
			if (primaryRole == ShipRole.ArmyCarrier)
			{
				if (enumerable.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.InvasionCombatValue()) < factionGoal.GetDesiredAssaultCombatValue())
				{
					flag = true;
				}
			}
			else if (enumerable.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.AssaultCombatValue(false)) < factionGoal.GetDesiredAssaultCombatValue())
			{
				primaryRole = ShipRole.TroopCarrier;
				flag = true;
			}
			FactionGoal_AttackWithFleet attackGoal = factionGoal as FactionGoal_AttackWithFleet;
			if (attackGoal != null && attackGoal.bombardmentGoal)
			{
				float desiredBombardmentValue = attackGoal.GetDesiredBombardmentValue();
				if (enumerable.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.BombardmentValue(attackGoal.target().ref_spaceBody)) < desiredBombardmentValue)
				{
					flag = true;
				}
			}
			bool flag3 = flag && this.shipDesigns.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == primaryRole).None<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.CanFulfillGoal(factionGoal));
			if (flag || flag2)
			{
				role = primaryRole;
				forceHull = desiredFlagshipHull;
				switch (role)
				{
				case ShipRole.NoRole:
					role = factionGoal.GetSecondaryShipRoles().SelectRandomWeightedItem<KeyValuePair<ShipRole, float>>((KeyValuePair<ShipRole, float> x) => x.Value, -1f, 1E-37f).Key;
					break;
				case ShipRole.TroopCarrier:
				case ShipRole.ArmyCarrier:
					size = ShipSize.Large;
					break;
				case ShipRole.InnerSystemColonyShip:
				case ShipRole.OuterSystemColonyShip:
				case ShipRole.EarthSurveillance:
					size = ShipSize.Medium;
					break;
				}
				TISpaceShipTemplate tispaceShipTemplate;
				if ((flag3 || (forceHull != null && this.shipDesigns.None<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == role && x.hullName == forceHull.dataName))) && this.DesignShip(false, role, out tispaceShipTemplate, this.DesiredStrategicRange_AU(), this.UnlockedExotics && this.GetCurrentResourceAmount(FactionResource.Exotics) > 0f, this.UnlockedAntimatter && this.GetDailyIncome(FactionResource.Antimatter, false, false) > 0f, forceHull, null, false, null, null, float.PositiveInfinity, float.PositiveInfinity) == TIFactionState.ShipDesignerOutcome.Success)
				{
					this.playerControl.StartAction(new SaveShipDesignAction(this, tispaceShipTemplate));
				}
			}
			else
			{
				role = factionGoal.GetSecondaryShipRoles().SelectRandomWeightedItem<KeyValuePair<ShipRole, float>>((KeyValuePair<ShipRole, float> x) => x.Value, -1f, 1E-37f).Key;
				IEnumerable<TISpaceShipTemplate> enumerable2 = enumerable.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.combatant || x.role == ShipRole.TroopCarrier);
				Dictionary<ShipSize, float> dictionary = new Dictionary<ShipSize, float>();
				float num = 2f;
				float num2 = 1f / num;
				using (IEnumerator enumerator = Enum.GetValues(typeof(ShipSize)).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ShipSize sizeCategory = (ShipSize)enumerator.Current;
						dictionary[sizeCategory] = 1f / (1f + this.aiValues.fleetMediums + this.aiValues.fleetSmalls);
						Dictionary<ShipSize, float> dictionary2;
						ShipSize shipSize;
						switch (sizeCategory)
						{
						case ShipSize.Small:
							dictionary2 = dictionary;
							shipSize = sizeCategory;
							dictionary2[shipSize] *= this.aiValues.fleetSmalls;
							break;
						case ShipSize.Medium:
							dictionary2 = dictionary;
							shipSize = sizeCategory;
							dictionary2[shipSize] *= this.aiValues.fleetMediums;
							break;
						case ShipSize.Large:
							dictionary2 = dictionary;
							shipSize = sizeCategory;
							dictionary2[shipSize] *= 1f;
							break;
						}
						int num3 = enumerable2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size == sizeCategory).Count<TISpaceShipTemplate>();
						float num4 = (float)enumerable2.Count<TISpaceShipTemplate>() * dictionary[sizeCategory];
						dictionary2 = dictionary;
						shipSize = sizeCategory;
						dictionary2[shipSize] /= ((float)num3 + num2) / (num4 + num2);
					}
				}
				size = dictionary.SelectRandomWeightedItem<KeyValuePair<ShipSize, float>>((KeyValuePair<ShipSize, float> x) => x.Value, -1f, 1E-37f).Key;
			}
			if (!enumerable.Any<TISpaceShipTemplate>() || forceHull != null)
			{
				List<ShipRole> allowedRoles = new List<ShipRole>();
				if (factionGoal.GetPrimaryShipRole() != ShipRole.NoRole)
				{
					allowedRoles.Add(factionGoal.GetPrimaryShipRole());
				}
				else
				{
					allowedRoles = factionGoal.GetSecondaryShipRoles().Keys.ToList<ShipRole>();
				}
				if (forceHull != null)
				{
					list2 = this.shipDesigns.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.hullTemplate == forceHull && allowedRoles.Contains(x.role) && !x.Obsolete(this)).ToList<TISpaceShipTemplate>();
				}
				if (list2.Count == 0)
				{
					list2 = this.shipDesigns.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => allowedRoles.Contains(x.role) && !x.Obsolete(this)).ToList<TISpaceShipTemplate>();
				}
			}
			else
			{
				list2 = this.shipDesigns.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == role && !x.Obsolete(this)).ToList<TISpaceShipTemplate>();
			}
			List<TIFactionState> enemyTotalWarFactions = this.enemyTotalWarFactions;
			if (this.IsAlienFaction && !enemyTotalWarFactions.Any<TIFactionState>())
			{
				TIShipHullTemplate flagshipHull = this.FlagshipHull;
				if (!flag2 || flagshipHull != desiredFlagshipHull)
				{
					list2.RemoveAll((TISpaceShipTemplate x) => x.hullName == flagshipHull.dataName);
				}
			}
			if (this.IsAlienFaction && !enemyTotalWarFactions.Any<TIFactionState>())
			{
				List<TISpaceShipTemplate> list3 = list2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => !x.requiresExotics).ToList<TISpaceShipTemplate>();
				if (list3.Count > 0 && TIGlobalConfig.AlienInnerSystemExoticAttacksAreActive && role.IsCombatantRole() && !flag && !flag2)
				{
					TISpaceBodyState ref_system = factionGoal.target().ref_system;
					if (((ref_system != null) ? ref_system.semiMajorAxis_AU : 0.0) < 4.5 && (factionGoal is FactionGoal_AttackWithFleet || factionGoal is FactionGoal_SurveilEarth || factionGoal is FactionGoal_SecureEarthSpace))
					{
						list2 = list3;
					}
				}
			}
			if (flag)
			{
				list2.RemoveAll((TISpaceShipTemplate x) => !x.CanFulfillGoal(factionGoal));
			}
			new List<TISpaceShipTemplate>(list2);
			List<TISpaceShipTemplate> list4 = new List<TISpaceShipTemplate>();
			float num5 = AIEvaluators.SpaceResourcesForShipBuild(factionGoal);
			float num6 = this.GetCurrentResourceAmount(FactionResource.Exotics);
			if (this.AISavingTarget.active && this.AISavingTarget.relatedGoal != factionGoal && this.AISavingTarget.importance > factionGoal.importance)
			{
				num6 -= this.AISavingTarget.GetResourcesToSave().GetSingleCostValue(FactionResource.Exotics);
			}
			num6 = Mathf.Max(0f, num6);
			Dictionary<TISpaceShipTemplate, TIResourcesCost> dictionary3 = list2.ToDictionary<TISpaceShipTemplate, TISpaceShipTemplate, TIResourcesCost>((TISpaceShipTemplate x) => x, (TISpaceShipTemplate x) => x.spaceResourceConstructionCost(false, null, true, false, false));
			foreach (KeyValuePair<TISpaceShipTemplate, TIResourcesCost> keyValuePair in dictionary3)
			{
				TISpaceShipTemplate key = keyValuePair.Key;
				TIResourcesCost value = keyValuePair.Value;
				if (this.UnlockedAntimatter && value.GetSingleCostValue(FactionResource.Antimatter) > this.GetCurrentResourceAmount(FactionResource.Antimatter) * num5)
				{
					list2.Remove(key);
				}
				else if (this.UnlockedExotics && value.GetSingleCostValue(FactionResource.Exotics) > num6 * num5)
				{
					list2.Remove(key);
				}
				else if (needNow && !value.CanAfford_AI(this, null, null, factionGoal.importance, false, false, 1f, null, float.PositiveInfinity))
				{
					list4.Add(key);
				}
			}
			Func<ResourceValue, float> <>9__46;
			Dictionary<TISpaceShipTemplate, float> dictionary4 = dictionary3.ToDictionary<KeyValuePair<TISpaceShipTemplate, TIResourcesCost>, TISpaceShipTemplate, float>((KeyValuePair<TISpaceShipTemplate, TIResourcesCost> x) => x.Key, delegate(KeyValuePair<TISpaceShipTemplate, TIResourcesCost> x)
			{
				TIResourcesCost value2 = x.Value;
				IEnumerable<ResourceValue> resourceCosts = value2.resourceCosts;
				Func<ResourceValue, float> func;
				if ((func = <>9__46) == null)
				{
					func = (<>9__46 = (ResourceValue x) => x.value / this.GetDailyIncome(x.resource, true, false));
				}
				return resourceCosts.Max<ResourceValue>(func) / value2.resourceCosts.Sum<ResourceValue>((ResourceValue x) => x.value);
			});
			List<TISpaceShipTemplate> list5 = (from x in dictionary4
				where x.Value < 0.09f
				select x.Key).ToList<TISpaceShipTemplate>();
			if (list5.Count > 0)
			{
				list2 = list5;
			}
			else
			{
				List<TISpaceShipTemplate> list6 = (from x in dictionary4
					where x.Value < 0.18f
					select x.Key).ToList<TISpaceShipTemplate>();
				if (list6.Count > 0)
				{
					list2 = list6;
				}
				else
				{
					List<TISpaceShipTemplate> list7 = (from x in dictionary4
						where !float.IsPositiveInfinity(x.Value)
						select x.Key).ToList<TISpaceShipTemplate>();
					if (list7.Count > 0)
					{
						list2 = list7;
					}
				}
			}
			if (needNow && list4.Count != list2.Count)
			{
				list2 = list2.Except<TISpaceShipTemplate>(list4).ToList<TISpaceShipTemplate>();
			}
			if (list2.Count <= 0)
			{
				return null;
			}
			if (this.IsAlienFaction)
			{
				if (factionGoal.importance < 15)
				{
					if (list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.hullName != "AlienMothership"))
					{
						list2.RemoveAll((TISpaceShipTemplate x) => x.hullName == "AlienMothership");
					}
				}
				if (factionGoal.importance < 10 && factionGoal.GetGoalType() != GoalType.InvadeEarth)
				{
					if (list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => !x.requiresExotics))
					{
						list2.RemoveAll((TISpaceShipTemplate x) => x.requiresExotics);
					}
				}
			}
			if (factionGoal.importance >= 10 || factionGoal.GetGoalType() == GoalType.InvadeEarth)
			{
				if (list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size == size))
				{
					return list2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size == size).MaxBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false));
				}
				if (list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size < size))
				{
					return list2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size < size).MaxBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false));
				}
				return list2.MaxBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false));
			}
			else
			{
				if (list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size == size))
				{
					return list2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size == size).MinBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.wetMass_tons);
				}
				if (list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size < size))
				{
					return list2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.size < size).MinBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.wetMass_tons);
				}
				return list2.MinBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.wetMass_tons);
			}
		}

		// Token: 0x06003498 RID: 13464 RVA: 0x0012A580 File Offset: 0x00128780
		public void FactionExposed(TIFactionState otherFaction)
		{
			foreach (TICouncilorState ticouncilorState in this.councilors)
			{
				otherFaction.GainIntelToMinimum(ticouncilorState, TemplateManager.global.intelToSeeCouncilorBasicData, TemplateManager.global.intelToSeeCouncilorBasicData, null, 1f);
				ticouncilorState.AddToParanoia(otherFaction);
			}
		}

		// Token: 0x06003499 RID: 13465 RVA: 0x0012A5F8 File Offset: 0x001287F8
		public void AISetSavingTarget(TIDataTemplate desiredPurchase, TIGameState location, TIFactionGoalState factionGoal)
		{
			this.AISavingTarget = new AISavingData(this, desiredPurchase, location, factionGoal, AISavingData.GetBankingPercentage(factionGoal));
		}

		// Token: 0x0600349A RID: 13466 RVA: 0x0012A60F File Offset: 0x0012880F
		public void AIClearSavingTarget(string stack)
		{
			this.AISavingTarget.ClearPurchaseData();
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x0600349B RID: 13467 RVA: 0x0012A61C File Offset: 0x0012881C
		public bool CanDetectTerrorMissions
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x0600349C RID: 13468 RVA: 0x0012A61F File Offset: 0x0012881F
		public bool CanDetectAbductions
		{
			get
			{
				return TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, this, 0f, null) >= 1f;
			}
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x0600349D RID: 13469 RVA: 0x0012A638 File Offset: 0x00128838
		public bool CanDetectEnthralls
		{
			get
			{
				return TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, this, 0f, null) >= 2f;
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x0600349E RID: 13470 RVA: 0x0012A651 File Offset: 0x00128851
		public bool CanDetectAllAlienMissions
		{
			get
			{
				return TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, this, 0f, null) >= 3f;
			}
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x0600349F RID: 13471 RVA: 0x0012A66A File Offset: 0x0012886A
		public bool CanDetectAlien
		{
			get
			{
				return TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, this, 0f, null) >= 4f;
			}
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x060034A0 RID: 13472 RVA: 0x0012A683 File Offset: 0x00128883
		public bool CanCaptureAlien
		{
			get
			{
				return TIEffectsState.CheckForAnyEffectInContext(Context.CanCaptureHydra, this);
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x060034A1 RID: 13473 RVA: 0x0012A68C File Offset: 0x0012888C
		public bool HasRelationsWithAliens
		{
			get
			{
				return TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, this);
			}
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x060034A2 RID: 13474 RVA: 0x0012A695 File Offset: 0x00128895
		public bool AlienContactBlocked
		{
			get
			{
				return TIEffectsState.SumEffectsModifiers(Context.CanContactAliens, this, 0f, null) > 0f;
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x060034A3 RID: 13475 RVA: 0x0012A6AB File Offset: 0x001288AB
		public bool CanContactAlien
		{
			get
			{
				return this.CanDetectAlien && this.proAlien && (this.HasRelationsWithAliens || this.currentlyTryingToContactHydra) && !this.AlienContactBlocked;
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x060034A4 RID: 13476 RVA: 0x0012A6D8 File Offset: 0x001288D8
		public bool CanCountAbductions
		{
			get
			{
				if (this.IsAlienFaction)
				{
					return true;
				}
				if (!this.permanentAlly(GameStateManager.AlienFaction()))
				{
					return false;
				}
				return this.councilors.Any<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false).Contains(TIFactionState.abductionsMission));
			}
		}

		// Token: 0x060034A5 RID: 13477 RVA: 0x0012A730 File Offset: 0x00128930
		public bool CanDetectAlienMission(TIMissionTemplate mission)
		{
			string dataName = mission.dataName;
			if (dataName != null)
			{
				if (dataName == "TerrorizeRegion")
				{
					return this.CanDetectTerrorMissions;
				}
				if (dataName == "Abductions")
				{
					return this.CanDetectAbductions;
				}
				if (dataName == "EnthrallPublic" || dataName == "EnthrallUnalignedElites" || dataName == "EnthrallElites" || dataName == "EnthrallOrg")
				{
					return this.CanDetectEnthralls;
				}
			}
			return this.CanDetectAllAlienMissions;
		}

		// Token: 0x060034A6 RID: 13478 RVA: 0x0012A7BC File Offset: 0x001289BC
		public void SetIntialPlanetaryConquestGoals(TIHabSiteState mainBaseSite)
		{
			List<TIHabSiteState> list = new List<TIHabSiteState> { mainBaseSite };
			TIHabSiteState tihabSiteState = LegacyHabPlanner.SelectHabSiteForDevelopment(this, this.primaryHab.habSite.parentBody, list, true, false, 1, false, null);
			if (tihabSiteState != null)
			{
				this.AddGoal(new FactionGoal_FoundBase(this, 8, tihabSiteState, GoalType.BuildMiningBase, null, GoalType.None, false, null), HandleDuplicateGoalRule.Ignore, null);
				list.Add(tihabSiteState);
			}
			else
			{
				TIHabSiteState tihabSiteState2 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 35f, 55f, list, false, false, false, null, 3, false, null);
				if (tihabSiteState2 != null)
				{
					this.AddGoal(new FactionGoal_FoundBase(this, 8, tihabSiteState2, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
					list.Add(tihabSiteState2);
				}
			}
			TIHabSiteState tihabSiteState3 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, this.primaryHab.habSite.parentBody, list, true, false, 1, false, null);
			if (tihabSiteState3 != null)
			{
				this.AddGoal(new FactionGoal_FoundBase(this, 3, tihabSiteState3, GoalType.BuildMiningBase, null, GoalType.None, false, null), HandleDuplicateGoalRule.Ignore, null);
				list.Add(tihabSiteState3);
			}
			else
			{
				TIHabSiteState tihabSiteState4 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 35f, 55f, list, false, false, false, null, 3, false, null);
				if (tihabSiteState4 != null)
				{
					this.AddGoal(new FactionGoal_FoundBase(this, 3, tihabSiteState4, GoalType.BuildMiningBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
					list.Add(tihabSiteState4);
				}
			}
			if (TIGlobalValuesState.GlobalValues.difficulty >= 3)
			{
				TIHabSiteState tihabSiteState5 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, this.primaryHab.habSite.parentBody, list, true, false, 1, false, null);
				if (tihabSiteState5 != null)
				{
					this.AddGoal(new FactionGoal_FoundBase(this, 3, tihabSiteState5, GoalType.BuildMiningBase, null, GoalType.None, false, null), HandleDuplicateGoalRule.Ignore, null);
					list.Add(tihabSiteState5);
				}
				else
				{
					TIHabSiteState tihabSiteState6 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 35f, 55f, list, false, false, false, null, 3, false, null);
					if (tihabSiteState6 != null)
					{
						this.AddGoal(new FactionGoal_FoundBase(this, 3, tihabSiteState6, GoalType.BuildMiningBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
						list.Add(tihabSiteState6);
					}
				}
			}
			TIHabSiteState tihabSiteState7 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 25f, 35f, list, false, false, false, null, 3, false, null);
			if (tihabSiteState7 != null)
			{
				this.AddGoal(new FactionGoal_FoundBase(this, 17, tihabSiteState7, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
				list.Add(tihabSiteState7);
			}
			TIHabSiteState tihabSiteState8 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 15f, 25f, list, false, false, false, null, 3, false, null);
			if (tihabSiteState8 != null)
			{
				this.AddGoal(new FactionGoal_FoundBase(this, 17, tihabSiteState8, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
				list.Add(tihabSiteState8);
			}
			TIHabSiteState tihabSiteState9 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 9f, 10f, list, true, false, false, null, 3, false, null);
			if (tihabSiteState9 != null)
			{
				this.AddGoal(new FactionGoal_FoundBase(this, 17, tihabSiteState9, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
				list.Add(tihabSiteState9);
			}
			TIHabSiteState tihabSiteState10 = LegacyHabPlanner.SelectHabSiteForDevelopment(this, 6f, 30f, list, false, true, false, ((tihabSiteState9 != null) ? tihabSiteState9.ref_spaceBody : null) ?? null, 3, false, null);
			if (tihabSiteState10 != null)
			{
				this.AddGoal(new FactionGoal_FoundBase(this, 15, tihabSiteState10, GoalType.BuildMiningBase, null, GoalType.BuildRefuellingStation, false, null), HandleDuplicateGoalRule.Ignore, null);
				list.Add(tihabSiteState10);
			}
			this.AddGoal(new FactionGoal_SurveilEarth(this, 15), HandleDuplicateGoalRule.ResetImportanceIfHigher, this.fleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x.HasSpecialModuleCapability(SpecialModuleRule.Surveillance)));
			this.AddGoal(new FactionGoal_SurveilEarth(this, 10), HandleDuplicateGoalRule.Ignore, null);
		}

		// Token: 0x060034A7 RID: 13479 RVA: 0x0012AAEC File Offset: 0x00128CEC
		public float AlienHabSurveillanceStrength()
		{
			float num = 0f;
			foreach (TIHabModuleState tihabModuleState in this.EarthSystemStations.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.ActiveModules()))
			{
				if (tihabModuleState.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienSurveillance))
				{
					num += tihabModuleState.moduleTemplate.specialRulesValue;
				}
			}
			return num;
		}

		// Token: 0x060034A8 RID: 13480 RVA: 0x0012AB80 File Offset: 0x00128D80
		public float GetBestNotionalMissionSuccessChance(TIMissionTemplate mission, TIGameState target, List<TICouncilorState> councilorsToCheck = null)
		{
			float num = -1f;
			if (councilorsToCheck == null)
			{
				councilorsToCheck = new List<TICouncilorState>(this.councilors);
			}
			foreach (TICouncilorState ticouncilorState in councilorsToCheck)
			{
				if (ticouncilorState.GetPossibleMissionList(false, false, true, null, false).Contains(mission) && mission.GetValidTargets(ticouncilorState).Contains(target))
				{
					num = Mathf.Max(num, mission.resolutionMethod.GetSuccessChance(mission, ticouncilorState, target, 0f, false));
				}
			}
			return num;
		}

		// Token: 0x060034A9 RID: 13481 RVA: 0x0012AC1C File Offset: 0x00128E1C
		public static List<TIFactionState.AdviceData> GetAdvice(TIGameState speaker, int kount, List<TIFactionState.Advice> allowedAdviceTypes = null)
		{
			TIFactionState faction = speaker.ref_faction;
			TICouncilorState councilor = speaker.ref_councilor;
			TIFactionState faction2 = faction;
			bool flag = councilor != null;
			List<TIFactionState.AdviceData> list = new List<TIFactionState.AdviceData>();
			if (faction == null)
			{
				return list;
			}
			List<TIFactionState> list2 = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			list2.Remove(faction);
			Dictionary<TICouncilorState, TIGameState> dictionary = faction.CurrentKnownCouncilors(true, null, true, false).ToDictionary<TICouncilorState, TICouncilorState, TIGameState>((TICouncilorState x) => x, (TICouncilorState x) => faction.GetViewofCouncilor(x).currentMissionTarget);
			List<TINationState> list3 = faction.executiveNations.OrderByDescending<TINationState, int>((TINationState x) => x.claims.Count).ToList<TINationState>();
			float availableCPCap = faction.AvailableCPCapSpace();
			bool flag2 = TIEffectsState.SumEffectsModifiers(Context.DetectAlienSpaceAssetsRange, faction, 0f, null) >= 60f && !GameStateManager.AlienFaction().AI_AtWarWithFaction(faction);
			TICouncilorState councilor2 = councilor;
			List<TIMissionTemplate> list4 = ((councilor2 != null) ? councilor2.GetPossibleMissionList(true, false, true, null, false) : null) ?? new List<TIMissionTemplate>();
			bool currentlyDetectingHydra = faction.currentlyDetectingHydra;
			if (allowedAdviceTypes == null)
			{
				allowedAdviceTypes = Enum.GetValues(typeof(TIFactionState.Advice)).Cast<TIFactionState.Advice>().ToList<TIFactionState.Advice>();
			}
			Func<TINationState, bool> <>9__21;
			Func<TINationState, float> <>9__22;
			Func<TINationState, bool> <>9__23;
			Func<TIOrgState, bool> <>9__7;
			Func<TIOrgState, float> <>9__26;
			Func<TIOrgState, bool> <>9__27;
			Func<TIOrgState, float> <>9__29;
			Func<TISpaceBodyState, bool> <>9__30;
			Func<TIProjectTemplate, float> <>9__33;
			Func<TIFactionState, bool> <>9__40;
			Func<TIFactionState, float> <>9__41;
			Func<TIProjectTemplate, float> <>9__42;
			Func<TIOrgState, bool> <>9__44;
			Func<TIFactionState, bool> <>9__43;
			Func<TIFactionState, float> <>9__45;
			Func<TIOrgState, bool> <>9__46;
			Func<TIControlPoint, bool> <>9__49;
			Func<TIControlPoint, float> <>9__50;
			Func<TIControlPoint, float> <>9__52;
			Func<TIFactionState, bool> <>9__55;
			Func<TIFactionState, bool> <>9__10;
			Func<TIFactionState, bool> <>9__11;
			Func<TIMissionTemplate, bool> <>9__62;
			foreach (TIFactionState.Advice advice in allowedAdviceTypes)
			{
				switch (advice)
				{
				case TIFactionState.Advice.CouncilorTargetedByEnemyMission:
				{
					if (!flag)
					{
						continue;
					}
					using (IEnumerator<KeyValuePair<TICouncilorState, TIGameState>> enumerator2 = dictionary.Where<KeyValuePair<TICouncilorState, TIGameState>>((KeyValuePair<TICouncilorState, TIGameState> x) => x.Value != null).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<TICouncilorState, TIGameState> keyValuePair = enumerator2.Current;
							TIGameState value = keyValuePair.Value;
							if (value == councilor && !councilor.faction.permanentAlly(keyValuePair.Key.faction))
							{
								List<string> list5 = TIFactionState.friendlyCouncilorToCouncilorMissions;
								TIMissionState activeMission = keyValuePair.Key.activeMission;
								if (!list5.Contains((activeMission != null) ? activeMission.missionTemplate.dataName : null))
								{
									list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
									{
										keyValuePair.Key.faction.displayNameWithColor,
										keyValuePair.Key.activeMission.displayName,
										TIFactionState.goToGroundMission.displayName
									}), 20f, value));
								}
							}
						}
						continue;
					}
					break;
				}
				case TIFactionState.Advice.CouncilorCanLevelUp:
					break;
				case TIFactionState.Advice.FactionTargetedByEnemyMission:
				{
					using (IEnumerator<KeyValuePair<TICouncilorState, TIGameState>> enumerator2 = dictionary.Where<KeyValuePair<TICouncilorState, TIGameState>>((KeyValuePair<TICouncilorState, TIGameState> x) => x.Value != null).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<TICouncilorState, TIGameState> keyValuePair2 = enumerator2.Current;
							TIGameState value2 = keyValuePair2.Value;
							if (value2.ref_faction == faction && !value2.isCouncilorState && keyValuePair2.Key.activeMission.missionTemplate.hate[4] > 0f)
							{
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
								{
									keyValuePair2.Key.faction.displayNameWithColor,
									keyValuePair2.Key.activeMission.displayName,
									TIUtilities.GetLocationString(keyValuePair2.Value, false, true)
								}), 18f, value2));
							}
						}
						continue;
					}
					goto IL_04F5;
				}
				case TIFactionState.Advice.FactionNationWithHighestUnrest:
					goto IL_04F5;
				case TIFactionState.Advice.FactionNationLargeSubmitPOIncrease:
					goto IL_05D8;
				case TIFactionState.Advice.FactionNationLargePOLoss:
					goto IL_0696;
				case TIFactionState.Advice.FactionNationUndefendedCPs:
					goto IL_0755;
				case TIFactionState.Advice.FactionNationBadXenoforming:
					goto IL_081B;
				case TIFactionState.Advice.FactionNationLowCohesion:
					goto IL_08D0;
				case TIFactionState.Advice.FactionNationsCanFederate:
					goto IL_0959;
				case TIFactionState.Advice.FactionNationsCanUnify:
					goto IL_0A67;
				case TIFactionState.Advice.FactionNationHighCoupChance:
					goto IL_0B7C;
				case TIFactionState.Advice.FactionNeededMiningSiteAvailable:
					goto IL_0BF9;
				case TIFactionState.Advice.AccessibleMostPopularNationWithNeutralCPs:
					goto IL_0DAC;
				case TIFactionState.Advice.AccessibleControlPointWithHighestBoost:
				{
					if (availableCPCap <= 0f || AIEvaluators.Abundant(faction, FactionResource.Boost, 1f))
					{
						continue;
					}
					IEnumerable<TINationState> enumerable = GameStateManager.AllExtantHumanNations();
					Func<TINationState, bool> func;
					if ((func = <>9__23) == null)
					{
						func = (<>9__23 = (TINationState x) => (x.NumNativeControlPoints >= 1 || (x.NumNativeControlPoints == 1 && x.numControlPoints == 1)) && x.boostIncome_month_dekatons > 0f && !x.atWar && x.ControlPointMaintenanceCost <= availableCPCap && faction.GetBestNotionalMissionSuccessChance(TIFactionState.controlNationMission, x, null) > 0.2f);
					}
					IEnumerable<TINationState> enumerable2 = enumerable.Where<TINationState>(func);
					TINationState tinationState;
					if (enumerable2 == null)
					{
						tinationState = null;
					}
					else
					{
						tinationState = enumerable2.MaxBy<TINationState, float>((TINationState x) => x.GetMonthlyBoostIncomeFromControlPoint());
					}
					TINationState tinationState2 = tinationState;
					if (tinationState2 != null)
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState2.displayNameWithArticleAndPlacePrep }), (float)(tinationState2.NumNativeControlPoints * 2), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionStockpileSufficentForLEOStation:
					if (faction.LEOStations.Count == 0 && faction.AvailableMissionControl > 0)
					{
						TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>("PlatformCore", false);
						using (List<TIOrbitState>.Enumerator enumerator3 = GameStateManager.LEOStates().GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								TIOrbitState tiorbitState = enumerator3.Current;
								if (tiorbitState.NewStationAllowed(1, null) && tihabModuleTemplate.CostFromEarth(faction, tiorbitState, false).CanAfford(faction, 1f, null, float.PositiveInfinity))
								{
									list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tiorbitState.displayName }), 20f, null));
									break;
								}
							}
							continue;
						}
						goto IL_1062;
					}
					continue;
				case TIFactionState.Advice.FactionStockpileSufficentForMoonOutpost:
					goto IL_1062;
				case TIFactionState.Advice.FactionStockpileSufficentForMarsOutpost:
				{
					TISpaceBodyState Mars = GameStateManager.Mars();
					if (faction.bases.Where<TIHabState>((TIHabState x) => x.ref_spaceBody == Mars).Count<TIHabState>() != 0 || faction.AvailableMissionControl <= 0 || !faction.Prospected(Mars) || !Mars.hasAvailableHabSites)
					{
						continue;
					}
					TIHabModuleTemplate tihabModuleTemplate2 = TemplateManager.Find<TIHabModuleTemplate>("OutpostCore", false);
					if (!tihabModuleTemplate2.FactionCanBuild(faction))
					{
						continue;
					}
					TIHabModuleTemplate tihabModuleTemplate3 = TemplateManager.Find<TIHabModuleTemplate>("FissionPile", false);
					if (!tihabModuleTemplate3.FactionCanBuild(faction))
					{
						continue;
					}
					TIHabModuleTemplate tihabModuleTemplate4 = TemplateManager.Find<TIHabModuleTemplate>("OutpostMiningComplex", false);
					if (!tihabModuleTemplate4.FactionCanBuild(faction))
					{
						continue;
					}
					TIResourcesCost tiresourcesCost = tihabModuleTemplate2.CostFromSpace(faction, Mars, false, false, 0, false);
					tiresourcesCost.SumCosts_NoDuration(tihabModuleTemplate3.CostFromSpace(faction, Mars, false, false, 0, false));
					tiresourcesCost.SumCosts_NoDuration(tihabModuleTemplate4.CostFromSpace(faction, Mars, false, false, 0, false));
					if (tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity) || tiresourcesCost.GetBoostSubstitutedCost(faction, Mars, true, null).CanAfford(faction, 1f, null, float.PositiveInfinity))
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { Mars.displayName }), 20f, null));
						continue;
					}
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_2"), new object[]
					{
						Mars.displayName,
						tiresourcesCost.GetString("Relevant", true, false, false, 7, false, false, faction, false, FactionResource.None)
					}), 19f, null));
					continue;
				}
				case TIFactionState.Advice.FactionMCUsedInvitesAlienAttack:
				{
					if (!flag2)
					{
						continue;
					}
					float num = AIEvaluators.FactionsGoToWarProgress(GameStateManager.AlienFaction(), faction);
					if (Utilities.Between((double)num, 0.8, 1.0, false, false))
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_2")), 20f * num, null));
						continue;
					}
					if (Utilities.Between((double)num, 0.6, 0.8, true, true))
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_3")), 20f * num, null));
						continue;
					}
					if (faction.ShouldWorryAboutMCBasedAlienHate())
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), 10f, null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.AffordableT3Org:
				{
					IEnumerable<TIOrgState> enumerable3 = faction.availableOrgs;
					Func<TIOrgState, bool> func2;
					if ((func2 = <>9__7) == null)
					{
						func2 = (<>9__7 = (TIOrgState x) => x.tier == 3 && x.GetPurchaseCost(faction).CanAfford(faction, 1f, null, float.PositiveInfinity) && faction.councilors.Any<TICouncilorState>((TICouncilorState y) => x.CouncilorCanAcquire(y)));
					}
					IEnumerable<TIOrgState> enumerable4 = enumerable3.Where<TIOrgState>(func2);
					if (enumerable4.Any<TIOrgState>())
					{
						IEnumerable<TIOrgState> enumerable5 = enumerable4;
						Func<TIOrgState, float> func3;
						if ((func3 = <>9__26) == null)
						{
							func3 = (<>9__26 = (TIOrgState x) => AIEvaluators.EvaluateOrgForTrade(x, faction));
						}
						TIOrgState tiorgState = enumerable5.MaxBy<TIOrgState, float>(func3);
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tiorgState.displayNameWithArticle }), 5f, null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.AffordableProjectOrgToUnlockSlot:
				{
					if (faction.orgProjectSlotUnlocked || faction.GetMonthlyIncome(FactionResource.Research, false, false) <= 100f)
					{
						continue;
					}
					if (!faction.unassignedOrgs.None<TIOrgState>((TIOrgState x) => x.projectCapacityGranted > 0))
					{
						continue;
					}
					IEnumerable<TIOrgState> enumerable6 = faction.availableOrgs;
					Func<TIOrgState, bool> func4;
					if ((func4 = <>9__27) == null)
					{
						func4 = (<>9__27 = (TIOrgState x) => x.projectCapacityGranted > 0 && x.GetPurchaseCost(faction).CanAfford(faction, 1f, null, float.PositiveInfinity) && faction.councilors.Any<TICouncilorState>((TICouncilorState y) => x.CouncilorCanAcquire(y)));
					}
					IEnumerable<TIOrgState> enumerable7 = enumerable6.Where<TIOrgState>(func4);
					if (enumerable7.Any<TIOrgState>())
					{
						IEnumerable<TIOrgState> enumerable8 = enumerable7;
						Func<TIOrgState, float> func5;
						if ((func5 = <>9__29) == null)
						{
							func5 = (<>9__29 = (TIOrgState x) => AIEvaluators.EvaluateOrgForTrade(x, faction));
						}
						TIOrgState tiorgState2 = enumerable8.MaxBy<TIOrgState, float>(func5);
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tiorgState2.displayNameWithArticle }), 8f, null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.AffordableProbesToRegionWithNoProbes:
				{
					using (List<List<TISpaceBodyState>>.Enumerator enumerator4 = GameStateManager.ColonizableSpaceBodiesByRegion().GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							List<TISpaceBodyState> list6 = enumerator4.Current;
							IEnumerable<TISpaceBodyState> enumerable9 = list6;
							Func<TISpaceBodyState, bool> func6;
							if ((func6 = <>9__30) == null)
							{
								func6 = (<>9__30 = (TISpaceBodyState x) => faction.CandidateForProspecting(x));
							}
							if (enumerable9.All<TISpaceBodyState>(func6))
							{
								string text = string.Empty;
								int num2 = 0;
								bool flag3 = false;
								if (list6[0] == GameStateManager.Mars())
								{
									text = Loc.T("UI.Habs.MartianSystem");
									num2 = 19;
								}
								else if (GameStateManager.Jupiter().naturalSatellites.Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.JupiterSystem");
									flag3 = true;
									num2 = 14;
								}
								else if (GameStateManager.Saturn().naturalSatellites.Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.SaturnSystem");
									flag3 = true;
									num2 = 10;
								}
								else if (GameStateManager.Uranus().naturalSatellites.Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.UranusSystem");
									flag3 = true;
									num2 = 3;
								}
								else if (GameStateManager.Neptune().naturalSatellites.Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.NeptuneSystem");
									flag3 = true;
									num2 = 2;
								}
								else if (GameStateManager.InnerSystemAsteroids(false).Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.InnerSystemAsteroids");
									num2 = 15;
								}
								else if (GameStateManager.InnerAsteroidBelt(false).Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.InnerBelt");
									flag3 = true;
									num2 = 16;
								}
								else if (GameStateManager.MidAsteroidBelt(false).Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.MidBelt");
									flag3 = true;
									num2 = 16;
								}
								else if (GameStateManager.OuterAsteroidBelt(false).Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.FarBelt");
									flag3 = true;
									num2 = 16;
								}
								else if (GameStateManager.Centaurs(false).Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.Centaurs");
									num2 = 1;
								}
								else if (GameStateManager.KuiperBeltObjects(false).Contains(list6[0]))
								{
									text = Loc.T("UI.Habs.KBO");
									flag3 = true;
									num2 = 1;
								}
								else
								{
									text = list6[0].displayName;
								}
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(flag3 ? TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "") : TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_2"), new object[] { text }), (float)num2, null));
							}
						}
						continue;
					}
					goto IL_1A38;
				}
				case TIFactionState.Advice.AvailableAntiEnthrallProject:
					goto IL_1A38;
				case TIFactionState.Advice.AlienVisible:
				{
					using (List<TICouncilorState>.Enumerator enumerator5 = faction.CurrentKnownCouncilors(true, new List<TIFactionState> { GameStateManager.AlienFaction() }, false, true).GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							TICouncilorState alien = enumerator5.Current;
							if (alien.OnEarth)
							{
								if (faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.HasMission).None<TICouncilorState>((TICouncilorState x) => x.activeMission.target == alien))
								{
									list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { TIUtilities.GetLocationString(alien.location, true, true) }), 20f, alien));
								}
							}
						}
						continue;
					}
					goto IL_1C0B;
				}
				case TIFactionState.Advice.SurveillanceFleetAtEarth:
					goto IL_1C0B;
				case TIFactionState.Advice.SurveillanceHabAtEarth:
				{
					if (faction.IsAlienProxy)
					{
						continue;
					}
					IEnumerable<TIHabState> enumerable10 = GameStateManager.Earth().habsInSystem.Where<TIHabState>((TIHabState x) => x.IsAlien());
					if (enumerable10.Count<TIHabState>() > 0)
					{
						TIHabState tihabState = enumerable10.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
						{
							tihabState.GetDisplayName(faction),
							TIUtilities.GetLocationString(tihabState.location, false, true)
						}), (float)(tihabState.tier * 5), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.VulnerableEnemyProject:
				{
					if (!list4.Contains(TIFactionState.stealProjectMission))
					{
						continue;
					}
					IEnumerable<TIFactionState> enumerable11 = list2;
					Func<TIFactionState, bool> func7;
					if ((func7 = <>9__40) == null)
					{
						func7 = (<>9__40 = (TIFactionState x) => x.StealableProjects(faction).Count > 0);
					}
					IEnumerable<TIFactionState> enumerable12 = enumerable11.Where<TIFactionState>(func7);
					TIFactionState tifactionState;
					if (enumerable12 == null)
					{
						tifactionState = null;
					}
					else
					{
						Func<TIFactionState, float> func8;
						if ((func8 = <>9__41) == null)
						{
							func8 = (<>9__41 = (TIFactionState x) => faction.GetFactionHate(x));
						}
						tifactionState = enumerable12.MaxBy<TIFactionState, float>(func8);
					}
					TIFactionState tifactionState2 = tifactionState;
					if (tifactionState2 != null)
					{
						IEnumerable<TIProjectTemplate> enumerable13 = tifactionState2.StealableProjects(faction);
						Func<TIProjectTemplate, float> func9;
						if ((func9 = <>9__42) == null)
						{
							func9 = (<>9__42 = (TIProjectTemplate x) => x.GetResearchCost(faction));
						}
						TIProjectTemplate tiprojectTemplate = enumerable13.MaxBy<TIProjectTemplate, float>(func9);
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tiprojectTemplate.displayName, tifactionState2.displayName }), 5f, null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.VulnerableEnemyOrg:
				{
					if (!list4.Contains(TIFactionState.hostileTakeoverMission))
					{
						continue;
					}
					IEnumerable<TIFactionState> enumerable14 = list2;
					Func<TIFactionState, bool> func10;
					if ((func10 = <>9__43) == null)
					{
						func10 = (<>9__43 = delegate(TIFactionState x)
						{
							IEnumerable<TIOrgState> stealableOrgs2 = x.GetStealableOrgs(councilor);
							Func<TIOrgState, bool> func23;
							if ((func23 = <>9__44) == null)
							{
								func23 = (<>9__44 = (TIOrgState x) => faction.GetBestNotionalMissionSuccessChance(TIFactionState.hostileTakeoverMission, x, new List<TICouncilorState> { councilor }) > 0.4f);
							}
							return stealableOrgs2.Any<TIOrgState>(func23);
						});
					}
					IEnumerable<TIFactionState> enumerable15 = enumerable14.Where<TIFactionState>(func10);
					if (!enumerable15.Any<TIFactionState>())
					{
						continue;
					}
					IEnumerable<TIFactionState> enumerable16 = enumerable15;
					Func<TIFactionState, float> func11;
					if ((func11 = <>9__45) == null)
					{
						func11 = (<>9__45 = (TIFactionState x) => faction.GetFactionHate(x));
					}
					TIFactionState tifactionState3 = enumerable16.MaxBy<TIFactionState, float>(func11);
					if (!(tifactionState3 != null))
					{
						continue;
					}
					IEnumerable<TIOrgState> stealableOrgs = tifactionState3.GetStealableOrgs(councilor);
					Func<TIOrgState, bool> func12;
					if ((func12 = <>9__46) == null)
					{
						func12 = (<>9__46 = (TIOrgState x) => faction.GetBestNotionalMissionSuccessChance(TIFactionState.hostileTakeoverMission, x, new List<TICouncilorState> { councilor }) > 0.4f);
					}
					IEnumerable<TIOrgState> enumerable17 = stealableOrgs.Where<TIOrgState>(func12);
					if (!enumerable17.Any<TIOrgState>())
					{
						continue;
					}
					TIOrgState tiorgState3 = enumerable17.MaxBy<TIOrgState, int>((TIOrgState x) => x.tier);
					if (!(tiorgState3 != null))
					{
						continue;
					}
					if (tiorgState3.hasCouncilor)
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
						{
							tifactionState3.adjectiveWithColor,
							tiorgState3.assignedCouncilor.displayName,
							tiorgState3.displayName
						}), (float)(3 * tiorgState3.tier), councilor));
						continue;
					}
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_Pool"), new object[] { tifactionState3.adjectiveWithColor, tiorgState3.displayName }), (float)(3 * tiorgState3.tier), councilor));
					continue;
				}
				case TIFactionState.Advice.VulnerableEnemyCP:
				{
					if (availableCPCap <= 0f || !list4.Contains(TIFactionState.purgeMission))
					{
						continue;
					}
					IEnumerable<TIControlPoint> enumerable18 = list2.SelectMany<TIFactionState, TIControlPoint>((TIFactionState x) => x.controlPoints);
					Func<TIControlPoint, bool> func13;
					if ((func13 = <>9__49) == null)
					{
						func13 = (<>9__49 = (TIControlPoint x) => !x.defended && availableCPCap >= x.BaselineMaintenanceCost && faction.GetBestNotionalMissionSuccessChance(TIFactionState.purgeMission, x, new List<TICouncilorState> { councilor }) > 0.4f);
					}
					IEnumerable<TIControlPoint> enumerable19 = enumerable18.Where<TIControlPoint>(func13);
					if (enumerable19.Any<TIControlPoint>())
					{
						TIFactionState.<>c__DisplayClass1351_6 CS$<>8__locals4 = new TIFactionState.<>c__DisplayClass1351_6();
						TIFactionState.<>c__DisplayClass1351_6 CS$<>8__locals5 = CS$<>8__locals4;
						IEnumerable<TIControlPoint> enumerable20 = enumerable19;
						Func<TIControlPoint, float> func14;
						if ((func14 = <>9__50) == null)
						{
							func14 = (<>9__50 = (TIControlPoint x) => faction.GetFactionHate(x.faction));
						}
						CS$<>8__locals5.targetFaction = enumerable20.MaxBy<TIControlPoint, float>(func14).faction;
						IEnumerable<TIControlPoint> enumerable21 = enumerable19.Where<TIControlPoint>((TIControlPoint x) => x.faction == CS$<>8__locals4.targetFaction);
						Func<TIControlPoint, float> func15;
						if ((func15 = <>9__52) == null)
						{
							func15 = (<>9__52 = (TIControlPoint x) => AIEvaluators.EvaluateControlPoint(faction, x));
						}
						TIControlPoint ticontrolPoint = enumerable21.MaxBy<TIControlPoint, float>(func15);
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
						{
							ticontrolPoint.nation.displayNameWithArticleAndPlacePrep,
							ticontrolPoint.faction.adjectiveWithColor
						}), (float)(3 * ticontrolPoint.nation.numControlPoints), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionBehindOnHumanShipTech:
				{
					IEnumerable<TIFactionState> enumerable22 = list2.Where<TIFactionState>((TIFactionState x) => x.ships.Count > 0);
					if (enumerable22.Count<TIFactionState>() >= 2)
					{
						float myBestShipScore = -1f;
						if (faction.ships.Count > 0)
						{
							myBestShipScore = faction.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
						}
						Func<TISpaceShipState, bool> <>9__56;
						if (!enumerable22.All<TIFactionState>(delegate(TIFactionState x)
						{
							IEnumerable<TISpaceShipState> ships = x.ships;
							Func<TISpaceShipState, bool> func24;
							if ((func24 = <>9__56) == null)
							{
								func24 = (<>9__56 = (TISpaceShipState x) => x.SpaceCombatValue(false, 0f) > myBestShipScore);
							}
							return ships.Any<TISpaceShipState>(func24);
						}))
						{
							IEnumerable<TIFactionState> enumerable23 = enumerable22;
							Func<TIFactionState, bool> func16;
							if ((func16 = <>9__55) == null)
							{
								func16 = (<>9__55 = (TIFactionState x) => x.completedProjects.Count<TIProjectTemplate>((TIProjectTemplate x) => x.AI_techRole == TechRole.SpaceWar) > faction.completedProjects.Count<TIProjectTemplate>((TIProjectTemplate x) => x.AI_techRole == TechRole.SpaceWar));
							}
							if (!enumerable23.All<TIFactionState>(func16))
							{
								continue;
							}
						}
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), (float)enumerable22.Count<TIFactionState>() * 2f, null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionBehindOnHumanFleetSizes:
				{
					IEnumerable<TIFactionState> enumerable24 = list2;
					Func<TIFactionState, bool> func17;
					if ((func17 = <>9__10) == null)
					{
						func17 = (<>9__10 = (TIFactionState x) => x.ships.Count > faction.ships.Count);
					}
					if (enumerable24.All<TIFactionState>(func17))
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), Mathf.Min((float)TITimeState.CampaignDuration_CompleteYears() / 2f, 16f), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionBehindOnMineProduction:
				{
					IEnumerable<TIFactionState> enumerable25 = list2;
					Func<TIFactionState, bool> func18;
					if ((func18 = <>9__11) == null)
					{
						func18 = (<>9__11 = (TIFactionState x) => x.habs.Count<TIHabState>((TIHabState x) => x.HasMine) > faction.habs.Count<TIHabState>((TIHabState x) => x.HasMine));
					}
					if (enumerable25.All<TIFactionState>(func18))
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), Mathf.Min((float)TITimeState.CampaignDuration_CompleteYears() / 2f, 16f), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionBehindOnTechCategoryBonus:
				{
					TechCategory[] techCategories = Enums.TechCategories;
					for (int i = 0; i < techCategories.Length; i++)
					{
						TechCategory techCategory = techCategories[i];
						if (list2.All<TIFactionState>((TIFactionState x) => x.SumCategoryModifiers(techCategory) > faction.SumCategoryModifiers(techCategory)))
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
							{
								TIGenericTechTemplate.GetTechCategoryString(techCategory),
								TIGenericTechTemplate.categoryInlineSprite(techCategory)
							}), Mathf.Min((float)TITimeState.CampaignDuration_CompleteYears() / 2f, 14f), null));
						}
					}
					continue;
				}
				case TIFactionState.Advice.FactionBehindOnSettlingRegion:
					if (faction.bases.Count == 0 && GameStateManager.Luna().vacantHabSites.Count > 0 && GameStateManager.Luna().vacantHabSites.Count <= GameStateManager.Luna().habSites.Length - list2.Count<TIFactionState>())
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_Luna"), new object[] { GameStateManager.Luna().displayName }), (float)GameStateManager.Luna().occupiedHabSites.Count * 2f, null));
						continue;
					}
					if (faction.bases.None<TIHabState>((TIHabState x) => x.ref_spaceBody == GameStateManager.Mars()) && GameStateManager.Mars().vacantHabSites.Count > 0 && GameStateManager.Mars().vacantHabSites.Count <= 4)
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_Mars"), new object[] { GameStateManager.Mars().displayName }), (float)Mathf.Min(GameStateManager.Mars().occupiedHabSites.Count, 19), null));
						continue;
					}
					continue;
				case TIFactionState.Advice.FactionCPCapOverMax:
				{
					float num3 = faction.GetAnnualControlPointMaintenanceCost() / 12f;
					if (num3 > 1f && (num3 > faction.GetMonthlyIncome(FactionResource.Influence, false, false) * 0.05f || faction.GetCurrentResourceAmount(FactionResource.Influence) < num3 * 3f))
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { num3.ToString("N1") }), Mathf.Min(num3 / 5f, 16f), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionMCCapOverMax:
				{
					int missionControlShortage = faction.MissionControlShortage;
					if (missionControlShortage > 0)
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), (float)Mathf.Min(missionControlShortage, 19), null));
						continue;
					}
					continue;
				}
				case TIFactionState.Advice.FactionMissingCriticalMission:
				{
					List<TIMissionTemplate> list7 = faction.ObjectiveCriticalMissions();
					IEnumerable<TIMissionTemplate> enumerable26 = faction.RequiredMissions(true).Distinct<TIMissionTemplate>();
					Func<TIMissionTemplate, bool> func19;
					if ((func19 = <>9__62) == null)
					{
						func19 = (<>9__62 = (TIMissionTemplate x) => faction.councilors.None<TICouncilorState>((TICouncilorState y) => y.GetPossibleMissionList(false, false, true, null, false).Contains(x)));
					}
					using (IEnumerator<TIMissionTemplate> enumerator6 = enumerable26.Where<TIMissionTemplate>(func19).GetEnumerator())
					{
						while (enumerator6.MoveNext())
						{
							TIMissionTemplate mission = enumerator6.Current;
							IEnumerable<TIOrgState> enumerable27 = faction.availableOrgs.Where<TIOrgState>((TIOrgState x) => x.missionsGranted.Contains(mission));
							IEnumerable<TICouncilorState> enumerable28 = faction.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, true, null, false).Contains(mission));
							if (enumerable27.Any<TIOrgState>())
							{
								List<TIFactionState.AdviceData> list8 = list;
								TIFactionState.Advice advice2 = advice;
								string text2 = TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_org");
								object[] array = new object[2];
								array[0] = mission.displayName;
								array[1] = enumerable27.MinBy<TIOrgState, int>((TIOrgState x) => x.tier).displayName;
								list8.Add(new TIFactionState.AdviceData(advice2, Loc.T(text2, array), (float)(list7.Contains(mission) ? 20 : 15), null));
							}
							else if (enumerable28.Any<TICouncilorState>())
							{
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_councilor"), new object[]
								{
									mission.displayName,
									enumerable28.SelectRandomItem<TICouncilorState>().displayName
								}), (float)(list7.Contains(mission) ? 20 : 15), null));
							}
							else
							{
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { mission.displayName }), (float)(list7.Contains(mission) ? 20 : 15), null));
							}
						}
						continue;
					}
					goto IL_2985;
				}
				case TIFactionState.Advice.FactionUsingBoostToSupportHabs:
					goto IL_2985;
				default:
					continue;
				}
				if (flag && councilor.CanAffordAnyCandidateAugmentations(true) && (float)councilor.XP >= (float)TemplateManager.global.XPToLevelUp * (1f + councilor.XPModifier))
				{
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), 12f, councilor));
					continue;
				}
				continue;
				IL_04F5:
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState3 = enumerator7.Current;
						if (tinationState3.unrest - tinationState3.historyUnrest[31] >= 1f)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_2"), new object[] { tinationState3.displayNameWithArticleAndPlacePrep }), (float)tinationState3.numControlPoints_unclamped + tinationState3.unrest, tinationState3));
						}
						else if (tinationState3.unrest > 6f)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState3.displayNameWithArticleAndPlacePrep }), (float)tinationState3.numControlPoints_unclamped + tinationState3.unrest, tinationState3));
						}
					}
					continue;
				}
				IL_05D8:
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState4 = enumerator7.Current;
						float publicOpinionOfFaction = tinationState4.GetPublicOpinionOfFaction(GameStateManager.AlienProxy());
						if (publicOpinionOfFaction - tinationState4.historyPublicOpinion[31][GameStateManager.AlienProxy().ideology.ideology] >= 0.1f)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
							{
								GameStateManager.AlienProxy().displayNameCapitalizedWithColor,
								tinationState4.displayNameWithArticleAndPlacePrep
							}), (float)tinationState4.numControlPoints_unclamped + publicOpinionOfFaction * 10f, tinationState4));
						}
					}
					continue;
				}
				IL_0696:
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState5 = enumerator7.Current;
						float publicOpinionOfFaction2 = tinationState5.GetPublicOpinionOfFaction(faction);
						if (publicOpinionOfFaction2 - tinationState5.historyPublicOpinion[31][faction.ideology.ideology] <= -0.1f)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState5.displayNameWithArticleAndPlacePrep }), (float)tinationState5.numControlPoints_unclamped + publicOpinionOfFaction2 * 10f, tinationState5));
						}
					}
					continue;
				}
				IL_0755:
				if (!list4.Contains(TIFactionState.defendInterestsMission))
				{
					continue;
				}
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState6 = enumerator7.Current;
						int count = tinationState6.FactionControlPoints(faction, false, false, false).Count;
						if (count > 0 && faction.GetBestNotionalMissionSuccessChance(TIFactionState.defendInterestsMission, tinationState6, null) > 0f)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState6.displayNameWithArticleAndPlacePrep }), (float)(tinationState6.numControlPoints_unclamped + count * 2), tinationState6));
						}
					}
					continue;
				}
				IL_081B:
				using (IEnumerator<TIRegionState> enumerator8 = list3.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions).GetEnumerator())
				{
					while (enumerator8.MoveNext())
					{
						TIRegionState tiregionState = enumerator8.Current;
						if (tiregionState.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tiregionState.displayNameSentIn }), (float)tiregionState.nation.numControlPoints_unclamped + tiregionState.xenoforming.xenoformingLevel / 10f, tiregionState));
						}
					}
					continue;
				}
				IL_08D0:
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState7 = enumerator7.Current;
						if (tinationState7.majorCohesionWarning)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
							{
								tinationState7.displayNameWithArticleAndPlacePrep,
								TIGlobalConfig.globalConfig.cohesionInlineSpritePath
							}), (float)(tinationState7.numControlPoints_unclamped + 10) - tinationState7.cohesion, tinationState7));
						}
					}
					continue;
				}
				IL_0959:
				Dictionary<TINationState, List<TINationState>> dictionary2 = new Dictionary<TINationState, List<TINationState>>();
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState8 = enumerator7.Current;
						dictionary2.Add(tinationState8, new List<TINationState>());
						foreach (TINationState tinationState9 in list3)
						{
							if (tinationState8 != tinationState9 && tinationState8.CanFormFederation(tinationState9) && (!dictionary2.ContainsKey(tinationState9) || !dictionary2[tinationState9].Contains(tinationState8)))
							{
								dictionary2[tinationState8].Add(tinationState9);
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState8.displayNameWithArticleCapitalized, tinationState9.displayNameWithArticle }), (float)(tinationState8.numControlPoints_unclamped + tinationState9.numControlPoints_unclamped), tinationState8));
							}
						}
					}
					continue;
				}
				IL_0A67:
				Dictionary<TINationState, List<TINationState>> dictionary3 = new Dictionary<TINationState, List<TINationState>>();
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState10 = enumerator7.Current;
						dictionary3.Add(tinationState10, new List<TINationState>());
						foreach (TINationState tinationState11 in list3)
						{
							if (tinationState10 != tinationState11 && tinationState10.eligibleUnifications.Contains(tinationState11) && (!dictionary3.ContainsKey(tinationState11) || !dictionary3[tinationState11].Contains(tinationState10)))
							{
								dictionary3[tinationState10].Add(tinationState11);
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState10.displayNameWithArticleCapitalized, tinationState11.displayNameWithArticle }), (float)(tinationState10.numControlPoints_unclamped + tinationState11.numControlPoints_unclamped + 1), tinationState10));
							}
						}
					}
					continue;
				}
				IL_0B7C:
				using (List<TINationState>.Enumerator enumerator7 = list3.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						TINationState tinationState12 = enumerator7.Current;
						float num4 = tinationState12.PeriodicOrganicCoupChance();
						if (num4 > 0f)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState12.displayNameWithArticleCapitalized }), (float)tinationState12.numControlPoints_unclamped + num4, tinationState12));
						}
					}
					continue;
				}
				IL_0BF9:
				using (HashSet<FactionResource>.Enumerator enumerator10 = TIResourcesCost.basicSpaceResources.GetEnumerator())
				{
					while (enumerator10.MoveNext())
					{
						FactionResource resource = enumerator10.Current;
						float dailyIncome = faction.GetDailyIncome(resource, true, true);
						IEnumerable<float> enumerable29 = from x in GameStateManager.AllHumanFactions()
							select x.GetDailyIncome(resource, true, true);
						if (dailyIncome >= 0f)
						{
							if (!enumerable29.Any<float>((float x) => x > 0f) || dailyIncome != enumerable29.Min())
							{
								continue;
							}
						}
						List<TISpaceBodyState> list9 = faction.ProspectedSpaceBodies();
						if (list9.Count > 0)
						{
							IEnumerable<TIHabSiteState> enumerable30 = from x in list9.SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites)
								where !x.hasPlannedOrOperatingBase
								select x;
							TIHabSiteState tihabSiteState = ((enumerable30 != null) ? enumerable30.MaxBy<TIHabSiteState, float>((TIHabSiteState x) => x.GetDailyProduction(resource)) : null);
							if (tihabSiteState != null && tihabSiteState.GetDailyProduction(resource) > 0f)
							{
								list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
								{
									TIUtilities.GetResourceString(resource),
									tihabSiteState.displayName,
									tihabSiteState.parentBody.displayName
								}), 12f, tihabSiteState));
							}
						}
					}
					continue;
				}
				IL_0DAC:
				if (availableCPCap <= 0f)
				{
					continue;
				}
				IEnumerable<TINationState> enumerable31 = GameStateManager.AllExtantHumanNations();
				Func<TINationState, bool> func20;
				if ((func20 = <>9__21) == null)
				{
					func20 = (<>9__21 = (TINationState x) => x.NumNativeControlPoints >= 2 && x.ControlPointMaintenanceCost <= availableCPCap && faction.GetBestNotionalMissionSuccessChance(TIFactionState.controlNationMission, x, null) > 0.2f);
				}
				IEnumerable<TINationState> enumerable32 = enumerable31.Where<TINationState>(func20);
				TINationState tinationState13;
				if (enumerable32 == null)
				{
					tinationState13 = null;
				}
				else
				{
					Func<TINationState, float> func21;
					if ((func21 = <>9__22) == null)
					{
						func21 = (<>9__22 = (TINationState x) => x.GetPublicOpinionOfFaction(faction.ideology));
					}
					tinationState13 = enumerable32.MaxBy<TINationState, float>(func21);
				}
				TINationState tinationState14 = tinationState13;
				if (tinationState14 != null)
				{
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tinationState14.displayNameWithArticleAndPlacePrep }), (float)(tinationState14.NumNativeControlPoints * 2), null));
					continue;
				}
				continue;
				IL_1062:
				TISpaceBodyState tispaceBodyState = GameStateManager.Luna();
				if (faction.bases.Count != 0 || faction.AvailableMissionControl <= 0 || !faction.Prospected(tispaceBodyState) || !tispaceBodyState.hasAvailableHabSites)
				{
					continue;
				}
				TIHabModuleTemplate tihabModuleTemplate5 = TemplateManager.Find<TIHabModuleTemplate>("OutpostCore", false);
				if (!tihabModuleTemplate5.FactionCanBuild(faction))
				{
					continue;
				}
				TIHabModuleTemplate tihabModuleTemplate6 = TemplateManager.Find<TIHabModuleTemplate>("SolarCollector", false);
				if (!tihabModuleTemplate6.FactionCanBuild(faction))
				{
					continue;
				}
				TIHabModuleTemplate tihabModuleTemplate7 = TemplateManager.Find<TIHabModuleTemplate>("OutpostMiningComplex", false);
				if (!tihabModuleTemplate7.FactionCanBuild(faction))
				{
					continue;
				}
				TIResourcesCost tiresourcesCost2 = tihabModuleTemplate5.CostFromEarth(faction, tispaceBodyState, false);
				tiresourcesCost2.SumCosts_NoDuration(tihabModuleTemplate6.CostFromEarth(faction, tispaceBodyState, false));
				tiresourcesCost2.SumCosts_NoDuration(tihabModuleTemplate7.CostFromEarth(faction, tispaceBodyState, false));
				if (tiresourcesCost2.CanAfford(faction, 1f, null, float.PositiveInfinity))
				{
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tispaceBodyState.displayName }), 20f, null));
					continue;
				}
				list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_2"), new object[]
				{
					tispaceBodyState.displayName,
					tiresourcesCost2.GetString("Relevant", true, false, false, 7, false, false, faction, false, FactionResource.None)
				}), 19f, null));
				continue;
				IL_1A38:
				if (faction.veryProAlien)
				{
					continue;
				}
				IEnumerable<TIProjectTemplate> enumerable33 = faction.availableProjects.Where<TIProjectTemplate>((TIProjectTemplate x) => x.Effects.Any<TIEffectTemplate>((TIEffectTemplate x) => x.GetContexts().Contains(Context.Mission_EnthrallElites_Def)));
				if (enumerable33.Count<TIProjectTemplate>() > 0)
				{
					IEnumerable<TIProjectTemplate> enumerable34 = enumerable33;
					Func<TIProjectTemplate, float> func22;
					if ((func22 = <>9__33) == null)
					{
						func22 = (<>9__33 = (TIProjectTemplate x) => x.GetResearchCost(faction));
					}
					TIProjectTemplate tiprojectTemplate2 = enumerable34.MinBy<TIProjectTemplate, float>(func22);
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[] { tiprojectTemplate2.displayName }), 13f, null));
					continue;
				}
				continue;
				IL_1C0B:
				if (faction.IsAlienProxy)
				{
					continue;
				}
				IEnumerable<TISpaceFleetState> enumerable35 = GameStateManager.Earth().fleetsInInterfaceOrbits.Where<TISpaceFleetState>((TISpaceFleetState x) => x.HasSpecialModuleCapability(SpecialModuleRule.Surveillance));
				if (enumerable35.Any<TISpaceFleetState>())
				{
					TISpaceFleetState tispaceFleetState = enumerable35.MinBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue());
					list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, ""), new object[]
					{
						tispaceFleetState.GetDisplayName(faction),
						TIUtilities.GetLocationString(tispaceFleetState.location, false, true)
					}), (float)Mathf.Min(18, 10 + tispaceFleetState.ships.Count), null));
					continue;
				}
				continue;
				IL_2985:
				if (faction.habs.Count > 0 && faction.SubstitutingBoostForSpaceResource())
				{
					if (faction.CanExplore(GameStateManager.Luna()))
					{
						if (faction.MineNetworkSize > 0)
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "")), (float)((faction.DailyHabBoostShortage() > 0f) ? 19 : 11), null));
						}
						else
						{
							list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_Early")), (float)((faction.DailyHabBoostShortage() > 0f) ? 19 : (6 + faction.habs.Count)), null));
						}
					}
					else
					{
						list.Add(new TIFactionState.AdviceData(advice, Loc.T(TIFactionState.<GetAdvice>g__AdviceLocPath|1351_3(advice, "_Premining")), (float)((faction.DailyHabBoostShortage() > 0f) ? 19 : (6 + faction.habs.Count)), null));
					}
				}
			}
			List<TIFactionState.AdviceData> list10 = new List<TIFactionState.AdviceData>();
			IEnumerable<TIFactionState.AdviceData> enumerable36 = list.Where<TIFactionState.AdviceData>((TIFactionState.AdviceData x) => x.priority >= 20f);
			list10.AddRange(enumerable36);
			if (list10.Count < kount)
			{
				list = list.Where<TIFactionState.AdviceData>((TIFactionState.AdviceData x) => x.priority < 20f).ToList<TIFactionState.AdviceData>();
				int num5 = list10.Count;
				while (num5 < kount && list.Any<TIFactionState.AdviceData>())
				{
					TIFactionState.AdviceData adviceData = list.SelectRandomWeightedItem<TIFactionState.AdviceData>((TIFactionState.AdviceData x) => x.priority, -1f, 1E-37f);
					list.Remove(adviceData);
					list10.Add(adviceData);
					num5++;
				}
			}
			return list10;
		}

		// Token: 0x060034AA RID: 13482 RVA: 0x0012D9B8 File Offset: 0x0012BBB8
		public override bool Equals(object obj)
		{
			TIFactionState tifactionState = obj as TIFactionState;
			if (tifactionState == null)
			{
				return false;
			}
			if (this.isDummy || tifactionState.isDummy)
			{
				return this == tifactionState;
			}
			return base.Equals(obj);
		}

		// Token: 0x060034B1 RID: 13489 RVA: 0x0012DEEC File Offset: 0x0012C0EC
		[CompilerGenerated]
		internal static bool <MonthlyFactionUpdate>g__AlternateTriggersForAdvancedAlienTechPassed|337_0()
		{
			if (TIGlobalConfig.globalConfig.UseAlternateTriggersForAlienAdvancedTech())
			{
				int num = 0;
				IEnumerable<TIFactionState> enumerable = from x in GameStateManager.AllHumanFactions()
					where !x.veryProAlien
					select x;
				if (enumerable.Any<TIFactionState>((TIFactionState x) => x.habs.Any<TIHabState>((TIHabState x) => x.GetSunOrbitingRelatedObject.semiMajorAxis_AU >= GameStateManager.Jupiter().semiMajorAxis_AU)))
				{
					num++;
				}
				if (enumerable.Any<TIFactionState>((TIFactionState x) => x.habs.Any<TIHabState>((TIHabState x) => x.GetSunOrbitingRelatedObject.semiMajorAxis_AU >= GameStateManager.Saturn().semiMajorAxis_AU)))
				{
					num++;
				}
				if (enumerable.Any<TIFactionState>((TIFactionState x) => x.habs.Any<TIHabState>((TIHabState x) => x.GetSunOrbitingRelatedObject.semiMajorAxis_AU >= GameStateManager.Neptune().semiMajorAxis_AU)))
				{
					num++;
				}
				if (enumerable.Any<TIFactionState>((TIFactionState x) => x.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) > GameStateManager.AlienFaction().fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue())))
				{
					num++;
				}
				if (enumerable.Any<TIFactionState>((TIFactionState x) => (double)x.majorityControlNations.Sum<TINationState>((TINationState x) => (float)x.GDP) > TIGlobalValuesState.globalGDP * 0.5))
				{
					num++;
				}
				if (enumerable.Any<TIFactionState>((TIFactionState x) => x.finishedProjectNames.Contains("Project_ExoticHybridSystems")))
				{
					num++;
				}
				return num >= 7 - TIGlobalValuesState.GlobalValues.difficulty;
			}
			return false;
		}

		// Token: 0x060034B2 RID: 13490 RVA: 0x0012E048 File Offset: 0x0012C248
		[CompilerGenerated]
		internal static float <LogTransfer>g__GetNewEstimate|552_0(float oldEstimate, ref TIFactionState.<>c__DisplayClass552_0 A_1)
		{
			if (oldEstimate <= 0f)
			{
				return A_1.dvPerDay;
			}
			return Mathf.Lerp(oldEstimate, A_1.dvPerDay, 0.05f);
		}

		// Token: 0x060034BA RID: 13498 RVA: 0x0012E0EC File Offset: 0x0012C2EC
		[CompilerGenerated]
		internal static TIOrgState <ValidateAllOrgs>g__GetOrgToRemoveToRelieveAdministrationDeficit|619_0(TICouncilorState councilor, ref TIFactionState.<>c__DisplayClass619_0 A_1)
		{
			return (from x in councilor.orgs.Except<TIOrgState>(A_1.badOrgs)
				orderby x.administration
				orderby x.tier
				select x).First<TIOrgState>();
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x0012E157 File Offset: 0x0012C357
		[CompilerGenerated]
		internal static void <ValidateAllOrgs>g__AddBadOrg|619_3(TIOrgState badOrg, ref TIFactionState.<>c__DisplayClass619_0 A_1, ref TIFactionState.<>c__DisplayClass619_1 A_2)
		{
			A_1.badOrgs.Add(badOrg);
			A_2.availableAdministration += badOrg.tier - badOrg.administration;
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x0012E180 File Offset: 0x0012C380
		[CompilerGenerated]
		private void <GenerateOrgsForAcquisition>g__CreateNewOrgs|621_0(int numNewOrgsToCreate, ref TIFactionState.<>c__DisplayClass621_0 A_2)
		{
			Dictionary<OrgType, int> orgTypeDict = new Dictionary<OrgType, int>();
			foreach (TIOrgTemplate tiorgTemplate in from org in TemplateManager.IterateByClass<TIOrgTemplate>(true)
				where org.randomized && org.allowedOnMarket
				select org)
			{
				if (!orgTypeDict.ContainsKey(tiorgTemplate.orgType))
				{
					orgTypeDict.Add(tiorgTemplate.orgType, 1);
				}
				else
				{
					Dictionary<OrgType, int> orgTypeDict2 = orgTypeDict;
					OrgType orgType = tiorgTemplate.orgType;
					orgTypeDict2[orgType]++;
				}
			}
			Dictionary<TIOrgTemplate, float> dictionary = (from orgTemplate in TemplateManager.IterateByClass<TIOrgTemplate>(true)
				where orgTemplate.randomized && orgTemplate.allowedOnMarket && orgTemplate.CanSpawn()
				select orgTemplate).ToDictionary<TIOrgTemplate, TIOrgTemplate, float>((TIOrgTemplate orgTemplate) => orgTemplate, (TIOrgTemplate orgTemplate) => (4f - (float)orgTemplate.tier) / (float)orgTypeDict[orgTemplate.orgType]);
			for (int i = 0; i < numNewOrgsToCreate; i++)
			{
				TIOrgState tiorgState = TIFactionState.CreateNewOrg(dictionary.SelectRandomWeightedItem<KeyValuePair<TIOrgTemplate, float>>((KeyValuePair<TIOrgTemplate, float> j) => j.Value, -1f, 1E-37f).Key);
				if (tiorgState.AllowedOnFactionMarket(this))
				{
					this.AddAvailableOrg(tiorgState, true);
					foreach (TIOrgTemplate tiorgTemplate2 in dictionary.Keys.ToList<TIOrgTemplate>())
					{
						if (tiorgTemplate2.orgType == tiorgState.orgType)
						{
							Dictionary<TIOrgTemplate, float> dictionary2 = dictionary;
							TIOrgTemplate tiorgTemplate3 = tiorgTemplate2;
							dictionary2[tiorgTemplate3] /= (float)(tiorgState.tier + 1);
						}
					}
					A_2.newAvailable = true;
				}
			}
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x0012E388 File Offset: 0x0012C588
		[CompilerGenerated]
		internal static int <TraitProjectCount>g__selector|665_1(TITraitTemplate y)
		{
			return y.incomeProjects;
		}

		// Token: 0x060034BE RID: 13502 RVA: 0x0012E390 File Offset: 0x0012C590
		[CompilerGenerated]
		internal static int <OrgProjectCount>g__selector|668_1(TIOrgState y)
		{
			return y.projectCapacityGranted;
		}

		// Token: 0x060034BF RID: 13503 RVA: 0x0012E398 File Offset: 0x0012C598
		[CompilerGenerated]
		internal static int <HabProjectCount>g__selector|674_1(TIHabModuleState y)
		{
			return y.moduleTemplate.incomeProjects;
		}

		// Token: 0x060034C7 RID: 13511 RVA: 0x0012E40F File Offset: 0x0012C60F
		[CompilerGenerated]
		internal static bool <CanExplore>g__IsCacheable|895_0(TISpaceGameState x)
		{
			return x.isNaturalSpaceObjectState || x.isHabState || (x.isOrbitState && !x.ref_orbit.isAdHocOrbit);
		}

		// Token: 0x060034F0 RID: 13552 RVA: 0x0012E6F2 File Offset: 0x0012C8F2
		[CompilerGenerated]
		internal static string <GetAdvice>g__AdviceLocPath|1351_3(TIFactionState.Advice advice, string extra)
		{
			return new StringBuilder("UI.Chat.").Append(advice.ToString()).Append(extra).ToString();
		}

		// Token: 0x040022AF RID: 8879
		private const int maxResearchSetting = 3;

		// Token: 0x040022B0 RID: 8880
		public const int maxTurnedCouncilors = 2;

		// Token: 0x040022B1 RID: 8881
		public const int HQProjectSlot = 3;

		// Token: 0x040022B2 RID: 8882
		public const int orgProjectSlot = 4;

		// Token: 0x040022B3 RID: 8883
		public const int habProjectSlot = 5;

		// Token: 0x040022B4 RID: 8884
		public TIPlayerState player;

		// Token: 0x040022B5 RID: 8885
		public List<TICouncilorState> councilors;

		// Token: 0x040022B6 RID: 8886
		public List<TICouncilorState> turnedCouncilors;

		// Token: 0x040022B7 RID: 8887
		public List<TICouncilorState> knownSpies;

		// Token: 0x040022B8 RID: 8888
		public List<TIFactionState> intelSharingFactions;

		// Token: 0x040022B9 RID: 8889
		public List<TIOrgState> unassignedOrgs;

		// Token: 0x040022BA RID: 8890
		public List<TISpaceFleetState> fleets;

		// Token: 0x040022BB RID: 8891
		public List<TISectorState> habSectors;

		// Token: 0x040022BC RID: 8892
		public List<TIOrgState> availableOrgs;

		// Token: 0x040022BD RID: 8893
		public List<TIOrgState> newAvailableOrgs;

		// Token: 0x040022BE RID: 8894
		public List<TICouncilorState> availableCouncilors;

		// Token: 0x040022BF RID: 8895
		public List<TICouncilorState> newAvailableCouncilors;

		// Token: 0x040022C0 RID: 8896
		public List<TISpaceShipTemplate> shipDesigns;

		// Token: 0x040022C1 RID: 8897
		private readonly object shipDesignsLock = new object();

		// Token: 0x040022C2 RID: 8898
		[Obsolete]
		public List<TISpaceShipTemplate> shipRefitDesigns;

		// Token: 0x040022C3 RID: 8899
		public List<string> shipRefitDesignNames;

		// Token: 0x040022C4 RID: 8900
		public List<string> obsoleteShipDesigns;

		// Token: 0x040022C5 RID: 8901
		public List<TIPriorityPresetTemplate> customPresets;

		// Token: 0x040022C6 RID: 8902
		public List<TIHabTemplate> habDesigns;

		// Token: 0x040022C7 RID: 8903
		public int savedHabDesigns;

		// Token: 0x040022C8 RID: 8904
		public List<TIControlPoint> controlPoints;

		// Token: 0x040022C9 RID: 8905
		public List<TINationState> permaAbandonedNations;

		// Token: 0x040022CA RID: 8906
		public List<TIArmyState> armies;

		// Token: 0x040022CB RID: 8907
		[SerializeField]
		public Dictionary<FactionResource, float> resources;

		// Token: 0x040022CC RID: 8908
		[SerializeField]
		private Dictionary<FactionResource, float> baseIncomes_year;

		// Token: 0x040022CD RID: 8909
		[fsIgnore]
		public bool He3Access;

		// Token: 0x040022CE RID: 8910
		public List<DailyResourceTransfer> dailyResourceTransfers;

		// Token: 0x040022CF RID: 8911
		public float lastWeeksSpoils;

		// Token: 0x040022D0 RID: 8912
		public float thisWeeksCumulativeSpoils;

		// Token: 0x040022D1 RID: 8913
		public float lastMonthsSpoils;

		// Token: 0x040022D2 RID: 8914
		public float thisMonthsCumulativeSpoils;

		// Token: 0x040022D3 RID: 8915
		public float cachedSTOFighterMinimumBoost;

		// Token: 0x040022D4 RID: 8916
		[fsIgnore]
		public bool fullSpaceVisibility;

		// Token: 0x040022D7 RID: 8919
		[SerializeField]
		private Dictionary<string, ObjectiveStatus> objectiveNames;

		// Token: 0x040022D9 RID: 8921
		[SerializeField]
		private List<string> availableProjectNames;

		// Token: 0x040022DB RID: 8923
		[SerializeField]
		private List<ProjectTrigger> activeProjectTriggers;

		// Token: 0x040022DC RID: 8924
		public List<ProjectProgress> currentProjectProgress;

		// Token: 0x040022DD RID: 8925
		public int[] researchWeights;

		// Token: 0x040022E1 RID: 8929
		public Dictionary<TIFactionState.AtrocityCause, int> numAtrocitiesByCause;

		// Token: 0x040022E3 RID: 8931
		[SerializeField]
		private Dictionary<TIGameState, float> intel;

		// Token: 0x040022E4 RID: 8932
		[SerializeField]
		private Dictionary<TIGameState, float> highestIntel;

		// Token: 0x040022E5 RID: 8933
		public Dictionary<TIFactionState, int> factionFleetsEncountered;

		// Token: 0x040022E6 RID: 8934
		public Dictionary<TIFactionState, int> factionAssassinations;

		// Token: 0x040022E7 RID: 8935
		public Dictionary<TICouncilorState, int> lastRecordedLoyalty;

		// Token: 0x040022E8 RID: 8936
		public Dictionary<TICouncilorState, TIDateTime> lastTimeSecretsWereSeen;

		// Token: 0x040022E9 RID: 8937
		public List<TIFactionState> ignoreContacts;

		// Token: 0x040022EA RID: 8938
		public List<TIFactionState> ignoreInterstateDiplomacy;

		// Token: 0x040022EB RID: 8939
		public string defaultPriorityPresetTemplateName;

		// Token: 0x040022ED RID: 8941
		public TIHabState primaryHab;

		// Token: 0x040022EE RID: 8942
		public int nextRefitNumber = 1000;

		// Token: 0x040022EF RID: 8943
		public int abductions;

		// Token: 0x040022F0 RID: 8944
		public int councilorsGenerated;

		// Token: 0x040022F1 RID: 8945
		public List<SpecialRegionAdjacencies> specialRegionAdjacencies;

		// Token: 0x040022F2 RID: 8946
		public int alienInvestigations;

		// Token: 0x040022F3 RID: 8947
		public int aliensRemoved;

		// Token: 0x040022F4 RID: 8948
		public Dictionary<ArmyType, int> armiesLost;

		// Token: 0x040022F5 RID: 8949
		public AIValues aiValues;

		// Token: 0x040022F7 RID: 8951
		[SerializeField]
		private Dictionary<TIFactionState, float> factionHate;

		// Token: 0x040022F8 RID: 8952
		[SerializeField]
		private float assessedAlienHateOfMe = -1f;

		// Token: 0x040022F9 RID: 8953
		[SerializeField]
		private TIDateTime lastDateOfFixedAlienHate;

		// Token: 0x040022FA RID: 8954
		public Dictionary<TICouncilorState, float> internalCouncilorSuspicion;

		// Token: 0x040022FB RID: 8955
		public float thisTurnsReveralScore;

		// Token: 0x040022FC RID: 8956
		public bool crazyIvan;

		// Token: 0x040022FD RID: 8957
		public Dictionary<GoalType, List<TIFactionGoalState>> factionGoals;

		// Token: 0x040022FE RID: 8958
		public DesiredShipClass desiredShipClass;

		// Token: 0x040022FF RID: 8959
		public List<AITaskCategory> factionEarlyToDoList;

		// Token: 0x04002300 RID: 8960
		public List<AITaskCategory> factionLateToDoList;

		// Token: 0x04002301 RID: 8961
		public bool minorCPTrouble;

		// Token: 0x04002302 RID: 8962
		public bool majorCPTrouble;

		// Token: 0x04002303 RID: 8963
		public bool alienProxyNeedsHelp;

		// Token: 0x04002304 RID: 8964
		public float currentRiskAversion;

		// Token: 0x04002305 RID: 8965
		public Dictionary<TIGameState, TIDateTime> knownAlienSites;

		// Token: 0x04002306 RID: 8966
		public bool AIReviewProjects;

		// Token: 0x04002307 RID: 8967
		public bool knowsWinCondition;

		// Token: 0x04002308 RID: 8968
		public bool updateShipDesignsFlag;

		// Token: 0x04002309 RID: 8969
		public bool updateHabPlanningFlag;

		// Token: 0x0400230A RID: 8970
		public List<FactionResource> resourceIncomeDeficiencies;

		// Token: 0x0400230B RID: 8971
		public TIFactionState mostPowerfulHumanEnemy;

		// Token: 0x0400230C RID: 8972
		public FactionSelfAssessment selfAssessement;

		// Token: 0x0400230D RID: 8973
		public AISavingData AISavingTarget;

		// Token: 0x0400230E RID: 8974
		public TIFactionGoalState focusGoal;

		// Token: 0x0400230F RID: 8975
		public Dictionary<TIControlPoint, TIDateTime> lostControlPoints;

		// Token: 0x04002310 RID: 8976
		public List<TINationState> initialAINationGoals = new List<TINationState>();

		// Token: 0x04002311 RID: 8977
		public float highestSpaceStrengthSinceLastAlienKnockdown;

		// Token: 0x04002312 RID: 8978
		[fsIgnore]
		public bool planningMissions;

		// Token: 0x04002313 RID: 8979
		[fsIgnore]
		public bool preppingForMissions;

		// Token: 0x04002314 RID: 8980
		public List<string> hiddenProjects;

		// Token: 0x04002315 RID: 8981
		public List<string> favoredProjects;

		// Token: 0x04002316 RID: 8982
		public List<string> obsoletedShipParts;

		// Token: 0x04002317 RID: 8983
		public List<string> missedProjects;

		// Token: 0x04002318 RID: 8984
		public List<string> sabotagedProjects;

		// Token: 0x04002319 RID: 8985
		public string longtermTechTarget;

		// Token: 0x0400231A RID: 8986
		public Dictionary<TIFactionState.BoostAccountName, TIDateTime> boostAccounts = new Dictionary<TIFactionState.BoostAccountName, TIDateTime>();

		// Token: 0x0400231B RID: 8987
		public Dictionary<TIFactionState, float> perceivedEnemyFleetStrengthFactors = new Dictionary<TIFactionState, float>();

		// Token: 0x0400231C RID: 8988
		public bool showRegularNotifications;

		// Token: 0x0400231D RID: 8989
		public bool showTimerNotifications;

		// Token: 0x0400231E RID: 8990
		public bool showAlerts;

		// Token: 0x0400231F RID: 8991
		public bool showSummaryLogs;

		// Token: 0x04002320 RID: 8992
		[Obsolete]
		public bool alertSpaceTimerNotifications;

		// Token: 0x04002321 RID: 8993
		public bool checkNotificationOverrides;

		// Token: 0x04002322 RID: 8994
		public Dictionary<string, TINotificationTemplateOverride> notificationOverrides;

		// Token: 0x04002323 RID: 8995
		public bool showMonthlyIncomesInTopBarAndIntel;

		// Token: 0x04002324 RID: 8996
		public bool showObsoleteParts = true;

		// Token: 0x04002325 RID: 8997
		public int defaultFleetArrivalAlert;

		// Token: 0x04002326 RID: 8998
		public int defaultFleetArrivalAlert_Earth;

		// Token: 0x04002327 RID: 8999
		public int defaultFleetArrivalAlienModifier;

		// Token: 0x04002328 RID: 9000
		public int defaultFleetArrivalAlienModifier_Earth;

		// Token: 0x04002329 RID: 9001
		public int defaultHullAppearanceIndex;

		// Token: 0x0400232A RID: 9002
		public List<Alarm> alarms;

		// Token: 0x0400232B RID: 9003
		public MapColorationStyle mapColorationStyle;

		// Token: 0x0400232C RID: 9004
		public Dictionary<string, int> shipsBuiltInClass;

		// Token: 0x0400232D RID: 9005
		public List<float> history_CPCapOverageByDay;

		// Token: 0x0400232E RID: 9006
		public List<int> history_MCCapOverageByDay;

		// Token: 0x0400232F RID: 9007
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x04002330 RID: 9008
		public bool defeated;

		// Token: 0x04002331 RID: 9009
		private GameTimeManager gameTime;

		// Token: 0x04002332 RID: 9010
		public const float intelMarkerForProspectorEnRoute = 0.1f;

		// Token: 0x04002333 RID: 9011
		public const float intelToProspectSpaceBody = 1f;

		// Token: 0x04002334 RID: 9012
		[fsIgnore]
		private Dictionary<TIObjectiveTemplate, ObjectiveStatus> objectives;

		// Token: 0x04002335 RID: 9013
		[fsIgnore]
		public List<TIProjectTemplate> availableProjects;

		// Token: 0x04002336 RID: 9014
		[fsIgnore]
		public List<TIProjectTemplate> completedProjects;

		// Token: 0x04002337 RID: 9015
		[fsIgnore]
		public Dictionary<TITechTemplate, float> techContributionHistory;

		// Token: 0x04002338 RID: 9016
		[fsIgnore]
		public TIPriorityPresetTemplate defaultPriorityPreset;

		// Token: 0x04002339 RID: 9017
		[fsIgnore]
		public TIFactionIdeologyTemplate ideology;

		// Token: 0x0400233A RID: 9018
		[fsIgnore]
		private Sprite _factionIcon64;

		// Token: 0x0400233B RID: 9019
		[fsIgnore]
		private Sprite _factionIcon128;

		// Token: 0x0400233C RID: 9020
		[fsIgnore]
		private Sprite _factionIcon256;

		// Token: 0x0400233D RID: 9021
		[fsIgnore]
		private Sprite _factionIcon64UI;

		// Token: 0x0400233E RID: 9022
		[fsIgnore]
		private Sprite _factionIcon128UI;

		// Token: 0x0400233F RID: 9023
		[fsIgnore]
		private Sprite _factionIcon256UI;

		// Token: 0x04002340 RID: 9024
		[fsIgnore]
		private Sprite _fleetIcon;

		// Token: 0x04002341 RID: 9025
		[fsIgnore]
		private Sprite _fleetIcon1;

		// Token: 0x04002342 RID: 9026
		[fsIgnore]
		private Sprite _fleetIcon2;

		// Token: 0x04002343 RID: 9027
		[fsIgnore]
		private Sprite _fleetIcon3;

		// Token: 0x04002344 RID: 9028
		[fsIgnore]
		private Sprite _baseIcon;

		// Token: 0x04002345 RID: 9029
		[fsIgnore]
		private Sprite _stationIcon;

		// Token: 0x04002346 RID: 9030
		[fsIgnore]
		private Sprite _leaderIcon;

		// Token: 0x04002347 RID: 9031
		[fsIgnore]
		private TICouncilorAppearanceTemplate _leaderAppearance;

		// Token: 0x04002348 RID: 9032
		[fsIgnore]
		private TIVictoryTemplate _victoryTemplate;

		// Token: 0x04002349 RID: 9033
		[fsIgnore]
		public Dictionary<TISpaceFleetState, FactionGoal_Fleet> fleetGoalTracker;

		// Token: 0x0400234A RID: 9034
		[fsIgnore]
		public Dictionary<PriorityType, float> cachedPriorityBonuses;

		// Token: 0x0400234B RID: 9035
		[fsIgnore]
		public Dictionary<TIGenericTechTemplate, string> cachedTechTooltipStrings;

		// Token: 0x0400234C RID: 9036
		private Player _playerControl;

		// Token: 0x0400234D RID: 9037
		private bool isDummy;

		// Token: 0x0400234F RID: 9039
		private const int minimumAge = 18;

		// Token: 0x04002350 RID: 9040
		private const int maximumAge = 85;

		// Token: 0x04002351 RID: 9041
		private const int elder = 65;

		// Token: 0x04002352 RID: 9042
		private readonly TITraitTemplate declining = TemplateManager.Find<TITraitTemplate>("Declining", false);

		// Token: 0x04002353 RID: 9043
		public const string DailyIncomeTransactionLabel = "Daily Income";

		// Token: 0x04002354 RID: 9044
		public Dictionary<string, List<TIFactionState.Transaction>> Transactions = new Dictionary<string, List<TIFactionState.Transaction>>();

		// Token: 0x04002355 RID: 9045
		private Dictionary<FactionResource, float> annualResourceIncomes = new Dictionary<FactionResource, float>();

		// Token: 0x04002356 RID: 9046
		private readonly TIDirtyResourcesTracker dirtyResourcesTracker = new TIDirtyResourcesTracker();

		// Token: 0x04002357 RID: 9047
		[SerializeField]
		private Dictionary<FactionResource, float> cachedYearlyRevenue = new Dictionary<FactionResource, float>();

		// Token: 0x04002358 RID: 9048
		public static FactionResource[] habSupportResources = new FactionResource[]
		{
			FactionResource.Boost,
			FactionResource.Water,
			FactionResource.Volatiles,
			FactionResource.Metals,
			FactionResource.NobleMetals,
			FactionResource.Fissiles
		};

		// Token: 0x0400235A RID: 9050
		private bool missionControlUsageDataDirty = true;

		// Token: 0x0400235B RID: 9051
		private Dictionary<FactionResource, float> cachedMiningMultiplier = TIResourcesCost.basicSpaceResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => 1f);

		// Token: 0x0400235C RID: 9052
		private Dictionary<FactionResource, int> miningMultiplierCachedFrame = TIResourcesCost.basicSpaceResources.ToDictionary<FactionResource, FactionResource, int>((FactionResource x) => x, (FactionResource x) => -1);

		// Token: 0x0400235D RID: 9053
		private int cachedGenericMissionControlAvailable;

		// Token: 0x0400235E RID: 9054
		private int genericMissionControlAvailableCachedFrame = -1;

		// Token: 0x0400235F RID: 9055
		public const float ExpenditureResolution_days = 28f;

		// Token: 0x04002360 RID: 9056
		[TupleElementNames(new string[] { "Date", "Value" })]
		[SerializeField]
		private Dictionary<TIFactionState.Expenditure, Dictionary<FactionResource, ValueTuple<TIDateTime, float>>> highestRecordedExpenditurePerDay = new Dictionary<TIFactionState.Expenditure, Dictionary<FactionResource, ValueTuple<TIDateTime, float>>>();

		// Token: 0x04002361 RID: 9057
		[SerializeField]
		private Dictionary<FactionResource, float> fleetWetMassDuringHighestShipMaintainence = new Dictionary<FactionResource, float>();

		// Token: 0x04002362 RID: 9058
		[TupleElementNames(new string[] { "DV_kps", "Days" })]
		[SerializeField]
		private List<ValueTuple<float, float>> LocalTransferDVLog = new List<ValueTuple<float, float>>();

		// Token: 0x04002363 RID: 9059
		[TupleElementNames(new string[] { "DV_kps", "Days" })]
		[SerializeField]
		private List<ValueTuple<float, float>> SolarTransferDVLog = new List<ValueTuple<float, float>>();

		// Token: 0x04002364 RID: 9060
		private Dictionary<PriorityType, float> cachedAverageNationPriorityFractions = new Dictionary<PriorityType, float>();

		// Token: 0x04002365 RID: 9061
		private int averagePriorityFractionsCachedFrame = -1;

		// Token: 0x04002366 RID: 9062
		public const int startMaxCouncilSize = 4;

		// Token: 0x04002367 RID: 9063
		public const int maxMaxCouncilSize = 6;

		// Token: 0x04002368 RID: 9064
		[fsIgnore]
		private Dictionary<CouncilorAttribute, int> cachedTotalStats = new Dictionary<CouncilorAttribute, int>();

		// Token: 0x04002369 RID: 9065
		[fsIgnore]
		public static readonly FactionResource[] councilorResources = new FactionResource[]
		{
			FactionResource.Money,
			FactionResource.Boost,
			FactionResource.Research,
			FactionResource.Influence,
			FactionResource.Operations,
			FactionResource.MissionControl,
			FactionResource.Projects
		};

		// Token: 0x0400236A RID: 9066
		private Dictionary<PriorityType, float> cachedLEOHabPriorityBonuses = new Dictionary<PriorityType, float>();

		// Token: 0x0400236B RID: 9067
		private Dictionary<PriorityType, float> cachedLEOHabPriorityBonuses_IncludeNonActive = new Dictionary<PriorityType, float>();

		// Token: 0x0400236C RID: 9068
		private int LEOHabPriorityBonusesCachedFrame = -1;

		// Token: 0x0400236D RID: 9069
		private int cachedAlienDetectionBonus;

		// Token: 0x0400236E RID: 9070
		private int alienDetectionBonusCachedFrame = -1;

		// Token: 0x0400236F RID: 9071
		private int cachedHumanDetectionBonus;

		// Token: 0x04002370 RID: 9072
		private int HumanDetectionBonusCachedFrame = -1;

		// Token: 0x04002371 RID: 9073
		private float cachedArmyCombatBonus;

		// Token: 0x04002372 RID: 9074
		private int armyCombatBonusCachedFrame = -1;

		// Token: 0x04002373 RID: 9075
		private float cachedPropagandaBonus;

		// Token: 0x04002374 RID: 9076
		private int propagandaBonusCachedFrame = -1;

		// Token: 0x04002375 RID: 9077
		private int cachedTraitProjectCount;

		// Token: 0x04002376 RID: 9078
		private int traitProjectCountCachedFrame;

		// Token: 0x04002377 RID: 9079
		private int cachedOrgProjectCount;

		// Token: 0x04002378 RID: 9080
		private int orgProjectCountCachedFrame;

		// Token: 0x04002379 RID: 9081
		private int cachedHabProjectCount;

		// Token: 0x0400237A RID: 9082
		private int habProjectCountCachedFrame = -1;

		// Token: 0x0400237B RID: 9083
		public static readonly List<Context> spaceRangeContexts = new List<Context>
		{
			Context.InnerExplorationRange_AU,
			Context.OuterExplorationRange_AU,
			Context.ExploreEarthLagrangePoints,
			Context.ExploreLuna
		};

		// Token: 0x0400237C RID: 9084
		public const float TechMultiplierCapBeforeDiminishingReturns = 0.5f;

		// Token: 0x0400237D RID: 9085
		private Dictionary<TechCategory, float> cachedBaseHabsMultipliers = new Dictionary<TechCategory, float>();

		// Token: 0x0400237E RID: 9086
		private Dictionary<TechCategory, int> baseHabsMultiplierCachedFrames;

		// Token: 0x0400237F RID: 9087
		private Dictionary<TechCategory, float> cachedTraitsMultiplier = new Dictionary<TechCategory, float>();

		// Token: 0x04002380 RID: 9088
		private Dictionary<TechCategory, int> traitsMultiplierCachedFrame = Enums.TechCategories.ToDictionary<TechCategory, TechCategory, int>((TechCategory x) => x, (TechCategory x) => -1);

		// Token: 0x04002381 RID: 9089
		private Dictionary<TechCategory, float> cachedOrgsMultiplier = new Dictionary<TechCategory, float>();

		// Token: 0x04002382 RID: 9090
		private Dictionary<TechCategory, int> orgsMultiplierCachedFrame = Enums.TechCategories.ToDictionary<TechCategory, TechCategory, int>((TechCategory x) => x, (TechCategory x) => -1);

		// Token: 0x04002383 RID: 9091
		private Dictionary<TechCategory, float> cachedFleetsModifier = new Dictionary<TechCategory, float>();

		// Token: 0x04002384 RID: 9092
		private Dictionary<TechCategory, int> fleetsModifierCachedFrame = Enums.TechCategories.ToDictionary<TechCategory, TechCategory, int>((TechCategory x) => x, (TechCategory x) => -1);

		// Token: 0x04002385 RID: 9093
		[SerializeField]
		private float globalResearchPurse;

		// Token: 0x04002386 RID: 9094
		[fsIgnore]
		private List<TIProjectTemplate> cachedTriggeredProjects = new List<TIProjectTemplate>();

		// Token: 0x04002387 RID: 9095
		[fsIgnore]
		private int triggeredProjectsCachedFrame = -1;

		// Token: 0x04002388 RID: 9096
		private const string specialReinvestigateProjectName = "Project_ReviewFailedProjects";

		// Token: 0x04002389 RID: 9097
		[fsIgnore]
		protected readonly TIProjectTemplate specialReinvestigateProject = TemplateManager.Find<TIProjectTemplate>("Project_ReviewFailedProjects", false);

		// Token: 0x0400238A RID: 9098
		private TIResourcesCost cachedAverageShipBuildCost;

		// Token: 0x0400238B RID: 9099
		private TIDateTime averageShipBuildCostCachedDate;

		// Token: 0x0400238C RID: 9100
		private TIResourcesCost cachedAverageShipFuelCost;

		// Token: 0x0400238D RID: 9101
		private TIDateTime averageShipFuelCostCachedDate;

		// Token: 0x0400238E RID: 9102
		[SerializeField]
		private float desiredStaticFleetFraction;

		// Token: 0x0400238F RID: 9103
		public List<TIFactionState.CombatLog> CombatLogs = new List<TIFactionState.CombatLog>();

		// Token: 0x04002390 RID: 9104
		public const int MaximumTotalCombatLogAttackCount = 5000;

		// Token: 0x04002391 RID: 9105
		public const int MaximumAttackCountPerCombatLog = 100;

		// Token: 0x04002392 RID: 9106
		public List<TIFactionState.HabDestructionLogEntry> HabDestructionLog = new List<TIFactionState.HabDestructionLogEntry>();

		// Token: 0x04002393 RID: 9107
		public const string ShipConstructionTransactionLabel = "Ship Construction";

		// Token: 0x04002394 RID: 9108
		public TIHabModuleState lastUnaffordableShipShipyard;

		// Token: 0x04002395 RID: 9109
		public const int AIMaxTransferDurationToAssignConstructionToShipyard_days = 540;

		// Token: 0x04002396 RID: 9110
		private List<HabSchematic> habSchematics;

		// Token: 0x04002397 RID: 9111
		private const float probeOvertakeFeasibility = 0.95f;

		// Token: 0x04002398 RID: 9112
		public const float innerBaseRange_AU = 0.98f;

		// Token: 0x04002399 RID: 9113
		public const float outerBaseRange_AU = 1.02f;

		// Token: 0x0400239A RID: 9114
		private HashSet<TISpaceGameState> cachedCanExplore = new HashSet<TISpaceGameState>();

		// Token: 0x0400239B RID: 9115
		private List<TIRegionAlienEntityState> cachedKnownAlienEntities = new List<TIRegionAlienEntityState>();

		// Token: 0x0400239C RID: 9116
		private int cachedKnownAlienEntitiesFrame = -1;

		// Token: 0x0400239D RID: 9117
		[TupleElementNames(new string[] { "Nation", "Date" })]
		public List<ValueTuple<TINationState, TIDateTime>> AlienControlPointGiftHistory = new List<ValueTuple<TINationState, TIDateTime>>();

		// Token: 0x0400239E RID: 9118
		public int shipDesignCount;

		// Token: 0x0400239F RID: 9119
		[NonSerialized]
		private List<TIShipHullTemplate> cachedAllowedShipHulls;

		// Token: 0x040023A0 RID: 9120
		[NonSerialized]
		private List<TIRadiatorTemplate> cachedAllowedRadiators;

		// Token: 0x040023A1 RID: 9121
		[NonSerialized]
		private List<TIDriveTemplate> cachedAllowedDrives;

		// Token: 0x040023A2 RID: 9122
		[NonSerialized]
		private List<TIBatteryTemplate> cachedAllowedBatteries;

		// Token: 0x040023A3 RID: 9123
		[NonSerialized]
		private List<TIShipArmorTemplate> cachedAllowedArmors;

		// Token: 0x040023A4 RID: 9124
		[NonSerialized]
		private List<TIPowerPlantTemplate> cachedAllowedPowerPlants;

		// Token: 0x040023A5 RID: 9125
		[NonSerialized]
		private List<TIShipWeaponTemplate> cachedAllowedNoseWeapons;

		// Token: 0x040023A6 RID: 9126
		[NonSerialized]
		private List<TIShipWeaponTemplate> cachedAllowedHullWeapons;

		// Token: 0x040023A7 RID: 9127
		[NonSerialized]
		private List<TIHeatSinkTemplate> cachedAllowedHeatSinks;

		// Token: 0x040023A8 RID: 9128
		[NonSerialized]
		private List<TIUtilityModuleTemplate> cachedAllowedUtilityModules;

		// Token: 0x040023A9 RID: 9129
		[fsIgnore]
		protected readonly List<WeaponClass> validWeaponClassesForHumanHabs = new List<WeaponClass>
		{
			WeaponClass.NavalGun,
			WeaponClass.Laser,
			WeaponClass.Magnetic,
			WeaponClass.Plasma
		};

		// Token: 0x040023AA RID: 9130
		[fsIgnore]
		protected readonly List<WeaponClass> validWeaponClassesForAlienHabs = new List<WeaponClass>
		{
			WeaponClass.Plasma,
			WeaponClass.Laser,
			WeaponClass.Magnetic
		};

		// Token: 0x040023AB RID: 9131
		[TupleElementNames(new string[] { null, null, "Acceleration_gs", "DV_kps" })]
		[NonSerialized]
		private Dictionary<ValueTuple<ShipRole, TIShipHullTemplate>, Dictionary<TIDriveTemplate, ValueTuple<float, float>>> shipDesigner_CachedDriveStats = new Dictionary<ValueTuple<ShipRole, TIShipHullTemplate>, Dictionary<TIDriveTemplate, ValueTuple<float, float>>>();

		// Token: 0x040023AC RID: 9132
		private const string defaultHumanFighterMissile = "KraitMissilePod";

		// Token: 0x040023AD RID: 9133
		private const string defaultAlienFighterCannon = "AlienMiniLightMagCannon";

		// Token: 0x040023AE RID: 9134
		private const string defaultAlienFighterMissile = "GlitteringJewelMissilePod";

		// Token: 0x040023AF RID: 9135
		private const string defaultAlienFighterDrive = "SuperKronosLiquidRocketx1";

		// Token: 0x040023B0 RID: 9136
		public const string TradeCreditTransactionLabel = "Trade Credit";

		// Token: 0x040023B1 RID: 9137
		public const string TradeDebitTransactionLabel = "Trade Debit";

		// Token: 0x040023B2 RID: 9138
		public static readonly string dumpfile = "AIDump.txt";

		// Token: 0x040023B3 RID: 9139
		public static readonly bool AIDump = true;

		// Token: 0x040023B4 RID: 9140
		private List<FactionGoal_Fleet> cachedFleetGoals;

		// Token: 0x040023B5 RID: 9141
		private bool fleetGoalsDirty = true;

		// Token: 0x040023B6 RID: 9142
		private List<FactionGoal_Fleet> cachedUnresolvedFleetGoals;

		// Token: 0x040023B7 RID: 9143
		private bool unresolvedFleetGoalsDirty = true;

		// Token: 0x040023B8 RID: 9144
		public Dictionary<TIFactionState, List<string>> Kills = new Dictionary<TIFactionState, List<string>>();

		// Token: 0x040023B9 RID: 9145
		[fsProperty]
		private int techRaceSlot = -1;

		// Token: 0x040023BA RID: 9146
		[fsProperty]
		private TIDateTime lastTechRaceDate;

		// Token: 0x040023BD RID: 9149
		[fsIgnore]
		private Dictionary<TIFactionState, bool> cachedFactionWarStatus = new Dictionary<TIFactionState, bool>();

		// Token: 0x040023BE RID: 9150
		[fsIgnore]
		private int factionWarStatusCachedFrame = -1;

		// Token: 0x040023BF RID: 9151
		public static readonly List<TIFactionState.Advice> repeatableAdvice = new List<TIFactionState.Advice>
		{
			TIFactionState.Advice.CouncilorTargetedByEnemyMission,
			TIFactionState.Advice.CouncilorCanLevelUp,
			TIFactionState.Advice.VulnerableEnemyProject,
			TIFactionState.Advice.VulnerableEnemyOrg,
			TIFactionState.Advice.VulnerableEnemyCP
		};

		// Token: 0x040023C0 RID: 9152
		public static readonly List<string> friendlyCouncilorToCouncilorMissions = new List<string>
		{
			TIFactionState.passTechnologyMission.dataName,
			TIFactionState.contactMission.dataName,
			TIFactionState.protectMission.dataName
		};

		// Token: 0x02000D88 RID: 3464
		public enum BoostAccountName
		{
			// Token: 0x040052B3 RID: 21171
			Base,
			// Token: 0x040052B4 RID: 21172
			Station,
			// Token: 0x040052B5 RID: 21173
			Probe,
			// Token: 0x040052B6 RID: 21174
			Org
		}

		// Token: 0x02000D89 RID: 3465
		public struct Transaction
		{
			// Token: 0x040052B7 RID: 21175
			public FactionResource Resource;

			// Token: 0x040052B8 RID: 21176
			public float Amount;

			// Token: 0x040052B9 RID: 21177
			public TIDateTime Date;
		}

		// Token: 0x02000D8A RID: 3466
		public enum Expenditure
		{
			// Token: 0x040052BB RID: 21179
			ShipMaintainence,
			// Token: 0x040052BC RID: 21180
			ShipConstruction,
			// Token: 0x040052BD RID: 21181
			HabConstruction
		}

		// Token: 0x02000D8B RID: 3467
		[Serializable]
		public class CombatLog
		{
			// Token: 0x170011C6 RID: 4550
			// (get) Token: 0x060071ED RID: 29165 RVA: 0x003115B0 File Offset: 0x0030F7B0
			// (set) Token: 0x060071EE RID: 29166 RVA: 0x003115B8 File Offset: 0x0030F7B8
			public TIFactionState Winner
			{
				get
				{
					return this.winner;
				}
				set
				{
					this.winner = value;
					this.WasSurprising = this.IsSurprising();
				}
			}

			// Token: 0x170011C7 RID: 4551
			// (get) Token: 0x060071EF RID: 29167 RVA: 0x003115CD File Offset: 0x0030F7CD
			public bool HabPresent
			{
				get
				{
					return this.HabFaction != null;
				}
			}

			// Token: 0x170011C8 RID: 4552
			// (get) Token: 0x060071F0 RID: 29168 RVA: 0x003115DB File Offset: 0x0030F7DB
			public bool FleetVsFleet
			{
				get
				{
					return this.Ships.Keys.Any<TIFactionState>((TIFactionState x) => !this.Ships.Keys.First<TIFactionState>().permanentAlly(x));
				}
			}

			// Token: 0x170011C9 RID: 4553
			// (get) Token: 0x060071F1 RID: 29169 RVA: 0x003115F9 File Offset: 0x0030F7F9
			public bool AliensPresent
			{
				get
				{
					return this.Ships.Keys.Any<TIFactionState>((TIFactionState x) => x.IsAlienFaction);
				}
			}

			// Token: 0x060071F2 RID: 29170 RVA: 0x0031162C File Offset: 0x0030F82C
			public CombatLog(IEnumerable<TISpaceShipState> ships, TIHabState hab = null)
			{
				this.Ships = (from x in ships
					group x by x.faction).ToDictionary<IGrouping<TIFactionState, TISpaceShipState>, TIFactionState, List<ValueTuple<string, string>>>((IGrouping<TIFactionState, TISpaceShipState> x) => x.Key, (IGrouping<TIFactionState, TISpaceShipState> x) => x.Select<TISpaceShipState, ValueTuple<string, string>>((TISpaceShipState y) => new ValueTuple<string, string>(y.templateName, y.hull.dataName)).ToList<ValueTuple<string, string>>());
				this.HabFaction = ((hab != null) ? hab.faction : null);
				this.Attacks = new List<TIFactionState.CombatLog.Attack>();
			}

			// Token: 0x060071F3 RID: 29171 RVA: 0x003116CA File Offset: 0x0030F8CA
			public void AddAttack(TIFactionState.CombatLog.Attack attack)
			{
				this.Attacks.Add(attack);
			}

			// Token: 0x060071F4 RID: 29172 RVA: 0x003116D8 File Offset: 0x0030F8D8
			public void SetAttacks(IEnumerable<TIFactionState.CombatLog.Attack> attacks)
			{
				this.Attacks = attacks.ToList<TIFactionState.CombatLog.Attack>();
			}

			// Token: 0x060071F5 RID: 29173 RVA: 0x003116E8 File Offset: 0x0030F8E8
			public TIFactionState.CombatLog.SurpriseType IsSurprising()
			{
				if (this.Winner == null)
				{
					return TIFactionState.CombatLog.SurpriseType.Indeterminate;
				}
				if (this.HabPresent || !this.FleetVsFleet || this.Ships.Keys.Count != 2)
				{
					return TIFactionState.CombatLog.SurpriseType.Indeterminate;
				}
				Dictionary<TIFactionState, List<TISpaceShipTemplate>> dictionary = this.Ships.ToDictionary<KeyValuePair<TIFactionState, List<ValueTuple<string, string>>>, TIFactionState, List<TISpaceShipTemplate>>(([TupleElementNames(new string[] { "TemplateName", "HullName" })] KeyValuePair<TIFactionState, List<ValueTuple<string, string>>> x) => x.Key, ([TupleElementNames(new string[] { "TemplateName", "HullName" })] KeyValuePair<TIFactionState, List<ValueTuple<string, string>>> x) => x.Value.Select<ValueTuple<string, string>, TISpaceShipTemplate>(([TupleElementNames(new string[] { "TemplateName", "HullName" })] ValueTuple<string, string> x) => TemplateManager.Find<TISpaceShipTemplate>(x.Item1, false)).ToList<TISpaceShipTemplate>());
				if (dictionary.SelectMany<KeyValuePair<TIFactionState, List<TISpaceShipTemplate>>, TISpaceShipTemplate>((KeyValuePair<TIFactionState, List<TISpaceShipTemplate>> x) => x.Value).Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x == null))
				{
					return TIFactionState.CombatLog.SurpriseType.Indeterminate;
				}
				if (dictionary.ToDictionary<KeyValuePair<TIFactionState, List<TISpaceShipTemplate>>, TIFactionState, float>((KeyValuePair<TIFactionState, List<TISpaceShipTemplate>> x) => x.Key, (KeyValuePair<TIFactionState, List<TISpaceShipTemplate>> x) => x.Value.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false))).MaxBy<KeyValuePair<TIFactionState, float>, float>((KeyValuePair<TIFactionState, float> x) => x.Value).Key != this.Winner)
				{
					return TIFactionState.CombatLog.SurpriseType.Yes;
				}
				return TIFactionState.CombatLog.SurpriseType.No;
			}

			// Token: 0x040052BE RID: 21182
			public TIDateTime Date;

			// Token: 0x040052BF RID: 21183
			[TupleElementNames(new string[] { "TemplateName", "HullName" })]
			public Dictionary<TIFactionState, List<ValueTuple<string, string>>> Ships;

			// Token: 0x040052C0 RID: 21184
			public TIFactionState HabFaction;

			// Token: 0x040052C1 RID: 21185
			public List<TIFactionState.CombatLog.Attack> Attacks;

			// Token: 0x040052C2 RID: 21186
			[SerializeField]
			private TIFactionState winner;

			// Token: 0x040052C3 RID: 21187
			public TIFactionState.CombatLog.SurpriseType WasSurprising;

			// Token: 0x020013E8 RID: 5096
			[Serializable]
			public struct Attack
			{
				// Token: 0x04007322 RID: 29474
				public string WeaponDataName;

				// Token: 0x04007323 RID: 29475
				public float Range_km;

				// Token: 0x04007324 RID: 29476
				public ArmorFacing ArmorFacing;

				// Token: 0x04007325 RID: 29477
				public float Angle;

				// Token: 0x04007326 RID: 29478
				public float TargetingBonus;
			}

			// Token: 0x020013E9 RID: 5097
			public enum SurpriseType
			{
				// Token: 0x04007328 RID: 29480
				Indeterminate,
				// Token: 0x04007329 RID: 29481
				No,
				// Token: 0x0400732A RID: 29482
				Yes
			}
		}

		// Token: 0x02000D8C RID: 3468
		public struct HabDestructionLogEntry
		{
			// Token: 0x170011CA RID: 4554
			// (get) Token: 0x060071F7 RID: 29175 RVA: 0x0031185C File Offset: 0x0030FA5C
			public bool IsStation
			{
				get
				{
					return this.HabType == HabType.Station;
				}
			}

			// Token: 0x170011CB RID: 4555
			// (get) Token: 0x060071F8 RID: 29176 RVA: 0x00311867 File Offset: 0x0030FA67
			public bool IsBase
			{
				get
				{
					return this.HabType == HabType.Base;
				}
			}

			// Token: 0x040052C4 RID: 21188
			public HabType HabType;

			// Token: 0x040052C5 RID: 21189
			public TISpaceBodyState SpaceBody;

			// Token: 0x040052C6 RID: 21190
			public TIDateTime Date;

			// Token: 0x040052C7 RID: 21191
			public TIFactionState Destroyer;
		}

		// Token: 0x02000D8D RID: 3469
		public enum ShipyardAISearchResult
		{
			// Token: 0x040052C9 RID: 21193
			Success,
			// Token: 0x040052CA RID: 21194
			Failure_Generic,
			// Token: 0x040052CB RID: 21195
			Failure_CantAfford,
			// Token: 0x040052CC RID: 21196
			Failure_TransferTooLong
		}

		// Token: 0x02000D8E RID: 3470
		public enum ShipDesignerOutcome
		{
			// Token: 0x040052CE RID: 21198
			Success,
			// Token: 0x040052CF RID: 21199
			NoAvailableHulls,
			// Token: 0x040052D0 RID: 21200
			NoHullsForRole,
			// Token: 0x040052D1 RID: 21201
			NoDrives,
			// Token: 0x040052D2 RID: 21202
			NoPowerPlants,
			// Token: 0x040052D3 RID: 21203
			NoWeapons,
			// Token: 0x040052D4 RID: 21204
			ForcedHullNotAvailable,
			// Token: 0x040052D5 RID: 21205
			NoCandidateDesigns,
			// Token: 0x040052D6 RID: 21206
			MinimumPropulsionRequirementsNotMet,
			// Token: 0x040052D7 RID: 21207
			NoScoredDesigns,
			// Token: 0x040052D8 RID: 21208
			AntimatterRequired,
			// Token: 0x040052D9 RID: 21209
			ExoticsRequired,
			// Token: 0x040052DA RID: 21210
			DesignNotAllowedForRole,
			// Token: 0x040052DB RID: 21211
			AITooManyPasses_InsufficientAcceleration,
			// Token: 0x040052DC RID: 21212
			AITooManyPasses_InsufficientDeltaV,
			// Token: 0x040052DD RID: 21213
			AITooManyPasses_Generic
		}

		// Token: 0x02000D8F RID: 3471
		public enum AtrocityCause
		{
			// Token: 0x040052DF RID: 21215
			SpaceBombardHumanNationRegions,
			// Token: 0x040052E0 RID: 21216
			ArmyRazeHumanNationRegions,
			// Token: 0x040052E1 RID: 21217
			MassCasualtiesfromRegionDamage,
			// Token: 0x040052E2 RID: 21218
			NuclearTesting,
			// Token: 0x040052E3 RID: 21219
			IncreaseUnrestCritFailure,
			// Token: 0x040052E4 RID: 21220
			AssassinatedBeloved,
			// Token: 0x040052E5 RID: 21221
			DestroyedCivilianModules,
			// Token: 0x040052E6 RID: 21222
			LostCivilianModules,
			// Token: 0x040052E7 RID: 21223
			EventEffect
		}

		// Token: 0x02000D90 RID: 3472
		public enum GoalFilter
		{
			// Token: 0x040052E9 RID: 21225
			none,
			// Token: 0x040052EA RID: 21226
			InProgressOnly,
			// Token: 0x040052EB RID: 21227
			NotInProgressOnly
		}

		// Token: 0x02000D91 RID: 3473
		public enum Advice
		{
			// Token: 0x040052ED RID: 21229
			CouncilorTargetedByEnemyMission,
			// Token: 0x040052EE RID: 21230
			CouncilorCanLevelUp,
			// Token: 0x040052EF RID: 21231
			FactionTargetedByEnemyMission,
			// Token: 0x040052F0 RID: 21232
			FactionNationWithHighestUnrest,
			// Token: 0x040052F1 RID: 21233
			FactionNationLargeSubmitPOIncrease,
			// Token: 0x040052F2 RID: 21234
			FactionNationLargePOLoss,
			// Token: 0x040052F3 RID: 21235
			FactionNationUndefendedCPs,
			// Token: 0x040052F4 RID: 21236
			FactionNationBadXenoforming,
			// Token: 0x040052F5 RID: 21237
			FactionNationLowCohesion,
			// Token: 0x040052F6 RID: 21238
			FactionNationsCanFederate,
			// Token: 0x040052F7 RID: 21239
			FactionNationsCanUnify,
			// Token: 0x040052F8 RID: 21240
			FactionNationHighCoupChance,
			// Token: 0x040052F9 RID: 21241
			FactionNeededMiningSiteAvailable,
			// Token: 0x040052FA RID: 21242
			AccessibleMostPopularNationWithNeutralCPs,
			// Token: 0x040052FB RID: 21243
			AccessibleControlPointWithHighestBoost,
			// Token: 0x040052FC RID: 21244
			FactionStockpileSufficentForLEOStation,
			// Token: 0x040052FD RID: 21245
			FactionStockpileSufficentForMoonOutpost,
			// Token: 0x040052FE RID: 21246
			FactionStockpileSufficentForMarsOutpost,
			// Token: 0x040052FF RID: 21247
			FactionMCUsedInvitesAlienAttack,
			// Token: 0x04005300 RID: 21248
			AffordableT3Org,
			// Token: 0x04005301 RID: 21249
			AffordableProjectOrgToUnlockSlot,
			// Token: 0x04005302 RID: 21250
			AffordableProbesToRegionWithNoProbes,
			// Token: 0x04005303 RID: 21251
			AvailableAntiEnthrallProject,
			// Token: 0x04005304 RID: 21252
			AlienVisible,
			// Token: 0x04005305 RID: 21253
			SurveillanceFleetAtEarth,
			// Token: 0x04005306 RID: 21254
			SurveillanceHabAtEarth,
			// Token: 0x04005307 RID: 21255
			VulnerableEnemyProject,
			// Token: 0x04005308 RID: 21256
			VulnerableEnemyOrg,
			// Token: 0x04005309 RID: 21257
			VulnerableEnemyCP,
			// Token: 0x0400530A RID: 21258
			FactionBehindOnHumanShipTech,
			// Token: 0x0400530B RID: 21259
			FactionBehindOnHumanFleetSizes,
			// Token: 0x0400530C RID: 21260
			FactionBehindOnMineProduction,
			// Token: 0x0400530D RID: 21261
			FactionBehindOnTechCategoryBonus,
			// Token: 0x0400530E RID: 21262
			FactionBehindOnSettlingRegion,
			// Token: 0x0400530F RID: 21263
			FactionCPCapOverMax,
			// Token: 0x04005310 RID: 21264
			FactionMCCapOverMax,
			// Token: 0x04005311 RID: 21265
			FactionMissingCriticalMission,
			// Token: 0x04005312 RID: 21266
			FactionUsingBoostToSupportHabs
		}

		// Token: 0x02000D92 RID: 3474
		public struct AdviceData
		{
			// Token: 0x060071F9 RID: 29177 RVA: 0x00311872 File Offset: 0x0030FA72
			public AdviceData(TIFactionState.Advice adviceType, string adviceText, float priority, TIGameState target)
			{
				this.adviceType = adviceType;
				this.adviceText = adviceText;
				this.priority = priority;
				this.target = target;
			}

			// Token: 0x04005313 RID: 21267
			public TIFactionState.Advice adviceType;

			// Token: 0x04005314 RID: 21268
			public string adviceText;

			// Token: 0x04005315 RID: 21269
			public float priority;

			// Token: 0x04005316 RID: 21270
			public TIGameState target;
		}
	}
}
