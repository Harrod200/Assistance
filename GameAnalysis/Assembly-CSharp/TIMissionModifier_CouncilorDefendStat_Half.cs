using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000205 RID: 517
public class TIMissionModifier_CouncilorDefendStat_Half : TIMissionModifier_CouncilorDefendStat
{
	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000708 RID: 1800 RVA: 0x00021FBC File Offset: 0x000201BC
	public override float multiplier
	{
		get
		{
			return 0.5f;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000709 RID: 1801 RVA: 0x00021FC3 File Offset: 0x000201C3
	public override string displayName
	{
		get
		{
			return Loc.T("TIMissionModifier_ScaledStat", new object[] { TIUtilities.GetAttributeString(this.defenderAttribute) });
		}
	}
}
