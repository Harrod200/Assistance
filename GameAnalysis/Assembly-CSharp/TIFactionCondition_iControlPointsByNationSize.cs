using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000AC RID: 172
public class TIFactionCondition_iControlPointsByNationSize : TIFactionCondition
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000353 RID: 851 RVA: 0x000127EE File Offset: 0x000109EE
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				this.strIdx.ToString(),
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00012814 File Offset: 0x00010A14
	public override bool PassesCondition(TIGameState state)
	{
		if (string.IsNullOrEmpty(this.strIdx))
		{
			this.strIdx = "0";
		}
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.nation.numControlPoints >= TIUtilities.GetIntValue(this.strIdx)).Count<TIControlPoint>(), TIUtilities.GetIntValue(this.strValue));
	}
}
