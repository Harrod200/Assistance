using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000943 RID: 2371
	internal abstract class ScoreDecisionBase : HabSchematicDecision
	{
		// Token: 0x17000F57 RID: 3927
		// (get) Token: 0x06005AA9 RID: 23209 RVA: 0x002B39BB File Offset: 0x002B1BBB
		public virtual Func<TIFactionState, TIGameState, TIHabModuleTemplate, HabSchematicOrder, float> Score
		{
			get
			{
				return (TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, HabSchematicOrder order) => AIEvaluators.EvaluateHabModule_PercentChange(faction, location, moduleTemplate, order.Preferences, order, null, true, moduleTemplate != this.firstChoice);
			}
		}

		// Token: 0x17000F58 RID: 3928
		// (get) Token: 0x06005AAA RID: 23210 RVA: 0x002B39C9 File Offset: 0x002B1BC9
		// (set) Token: 0x06005AAB RID: 23211 RVA: 0x002B39D1 File Offset: 0x002B1BD1
		public bool ChooseRandomly { get; protected set; }

		// Token: 0x06005AAC RID: 23212 RVA: 0x002B39DA File Offset: 0x002B1BDA
		public ScoreDecisionBase Randomize()
		{
			this.ChooseRandomly = true;
			return this;
		}

		// Token: 0x06005AAD RID: 23213
		public abstract IEnumerable<TIHabModuleTemplate> GetChoices(TIFactionState faction, TIGameState location, HabSchematicOrder order);

		// Token: 0x06005AAE RID: 23214 RVA: 0x002B39E4 File Offset: 0x002B1BE4
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			List<TIHabModuleTemplate> list = (from x in this.GetChoices(faction, location, order).Distinct<TIHabModuleTemplate>()
				where HabSchematicDecision.IsValidModule(faction, location, x, order)
				select x).ToList<TIHabModuleTemplate>();
			if (!list.Any<TIHabModuleTemplate>())
			{
				return HabSchematicDecision.Nothing;
			}
			float unadjustedOrderScore = order.Score(faction, location, null, false, false);
			float currentProductivity = order.Aggregate(1f, (float l, TIHabModuleTemplate r) => l * (1f + r.EfficiencyBonus));
			int currentMissionControlIncome = order.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.missionControl);
			float currentCombatStrength = order.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.SpaceCombatValue(faction, null, false));
			float currentAssaultStrength = order.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.spaceAssaultValue);
			float orderScore = HabSchematicOrder.GetMetaScore(faction, unadjustedOrderScore, currentProductivity, currentMissionControlIncome, currentCombatStrength, currentAssaultStrength);
			this.firstChoice = list.First<TIHabModuleTemplate>();
			Dictionary<TIHabModuleTemplate, float> dictionary = list.ToDictionary<TIHabModuleTemplate, TIHabModuleTemplate, float>((TIHabModuleTemplate x) => x, delegate(TIHabModuleTemplate moduleTemplate)
			{
				float num = this.Score(faction, location, moduleTemplate, order);
				if (moduleTemplate.EfficiencyBonus != 0f || moduleTemplate.missionControl != 0 || moduleTemplate.spaceCombatModule || moduleTemplate.spaceAssaultValue > 0f)
				{
					float metaScore = HabSchematicOrder.GetMetaScore(faction, unadjustedOrderScore, currentProductivity * (1f + moduleTemplate.EfficiencyBonus), currentMissionControlIncome + moduleTemplate.missionControl, currentCombatStrength + moduleTemplate.SpaceCombatValue(faction, null, false), currentAssaultStrength + moduleTemplate.spaceAssaultValue);
					float habModuleSize = AIEvaluators.GetHabModuleSize(faction, location, moduleTemplate, order);
					float num2 = Mathf.Pow(metaScore / orderScore, 1f / habModuleSize) - 1f;
					num = num2 * orderScore + (num2 + 1f) * (orderScore / unadjustedOrderScore) * num;
				}
				else
				{
					num *= orderScore / unadjustedOrderScore;
				}
				return num;
			});
			TIHabModuleTemplate tihabModuleTemplate;
			if (this.ChooseRandomly)
			{
				tihabModuleTemplate = dictionary.SelectRandomWeightedItem<KeyValuePair<TIHabModuleTemplate, float>>((KeyValuePair<TIHabModuleTemplate, float> x) => x.Value, -1f, 1E-37f).Key;
			}
			else
			{
				tihabModuleTemplate = dictionary.MaxBy<KeyValuePair<TIHabModuleTemplate, float>, float>((KeyValuePair<TIHabModuleTemplate, float> x) => x.Value).Key;
			}
			return HabSchematicDecision.Nothing.Append(tihabModuleTemplate);
		}

		// Token: 0x04004157 RID: 16727
		private TIHabModuleTemplate firstChoice;
	}
}
