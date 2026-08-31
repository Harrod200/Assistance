using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000129 RID: 297
public class TISpaceShipCondition_bIsAlien : TISpaceShipCondition
{
	// Token: 0x0600048C RID: 1164 RVA: 0x000154AC File Offset: 0x000136AC
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x000154B4 File Offset: 0x000136B4
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.isAlien || state.ref_ship.faction.IsAlienFaction, TIUtilities.GetBoolValue(this.strValue));
	}
}
