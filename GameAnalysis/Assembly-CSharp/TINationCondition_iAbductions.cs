using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000055 RID: 85
public class TINationCondition_iAbductions : TINationCondition
{
	// Token: 0x06000257 RID: 599 RVA: 0x0001091C File Offset: 0x0000EB1C
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			return TICondition.PassesComparison(this.sign, state.ref_nation.regions.Sum<TIRegionState>((TIRegionState x) => x.abductions), TIUtilities.GetIntValue(this.strValue));
		}
		return false;
	}
}
