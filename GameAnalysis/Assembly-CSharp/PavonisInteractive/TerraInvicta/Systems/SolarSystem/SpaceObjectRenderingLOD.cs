using System;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x020009A2 RID: 2466
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(CameraManager))]
	public class SpaceObjectRenderingLOD : StrategyLayerComponentSystem
	{
		// Token: 0x06005CFC RID: 23804 RVA: 0x002C52F0 File Offset: 0x002C34F0
		protected override void OnUpdate()
		{
			this.politicalView = GameControl.control.viewMgr.currentView == ViewType.PoliticalMap;
			if (this.camera.ForceVisualizationUpdate || this.camera.IsAnimating || TIUtilities.IsTimeFlowing || TIUtilities.IsTimeFlowing != this.wasTimeFlowingLastFrame)
			{
				for (int i = 0; i < this.spaceObjects.Length; i++)
				{
					this.DetermineLOD(ref this.spaceObjects.Orbit[i].Value, ref this.spaceObjects.SpaceObject[i].Value, ref this.spaceObjects.LOD[i].Value, this.spaceObjects.Controller[i], this.spaceObjects.GameObject[i], this.politicalView);
				}
			}
			this.wasTimeFlowingLastFrame = TIUtilities.IsTimeFlowing;
			if (!this.selection.HasSelection)
			{
				for (int j = 0; j < this.spaceObjects.Length; j++)
				{
					this.spaceObjects.LOD[j].Value.DisplayModel = false;
					this.spaceObjects.LOD[j].Value.DisplaySurface = false;
					this.spaceObjects.LOD[j].Value.DisplaySymbol = false;
					this.spaceObjects.LOD[j].Value.DisplayOrbitTrail = false;
					this.spaceObjects.LOD[j].Value.JustPoppedIn = false;
				}
				return;
			}
			SpaceObjectLOD value = this.selection.ObjectSelected.GetComponent<SpaceObjectLODComponent>().Value;
			if (this.selection.ObjectSelected.GetComponent<SpaceObjectComponent>().Value.ObjectType == SpaceObjectType.Star)
			{
				this.camera.LOD = CameraManagerLOD.SolarSystem;
				return;
			}
			if (value.DisplaySymbol)
			{
				this.camera.LOD = CameraManagerLOD.SolarSystem;
				return;
			}
			if (value.DisplaySurface)
			{
				this.camera.LOD = (this.selection.ObjectSelected.Has<SpaceBodyRotationComponent>() ? CameraManagerLOD.Surface : CameraManagerLOD.Vessel);
				return;
			}
			if (value.DisplayModel)
			{
				this.camera.LOD = CameraManagerLOD.PlanetSystem;
				return;
			}
			this.camera.LOD = CameraManagerLOD.SolarSystem;
		}

		// Token: 0x06005CFD RID: 23805 RVA: 0x002C5534 File Offset: 0x002C3734
		private void DetermineLOD(ref Orbit orbit, ref SpaceObject spaceObject, ref SpaceObjectLOD LOD, SpaceObjectController controller, GameObject gameObject, bool politicalView)
		{
			SpaceObjectLOD spaceObjectLOD = LOD;
			LOD.DisplayModel = this.ShouldDisplayModel(ref spaceObject, controller);
			LOD.DisplaySurface = LOD.DisplayModel && this.ShouldDisplaySurface(ref spaceObject, gameObject);
			LOD.DisplaySymbol = !LOD.DisplayModel && this.ShouldDisplaySymbol(ref spaceObject, ref orbit, controller, gameObject, false, ref LOD.DisplaySymbolName);
			LOD.DisplayOrbitTrail = this.ShouldDisplayOrbitTrail(ref spaceObject, ref LOD, ref orbit, gameObject, controller, politicalView);
			LOD.JustPoppedIn = LOD.DisplayModel && !spaceObjectLOD.DisplayModel;
		}

		// Token: 0x06005CFE RID: 23806 RVA: 0x002C55C4 File Offset: 0x002C37C4
		private bool ShouldDisplayModel(ref SpaceObject spaceObject, SpaceObjectController controller)
		{
			switch (spaceObject.ObjectType)
			{
			case SpaceObjectType.Fleet:
			{
				if (controller.spaceObjectState.ref_fleet.landed)
				{
					return false;
				}
				Vector3d vector3d = this.camera.Position;
				return Vector3d.Distance(in vector3d, in spaceObject.Position) / controller.spaceObjectState.meanRadius_m < SpaceObjectRenderingLOD.ModelRadiusRatio;
			}
			case SpaceObjectType.Hab:
			{
				Vector3d vector3d = this.camera.Position;
				return Vector3d.Distance(in vector3d, in spaceObject.Position) / controller.spaceObjectState.meanRadius_m < SpaceObjectRenderingLOD.ModelRadiusRatio;
			}
			case SpaceObjectType.LagrangePoint:
				return false;
			default:
			{
				Vector3d vector3d = this.camera.Position;
				double num = Vector3d.Distance(in vector3d, in spaceObject.Position);
				double num2 = controller.spaceObjectState.GetAngularDiameter(num);
				if (controller.spaceObjectState.isSpaceBodyState)
				{
					num2 *= (double)controller.spaceObjectState.ref_spaceBody.template.angularDiameterMultiplier;
				}
				if (num2 < 0.10000000149011612)
				{
					return false;
				}
				double num3 = (double)(Vector3.Distance(controller.symbolController.transform.position, this.camera.Transform.position) * 45f / 100000f);
				float scaleSize = controller.symbolController.scaleSize;
				double num4 = num3 * (double)scaleSize * (double)0.85f * (double)controller.symbolController.buttonImage.rectTransform.rect.height / (double)2f;
				float num5 = Vector3.Distance(this.camera.unityCamera.transform.position, controller.symbolController.transform.position);
				float num6 = (float)Mathd.AngularDiameterOfPlane(num4, (double)num5);
				return num2 > (double)num6;
			}
			}
		}

		// Token: 0x06005CFF RID: 23807 RVA: 0x002C5764 File Offset: 0x002C3964
		private bool ShouldDisplaySurface(ref SpaceObject spaceObject, GameObject gameObject)
		{
			if (this.selection.ObjectSelected != gameObject)
			{
				return false;
			}
			if (this.camera.IsAnimating && this.camera.TargetSpherical.radius / spaceObject.MeanRadius > SpaceObjectRenderingLOD.MapRadiusRatio)
			{
				return false;
			}
			Vector3d vector3d = this.camera.Position - spaceObject.Position;
			return Vector3d.Magnitude(in vector3d) / spaceObject.MeanRadius < SpaceObjectRenderingLOD.MapRadiusRatio || (GeneralControlsController.UIPlayerInTargetingMode && GeneralControlsController.UITargetingMode.forceMap);
		}

		// Token: 0x06005D00 RID: 23808 RVA: 0x002C57F4 File Offset: 0x002C39F4
		private bool ShouldDisplaySymbol(ref SpaceObject spaceObject, ref Orbit orbit, SpaceObjectController controller, GameObject gameObject, bool fullSpeedCheck, ref bool ShouldDisplaySymbolName)
		{
			if (!fullSpeedCheck)
			{
				SpaceObjectType objectType = spaceObject.ObjectType;
				if (objectType != SpaceObjectType.Fleet)
				{
					if (objectType == SpaceObjectType.Hab)
					{
						if (this.gameTime.currentSpeed >= 3600f && !controller.spaceObjectState.barycenter.isLagrangePointState)
						{
							return false;
						}
					}
				}
				else
				{
					if (controller.spaceObjectState.ref_fleet.landed)
					{
						return false;
					}
					if (this.gameTime.currentSpeed >= 3600f && !controller.spaceObjectState.ref_fleet.inTransfer && !controller.spaceObjectState.barycenter.isLagrangePointState && !controller.spaceObjectState.barycenter.isSun)
					{
						return false;
					}
				}
			}
			if (this.camera.LOD != CameraManagerLOD.SolarSystem && (!GameControl.solarSystem.showDistantSymbols || (this.politicalView && !controller.spaceObjectState.inEarthSystem)))
			{
				return false;
			}
			Vector3d vector3d;
			if (spaceObject.ObjectType == SpaceObjectType.LagrangePoint && (controller.spaceObjectState.ref_lagrangePoint.lagrangeValue == LagrangeValue.L1 || controller.spaceObjectState.ref_lagrangePoint.lagrangeValue == LagrangeValue.L2) && GameControl.solarSystem.showDistantSymbols && (!this.politicalView || controller.spaceObjectState.inEarthSystem))
			{
				Vector3d position = gameObject.GetComponent<LagrangePointComponent>().Value.RelatedSpaceBody.GetComponent<SpaceObjectComponent>().Value.Position;
				Vector3d position2 = spaceObject.Position;
				vector3d = position - position2;
				double num = Vector3d.Magnitude(in vector3d);
				vector3d = this.camera.Position - position;
				return Vector3d.Magnitude(in vector3d) / num < SpaceObjectRenderingLOD.SatelliteSymbolRadiusRatio_Other;
			}
			SpaceObject value = base.EntityManager.GetComponentObject<SpaceObjectComponent>(orbit.Barycenter).Value;
			vector3d = value.Position - spaceObject.Position;
			double num2 = Vector3d.Magnitude(in vector3d);
			vector3d = this.camera.Position - value.Position;
			double num3 = Vector3d.Magnitude(in vector3d) / num2;
			if (num3 < ((value.ObjectType == SpaceObjectType.Star) ? SpaceObjectRenderingLOD.SatelliteSymbolRadiusRatio_Sun : SpaceObjectRenderingLOD.SatelliteSymbolRadiusRatio_Other))
			{
				ShouldDisplaySymbolName = !fullSpeedCheck && num3 < (double)((spaceObject.ObjectType == SpaceObjectType.Asteroid) ? 10 : 18);
				return true;
			}
			ShouldDisplaySymbolName = false;
			return false;
		}

		// Token: 0x06005D01 RID: 23809 RVA: 0x002C5A20 File Offset: 0x002C3C20
		private bool ShouldDisplayOrbitTrail(ref SpaceObject spaceObject, ref SpaceObjectLOD LOD, ref Orbit orbit, GameObject gameObject, SpaceObjectController controller, bool politicalView)
		{
			switch (spaceObject.ObjectType)
			{
			case SpaceObjectType.Star:
			case SpaceObjectType.LagrangePoint:
				return false;
			case SpaceObjectType.Planet:
				return !politicalView && LOD.DisplaySymbol && this.camera.LOD == CameraManagerLOD.SolarSystem;
			case SpaceObjectType.PlanetaryMoon:
				return this.camera.LOD != CameraManagerLOD.Vessel && !politicalView && LOD.DisplaySymbol;
			case SpaceObjectType.Fleet:
			{
				TISpaceFleetState ref_fleet = controller.spaceObjectState.ref_fleet;
				if (!ref_fleet.dockedOrLanded)
				{
					if (!LOD.DisplaySymbol)
					{
						return TIPlayerProfileManager.showHighSpeedOrbitTrails && this.gameTime.currentSpeed >= 3600f && !ref_fleet.inTransfer && !ref_fleet.barycenter.isSun && this.ShouldDisplaySymbol(ref spaceObject, ref orbit, controller, gameObject, true, ref LOD.DisplaySymbolName);
					}
					if (this.selection.ObjectSelected == gameObject)
					{
						return true;
					}
					if (ref_fleet.inTransfer && ref_fleet.trajectory.destination != null)
					{
						if (ref_fleet.faction == GameControl.control.activePlayer || ref_fleet.alwaysShowOrbitTrailDuringTransfer || ref_fleet.trajectory.destination.ref_faction == GameControl.control.activePlayer)
						{
							return true;
						}
						if (!GameControl.control.activePlayer.fullSpaceVisibility && !ref_fleet.VisibleToFaction(GameControl.control.activePlayer))
						{
							return false;
						}
						if (!(ref_fleet.trajectory.destination == this.selection.spaceObjectStateSelected))
						{
							TINaturalSpaceObjectState ref_naturalSpaceObject = ref_fleet.trajectory.destination.ref_naturalSpaceObject;
							return ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.GetSunOrbitingRelatedObject : null) == this.selection.spaceObjectStateSelected.GetSunOrbitingRelatedObject;
						}
						return true;
					}
				}
				return false;
			}
			case SpaceObjectType.Hab:
				return (LOD.DisplaySymbol && controller.spaceObjectState.barycenter.isLagrangePointState) || (TIPlayerProfileManager.showHighSpeedOrbitTrails && this.gameTime.currentSpeed >= 3600f && !LOD.DisplayModel && this.ShouldDisplaySymbol(ref spaceObject, ref orbit, controller, gameObject, true, ref LOD.DisplaySymbolName));
			}
			return !politicalView && LOD.DisplaySymbol && this.selection.ObjectSelected == gameObject;
		}

		// Token: 0x04004284 RID: 17028
		private static readonly double SatelliteSymbolRadiusRatio_Sun = 40.0;

		// Token: 0x04004285 RID: 17029
		private static readonly double SatelliteSymbolRadiusRatio_Other = 120.0;

		// Token: 0x04004286 RID: 17030
		private static readonly double ModelRadiusRatio = 30.0;

		// Token: 0x04004287 RID: 17031
		private static readonly double MapRadiusRatio = 5.0;

		// Token: 0x04004288 RID: 17032
		private const int speedToSwapToOrbitTrails = 3600;

		// Token: 0x04004289 RID: 17033
		[Inject]
		private SpaceObjectRenderingLOD.SpaceObjectGroup spaceObjects;

		// Token: 0x0400428A RID: 17034
		[Inject]
		private SpaceObjectRenderingLOD.NavigableGroup navigables;

		// Token: 0x0400428B RID: 17035
		[Inject]
		private SpaceObjectSelection selection;

		// Token: 0x0400428C RID: 17036
		[Inject]
		private CameraManager camera;

		// Token: 0x0400428D RID: 17037
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x0400428E RID: 17038
		private bool politicalView;

		// Token: 0x0400428F RID: 17039
		private bool wasTimeFlowingLastFrame;

		// Token: 0x0200134D RID: 4941
		private struct SpaceObjectGroup
		{
			// Token: 0x04006FA6 RID: 28582
			public readonly int Length;

			// Token: 0x04006FA7 RID: 28583
			public GameObjectArray GameObject;

			// Token: 0x04006FA8 RID: 28584
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006FA9 RID: 28585
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006FAA RID: 28586
			[ReadOnly]
			public ComponentArray<SpaceObjectController> Controller;

			// Token: 0x04006FAB RID: 28587
			[ReadOnly]
			public ComponentArray<OrbitComponent> Orbit;
		}

		// Token: 0x0200134E RID: 4942
		private struct NavigableGroup
		{
			// Token: 0x04006FAC RID: 28588
			public readonly int Length;

			// Token: 0x04006FAD RID: 28589
			public GameObjectArray GameObject;

			// Token: 0x04006FAE RID: 28590
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006FAF RID: 28591
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006FB0 RID: 28592
			[ReadOnly]
			public ComponentArray<SpaceObjectController> Controller;

			// Token: 0x04006FB1 RID: 28593
			[ReadOnly]
			public ComponentArray<OrbitComponent> Orbit;

			// Token: 0x04006FB2 RID: 28594
			[ReadOnly]
			public ComponentArray<NavigableComponent> Navigable;

			// Token: 0x04006FB3 RID: 28595
			[ReadOnly]
			public ComponentArray<LagrangePointComponent> LagrangePoint;
		}
	}
}
