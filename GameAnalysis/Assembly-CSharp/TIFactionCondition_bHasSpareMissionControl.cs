using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200009A RID: 154
public class TIFactionCondition_bHasSpareMissionControl : TIFactionCondition
{
	// Token: 0x0600031A RID: 794 RVA: 0x00012087 File Offset: 0x00010287
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0001208F File Offset: 0x0001028F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.AnyAvailableMissionControl, TIUtilities.GetBoolValue(this.strValue));
	}
}
