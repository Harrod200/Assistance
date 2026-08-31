using System;

// Token: 0x02000315 RID: 789
public abstract class FoundAlienSurveillanceHab : FoundPlatformFromFleetOperation
{
	// Token: 0x06000CC8 RID: 3272 RVA: 0x00041682 File Offset: 0x0003F882
	public override bool DestroyShipOnExecute()
	{
		return true;
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00041685 File Offset: 0x0003F885
	public override bool isAlien()
	{
		return true;
	}
}
