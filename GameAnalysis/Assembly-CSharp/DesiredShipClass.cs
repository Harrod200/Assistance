using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200014F RID: 335
public struct DesiredShipClass
{
	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000522 RID: 1314 RVA: 0x00016725 File Offset: 0x00014925
	public TISpaceShipTemplate shipClass
	{
		get
		{
			return TemplateManager.Find<TISpaceShipTemplate>(this.shipDataName, false);
		}
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00016734 File Offset: 0x00014934
	public DesiredShipClass(FactionGoal_Fleet associatedFactionGoal, TISpaceShipTemplate ship)
	{
		this.shipDataName = ship.dataName;
		this.associatedFactionGoal = associatedFactionGoal;
		TISpaceFleetState tispaceFleetState;
		if ((tispaceFleetState = associatedFactionGoal.assignedFleet) == null)
		{
			TIGameState tigameState = associatedFactionGoal.target();
			tispaceFleetState = ((tigameState != null) ? tigameState.ref_spaceObject : null) ?? associatedFactionGoal.location();
		}
		this.destination = tispaceFleetState;
	}

	// Token: 0x04000237 RID: 567
	public string shipDataName;

	// Token: 0x04000238 RID: 568
	public TIGameState destination;

	// Token: 0x04000239 RID: 569
	public FactionGoal_Fleet associatedFactionGoal;
}
