using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x020009A5 RID: 2469
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectRenderingLOD))]
	[UpdateAfter(typeof(SpaceObjectRendering))]
	public class SymbolRendering : StrategyLayerComponentSystem
	{
		// Token: 0x06005D13 RID: 23827 RVA: 0x002C619C File Offset: 0x002C439C
		protected override void OnUpdate()
		{
			if (this.cameraMgr.ForceVisualizationUpdate || this.cameraMgr.IsAnimating || TIUtilities.IsTimeFlowing || TIUtilities.IsTimeFlowing != this.wasTimeFlowingLastFrame || TIFrameCounter.FrameCount == GameControl.frameFinishedLoading)
			{
				this.updatedSymbols.Clear();
				for (int i = 0; i < this.spaceObjects.Length; i++)
				{
					Transform symbolTransform = this.spaceObjects.Controller[i].symbolTransform;
					SpaceObjectSymbolController symbolController = this.spaceObjects.Controller[i].symbolController;
					SpaceObjectLOD value = this.spaceObjects.LOD[i].Value;
					if (symbolTransform.gameObject.activeSelf != value.DisplaySymbol)
					{
						symbolTransform.gameObject.SetActive(value.DisplaySymbol);
						symbolController.SetVisible(value.DisplaySymbol);
						this.updatedSymbols.Add(symbolController);
					}
					if (value.DisplaySymbol)
					{
						symbolTransform.rotation = this.cameraMgr.BillboardRotation;
						symbolTransform.localScale = Vector3.one * (Vector3.Distance(symbolTransform.position, this.cameraMgr.Transform.position) * 45f / 100000f);
						symbolController.buttonImage.transform.localScale = Vector3.one * symbolController.scaleSize;
						if (value.DisplaySymbolName && !symbolController.objectName.enabled)
						{
							symbolController.SetDisplayName();
						}
						else if (!value.DisplaySymbolName && symbolController.objectName.enabled)
						{
							symbolController.HideDisplayName();
						}
					}
					if (this.updatedSymbols.Count > 12 && TIFrameCounter.FrameCount != GameControl.frameFinishedLoading)
					{
						break;
					}
				}
				for (int j = 0; j < this.updatedSymbols.Count; j++)
				{
					this.updatedSymbols[j].VisibilityChange();
				}
			}
			this.wasTimeFlowingLastFrame = TIUtilities.IsTimeFlowing;
		}

		// Token: 0x040042A0 RID: 17056
		[Inject]
		private CameraManager cameraMgr;

		// Token: 0x040042A1 RID: 17057
		[Inject]
		private SymbolRendering.SpaceObjectGroup spaceObjects;

		// Token: 0x040042A2 RID: 17058
		private List<SpaceObjectSymbolController> updatedSymbols = new List<SpaceObjectSymbolController>();

		// Token: 0x040042A3 RID: 17059
		private bool wasTimeFlowingLastFrame;

		// Token: 0x02001350 RID: 4944
		private struct SpaceObjectGroup
		{
			// Token: 0x04006FB8 RID: 28600
			public readonly int Length;

			// Token: 0x04006FB9 RID: 28601
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006FBA RID: 28602
			public ComponentArray<OrbitComponent> Orbit;

			// Token: 0x04006FBB RID: 28603
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006FBC RID: 28604
			public ComponentArray<SpaceObjectController> Controller;
		}
	}
}
