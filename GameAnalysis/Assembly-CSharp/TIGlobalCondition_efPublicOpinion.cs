using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D3 RID: 211
public class TIGlobalCondition_efPublicOpinion : TIGlobalCondition
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060003B5 RID: 949 RVA: 0x000134CB File Offset: 0x000116CB
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

	// Token: 0x060003B6 RID: 950 RVA: 0x000134F8 File Offset: 0x000116F8
	public override bool PassesCondition(TIGameState state)
	{
		FactionIdeology factionIdeology = this.strIdx.ToEnum(FactionIdeology.None);
		Dictionary<FactionIdeology, float> globalPublicOpinionProportions = TIGlobalValuesState.GlobalValues.GetGlobalPublicOpinionProportions();
		float num;
		return (from x in GameStateManager.ActiveIdeologies()
			select x.ideology).Contains(factionIdeology) && globalPublicOpinionProportions.TryGetValue(factionIdeology, out num) && TICondition.PassesComparison(this.sign, num, TIUtilities.GetFloatValue(this.strValue));
	}
}
