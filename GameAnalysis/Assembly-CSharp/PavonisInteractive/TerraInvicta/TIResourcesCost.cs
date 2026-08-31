using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000712 RID: 1810
	public class TIResourcesCost
	{
		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06002B3B RID: 11067 RVA: 0x000EB142 File Offset: 0x000E9342
		// (set) Token: 0x06002B3C RID: 11068 RVA: 0x000EB14A File Offset: 0x000E934A
		public List<ResourceValue> resourceCosts { get; private set; }

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06002B3D RID: 11069 RVA: 0x000EB153 File Offset: 0x000E9353
		// (set) Token: 0x06002B3E RID: 11070 RVA: 0x000EB15B File Offset: 0x000E935B
		public float completionTime_days { get; private set; }

		// Token: 0x06002B3F RID: 11071 RVA: 0x000EB164 File Offset: 0x000E9364
		public TIResourcesCost()
		{
			this.resourceCosts = new List<ResourceValue>();
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x000EB177 File Offset: 0x000E9377
		public TIResourcesCost(FactionResource resource, float value)
		{
			this.resourceCosts = new List<ResourceValue>
			{
				new ResourceValue(resource, value)
			};
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x000EB197 File Offset: 0x000E9397
		public TIResourcesCost(List<ResourceValue> resources)
		{
			this.resourceCosts = resources.ToList<ResourceValue>();
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x000EB1AC File Offset: 0x000E93AC
		public TIResourcesCost(TIResourcesCost costToCopy)
		{
			this.resourceCosts = new List<ResourceValue>();
			foreach (ResourceValue resourceValue in costToCopy.resourceCosts)
			{
				this.resourceCosts.Add(new ResourceValue(resourceValue.resource, resourceValue.value));
			}
			this.completionTime_days = costToCopy.completionTime_days;
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x000EB234 File Offset: 0x000E9434
		public void SetCompletionTime_Days(float value)
		{
			this.completionTime_days = value;
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x000EB23D File Offset: 0x000E943D
		public void AddToCompletionTime_Days(float value)
		{
			this.completionTime_days += value;
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x000EB24D File Offset: 0x000E944D
		public void ConstructCost(params ResourceValue[] resourceCostArray)
		{
			this.resourceCosts = new List<ResourceValue>();
			this.resourceCosts = resourceCostArray.ToList<ResourceValue>();
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x000EB268 File Offset: 0x000E9468
		public float GetSingleCostValue(FactionResource resource)
		{
			float num = 0f;
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				if (resourceValue.resource == resource)
				{
					num += resourceValue.value;
				}
			}
			return num;
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x000EB2D0 File Offset: 0x000E94D0
		public TIResourcesCost CreateSingleCost(FactionResource resource)
		{
			float num = 0f;
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				if (resourceValue.resource == resource)
				{
					num += resourceValue.value;
				}
			}
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.AddCost(resource, num, true);
			return tiresourcesCost;
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x000EB344 File Offset: 0x000E9544
		public void AddCost(FactionResource resourceToAdd, float resourceAmount, bool allowNegative = true)
		{
			bool flag = false;
			ResourceValue resourceValue = new ResourceValue
			{
				resource = resourceToAdd,
				value = resourceAmount
			};
			if (this.resourceCosts == null)
			{
				this.resourceCosts = new List<ResourceValue>();
			}
			if (resourceAmount != 0f)
			{
				ResourceValue resourceValue2 = default(ResourceValue);
				ResourceValue resourceValue3 = default(ResourceValue);
				foreach (ResourceValue resourceValue4 in this.resourceCosts)
				{
					if (resourceValue4.resource == resourceToAdd)
					{
						flag = true;
						resourceValue2 = resourceValue4;
						resourceValue3.resource = resourceValue4.resource;
						resourceValue3.value = resourceAmount + resourceValue4.value;
						if (!allowNegative)
						{
							resourceValue3.value = Mathf.Max(resourceValue3.value, 0f);
							break;
						}
						break;
					}
				}
				if (flag)
				{
					this.resourceCosts.Remove(resourceValue2);
					this.resourceCosts.Add(resourceValue3);
					return;
				}
				this.resourceCosts.Add(resourceValue);
			}
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x000EB450 File Offset: 0x000E9650
		public void RemoveCost(FactionResource resourceToRemove)
		{
			ResourceValue resourceValue = new ResourceValue
			{
				resource = resourceToRemove
			};
			bool flag = false;
			foreach (ResourceValue resourceValue2 in this.resourceCosts)
			{
				if (resourceValue2.resource == resourceToRemove)
				{
					flag = true;
					resourceValue.value = resourceValue2.value;
				}
			}
			if (flag)
			{
				this.resourceCosts.Remove(resourceValue);
			}
		}

		// Token: 0x06002B4A RID: 11082 RVA: 0x000EB4DC File Offset: 0x000E96DC
		public void SumCosts_NoDuration(TIResourcesCost costToAdd)
		{
			if (costToAdd != null && costToAdd.resourceCosts != null && costToAdd.resourceCosts.Count > 0)
			{
				foreach (ResourceValue resourceValue in costToAdd.resourceCosts)
				{
					this.AddCost(resourceValue.resource, resourceValue.value, true);
				}
			}
		}

		// Token: 0x06002B4B RID: 11083 RVA: 0x000EB554 File Offset: 0x000E9754
		public void SumCostsWithDuration(TIResourcesCost costToAdd)
		{
			this.SumCosts_NoDuration(costToAdd);
			this.SetCompletionTime_Days(this.completionTime_days + costToAdd.completionTime_days);
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x000EB570 File Offset: 0x000E9770
		public static TIResourcesCost operator +(TIResourcesCost a, TIResourcesCost b)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.SumCosts_NoDuration(a);
			tiresourcesCost.SumCosts_NoDuration(b);
			return tiresourcesCost;
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x000EB588 File Offset: 0x000E9788
		public void SubtractRefitDiscountCost(TIResourcesCost costToDiscount)
		{
			if (costToDiscount != null && costToDiscount.resourceCosts != null && costToDiscount.resourceCosts.Count > 0)
			{
				foreach (ResourceValue resourceValue in costToDiscount.resourceCosts)
				{
					this.AddCost(resourceValue.resource, -resourceValue.value * TemplateManager.global.scuttleRefund, true);
				}
			}
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x000EB60C File Offset: 0x000E980C
		public TIResourcesCost GetBoostSubstitutedCost(TIFactionState faction, TIGameState location, bool ignoreTime = false, List<ResourceValue> availableResources = null)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				FactionResource resource = resourceValue.resource;
				float value = resourceValue.value;
				float num = 0f;
				if (availableResources == null)
				{
					num = faction.GetCurrentResourceAmount(resource);
				}
				else
				{
					foreach (ResourceValue resourceValue2 in availableResources)
					{
						if (resourceValue2.resource == resource)
						{
							num = resourceValue2.value;
							break;
						}
					}
				}
				if (num >= value || TIResourcesCost.irreplaceableSpaceResources.Contains(resource))
				{
					tiresourcesCost.AddCost(resource, value, true);
				}
				else
				{
					tiresourcesCost.AddCost(resource, num, true);
					float num2 = value - num;
					float num3 = (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, location, num2 / TemplateManager.global.spaceResourceToTons);
					tiresourcesCost.AddCost(FactionResource.Boost, num3, true);
					float num4 = num2 * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(resource);
					tiresourcesCost.AddCost(FactionResource.Money, num4, true);
				}
			}
			if (!ignoreTime)
			{
				float num5 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, location);
				num5 += TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num5, null);
				tiresourcesCost.completionTime_days = this.completionTime_days + num5;
			}
			return tiresourcesCost;
		}

		// Token: 0x06002B4F RID: 11087 RVA: 0x000EB76C File Offset: 0x000E996C
		public void SubtractRefitPropellantCost(TIResourcesCost costToDiscount)
		{
			if (costToDiscount != null && costToDiscount.resourceCosts != null && costToDiscount.resourceCosts.Count > 0)
			{
				foreach (ResourceValue resourceValue in costToDiscount.resourceCosts)
				{
					this.AddCost(resourceValue.resource, -resourceValue.value, false);
				}
			}
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x000EB7E8 File Offset: 0x000E99E8
		public void GetRefundCost(out TIResourcesCost refundCost)
		{
			refundCost = new TIResourcesCost();
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				if (resourceValue.value < 0f)
				{
					refundCost.AddCost(resourceValue.resource, resourceValue.value, true);
				}
			}
			foreach (ResourceValue resourceValue2 in refundCost.resourceCosts)
			{
				this.AddCost(resourceValue2.resource, -resourceValue2.value, true);
			}
		}

		// Token: 0x06002B51 RID: 11089 RVA: 0x000EB8AC File Offset: 0x000E9AAC
		public bool CanAfford(TIFactionState faction, float maxFractionCanSpend = 1f, List<FactionResource> resourcesToPreserve = null, float maxDays = float.PositiveInfinity)
		{
			maxFractionCanSpend = Mathf.Clamp(maxFractionCanSpend, 0f, 1f);
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				if (resourceValue.value > 0f)
				{
					if (resourcesToPreserve != null && resourcesToPreserve.Contains(resourceValue.resource))
					{
						if (faction.GetCurrentResourceAmount(resourceValue.resource) * maxFractionCanSpend < resourceValue.value)
						{
							return false;
						}
					}
					else if (faction.GetCurrentResourceAmount(resourceValue.resource) < resourceValue.value)
					{
						return false;
					}
					if (this.completionTime_days > maxDays)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x000EB96C File Offset: 0x000E9B6C
		public static bool ShouldTapSavings(TIFactionState faction, TIDataTemplate desiredPurchase = null, TIGameState purchaseLocation = null, int importance = 1)
		{
			if (!faction.AISavingTarget.active)
			{
				return false;
			}
			bool flag = desiredPurchase == faction.AISavingTarget.desiredPurchase && purchaseLocation == faction.AISavingTarget.location;
			if (!flag)
			{
				TIHabModuleTemplate tihabModuleTemplate = desiredPurchase as TIHabModuleTemplate;
				if (tihabModuleTemplate != null)
				{
					string dataName = faction.AISavingTarget.desiredPurchase.dataName;
					TIHabModuleTemplate upgradesTo = tihabModuleTemplate.UpgradesTo;
					if (!(dataName == ((upgradesTo != null) ? upgradesTo.dataName : null)))
					{
						string dataName2 = faction.AISavingTarget.desiredPurchase.dataName;
						TIHabModuleTemplate upgradesTo2 = tihabModuleTemplate.UpgradesTo;
						string text;
						if (upgradesTo2 == null)
						{
							text = null;
						}
						else
						{
							TIHabModuleTemplate upgradesTo3 = upgradesTo2.UpgradesTo;
							text = ((upgradesTo3 != null) ? upgradesTo3.dataName : null);
						}
						if (!(dataName2 == text))
						{
							goto IL_009F;
						}
					}
					flag = true;
				}
			}
			IL_009F:
			return flag || importance > faction.AISavingTarget.importance;
		}

		// Token: 0x06002B53 RID: 11091 RVA: 0x000EBA2C File Offset: 0x000E9C2C
		public TIResourcesCost GetShortfall(TIFactionState faction, TIDataTemplate desiredPurchase = null, TIGameState purchaseLocation = null, int importance = 1, bool tapSavings = false)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tapSavings = tapSavings || TIResourcesCost.ShouldTapSavings(faction, desiredPurchase, purchaseLocation, importance);
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				float num = faction.GetCurrentResourceAmount(resourceValue.resource);
				if (!tapSavings && faction.AISavingTarget.active)
				{
					num = Mathf.Max(num - faction.AISavingTarget.GetBankedQuantity(resourceValue.resource), 0f);
				}
				tiresourcesCost.AddCost(resourceValue.resource, Mathf.Max(0f, resourceValue.value - num), true);
			}
			return tiresourcesCost;
		}

		// Token: 0x06002B54 RID: 11092 RVA: 0x000EBAF0 File Offset: 0x000E9CF0
		public bool CanAfford_AI(TIFactionState faction, TIDataTemplate desiredPurchase = null, TIGameState purchaseLocation = null, int importance = 1, bool isPlanned = false, bool tapSavings = false, float maxFractionCanSpend = 1f, List<FactionResource> resourcesToPreserve = null, float maxDays = float.PositiveInfinity)
		{
			tapSavings = tapSavings || TIResourcesCost.ShouldTapSavings(faction, desiredPurchase, purchaseLocation, importance);
			return AIEvaluators.PassesBudgetingRules(faction, desiredPurchase, this, isPlanned, tapSavings) && this.CanAfford(faction, maxFractionCanSpend, resourcesToPreserve, maxDays);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x000EBB24 File Offset: 0x000E9D24
		public bool CanPayInFuture(TIFactionState faction, int daysInTheFuture = 180)
		{
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				float currentResourceAmount = faction.GetCurrentResourceAmount(resourceValue.resource);
				float num = AIEvaluators.EstimateFutureIncomePerMonth(faction, resourceValue.resource, false, false, false) / 30.436874f;
				if (currentResourceAmount + num * (float)daysInTheFuture < resourceValue.value)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x000EBBA8 File Offset: 0x000E9DA8
		public int CanAfford_Count(TIFactionState faction, Dictionary<FactionResource, float> resourcesAvailable = null)
		{
			if (this.resourceCosts.Any<ResourceValue>())
			{
				if (!this.resourceCosts.All<ResourceValue>((ResourceValue x) => x.value == 0f))
				{
					if (resourcesAvailable == null)
					{
						resourcesAvailable = faction.copyResources;
					}
					int num = 0;
					bool flag = true;
					while (flag)
					{
						foreach (ResourceValue resourceValue in this.resourceCosts)
						{
							if (resourcesAvailable[resourceValue.resource] < resourceValue.value)
							{
								return num;
							}
							Dictionary<FactionResource, float> dictionary = resourcesAvailable;
							FactionResource resource = resourceValue.resource;
							dictionary[resource] -= resourceValue.value;
						}
						num++;
						if (num == 2147483647)
						{
							flag = false;
							continue;
						}
						continue;
					}
					return num;
				}
			}
			return int.MaxValue;
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x000EBC98 File Offset: 0x000E9E98
		public List<ResourceValue> LackingResources(TIFactionState faction)
		{
			List<ResourceValue> list = new List<ResourceValue>();
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				float num = resourceValue.value - faction.GetCurrentResourceAmount(resourceValue.resource);
				if (num > 0f)
				{
					list.Add(new ResourceValue
					{
						resource = resourceValue.resource,
						value = num
					});
				}
			}
			return list;
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x000EBD2C File Offset: 0x000E9F2C
		public void PayCost(TIFactionState faction, string label = null)
		{
			if (this.resourceCosts != null)
			{
				foreach (ResourceValue resourceValue in this.resourceCosts)
				{
					faction.SubtractFromCurrentResource(resourceValue.value, resourceValue.resource, true, label);
					if (resourceValue.resource == FactionResource.Boost && resourceValue.value > 0f)
					{
						int num = (int)Mathf.Clamp(resourceValue.value * TemplateManager.global.spaceResourceToTons, 1f, 3f);
						for (int i = 0; i < num; i++)
						{
							TIRegionSpaceFacilityState tiregionSpaceFacilityState = faction.SelectRandomLaunchSite();
							if (tiregionSpaceFacilityState != null)
							{
								TIDateTime tidateTime = TITimeState.Now();
								tidateTime.AddDays(this.completionTime_days - TIUtilities.RandomRange(0.01f, 0.25f) * this.completionTime_days);
								TITimeEvent.CreateNewTimeEvent(tidateTime, tiregionSpaceFacilityState, null, null, "Launch Rocket to Orbit", false, false, TITimeQueueRepeatType.None, 1, true, false);
							}
						}
					}
				}
				GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(faction), null, new object[] { faction });
			}
		}

		// Token: 0x06002B59 RID: 11097 RVA: 0x000EBE50 File Offset: 0x000EA050
		public void RefundCost(TIFactionState faction, string label = null)
		{
			if (this.resourceCosts != null)
			{
				foreach (ResourceValue resourceValue in this.resourceCosts)
				{
					faction.AddToCurrentResource(resourceValue.value, resourceValue.resource, true, label);
				}
				GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(faction), null, new object[] { faction });
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06002B5A RID: 11098 RVA: 0x000EBED4 File Offset: 0x000EA0D4
		public bool anyDebit
		{
			get
			{
				List<ResourceValue> resourceCosts = this.resourceCosts;
				if (resourceCosts == null)
				{
					return false;
				}
				return resourceCosts.Any<ResourceValue>((ResourceValue x) => x.value > 0f);
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06002B5B RID: 11099 RVA: 0x000EBF06 File Offset: 0x000EA106
		public bool anyCredit
		{
			get
			{
				List<ResourceValue> resourceCosts = this.resourceCosts;
				if (resourceCosts == null)
				{
					return false;
				}
				return resourceCosts.Any<ResourceValue>((ResourceValue x) => x.value < 0f);
			}
		}

		// Token: 0x06002B5C RID: 11100 RVA: 0x000EBF38 File Offset: 0x000EA138
		public string GetString(string format, bool includeCostStr, bool includeCompletionTime, bool completionTimeOnly, int relevantCap = 7, bool costsOnly = false, bool gainsOnly = false, TIFactionState faction = null, bool iconsOnly = false, FactionResource resourceForZero = FactionResource.None)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = format == "Relevant";
			if (completionTimeOnly)
			{
				float num = this.completionTime_days;
				return num.ToString(flag ? TIUtilities.DecimalPlaces((double)this.completionTime_days, relevantCap, 0) : format);
			}
			List<ResourceValue> resourceCosts = this.resourceCosts;
			if (resourceCosts != null && resourceCosts.Count > 0)
			{
				this.resourceCosts = this.resourceCosts.OrderBy<ResourceValue, FactionResource>((ResourceValue x) => x.resource).ToList<ResourceValue>();
				using (List<ResourceValue>.Enumerator enumerator = this.resourceCosts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ResourceValue resourceValue = enumerator.Current;
						if ((!costsOnly && !gainsOnly) || (costsOnly && resourceValue.value > 0f))
						{
							if (faction == null || faction.GetCurrentResourceAmount(resourceValue.resource) >= resourceValue.value)
							{
								stringBuilder.Append(iconsOnly ? TIUtilities.InlineResourceStr(resourceValue.resource) : resourceValue.ToString()).Append(" ");
							}
							else
							{
								stringBuilder.Append(iconsOnly ? TIUtilities.InlineResourceStr(resourceValue.resource) : TIUtilities.RedLine(resourceValue.ToString())).Append(" ");
							}
						}
						else if (gainsOnly && resourceValue.value < 0f)
						{
							stringBuilder.Append(TIUtilities.InlineResourceStr(resourceValue.resource));
							if (!iconsOnly)
							{
								if (flag)
								{
									stringBuilder.Append(TIUtilities.FormatSmallNumber(-resourceValue.value, 7, 0, (double)Mathf.Abs(resourceValue.value) >= 0.001, false));
								}
								else
								{
									StringBuilder stringBuilder2 = stringBuilder;
									float num = -resourceValue.value;
									stringBuilder2.Append(num.ToString(format));
								}
							}
							stringBuilder.Append(" ");
						}
					}
					goto IL_0225;
				}
			}
			if (resourceForZero == FactionResource.None)
			{
				stringBuilder.Append(Loc.T("TIResourceCost.NoCost"));
			}
			else
			{
				stringBuilder.Append(TIUtilities.InlineResourceStr(resourceForZero)).Append("0");
			}
			IL_0225:
			if (includeCostStr)
			{
				if (this.completionTime_days <= 0f || !includeCompletionTime)
				{
					return Loc.T("TIResourceCost.Cost", new object[] { stringBuilder.ToString().TrimEnd(Array.Empty<char>()) });
				}
				float num;
				if (this.completionTime_days < 1f)
				{
					string text = "TIResourceCost.CostWithCompletionTime_Hours";
					object[] array = new object[2];
					array[0] = stringBuilder.ToString();
					int num2 = 1;
					num = this.completionTime_days / 24f;
					array[num2] = num.ToString(flag ? TIUtilities.DecimalPlaces((double)this.completionTime_days, relevantCap, 0) : format);
					return Loc.T(text, array);
				}
				string text2 = "TIResourceCost.CostWithCompletionTime";
				object[] array2 = new object[2];
				array2[0] = stringBuilder.ToString();
				int num3 = 1;
				num = this.completionTime_days;
				array2[num3] = num.ToString(flag ? TIUtilities.DecimalPlaces((double)this.completionTime_days, relevantCap, 0) : format);
				return Loc.T(text2, array2);
			}
			else
			{
				if (this.completionTime_days <= 0f || !includeCompletionTime)
				{
					return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
				}
				float num;
				if (this.completionTime_days < 1f)
				{
					string text3 = "TIResourceCost.WithCompletionTime_Hours";
					object[] array3 = new object[2];
					array3[0] = stringBuilder.ToString();
					int num4 = 1;
					num = this.completionTime_days * 24f;
					array3[num4] = num.ToString(flag ? TIUtilities.DecimalPlaces((double)this.completionTime_days, relevantCap, 0) : format);
					return Loc.T(text3, array3);
				}
				string text4 = "TIResourceCost.WithCompletionTime";
				object[] array4 = new object[2];
				array4[0] = stringBuilder.ToString();
				int num5 = 1;
				num = this.completionTime_days;
				array4[num5] = num.ToString(flag ? TIUtilities.DecimalPlaces((double)this.completionTime_days, relevantCap, 0) : format);
				return Loc.T(text4, array4);
			}
		}

		// Token: 0x06002B5D RID: 11101 RVA: 0x000EC310 File Offset: 0x000EA510
		public TIResourcesCost MultiplyCost(float modifier)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (modifier != 0f)
			{
				foreach (ResourceValue resourceValue in this.resourceCosts)
				{
					tiresourcesCost.AddCost(resourceValue.resource, resourceValue.value * modifier, true);
				}
			}
			return tiresourcesCost;
		}

		// Token: 0x06002B5E RID: 11102 RVA: 0x000EC380 File Offset: 0x000EA580
		public string ToString(string format = "Relevant", bool gainsOnly = false, bool costsOnly = false, TIFactionState faction = null, bool iconsOnly = false, FactionResource resourceIconForAllZero = FactionResource.None)
		{
			return this.GetString(format, false, false, false, 7, costsOnly, gainsOnly, faction, iconsOnly, resourceIconForAllZero);
		}

		// Token: 0x0400211F RID: 8479
		public static readonly HashSet<FactionResource> spaceResources = new HashSet<FactionResource>
		{
			FactionResource.Water,
			FactionResource.Volatiles,
			FactionResource.Metals,
			FactionResource.NobleMetals,
			FactionResource.Fissiles,
			FactionResource.Antimatter,
			FactionResource.Exotics
		};

		// Token: 0x04002120 RID: 8480
		public static readonly HashSet<FactionResource> basicSpaceResources = new HashSet<FactionResource>
		{
			FactionResource.Water,
			FactionResource.Volatiles,
			FactionResource.Metals,
			FactionResource.NobleMetals,
			FactionResource.Fissiles
		};

		// Token: 0x04002121 RID: 8481
		public static readonly HashSet<FactionResource> basicSpaceResourcesSansFissiles = new HashSet<FactionResource>
		{
			FactionResource.Water,
			FactionResource.Volatiles,
			FactionResource.Metals,
			FactionResource.NobleMetals
		};

		// Token: 0x04002122 RID: 8482
		public static readonly HashSet<FactionResource> replaceableSpaceResources = new HashSet<FactionResource>
		{
			FactionResource.Water,
			FactionResource.Volatiles,
			FactionResource.Metals,
			FactionResource.NobleMetals,
			FactionResource.Fissiles
		};

		// Token: 0x04002123 RID: 8483
		public static readonly HashSet<FactionResource> irreplaceableSpaceResources = new HashSet<FactionResource>
		{
			FactionResource.Exotics,
			FactionResource.Antimatter
		};

		// Token: 0x04002124 RID: 8484
		public static readonly HashSet<FactionResource> unTradeableResources = new HashSet<FactionResource>
		{
			FactionResource.MissionControl,
			FactionResource.Projects,
			FactionResource.None,
			FactionResource.Research
		};

		// Token: 0x04002125 RID: 8485
		public static readonly HashSet<FactionResource> tradeableResources = new HashSet<FactionResource>(Enums.FactionResources.Except<FactionResource>(TIResourcesCost.unTradeableResources));

		// Token: 0x04002126 RID: 8486
		public static readonly HashSet<FactionResource> unAccumulatableResources = new HashSet<FactionResource>
		{
			FactionResource.MissionControl,
			FactionResource.Projects,
			FactionResource.None
		};

		// Token: 0x04002127 RID: 8487
		public static readonly HashSet<FactionResource> accumulatableResources = new HashSet<FactionResource>(Enums.FactionResources.Except<FactionResource>(TIResourcesCost.unAccumulatableResources));

		// Token: 0x04002128 RID: 8488
		public static readonly HashSet<FactionResource> resourcesAllowedToGoNegative = new HashSet<FactionResource> { FactionResource.Money };

		// Token: 0x04002129 RID: 8489
		public static readonly HashSet<FactionResource> habResources = new HashSet<FactionResource>
		{
			FactionResource.Money,
			FactionResource.Influence,
			FactionResource.Operations,
			FactionResource.Research,
			FactionResource.Projects,
			FactionResource.Boost,
			FactionResource.MissionControl,
			FactionResource.Water,
			FactionResource.Volatiles,
			FactionResource.Metals,
			FactionResource.NobleMetals,
			FactionResource.Fissiles,
			FactionResource.Antimatter,
			FactionResource.Exotics
		};

		// Token: 0x0400212A RID: 8490
		public static readonly HashSet<FactionResource> farmResources = new HashSet<FactionResource>
		{
			FactionResource.Water,
			FactionResource.Volatiles
		};
	}
}
