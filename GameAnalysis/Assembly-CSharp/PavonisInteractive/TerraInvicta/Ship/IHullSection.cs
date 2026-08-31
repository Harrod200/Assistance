using System;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000979 RID: 2425
	public interface IHullSection : IComponent
	{
		// Token: 0x06005C4E RID: 23630
		bool Contains(float angle);

		// Token: 0x06005C4F RID: 23631
		Damage ApplyDamage(Damage damage, float angle, out float internalDamageApplied);

		// Token: 0x06005C50 RID: 23632
		void AddFacing(Facing facing);
	}
}
