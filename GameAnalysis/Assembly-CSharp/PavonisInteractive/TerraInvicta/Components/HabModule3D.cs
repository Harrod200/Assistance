using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Components
{
	// Token: 0x020009C9 RID: 2505
	public class HabModule3D
	{
		// Token: 0x06005E36 RID: 24118 RVA: 0x002CCA1C File Offset: 0x002CAC1C
		public void Empty(TIHabModuleState module)
		{
			if (module != null)
			{
				this.sector = module.sectorNum;
				this.moduleSlot = module.slot;
				this.UpdateConnections(module, false);
			}
			else
			{
				this.N1.enabled = false;
				this.N2.enabled = false;
				this.W1.enabled = false;
				this.W2.enabled = false;
				this.S1.enabled = false;
				this.S2.enabled = false;
				this.E1.enabled = false;
				this.E2.enabled = false;
			}
			if (this.meshFilter != null)
			{
				this.meshFilter.sharedMesh = null;
			}
			if (this.renderer != null)
			{
				this.renderer.enabled = false;
			}
			this.explosionSequencePrefab = null;
		}

		// Token: 0x06005E37 RID: 24119 RVA: 0x002CCAF0 File Offset: 0x002CACF0
		public void SetMesh(TIHabModuleState module, bool alienStation)
		{
			this.renderer.enabled = false;
			this.sector = module.sectorNum;
			this.moduleSlot = module.slot;
			if (!module.empty)
			{
				GameObject gameObject;
				if (module.underConstruction)
				{
					AssetCacheManager.constructionModulePrefabs.TryGetValue(module.moduleTemplate.constructionModelResource, out gameObject);
					AssetCacheManager.destructionSequencePrefabs.TryGetValue(module.moduleTemplate.constructionModelDestructionResource, out this.explosionSequencePrefab);
				}
				else
				{
					AssetCacheManager.stationModulePrefabs.TryGetValue(module.moduleTemplate.stationModelResource, out gameObject);
					AssetCacheManager.destructionSequencePrefabs.TryGetValue(module.moduleTemplate.stationDestructionResource, out this.explosionSequencePrefab);
				}
				if (gameObject == null)
				{
					Log.Error("PREFAB habmodules/" + module.moduleTemplate.dataName + " COULD NOT BE LOADED", Array.Empty<object>());
					return;
				}
				if (this.explosionSequencePrefab == null)
				{
					Log.Error(module.moduleTemplate.stationDestructionResource + " not found for " + module.moduleTemplate.dataName, Array.Empty<object>());
				}
				this.meshFilter.sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
				Material[] sharedMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
				this.renderer.sharedMaterials = new Material[sharedMaterials.Length];
				this.renderer.sharedMaterials = sharedMaterials;
				if (gameObject.GetComponent<Animator>() != null)
				{
					if (this.animator == null)
					{
						this.animator = this.meshFilter.gameObject.GetComponent<Animator>();
						if (this.animator == null)
						{
							this.animator = this.meshFilter.gameObject.AddComponent<Animator>();
						}
					}
					this.animator.runtimeAnimatorController = gameObject.GetComponent<Animator>().runtimeAnimatorController;
					this.animator.enabled = true;
				}
				else if (this.animator != null)
				{
					this.animator.enabled = false;
				}
				foreach (Collider collider in this.renderer.gameObject.GetComponentsInChildren<Collider>())
				{
					if (!(collider.gameObject.name == "CollisionObject") && !collider.gameObject.name.Contains("StationModuleUI"))
					{
						global::UnityEngine.Object.Destroy(collider);
					}
				}
				Collider[] array = gameObject.GetComponentsInChildren<Collider>(true);
				for (int i = 0; i < array.Length; i++)
				{
					TIUtilities.CopyComponent(array[i], this.renderer.gameObject);
				}
				this.renderer.enabled = true;
				if (module.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Solar_Power_Variable_Output))
				{
					Transform transform = this.renderer.gameObject.transform;
					if (transform.rotation != Quaternion.identity)
					{
						Transform[] array2 = new Transform[transform.childCount];
						int num = 0;
						foreach (object obj in transform)
						{
							Transform transform2 = (Transform)obj;
							array2[num++] = transform2;
						}
						transform.DetachChildren();
						transform.rotation = Quaternion.identity;
						Transform[] array3 = array2;
						for (int i = 0; i < array3.Length; i++)
						{
							array3[i].SetParent(transform);
						}
						array2 = null;
					}
				}
			}
			else
			{
				this.renderer.enabled = false;
			}
			if (alienStation)
			{
				this.SetAlienConnections();
			}
			this.UpdateConnections(module, false);
		}

		// Token: 0x06005E38 RID: 24120 RVA: 0x002CCE64 File Offset: 0x002CB064
		public void HideModule()
		{
			this.renderer.enabled = false;
		}

		// Token: 0x06005E39 RID: 24121 RVA: 0x002CCE74 File Offset: 0x002CB074
		public void SetAlienConnections()
		{
			GameObject basicAlienConnectorPrefab = AssetCacheManager.basicAlienConnectorPrefab;
			if (this.N1 != null)
			{
				this.N1.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.N1.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.N1.GetComponent<Animator>().enabled = false;
			}
			if (this.N2 != null)
			{
				this.N2.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.N2.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.N2.GetComponent<Animator>().enabled = false;
			}
			if (this.W1 != null)
			{
				this.W1.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.W1.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.W1.GetComponent<Animator>().enabled = false;
			}
			if (this.W2 != null)
			{
				this.W2.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.W2.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.W2.GetComponent<Animator>().enabled = false;
			}
			if (this.S1 != null)
			{
				this.S1.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.S1.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.S1.GetComponent<Animator>().enabled = false;
			}
			if (this.S2 != null)
			{
				this.S2.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.S2.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.S2.GetComponent<Animator>().enabled = false;
			}
			if (this.E1 != null)
			{
				this.E1.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.E1.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.E1.GetComponent<Animator>().enabled = false;
			}
			if (this.E2 != null)
			{
				this.E2.GetComponent<MeshFilter>().sharedMesh = basicAlienConnectorPrefab.GetComponent<MeshFilter>().sharedMesh;
				this.E2.sharedMaterials = basicAlienConnectorPrefab.GetComponent<MeshRenderer>().sharedMaterials;
				this.E2.GetComponent<Animator>().enabled = false;
			}
			if (this.moduleConnector != null)
			{
				GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>("habmodules/Alien_T_Connector_Module");
				this.moduleConnector.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
				this.moduleConnector.sharedMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
				this.moduleConnector.GetComponent<Animator>().enabled = false;
			}
		}

		// Token: 0x06005E3A RID: 24122 RVA: 0x002CD168 File Offset: 0x002CB368
		public void UpdateConnections(TIHabModuleState habModule, bool hide)
		{
			if (this.N1 != null)
			{
				this.N1.enabled = !hide && habModule.N1;
			}
			if (this.N2 != null)
			{
				this.N2.enabled = !hide && habModule.N2;
			}
			if (this.W1 != null)
			{
				this.W1.enabled = !hide && habModule.W1;
			}
			if (this.W2 != null)
			{
				this.W2.enabled = !hide && habModule.W2;
			}
			if (this.S1 != null)
			{
				this.S1.enabled = !hide && habModule.S1;
			}
			if (this.S2 != null)
			{
				this.S2.enabled = !hide && habModule.S2;
			}
			if (this.E1 != null)
			{
				this.E1.enabled = !hide && habModule.E1;
			}
			if (this.E2 != null)
			{
				this.E2.enabled = !hide && habModule.E2;
			}
			if (this.moduleConnector != null)
			{
				this.moduleConnector.enabled = !hide && habModule.C0;
			}
		}

		// Token: 0x04004356 RID: 17238
		public MeshFilter meshFilter;

		// Token: 0x04004357 RID: 17239
		public MeshRenderer renderer;

		// Token: 0x04004358 RID: 17240
		public MeshRenderer N1;

		// Token: 0x04004359 RID: 17241
		public MeshRenderer N2;

		// Token: 0x0400435A RID: 17242
		public MeshRenderer W1;

		// Token: 0x0400435B RID: 17243
		public MeshRenderer W2;

		// Token: 0x0400435C RID: 17244
		public MeshRenderer S1;

		// Token: 0x0400435D RID: 17245
		public MeshRenderer S2;

		// Token: 0x0400435E RID: 17246
		public MeshRenderer E1;

		// Token: 0x0400435F RID: 17247
		public MeshRenderer E2;

		// Token: 0x04004360 RID: 17248
		public MeshRenderer moduleConnector;

		// Token: 0x04004361 RID: 17249
		public Animator animator;

		// Token: 0x04004362 RID: 17250
		public GameObject explosionSequencePrefab;

		// Token: 0x04004363 RID: 17251
		public GameObject explosionSequenceInstance;

		// Token: 0x04004364 RID: 17252
		public int sector;

		// Token: 0x04004365 RID: 17253
		public int moduleSlot;
	}
}
