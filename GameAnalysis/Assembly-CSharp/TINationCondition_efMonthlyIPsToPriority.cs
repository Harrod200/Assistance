using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000057 RID: 87
public class TINationCondition_efMonthlyIPsToPriority : TINationCondition
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600025C RID: 604 RVA: 0x00010A08 File Offset: 0x0000EC08
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

	// Token: 0x0600025D RID: 605 RVA: 0x00010A38 File Offset: 0x0000EC38
	public override bool PassesCondition(TIGameState state)
	{
		PriorityType priorityType = this.strIdx.ToEnum(PriorityType.Economy);
		TINationState ref_nation = state.ref_nation;
		return ref_nation != null && TICondition.PassesComparison(this.sign, ref_nation.ControlPointWeightsTotalToPriorityIP(priorityType) * 30.436874f, TIUtilities.GetFloatValue(this.strValue));
	}
}
