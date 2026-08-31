using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000394 RID: 916
public abstract class TIFleetManueverCommandTemplate_Spin : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010AC RID: 4268
	public abstract CombatManeuver OppositeManeuver();

	// Token: 0x060010AD RID: 4269
	public abstract CombatManeuver CancelManeuver();

	// Token: 0x060010AE RID: 4270
	public abstract CombatManeuver CancelOppositeManeuver();

	// Token: 0x060010AF RID: 4271 RVA: 0x00055599 File Offset: 0x00053799
	public List<CombatManeuver> RestrictedManeuvers()
	{
		return new List<CombatManeuver>
		{
			this.Maneuver(),
			this.OppositeManeuver(),
			this.CancelOppositeManeuver()
		};
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x000555C4 File Offset: 0x000537C4
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.PerformingCombatManeuver(this.Maneuver()) && !x.PerformingCombatManeuver(this.CancelManeuver()));
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x000555D8 File Offset: 0x000537D8
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && !x.PerformingCombatManeuver(this.RestrictedManeuvers()) && !x.PerformingCombatManeuver(TIFleetManeuverCommandTemplate.exclusiveManeuvers) && x.CanRotateAndRoll());
	}
}
