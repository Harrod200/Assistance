using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012E RID: 302
public class TISpaceShipCondition_tiNumberOfUtilityModulesType : TISpaceShipCondition_Numeric
{
	// Token: 0x17000095 RID: 149
	// (get) Token: 0x0600049C RID: 1180 RVA: 0x000156BD File Offset: 0x000138BD
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TIUtilities.GetTemplateValue<TIUtilityModuleTemplate>(this.strIdx).displayName,
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x000156E8 File Offset: 0x000138E8
	public override bool PassesCondition(TIGameState state)
	{
		TIUtilityModuleTemplate template = TIUtilities.GetTemplateValue<TIUtilityModuleTemplate>(this.strIdx);
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.GetFunctionalUtilitySlotModuleTemplates(1f).Count<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x == template), TIUtilities.GetIntValue(this.strValue));
	}
}
