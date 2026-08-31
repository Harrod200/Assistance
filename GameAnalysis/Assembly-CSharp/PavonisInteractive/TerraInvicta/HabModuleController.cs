using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000599 RID: 1433
	public class HabModuleController : MonoBehaviour
	{
		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x0600264A RID: 9802 RVA: 0x000CF989 File Offset: 0x000CDB89
		// (set) Token: 0x0600264B RID: 9803 RVA: 0x000CF991 File Offset: 0x000CDB91
		public TIHabState hab { get; private set; }

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x0600264C RID: 9804 RVA: 0x000CF99A File Offset: 0x000CDB9A
		// (set) Token: 0x0600264D RID: 9805 RVA: 0x000CF9A2 File Offset: 0x000CDBA2
		public List<MeshRenderer> renderers { get; private set; }

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x0600264E RID: 9806 RVA: 0x000CF9AB File Offset: 0x000CDBAB
		// (set) Token: 0x0600264F RID: 9807 RVA: 0x000CF9B3 File Offset: 0x000CDBB3
		public HabModelController habModelController { get; private set; }

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06002650 RID: 9808 RVA: 0x000CF9BC File Offset: 0x000CDBBC
		// (set) Token: 0x06002651 RID: 9809 RVA: 0x000CF9C4 File Offset: 0x000CDBC4
		public HabModuleUIElementController UIController { get; private set; }

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06002652 RID: 9810 RVA: 0x000CF9CD File Offset: 0x000CDBCD
		// (set) Token: 0x06002653 RID: 9811 RVA: 0x000CF9D5 File Offset: 0x000CDBD5
		public bool fullVisualization { get; private set; }

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06002654 RID: 9812 RVA: 0x000CF9DE File Offset: 0x000CDBDE
		private string ExodusConstructionID
		{
			get
			{
				return new StringBuilder("ExodusConstructionUpdate").Append(this.habModule.ID).ToString();
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06002655 RID: 9813 RVA: 0x000CFA04 File Offset: 0x000CDC04
		public TIHabModuleState habModule
		{
			get
			{
				if (this._habModuleState == null)
				{
					TIHabState hab = this.hab;
					this._habModuleState = ((hab != null) ? hab.GetModule(this.sector, this.moduleNum) : null) ?? null;
				}
				return this._habModuleState;
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06002656 RID: 9814 RVA: 0x000CFA44 File Offset: 0x000CDC44
		public CombatHabModuleController CombatHabModuleController
		{
			get
			{
				if (!this.habModule.isCombatModule)
				{
					return null;
				}
				CombatHabModuleController combatHabModuleController = base.GetComponent<CombatHabModuleController>();
				if (combatHabModuleController == null)
				{
					combatHabModuleController = base.gameObject.AddComponent<CombatHabModuleController>();
				}
				return combatHabModuleController;
			}
		}

		// Token: 0x06002657 RID: 9815 RVA: 0x000CFA80 File Offset: 0x000CDC80
		public void Initialize(bool includingUIControllers)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.renderers = base.GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();
			this.renderers.ForEach(delegate(MeshRenderer x)
			{
				this.SetNormalColor(x);
			});
			if (this.UIController == null && includingUIControllers)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>("ui/StationModuleUI"), base.transform);
				this.UIController = gameObject.GetComponent<HabModuleUIElementController>();
				this.UIController.SetController(this);
			}
		}

		// Token: 0x06002658 RID: 9816 RVA: 0x000CFB08 File Offset: 0x000CDD08
		public void SetHighlightColor(MeshRenderer rend)
		{
			if (this.fullVisualization)
			{
				rend.material.color = TIUtilities.UIHighlightColor;
			}
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x000CFB22 File Offset: 0x000CDD22
		public void SetNormalColor(MeshRenderer rend)
		{
			rend.material.color = Color.white;
		}

		// Token: 0x0600265A RID: 9818 RVA: 0x000CFB34 File Offset: 0x000CDD34
		public void SetModuleValue(TIHabState hab, HabModelController habModelController, bool fullVisualization)
		{
			this.hab = hab;
			this.habModelController = habModelController;
			this.fullVisualization = fullVisualization;
			if (fullVisualization)
			{
				GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabModuleUpdated), null, this.habModule, false, false);
			}
			this.UIController.Initialize(this.habModule);
			this.highlighted = false;
			this.UpdateModuleData();
		}

		// Token: 0x0600265B RID: 9819 RVA: 0x000CFB96 File Offset: 0x000CDD96
		private void OnHabModuleUpdated(HabModuleConstructionStatusChange e)
		{
			this.UpdateModuleData();
		}

		// Token: 0x0600265C RID: 9820 RVA: 0x000CFBA0 File Offset: 0x000CDDA0
		public void CreateShipConstructionVisControllerObject()
		{
			if (this.shipVisObject == null)
			{
				this.shipVisObject = global::UnityEngine.Object.Instantiate<GameObject>(this.shipPrefab, this.shipContructionRootObject[this._habModuleState.tier - 1].transform);
				this.shipVisObject.transform.localPosition = Vector3.zero;
				this.shipVisObject.transform.localScale = Vector3.one;
				this.shipVisObject.SetActive(true);
			}
			if (this.shipConstructionVisController == null)
			{
				this.shipConstructionVisController = this.shipVisObject.GetComponent<ShipConstructionVisController>();
			}
		}

		// Token: 0x0600265D RID: 9821 RVA: 0x000CFC3C File Offset: 0x000CDE3C
		public void UpdateModuleData()
		{
			if (base.isActiveAndEnabled)
			{
				if (TIGameState.Valid(this.habModule))
				{
					if (this.habModule.moduleTemplate != this.initializedModuleTemplate)
					{
						this.initializedModuleTemplate = this.habModule.moduleTemplate;
						if (this.habModule.hasModule)
						{
							if (!this.projectExodusStarted && this.habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.InterstellarLaunchModule))
							{
								this.shipyard = false;
								this.projectExodusStarted = true;
								this.exodusConstructionTimeEvent = TITimeState.Now();
								double totalDays = (this.habModule.completionDate - this.exodusConstructionTimeEvent.ExportTime()).TotalDays;
								TIDateTime tidateTime = this.exodusConstructionTimeEvent;
								string exodusConstructionID = this.ExodusConstructionID;
								TITimeEvent.CreateNewTimeEvent(tidateTime, this.habModule, null, null, exodusConstructionID, false, false, TITimeQueueRepeatType.Day, 1, true, false);
								GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateShipConstructionAssets), this.ExodusConstructionID, null, true, false);
								this.CreateShipConstructionVisControllerObject();
								this.shipConstructionVisController.TrackExodusShipConstruction(this.ExodusShipRootObject, this.exodusConstructionTimeEvent, totalDays);
							}
							else
							{
								if (this.projectExodusStarted && !this.habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.InterstellarLaunchModule))
								{
									this.exodusConstructionTimeEvent = null;
									this.ExodusShipRootObject = null;
									GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateShipConstructionAssets), this.ExodusConstructionID);
								}
								bool flag = this.shipyard;
								this.shipyard = this.habModule.moduleTemplate.allowsShipConstruction;
								if (!flag && this.shipyard)
								{
									this.CreateShipConstructionVisControllerObject();
									this.AddListeners();
								}
								else if (flag && !this.shipyard)
								{
									if (this.shipConstructionVisController.showingShipBuilding)
									{
										this.shipConstructionVisController.EndShipConstruction();
									}
									this.RemoveListeners();
								}
							}
						}
						else if (this.projectExodusStarted)
						{
							this.projectExodusStarted = false;
							this.exodusConstructionTimeEvent = null;
							this.ExodusShipRootObject = null;
							GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateShipConstructionAssets), this.ExodusConstructionID);
							if (this.shipConstructionVisController.showingShipBuilding)
							{
								this.shipConstructionVisController.EndShipConstruction();
							}
						}
						else if (this.shipyard)
						{
							this.shipyard = false;
							this.shipConstructionVisController.EndShipConstruction();
						}
					}
					if (this.projectExodusStarted)
					{
						if (this.shipConstructionVisController.UpdateShipProgress() >= 1.0)
						{
							this.projectExodusStarted = false;
							this.exodusConstructionTimeEvent = null;
							this.ExodusShipRootObject = null;
							GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateShipConstructionAssets), this.ExodusConstructionID);
							if (this.shipConstructionVisController.showingShipBuilding)
							{
								this.shipConstructionVisController.EndShipConstruction();
							}
							this.gameTime.CancelTimeEvent(this.ExodusConstructionID, this.habModule, null, null, this.exodusConstructionTimeEvent);
							return;
						}
					}
					else if (this.shipyard)
					{
						if (this.habModule.buildingShip)
						{
							ShipConstructionQueueItem currentShipConstructionQueueItem = this.habModule.currentShipConstructionQueueItem;
							if (this.shipConstructionVisController.shipItem != currentShipConstructionQueueItem || this.shipConstructionVisController.shipTemplate == null)
							{
								if (currentShipConstructionQueueItem != null)
								{
									this.shipConstructionVisController.SetNewShipConstruction(currentShipConstructionQueueItem, currentShipConstructionQueueItem.shipDesign.hullTemplate.shipyardyOffset[this.habModule.moduleTemplate.tier - 1]);
								}
								else
								{
									this.shipConstructionVisController.EndShipConstruction();
								}
							}
							this.shipConstructionVisController.UpdateShipProgress();
							return;
						}
						if (this.shipConstructionVisController.showingShipBuilding)
						{
							this.shipConstructionVisController.EndShipConstruction();
							return;
						}
					}
				}
				else
				{
					this.projectExodusStarted = false;
					this.exodusConstructionTimeEvent = null;
					this.ExodusShipRootObject = null;
					this.shipyard = false;
				}
			}
		}

		// Token: 0x0600265E RID: 9822 RVA: 0x000CFFD8 File Offset: 0x000CE1D8
		public void DuplicateMaterialsForUIDisplay()
		{
			foreach (MeshRenderer meshRenderer in this.renderers)
			{
				int num = 0;
				Material[] materials = meshRenderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					Material material = new Material(materials[i]);
					meshRenderer.materials[num++] = material;
				}
				this.SetNormalColor(meshRenderer);
			}
		}

		// Token: 0x0600265F RID: 9823 RVA: 0x000D0060 File Offset: 0x000CE260
		private void AddListeners()
		{
			if (this.shipyard || this.projectExodusStarted)
			{
				GameControl.eventManager.AddListener<ShipConstructionUpdated>(new EventManager.EventDelegate<ShipConstructionUpdated>(this.UpdateShipConstructionAssets), null, this.habModule, false, false);
				GameControl.eventManager.AddListener<ShipConstructionCompleted>(new EventManager.EventDelegate<ShipConstructionCompleted>(this.ShipConstructionCompleted), null, this.habModule, false, false);
			}
		}

		// Token: 0x06002660 RID: 9824 RVA: 0x000D00BB File Offset: 0x000CE2BB
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<ShipConstructionUpdated>(new EventManager.EventDelegate<ShipConstructionUpdated>(this.UpdateShipConstructionAssets), null);
			GameControl.eventManager.RemoveListener<ShipConstructionCompleted>(new EventManager.EventDelegate<ShipConstructionCompleted>(this.ShipConstructionCompleted), null);
		}

		// Token: 0x06002661 RID: 9825 RVA: 0x000D00EB File Offset: 0x000CE2EB
		private void ShipConstructionCompleted(ShipConstructionCompleted e)
		{
			this.UpdateModuleData();
		}

		// Token: 0x06002662 RID: 9826 RVA: 0x000D00F3 File Offset: 0x000CE2F3
		private void UpdateShipConstructionAssets(TimeEventStart e)
		{
			this.UpdateModuleData();
		}

		// Token: 0x06002663 RID: 9827 RVA: 0x000D00FB File Offset: 0x000CE2FB
		private void UpdateShipConstructionAssets(ShipConstructionUpdated e)
		{
			this.UpdateModuleData();
		}

		// Token: 0x06002664 RID: 9828 RVA: 0x000D0104 File Offset: 0x000CE304
		public void DestroyHabModule(TIFactionState destroyer)
		{
			GameObject gameObject;
			AssetCacheManager.destructionSequencePrefabs.TryGetValue(this._habModuleState.moduleTemplate.stationDestructionResource, out gameObject);
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			base.StartCoroutine(this.DestroyModuleDelayed(destroyer, 1.9f));
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x000D016C File Offset: 0x000CE36C
		private IEnumerator DestroyModuleDelayed(TIFactionState destroyer, float delay)
		{
			yield return delay;
			this._habModuleState.hab.DestroyModule(destroyer, this._habModuleState, false, false, true, 0f, true, false);
			yield break;
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000D0189 File Offset: 0x000CE389
		private void OnEnable()
		{
			this.UpdateModuleData();
			this.AddListeners();
			if (this.fullVisualization && this.UIController != null)
			{
				this.UIController.gameObject.SetActive(true);
			}
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x000D01C0 File Offset: 0x000CE3C0
		private void OnDisable()
		{
			List<MeshRenderer> renderers = this.renderers;
			if (renderers != null)
			{
				renderers.ForEach(delegate(MeshRenderer x)
				{
					this.SetNormalColor(x);
				});
			}
			if (this.UIController != null)
			{
				this.UIController.gameObject.SetActive(false);
			}
			this.highlighted = false;
			this.RemoveListeners();
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x000D0218 File Offset: 0x000CE418
		public void OnDestroy()
		{
			this.RemoveListeners();
			if (this.projectExodusStarted && this.habModule != null)
			{
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateShipConstructionAssets), this.ExodusConstructionID);
			}
			this._habModuleState = null;
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabModuleUpdated), null);
			if (!this.fullVisualization)
			{
				foreach (MeshRenderer meshRenderer in this.renderers)
				{
					Material[] materials = meshRenderer.materials;
					for (int i = 0; i < materials.Length; i++)
					{
						global::UnityEngine.Object.Destroy(materials[i]);
					}
				}
			}
		}

		// Token: 0x04001C7C RID: 7292
		public int sector;

		// Token: 0x04001C7D RID: 7293
		public int moduleNum;

		// Token: 0x04001C7E RID: 7294
		public bool highlighted;

		// Token: 0x04001C84 RID: 7300
		private TIHabModuleState _habModuleState;

		// Token: 0x04001C85 RID: 7301
		private TIHabModuleTemplate initializedModuleTemplate;

		// Token: 0x04001C86 RID: 7302
		public Transform ExodusShipRootObject;

		// Token: 0x04001C87 RID: 7303
		public Transform[] shipContructionRootObject;

		// Token: 0x04001C88 RID: 7304
		public GameObject shipPrefab;

		// Token: 0x04001C89 RID: 7305
		private bool shipyard;

		// Token: 0x04001C8A RID: 7306
		private bool projectExodusStarted;

		// Token: 0x04001C8B RID: 7307
		private GameObject shipVisObject;

		// Token: 0x04001C8C RID: 7308
		private ShipConstructionVisController shipConstructionVisController;

		// Token: 0x04001C8D RID: 7309
		private GameTimeManager gameTime;

		// Token: 0x04001C8E RID: 7310
		private TIDateTime exodusConstructionTimeEvent;
	}
}
