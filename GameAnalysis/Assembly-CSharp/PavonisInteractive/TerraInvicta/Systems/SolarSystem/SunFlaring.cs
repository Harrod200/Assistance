using System;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x020009A4 RID: 2468
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectPositioning))]
	public class SunFlaring : StrategyLayerComponentSystem
	{
		// Token: 0x06005D11 RID: 23825 RVA: 0x002C60E4 File Offset: 0x002C42E4
		protected override void OnUpdate()
		{
			if (this.flare == null)
			{
				this.flare = this.suns.Entity[0].GetComponentInChildren<LensFlare>();
				this.sun = this.suns.SpaceObject[0].Value;
				return;
			}
			double num = 387300.0;
			Vector3d vector3d = this.sun.Position - this.camera.Position;
			double num2 = num / Mathd.Sqrt(Vector3d.Magnitude(in vector3d));
			num2 = Mathd.Clamp(num2, 0.1, 3.0);
			this.flare.brightness = (float)num2;
		}

		// Token: 0x04004299 RID: 17049
		private const double FlareScale = 387300.0;

		// Token: 0x0400429A RID: 17050
		private const double FlareMinBrightness = 0.1;

		// Token: 0x0400429B RID: 17051
		private const double FlareMaxBrightness = 3.0;

		// Token: 0x0400429C RID: 17052
		[Inject]
		private CameraManager camera;

		// Token: 0x0400429D RID: 17053
		[Inject]
		private SunFlaring.SunGroup suns;

		// Token: 0x0400429E RID: 17054
		private LensFlare flare;

		// Token: 0x0400429F RID: 17055
		private SpaceObject sun;

		// Token: 0x0200134F RID: 4943
		private struct SunGroup
		{
			// Token: 0x04006FB4 RID: 28596
			public readonly int Length;

			// Token: 0x04006FB5 RID: 28597
			public GameObjectArray Entity;

			// Token: 0x04006FB6 RID: 28598
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006FB7 RID: 28599
			private SubtractiveComponent<OrbitComponent> _;
		}
	}
}
