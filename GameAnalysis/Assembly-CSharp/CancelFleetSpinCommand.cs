using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000397 RID: 919
public abstract class CancelFleetSpinCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010C3 RID: 4291 RVA: 0x000556E8 File Offset: 0x000538E8
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return base.CommandVisibleToPlayer(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.PerformingCombatManeuver(this.CancelManeuver()));
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00055707 File Offset: 0x00053907
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.PerformingCombatManeuver(this.CancelManeuver()) && !x.PerformingCombatManeuver(this.Maneuver()) && x.CanRotateAndRoll());
	}

	// Token: 0x060010C5 RID: 4293
	public abstract CombatManeuver CancelManeuver();
}
