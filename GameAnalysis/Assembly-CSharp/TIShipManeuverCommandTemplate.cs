using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003B8 RID: 952
public abstract class TIShipManeuverCommandTemplate : TIShipCommandTemplate
{
	// Token: 0x060011A5 RID: 4517
	public abstract CombatManeuver Maneuver();

	// Token: 0x060011A6 RID: 4518 RVA: 0x00057028 File Offset: 0x00055228
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return true;
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0005702B File Offset: 0x0005522B
	public override string GetCommandIconImagePath_On()
	{
		return base.GetCommandIconImagePath_Off();
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060011A8 RID: 4520 RVA: 0x00057033 File Offset: 0x00055233
	public override bool TriggersManeuver
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x00057036 File Offset: 0x00055236
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new AddCombatManeuverAction(ship, this.Maneuver()));
		base.OnExecuteCommand(ship);
	}

	// Token: 0x040010BF RID: 4287
	public static readonly List<CombatManeuver> exclusiveManeuvers = new List<CombatManeuver>
	{
		CombatManeuver.Padlock,
		CombatManeuver.AllStop,
		CombatManeuver.MatchVelocity,
		CombatManeuver.DefensiveManuevers
	};
}
