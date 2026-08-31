using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class TIRegionCondition_fAbsLatitude : TIRegionCondition_Numeric_NoSymbol
{
	// Token: 0x060002B7 RID: 695 RVA: 0x0001146F File Offset: 0x0000F66F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, Mathf.Abs(state.ref_region.latitude), TIUtilities.GetFloatValue(this.strValue));
	}
}
