using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000041 RID: 65
public class TINationCondition_fChangePerCapitaGDP : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600022B RID: 555 RVA: 0x000102D6 File Offset: 0x0000E4D6
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.perCapitaGDPInlineSpritePath;
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x000102E4 File Offset: 0x0000E4E4
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			float floatValue = TIUtilities.GetFloatValue(this.strValue);
			for (int i = 1; i <= 31; i++)
			{
				if (TICondition.PassesComparison(this.sign, state.ref_nation.perCapitaGDP - state.ref_nation.HistoryPerCapitaGDP(i), floatValue))
				{
					return true;
				}
			}
		}
		return false;
	}
}
