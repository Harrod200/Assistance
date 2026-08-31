using System;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x0200099D RID: 2461
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectRendering))]
	public class ModelRendering : StrategyLayerComponentSystem
	{
		// Token: 0x06005CE6 RID: 23782 RVA: 0x002C3DD4 File Offset: 0x002C1FD4
		protected override void OnUpdate()
		{
			if (this.camera.ForceVisualizationUpdate || this.camera.IsAltitudeChanging || TIUtilities.IsTimeFlowing)
			{
				if (this.selection == null)
				{
					this.selection = World.Active.GetExistingManager<SpaceObjectSelection>();
				}
				for (int i = 0; i < this.spaceObjects.Length; i++)
				{
					SpaceObjectController spaceObjectController = this.spaceObjects.Controller[i];
					GameObject modelLink = spaceObjectController.modelLink;
					if (!(modelLink == null))
					{
						SpaceObjectLOD value = this.spaceObjects.LOD[i].Value;
						if (modelLink.activeSelf != value.DisplayModel)
						{
							modelLink.SetActive(value.DisplayModel);
							if (!value.DisplayModel && spaceObjectController.eventInstance.isValid())
							{
								spaceObjectController.eventInstance.Stop(STOP_MODE.IMMEDIATE);
							}
						}
						if (value.DisplayModel)
						{
							SpaceObject value2 = this.spaceObjects.SpaceObject[i].Value;
							Transform transform = modelLink.transform;
							float num = (float)((double)Vector3.Distance(this.camera.Transform.position, transform.transform.position));
							float num2 = Mathf.Sin(Mathf.Min((float)spaceObjectController.spaceObjectState.GetAngularDiameter(), 179f) * 0.017453292f / 2f);
							float num3 = num * num2;
							transform.localScale = Vector3.one * num3 / (float)value2.ModelScale;
							EventInstance eventInstance = spaceObjectController.eventInstance;
							if (!eventInstance.isValid())
							{
								spaceObjectController.SetAmbientAudioClip();
							}
							if (spaceObjectController.spaceObjectState.objectType == SpaceObjectType.Fleet && ((spaceObjectController.spaceObjectState.ref_fleet.inTransfer && !spaceObjectController.thrusterAudio) || (!spaceObjectController.spaceObjectState.ref_fleet.inTransfer && spaceObjectController.thrusterAudio)))
							{
								eventInstance.Stop(STOP_MODE.IMMEDIATE);
								eventInstance.Release();
								spaceObjectController.SetAmbientAudioClip();
							}
							Vector3d vector3d = this.camera.Position - value2.Position;
							double num4 = Vector3d.Magnitude(in vector3d) / value2.MeanRadius;
							float num5 = 1.25f / (float)num4;
							if (this.selection.ObjectSelected != transform.parent.gameObject && num5 > 0.1f)
							{
								num5 = 0.11000001f;
							}
							if (eventInstance.isValid())
							{
								if (eventInstance.IsPlaying())
								{
									float volume = eventInstance.GetVolume();
									if (num5 <= 0.1f)
									{
										eventInstance.ChangeVolume(-0.3f * Time.deltaTime);
										if (volume <= 0f)
										{
											eventInstance.Stop(STOP_MODE.IMMEDIATE);
										}
									}
									else if (volume > num5)
									{
										if (volume - 0.3f * Time.deltaTime < num5)
										{
											eventInstance.SetVolume(num5);
										}
										else
										{
											eventInstance.ChangeVolume(-0.3f * Time.deltaTime);
										}
									}
									else if (volume < num5)
									{
										if (volume + 0.3f * Time.deltaTime > num5)
										{
											eventInstance.SetVolume(num5);
										}
										else
										{
											eventInstance.ChangeVolume(0.3f * Time.deltaTime);
										}
									}
								}
								else if (num5 > 0.1f && !spaceObjectController.spaceObjectState.deleted)
								{
									eventInstance.Play(this.spaceObjects.Controller[i].modelLink);
									eventInstance.SetTime(TIUtilities.RandomRange(0, eventInstance.GetLength()));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04004267 RID: 16999
		[Inject]
		private CameraManager camera;

		// Token: 0x04004268 RID: 17000
		[Inject]
		private ModelRendering.SpaceObjectGroup spaceObjects;

		// Token: 0x04004269 RID: 17001
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x0400426A RID: 17002
		private SpaceObjectSelection selection;

		// Token: 0x02001343 RID: 4931
		private struct SpaceObjectGroup
		{
			// Token: 0x04006F7C RID: 28540
			public readonly int Length;

			// Token: 0x04006F7D RID: 28541
			public ComponentArray<SpaceObjectComponent> SpaceObject;

			// Token: 0x04006F7E RID: 28542
			public ComponentArray<SpaceObjectLODComponent> LOD;

			// Token: 0x04006F7F RID: 28543
			public ComponentArray<SpaceObjectController> Controller;
		}
	}
}
