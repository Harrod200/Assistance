using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200032F RID: 815
public class BombardOperation_High : BombardOperation
{
	// Token: 0x06000D9B RID: 3483 RVA: 0x00043DF8 File Offset: 0x00041FF8
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00043DFB File Offset: 0x00041FFB
	public override float bombardmentAltitude_km(TISpaceBodyState targetBody)
	{
		return BombardOperation_High.alt_km;
	}

	// Token: 0x04000EB4 RID: 3764
	public static readonly float alt_km = TemplateManager.global.highBombardmentAltitude_km;
}
