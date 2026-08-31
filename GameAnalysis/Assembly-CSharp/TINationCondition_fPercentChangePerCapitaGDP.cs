using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000042 RID: 66
public class TINationCondition_fPercentChangePerCapitaGDP : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600022E RID: 558 RVA: 0x00010349 File Offset: 0x0000E549
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.perCapitaGDPInlineSpritePath;
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00010358 File Offset: 0x0000E558
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			float floatValue = TIUtilities.GetFloatValue(this.strValue);
			for (int i = 1; i <= 31; i++)
			{
				if (TICondition.PassesComparison(this.sign, (state.ref_nation.perCapitaGDP - state.ref_nation.HistoryPerCapitaGDP(i)) / state.ref_nation.HistoryPerCapitaGDP(i), floatValue))
				{
					return true;
				}
			}
		}
		return false;
	}
}
