using System;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006EE RID: 1774
	public class SolarSystemControl : MonoBehaviour
	{
		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06002944 RID: 10564 RVA: 0x000DBF58 File Offset: 0x000DA158
		private GameObjectDictionary<string> container
		{
			get
			{
				GameObjectDictionary<string> gameObjectDictionary;
				if ((gameObjectDictionary = this._container) == null)
				{
					gameObjectDictionary = (this._container = new GameObjectDictionary<string>("Solar System Container"));
				}
				return gameObjectDictionary;
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06002945 RID: 10565 RVA: 0x000DBF84 File Offset: 0x000DA184
		private GameObjectDictionary<string> orbitTrailContainer
		{
			get
			{
				GameObjectDictionary<string> gameObjectDictionary;
				if ((gameObjectDictionary = this._orbitTrailContainer) == null)
				{
					gameObjectDictionary = (this._orbitTrailContainer = new GameObjectDictionary<string>("Orbit Trail Container"));
				}
				return gameObjectDictionary;
			}
		}

		// Token: 0x06002946 RID: 10566 RVA: 0x000DBFAE File Offset: 0x000DA1AE
		public void Awake()
		{
			this.mainCamera = Camera.main;
			this.storedMask = this.mainCamera.cullingMask;
		}

		// Token: 0x06002947 RID: 10567 RVA: 0x000DBFCC File Offset: 0x000DA1CC
		public void AddObject(GameObject newObject, bool worldPositionStays = true)
		{
			if (!this.container.Add(newObject.name, newObject, worldPositionStays, false))
			{
				Log.Error("Failed to add object, name already used: " + newObject.name, Array.Empty<object>());
			}
			newObject.layer = LayerMask.NameToLayer("Solar System");
		}

		// Token: 0x06002948 RID: 10568 RVA: 0x000DC019 File Offset: 0x000DA219
		public void DestroySolarSystem()
		{
			GameObjectDictionary<string> container = this._container;
			if (container != null)
			{
				container.Clear(true);
			}
			GameObjectDictionary<string> orbitTrailContainer = this._orbitTrailContainer;
			if (orbitTrailContainer == null)
			{
				return;
			}
			orbitTrailContainer.Clear(true);
		}

		// Token: 0x06002949 RID: 10569 RVA: 0x000DC03E File Offset: 0x000DA23E
		public void AddOrbitTrailToContainer(GameObject orbitTrail)
		{
			this.orbitTrailContainer.Add(orbitTrail.name, orbitTrail, false, true);
			orbitTrail.layer = LayerMask.NameToLayer("Solar System");
		}

		// Token: 0x0600294A RID: 10570 RVA: 0x000DC065 File Offset: 0x000DA265
		public void TurnOffOrbitTrails()
		{
			this.orbitTrailContainer.gameObject.SetActive(false);
		}

		// Token: 0x0600294B RID: 10571 RVA: 0x000DC078 File Offset: 0x000DA278
		public void TurnOnOrbitTrails()
		{
			this.orbitTrailContainer.gameObject.SetActive(true);
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x000DC08B File Offset: 0x000DA28B
		public void ToggleOrbitTrails()
		{
			this.orbitTrailContainer.gameObject.SetActive(!this.orbitTrailContainer.gameObject.activeSelf);
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x000DC0B0 File Offset: 0x000DA2B0
		public void ToggleDistantSymbols()
		{
			this.showDistantSymbols = !this.showDistantSymbols;
		}

		// Token: 0x0600294E RID: 10574 RVA: 0x000DC0C1 File Offset: 0x000DA2C1
		public void ToggleProspectData()
		{
			this.showProspectData = !this.showProspectData;
			GameControl.eventManager.TriggerEvent(new ResetProspectSymbols(), null, Array.Empty<object>());
		}

		// Token: 0x0600294F RID: 10575 RVA: 0x000DC0E7 File Offset: 0x000DA2E7
		public void ToggleShowAllColonizedBodyNames()
		{
			this.showAllColonizedNames = !this.showAllColonizedNames;
			GameControl.eventManager.TriggerEvent(new ResetShowAllColonizedNames(), null, Array.Empty<object>());
		}

		// Token: 0x06002950 RID: 10576 RVA: 0x000DC110 File Offset: 0x000DA310
		public Transform FindObject(string name)
		{
			GameObject gameObject;
			if (this.container.TryFind(name, out gameObject))
			{
				return gameObject.transform;
			}
			foreach (object obj in this.container.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name == name)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x06002951 RID: 10577 RVA: 0x000DC198 File Offset: 0x000DA398
		private void OnEnable()
		{
			if (this.isEnabled)
			{
				return;
			}
			this.isEnabled = true;
			this.mainCamera = Camera.main;
			this.orbitTrailContainer.gameObject.SetActive(true);
			this.container.gameObject.layer = LayerMask.NameToLayer("Solar System");
			this.mainCamera.cullingMask = this.storedMask;
		}

		// Token: 0x06002952 RID: 10578 RVA: 0x000DC1FC File Offset: 0x000DA3FC
		private void OnDisable()
		{
			if (!this.isEnabled)
			{
				return;
			}
			this.isEnabled = false;
			if (World.Active != null && !this.shuttingDown)
			{
				World.Active.GetExistingManager<CanvasManager>().HideStrategyLayerUIs();
				if (this.orbitTrailContainer != null)
				{
					this.orbitTrailContainer.gameObject.SetActive(false);
				}
				SpaceObjectController[] componentsInChildren = this.container.transform.GetComponentsInChildren<SpaceObjectController>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].TurnOffAmbientAudio();
				}
			}
			this.mainCamera = Camera.main;
			if (this.mainCamera != null)
			{
				this.storedMask = this.mainCamera.cullingMask;
				this.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Solar System"));
			}
		}

		// Token: 0x06002953 RID: 10579 RVA: 0x000DC2C3 File Offset: 0x000DA4C3
		public void DisableSolarSystem()
		{
			this.OnDisable();
		}

		// Token: 0x06002954 RID: 10580 RVA: 0x000DC2CC File Offset: 0x000DA4CC
		public void DisableSolarSystemObjectsForSkirmishMode(IScenario scenario)
		{
			this.DisableSolarSystem();
			foreach (SpaceObjectController spaceObjectController in this.container.transform.GetComponentsInChildren<SpaceObjectController>())
			{
				SpaceObjectType objectType = spaceObjectController.spaceObjectState.objectType;
				if (objectType != SpaceObjectType.Star)
				{
					if (objectType != SpaceObjectType.Fleet)
					{
						if (objectType != SpaceObjectType.Hab)
						{
						}
						spaceObjectController.gameObject.SetActive(false);
					}
				}
				else
				{
					spaceObjectController.gameObject.transform.localPosition = new Vector3(5000f, 0f, 0f);
				}
			}
		}

		// Token: 0x06002955 RID: 10581 RVA: 0x000DC34F File Offset: 0x000DA54F
		private void OnDestroy()
		{
			this.shuttingDown = true;
		}

		// Token: 0x04001FB7 RID: 8119
		[SerializeField]
		private GameObjectDictionary<string> _container;

		// Token: 0x04001FB8 RID: 8120
		[SerializeField]
		private GameObjectDictionary<string> _orbitTrailContainer;

		// Token: 0x04001FB9 RID: 8121
		private int storedMask;

		// Token: 0x04001FBA RID: 8122
		private bool shuttingDown;

		// Token: 0x04001FBB RID: 8123
		private bool isEnabled;

		// Token: 0x04001FBC RID: 8124
		public bool showDistantSymbols = true;

		// Token: 0x04001FBD RID: 8125
		public bool showProspectData;

		// Token: 0x04001FBE RID: 8126
		public bool showAllColonizedNames = true;

		// Token: 0x04001FBF RID: 8127
		public Camera mainCamera;
	}
}
