using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000CD RID: 205
public class TIGlobalCondition_iSpacePopulation : TIGlobalCondition
{
	// Token: 0x060003A9 RID: 937 RVA: 0x00013270 File Offset: 0x00011470
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, (from x in GameStateManager.IterateByClass<TIHabState>(false)
			where !x.IsAlien()
			select x).Sum<TIHabState>((TIHabState x) => x.crew), TIUtilities.GetIntValue(this.strValue));
	}
}
