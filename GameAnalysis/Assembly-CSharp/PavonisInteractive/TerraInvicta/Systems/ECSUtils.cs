using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000997 RID: 2455
	public static class ECSUtils
	{
		// Token: 0x06005CDA RID: 23770 RVA: 0x002C3B9C File Offset: 0x002C1D9C
		public static GameObject CreateEntity(string type, string name)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/" + type));
			GameObject gameObject2 = GameObject.Find("Entities/" + type);
			gameObject.transform.SetParent(gameObject2.transform);
			gameObject.name = name;
			return gameObject;
		}
	}
}
