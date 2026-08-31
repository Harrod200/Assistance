using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B9 RID: 185
public class TIFactionCondition_iMines : TIFactionCondition
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600037A RID: 890 RVA: 0x00012CE9 File Offset: 0x00010EE9
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00012D00 File Offset: 0x00010F00
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_faction.habs.Count<TIHabState>((TIHabState x) => x.HasMineFunctional), TIUtilities.GetIntValue(this.strValue));
	}
}
