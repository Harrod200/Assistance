using System;

// Token: 0x020003E5 RID: 997
public struct DamageBreakdown
{
	// Token: 0x060013DE RID: 5086 RVA: 0x0005D74B File Offset: 0x0005B94B
	public DamageBreakdown(float directDamage_Points, float chippingDamage_Points)
	{
		this.directDamage_Points = directDamage_Points;
		this.chippingDamage_Points = chippingDamage_Points;
	}

	// Token: 0x0400119C RID: 4508
	public float directDamage_Points;

	// Token: 0x0400119D RID: 4509
	public float chippingDamage_Points;
}
