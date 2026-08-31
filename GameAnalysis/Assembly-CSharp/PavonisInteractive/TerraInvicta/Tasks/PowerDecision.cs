using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000942 RID: 2370
	internal class PowerDecision : ArchetypeDecision
	{
		// Token: 0x06005AA4 RID: 23204 RVA: 0x002B36E2 File Offset: 0x002B18E2
		public PowerDecision()
			: base(ArchetypeDecision.HabModuleArchetype.Power, true)
		{
		}

		// Token: 0x06005AA5 RID: 23205 RVA: 0x002B36EC File Offset: 0x002B18EC
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			int num = order.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.ProspectivePower(location, faction));
			if (num >= 0)
			{
				return HabSchematicDecision.Nothing;
			}
			TIHabModuleTemplate bestPowerModuleTemplate_Internal = this.GetBestPowerModuleTemplate_Internal(faction, location, order);
			if (bestPowerModuleTemplate_Internal == null)
			{
				return HabSchematicDecision.Nothing;
			}
			int num2 = bestPowerModuleTemplate_Internal.ProspectivePower(location, faction);
			int num3 = 0;
			do
			{
				num3++;
				num += num2;
			}
			while (num < 0);
			return Enumerable.Repeat<TIHabModuleTemplate>(bestPowerModuleTemplate_Internal, num3);
		}

		// Token: 0x06005AA6 RID: 23206 RVA: 0x002B3774 File Offset: 0x002B1974
		private TIHabModuleTemplate GetBestPowerModuleTemplate_Internal(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			PowerDecision.<>c__DisplayClass4_0 CS$<>8__locals1 = new PowerDecision.<>c__DisplayClass4_0();
			CS$<>8__locals1.location = location;
			CS$<>8__locals1.faction = faction;
			if (PowerDecision.bestPowerModulesCachedFrame != TIFrameCounter.FrameCount)
			{
				PowerDecision.cachedBestPowerModules.Clear();
				PowerDecision.bestPowerModulesCachedFrame = TIFrameCounter.FrameCount;
			}
			TIHabModuleTemplate tihabModuleTemplate = order.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule);
			if (tihabModuleTemplate == null)
			{
				return null;
			}
			ValueTuple<TIFactionState, TISpaceBodyState, int, bool> valueTuple = new ValueTuple<TIFactionState, TISpaceBodyState, int, bool>(CS$<>8__locals1.faction, CS$<>8__locals1.location.ref_system, tihabModuleTemplate.tier, tihabModuleTemplate.automated);
			TIHabModuleTemplate tihabModuleTemplate2;
			if (!PowerDecision.cachedBestPowerModules.TryGetValue(valueTuple, out tihabModuleTemplate2))
			{
				PowerDecision.<>c__DisplayClass4_1 CS$<>8__locals2 = new PowerDecision.<>c__DisplayClass4_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				IEnumerable<TIHabModuleTemplate> enumerable = base.Decide(CS$<>8__locals2.CS$<>8__locals1.faction, CS$<>8__locals2.CS$<>8__locals1.location, order);
				TIHabModuleTemplate tihabModuleTemplate3 = enumerable.MaxBy<TIHabModuleTemplate, float>(new Func<TIHabModuleTemplate, float>(CS$<>8__locals2.CS$<>8__locals1.<GetBestPowerModuleTemplate_Internal>g__ScoreOptions|1));
				CS$<>8__locals2.idealOptionProspectivePower = (float)((tihabModuleTemplate3 != null) ? tihabModuleTemplate3.ProspectivePower(CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction) : 0);
				if (CS$<>8__locals2.idealOptionProspectivePower <= 0f)
				{
					tihabModuleTemplate2 = null;
				}
				else
				{
					TIHabModuleTemplate tihabModuleTemplate4 = enumerable.Where<TIHabModuleTemplate>(delegate(TIHabModuleTemplate option)
					{
						PowerDecision.<>c__DisplayClass4_2 CS$<>8__locals3 = new PowerDecision.<>c__DisplayClass4_2();
						CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
						CS$<>8__locals3.option = option;
						PowerDecision.<>c__DisplayClass4_2 CS$<>8__locals4 = CS$<>8__locals3;
						TIHabModuleTemplate option2 = CS$<>8__locals3.option;
						CS$<>8__locals4.prospectivePower = (float)((option2 != null) ? option2.ProspectivePower(CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction) : 0);
						return TIResourcesCost.basicSpaceResources.None<FactionResource>(delegate(FactionResource resource)
						{
							float num2 = CS$<>8__locals3.option.supportMaterials_month.GetWeightedCost(resource) * 12f * CS$<>8__locals3.CS$<>8__locals2.idealOptionProspectivePower / CS$<>8__locals3.prospectivePower;
							return AIEvaluators.ShouldNotTakeOnElectiveExpenditureRightNow(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction, resource, num2);
						});
					}).MaxBy<TIHabModuleTemplate, float>(new Func<TIHabModuleTemplate, float>(CS$<>8__locals2.CS$<>8__locals1.<GetBestPowerModuleTemplate_Internal>g__ScoreOptions|1));
					float num = ((tihabModuleTemplate3 == tihabModuleTemplate4) ? CS$<>8__locals2.idealOptionProspectivePower : ((float)((tihabModuleTemplate4 != null) ? tihabModuleTemplate4.ProspectivePower(CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction) : 0)));
					if (tihabModuleTemplate3 != tihabModuleTemplate4 && num >= 3f)
					{
						if (CS$<>8__locals2.idealOptionProspectivePower >= 4f * num && tihabModuleTemplate.tier > 1)
						{
							tihabModuleTemplate2 = tihabModuleTemplate3;
						}
						else
						{
							tihabModuleTemplate2 = tihabModuleTemplate4;
						}
					}
					else if (CS$<>8__locals2.idealOptionProspectivePower > 0f)
					{
						tihabModuleTemplate2 = tihabModuleTemplate3;
					}
					else
					{
						tihabModuleTemplate2 = null;
					}
				}
				PowerDecision.cachedBestPowerModules[valueTuple] = tihabModuleTemplate2;
			}
			return tihabModuleTemplate2;
		}

		// Token: 0x06005AA7 RID: 23207 RVA: 0x002B3960 File Offset: 0x002B1B60
		public static TIHabModuleTemplate GetBestPowerModuleTemplate(TIFactionState faction, TIGameState location, IEnumerable<TIHabModuleTemplate> existingModules = null)
		{
			if (existingModules == null && location.isHabState)
			{
				existingModules = Enumerable.Empty<TIHabModuleTemplate>().Append(location.ref_hab.coreModule.moduleTemplate);
			}
			HabSchematicOrder habSchematicOrder = new HabSchematicOrder(null, existingModules);
			return new PowerDecision().GetBestPowerModuleTemplate_Internal(faction, location, habSchematicOrder);
		}

		// Token: 0x04004155 RID: 16725
		private static Dictionary<ValueTuple<TIFactionState, TISpaceBodyState, int, bool>, TIHabModuleTemplate> cachedBestPowerModules = new Dictionary<ValueTuple<TIFactionState, TISpaceBodyState, int, bool>, TIHabModuleTemplate>();

		// Token: 0x04004156 RID: 16726
		private static int bestPowerModulesCachedFrame = -1;
	}
}
