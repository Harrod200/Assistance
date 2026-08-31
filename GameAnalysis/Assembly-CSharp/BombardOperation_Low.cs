using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000331 RID: 817
public class BombardOperation_Low : BombardOperation
{
	// Token: 0x06000DA3 RID: 3491 RVA: 0x00043E3E File Offset: 0x0004203E
	public override int SortOrder()
	{
		return 3;
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00043E41 File Offset: 0x00042041
	public override float bombardmentAltitude_km(TISpaceBodyState targetBody)
	{
		return BombardOperation_Low.alt_km;
	}

	// Token: 0x04000EB6 RID: 3766
	public static readonly float alt_km = TemplateManager.global.lowBombardmentAltitude_km;
}
