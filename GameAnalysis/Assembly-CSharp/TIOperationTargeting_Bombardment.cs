using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;

// Token: 0x020002F4 RID: 756
public class TIOperationTargeting_Bombardment : TIOperationTargeting
{
	// Token: 0x06000B74 RID: 2932 RVA: 0x0003E2A4 File Offset: 0x0003C4A4
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TIHabState),
			typeof(TISpaceFleetState),
			typeof(TIRegionState),
			typeof(TIRegionSpaceFacilityState),
			typeof(TIArmyState),
			typeof(TIRegionAlienFacilityState),
			typeof(TIRegionUFOLandingState)
		};
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0003E326 File Offset: 0x0003C526
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0003E32C File Offset: 0x0003C52C
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.fleet = actorState as TISpaceFleetState;
		this.faction = this.fleet.faction;
		this.spaceBody = this.fleet.barycenter.ref_spaceBody;
		this.possibleTargets = operationType.GetPossibleTargets(actorState, defaultTarget);
	}

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06000B77 RID: 2935 RVA: 0x0003E388 File Offset: 0x0003C588
	public override bool forceMap
	{
		get
		{
			return this.spaceBody.isEarth;
		}
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0003E398 File Offset: 0x0003C598
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			base.SetDefaultTarget(forceTarget ?? (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState) ? GeneralControlsController.UIOtherSelectedState : null));
			if (this.spaceBody.isEarth)
			{
				SpaceObjectSelection.SelectSpaceObject(GameControl.control.viewMgr.earthObject, false, false, false);
				GameControl.eventManager.AddListener<AlienAssetTargetSelected>(new EventManager.EventDelegate<AlienAssetTargetSelected>(this.AlienAssetSelectedForTargeting), null, null, true, false);
				GameControl.eventManager.AddListener<ArmyMapItemSelected>(new EventManager.EventDelegate<ArmyMapItemSelected>(this.ArmySelectedForTargeting), null, null, true, false);
				GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null, null, true, false);
				GameControl.eventManager.AddListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.SpaceFacilitySelectedForTargeting), null, null, true, false);
				return;
			}
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<FleetTargetSelectedEvent>(new EventManager.EventDelegate<FleetTargetSelectedEvent>(this.FleetSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x0003E4A0 File Offset: 0x0003C6A0
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			if (this.spaceBody.isEarth)
			{
				GameControl.eventManager.RemoveListener<AlienAssetTargetSelected>(new EventManager.EventDelegate<AlienAssetTargetSelected>(this.AlienAssetSelectedForTargeting), null);
				GameControl.eventManager.RemoveListener<ArmyMapItemSelected>(new EventManager.EventDelegate<ArmyMapItemSelected>(this.ArmySelectedForTargeting), null);
				GameControl.eventManager.RemoveListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null);
				GameControl.eventManager.RemoveListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.SpaceFacilitySelectedForTargeting), null);
				return;
			}
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<FleetTargetSelectedEvent>(new EventManager.EventDelegate<FleetTargetSelectedEvent>(this.FleetSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0003E556 File Offset: 0x0003C756
	public override TIGameState GetDefaultTarget()
	{
		if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
		{
			return GeneralControlsController.UIOtherSelectedState;
		}
		return this.possibleTargets[0];
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x0003E57C File Offset: 0x0003C77C
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.AttemptSetTarget(e.hab);
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0003E58A File Offset: 0x0003C78A
	private void FleetSelectedForTargeting(FleetTargetSelectedEvent e)
	{
		base.AttemptSetTarget(e.targetedFleet);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0003E598 File Offset: 0x0003C798
	private void ArmySelectedForTargeting(ArmyMapItemSelected e)
	{
		base.AttemptSetTarget(e.army);
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0003E5A6 File Offset: 0x0003C7A6
	private void AlienAssetSelectedForTargeting(AlienAssetTargetSelected e)
	{
		base.AttemptSetTarget(e.alienAsset);
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0003E5B4 File Offset: 0x0003C7B4
	private void RegionSelectedForTargeting(RegionStateSelected e)
	{
		base.AttemptSetTarget(e.region);
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0003E5C2 File Offset: 0x0003C7C2
	private void SpaceFacilitySelectedForTargeting(SpaceFacilityMapObjectSelected e)
	{
		base.AttemptSetTarget(e.regionSpaceFacility);
	}

	// Token: 0x04000E96 RID: 3734
	private TISpaceBodyState spaceBody;

	// Token: 0x04000E97 RID: 3735
	private TIFactionState faction;

	// Token: 0x04000E98 RID: 3736
	private TISpaceFleetState fleet;
}
