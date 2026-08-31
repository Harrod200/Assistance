using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000722 RID: 1826
	public struct CouncilorAugmentationOption
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06002D35 RID: 11573 RVA: 0x000F93AE File Offset: 0x000F75AE
		// (set) Token: 0x06002D36 RID: 11574 RVA: 0x000F93B6 File Offset: 0x000F75B6
		public CouncilorAttribute stat { readonly get; private set; }

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06002D37 RID: 11575 RVA: 0x000F93BF File Offset: 0x000F75BF
		// (set) Token: 0x06002D38 RID: 11576 RVA: 0x000F93C7 File Offset: 0x000F75C7
		public int statValue { readonly get; private set; }

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06002D39 RID: 11577 RVA: 0x000F93D0 File Offset: 0x000F75D0
		// (set) Token: 0x06002D3A RID: 11578 RVA: 0x000F93D8 File Offset: 0x000F75D8
		public TITraitTemplate traitToGain { readonly get; private set; }

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06002D3B RID: 11579 RVA: 0x000F93E1 File Offset: 0x000F75E1
		// (set) Token: 0x06002D3C RID: 11580 RVA: 0x000F93E9 File Offset: 0x000F75E9
		public TITraitTemplate traitToLose { readonly get; private set; }

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06002D3D RID: 11581 RVA: 0x000F93F2 File Offset: 0x000F75F2
		// (set) Token: 0x06002D3E RID: 11582 RVA: 0x000F93FA File Offset: 0x000F75FA
		public TIResourcesCost resourceCost { readonly get; private set; }

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06002D3F RID: 11583 RVA: 0x000F9403 File Offset: 0x000F7603
		// (set) Token: 0x06002D40 RID: 11584 RVA: 0x000F940B File Offset: 0x000F760B
		public int XPCost { readonly get; private set; }

		// Token: 0x06002D41 RID: 11585 RVA: 0x000F9414 File Offset: 0x000F7614
		public void SetAugmentationStrings(out string description1, out string description2, out string tooltipDescription, out string costString)
		{
			description1 = string.Empty;
			description2 = string.Empty;
			tooltipDescription = string.Empty;
			if (this.stat != CouncilorAttribute.None)
			{
				description1 = Loc.T("UI.Councilor.StatIncrease");
				description2 = Loc.T("UI.Councilor.StatIncreaseDetail", new object[]
				{
					TIUtilities.InlineAttributeStr(this.stat),
					this.statValue.ToString("N0"),
					TIUtilities.GetAttributeString(this.stat)
				});
				tooltipDescription = Loc.T("UI.Councilor.StatIncreaseTooltip", new object[] { TemplateManager.global.maxCouncilorAttribute });
			}
			if (this.traitToGain != null)
			{
				if (this.traitToGain.requiresProject)
				{
					description1 = Loc.T("UI.Councilor.CyberneticAugmentation");
				}
				else
				{
					description1 = TIUtilities.GreenLine(Loc.T("UI.Councilor.GainTrait"));
				}
				description2 = this.traitToGain.displayName;
				tooltipDescription = this.traitToGain.fullTraitSummary;
				if (this.traitToLose != null)
				{
					StringBuilder stringBuilder = new StringBuilder(tooltipDescription);
					stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Councilor.GainWillRemoveTrait", new object[] { this.traitToLose.displayName })));
					tooltipDescription = stringBuilder.ToString();
				}
			}
			else if (this.traitToLose != null)
			{
				description1 = Loc.T("UI.Councilor.RemoveTrait");
				description2 = this.traitToLose.displayName;
				tooltipDescription = this.traitToLose.fullTraitSummary;
			}
			StringBuilder stringBuilder2 = new StringBuilder(Loc.T("UI.Councilor.AugmentationCost"));
			if (this.XPCost != 0)
			{
				stringBuilder2.Append(Loc.T("UI.Councilor.XPCost", new object[] { this.XPCost.ToString("N0") })).Append(" ");
			}
			if (this.resourceCost != null && this.resourceCost.anyDebit)
			{
				stringBuilder2.Append(this.resourceCost.ToString("Relevant", false, false, null, false, FactionResource.None));
			}
			costString = stringBuilder2.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06002D42 RID: 11586 RVA: 0x000F9608 File Offset: 0x000F7808
		public CouncilorAugmentationOption(CouncilorAttribute stat, TITraitTemplate trait, float addTraitCostMultiplier, float addTraitMoneyCostMultiplier, float councilorXPModifier)
		{
			this.stat = stat;
			this.traitToLose = null;
			this.traitToGain = null;
			this.XPCost = 0;
			if (stat != CouncilorAttribute.None)
			{
				this.statValue = 1;
				this.XPCost = Mathf.RoundToInt((float)TemplateManager.global.XPToLevelUp * (1f + councilorXPModifier));
			}
			else
			{
				this.statValue = 0;
			}
			this.resourceCost = new TIResourcesCost();
			if (trait != null)
			{
				if (trait.XPCost > 0 || trait.moneyCost > 0 || trait.influenceCost > 0 || trait.opsCost > 0 || trait.boostCost > 0)
				{
					this.traitToGain = trait;
					this.XPCost = Mathf.RoundToInt((float)trait.XPCost * addTraitCostMultiplier * (1f + councilorXPModifier));
					this.resourceCost.AddCost(FactionResource.Money, (float)trait.moneyCost * addTraitCostMultiplier * addTraitMoneyCostMultiplier, true);
					this.resourceCost.AddCost(FactionResource.Influence, (float)trait.influenceCost * addTraitCostMultiplier, true);
					this.resourceCost.AddCost(FactionResource.Operations, (float)trait.opsCost * addTraitCostMultiplier, true);
					this.resourceCost.AddCost(FactionResource.Boost, (float)trait.boostCost * addTraitCostMultiplier, true);
					this.traitToLose = this.traitToGain.requiredTraitForUpgrade;
					return;
				}
				if (trait.XPCost < 0 || trait.moneyCost < 0 || trait.influenceCost < 0 || trait.opsCost < 0 || trait.boostCost < 0)
				{
					this.traitToLose = trait;
					this.XPCost = Mathf.RoundToInt((float)Mathf.Abs(this.traitToLose.XPCost) * (1f + councilorXPModifier));
					this.resourceCost.AddCost(FactionResource.Money, (float)Mathf.Abs(this.traitToLose.moneyCost), true);
					this.resourceCost.AddCost(FactionResource.Influence, (float)Mathf.Abs(this.traitToLose.influenceCost), true);
					this.resourceCost.AddCost(FactionResource.Operations, (float)Mathf.Abs(this.traitToLose.opsCost), true);
					this.resourceCost.AddCost(FactionResource.Boost, (float)Mathf.Abs(this.traitToLose.boostCost), true);
				}
			}
		}

		// Token: 0x06002D43 RID: 11587 RVA: 0x000F980C File Offset: 0x000F7A0C
		public bool CouncilorEligibleForAugmentation(TICouncilorState councilor)
		{
			TIFactionState tifactionState = ((councilor != null) ? councilor.faction : null);
			if (councilor == null || tifactionState == null)
			{
				return false;
			}
			if (this.stat != CouncilorAttribute.None && this.traitToGain == null && this.traitToLose == null && councilor.GetAttribute(this.stat, false, true, true, false, false, false) < TemplateManager.global.maxCouncilorAttribute)
			{
				return true;
			}
			if (this.traitToGain != null)
			{
				bool flag;
				if (this.traitToGain.requiresProject)
				{
					flag = false;
					using (List<TIProjectTemplate>.Enumerator enumerator = tifactionState.completedProjects.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIProjectTemplate tiprojectTemplate = enumerator.Current;
							if (this.traitToGain.IsMatchingProject(tiprojectTemplate))
							{
								flag = true;
								break;
							}
						}
						goto IL_00B6;
					}
				}
				flag = true;
				IL_00B6:
				if (flag)
				{
					TITraitTemplate requiredTraitForUpgrade = this.traitToGain.requiredTraitForUpgrade;
					List<TITraitTemplate> list = new List<TITraitTemplate>(councilor.traits);
					if (requiredTraitForUpgrade != null)
					{
						list.Remove(requiredTraitForUpgrade);
					}
					int traitGrouping = this.traitToGain.grouping.GetValueOrDefault();
					if ((traitGrouping == 0 || (list.Count > 0 && list.None<TITraitTemplate>(delegate(TITraitTemplate x)
					{
						int? grouping = x.grouping;
						int traitGrouping2 = traitGrouping;
						return (grouping.GetValueOrDefault() == traitGrouping2) & (grouping != null);
					}))) && (this.traitToGain.requiresProject || councilor.GetIndividualTraitChance(this.traitToGain, councilor.faction) > 0f || (requiredTraitForUpgrade != null && councilor.traits.Contains(requiredTraitForUpgrade))))
					{
						return this.traitToLose == null || councilor.traits.Contains(this.traitToLose);
					}
				}
			}
			else if (this.traitToLose != null)
			{
				return councilor.traits.Contains(this.traitToLose);
			}
			return false;
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x000F99C8 File Offset: 0x000F7BC8
		public bool CouncilorCanAfford(TICouncilorState councilor)
		{
			return councilor.XP >= this.XPCost && (this.resourceCost == null || this.resourceCost.CanAfford(councilor.faction, 1f, null, float.PositiveInfinity));
		}
	}
}
