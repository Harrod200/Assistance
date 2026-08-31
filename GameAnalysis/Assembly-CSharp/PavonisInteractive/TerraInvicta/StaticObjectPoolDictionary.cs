using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008EC RID: 2284
	public static class StaticObjectPoolDictionary
	{
		// Token: 0x060057C0 RID: 22464 RVA: 0x00284D28 File Offset: 0x00282F28
		private static StaticObjectPoolDictionary.ObjectRef CreateObject(GameObject prefab)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab);
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
			return new StaticObjectPoolDictionary.ObjectRef
			{
				RefCount = 0,
				Instance = gameObject
			};
		}

		// Token: 0x060057C1 RID: 22465 RVA: 0x00284D64 File Offset: 0x00282F64
		public static void InitializePool(GameObject prefab, int size = 12)
		{
			StaticObjectPoolDictionary.ObjectRef[] array;
			if (StaticObjectPoolDictionary._objectPools.TryGetValue(prefab, out array))
			{
				if (array.Length < size)
				{
					StaticObjectPoolDictionary.ResizeObjectPool(prefab, size - array.Length);
				}
				return;
			}
			array = new StaticObjectPoolDictionary.ObjectRef[size];
			for (int i = 0; i < size; i++)
			{
				array[i] = StaticObjectPoolDictionary.CreateObject(prefab);
			}
			StaticObjectPoolDictionary._objectPools[prefab] = array;
		}

		// Token: 0x060057C2 RID: 22466 RVA: 0x00284DC0 File Offset: 0x00282FC0
		public static void ResizeObjectPool(GameObject prefab, int amount)
		{
			if (amount == 0)
			{
				return;
			}
			StaticObjectPoolDictionary.ObjectRef[] array;
			if (StaticObjectPoolDictionary._objectPools.TryGetValue(prefab, out array))
			{
				int num = amount - array.Length;
				if (num < 0)
				{
					int num2 = 0;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].RefCount > 0)
						{
							num2++;
						}
					}
					int num3 = array.Length - num2;
					if (num3 < num)
					{
						num = Mathf.Min(num3, num);
						Log.Warn("StaticObjectPoolDictionary.ResizeObjectPool: Some objects are still in use! Cannot reduce by full amount. Attempted {0} Reduced by: {1}", new object[] { num3, num });
					}
					else
					{
						num = Mathf.Min(num3, num);
					}
					Array.Sort<StaticObjectPoolDictionary.ObjectRef>(array, (StaticObjectPoolDictionary.ObjectRef a, StaticObjectPoolDictionary.ObjectRef b) => -a.RefCount.CompareTo(b.RefCount));
				}
				int num4 = array.Length;
				int num5 = num4 + num;
				Array.Resize<StaticObjectPoolDictionary.ObjectRef>(ref array, num5);
				if (num > 0)
				{
					for (int j = num4; j < num5; j++)
					{
						array[j] = StaticObjectPoolDictionary.CreateObject(prefab);
					}
				}
			}
		}

		// Token: 0x060057C3 RID: 22467 RVA: 0x00284EB8 File Offset: 0x002830B8
		public static bool TryClaimObject(GameObject prefab, out GameObject entry)
		{
			StaticObjectPoolDictionary.ObjectRef[] array;
			if (StaticObjectPoolDictionary._objectPools.TryGetValue(prefab, out array))
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].RefCount <= 0)
					{
						entry = array[i].Instance;
						StaticObjectPoolDictionary.ObjectRef[] array2 = array;
						int num = i;
						array2[num].RefCount = array2[num].RefCount + 1;
						return true;
					}
				}
				Log.Warn("StaticObjectPoolDictionary.TryClaimObject: All objects are in use for the given prefab type. Consider initializing a larger list.", Array.Empty<object>());
				entry = null;
				return false;
			}
			Log.Warn("StaticObjectPoolDictionary.TryClaimObject: No object pool of given prefab was initialized. A new object pool will be created for this call.", Array.Empty<object>());
			StaticObjectPoolDictionary.InitializePool(prefab, 12);
			StaticObjectPoolDictionary.TryClaimObject(prefab, out entry);
			return true;
		}

		// Token: 0x060057C4 RID: 22468 RVA: 0x00284F48 File Offset: 0x00283148
		public static void TryClaimObjects(GameObject prefab, ref GameObject[] entries)
		{
			if (entries == null)
			{
				return;
			}
			StaticObjectPoolDictionary.ObjectRef[] array;
			if (StaticObjectPoolDictionary._objectPools.TryGetValue(prefab, out array))
			{
				int num = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].RefCount <= 0)
					{
						entries[num++] = array[i].Instance;
						StaticObjectPoolDictionary.ObjectRef[] array2 = array;
						int num2 = i;
						array2[num2].RefCount = array2[num2].RefCount + 1;
						if (entries.Length == num)
						{
							return;
						}
					}
				}
				return;
			}
			Log.Warn("StaticObjectPoolDictionary.TryClaimObjects: No object pool of given prefab was initialized. A new object pool will be created for this call.", Array.Empty<object>());
			StaticObjectPoolDictionary.InitializePool(prefab, 12);
			StaticObjectPoolDictionary.TryClaimObjects(prefab, ref entries);
		}

		// Token: 0x060057C5 RID: 22469 RVA: 0x00284FD4 File Offset: 0x002831D4
		public static void ReleaseClaimedObject(GameObject prefab, ref GameObject entry)
		{
			StaticObjectPoolDictionary.ObjectRef[] array;
			if (StaticObjectPoolDictionary._objectPools.TryGetValue(prefab, out array))
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Instance == entry)
					{
						StaticObjectPoolDictionary.ObjectRef[] array2 = array;
						int num = i;
						array2[num].RefCount = array2[num].RefCount - 1;
						entry = null;
						return;
					}
				}
			}
		}

		// Token: 0x060057C6 RID: 22470 RVA: 0x00285028 File Offset: 0x00283228
		public static void ReleaseClaimedObjects(GameObject prefab, ref GameObject[] entries)
		{
			for (int i = 0; i < entries.Length; i++)
			{
				for (int j = 0; j < entries.Length; j++)
				{
					if (entries[i] == entries[j])
					{
						entries[j] = null;
					}
				}
			}
			StaticObjectPoolDictionary.ObjectRef[] array;
			if (StaticObjectPoolDictionary._objectPools.TryGetValue(prefab, out array))
			{
				for (int k = 0; k < entries.Length; k++)
				{
					for (int l = 0; l < array.Length; l++)
					{
						if (array[l].Instance == entries[k])
						{
							StaticObjectPoolDictionary.ObjectRef[] array2 = array;
							int num = l;
							array2[num].RefCount = array2[num].RefCount - 1;
							entries[k] = null;
							return;
						}
					}
				}
			}
		}

		// Token: 0x04003F5B RID: 16219
		private static Dictionary<GameObject, StaticObjectPoolDictionary.ObjectRef[]> _objectPools = new Dictionary<GameObject, StaticObjectPoolDictionary.ObjectRef[]>();

		// Token: 0x020011E3 RID: 4579
		private struct ObjectRef
		{
			// Token: 0x0400687C RID: 26748
			public int RefCount;

			// Token: 0x0400687D RID: 26749
			public GameObject Instance;
		}
	}
}
