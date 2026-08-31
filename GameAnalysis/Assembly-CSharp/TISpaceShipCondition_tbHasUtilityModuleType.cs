using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012D RID: 301
public class TISpaceShipCondition_tbHasUtilityModuleType : TISpaceShipCondition
{
	// Token: 0x06000498 RID: 1176 RVA: 0x00015638 File Offset: 0x00013838
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000499 RID: 1177 RVA: 0x00015640 File Offset: 0x00013840
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIUtilityModuleTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00015660 File Offset: 0x00013860
	public override bool PassesCondition(TIGameState state)
	{
		TIUtilityModuleTemplate templateValue = TIUtilities.GetTemplateValue<TIUtilityModuleTemplate>(this.strIdx);
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.GetFunctionalUtilitySlotModuleTemplates(1f).Contains(templateValue), TIUtilities.GetBoolValue(this.strValue));
	}
}
