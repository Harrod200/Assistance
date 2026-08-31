using System;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x020009A0 RID: 2464
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(CameraManager))]
	public class SpaceBodyRotating : StrategyLayerComponentSystem
	{
		// Token: 0x06005CF0 RID: 23792 RVA: 0x002C4990 File Offset: 0x002C2B90
		protected override void OnUpdate()
		{
			for (int i = 0; i < this.spaceBodies.Length; i++)
			{
				if (this.camera.ForceVisualizationUpdate || TIUtilities.IsTimeFlowing || this.spaceBodies.LOD[i].Value.JustPoppedIn)
				{
					if (this.camera.SelectedState != null && this.camera.SelectedState == this.spaceBodies.SpaceObject[i].State)
					{
						SpaceBodyRotating.CenterSpaceObject(this.spaceBodies.SpaceObject[i].State);
					}
					else if (this.spaceBodies.LOD[i].Value.DisplayModel)
					{
						SpaceBodyRotating.RotateTransform(this.spaceBodies.Controller[i].modelLink.transform, ref this.spaceBodies.SpaceObject[i].Value, ref this.spaceBodies.Rotation[i].Value, this.gameTime.Now);
						this.spaceBodies.Controller[i].modelLink.transform.rotation = Quaternion.Inverse(this.camera.SurfaceRotation) * this.spaceBodies.Controller[i].modelLink.transform.rotation;
						if (this.spaceBodies.Controller[i].HasMap && this.spaceBodies.Controller[i].mapController.enabled)
						{
							SpaceBodyRotating.RotateTransform(this.spaceBodies.Controller[i].mapTransform, ref this.spaceBodies.SpaceObject[i].Value, ref this.spaceBodies.Rotation[i].Value, this.gameTime.Now);
							this.spaceBodies.Controller[i].mapTransform.rotation = Quaternion.Inverse(this.camera.SurfaceRotation) * this.spaceBodies.Controller[i].mapTransform.rotation;
						}
					}
				}
			}
		}

		// Token: 0x06005CF1 RID: 23793 RVA: 0x002C4BEC File Offset: 0x002C2DEC
		public static void CenterSpaceObject(TISpaceObjectState spaceObjectState)
		{
			Transform transform = spaceObjectState.controller.transform;
			Transform transform2 = spaceObjectState.controller.modelLink.transform;
			if (transform.position != Vector3.zero || transform.rotation != Quaternion.identity || transform2.rotation != Quaternion.identity)
			{
				transform.position = Vector3.zero;
				transform.rotation = Quaternion.identity;
				transform2.rotation = Quaternion.identity;
				if (spaceObjectState.controller.HasMap && spaceObjectState.controller.mapController.enabled)
				{
					spaceObjectState.controller.mapController.mapTransform.rotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06005CF2 RID: 23794 RVA: 0x002C4CA8 File Offset: 0x002C2EA8
		private static void RotateTransform(Transform transform, ref SpaceObject spaceObject, ref SpaceBodyRotation rotation, DateTime now)
		{
			transform.rotation = (Quaternion)spaceObject.SpatialRotation;
			float num = (float)(SpaceBodyRotating.GetSurfaceRotation(ref spaceObject, ref rotation, now) * 57.29577951308232);
			transform.Rotate(Vector3.up, num, Space.Self);
		}

		// Token: 0x06005CF3 RID: 23795 RVA: 0x002C4CE8 File Offset: 0x002C2EE8
		public static double GetSurfaceRotation(ref SpaceObject spaceObject, ref SpaceBodyRotation rotation, DateTime time)
		{
			double num = rotation.RotationOffset_rad / 6.283185307179586;
			return 6.283185307179586 - (num + (time - spaceObject.Epoch).TotalSeconds / rotation.RotationPeriod_s) % 1.0 * 6.283185307179586;
		}

		// Token: 0x06005CF4 RID: 23796 RVA: 0x002C4D44 File Offset: 0x002C2F44
		public static double GetSurfaceRotation(SpaceObjectController spaceObjectController)
		{
			if (!spaceObjectController.HasSpaceBodyRotation)
			{
				return 0.0;
			}
			SpaceObject spaceObject = spaceObjectController.SpaceObject;
			SpaceBodyRotation spaceBodyRotation = spaceObjectController.SpaceBodyRotation;
			TIDateTime tidateTime = TITimeState.Now();
			if (tidateTime == null)
			{
				return 0.0;
			}
			return SpaceBodyRotating.GetSurfaceRotation(ref spaceObject, ref spaceBodyRotation, tidateTime.ExportTime());
		}

		// Token: 0x04004274 RID: 17012
		[Inject]
		private SpaceBodyRotating.SpaceBodyGroup spaceBodies;

		// Token: 0x04004275 RID: 17013
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x04004276 RID: 17014
		[Inject]
		private CameraManager camera;

		// Token: 0x02001347 RID: 4935
		private struct SpaceBodyGroup
		{
			// Token: 0x04006F8F RID: 28559
			public readonly int Length;

			// Token: 0x04006F90 RID: 28560
			[ReadOnly]
			public ComponentArray<SpaceBodyRotationComponent> Rotation;

			// Token: 0x04006F91 RID: 28561
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006F92 RID: 28562
			[ReadOnly]
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006F93 RID: 28563
			public ComponentArray<SpaceObjectController> Controller;
		}
	}
}
