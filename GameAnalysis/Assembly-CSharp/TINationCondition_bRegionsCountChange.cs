using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000050 RID: 80
public class TINationCondition_bRegionsCountChange : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600024D RID: 589 RVA: 0x0001078C File Offset: 0x0000E98C
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			bool flag = false;
			for (int i = 1; i <= 31; i++)
			{
				if (state.ref_nation.regions.Count != state.ref_nation.historyNumRegions[i])
				{
					flag = true;
					break;
				}
			}
			return TICondition.PassesComparison(this.sign, flag, TIUtilities.GetBoolValue(this.strValue));
		}
		return false;
	}
}
