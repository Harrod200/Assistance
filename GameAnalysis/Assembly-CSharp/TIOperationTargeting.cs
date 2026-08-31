using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;

// Token: 0x020002E2 RID: 738
public abstract class TIOperationTargeting : TITargeting
{
	// Token: 0x06000AF3 RID: 2803
	public abstract TIGameState GetDefaultTarget();

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0003CD1F File Offset: 0x0003AF1F
	public TIGameState GetDefaultTargetOrNull()
	{
		if (this.possibleTargets.Count <= 0)
		{
			return null;
		}
		return this.GetDefaultTarget();
	}

	// Token: 0x06000AF5 RID: 2805
	public abstract void Activate(TIGameState forceTarget = null);

	// Token: 0x06000AF6 RID: 2806
	public abstract void Shutdown();

	// Token: 0x06000AF7 RID: 2807
	public abstract OperationTargetingUIType UIType();

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0003CD37 File Offset: 0x0003AF37
	public TIGameState GetTargetted()
	{
		return this.currentTarget;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003CD3F File Offset: 0x0003AF3F
	public string GetTargetName()
	{
		if (!(this.currentTarget != null))
		{
			return Loc.T("UI.Operations.NoTarget");
		}
		return TIUtilities.GetStateDisplayName(this.currentTarget, GameControl.control.activePlayer, false, false, false, false, true);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0003CD74 File Offset: 0x0003AF74
	protected void SetDefaultTarget(TIGameState forceTarget = null)
	{
		if (forceTarget != null)
		{
			this.AttemptSetTarget(forceTarget);
		}
		if (forceTarget == null || this.currentTarget == null)
		{
			TIGameState defaultTargetOrNull = this.GetDefaultTargetOrNull();
			if (defaultTargetOrNull != null)
			{
				this.AttemptSetTarget(defaultTargetOrNull);
			}
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0003CDBF File Offset: 0x0003AFBF
	public virtual void ForceTarget(TIGameState target)
	{
		this.AttemptSetTarget(target);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0003CDC8 File Offset: 0x0003AFC8
	protected void AttemptSetTarget(TIGameState target)
	{
		if (this.possibleTargets.Contains(target))
		{
			this.currentTarget = target;
			if (base.activated && !GeneralControlsController.ActivePlayerAsset(target))
			{
				if (this.operationType is TISpaceBodyOperationTemplate)
				{
					TIUtilities.GotoGameState(target, false, true, false, false, false, -1f);
					GeneralControlsController.SetUITargetedState(this.currentTarget);
				}
				else
				{
					TIUtilities.GotoSelectedStateUI(target, false);
				}
			}
			else
			{
				GeneralControlsController.SetUITargetedState(this.currentTarget);
			}
			if (this.currentTarget != null)
			{
				GameControl.eventManager.TriggerEvent(new OperationTargettedEvent(this.currentTarget, this.actorState), null, Array.Empty<object>());
				return;
			}
		}
		else
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			this.currentTarget = null;
		}
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003CE7E File Offset: 0x0003B07E
	public virtual void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.possibleTargets = operationType.GetPossibleTargets(actorState, null);
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0003CE9C File Offset: 0x0003B09C
	protected void SetActivation(TIOperationTargeting mode)
	{
		base.activated = true;
		GeneralControlsController.SetUIGlobalTargetingMode(this.currentTarget, mode);
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003CEB1 File Offset: 0x0003B0B1
	protected void SetShutdown()
	{
		base.activated = false;
		GeneralControlsController.ShutdownUIGlobalTargetingMode(GameControl.control.activePlayer);
	}

	// Token: 0x04000E8B RID: 3723
	protected IOperation operationType;

	// Token: 0x04000E8C RID: 3724
	protected TIGameState actorState;
}
