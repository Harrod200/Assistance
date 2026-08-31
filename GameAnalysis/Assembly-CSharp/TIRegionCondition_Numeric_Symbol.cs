using System;
using System.Collections.Generic;

// Token: 0x0200006F RID: 111
public abstract class TIRegionCondition_Numeric_Symbol : TIRegionCondition
{
	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060002A0 RID: 672 RVA: 0x00011219 File Offset: 0x0000F419
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1)
			{
				this.symbolResource,
				base.GetNumericComparisonString(false)
			};
		}
	}
}
