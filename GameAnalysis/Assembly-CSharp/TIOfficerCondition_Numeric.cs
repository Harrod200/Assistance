using System;
using System.Collections.Generic;

// Token: 0x02000135 RID: 309
public abstract class TIOfficerCondition_Numeric : TIOfficerCondition
{
	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060004AE RID: 1198 RVA: 0x000158F7 File Offset: 0x00013AF7
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
