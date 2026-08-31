using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta.Systems;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Components
{
	// Token: 0x020009C8 RID: 2504
	public class HabComponent : MonoBehaviour
	{
		// Token: 0x17001033 RID: 4147
		// (get) Token: 0x06005E29 RID: 24105 RVA: 0x002CC277 File Offset: 0x002CA477
		// (set) Token: 0x06005E2A RID: 24106 RVA: 0x002CC27F File Offset: 0x002CA47F
		public TIHabState hab { get; private set; }

		// Token: 0x06005E2B RID: 24107 RVA: 0x002CC288 File Offset: 0x002CA488
		public void InitStation()
		{
			this.CacheHabModules();
		}

		// Token: 0x06005E2C RID: 24108 RVA: 0x002CC290 File Offset: 0x002CA490
		public void Initialize(TIHabState hab)
		{
			this.hab = hab;
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.UpdateModel), null, hab, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.ModuleDestroyed), null, hab, true, false);
		}

		// Token: 0x06005E2D RID: 24109 RVA: 0x002CC2CD File Offset: 0x002CA4CD
		private void UpdateModel(HabModuleConstructionStatusChange e)
		{
			this.UpdateDestructionVFX(e.habModule);
			this.Update3DModel();
		}

		// Token: 0x06005E2E RID: 24110 RVA: 0x002CC2E4 File Offset: 0x002CA4E4
		private void ModuleDestroyed(HabModuleDestroyed e)
		{
			TISectorState sector = e.habModule.sector;
			int sectorNum = e.habModule.sectorNum;
			int slot = e.habModule.slot;
			HabModule3D habModule3D;
			if (!this.hab.IsStation || !this.modules.TryGetValue(new StringBuilder("S").Append(sectorNum).Append("_M").Append(slot)
				.ToString(), out habModule3D))
			{
				this.UpdateModule(null, e.habModule);
				return;
			}
			if (habModule3D.explosionSequencePrefab != null && base.gameObject.activeInHierarchy && !e.combat)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(habModule3D.explosionSequencePrefab, habModule3D.renderer.transform);
				habModule3D.explosionSequenceInstance = gameObject;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				base.StartCoroutine(this.UpdateModuleDelayed(habModule3D, e.habModule));
				return;
			}
			this.UpdateModule(habModule3D, e.habModule);
		}

		// Token: 0x06005E2F RID: 24111 RVA: 0x002CC3ED File Offset: 0x002CA5ED
		private IEnumerator UpdateModuleDelayed(HabModule3D module, TIHabModuleState state)
		{
			yield return this.delay;
			this.UpdateModule(module, state);
			yield break;
		}

		// Token: 0x06005E30 RID: 24112 RVA: 0x002CC40C File Offset: 0x002CA60C
		private void UpdateModule(HabModule3D module, TIHabModuleState state)
		{
			if (this.hab.IsStation && !this.hab.archived)
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.hab.sectors[i].faction == null)
					{
						this.EmptySector(i);
					}
				}
				module.SetMesh(state, this.hab.IsAlien());
				this.torusRenderers[0].enabled = this.hab.ringStruct.NE;
				this.torusRenderers[1].enabled = this.hab.ringStruct.SE;
				this.torusRenderers[2].enabled = this.hab.ringStruct.SW;
				this.torusRenderers[3].enabled = this.hab.ringStruct.NW;
			}
		}

		// Token: 0x06005E31 RID: 24113 RVA: 0x002CC4F0 File Offset: 0x002CA6F0
		private void CacheHabModules()
		{
			this.modules = new Dictionary<string, HabModule3D>();
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>(true);
			this.torusRenderers = new MeshRenderer[4];
			this.torusRenderers[0] = base.gameObject.GetComponentOnChild<MeshRenderer>("Torus_1_2");
			this.torusRenderers[1] = base.gameObject.GetComponentOnChild<MeshRenderer>("Torus_2_3");
			this.torusRenderers[2] = base.gameObject.GetComponentOnChild<MeshRenderer>("Torus_3_4");
			this.torusRenderers[3] = base.gameObject.GetComponentOnChild<MeshRenderer>("Torus_4_1");
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				string name = componentsInChildren[i].gameObject.name;
				if (name.Contains("Module"))
				{
					HabModule3D habModule3D = new HabModule3D
					{
						sector = int.Parse(componentsInChildren[i].transform.parent.name.Replace("Sector", "")),
						moduleSlot = int.Parse(name.Replace("Module", "")),
						meshFilter = componentsInChildren[i],
						renderer = componentsInChildren[i].GetComponent<MeshRenderer>()
					};
					habModule3D.N1 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("N1");
					habModule3D.N2 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("N2");
					habModule3D.W1 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("W1");
					habModule3D.W2 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("W2");
					habModule3D.E1 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("E1");
					habModule3D.E2 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("E2");
					habModule3D.S1 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("S1");
					habModule3D.S2 = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("S2");
					habModule3D.moduleConnector = componentsInChildren[i].gameObject.GetComponentOnChild<MeshRenderer>("C_M");
					this.modules.Add(new StringBuilder("S").Append(habModule3D.sector).Append("_M").Append(habModule3D.moduleSlot)
						.ToString(), habModule3D);
				}
			}
		}

		// Token: 0x06005E32 RID: 24114 RVA: 0x002CC728 File Offset: 0x002CA928
		public void Update3DModel()
		{
			if (!this.hab.deleted && this.hab.IsStation)
			{
				for (int i = 0; i < 5; i++)
				{
					TISectorState tisectorState = this.hab.sectors[i];
					if (tisectorState.faction == null)
					{
						this.EmptySector(i);
					}
					else
					{
						for (int j = 0; j < 5; j++)
						{
							HabModule3D habModule3D;
							if (this.modules.TryGetValue(new StringBuilder("S").Append(i).Append("_M").Append(j)
								.ToString(), out habModule3D))
							{
								habModule3D.SetMesh(tisectorState.habModules[j], this.hab.IsAlien());
							}
						}
					}
				}
				if (!this.initialized)
				{
					if (this.hab.IsAlien())
					{
						GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>("habmodules/station_T3_Hydra_Ring_Torus_Module");
						for (int k = 0; k < 4; k++)
						{
							this.torusRenderers[k].gameObject.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
							this.torusRenderers[k].sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
							this.torusRenderers[k].gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
						}
					}
					this.initialized = true;
				}
				this.torusRenderers[0].enabled = this.hab.ringStruct.NE;
				this.torusRenderers[1].enabled = this.hab.ringStruct.SE;
				this.torusRenderers[2].enabled = this.hab.ringStruct.SW;
				this.torusRenderers[3].enabled = this.hab.ringStruct.NW;
			}
		}

		// Token: 0x06005E33 RID: 24115 RVA: 0x002CC908 File Offset: 0x002CAB08
		private void UpdateDestructionVFX(TIHabModuleState habModule)
		{
			TISectorState sector = habModule.sector;
			int sectorNum = habModule.sectorNum;
			int slot = habModule.slot;
			HabModule3D habModule3D;
			if (this.modules != null && this.modules.TryGetValue(new StringBuilder("S").Append(sectorNum).Append("_M").Append(slot)
				.ToString(), out habModule3D) && habModule3D.explosionSequenceInstance != null && !habModule.destroyed)
			{
				global::UnityEngine.Object.Destroy(habModule3D.explosionSequenceInstance);
			}
		}

		// Token: 0x06005E34 RID: 24116 RVA: 0x002CC988 File Offset: 0x002CAB88
		private void EmptySector(int s)
		{
			if (!this.hab.archived)
			{
				for (int i = 0; i < 5; i++)
				{
					HabModule3D habModule3D;
					if (this.modules.TryGetValue(new StringBuilder("S").Append(s).Append("_M").Append(i)
						.ToString(), out habModule3D))
					{
						habModule3D.Empty(this.hab.sectors[s].habModules[i]);
					}
				}
			}
		}

		// Token: 0x0400434E RID: 17230
		public Hab Value;

		// Token: 0x0400434F RID: 17231
		public Dictionary<string, HabModule3D> modules;

		// Token: 0x04004350 RID: 17232
		public MeshRenderer[] torusRenderers;

		// Token: 0x04004352 RID: 17234
		public bool notBuilding;

		// Token: 0x04004353 RID: 17235
		private bool initialized;

		// Token: 0x04004354 RID: 17236
		public HabModelController habModelController;

		// Token: 0x04004355 RID: 17237
		private readonly WaitForSeconds delay = new WaitForSeconds(2.4f);
	}
}
