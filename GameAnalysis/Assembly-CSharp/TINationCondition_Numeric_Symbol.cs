using System;
using System.Collections.Generic;

// Token: 0x02000037 RID: 55
public abstract class TINationCondition_Numeric_Symbol : TINationCondition
{
	// Token: 0x0600020F RID: 527 RVA: 0x0001005D File Offset: 0x0000E25D
	public override string GetDescriptionPath()
	{
		return "TINationCondition_Generic";
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000210 RID: 528 RVA: 0x00010064 File Offset: 0x0000E264
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				this.symbolResource,
				base.GetNumericComparisonString(false)
			};
		}
	}
}
