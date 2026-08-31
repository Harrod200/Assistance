using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C5 RID: 1989
	public class PlannedResupplyAndRepair
	{
		// Token: 0x17000DE8 RID: 3560
		// (get) Token: 0x060046D6 RID: 18134 RVA: 0x001CF7AD File Offset: 0x001CD9AD
		public float duration_days
		{
			get
			{
				return this.resupplyCost.completionTime_days + this.repairCost.completionTime_days;
			}
		}

		// Token: 0x060046D7 RID: 18135 RVA: 0x001CF7C8 File Offset: 0x001CD9C8
		public void SetStartDate(TIDateTime startDate)
		{
			if (this.startDate == null || this.startDate < TITimeState.Now() || this.startDate < startDate)
			{
				if (startDate < TITimeState.Now())
				{
					startDate = TITimeState.Now();
				}
				this.startDate = new TIDateTime(startDate);
			}
		}

		// Token: 0x060046D8 RID: 18136 RVA: 0x001CF823 File Offset: 0x001CDA23
		public void AddtoResupplyCost(TIResourcesCost totalCost)
		{
			this.resupplyCost.SumCostsWithDuration(totalCost);
		}

		// Token: 0x060046D9 RID: 18137 RVA: 0x001CF831 File Offset: 0x001CDA31
		public void AddtoRepairCost(TIResourcesCost totalCost)
		{
			this.repairCost.SumCostsWithDuration(totalCost);
		}

		// Token: 0x060046DA RID: 18138 RVA: 0x001CF840 File Offset: 0x001CDA40
		public bool OnlyRefueling(bool freeOnly)
		{
			return this.propellantToReload > 0f && this.shipSystemsToRepair.Count == 0 && this.modulesToRepair.Count == 0 && (this.ammoToReload.Count == 0 || this.ammoToReload.Values.Sum() == 0) && this.armorToRepair.Count == 0 && (!freeOnly || (!this.resupplyCost.anyDebit && !this.repairCost.anyCredit));
		}

		// Token: 0x060046DB RID: 18139 RVA: 0x001CF8C4 File Offset: 0x001CDAC4
		public void AddPropellantToReload(float propellant_tons)
		{
			this.propellantToReload += propellant_tons;
			this.active = true;
		}

		// Token: 0x060046DC RID: 18140 RVA: 0x001CF8DB File Offset: 0x001CDADB
		public void AddSystemToRepair(ShipSystem system)
		{
			this.shipSystemsToRepair.Add(system);
			this.active = true;
		}

		// Token: 0x060046DD RID: 18141 RVA: 0x001CF8F0 File Offset: 0x001CDAF0
		public void AddModuleToRepair(DamagedShipPartData damagedPart)
		{
			this.modulesToRepair.Add(damagedPart);
			this.active = true;
		}

		// Token: 0x060046DE RID: 18142 RVA: 0x001CF908 File Offset: 0x001CDB08
		public void AddAmmoOrder(ModuleDataEntry weapon, int valueToReload)
		{
			if (!this.ammoToReload.ContainsKey(weapon))
			{
				this.ammoToReload.Add(weapon, 0);
			}
			Dictionary<ModuleDataEntry, int> dictionary = this.ammoToReload;
			dictionary[weapon] += valueToReload;
			if (valueToReload > 0)
			{
				this.active = true;
			}
		}

		// Token: 0x060046DF RID: 18143 RVA: 0x001CF954 File Offset: 0x001CDB54
		public void AddArmorFacingToRepair(ArmorFacing facing)
		{
			this.armorToRepair.Add(facing);
			this.active = true;
		}

		// Token: 0x060046E0 RID: 18144 RVA: 0x001CF96C File Offset: 0x001CDB6C
		public void ProcessResupplyAndRepair(TISpaceShipState ship)
		{
			if (TIGameState.Valid(ship))
			{
				foreach (ShipSystem shipSystem in this.shipSystemsToRepair)
				{
					ship.RepairSystem(shipSystem);
				}
				foreach (DamagedShipPartData damagedShipPartData in this.modulesToRepair)
				{
					ship.RepairPart(damagedShipPartData);
				}
				foreach (ArmorFacing armorFacing in this.armorToRepair)
				{
					ship.RepairArmorFacing(armorFacing);
				}
				ship.RefuelPropellant(this.propellantToReload);
				foreach (ModuleDataEntry moduleDataEntry in this.ammoToReload.Keys)
				{
					Dictionary<ModuleDataEntry, int> ammo = ship.ammo;
					ModuleDataEntry moduleDataEntry2 = moduleDataEntry;
					ammo[moduleDataEntry2] += this.ammoToReload[moduleDataEntry];
					if (ship.ammo[moduleDataEntry] > moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_Current(ship))
					{
						ship.ammo[moduleDataEntry] = moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_Current(ship);
						Log.Warn("Attempted to load too much ammo", Array.Empty<object>());
					}
				}
				ship.visualizerLink.ModelController.OnWeaponsRepaired();
				ship.ClearShipDamageVisualizations();
			}
			this.ClearAllResupplyAndRepair();
		}

		// Token: 0x060046E1 RID: 18145 RVA: 0x001CFB3C File Offset: 0x001CDD3C
		public void CancelResupply(TIFactionState faction)
		{
			this.ammoToReload.Clear();
			this.propellantToReload = 0f;
			if (this.active)
			{
				this.resupplyCost.RefundCost(faction, "Resupply Cancel");
			}
			this.resupplyCost = new TIResourcesCost();
			this.active = this.active && (this.shipSystemsToRepair.Count > 0 || this.armorToRepair.Count > 0 || this.modulesToRepair.Count > 0);
			if (!this.active)
			{
				this.startDate = null;
			}
		}

		// Token: 0x060046E2 RID: 18146 RVA: 0x001CFBD0 File Offset: 0x001CDDD0
		public void CancelRepair(TIFactionState faction)
		{
			this.shipSystemsToRepair.Clear();
			this.modulesToRepair.Clear();
			this.armorToRepair.Clear();
			if (this.active)
			{
				this.repairCost.RefundCost(faction, "Repair Cancel");
			}
			this.repairCost = new TIResourcesCost();
			this.active = this.active && (this.ammoToReload.Count > 0 || this.propellantToReload > 0f);
			if (!this.active)
			{
				this.startDate = null;
			}
		}

		// Token: 0x060046E3 RID: 18147 RVA: 0x001CFC60 File Offset: 0x001CDE60
		public void ClearAllResupplyAndRepair()
		{
			this.shipSystemsToRepair.Clear();
			this.modulesToRepair.Clear();
			this.propellantToReload = 0f;
			this.ammoToReload.Clear();
			this.armorToRepair.Clear();
			this.resupplyCost = new TIResourcesCost();
			this.repairCost = new TIResourcesCost();
			this.startDate = null;
			this.active = false;
		}

		// Token: 0x04002927 RID: 10535
		public TISpaceShipState ship;

		// Token: 0x04002928 RID: 10536
		public TIResourcesCost resupplyCost = new TIResourcesCost();

		// Token: 0x04002929 RID: 10537
		public TIResourcesCost repairCost = new TIResourcesCost();

		// Token: 0x0400292A RID: 10538
		public TIDateTime startDate;

		// Token: 0x0400292B RID: 10539
		public List<ShipSystem> shipSystemsToRepair = new List<ShipSystem>();

		// Token: 0x0400292C RID: 10540
		public List<DamagedShipPartData> modulesToRepair = new List<DamagedShipPartData>();

		// Token: 0x0400292D RID: 10541
		public float propellantToReload;

		// Token: 0x0400292E RID: 10542
		public Dictionary<ModuleDataEntry, int> ammoToReload = new Dictionary<ModuleDataEntry, int>();

		// Token: 0x0400292F RID: 10543
		public List<ArmorFacing> armorToRepair = new List<ArmorFacing>();

		// Token: 0x04002930 RID: 10544
		public bool active;
	}
}
