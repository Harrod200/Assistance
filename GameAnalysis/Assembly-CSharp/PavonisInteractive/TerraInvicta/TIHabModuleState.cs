using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A3 RID: 1955
	public class TIHabModuleState : TIGameState, CombatWeaponCarrierState, CombatTargetableState
	{
		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06003EE0 RID: 16096 RVA: 0x001963FF File Offset: 0x001945FF
		// (set) Token: 0x06003EE1 RID: 16097 RVA: 0x00196407 File Offset: 0x00194607
		public bool constructionCompleted { get; private set; }

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06003EE2 RID: 16098 RVA: 0x00196410 File Offset: 0x00194610
		// (set) Token: 0x06003EE3 RID: 16099 RVA: 0x00196418 File Offset: 0x00194618
		public DateTime completionDate { get; private set; }

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x06003EE4 RID: 16100 RVA: 0x00196421 File Offset: 0x00194621
		// (set) Token: 0x06003EE5 RID: 16101 RVA: 0x00196429 File Offset: 0x00194629
		public bool decommissioning { get; private set; }

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06003EE6 RID: 16102 RVA: 0x00196432 File Offset: 0x00194632
		// (set) Token: 0x06003EE7 RID: 16103 RVA: 0x0019643A File Offset: 0x0019463A
		public DateTime decommissionDate { get; private set; }

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06003EE8 RID: 16104 RVA: 0x00196443 File Offset: 0x00194643
		// (set) Token: 0x06003EE9 RID: 16105 RVA: 0x0019644B File Offset: 0x0019464B
		public bool powered { get; private set; }

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06003EEA RID: 16106 RVA: 0x00196454 File Offset: 0x00194654
		// (set) Token: 0x06003EEB RID: 16107 RVA: 0x0019645C File Offset: 0x0019465C
		public int slot { get; private set; }

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06003EEC RID: 16108 RVA: 0x00196465 File Offset: 0x00194665
		// (set) Token: 0x06003EED RID: 16109 RVA: 0x0019646D File Offset: 0x0019466D
		public TISectorState sector { get; private set; }

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06003EEE RID: 16110 RVA: 0x00196476 File Offset: 0x00194676
		// (set) Token: 0x06003EEF RID: 16111 RVA: 0x0019647E File Offset: 0x0019467E
		public bool destroyed { get; private set; }

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06003EF0 RID: 16112 RVA: 0x00196487 File Offset: 0x00194687
		public bool isCombatModule
		{
			get
			{
				TIHabModuleTemplate moduleTemplate = this.moduleTemplate;
				return moduleTemplate != null && moduleTemplate.spaceCombatModule;
			}
		}

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06003EF1 RID: 16113 RVA: 0x0019649A File Offset: 0x0019469A
		// (set) Token: 0x06003EF2 RID: 16114 RVA: 0x001964A2 File Offset: 0x001946A2
		public string defenseWeaponTemplateName { get; private set; }

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06003EF3 RID: 16115 RVA: 0x001964AB File Offset: 0x001946AB
		// (set) Token: 0x06003EF4 RID: 16116 RVA: 0x001964B3 File Offset: 0x001946B3
		public string defenseWeaponTemplateName_gun { get; private set; }

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06003EF5 RID: 16117 RVA: 0x001964BC File Offset: 0x001946BC
		// (set) Token: 0x06003EF6 RID: 16118 RVA: 0x001964C4 File Offset: 0x001946C4
		public string defenseWeaponTemplateName_plasma { get; private set; }

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x06003EF7 RID: 16119 RVA: 0x001964CD File Offset: 0x001946CD
		// (set) Token: 0x06003EF8 RID: 16120 RVA: 0x001964D5 File Offset: 0x001946D5
		public float _spaceCombatValue { get; private set; }

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x06003EF9 RID: 16121 RVA: 0x001964DE File Offset: 0x001946DE
		// (set) Token: 0x06003EFA RID: 16122 RVA: 0x001964E6 File Offset: 0x001946E6
		public string priorModuleTemplateName { get; private set; }

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x06003EFB RID: 16123 RVA: 0x001964EF File Offset: 0x001946EF
		// (set) Token: 0x06003EFC RID: 16124 RVA: 0x001964F7 File Offset: 0x001946F7
		public bool priorModuleCompleted { get; private set; }

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x06003EFD RID: 16125 RVA: 0x00196500 File Offset: 0x00194700
		// (set) Token: 0x06003EFE RID: 16126 RVA: 0x00196508 File Offset: 0x00194708
		public TIDateTime priorModuleCompletionDate { get; private set; }

		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x06003EFF RID: 16127 RVA: 0x00196511 File Offset: 0x00194711
		// (set) Token: 0x06003F00 RID: 16128 RVA: 0x00196519 File Offset: 0x00194719
		public TIDateTime abilityCooldownEnds { get; private set; }

		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x06003F01 RID: 16129 RVA: 0x00196522 File Offset: 0x00194722
		// (set) Token: 0x06003F02 RID: 16130 RVA: 0x0019652A File Offset: 0x0019472A
		[fsIgnore]
		public TIHabModuleTemplate moduleTemplate { get; private set; }

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x06003F03 RID: 16131 RVA: 0x00196533 File Offset: 0x00194733
		// (set) Token: 0x06003F04 RID: 16132 RVA: 0x0019653B File Offset: 0x0019473B
		[fsIgnore]
		public TIShipWeaponTemplate defenseWeapon { get; private set; }

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x06003F05 RID: 16133 RVA: 0x00196544 File Offset: 0x00194744
		// (set) Token: 0x06003F06 RID: 16134 RVA: 0x0019654C File Offset: 0x0019474C
		[fsIgnore]
		public TIShipWeaponTemplate defenseWeapon_gun { get; private set; }

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x06003F07 RID: 16135 RVA: 0x00196555 File Offset: 0x00194755
		// (set) Token: 0x06003F08 RID: 16136 RVA: 0x0019655D File Offset: 0x0019475D
		[fsIgnore]
		public TIShipWeaponTemplate defenseWeapon_plasma { get; private set; }

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x06003F09 RID: 16137 RVA: 0x00196566 File Offset: 0x00194766
		public bool empty
		{
			get
			{
				return this.templateName == null || this.templateName == string.Empty;
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x06003F0A RID: 16138 RVA: 0x00196582 File Offset: 0x00194782
		public bool underConstruction
		{
			get
			{
				return !this.empty && !this.constructionCompleted;
			}
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x06003F0B RID: 16139 RVA: 0x00196597 File Offset: 0x00194797
		public bool hasModule
		{
			get
			{
				return !this.empty;
			}
		}

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x06003F0C RID: 16140 RVA: 0x001965A2 File Offset: 0x001947A2
		public bool completed
		{
			get
			{
				return !this.empty && this.constructionCompleted;
			}
		}

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x06003F0D RID: 16141 RVA: 0x001965B4 File Offset: 0x001947B4
		public bool okay
		{
			get
			{
				return !this.empty && !this.destroyed && !this.decommissioning;
			}
		}

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x06003F0E RID: 16142 RVA: 0x001965D1 File Offset: 0x001947D1
		public bool functional
		{
			get
			{
				return this.completed && !this.destroyed && !this.decommissioning;
			}
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x06003F0F RID: 16143 RVA: 0x001965EE File Offset: 0x001947EE
		public bool active
		{
			get
			{
				return this.functional && this.powered;
			}
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x06003F10 RID: 16144 RVA: 0x00196600 File Offset: 0x00194800
		public bool present
		{
			get
			{
				return !this.empty && !this.destroyed;
			}
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x06003F11 RID: 16145 RVA: 0x00196615 File Offset: 0x00194815
		public bool mineLocation
		{
			get
			{
				return this.hab.IsBase && this.sectorNum == 0 && this.slot == 1;
			}
		}

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x06003F12 RID: 16146 RVA: 0x00196637 File Offset: 0x00194837
		public int sectorNum
		{
			get
			{
				return this.sector.sectorNum;
			}
		}

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06003F13 RID: 16147 RVA: 0x00196644 File Offset: 0x00194844
		public int tier
		{
			get
			{
				return this.moduleTemplate.tier;
			}
		}

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x06003F14 RID: 16148 RVA: 0x00196651 File Offset: 0x00194851
		public TIHabState hab
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.hab;
			}
		}

		// Token: 0x06003F15 RID: 16149 RVA: 0x00196664 File Offset: 0x00194864
		public bool IsAlien()
		{
			return this.ref_faction.IsAlienFaction;
		}

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x06003F16 RID: 16150 RVA: 0x00196671 File Offset: 0x00194871
		public override bool isHabModuleState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003F17 RID: 16151 RVA: 0x00196674 File Offset: 0x00194874
		public bool isShip()
		{
			return false;
		}

		// Token: 0x06003F18 RID: 16152 RVA: 0x00196677 File Offset: 0x00194877
		public bool isHabModule()
		{
			return true;
		}

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x06003F19 RID: 16153 RVA: 0x0019667A File Offset: 0x0019487A
		public override TIFactionState ref_faction
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_faction;
			}
		}

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x06003F1A RID: 16154 RVA: 0x0019668D File Offset: 0x0019488D
		public override TIHabState ref_hab
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.hab;
			}
		}

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x06003F1B RID: 16155 RVA: 0x001966A0 File Offset: 0x001948A0
		public override TIHabSiteState ref_habSite
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_habSite;
			}
		}

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x06003F1C RID: 16156 RVA: 0x001966B3 File Offset: 0x001948B3
		public override TIOrbitState ref_orbit
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_orbit;
			}
		}

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x06003F1D RID: 16157 RVA: 0x001966C6 File Offset: 0x001948C6
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_naturalSpaceObject;
			}
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x06003F1E RID: 16158 RVA: 0x001966D9 File Offset: 0x001948D9
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_spaceBody;
			}
		}

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x06003F1F RID: 16159 RVA: 0x001966EC File Offset: 0x001948EC
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_spaceObject;
			}
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06003F20 RID: 16160 RVA: 0x001966FF File Offset: 0x001948FF
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				TISectorState sector = this.sector;
				if (sector == null)
				{
					return null;
				}
				return sector.ref_spaceAsset;
			}
		}

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x06003F21 RID: 16161 RVA: 0x00196712 File Offset: 0x00194912
		public override TIHabModuleState ref_habModule
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06003F22 RID: 16162 RVA: 0x00196715 File Offset: 0x00194915
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06003F23 RID: 16163 RVA: 0x00196718 File Offset: 0x00194918
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x0019671B File Offset: 0x0019491B
		public TISpaceShipState ref_shipCarrier()
		{
			return null;
		}

		// Token: 0x06003F25 RID: 16165 RVA: 0x0019671E File Offset: 0x0019491E
		public TIHabModuleState ref_habModuleCarrier()
		{
			return this;
		}

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06003F26 RID: 16166 RVA: 0x00196721 File Offset: 0x00194921
		public int crew
		{
			get
			{
				if (this.moduleTemplate == null || this.destroyed)
				{
					return 0;
				}
				return this.moduleTemplate.crew;
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x06003F27 RID: 16167 RVA: 0x00196740 File Offset: 0x00194940
		public HabModuleController HabModuleController
		{
			get
			{
				return this.hab.controller.GetComponentInChildren<HabModelController>(true).GetModuleControllers().FirstOrDefault<HabModuleController>((HabModuleController x) => x.habModule == this);
			}
		}

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x06003F28 RID: 16168 RVA: 0x00196769 File Offset: 0x00194969
		public TIHabModuleTemplate priorModuleTemplate
		{
			get
			{
				return TemplateManager.Find<TIHabModuleTemplate>(this.priorModuleTemplateName, false);
			}
		}

		// Token: 0x06003F29 RID: 16169 RVA: 0x00196778 File Offset: 0x00194978
		public bool CanUpgrade(TIFactionState faction)
		{
			if (this.constructionCompleted && !this.decommissioning)
			{
				TIHabModuleTemplate tihabModuleTemplate = this.moduleTemplate.UpgradeModuleTemplate(faction, true);
				return tihabModuleTemplate != null && this.hab.IsModuleAllowedForThisHab(faction, tihabModuleTemplate, false) && this.sector.ValidModuleForSlot(tihabModuleTemplate, this.slot);
			}
			return false;
		}

		// Token: 0x06003F2A RID: 16170 RVA: 0x001967CC File Offset: 0x001949CC
		public int AtrocitiesToDestroy()
		{
			if (this.functional)
			{
				if (this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.AtrocityToKill))
				{
					return 1;
				}
				if (this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.AtrocityToKill_Populous) && this.ref_naturalSpaceObject.Populous())
				{
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06003F2B RID: 16171 RVA: 0x0019681B File Offset: 0x00194A1B
		public int AtrocitiesToLose()
		{
			if (this.functional && this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.AtrocityToLose))
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06003F2C RID: 16172 RVA: 0x0019683C File Offset: 0x00194A3C
		public void InitializeEmpty(TISectorState sector, int slot)
		{
			this.sector = sector;
			this.slot = slot;
			this.constructionCompleted = false;
			this.decommissioning = false;
			this.powered = false;
			this.destroyed = false;
			this.priorModuleTemplateName = string.Empty;
			this.priorModuleCompleted = false;
			this.templateName = string.Empty;
			this.moduleTemplate = null;
			this._priorModuleTemplate = null;
		}

		// Token: 0x06003F2D RID: 16173 RVA: 0x0019689E File Offset: 0x00194A9E
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.moduleTemplate == null && this.templateName != string.Empty)
			{
				this.moduleTemplate = TemplateManager.Find<TIHabModuleTemplate>(this.templateName, false);
			}
		}

		// Token: 0x06003F2E RID: 16174 RVA: 0x001968CC File Offset: 0x00194ACC
		public override void PostInitializationInit_4()
		{
			if (this.sector == null || this.sector.deleted || this.hab == null || this.hab.deleted)
			{
				base.ArchiveState(true);
				return;
			}
			if (this.isCombatModule)
			{
				this.SetSpaceCombatWeapons(this.ref_faction);
				if (this.hab.underBombardment)
				{
					this.InitializeForBombardment();
					return;
				}
			}
			else
			{
				this._spaceCombatValue = 0f;
			}
		}

		// Token: 0x06003F2F RID: 16175 RVA: 0x0019694A File Offset: 0x00194B4A
		public bool IsModuleValidForSlot(TIHabModuleTemplate moduleTemplate)
		{
			return this.sector.ValidModuleForSlot(moduleTemplate, this.slot);
		}

		// Token: 0x06003F30 RID: 16176 RVA: 0x00196960 File Offset: 0x00194B60
		private void SetModuleTemplate(string newModuleTemplateName)
		{
			if (!string.IsNullOrEmpty(this.templateName) && this.templateName != newModuleTemplateName)
			{
				this.priorModuleTemplateName = this.templateName;
				this._priorModuleTemplate = this.moduleTemplate;
				this.priorModuleCompleted = this.completed;
				this.priorModuleCompletionDate = new TIDateTime(this.completionDate);
			}
			this.templateName = newModuleTemplateName;
			this.moduleTemplate = TemplateManager.Find<TIHabModuleTemplate>(newModuleTemplateName, false);
			if (this.moduleTemplate == null)
			{
				Log.Error("Missing template for moduleName " + this.templateName, Array.Empty<object>());
			}
			this.displayName = this.moduleTemplate.displayName;
		}

		// Token: 0x06003F31 RID: 16177 RVA: 0x00196A04 File Offset: 0x00194C04
		public void SetCompletedModule(string moduleTemplateName, bool startup = false)
		{
			this.SetModuleTemplate(moduleTemplateName);
			this.completionDate = World.Active.GetExistingManager<GameTimeManager>().Now;
			this.CompleteConstruction(startup);
			this.destroyed = false;
			this.destroyedTime = null;
		}

		// Token: 0x06003F32 RID: 16178 RVA: 0x00196A38 File Offset: 0x00194C38
		public void InitiateConstructModule(string moduleTemplateName, TIResourcesCost cost, double selectedCompletionTime_Days)
		{
			this._spaceCombatValue = 0f;
			this.destroyed = false;
			this.SetModuleTemplate(moduleTemplateName);
			this.constructionCompleted = false;
			this.decommissioning = false;
			this.hab.SetModulesDirty();
			this.SetPowerStatus(false, false);
			if (cost != null)
			{
				this.buildCost = new TIResourcesCost(cost);
			}
			float habConstructionDurationModifier = this.hab.faction.GetHabConstructionDurationModifier();
			this.baseBuildDuration_days = this.moduleTemplate.buildTime_Days * TIGlobalValuesState.GetHabModuleConstructionTimeSettingsModifier(this.hab.faction) * ((this.moduleTemplate.UpgradesFrom == this.priorModuleTemplate) ? 0.6666667f : 1f) * habConstructionDurationModifier;
			this.appliedBuildConstructionBonus = this.hab.GetModuleConstructionTimeModifier(false, null);
			DateTime dateTime = World.Active.GetExistingManager<GameTimeManager>().Now;
			this.completionDate = dateTime.AddDays((selectedCompletionTime_Days >= 0.0) ? selectedCompletionTime_Days : ((double)(this.moduleTemplate.buildTime_Days * TIGlobalValuesState.GetHabModuleConstructionTimeSettingsModifier(this.hab.faction) * habConstructionDurationModifier)));
			dateTime = this.completionDate;
			this.startBuildDate = dateTime.AddDays((double)(-(double)this.baseBuildDuration_days * this.appliedBuildConstructionBonus));
		}

		// Token: 0x06003F33 RID: 16179 RVA: 0x00196B64 File Offset: 0x00194D64
		public bool InTransit()
		{
			return this.underConstruction && new TIDateTime(this.startBuildDate) > TITimeState.Now();
		}

		// Token: 0x06003F34 RID: 16180 RVA: 0x00196B88 File Offset: 0x00194D88
		public float PercentBuilt()
		{
			if (!this.underConstruction)
			{
				return 1f;
			}
			if (new TIDateTime(this.startBuildDate) > TITimeState.Now())
			{
				return 0f;
			}
			double totalDays = (this.completionDate - this.startBuildDate).TotalDays;
			return (float)(this.completionDate - TITimeState.SystemNow()).TotalDays / (float)totalDays;
		}

		// Token: 0x06003F35 RID: 16181 RVA: 0x00196BF8 File Offset: 0x00194DF8
		public void CompleteConstruction(bool startup = false)
		{
			this.constructionCompleted = true;
			TIHabModuleTemplate moduleTemplate = this.moduleTemplate;
			if (moduleTemplate != null && moduleTemplate.coreModule)
			{
				this.hab.anyCoreCompleted = true;
				if (!this.hab.createdFromTemplate)
				{
					TIGlobalValuesState.GlobalValues.CheckGlobalMilestoneOnHabFounding(this.hab, false);
				}
			}
			TIHabModuleTemplate moduleTemplate2 = this.moduleTemplate;
			if (moduleTemplate2 != null && moduleTemplate2.allowsShipConstruction)
			{
				this.sector.faction.AddShipyardToFaction(this, startup);
				if (!startup && this.sector.faction.shipConstructionModules.Count == 1)
				{
					TIFactionState faction = this.sector.faction;
					IEnumerable<TIShipHullTemplate> allowedShipHulls = this.sector.faction.allowedShipHulls;
					faction.updateShipDesignsFlag = allowedShipHulls != null && allowedShipHulls.Count<TIShipHullTemplate>() > 0;
				}
			}
			TIHabModuleTemplate moduleTemplate3 = this.moduleTemplate;
			TIProjectTemplate tiprojectTemplate = ((moduleTemplate3 != null) ? moduleTemplate3.GetProjectUnlocked() : null);
			if (tiprojectTemplate != null && !this.sector.faction.completedProjects.Contains(tiprojectTemplate))
			{
				this.ref_faction.AddAvailableProject(tiprojectTemplate, null);
			}
			if (this.isCombatModule && !startup)
			{
				this.SetSpaceCombatWeapons(this.ref_faction);
			}
			this.hab.SetModulesDirty();
		}

		// Token: 0x06003F36 RID: 16182 RVA: 0x00196D18 File Offset: 0x00194F18
		public void ChangeFutureCompletionDate(float days)
		{
			if (this.completionDate > TITimeState.SystemNow())
			{
				this.completionDate = this.completionDate.AddDays((double)days);
			}
		}

		// Token: 0x06003F37 RID: 16183 RVA: 0x00196D4D File Offset: 0x00194F4D
		public bool PowerProvider()
		{
			return this.ModulePower() > 0;
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x00196D58 File Offset: 0x00194F58
		public bool PowerConsumer()
		{
			return this.ModulePower() < 0;
		}

		// Token: 0x06003F39 RID: 16185 RVA: 0x00196D63 File Offset: 0x00194F63
		public int PowerConsumed()
		{
			return this.ModulePower() * -1;
		}

		// Token: 0x06003F3A RID: 16186 RVA: 0x00196D70 File Offset: 0x00194F70
		public void SetSpaceCombatWeapons(TIFactionState faction)
		{
			this.defenseWeaponTemplateName = faction.GetBestHabWeapon(this.hab.IsBase, this.tier, WeaponClass.Laser, this.hab.ref_spaceBody, null);
			this.defenseWeapon = TemplateManager.Find<TIShipWeaponTemplate>(this.defenseWeaponTemplateName, true);
			this.defenseWeaponTemplateName_gun = faction.GetBestHabWeapon(this.hab.IsBase, this.tier, WeaponClass.Magnetic, this.hab.ref_spaceBody, null);
			this.defenseWeapon_gun = TemplateManager.Find<TIShipWeaponTemplate>(this.defenseWeaponTemplateName_gun, true);
			if (this.moduleTemplate.weaponMounts >= 4)
			{
				this.defenseWeaponTemplateName_plasma = faction.GetBestHabWeapon(this.hab.IsBase, this.tier, WeaponClass.Plasma, this.hab.ref_spaceBody, null);
				this.defenseWeapon_plasma = TemplateManager.Find<TIShipWeaponTemplate>(this.defenseWeaponTemplateName_plasma, true);
			}
			if (this.moduleTemplate != null)
			{
				this._spaceCombatValue = this.moduleTemplate.SpaceCombatValue(faction, this.hab, true);
				return;
			}
			this._spaceCombatValue = 0f;
		}

		// Token: 0x06003F3B RID: 16187 RVA: 0x00196E6C File Offset: 0x0019506C
		public float GetSpaceCombatRange()
		{
			if (this.isCombatModule && this.ref_faction != null)
			{
				return this.moduleTemplate.NotionalWeaponsList(this.hab.faction, this.hab.IsBase, this.hab.ref_spaceBody, true).Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km);
			}
			return 0f;
		}

		// Token: 0x06003F3C RID: 16188 RVA: 0x00196EE6 File Offset: 0x001950E6
		public float SpaceCombatValue()
		{
			return this._spaceCombatValue;
		}

		// Token: 0x06003F3D RID: 16189 RVA: 0x00196EEE File Offset: 0x001950EE
		public float GetCrossSectionalArea_m2(float angle = -3.4028235E+38f)
		{
			return this.moduleTemplate.GetCrossSectionalArea_m2(angle);
		}

		// Token: 0x06003F3E RID: 16190 RVA: 0x00196EFC File Offset: 0x001950FC
		public string GetCombatSummary()
		{
			StringBuilder stringBuilder = new StringBuilder(this.hab.displayName).AppendLine();
			stringBuilder.AppendLine(this.displayName);
			stringBuilder.AppendLine();
			if (this.isCombatModule && this.ref_faction != null)
			{
				foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.moduleTemplate.NotionalWeaponsList(this.hab.faction, this.hab.IsBase, this.hab.ref_spaceBody, true))
				{
					stringBuilder.AppendLine(tishipWeaponTemplate.displayName);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003F3F RID: 16191 RVA: 0x00196FC4 File Offset: 0x001951C4
		public static string FullSummary(TIHabModuleState habModule, bool includeExtended)
		{
			if (!TIGameState.Valid(habModule) || habModule.moduleTemplate == null || habModule.ref_faction == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(habModule.moduleTemplate.displayName);
			if (!habModule.IsAlien())
			{
				if (habModule.decommissioning)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.Decommissioning", new object[] { new TIDateTime(habModule.decommissionDate).ToCustomDateString() }));
				}
				else
				{
					if (habModule.underConstruction)
					{
						stringBuilder.Append(TemplateManager.global.underConstructionInlineSpritePath).AppendLine(Loc.T("UI.Habs.CompletionDate", new object[] { new TIDateTime(habModule.completionDate).ToCustomDateString() }));
					}
					if (habModule.isCombatModule && habModule.ref_faction != null)
					{
						foreach (TIShipWeaponTemplate tishipWeaponTemplate in habModule.moduleTemplate.NotionalWeaponsList(habModule.GetFaction(), habModule.hab.IsBase, habModule.ref_spaceBody, true))
						{
							stringBuilder.AppendLine(tishipWeaponTemplate.displayName);
						}
					}
					stringBuilder.AppendLine(habModule.moduleTemplate.benefitsAndCostsDescription(habModule.GetFaction(), habModule.hab, false));
					if (includeExtended)
					{
						stringBuilder.AppendLine(habModule.moduleTemplate.extendedDescription);
					}
					else
					{
						stringBuilder.AppendLine(habModule.moduleTemplate.description);
					}
					if (habModule.moduleTemplate.allowsShipConstruction && habModule.GetFaction().nShipyardQueues.ContainsKey(habModule))
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Habs.ShipyardStatus", new object[] { habModule.GetFaction().nShipyardQueues[habModule].Count.ToString("N0") }));
					}
					if (habModule.moduleTemplate.IsSolarPower && habModule.hab.IsBase && TIHabModuleState.SolarMirrorBonus(habModule.hab, habModule.hab.faction, habModule.tier) > 0)
					{
						stringBuilder.AppendLine(Loc.T("UI.Intel.SolarMirrorBonus_SingleModule", new object[] { TIHabModuleState.SolarMirrorBonus(habModule.hab, habModule.hab.faction, habModule.tier) }));
					}
					if (habModule.destroyed && habModule.priorModuleTemplate != null && !habModule.priorModuleTemplate.destroyed)
					{
						stringBuilder.AppendLine(Loc.T("UI.Habs.PriorModule", new object[] { habModule.priorModuleTemplate.displayName }));
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x06003F40 RID: 16192 RVA: 0x00197278 File Offset: 0x00195478
		public string pointDefenseWeaponTemplateName
		{
			get
			{
				if (!this.hab.IsAlien())
				{
					return this.ref_faction.GetBestPointDefenseWeaponTemplateName();
				}
				return "AlienPointDefenseLaserTurret";
			}
		}

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x06003F41 RID: 16193 RVA: 0x00197298 File Offset: 0x00195498
		public TIShipWeaponTemplate defenseWeaponTemplate
		{
			get
			{
				return TemplateManager.Find<TIShipWeaponTemplate>(this.defenseWeaponTemplateName, true);
			}
		}

		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x06003F42 RID: 16194 RVA: 0x001972A6 File Offset: 0x001954A6
		public TIShipWeaponTemplate defenseWeaponTemplate_gun
		{
			get
			{
				return TemplateManager.Find<TIShipWeaponTemplate>(this.defenseWeaponTemplateName_gun, true);
			}
		}

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06003F43 RID: 16195 RVA: 0x001972B4 File Offset: 0x001954B4
		public TIShipWeaponTemplate defenseWeaponTemplate_plasma
		{
			get
			{
				return TemplateManager.Find<TIShipWeaponTemplate>(this.defenseWeaponTemplateName_plasma, true);
			}
		}

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x06003F44 RID: 16196 RVA: 0x001972C2 File Offset: 0x001954C2
		public TIShipWeaponTemplate PointDefenseWeaponTemplate
		{
			get
			{
				return TemplateManager.Find<TIShipWeaponTemplate>(this.pointDefenseWeaponTemplateName, true);
			}
		}

		// Token: 0x06003F45 RID: 16197 RVA: 0x001972D0 File Offset: 0x001954D0
		public float TargetingBonus(TIShipWeaponTemplate weapon, TIHabState alliedHab)
		{
			if (!this.active)
			{
				return 0f;
			}
			return this.moduleTemplate.TargetingBonus(this.ref_faction, alliedHab);
		}

		// Token: 0x06003F46 RID: 16198 RVA: 0x001972F4 File Offset: 0x001954F4
		public float ECMValue(TIFactionState attacker)
		{
			if (!this.active)
			{
				return 0f;
			}
			float specialRuleValue = this.moduleTemplate.GetSpecialRuleValue(HabModuleSpecialRule.FleetECM);
			return TIEffectsState.SumEffectsModifiers(Context.GlobalECMBonus, this.ref_faction, specialRuleValue, null);
		}

		// Token: 0x06003F47 RID: 16199 RVA: 0x00197331 File Offset: 0x00195531
		public float ECMValue(TIFactionState attacker, TIHabState alliedHab)
		{
			if (!this.active)
			{
				return 0f;
			}
			return this.moduleTemplate.ECMValue(this.ref_faction, alliedHab);
		}

		// Token: 0x06003F48 RID: 16200 RVA: 0x00197353 File Offset: 0x00195553
		public float FleetTargetingBonus()
		{
			return this.moduleTemplate.GetSpecialRuleValue(HabModuleSpecialRule.FleetTargeting);
		}

		// Token: 0x06003F49 RID: 16201 RVA: 0x00197362 File Offset: 0x00195562
		public float FleetECMBonus()
		{
			return this.moduleTemplate.GetSpecialRuleValue(HabModuleSpecialRule.FleetECM);
		}

		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06003F4A RID: 16202 RVA: 0x00197374 File Offset: 0x00195574
		public TIShipArmorTemplate armorTemplate
		{
			get
			{
				TIFactionState faction = this.GetFaction();
				bool flag;
				if (this.GetFaction().IsAlienFaction)
				{
					TIGameState ref_naturalSpaceObject = this.hab.ref_naturalSpaceObject;
					TIHabState primaryHab = this.GetFaction().primaryHab;
					flag = ref_naturalSpaceObject == ((primaryHab != null) ? primaryHab.ref_naturalSpaceObject : null);
				}
				else
				{
					flag = false;
				}
				return faction.GetBestArmor(flag);
			}
		}

		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x06003F4B RID: 16203 RVA: 0x001973C3 File Offset: 0x001955C3
		public float StationModuleArmorPoints
		{
			get
			{
				return this.moduleTemplate.StationModuleArmorPoints;
			}
		}

		// Token: 0x06003F4C RID: 16204 RVA: 0x001973D0 File Offset: 0x001955D0
		public float AntiBombardmentArmor(bool fullCalculation)
		{
			TIHabModuleTemplate tihabModuleTemplate = null;
			if (this.completed || this.underConstruction)
			{
				tihabModuleTemplate = this.moduleTemplate;
			}
			if (tihabModuleTemplate == null)
			{
				return 0f;
			}
			float num = (float)(2 * tihabModuleTemplate.tier) / this.armorTemplate.mass_damagePoint_kg;
			if (tihabModuleTemplate.spaceCombatModule || tihabModuleTemplate.SpecialRules.Contains(HabModuleSpecialRule.DropTroops))
			{
				num *= ((fullCalculation && this.powered) ? 4f : 2f);
			}
			else if (tihabModuleTemplate.coreModule || this.PowerProvider())
			{
				num *= 2f;
			}
			if (fullCalculation && this.underConstruction)
			{
				float num2 = this.PercentBuilt();
				num *= num2 * num2;
			}
			return num;
		}

		// Token: 0x17000B76 RID: 2934
		// (get) Token: 0x06003F4D RID: 16205 RVA: 0x00197478 File Offset: 0x00195678
		// (set) Token: 0x06003F4E RID: 16206 RVA: 0x00197480 File Offset: 0x00195680
		public float armorChipped { get; private set; }

		// Token: 0x06003F4F RID: 16207 RVA: 0x00197489 File Offset: 0x00195689
		public void ChipBombardmentArmor(float chipDamage)
		{
			this.armorChipped += chipDamage;
			this.armorChipped = Mathf.Clamp01(this.armorChipped);
		}

		// Token: 0x06003F50 RID: 16208 RVA: 0x001974AA File Offset: 0x001956AA
		public void ResetBombadardmentArmor()
		{
			this.armorChipped = 0f;
		}

		// Token: 0x06003F51 RID: 16209 RVA: 0x001974B7 File Offset: 0x001956B7
		public bool CanPower()
		{
			return !this.decommissioning && !this.destroyed && this.MeetsPopulationRequirements() && (this.PowerProvider() || this.hab.NetPower(false, false) >= this.PowerConsumed());
		}

		// Token: 0x06003F52 RID: 16210 RVA: 0x001974F8 File Offset: 0x001956F8
		public bool CanDepower()
		{
			int num = this.ModulePower();
			if (this.PowerProvider())
			{
				return this.hab.NetPower(false, false) >= num;
			}
			if (this.moduleTemplate.allowsShipConstruction && this.completed)
			{
				if (this.ref_faction.GetShipyardQueue(this).Count <= 0)
				{
					if (!this.hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.IsRepairing()))
					{
						goto IL_007F;
					}
				}
				return false;
			}
			IL_007F:
			if (this.moduleTemplate.allowsResupply && this.completed)
			{
				if (this.hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.IsResupplying()))
				{
					return false;
				}
			}
			if (this.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.PowerFirst))
			{
				return this.hab.UnpoweredModules().None<TIHabModuleState>((TIHabModuleState x) => x.PowerProvider()) && this.hab.NetPower(false, false) < 0;
			}
			return true;
		}

		// Token: 0x06003F53 RID: 16211 RVA: 0x00197628 File Offset: 0x00195828
		public bool MeetsPopulationRequirements()
		{
			return (!this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.Requires_Colonized_Body) || this.hab.location.ref_naturalSpaceObject.Colonized()) && (!this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.Requires_Inhabited_Body) || this.hab.location.ref_naturalSpaceObject.Populous());
		}

		// Token: 0x06003F54 RID: 16212 RVA: 0x0019768C File Offset: 0x0019588C
		public void SetPowerStatus(bool powerSetting, bool skipFullResourceUpdate = false)
		{
			bool powered = this.powered;
			if (powerSetting == powered || this.moduleTemplate == null)
			{
				return;
			}
			if (this.decommissioning)
			{
				this.powered = false;
			}
			else if (this.hab.anyCoreCompleted)
			{
				if (powerSetting && ((this.PowerConsumer() && this.PowerConsumed() > this.hab.NetPower(false, false)) || !this.MeetsPopulationRequirements()))
				{
					this.powered = false;
				}
				else
				{
					this.powered = powerSetting;
				}
			}
			else
			{
				this.powered = false;
			}
			if (powered != this.powered)
			{
				if (this.active && this.moduleTemplate.objectiveModule)
				{
					this.ref_faction.CheckForObjectivesCompleteViaHabModuleActivated(this);
				}
				if (this.moduleTemplate.incomeProjects > 0)
				{
					this.sector.faction.CheckforHabProjectUnlock();
				}
				if (this.moduleTemplate.moduleConstructionSpeedModifier != 1f)
				{
					this.hab.UpdateAllModuleConstructionTimes();
				}
				if (this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.HarvestHelium3))
				{
					this.sector.faction.SetHe3Access();
				}
				if (this.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.SolarMirror))
				{
					int num = (int)this.moduleTemplate.specialRulesValue * (this.powered ? 1 : (-1));
					if (this.hab.orbitState.barycenter.isSpaceBodyState)
					{
						this.hab.orbitState.barycenter.ref_spaceBody.ChangeSolarMirrorBonus(num, this.hab.faction);
					}
					else if (this.hab.orbitState.barycenter.isLagrangePointState)
					{
						TILagrangePointState ref_lagrangePoint = this.hab.orbitState.ref_lagrangePoint;
						if (ref_lagrangePoint.secondaryObject.isaMoon)
						{
							ref_lagrangePoint.secondaryObject.ChangeSolarMirrorBonus(num, this.hab.faction);
						}
						else if (ref_lagrangePoint.lagrangeValue == LagrangeValue.L1)
						{
							ref_lagrangePoint.secondaryObject.ChangeSolarMirrorBonus(num, this.hab.faction);
							foreach (TISpaceBodyState tispaceBodyState in ref_lagrangePoint.secondaryObject.naturalSatellites)
							{
								tispaceBodyState.ChangeSolarMirrorBonus(num, this.hab.faction);
							}
						}
					}
				}
				if (this.completed && !powerSetting)
				{
					if (this.moduleTemplate.allowsResupply && !this.hab.AllowsResupply(this.ref_faction, false, false))
					{
						foreach (TISpaceFleetState tispaceFleetState in this.hab.dockedFleets)
						{
							if (tispaceFleetState.IsResupplying())
							{
								tispaceFleetState.CancelOperation(tispaceFleetState.CurrentOperations().First<OperationData>((OperationData x) => x.operation is ResupplyOperation));
							}
						}
					}
					if (this.moduleTemplate.allowsShipConstruction && !this.hab.AllowsShipConstruction(this.ref_faction, false, false))
					{
						foreach (TISpaceFleetState tispaceFleetState2 in this.hab.dockedFleets)
						{
							if (tispaceFleetState2.IsRepairing())
							{
								tispaceFleetState2.CancelOperation(tispaceFleetState2.CurrentOperations().First<OperationData>((OperationData x) => x.operation is RepairFleetOperation));
							}
						}
					}
					if (this.moduleTemplate.spaceCombatModule && this.hab.underBombardment && this.hab.habSite.GetController() != null)
					{
						this.hab.habSite.GetController().CeaseBeamFire(this);
					}
				}
				if (!skipFullResourceUpdate)
				{
					this.sector.hab.UpdateCurrentAnnualNetResourceIncomes(false);
				}
			}
		}

		// Token: 0x06003F55 RID: 16213 RVA: 0x00197A84 File Offset: 0x00195C84
		public int ModulePower()
		{
			for (int i = 0; i < this.moduleTemplate.SpecialRules.Count; i++)
			{
				if (this.moduleTemplate.SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output)
				{
					return TIHabModuleState.SolarPowerOutput(this.hab, (float)this.moduleTemplate.power, this.ref_faction, this.moduleTemplate.tier, false);
				}
				if (this.moduleTemplate.SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity)
				{
					return TIHabModuleState.EscapeVelocityBasedPowerRequirement(this.hab, this.moduleTemplate, this.ref_faction);
				}
			}
			return this.moduleTemplate.power;
		}

		// Token: 0x06003F56 RID: 16214 RVA: 0x00197B24 File Offset: 0x00195D24
		public static int SolarPowerOutput(TIGameState location, float powerValue, TIFactionState faction, int tier, bool skipMirrors = false)
		{
			int num = (int)Mathf.Round(TIHabModuleState.NaturalSolarPowerMultiplier(location) * powerValue);
			if (!skipMirrors && (location.isSpaceBodyState || location.isHabSiteState || (location.isHabState && location.ref_hab.IsBase)))
			{
				num += TIHabModuleState.SolarMirrorBonus(location, faction, tier);
			}
			return Mathf.Min(num, (int)(8f * powerValue));
		}

		// Token: 0x06003F57 RID: 16215 RVA: 0x00197B82 File Offset: 0x00195D82
		public static int SolarMirrorBonus(TIGameState location, TIFactionState faction, int tier)
		{
			TISpaceBodyState ref_spaceBody = location.ref_spaceBody;
			if (ref_spaceBody == null)
			{
				return 0;
			}
			return ref_spaceBody.solarMirrorBonus[faction] * tier;
		}

		// Token: 0x06003F58 RID: 16216 RVA: 0x00197BA0 File Offset: 0x00195DA0
		public static float NaturalSolarPowerMultiplier(TIGameState location)
		{
			if (location.ref_orbit != null)
			{
				return location.ref_orbit.solarMultiplier;
			}
			if (location.ref_habSite != null)
			{
				return location.ref_habSite.solarMultiplier;
			}
			if (location.ref_spaceBody != null)
			{
				return location.ref_spaceBody.solarMultiplier;
			}
			return 0f;
		}

		// Token: 0x06003F59 RID: 16217 RVA: 0x00197C00 File Offset: 0x00195E00
		public static float AtmosphereSolarModifier(TIGameState location)
		{
			switch (location.ref_spaceBody.atmosphere)
			{
			case Atmosphere.Thin:
				return 0.75f;
			case Atmosphere.Standard:
				return 0.5f;
			case Atmosphere.Thick:
				return 0.25f;
			case Atmosphere.Massive:
				return 0f;
			default:
				return 1f;
			}
		}

		// Token: 0x06003F5A RID: 16218 RVA: 0x00197C50 File Offset: 0x00195E50
		public static float SetLocationSolarPowerMultiplier(TIGameState location)
		{
			float num = 1f;
			double semiMajorAxis_AU = location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
			float num2;
			if (location.isHabSiteState || location.isSpaceBodyState || (location.isHabState && location.ref_hab.IsBase))
			{
				switch (location.ref_spaceBody.objectType)
				{
				case SpaceObjectType.Star:
					return 1f;
				case SpaceObjectType.Asteroid:
				case SpaceObjectType.AsteroidalMoon:
					num2 = 0.6f;
					goto IL_0390;
				case SpaceObjectType.Comet:
					num2 = 0.6f;
					num = 0.5f;
					goto IL_0390;
				}
				if (location.isHabSiteState && location.ref_spaceBody.barycenter.isSun && location.ref_spaceBody.tilt_Deg < 5f && Mathf.Abs(location.ref_habSite.latitude) > 85f)
				{
					num2 = 0.5f + Mathf.Abs(location.ref_habSite.latitude) / 360f;
				}
				else
				{
					num2 = 0.5f;
				}
				num = TIHabModuleState.AtmosphereSolarModifier(location);
			}
			else if (location.ref_naturalSpaceObject.isLagrangePointState)
			{
				TILagrangePointState ref_lagrangePoint = location.ref_lagrangePoint;
				TISpaceBodyState secondaryObject = ref_lagrangePoint.secondaryObject;
				if (ref_lagrangePoint.lagrangeValue == LagrangeValue.L2 && secondaryObject.barycenter.isSun)
				{
					double num3 = secondaryObject.semiMajorAxis_km * (secondaryObject.meanRadius_km * 2.0) / (secondaryObject.barycenter.meanRadius_km * 2.0);
					double num4 = secondaryObject.semiMajorAxis_km * (1.0 - secondaryObject.ecc) * Mathd.Pow(secondaryObject.mass_kg / (3.0 * secondaryObject.barycenter.mass_kg), 0.3333333333333333);
					double num5 = secondaryObject.semiMajorAxis_km * Mathd.Pow(secondaryObject.mass_kg / (3.0 * secondaryObject.barycenter.mass_kg), 0.3333333333333333);
					double num6 = secondaryObject.semiMajorAxis_km * (1.0 + secondaryObject.ecc) * Mathd.Pow(secondaryObject.mass_kg / (3.0 * secondaryObject.barycenter.mass_kg), 0.3333333333333333);
					if (num4 > num3)
					{
						num2 = 1f;
					}
					else
					{
						double semiMajorAxis_km = location.ref_orbit.semiMajorAxis_km;
						double num7 = num4 * (secondaryObject.meanRadius_km / num3);
						if (semiMajorAxis_km > num7)
						{
							num2 = 1f;
						}
						else if (num6 < num3)
						{
							num2 = 0.05f;
						}
						else
						{
							num2 = (float)(num5 / num6);
						}
					}
				}
				else
				{
					num2 = 1f;
				}
			}
			else
			{
				TIOrbitState ref_orbit = location.ref_orbit;
				TINaturalSpaceObjectState barycenter = ref_orbit.barycenter;
				double meanRadius_km = barycenter.meanRadius_km;
				double semiMajorAxis_km2 = ref_orbit.semiMajorAxis_km;
				num2 = 1f - (float)(Mathd.Atan(meanRadius_km / semiMajorAxis_km2) / 3.141592653589793);
				if (barycenter.isaMoon && barycenter.inclination_Rad * 57.295780181884766 + (double)barycenter.barycenter.ref_spaceBody.tilt_Deg < 5.0 && barycenter.barycenter.semiMajorAxis_km * (barycenter.barycenter.meanRadius_km * 2.0) / (barycenter.barycenter.barycenter.meanRadius_km * 2.0) > barycenter.semiMajorAxis_km)
				{
					double meanRadius_km2 = barycenter.barycenter.meanRadius_km;
					double semiMajorAxis_km3 = barycenter.semiMajorAxis_km;
					num2 *= 1f - (float)(Mathd.Atan(meanRadius_km2 / semiMajorAxis_km3) / 3.141592653589793);
				}
			}
			IL_0390:
			return (float)((double)(num * num2) / (semiMajorAxis_AU * semiMajorAxis_AU));
		}

		// Token: 0x06003F5B RID: 16219 RVA: 0x00197FF6 File Offset: 0x001961F6
		public static int EscapeVelocityBasedPowerRequirement(TISpaceBodyState body, TIHabModuleTemplate moduleTemplate, TIFactionState faction)
		{
			return (int)((float)moduleTemplate.power / 2f + Mathf.Round((float)moduleTemplate.power / 2f * (float)body.relativeEnergyForMining(faction)));
		}

		// Token: 0x06003F5C RID: 16220 RVA: 0x00198022 File Offset: 0x00196222
		public static int EscapeVelocityBasedPowerRequirement(TIHabSiteState site, TIHabModuleTemplate moduleTemplate, TIFactionState faction)
		{
			return TIHabModuleState.EscapeVelocityBasedPowerRequirement(site.parentBody, moduleTemplate, faction);
		}

		// Token: 0x06003F5D RID: 16221 RVA: 0x00198031 File Offset: 0x00196231
		public static int EscapeVelocityBasedPowerRequirement(TIHabState hab, TIHabModuleTemplate moduleTemplate, TIFactionState faction)
		{
			if (hab.IsBase)
			{
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(hab.habSite, moduleTemplate, faction);
			}
			return moduleTemplate.power;
		}

		// Token: 0x06003F5E RID: 16222 RVA: 0x00198050 File Offset: 0x00196250
		public void DestroyModule()
		{
			TIHabState hab = this.hab;
			TIFactionState tifactionState = ((hab != null) ? hab.faction : null);
			if (tifactionState != null && tifactionState.AISavingTarget.active)
			{
				TIGameState location = tifactionState.AISavingTarget.location;
				if (((location != null) ? location.ref_habModule : null) == this)
				{
					tifactionState.AIClearSavingTarget("Hab module destroyed");
				}
			}
			if (this.moduleTemplate.allowsShipConstruction)
			{
				this.sector.faction.RemoveShipyardFromFaction(this, false);
			}
			if (this.underConstruction)
			{
				this.completionDate = TITimeState.SystemNow();
			}
			this.SetPowerStatus(false, false);
			this.destroyed = true;
			this.hab.SetModulesDirty();
			this.constructionCompleted = true;
			if (this.moduleTemplate.coreModule)
			{
				this.hab.anyCoreCompleted = false;
			}
			int num = TIUtilities.RandomRange(1, 3);
			string text = new StringBuilder(this.moduleTemplate.alienModule ? "AlienDestroyedModule" : "DestroyedModule").Append(this.moduleTemplate.tier.ToString()).Append(num.ToString()).ToString();
			this.SetModuleTemplate(text);
			this.hab.ValidateLocalPopulationRequirementsForAllNearbyHabs();
			this.SetPowerStatus(true, false);
		}

		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x06003F5F RID: 16223 RVA: 0x00198183 File Offset: 0x00196383
		public int controlPointCapacity
		{
			get
			{
				return this.moduleTemplate.ControlPointCapacity(this.hab.inEarthLEO);
			}
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x06003F60 RID: 16224 RVA: 0x0019819C File Offset: 0x0019639C
		public bool buildingShip
		{
			get
			{
				return this.active && this.moduleTemplate.allowsShipConstruction && this.hab.faction.nShipyardQueues.ContainsKey(this) && this.hab.faction.nShipyardQueues[this].Count > 0 && this.hab.faction.nShipyardQueues[this][0].costPaid;
			}
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x06003F61 RID: 16225 RVA: 0x00198218 File Offset: 0x00196418
		public ShipConstructionQueueItem currentShipConstructionQueueItem
		{
			get
			{
				if (!this.active || !this.moduleTemplate.allowsShipConstruction || this.hab.faction.nShipyardQueues[this].Count <= 0)
				{
					return null;
				}
				return this.hab.faction.nShipyardQueues[this][0];
			}
		}

		// Token: 0x06003F62 RID: 16226 RVA: 0x00198276 File Offset: 0x00196476
		public void SetPrimaryAbilityCooldown(int daysFromNow)
		{
			this.abilityCooldownEnds = TITimeState.Now();
			this.abilityCooldownEnds.AddDays((float)daysFromNow);
		}

		// Token: 0x06003F63 RID: 16227 RVA: 0x00198290 File Offset: 0x00196490
		public bool PrimaryAbilityOnCooldown()
		{
			return this.abilityCooldownEnds != null && this.abilityCooldownEnds > TITimeState.Now();
		}

		// Token: 0x06003F64 RID: 16228 RVA: 0x001982B4 File Offset: 0x001964B4
		public bool CanDecommissionModule(bool immediateCancel)
		{
			return this.okay && !this.hab.IsAlien() && !this.hab.underBombardment && !this.hab.underAssault && !this.decommissioning && this != this.hab.coreModule && (!this.PowerProvider() || this.hab.NetPower(false, false) >= this.ModulePower() || this.underConstruction || !this.powered) && (!immediateCancel || this.priorModuleTemplate == null || this.hab.IsModuleAllowedForThisHab(this.ref_faction, this.priorModuleTemplate, false));
		}

		// Token: 0x06003F65 RID: 16229 RVA: 0x00198368 File Offset: 0x00196568
		public TIResourcesCost DecommissionModuleCost()
		{
			float num = this.DecommissionDuration_days();
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (num > 0f)
			{
				TIResourcesCost tiresourcesCost2 = tiresourcesCost;
				FactionResource factionResource = FactionResource.Boost;
				TIOrbitState ref_orbit = this.ref_orbit;
				tiresourcesCost2.AddCost(factionResource, (ref_orbit != null && ref_orbit.isEarthLEO) ? 0.1f : ((float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(this.ref_faction, this.hab.IsBase ? this.hab.ref_habSite.ref_gameState : this.hab.ref_orbit.ref_gameState, (float)this.crew * TemplateManager.global.scuttlePerCrewMassCost)), true);
				tiresourcesCost.SetCompletionTime_Days(this.DecommissionDuration_days());
			}
			return tiresourcesCost;
		}

		// Token: 0x06003F66 RID: 16230 RVA: 0x00198408 File Offset: 0x00196608
		public TIResourcesCost DecomissionModuleResourceRefund()
		{
			return this.moduleTemplate.BuildMaterials(this.hab.irradiatedMultiplier, this.ref_spaceBody, this.ref_naturalSpaceObject, this.hab.faction, TemplateManager.global.decomissionModuleRefundRate).ToResourcesCost(1f);
		}

		// Token: 0x06003F67 RID: 16231 RVA: 0x0019845C File Offset: 0x0019665C
		public void BeginDecomissionModule()
		{
			if (this.moduleTemplate.allowsShipConstruction)
			{
				this.sector.faction.RemoveShipyardFromFaction(this, true);
			}
			this.SetPowerStatus(false, false);
			if (this.DecommissionDuration_days() <= 0f)
			{
				this.hab.CompleteDecommissionModule(this, false);
				TIResourcesCost tiresourcesCost = this.buildCost;
				if (tiresourcesCost != null)
				{
					tiresourcesCost.RefundCost(this.sector.faction, "Decomission Refund");
				}
				if (this.priorModuleTemplate != null)
				{
					if (!this.priorModuleCompleted)
					{
						TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
						tiresourcesCost2.SetCompletionTime_Days((float)this.priorModuleCompletionDate.DifferenceInDays(TITimeState.Now()));
						this.hab.InitiateModuleConstruction(this.sector, this.slot, this.priorModuleTemplate, tiresourcesCost2);
						return;
					}
					this.SetCompletedModule(this.priorModuleTemplate.dataName, false);
					this.SetPowerStatus(this.priorModuleTemplate.PowerFirst, false);
					this.hab.UpdatePowerManagement(false, this, this.ref_faction.player.isAI);
					this.hab.UpdateAllModuleConnectors();
				}
			}
			else
			{
				this.DecommissionModuleCost().PayCost(this.sector.faction, "Decommission");
				TIDateTime tidateTime = TITimeState.Now();
				tidateTime.AddDays(this.DecommissionDuration_days());
				this.decommissionDate = tidateTime.ExportTime();
				this.decommissioning = true;
			}
			this.hab.ValidateLocalPopulationRequirementsForAllNearbyHabs();
			this.constructionCompleted = true;
		}

		// Token: 0x06003F68 RID: 16232 RVA: 0x001985C0 File Offset: 0x001967C0
		public float DecommissionDuration_days()
		{
			TIHabModuleTemplate tihabModuleTemplate = null;
			if (this.moduleTemplate.coreModule || this.constructionCompleted)
			{
				tihabModuleTemplate = this.moduleTemplate;
			}
			else if (this.underConstruction)
			{
				if (this.startBuildDate >= GameStateManager.Time().Time_SystemNow())
				{
					return 0f;
				}
				if (this.priorModuleTemplate == null)
				{
					tihabModuleTemplate = this.moduleTemplate;
				}
				else
				{
					tihabModuleTemplate = this.priorModuleTemplate;
				}
			}
			float num = (float)tihabModuleTemplate.tier * 60f;
			if (this.moduleTemplate.coreModule)
			{
				num += 5f;
			}
			return num;
		}

		// Token: 0x06003F69 RID: 16233 RVA: 0x0019864E File Offset: 0x0019684E
		public void CancelDecommissionModule()
		{
			this.decommissioning = false;
			if (this.moduleTemplate.allowsShipConstruction)
			{
				this.sector.faction.AddShipyardToFaction(this, false);
			}
		}

		// Token: 0x06003F6A RID: 16234 RVA: 0x00198676 File Offset: 0x00196876
		public void CompleteDecommissionModule(bool clearPriorModule)
		{
			if (!this.moduleTemplate.coreModule)
			{
				this.decommissioning = false;
				TINotificationQueueState.LogDecommissionModuleComplete(this);
				this.templateName = string.Empty;
				this.moduleTemplate = null;
				if (clearPriorModule)
				{
					this.priorModuleTemplateName = string.Empty;
				}
			}
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06003F6B RID: 16235 RVA: 0x001986B4 File Offset: 0x001968B4
		public string baseSTOFireStr
		{
			get
			{
				return new StringBuilder("STOFireMission").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06003F6C RID: 16236 RVA: 0x001986E9 File Offset: 0x001968E9
		public void InitializeForBombardment()
		{
			this.SetSpaceCombatWeapons(this.ref_faction);
		}

		// Token: 0x06003F6D RID: 16237 RVA: 0x001986F8 File Offset: 0x001968F8
		public static TISpaceShipState SelectSTOTarget(TIGameState shooter, TIDateTime time, TISpaceFleetState targetFleet = null)
		{
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			if (targetFleet == null)
			{
				using (List<TISpaceFleetState>.Enumerator enumerator = shooter.ref_spaceBody.fleetsInOrbit.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceFleetState tispaceFleetState = enumerator.Current;
						if (tispaceFleetState.bombardmentTarget != null && tispaceFleetState.bombardmentTarget.ref_hab == shooter.ref_hab && shooter.ref_hab.CheckLOSToOrbitalTarget(tispaceFleetState, time))
						{
							list.AddRange(tispaceFleetState.ships);
						}
					}
					goto IL_00AF;
				}
			}
			list.AddRange(targetFleet.ships);
			IL_00AF:
			Dictionary<TISpaceShipState, float> targetDict = list.ToDictionary<TISpaceShipState, TISpaceShipState, float>((TISpaceShipState x) => x, (TISpaceShipState x) => x.BombardmentValue(shooter.ref_spaceBody));
			if (list.Count > 0)
			{
				if (targetDict.Values.Any<float>((float x) => x > 0f))
				{
					return list.SelectRandomWeightedItem<TISpaceShipState>((TISpaceShipState x) => targetDict[x], -1f, 1E-37f);
				}
			}
			return null;
		}

		// Token: 0x06003F6E RID: 16238 RVA: 0x00198854 File Offset: 0x00196A54
		public bool OnFireMissionOrder(TISpaceShipState target, TIDateTime time)
		{
			if (target == null)
			{
				target = TIHabModuleState.SelectSTOTarget(this, time, null);
			}
			if (!TIGameState.Valid(target) || !TIGameState.Valid(target.ref_fleet))
			{
				return false;
			}
			if (this.antiBombardmentWeapon == null)
			{
				ModuleDataEntry moduleDataEntry = new ModuleDataEntry(this.defenseWeapon.ref_laserWeapon, 0);
				this.antiBombardmentWeapon = new BeamWeapon(this, moduleDataEntry);
			}
			if (TIUtilities.RandomFloatValue() < 1f + this.TargetingBonus(this.antiBombardmentWeapon.weaponTemplate, this.hab) - target.ECMValue(this.ref_faction, null))
			{
				if (this.ref_naturalSpaceObject != null && this.ref_naturalSpaceObject.controller != null && this.ref_naturalSpaceObject.controller.modelLink != null && this.ref_naturalSpaceObject.controller.modelLink.activeInHierarchy)
				{
					this.hab.habSite.GetController().DisplayBeam(this, target, time);
				}
				if (target.visualizerLink == null || target.visualizerLink.transform == null || target.visualizerLink.transform.parent == null)
				{
					return false;
				}
				StrategyShipController component = target.visualizerLink.transform.parent.GetComponent<StrategyShipController>();
				if (component == null)
				{
					return false;
				}
				this.antiBombardmentWeapon.SetTarget_Strategy(component, component.ShipState.globalPositionAtTime(time));
				BeamWeapon.Beam damageSource = this.antiBombardmentWeapon.GetDamageSource(this, target.fleet.bombardmentAltitude_km);
				float num = component.ApplyDamage(damageSource);
				if (component.ShipState.ShipDestroyed())
				{
					component.ShipState.fleet.AddToBombardmentLog(Loc.T("Bombard.Log.CounterfireKill", new object[]
					{
						time.ToCustomTimeString(),
						this.displayName,
						this.antiBombardmentWeapon.weaponTemplate.displayName,
						component.ShipState.displayName,
						TIUtilities.FormatBigOrSmallNumber(damageSource.damage.amount, 1, 7, 0, false, false),
						TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false),
						this.hab.displayName
					}), time);
					TINotificationQueueState.LogShipDestroyedInStrat(component.ShipState, this.hab.habSite.hab.ref_factions, component.ShipState.fleet.location, new Dictionary<TIFactionState, string> { 
					{
						component.ShipState.faction,
						component.ShipState.KillAllOfficersReport()
					} });
					TISpaceShipState shipState = component.ShipState;
					bool flag = true;
					CombatWeaponCarrierState attacker = damageSource.attacker;
					shipState.DestroyShip(flag, (attacker != null) ? attacker.GetFaction() : null);
				}
				else if (num > 0f)
				{
					component.ShipState.fleet.AddToBombardmentLog(Loc.T("Bombard.Log.CounterfireHit", new object[]
					{
						time.ToCustomTimeString(),
						this.displayName,
						this.antiBombardmentWeapon.weaponTemplate.displayName,
						component.ShipState.displayName,
						TIUtilities.FormatBigOrSmallNumber(damageSource.damage.amount, 1, 7, 0, false, false),
						TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false),
						this.hab.displayName
					}), time);
				}
				else
				{
					component.ShipState.fleet.AddToBombardmentLog(Loc.T("Bombard.Log.CounterfireAbsorbed", new object[]
					{
						time.ToCustomTimeString(),
						this.displayName,
						this.antiBombardmentWeapon.weaponTemplate.displayName,
						component.ShipState.displayName,
						TIUtilities.FormatBigOrSmallNumber(damageSource.damage.amount, 1, 7, 0, false, false),
						this.hab.displayName
					}), time);
				}
			}
			else
			{
				target.ref_fleet.AddToBombardmentLog(Loc.T("Bombard.Log.ECMPreventedCounterfire", new object[]
				{
					time.ToCustomTimeString(),
					target.displayName
				}), time);
			}
			return true;
		}

		// Token: 0x06003F6F RID: 16239 RVA: 0x00198C3E File Offset: 0x00196E3E
		public TIGameState GetTargetableState()
		{
			return this;
		}

		// Token: 0x06003F70 RID: 16240 RVA: 0x00198C41 File Offset: 0x00196E41
		public void AddTargetedProjectile(TISpaceCombatProjectileState projectile)
		{
			projectile.EnemyTargetsMe(this);
		}

		// Token: 0x06003F71 RID: 16241 RVA: 0x00198C4A File Offset: 0x00196E4A
		public TIFactionState GetFaction()
		{
			return this.ref_faction;
		}

		// Token: 0x06003F72 RID: 16242 RVA: 0x00198C52 File Offset: 0x00196E52
		public float FireControlFunction()
		{
			return 1f;
		}

		// Token: 0x06003F73 RID: 16243 RVA: 0x00198C59 File Offset: 0x00196E59
		public bool WeaponIsOperable(ModuleDataEntry moduleData)
		{
			return this.isCombatModule;
		}

		// Token: 0x06003F74 RID: 16244 RVA: 0x00198C61 File Offset: 0x00196E61
		public bool WeaponCanFire(ModuleDataEntry moduleData)
		{
			return this.WeaponIsOperable(moduleData);
		}

		// Token: 0x06003F75 RID: 16245 RVA: 0x00198C6A File Offset: 0x00196E6A
		public void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile)
		{
			if (targetedProjectile != null)
			{
				this.AddTargetedProjectile(targetedProjectile);
			}
		}

		// Token: 0x04002728 RID: 10024
		public bool C0;

		// Token: 0x04002729 RID: 10025
		public bool N1;

		// Token: 0x0400272A RID: 10026
		public bool N2;

		// Token: 0x0400272B RID: 10027
		public bool E1;

		// Token: 0x0400272C RID: 10028
		public bool E2;

		// Token: 0x0400272D RID: 10029
		public bool W1;

		// Token: 0x0400272E RID: 10030
		public bool W2;

		// Token: 0x0400272F RID: 10031
		public bool S1;

		// Token: 0x04002730 RID: 10032
		public bool S2;

		// Token: 0x04002739 RID: 10041
		public TIResourcesCost buildCost;

		// Token: 0x0400273A RID: 10042
		public bool shipyardAllowPayFromEarth;

		// Token: 0x0400273B RID: 10043
		[Obsolete]
		[SerializeField]
		private TIDateTime lastTimeFiredAtShip;

		// Token: 0x0400273D RID: 10045
		private TIHabModuleTemplate _priorModuleTemplate;

		// Token: 0x04002741 RID: 10049
		public float baseBuildDuration_days = -1f;

		// Token: 0x04002742 RID: 10050
		public float appliedBuildConstructionBonus = 1f;

		// Token: 0x04002743 RID: 10051
		public DateTime startBuildDate;

		// Token: 0x04002745 RID: 10053
		public const int MAX_SOLAR_POWER_MULTIPLIER = 8;

		// Token: 0x04002746 RID: 10054
		public const float COMET_SOLAR_POWER_MODIFIER = 0.5f;

		// Token: 0x04002747 RID: 10055
		private BeamWeapon antiBombardmentWeapon;

		// Token: 0x04002748 RID: 10056
		public TIDateTime destroyedTime;
	}
}
