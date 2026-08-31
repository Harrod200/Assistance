using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003AD RID: 941
public class RammingSpeedCommand : TIShipCommandTemplate
{
	// Token: 0x06001168 RID: 4456 RVA: 0x0005630B File Offset: 0x0005450B
	public override int IconPosition()
	{
		return 9;
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001169 RID: 4457 RVA: 0x0005630F File Offset: 0x0005450F
	public override bool TriggersManeuver
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x00056314 File Offset: 0x00054514
	public override string GetDescription(TISpaceShipState ship = null)
	{
		if (ship == null)
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { "" });
		}
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { ship.GetRammingSpeedCost().ToString("Relevant", false, false, null, false, FactionResource.Influence) });
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0005639F File Offset: 0x0005459F
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return base.CommandVisibleToActor(ship) && !ship.disengageFromCombat && !ship.canSuicide;
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x000563BD File Offset: 0x000545BD
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && !ship.disengageFromCombat && this.GetResourcesCost(ship).CanAfford(ship.faction, 1f, null, float.PositiveInfinity) && !ship.canSuicide;
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x000563FA File Offset: 0x000545FA
	public override TIResourcesCost GetResourcesCost(TISpaceShipState ship)
	{
		return ship.GetRammingSpeedCost();
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x00056402 File Offset: 0x00054602
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetRammingSpeedAction(ship, true));
		this.GetResourcesCost(ship).PayCost(ship.faction, "Ramming");
		base.OnExecuteCommand(ship);
	}
}
