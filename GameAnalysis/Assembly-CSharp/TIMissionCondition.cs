using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000188 RID: 392
public abstract class TIMissionCondition
{
	// Token: 0x060005E5 RID: 1509
	public abstract string CanTarget(TICouncilorState councilor, TIGameState possibleTarget);

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x060005E6 RID: 1510 RVA: 0x0001B58E File Offset: 0x0001978E
	public virtual List<string> feedback
	{
		get
		{
			return new List<string> { base.GetType().Name };
		}
	}

	// Token: 0x04000617 RID: 1559
	public const string pass = "_Pass";

	// Token: 0x04000618 RID: 1560
	public const string fail = "_Fail";
}
