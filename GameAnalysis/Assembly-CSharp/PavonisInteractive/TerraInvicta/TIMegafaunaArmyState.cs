using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200076A RID: 1898
	public class TIMegafaunaArmyState : TIArmyState
	{
		// Token: 0x060036C1 RID: 14017 RVA: 0x0013DE14 File Offset: 0x0013C014
		public void SpawnArmy(TIRegionState startingRegion)
		{
			this.createdFromTemplate = false;
			this.deploymentType = DeploymentType.Standard;
			this.controlPointIdx = -1;
			base.currentRegion = startingRegion;
			this.homeRegion = startingRegion;
			base.NewArmy(ArmyType.AlienMegafauna, 0, 1f);
			this.homeNation.AddArmy(this);
			base.AssignToFaction(GameStateManager.AlienFaction(), false);
			base.MoveArmyToRegion(startingRegion, true);
			if (TIUtilities.RandomFloatValue() < 0.5f)
			{
				TIRegionState armyDestination = TIArmyState.GetArmyDestination(this, AIArmyDestination.RandomAdjacentRegion, 4);
				if (armyDestination != null && armyDestination != base.currentRegion)
				{
					this.faction.playerControl.StartAction(new ConfirmOperationAction(this, armyDestination, new DeployArmyOperation_OpenTarget(false), null, null));
				}
			}
			TIFactionState[] array = GameStateManager.AllHumanFactions();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CompleteMilestone(CampaignMilestone.AlienMegafaunaSpawns);
			}
			base.SetGameStateCreated();
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x060036C2 RID: 14018 RVA: 0x0013DEE0 File Offset: 0x0013C0E0
		public override float techLevel
		{
			get
			{
				return Mathf.Min(2f + (float)GameStateManager.AlienFaction().abductions / 100f + this.bonusTechLevel, 6f + this.bonusTechLevel);
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x060036C3 RID: 14019 RVA: 0x0013DF14 File Offset: 0x0013C114
		public override bool InFriendlyRegion
		{
			get
			{
				if (this.faction.IsAlienFaction)
				{
					return base.currentNation.alienNation || (TIEffectsState.CheckForAnyEffectInContext(Context.MegafaunaRepellent, base.currentNation.executiveFaction) && base.GetEnemyArmiesInRegion().Count == 0);
				}
				using (List<TIArmyState>.Enumerator enumerator = base.currentRegion.FactionArmiesPresent(this.faction, true, true, true, false).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.homeNation.IsAtWarWith(base.currentNation))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x060036C4 RID: 14020 RVA: 0x0013DFC8 File Offset: 0x0013C1C8
		public override bool IsAttacking()
		{
			return base.InBattleWithArmies();
		}

		// Token: 0x060036C5 RID: 14021 RVA: 0x0013DFD0 File Offset: 0x0013C1D0
		public override bool LegalRegion(TIRegionState region)
		{
			return true;
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x060036C6 RID: 14022 RVA: 0x0013DFD3 File Offset: 0x0013C1D3
		public override bool CanTakeOffensiveAction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x060036C7 RID: 14023 RVA: 0x0013DFD6 File Offset: 0x0013C1D6
		public override float adjustedTechLevel
		{
			get
			{
				return this.techLevel;
			}
		}

		// Token: 0x060036C8 RID: 14024 RVA: 0x0013DFDE File Offset: 0x0013C1DE
		public override bool OccupyingRegion(bool includeLiberation)
		{
			return false;
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x060036C9 RID: 14025 RVA: 0x0013DFE1 File Offset: 0x0013C1E1
		public override TINationState homeNation
		{
			get
			{
				return GameStateManager.AlienNation();
			}
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x060036CA RID: 14026 RVA: 0x0013DFE8 File Offset: 0x0013C1E8
		public override float investmentArmyFactor
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x060036CB RID: 14027 RVA: 0x0013DFEF File Offset: 0x0013C1EF
		public override float investmentNavyFactor
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x060036CC RID: 14028 RVA: 0x0013DFF6 File Offset: 0x0013C1F6
		public override bool InBattleWithArmies()
		{
			return base.InBattleWithArmies() || !this.InFriendlyRegion;
		}

		// Token: 0x060036CD RID: 14029 RVA: 0x0013E00B File Offset: 0x0013C20B
		public override bool CanHeal()
		{
			return this.strength > 0f && this.strength < 1f && this.CanHealInRegion(base.currentRegion);
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x060036CE RID: 14030 RVA: 0x0013E035 File Offset: 0x0013C235
		public override string AnimatorResource
		{
			get
			{
				return "AlienFaunaArmyBase";
			}
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x060036CF RID: 14031 RVA: 0x0013E03C File Offset: 0x0013C23C
		public override string FightingSpriteSheet
		{
			get
			{
				return "SpriteSheet_Fauna_army_att_";
			}
		}

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x060036D0 RID: 14032 RVA: 0x0013E043 File Offset: 0x0013C243
		public override string MovingSpriteSheet
		{
			get
			{
				return "SpriteSheet_Fauna_army_def_";
			}
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x060036D1 RID: 14033 RVA: 0x0013E04A File Offset: 0x0013C24A
		public override string GetIconForegroundResource
		{
			get
			{
				return TemplateManager.global.pathAlienMegafaunaArmy;
			}
		}

		// Token: 0x060036D2 RID: 14034 RVA: 0x0013E056 File Offset: 0x0013C256
		public override Sprite GetForegroundIcon()
		{
			return AssetCacheManager.alienMegafaunaArmy;
		}

		// Token: 0x060036D3 RID: 14035 RVA: 0x0013E05D File Offset: 0x0013C25D
		public override string GetModelResource()
		{
			return "3dearthmodels/Hydra_Fauna";
		}

		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x060036D4 RID: 14036 RVA: 0x0013E064 File Offset: 0x0013C264
		public override bool HumanArmy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x060036D5 RID: 14037 RVA: 0x0013E067 File Offset: 0x0013C267
		public override bool AlienMegafaunaArmy
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x060036D6 RID: 14038 RVA: 0x0013E06A File Offset: 0x0013C26A
		public override TIMegafaunaArmyState ref_megafaunaArmyState
		{
			get
			{
				return this;
			}
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x060036D7 RID: 14039 RVA: 0x0013E06D File Offset: 0x0013C26D
		public override TIControlPoint ref_controlPoint
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060036D8 RID: 14040 RVA: 0x0013E070 File Offset: 0x0013C270
		public bool CanHealInRegion(TIRegionState region)
		{
			return (region.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming || base.currentNation.alienNation) && !base.InBattleWithArmies() && base.CurrentOperations().Count == 0;
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x060036D9 RID: 14041 RVA: 0x0013E0AC File Offset: 0x0013C2AC
		public override float dailyHealRate
		{
			get
			{
				return 1f / (60f * (float)Mathf.Max(1, base.currentRegion.armies.Count<TIArmyState>((TIArmyState x) => x.AlienMegafaunaArmy && x.strength < 1f)));
			}
		}

		// Token: 0x060036DA RID: 14042 RVA: 0x0013E0FC File Offset: 0x0013C2FC
		public void MergeWithOtherXenofauna(TIMegafaunaArmyState armyToMergeWith)
		{
			float num = armyToMergeWith.AttemptRepair(this.strength);
			armyToMergeWith.bonusTechLevel += (this.techLevel - 6f + 0.1f) * (1f - num);
			base.SetStrength(0f);
			base.Disband();
		}

		// Token: 0x060036DB RID: 14043 RVA: 0x0013E150 File Offset: 0x0013C350
		public bool AI_DesiredRegion(TIRegionState region)
		{
			if (!base.InBattleWithArmies())
			{
				bool flag = this.strength < 1f;
				bool flag2 = this.CanHealInRegion(region);
				if (flag && flag2)
				{
					return true;
				}
				if ((from x in region.MegafaunaArmiesPresent()
					where !x.IsMoving
					select x).Count<TIArmyState>() > 1)
				{
					return false;
				}
				if (region.nation.alienNation && this.faction.IsAlienFaction)
				{
					return region.nation.atWar && region.FilteredArmiesPresent(false, false, true, true, false).Count > 0;
				}
				if (TIEffectsState.CheckForAnyEffectInContext(Context.MegafaunaRepellent, base.currentRegion.nation.executiveFaction))
				{
					return false;
				}
				if (TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, base.currentRegion.nation.executiveFaction) && (GameStateManager.AlienFaction().councilors.Any<TICouncilorState>((TICouncilorState x) => x.location == region) || region.hasAlienFacility || TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, this.faction)))
				{
					return false;
				}
				if (region.regionalPerCapitaGDP < 1000.0 && region.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060036DC RID: 14044 RVA: 0x0013E2B8 File Offset: 0x0013C4B8
		public override void EngageLocalForcesAndOccupy(bool regionReturnFireOnly = false)
		{
			if (!regionReturnFireOnly)
			{
				float num = this.adjustedTechLevel * this.strength / 2000f;
				num += TIEffectsState.SumEffectsModifiers(Context.MegafaunaDamageMitigation, base.currentNation.executiveFaction, num, null);
				base.currentRegion.ApplyDamageToRegion(num, this.faction, null, false, false, false, false);
			}
			float num2 = base.LocalForcesBaseDefenseLevel(true, null);
			float combatSuccessChance = base.GetCombatSuccessChance(num2, this.adjustedTechLevel);
			if (TIUtilities.RandomFloatValue() < combatSuccessChance)
			{
				float num3 = num2 / 300f;
				num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoAllArmies, base.currentRegion.nation.executiveFaction, num3, null);
				num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoMegafauna, base.currentRegion.nation.executiveFaction, num3, null);
				base.TakeDamage(num3, base.currentRegion.ref_faction, null, false);
			}
		}

		// Token: 0x04002475 RID: 9333
		public float bonusTechLevel;

		// Token: 0x04002476 RID: 9334
		private const float BASE_MEGAFAUNA_TECH_LEVEL = 6f;

		// Token: 0x04002477 RID: 9335
		public const float MILTECH_BONUS_ON_MERGE = 0.1f;

		// Token: 0x04002478 RID: 9336
		private const float MEGAFAUNA_HEALING_FACTOR = 60f;
	}
}
