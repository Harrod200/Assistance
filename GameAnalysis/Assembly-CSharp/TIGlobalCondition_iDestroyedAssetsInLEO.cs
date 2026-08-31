using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D0 RID: 208
public class TIGlobalCondition_iDestroyedAssetsInLEO : TIGlobalCondition
{
	// Token: 0x060003AF RID: 943 RVA: 0x00013340 File Offset: 0x00011540
	public override bool PassesCondition(TIGameState state)
	{
		int num = 0;
		foreach (TIOrbitState tiorbitState in GameStateManager.Earth().orbits.Where<TIOrbitState>((TIOrbitState x) => x.isEarthLEO))
		{
			num += tiorbitState.destroyedAssets;
		}
		return TICondition.PassesComparison(this.sign, num, TIUtilities.GetIntValue(this.strValue));
	}
}
