using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x0200099E RID: 2462
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectRenderingLOD))]
	[UpdateAfter(typeof(SpaceObjectRendering))]
	public class OrbitTrailRendering : StrategyLayerComponentSystem
	{
		// Token: 0x06005CE8 RID: 23784 RVA: 0x002C4140 File Offset: 0x002C2340
		public void TriggerForceTransferUpdate(TISpaceFleetState fleet)
		{
			for (int i = 0; i < this.transferOrbits.Length; i++)
			{
				FleetTransferPlan value = this.transferOrbits.Plan[i].Value;
				if (fleet == null || value.fleet == fleet)
				{
					SpaceObjectLOD value2 = this.transferOrbits.LOD[i].Value;
					for (int j = 0; j < value.TransferSegments.Count; j++)
					{
						this.DrawTransferOrbit(value, j, value2);
					}
				}
			}
		}

		// Token: 0x06005CE9 RID: 23785 RVA: 0x002C41C8 File Offset: 0x002C23C8
		protected override void OnUpdate()
		{
			if (TIUtilities.IsTimeFlowing || TIUtilities.IsTimeFlowing != this.wasTimeFlowingLastFrame || this.cameraManager.ForceVisualizationUpdate || this.cameraManager.IsAnimating)
			{
				for (int i = 0; i < this.fixedOrbits.Length; i++)
				{
					this.DrawCompleteOrbit(this.fixedOrbits.Orbit[i].Value, this.fixedOrbits.SpaceObject[i].Value, this.fixedOrbits.LOD[i].Value);
				}
				for (int j = 0; j < this.transferOrbits.Length; j++)
				{
					FleetTransferPlan value = this.transferOrbits.Plan[j].Value;
					SpaceObjectLOD value2 = this.transferOrbits.LOD[j].Value;
					for (int k = 0; k < value.TransferSegments.Count; k++)
					{
						this.DrawTransferOrbit(value, k, value2);
					}
				}
			}
			this.wasTimeFlowingLastFrame = TIUtilities.IsTimeFlowing;
		}

		// Token: 0x06005CEA RID: 23786 RVA: 0x002C42D8 File Offset: 0x002C24D8
		private void DrawCompleteOrbit(Orbit orbit, SpaceObject spaceObject, SpaceObjectLOD lod)
		{
			if (orbit.OrbitTrail == null)
			{
				return;
			}
			orbit.OrbitTrail.active = lod.DisplayOrbitTrail;
			if (!lod.DisplayOrbitTrail)
			{
				return;
			}
			if (orbit.OrbitTrail.rectTransform == null)
			{
				orbit.OrbitTrail.active = false;
				return;
			}
			if (orbit.Period == 0.0)
			{
				orbit.OrbitTrail.active = false;
				return;
			}
			this.cameraManager.ScaledPositions(orbit);
			double num = (this.gameTime.Now - orbit.PeriapsisEpoch).TotalSeconds % orbit.Period;
			int num2 = Array.BinarySearch<double>(orbit.TimeAtPoint_s, num);
			int num3 = num2 * 3 + 2;
			if (num2 < 0)
			{
				num2 = ~num2;
				num3 -= 3;
			}
			if (num2 >= orbit.TimeAtPoint_s.Length)
			{
				Log.Warn(string.Concat(new string[]
				{
					"OrbitTrailRendering: time in orbit would be outside range of orbit trail points.\norbitTime = ",
					num.ToString(),
					"s\nlast possible time = ",
					orbit.TimeAtPoint_s[orbit.TimeAtPoint_s.Length - 1].ToString(),
					"s\norbit period = ",
					orbit.Period.ToString(),
					"s"
				}), Array.Empty<object>());
				num2 = orbit.TimeAtPoint_s.Length - 1;
			}
			Array.Copy(orbit.ScaledPoints, num2, orbit.ScaledPoints, num2 + 1, orbit.ScaledPoints.Length - num2 - 1);
			orbit.ScaledPoints[num2] = this.cameraManager.ScaledPosition(spaceObject.Position);
			int num4 = orbit.OrbitTrail.points3.Count - 1;
			List<Color> list = new List<Color>(num4);
			Color color = orbit.OrbitTrail.GetColor(0);
			double num5 = orbit.TimeAtPoint_s[num2];
			double num6;
			if (num2 > 0)
			{
				num6 = orbit.TimeAtPoint_s[num2 - 1];
			}
			else
			{
				num6 = orbit.TimeAtPoint_s[orbit.TimeAtPoint_s.Length - 2] - orbit.TimeAtPoint_s[orbit.TimeAtPoint_s.Length - 1];
			}
			double num7 = (num5 * 2.0 + num6) / 3.0;
			num7 += orbit.Period / 1024.0;
			for (int i = 0; i < num4; i++)
			{
				double num8 = orbit.TimeAtPoint_s[i / 3];
				double num9 = ((i / 3 + 1 < orbit.TimeAtPoint_s.Length) ? orbit.TimeAtPoint_s[i / 3 + 1] : (orbit.Period + orbit.TimeAtPoint_s[1]));
				double num10 = Mathd.Lerp(num8, num9, (double)(i % 3) / 3.0);
				double num11 = num7 - num10;
				num11 = (num11 + orbit.Period) % orbit.Period;
				list.Add(new Color(color.r, color.g, color.b, (float)(num11 / orbit.Period)));
			}
			orbit.OrbitTrail.MakeSpline(orbit.ScaledPoints, false);
			orbit.OrbitTrail.smoothColor = false;
			orbit.OrbitTrail.SetColors(list);
			orbit.OrbitTrail.material.SetFloat(OrbitTrailRendering.s_uniformOrbitBodyPos, (float)(num / orbit.Period));
			orbit.OrbitTrail.Draw3D();
			GameControl.solarSystem.AddOrbitTrailToContainer(orbit.OrbitTrail.rectTransform.gameObject);
		}

		// Token: 0x06005CEB RID: 23787 RVA: 0x002C4610 File Offset: 0x002C2810
		private void DrawTransferOrbit(FleetTransferPlan plan, int segment, SpaceObjectLOD LOD)
		{
			Orbit orbit = plan.TransferSegments[segment];
			orbit.OrbitTrail.active = LOD.DisplayOrbitTrail || plan.planningOnly;
			if ((!LOD.DisplayOrbitTrail && !plan.planningOnly) || orbit.OrbitTrail.rectTransform == null)
			{
				return;
			}
			DateTime now = this.gameTime.Now;
			for (int i = 0; i < orbit.WorldPoints.Length; i++)
			{
				DateTime dateTime = plan.StartTime.AddSeconds(orbit.TimeAtPoint_s[i]);
				if (now > dateTime)
				{
					orbit.WorldPoints[i] = plan.fleet.GetGlobalPosition();
				}
				orbit.ScaledPoints[i] = this.cameraManager.ScaledPosition(orbit.WorldPoints[i]);
			}
			int num = orbit.OrbitTrail.points3.Count - 1;
			List<Color> list = new List<Color>(num);
			for (int j = 0; j < num; j++)
			{
				Color color = orbit.OrbitTrail.GetColor(j);
				list.Add(new Color(color.r, color.g, color.b, 0f));
			}
			orbit.OrbitTrail.MakeSpline(orbit.ScaledPoints, false);
			orbit.OrbitTrail.smoothColor = false;
			orbit.OrbitTrail.SetColors(list);
			orbit.OrbitTrail.Draw3D();
			if (orbit.OrbitTrail.rectTransform == null)
			{
				return;
			}
			GameControl.solarSystem.AddOrbitTrailToContainer(orbit.OrbitTrail.rectTransform.gameObject);
		}

		// Token: 0x0400426B RID: 17003
		[Inject]
		private OrbitTrailRendering.FixedOrbitGroup fixedOrbits;

		// Token: 0x0400426C RID: 17004
		[Inject]
		private OrbitTrailRendering.TransferOrbitGroup transferOrbits;

		// Token: 0x0400426D RID: 17005
		[Inject]
		private CameraManager cameraManager;

		// Token: 0x0400426E RID: 17006
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x0400426F RID: 17007
		private static int s_uniformOrbitBodyPos = Shader.PropertyToID("_OrbitBodyPos");

		// Token: 0x04004270 RID: 17008
		private bool wasTimeFlowingLastFrame;

		// Token: 0x02001344 RID: 4932
		private struct FixedOrbitGroup
		{
			// Token: 0x04006F80 RID: 28544
			public readonly int Length;

			// Token: 0x04006F81 RID: 28545
			[ReadOnly]
			public ComponentArray<OrbitComponent> Orbit;

			// Token: 0x04006F82 RID: 28546
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006F83 RID: 28547
			[ReadOnly]
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006F84 RID: 28548
			private SubtractiveComponent<TransferPlanComponent> _;

			// Token: 0x04006F85 RID: 28549
			private SubtractiveComponent<NavigableComponent> _2;
		}

		// Token: 0x02001345 RID: 4933
		private struct TransferOrbitGroup
		{
			// Token: 0x04006F86 RID: 28550
			public readonly int Length;

			// Token: 0x04006F87 RID: 28551
			public ComponentArray<TransferPlanComponent> Plan;

			// Token: 0x04006F88 RID: 28552
			[ReadOnly]
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006F89 RID: 28553
			[ReadOnly]
			public ComponentArray<SpaceObjectLODComponent> LOD;
		}
	}
}
