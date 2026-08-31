using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000330 RID: 816
public class BombardOperation_Med : BombardOperation
{
	// Token: 0x06000D9F RID: 3487 RVA: 0x00043E1B File Offset: 0x0004201B
	public override int SortOrder()
	{
		return 2;
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00043E1E File Offset: 0x0004201E
	public override float bombardmentAltitude_km(TISpaceBodyState targetBody)
	{
		return BombardOperation_Med.alt_km;
	}

	// Token: 0x04000EB5 RID: 3765
	public static readonly float alt_km = TemplateManager.global.medBombardmentAltitude_km;
}
