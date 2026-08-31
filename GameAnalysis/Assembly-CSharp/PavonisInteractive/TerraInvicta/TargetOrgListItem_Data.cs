using System;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000849 RID: 2121
	public class TargetOrgListItem_Data
	{
		// Token: 0x17000EAC RID: 3756
		// (get) Token: 0x06004D06 RID: 19718 RVA: 0x0020BAD9 File Offset: 0x00209CD9
		// (set) Token: 0x06004D07 RID: 19719 RVA: 0x0020BAE1 File Offset: 0x00209CE1
		public float toHitValue { get; private set; }

		// Token: 0x06004D08 RID: 19720 RVA: 0x0020BAEC File Offset: 0x00209CEC
		public void SetTargetOrgData(TIOrgState org, TICouncilorState targetingCouncilor, TIMissionTemplate missionTemplate, bool validTarget, CouncilorMissionCanvasController controller)
		{
			this.targetingCouncilor = targetingCouncilor;
			this.selectButtonText = Loc.T("UI.OrgTargeting.OrgSelect");
			if (targetingCouncilor != null && missionTemplate != null && validTarget)
			{
				this.toHitValue = missionTemplate.resolutionMethod.GetSuccessChance(missionTemplate, targetingCouncilor, org, 0f, false);
				this.successChance = missionTemplate.resolutionMethod.GetSuccessChanceString(missionTemplate, targetingCouncilor, org, 0f, false, 2);
			}
			else
			{
				this.toHitValue = -1f;
				this.successChance = string.Empty;
			}
			this.orgName = org.displayName;
			this.orgIcon = org.icon;
			this.orgDescription = org.description(true, GameControl.control.activePlayer, true, false);
			this.tier = org.tierStarsInline;
			if (org.assignedCouncilor != null)
			{
				this.owningCouncilorForeground = org.assignedCouncilor.GetIcon(false);
				this.owningCouncilorBackgroundColor = org.factionOrbit.template.color;
			}
			else
			{
				this.owningCouncilorForeground = org.factionOrbit.factionIcon128UI;
			}
			this.persuasion = ((org.persuasion != 0) ? TIUtilities.ForceValueSign((float)org.persuasion, false, false, "") : string.Empty);
			this.investigation = ((org.investigation != 0) ? TIUtilities.ForceValueSign((float)org.investigation, false, false, "") : string.Empty);
			this.espionage = ((org.espionage != 0) ? TIUtilities.ForceValueSign((float)org.espionage, false, false, "") : string.Empty);
			this.command = ((org.command != 0) ? TIUtilities.ForceValueSign((float)org.command, false, false, "") : string.Empty);
			this.administration = ((org.administration != 0) ? TIUtilities.ForceValueSign((float)org.administration, false, false, "") : string.Empty);
			this.science = ((org.science != 0) ? TIUtilities.ForceValueSign((float)org.science, false, false, "") : string.Empty);
			this.security = ((org.security != 0) ? TIUtilities.ForceValueSign((float)org.security, false, false, "") : string.Empty);
			this.money = ((org.adjustedIncomeMoney_month != 0f) ? TIUtilities.ForceValueSign(org.adjustedIncomeMoney_month, false, false, "") : string.Empty);
			this.influence = ((org.adjustedIncomeInfluence_month != 0f) ? TIUtilities.ForceValueSign(org.adjustedIncomeInfluence_month, false, false, "") : string.Empty);
			this.ops = ((org.adjustedIncomeOps_month != 0f) ? TIUtilities.ForceValueSign(org.adjustedIncomeOps_month, false, false, "") : string.Empty);
			this.research = ((org.adjustedIncomeResearch_month != 0f) ? TIUtilities.ForceValueSign(org.adjustedIncomeResearch_month, false, false, "") : string.Empty);
			this.boost = ((org.adjustedIncomeBoost_month != 0f) ? TIUtilities.ForceValueSign(org.adjustedIncomeBoost_month, false, false, "") : string.Empty);
			this.missionControl = ((org.incomeMissionControl != 0f) ? TIUtilities.ForceValueSign(org.incomeMissionControl, false, false, "") : string.Empty);
			this.projects = ((org.projectCapacityGranted != 0) ? TIUtilities.ForceValueSign((float)org.projectCapacityGranted, false, false, "") : string.Empty);
			this.priority_ECO = ((org.economyBonus != 0f) ? TIUtilities.ForceValueSign(org.economyBonus, org.economyBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_WEL = ((org.welfareBonus != 0f) ? TIUtilities.ForceValueSign(org.welfareBonus, org.welfareBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_ENV = ((org.environmentBonus != 0f) ? TIUtilities.ForceValueSign(org.environmentBonus, org.environmentBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_KNO = ((org.knowledgeBonus != 0f) ? TIUtilities.ForceValueSign(org.knowledgeBonus, org.knowledgeBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_GOV = ((org.governmentBonus != 0f) ? TIUtilities.ForceValueSign(org.governmentBonus, org.governmentBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_UNI = ((org.unityBonus != 0f) ? TIUtilities.ForceValueSign(org.unityBonus, org.unityBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_MIL = ((org.militaryBonus != 0f) ? TIUtilities.ForceValueSign(org.militaryBonus, org.militaryBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_OPP = ((org.oppressionBonus != 0f) ? TIUtilities.ForceValueSign(org.oppressionBonus, org.oppressionBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_FUN = ((org.spaceDevBonus != 0f) ? TIUtilities.ForceValueSign(org.spaceDevBonus, org.spaceDevBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_SPO = ((org.spoilsBonus != 0f) ? TIUtilities.ForceValueSign(org.spoilsBonus, org.spoilsBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_FLI = ((org.spaceflightBonus != 0f) ? TIUtilities.ForceValueSign(org.spaceflightBonus, org.spaceflightBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.priority_MC = ((org.MCBonus != 0f) ? TIUtilities.ForceValueSign(org.MCBonus, org.MCBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.miningBonus = ((org.miningBonus != 0f) ? TIUtilities.ForceValueSign(org.miningBonus, org.miningBonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood) : string.Empty);
			this.techBonus = ((org.techBonuses.Length != 0) ? new StringBuilder(TIGenericTechTemplate.categoryInlineSprite(org.techBonuses[0].category)).Append(TIUtilities.ForceValueSign(org.techBonuses[0].bonus, org.techBonuses[0].bonus.ToPercent("P0"), false, false, NationInfoController.WhatIsGood.upIsGood)).ToString() : string.Empty);
		}

		// Token: 0x04002F78 RID: 12152
		public bool showInList;

		// Token: 0x04002F79 RID: 12153
		public CouncilorMissionCanvasController controller;

		// Token: 0x04002F7A RID: 12154
		public string selectButtonText;

		// Token: 0x04002F7B RID: 12155
		public string successChance;

		// Token: 0x04002F7C RID: 12156
		public Sprite warningIcon;

		// Token: 0x04002F7D RID: 12157
		public Color32 owningCouncilorBackgroundColor;

		// Token: 0x04002F7E RID: 12158
		public Sprite owningCouncilorForeground;

		// Token: 0x04002F7F RID: 12159
		public Sprite orgIcon;

		// Token: 0x04002F80 RID: 12160
		public string tier;

		// Token: 0x04002F81 RID: 12161
		public string orgName;

		// Token: 0x04002F82 RID: 12162
		public string orgDescription;

		// Token: 0x04002F83 RID: 12163
		public string persuasion;

		// Token: 0x04002F84 RID: 12164
		public string investigation;

		// Token: 0x04002F85 RID: 12165
		public string espionage;

		// Token: 0x04002F86 RID: 12166
		public string command;

		// Token: 0x04002F87 RID: 12167
		public string administration;

		// Token: 0x04002F88 RID: 12168
		public string science;

		// Token: 0x04002F89 RID: 12169
		public string security;

		// Token: 0x04002F8A RID: 12170
		public string money;

		// Token: 0x04002F8B RID: 12171
		public string influence;

		// Token: 0x04002F8C RID: 12172
		public string ops;

		// Token: 0x04002F8D RID: 12173
		public string research;

		// Token: 0x04002F8E RID: 12174
		public string boost;

		// Token: 0x04002F8F RID: 12175
		public string missionControl;

		// Token: 0x04002F90 RID: 12176
		public string projects;

		// Token: 0x04002F91 RID: 12177
		public string priority_ECO;

		// Token: 0x04002F92 RID: 12178
		public string priority_WEL;

		// Token: 0x04002F93 RID: 12179
		public string priority_ENV;

		// Token: 0x04002F94 RID: 12180
		public string priority_KNO;

		// Token: 0x04002F95 RID: 12181
		public string priority_GOV;

		// Token: 0x04002F96 RID: 12182
		public string priority_UNI;

		// Token: 0x04002F97 RID: 12183
		public string priority_MIL;

		// Token: 0x04002F98 RID: 12184
		public string priority_OPP;

		// Token: 0x04002F99 RID: 12185
		public string priority_FUN;

		// Token: 0x04002F9A RID: 12186
		public string priority_SPO;

		// Token: 0x04002F9B RID: 12187
		public string priority_FLI;

		// Token: 0x04002F9C RID: 12188
		public string priority_MC;

		// Token: 0x04002F9D RID: 12189
		public string miningBonus;

		// Token: 0x04002F9E RID: 12190
		public string techBonus;

		// Token: 0x04002F9F RID: 12191
		public string missionsTip;

		// Token: 0x04002FA0 RID: 12192
		public TIOrgState org;

		// Token: 0x04002FA1 RID: 12193
		public TICouncilorState targetingCouncilor;

		// Token: 0x04002FA2 RID: 12194
		public TIMissionTemplate missionTemplate;

		// Token: 0x04002FA3 RID: 12195
		public bool validTarget;
	}
}
