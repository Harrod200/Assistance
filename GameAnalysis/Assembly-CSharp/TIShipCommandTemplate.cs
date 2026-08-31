using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A6 RID: 934
public abstract class TIShipCommandTemplate : TIDataTemplate, IShipCommand
{
	// Token: 0x06001130 RID: 4400 RVA: 0x00055EA6 File Offset: 0x000540A6
	public string GetDisplayName()
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName").ToString());
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x00055ECC File Offset: 0x000540CC
	public virtual string GetDescription(TISpaceShipState ship = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString());
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x00055EF2 File Offset: 0x000540F2
	public virtual string GetTooltipText(TISpaceShipState ship = null)
	{
		return new StringBuilder(this.GetDisplayName()).AppendLine().AppendLine(this.GetDescription(ship)).ToString();
	}

	// Token: 0x06001133 RID: 4403
	public abstract int IconPosition();

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001134 RID: 4404 RVA: 0x00055F15 File Offset: 0x00054115
	public string commandIconImagePath
	{
		get
		{
			return new StringBuilder("ui_spacecombat/ICO_").Append(base.GetType().Name).ToString();
		}
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x00055F36 File Offset: 0x00054136
	public virtual string GetCommandIconImagePath_On()
	{
		return new StringBuilder(this.commandIconImagePath).Append("_on").ToString();
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x00055F52 File Offset: 0x00054152
	public string GetCommandIconImagePath_Off()
	{
		return new StringBuilder(this.commandIconImagePath).Append("_off").ToString();
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x00055F6E File Offset: 0x0005416E
	public virtual bool RequiresTarget()
	{
		return false;
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x00055F71 File Offset: 0x00054171
	public TIShipCommandTemplate GetTemplate()
	{
		return this;
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x00055F74 File Offset: 0x00054174
	public virtual bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return !ship.combatAIControl && ship.CanPerformShipCommands();
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x00055F86 File Offset: 0x00054186
	public virtual bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return ship.CanPerformShipCommands();
	}

	// Token: 0x0600113B RID: 4411
	public abstract void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null);

	// Token: 0x0600113C RID: 4412 RVA: 0x00055F8E File Offset: 0x0005418E
	public virtual TIResourcesCost GetResourcesCost(TISpaceShipState ship)
	{
		return null;
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x0600113D RID: 4413 RVA: 0x00055F91 File Offset: 0x00054191
	public virtual bool TriggersManeuver
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x00055F94 File Offset: 0x00054194
	public TIShipCommandTemplate()
	{
		base.dataName = base.GetType().ToString();
		this._displayName = this.GetDisplayName();
		base.friendlyName = this._displayName;
		TemplateManager.Add(this, typeof(TIFleetCommandTemplate), false);
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x00055FE1 File Offset: 0x000541E1
	public void OnExecuteCommand(TISpaceShipState ship)
	{
		GameControl.eventManager.TriggerEvent(new ShipCommandExecuted(ship, this), null, new object[] { ship });
	}
}
