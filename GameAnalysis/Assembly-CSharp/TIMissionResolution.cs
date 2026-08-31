using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000260 RID: 608
public abstract class TIMissionResolution
{
	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060007D4 RID: 2004 RVA: 0x00024BEE File Offset: 0x00022DEE
	public virtual bool automaticSuccess
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060007D5 RID: 2005
	public abstract float GetSuccessChance(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f, bool reValidateTarget = false);

	// Token: 0x060007D6 RID: 2006
	public abstract TIMissionResult GetMissionOutcome(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f);

	// Token: 0x060007D7 RID: 2007 RVA: 0x00024BF4 File Offset: 0x00022DF4
	public string GetSuccessChanceString(TIMissionTemplate mission, out float successChance, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f, bool reValidateTarget = false, int digits = 2)
	{
		digits = Mathf.Max(2, digits);
		successChance = this.GetSuccessChance(mission, councilor, target, resourcesSpent, reValidateTarget);
		return Utilities.VariableTruncate(successChance, digits).ToPercent("P" + (digits - 2).ToString());
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00024C40 File Offset: 0x00022E40
	public string GetSuccessChanceString(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f, bool reValidateTarget = false, int digits = 2)
	{
		digits = Mathf.Max(2, digits);
		return Utilities.VariableTruncate(this.GetSuccessChance(mission, councilor, target, resourcesSpent, reValidateTarget), digits).ToPercent("P" + (digits - 2).ToString());
	}

	// Token: 0x0400062B RID: 1579
	public List<TIMissionModifier> attackingModifiers;

	// Token: 0x0400062C RID: 1580
	public List<TIMissionModifier> defendingModifiers;

	// Token: 0x0400062D RID: 1581
	public float baseDifficulty;
}
