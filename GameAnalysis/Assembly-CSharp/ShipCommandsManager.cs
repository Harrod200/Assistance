using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000382 RID: 898
public static class ShipCommandsManager
{
	// Token: 0x06001037 RID: 4151 RVA: 0x000545E0 File Offset: 0x000527E0
	public static void Initialize()
	{
		ShipCommandsManager.shipCommands.Clear();
		ShipCommandsManager.fleetCommands.Clear();
		ShipCommandsManager.maneuverIcons.Clear();
		ShipCommandsManager.shipCommands.Add(new SelectTargetCommand());
		ShipCommandsManager.shipCommands.Add(new ClearTargetCommand());
		ShipCommandsManager.shipCommands.Add(new SelectSalvoTargetCommand());
		ShipCommandsManager.shipCommands.Add(new ExtendRadiatorsCommand());
		ShipCommandsManager.shipCommands.Add(new RetractRadiatorsCommand());
		ShipCommandsManager.shipCommands.Add(new RammingSpeedCommand());
		ShipCommandsManager.shipCommands.Add(new CancelRammingSpeedCommand());
		ShipCommandsManager.shipCommands.Add(new SetAIControlCommand());
		ShipCommandsManager.shipCommands.Add(new ReleaseAIControlCommand());
		ShipCommandsManager.shipCommands.Add(new DisengageCommand());
		ShipCommandsManager.shipCommands.Add(new CancelDisengageCommand());
		ShipCommandsManager.shipCommands.Add(new FocusFireCommand());
		ShipCommandsManager.shipCommands.Add(new AttackCommand());
		ShipCommandsManager.shipCommands.Add(new BalancedCommand());
		ShipCommandsManager.shipCommands.Add(new DefensiveCommand());
		ShipCommandsManager.shipCommands.Add(new FortifyCommand());
		ShipCommandsManager.shipCommands.Add(new FullSpeedAheadCommand());
		ShipCommandsManager.shipCommands.Add(new InterceptCourseCommand());
		ShipCommandsManager.shipCommands.Add(new MatchVelocityCommand());
		ShipCommandsManager.shipCommands.Add(new CancelMatchVelocityCommand());
		ShipCommandsManager.shipCommands.Add(new FaceVelocityVectorCommand());
		ShipCommandsManager.shipCommands.Add(new DefensiveManeuversCommand());
		ShipCommandsManager.shipCommands.Add(new CancelDefensiveManeuversCommand());
		ShipCommandsManager.shipCommands.Add(new SpinPortCommand());
		ShipCommandsManager.shipCommands.Add(new SpinStarboardCommand());
		ShipCommandsManager.shipCommands.Add(new CancelSpinPortCommand());
		ShipCommandsManager.shipCommands.Add(new CancelSpinStarboardCommand());
		ShipCommandsManager.shipCommands.Add(new AllStopCommand());
		ShipCommandsManager.shipCommands.Add(new CancelAllStopCommand());
		ShipCommandsManager.shipCommands.Add(new PadlockPrimaryTargetCommand());
		ShipCommandsManager.shipCommands.Add(new CancelPadlockPrimaryTargetCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetSelectTargetCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetClearTargetCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetSelectSalvoTargetCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetExtendRadiatorsCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetRetractRadiatorsCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetSetAIControlCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetReleaseAIControlCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetFocusFireCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetAttackCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetBalancedCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetDefensiveCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetFortifyCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetFullSpeedAheadCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetInterceptCourseCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetMatchVelocityCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetCancelMatchVelocityCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetFaceVelocityVectorCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetDefensiveManeuversCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetCancelDefensiveManeuversCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetSpinPortCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetSpinStarboardCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetCancelSpinPortCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetCancelSpinStarboardCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetAllStopCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetCancelAllStopCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetPadlockPrimaryTargetCommand());
		ShipCommandsManager.fleetCommands.Add(new FleetCancelPadlockPrimaryTargetCommand());
		foreach (IShipCommand shipCommand in ShipCommandsManager.shipCommands.Where<IShipCommand>((IShipCommand x) => x is TIShipManeuverCommandTemplate))
		{
			TIShipManeuverCommandTemplate tishipManeuverCommandTemplate = shipCommand.GetTemplate() as TIShipManeuverCommandTemplate;
			ShipCommandsManager.maneuverIcons.Add(tishipManeuverCommandTemplate.Maneuver(), GameControl.assetLoader.LoadAsset<Sprite>(tishipManeuverCommandTemplate.GetCommandIconImagePath_Off()));
		}
	}

	// Token: 0x040010B4 RID: 4276
	public static List<IShipCommand> shipCommands = new List<IShipCommand>();

	// Token: 0x040010B5 RID: 4277
	public static List<IFleetCommand> fleetCommands = new List<IFleetCommand>();

	// Token: 0x040010B6 RID: 4278
	public static Dictionary<CombatManeuver, Sprite> maneuverIcons = new Dictionary<CombatManeuver, Sprite>();
}
