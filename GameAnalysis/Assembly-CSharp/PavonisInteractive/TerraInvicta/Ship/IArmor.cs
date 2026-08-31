using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000972 RID: 2418
	public interface IArmor
	{
		// Token: 0x06005C24 RID: 23588
		Damage ApplyDamage(Damage damage);

		// Token: 0x17000FB9 RID: 4025
		// (get) Token: 0x06005C25 RID: 23589
		CombatShipController ShipController { get; }
	}
}
