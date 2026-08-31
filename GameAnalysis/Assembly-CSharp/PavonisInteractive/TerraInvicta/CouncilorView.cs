using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200071E RID: 1822
	public struct CouncilorView
	{
		// Token: 0x06002BF3 RID: 11251 RVA: 0x000F091E File Offset: 0x000EEB1E
		public CouncilorView(TICouncilorState councilor, TIFactionState playerCouncil)
		{
			this.councilor = councilor;
			this.playerCouncil = playerCouncil;
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06002BF4 RID: 11252 RVA: 0x000F092E File Offset: 0x000EEB2E
		public bool isEnemy
		{
			get
			{
				return this.councilor.faction != this.playerCouncil;
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06002BF5 RID: 11253 RVA: 0x000F0946 File Offset: 0x000EEB46
		public bool playerCouncilAgent
		{
			get
			{
				return this.councilor.turned && this.councilor.agentForFaction == this.playerCouncil;
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06002BF6 RID: 11254 RVA: 0x000F096D File Offset: 0x000EEB6D
		public TIFactionState agentForFaction
		{
			get
			{
				if (!this.councilor.turned || !this.playerCouncil.HasIntelOnCouncilorSecrets(this.councilor))
				{
					return null;
				}
				return this.councilor.agentForFaction;
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06002BF7 RID: 11255 RVA: 0x000F099C File Offset: 0x000EEB9C
		public bool turned
		{
			get
			{
				return this.playerCouncilAgent || (this.councilor.turned && this.playerCouncil.HasIntelOnCouncilorSecrets(this.councilor));
			}
		}

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06002BF8 RID: 11256 RVA: 0x000F09C8 File Offset: 0x000EEBC8
		public bool detained
		{
			get
			{
				return this.councilor.detainingFaction == this.playerCouncil || (this.councilor.detained && this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor));
			}
		}

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06002BF9 RID: 11257 RVA: 0x000F0A04 File Offset: 0x000EEC04
		public string displayNameMemory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.councilor.displayName;
			}
		}

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x06002BFA RID: 11258 RVA: 0x000F0A2F File Offset: 0x000EEC2F
		public string displayNameMemorySentence
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return Loc.T("UI.CouncilorView.UnknownSentence");
				}
				return this.councilor.displayName;
			}
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06002BFB RID: 11259 RVA: 0x000F0A5A File Offset: 0x000EEC5A
		public string displayNameCurrent
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.councilor.displayName;
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x06002BFC RID: 11260 RVA: 0x000F0A85 File Offset: 0x000EEC85
		public string displayNameCurrentSentence
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return Loc.T("UI.CouncilorView.UnknownSentence");
				}
				return this.councilor.displayName;
			}
		}

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06002BFD RID: 11261 RVA: 0x000F0AB0 File Offset: 0x000EECB0
		public TIFactionState factionMemory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return null;
				}
				return this.councilor.faction;
			}
		}

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06002BFE RID: 11262 RVA: 0x000F0AD2 File Offset: 0x000EECD2
		public TIFactionState factionCurrent
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return null;
				}
				return this.councilor.faction;
			}
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06002BFF RID: 11263 RVA: 0x000F0AF4 File Offset: 0x000EECF4
		public string factionIcon64Memory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return string.Empty;
				}
				return this.councilor.faction.factionIcon64path;
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06002C00 RID: 11264 RVA: 0x000F0B1F File Offset: 0x000EED1F
		public string factionIcon128Memory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return string.Empty;
				}
				return this.councilor.faction.factionIcon128path;
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06002C01 RID: 11265 RVA: 0x000F0B4A File Offset: 0x000EED4A
		public string factionIcon256Memory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return string.Empty;
				}
				return this.councilor.faction.factionIcon256path;
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06002C02 RID: 11266 RVA: 0x000F0B75 File Offset: 0x000EED75
		public string factionIcon64Current
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return TemplateManager.global.pathGeoscapeUnidentifiedCouncilor;
				}
				return this.councilor.faction.factionIcon64path;
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06002C03 RID: 11267 RVA: 0x000F0BA5 File Offset: 0x000EEDA5
		public Sprite councilorActionIcon64CurrentSprite
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return AssetCacheManager.unidentifiedCouncilor;
				}
				return this.councilor.faction.factionIcon64;
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06002C04 RID: 11268 RVA: 0x000F0BD0 File Offset: 0x000EEDD0
		public TICouncilorTypeTemplate councilorJobMemory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return null;
				}
				return this.councilor.typeTemplate;
			}
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06002C05 RID: 11269 RVA: 0x000F0BF2 File Offset: 0x000EEDF2
		public string councilorJobStringMemory
		{
			get
			{
				if (this.councilorJobMemory == null)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.councilorJobMemory.displayName;
			}
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x06002C06 RID: 11270 RVA: 0x000F0C12 File Offset: 0x000EEE12
		public TICouncilorTypeTemplate councilorJobCurrent
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return null;
				}
				return this.councilor.typeTemplate;
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06002C07 RID: 11271 RVA: 0x000F0C34 File Offset: 0x000EEE34
		public string councilorJobStringCurrent
		{
			get
			{
				if (this.councilorJobCurrent == null)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.councilorJobCurrent.displayName;
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06002C08 RID: 11272 RVA: 0x000F0C54 File Offset: 0x000EEE54
		public string mapIconResourcePathMemory
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return TemplateManager.global.pathGeoscapeUnidentifiedCouncilor;
				}
				return this.councilor.iconResource;
			}
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06002C09 RID: 11273 RVA: 0x000F0C7F File Offset: 0x000EEE7F
		public string mapIconResourcePathCurrent
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return TemplateManager.global.pathGeoscapeUnidentifiedCouncilor;
				}
				return this.councilor.iconResource;
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06002C0A RID: 11274 RVA: 0x000F0CAA File Offset: 0x000EEEAA
		public string genericIconResourcePath
		{
			get
			{
				if (this.councilor.isAlien || !this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return TemplateManager.global.pathGeoscapeUnidentifiedCouncilor;
				}
				return this.councilor.genericIconPath;
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x06002C0B RID: 11275 RVA: 0x000F0CE2 File Offset: 0x000EEEE2
		public string portraitPath
		{
			get
			{
				if (!this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return string.Empty;
				}
				return this.councilor.portraitResource;
			}
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06002C0C RID: 11276 RVA: 0x000F0D08 File Offset: 0x000EEF08
		public CouncilorIllustrationData missionPhaseIllustrationData
		{
			get
			{
				if (!TIMissionPhaseState.InMissionPhase() || !(this.councilor.faction != this.playerCouncil))
				{
					return this.councilor.GetIllustrationData();
				}
				return this.councilor.SetIllustrationData(this.councilor.preMissionPhaseLocation, false, false);
			}
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06002C0D RID: 11277 RVA: 0x000F0D58 File Offset: 0x000EEF58
		public string councilorAge
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.councilor.age.ToString("N0");
			}
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06002C0E RID: 11278 RVA: 0x000F0D9B File Offset: 0x000EEF9B
		public string councilorHomeTown
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return TIUtilities.GetLocationString(this.councilor.homeRegion, true, false);
			}
		}

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06002C0F RID: 11279 RVA: 0x000F0DCD File Offset: 0x000EEFCD
		public bool isKnownAlien
		{
			get
			{
				TIFactionState factionCurrent = this.factionCurrent;
				return factionCurrent != null && factionCurrent.IsAlienFaction;
			}
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x000F0DE0 File Offset: 0x000EEFE0
		public string factionStringMemory(bool capitalize)
		{
			if (this.factionMemory == null)
			{
				if (!capitalize)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return Utilities.Capitalize(Loc.T("UI.CouncilorView.Unknown"));
			}
			else
			{
				if (!capitalize)
				{
					return this.factionMemory.displayName;
				}
				return this.factionMemory.displayNameCapitalized;
			}
		}

		// Token: 0x06002C11 RID: 11281 RVA: 0x000F0E34 File Offset: 0x000EF034
		public string factionStringCurrentKnowledge(bool capitalize, bool color = false)
		{
			if (!(this.factionCurrent == null))
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (color)
				{
					stringBuilder.Append(this.factionCurrent.template.inlineColorString);
				}
				if (!capitalize)
				{
					stringBuilder.Append(this.factionCurrent.displayName);
				}
				else
				{
					stringBuilder.Append(this.factionCurrent.displayNameCapitalized);
				}
				if (color)
				{
					stringBuilder.Append("</color>");
				}
				return stringBuilder.ToString();
			}
			if (!capitalize)
			{
				return Loc.T("UI.CouncilorView.Unknown");
			}
			return Utilities.Capitalize(Loc.T("UI.CouncilorView.Unknown"));
		}

		// Token: 0x06002C12 RID: 11282 RVA: 0x000F0ECC File Offset: 0x000EF0CC
		public float EstimateAttributeFromJob(TICouncilorState councilor, CouncilorAttribute attribute)
		{
			switch (attribute)
			{
			case CouncilorAttribute.Persuasion:
				return (float)(councilor.typeTemplate.basePersuasion + councilor.typeTemplate.randPersuasion / 2);
			case CouncilorAttribute.Investigation:
				return (float)(councilor.typeTemplate.baseInvestigation + councilor.typeTemplate.randInvestigation / 2);
			case CouncilorAttribute.Espionage:
				return (float)(councilor.typeTemplate.baseEspionage + councilor.typeTemplate.randEspionage / 2);
			case CouncilorAttribute.Command:
				return (float)(councilor.typeTemplate.baseCommand + councilor.typeTemplate.randCommand / 2);
			case CouncilorAttribute.Administration:
				return (float)(councilor.typeTemplate.baseAdministration + councilor.typeTemplate.randAdministration / 2);
			case CouncilorAttribute.Science:
				return (float)(councilor.typeTemplate.baseScience + councilor.typeTemplate.randScience / 2);
			case CouncilorAttribute.Security:
				return (float)(councilor.typeTemplate.baseSecurity + councilor.typeTemplate.randSecurity / 2);
			case CouncilorAttribute.Loyalty:
				return (float)(councilor.typeTemplate.baseLoyalty + councilor.typeTemplate.randLoyalty / 2);
			case CouncilorAttribute.ApparentLoyalty:
				return (float)(councilor.typeTemplate.baseLoyalty + councilor.typeTemplate.randLoyalty / 2);
			default:
				return 0f;
			}
		}

		// Token: 0x06002C13 RID: 11283 RVA: 0x000F1004 File Offset: 0x000EF204
		public float GetAttribute(CouncilorAttribute attribute)
		{
			if (this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor))
			{
				if (attribute != CouncilorAttribute.Loyalty)
				{
					return (float)this.councilor.GetAttribute(attribute, true, true, true, false, false, false);
				}
				if (this.playerCouncil.HasIntelOnCouncilorSecrets(this.councilor) || (this.councilor.transparentLoyalty && this.councilor.faction == this.playerCouncil))
				{
					return (float)this.councilor.GetAttribute(attribute, true, true, true, false, false, false);
				}
				return (float)this.councilor.GetAttribute(CouncilorAttribute.ApparentLoyalty, true, true, true, false, false, false);
			}
			else
			{
				if (this.playerCouncil.HasMemoryOnCouncilorBasicData(this.councilor))
				{
					return this.EstimateAttributeFromJob(this.councilor, attribute);
				}
				return -1f;
			}
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x000F10C4 File Offset: 0x000EF2C4
		public string GetAttributeString(CouncilorAttribute attribute)
		{
			float attribute2 = this.GetAttribute(attribute);
			if (attribute2 == -1f)
			{
				return Loc.T("UI.CouncilorView.UnknownSymbol");
			}
			if (!this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor))
			{
				return TIUtilities.RedLine(new StringBuilder(attribute2.ToString()).Append(Loc.T("UI.CouncilorView.UnknownSymbol")).ToString());
			}
			if (attribute != CouncilorAttribute.Loyalty && attribute != CouncilorAttribute.ApparentLoyalty)
			{
				return attribute2.ToString();
			}
			if (this.playerCouncil.HasIntelOnCouncilorSecrets(this.councilor) || (this.councilor.transparentLoyalty && this.councilor.faction == this.playerCouncil))
			{
				return attribute2.ToString();
			}
			return TIUtilities.RedLine(new StringBuilder(attribute2.ToString()).Append(Loc.T("UI.CouncilorView.UnknownSymbol")).ToString());
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x000F11A0 File Offset: 0x000EF3A0
		public List<TIMissionTemplate> GetMissionsList(TICouncilorState councilor)
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>();
			if (this.playerCouncil.HasIntelOnCouncilorDetails(councilor))
			{
				list = councilor.GetPossibleMissionList(false, true, false, null, false);
			}
			else if (this.playerCouncil.HasMemoryOnCouncilorBasicData(councilor))
			{
				if (councilor.isHuman)
				{
					foreach (TIMissionTemplate timissionTemplate in TemplateManager.IterateByClass<TIMissionTemplate>(true))
					{
						if (timissionTemplate.baseMission && !timissionTemplate.disable)
						{
							list.Add(timissionTemplate);
						}
					}
				}
				foreach (TIMissionTemplate timissionTemplate2 in councilor.typeTemplate.missions)
				{
					if (!list.Contains(timissionTemplate2))
					{
						list.Add(timissionTemplate2);
					}
				}
				list = list.OrderBy<TIMissionTemplate, int>((TIMissionTemplate x) => x.sortOrder).ToList<TIMissionTemplate>();
			}
			if (councilor.faction != this.playerCouncil)
			{
				list = list.Where<TIMissionTemplate>((TIMissionTemplate x) => string.IsNullOrEmpty(x.knowledgeProject) || GameControl.control.activePlayer.finishedProjectNames.Contains(x.knowledgeProject)).ToList<TIMissionTemplate>();
			}
			return list;
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x000F12F8 File Offset: 0x000EF4F8
		public string locationString(bool longform)
		{
			if (!TIGameState.Valid(this.location))
			{
				return string.Empty;
			}
			return TIUtilities.GetLocationString(this.location, longform, false);
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06002C17 RID: 11287 RVA: 0x000F131A File Offset: 0x000EF51A
		public string associatedLocationString
		{
			get
			{
				if (!TIGameState.Valid(this.associatedLocation))
				{
					return string.Empty;
				}
				return TIUtilities.GetLocationString(this.associatedLocation, true, false);
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x06002C18 RID: 11288 RVA: 0x000F133C File Offset: 0x000EF53C
		public TIGameState location
		{
			get
			{
				if (this.playerCouncil == this.councilor.faction)
				{
					return this.councilor.location;
				}
				if (!this.playerCouncil.HasIntelOnCouncilorLocation(this.councilor))
				{
					return null;
				}
				if (TIMissionPhaseState.InMissionPhase())
				{
					return this.councilor.preMissionPhaseLocation;
				}
				return this.councilor.location;
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06002C19 RID: 11289 RVA: 0x000F13A0 File Offset: 0x000EF5A0
		public TIGameState associatedLocation
		{
			get
			{
				if (this.playerCouncil == this.councilor.faction)
				{
					return this.councilor.location;
				}
				if (!this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return null;
				}
				if (TIMissionPhaseState.InMissionPhase())
				{
					return this.councilor.preMissionPhaseLocation;
				}
				return this.councilor.location;
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06002C1A RID: 11290 RVA: 0x000F1404 File Offset: 0x000EF604
		public bool HasMission
		{
			get
			{
				return this.GetActiveMission != null;
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06002C1B RID: 11291 RVA: 0x000F1414 File Offset: 0x000EF614
		public TIMissionState GetActiveMission
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorMission(this.councilor))
				{
					return null;
				}
				if (this.playerCouncil == this.councilor.faction)
				{
					return this.councilor.activeMission;
				}
				if (TIMissionPhaseState.InMissionPhase())
				{
					return null;
				}
				return this.councilor.activeMission;
			}
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06002C1C RID: 11292 RVA: 0x000F1470 File Offset: 0x000EF670
		public string currentMissionResolveTime
		{
			get
			{
				if (this.playerCouncil.HasIntelOnCouncilorMission(this.councilor))
				{
					TIMissionState activeMission = this.councilor.activeMission;
					string text;
					if (activeMission == null)
					{
						text = null;
					}
					else
					{
						TIDateTime resolveTime = activeMission.resolveTime;
						text = ((resolveTime != null) ? resolveTime.ToCustomDateString() : null);
					}
					return text ?? string.Empty;
				}
				return string.Empty;
			}
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06002C1D RID: 11293 RVA: 0x000F14C4 File Offset: 0x000EF6C4
		public TIMissionState GetCompletedMission
		{
			get
			{
				if (!this.playerCouncil.HasIntelOnCouncilorMission(this.councilor))
				{
					return null;
				}
				if (this.playerCouncil == this.councilor.faction)
				{
					return this.councilor.completedMission;
				}
				if (TIMissionPhaseState.InMissionPhase())
				{
					return null;
				}
				return this.councilor.completedMission;
			}
		}

		// Token: 0x06002C1E RID: 11294 RVA: 0x000F151E File Offset: 0x000EF71E
		public string GetActiveMissionIcon()
		{
			TIMissionState getActiveMission = this.GetActiveMission;
			return ((getActiveMission != null) ? getActiveMission.missionTemplate.missionIconImagePath_Off : null) ?? string.Empty;
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06002C1F RID: 11295 RVA: 0x000F1540 File Offset: 0x000EF740
		public TIMissionTemplate currentMissionTemplate
		{
			get
			{
				TIMissionState getActiveMission = this.GetActiveMission;
				if (getActiveMission == null)
				{
					return null;
				}
				return getActiveMission.missionTemplate;
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06002C20 RID: 11296 RVA: 0x000F1553 File Offset: 0x000EF753
		public TIGameState currentMissionTarget
		{
			get
			{
				TIMissionState getActiveMission = this.GetActiveMission;
				if (getActiveMission == null)
				{
					return null;
				}
				return getActiveMission.target;
			}
		}

		// Token: 0x06002C21 RID: 11297 RVA: 0x000F1568 File Offset: 0x000EF768
		public string GetCurrentMissionString(bool includeTarget = true, bool includeResolveTime = false, bool twoLineTarget = false)
		{
			if (this.GetActiveMission != null)
			{
				if (this.playerCouncil == this.councilor.faction)
				{
					return this.councilor.GetCurrentMissionString(includeTarget, includeResolveTime, twoLineTarget);
				}
				if (this.playerCouncil.HasIntelOnCouncilorMission(this.councilor) && !TIMissionPhaseState.InMissionPhase())
				{
					if (twoLineTarget)
					{
						return Loc.T("UI.CouncilorView.MissionStringWithTarget_TwoLine", new object[] { this.currentMissionDisplayName, this.currentMissionTargetDisplayName });
					}
					return Loc.T("UI.CouncilorView.MissionStringWithTarget", new object[] { this.currentMissionDisplayName, this.currentMissionTargetDisplayName });
				}
			}
			if (this.detained)
			{
				return Loc.T("UI.Councilor.Detained");
			}
			if (!this.playerCouncil.HasIntelOnCouncilorMission(this.councilor))
			{
				return Loc.T("UI.CouncilorView.UnknownMission");
			}
			if (TIMissionPhaseState.InMissionPhase())
			{
				return Loc.T("UI.CouncilorView.NoMission");
			}
			return Loc.T("UI.CouncilorView.NoMissionPhase");
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06002C22 RID: 11298 RVA: 0x000F165D File Offset: 0x000EF85D
		public string currentMissionDisplayName
		{
			get
			{
				if (this.currentMissionTemplate == null)
				{
					return Loc.T("UI.CouncilorView.UnknownMission");
				}
				return this.currentMissionTemplate.displayName;
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06002C23 RID: 11299 RVA: 0x000F167D File Offset: 0x000EF87D
		public string currentMissionTargetDisplayName
		{
			get
			{
				if (!(this.currentMissionTarget != null))
				{
					return string.Empty;
				}
				return TIUtilities.GetStateDisplayName(this.currentMissionTarget, this.playerCouncil, false, true, false, false, true);
			}
		}

		// Token: 0x06002C24 RID: 11300 RVA: 0x000F16A9 File Offset: 0x000EF8A9
		public bool GrantsMarkedToAssassin()
		{
			return this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor) && this.councilor.GrantsMarkedToAssassin();
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x000F16CC File Offset: 0x000EF8CC
		public float EvaluateCouncilor()
		{
			float num = 0f;
			List<TIMissionTemplate> missionsList = this.GetMissionsList(this.councilor);
			if (this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
			{
				foreach (TIMissionTemplate timissionTemplate in missionsList)
				{
					CouncilorAttribute primaryAttackerStat = timissionTemplate.primaryAttackerStat;
					num += timissionTemplate.utilityScore * ((primaryAttackerStat == CouncilorAttribute.None) ? 1f : (this.GetAttribute(primaryAttackerStat) / 6f));
				}
				if (this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor))
				{
					num += this.councilor.GetMonthlyIncome(FactionResource.Money) / 100f;
					num += this.councilor.GetMonthlyIncome(FactionResource.Influence) / 50f;
					num += this.councilor.GetMonthlyIncome(FactionResource.Operations) / 50f;
					num += this.councilor.GetMonthlyIncome(FactionResource.Research) / 50f;
					num += this.councilor.GetMonthlyIncome(FactionResource.Boost) / 25f;
					num += this.councilor.GetMonthlyIncome(FactionResource.MissionControl);
					num += this.councilor.GetMonthlyIncome(FactionResource.Projects) * 2f;
				}
				else
				{
					num += 5f;
				}
			}
			else
			{
				num = 25f;
			}
			return num;
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06002C26 RID: 11302 RVA: 0x000F1820 File Offset: 0x000EFA20
		public List<TIOrgState> orgs
		{
			get
			{
				if (this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor))
				{
					return this.councilor.orgs;
				}
				return new List<TIOrgState>();
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06002C27 RID: 11303 RVA: 0x000F1848 File Offset: 0x000EFA48
		public List<TITraitTemplate> traits
		{
			get
			{
				if (this.playerCouncil.HasIntelOnCouncilorDetails(this.councilor))
				{
					return this.councilor.traits;
				}
				if (this.playerCouncil.HasIntelOnCouncilorBasicData(this.councilor))
				{
					return this.councilor.traits.Where<TITraitTemplate>((TITraitTemplate x) => x.easilyVisible).ToList<TITraitTemplate>();
				}
				return new List<TITraitTemplate>();
			}
		}

		// Token: 0x04002172 RID: 8562
		private readonly TIFactionState playerCouncil;

		// Token: 0x04002173 RID: 8563
		public readonly TICouncilorState councilor;
	}
}
