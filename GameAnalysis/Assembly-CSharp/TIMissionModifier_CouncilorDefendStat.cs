using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000204 RID: 516
public class TIMissionModifier_CouncilorDefendStat : TIMissionModifier_CouncilorStat
{
	// Token: 0x06000705 RID: 1797 RVA: 0x00021F68 File Offset: 0x00020168
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TICouncilorState ref_councilor = target.ref_councilor;
		if (ref_councilor != null)
		{
			num = (float)ref_councilor.GetAttribute(this.defenderAttribute, true, true, true, false, false, false) * this.multiplier;
		}
		return num;
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000706 RID: 1798 RVA: 0x00021FA7 File Offset: 0x000201A7
	public override string displayName
	{
		get
		{
			return TIUtilities.GetAttributeString(this.defenderAttribute);
		}
	}
}
