using System;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003E1 RID: 993
public class TIGunTemplate : TIGunTypeWeaponTemplate
{
	// Token: 0x17000290 RID: 656
	// (get) Token: 0x060013A0 RID: 5024 RVA: 0x0005CCFE File Offset: 0x0005AEFE
	public override WeaponClass weaponClass
	{
		get
		{
			return WeaponClass.NavalGun;
		}
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x0005CD01 File Offset: 0x0005AF01
	public override float EnergyUsage_GJ(float extraInput_MW = 0f)
	{
		return 0f;
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x0005CD08 File Offset: 0x0005AF08
	public override float HeatGeneration_GJ(float extraInput_MJ = 0f)
	{
		return 0f;
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x060013A3 RID: 5027 RVA: 0x0005CD0F File Offset: 0x0005AF0F
	public override bool selfPowered
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x0005CD12 File Offset: 0x0005AF12
	public override DamageType GetDamageType()
	{
		return DamageType.Kinetic;
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x060013A5 RID: 5029 RVA: 0x0005CD15 File Offset: 0x0005AF15
	public override bool isNavalGunWeapon
	{
		get
		{
			return true;
		}
	}
}
