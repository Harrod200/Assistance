using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000940 RID: 2368
	public class HabSchematicOrder : List<TIHabModuleTemplate>
	{
		// Token: 0x17000F56 RID: 3926
		// (get) Token: 0x06005A9B RID: 23195 RVA: 0x002B3285 File Offset: 0x002B1485
		// (set) Token: 0x06005A9C RID: 23196 RVA: 0x002B328D File Offset: 0x002B148D
		public HabPreferences Preferences { get; private set; }

		// Token: 0x06005A9D RID: 23197 RVA: 0x002B3296 File Offset: 0x002B1496
		public HabSchematicOrder(HabPreferences preferences = null, IEnumerable<TIHabModuleTemplate> habModuleTemplates = null)
		{
			if (preferences == null)
			{
				this.Preferences = new HabPreferences();
			}
			else
			{
				this.Preferences = preferences;
			}
			if (habModuleTemplates != null)
			{
				base.AddRange(habModuleTemplates);
			}
		}

		// Token: 0x06005A9E RID: 23198 RVA: 0x002B32C0 File Offset: 0x002B14C0
		public float Score(TIFactionState faction, TIGameState location, Func<FactionResource, float> GetMonthlyIncome = null, bool onlyScoreNewModules = false, bool applyMetaAdjustment = true)
		{
			float num = 0f;
			List<TIHabModuleTemplate> list = this.ToList<TIHabModuleTemplate>();
			List<TIHabModuleTemplate> list2 = new List<TIHabModuleTemplate>();
			if (location.isHabState && onlyScoreNewModules)
			{
				foreach (TIHabModuleTemplate tihabModuleTemplate in from x in location.ref_hab.OkayModules()
					select x.moduleTemplate)
				{
					list.Remove(tihabModuleTemplate);
					list2.Add(tihabModuleTemplate);
				}
			}
			list = (from x in list
				orderby x.coreModule descending, x.IsFarm descending, x.mine descending, x.powerSource, x.dataName
				select x).ToList<TIHabModuleTemplate>();
			HabPreferences habPreferences = new HabPreferences();
			habPreferences.Weight = this.Preferences.Weight;
			foreach (TIHabModuleTemplate tihabModuleTemplate2 in list)
			{
				num += AIEvaluators.EvaluateHabModule_PercentChange(faction, location, tihabModuleTemplate2, habPreferences, list2, GetMonthlyIncome, false, false);
				list2.Add(tihabModuleTemplate2);
			}
			if (applyMetaAdjustment)
			{
				num = this.GetMetaScore(faction, num);
			}
			return num;
		}

		// Token: 0x06005A9F RID: 23199 RVA: 0x002B3498 File Offset: 0x002B1698
		public static float GetMetaScore(TIFactionState faction, float score, float productivity, int missionControlIncome, float spaceCombatStrength, float troopStrength)
		{
			float num = 1f / ((float)Mathf.Max(-missionControlIncome, 0) + 0.5f);
			float num2;
			switch (faction.GetMoneySituation(0f))
			{
			case AIEvaluators.MoneySitation.Terrible:
				num2 = 10f;
				break;
			case AIEvaluators.MoneySitation.Bad:
				num2 = 3f;
				break;
			case AIEvaluators.MoneySitation.Tight:
				num2 = 2f;
				break;
			default:
				num2 = 1f;
				break;
			}
			num = Mathf.Pow(num, 1f / num2);
			if (score < 0f)
			{
				num = 1f / num;
			}
			score *= num;
			score *= productivity;
			float num3 = Mathf.Log(1f + spaceCombatStrength * 1000f) + 0.1f;
			if (score < 0f)
			{
				num3 = 1f / num3;
			}
			score *= num3;
			float num4 = Mathf.Log(1f + troopStrength * 10000f) + 0.1f;
			if (score < 0f)
			{
				num4 = 1f / num4;
			}
			score *= num4;
			return score;
		}

		// Token: 0x06005AA0 RID: 23200 RVA: 0x002B3584 File Offset: 0x002B1784
		public float GetMetaScore(TIFactionState faction, float score)
		{
			return HabSchematicOrder.GetMetaScore(faction, score, this.Aggregate(1f, (float l, TIHabModuleTemplate r) => l * (1f + r.EfficiencyBonus)), this.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.missionControl), this.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.SpaceCombatValue(faction, null, false)), this.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.spaceAssaultValue));
		}
	}
}
