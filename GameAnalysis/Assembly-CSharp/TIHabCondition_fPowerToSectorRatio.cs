using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000111 RID: 273
public class TIHabCondition_fPowerToSectorRatio : TIHabCondition_Numeric
{
	// Token: 0x06000454 RID: 1108 RVA: 0x00014D54 File Offset: 0x00012F54
	public override bool PassesCondition(TIGameState state)
	{
		float num = 0f;
		foreach (TISectorState tisectorState in state.ref_hab.sectors)
		{
			num += (float)tisectorState.SectorPowerGeneration;
		}
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, num / (float)state.ref_hab.sectors.Count, TIUtilities.GetFloatValue(this.strValue));
	}
}
