using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000093 RID: 147
public class TIFactionCondition_bAlienFaction : TIFactionCondition
{
	// Token: 0x060002FE RID: 766 RVA: 0x00011D42 File Offset: 0x0000FF42
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060002FF RID: 767 RVA: 0x00011D4A File Offset: 0x0000FF4A
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { GameStateManager.AlienFaction().GetDisplayName(GameControl.control.activePlayer) };
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00011D6C File Offset: 0x0000FF6C
	public override bool PassesCondition(TIGameState state)
	{
		return GameStateManager.CampaignHasAlienFaction() && TICondition.PassesComparison(this.sign, state.ref_faction.IsAlienFaction, TIUtilities.GetBoolValue(this.strValue));
	}
}
