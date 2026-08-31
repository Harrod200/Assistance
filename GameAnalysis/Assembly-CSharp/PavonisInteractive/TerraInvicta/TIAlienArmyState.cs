using System;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000765 RID: 1893
	public class TIAlienArmyState : TIArmyState
	{
		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06003588 RID: 13704 RVA: 0x00134ADC File Offset: 0x00132CDC
		public override float techLevel
		{
			get
			{
				return Mathf.Min(TemplateManager.global.alienArmyTechCap, TemplateManager.global.alienArmyTechLevel + TemplateManager.global.alienArmyTechFromAbductions * (float)this.faction.abductions);
			}
		}

		// Token: 0x06003589 RID: 13705 RVA: 0x00134B0F File Offset: 0x00132D0F
		public override bool LegalRegion(TIRegionState region)
		{
			return !GameStateManager.AlienNation().extant || base.LegalRegion(region);
		}

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x0600358A RID: 13706 RVA: 0x00134B26 File Offset: 0x00132D26
		public override bool CanTakeOffensiveAction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x0600358B RID: 13707 RVA: 0x00134B29 File Offset: 0x00132D29
		public override float adjustedTechLevel
		{
			get
			{
				return this.techLevel;
			}
		}

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x0600358C RID: 13708 RVA: 0x00134B31 File Offset: 0x00132D31
		public override TINationState homeNation
		{
			get
			{
				return GameStateManager.AlienNation();
			}
		}

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x0600358D RID: 13709 RVA: 0x00134B38 File Offset: 0x00132D38
		public override float investmentArmyFactor
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x0600358E RID: 13710 RVA: 0x00134B3F File Offset: 0x00132D3F
		public override float investmentNavyFactor
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x0600358F RID: 13711 RVA: 0x00134B46 File Offset: 0x00132D46
		public override bool CanHeal()
		{
			return this.strength > 0f && this.strength < 1f && !this.InBattleWithArmies() && !this.OccupyingRegion(true) && base.CurrentOperations().Count == 0;
		}

		// Token: 0x06003590 RID: 13712 RVA: 0x00134B83 File Offset: 0x00132D83
		public override string GetModelResource()
		{
			return "3dearthmodels/hydra_walker";
		}

		// Token: 0x06003591 RID: 13713 RVA: 0x00134B8A File Offset: 0x00132D8A
		public override Sprite GetTransportIcon()
		{
			return AssetCacheManager.alienNavyTransportIcon;
		}

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x06003592 RID: 13714 RVA: 0x00134B91 File Offset: 0x00132D91
		public override string AnimatorResource
		{
			get
			{
				if (!base.UseAttackingVisuals)
				{
					return "Alien_army_def_animator";
				}
				return "Alien_army_att_animator";
			}
		}

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x06003593 RID: 13715 RVA: 0x00134BA6 File Offset: 0x00132DA6
		public override string FightingSpriteSheet
		{
			get
			{
				if (!base.UseAttackingVisuals)
				{
					return "SpriteSheet_Alien_army_def";
				}
				return "SpriteSheet_Alien_army_att";
			}
		}

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06003594 RID: 13716 RVA: 0x00134BBB File Offset: 0x00132DBB
		public override string MovingSpriteSheet
		{
			get
			{
				if (!base.UseAttackingVisuals)
				{
					return "SpriteSheet_Alien_army_def2";
				}
				return "SpriteSheet_Alien_army_att2";
			}
		}

		// Token: 0x06003595 RID: 13717 RVA: 0x00134BD0 File Offset: 0x00132DD0
		public override Sprite GetForegroundIcon()
		{
			if (!base.UseAttackingVisuals)
			{
				return AssetCacheManager.alienArmy_def;
			}
			return AssetCacheManager.alienArmy_att;
		}

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06003596 RID: 13718 RVA: 0x00134BE5 File Offset: 0x00132DE5
		public override string GetIconForegroundResource
		{
			get
			{
				if (!base.UseAttackingVisuals)
				{
					return TemplateManager.global.pathAlienArmy_defending;
				}
				return TemplateManager.global.pathAlienArmy_attacking;
			}
		}

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x06003597 RID: 13719 RVA: 0x00134C04 File Offset: 0x00132E04
		public override bool HumanArmy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x06003598 RID: 13720 RVA: 0x00134C07 File Offset: 0x00132E07
		public override bool AlienRegularArmy
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x06003599 RID: 13721 RVA: 0x00134C0A File Offset: 0x00132E0A
		public override TIAlienArmyState ref_alienArmyState
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x0600359A RID: 13722 RVA: 0x00134C0D File Offset: 0x00132E0D
		public override TIControlPoint ref_controlPoint
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600359B RID: 13723 RVA: 0x00134C10 File Offset: 0x00132E10
		public void SpawnArmy(TIRegionState startingRegion)
		{
			this.createdFromTemplate = false;
			this.deploymentType = DeploymentType.Naval;
			this.controlPointIdx = -1;
			base.currentRegion = startingRegion;
			this.homeRegion = startingRegion;
			base.NewArmy(ArmyType.AlienInvader, 0, 0.5f);
			this.spawning = true;
			base.MoveArmyToRegion(startingRegion, true);
			base.AssignToFaction(GameStateManager.AlienFaction(), false);
			GameStateManager.AlienNation().AddArmy(this);
			EventManager eventManager = GameControl.eventManager;
			GameEvent gameEvent = new ArmyMajorStatusUpdate(this);
			string text = null;
			object[] array = new object[4];
			array[0] = this;
			array[1] = base.currentRegion;
			array[2] = this.homeRegion;
			int num = 3;
			TIRegionState homeRegion = this.homeRegion;
			array[num] = ((homeRegion != null) ? homeRegion.nation : null);
			eventManager.TriggerEvent(gameEvent, text, (from x in array.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			GameControl.eventManager.TriggerEvent(new NationDataUpdated(base.currentNation), null, new object[] { base.currentNation });
			base.SetGameStateCreated();
		}

		// Token: 0x0600359C RID: 13724 RVA: 0x00134D0E File Offset: 0x00132F0E
		public override bool OccupyingRegion(bool includeLiberating = false)
		{
			return !this.spawning && base.OccupyingRegion(includeLiberating);
		}

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x0600359D RID: 13725 RVA: 0x00134D24 File Offset: 0x00132F24
		public override float dailyHealRate
		{
			get
			{
				float num;
				if (base.currentRegion.alienLanding.Extant())
				{
					num = 1f / (float)TemplateManager.global.daysToPrepareFullArmyFromUFO;
				}
				else
				{
					num = 0.01f;
					if (base.currentRegion.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming)
					{
						num += 0.005f;
					}
					if (base.currentRegion.hasAlienFacility)
					{
						num += 0.01f;
					}
					if (base.currentNation.alienNation)
					{
						num += 0.01f;
					}
					else
					{
						TIFactionState executiveFaction = base.currentNation.executiveFaction;
						if (executiveFaction != null && executiveFaction.IsAlienProxy)
						{
							num += 0.005f;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x04002411 RID: 9233
		public bool spawning;
	}
}
