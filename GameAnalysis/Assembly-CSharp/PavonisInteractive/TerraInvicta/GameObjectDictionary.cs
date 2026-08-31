using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E7 RID: 1767
	public class GameObjectDictionary<TKey> : IEnumerable<GameObject>, IEnumerable
	{
		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x0600291F RID: 10527 RVA: 0x000DB977 File Offset: 0x000D9B77
		public GameObject gameObject
		{
			get
			{
				if (this._gameObject == null)
				{
					this._gameObject = new GameObject(this.name);
				}
				return this._gameObject;
			}
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06002920 RID: 10528 RVA: 0x000DB99E File Offset: 0x000D9B9E
		public Transform transform
		{
			get
			{
				return this.gameObject.transform;
			}
		}

		// Token: 0x170005A9 RID: 1449
		public GameObject this[TKey key]
		{
			get
			{
				return this.lookup[key];
			}
			set
			{
				this.lookup[key] = value;
			}
		}

		// Token: 0x06002923 RID: 10531 RVA: 0x000DB9C8 File Offset: 0x000D9BC8
		public GameObjectDictionary(string name)
		{
			this.name = name;
			this._gameObject = new GameObject(name);
		}

		// Token: 0x06002924 RID: 10532 RVA: 0x000DB9F0 File Offset: 0x000D9BF0
		public bool Add(TKey key, GameObject value, bool worldPositionStays = false, bool overwrite = false)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!overwrite && this.lookup.ContainsKey(key))
			{
				Log.Warn("GameObjectDictionary<TKey>.Add(): Lookup table already contains key! game object not parented to transform.", Array.Empty<object>());
				return false;
			}
			this.lookup[key] = value;
			value.transform.SetParent(this.transform, worldPositionStays);
			return true;
		}

		// Token: 0x06002925 RID: 10533 RVA: 0x000DBA54 File Offset: 0x000D9C54
		public bool ContainsKey(TKey key)
		{
			return this.lookup.ContainsKey(key);
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x000DBA62 File Offset: 0x000D9C62
		public bool TryFind(TKey key, out GameObject value)
		{
			return this.lookup.TryGetValue(key, out value);
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x000DBA74 File Offset: 0x000D9C74
		public bool Remove(TKey key, bool destroy = true)
		{
			bool flag = false;
			GameObject gameObject;
			if (destroy && this.lookup.TryGetValue(key, out gameObject))
			{
				global::UnityEngine.Object.Destroy(gameObject);
				flag = true;
			}
			this.lookup.Remove(key);
			return destroy == flag;
		}

		// Token: 0x06002928 RID: 10536 RVA: 0x000DBAB0 File Offset: 0x000D9CB0
		public void Clear(bool destroy = true)
		{
			if (destroy)
			{
				foreach (TKey tkey in this.lookup.Keys)
				{
					global::UnityEngine.Object.Destroy(this.lookup[tkey]);
				}
			}
			this.lookup.Clear();
		}

		// Token: 0x06002929 RID: 10537 RVA: 0x000DBB1C File Offset: 0x000D9D1C
		public IEnumerator<GameObject> GetEnumerator()
		{
			return this.lookup.Values.GetEnumerator();
		}

		// Token: 0x0600292A RID: 10538 RVA: 0x000DBB2E File Offset: 0x000D9D2E
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.lookup.Values.GetEnumerator();
		}

		// Token: 0x04001F8A RID: 8074
		private string name;

		// Token: 0x04001F8B RID: 8075
		private IDictionary<TKey, GameObject> lookup = new Dictionary<TKey, GameObject>();

		// Token: 0x04001F8C RID: 8076
		private GameObject _gameObject;
	}
}
