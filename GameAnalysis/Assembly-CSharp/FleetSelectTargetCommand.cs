using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000386 RID: 902
public class FleetSelectTargetCommand : TIFleetCommandTemplate, IFleetCommandWithTarget
{
	// Token: 0x0600105A RID: 4186 RVA: 0x00054CBC File Offset: 0x00052EBC
	public override int IconPosition()
	{
		return 0;
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00054CBF File Offset: 0x00052EBF
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(SelectTargetCommand)).GetTemplate();
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x00054CEF File Offset: 0x00052EEF
	public override bool RequiresTarget()
	{
		return true;
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00054CF2 File Offset: 0x00052EF2
	public bool IncludeFriendlyTargets()
	{
		return (this.GetShipCommandTemplate() as IShipCommandWithTarget).IncludeFriendlyTargets();
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x00054D04 File Offset: 0x00052F04
	public bool OnlyFriendlyTargets()
	{
		return (this.GetShipCommandTemplate() as IShipCommandWithTarget).OnlyFriendlyTargets();
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x00054D16 File Offset: 0x00052F16
	public Type GetTargetingMethod()
	{
		return typeof(TICommandTargetableTargeting);
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00054D24 File Offset: 0x00052F24
	public void InitiateTargeting(List<TISpaceShipState> ships)
	{
		ships = ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.ShipDestroyed()).ToList<TISpaceShipState>();
		TICommandTargeting ticommandTargeting = Activator.CreateInstance(this.GetTargetingMethod()) as TICommandTargeting;
		ticommandTargeting.Initialize(ships, this);
		this.ships = ships;
		GeneralControlsController.SetUIGlobalTargetingMode(ships[0], ticommandTargeting);
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00054D8A File Offset: 0x00052F8A
	public void EndTargeting(TIFactionState faction)
	{
		GeneralControlsController.ShutdownUIGlobalTargetingMode(faction);
	}

	// Token: 0x040010B7 RID: 4279
	public List<TISpaceShipState> ships;
}
