using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200017B RID: 379
public struct PropellantGroup
{
	// Token: 0x06000566 RID: 1382 RVA: 0x00018008 File Offset: 0x00016208
	public PropellantGroup(List<TISpaceShipState> ships)
	{
		this.ships = ships;
		this.propellant = ships[0].propellant;
		this.propellantComposition = ships[0].drive.GetPerTankPropellantMaterials(ships[0].faction).ToRVCollection(1f);
		this.str = ships[0].drive.PropellantIcons(true, ships[0].faction);
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x00018082 File Offset: 0x00016282
	public override string ToString()
	{
		return new StringBuilder(TIDriveTemplate.propellantStr(this.propellant)).Append(this.str).ToString();
	}

	// Token: 0x04000551 RID: 1361
	public List<TISpaceShipState> ships;

	// Token: 0x04000552 RID: 1362
	public Propellant propellant;

	// Token: 0x04000553 RID: 1363
	public Dictionary<FactionResource, float> propellantComposition;

	// Token: 0x04000554 RID: 1364
	private string str;
}
