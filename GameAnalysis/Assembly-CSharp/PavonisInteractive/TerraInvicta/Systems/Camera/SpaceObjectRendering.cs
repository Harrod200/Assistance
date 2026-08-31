using System;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.Camera
{
	// Token: 0x020009B2 RID: 2482
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectRenderingLOD))]
	[UpdateAfter(typeof(CameraManager))]
	public class SpaceObjectRendering : StrategyLayerComponentSystem
	{
		// Token: 0x06005DB7 RID: 23991 RVA: 0x002C9F64 File Offset: 0x002C8164
		protected override void OnUpdate()
		{
			if (this.cameraManager.ForceVisualizationUpdate || this.cameraManager.IsAltitudeChanging || TIUtilities.IsTimeFlowing || TIUtilities.IsTimeFlowing != this.wasTimeFlowingLastFrame)
			{
				for (int i = 0; i < this.spaceObjects.Length; i++)
				{
					SpaceObjectLOD value = this.spaceObjects.LOD[i].Value;
					if (this.cameraManager.ForceVisualizationUpdate || this.cameraManager.IsAltitudeChanging || value.DisplayModel || value.DisplaySymbol || value.DisplaySurface)
					{
						SpaceObjectComponent spaceObjectComponent = this.spaceObjects.SpaceObject[i];
						if (this.cameraManager.SelectedState != null && this.cameraManager.SelectedState == spaceObjectComponent.State)
						{
							SpaceBodyRotating.CenterSpaceObject(spaceObjectComponent.State);
						}
						else
						{
							Vector3 vector = this.cameraManager.ScaledPosition(spaceObjectComponent.Value.Position);
							if (!float.IsNaN(vector.magnitude))
							{
								this.spaceObjects.Transform[i].position = vector;
							}
						}
					}
				}
			}
			this.wasTimeFlowingLastFrame = TIUtilities.IsTimeFlowing;
		}

		// Token: 0x04004310 RID: 17168
		[Inject]
		private SpaceObjectRendering.SpaceObjectGroup spaceObjects;

		// Token: 0x04004311 RID: 17169
		[Inject]
		private CameraManager cameraManager;

		// Token: 0x04004312 RID: 17170
		private bool wasTimeFlowingLastFrame;

		// Token: 0x02001358 RID: 4952
		private struct SpaceObjectGroup
		{
			// Token: 0x04006FD2 RID: 28626
			public readonly int Length;

			// Token: 0x04006FD3 RID: 28627
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006FD4 RID: 28628
			[ReadOnly]
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006FD5 RID: 28629
			public ComponentArray<Transform> Transform;
		}
	}
}
