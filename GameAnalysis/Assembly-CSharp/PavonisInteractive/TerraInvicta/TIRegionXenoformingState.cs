using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200077A RID: 1914
	public class TIRegionXenoformingState : TIRegionAlienAssetState
	{
		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06003B27 RID: 15143 RVA: 0x0015D206 File Offset: 0x0015B406
		// (set) Token: 0x06003B28 RID: 15144 RVA: 0x0015D20E File Offset: 0x0015B40E
		public float xenoformingLevel { get; protected set; }

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06003B29 RID: 15145 RVA: 0x0015D217 File Offset: 0x0015B417
		public override bool isRegionXenoformingState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x06003B2A RID: 15146 RVA: 0x0015D21A File Offset: 0x0015B41A
		public override TIRegionXenoformingState ref_xenoforming
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06003B2B RID: 15147 RVA: 0x0015D220 File Offset: 0x0015B420
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
				this.xenoformingLevel = 0f;
				this.gameStateSubjectCreated = true;
				if (TemplateManager.global.debug_advancedFactionStart)
				{
					if (region.templateName == "Texas")
					{
						this.xenoformingLevel = 30f;
					}
					if (region.templateName == "MexicoCity")
					{
						this.xenoformingLevel = TIRegionXenoformingState.autodetectThreshold;
					}
					if (region.templateName == "RockyMountains")
					{
						this.xenoformingLevel = 55f;
					}
					if (region.templateName == "Monterrey")
					{
						this.xenoformingLevel = 99.5f;
					}
					if (region.templateName == "Texas")
					{
						this.xenoformingLevel = 99.9f;
					}
				}
			}
		}

		// Token: 0x06003B2C RID: 15148 RVA: 0x0015D30C File Offset: 0x0015B50C
		public override void PostInitializationInit_4()
		{
			this.spreadToAdjacentThreshold = (int)Mathf.Clamp(Mathf.Pow(base.region.area_km2, 0.7f) / 250f, 50f, 70f);
			if (base.region.terrain == TerrainType.Rugged)
			{
				this.spreadToAdjacentThreshold += 10;
			}
		}

		// Token: 0x06003B2D RID: 15149 RVA: 0x0015D367 File Offset: 0x0015B567
		public override string GetIconResourcePath(TIFactionState faction)
		{
			if (this.xenoformingLevel < TIRegionXenoformingState.stage2Xenoforming)
			{
				return TemplateManager.global.pathGeoscapeXenoform1;
			}
			if (this.xenoformingLevel < TIRegionXenoformingState.stage3Xenoforming)
			{
				return TemplateManager.global.pathGeoscapeXenoform2;
			}
			return TemplateManager.global.pathGeoscapeXenoform3;
		}

		// Token: 0x06003B2E RID: 15150 RVA: 0x0015D3A3 File Offset: 0x0015B5A3
		public override string GetIllustrationPath(TIFactionState faction)
		{
			if (this.xenoformingLevel < TIRegionXenoformingState.stage2Xenoforming)
			{
				return TemplateManager.global.illus_xenoformingStage1;
			}
			if (this.xenoformingLevel < TIRegionXenoformingState.stage3Xenoforming)
			{
				return TemplateManager.global.illus_xenoformingStage2;
			}
			return TemplateManager.global.illus_xenoformingStage3;
		}

		// Token: 0x06003B2F RID: 15151 RVA: 0x0015D3DF File Offset: 0x0015B5DF
		public override string GetDestroyedIllustrationPath()
		{
			return TemplateManager.global.illus_assaultXenoforming;
		}

		// Token: 0x06003B30 RID: 15152 RVA: 0x0015D3EB File Offset: 0x0015B5EB
		public void ChangeXenoformingLevel(float byValue)
		{
			if (byValue != 0f)
			{
				this.xenoformingLevel += byValue;
				this.xenoformingLevel = Mathf.Max(0f, this.xenoformingLevel);
				this.UpdateIntel(byValue < 0f);
			}
		}

		// Token: 0x06003B31 RID: 15153 RVA: 0x0015D428 File Offset: 0x0015B628
		public void SetXenoformingLevel(float toValue)
		{
			float xenoformingLevel = this.xenoformingLevel;
			this.xenoformingLevel = toValue;
			this.xenoformingLevel = Mathf.Max(0f, this.xenoformingLevel);
			if (xenoformingLevel != this.xenoformingLevel)
			{
				this.UpdateIntel(toValue < xenoformingLevel);
			}
		}

		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x06003B32 RID: 15154 RVA: 0x0015D46C File Offset: 0x0015B66C
		public string severityDescription
		{
			get
			{
				if (this.xenoformingLevel < TIRegionXenoformingState.stage2Xenoforming)
				{
					return Loc.T("TIRegionXenoformingState.description_light");
				}
				if (this.xenoformingLevel < TIRegionXenoformingState.stage3Xenoforming)
				{
					return Loc.T("TIRegionXenoformingState.description_heavy");
				}
				return Loc.T("TIRegionXenoformingState.description_severe");
			}
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x0015D4A8 File Offset: 0x0015B6A8
		public override bool Extant()
		{
			return this.xenoformingLevel > 0f;
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x0015D4B8 File Offset: 0x0015B6B8
		public void UpdateIntel(bool allowVanish)
		{
			if (!this.Extant())
			{
				TIFactionState[] array = GameStateManager.AllFactions();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ExpireIntel(this, true);
				}
				return;
			}
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState.IsAlienFaction)
				{
					tifactionState.SetIntel(this, 1f, this, false);
				}
				else
				{
					bool flag = !this.VisibleToFaction(tifactionState);
					if (allowVanish)
					{
						tifactionState.SetIntel(this, this.xenoformingLevel / 100f, this, false);
					}
					else
					{
						tifactionState.SetIntelIfValueHigher(this, this.xenoformingLevel / 100f, this);
					}
					if (flag && this.VisibleToFaction(tifactionState))
					{
						this.SightedByFaction(tifactionState, true);
					}
				}
			}
		}

		// Token: 0x06003B35 RID: 15157 RVA: 0x0015D568 File Offset: 0x0015B768
		public void DailyXenoformingGrowth()
		{
			if (this.xenoformingLevel >= TIRegionXenoformingState.spawnArmyValue)
			{
				List<TIMegafaunaArmyState> existingArmies = base.region.MegafaunaArmiesPresent().ConvertAll<TIMegafaunaArmyState>((TIArmyState x) => x.ref_megafaunaArmyState);
				foreach (TIMegafaunaArmyState timegafaunaArmyState in existingArmies)
				{
					if (this.xenoformingLevel > 0f)
					{
						this.xenoformingLevel -= timegafaunaArmyState.AttemptRepair(this.xenoformingLevel) * 0.5f;
					}
				}
				if (this.xenoformingLevel >= TIRegionXenoformingState.spawnArmyValue)
				{
					if (existingArmies.Count == 0)
					{
						this.SpawnMegafaunaArmy();
						this.ChangeXenoformingLevel(TIRegionXenoformingState.megafaunaSpawnCost);
					}
					else
					{
						existingArmies.ForEach(delegate(TIMegafaunaArmyState x)
						{
							x.bonusTechLevel += 0.1f / (float)existingArmies.Count;
						});
						if (!base.region.nation.alienNation)
						{
							base.region.ApplyDamageToRegion(TIUtilities.RandomRange(0.01f, 0.02f), null, null, false, false, true, false);
						}
						this.ChangeXenoformingLevel(TIRegionXenoformingState.megafaunaSpawnCost);
					}
				}
				foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
				{
					bool flag = !this.VisibleToFaction(tifactionState);
					tifactionState.SetIntel(this, 1f, this, false);
					if (flag && this.VisibleToFaction(tifactionState))
					{
						this.SightedByFaction(tifactionState, true);
					}
				}
				return;
			}
			if (TITimeState.Now().day % 7 == 0)
			{
				List<TIMegafaunaArmyState> list = base.region.MegafaunaArmiesPresent().ConvertAll<TIMegafaunaArmyState>((TIArmyState x) => x.ref_megafaunaArmyState);
				if (list.Count > 1)
				{
					list = list.OrderBy<TIMegafaunaArmyState, float>((TIMegafaunaArmyState x) => x.techLevel).ToList<TIMegafaunaArmyState>();
					TIMegafaunaArmyState timegafaunaArmyState2 = list.Last<TIMegafaunaArmyState>();
					list.Remove(list.Last<TIMegafaunaArmyState>());
					foreach (TIMegafaunaArmyState timegafaunaArmyState3 in list.ToList<TIMegafaunaArmyState>())
					{
						timegafaunaArmyState3.MergeWithOtherXenofauna(timegafaunaArmyState2);
					}
				}
			}
			if (this.Extant())
			{
				float num = 0.025f + TIUtilities.RandomFloatValue() / 25f;
				if (base.region.terrain == TerrainType.Rugged)
				{
					num /= 2f;
				}
				if (Mathf.Abs(base.region.latitude) > 50f)
				{
					num /= 2f;
				}
				if (base.region.hasAlienFacility)
				{
					num *= 2f;
				}
				num *= Mathf.Max(0.25f, GameStateManager.GlobalValues().earthAtmosphericCH4_ppm / 1.5f);
				num *= Mathf.Max(0.25f, GameStateManager.GlobalValues().earthAtmosphericCO2_ppm / 400f);
				this.ChangeXenoformingLevel(Mathf.Min(num, this.xenoformingLevel));
				if (this.xenoformingLevel >= (float)this.spreadToAdjacentThreshold && TIUtilities.RandomFloatValue() < 0.0005f * this.xenoformingLevel)
				{
					IEnumerable<TIRegionState> enumerable = from x in base.region.AdjacentRegions(false)
						where !x.xenoforming.Extant()
						select x;
					TIRegionState tiregionState = ((enumerable != null) ? enumerable.SelectRandomItem<TIRegionState>() : null);
					if (tiregionState == null)
					{
						return;
					}
					tiregionState.xenoforming.ChangeXenoformingLevel(TIUtilities.RandomFloatValue());
				}
			}
		}

		// Token: 0x06003B36 RID: 15158 RVA: 0x0015D900 File Offset: 0x0015BB00
		public void SpawnMegafaunaArmy()
		{
			TIMegafaunaArmyState timegafaunaArmyState = GameStateManager.CreateNewGameState<TIMegafaunaArmyState>();
			timegafaunaArmyState.SpawnArmy(base.region);
			TINotificationQueueState.LogAlienFaunaArmySpawned(timegafaunaArmyState);
			GameControl.eventManager.TriggerEvent(new NationDataUpdated(timegafaunaArmyState.currentNation), null, new object[] { timegafaunaArmyState.currentNation });
		}

		// Token: 0x06003B37 RID: 15159 RVA: 0x0015D94A File Offset: 0x0015BB4A
		public override bool VisibleToFaction(TIFactionState faction)
		{
			return this.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming || (this.xenoformingLevel > 0f && faction != null && faction.SufficientIntel(this, (TIRegionXenoformingState.autodetectThreshold - (float)faction.alienInvestigations) / 100f));
		}

		// Token: 0x06003B38 RID: 15160 RVA: 0x0015D989 File Offset: 0x0015BB89
		public void SightedByFaction(TIFactionState faction, bool triggerVisualUpdate)
		{
			if (triggerVisualUpdate)
			{
				GameControl.eventManager.TriggerEvent(new RegionXenoformingIntelUpdate(faction, base.region), null, new object[] { base.region });
			}
			TINotificationQueueState.LogXenoformingDetected(faction, this);
			faction.CompleteMilestone(CampaignMilestone.DetectXenoforming);
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x0015D9C3 File Offset: 0x0015BBC3
		public float AlienAttributeBonus()
		{
			return this.xenoformingLevel * TemplateManager.global.GetXenoformingAttributeBonusDifficultyScaling();
		}

		// Token: 0x06003B3A RID: 15162 RVA: 0x0015D9D6 File Offset: 0x0015BBD6
		public override float GetArmyAssaultDefenseScore()
		{
			return 2f + ((base.region.terrain == TerrainType.Rugged) ? 1f : 0f);
		}

		// Token: 0x06003B3B RID: 15163 RVA: 0x0015D9F8 File Offset: 0x0015BBF8
		public override string ResolveAssault(TIGameState assaultingState, TIFactionState assaultingfaction, TIMissionOutcome outcome)
		{
			float xenoformingLevel = this.xenoformingLevel;
			if (outcome >= TIMissionOutcome.Success)
			{
				if (assaultingState.isArmyState)
				{
					TIArmyState ref_army = assaultingState.ref_army;
					float num = ref_army.adjustedTechLevel * ref_army.strength;
					num += TIEffectsState.SumEffectsModifiers(Context.XenoformingDestructionStrength, assaultingState.ref_faction, num, null);
					float num2 = 1f - num / 10f;
					num2 *= ((outcome == TIMissionOutcome.CriticalSuccess) ? 0.5f : 1f);
					this.SetXenoformingLevel(this.xenoformingLevel * num2);
				}
				else if (assaultingState.isCouncilorState)
				{
					float num3 = -20f - TIUtilities.RandomFloatValue() * 20f;
					num3 += TIEffectsState.SumEffectsModifiers(Context.XenoformingDestructionStrength, assaultingState.ref_faction, num3, null);
					num3 *= (float)((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1);
					num3 /= TIMissionPhaseState.phasesPerMonth;
					this.ChangeXenoformingLevel(num3);
					if (outcome == TIMissionOutcome.CriticalSuccess)
					{
						base.region.ConductAbductions(this.ref_faction, -1);
					}
				}
			}
			this.UpdateIntel(true);
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { this });
			GameControl.eventManager.TriggerEvent(new TIGameStateAttacking(this), null, new object[] { assaultingState });
			GameControl.eventManager.TriggerEvent(new XenoformingDamaged(this), null, new object[] { this });
			if (this.xenoformingLevel > 0f)
			{
				float num4 = Mathf.Min(0.99f, -((this.xenoformingLevel - xenoformingLevel) / xenoformingLevel));
				return Loc.T("UI.Notifications.XenoformingChange", new object[] { num4.ToPercent("P0") });
			}
			GameControl.eventManager.TriggerEvent(new XenoformingDestroyed(this), null, new object[] { this });
			return Loc.T("UI.Notifications.XenoformingKilled");
		}

		// Token: 0x06003B3C RID: 15164 RVA: 0x0015DBA0 File Offset: 0x0015BDA0
		public override List<CampaignMilestone> CampaignMilestonesGrantedOnCapture(TIFactionState faction, TIMissionOutcome outcome)
		{
			return new List<CampaignMilestone> { CampaignMilestone.DestroyXenoforming };
		}

		// Token: 0x040025BA RID: 9658
		public static readonly float spawnArmyValue = 100f;

		// Token: 0x040025BB RID: 9659
		public static readonly float stage3Xenoforming = 75f;

		// Token: 0x040025BC RID: 9660
		public static readonly float stage2Xenoforming = 30f;

		// Token: 0x040025BD RID: 9661
		public static readonly float autodetectThreshold = 65f;

		// Token: 0x040025BE RID: 9662
		public static readonly float megafaunaSpawnCost = -50f;

		// Token: 0x040025BF RID: 9663
		private int spreadToAdjacentThreshold;
	}
}
