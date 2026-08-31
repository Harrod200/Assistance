using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200035E RID: 862
public abstract class TICommandTargeting : TITargeting
{
	// Token: 0x06000F2B RID: 3883
	public abstract void Initialize(TISpaceShipState ship, IShipCommandWithTarget command);

	// Token: 0x06000F2C RID: 3884
	public abstract void Initialize(List<TISpaceShipState> ships, IFleetCommandWithTarget command);
}
