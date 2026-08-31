using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;

// Token: 0x0200027C RID: 636
public abstract class TIMissionTargeting : TITargeting
{
	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000864 RID: 2148 RVA: 0x000271BF File Offset: 0x000253BF
	// (set) Token: 0x06000865 RID: 2149 RVA: 0x000271C7 File Offset: 0x000253C7
	public TIMissionTemplate missionTemplate { get; protected set; }

	// Token: 0x06000866 RID: 2150 RVA: 0x000271D0 File Offset: 0x000253D0
	public virtual TIGameState GetDefaultTarget()
	{
		if (this.possibleTargets.Count > 0)
		{
			return this.possibleTargets[0];
		}
		return null;
	}

	// Token: 0x06000867 RID: 2151
	public abstract void Activate();

	// Token: 0x06000868 RID: 2152
	public abstract void Shutdown();

	// Token: 0x06000869 RID: 2153 RVA: 0x000271F0 File Offset: 0x000253F0
	protected void SetDefaultTarget()
	{
		TIGameState defaultTarget = this.GetDefaultTarget();
		this.SetTarget(defaultTarget);
		if (defaultTarget != null)
		{
			TIUtilities.GotoSelectedStateUI(defaultTarget, true);
		}
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0002721B File Offset: 0x0002541B
	public virtual string GetTargetName()
	{
		if (this.currentTarget != null)
		{
			return TIUtilities.GetStateDisplayName(this.currentTarget, this.councilor.faction, false, false, false, false, false);
		}
		return Loc.T("TIMissionTargeting_NoTarget");
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00027254 File Offset: 0x00025454
	protected void SetTarget(TIGameState target)
	{
		if (this.possibleTargets.Contains(target))
		{
			TIGameState currentTarget = this.currentTarget;
			this.currentTarget = target;
			GeneralControlsController.SetUITargetedState(target);
			if (this.currentTarget != null)
			{
				object[] array = (from x in new object[]
					{
						(currentTarget != null) ? currentTarget.ref_region : null,
						this.currentTarget.ref_region
					}.Distinct<object>()
					where x != null
					select x).ToArray<object>();
				GameControl.eventManager.TriggerEvent(new MissionTargettedEvent(this.currentTarget, this.councilor, this.missionTemplate), null, array);
				if (currentTarget != null)
				{
					if (this.currentTarget.isCouncilorState && !currentTarget.isCouncilorState)
					{
						GameControl.eventManager.TriggerEvent(new CouncilorSelectedOffMap(this.currentTarget.ref_councilor), null, new object[] { this.currentTarget.ref_region });
						return;
					}
					if (currentTarget.isCouncilorState && !this.currentTarget.isCouncilorState)
					{
						GameControl.eventManager.TriggerEvent(new CouncilorSelectedOffMap(currentTarget.ref_councilor), null, new object[] { currentTarget.ref_region });
						return;
					}
				}
			}
		}
		else
		{
			if (target == null)
			{
				this.currentTarget = null;
				GeneralControlsController.SetUITargetedState(null);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x000273BB File Offset: 0x000255BB
	public virtual TIGameState GetTargetted()
	{
		return this.currentTarget;
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x000273C3 File Offset: 0x000255C3
	public virtual void Init(TIMissionTemplate missionType, TICouncilorState councilor)
	{
		this.missionTemplate = missionType;
		this.councilor = councilor;
		this.currentTarget = null;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x000273DA File Offset: 0x000255DA
	public virtual void ForceTarget(TIGameState target)
	{
		this.SetTarget(target);
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x000273E3 File Offset: 0x000255E3
	protected void SetActivation(TIMissionTargeting mode)
	{
		base.activated = true;
		GeneralControlsController.SetUIGlobalTargetingMode(this.currentTarget, mode);
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x000273F8 File Offset: 0x000255F8
	protected void SetShutdown()
	{
		base.activated = false;
		this.SetTarget(null);
		GeneralControlsController.ShutdownUIGlobalTargetingMode(GameControl.control.activePlayer);
	}

	// Token: 0x0400063E RID: 1598
	protected TICouncilorState councilor;
}
