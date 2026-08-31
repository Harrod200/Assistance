using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x02000926 RID: 2342
	public abstract class ListPane<U> : MonoBehaviour
	{
		// Token: 0x17000F38 RID: 3896
		// (get) Token: 0x06005976 RID: 22902 RVA: 0x00291074 File Offset: 0x0028F274
		public IEnumerable<U> Items
		{
			get
			{
				return this.itemControllers.Keys;
			}
		}

		// Token: 0x17000F39 RID: 3897
		// (get) Token: 0x06005977 RID: 22903 RVA: 0x00291081 File Offset: 0x0028F281
		public IEnumerable<IListPaneItem<U>> ItemControllers
		{
			get
			{
				return this.itemControllers.Values;
			}
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x0029108E File Offset: 0x0028F28E
		public void Initialize()
		{
			base.transform.DestroyChildren();
			this.InitializeItems(this.ItemsToDisplay());
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x002910A8 File Offset: 0x0028F2A8
		public void Refresh()
		{
			IEnumerable<U> enumerable = this.ItemsToDisplay();
			this.newItemSet.Clear();
			this.newItemSet.UnionWith(enumerable);
			if (!this.cachedItemSet.SetEquals(this.newItemSet))
			{
				HashSet<U> hashSet = new HashSet<U>(this.newItemSet);
				HashSet<U> hashSet2 = new HashSet<U>(this.cachedItemSet);
				hashSet.ExceptWith(this.cachedItemSet);
				hashSet2.ExceptWith(this.newItemSet);
				if (hashSet.Any<U>())
				{
					this.InitializeItems(hashSet);
				}
				if (hashSet2.Any<U>())
				{
					foreach (U u in hashSet2)
					{
						global::UnityEngine.Object.Destroy(this.itemGOs[u]);
						this.itemGOs.Remove(u);
						this.itemControllers.Remove(u);
						this.cachedItemSet.Remove(u);
					}
				}
			}
			foreach (IListPaneItem<U> listPaneItem in this.itemControllers.Values)
			{
				listPaneItem.Refresh();
			}
		}

		// Token: 0x0600597A RID: 22906
		protected abstract IEnumerable<U> ItemsToDisplay();

		// Token: 0x0600597B RID: 22907 RVA: 0x002911EC File Offset: 0x0028F3EC
		protected virtual void BeforeInitialize(IListPaneItem<U> listItem)
		{
		}

		// Token: 0x0600597C RID: 22908 RVA: 0x002911F0 File Offset: 0x0028F3F0
		private void InitializeItems(IEnumerable<U> items)
		{
			foreach (U u in items)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.prefab, base.transform);
				Loc.SwapFonts(gameObject);
				IListPaneItem<U> component = gameObject.GetComponent<IListPaneItem<U>>();
				if (component == null)
				{
					Debug.LogError(string.Format("Missing IListPaneItem for {0}", base.GetType()));
					break;
				}
				this.BeforeInitialize(component);
				component.Initialize(u);
				gameObject.SetActive(true);
				this.itemGOs.Add(u, gameObject);
				this.itemControllers.Add(u, component);
				this.cachedItemSet.Add(u);
			}
		}

		// Token: 0x040040A1 RID: 16545
		public GameObject prefab;

		// Token: 0x040040A2 RID: 16546
		private Dictionary<U, IListPaneItem<U>> itemControllers = new Dictionary<U, IListPaneItem<U>>();

		// Token: 0x040040A3 RID: 16547
		private Dictionary<U, GameObject> itemGOs = new Dictionary<U, GameObject>();

		// Token: 0x040040A4 RID: 16548
		private HashSet<U> cachedItemSet = new HashSet<U>();

		// Token: 0x040040A5 RID: 16549
		private HashSet<U> newItemSet = new HashSet<U>();
	}
}
