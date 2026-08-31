using System;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F4 RID: 2036
	public static class EntityExtensions
	{
		// Token: 0x060049F6 RID: 18934 RVA: 0x001F0F0F File Offset: 0x001EF10F
		public static T GetComponent<T>(this Entity entity) where T : Component
		{
			return World.Active.GetOrCreateManager<EntityManager>().GetComponentObject<T>(entity);
		}

		// Token: 0x060049F7 RID: 18935 RVA: 0x001F0F21 File Offset: 0x001EF121
		public static bool HasComponent<T>(this Entity entity) where T : Component
		{
			return World.Active.GetOrCreateManager<EntityManager>().HasComponent<T>(entity);
		}
	}
}
