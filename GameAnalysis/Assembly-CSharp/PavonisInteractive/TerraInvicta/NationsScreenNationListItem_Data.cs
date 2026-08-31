using System;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200089D RID: 2205
	public class NationsScreenNationListItem_Data
	{
		// Token: 0x06005337 RID: 21303 RVA: 0x00250524 File Offset: 0x0024E724
		public void SetNationData(TINationState nationState)
		{
			this.nation = nationState;
			TIFactionState activePlayer = this.controller.activePlayer;
			this.nationName = this.nation.displayName;
			if (this.nation.inFederation)
			{
				this.federationName = this.nation.federation.displayName;
			}
			FactionIdeology ideology = this.nation.GetMostPopularIdeology(false).ideology;
			float num = this.nation.GetMostPopularFactionValue(false);
			float num2;
			this.nation.historyPublicOpinion[31].TryGetValue(ideology, out num2);
			TIFactionState factionByIdeology = TIFactionIdeologyTemplate.GetFactionByIdeology(ideology);
			if (ideology != FactionIdeology.Undecided)
			{
				this.mostPopularFactionForIcon = factionByIdeology;
			}
			else
			{
				this.mostPopularFactionForIcon = null;
			}
			this.mostPopularFactionValue = new StringBuilder(num.ToPercent("P0")).Append(NationInfoController.numberToArrow((double)(num - num2), (factionByIdeology == activePlayer) ? NationInfoController.WhatIsGood.upIsGood : NationInfoController.WhatIsGood.downIsGood, 0f, 5f)).ToString();
			float publicOpinionOfFaction = this.nation.GetPublicOpinionOfFaction(activePlayer.ideology);
			float num3;
			this.nation.historyPublicOpinion[31].TryGetValue(activePlayer.ideology.ideology, out num3);
			this.myFactionValue = new StringBuilder(publicOpinionOfFaction.ToPercent("P0")).Append(NationInfoController.numberToArrow((double)(publicOpinionOfFaction - num3), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.population = new StringBuilder(TIUtilities.FormatBigNumber((double)this.nation.population, 1, false)).Append(NationInfoController.numberToArrow((double)(this.nation.population_Millions - this.nation.historyPopulation[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.sustainability = new StringBuilder(TINationState.SustainabilityValueForDisplay(this.nation.sustainability)).Append(NationInfoController.numberToArrow((double)(-1f * (this.nation.sustainability - this.nation.historySustainability[31])), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.investmentPoints = new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.BaseInvestmentPoints_month(), 2, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.BaseInvestmentPoints_month() - this.nation.historyInvestmentPoints[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.government = new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.democracy, 1, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.democracy - this.nation.historyDemocracy[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.education = new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.education, 1, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.education - this.nation.historyEducation[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.inequality = new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.inequality, 1, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.inequality - this.nation.historyInequality[31]), NationInfoController.WhatIsGood.downIsGood, 0f, 5f)).ToString();
			this.cohesion = new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.cohesion, 1, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.cohesion - this.nation.historyCohesion[31]), NationInfoController.WhatIsGood.upOrMiddleIsGood, this.nation.cohesion, 5f)).Append(this.nation.CohesionRestStateInlineSpritePath()).ToString();
			this.unrest = new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.unrest, 1, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.unrest - this.nation.historyUnrest[31]), NationInfoController.WhatIsGood.downIsGood, 0f, 5f)).Append(this.nation.UnrestRestStateInlineSpritePath()).ToString();
			this.funding = new StringBuilder(TIUtilities.FormatBigOrSmallNumber(this.nation.spaceFunding_month, 1, 7, 0, false, false)).Append(NationInfoController.numberToArrow((double)(this.nation.spaceFunding_month - this.nation.historySpaceFunding[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.research = new StringBuilder(TIUtilities.FormatBigOrSmallNumber(this.nation.research_month, 1, 7, 0, false, false)).Append(NationInfoController.numberToArrow((double)(this.nation.research_month - this.nation.historyResearch[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.boost = new StringBuilder(TIUtilities.FormatBigOrSmallNumber(this.nation.boostIncome_month_dekatons, 1, 7, 0, false, false)).Append(NationInfoController.numberToArrow((double)(this.nation.boostIncome_month_dekatons - this.nation.historyBoost[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.missionControl = new StringBuilder(this.nation.currentMissionControl.ToString("N0")).Append(NationInfoController.numberToArrow((double)(this.nation.currentMissionControl - this.nation.historyMissionControl[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.warPeaceIcon = GameControl.assetLoader.LoadAsset<Sprite>(this.nation.atWar ? TemplateManager.global.pathWarIcon : TemplateManager.global.pathPeaceIcon);
			this.miltech = new StringBuilder(this.nation.military ? TIUtilities.FormatSmallNumber(this.nation.militaryTechLevel, 1, 0, true, false) : TemplateManager.global.noneIconInlineSpritePath).Append(NationInfoController.numberToArrow((double)(this.nation.militaryTechLevel - this.nation.historyMiltech[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.nukes = new StringBuilder(this.nation.nuclearProgram ? this.nation.numNuclearWeapons.ToString("N0") : TIGlobalConfig.globalConfig.noneIconInlineSpritePath).Append(NationInfoController.numberToArrow((double)(this.nation.numNuclearWeapons - this.nation.historyNukes[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).ToString();
			this.armies = this.nation.armies.Count.ToString("N0");
			this.navies = this.nation.numNavies.ToString("N0");
			this.stoFighters = this.nation.numSTOFighters.ToString("N0");
		}

		// Token: 0x04003896 RID: 14486
		public NationsScreenController controller;

		// Token: 0x04003897 RID: 14487
		public TIControlPoint controlPoint;

		// Token: 0x04003898 RID: 14488
		public TINationState nation;

		// Token: 0x04003899 RID: 14489
		public bool nationLine;

		// Token: 0x0400389A RID: 14490
		public bool showInList;

		// Token: 0x0400389B RID: 14491
		[Header("Nation Data")]
		public string nationName;

		// Token: 0x0400389C RID: 14492
		public string federationName;

		// Token: 0x0400389D RID: 14493
		public string mostPopularFactionValue;

		// Token: 0x0400389E RID: 14494
		public TIFactionState mostPopularFactionForIcon;

		// Token: 0x0400389F RID: 14495
		public string myFactionValue;

		// Token: 0x040038A0 RID: 14496
		public string population;

		// Token: 0x040038A1 RID: 14497
		public string investmentPoints;

		// Token: 0x040038A2 RID: 14498
		public string perCapitaGDP;

		// Token: 0x040038A3 RID: 14499
		public string sustainability;

		// Token: 0x040038A4 RID: 14500
		public string government;

		// Token: 0x040038A5 RID: 14501
		public string education;

		// Token: 0x040038A6 RID: 14502
		public string inequality;

		// Token: 0x040038A7 RID: 14503
		public string cohesion;

		// Token: 0x040038A8 RID: 14504
		public string unrest;

		// Token: 0x040038A9 RID: 14505
		public string funding;

		// Token: 0x040038AA RID: 14506
		public string research;

		// Token: 0x040038AB RID: 14507
		public string boost;

		// Token: 0x040038AC RID: 14508
		public string missionControl;

		// Token: 0x040038AD RID: 14509
		public Sprite warPeaceIcon;

		// Token: 0x040038AE RID: 14510
		public string miltech;

		// Token: 0x040038AF RID: 14511
		public string armies;

		// Token: 0x040038B0 RID: 14512
		public string nukes;

		// Token: 0x040038B1 RID: 14513
		public string navies;

		// Token: 0x040038B2 RID: 14514
		public string stoFighters;
	}
}
