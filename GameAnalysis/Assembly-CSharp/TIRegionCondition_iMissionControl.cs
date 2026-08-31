using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200007E RID: 126
public class TIRegionCondition_iMissionControl : TIRegionCondition_Numeric_Symbol
{
	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060002CE RID: 718 RVA: 0x0001168A File Offset: 0x0000F88A
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.missionControlInlineSpritePath;
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00011696 File Offset: 0x0000F896
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.missionControl, TIUtilities.GetIntValue(this.strValue));
	}
}
