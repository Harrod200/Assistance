using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003BB RID: 955
public abstract class TIShipManueverCommandTemplate_Spin : TIShipManeuverCommandTemplate
{
	// Token: 0x060011B0 RID: 4528
	public abstract CombatManeuver OppositeManeuver();

	// Token: 0x060011B1 RID: 4529
	public abstract CombatManeuver CancelManeuver();

	// Token: 0x060011B2 RID: 4530
	public abstract CombatManeuver CancelOppositeManeuver();

	// Token: 0x060011B3 RID: 4531 RVA: 0x000570F0 File Offset: 0x000552F0
	public List<CombatManeuver> RestrictedManeuvers()
	{
		return new List<CombatManeuver>
		{
			this.Maneuver(),
			this.OppositeManeuver(),
			this.CancelOppositeManeuver()
		};
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x0005711B File Offset: 0x0005531B
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return !ship.PerformingCombatManeuver(this.Maneuver()) && !ship.PerformingCombatManeuver(this.CancelManeuver());
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x0005713C File Offset: 0x0005533C
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && !ship.PerformingCombatManeuver(this.RestrictedManeuvers()) && !ship.PerformingCombatManeuver(TIShipManeuverCommandTemplate.exclusiveManeuvers) && ship.CanRotateAndRoll();
	}
}
