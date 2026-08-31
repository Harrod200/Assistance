using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200077E RID: 1918
	public class TechProgress
	{
		// Token: 0x06003BBD RID: 15293 RVA: 0x00168B94 File Offset: 0x00166D94
		public TechProgress(string templateName)
		{
			this.factionContributions = GameStateManager.AllHumanFactions().ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, (TIFactionState x) => 0f);
			this.techTemplateName = templateName;
			this.accumulatedResearch = 0f;
		}

		// Token: 0x06003BBE RID: 15294 RVA: 0x00168C07 File Offset: 0x00166E07
		public TechProgress(TITechTemplate template)
			: this(template.dataName)
		{
		}

		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x06003BBF RID: 15295 RVA: 0x00168C15 File Offset: 0x00166E15
		public TITechTemplate techTemplate
		{
			get
			{
				return TemplateManager.Find<TITechTemplate>(this.techTemplateName, false);
			}
		}

		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x06003BC0 RID: 15296 RVA: 0x00168C23 File Offset: 0x00166E23
		public TechCategory TechCategory
		{
			get
			{
				return this.techTemplate.techCategory;
			}
		}

		// Token: 0x17000A9A RID: 2714
		// (get) Token: 0x06003BC1 RID: 15297 RVA: 0x00168C30 File Offset: 0x00166E30
		public float progressFrac
		{
			get
			{
				return this.accumulatedResearch / this.techTemplate.GetResearchCost(null);
			}
		}

		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x06003BC2 RID: 15298 RVA: 0x00168C45 File Offset: 0x00166E45
		public float remainingResearch
		{
			get
			{
				return this.techTemplate.GetResearchCost(null) - this.accumulatedResearch;
			}
		}

		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x06003BC3 RID: 15299 RVA: 0x00168C5A File Offset: 0x00166E5A
		public int slot
		{
			get
			{
				return GameStateManager.GlobalResearch().GetSlotForTech(this.techTemplate);
			}
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x00168C6C File Offset: 0x00166E6C
		public TIFactionState GetExpectedWinner(bool fast = false)
		{
			Dictionary<TIFactionState, float> dictionary = (from x in GameStateManager.AllHumanFactions()
				where x.IsActiveHumanFaction
				select x).ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, (TIFactionState x) => x.FractionWeightInSlot(this.slot) * x.GetEffectiveResearchPerDay(this.TechCategory, false, fast));
			float daysToCompletion = (this.techTemplate.GetResearchCost(null) - this.accumulatedResearch) / dictionary.Values.Sum();
			if (float.IsInfinity(daysToCompletion) || float.IsNaN(daysToCompletion))
			{
				daysToCompletion = 0f;
			}
			if (this.factionContributions.Count > 1)
			{
				List<KeyValuePair<TIFactionState, float>> list = this.factionContributions.OrderByDescending<KeyValuePair<TIFactionState, float>, float>((KeyValuePair<TIFactionState, float> x) => x.Value).Take<KeyValuePair<TIFactionState, float>>(2).ToList<KeyValuePair<TIFactionState, float>>();
				float num = list[0].Value - list[1].Value;
				float num2 = this.techTemplate.GetResearchCost(null) - this.accumulatedResearch;
				if (num > num2)
				{
					return list[0].Key;
				}
			}
			return dictionary.Where<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => this.factionContributions[x.Key] > 0f).MaxBy<KeyValuePair<TIFactionState, float>, float>((KeyValuePair<TIFactionState, float> x) => this.factionContributions[x.Key] + Mathf.Abs(daysToCompletion) * x.Value).Key;
		}

		// Token: 0x06003BC5 RID: 15301 RVA: 0x00168DEC File Offset: 0x00166FEC
		public List<TIFactionState> GetPlacements()
		{
			if (this.factionContributions == null)
			{
				return new List<TIFactionState>();
			}
			return (from x in this.factionContributions
				where x.Value > 0f
				orderby x.Value descending
				select x.Key).ToList<TIFactionState>();
		}

		// Token: 0x06003BC6 RID: 15302 RVA: 0x00168E7E File Offset: 0x0016707E
		public int GetPlacement(TIFactionState faction)
		{
			return this.GetPlacements().IndexOf(faction);
		}

		// Token: 0x06003BC7 RID: 15303 RVA: 0x00168E8C File Offset: 0x0016708C
		public bool CantWin(TIFactionState faction)
		{
			if (faction.IsActiveHumanFaction)
			{
				float maxContribution = this.factionContributions[faction] + this.remainingResearch;
				return GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x != faction && this.factionContributions[x] >= maxContribution);
			}
			return true;
		}

		// Token: 0x06003BC8 RID: 15304 RVA: 0x00168EF0 File Offset: 0x001670F0
		public bool CantLose(TIFactionState faction)
		{
			return faction.IsActiveHumanFaction && GameStateManager.AllHumanFactions().All<TIFactionState>((TIFactionState otherFaction) => otherFaction == faction || this.factionContributions[otherFaction] + this.remainingResearch < this.factionContributions[faction]);
		}

		// Token: 0x040025E0 RID: 9696
		public string techTemplateName;

		// Token: 0x040025E1 RID: 9697
		public float accumulatedResearch;

		// Token: 0x040025E2 RID: 9698
		public Dictionary<TIFactionState, float> factionContributions;

		// Token: 0x040025E3 RID: 9699
		public TIFactionState selector;
	}
}
