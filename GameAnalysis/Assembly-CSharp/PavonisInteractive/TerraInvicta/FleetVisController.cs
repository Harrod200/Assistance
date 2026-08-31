using System;
using System.Collections.Generic;
using System.Linq;
using ModelShark;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000580 RID: 1408
	public class FleetVisController : SolarSysModelController
	{
		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x0600251D RID: 9501 RVA: 0x000C7D61 File Offset: 0x000C5F61
		// (set) Token: 0x0600251E RID: 9502 RVA: 0x000C7D69 File Offset: 0x000C5F69
		public TISpaceFleetState fleetState { get; private set; }

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x0600251F RID: 9503 RVA: 0x000C7D72 File Offset: 0x000C5F72
		// (set) Token: 0x06002520 RID: 9504 RVA: 0x000C7D7A File Offset: 0x000C5F7A
		public List<GameObject> shipStratControllerObjects { get; private set; }

		// Token: 0x06002521 RID: 9505 RVA: 0x000C7D84 File Offset: 0x000C5F84
		public override void InitializeModel(SpaceObjectController container)
		{
			this.shipStratControllerObjects = new List<GameObject>();
			base.InitializeModel(container);
			base.name = container.name + " Container";
			this.fleetState = container.spaceObjectState as TISpaceFleetState;
			foreach (TISpaceShipState tispaceShipState in this.fleetState.ships)
			{
				GameObject gameObject = new GameObject(tispaceShipState.ID.ToString() + " Strategy Controller");
				gameObject.transform.SetParent(base.transform);
				gameObject.AddComponent<StrategyShipController>().Initialize(this.shipPrefab, tispaceShipState, this, false);
				this.shipStratControllerObjects.Add(gameObject);
			}
			GameControl.eventManager.AddListener<FleetDisbanded>(new EventManager.EventDelegate<FleetDisbanded>(this.OnFleetDisbanded), null, this.fleetState, false, false);
			base.SetShadowBehavior();
			this.init = true;
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x000C7E90 File Offset: 0x000C6090
		public void InitializeForUIAppearanceOnly(TISpaceFleetState fleet)
		{
			this.fleetState = fleet;
			ShipVisController[] componentsInChildren = base.transform.GetComponentsInChildren<ShipVisController>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ShipVisController shipVisController = componentsInChildren[i];
				shipVisController.SetAsUIVisualization(fleet.ships.SingleOrDefault<TISpaceShipState>((TISpaceShipState x) => x.ID.ToString() == shipVisController.name), true);
			}
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x000C7EF4 File Offset: 0x000C60F4
		private void OnFleetDisbanded(FleetDisbanded e)
		{
			GameControl.eventManager.RemoveListener<FleetDisbanded>(new EventManager.EventDelegate<FleetDisbanded>(this.OnFleetDisbanded), null);
			if (this != null)
			{
				SpaceObjectController componentInParent = base.GetComponentInParent<SpaceObjectController>();
				componentInParent.symbolController.enabled = false;
				componentInParent.symbolLink.SetActive(false);
				SolarSysModelController modelController = componentInParent.modelController;
				if (modelController != null)
				{
					GameObject gameObject = modelController.gameObject;
					if (gameObject != null)
					{
						gameObject.SetActive(false);
					}
				}
				componentInParent.enabled = false;
				base.enabled = false;
				if (TooltipManager.Instance.TooltipContainer.transform.IsChildOf(componentInParent.transform))
				{
					TooltipManager.Instance.MoveContainerToDummyCanvas(false);
				}
				componentInParent.Invoke("DestroyThis", 15f);
			}
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x000C7FA8 File Offset: 0x000C61A8
		private void OnDisable()
		{
			if (!this.init)
			{
				return;
			}
			foreach (GameObject gameObject in this.shipStratControllerObjects)
			{
				gameObject.GetComponent<StrategyShipController>().ModelController.StopThrusterAudio();
			}
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x000C800C File Offset: 0x000C620C
		private void OnEnable()
		{
			if (!this.init)
			{
				return;
			}
			foreach (GameObject gameObject in this.shipStratControllerObjects)
			{
				gameObject.GetComponent<StrategyShipController>().SetDirty();
			}
		}

		// Token: 0x04001BCB RID: 7115
		public GameObject shipPrefab;

		// Token: 0x04001BCE RID: 7118
		private bool init;
	}
}
