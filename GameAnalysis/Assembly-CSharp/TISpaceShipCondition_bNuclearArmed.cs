using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000131 RID: 305
public class TISpaceShipCondition_bNuclearArmed : TISpaceShipCondition
{
	// Token: 0x060004A4 RID: 1188 RVA: 0x000157E0 File Offset: 0x000139E0
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x000157E8 File Offset: 0x000139E8
	public override bool PassesCondition(TIGameState state)
	{
		IEnumerable<ModuleDataEntry> enumerable = from x in state.ref_ship.NuclearWeaponModuleData()
			where state.ref_ship.WeaponHasAmmo(x)
			select x;
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, enumerable.Count<ModuleDataEntry>() > 0, TIUtilities.GetBoolValue(this.strValue));
	}
}
