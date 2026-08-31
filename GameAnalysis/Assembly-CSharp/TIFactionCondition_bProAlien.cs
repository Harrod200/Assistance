using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000AF RID: 175
public class TIFactionCondition_bProAlien : TIFactionCondition
{
	// Token: 0x0600035D RID: 861 RVA: 0x0001293F File Offset: 0x00010B3F
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00012947 File Offset: 0x00010B47
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.proAlien, TIUtilities.GetBoolValue(this.strValue));
	}
}
