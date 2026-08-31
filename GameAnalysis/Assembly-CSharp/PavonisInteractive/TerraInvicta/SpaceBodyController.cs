using System;
using System.Collections.Generic;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A1 RID: 1441
	public class SpaceBodyController : SolarSysModelController
	{
		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x060026C5 RID: 9925 RVA: 0x000D2EB8 File Offset: 0x000D10B8
		// (set) Token: 0x060026C6 RID: 9926 RVA: 0x000D2EC0 File Offset: 0x000D10C0
		public TISpaceBodyState spaceBody { get; protected set; }

		// Token: 0x060026C7 RID: 9927 RVA: 0x000D2ECC File Offset: 0x000D10CC
		public override void InitializeModel(SpaceObjectController container)
		{
			base.InitializeModel(container);
			this.template = container.spaceObjectState.GetMyTemplate<TISpaceBodyTemplate>();
			this.spaceBody = container.spaceObjectState as TISpaceBodyState;
			this.spaceBodyCollider = base.GetComponent<SphereCollider>();
			this.activeShotEffects = new Dictionary<BeamWeaponController, SpaceBodyController.ActiveSTOEffect>();
			if (this.spaceBody.isEarth)
			{
				GameControl.control.viewMgr.SetEarthObject(container);
			}
			this.habSiteControllers = new List<HabSiteController>();
			foreach (TIHabSiteState tihabSiteState in this.spaceBody.habSites)
			{
				GameObject gameObject = GameControl.assetLoader.InstantiatePrefab("planets/HabSiteSymbol", base.transform);
				gameObject.transform.SetLayer(8, true);
				HabSiteController component = gameObject.GetComponent<HabSiteController>();
				this.habSiteControllers.Add(component);
				component.Initialize(tihabSiteState, this);
			}
			if (this.spaceBodyCollider != null)
			{
				this.spaceBodyCollider.radius = this.spaceBody.template.ModelScale;
			}
			base.SetShadowBehavior();
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x000D2FCC File Offset: 0x000D11CC
		public BeamWeaponController RequestSTOBeam(Func<TISpaceFleetState, Vector3> origin, TIRegionState originState, TISpaceShipState targetShip, TISpaceObjectState modelState, TIDateTime time, TIBeamWeaponTemplate weaponTemplate)
		{
			SpaceBodyController.ActiveSTOEffect activeSTOEffect = new SpaceBodyController.ActiveSTOEffect
			{
				Prefab = GameControl.assetLoader.LoadAsset<GameObject>((weaponTemplate == null) ? "spacecombat/Standard Laser Beam Red" : weaponTemplate.effectResource),
				TargetShip = targetShip,
				ModelState = modelState,
				Origin = origin
			};
			Vector3 vector = CameraManager.Singleton.ScaledPosition_DoNotTouchCache(originState.GetGlobalPosition(time));
			Vector3 position = targetShip.fleet.controller.transform.position;
			GameObject gameObject;
			BeamWeaponController beamWeaponController;
			if (this.TryGetInActiveSTOBeamInstance(out gameObject))
			{
				gameObject.transform.position = vector;
				gameObject.transform.rotation = Quaternion.LookRotation((position - vector).normalized);
				gameObject.transform.localScale = Vector3.one * (1f / modelState.modelScale);
				gameObject.SetActive(true);
				activeSTOEffect.Instance = gameObject;
				beamWeaponController = gameObject.GetComponent<BeamWeaponController>();
				beamWeaponController.EnableLaser();
				beamWeaponController.Initialize(originState, targetShip, time, LayerMask.NameToLayer("Solar System"));
				gameObject.GetComponent<LineRenderer>().endWidth = 0f;
			}
			else
			{
				GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>((weaponTemplate == null) ? "spacecombat/Standard Laser Beam Red" : weaponTemplate.effectResource), vector, Quaternion.LookRotation((position - vector).normalized));
				gameObject2.transform.SetParent(modelState.gameObjectLink.transform);
				gameObject2.transform.localScale = Vector3.one * (1f / modelState.modelScale);
				gameObject2.SetActive(true);
				activeSTOEffect.Instance = gameObject2;
				beamWeaponController = gameObject2.GetComponent<BeamWeaponController>();
				beamWeaponController.EnableLaser();
				beamWeaponController.Initialize(originState, targetShip, time, LayerMask.NameToLayer("Solar System"));
				gameObject2.GetComponent<LineRenderer>().endWidth = 0f;
				this.activeShotEffects.Add(beamWeaponController, activeSTOEffect);
			}
			if (base.gameObject.activeInHierarchy)
			{
				string text = ((weaponTemplate == null) ? "event:/SFX/Game_SFX/Lasers/trig_SFX_Charged_Particle_Turret" : weaponTemplate.fireSoundFXResource);
				if (!this.eventInstance.isValid())
				{
					this.eventInstance = AudioManager.CreateFMODInstance(text);
				}
				this.eventInstance.Play(base.gameObject);
			}
			return beamWeaponController;
		}

		// Token: 0x060026C9 RID: 9929 RVA: 0x000D3208 File Offset: 0x000D1408
		public void ReleaseSTOBeamController(BeamWeaponController controller)
		{
			SpaceBodyController.ActiveSTOEffect activeSTOEffect;
			if (controller != null && this.activeShotEffects.TryGetValue(controller, out activeSTOEffect))
			{
				controller.DisableLaser();
				activeSTOEffect.Instance.SetActive(false);
			}
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x000D3240 File Offset: 0x000D1440
		public void OnMouseEnter()
		{
			if (GeneralControlsController.UITargetingMode != null && GameControl.control.viewMgr.currentView != ViewType.PoliticalMap && !EventSystem.current.IsPointerOverGameObject())
			{
				if (this.OrbitValidation(GeneralControlsController.UITargetingMode.GetPossibleTargets, this.spaceBody.ref_naturalSpaceObject.orbits))
				{
					TIInputManager.SetCursor(TIInputManager.targetCursorValid, true);
					return;
				}
				TIInputManager.SetCursor(TIInputManager.targetCursor, true);
			}
		}

		// Token: 0x060026CB RID: 9931 RVA: 0x000D32AB File Offset: 0x000D14AB
		public void OnMouseExit()
		{
			if (GeneralControlsController.UITargetingMode != null && GameControl.control.viewMgr.currentView != ViewType.PoliticalMap && !EventSystem.current.IsPointerOverGameObject())
			{
				TIInputManager.SetCursor(TIInputManager.targetCursor, true);
			}
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x000D32E0 File Offset: 0x000D14E0
		public void LateUpdate()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			foreach (KeyValuePair<BeamWeaponController, SpaceBodyController.ActiveSTOEffect> keyValuePair in this.activeShotEffects)
			{
				if (keyValuePair.Value.Instance.activeInHierarchy)
				{
					keyValuePair.Value.Instance.transform.position = keyValuePair.Value.Origin(keyValuePair.Value.TargetShip.fleet);
				}
			}
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x000D3388 File Offset: 0x000D1588
		private bool OrbitValidation(IList<TIGameState> targetList, List<TIOrbitState> orbitList)
		{
			for (int i = 0; i < orbitList.Count; i++)
			{
				if (targetList.Contains(orbitList[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060026CE RID: 9934 RVA: 0x000D33B8 File Offset: 0x000D15B8
		private bool TryGetInActiveSTOBeamInstance(out GameObject beamEffect)
		{
			foreach (SpaceBodyController.ActiveSTOEffect activeSTOEffect in this.activeShotEffects.Values)
			{
				if (!activeSTOEffect.Instance.activeSelf)
				{
					beamEffect = activeSTOEffect.Instance;
					return true;
				}
			}
			beamEffect = null;
			return false;
		}

		// Token: 0x04001CCF RID: 7375
		protected TISpaceBodyTemplate template;

		// Token: 0x04001CD1 RID: 7377
		public List<HabSiteController> habSiteControllers;

		// Token: 0x04001CD2 RID: 7378
		private SphereCollider spaceBodyCollider;

		// Token: 0x04001CD3 RID: 7379
		private EventInstance eventInstance;

		// Token: 0x04001CD4 RID: 7380
		private Dictionary<BeamWeaponController, SpaceBodyController.ActiveSTOEffect> activeShotEffects;

		// Token: 0x02000D00 RID: 3328
		private struct ActiveSTOEffect
		{
			// Token: 0x04005030 RID: 20528
			public GameObject Prefab;

			// Token: 0x04005031 RID: 20529
			public GameObject Instance;

			// Token: 0x04005032 RID: 20530
			public TISpaceShipState TargetShip;

			// Token: 0x04005033 RID: 20531
			public TISpaceObjectState ModelState;

			// Token: 0x04005034 RID: 20532
			public Func<TISpaceFleetState, Vector3> Origin;
		}
	}
}
