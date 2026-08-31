using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class TIMissionCost_Bonus : TIMissionCost
{
	// Token: 0x06000674 RID: 1652 RVA: 0x0001D82C File Offset: 0x0001BA2C
	public override float GetCost(float bonus, TICouncilorState councilor = null, TIGameState scalingState = null)
	{
		if (this.resourceType == FactionResource.Money)
		{
			if (bonus < 1f)
			{
				return bonus * TemplateManager.global.missionMoneyMultiplier;
			}
			return Mathf.Pow(2f, bonus - 1f) * TemplateManager.global.missionMoneyMultiplier;
		}
		else
		{
			if (bonus < 1f)
			{
				return bonus;
			}
			return Mathf.Pow(2f, bonus - 1f);
		}
	}
}
