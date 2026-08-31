using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002EB RID: 747
public class TIOperationTargeting_AlienAsset : TIOperationTargeting
{
	// Token: 0x06000B36 RID: 2870 RVA: 0x0003D597 File Offset: 0x0003B797
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIOperationTargeting_AlienAsset) };
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003D5AE File Offset: 0x0003B7AE
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0003D5B4 File Offset: 0x0003B7B4
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetDefaultTarget(forceTarget);
			base.SetActivation(this);
			TIArmyState tiarmyState = this.actorState as TIArmyState;
			if (tiarmyState != null)
			{
				GameControl.eventManager.TriggerEvent(new ArmyTargetSpaceFacilities(tiarmyState, this.operationType), null, Array.Empty<object>());
			}
			GameControl.eventManager.AddListener<AlienAssetTargetSelected>(new EventManager.EventDelegate<AlienAssetTargetSelected>(this.AlienAssetSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003D622 File Offset: 0x0003B822
	public override TIGameState GetDefaultTarget()
	{
		if (this.possibleTargets.Count <= 0)
		{
			return this.currentTarget;
		}
		if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
		{
			return GeneralControlsController.UIOtherSelectedState;
		}
		return this.possibleTargets[0];
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0003D65D File Offset: 0x0003B85D
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetAlienAssets(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<AlienAssetTargetSelected>(new EventManager.EventDelegate<AlienAssetTargetSelected>(this.AlienAssetSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0003D699 File Offset: 0x0003B899
	private void AlienAssetSelectedForTargeting(AlienAssetTargetSelected e)
	{
		base.AttemptSetTarget(e.alienAsset);
	}
}
