using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200097A RID: 2426
	public class HullSection : IHullSection, IComponent
	{
		// Token: 0x17000FC8 RID: 4040
		// (get) Token: 0x06005C51 RID: 23633 RVA: 0x002C0B15 File Offset: 0x002BED15
		// (set) Token: 0x06005C52 RID: 23634 RVA: 0x002C0B1D File Offset: 0x002BED1D
		public ComponentMap map { get; private set; }

		// Token: 0x06005C53 RID: 23635 RVA: 0x002C0B26 File Offset: 0x002BED26
		public HullSection(TISpaceShipState state, Facing facing)
		{
			this.shipState = state;
			this.AddFacing(facing);
		}

		// Token: 0x06005C54 RID: 23636 RVA: 0x002C0B3C File Offset: 0x002BED3C
		public HullSection(TISpaceShipState state)
		{
			this.shipState = state;
			this.map = ComponentMap.single;
		}

		// Token: 0x06005C55 RID: 23637 RVA: 0x002C0B58 File Offset: 0x002BED58
		public void AddFacing(Facing facing)
		{
			if (this.facings == null)
			{
				this.facings = new List<Facing>();
			}
			if (this.facings.Contains(facing))
			{
				Error.Log("Tried to add duplicate facing: {0}", new object[] { facing });
				return;
			}
			this.facings.Add(facing);
		}

		// Token: 0x06005C56 RID: 23638 RVA: 0x002C0BAC File Offset: 0x002BEDAC
		public bool Contains(float angle)
		{
			return this.facings.Any<Facing>((Facing facing) => facing.Contains(angle));
		}

		// Token: 0x06005C57 RID: 23639 RVA: 0x002C0BE0 File Offset: 0x002BEDE0
		public Damage ApplyDamage(Damage damage, float angle, out float internalDamageApplied)
		{
			Facing facing = this.facings.Single<Facing>((Facing f) => f.Contains(angle));
			internalDamageApplied = 0f;
			if (Error.IsNull<Facing>(facing))
			{
				return Damage.None;
			}
			float amount = damage.amount;
			float num;
			this.shipState.ApplyDamage(damage.weapon, facing.armorFacing, damage.range_km, amount, damage.chippingAmount, damage.type, angle, damage.applyingFaction, out internalDamageApplied, out num, damage.shreddingAmount);
			GameControl.eventManager.TriggerEvent(new ShipArmorFacingStruckInCombat(this.shipState, facing.armorFacing, damage.weapon, amount, internalDamageApplied, num), null, new object[] { this.shipState });
			return Damage.None;
		}

		// Token: 0x040041DE RID: 16862
		public IList<Facing> facings;

		// Token: 0x040041DF RID: 16863
		public TISpaceShipState shipState;
	}
}
