using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000384 RID: 900
public abstract class TIFleetCommandTemplate : TIDataTemplate, IFleetCommand
{
	// Token: 0x06001045 RID: 4165 RVA: 0x00054A18 File Offset: 0x00052C18
	public virtual string GetDisplayName(bool isGroupCommand = false)
	{
		if (!isGroupCommand)
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName").ToString());
		}
		return Loc.T(new StringBuilder("Group").Append(base.GetType().Name).Append(".displayName").ToString());
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00054A7C File Offset: 0x00052C7C
	public virtual string GetDescription(bool isGroupCommand = false)
	{
		if (!isGroupCommand)
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString());
		}
		return Loc.T(new StringBuilder("Group").Append(base.GetType().Name).Append(".description").ToString());
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00054ADF File Offset: 0x00052CDF
	public virtual string GetTooltipText(bool isGroupCommand = false)
	{
		return new StringBuilder(this.GetDisplayName(isGroupCommand)).AppendLine().AppendLine(this.GetDescription(isGroupCommand)).ToString();
	}

	// Token: 0x06001048 RID: 4168
	public abstract int IconPosition();

	// Token: 0x06001049 RID: 4169 RVA: 0x00054B03 File Offset: 0x00052D03
	public virtual string CommandIconImagePath()
	{
		return new StringBuilder("ui_spacecombat/ICO_").Append(base.GetType().Name).ToString();
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00054B24 File Offset: 0x00052D24
	public virtual string GetCommandIconImagePath_On()
	{
		return new StringBuilder(this.CommandIconImagePath()).Append("_on").ToString();
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00054B40 File Offset: 0x00052D40
	public virtual string GetCommandIconImagePath_Off()
	{
		return new StringBuilder(this.CommandIconImagePath()).Append("_off").ToString();
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00054B5C File Offset: 0x00052D5C
	public virtual bool RequiresTarget()
	{
		return false;
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00054B5F File Offset: 0x00052D5F
	public TIFleetCommandTemplate GetTemplate()
	{
		return this;
	}

	// Token: 0x0600104E RID: 4174
	public abstract TIShipCommandTemplate GetShipCommandTemplate();

	// Token: 0x0600104F RID: 4175 RVA: 0x00054B62 File Offset: 0x00052D62
	public virtual bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl && x.CanPerformShipCommands());
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x00054B89 File Offset: 0x00052D89
	public virtual bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl && x.CanPerformShipCommands());
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x00054BB0 File Offset: 0x00052DB0
	public virtual List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return playerShips.Where<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl && x.CanPerformShipCommands()).ToList<TISpaceShipState>();
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x00054BDC File Offset: 0x00052DDC
	public TIFleetCommandTemplate()
	{
		base.dataName = base.GetType().ToString();
		this._displayName = this.GetDisplayName(false);
		base.friendlyName = this._displayName;
		TemplateManager.Add(this, typeof(TIFleetCommandTemplate), false);
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x00054C2C File Offset: 0x00052E2C
	public virtual void OnExecuteFleetCommand(List<TISpaceShipState> playerShips, CombatTargetableState target = null)
	{
		foreach (TISpaceShipState tispaceShipState in from x in this.GetEligibleShips(playerShips)
			where !x.ShipDestroyed()
			select x)
		{
			this.GetShipCommandTemplate().OnCommandExecute(tispaceShipState, target);
		}
		GameControl.eventManager.TriggerEvent(new FleetCommandExecuted(playerShips, this), null, Array.Empty<object>());
	}
}
