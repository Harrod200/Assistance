using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000720 RID: 1824
	public class TICouncilorState : TIGameState
	{
		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06002C34 RID: 11316 RVA: 0x000F1B51 File Offset: 0x000EFD51
		// (set) Token: 0x06002C35 RID: 11317 RVA: 0x000F1B59 File Offset: 0x000EFD59
		public TIFactionState faction { get; private set; }

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06002C36 RID: 11318 RVA: 0x000F1B62 File Offset: 0x000EFD62
		// (set) Token: 0x06002C37 RID: 11319 RVA: 0x000F1B6A File Offset: 0x000EFD6A
		public TIFactionState agentForFaction { get; private set; }

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06002C38 RID: 11320 RVA: 0x000F1B73 File Offset: 0x000EFD73
		// (set) Token: 0x06002C39 RID: 11321 RVA: 0x000F1B7B File Offset: 0x000EFD7B
		public float autofailMissionsValue { get; private set; }

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06002C3A RID: 11322 RVA: 0x000F1B84 File Offset: 0x000EFD84
		// (set) Token: 0x06002C3B RID: 11323 RVA: 0x000F1B8C File Offset: 0x000EFD8C
		public TIGameState protectingTarget { get; private set; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06002C3C RID: 11324 RVA: 0x000F1B95 File Offset: 0x000EFD95
		// (set) Token: 0x06002C3D RID: 11325 RVA: 0x000F1B9D File Offset: 0x000EFD9D
		public TIGameState location { get; private set; }

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06002C3E RID: 11326 RVA: 0x000F1BA6 File Offset: 0x000EFDA6
		// (set) Token: 0x06002C3F RID: 11327 RVA: 0x000F1BAE File Offset: 0x000EFDAE
		public TIDateTime recruitDate { get; private set; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06002C40 RID: 11328 RVA: 0x000F1BB7 File Offset: 0x000EFDB7
		// (set) Token: 0x06002C41 RID: 11329 RVA: 0x000F1BBF File Offset: 0x000EFDBF
		public TIDateTime detainedReleaseDate { get; private set; }

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06002C42 RID: 11330 RVA: 0x000F1BC8 File Offset: 0x000EFDC8
		// (set) Token: 0x06002C43 RID: 11331 RVA: 0x000F1BD0 File Offset: 0x000EFDD0
		public List<string> traitTemplateNames { get; private set; }

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06002C44 RID: 11332 RVA: 0x000F1BD9 File Offset: 0x000EFDD9
		// (set) Token: 0x06002C45 RID: 11333 RVA: 0x000F1BE1 File Offset: 0x000EFDE1
		public List<string> learnedMissionsTemplateNames { get; private set; }

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06002C46 RID: 11334 RVA: 0x000F1BEA File Offset: 0x000EFDEA
		// (set) Token: 0x06002C47 RID: 11335 RVA: 0x000F1BF2 File Offset: 0x000EFDF2
		public Dictionary<CouncilorAttribute, int> attributes { get; private set; }

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06002C48 RID: 11336 RVA: 0x000F1BFB File Offset: 0x000EFDFB
		// (set) Token: 0x06002C49 RID: 11337 RVA: 0x000F1C03 File Offset: 0x000EFE03
		public List<TIFactionState> knowsIveBeenSeenBy { get; private set; }

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06002C4A RID: 11338 RVA: 0x000F1C0C File Offset: 0x000EFE0C
		// (set) Token: 0x06002C4B RID: 11339 RVA: 0x000F1C14 File Offset: 0x000EFE14
		public TIMissionState activeMission { get; private set; }

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06002C4C RID: 11340 RVA: 0x000F1C1D File Offset: 0x000EFE1D
		// (set) Token: 0x06002C4D RID: 11341 RVA: 0x000F1C25 File Offset: 0x000EFE25
		public TIMissionState completedMission { get; private set; }

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06002C4E RID: 11342 RVA: 0x000F1C2E File Offset: 0x000EFE2E
		// (set) Token: 0x06002C4F RID: 11343 RVA: 0x000F1C36 File Offset: 0x000EFE36
		public string priorMissionTemplateName { get; private set; }

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06002C50 RID: 11344 RVA: 0x000F1C3F File Offset: 0x000EFE3F
		// (set) Token: 0x06002C51 RID: 11345 RVA: 0x000F1C47 File Offset: 0x000EFE47
		public TIGameState priorMissionTarget { get; private set; }

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06002C52 RID: 11346 RVA: 0x000F1C50 File Offset: 0x000EFE50
		// (set) Token: 0x06002C53 RID: 11347 RVA: 0x000F1C58 File Offset: 0x000EFE58
		public bool repeatOrder { get; private set; }

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06002C54 RID: 11348 RVA: 0x000F1C61 File Offset: 0x000EFE61
		// (set) Token: 0x06002C55 RID: 11349 RVA: 0x000F1C69 File Offset: 0x000EFE69
		public bool permanentAssignment { get; private set; }

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06002C56 RID: 11350 RVA: 0x000F1C72 File Offset: 0x000EFE72
		// (set) Token: 0x06002C57 RID: 11351 RVA: 0x000F1C7A File Offset: 0x000EFE7A
		public bool permanentDefenseMode { get; private set; }

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06002C58 RID: 11352 RVA: 0x000F1C83 File Offset: 0x000EFE83
		// (set) Token: 0x06002C59 RID: 11353 RVA: 0x000F1C8B File Offset: 0x000EFE8B
		public List<string> missionsExcludedFromDefenseMode { get; private set; } = new List<string>();

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06002C5A RID: 11354 RVA: 0x000F1C94 File Offset: 0x000EFE94
		// (set) Token: 0x06002C5B RID: 11355 RVA: 0x000F1C9C File Offset: 0x000EFE9C
		[fsIgnore]
		public bool inTransit { get; private set; }

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06002C5C RID: 11356 RVA: 0x000F1CA5 File Offset: 0x000EFEA5
		// (set) Token: 0x06002C5D RID: 11357 RVA: 0x000F1CAD File Offset: 0x000EFEAD
		[fsIgnore]
		public List<TITraitTemplate> traits { get; private set; }

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06002C5E RID: 11358 RVA: 0x000F1CB6 File Offset: 0x000EFEB6
		// (set) Token: 0x06002C5F RID: 11359 RVA: 0x000F1CBE File Offset: 0x000EFEBE
		[fsIgnore]
		public List<TIMissionTemplate> learnedMissions { get; private set; }

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06002C60 RID: 11360 RVA: 0x000F1CC7 File Offset: 0x000EFEC7
		public override bool isCouncilorState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06002C61 RID: 11361 RVA: 0x000F1CCA File Offset: 0x000EFECA
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06002C62 RID: 11362 RVA: 0x000F1CCD File Offset: 0x000EFECD
		public override TICouncilorState ref_councilor
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06002C63 RID: 11363 RVA: 0x000F1CD0 File Offset: 0x000EFED0
		public override TIFactionState ref_faction
		{
			get
			{
				return this.faction;
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06002C64 RID: 11364 RVA: 0x000F1CD8 File Offset: 0x000EFED8
		public override TIRegionState ref_region
		{
			get
			{
				TIGameState location = this.location;
				return ((location != null) ? location.ref_region : null) ?? null;
			}
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06002C65 RID: 11365 RVA: 0x000F1CF1 File Offset: 0x000EFEF1
		public override TINationState ref_nation
		{
			get
			{
				TIRegionState ref_region = this.ref_region;
				return ((ref_region != null) ? ref_region.nation : null) ?? null;
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06002C66 RID: 11366 RVA: 0x000F1D0A File Offset: 0x000EFF0A
		public override TISpaceFleetState ref_fleet
		{
			get
			{
				return this.location.ref_fleet;
			}
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06002C67 RID: 11367 RVA: 0x000F1D17 File Offset: 0x000EFF17
		public override List<TIFactionState> ref_factions
		{
			get
			{
				if (!this.turned)
				{
					return new List<TIFactionState> { this.faction };
				}
				return new List<TIFactionState> { this.faction, this.agentForFaction };
			}
		}

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06002C68 RID: 11368 RVA: 0x000F1D50 File Offset: 0x000EFF50
		public override TIHabState ref_hab
		{
			get
			{
				TIGameState location = this.location;
				if (location == null)
				{
					return null;
				}
				return location.ref_hab;
			}
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06002C69 RID: 11369 RVA: 0x000F1D63 File Offset: 0x000EFF63
		public override TIHabSiteState ref_habSite
		{
			get
			{
				return this.location.ref_habSite;
			}
		}

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06002C6A RID: 11370 RVA: 0x000F1D70 File Offset: 0x000EFF70
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.location.ref_spaceBody;
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06002C6B RID: 11371 RVA: 0x000F1D7D File Offset: 0x000EFF7D
		public override TIOrbitState ref_orbit
		{
			get
			{
				return this.location.ref_orbit;
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x06002C6C RID: 11372 RVA: 0x000F1D8A File Offset: 0x000EFF8A
		public override TISpaceShipState ref_ship
		{
			get
			{
				TIGameState location = this.location;
				if (location == null)
				{
					return null;
				}
				return location.ref_ship;
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x06002C6D RID: 11373 RVA: 0x000F1D9D File Offset: 0x000EFF9D
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				TISpaceBodyState tispaceBodyState;
				if ((tispaceBodyState = this.ref_spaceBody) == null)
				{
					if (!(this.ref_fleet != null))
					{
						return this.ref_hab;
					}
					tispaceBodyState = this.ref_fleet;
				}
				return tispaceBodyState;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x06002C6E RID: 11374 RVA: 0x000F1DC4 File Offset: 0x000EFFC4
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.location.ref_naturalSpaceObject;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06002C6F RID: 11375 RVA: 0x000F1DD1 File Offset: 0x000EFFD1
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				TIHabState tihabState;
				if (!(this.location.ref_fleet != null))
				{
					if ((tihabState = this.location.ref_hab) == null)
					{
						return null;
					}
				}
				else
				{
					tihabState = this.location.ref_fleet;
				}
				return tihabState;
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06002C70 RID: 11376 RVA: 0x000F1E02 File Offset: 0x000F0002
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06002C71 RID: 11377 RVA: 0x000F1E05 File Offset: 0x000F0005
		public override bool hasEarthMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06002C72 RID: 11378 RVA: 0x000F1E08 File Offset: 0x000F0008
		public override bool inSpace
		{
			get
			{
				return this.OnAShip || this.InAHab;
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06002C73 RID: 11379 RVA: 0x000F1E1A File Offset: 0x000F001A
		public TICouncilorTemplate template
		{
			get
			{
				return this.GetMyTemplate<TICouncilorTemplate>();
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06002C74 RID: 11380 RVA: 0x000F1E22 File Offset: 0x000F0022
		public TINationState homeNation
		{
			get
			{
				if (!this.isAlien)
				{
					return this.homeRegion.nation;
				}
				return GameStateManager.AlienNation();
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06002C75 RID: 11381 RVA: 0x000F1E3D File Offset: 0x000F003D
		public TINationState currentNation
		{
			get
			{
				return this.ref_nation;
			}
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x06002C76 RID: 11382 RVA: 0x000F1E45 File Offset: 0x000F0045
		public bool turned
		{
			get
			{
				return this.agentForFaction != null;
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x06002C77 RID: 11383 RVA: 0x000F1E53 File Offset: 0x000F0053
		public bool detained
		{
			get
			{
				return this.detainingFaction != null;
			}
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06002C78 RID: 11384 RVA: 0x000F1E61 File Offset: 0x000F0061
		public bool active
		{
			get
			{
				return this.status == CouncilorStatus.Active && !this.detained;
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06002C79 RID: 11385 RVA: 0x000F1E77 File Offset: 0x000F0077
		public bool OnEarth
		{
			get
			{
				return this.ref_region != null;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06002C7A RID: 11386 RVA: 0x000F1E85 File Offset: 0x000F0085
		public bool OnOrAroundEarth
		{
			get
			{
				return this.ref_naturalSpaceObject.isEarth;
			}
		}

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06002C7B RID: 11387 RVA: 0x000F1E92 File Offset: 0x000F0092
		public bool OnAShip
		{
			get
			{
				return this.location.isSpaceShipState;
			}
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000F1E9F File Offset: 0x000F009F
		public bool InAHab
		{
			get
			{
				return this.location.isHabState || this.location.isHabModuleState;
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06002C7D RID: 11389 RVA: 0x000F1EBB File Offset: 0x000F00BB
		public bool AtABase
		{
			get
			{
				return this.InAHab && this.ref_hab.IsBase;
			}
		}

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06002C7E RID: 11390 RVA: 0x000F1ED2 File Offset: 0x000F00D2
		public bool OnAStation
		{
			get
			{
				return this.InAHab && this.ref_hab.IsStation;
			}
		}

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06002C7F RID: 11391 RVA: 0x000F1EE9 File Offset: 0x000F00E9
		public bool HasMission
		{
			get
			{
				return this.activeMission != null;
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06002C80 RID: 11392 RVA: 0x000F1EF7 File Offset: 0x000F00F7
		private int maxCouncilorAttribute
		{
			get
			{
				return TemplateManager.global.maxCouncilorAttribute;
			}
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06002C81 RID: 11393 RVA: 0x000F1F03 File Offset: 0x000F0103
		public bool elasticApparentLoyalty
		{
			get
			{
				return this.traits.None<TITraitTemplate>((TITraitTemplate x) => x.statMods.Any<StatModifier>((StatModifier y) => y.operation == StatModSetOperation.SetToFixedValue && y.stat == CouncilorAttribute.ApparentLoyalty));
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06002C82 RID: 11394 RVA: 0x000F1F2F File Offset: 0x000F012F
		public bool transparentLoyalty
		{
			get
			{
				return this.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.statMods.Any<StatModifier>((StatModifier y) => y.operation == StatModSetOperation.SetToAnotherAttribute && y.stat == CouncilorAttribute.ApparentLoyalty && y.strValue == "Loyalty"));
			}
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06002C83 RID: 11395 RVA: 0x000F1F5B File Offset: 0x000F015B
		public TICouncilorTypeTemplate typeTemplate
		{
			get
			{
				if (this._typeTemplate == null)
				{
					this._typeTemplate = TemplateManager.Find<TICouncilorTypeTemplate>(this.typeTemplateName, true);
				}
				return this._typeTemplate;
			}
		}

		// Token: 0x06002C84 RID: 11396 RVA: 0x000F1F80 File Offset: 0x000F0180
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			this.traits = new List<TITraitTemplate>();
			this.learnedMissions = new List<TIMissionTemplate>();
			this.attributes = new Dictionary<CouncilorAttribute, int>();
			this.learnedMissionsTemplateNames = new List<string>();
			this.traitTemplateNames = new List<string>();
			this.knowsIveBeenSeenBy = new List<TIFactionState>();
			this.orgs = new List<TIOrgState>();
			this.dateBorn = new TIDateTime();
			TICouncilorTemplate ticouncilorTemplate = template as TICouncilorTemplate;
			if (ticouncilorTemplate == null)
			{
				return;
			}
			this.templateName = ticouncilorTemplate.dataName;
			this.status = CouncilorStatus.Active;
			this.autofailMissionsValue = 0.5f;
			this.assassinations = new Dictionary<TIFactionState, int>();
		}

		// Token: 0x06002C85 RID: 11397 RVA: 0x000F2020 File Offset: 0x000F0220
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (!this.gameStateSubjectCreated)
			{
				this.NewCharacterGeneration(null, null, null, false, false);
			}
			else
			{
				if (TemplateManager.Find<TICouncilorTemplate>(this.templateName, false) == null)
				{
					this.templateName = "randomizedCouncilor1";
				}
				this.SetLearnedMissions();
				this.SetTraits();
				if (this.appearanceTemplate == null)
				{
					Log.Error("New appearance assigned. Missing apperance template " + this.appearanceTemplateName, Array.Empty<object>());
					this.appearanceTemplateName = this.SelectAppearance();
				}
			}
			if (!TIGameState.Valid(this.location))
			{
				TIRegionState tiregionState = this.homeRegion;
				this.SetLocation(((tiregionState != null) ? tiregionState.ref_gameState : null) ?? GameStateManager.AlienFaction().primaryHab.ref_gameState);
			}
			if (this.assassinations == null)
			{
				this.assassinations = new Dictionary<TIFactionState, int>();
			}
			if (string.IsNullOrEmpty(this.locationIllustration.illustrationPath))
			{
				this.locationIllustration = this.SetIllustrationData(this.location, true, false);
			}
			GameControl.eventManager.TriggerEvent(new CouncilorPositionUpdated(this, this.location), null, (from x in new object[]
				{
					this,
					this.faction,
					this.location,
					this.location.ref_nation,
					this.location.ref_fleet,
					this.location.ref_spaceBody
				}.Distinct<object>()
				where x != null
				select x).ToArray<object>());
		}

		// Token: 0x06002C86 RID: 11398 RVA: 0x000F2194 File Offset: 0x000F0394
		public override void PostVisualizerCreationInit_6()
		{
			if (this.detained)
			{
				if (this.detainedReleaseDate == null || TITimeState.Now() > this.detainedReleaseDate)
				{
					Log.Error("Councilor " + this.displayName + " was not released on schedule. Releasing now.", Array.Empty<object>());
					this.ReleaseCouncilor(true);
				}
				else
				{
					GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledCouncilorRelease), this.ReleaseDetailedCouncilorEventName, null, true, false);
				}
			}
			for (int i = 0; i < this.orgs.Count; i++)
			{
				if (this.orgs[i].deleted)
				{
					Log.Error("Removing deleted org " + this.orgs[i].ID.ToString() + " from councilor " + base.ID.ToString(), Array.Empty<object>());
					this.orgs.Remove(this.orgs[i]);
				}
				else if (this.orgs[i].assignedCouncilor != this)
				{
					Log.Error(this.displayName + " has org not recorded as assigned to this councilor.", Array.Empty<object>());
					this.orgs[i].AssignCouncilor(this);
				}
			}
			if (this.appearanceTemplate == null)
			{
				Log.Error("New appearance assigned. Missing apperance template " + this.appearanceTemplateName, Array.Empty<object>());
				this.appearanceTemplateName = this.SelectAppearance();
			}
		}

		// Token: 0x06002C87 RID: 11399 RVA: 0x000F2318 File Offset: 0x000F0518
		public void RemoveFromGoals()
		{
			if (this.faction != null)
			{
				foreach (FactionGoal_Fleet factionGoal_Fleet in new List<FactionGoal_Fleet>(this.faction.AllFleetGoals(false)))
				{
					FactionGoal_FleetCouncilorGoal factionGoal_FleetCouncilorGoal = factionGoal_Fleet as FactionGoal_FleetCouncilorGoal;
					if (factionGoal_FleetCouncilorGoal != null)
					{
						factionGoal_FleetCouncilorGoal.assignedCouncilors.Remove(this);
					}
				}
			}
		}

		// Token: 0x06002C88 RID: 11400 RVA: 0x000F2394 File Offset: 0x000F0594
		public void Retire()
		{
			if (this.HasMission)
			{
				this.activeMission.ResolveMission((this.status == CouncilorStatus.Dead) ? TIMissionState.AbortReason.CouncilorDead : TIMissionState.AbortReason.CouncilorRetired, "");
			}
			this.RemoveFromGoals();
			TIHabState ref_hab = this.ref_hab;
			if (ref_hab != null)
			{
				ref_hab.RemoveAdvisingCouncilor(this);
			}
			TINationState currentNation = this.currentNation;
			if (currentNation != null)
			{
				currentNation.RemoveAdvisingCouncilor(this);
			}
			base.ArchiveState(true);
			if (this.isAlien)
			{
				int num = (int)(this.MonthsSinceRecruitDate() / 3f);
				for (int i = 0; i < num; i++)
				{
					GameStateManager.AllRegions().SelectRandomItem<TIRegionState>().ConductAbductions(this.faction, -1);
				}
				if (this.inSpace)
				{
					foreach (TIOrgState tiorgState in this.orgs.ToList<TIOrgState>())
					{
						if (tiorgState.templateName == TemplateManager.global.alienShockTroopOrgDataName)
						{
							this.orgs.Remove(tiorgState);
							GameStateManager.RemoveGameState<TIOrgState>(tiorgState.ID, false);
						}
					}
				}
			}
			TINotificationQueueState.CleanQueueOfArchivedState(this, this.location);
			if (this.detained)
			{
				World.Active.GetExistingManager<GameTimeManager>().CancelTimeEvent(this.ReleaseDetailedCouncilorEventName, null, null, null, this.detainedReleaseDate);
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledCouncilorRelease), this.ReleaseDetailedCouncilorEventName);
			}
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				tifactionState.ExpireIntel(this, true);
				if (tifactionState.lastRecordedLoyalty.ContainsKey(this))
				{
					tifactionState.lastRecordedLoyalty.Remove(this);
					tifactionState.lastTimeSecretsWereSeen.Remove(this);
				}
			}
			if (this.turned)
			{
				this.UnTurnCouncilor(false, false);
			}
			List<TIOrgState> list = new List<TIOrgState>(this.orgs);
			foreach (TIOrgState tiorgState2 in list)
			{
				this.faction.AddOrgToFactionPool(tiorgState2, this, tiorgState2 != list.Last<TIOrgState>());
			}
			TIFactionState faction = this.faction;
			if (faction != null)
			{
				faction.SetResourceIncomeDataDirty(TIFactionState.councilorResources);
			}
			TIFactionState faction2 = this.faction;
			if (faction2 != null)
			{
				faction2.councilors.Remove(this);
			}
			TIFactionState faction3 = this.faction;
			if (faction3 != null)
			{
				faction3.ValidateAllOrgs(false);
			}
			GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
			{
				x.availableCouncilors.Remove(this);
			});
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(this.faction, this, this.location, false), null, Array.Empty<object>());
			this.EndProtectionOfTarget();
			this.GetProtectors().ToList<TICouncilorState>().ForEach(delegate(TICouncilorState x)
			{
				x.EndProtectionOfTarget();
			});
			this.faction = null;
			this.location = null;
			TIGlobalValuesState.GlobalValues.councilorAppearanceTemplatesInUse.Remove(this.appearanceTemplateName);
			GameStateManager.RemoveGameState<TICouncilorState>(base.ID, false);
		}

		// Token: 0x06002C89 RID: 11401 RVA: 0x000F26AC File Offset: 0x000F08AC
		public void SetDisplayName()
		{
			this.displayName = (this.personalName + " " + this.familyName).Trim();
		}

		// Token: 0x06002C8A RID: 11402 RVA: 0x000F26D0 File Offset: 0x000F08D0
		public override string GetDisplayName(TIFactionState faction)
		{
			return faction.GetViewofCouncilor(this).displayNameCurrent;
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x000F26EC File Offset: 0x000F08EC
		public TIResourcesCost HireRecruitCost(TIFactionState faction)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			float num = 0f;
			if (!faction.ideology.alien && !this.template.alien)
			{
				if (this.typeTemplate.affinities.Contains(faction.ideology.ideology))
				{
					num = (float)TemplateManager.global.affinityCouncilorRecruitCost_influence;
				}
				else if (this.typeTemplate.antiAffinities.Contains(faction.ideology.ideology))
				{
					num = (float)TemplateManager.global.antiAffinityCouncilorRecruitCost_influence;
				}
				else
				{
					num = (float)TemplateManager.global.baseCouncilorRecruitCost_influence;
				}
			}
			tiresourcesCost.AddCost(FactionResource.Influence, num, true);
			return tiresourcesCost;
		}

		// Token: 0x06002C8C RID: 11404 RVA: 0x000F278C File Offset: 0x000F098C
		public static Tuple<string, string> GenerateNameFromRegionAncestry(TIRegionState homeRegion, CouncilorAncestry ancestry, CouncilorGender gender)
		{
			string text = "Firstname";
			string text2 = "Lastname";
			string[] array;
			string[] array2;
			float[] array3;
			switch (ancestry)
			{
			case CouncilorAncestry.African:
				array = homeRegion.template.afrPersonal;
				array2 = homeRegion.template.afrFamily;
				array3 = homeRegion.template.afrWeight;
				break;
			case CouncilorAncestry.Asian:
				array = homeRegion.template.asiPersonal;
				array2 = homeRegion.template.asiFamily;
				array3 = homeRegion.template.asiWeight;
				break;
			case CouncilorAncestry.EastAsian:
				array = homeRegion.template.easPersonal;
				array2 = homeRegion.template.easFamily;
				array3 = homeRegion.template.easWeight;
				break;
			case CouncilorAncestry.European:
				array = homeRegion.template.eurPersonal;
				array2 = homeRegion.template.eurFamily;
				array3 = homeRegion.template.eurWeight;
				break;
			case CouncilorAncestry.Hispanic:
				array = homeRegion.template.hisPersonal;
				array2 = homeRegion.template.hisFamily;
				array3 = homeRegion.template.hisWeight;
				break;
			case CouncilorAncestry.Oceanic:
				array = homeRegion.template.ocePersonal;
				array2 = homeRegion.template.oceFamily;
				array3 = homeRegion.template.oceWeight;
				break;
			default:
				array = null;
				array2 = null;
				array3 = null;
				break;
			}
			if (array3 == null)
			{
				Log.Error("Bad ancestry " + ancestry.ToString() + " passed to GenerateNameFromRegionAncestry:" + ((homeRegion != null) ? homeRegion.displayName : null), Array.Empty<object>());
			}
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			int num = 0;
			bool flag = true;
			while (num < 10 && flag)
			{
				dictionary.Clear();
				int num2 = 0;
				foreach (float num3 in array3)
				{
					dictionary.Add(num2, num3);
					num2++;
				}
				int key = dictionary.SelectRandomWeightedItem<KeyValuePair<int, float>>((KeyValuePair<int, float> j) => j.Value, -1f, 1E-37f).Key;
				string text3 = array[key];
				string text4 = array2[key];
				CouncilorName councilorName;
				CouncilorName councilorName2;
				if (gender != CouncilorGender.Female)
				{
					if (gender == CouncilorGender.Male)
					{
						councilorName.gender = "male";
						councilorName2.gender = "male";
					}
					else
					{
						councilorName.gender = "any";
						councilorName2.gender = "any";
					}
				}
				else
				{
					councilorName.gender = "female";
					councilorName2.gender = "female";
				}
				councilorName.group = text3;
				councilorName2.group = text4;
				councilorName.segment = "personal";
				councilorName2.segment = "family";
				if (!GameControl.namelists.TryGetName<CouncilorName>(councilorName, out text))
				{
					councilorName.gender = "any";
					text = GameControl.namelists.GetName<CouncilorName>(councilorName);
				}
				if (!GameControl.namelists.TryGetName<CouncilorName>(councilorName2, out text2))
				{
					councilorName2.gender = "any";
					text2 = GameControl.namelists.GetName<CouncilorName>(councilorName2);
				}
				if (text2.Contains(text) || text.Contains(text2))
				{
					flag = true;
					num++;
				}
				else
				{
					flag = false;
				}
			}
			return new Tuple<string, string>(text, text2);
		}

		// Token: 0x06002C8D RID: 11405 RVA: 0x000F2A94 File Offset: 0x000F0C94
		public static CouncilorAncestry RandomizeAncestryFromRegion(TIRegionState homeRegion)
		{
			Dictionary<CouncilorAncestry, float> dictionary = new Dictionary<CouncilorAncestry, float>();
			dictionary.Add(CouncilorAncestry.African, homeRegion.template.afr.GetValueOrDefault());
			dictionary.Add(CouncilorAncestry.Asian, homeRegion.template.asi.GetValueOrDefault());
			dictionary.Add(CouncilorAncestry.EastAsian, homeRegion.template.eas.GetValueOrDefault());
			dictionary.Add(CouncilorAncestry.European, homeRegion.template.eur.GetValueOrDefault());
			dictionary.Add(CouncilorAncestry.Hispanic, homeRegion.template.his.GetValueOrDefault());
			dictionary.Add(CouncilorAncestry.Oceanic, homeRegion.template.oce.GetValueOrDefault());
			return dictionary.SelectRandomWeightedItem<KeyValuePair<CouncilorAncestry, float>>((KeyValuePair<CouncilorAncestry, float> k) => k.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06002C8E RID: 11406 RVA: 0x000F2B68 File Offset: 0x000F0D68
		private void RandomizeBirthday()
		{
			int num = TIUtilities.RandomRange(8, 23) + TIUtilities.RandomRange(8, 23) + TIUtilities.RandomRange(8, 23);
			this.dateBorn.year = TITimeState.Now().year - num;
			this.dateBorn.month = TIUtilities.RandomRange(1, 13);
			this.dateBorn.day = TIUtilities.RandomRange(1, DateTime.DaysInMonth(this.dateBorn.year, this.dateBorn.month));
		}

		// Token: 0x06002C8F RID: 11407 RVA: 0x000F2BE8 File Offset: 0x000F0DE8
		public static CouncilorGender RandomizeGender(TIRegionState homeRegion)
		{
			Dictionary<CouncilorGender, float> dictionary = new Dictionary<CouncilorGender, float>();
			dictionary.Add(CouncilorGender.Male, 50.21f);
			dictionary.Add(CouncilorGender.Female, 49.21f * Math.Min(1f, homeRegion.nation.education / 10f));
			return dictionary.SelectRandomWeightedItem<KeyValuePair<CouncilorGender, float>>((KeyValuePair<CouncilorGender, float> k) => k.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06002C90 RID: 11408 RVA: 0x000F2C64 File Offset: 0x000F0E64
		public static TIRegionState RandomizeRegionWeightedByPopulation(bool considerSocialDemographics, TIFactionState forFaction = null)
		{
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
			{
				float num = tiregionState.populationInMillions;
				if (considerSocialDemographics)
				{
					TINationState nation = tiregionState.nation;
					if (Error.IsNull<TINationState>(nation))
					{
						Error.Log("nationState is null in RandomizeRegionWeightedByPopulation: " + tiregionState.displayName, Array.Empty<object>());
					}
					num *= nation.education / 10f;
					if (tiregionState.coreEconomicRegion)
					{
						num *= TemplateManager.global.characterGenRegionCoreEcoModifier;
					}
					if (tiregionState.nation.education >= TemplateManager.global.characterGenRegionHighEducationThreshhold)
					{
						num *= TemplateManager.global.characterGenRegionHighEducationModifer;
					}
					if (tiregionState.nation.education >= TemplateManager.global.characterGenRegionVeryHighEducationThreshhold)
					{
						num *= TemplateManager.global.characterGenRegionVeryHighEducationModifier;
					}
					if (forFaction != null && tiregionState.nation.publicOpinion.ContainsKey(forFaction.ideology.ideology))
					{
						num *= Mathf.Max(tiregionState.nation.publicOpinion[forFaction.ideology.ideology], 0.5f);
					}
				}
				dictionary.Add(tiregionState, num);
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> k) => k.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06002C91 RID: 11409 RVA: 0x000F2DD8 File Offset: 0x000F0FD8
		private void RandomizeJob(TIFactionState faction, bool considerAvailable)
		{
			Dictionary<TICouncilorTypeTemplate, float> dictionary = TemplateManager.IterateByClass<TICouncilorTypeTemplate>(false).ToDictionary<TICouncilorTypeTemplate, TICouncilorTypeTemplate, float>((TICouncilorTypeTemplate jobTemplate) => jobTemplate, delegate(TICouncilorTypeTemplate jobTemplate)
			{
				if (!jobTemplate.unlocked)
				{
					return 0f;
				}
				float weight = jobTemplate.weight;
				float num;
				if (!considerAvailable)
				{
					num = 1f;
				}
				else
				{
					int num2 = 1;
					TIFactionState faction2 = faction;
					int? num3 = ((faction2 != null) ? new int?(faction2.councilors.Count<TICouncilorState>((TICouncilorState x) => x.typeTemplate == jobTemplate)) : null);
					TIFactionState faction3 = faction;
					int? num4 = ((faction3 != null) ? new int?(faction3.availableCouncilors.Count<TICouncilorState>((TICouncilorState x) => x.typeTemplate == jobTemplate)) : null);
					num = Mathf.Pow((float)Mathf.Max(num2, (((num3 != null) & (num4 != null)) ? new int?(num3.GetValueOrDefault() + num4.GetValueOrDefault() + 1) : null) ?? 1), 2f);
				}
				return weight / num;
			});
			this._typeTemplate = dictionary.SelectRandomWeightedItem<KeyValuePair<TICouncilorTypeTemplate, float>>((KeyValuePair<TICouncilorTypeTemplate, float> k) => k.Value, -1f, 1E-37f).Key;
			this.typeTemplateName = this._typeTemplate.dataName;
		}

		// Token: 0x06002C92 RID: 11410 RVA: 0x000F2E80 File Offset: 0x000F1080
		private void RandomizeStats(TIFactionState forFaction, bool forceBestStats)
		{
			TICouncilorTypeTemplate typeTemplate = this._typeTemplate;
			CouncilorAttribute[] array;
			if (typeTemplate == null)
			{
				array = null;
			}
			else
			{
				CouncilorAttribute[] keyStat = typeTemplate.keyStat;
				if (keyStat == null)
				{
					array = null;
				}
				else
				{
					array = keyStat.Where<CouncilorAttribute>((CouncilorAttribute x) => x > CouncilorAttribute.None).Distinct<CouncilorAttribute>().ToArray<CouncilorAttribute>();
				}
			}
			CouncilorAttribute[] array2 = array;
			if (forceBestStats && !this.isAlien)
			{
				if (array2.Length > 1)
				{
					this.attributes[array2[0]] = 7;
					this.attributes[array2[1]] = 6;
				}
				else if (array2.Length == 1)
				{
					this.attributes[array2[0]] = 8;
				}
			}
			foreach (CouncilorAttribute councilorAttribute in Enums.CouncilorAttributes)
			{
				if (!forceBestStats || array2 == null || !array2.Contains(councilorAttribute))
				{
					switch (councilorAttribute)
					{
					case CouncilorAttribute.Persuasion:
						this.attributes[CouncilorAttribute.Persuasion] = this.typeTemplate.basePersuasion + TIUtilities.RandomRange(0, this.typeTemplate.randPersuasion + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.basePersuasion, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Investigation:
						this.attributes[CouncilorAttribute.Investigation] = this.typeTemplate.baseInvestigation + TIUtilities.RandomRange(0, this.typeTemplate.randInvestigation + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseInvestigation, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Espionage:
						this.attributes[CouncilorAttribute.Espionage] = this.typeTemplate.baseEspionage + TIUtilities.RandomRange(0, this.typeTemplate.randEspionage + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseEspionage, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Command:
						this.attributes[CouncilorAttribute.Command] = this.typeTemplate.baseCommand + TIUtilities.RandomRange(0, this.typeTemplate.randCommand + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseCommand, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Administration:
						this.attributes[CouncilorAttribute.Administration] = this.typeTemplate.baseAdministration + TIUtilities.RandomRange(0, this.typeTemplate.randAdministration + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseAdministration, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Science:
						this.attributes[CouncilorAttribute.Science] = this.typeTemplate.baseScience + TIUtilities.RandomRange(0, this.typeTemplate.randScience + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseScience, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Security:
						this.attributes[CouncilorAttribute.Security] = this.typeTemplate.baseSecurity + TIUtilities.RandomRange(0, this.typeTemplate.randSecurity + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseSecurity, councilorAttribute.ToString());
						break;
					case CouncilorAttribute.Loyalty:
						this.attributes[CouncilorAttribute.Loyalty] = this.typeTemplate.baseLoyalty + TIUtilities.RandomRange(0, this.typeTemplate.randLoyalty + 1) + (int)TIEffectsState.SumEffectsModifiers(Context.AllRecruitStats, forFaction, (float)this.typeTemplate.baseLoyalty, councilorAttribute.ToString());
						break;
					}
				}
			}
			this.attributes[CouncilorAttribute.ApparentLoyalty] = this.attributes[CouncilorAttribute.Loyalty] - 2 + TIUtilities.RandomRange(0, 4);
			foreach (KeyValuePair<CouncilorAttribute, int> keyValuePair in this.attributes.ToList<KeyValuePair<CouncilorAttribute, int>>())
			{
				if (keyValuePair.Value < 0)
				{
					this.attributes[keyValuePair.Key] = 0;
				}
			}
		}

		// Token: 0x06002C93 RID: 11411 RVA: 0x000F3290 File Offset: 0x000F1490
		public float GetIndividualTraitChance(TITraitTemplate traitTemplate, TIFactionState forFaction)
		{
			for (int i = 0; i < traitTemplate.classChance.Count; i++)
			{
				if (traitTemplate.classChance[i].councilorClass == this.typeTemplateName)
				{
					float num = traitTemplate.classChance[i].chance ?? traitTemplate.baseChance.GetValueOrDefault();
					return num + TIEffectsState.SumEffectsModifiers(Context.TraitSpawnChance, forFaction, num, traitTemplate.dataName);
				}
			}
			float valueOrDefault = traitTemplate.baseChance.GetValueOrDefault();
			return valueOrDefault + TIEffectsState.SumEffectsModifiers(Context.TraitSpawnChance, forFaction, valueOrDefault, traitTemplate.dataName);
		}

		// Token: 0x06002C94 RID: 11412 RVA: 0x000F3334 File Offset: 0x000F1534
		public void AddTrait(string templateName)
		{
			TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(templateName, false);
			if (titraitTemplate != null)
			{
				this.AddTrait(titraitTemplate, false);
			}
		}

		// Token: 0x06002C95 RID: 11413 RVA: 0x000F3354 File Offset: 0x000F1554
		public void AddTrait(TITraitTemplate template, bool notify = false)
		{
			if (!this.traits.Contains(template) && template != null)
			{
				this.traitTemplateNames.Add(template.dataName);
				this.traits.Add(template);
				TIFactionState faction = this.faction;
				if (faction != null)
				{
					faction.SetResourceIncomeDataDirty();
				}
				this.SetAttributesDirty();
				TIFactionState faction2 = this.faction;
				if (faction2 != null)
				{
					faction2.ValidateAllOrgs(false);
				}
				if (notify)
				{
					TINotificationQueueState.LogCouncilorGainsTrait(this, template);
				}
				GameControl.eventManager.TriggerEvent(new CouncilorValuesChanged(this), null, new object[] { this });
				return;
			}
			Log.Warn(this.displayName + "given duplicate trait or null trait: " + ((template != null) ? template.displayName : null), Array.Empty<object>());
		}

		// Token: 0x06002C96 RID: 11414 RVA: 0x000F3408 File Offset: 0x000F1608
		public bool RemoveTrait(TITraitTemplate template)
		{
			if (template != null)
			{
				this.traitTemplateNames.Remove(template.dataName);
				bool flag = this.traits.Remove(template);
				if (flag)
				{
					TIFactionState faction = this.faction;
					if (faction != null)
					{
						faction.SetResourceIncomeDataDirty();
					}
					this.SetAttributesDirty();
					TIFactionState faction2 = this.faction;
					if (faction2 != null)
					{
						faction2.ValidateAllOrgs(false);
					}
					GameControl.eventManager.TriggerEvent(new CouncilorValuesChanged(this), null, new object[] { this });
				}
				return flag;
			}
			return false;
		}

		// Token: 0x06002C97 RID: 11415 RVA: 0x000F3484 File Offset: 0x000F1684
		public void SetTraits()
		{
			if (this.traits == null)
			{
				this.traits = new List<TITraitTemplate>();
			}
			foreach (string text in this.traitTemplateNames)
			{
				TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(text, false);
				if (titraitTemplate != null)
				{
					this.traits.Add(titraitTemplate);
				}
				else
				{
					Log.Error("Bad trait template Name " + text + " in list for " + this.displayName, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06002C98 RID: 11416 RVA: 0x000F3520 File Offset: 0x000F1720
		private void RandomizeTraits(TIFactionState faction)
		{
			this.traits.Clear();
			this.traitTemplateNames.Clear();
			int num = 0;
			List<int> list = new List<int>();
			foreach (TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(false))
			{
				float individualTraitChance = this.GetIndividualTraitChance(titraitTemplate, faction);
				if (titraitTemplate.grouping == null || individualTraitChance >= 100f)
				{
					if ((float)Mathd.d100() <= individualTraitChance)
					{
						this.AddTrait(titraitTemplate, false);
						if (titraitTemplate.grouping != null)
						{
							list.Add(titraitTemplate.grouping.Value);
						}
					}
				}
				else if (titraitTemplate.grouping != null)
				{
					int? num2 = titraitTemplate.grouping;
					int num3 = num;
					if ((num2.GetValueOrDefault() > num3) & (num2 != null))
					{
						num = titraitTemplate.grouping.Value;
					}
				}
			}
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			for (int i = 1; i <= num; i++)
			{
				if (!list.Contains(i))
				{
					float num4 = 0f;
					dictionary.Clear();
					foreach (TITraitTemplate titraitTemplate2 in TemplateManager.IterateByClass<TITraitTemplate>(false))
					{
						int? num2 = titraitTemplate2.grouping;
						int num3 = i;
						if ((num2.GetValueOrDefault() == num3) & (num2 != null))
						{
							dictionary.Add(titraitTemplate2.dataName, this.GetIndividualTraitChance(titraitTemplate2, faction));
							num4 += this.GetIndividualTraitChance(titraitTemplate2, faction);
						}
					}
					if (dictionary.Count > 0)
					{
						if (num4 < 100f)
						{
							dictionary.Add("", Math.Max(100f - num4, 0f));
						}
						string key = dictionary.SelectRandomWeightedItem<KeyValuePair<string, float>>((KeyValuePair<string, float> j) => j.Value, -1f, 1E-37f).Key;
						TITraitTemplate titraitTemplate3 = (TITraitTemplate)TemplateManager.Find(key, typeof(TITraitTemplate), false);
						if (key != "" && !this.traits.Contains(titraitTemplate3))
						{
							this.AddTrait(titraitTemplate3, false);
						}
					}
				}
			}
			foreach (TITraitTemplate titraitTemplate4 in this.traits.ToList<TITraitTemplate>())
			{
				titraitTemplate4.RerollTrait(this, faction);
			}
		}

		// Token: 0x06002C99 RID: 11417 RVA: 0x000F37C0 File Offset: 0x000F19C0
		private void AssignStatsFromTemplate()
		{
			this.attributes[CouncilorAttribute.Persuasion] = Mathf.Clamp(this.template.persuasion ?? this.attributes[CouncilorAttribute.Persuasion], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Espionage] = Mathf.Clamp(this.template.espionage ?? this.attributes[CouncilorAttribute.Espionage], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Command] = Mathf.Clamp(this.template.command ?? this.attributes[CouncilorAttribute.Command], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Investigation] = Mathf.Clamp(this.template.investigation ?? this.attributes[CouncilorAttribute.Investigation], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Science] = Mathf.Clamp(this.template.science ?? this.attributes[CouncilorAttribute.Science], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Administration] = Mathf.Clamp(this.template.administration ?? this.attributes[CouncilorAttribute.Administration], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Security] = Mathf.Clamp(this.template.security ?? this.attributes[CouncilorAttribute.Security], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.Loyalty] = Mathf.Clamp(this.template.loyalty ?? this.attributes[CouncilorAttribute.Loyalty], 0, TemplateManager.global.maxCouncilorAttribute);
			this.attributes[CouncilorAttribute.ApparentLoyalty] = this.attributes[CouncilorAttribute.Loyalty];
		}

		// Token: 0x06002C9A RID: 11418 RVA: 0x000F3A18 File Offset: 0x000F1C18
		private string SelectAppearance()
		{
			if (!string.IsNullOrEmpty(this.template.appearanceTemplateName))
			{
				TICouncilorAppearanceTemplate ticouncilorAppearanceTemplate = TemplateManager.Find<TICouncilorAppearanceTemplate>(this.template.appearanceTemplateName, false);
				if (ticouncilorAppearanceTemplate != null && ticouncilorAppearanceTemplate.enable)
				{
					return ticouncilorAppearanceTemplate.dataName;
				}
			}
			int year = GameStateManager.Time().template.year - 50;
			List<TICouncilorAppearanceTemplate> list = (from aTemplate in TemplateManager.IterateByClass<TICouncilorAppearanceTemplate>(true)
				where aTemplate.ValidForCharacter(this, year, true, true, true)
				select aTemplate).ToList<TICouncilorAppearanceTemplate>();
			if (list.Count == 0)
			{
				list = (from aTemplate in TemplateManager.IterateByClass<TICouncilorAppearanceTemplate>(true)
					where aTemplate.ValidForCharacter(this, year, false, true, true)
					select aTemplate).ToList<TICouncilorAppearanceTemplate>();
				if (list.Count == 0)
				{
					list = (from aTemplate in TemplateManager.IterateByClass<TICouncilorAppearanceTemplate>(true)
						where aTemplate.ValidForCharacter(this, year, false, false, true)
						select aTemplate).ToList<TICouncilorAppearanceTemplate>();
					if (list.Count == 0)
					{
						list = (from aTemplate in TemplateManager.IterateByClass<TICouncilorAppearanceTemplate>(true)
							where aTemplate.ValidForCharacter(this, year, false, false, false)
							select aTemplate).ToList<TICouncilorAppearanceTemplate>();
					}
				}
			}
			if (list.Count > 0)
			{
				return list.SelectRandomItem<TICouncilorAppearanceTemplate>().dataName;
			}
			if (this.gender != CouncilorGender.Female)
			{
				return "CharImage6";
			}
			return "CharImage2";
		}

		// Token: 0x06002C9B RID: 11419 RVA: 0x000F3B38 File Offset: 0x000F1D38
		public void NewCharacterGeneration(TICouncilorTypeTemplate forcedJob = null, TIRegionState forcedRegion = null, TIFactionState forFaction = null, bool forceMaxStats = false, bool startup = false)
		{
			if (!this.template.alien)
			{
				this.RandomizeBirthday();
				if (!this.template.randomized)
				{
					this.dateBorn.SetTime(this.template.yearBorn ?? this.dateBorn.year, this.template.monthBorn ?? this.dateBorn.month, this.template.dayBorn ?? this.dateBorn.day, 0, 0, 0, 0);
					int num = TITimeState.Now().year - 2022;
					if (this.template.yearBorn != null && (num < 0 || num > 10))
					{
						this.dateBorn.SetTime(this.dateBorn.year + num, this.dateBorn.month, this.dateBorn.day, 0, 0, 0, 0);
					}
				}
				if (forcedRegion == null)
				{
					this.homeRegion = TICouncilorState.RandomizeRegionWeightedByPopulation(true, forFaction);
					if (!this.template.randomized)
					{
						bool flag = TemplateManager.Find<TIMapRegionTemplate>(this.template.mapRegionBorn ?? "", false) != null;
						TIRegionTemplate tiregionTemplate = TemplateManager.Find<TIRegionTemplate>(this.template.regionBorn ?? "", false);
						if (flag)
						{
							this.homeRegion = GameStateManager.MapRegionLookup(this.template.mapRegionBorn);
						}
						else if (tiregionTemplate != null)
						{
							this.homeRegion = GameStateManager.FindByTemplate<TIRegionState>(this.template.regionBorn, false);
						}
						else if (this.template.regionBorn.Length > 0)
						{
							Log.Warn("Councilor " + this.template.dataName + " has nonexisting home region " + this.template.regionBorn, Array.Empty<object>());
						}
					}
				}
				else
				{
					this.homeRegion = forcedRegion;
				}
				this.ancestry = TICouncilorState.RandomizeAncestryFromRegion(this.homeRegion);
				if (!this.template.randomized)
				{
					CouncilorAncestry councilorAncestry = (this.template.strAncestry ?? "").ToEnum(CouncilorAncestry.None);
					if (councilorAncestry != CouncilorAncestry.None)
					{
						this.ancestry = councilorAncestry;
					}
				}
				this.gender = TICouncilorState.RandomizeGender(this.homeRegion);
				if (!this.template.randomized)
				{
					CouncilorGender councilorGender = (this.template.strGender ?? "").ToEnum(CouncilorGender.None);
					if (councilorGender != CouncilorGender.None)
					{
						this.gender = councilorGender;
					}
				}
				if (!this.template.randomized)
				{
					this.personalName = this.template.personalName ?? "Missing Personal Name";
					this.familyName = this.template.familyName ?? "Missing Family Name";
					this.SetDisplayName();
				}
				else
				{
					Tuple<string, string> tuple = TICouncilorState.GenerateNameFromRegionAncestry(this.homeRegion, this.ancestry, this.gender);
					this.personalName = tuple.Item1;
					this.familyName = tuple.Item2;
					this.SetDisplayName();
				}
				if (forcedJob == null)
				{
					this.RandomizeJob(forFaction, true);
				}
				else
				{
					this._typeTemplate = forcedJob;
					this.typeTemplateName = forcedJob.dataName;
				}
				if (!this.template.randomized)
				{
					if (this.template.type != "")
					{
						this.typeTemplateName = this.template.type ?? this.typeTemplateName;
					}
					this._typeTemplate = TemplateManager.Find<TICouncilorTypeTemplate>(this.typeTemplateName, false);
					if (this._typeTemplate == null)
					{
						this.RandomizeJob(forFaction, false);
						Error.Log("Bad job name" + this.template.type + " passed to character creator in TICouncilorState", Array.Empty<object>());
					}
				}
				this.RandomizeStats(forFaction, forceMaxStats);
				if (!this.template.randomized)
				{
					this.AssignStatsFromTemplate();
				}
				if (this.template.randomizeTraits)
				{
					this.RandomizeTraits(forFaction);
					if (!this.template.randomized && !this.template.allowRandomOnlyTraits)
					{
						this.traits.RemoveAll((TITraitTemplate x) => x.randomCouncilorsOnly);
						this.traitTemplateNames = this.traits.Select<TITraitTemplate, string>((TITraitTemplate x) => x.dataName).ToList<string>();
					}
					if (startup)
					{
						this.traits.RemoveAll((TITraitTemplate x) => x.restrictedLocations > RestrictedLocations.None);
						this.traitTemplateNames = this.traits.Select<TITraitTemplate, string>((TITraitTemplate x) => x.dataName).ToList<string>();
					}
				}
				else
				{
					this.traits.Clear();
					this.traitTemplateNames.Clear();
					if (this.template.traits != null)
					{
						string[] array = this.template.traits;
						for (int i = 0; i < array.Length; i++)
						{
							TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(array[i], false);
							if (titraitTemplate != null)
							{
								bool flag2 = false;
								foreach (TITraitTemplate titraitTemplate2 in this.traits)
								{
									int? num2 = titraitTemplate2.grouping;
									int? num3 = titraitTemplate.grouping;
									if ((num2.GetValueOrDefault() == num3.GetValueOrDefault()) & (num2 != null == (num3 != null)))
									{
										flag2 = true;
									}
								}
								if (!flag2 && !this.traits.Contains(titraitTemplate))
								{
									this.AddTrait(titraitTemplate, false);
								}
							}
						}
					}
				}
				if (forFaction != null)
				{
					foreach (TIEffectTemplate tieffectTemplate in TIEffectsState.GetFactionEffectsForContext(Context.AllRecruitTraits, forFaction))
					{
						if (tieffectTemplate.value == 1f)
						{
							TITraitTemplate titraitTemplate3 = TemplateManager.Find<TITraitTemplate>(tieffectTemplate.strValue, false);
							if (titraitTemplate3 != null && titraitTemplate3.CouncilorCanHave(this, forFaction, true))
							{
								this.AddTrait(titraitTemplate3, false);
							}
						}
					}
				}
				this.SetLocation(this.homeRegion);
				using (List<TITraitTemplate>.Enumerator enumerator = this.traits.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TITraitTemplate titraitTemplate4 = enumerator.Current;
						RestrictedLocations restrictedLocations = titraitTemplate4.restrictedLocations;
						if (restrictedLocations - RestrictedLocations.HomeNation > 1)
						{
							if (restrictedLocations == RestrictedLocations.HighUnrestNations)
							{
								if (this.homeNation.unrest >= TemplateManager.global.HighUnrestDefinition)
								{
									IEnumerable<TINationState> enumerable = from x in GameStateManager.AllExtantHumanNations()
										where x.unrest < TemplateManager.global.HighUnrestDefinition
										select x;
									if (enumerable.Count<TINationState>() > 0)
									{
										this.SetLocation(enumerable.SelectRandomItem<TINationState>().capital);
									}
								}
							}
						}
						else
						{
							IEnumerable<TINationState> enumerable2 = from x in GameStateManager.AllExtantHumanNations()
								where !x.IsAlliedWith(this.homeNation, true)
								select x;
							if (enumerable2.Count<TINationState>() > 0)
							{
								this.SetLocation(enumerable2.SelectRandomWeightedItem<TINationState>((TINationState x) => (float)x.numControlPoints_unclamped, -1f, 1E-37f).capital);
							}
						}
						if (this.location == null)
						{
							this.SetLocation(this.homeRegion);
						}
					}
					goto IL_0965;
				}
			}
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			tifactionState.councilorsGenerated++;
			this.RandomizeBirthday();
			this.gender = CouncilorGender.None;
			this.ancestry = CouncilorAncestry.Alien;
			this.typeTemplateName = "Alien";
			this.RandomizeStats(tifactionState, forceMaxStats);
			this.personalName = Loc.T("TICouncilorTemplate.alienName1");
			this.familyName = Loc.T("TICouncilorTemplate.alienName2", new object[] { tifactionState.councilorsGenerated.ToString() });
			this.SetDisplayName();
			if (TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, tifactionState))
			{
				if (GameStateManager.AlienNation().extant)
				{
					this.SetLocation(GameStateManager.AlienNation().capital);
				}
				else
				{
					IEnumerable<TIRegionState> enumerable3 = from x in GameStateManager.AllRegions()
						where x.alienFacility.Extant()
						select x;
					if (enumerable3.Count<TIRegionState>() > 0)
					{
						this.SetLocation(enumerable3.SelectRandomItem<TIRegionState>().ref_region);
					}
					else
					{
						IEnumerable<TIRegionState> enumerable4 = from x in GameStateManager.AllRegions()
							where x.alienLanding.Extant()
							select x;
						if (enumerable4.Count<TIRegionState>() > 0)
						{
							this.SetLocation(enumerable4.SelectRandomItem<TIRegionState>().ref_region);
						}
						else
						{
							this.SetLocation(tifactionState.primaryHab);
						}
					}
				}
			}
			else
			{
				this.SetLocation(tifactionState.primaryHab);
			}
			if (this.template.traits != null)
			{
				string[] array = this.template.traits;
				for (int i = 0; i < array.Length; i++)
				{
					TITraitTemplate titraitTemplate5 = TemplateManager.Find<TITraitTemplate>(array[i], false);
					if (titraitTemplate5 != null)
					{
						bool flag3 = false;
						foreach (TITraitTemplate titraitTemplate6 in this.traits)
						{
							int? num3 = titraitTemplate6.grouping;
							int? num2 = titraitTemplate5.grouping;
							if ((num3.GetValueOrDefault() == num2.GetValueOrDefault()) & (num3 != null == (num2 != null)))
							{
								flag3 = true;
							}
						}
						if (!flag3 && !this.traits.Contains(titraitTemplate5))
						{
							this.AddTrait(titraitTemplate5, false);
						}
					}
				}
			}
			IL_0965:
			if (!this.template.randomized)
			{
				foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
				{
					if (tifactionState2.templateName == this.template.debugStartingCouncil && tifactionState2.councilors.Count < 6)
					{
						tifactionState2.AddAvailableCouncilor(this, true);
						tifactionState2.SetIntel(this, TemplateManager.global.intelToSeeCouncilorMission, null, false);
						break;
					}
				}
				foreach (TINationState tinationState in GameStateManager.AllExtantNations())
				{
					if (tinationState.templateName == this.template.debugStartingNation)
					{
						this.SetLocation(tinationState.capital);
						break;
					}
				}
			}
			this.appearanceTemplateName = this.SelectAppearance();
			TIGlobalValuesState.GlobalValues.councilorAppearanceTemplatesInUse.Add(this.appearanceTemplateName);
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06002C9C RID: 11420 RVA: 0x000F461C File Offset: 0x000F281C
		public bool isAlien
		{
			get
			{
				return this.template.alien;
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06002C9D RID: 11421 RVA: 0x000F4629 File Offset: 0x000F2829
		public bool isHuman
		{
			get
			{
				return !this.isAlien;
			}
		}

		// Token: 0x06002C9E RID: 11422 RVA: 0x000F4634 File Offset: 0x000F2834
		public void UpdateBiographicalInformation(string givenName, string familyName, TICouncilorAppearanceTemplate appearanceTemplate, TICouncilorVoiceTemplate voiceTemplate)
		{
			this.personalName = givenName;
			this.familyName = familyName;
			this.SetDisplayName();
			this.appearanceTemplateName = appearanceTemplate.dataName;
			this._appearanceTemplate = appearanceTemplate;
			this.voiceTemplateName = voiceTemplate.dataName;
			this._voiceTemplate = voiceTemplate;
			GameControl.eventManager.TriggerEvent(new CouncilorVisibilityChanged(this, this.faction), null, (from x in new object[]
				{
					this,
					this.faction,
					this.location,
					this.location.ref_region,
					this.location.ref_nation,
					this.location.ref_hab,
					this.location.ref_fleet,
					this.location.ref_naturalSpaceObject
				}.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			GameControl.eventManager.TriggerEvent(new CouncilorValuesChanged(this), null, new object[] { this });
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06002C9F RID: 11423 RVA: 0x000F4740 File Offset: 0x000F2940
		public int age
		{
			get
			{
				TIDateTime tidateTime = TITimeState.Now();
				return (int)((tidateTime != null) ? new double?(tidateTime.DifferenceInJulianYears(this.dateBorn)) : null).Value;
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06002CA0 RID: 11424 RVA: 0x000F477A File Offset: 0x000F297A
		public TICouncilorAppearanceTemplate appearanceTemplate
		{
			get
			{
				if (this._appearanceTemplate == null)
				{
					this._appearanceTemplate = TemplateManager.Find<TICouncilorAppearanceTemplate>(this.appearanceTemplateName, false);
				}
				return this._appearanceTemplate;
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06002CA1 RID: 11425 RVA: 0x000F479C File Offset: 0x000F299C
		public bool useOldPortrait
		{
			get
			{
				return !this.isAlien && this.age > TICouncilorAppearanceTemplate.ageCutPoint;
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06002CA2 RID: 11426 RVA: 0x000F47B5 File Offset: 0x000F29B5
		public string videoResource
		{
			get
			{
				return this.appearanceTemplate.idleVideo(this);
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06002CA3 RID: 11427 RVA: 0x000F47C3 File Offset: 0x000F29C3
		public string portraitResource
		{
			get
			{
				return this.appearanceTemplate.portrait(this);
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06002CA4 RID: 11428 RVA: 0x000F47D1 File Offset: 0x000F29D1
		public string iconResource
		{
			get
			{
				return this.appearanceTemplate.icon(this);
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06002CA5 RID: 11429 RVA: 0x000F47DF File Offset: 0x000F29DF
		public string iconBackground
		{
			get
			{
				return TemplateManager.global.pathCouncilorIconBackground;
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06002CA6 RID: 11430 RVA: 0x000F47EC File Offset: 0x000F29EC
		public TICouncilorVoiceTemplate voiceTemplate
		{
			get
			{
				if (this._voiceTemplate == null)
				{
					this._voiceTemplate = TemplateManager.Find<TICouncilorVoiceTemplate>(this.voiceTemplateName, false);
				}
				if (this._voiceTemplate == null)
				{
					Debug.LogWarning(string.Concat(new string[] { "Missing Voice Template for: ", this.voiceTemplateName, " on councilor: ", this.displayName, ", assigning fallback" }));
					if (this.gender == CouncilorGender.Male || this.gender == CouncilorGender.Female)
					{
						this._voiceTemplate = (from x in TemplateManager.IterateByClass<TICouncilorVoiceTemplate>(true)
							where x.eGender == this.gender
							select x).FirstOrDefault<TICouncilorVoiceTemplate>();
					}
					else
					{
						this._voiceTemplate = TemplateManager.IterateByClass<TICouncilorVoiceTemplate>(true).First<TICouncilorVoiceTemplate>();
					}
				}
				return this._voiceTemplate;
			}
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x000F48A8 File Offset: 0x000F2AA8
		public void SelectVoice()
		{
			if (!string.IsNullOrEmpty(this.template.voiceTemplateName))
			{
				TICouncilorVoiceTemplate ticouncilorVoiceTemplate = TemplateManager.Find<TICouncilorVoiceTemplate>(this.template.voiceTemplateName, false);
				if (ticouncilorVoiceTemplate != null && ticouncilorVoiceTemplate.enable)
				{
					this.voiceTemplateName = ticouncilorVoiceTemplate.dataName;
					return;
				}
			}
			else if (!this.isAlien && string.IsNullOrEmpty(this.voiceTemplateName))
			{
				Dictionary<TICouncilorVoiceTemplate, int> dictionary = (from aTemplate in TemplateManager.IterateByClass<TICouncilorVoiceTemplate>(true)
					where aTemplate.ValidForCharacter(this, this.gender, this.homeRegion.template.language, this.homeRegion.template.accent(this.ancestry)) && !aTemplate.specific_person
					select aTemplate).ToDictionary<TICouncilorVoiceTemplate, TICouncilorVoiceTemplate, int>((TICouncilorVoiceTemplate aTemplate) => aTemplate, (TICouncilorVoiceTemplate aTemplate) => 1);
				if (dictionary.Count > 0)
				{
					this.voiceTemplateName = dictionary.SelectRandomWeightedItem<KeyValuePair<TICouncilorVoiceTemplate, int>>((KeyValuePair<TICouncilorVoiceTemplate, int> j) => (float)j.Value, -1f, 1E-37f).Key.dataName;
					return;
				}
				this.voiceTemplateName = new StringBuilder(this.homeRegion.template.language).Append("_").Append(this.homeRegion.template.accent(this.ancestry)).Append("_")
					.Append((this.gender == CouncilorGender.Male) ? "M" : "F")
					.Append("_0")
					.ToString();
			}
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x000F4A2C File Offset: 0x000F2C2C
		public void PlayMissionVoice(TIMissionTemplate missionTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation voiceMissionSituation, bool onEarth)
		{
			TICouncilorVoiceTemplate voiceTemplate = this.voiceTemplate;
			if (voiceTemplate == null)
			{
				return;
			}
			voiceTemplate.PlayMissionVoice(missionTemplate, voiceMissionSituation, onEarth, true);
		}

		// Token: 0x06002CA9 RID: 11433 RVA: 0x000F4A42 File Offset: 0x000F2C42
		public void PlayMissionVoice(TIMissionTemplate missionTemplate, TIMissionOutcome voiceMissionOutcome, bool onEarth)
		{
			TICouncilorVoiceTemplate voiceTemplate = this.voiceTemplate;
			if (voiceTemplate == null)
			{
				return;
			}
			voiceTemplate.PlayMissionVoice(missionTemplate, voiceMissionOutcome, onEarth);
		}

		// Token: 0x06002CAA RID: 11434 RVA: 0x000F4A57 File Offset: 0x000F2C57
		public void PlaySelectionVoice()
		{
			TICouncilorVoiceTemplate voiceTemplate = this.voiceTemplate;
			if (voiceTemplate == null)
			{
				return;
			}
			voiceTemplate.PlaySelectionVoice(this, this.OnEarth);
		}

		// Token: 0x06002CAB RID: 11435 RVA: 0x000F4A70 File Offset: 0x000F2C70
		public void SetAttributesDirty()
		{
			this.cachedFinalAttributeValues.Clear();
			TIFactionState faction = this.faction;
			if (faction == null)
			{
				return;
			}
			faction.SetCouncilStatsDirty();
		}

		// Token: 0x06002CAC RID: 11436 RVA: 0x000F4A90 File Offset: 0x000F2C90
		public int GetClampedMaxStatValue(CouncilorAttribute attribute)
		{
			int num = this.maxCouncilorAttribute;
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				int num2 = titraitTemplate.ApplyTraitStatValue(attribute, this, this.faction, WhichStatModifier.UnconditionalOnly, false, null);
				if (num2 < 0)
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x06002CAD RID: 11437 RVA: 0x000F4AFC File Offset: 0x000F2CFC
		public int GetAttribute(CouncilorAttribute type, bool includeOrgs = true, bool includeAllUnconditionalTraits = true, bool capped = true, bool adminForOrgControl = false, bool useProspectiveOrgs = false, bool fullyUnclamped = false)
		{
			bool flag = includeOrgs && includeAllUnconditionalTraits && capped && !adminForOrgControl && !useProspectiveOrgs;
			int num;
			if (flag && this.cachedFinalAttributeValues.TryGetValue(type, out num))
			{
				return num;
			}
			int num2 = this.attributes[type];
			int num3 = this.maxCouncilorAttribute;
			if (includeAllUnconditionalTraits)
			{
				foreach (TITraitTemplate titraitTemplate in this.traits)
				{
					int num4 = titraitTemplate.ApplyTraitStatValue(type, this, this.faction, WhichStatModifier.UnconditionalOnly, false, null);
					num2 += num4;
					if (num4 < 0)
					{
						num3 += num4;
					}
				}
			}
			if (includeOrgs)
			{
				List<TIOrgState> list;
				if (useProspectiveOrgs)
				{
					list = this.prospectiveOrgs;
				}
				else
				{
					list = (adminForOrgControl ? this.orgs : this.activeOrgs);
				}
				foreach (TIOrgState tiorgState in list)
				{
					int statBonus = tiorgState.GetStatBonus(type);
					num2 += statBonus;
					if (statBonus < 0)
					{
						num3 += statBonus;
					}
				}
			}
			if (fullyUnclamped)
			{
				return num2;
			}
			int num5 = (capped ? Mathf.Clamp(num2, 0, num3) : Mathf.Max(0, num2));
			if (flag)
			{
				this.cachedFinalAttributeValues[type] = num5;
			}
			return num5;
		}

		// Token: 0x06002CAE RID: 11438 RVA: 0x000F4C50 File Offset: 0x000F2E50
		public bool ModifyAttribute(CouncilorAttribute attribute, int value)
		{
			if (value == 0)
			{
				return false;
			}
			int num = this.attributes[attribute];
			Dictionary<CouncilorAttribute, int> attributes = this.attributes;
			attributes[attribute] += value;
			this.attributes[attribute] = Mathf.Clamp(this.attributes[attribute], 0, this.maxCouncilorAttribute);
			if (this.attributes[attribute] != num)
			{
				this.SetAttributesDirty();
				GameControl.eventManager.TriggerEvent(new CouncilorValuesChanged(this), null, new object[] { this });
				if (TICouncilorState.resourceModifyingAttributes.Contains(attribute))
				{
					TIFactionState faction = this.faction;
					if (faction != null)
					{
						faction.SetResourceIncomeDataDirty();
					}
				}
				if (attribute == CouncilorAttribute.Administration && value < 0)
				{
					TIFactionState faction2 = this.faction;
					if (faction2 != null)
					{
						faction2.ValidateAllOrgs(false);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06002CAF RID: 11439 RVA: 0x000F4D18 File Offset: 0x000F2F18
		public float DetectCouncilorScore
		{
			get
			{
				return (float)(this.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false) + this.traits.Sum<TITraitTemplate>((TITraitTemplate x) => x.detectionInvBonus));
			}
		}

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06002CB0 RID: 11440 RVA: 0x000F4D54 File Offset: 0x000F2F54
		public float HideScore
		{
			get
			{
				return (float)(this.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false) + this.traits.Sum<TITraitTemplate>((TITraitTemplate x) => x.detectionEspBonus)) + Mathf.Max(TemplateManager.global.postRecruitingDetectionDefenseMultiplier * ((float)TemplateManager.global.monthsOfDetectionDefenseAfterRecruiting - this.MonthsSinceRecruitDate()), 0f);
			}
		}

		// Token: 0x06002CB1 RID: 11441 RVA: 0x000F4DC4 File Offset: 0x000F2FC4
		public int SumMissionRelevantAttributes()
		{
			int num = 0;
			foreach (CouncilorAttribute councilorAttribute in Enums.CouncilorAttributes)
			{
				if (councilorAttribute != CouncilorAttribute.Loyalty && councilorAttribute != CouncilorAttribute.ApparentLoyalty)
				{
					num += this.GetAttribute(councilorAttribute, true, true, true, false, false, false);
				}
			}
			return num;
		}

		// Token: 0x06002CB2 RID: 11442 RVA: 0x000F4E05 File Offset: 0x000F3005
		public void SetFaction(TIFactionState faction)
		{
			this.faction = faction;
			this.knowsIveBeenSeenBy.Clear();
		}

		// Token: 0x06002CB3 RID: 11443 RVA: 0x000F4E19 File Offset: 0x000F3019
		public void SetRecruitDate()
		{
			this.recruitDate = new TIDateTime(TITimeState.Now());
		}

		// Token: 0x06002CB4 RID: 11444 RVA: 0x000F4E2B File Offset: 0x000F302B
		public float MonthsSinceRecruitDate()
		{
			if (this.recruitDate != null)
			{
				return (float)TITimeState.Now().DifferenceInDays(this.recruitDate) / 30.436874f;
			}
			return 0f;
		}

		// Token: 0x06002CB5 RID: 11445 RVA: 0x000F4E58 File Offset: 0x000F3058
		public void ProtectTarget(TIGameState target)
		{
			this.protectingTarget = target;
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x000F4E61 File Offset: 0x000F3061
		public void EndProtectionOfTarget()
		{
			this.protectingTarget = null;
		}

		// Token: 0x06002CB7 RID: 11447 RVA: 0x000F4E6C File Offset: 0x000F306C
		public List<TICouncilorState> GetProtectors()
		{
			if (this.faction == null)
			{
				return new List<TICouncilorState>();
			}
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in this.faction.councilors)
			{
				if (ticouncilorState.active && ticouncilorState.location == this.location && ticouncilorState.protectingTarget == this)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x06002CB8 RID: 11448 RVA: 0x000F4F08 File Offset: 0x000F3108
		public float GetProtectionBonus(CouncilorAttribute attribute)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.GetProtectors())
			{
				num += (float)ticouncilorState.GetAttribute(attribute, true, true, true, false, false, false);
			}
			return num;
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06002CB9 RID: 11449 RVA: 0x000F4F6C File Offset: 0x000F316C
		public string ReleaseDetailedCouncilorEventName
		{
			get
			{
				return new StringBuilder("ReleaseDetainedCouncilor").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06002CBA RID: 11450 RVA: 0x000F4FA4 File Offset: 0x000F31A4
		public string DetainCouncilor(TIFactionState newDetainingFaction, float baseDuration_Turns, float extendDuration_Turns, bool passiveCapture)
		{
			if (passiveCapture && (this.isAlien || newDetainingFaction == this.faction))
			{
				return string.Empty;
			}
			float num = Mathf.Max(0.5f, baseDuration_Turns / TIMissionPhaseState.phasesPerMonth);
			newDetainingFaction.SetIntelIfValueHigher(this, TemplateManager.global.intelToSeeCouncilorDetails + 0.05f, null);
			if (this.detainingFaction != newDetainingFaction)
			{
				World.Active.GetExistingManager<GameTimeManager>().CancelAllTimeEventsByName(this.ReleaseDetailedCouncilorEventName);
				TIDateTime tidateTime = TITimeState.Now();
				if (this.detainingFaction != null)
				{
					World.Active.GetExistingManager<GameTimeManager>().CancelTimeEvent(this.ReleaseDetailedCouncilorEventName, null, null, null, this.detainedReleaseDate);
				}
				this.detainingFaction = newDetainingFaction;
				tidateTime.ExportTime();
				if (num > 0f)
				{
					tidateTime.AddDays(num * 30.436874f);
				}
				this.detainedReleaseDate = new TIDateTime(tidateTime);
				TITimeEvent.CreateNewTimeEvent(tidateTime, null, null, null, this.ReleaseDetailedCouncilorEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
				GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledCouncilorRelease), this.ReleaseDetailedCouncilorEventName, null, true, false);
				GameControl.eventManager.TriggerEvent(new CouncilorPositionUpdated(this, this.location), null, (from x in new object[]
					{
						this,
						this.faction,
						this.location,
						this.location.ref_nation,
						this.location.ref_fleet,
						this.location.ref_spaceBody
					}.Distinct<object>()
					where x != null
					select x).ToArray<object>());
				this.locationIllustration = this.SetIllustrationData(this.location, true, false);
				if (newDetainingFaction != this.faction)
				{
					TINotificationQueueState.LogMyCouncilorDetained(newDetainingFaction, this);
				}
				if (passiveCapture)
				{
					TINotificationQueueState.LogIPassivelyCapturedACouncilor(this, newDetainingFaction, this.location);
				}
				this.DeactivateAllOrgs();
				return tidateTime.ToCustomDateString();
			}
			int num2 = (int)(extendDuration_Turns / TIMissionPhaseState.phasesPerMonth * 30.436874f);
			this.detainingFaction.GainIntel(this, (float)((this.maxCouncilorAttribute - this.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false)) / this.maxCouncilorAttribute), null, false);
			World.Active.GetExistingManager<GameTimeManager>().ExtendTimeEvent(this.ReleaseDetailedCouncilorEventName, null, null, null, num2, TITimeQueueRepeatType.Day);
			this.detainedReleaseDate.AddDays((float)num2);
			return this.detainedReleaseDate.ToCustomDateString();
		}

		// Token: 0x06002CBB RID: 11451 RVA: 0x000F51F5 File Offset: 0x000F33F5
		public void ScheduledCouncilorRelease(TimeEventStart e)
		{
			if (this.status == CouncilorStatus.Dead || !this.detained)
			{
				return;
			}
			this.ReleaseCouncilor(true);
			if (this.faction != null)
			{
				TINotificationQueueState.AddCouncilorMessage(this, CouncilorChatType.CouncilorReleased, this.faction);
			}
		}

		// Token: 0x06002CBC RID: 11452 RVA: 0x000F522C File Offset: 0x000F342C
		public void ReleaseCouncilor(bool onTime)
		{
			if (this.status == CouncilorStatus.Dead || !this.detained)
			{
				return;
			}
			if (onTime)
			{
				float num = (float)((this.maxCouncilorAttribute - this.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false)) / this.maxCouncilorAttribute);
				num += TIEffectsState.SumEffectsModifiers(Context.InterrogationBonus, this.detainingFaction, num, null);
				TIFactionState tifactionState = this.detainingFaction;
				if (tifactionState != null)
				{
					tifactionState.GainIntel(this, num, null, false);
				}
			}
			if (this.faction != null)
			{
				TINotificationQueueState.LogMyCouncilorReleased(this.detainingFaction, this);
				this.locationIllustration = this.SetIllustrationData(this.location, true, false);
			}
			World.Active.GetExistingManager<GameTimeManager>().CancelAllTimeEventsByName(this.ReleaseDetailedCouncilorEventName);
			this.detainingFaction = null;
			this.detainedReleaseDate = null;
			TIFactionState faction = this.faction;
			if (faction != null)
			{
				faction.SetResourceIncomeDataDirty();
			}
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ScheduledCouncilorRelease), this.ReleaseDetailedCouncilorEventName);
			GameControl.eventManager.TriggerEvent(new CouncilorPositionUpdated(this, this.location), null, (from x in new object[]
				{
					this,
					this.faction,
					this.location,
					this.location.ref_nation,
					this.location.ref_fleet,
					this.location.ref_spaceBody
				}.Distinct<object>()
				where x != null
				select x).ToArray<object>());
		}

		// Token: 0x06002CBD RID: 11453 RVA: 0x000F539C File Offset: 0x000F359C
		public void TurnCouncilor(TIFactionState turningFaction)
		{
			if (this.turned)
			{
				this.UnTurnCouncilor(false, turningFaction == this.faction);
			}
			this.agentForFaction = turningFaction;
			turningFaction.turnedCouncilors.Add(this);
			turningFaction.SetIntelIfValueHigher(this, TemplateManager.global.intelToSeeCouncilorSecrets, null);
			this.PassIntel();
			if (this.faction.GetIntel(this) < TemplateManager.global.intelToSeeCouncilorSecrets)
			{
				if (!this.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.LoyaltyMonitor))
				{
					goto IL_00A1;
				}
			}
			this.faction.knownSpies.Add(this);
			TINotificationQueueState.LogSpyDiscovered(this);
			IL_00A1:
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(this.faction, this, this.location, false), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(this.agentForFaction, this, this.location, false), null, Array.Empty<object>());
			if (turningFaction != null && turningFaction.isActivePlayer)
			{
				turningFaction.UnlockAchievement("turnCouncilor");
				if (turningFaction.councilors.Count >= 6 && turningFaction.turnedCouncilors.Count == 2)
				{
					turningFaction.UnlockAchievement("controlFullCouncilTurned");
				}
			}
		}

		// Token: 0x06002CBE RID: 11454 RVA: 0x000F54D3 File Offset: 0x000F36D3
		public bool AutofailTurnedCouncilor(TIMissionTemplate mission, TIGameState target)
		{
			return this.turned && target != null && target.ref_faction == this.agentForFaction && mission.dataName == TIFactionState.assassinateMission.dataName;
		}

		// Token: 0x06002CBF RID: 11455 RVA: 0x000F5513 File Offset: 0x000F3713
		public void PassIntel()
		{
			if (this.agentForFaction != null)
			{
				this.faction.GiveIntelToFaction(this.agentForFaction, true);
			}
		}

		// Token: 0x06002CC0 RID: 11456 RVA: 0x000F5538 File Offset: 0x000F3738
		public void UnTurnCouncilor(bool dismissedByTurningFaction, bool betraysToFaction)
		{
			if (!dismissedByTurningFaction)
			{
				TINotificationQueueState.LogSpyLost(this.agentForFaction, this, betraysToFaction);
			}
			if (betraysToFaction)
			{
				this.agentForFaction.GiveIntelToFaction(this.faction, true);
			}
			this.agentForFaction.turnedCouncilors.Remove(this);
			this.faction.knownSpies.Remove(this);
			TIFactionState agentForFaction = this.agentForFaction;
			this.agentForFaction = null;
			agentForFaction.SetIntelIfValueLower(this, TIGlobalConfig.globalConfig.intelToSeeCouncilorDetails, null, false);
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(agentForFaction, this, this.location, false), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new CouncilCompositionChanged(this.faction, this, this.location, false), null, Array.Empty<object>());
		}

		// Token: 0x06002CC1 RID: 11457 RVA: 0x000F55F0 File Offset: 0x000F37F0
		public void KillCouncilorOnMission(TIMissionState mission)
		{
			TINotificationQueueState.LogMyCouncilorKilledOnMission(mission);
			TINotificationQueueState.LogEnemyCouncilorKilledOnMissionTargetingMe(mission);
			this.KillCouncilor(true, mission.target.ref_faction);
		}

		// Token: 0x06002CC2 RID: 11458 RVA: 0x000F5610 File Offset: 0x000F3810
		public void KillCouncilor(bool violent, TIFactionState killer = null)
		{
			TIFactionState faction = this.faction;
			if (violent && faction != null)
			{
				if (this.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.GlobalPropagandaIfKilled))
				{
					TITraitTemplate.ProcessPropagandaFromTraits(faction, SpecialTraitRule.GlobalPropagandaIfKilled, this.traits.First<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.GlobalPropagandaIfKilled).specialTraitRuleValue);
				}
			}
			this.status = CouncilorStatus.Dead;
			this.Retire();
			if (faction != null)
			{
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.MyCouncilorKilled, faction, killer);
				if (violent)
				{
					TITraitTemplate.ProcessLoyaltyChangeFromTraits(faction, SpecialTraitRule.LoyaltyLossOnFactionCouncilorKilled, 1);
				}
			}
		}

		// Token: 0x06002CC3 RID: 11459 RVA: 0x000F56BD File Offset: 0x000F38BD
		public void SetAutofailMissionsValue(float value)
		{
			this.autofailMissionsValue = Mathf.Clamp(value, 0f, 1f);
		}

		// Token: 0x06002CC4 RID: 11460 RVA: 0x000F56D8 File Offset: 0x000F38D8
		public float GetResourceMultiplierFromAttributes(FactionResource resourceType)
		{
			float num = 1f;
			switch (resourceType)
			{
			case FactionResource.Money:
				num += (float)this.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false) / 100f;
				break;
			case FactionResource.Influence:
				num += (float)this.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) / 100f;
				break;
			case FactionResource.Operations:
				num += (float)this.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false) / 100f;
				break;
			case FactionResource.Research:
				num += (float)this.GetAttribute(CouncilorAttribute.Science, true, true, true, false, false, false) / 100f;
				break;
			}
			return num;
		}

		// Token: 0x06002CC5 RID: 11461 RVA: 0x000F5768 File Offset: 0x000F3968
		private float GetMonthlyIncomeFromTraits(FactionResource resource)
		{
			float num = 0f;
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				switch (resource)
				{
				case FactionResource.Money:
					num += titraitTemplate.incomeMoney;
					break;
				case FactionResource.Influence:
					num += titraitTemplate.incomeInfluence;
					break;
				case FactionResource.Operations:
					num += titraitTemplate.incomeOps;
					break;
				case FactionResource.Research:
					num += titraitTemplate.incomeResearch;
					break;
				case FactionResource.Projects:
					num += (float)titraitTemplate.incomeProjects;
					break;
				case FactionResource.Boost:
					num += titraitTemplate.incomeBoost;
					break;
				}
			}
			return num;
		}

		// Token: 0x06002CC6 RID: 11462 RVA: 0x000F5820 File Offset: 0x000F3A20
		private float GetMonthlyIncomeFromOrgs(FactionResource resource)
		{
			float num = 0f;
			foreach (TIOrgState tiorgState in this.activeOrgs)
			{
				switch (resource)
				{
				case FactionResource.Money:
					num += tiorgState.adjustedIncomeMoney_month;
					break;
				case FactionResource.Influence:
					num += tiorgState.adjustedIncomeInfluence_month;
					break;
				case FactionResource.Operations:
					num += tiorgState.adjustedIncomeOps_month;
					break;
				case FactionResource.Research:
					num += tiorgState.adjustedIncomeResearch_month;
					break;
				case FactionResource.Projects:
					num += (float)tiorgState.projectCapacityGranted;
					break;
				case FactionResource.Boost:
					num += tiorgState.adjustedIncomeBoost_month;
					break;
				case FactionResource.MissionControl:
					num += tiorgState.incomeMissionControl;
					break;
				}
			}
			return num;
		}

		// Token: 0x06002CC7 RID: 11463 RVA: 0x000F58E8 File Offset: 0x000F3AE8
		private float GetMonthlyIncomeFromTraits_PositiveOnly(FactionResource resource)
		{
			float num = 0f;
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				switch (resource)
				{
				case FactionResource.Money:
					num += ((titraitTemplate.incomeMoney > 0f) ? titraitTemplate.incomeMoney : 0f);
					break;
				case FactionResource.Influence:
					num += ((titraitTemplate.incomeInfluence > 0f) ? titraitTemplate.incomeInfluence : 0f);
					break;
				case FactionResource.Operations:
					num += ((titraitTemplate.incomeOps > 0f) ? titraitTemplate.incomeOps : 0f);
					break;
				case FactionResource.Research:
					num += titraitTemplate.incomeResearch;
					break;
				case FactionResource.Projects:
					num += (float)titraitTemplate.incomeProjects;
					break;
				case FactionResource.Boost:
					num += ((titraitTemplate.incomeBoost > 0f) ? titraitTemplate.incomeBoost : 0f);
					break;
				}
			}
			return num;
		}

		// Token: 0x06002CC8 RID: 11464 RVA: 0x000F59F8 File Offset: 0x000F3BF8
		private float GetMonthlyIncomeFromTraits_NegativeOnly(FactionResource resource)
		{
			float num = 0f;
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				switch (resource)
				{
				case FactionResource.Money:
					num += ((titraitTemplate.incomeMoney < 0f) ? titraitTemplate.incomeMoney : 0f);
					break;
				case FactionResource.Influence:
					num += ((titraitTemplate.incomeInfluence < 0f) ? titraitTemplate.incomeInfluence : 0f);
					break;
				case FactionResource.Operations:
					num += ((titraitTemplate.incomeOps < 0f) ? titraitTemplate.incomeOps : 0f);
					break;
				case FactionResource.Boost:
					num += ((titraitTemplate.incomeBoost < 0f) ? titraitTemplate.incomeBoost : 0f);
					break;
				}
			}
			return num;
		}

		// Token: 0x06002CC9 RID: 11465 RVA: 0x000F5AF0 File Offset: 0x000F3CF0
		private float GetMonthlyIncomeFromOrgs_PositiveOnly(FactionResource resource)
		{
			float num = 0f;
			foreach (TIOrgState tiorgState in this.activeOrgs)
			{
				switch (resource)
				{
				case FactionResource.Money:
					num += ((tiorgState.adjustedIncomeMoney_month > 0f) ? tiorgState.adjustedIncomeMoney_month : 0f);
					break;
				case FactionResource.Influence:
					num += ((tiorgState.adjustedIncomeInfluence_month > 0f) ? tiorgState.adjustedIncomeInfluence_month : 0f);
					break;
				case FactionResource.Operations:
					num += ((tiorgState.adjustedIncomeOps_month > 0f) ? tiorgState.adjustedIncomeOps_month : 0f);
					break;
				case FactionResource.Research:
					num += tiorgState.adjustedIncomeResearch_month;
					break;
				case FactionResource.Projects:
					num += (float)tiorgState.projectCapacityGranted;
					break;
				case FactionResource.Boost:
					num += ((tiorgState.adjustedIncomeBoost_month > 0f) ? tiorgState.adjustedIncomeBoost_month : 0f);
					break;
				case FactionResource.MissionControl:
					num += ((tiorgState.incomeMissionControl > 0f) ? tiorgState.incomeMissionControl : 0f);
					break;
				}
			}
			return num;
		}

		// Token: 0x06002CCA RID: 11466 RVA: 0x000F5C28 File Offset: 0x000F3E28
		private float GetMonthlyIncomeFromOrgs_NegativeOnly(FactionResource resource)
		{
			float num = 0f;
			foreach (TIOrgState tiorgState in this.activeOrgs)
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

		// Token: 0x06002CCB RID: 11467 RVA: 0x000F5D44 File Offset: 0x000F3F44
		public float GetMonthlyIncome(FactionResource resourceType)
		{
			float num = 0f;
			if (this.detained)
			{
				return 0f;
			}
			if (this.isAlien && (resourceType == FactionResource.Research || resourceType == FactionResource.Projects))
			{
				return 0f;
			}
			num += this.GetMonthlyIncome_PositiveOnly(resourceType);
			return num + this.GetMonthlyIncome_NegativeOnly(resourceType, false);
		}

		// Token: 0x06002CCC RID: 11468 RVA: 0x000F5D91 File Offset: 0x000F3F91
		public float GetMonthlyIncome_PositiveOnly(FactionResource resourceType)
		{
			float num = 0f + this.GetMonthlyIncomeFromTraits_PositiveOnly(resourceType) + this.GetMonthlyIncomeFromOrgs_PositiveOnly(resourceType);
			return num * ((num > 0f) ? this.GetResourceMultiplierFromAttributes(resourceType) : 1f);
		}

		// Token: 0x06002CCD RID: 11469 RVA: 0x000F5DBF File Offset: 0x000F3FBF
		public float GetMonthlyIncome_NegativeOnly(FactionResource resourceType, bool returnPositiveNumber)
		{
			return (0f + this.GetMonthlyIncomeFromTraits_NegativeOnly(resourceType) + this.GetMonthlyIncomeFromOrgs_NegativeOnly(resourceType)) * (float)(returnPositiveNumber ? (-1) : 1);
		}

		// Token: 0x06002CCE RID: 11470 RVA: 0x000F5DE0 File Offset: 0x000F3FE0
		public float GetYearlyIncome(FactionResource resourceType)
		{
			float num = 0f;
			switch (resourceType)
			{
			case FactionResource.Money:
			case FactionResource.Influence:
			case FactionResource.Operations:
			case FactionResource.Research:
			case FactionResource.Boost:
				num += 12f * this.GetMonthlyIncome(resourceType);
				break;
			case FactionResource.Projects:
			case FactionResource.MissionControl:
				num = this.GetMonthlyIncome(resourceType);
				break;
			}
			return num;
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06002CCF RID: 11471 RVA: 0x000F5E34 File Offset: 0x000F4034
		public int controlPointCapacity
		{
			get
			{
				return this.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) + this.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false) + this.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false);
			}
		}

		// Token: 0x06002CD0 RID: 11472 RVA: 0x000F5E6A File Offset: 0x000F406A
		public float TotalTechBonus(TechCategory category, bool activeOrgsOnly)
		{
			return this.TechCategoryBonusFromTraits(category) + this.TechCategoryBonusFromOrgs(category, activeOrgsOnly);
		}

		// Token: 0x06002CD1 RID: 11473 RVA: 0x000F5E7C File Offset: 0x000F407C
		public float ProjectUnlockBonus()
		{
			return this.traits.Where<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.ProjectUnlockChance).Sum<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRuleValue);
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06002CD2 RID: 11474 RVA: 0x000F5ED7 File Offset: 0x000F40D7
		public List<TIOrgState> activeOrgs
		{
			get
			{
				return this.orgs.Where<TIOrgState>((TIOrgState x) => x.applyingBonuses).ToList<TIOrgState>();
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06002CD3 RID: 11475 RVA: 0x000F5F08 File Offset: 0x000F4108
		public int orgsWeight
		{
			get
			{
				return this.orgs.Sum<TIOrgState>((TIOrgState x) => x.tier);
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06002CD4 RID: 11476 RVA: 0x000F5F34 File Offset: 0x000F4134
		public int prospectiveOrgsWeight
		{
			get
			{
				return this.prospectiveOrgs.Sum<TIOrgState>((TIOrgState x) => x.tier);
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06002CD5 RID: 11477 RVA: 0x000F5F60 File Offset: 0x000F4160
		public int availableAdministration
		{
			get
			{
				return Mathf.Min(this.GetAttribute(CouncilorAttribute.Administration, true, true, true, true, false, false) - this.orgsWeight, this.maxCouncilorAttribute);
			}
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x000F5F84 File Offset: 0x000F4184
		public bool HasOrg(TIOrgTemplate orgTemplate)
		{
			using (List<TIOrgState>.Enumerator enumerator = this.orgs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.template == orgTemplate)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06002CD7 RID: 11479 RVA: 0x000F5FE0 File Offset: 0x000F41E0
		public bool SufficientCapacityForOrg(TIOrgState org)
		{
			return this.orgs.Count < TemplateManager.global.councilorMaxOrgs && this.availableAdministration >= org.tier - org.administration && this.orgsWeight + org.tier <= this.GetClampedMaxStatValue(CouncilorAttribute.Administration);
		}

		// Token: 0x06002CD8 RID: 11480 RVA: 0x000F6034 File Offset: 0x000F4234
		public bool AreProspectiveOrgsValid(out string reason)
		{
			StringBuilder stringBuilder = new StringBuilder();
			reason = "";
			bool flag = true;
			if (this.prospectiveOrgs.Count > TemplateManager.global.councilorMaxOrgs)
			{
				stringBuilder.Append(TIUtilities.RedLine(Loc.T("UI.Council.OrgManagement.Feedback.CouncilorOverageOrgs")));
				flag = false;
			}
			int[] array = new int[1];
			array[0] = this.GetAttribute(CouncilorAttribute.Administration, true, true, true, true, true, false) - this.prospectiveOrgs.Sum<TIOrgState>((TIOrgState x) => x.tier);
			if (Mathf.Min(array) < 0)
			{
				if (!flag)
				{
					stringBuilder.Append(TIUtilities.RedLine(Loc.T("UI.Global.SerialDividerWithSpace")));
				}
				stringBuilder.Append(TIUtilities.RedLine(Loc.T("UI.Council.OrgManagement.Feedback.CouncilorOverageAdmin")));
				flag = false;
			}
			reason = stringBuilder.ToString();
			return flag;
		}

		// Token: 0x06002CD9 RID: 11481 RVA: 0x000F6102 File Offset: 0x000F4302
		public int SpareCapacityForOrgs()
		{
			return Mathf.Min(TemplateManager.global.councilorMaxOrgs - this.orgs.Count, this.availableAdministration);
		}

		// Token: 0x06002CDA RID: 11482 RVA: 0x000F6125 File Offset: 0x000F4325
		public bool CanAddExternalOrgValidatedForFaction(TIOrgState org)
		{
			return org.CouncilorCanAcquire(this) && this.SufficientCapacityForOrg(org);
		}

		// Token: 0x06002CDB RID: 11483 RVA: 0x000F613C File Offset: 0x000F433C
		public void AddOrg(TIOrgState org)
		{
			this.orgs.Add(org);
			this.orgs = (from x in this.orgs
				orderby x.tier descending, x.displayName descending
				select x).ToList<TIOrgState>();
			org.SetFactionOrbit(this.faction);
			org.AssignCouncilor(this);
			org.SetOrgActivationStatus(TIMissionPhaseState.InMissionPhase() && !this.detained);
			this.SetAttributesDirty();
			TIProjectTemplate projectGranted = org.projectGranted;
			if (projectGranted != null && !this.faction.completedProjects.Contains(projectGranted) && this.faction.AddAvailableProject(org.projectGranted, null))
			{
				TINotificationQueueState.LogProjectTriggered(this.faction, org.projectGranted, false);
			}
			if (this.faction.isActivePlayer && this.GetAttribute(CouncilorAttribute.Administration, true, true, true, true, false, false) >= 25)
			{
				this.faction.UnlockAchievement("stackedCouncilor");
			}
		}

		// Token: 0x06002CDC RID: 11484 RVA: 0x000F6254 File Offset: 0x000F4454
		public bool OrgProvidingActiveMission(TIOrgState org)
		{
			if (!this.HasMission)
			{
				return false;
			}
			TIMissionTemplate mission = this.activeMission.missionTemplate;
			return org.applyingBonuses && org.template.missionsGrantedNames.Contains(mission.dataName) && this.GetPossibleMissionList(false, false, false, org, false).None<TIMissionTemplate>((TIMissionTemplate x) => x.dataName == mission.dataName);
		}

		// Token: 0x06002CDD RID: 11485 RVA: 0x000F62C8 File Offset: 0x000F44C8
		public List<TIOrgState> RemoveableOrgs()
		{
			int num = this.GetAttribute(CouncilorAttribute.Administration, true, true, false, true, false, false) - this.orgsWeight;
			List<TIOrgState> list = new List<TIOrgState>();
			foreach (TIOrgState tiorgState in this.orgs)
			{
				if ((tiorgState.administration <= tiorgState.tier || num >= tiorgState.administration - tiorgState.tier) && !this.OrgProvidingActiveMission(tiorgState))
				{
					list.Add(tiorgState);
				}
			}
			return list;
		}

		// Token: 0x06002CDE RID: 11486 RVA: 0x000F6360 File Offset: 0x000F4560
		public bool CanRemoveOrg(TIOrgState org)
		{
			return this.CanRemoveOrg_Admin(org) && !this.OrgProvidingActiveMission(org);
		}

		// Token: 0x06002CDF RID: 11487 RVA: 0x000F6377 File Offset: 0x000F4577
		public bool CanRemoveOrg_Admin(TIOrgState org)
		{
			return org.administration <= org.tier || this.GetAttribute(CouncilorAttribute.Administration, true, true, false, true, false, false) - this.orgsWeight >= org.administration - org.tier;
		}

		// Token: 0x06002CE0 RID: 11488 RVA: 0x000F63AF File Offset: 0x000F45AF
		public void RemoveOrg(TIOrgState org)
		{
			this.orgs.Remove(org);
			org.SetOrgActivationStatus(false);
			org.UnassignCouncilor(this);
			this.SetAttributesDirty();
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06002CE1 RID: 11489 RVA: 0x000F63D4 File Offset: 0x000F45D4
		public TIResourcesCost AllOrgsSaleValue
		{
			get
			{
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				foreach (TIOrgState tiorgState in this.orgs)
				{
					tiresourcesCost.AddCost(FactionResource.Money, tiorgState.costMoney * TemplateManager.global.sellOrgDiscount, true);
				}
				return tiresourcesCost;
			}
		}

		// Token: 0x06002CE2 RID: 11490 RVA: 0x000F6440 File Offset: 0x000F4640
		public List<TIOrgState> GetStealableOrgs(TICouncilorState targetingCouncilor)
		{
			List<TIOrgState> list = new List<TIOrgState>();
			if (targetingCouncilor.faction.HasIntelOnCouncilorDetails(this))
			{
				list = targetingCouncilor.faction.GetViewofCouncilor(this).orgs.Where<TIOrgState>((TIOrgState x) => x.IsEligibleForFaction(targetingCouncilor.faction)).ToList<TIOrgState>();
				list.AddRange(this.faction.unassignedOrgs.Where<TIOrgState>((TIOrgState x) => x.IsEligibleForFaction(targetingCouncilor.faction)));
			}
			return list;
		}

		// Token: 0x06002CE3 RID: 11491 RVA: 0x000F64C6 File Offset: 0x000F46C6
		public List<TIOrgState> GetLoseableOrgs()
		{
			return this.orgs.Where<TIOrgState>((TIOrgState x) => x.template.allowedOnMarket).ToList<TIOrgState>();
		}

		// Token: 0x06002CE4 RID: 11492 RVA: 0x000F64F8 File Offset: 0x000F46F8
		public bool StealOrg(TIOrgState org, out List<TIOrgState> discardedOrgs)
		{
			TIFactionState factionOrbit = org.factionOrbit;
			factionOrbit.LoseOrg(org);
			discardedOrgs = factionOrbit.ValidateAllOrgs(true);
			if (this.CanAddExternalOrgValidatedForFaction(org))
			{
				this.faction.AssignOrgToCouncilor(org, this);
				return true;
			}
			this.faction.AddOrgToFactionPool(org, null, false);
			return false;
		}

		// Token: 0x06002CE5 RID: 11493 RVA: 0x000F6544 File Offset: 0x000F4744
		public void ActivateAllOrgs()
		{
			if (this.active && !this.detained)
			{
				foreach (TIOrgState tiorgState in this.orgs)
				{
					tiorgState.SetOrgActivationStatus(true);
				}
			}
		}

		// Token: 0x06002CE6 RID: 11494 RVA: 0x000F65A8 File Offset: 0x000F47A8
		public void DeactivateAllOrgs()
		{
			foreach (TIOrgState tiorgState in this.orgs)
			{
				tiorgState.SetOrgActivationStatus(false);
			}
		}

		// Token: 0x06002CE7 RID: 11495 RVA: 0x000F65FC File Offset: 0x000F47FC
		public float TechCategoryBonusFromOrgs(TechCategory techCategory, bool activeOrgsOnly)
		{
			float num = 0f;
			foreach (TIOrgState tiorgState in (activeOrgsOnly ? this.activeOrgs : this.orgs))
			{
				for (int i = 0; i < tiorgState.techBonuses.Length; i++)
				{
					if (tiorgState.techBonuses[i].category == techCategory)
					{
						num += tiorgState.techBonuses[i].bonus;
					}
				}
			}
			return num;
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x000F6698 File Offset: 0x000F4898
		public List<TIMissionTemplate> GetPossibleMissionList(bool filterForCouncilorConditions = false, bool sort = false, bool checkDetained = true, TIOrgState skipOrg = null, bool useProspectiveOrgs = false)
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>();
			if (checkDetained && this.detained)
			{
				return list;
			}
			if (this.isHuman)
			{
				list.AddRange(TIMissionPhaseState.baseHumanMissions);
			}
			list.AddRange(this.typeTemplate.missions);
			foreach (TIMissionTemplate timissionTemplate in this.learnedMissions)
			{
				if (timissionTemplate != null)
				{
					list.Add(timissionTemplate);
				}
			}
			foreach (TIOrgState tiorgState in this.activeOrgs)
			{
				if (!(skipOrg != null) || !(tiorgState == skipOrg))
				{
					list.AddRange(tiorgState.missionsGranted);
				}
			}
			if (useProspectiveOrgs)
			{
				foreach (TIOrgState tiorgState2 in this.prospectiveOrgs)
				{
					if (!(skipOrg != null) || !(tiorgState2 == skipOrg))
					{
						list.AddRange(tiorgState2.missionsGranted);
					}
				}
			}
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				list.AddRange(titraitTemplate.MissionsGranted);
			}
			list = (from x in list.Distinct<TIMissionTemplate>()
				where x != null
				select x).ToList<TIMissionTemplate>();
			foreach (TITraitTemplate titraitTemplate2 in this.traits)
			{
				foreach (TIMissionTemplate timissionTemplate2 in titraitTemplate2.RestrictedMissions)
				{
					list.Remove(timissionTemplate2);
				}
			}
			if (filterForCouncilorConditions)
			{
				MissionContext missionContext;
				if (this.OnEarth)
				{
					missionContext = MissionContext.SpaceOnly;
				}
				else
				{
					missionContext = MissionContext.EarthOnly;
				}
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (list[i].missionContext == missionContext)
					{
						list.Remove(list[i]);
					}
				}
			}
			if (sort)
			{
				list = list.OrderBy<TIMissionTemplate, int>((TIMissionTemplate o) => o.sortOrder).ToList<TIMissionTemplate>();
			}
			return list;
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x000F695C File Offset: 0x000F4B5C
		public List<TIMissionTemplate> RestrictedMissions()
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>();
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				foreach (TIMissionTemplate timissionTemplate in titraitTemplate.RestrictedMissions)
				{
					list.Remove(timissionTemplate);
				}
			}
			return list;
		}

		// Token: 0x06002CEA RID: 11498 RVA: 0x000F69F0 File Offset: 0x000F4BF0
		public List<MissionOption> MissionOptionsForTarget(TIGameState target)
		{
			List<MissionOption> list = new List<MissionOption>();
			foreach (TIMissionTemplate timissionTemplate in this.GetPossibleMissionList(true, false, true, null, false))
			{
				if (timissionTemplate.GetValidTargets(this).ToList<TIGameState>().Contains(target))
				{
					float successChance = timissionTemplate.resolutionMethod.GetSuccessChance(timissionTemplate, this, target, 0f, false);
					string successChanceString = timissionTemplate.resolutionMethod.GetSuccessChanceString(timissionTemplate, this, target, 0f, false, 2);
					list.Add(new MissionOption
					{
						councilor = this,
						mission = timissionTemplate,
						target = target,
						baseChance = successChance,
						baseChanceString = successChanceString
					});
				}
			}
			return list;
		}

		// Token: 0x06002CEB RID: 11499 RVA: 0x000F6AC8 File Offset: 0x000F4CC8
		public TIOrgState OrgGrantingMission(TIMissionTemplate mission, bool exclusiveToOrg)
		{
			if (!exclusiveToOrg)
			{
				return this.activeOrgs.FirstOrDefault<TIOrgState>((TIOrgState x) => x.missionsGranted.Contains(mission));
			}
			IEnumerable<TIMissionTemplate> enumerable = this.activeOrgs.SelectMany<TIOrgState, TIMissionTemplate>((TIOrgState x) => x.missionsGranted);
			List<TIMissionTemplate> list = new List<TIMissionTemplate>(this.typeTemplate.missions);
			list.AddRange(from x in TemplateManager.IterateByClass<TIMissionTemplate>(true)
				where x.baseMission
				select x);
			list.AddRange(this.traits.SelectMany<TITraitTemplate, TIMissionTemplate>((TITraitTemplate x) => x.MissionsGranted));
			list.AddRange(this.learnedMissions);
			if (enumerable.Except<TIMissionTemplate>(list).Except<TIMissionTemplate>(this.traits.SelectMany<TITraitTemplate, TIMissionTemplate>((TITraitTemplate x) => x.RestrictedMissions)).Contains(mission))
			{
				return this.activeOrgs.FirstOrDefault<TIOrgState>((TIOrgState x) => x.missionsGranted.Contains(mission));
			}
			return null;
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x000F6C04 File Offset: 0x000F4E04
		public void SetActiveMission(TIMissionState mission)
		{
			this.activeMission = mission;
			GameControl.eventManager.TriggerEvent(new CouncilorMissionUpdated(this, mission), null, new object[] { this, this.faction, this.location, this.ref_nation }.Where<object>((object x) => x != null).ToArray<object>());
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x000F6C78 File Offset: 0x000F4E78
		public void SetRepeatOrder(bool repeatOrder)
		{
			this.repeatOrder = repeatOrder;
			if (this.permanentDefenseMode)
			{
				this.SetPermanentDefenseMode(false);
			}
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x000F6C90 File Offset: 0x000F4E90
		public void SetPermanentAssignment(bool setting)
		{
			this.permanentAssignment = setting;
			if (this.permanentDefenseMode)
			{
				this.SetPermanentDefenseMode(false);
			}
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x000F6CA8 File Offset: 0x000F4EA8
		public void ClearActiveMission()
		{
			if (this.HasMission)
			{
				this.activeMission = null;
				GameControl.eventManager.TriggerEvent(new CouncilorMissionUpdated(this, null), null, new object[] { this, this.faction, this.location, this.ref_nation }.Where<object>((object x) => x != null).ToArray<object>());
			}
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000F6D24 File Offset: 0x000F4F24
		public void SetPriorMission(TIMissionTemplate mission, TIGameState target)
		{
			this.priorMissionTemplateName = mission.dataName;
			this.priorMissionTarget = (TIGameState.Valid(target) ? target : null);
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000F6D44 File Offset: 0x000F4F44
		public void SetCompletedMission(TIMissionState missionState)
		{
			this.completedMission = missionState;
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x000F6D4D File Offset: 0x000F4F4D
		public void ClearCompletedMission()
		{
			if (TIGameState.Valid(this.completedMission))
			{
				this.completedMission.ArchiveState(true);
				GameStateManager.RemoveGameState<TIMissionState>(this.completedMission.ID, false);
			}
			this.repeatOrder = this.permanentAssignment;
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x000F6D88 File Offset: 0x000F4F88
		public bool CanRepeatMission(TIMissionState mission)
		{
			string text;
			return mission != null && mission.councilor.active && mission.councilor.faction != null && mission.missionTemplate.utilityScore < 999f && (this.GetPossibleMissionList(true, false, true, null, false).Contains(mission.missionTemplate) && mission.missionTemplate.GetValidTargets(this).Contains(mission.target) && this.ValidDestination(TIUtilities.ObjectToExactLocation(mission.target), out text)) && (!mission.missionTemplate.hasCost || mission.resources == 0f || mission.resources <= this.faction.GetCurrentResourceAmount(mission.missionTemplate.cost.resourceType));
		}

		// Token: 0x06002CF4 RID: 11508 RVA: 0x000F6E68 File Offset: 0x000F5068
		public void SetPermanentDefenseMode(bool setting)
		{
			bool permanentDefenseMode = this.permanentDefenseMode;
			if (setting)
			{
				this.permanentAssignment = false;
				this.repeatOrder = false;
			}
			this.permanentDefenseMode = setting;
			if (this.permanentDefenseMode != permanentDefenseMode)
			{
				GameControl.eventManager.TriggerEvent(new CouncilorChangesAutoDefenseMode(this, setting), null, new object[] { this });
			}
			if (this.permanentDefenseMode && TIMissionPhaseState.InMissionPhase())
			{
				this.SelectPermanentDefenseModeMission();
			}
		}

		// Token: 0x06002CF5 RID: 11509 RVA: 0x000F6ED0 File Offset: 0x000F50D0
		public void ToggleDefenseModeMission(TIMissionTemplate mission, bool shouldBeActive)
		{
			bool flag = this.missionsExcludedFromDefenseMode.Contains(mission.dataName);
			if (shouldBeActive && flag)
			{
				this.missionsExcludedFromDefenseMode.Remove(mission.dataName);
				return;
			}
			if (!flag)
			{
				this.missionsExcludedFromDefenseMode.Add(mission.dataName);
			}
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x000F6F1C File Offset: 0x000F511C
		public bool SelectPermanentDefenseModeMission()
		{
			Dictionary<AIForcedMissionEntry, float> possibleMissionDictionary = new Dictionary<AIForcedMissionEntry, float>();
			List<TIMissionTemplate> list = (from x in this.GetPossibleMissionList(true, false, true, null, false)
				where x.allowedForAutoDefense && !this.missionsExcludedFromDefenseMode.Contains(x.dataName)
				select x).ToList<TIMissionTemplate>();
			float num = TITimeState.CampaignDuration_years_Exact();
			TIRegionState ref_region = this.faction.MostRecentAlienSite(true).ref_region;
			float num2 = this.faction.MostRecentAlienSiteAge_days(true);
			AICouncilorMissionPlanner.singleton.SetRawNationPayoffsByFaction(this.faction, this.faction.isActivePlayer && AICouncilorMissionPlanner.cachedPayoffFrame != TIFrameCounter.FrameCount);
			AICouncilorMissionPlanner.singleton.SetPayoffValues(this.faction, this.faction.isActivePlayer && AICouncilorMissionPlanner.cachedPayoffFrame != TIFrameCounter.FrameCount);
			bool flag = this.faction.controlPoints.Count == 0;
			if (list.Count > 0)
			{
				foreach (TIMissionTemplate timissionTemplate in list)
				{
					if (timissionTemplate.CanAfford(this.faction, null))
					{
						foreach (TIGameState tigameState in timissionTemplate.GetValidTargets(this))
						{
							if (tigameState.isNationState || tigameState.isRegionState || tigameState.isControlPointState)
							{
								if (flag)
								{
									if (tigameState.ref_nation.controlPoints.Any<TIControlPoint>((TIControlPoint x) => x.faction != null))
									{
										continue;
									}
								}
								else if (tigameState.ref_nation.CountFactionControlPoints(this.faction, true, false, true) == 0)
								{
									continue;
								}
							}
							else if (tigameState.isCouncilorState)
							{
								if (tigameState.ref_councilor.faction != this.faction)
								{
									continue;
								}
							}
							else if (tigameState.isRegionAlienEntity && (!tigameState.isRegionXenoformingState || tigameState.ref_nation.CountFactionControlPoints(this.faction, true, false, true) == 0))
							{
								continue;
							}
							if ((!this.OnAShip || (!tigameState.isHabState && !tigameState.isHabModuleState && !tigameState.isHabSiteState)) && (!this.InAHab || (!tigameState.isSpaceShipState && !tigameState.isHabState)))
							{
								int num3 = 0;
								bool flag2 = false;
								foreach (TICouncilorState ticouncilorState in this.faction.councilors)
								{
									if (ticouncilorState != this && ticouncilorState.activeMission != null)
									{
										if (AIEvaluators.AI_ShouldAvoidDoublingUpMissionTarget(ticouncilorState, ticouncilorState.activeMission.missionTemplate, ticouncilorState.activeMission.target, ticouncilorState.activeMission.GetSuccessChance(), this, timissionTemplate, tigameState))
										{
											flag2 = true;
											break;
										}
										if (ticouncilorState.activeMission.missionTemplate == timissionTemplate && ticouncilorState.activeMission.target == tigameState)
										{
											num3++;
										}
									}
								}
								if (!flag2)
								{
									AIForcedMissionEntry aiforcedMissionEntry = new AIForcedMissionEntry(this, tigameState, timissionTemplate);
									float num4 = AICouncilorMissionPlanner.singleton.GetPayoffForMissionTarget(this.faction, timissionTemplate, this, tigameState, new List<TIMissionTemplate>(), new List<TIMissionTemplate>(), null, new List<CampaignMilestone>(), num, false, 0f, new List<TIFactionState>(), ref_region, num2, false);
									if (num3 > 0)
									{
										num4 /= (float)(num3 + 8);
									}
									if (timissionTemplate.ContestedMission)
									{
										float successChance = timissionTemplate.resolutionMethod.GetSuccessChance(timissionTemplate, this, tigameState, 0f, false);
										num4 *= successChance * successChance;
									}
									if (num4 > 0f)
									{
										possibleMissionDictionary.Add(aiforcedMissionEntry, num4);
									}
								}
							}
						}
					}
				}
				if (possibleMissionDictionary.Count > 0)
				{
					float best = possibleMissionDictionary.Keys.Max<AIForcedMissionEntry>((AIForcedMissionEntry x) => possibleMissionDictionary[x]);
					AIForcedMissionEntry key = possibleMissionDictionary.Where<KeyValuePair<AIForcedMissionEntry, float>>((KeyValuePair<AIForcedMissionEntry, float> x) => x.Value == best).SelectRandomItem<KeyValuePair<AIForcedMissionEntry, float>>().Key;
					float num5 = 0f;
					if (key.mission.ContestedMission && key.mission.cost.resourceType != FactionResource.None)
					{
						int num6 = this.CurrentMaxSliderSteps(key.mission, 1f);
						float num7 = 0f;
						float num8 = Mathf.Min(this.faction.GetCurrentResourceAmount(key.mission.primaryResource), this.faction.GetDailyIncome(key.mission.primaryResource, false, false) * 7f);
						for (int i = 0; i <= num6; i++)
						{
							float cost = key.mission.cost.GetCost((float)i, this, null);
							if (cost <= num8)
							{
								float num9 = cost * AIEvaluators.GetAIRelativeValuation(key.mission.cost.resourceType) * 5f;
								float successChance2 = key.mission.resolutionMethod.GetSuccessChance(key.mission, key.councilor, key.target, cost, false);
								float num10 = possibleMissionDictionary[key] * successChance2 - num9;
								if (successChance2 < 0.33333334f)
								{
									num10 = float.Epsilon;
								}
								if (num10 > num7)
								{
									num5 = (float)((int)cost);
									num7 = num10;
								}
							}
						}
					}
					else if (key.mission.hasCost)
					{
						num5 = key.mission.cost.GetCost(0f, key.councilor, key.target);
					}
					this.faction.playerControl.StartAction(new AssignCouncilorToMission(this, key.mission, key.target, num5, false));
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002CF7 RID: 11511 RVA: 0x000F7548 File Offset: 0x000F5748
		public int MaxSliderSteps()
		{
			return TemplateManager.global.initialMaxAllowedResourceSteps + (int)TIEffectsState.SumEffectsModifiers(Context.MaxMissionSliderSteps, this.faction, (float)TemplateManager.global.initialMaxAllowedResourceSteps, null);
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x000F7570 File Offset: 0x000F5770
		public int CurrentMaxSliderSteps(TIMissionTemplate mission, float AIAvailableFraction = 1f)
		{
			if (mission.cost != null && mission.cost.GetType() == typeof(TIMissionCost_Bonus))
			{
				float num = this.faction.GetCurrentResourceAmount(mission.cost.resourceType) * AIAvailableFraction;
				for (int i = this.MaxSliderSteps(); i >= 0; i--)
				{
					if (mission.cost.GetCost((float)i, this, null) <= num)
					{
						return i;
					}
				}
			}
			return 0;
		}

		// Token: 0x06002CF9 RID: 11513 RVA: 0x000F75E0 File Offset: 0x000F57E0
		public bool ValidDestination(TIGameState candidateDestination, out string reason)
		{
			if (candidateDestination == null || candidateDestination.deleted)
			{
				reason = "UI.Councilor.MoveFail_NoTarget";
				return false;
			}
			if (candidateDestination.isRegionState || candidateDestination.isHabState || candidateDestination.isSpaceShipState)
			{
				TIRegionState tiregionState = (candidateDestination.isRegionState ? candidateDestination.ref_region : null);
				TIHabState tihabState = (candidateDestination.isHabState ? candidateDestination.ref_hab : null);
				TISpaceShipState tispaceShipState = (candidateDestination.isSpaceShipState ? candidateDestination.ref_ship : null);
				bool flag = false;
				if (this.isAlien)
				{
					if (candidateDestination.isRegionState && !tiregionState.AllowedDestinationForAlienCouncilor(this))
					{
						reason = "UI.Councilor.MoveFail_Alien";
						return false;
					}
					if (this.OnEarth && candidateDestination.inSpace)
					{
						reason = "UI.Councilor.MoveFail_Alien";
						return false;
					}
					if (candidateDestination.isHabState && !candidateDestination.ref_hab.IsAlien())
					{
						reason = "UI.Councilor.MoveFail_Alien";
						return false;
					}
					if (candidateDestination.isSpaceShipState && !candidateDestination.ref_ship.isAlien)
					{
						reason = "UI.Councilor.MoveFail_Alien";
						return false;
					}
				}
				if (candidateDestination.isHabState)
				{
					if (!this.isAlien && candidateDestination.ref_hab.IsAlien())
					{
						if (candidateDestination.ref_hab != candidateDestination.ref_faction.primaryHab)
						{
							reason = "UI.Councilor.MoveFail_AlienNotPrimaryHab";
							return false;
						}
						if (this.faction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked).None<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.AlienHQ))
						{
							reason = "UI.Councilor.MoveFail_AlienPrimaryHabObjectiveLocked";
							return false;
						}
						if (candidateDestination.ref_hab.ActiveCombatModules().Count > 0)
						{
							reason = "UI.Councilor.MoveFail_AlienPrimaryHabObjectiveHasCombatModules";
							return false;
						}
						reason = "Valid";
						return true;
					}
				}
				else if (candidateDestination.isSpaceShipState && !this.isAlien && candidateDestination.ref_ship.isAlien)
				{
					reason = "UI.Councilor.MoveFail_AlienShip";
					return false;
				}
				foreach (TITraitTemplate titraitTemplate in this.traits)
				{
					if (titraitTemplate != null)
					{
						if (candidateDestination.isRegionState)
						{
							switch (titraitTemplate.restrictedLocations)
							{
							case RestrictedLocations.HomeNation:
								if (tiregionState.nation == this.homeNation)
								{
									reason = new StringBuilder("UI.Councilor.MoveFail_").Append(titraitTemplate.restrictedLocations.ToString()).ToString();
									return false;
								}
								break;
							case RestrictedLocations.HomeNationAndAllies:
								if (tiregionState.nation == this.homeNation || tiregionState.nation.IsAlliedWith(tiregionState.nation, false))
								{
									reason = new StringBuilder("UI.Councilor.MoveFail_").Append(titraitTemplate.restrictedLocations.ToString()).ToString();
									return false;
								}
								break;
							case RestrictedLocations.HomeNationRivals:
								if (this.homeRegion.nation.IsRivalWith(tiregionState.nation) || this.homeRegion.nation.IsAtWarWith(tiregionState.nation))
								{
									reason = new StringBuilder("UI.Councilor.MoveFail_").Append(titraitTemplate.restrictedLocations.ToString()).ToString();
									return false;
								}
								break;
							case RestrictedLocations.HomeNationWarOpponents:
								if (this.homeRegion.nation.IsAtWarWith(tiregionState.nation))
								{
									reason = new StringBuilder("UI.Councilor.MoveFail_").Append(titraitTemplate.restrictedLocations.ToString()).ToString();
									return false;
								}
								break;
							case RestrictedLocations.HighUnrestNations:
								if (tiregionState.nation.unrest >= TemplateManager.global.HighUnrestDefinition)
								{
									reason = new StringBuilder("UI.Councilor.MoveFail_").Append(titraitTemplate.restrictedLocations.ToString()).ToString();
									return false;
								}
								break;
							}
						}
						else if (titraitTemplate.restrictedLocations == RestrictedLocations.Space)
						{
							reason = new StringBuilder("UI.Councilor.MoveFail_").Append(titraitTemplate.restrictedLocations.ToString()).ToString();
							return false;
						}
						if (titraitTemplate.specialTraitRule == SpecialTraitRule.Undercover)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					if (candidateDestination.isHabState && tihabState.tier == 1 && tihabState.faction != this.faction)
					{
						reason = new StringBuilder("UI.Councilor.MoveFail_Undercover").ToString();
						return false;
					}
					if (candidateDestination.isSpaceShipState && tispaceShipState.fleet.faction != this.faction)
					{
						reason = new StringBuilder("UI.Councilor.MoveFail_Undercover").ToString();
						return false;
					}
				}
				reason = "Valid";
				return true;
			}
			reason = "Can't figure out where councilor " + this.displayName + " is considering as destination to go, passed GS: " + candidateDestination.ToString();
			Log.Error(reason, Array.Empty<object>());
			return false;
		}

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06002CFA RID: 11514 RVA: 0x000F7AB0 File Offset: 0x000F5CB0
		public List<TIFactionState> enemyFactionsTargetingMe
		{
			get
			{
				List<TIFactionState> list = new List<TIFactionState>();
				foreach (TICouncilorState ticouncilorState in GameStateManager.IterateByClass<TICouncilorState>(false))
				{
					if (ticouncilorState.active && ticouncilorState.faction != this.faction && !list.Contains(ticouncilorState.faction) && ticouncilorState.HasMission && ticouncilorState.activeMission.target.ref_councilor == this)
					{
						list.Add(ticouncilorState.faction);
					}
				}
				return list;
			}
		}

		// Token: 0x06002CFB RID: 11515 RVA: 0x000F7B54 File Offset: 0x000F5D54
		public void AddToParanoia(TIFactionState otherFaction)
		{
			if (otherFaction == null)
			{
				List<TIFactionState> factionsAtWarWithMe = this.faction.factionsAtWarWithMe;
				if (factionsAtWarWithMe.Count > 0)
				{
					using (List<TIFactionState>.Enumerator enumerator = factionsAtWarWithMe.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIFactionState tifactionState = enumerator.Current;
							if (!tifactionState.IsAlienFaction || this.faction.CanDetectAlien)
							{
								this.knowsIveBeenSeenBy.AddUnique(tifactionState);
							}
						}
						return;
					}
				}
				this.knowsIveBeenSeenBy.AddRange((from x in GameStateManager.AllFactions()
					where !this.faction.permanentAlly(x) && !this.knowsIveBeenSeenBy.Contains(x)
					select x).ToList<TIFactionState>());
				return;
			}
			if (otherFaction.IsAlienFaction && !this.faction.CanDetectAlien)
			{
				return;
			}
			this.knowsIveBeenSeenBy.AddUnique(otherFaction);
		}

		// Token: 0x06002CFC RID: 11516 RVA: 0x000F7C28 File Offset: 0x000F5E28
		public void SetLearnedMissions()
		{
			if (this.learnedMissions == null)
			{
				this.learnedMissions = new List<TIMissionTemplate>();
			}
			foreach (string text in this.learnedMissionsTemplateNames)
			{
				TIMissionTemplate timissionTemplate = TemplateManager.Find<TIMissionTemplate>(text, false);
				if (timissionTemplate != null)
				{
					this.learnedMissions.AddUnique(timissionTemplate);
				}
				else
				{
					Log.Error("Bad " + text + "passed to SetLearnedMissions", Array.Empty<object>());
				}
			}
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x000F7CC0 File Offset: 0x000F5EC0
		public bool LearnMission(TIMissionTemplate template)
		{
			if (!this.learnedMissionsTemplateNames.Contains(template.dataName))
			{
				this.learnedMissionsTemplateNames.Add(template.dataName);
				this.learnedMissions.Add(template);
				return true;
			}
			return false;
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x000F7CF5 File Offset: 0x000F5EF5
		public void RecordLocation()
		{
			this.preMissionPhaseLocation = this.location;
		}

		// Token: 0x06002CFF RID: 11519 RVA: 0x000F7D03 File Offset: 0x000F5F03
		public void EnterTransit()
		{
			this.inTransit = true;
		}

		// Token: 0x06002D00 RID: 11520 RVA: 0x000F7D0C File Offset: 0x000F5F0C
		public void ExitTransit()
		{
			this.inTransit = false;
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x000F7D15 File Offset: 0x000F5F15
		public bool InTransit()
		{
			return this.inTransit;
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x000F7D20 File Offset: 0x000F5F20
		public bool CheckAndChaseMissionTarget()
		{
			if (this.active && this.HasMission)
			{
				TIMissionState activeMission = this.activeMission;
				if (activeMission != null)
				{
					TIGameState target = activeMission.target;
					if (target != null)
					{
						if (target.isCouncilorState)
						{
							TICouncilorState ref_councilor = target.ref_councilor;
							if (ref_councilor != null && this.location != ref_councilor.location)
							{
								string text;
								if (!this.ValidDestination(ref_councilor.location, out text))
								{
									activeMission.ResolveMission(TIMissionState.AbortReason.UseDetail, text);
									return false;
								}
								this.ChangeLocation(ref_councilor.location);
								GameControl.eventManager.TriggerEvent(new CouncilorMissionUpdated(this, this.activeMission), null, new object[] { this, this.faction, this.location, this.ref_nation });
							}
						}
						else if (target.isNationState || target.isControlPointState)
						{
							TINationState ref_nation = target.ref_nation;
							if (this.location != ref_nation.capital)
							{
								string text2;
								if (!this.ValidDestination(ref_nation.capital, out text2))
								{
									activeMission.ResolveMission(TIMissionState.AbortReason.UseDetail, text2);
									return false;
								}
								this.ChangeLocation(ref_nation.capital);
								GameControl.eventManager.TriggerEvent(new CouncilorMissionUpdated(this, this.activeMission), null, new object[] { this, this.faction, this.location, this.ref_nation });
							}
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x000F7EA0 File Offset: 0x000F60A0
		public void RemoveFromCurrentLocation()
		{
			if (this.OnEarth)
			{
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new CouncilorDepartsRegion(this, this.ref_region);
				string text = null;
				object[] array = new TIGameState[] { this, this.ref_region, this.ref_nation };
				eventManager.TriggerEvent(gameEvent, text, array);
				return;
			}
			if (this.InAHab)
			{
				if (this.location.ref_hab == this.location)
				{
					this.location.ref_hab.DepartCouncilor(this);
				}
				GameControl.eventManager.TriggerEvent(new CouncilorDepartsHab(this, this.ref_hab), null, new object[] { this, this.ref_hab });
				return;
			}
			if (this.location.isSpaceShipState)
			{
				GameControl.eventManager.TriggerEvent(new CouncilorDepartsShip(this, this.location.ref_ship), null, new object[]
				{
					this,
					this.location.ref_ship,
					this.location.ref_ship.fleet
				});
			}
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x000F7F9C File Offset: 0x000F619C
		public void ChangeLocation(TIGameState destination)
		{
			this.priorLocation = this.location;
			this.RemoveFromCurrentLocation();
			this.SetLocation(destination);
			GameControl.eventManager.TriggerEvent(new CouncilorPositionUpdated(this, destination), null, new object[] { this, this.priorLocation, this.preMissionPhaseLocation, destination });
			foreach (TICouncilorState ticouncilorState in this.faction.councilors)
			{
				if (ticouncilorState != this)
				{
					TIMissionState activeMission = ticouncilorState.activeMission;
					if (((activeMission != null) ? activeMission.target.ref_councilor : null) == this)
					{
						ticouncilorState.CheckAndChaseMissionTarget();
					}
				}
			}
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x000F8068 File Offset: 0x000F6268
		public void SetLocation(TIGameState location)
		{
			bool flag = this.location == location;
			this.location = location;
			if (location.ref_hab == location)
			{
				location.ref_hab.ArriveCouncilor(this);
			}
			if (location != null)
			{
				this.locationIllustration = this.SetIllustrationData(location, true, flag);
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06002D06 RID: 11526 RVA: 0x000F80BC File Offset: 0x000F62BC
		public string LongHaulHomeEventName
		{
			get
			{
				return new StringBuilder("LongHaulForCouncilor").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x000F80F4 File Offset: 0x000F62F4
		public void LongHaulHome(TISpaceGameState origin)
		{
			float num = TISpaceObjectState.GenericTransferTime_d(this.faction, origin, GameStateManager.Earth());
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddDays(num);
			TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, this.LongHaulHomeEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnLongHaulHomeComplete), this.LongHaulHomeEventName, null, true, true);
			this.status = CouncilorStatus.Offmap;
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x000F815C File Offset: 0x000F635C
		public void OnLongHaulHomeComplete(TimeEventStart e)
		{
			IEnumerable<TIRegionState> enumerable = GameStateManager.AllRegions().Where<TIRegionState>(delegate(TIRegionState x)
			{
				string text;
				return this.ValidDestination(x, out text);
			});
			if (enumerable.Count<TIRegionState>() > 0)
			{
				this.SetLocation(enumerable.SelectRandomItem<TIRegionState>());
			}
			else
			{
				GameStateManager.AllRegions().SelectRandomItem<TIRegionState>();
			}
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnLongHaulHomeComplete), this.LongHaulHomeEventName);
			this.status = CouncilorStatus.Active;
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x000F81C5 File Offset: 0x000F63C5
		public float AdvisingBonus(CouncilorAttribute attribute)
		{
			return (float)this.GetAttribute(attribute, true, true, true, false, false, false) / 100f;
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x000F81DB File Offset: 0x000F63DB
		public void ChangeXP(int value)
		{
			this.XP = Mathf.Max(this.XP + value, 0);
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06002D0B RID: 11531 RVA: 0x000F81F4 File Offset: 0x000F63F4
		public float XPModifier
		{
			get
			{
				return this.traits.Sum<TITraitTemplate>((TITraitTemplate x) => x.XPModifier) + this.activeOrgs.Sum<TIOrgState>((TIOrgState x) => x.XPModifier);
			}
		}

		// Token: 0x06002D0C RID: 11532 RVA: 0x000F8256 File Offset: 0x000F6456
		public bool CanAffordAnyCandidateAugmentations(bool XPAugmentsOnly)
		{
			if (!XPAugmentsOnly)
			{
				return this.GetCandidateAugmentations().Any<CouncilorAugmentationOption>((CouncilorAugmentationOption option) => option.CouncilorCanAfford(this));
			}
			return this.GetCandidateAugmentations().Any<CouncilorAugmentationOption>((CouncilorAugmentationOption option) => option.XPCost > 0 && option.CouncilorCanAfford(this));
		}

		// Token: 0x06002D0D RID: 11533 RVA: 0x000F828C File Offset: 0x000F648C
		public List<CouncilorAugmentationOption> GetCandidateAugmentations()
		{
			float num = 1f + TIEffectsState.SumEffectsModifiers(Context.AugmentationMoneyCost, this.faction, 1f, null);
			List<CouncilorAugmentationOption> list = new List<CouncilorAugmentationOption>();
			foreach (CouncilorAttribute councilorAttribute in Enums.CouncilorAttributes)
			{
				if (councilorAttribute != CouncilorAttribute.Loyalty && councilorAttribute != CouncilorAttribute.ApparentLoyalty && this.GetAttribute(councilorAttribute, false, true, true, false, false, false) < this.GetClampedMaxStatValue(councilorAttribute))
				{
					list.Add(new CouncilorAugmentationOption(councilorAttribute, null, 1f, num, this.XPModifier));
				}
			}
			foreach (TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(false))
			{
				if (titraitTemplate.CouncilorCanAddByAugment(this) || titraitTemplate.CouncilorCanRemoveByAugment(this))
				{
					CouncilorAugmentationOption councilorAugmentationOption = new CouncilorAugmentationOption(CouncilorAttribute.None, titraitTemplate, (!titraitTemplate.requiresProject && this.GetIndividualTraitChance(titraitTemplate, this.faction) == 0f) ? 2f : 1f, num, this.XPModifier);
					if (councilorAugmentationOption.CouncilorEligibleForAugmentation(this))
					{
						list.Add(councilorAugmentationOption);
					}
				}
			}
			return list;
		}

		// Token: 0x06002D0E RID: 11534 RVA: 0x000F83B0 File Offset: 0x000F65B0
		public void ApplyAugmentation(CouncilorAugmentationOption augmentation)
		{
			if (augmentation.traitToGain != null)
			{
				this.AddTrait(augmentation.traitToGain, false);
			}
			if (augmentation.traitToLose != null)
			{
				this.RemoveTrait(augmentation.traitToLose);
			}
			if (augmentation.stat != CouncilorAttribute.None)
			{
				this.ModifyAttribute(augmentation.stat, augmentation.statValue);
			}
			TIResourcesCost resourceCost = augmentation.resourceCost;
			if (resourceCost != null)
			{
				resourceCost.PayCost(this.faction, "Augmentation");
			}
			if (augmentation.XPCost > 0)
			{
				this.ChangeXP(-augmentation.XPCost);
			}
			GameControl.eventManager.TriggerEvent(new CouncilorValuesChanged(this), null, new object[] { this });
			if (this.faction != null && this.faction.isActivePlayer)
			{
				this.faction.UnlockAchievement("augment");
			}
		}

		// Token: 0x06002D0F RID: 11535 RVA: 0x000F8484 File Offset: 0x000F6684
		public TITraitTemplate GetTraitGrouping(int grouping)
		{
			foreach (TITraitTemplate titraitTemplate in this.traits)
			{
				int? grouping2 = titraitTemplate.grouping;
				if ((grouping2.GetValueOrDefault() == grouping) & (grouping2 != null))
				{
					return titraitTemplate;
				}
			}
			return null;
		}

		// Token: 0x06002D10 RID: 11536 RVA: 0x000F84F8 File Offset: 0x000F66F8
		public static List<TITraitTemplate> GetAllTraitsOfGrouping(int grouping)
		{
			List<TITraitTemplate> list = new List<TITraitTemplate>();
			foreach (TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(false))
			{
				int? grouping2 = titraitTemplate.grouping;
				if ((grouping2.GetValueOrDefault() == grouping) & (grouping2 != null))
				{
					list.Add(titraitTemplate);
				}
			}
			return list;
		}

		// Token: 0x06002D11 RID: 11537 RVA: 0x000F856C File Offset: 0x000F676C
		public TITraitTemplate GetTraitWithSpecialTraitRule(SpecialTraitRule rule)
		{
			return this.traits.FirstOrDefault<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == rule);
		}

		// Token: 0x06002D12 RID: 11538 RVA: 0x000F85A0 File Offset: 0x000F67A0
		public bool HasTraitWithTag(string tag)
		{
			if (!string.IsNullOrEmpty(tag))
			{
				using (List<TITraitTemplate>.Enumerator enumerator = this.traits.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.tags.Contains(tag))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06002D13 RID: 11539 RVA: 0x000F8608 File Offset: 0x000F6808
		public bool GrantsMarkedToAssassin()
		{
			if (this.isAlien)
			{
				return false;
			}
			if (this.GetTraitWithSpecialTraitRule(SpecialTraitRule.MarkedToAssassin) == null)
			{
				return this.activeOrgs.Any<TIOrgState>((TIOrgState x) => x.grantsMarked);
			}
			return true;
		}

		// Token: 0x06002D14 RID: 11540 RVA: 0x000F8658 File Offset: 0x000F6858
		public List<TITraitTemplate> GetAllCouncilorTraitsWithTag(string tag)
		{
			List<TITraitTemplate> list = new List<TITraitTemplate>();
			if (!string.IsNullOrEmpty(tag))
			{
				foreach (TITraitTemplate titraitTemplate in this.traits)
				{
					if (titraitTemplate.tags.Contains(tag))
					{
						list.Add(titraitTemplate);
					}
				}
			}
			return list;
		}

		// Token: 0x06002D15 RID: 11541 RVA: 0x000F86C8 File Offset: 0x000F68C8
		public static List<TITraitTemplate> GetAllTraitsWithTag(string tag)
		{
			List<TITraitTemplate> list = new List<TITraitTemplate>();
			if (!string.IsNullOrEmpty(tag))
			{
				foreach (TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(false))
				{
					if (titraitTemplate.tags.Contains(tag))
					{
						list.Add(titraitTemplate);
					}
				}
			}
			return list;
		}

		// Token: 0x06002D16 RID: 11542 RVA: 0x000F8734 File Offset: 0x000F6934
		public float TechCategoryBonusFromTraits(TechCategory techCategory)
		{
			float num = 0f;
			if (this.status == CouncilorStatus.Active)
			{
				foreach (TITraitTemplate titraitTemplate in this.traits)
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
			return num;
		}

		// Token: 0x06002D17 RID: 11543 RVA: 0x000F87D0 File Offset: 0x000F69D0
		public float MissionQuality(TIMissionTemplate mission)
		{
			if (mission.primaryAttackerStat != CouncilorAttribute.None)
			{
				return (float)this.GetAttribute(mission.primaryAttackerStat, true, true, true, false, false, false) / (float)TemplateManager.global.maxCouncilorAttribute;
			}
			return 1f;
		}

		// Token: 0x06002D18 RID: 11544 RVA: 0x000F8800 File Offset: 0x000F6A00
		public Sprite GetAirplaneTexture()
		{
			if (TIEffectsState.CheckForAnyEffectInContext(Context.AdvancedAircraft, this.faction))
			{
				if (this.GetMonthlyIncome(FactionResource.Money) > 10f)
				{
					return AssetCacheManager.privateJet2;
				}
				return AssetCacheManager.airliner2;
			}
			else
			{
				if (this.GetMonthlyIncome(FactionResource.Money) > 10f)
				{
					return AssetCacheManager.privateJet1;
				}
				return AssetCacheManager.airliner1;
			}
		}

		// Token: 0x06002D19 RID: 11545 RVA: 0x000F8850 File Offset: 0x000F6A50
		public string GetRecruitCostString(TIFactionState faction, bool includeCostString = true)
		{
			return this.HireRecruitCost(faction).GetString("N0", includeCostString, false, false, 0, false, false, faction, false, FactionResource.None);
		}

		// Token: 0x06002D1A RID: 11546 RVA: 0x000F8878 File Offset: 0x000F6A78
		public string GetCurrentMissionString(bool includeTarget = true, bool includeResolveTime = false, bool twoLineTarget = false)
		{
			if (this.HasMission)
			{
				if (includeTarget)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (twoLineTarget)
					{
						stringBuilder.Append(Loc.T("UI.CouncilorView.MissionStringWithTarget_TwoLine", new object[]
						{
							this.activeMission.GetMyTemplate().displayName,
							TIUtilities.GetStateDisplayName(this.activeMission.target, this.faction, false, false, false, false, false)
						}));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.CouncilorView.MissionStringWithTarget", new object[]
						{
							this.activeMission.GetMyTemplate().displayName,
							TIUtilities.GetStateDisplayName(this.activeMission.target, this.faction, false, false, false, false, false)
						}));
					}
					if (includeResolveTime && this.activeMission.resolveTimeAssigned)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.CouncilorView.ResolveTime", new object[] { this.activeMission.resolveTime.ToCustomDateString() }));
					}
					return stringBuilder.ToString();
				}
				return this.activeMission.GetMyTemplate().displayName;
			}
			else
			{
				if (this.detained)
				{
					return Loc.T("UI.Councilor.Detained");
				}
				if (TIMissionPhaseState.InMissionPhase())
				{
					return Loc.T("UI.CouncilorView.NoMission");
				}
				return Loc.T("UI.CouncilorView.NoMissionPhase");
			}
		}

		// Token: 0x06002D1B RID: 11547 RVA: 0x000F89B8 File Offset: 0x000F6BB8
		public Sprite GetIcon(bool forceUpdate = false)
		{
			if (this.icon == null || forceUpdate || this.usingOldPortrait != this.useOldPortrait)
			{
				this.usingOldPortrait = this.useOldPortrait;
				this.icon = GameControl.assetLoader.LoadAsset<Sprite>(this.iconResource);
			}
			return this.icon;
		}

		// Token: 0x06002D1C RID: 11548 RVA: 0x000F8A0C File Offset: 0x000F6C0C
		public string GetCurrentMissionIcon(bool on)
		{
			if (!this.HasMission)
			{
				return string.Empty;
			}
			TIMissionTemplate missionTemplate = this.activeMission.missionTemplate;
			if (!on)
			{
				return missionTemplate.missionIconImagePath_Off;
			}
			return missionTemplate.missionIconImagePath_On;
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06002D1D RID: 11549 RVA: 0x000F8A43 File Offset: 0x000F6C43
		public string jobDisplayName
		{
			get
			{
				return this.typeTemplate.displayName;
			}
		}

		// Token: 0x06002D1E RID: 11550 RVA: 0x000F8A50 File Offset: 0x000F6C50
		public string subjectivePronoun(bool cap)
		{
			string text = string.Empty;
			switch (this.gender)
			{
			case CouncilorGender.Female:
				text = Loc.T("TICouncilorTemplate.femaleSubjectivePronoun");
				break;
			case CouncilorGender.Male:
				text = Loc.T("TICouncilorTemplate.maleSubjectivePronoun");
				break;
			case CouncilorGender.Nonbinary:
				text = Loc.T("TICouncilorTemplate.nonBinarySubjectivePronoun");
				break;
			}
			if (cap)
			{
				Utilities.Capitalize(text);
			}
			return text;
		}

		// Token: 0x06002D1F RID: 11551 RVA: 0x000F8AB0 File Offset: 0x000F6CB0
		public string objectivePronoun(bool cap)
		{
			string text = string.Empty;
			switch (this.gender)
			{
			case CouncilorGender.Female:
				text = Loc.T("TICouncilorTemplate.femaleObjectivePronoun");
				break;
			case CouncilorGender.Male:
				text = Loc.T("TICouncilorTemplate.maleObjectivePronoun");
				break;
			case CouncilorGender.Nonbinary:
				text = Loc.T("TICouncilorTemplate.nonBinaryObjectivePronoun");
				break;
			}
			if (cap)
			{
				Utilities.Capitalize(text);
			}
			return text;
		}

		// Token: 0x06002D20 RID: 11552 RVA: 0x000F8B10 File Offset: 0x000F6D10
		public string possessivePronoun(bool cap)
		{
			string text = string.Empty;
			switch (this.gender)
			{
			case CouncilorGender.Female:
				text = Loc.T("TICouncilorTemplate.femalePossessivePronoun");
				break;
			case CouncilorGender.Male:
				text = Loc.T("TICouncilorTemplate.malePossessivePronoun");
				break;
			case CouncilorGender.Nonbinary:
				text = Loc.T("TICouncilorTemplate.nonBinaryPossessivePronoun");
				break;
			}
			if (cap)
			{
				Utilities.Capitalize(text);
			}
			return text;
		}

		// Token: 0x06002D21 RID: 11553 RVA: 0x000F8B70 File Offset: 0x000F6D70
		public string GetHomeLocationString()
		{
			return Loc.T("UI.Councilor.Hometown", new object[]
			{
				this.homeRegion.displayName,
				this.homeNation.displayName
			});
		}

		// Token: 0x06002D22 RID: 11554 RVA: 0x000F8B9E File Offset: 0x000F6D9E
		public string GetVerboseHomeLocationString()
		{
			return Loc.T("UI.Councilor.VerboseHometown", new object[] { this.GetHomeLocationString() });
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x000F8BB9 File Offset: 0x000F6DB9
		public string GetVerboseAgeString()
		{
			return Loc.T("UI.Councilor.Age", new object[] { this.age });
		}

		// Token: 0x06002D24 RID: 11556 RVA: 0x000F8BD9 File Offset: 0x000F6DD9
		public string GetDOBString()
		{
			return Loc.T("UI.Councilor.DOB", new object[]
			{
				this.dateBorn.ToCustomDateString(),
				this.GetVerboseAgeString()
			});
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06002D25 RID: 11557 RVA: 0x000F8C02 File Offset: 0x000F6E02
		public string genericIconPath
		{
			get
			{
				return this.faction.template.genericCouncilorIcon;
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06002D26 RID: 11558 RVA: 0x000F8C14 File Offset: 0x000F6E14
		public string projectContributionString
		{
			get
			{
				if (this.GetMonthlyIncome(FactionResource.Projects) <= 0f)
				{
					return "-";
				}
				return new StringBuilder(this.GetMonthlyIncome(FactionResource.Projects).ToString("N0")).ToString();
			}
		}

		// Token: 0x06002D27 RID: 11559 RVA: 0x000F8C54 File Offset: 0x000F6E54
		public string VisibleSummary(TIFactionState viewingFaction)
		{
			CouncilorView viewofCouncilor = viewingFaction.GetViewofCouncilor(this);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(viewofCouncilor.displayNameCurrent);
			if (viewingFaction.GetIntel(this) >= TemplateManager.global.intelToSeeCouncilorBasicData)
			{
				stringBuilder.AppendLine(viewofCouncilor.factionStringCurrentKnowledge(true, true));
				stringBuilder.AppendLine(viewofCouncilor.councilorJobStringCurrent);
				stringBuilder.AppendLine(viewofCouncilor.GetCurrentMissionString(true, true, false));
				foreach (CouncilorAttribute councilorAttribute in Enums.CouncilorAttributes)
				{
					if (councilorAttribute != CouncilorAttribute.ApparentLoyalty)
					{
						stringBuilder.Append(TIUtilities.InlineAttributeStr(councilorAttribute)).Append(viewofCouncilor.GetAttributeString(councilorAttribute));
					}
				}
				stringBuilder.AppendLine();
				if (viewofCouncilor.traits.Count > 0)
				{
					if (viewingFaction != this.faction && !this.isAlien)
					{
						if (!viewofCouncilor.traits.Any<TITraitTemplate>(delegate(TITraitTemplate x)
						{
							if (x.specialTraitRule != SpecialTraitRule.GlobalPropagandaIfKilled)
							{
								return x.tags.Any<string>((string y) => y == "Dangerous");
							}
							return true;
						}))
						{
							if (!viewofCouncilor.orgs.Any<TIOrgState>((TIOrgState x) => x.grantsMarked))
							{
								goto IL_0164;
							}
						}
						stringBuilder.Append(TemplateManager.global.warningInlineSpritePath).Append(TIUtilities.RedLine(Loc.T("UI.Traits.Dangerous"))).Append(TemplateManager.global.warningInlineSpritePath)
							.AppendLine();
					}
					IL_0164:
					stringBuilder.AppendLine(viewofCouncilor.traits.Select<TITraitTemplate, string>((TITraitTemplate x) => x.displayName).ToCommaSeparatedString<string>(null));
				}
				if (viewofCouncilor.orgs.Count > 0)
				{
					foreach (TIOrgState tiorgState in viewofCouncilor.orgs)
					{
						stringBuilder.Append(tiorgState.tierStarsInline).Append(tiorgState.displayName).Append(tiorgState.QuickDescription(false))
							.AppendLine();
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002D28 RID: 11560 RVA: 0x000F8E80 File Offset: 0x000F7080
		public CouncilorIllustrationData GetIllustrationData()
		{
			return this.locationIllustration;
		}

		// Token: 0x06002D29 RID: 11561 RVA: 0x000F8E88 File Offset: 0x000F7088
		public static CouncilorIllustrationData GetUnknownIllustrationData(TICouncilorState councilor)
		{
			int num = Math.Abs(councilor.familyName.GetHashCode() + councilor.location.displayName.GetHashCode());
			if (councilor.OnEarth)
			{
				int num2 = num % TemplateManager.global.illus_UnknownOnEarth.Count;
				return new CouncilorIllustrationData
				{
					illustrationPath = TemplateManager.global.illus_UnknownOnEarth[num2],
					offset = 0.25f
				};
			}
			if (councilor.OnAShip || councilor.InAHab)
			{
				int num3 = num % TemplateManager.global.illus_UnknownInSpace.Count;
				return new CouncilorIllustrationData
				{
					illustrationPath = TemplateManager.global.illus_UnknownInSpace[num3],
					offset = 0.25f
				};
			}
			return new CouncilorIllustrationData
			{
				illustrationPath = TemplateManager.global.illus_UnknownOnEarth[0],
				offset = 0.25f
			};
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x000F8F7C File Offset: 0x000F717C
		public CouncilorIllustrationData SetIllustrationData(TIGameState location, bool randomizeOffset, bool sameLocation = false)
		{
			int num = Math.Abs(this.familyName.GetHashCode() + TITimeState.Now().millisecond);
			float num2 = ((randomizeOffset && !sameLocation) ? TIUtilities.RandomRange(0f, 0.5f) : this.locationIllustration.offset);
			if (this.detained && this.OnEarth)
			{
				int num3 = num % TemplateManager.global.illus_detainedEarth.Count;
				return new CouncilorIllustrationData
				{
					illustrationPath = TemplateManager.global.illus_detainedEarth[num3],
					offset = num2
				};
			}
			if (location.isSpaceShipState)
			{
				int num4 = num % TemplateManager.global.illus_ShipInteriorPaths.Count;
				return new CouncilorIllustrationData
				{
					illustrationPath = TemplateManager.global.illus_ShipInteriorPaths[num4],
					offset = num2
				};
			}
			if (location.isHabState)
			{
				if (location.ref_hab.IsBase)
				{
					int num5 = num % TemplateManager.global.illus_BaseInteriorPaths.Count<string>();
					return new CouncilorIllustrationData
					{
						illustrationPath = TemplateManager.global.illus_BaseInteriorPaths[num5],
						offset = num2
					};
				}
				bool flag = location.ref_hab.ref_naturalSpaceObject.isEarth;
				if (!flag && location.ref_hab.ref_naturalSpaceObject.isLagrangePointState && location.ref_hab.GetSunOrbitingRelatedObject.isEarth)
				{
					TILagrangePointState ref_lagrangePoint = location.ref_hab.ref_naturalSpaceObject.ref_lagrangePoint;
					flag = ref_lagrangePoint.secondaryObject.isEarth || !ref_lagrangePoint.secondaryObject.barycenter.isEarth || ref_lagrangePoint.lagrangeValue != LagrangeValue.L2;
				}
				if (flag)
				{
					int num6 = num % TemplateManager.global.illus_EarthStationPaths.Count<string>();
					return new CouncilorIllustrationData
					{
						illustrationPath = TemplateManager.global.illus_EarthStationPaths[num6],
						offset = num2
					};
				}
				int num7 = num % TemplateManager.global.illus_StationInteriorPaths.Count;
				return new CouncilorIllustrationData
				{
					illustrationPath = TemplateManager.global.illus_StationInteriorPaths[num7],
					offset = num2
				};
			}
			else
			{
				if (!(location.ref_region != null))
				{
					return new CouncilorIllustrationData
					{
						illustrationPath = "illustrations/illus_Ecuador_0",
						offset = num2
					};
				}
				if (!this.isHuman)
				{
					int num8 = num % TemplateManager.global.illus_alienEarth.Count;
					return new CouncilorIllustrationData
					{
						illustrationPath = TemplateManager.global.illus_alienEarth[num8],
						offset = num2
					};
				}
				TIRegionState ref_region = location.ref_region;
				if (ref_region.illustrationPaths.Count > 0)
				{
					int num9 = num % ref_region.illustrationPaths.Count;
					return new CouncilorIllustrationData
					{
						illustrationPath = ref_region.illustrationPaths[num9],
						offset = num2
					};
				}
				return new CouncilorIllustrationData
				{
					illustrationPath = "illustrations/illus_Ecuador_0",
					offset = num2
				};
			}
		}

		// Token: 0x04002176 RID: 8566
		public string personalName;

		// Token: 0x04002177 RID: 8567
		public string familyName;

		// Token: 0x04002178 RID: 8568
		public string typeTemplateName;

		// Token: 0x0400217A RID: 8570
		public TIRegionState homeRegion;

		// Token: 0x0400217B RID: 8571
		public TIFactionState possibleFaction;

		// Token: 0x0400217C RID: 8572
		public TIFactionState detainingFaction;

		// Token: 0x04002180 RID: 8576
		public List<TIOrgState> orgs;

		// Token: 0x04002181 RID: 8577
		[fsIgnore]
		public List<TIOrgState> prospectiveOrgs = new List<TIOrgState>();

		// Token: 0x04002183 RID: 8579
		public TIGameState priorLocation;

		// Token: 0x04002184 RID: 8580
		public TIGameState preMissionPhaseLocation;

		// Token: 0x04002187 RID: 8583
		[SerializeField]
		private CouncilorIllustrationData locationIllustration;

		// Token: 0x04002188 RID: 8584
		public TIDateTime dateBorn;

		// Token: 0x04002189 RID: 8585
		public CouncilorGender gender;

		// Token: 0x0400218A RID: 8586
		public CouncilorAncestry ancestry;

		// Token: 0x0400218B RID: 8587
		public CouncilorStatus status;

		// Token: 0x0400218C RID: 8588
		public bool everBeenAvailable;

		// Token: 0x0400218D RID: 8589
		public string appearanceTemplateName;

		// Token: 0x0400218E RID: 8590
		public string voiceTemplateName;

		// Token: 0x0400218F RID: 8591
		public int XP;

		// Token: 0x04002194 RID: 8596
		public bool imBeingTargeted;

		// Token: 0x04002195 RID: 8597
		public bool targetedLastTurn;

		// Token: 0x0400219F RID: 8607
		public Dictionary<TIFactionState, int> assassinations = new Dictionary<TIFactionState, int>();

		// Token: 0x040021A0 RID: 8608
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040021A3 RID: 8611
		private TICouncilorTypeTemplate _typeTemplate;

		// Token: 0x040021A4 RID: 8612
		private TICouncilorVoiceTemplate _voiceTemplate;

		// Token: 0x040021A5 RID: 8613
		private Sprite icon;

		// Token: 0x040021A6 RID: 8614
		private bool usingOldPortrait;

		// Token: 0x040021A7 RID: 8615
		private TICouncilorAppearanceTemplate _appearanceTemplate;

		// Token: 0x040021A8 RID: 8616
		private Dictionary<CouncilorAttribute, int> cachedFinalAttributeValues = new Dictionary<CouncilorAttribute, int>();

		// Token: 0x040021A9 RID: 8617
		public static readonly CouncilorAttribute[] resourceModifyingAttributes = new CouncilorAttribute[]
		{
			CouncilorAttribute.Persuasion,
			CouncilorAttribute.Administration,
			CouncilorAttribute.Science,
			CouncilorAttribute.Command
		};
	}
}
