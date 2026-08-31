using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000095 RID: 149
public class TIFactionCondition_bAlienAppeaser : TIFactionCondition
{
	// Token: 0x06000306 RID: 774 RVA: 0x00011DFE File Offset: 0x0000FFFE
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000307 RID: 775 RVA: 0x00011E06 File Offset: 0x00010006
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { GameStateManager.AlienAppeaser().GetDisplayName(GameControl.control.activePlayer) };
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00011E28 File Offset: 0x00010028
	public override bool PassesCondition(TIGameState state)
	{
		return GameStateManager.CampaignHasAlienAppeaser() && TICondition.PassesComparison(this.sign, state.ref_faction.isAlienAppeaser, TIUtilities.GetBoolValue(this.strValue));
	}
}
