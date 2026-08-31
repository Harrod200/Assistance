using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000586 RID: 1414
	public class ShipConstructionVisController : MonoBehaviour
	{
		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06002533 RID: 9523 RVA: 0x000C82AB File Offset: 0x000C64AB
		// (set) Token: 0x06002534 RID: 9524 RVA: 0x000C82B3 File Offset: 0x000C64B3
		public ShipConstructionQueueItem shipItem { get; private set; }

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06002535 RID: 9525 RVA: 0x000C82BC File Offset: 0x000C64BC
		// (set) Token: 0x06002536 RID: 9526 RVA: 0x000C82C4 File Offset: 0x000C64C4
		public TISpaceShipTemplate shipTemplate { get; private set; }

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06002537 RID: 9527 RVA: 0x000C82CD File Offset: 0x000C64CD
		// (set) Token: 0x06002538 RID: 9528 RVA: 0x000C82D5 File Offset: 0x000C64D5
		public bool showingShipBuilding { get; private set; }

		// Token: 0x06002539 RID: 9529 RVA: 0x000C82DE File Offset: 0x000C64DE
		public void TrackExodusShipConstruction(Transform root, TIDateTime startDate, double daysToCompletion)
		{
			this.daysToCompletion = daysToCompletion;
			this.startDate = startDate;
			this.visRootObject = base.transform;
			this.showingShipBuilding = true;
			this.isExodusProjectModule = true;
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x000C8308 File Offset: 0x000C6508
		public void SetNewShipConstruction(ShipConstructionQueueItem item, float yOffset)
		{
			this.EndShipConstruction();
			this.daysToCompletion = (double)item.daysToCompletion;
			this.startDate = item.startDate;
			this.shipItem = item;
			this.visRootObject = base.transform;
			this.constructionProgress = 0.0;
			this.shipTemplate = item.shipDesign;
			this.yOffset = yOffset;
			this.isExodusProjectModule = false;
			this.showingShipBuilding = true;
			if (base.gameObject.activeInHierarchy)
			{
				this.modelPending = true;
				base.StartCoroutine(this.InitializeShipModelWithDelay());
			}
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x000C8398 File Offset: 0x000C6598
		private IEnumerator InitializeShipModelWithDelay()
		{
			yield return null;
			if (TIGameState.Valid(this.shipItem.shipyard))
			{
				this.InitializeShipModel();
			}
			else
			{
				this.showingShipBuilding = false;
			}
			this.modelPending = false;
			yield break;
		}

		// Token: 0x0600253C RID: 9532 RVA: 0x000C83A8 File Offset: 0x000C65A8
		private void InitializeShipModel()
		{
			if (this.shipVisController != null && this.shipTemplate != null)
			{
				this.shipVisController.InitializeModelOnly(this.shipTemplate);
				this.modelShip = base.transform.GetChild(0).gameObject;
				this.modelShip.transform.localPosition = new Vector3(0f, this.yOffset, 0f);
				this.modelShip.transform.localScale = Vector3.one;
				this.modelShip.transform.localRotation = Quaternion.identity;
				this.shipModelController = this.modelShip.GetComponent<ShipModelController>();
				if (this.shipModelController == null)
				{
					this.DestroyParticleEffects();
					global::UnityEngine.Object.Destroy(this.modelShip);
					this.showingShipBuilding = false;
					return;
				}
				this.shipModelController.BuildShip(this.shipVisController, this.shipItem.shipDesign, null, false);
				this.shipModelController.thrusterModel.SetActive(false);
				this.shipModelController.SetRadiatorsActive(this.shipItem.shipDesign, false);
				this.shipModelController.SetWeaponsActive(false);
				this.InstantiateParticleEffects(this.modelShip.transform, this.shipTemplate.isAlien);
				this.showingShipBuilding = true;
			}
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x000C84F8 File Offset: 0x000C66F8
		public void EndShipConstruction()
		{
			this.daysToCompletion = 0.0;
			this.startDate = null;
			this.DestroyParticleEffects();
			global::UnityEngine.Object.Destroy(this.modelShip);
			this.shipModelController = null;
			this.shipTemplate = null;
			this.yOffset = 0f;
			this.constructionProgress = 0.0;
			this.instantiatedshipPhase = -1;
			this.lastCheckedPercentage = -1.0;
			this.showingShipBuilding = false;
		}

		// Token: 0x0600253E RID: 9534 RVA: 0x000C8574 File Offset: 0x000C6774
		public double UpdateShipProgress()
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (this.isExodusProjectModule)
				{
					this.constructionProgress = (TITimeState.Now() - this.startDate).TotalDays / this.daysToCompletion;
					if (this.lastCheckedPercentage < this.constructionProgress)
					{
						if (this.constructionProgress < 0.33000001311302185 && this.instantiatedshipPhase < 0)
						{
							this.instantiatedshipPhase = 0;
							if (this.modelShip != null)
							{
								global::UnityEngine.Object.Destroy(this.modelShip);
							}
							this.modelShip = global::UnityEngine.Object.Instantiate<GameObject>(this.projectExodusShipPrefabs[this.instantiatedshipPhase], this.visRootObject);
							this.modelShip.transform.localScale = Vector3.one;
							this.modelShip.transform.localPosition = Vector3.zero;
							this.modelShip.transform.localRotation = Quaternion.identity;
							this.InstantiateParticleEffects(this.modelShip.transform, false);
						}
						else if (this.constructionProgress < 0.6600000262260437 && this.instantiatedshipPhase < 1)
						{
							this.instantiatedshipPhase = 1;
							this.DestroyParticleEffects();
							if (this.modelShip != null)
							{
								global::UnityEngine.Object.Destroy(this.modelShip);
							}
							this.modelShip = global::UnityEngine.Object.Instantiate<GameObject>(this.projectExodusShipPrefabs[this.instantiatedshipPhase], this.visRootObject);
							this.modelShip.transform.localScale = Vector3.one;
							this.modelShip.transform.localPosition = Vector3.zero;
							this.modelShip.transform.localRotation = Quaternion.identity;
							this.InstantiateParticleEffects(this.modelShip.transform, false);
						}
						else if (this.instantiatedshipPhase < 2)
						{
							this.instantiatedshipPhase = 2;
							this.DestroyParticleEffects();
							if (this.modelShip != null)
							{
								global::UnityEngine.Object.Destroy(this.modelShip);
							}
							this.modelShip = global::UnityEngine.Object.Instantiate<GameObject>(this.projectExodusShipPrefabs[this.instantiatedshipPhase], this.visRootObject);
							this.modelShip.transform.localScale = Vector3.one;
							this.modelShip.transform.localPosition = Vector3.zero;
							this.modelShip.transform.localRotation = Quaternion.identity;
							this.InstantiateParticleEffects(this.modelShip.transform, false);
						}
					}
				}
				else
				{
					if (this.modelShip == null && !this.modelPending)
					{
						this.InitializeShipModel();
					}
					if (this.shipModelController != null)
					{
						int num = (int)((double)this.shipModelController.MaxShipBuildSteps * this.constructionProgress);
						if (this.currentBuildStep != num)
						{
							for (int i = 0; i <= num; i++)
							{
								switch (num)
								{
								case 1:
									this.shipModelController.thrusterModel.SetActive(true);
									break;
								case 2:
									this.shipModelController.SetRadiatorsActive(this.shipItem.shipDesign, true);
									break;
								case 3:
									this.shipModelController.SetWeaponsActive(true);
									break;
								}
							}
							this.currentBuildStep = num;
						}
					}
				}
			}
			this.lastCheckedPercentage = this.constructionProgress;
			return this.constructionProgress;
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x000C88A4 File Offset: 0x000C6AA4
		private void InstantiateParticleEffects(Transform modelInstance, bool isAlien = false)
		{
			GameObject gameObject = (isAlien ? this.alienConstructionSparkParticlePrefab : this.humanConstructionSparkParticlePrefab);
			MeshRenderer[] componentsInChildren = modelInstance.GetComponentsInChildren<MeshRenderer>();
			this.constructionSparkParticleInstances = new List<GameObject>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, meshRenderer.transform.position, meshRenderer.transform.rotation, meshRenderer.transform);
				gameObject2.GetComponent<ParticleSystem>().shape.meshRenderer = meshRenderer;
				this.constructionSparkParticleInstances.Add(gameObject2);
			}
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x000C8930 File Offset: 0x000C6B30
		private void DestroyParticleEffects()
		{
			if (this.constructionSparkParticleInstances == null)
			{
				return;
			}
			for (int i = 0; i < this.constructionSparkParticleInstances.Count; i++)
			{
				global::UnityEngine.Object.Destroy(this.constructionSparkParticleInstances[i]);
			}
			this.constructionSparkParticleInstances.Clear();
		}

		// Token: 0x04001BD1 RID: 7121
		private int currentBuildStep;

		// Token: 0x04001BD2 RID: 7122
		private double daysToCompletion;

		// Token: 0x04001BD3 RID: 7123
		private double lastCheckedPercentage = -1.0;

		// Token: 0x04001BD4 RID: 7124
		private TIDateTime startDate;

		// Token: 0x04001BD5 RID: 7125
		private GameObject modelShip;

		// Token: 0x04001BD6 RID: 7126
		private ShipModelController shipModelController;

		// Token: 0x04001BD8 RID: 7128
		private List<GameObject> constructionSparkParticleInstances;

		// Token: 0x04001BD9 RID: 7129
		[SerializeField]
		private GameObject[] projectExodusShipPrefabs;

		// Token: 0x04001BDA RID: 7130
		private int instantiatedshipPhase = -1;

		// Token: 0x04001BDB RID: 7131
		private Transform visRootObject;

		// Token: 0x04001BDC RID: 7132
		[SerializeField]
		private ShipVisController shipVisController;

		// Token: 0x04001BDD RID: 7133
		[SerializeField]
		private GameObject humanConstructionSparkParticlePrefab;

		// Token: 0x04001BDE RID: 7134
		[SerializeField]
		private GameObject alienConstructionSparkParticlePrefab;

		// Token: 0x04001BE0 RID: 7136
		private float yOffset;

		// Token: 0x04001BE1 RID: 7137
		private bool isExodusProjectModule;

		// Token: 0x04001BE2 RID: 7138
		private double constructionProgress;

		// Token: 0x04001BE3 RID: 7139
		public bool modelPending;
	}
}
