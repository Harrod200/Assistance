using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000056 RID: 86
public class TINationCondition_efPriority : TINationCondition
{
	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000259 RID: 601 RVA: 0x00010986 File Offset: 0x0000EB86
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TIUtilities.GetPriorityString(this.strIdx.ToEnum(PriorityType.Economy), false),
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x0600025A RID: 602 RVA: 0x000109B4 File Offset: 0x0000EBB4
	public override bool PassesCondition(TIGameState state)
	{
		PriorityType priorityType = this.strIdx.ToEnum(PriorityType.Economy);
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.percentWeighttoPriority(priorityType), TIUtilities.GetFloatValue(this.strValue));
	}
}
