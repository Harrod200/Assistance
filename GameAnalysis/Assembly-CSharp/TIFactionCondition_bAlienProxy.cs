using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000094 RID: 148
public class TIFactionCondition_bAlienProxy : TIFactionCondition
{
	// Token: 0x06000302 RID: 770 RVA: 0x00011DA0 File Offset: 0x0000FFA0
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000303 RID: 771 RVA: 0x00011DA8 File Offset: 0x0000FFA8
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { GameStateManager.AlienProxy().GetDisplayName(GameControl.control.activePlayer) };
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00011DCA File Offset: 0x0000FFCA
	public override bool PassesCondition(TIGameState state)
	{
		return GameStateManager.CampaignHasAlienProxy() && TICondition.PassesComparison(this.sign, state.ref_faction.IsAlienProxy, TIUtilities.GetBoolValue(this.strValue));
	}
}
