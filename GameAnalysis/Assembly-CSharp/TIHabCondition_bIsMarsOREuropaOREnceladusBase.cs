using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000117 RID: 279
public class TIHabCondition_bIsMarsOREuropaOREnceladusBase : TIHabCondition
{
	// Token: 0x06000465 RID: 1125 RVA: 0x0001502A File Offset: 0x0001322A
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00015034 File Offset: 0x00013234
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && (TICondition.PassesComparison(this.sign, state.ref_hab.habSite.parentBody.displayName == "Mars", TIUtilities.GetBoolValue(this.strValue)) || TICondition.PassesComparison(this.sign, state.ref_hab.habSite.parentBody.displayName == "Europa", TIUtilities.GetBoolValue(this.strValue)) || TICondition.PassesComparison(this.sign, state.ref_hab.habSite.parentBody.displayName == "Enceladus", TIUtilities.GetBoolValue(this.strValue)));
	}
}
