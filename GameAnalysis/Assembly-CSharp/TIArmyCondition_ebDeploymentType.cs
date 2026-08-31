using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000147 RID: 327
public class TIArmyCondition_ebDeploymentType : TIArmyCondition
{
	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060004DE RID: 1246 RVA: 0x00015D6A File Offset: 0x00013F6A
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { this.strIdx };
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00015D7E File Offset: 0x00013F7E
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x00015D88 File Offset: 0x00013F88
	public override bool PassesCondition(TIGameState state)
	{
		DeploymentType deploymentType = this.strIdx.ToEnum(DeploymentType.None);
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.deploymentType == deploymentType, TIUtilities.GetBoolValue(this.strValue));
	}
}
