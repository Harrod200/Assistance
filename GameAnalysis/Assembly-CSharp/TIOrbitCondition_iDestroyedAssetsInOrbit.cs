using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000119 RID: 281
public class TIOrbitCondition_iDestroyedAssetsInOrbit : TIHabCondition
{
	// Token: 0x0600046A RID: 1130 RVA: 0x0001510C File Offset: 0x0001330C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_orbit != null && TICondition.PassesComparison(this.sign, state.ref_orbit.destroyedAssets, TIUtilities.GetIntValue(this.strValue));
	}
}
