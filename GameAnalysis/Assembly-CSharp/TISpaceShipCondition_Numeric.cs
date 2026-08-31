using System;
using System.Collections.Generic;

// Token: 0x02000128 RID: 296
public abstract class TISpaceShipCondition_Numeric : TISpaceShipCondition
{
	// Token: 0x17000091 RID: 145
	// (get) Token: 0x0600048A RID: 1162 RVA: 0x0001548F File Offset: 0x0001368F
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
