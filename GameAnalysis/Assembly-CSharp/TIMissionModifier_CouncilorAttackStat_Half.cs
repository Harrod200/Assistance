using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000202 RID: 514
public class TIMissionModifier_CouncilorAttackStat_Half : TIMissionModifier_CouncilorAttackStat
{
	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x060006FF RID: 1791 RVA: 0x00021EB8 File Offset: 0x000200B8
	public override float multiplier
	{
		get
		{
			return 0.5f;
		}
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000700 RID: 1792 RVA: 0x00021EBF File Offset: 0x000200BF
	public override string displayName
	{
		get
		{
			return Loc.T("TIMissionModifier_ScaledStat", new object[] { TIUtilities.GetAttributeString(this.attackerAttribute) });
		}
	}
}
