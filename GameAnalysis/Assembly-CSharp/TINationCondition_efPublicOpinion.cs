using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200005B RID: 91
public class TINationCondition_efPublicOpinion : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000267 RID: 615 RVA: 0x00010B56 File Offset: 0x0000ED56
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TemplateManager.Find<TIFactionIdeologyTemplate>(this.strIdx, false).ideologyStrPublicOpinion,
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00010B84 File Offset: 0x0000ED84
	public override bool PassesCondition(TIGameState state)
	{
		FactionIdeology factionIdeology = this.strIdx.ToEnum(FactionIdeology.None);
		if (state.ref_nation != null)
		{
			if ((from x in GameStateManager.ActiveIdeologies()
				select x.ideology).Contains(factionIdeology))
			{
				return TICondition.PassesComparison(this.sign, state.ref_nation.GetPublicOpinionOfFaction(factionIdeology), TIUtilities.GetFloatValue(this.strValue));
			}
		}
		return false;
	}
}
