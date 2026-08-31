using System;
using System.Collections.Generic;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A3 RID: 1443
	public class SpaceObjectController : MonoBehaviour
	{
		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x060026EF RID: 9967 RVA: 0x000D3F12 File Offset: 0x000D2112
		public SpaceBodyRotationComponent SpaceBodyRotationComponent
		{
			get
			{
				return base.GetComponent<SpaceBodyRotationComponent>();
			}
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x060026F0 RID: 9968 RVA: 0x000D3F1A File Offset: 0x000D211A
		public bool HasSpaceBodyRotation
		{
			get
			{
				return this.SpaceBodyRotationComponent != null;
			}
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x060026F1 RID: 9969 RVA: 0x000D3F28 File Offset: 0x000D2128
		public SpaceBodyRotation SpaceBodyRotation
		{
			get
			{
				if (!this.HasSpaceBodyRotation)
				{
					return default(SpaceBodyRotation);
				}
				return this.SpaceBodyRotationComponent.Value;
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x060026F2 RID: 9970 RVA: 0x000D3F52 File Offset: 0x000D2152
		public SpaceObjectComponent SpaceObjectComponent
		{
			get
			{
				return base.GetComponent<SpaceObjectComponent>();
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x060026F3 RID: 9971 RVA: 0x000D3F5A File Offset: 0x000D215A
		public bool HasSpaceObject
		{
			get
			{
				return this.SpaceObjectComponent != null;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x060026F4 RID: 9972 RVA: 0x000D3F68 File Offset: 0x000D2168
		public SpaceObject SpaceObject
		{
			get
			{
				if (!this.HasSpaceObject)
				{
					return default(SpaceObject);
				}
				return this.SpaceObjectComponent.Value;
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x060026F5 RID: 9973 RVA: 0x000D3F92 File Offset: 0x000D2192
		public bool HasSymbol
		{
			get
			{
				return this.symbolController != null;
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x060026F6 RID: 9974 RVA: 0x000D3FA0 File Offset: 0x000D21A0
		public bool HasMap
		{
			get
			{
				return this.mapController != null;
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x060026F7 RID: 9975 RVA: 0x000D3FAE File Offset: 0x000D21AE
		public float radius_gameUnits
		{
			get
			{
				return this.sphereCollider.radius;
			}
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x000D3FBB File Offset: 0x000D21BB
		private int LagrangePointIndex()
		{
			if (this.spaceObjectState.isLagrangePointState)
			{
				return (int)this.spaceObjectState.ref_lagrangePoint.lagrangeValue;
			}
			return 0;
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x000D3FDC File Offset: 0x000D21DC
		private void OnDisable()
		{
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(STOP_MODE.IMMEDIATE);
				if (this.spaceObjectState.objectType == SpaceObjectType.Fleet)
				{
					this.eventInstance.release();
				}
			}
		}

		// Token: 0x060026FA RID: 9978 RVA: 0x000D4014 File Offset: 0x000D2214
		public void SetAmbientAudioClip()
		{
			string text = string.Empty;
			switch (this.spaceObjectState.objectType)
			{
			case SpaceObjectType.Planet:
				text = ((this.spaceObjectState.ref_spaceBody.meanRadius_km > 20000.0) ? "event:/SFX/Environment/trig_SFX_AmbientGasGiant" : "event:/SFX/Environment/trig_SFX_AmbientPlanet");
				break;
			case SpaceObjectType.DwarfPlanet:
				text = "event:/SFX/Environment/trig_SFX_AmbientDwarfPlanet";
				break;
			case SpaceObjectType.Asteroid:
				text = "event:/SFX/Environment/trig_SFX_AmbientAsteroid";
				break;
			case SpaceObjectType.Comet:
				text = "event:/SFX/Environment/trig_SFX_AmbientAsteroid";
				break;
			case SpaceObjectType.PlanetaryMoon:
				text = "event:/SFX/Environment/trig_SFX_AmbientAlienShip";
				break;
			case SpaceObjectType.AsteroidalMoon:
				text = "event:/SFX/Environment/trig_SFX_AmbientAsteroid";
				break;
			case SpaceObjectType.Fleet:
				text = (this.spaceObjectState.ref_fleet.IsAlien() ? "event:/SFX/Environment/trig_SFX_AmbientAlienShip" : "event:/SFX/Environment/trig_SFX_AmbientHumanShip");
				this.thrusterAudio = false;
				if (this.spaceObjectState.ref_fleet.inTransfer)
				{
					text = this.spaceObjectState.ref_fleet.ships[0].ThrusterSFXStringStrategyLayer();
					this.thrusterAudio = true;
				}
				break;
			case SpaceObjectType.Hab:
				if (this.spaceObjectState.ref_hab.IsStation)
				{
					text = (this.spaceObjectState.ref_hab.IsAlien() ? "event:/SFX/Environment/trig_SFX_AmbientAlienHab" : "event:/SFX/Environment/HumanHabMix");
				}
				if (this.spaceObjectState.ref_hab.IsBase)
				{
					text = (this.spaceObjectState.ref_hab.IsAlien() ? "event:/SFX/Environment/trig_SFX_AmbientAlienHab" : "event:/SFX/Environment/trig_SFX_AmbientAlienHQHab");
				}
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.eventInstance = AudioManager.CreateFMODInstance(text, this.modelLink.gameObject);
				this.eventInstance.SetTime(global::UnityEngine.Random.Range(0, this.eventInstance.GetLength()));
				this.eventInstance.SetVolume(0f);
			}
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x000D41D8 File Offset: 0x000D23D8
		public void TurnOffAmbientAudio()
		{
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x000D41F4 File Offset: 0x000D23F4
		public void TurnOnAmbientAudio()
		{
			if (this.eventInstance.isValid() && !this.eventInstance.IsPlaying())
			{
				this.eventInstance.Play();
			}
		}

		// Token: 0x060026FD RID: 9981 RVA: 0x000D421C File Offset: 0x000D241C
		public void ToggleAmbientAudioClipState()
		{
			if (!this.eventInstance.IsPlaying())
			{
				this.TurnOffAmbientAudio();
				return;
			}
			this.TurnOnAmbientAudio();
		}

		// Token: 0x060026FE RID: 9982 RVA: 0x000D4238 File Offset: 0x000D2438
		public void Initialize(TIGameState state)
		{
			SpaceObjectController.<>c__DisplayClass40_0 CS$<>8__locals1 = new SpaceObjectController.<>c__DisplayClass40_0();
			CS$<>8__locals1.<>4__this = this;
			if (GameControl.bootstrapFinished && !this.initialized)
			{
				if (state.isHabState && state.ref_hab.IsBase)
				{
					return;
				}
				this.spaceObjectControllerTransform = base.transform;
				Transform transform = this.spaceObjectControllerTransform.Find("Selector");
				this.sphereCollider = transform.GetComponent<SphereCollider>();
				if (this.sphereCollider == null)
				{
					Log.Error("No collider on SpaceObject selector " + state.templateName, Array.Empty<object>());
					return;
				}
				this.spaceObjectState = state as TISpaceObjectState;
				if (this.spaceObjectState == null)
				{
					throw new Exception("Null SpaceObjectState");
				}
				if (this.spaceObjectState.isLagrangePointState)
				{
					TILagrangePointState ref_lagrangePoint = this.spaceObjectState.ref_lagrangePoint;
					base.GetComponent<NavigableComponent>().State = ref_lagrangePoint;
					base.GetComponent<SpaceObjectComponent>().Controller = this;
					GameObject gameObject = ref_lagrangePoint.secondaryObject.gameObjectLink;
					if (gameObject == null)
					{
						gameObject = GameObject.Find(ref_lagrangePoint.secondaryObject.ID.ToString());
					}
					Entity entity = gameObject.GetComponent<TIGameObjectEntity>().Entity;
					base.GetComponent<LagrangePointComponent>().Value = new LagrangePoint
					{
						RelatedSpaceBody = entity,
						Point = this.LagrangePointIndex()
					};
					SpaceObjectComponent component = base.GetComponent<SpaceObjectComponent>();
					SpaceObject spaceObject = new SpaceObject
					{
						ObjectType = this.spaceObjectState.objectType,
						Mass = this.spaceObjectState.mass_kg,
						MeanRadius = 500.0,
						SpatialRotation = Quaterniond.identity,
						ModelScale = 1.0,
						MapScale = 1.0,
						Epoch = ref_lagrangePoint.secondaryObject.epoch_DateTime.ExportTime()
					};
					component.Value = spaceObject;
					GameObject gameObject2 = ref_lagrangePoint.secondaryObject.barycenter.gameObjectLink;
					if (gameObject2 == null)
					{
						gameObject2 = GameObject.Find(ref_lagrangePoint.secondaryObject.barycenter.ID.ToString());
					}
					Orbit orbit = new Orbit
					{
						Eccentricity = ref_lagrangePoint.ecc,
						SemimajorAxis_m = ref_lagrangePoint.semiMajorAxis_m,
						Inclination_Rad = ref_lagrangePoint.inclination_Rad,
						LongitudeAscendingNode_Rad = ref_lagrangePoint.longAscendingNode_Rad,
						MeanAnomalyAtEpoch_Rad = ref_lagrangePoint.meanAnomalyAtEpoch_Rad,
						ArgumentPeriapsis_Rad = ref_lagrangePoint.argPeriapsis_Rad,
						Epoch = ref_lagrangePoint.epoch_DateTime.ExportTime(),
						Barycenter = gameObject2.GetComponent<TIGameObjectEntity>().Entity
					}.Fill(false);
					base.GetComponent<OrbitComponent>().Value = orbit;
				}
				else
				{
					if (this.spaceObjectState.orbitalPeriod_s > 60.0)
					{
						if (this.spaceObjectState.epoch_DateTime != null && this.spaceObjectState.epoch_DateTime > TITimeState.Now())
						{
							while (this.spaceObjectState.epoch_DateTime > TITimeState.Now())
							{
								this.spaceObjectState.epoch_DateTime.AddSeconds(-this.spaceObjectState.orbitalPeriod_Years * 31556924.0);
							}
						}
					}
					else if (this.spaceObjectState.isSpaceAssetState && !double.IsNaN(this.spaceObjectState.orbitalPeriod_s))
					{
						Log.Error("Mobile space object " + this.spaceObjectState.displayName + " has near-zero period.", Array.Empty<object>());
					}
					SpaceObjectController.<>c__DisplayClass40_0 CS$<>8__locals2 = CS$<>8__locals1;
					SpaceObject spaceObject = new SpaceObject
					{
						ObjectType = this.spaceObjectState.objectType,
						Mass = this.spaceObjectState.mass_kg,
						MeanRadius = this.spaceObjectState.meanRadius_m,
						SpatialRotation = this.spaceObjectState.SpatialRotation,
						ModelScale = (double)this.spaceObjectState.modelScale,
						MapScale = (this.spaceObjectState.isSpaceBodyState ? this.spaceObjectState.ref_spaceBody.mapScale : 1.0),
						Epoch = this.spaceObjectState.epoch_DateTime.ExportTime()
					};
					CS$<>8__locals2.spaceObject = spaceObject;
					if (this.spaceObjectState.isSpaceBodyState)
					{
						TISpaceBodyState ref_spaceBody = this.spaceObjectState.ref_spaceBody;
						base.GetComponent<SpaceBodyRotationComponent>().Value = new SpaceBodyRotation
						{
							RotationPeriod_s = ref_spaceBody.rotationperiod_s,
							RotationOffset_rad = (double)ref_spaceBody.rotationOffset_Deg * 0.017453292519943295
						};
					}
					if (this.spaceObjectState.barycenter != null)
					{
						GameObject gameObject3 = this.spaceObjectState.barycenter.gameObjectLink;
						if (gameObject3 == null)
						{
							gameObject3 = GameObject.Find(this.spaceObjectState.barycenter.ID.ToString());
						}
						if (gameObject3 == null)
						{
							Log.Error("Could not find barycenter " + this.spaceObjectState.barycenter.displayName + " for " + this.spaceObjectState.displayName, Array.Empty<object>());
							return;
						}
						try
						{
							this.UpdateOrbitComponentForAsset(false);
						}
						catch
						{
							this.UpdateOrbitComponentForAsset(false);
						}
						CS$<>8__locals1.spaceObject.SOI = (this.spaceObjectState.isNaturalSpaceObjectState ? this.spaceObjectState.ref_naturalSpaceObject.sphereOfInfluence_m : 0.0);
					}
					if (this.spaceObjectState.isSpaceFleetState)
					{
						FleetComponent component2 = base.gameObject.GetComponent<FleetComponent>();
						component2.Fleet = this.spaceObjectState.ref_fleet;
						component2.Controller = this;
						component2.Value = default(FleetComponentObject);
					}
					if (this.spaceObjectState.template != null && this.spaceObjectState.isSun)
					{
						World.Active.GetExistingManager<SpaceObjectSelection>().SelectObject(base.gameObject, false, false);
						CameraManager existingManager = World.Active.GetExistingManager<CameraManager>();
						CameraManager cameraManager = existingManager;
						CameraManager cameraManager2 = existingManager;
						SVector3d svector3d = new SVector3d(1000000000000.0, 0.7330383062362671, 2.7925267219543457);
						cameraManager2.Spherical = svector3d;
						cameraManager.TargetSpherical = svector3d;
						existingManager.Transform.rotation = existingManager.TargetLookRotation;
					}
					base.GetComponent<SpaceObjectComponent>().Value = CS$<>8__locals1.spaceObject;
					base.GetComponent<SpaceObjectComponent>().State = this.spaceObjectState;
					base.GetComponent<SpaceObjectComponent>().Controller = this;
					if (this.spaceObjectState.isSpaceBodyState && !string.IsNullOrEmpty(this.spaceObjectState.ref_spaceBody.mapResource))
					{
						Log.Time("<color=#00cc00>LoadTime:</color> SpaceObjectController Create MapController", delegate
						{
							CS$<>8__locals1.<>4__this.mapLink = GameControl.assetLoader.InstantiatePrefab("mapicons/MapController", CS$<>8__locals1.<>4__this.transform);
							CS$<>8__locals1.<>4__this.mapTransform = CS$<>8__locals1.<>4__this.mapLink.transform;
							CS$<>8__locals1.<>4__this.mapController = CS$<>8__locals1.<>4__this.mapLink.GetComponent<MapController>();
							if (CS$<>8__locals1.<>4__this.mapController != null)
							{
								CS$<>8__locals1.<>4__this.mapController.InitializeMap(CS$<>8__locals1.<>4__this, CS$<>8__locals1.<>4__this.spaceObjectState.ref_spaceBody.mapResource);
								CS$<>8__locals1.<>4__this.mapLink.layer = LayerMask.NameToLayer("Solar System");
							}
							MapComponent orAdd = CS$<>8__locals1.<>4__this.gameObject.GetOrAdd<MapComponent>();
							orAdd.MapController = CS$<>8__locals1.<>4__this.mapController;
							orAdd.State = CS$<>8__locals1.<>4__this.spaceObjectState as TISpaceBodyState;
							orAdd.SpaceObjectController = CS$<>8__locals1.<>4__this;
							orAdd.SpaceObjectLink = CS$<>8__locals1.spaceObject;
							orAdd.LodComponentLink = CS$<>8__locals1.<>4__this.GetComponent<SpaceObjectLODComponent>();
						}, true, true);
					}
				}
				if (!string.IsNullOrEmpty("ui/SpaceObjectSymbol"))
				{
					this.symbolLink = GameControl.assetLoader.InstantiatePrefab("ui/SpaceObjectSymbol", base.transform);
					this.symbolController = this.symbolLink.GetComponent<SpaceObjectSymbolController>();
					this.symbolController.InitializeSymbol(this.spaceObjectState, this);
					this.symbolTransform = this.symbolLink.transform;
					this.symbolLink.GetComponent<Canvas>().worldCamera = GameControl.control.mainCamera;
				}
				if (this.spaceObjectState.objectType != SpaceObjectType.Hab)
				{
					if (!string.IsNullOrEmpty(this.spaceObjectState.modelResource))
					{
						this.modelLink = GameControl.assetLoader.InstantiatePrefab(this.spaceObjectState.modelResource, base.transform);
						if (this.modelLink == null)
						{
							Log.Error("Failed to load model for " + this.spaceObjectState.displayName, Array.Empty<object>());
						}
						else
						{
							this.modelLink.layer = LayerMask.NameToLayer("Solar System");
							if (!(this.spaceObjectState is TILagrangePointState))
							{
								this.modelController = this.modelLink.GetComponent<SolarSysModelController>();
								if (this.modelController == null)
								{
									if (!this.spaceObjectState.isSpaceBodyState)
									{
										Log.Error(string.Format("Missing SolarSysModel on {0}({1})", this.spaceObjectState.displayName, this.spaceObjectState.ID), Array.Empty<object>());
										return;
									}
									this.modelController = this.modelLink.AddComponent<SpaceBodyController>();
								}
								this.modelController.InitializeModel(this);
							}
							this.modelLink.SetActive(false);
						}
					}
					else
					{
						Log.Warn("no model resource for " + this.spaceObjectState.templateName, Array.Empty<object>());
					}
				}
				if (this.spaceObjectState.objectType == SpaceObjectType.Fleet)
				{
					this.AddFleetListeners();
				}
				else if (this.spaceObjectState.isSpaceBodyState)
				{
					this.AddSpaceBodyListeners();
				}
				if (this.spaceObjectState.objectType == SpaceObjectType.Comet)
				{
					global::UnityEngine.Object.Instantiate<CometController>(this.CometControllerPrefab, base.transform);
				}
				this.initialized = true;
			}
		}

		// Token: 0x060026FF RID: 9983 RVA: 0x000D4AF0 File Offset: 0x000D2CF0
		public void DestroyThis()
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x000D4AFD File Offset: 0x000D2CFD
		private void AddSpaceBodyListeners()
		{
			GameControl.eventManager.AddListener<ForceUpdateSpaceBodyModel>(new EventManager.EventDelegate<ForceUpdateSpaceBodyModel>(this.OnForceUpdateSpaceBodyModel), null, this.spaceObjectState.ref_spaceBody, true, false);
		}

		// Token: 0x06002701 RID: 9985 RVA: 0x000D4B23 File Offset: 0x000D2D23
		private void RemoveSpaceBodyListeners()
		{
			GameControl.eventManager.RemoveListener<ForceUpdateSpaceBodyModel>(new EventManager.EventDelegate<ForceUpdateSpaceBodyModel>(this.OnForceUpdateSpaceBodyModel), null);
		}

		// Token: 0x06002702 RID: 9986 RVA: 0x000D4B3C File Offset: 0x000D2D3C
		private void OnForceUpdateSpaceBodyModel(ForceUpdateSpaceBodyModel e)
		{
			GameObject gameObject = this.modelLink;
			gameObject.SetActive(false);
			this.modelLink = GameControl.assetLoader.InstantiatePrefab(this.spaceObjectState.modelResource, base.transform);
			this.modelLink.transform.localScale = gameObject.transform.localScale;
			this.modelLink.transform.localPosition = gameObject.transform.localPosition;
			this.modelLink.transform.localRotation = gameObject.transform.localRotation;
			this.modelLink.layer = LayerMask.NameToLayer("Solar System");
			this.modelController = this.modelLink.AddComponent<SpaceBodyController>();
			this.modelController.InitializeModel(this);
			global::UnityEngine.Object.Destroy(gameObject);
			GameControl.eventManager.TriggerEvent(new ForceUpdateSpaceBodyModelFinished(e.spaceBody), null, Array.Empty<object>());
		}

		// Token: 0x06002703 RID: 9987 RVA: 0x000D4C1C File Offset: 0x000D2E1C
		private void AddFleetListeners()
		{
			GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.UpdateFleetComposition), null, this.spaceObjectState, false, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateFleetComposition), null, this.spaceObjectState, false, false);
			GameControl.eventManager.AddListener<ResetFleetFormationVisuals>(new EventManager.EventDelegate<ResetFleetFormationVisuals>(this.OnFleetFormationReset), null, this.spaceObjectState, false, false);
			GameControl.eventManager.AddListener<CombatStarts>(new EventManager.EventDelegate<CombatStarts>(this.OnCombatBegin), null, null, true, false);
		}

		// Token: 0x06002704 RID: 9988 RVA: 0x000D4CA0 File Offset: 0x000D2EA0
		private void RemoveFleetListeners()
		{
			GameControl.eventManager.RemoveListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.UpdateFleetComposition), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateFleetComposition), null);
			GameControl.eventManager.RemoveListener<ResetFleetFormationVisuals>(new EventManager.EventDelegate<ResetFleetFormationVisuals>(this.OnFleetFormationReset), null);
			GameControl.eventManager.RemoveListener<CombatStarts>(new EventManager.EventDelegate<CombatStarts>(this.OnCombatBegin), null);
		}

		// Token: 0x06002705 RID: 9989 RVA: 0x000D4D0C File Offset: 0x000D2F0C
		public void UpdateOrbitComponentForAsset(bool destroyOnly = false)
		{
			if (this.orbitTrailLink != null)
			{
				global::UnityEngine.Object.Destroy(this.orbitTrailLink);
			}
			if (!destroyOnly)
			{
				TIDateTime tidateTime = TITimeState.Now();
				TISpaceFleetState tispaceFleetState = this.spaceObjectState as TISpaceFleetState;
				Orbit orbit;
				if (tispaceFleetState != null && (tispaceFleetState.inTransfer || (tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime <= tidateTime)))
				{
					TIOrbitState tiorbitState;
					if ((tiorbitState = tispaceFleetState.trajectory.originOrbit) == null)
					{
						tiorbitState = tispaceFleetState.trajectory.destinationOrbit ?? GameStateManager.Earth().orbits[0];
					}
					TIOrbitState tiorbitState2 = tiorbitState;
					TINaturalSpaceObjectState tinaturalSpaceObjectState = tiorbitState2.barycenter;
					if (!TIGameState.Valid(tinaturalSpaceObjectState))
					{
						Log.Error("Invalid origin orbit or barycenter for " + tispaceFleetState.displayName, Array.Empty<object>());
						tinaturalSpaceObjectState = GameStateManager.Sol();
					}
					orbit = new Orbit
					{
						Eccentricity = tiorbitState2.eccentricity,
						SemimajorAxis_m = tiorbitState2.semiMajorAxis_m,
						Inclination_Rad = tiorbitState2.inclination_Rad,
						LongitudeAscendingNode_Rad = tiorbitState2.longitudeAscendingNode_Rad,
						MeanAnomalyAtEpoch_Rad = 0.0,
						Epoch = tispaceFleetState.trajectory.launchTime.ExportTime(),
						Barycenter = tinaturalSpaceObjectState.gameObjectLink.GetComponent<TIGameObjectEntity>().Entity
					}.Fill(false).FillOrbitTrail(this.spaceObjectState, out this.orbitTrailLink);
				}
				else
				{
					GameObject gameObjectLink = this.spaceObjectState.barycenter.gameObjectLink;
					orbit = new Orbit
					{
						Eccentricity = this.spaceObjectState.ecc,
						SemimajorAxis_m = this.spaceObjectState.semiMajorAxis_m,
						Inclination_Rad = this.spaceObjectState.inclination_Rad,
						LongitudeAscendingNode_Rad = this.spaceObjectState.longAscendingNode_Rad,
						MeanAnomalyAtEpoch_Rad = this.spaceObjectState.meanAnomalyAtEpoch_Rad,
						ArgumentPeriapsis_Rad = this.spaceObjectState.argPeriapsis_Rad,
						Epoch = this.spaceObjectState.epoch_DateTime.ExportTime(),
						Barycenter = gameObjectLink.GetComponent<TIGameObjectEntity>().Entity
					}.Fill(false).FillOrbitTrail(this.spaceObjectState, out this.orbitTrailLink);
				}
				base.GetComponent<OrbitComponent>().Value = orbit;
			}
		}

		// Token: 0x06002706 RID: 9990 RVA: 0x000D4F4C File Offset: 0x000D314C
		private void OnCombatBegin(CombatStarts e)
		{
			if (this.sphereCollider != null)
			{
				this.sphereCollider.enabled = false;
				return;
			}
			TISpaceObjectState tispaceObjectState = this.spaceObjectState;
			Log.Error(((tispaceObjectState != null) ? tispaceObjectState.displayName : null) + " has no sphere collider in OnCombatBegin!", Array.Empty<object>());
		}

		// Token: 0x06002707 RID: 9991 RVA: 0x000D4F9A File Offset: 0x000D319A
		public void UpdateFleetComposition(ShipsRemovedFromFleet e)
		{
			this.UpdateFleetComposition(e.fleet, e.gainingFleet);
		}

		// Token: 0x06002708 RID: 9992 RVA: 0x000D4FAE File Offset: 0x000D31AE
		public void UpdateFleetComposition(ShipsAddedToFleet e)
		{
			this.UpdateFleetComposition(e.fleet, e.fleet);
		}

		// Token: 0x06002709 RID: 9993 RVA: 0x000D4FC4 File Offset: 0x000D31C4
		public void UpdateFleetComposition(TISpaceFleetState fleetToUpdate, TISpaceFleetState newFocusFleet)
		{
			if (this.spaceObjectState == fleetToUpdate)
			{
				FleetVisController fleetVisController = this.modelController as FleetVisController;
				Dictionary<TISpaceShipState, StrategyShipController> dictionary = new Dictionary<TISpaceShipState, StrategyShipController>();
				List<FleetVisController> list = new List<FleetVisController>();
				foreach (TISpaceShipState tispaceShipState in fleetToUpdate.ships)
				{
					if (tispaceShipState != null && tispaceShipState.visualizerLink != null)
					{
						dictionary.Add(tispaceShipState, tispaceShipState.visualizerLink.strategyShipController);
					}
				}
				foreach (TISpaceShipState tispaceShipState2 in dictionary.Keys)
				{
					StrategyShipController strategyShipController = dictionary[tispaceShipState2];
					if (strategyShipController.FleetVisController != fleetVisController)
					{
						if (!list.Contains(strategyShipController.FleetVisController))
						{
							list.Add(strategyShipController.FleetVisController);
						}
						this.AddShipVisualizerToFleet(fleetVisController, strategyShipController);
					}
				}
				foreach (FleetVisController fleetVisController2 in list)
				{
					if (fleetVisController2.shipStratControllerObjects.Count == 0)
					{
						global::UnityEngine.Object.Destroy(fleetVisController2.gameObject);
					}
				}
				if (newFocusFleet != null && newFocusFleet.ships.Count > 0 && newFocusFleet != fleetToUpdate && (fleetToUpdate == GeneralControlsController.UISelectedAssetState || (fleetToUpdate == GeneralControlsController.UIOtherSelectedState && GeneralControlsController.UISelectedAssetState == null)))
				{
					TIUtilities.GotoGameState(newFocusFleet, false, true, true, false, false, -1f);
				}
			}
		}

		// Token: 0x0600270A RID: 9994 RVA: 0x000D518C File Offset: 0x000D338C
		public void AddShipVisualizerToFleet(FleetVisController fleetController, StrategyShipController stratShipController)
		{
			stratShipController.gameObject.transform.SetParent(fleetController.gameObject.transform, false);
			stratShipController.FleetVisController.shipStratControllerObjects.Remove(stratShipController.gameObject);
			stratShipController.FleetVisController = fleetController;
			fleetController.shipStratControllerObjects.Add(stratShipController.gameObject);
			stratShipController.transform.localScale = Vector3.one;
			GameControl.eventManager.TriggerEvent(new ShipVisualizationDataDirty(stratShipController.ShipState), null, new object[]
			{
				stratShipController.ShipState,
				stratShipController.ShipState.fleet
			});
		}

		// Token: 0x0600270B RID: 9995 RVA: 0x000D5228 File Offset: 0x000D3428
		public void OnFleetFormationReset()
		{
			foreach (StrategyShipController strategyShipController in base.transform.GetComponentsInChildren(typeof(StrategyShipController), true))
			{
				if (TIGameState.Valid(strategyShipController.ShipState))
				{
					if (strategyShipController.VisController.transform != null)
					{
						strategyShipController.VisController.transform.localPosition = (Vector3)strategyShipController.ShipState.fleetFormationOffset;
					}
					else
					{
						Debug.LogError(TIUtilities.CombineStrings(new string[]
						{
							"missing viscontroller transform on ",
							strategyShipController.ShipState.ID.ToString(),
							", ",
							strategyShipController.ShipState.displayName
						}));
					}
				}
			}
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x000D52F5 File Offset: 0x000D34F5
		public void OnFleetFormationReset(ResetFleetFormationVisuals e)
		{
			this.OnFleetFormationReset();
		}

		// Token: 0x0600270D RID: 9997 RVA: 0x000D5300 File Offset: 0x000D3500
		public void OnDestroy()
		{
			if (this.spaceObjectState.objectType == SpaceObjectType.Fleet)
			{
				this.RemoveFleetListeners();
			}
			else if (this.spaceObjectState.isSpaceBodyState)
			{
				this.RemoveSpaceBodyListeners();
			}
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(STOP_MODE.IMMEDIATE);
				this.eventInstance.Release();
			}
		}

		// Token: 0x04001CF3 RID: 7411
		public TISpaceObjectState spaceObjectState;

		// Token: 0x04001CF4 RID: 7412
		public GameObject modelLink;

		// Token: 0x04001CF5 RID: 7413
		public GameObject symbolLink;

		// Token: 0x04001CF6 RID: 7414
		public Transform symbolTransform;

		// Token: 0x04001CF7 RID: 7415
		public GameObject mapLink;

		// Token: 0x04001CF8 RID: 7416
		public Transform mapTransform;

		// Token: 0x04001CF9 RID: 7417
		public GameObject orbitTrailLink;

		// Token: 0x04001CFA RID: 7418
		public SolarSysModelController modelController;

		// Token: 0x04001CFB RID: 7419
		public SpaceObjectSymbolController symbolController;

		// Token: 0x04001CFC RID: 7420
		public MapController mapController;

		// Token: 0x04001CFD RID: 7421
		public SphereCollider sphereCollider;

		// Token: 0x04001CFE RID: 7422
		public Transform spaceObjectControllerTransform;

		// Token: 0x04001CFF RID: 7423
		public CometController CometControllerPrefab;

		// Token: 0x04001D00 RID: 7424
		public EventInstance eventInstance;

		// Token: 0x04001D01 RID: 7425
		public bool thrusterAudio;

		// Token: 0x04001D02 RID: 7426
		public bool initialized;
	}
}
