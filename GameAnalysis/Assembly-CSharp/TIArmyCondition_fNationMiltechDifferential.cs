using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class TIArmyCondition_fNationMiltechDifferential : TIArmyCondition_Numeric
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060004E2 RID: 1250 RVA: 0x00015DDE File Offset: 0x00013FDE
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.miltechInlineSpritePath;
		}
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00015DEC File Offset: 0x00013FEC
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && state.ref_nation != null && TICondition.PassesComparison(this.sign, Mathf.Abs(state.ref_army.techLevel - state.ref_nation.militaryTechLevel), TIUtilities.GetFloatValue(this.strValue));
	}
}
