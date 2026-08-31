using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000048 RID: 72
public class TINationCondition_iArmiesPresent_Enemy : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600023D RID: 573 RVA: 0x00010544 File Offset: 0x0000E744
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			return TICondition.PassesComparison(this.sign, state.ref_nation.regions.Sum<TIRegionState>((TIRegionState x) => x.NumArmiesPresent(false, false, true, false)), TIUtilities.GetIntValue(this.strValue));
		}
		return false;
	}
}
