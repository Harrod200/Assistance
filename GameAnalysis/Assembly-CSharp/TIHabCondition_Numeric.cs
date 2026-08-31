using System;
using System.Collections.Generic;

// Token: 0x020000F4 RID: 244
public abstract class TIHabCondition_Numeric : TIHabCondition
{
	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000415 RID: 1045 RVA: 0x000144AD File Offset: 0x000126AD
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
