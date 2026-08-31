using System;
using System.Collections.Generic;

// Token: 0x02000038 RID: 56
public abstract class TINationCondition_Numeric_NoSymbol : TINationCondition
{
	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000212 RID: 530 RVA: 0x0001008D File Offset: 0x0000E28D
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
