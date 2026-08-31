using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E9 RID: 745
public class TIOperationTargeting_Region : TIOperationTargeting
{
	// Token: 0x06000B28 RID: 2856 RVA: 0x0003D2A4 File Offset: 0x0003B4A4
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIRegionState) };
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0003D2BB File Offset: 0x0003B4BB
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0003D2C0 File Offset: 0x0003B4C0
	public override void Activate(TIGameState forceTarget = null)
	{
		base.SetDefaultTarget(forceTarget);
		base.SetActivation(this);
		GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null, null, true, false);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0003D313 File Offset: 0x0003B513
	public override void Shutdown()
	{
		base.SetShutdown();
		GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		GameControl.eventManager.RemoveListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null);
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x0003D350 File Offset: 0x0003B550
	private void RegionSelectedForTargeting(RegionStateSelected e)
	{
		base.AttemptSetTarget(e.region);
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0003D360 File Offset: 0x0003B560
	public override TIGameState GetDefaultTarget()
	{
		if (this.actorState.isArmyState)
		{
			TIArmyState ref_army = this.actorState.ref_army;
			if (this.possibleTargets.Contains(ref_army.currentRegion))
			{
				return ref_army.currentRegion;
			}
			ICollection<TIGameState> possibleTargets = this.possibleTargets;
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (possibleTargets.Contains((uiotherSelectedState != null) ? uiotherSelectedState.ref_region : null))
			{
				return GeneralControlsController.UIOtherSelectedState.ref_region;
			}
			foreach (TIRegionState tiregionState in ref_army.currentRegion.AdjacentRegions(false))
			{
				if (this.possibleTargets.Contains(tiregionState))
				{
					return tiregionState;
				}
			}
			if (this.possibleTargets.Count > 0)
			{
				return this.possibleTargets[0];
			}
			return null;
		}
		else
		{
			if (this.possibleTargets.Count <= 0)
			{
				return null;
			}
			if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
			{
				return GeneralControlsController.UIOtherSelectedState;
			}
			return this.possibleTargets[0];
		}
	}
}
