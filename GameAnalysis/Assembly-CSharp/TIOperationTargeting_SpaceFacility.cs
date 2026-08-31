using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002EA RID: 746
public class TIOperationTargeting_SpaceFacility : TIOperationTargeting
{
	// Token: 0x06000B2F RID: 2863 RVA: 0x0003D480 File Offset: 0x0003B680
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIRegionSpaceFacilityState) };
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0003D497 File Offset: 0x0003B697
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0003D49C File Offset: 0x0003B69C
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetDefaultTarget(forceTarget);
			TIArmyState tiarmyState = this.actorState as TIArmyState;
			if (tiarmyState != null)
			{
				GameControl.eventManager.TriggerEvent(new ArmyTargetSpaceFacilities(tiarmyState, this.operationType), null, Array.Empty<object>());
			}
			GameControl.eventManager.AddListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.SpaceFacilitySelectedForTargeting), null, null, true, false);
			base.SetActivation(this);
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0003D50A File Offset: 0x0003B70A
	public override void Shutdown()
	{
		if (base.activated)
		{
			GameControl.eventManager.TriggerEvent(new DeTargetSpaceFacilities(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.SpaceFacilitySelectedForTargeting), null);
			base.SetShutdown();
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0003D546 File Offset: 0x0003B746
	private void SpaceFacilitySelectedForTargeting(SpaceFacilityMapObjectSelected e)
	{
		base.AttemptSetTarget(e.regionSpaceFacility);
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0003D554 File Offset: 0x0003B754
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
}
