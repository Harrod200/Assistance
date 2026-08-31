using System;
using System.Collections.Generic;

// Token: 0x0200006E RID: 110
public abstract class TIRegionCondition_Numeric_NoSymbol : TIRegionCondition
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600029E RID: 670 RVA: 0x000111FC File Offset: 0x0000F3FC
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
