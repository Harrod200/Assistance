using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000392 RID: 914
public abstract class TIFleetManeuverCommandTemplate : TIFleetCommandTemplate
{
	// Token: 0x060010A3 RID: 4259 RVA: 0x000554B1 File Offset: 0x000536B1
	public override string GetTooltipText(bool isGroupCommand)
	{
		return new StringBuilder(this.GetDisplayName(false)).AppendLine().AppendLine(this.GetDescription(false)).ToString();
	}

	// Token: 0x060010A4 RID: 4260
	public abstract CombatManeuver Maneuver();

	// Token: 0x060010A5 RID: 4261 RVA: 0x000554D5 File Offset: 0x000536D5
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl && x.CanPerformShipCommands());
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000554FC File Offset: 0x000536FC
	public override string CommandIconImagePath()
	{
		return new StringBuilder("ui_spacecombat/ICO_").Append(base.GetType().Name.Remove(0, 5)).ToString();
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x00055524 File Offset: 0x00053724
	public override string GetCommandIconImagePath_On()
	{
		return this.GetCommandIconImagePath_Off();
	}

	// Token: 0x040010B9 RID: 4281
	public static readonly List<CombatManeuver> exclusiveManeuvers = new List<CombatManeuver>
	{
		CombatManeuver.Padlock,
		CombatManeuver.AllStop,
		CombatManeuver.MatchVelocity,
		CombatManeuver.DefensiveManuevers
	};
}
