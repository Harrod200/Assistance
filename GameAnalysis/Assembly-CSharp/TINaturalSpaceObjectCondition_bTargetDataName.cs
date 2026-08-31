using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200011B RID: 283
public class TINaturalSpaceObjectCondition_bTargetDataName : TINaturalSpaceObjectCondition
{
	// Token: 0x0600046E RID: 1134 RVA: 0x00015184 File Offset: 0x00013384
	public override bool PassesCondition(TIGameState state)
	{
		bool flag = state.ref_naturalSpaceObject != null;
		if (flag)
		{
			return TICondition.PassesComparison(this.sign, state.ref_naturalSpaceObject.templateName, this.strIdx);
		}
		return flag;
	}
}
