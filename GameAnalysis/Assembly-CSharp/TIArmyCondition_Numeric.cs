using System;
using System.Collections.Generic;

// Token: 0x0200013E RID: 318
public abstract class TIArmyCondition_Numeric : TIArmyCondition
{
	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060004C5 RID: 1221 RVA: 0x00015B1B File Offset: 0x00013D1B
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}
}
