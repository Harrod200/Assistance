using System;
using System.Collections.Generic;

// Token: 0x02000123 RID: 291
public abstract class TIFleetCondition_Numeric : TIFleetCondition
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000480 RID: 1152 RVA: 0x000153B4 File Offset: 0x000135B4
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
