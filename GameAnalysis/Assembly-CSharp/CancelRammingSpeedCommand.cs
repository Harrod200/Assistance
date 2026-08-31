using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003AE RID: 942
public class CancelRammingSpeedCommand : TIShipCommandTemplate
{
	// Token: 0x06001170 RID: 4464 RVA: 0x00056441 File Offset: 0x00054641
	public override int IconPosition()
	{
		return 9;
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06001171 RID: 4465 RVA: 0x00056445 File Offset: 0x00054645
	public override bool TriggersManeuver
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x00056448 File Offset: 0x00054648
	public override string GetDescription(TISpaceShipState ship = null)
	{
		if (ship == null)
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { "" });
		}
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { ship.GetRammingSpeedCost().ToString("Relevant", false, false, null, false, FactionResource.Influence) });
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x000564D3 File Offset: 0x000546D3
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return base.CommandVisibleToActor(ship) && !ship.disengageFromCombat && ship.canSuicide;
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x000564EE File Offset: 0x000546EE
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && !ship.disengageFromCombat && ship.canSuicide;
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x00056509 File Offset: 0x00054709
	public override TIResourcesCost GetResourcesCost(TISpaceShipState ship)
	{
		return ship.GetRammingSpeedCost();
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x00056511 File Offset: 0x00054711
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetRammingSpeedAction(ship, false));
		base.OnExecuteCommand(ship);
	}
}
