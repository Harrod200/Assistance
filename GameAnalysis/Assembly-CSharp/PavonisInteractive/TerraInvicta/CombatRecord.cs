using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B6 RID: 1974
	public struct CombatRecord
	{
		// Token: 0x17000CC6 RID: 3270
		// (get) Token: 0x0600434D RID: 17229 RVA: 0x001B4328 File Offset: 0x001B2528
		public TIHabState Hab
		{
			get
			{
				foreach (CombatRecord.SingleAssetCombatRecord singleAssetCombatRecord in this.singleAssetRecords)
				{
					if (singleAssetCombatRecord.assetName == this.habName)
					{
						return singleAssetCombatRecord.asset as TIHabState;
					}
				}
				return null;
			}
		}

		// Token: 0x0600434E RID: 17230 RVA: 0x001B4398 File Offset: 0x001B2598
		public void AddAssetSurvivedRecord(TIGameState asset, bool fled = false, SingleAssetCombatOutcome overrideOutcome = SingleAssetCombatOutcome.None)
		{
			List<CombatRecord.SingleAssetCombatRecord> list = this.singleAssetRecords;
			if (list != null && list.Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.asset == asset))
			{
				return;
			}
			string text = "";
			SingleAssetCombatOutcome singleAssetCombatOutcome = SingleAssetCombatOutcome.Undamaged;
			if (asset.isSpaceShipState)
			{
				text = asset.ref_ship.template.quickSummary(asset.ref_ship.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), asset.ref_ship, false, false, false);
				if (asset.ref_ship.ShipDestroyed())
				{
					singleAssetCombatOutcome = SingleAssetCombatOutcome.Destroyed;
				}
				else if (asset.ref_ship.damaged)
				{
					if (asset.ref_ship.MissionKilled())
					{
						singleAssetCombatOutcome = SingleAssetCombatOutcome.MissionKilled;
					}
					else
					{
						singleAssetCombatOutcome = SingleAssetCombatOutcome.Damaged;
					}
				}
			}
			if (asset.isHabState)
			{
				SingleAssetCombatOutcome singleAssetCombatOutcome2;
				if (asset.ref_hab.SpaceCombatValue() <= 0f)
				{
					singleAssetCombatOutcome2 = (asset.ref_hab.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.destroyed) ? SingleAssetCombatOutcome.HabDisabled : SingleAssetCombatOutcome.HabNoncombatant);
				}
				else
				{
					singleAssetCombatOutcome2 = SingleAssetCombatOutcome.Undamaged;
				}
				singleAssetCombatOutcome = singleAssetCombatOutcome2;
				text = asset.ref_hab.GetLocalizedHabModuleList();
			}
			if (overrideOutcome != SingleAssetCombatOutcome.None)
			{
				singleAssetCombatOutcome = overrideOutcome;
			}
			if (this.singleAssetRecords == null)
			{
				this.singleAssetRecords = new List<CombatRecord.SingleAssetCombatRecord>();
			}
			this.singleAssetRecords.RemoveAll((CombatRecord.SingleAssetCombatRecord x) => x.asset == asset);
			this.singleAssetRecords.Add(new CombatRecord.SingleAssetCombatRecord
			{
				faction = asset.ref_faction,
				assetName = asset.GetDisplayName(GameControl.control.activePlayer),
				outcome = singleAssetCombatOutcome,
				fled = fled,
				asset = asset,
				assetSummary = text
			});
		}

		// Token: 0x0600434F RID: 17231 RVA: 0x001B458C File Offset: 0x001B278C
		public void AddAssetDestroyedRecord(TISpaceShipState ship, TIGameState killer, TIShipWeaponTemplate killerWeapon)
		{
			if (this.singleAssetRecords == null)
			{
				this.singleAssetRecords = new List<CombatRecord.SingleAssetCombatRecord>();
			}
			TIResourcesCost tiresourcesCost = ship.template.spaceResourceConstructionCost(false, null, false, true, false);
			TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
			if (killerWeapon == null || !killerWeapon.isMissileWeapon || !killerWeapon.ref_missileWeapon.AOEWeapon)
			{
				foreach (ResourceValue resourceValue in tiresourcesCost.resourceCosts)
				{
					switch (resourceValue.resource)
					{
					case FactionResource.Volatiles:
					case FactionResource.Metals:
					case FactionResource.NobleMetals:
						tiresourcesCost2.AddCost(resourceValue.resource, tiresourcesCost.GetSingleCostValue(resourceValue.resource) * TIUtilities.RandomRange(0f, TemplateManager.global.basicSalvageRecoveryCap), true);
						break;
					case FactionResource.Antimatter:
						if (TIUtilities.RandomFloatValue() >= TemplateManager.global.antimatterSalvageChance)
						{
							tiresourcesCost2.AddCost(resourceValue.resource, tiresourcesCost.GetSingleCostValue(resourceValue.resource), true);
						}
						break;
					case FactionResource.Exotics:
						tiresourcesCost2.AddCost(resourceValue.resource, tiresourcesCost.GetSingleCostValue(resourceValue.resource) * TemplateManager.global.Diff_GetExoticsSalvageRate() * TIUtilities.RandomRange(0f, TemplateManager.global.exoticsSalvageRecoveryCap), true);
						break;
					}
				}
			}
			if (this.winnerSalvage == null)
			{
				this.winnerSalvage = new TIResourcesCost();
			}
			this.winnerSalvage.SumCosts_NoDuration(tiresourcesCost2);
			this.singleAssetRecords.RemoveAll((CombatRecord.SingleAssetCombatRecord x) => x.asset == ship);
			this.singleAssetRecords.Add(new CombatRecord.SingleAssetCombatRecord
			{
				faction = ship.faction,
				assetName = ship.GetDisplayName(GameControl.control.activePlayer),
				outcome = SingleAssetCombatOutcome.Destroyed,
				fled = false,
				asset = ship,
				killer = killer,
				killerWeaponTemplateName = (((killerWeapon != null) ? killerWeapon.dataName : null) ?? string.Empty),
				assetSummary = ship.template.quickSummary(ship.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), ship, false, false, false)
			});
			List<string> list = new List<string> { "AlienMothership", "AlienTitan", "Titan" };
			if (!GameControl.control.skirmishMode && killer != null)
			{
				TIFactionState ref_faction = killer.ref_faction;
				if (ref_faction != null && ref_faction.isActivePlayer && killer.isSpaceShipState && killer.ref_ship.hull.simpleHull && list.Contains(ship.hull.dataName))
				{
					killer.ref_faction.UnlockAchievement("exofighterWin");
				}
			}
		}

		// Token: 0x06004350 RID: 17232 RVA: 0x001B489C File Offset: 0x001B2A9C
		public CombatRecord Copy()
		{
			TIResourcesCost tiresourcesCost = null;
			if (this.winnerSalvage != null)
			{
				tiresourcesCost = new TIResourcesCost(this.winnerSalvage);
			}
			List<CombatRecord.SingleAssetCombatRecord> list = null;
			if (this.singleAssetRecords != null)
			{
				list = this.singleAssetRecords.ToList<CombatRecord.SingleAssetCombatRecord>();
			}
			return new CombatRecord
			{
				combatName = this.combatName,
				faction1 = this.faction1,
				faction2 = this.faction2,
				fleet1Name = this.fleet1Name,
				fleet2Name = this.fleet2Name,
				habName = this.habName,
				winnerSalvage = tiresourcesCost,
				singleAssetRecords = list
			};
		}

		// Token: 0x0400282D RID: 10285
		public string combatName;

		// Token: 0x0400282E RID: 10286
		public TIFactionState faction1;

		// Token: 0x0400282F RID: 10287
		public TIFactionState faction2;

		// Token: 0x04002830 RID: 10288
		public string fleet1Name;

		// Token: 0x04002831 RID: 10289
		public string fleet2Name;

		// Token: 0x04002832 RID: 10290
		public string habName;

		// Token: 0x04002833 RID: 10291
		public TIResourcesCost winnerSalvage;

		// Token: 0x04002834 RID: 10292
		public List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords;

		// Token: 0x02000F35 RID: 3893
		public struct SingleAssetCombatRecord
		{
			// Token: 0x04005CF2 RID: 23794
			public TIFactionState faction;

			// Token: 0x04005CF3 RID: 23795
			public string assetName;

			// Token: 0x04005CF4 RID: 23796
			public SingleAssetCombatOutcome outcome;

			// Token: 0x04005CF5 RID: 23797
			public TIGameState asset;

			// Token: 0x04005CF6 RID: 23798
			public bool fled;

			// Token: 0x04005CF7 RID: 23799
			public string assetSummary;

			// Token: 0x04005CF8 RID: 23800
			public TIGameState killer;

			// Token: 0x04005CF9 RID: 23801
			public string killerWeaponTemplateName;
		}
	}
}
