using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B6 RID: 182
public class TIFactionCondition_HasControlPointofTypeForMapRegion : TIFactionCondition
{
	// Token: 0x06000372 RID: 882 RVA: 0x00012BAC File Offset: 0x00010DAC
	public override bool PassesCondition(TIGameState state)
	{
		TIRegionState tiregionState = GameStateManager.MapRegionLookup(this.strIdx);
		ControlPointType controlPointType;
		Enum.TryParse<ControlPointType>(this.strValue, out controlPointType);
		TIControlPoint controlPointOfType = tiregionState.nation.GetControlPointOfType(controlPointType);
		return controlPointOfType != null && state.ref_faction != null && TICondition.PassesComparison(this.sign, controlPointOfType.faction == state.ref_faction, true);
	}
}
