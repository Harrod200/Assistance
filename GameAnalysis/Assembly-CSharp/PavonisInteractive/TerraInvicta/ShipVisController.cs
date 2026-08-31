using System;
using System.Collections;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200058C RID: 1420
	public class ShipVisController : MonoBehaviour
	{
		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x060025B7 RID: 9655 RVA: 0x000CC7F0 File Offset: 0x000CA9F0
		// (set) Token: 0x060025B8 RID: 9656 RVA: 0x000CC7F8 File Offset: 0x000CA9F8
		public ShipUIController UIController { get; private set; }

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x060025B9 RID: 9657 RVA: 0x000CC801 File Offset: 0x000CAA01
		// (set) Token: 0x060025BA RID: 9658 RVA: 0x000CC809 File Offset: 0x000CAA09
		public bool UIVisualizationOnly { get; private set; }

		// Token: 0x060025BB RID: 9659 RVA: 0x000CC814 File Offset: 0x000CAA14
		public void InitializeModelOnly(TISpaceShipTemplate shipTemplate)
		{
			this.shipState = null;
			this.fleetVisController = null;
			this.modelLink = GameControl.assetLoader.InstantiatePrefab(shipTemplate.hullTemplate.modelResource[shipTemplate.GetHullAppearanceIndex], base.transform);
			this.ModelController = this.modelLink.GetComponent<ShipModelController>();
			this.ModelController.BuildShip(this, shipTemplate, null, false);
			if (this.ModelController != null && this.ModelController.selectionAnimObject != null)
			{
				this.ModelController.selectionAnimObject.SetActive(false);
			}
			if (this.ModelController != null && this.ModelController.groupSelectionAnimObject != null)
			{
				this.ModelController.groupSelectionAnimObject.SetActive(false);
			}
			if (this.ModelController != null && this.ModelController.padlockIconObject != null)
			{
				this.ModelController.padlockIconObject.SetActive(false);
			}
			BoxCollider[] componentsInChildren = base.GetComponentsInChildren<BoxCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			CapsuleCollider[] componentsInChildren2 = base.GetComponentsInChildren<CapsuleCollider>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].enabled = false;
			}
		}

		// Token: 0x060025BC RID: 9660 RVA: 0x000CC94C File Offset: 0x000CAB4C
		public void InitializeShipVisualizer(TISpaceShipTemplate shipTemplate, TISpaceShipState ship, FleetVisController fleetVisController, StrategyShipController strategyShipController, bool fullVersion)
		{
			this.shipState = ship;
			this.fleetVisController = fleetVisController;
			this.strategyShipController = strategyShipController;
			base.name = this.shipState.ID.ToString();
			this.modelLink = GameControl.assetLoader.InstantiatePrefab(shipTemplate.hullTemplate.modelResource[shipTemplate.GetHullAppearanceIndex], base.transform);
			this.modelLink.SetActive(false);
			this.ModelController = this.modelLink.GetComponent<ShipModelController>();
			this.ModelController.BuildShip(this, shipTemplate, this.shipState, false);
			if (this.shipState.radiators != null)
			{
				this.ModelController.SetRadiatorEmissiveKelvinRange(295.0, (double)this.shipState.radiators.operatingTemp_K);
			}
			if (fullVersion)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>("ui_spaceCombat/Ship UI Object"), base.transform);
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.UIController = gameObject.GetComponent<ShipUIController>();
				this.UIController.Initialize(this);
				this.UIVisualizationOnly = false;
				GameControl.eventManager.AddListener<ShipEntersCombat>(new EventManager.EventDelegate<ShipEntersCombat>(this.EnterCombat), this.shipState.ID.ToString(), this.shipState, true, false);
				GameControl.eventManager.AddListener<ShipLeavesCombat>(new EventManager.EventDelegate<ShipLeavesCombat>(this.PostCombat), this.shipState.ID.ToString(), this.shipState, false, false);
			}
			else
			{
				this.SetAsUIVisualization(this.shipState, false);
			}
			if (fleetVisController != null)
			{
				base.transform.localPosition = ((this.shipState.fleet.formation.pattern == null) ? ((Vector3)this.shipState.defaultPositionOnCreation) : ((Vector3)this.shipState.fleetFormationOffset));
			}
			if (this.ModelController != null && this.ModelController.selectionAnimObject != null)
			{
				this.ModelController.selectionAnimObject.SetActive(false);
			}
			if (this.ModelController != null && this.ModelController.groupSelectionAnimObject != null)
			{
				this.ModelController.groupSelectionAnimObject.SetActive(false);
			}
			if (this.ModelController != null && this.ModelController.padlockIconObject != null)
			{
				this.ModelController.padlockIconObject.SetActive(false);
			}
			this.modelLink.SetActive(true);
			if (this.shipState.thrustersActive)
			{
				this.ModelController.ActivateThrusters(false);
			}
		}

		// Token: 0x060025BD RID: 9661 RVA: 0x000CCC04 File Offset: 0x000CAE04
		public void SetAsUIVisualization(TISpaceShipState shipState, bool copiedFleet)
		{
			this.UIVisualizationOnly = true;
			if (shipState != null)
			{
				if (copiedFleet)
				{
					this.shipState = shipState;
					base.name += " UI Clone";
					this.ModelController.SetShipCopy(shipState);
					this.ModelController.name = this.ModelController.name + " UI Clone";
					this.ModelController.SetRadiators(shipState.template);
					this.modelLink = this.ModelController.gameObject;
					this.UIController = base.GetComponentInChildren<ShipUIController>();
					foreach (ShipWeaponVisController shipWeaponVisController in this.ModelController.allWeaponControllers)
					{
						if (shipWeaponVisController.weaponExplosionInstance != null)
						{
							global::UnityEngine.Object.Destroy(shipWeaponVisController.weaponExplosionInstance);
						}
					}
					this.UIController.gameObject.SetActive(false);
				}
				Collider[] componentsInChildren = base.transform.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
				this.ModelController.allWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
				{
					x.enabled = false;
				});
			}
		}

		// Token: 0x060025BE RID: 9662 RVA: 0x000CCD64 File Offset: 0x000CAF64
		public void DisableAllThrusterFX()
		{
			this.ModelController.DeactivateAllVectorThrusters();
			this.ModelController.DeactivateThrusters(false);
			this.ModelController.DisableRadiatorEmissives();
		}

		// Token: 0x060025BF RID: 9663 RVA: 0x000CCD88 File Offset: 0x000CAF88
		private void EnterCombat(ShipEntersCombat e)
		{
			this.DisableAllThrusterFX();
		}

		// Token: 0x060025C0 RID: 9664 RVA: 0x000CCD90 File Offset: 0x000CAF90
		private void PostCombat(ShipLeavesCombat e)
		{
			this.shipState.DeactivateThrusters();
			this.ModelController.ResetRadiatorEmissives();
			this.ModelController.StopSelectionAnimation();
			this.ModelController.StopGroupSelectionAnimation();
			if (this.UIController != null)
			{
				this.UIController.TurnOffRangeVisuals();
			}
		}

		// Token: 0x060025C1 RID: 9665 RVA: 0x000CCDE2 File Offset: 0x000CAFE2
		public IEnumerator FireRandomRCSFX()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f + TIUtilities.RandomFloatValue() / 10f);
			ParticleSystem FX = this.ModelController.ActivateRandomThruster();
			yield return waitForSeconds;
			FX.Stop();
			yield break;
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x000CCDF4 File Offset: 0x000CAFF4
		private void Update()
		{
			if (this.ModelController != null && this.shipState != null)
			{
				if (this.shipState.cooling && this.ModelController.RadiatorsExtended && (this.shipState.accumulatedHeat_GJ > 0f || (this.shipState.thrustersActive && !this.shipState.drive.openCycleCooling) || this.shipState.generatorWorking))
				{
					this.ModelController.EnableRadiatorEmissives();
				}
				else
				{
					this.ModelController.DisableRadiatorEmissives();
				}
				if (!this.UIVisualizationOnly && !this.shipState.thrustersActive && this.shipState.fleet != null && !this.shipState.fleet.dockedOrLanded && !World.Active.GetExistingManager<GameTimeManager>().Paused && !TIGlobalValuesState.isSpaceCombatEnabled && !this.shipState.inManeuver && !this.shipState.IsAlien() && TIUtilities.RandomFloatValue() < 0.01f)
				{
					base.StartCoroutine(this.FireRandomRCSFX());
				}
			}
		}

		// Token: 0x060025C3 RID: 9667 RVA: 0x000CCF18 File Offset: 0x000CB118
		private void OnDestroy()
		{
			if (!this.UIVisualizationOnly)
			{
				GameControl.eventManager.RemoveListener<ShipEntersCombat>(new EventManager.EventDelegate<ShipEntersCombat>(this.EnterCombat), null);
				GameControl.eventManager.RemoveListener<ShipLeavesCombat>(new EventManager.EventDelegate<ShipLeavesCombat>(this.PostCombat), null);
			}
		}

		// Token: 0x04001C32 RID: 7218
		[HideInInspector]
		public ShipModelController ModelController;

		// Token: 0x04001C33 RID: 7219
		public TISpaceShipState shipState;

		// Token: 0x04001C34 RID: 7220
		public FleetVisController fleetVisController;

		// Token: 0x04001C35 RID: 7221
		public StrategyShipController strategyShipController;

		// Token: 0x04001C36 RID: 7222
		private GameObject modelLink;
	}
}
