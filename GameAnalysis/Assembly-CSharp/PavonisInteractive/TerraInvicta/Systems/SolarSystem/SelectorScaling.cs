using System;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x0200099F RID: 2463
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectRenderingLOD))]
	[UpdateAfter(typeof(SymbolRendering))]
	[UpdateAfter(typeof(ModelRendering))]
	public class SelectorScaling : StrategyLayerComponentSystem
	{
		// Token: 0x06005CEE RID: 23790 RVA: 0x002C47CC File Offset: 0x002C29CC
		protected override void OnUpdate()
		{
			if (this.camera.ForceVisualizationUpdate || this.camera.IsAnimating || !this.gameTime.Paused || this.gameTime.PausedThisFrame())
			{
				for (int i = 0; i < this.spaceObjects.Length; i++)
				{
					SpaceObjectLOD value = this.spaceObjects.LOD[i].Value;
					SpaceObjectController spaceObjectController = this.spaceObjects.Controller[i];
					SphereCollider sphereCollider = spaceObjectController.sphereCollider;
					if (!value.DisplayModel && !value.DisplaySymbol)
					{
						sphereCollider.enabled = false;
					}
					else if (value.DisplaySymbol)
					{
						sphereCollider.radius = Vector3.Distance(spaceObjectController.symbolLink.transform.position, this.camera.Transform.position) * 0.0045f * 3f * spaceObjectController.symbolController.scaleSize;
						sphereCollider.enabled = true;
					}
					else if (spaceObjectController.spaceObjectState.isSpaceAssetState)
					{
						sphereCollider.enabled = false;
					}
					else if (value.DisplayModel)
					{
						SpaceObject value2 = this.spaceObjects.SpaceObject[i].Value;
						sphereCollider.enabled = true;
						Vector3d vector3d = value2.Position - this.camera.Position;
						double num = Vector3d.Magnitude(in vector3d);
						double num2 = (double)Vector3.Distance(this.camera.Transform.position, spaceObjectController.transform.position);
						num /= this.camera.WorldScale;
						double num3 = value2.MeanRadius / this.camera.WorldScale;
						sphereCollider.radius = (float)(num3 * (num2 / num));
					}
				}
			}
		}

		// Token: 0x04004271 RID: 17009
		[Inject]
		private SelectorScaling.SpaceObjectGroup spaceObjects;

		// Token: 0x04004272 RID: 17010
		[Inject]
		private CameraManager camera;

		// Token: 0x04004273 RID: 17011
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x02001346 RID: 4934
		private struct SpaceObjectGroup
		{
			// Token: 0x04006F8A RID: 28554
			public readonly int Length;

			// Token: 0x04006F8B RID: 28555
			public GameObjectArray GameObject;

			// Token: 0x04006F8C RID: 28556
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006F8D RID: 28557
			[ReadOnly]
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006F8E RID: 28558
			[ReadOnly]
			public ComponentArray<SpaceObjectController> Controller;
		}
	}
}
