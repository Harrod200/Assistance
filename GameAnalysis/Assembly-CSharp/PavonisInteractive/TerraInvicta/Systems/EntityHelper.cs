using System;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000998 RID: 2456
	public class EntityHelper : IInitializable
	{
		// Token: 0x06005CDB RID: 23771 RVA: 0x002C3BE7 File Offset: 0x002C1DE7
		public void Initialize()
		{
			this.entityContainer = GameObject.Find("Entities");
			this.entityManager = World.Active.GetOrCreateManager<EntityManager>();
		}

		// Token: 0x06005CDC RID: 23772 RVA: 0x002C3C09 File Offset: 0x002C1E09
		public T CreateEntity<T>(GameObject prefab) where T : MonoBehaviour
		{
			return global::UnityEngine.Object.Instantiate<GameObject>(prefab).GetComponent<T>();
		}

		// Token: 0x06005CDD RID: 23773 RVA: 0x002C3C18 File Offset: 0x002C1E18
		public T CreateEntity<T>() where T : MonoBehaviour
		{
			Entity entity = this.entityManager.Instantiate(this.entitySettings.entityPrefab);
			this.entityManager.AddComponent(entity, typeof(T));
			return this.entityManager.GetComponentObject<T>(entity);
		}

		// Token: 0x04004261 RID: 16993
		private GameObject entityContainer;

		// Token: 0x04004262 RID: 16994
		private EntityManager entityManager;

		// Token: 0x04004263 RID: 16995
		[global::Zenject.Inject]
		private TestSettings.EntitySettings entitySettings;
	}
}
