using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000054 RID: 84
public class TINationCondition_iNuclearWeaponsInEnemies : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000255 RID: 597 RVA: 0x000108B0 File Offset: 0x0000EAB0
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			return TICondition.PassesComparison(this.sign, state.ref_nation.enemies.Sum<TINationState>((TINationState x) => x.numNuclearWeapons), TIUtilities.GetIntValue(this.strValue));
		}
		return false;
	}
}
