using System;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x0200099C RID: 2460
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectRenderingLOD))]
	[UpdateAfter(typeof(SpaceObjectRendering))]
	[UpdateAfter(typeof(SpaceBodyRotating))]
	[UpdateAfter(typeof(ModelRendering))]
	public class MapRendering : StrategyLayerComponentSystem
	{
		// Token: 0x06005CE3 RID: 23779 RVA: 0x002C3C80 File Offset: 0x002C1E80
		protected override void OnStopRunning()
		{
			if (GameControl.spaceCombat != null && TIGlobalValuesState.isSpaceCombatEnabled)
			{
				for (int i = 0; i < this.mapObjects.Length; i++)
				{
					MapComponent mapComponent = this.mapObjects.Map[i];
					MapController mapController = ((mapComponent != null) ? mapComponent.MapController : null);
					if (mapController.isActive)
					{
						mapController.MakeActive(false);
					}
				}
			}
			base.OnStopRunning();
		}

		// Token: 0x06005CE4 RID: 23780 RVA: 0x002C3CEC File Offset: 0x002C1EEC
		protected override void OnUpdate()
		{
			for (int i = 0; i < this.mapObjects.Length; i++)
			{
				MapController mapController = this.mapObjects.Map[i].MapController;
				SpaceObjectLOD value = this.mapObjects.Map[i].LodComponentLink.Value;
				if (mapController.isActive != value.DisplaySurface)
				{
					mapController.MakeActive(value.DisplaySurface);
				}
				Transform transform = this.mapObjects.Map[i].SpaceObjectController.modelController.transform;
				if (mapController.transform.localScale != transform.localScale)
				{
					mapController.transform.localScale = this.mapObjects.Map[i].SpaceObjectController.modelController.transform.localScale;
				}
			}
		}

		// Token: 0x04004264 RID: 16996
		[Inject]
		private CameraManager camera;

		// Token: 0x04004265 RID: 16997
		[Inject]
		private MapRendering.MapObjectGroup mapObjects;

		// Token: 0x04004266 RID: 16998
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x02001342 RID: 4930
		private struct MapObjectGroup
		{
			// Token: 0x04006F7A RID: 28538
			public readonly int Length;

			// Token: 0x04006F7B RID: 28539
			public ComponentArray<MapComponent> Map;
		}
	}
}
