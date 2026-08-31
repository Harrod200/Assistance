using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E4 RID: 2276
	public class ListManagerBase : MonoBehaviour
	{
		// Token: 0x17000F1D RID: 3869
		// (get) Token: 0x0600579F RID: 22431 RVA: 0x002847DD File Offset: 0x002829DD
		public int size
		{
			get
			{
				return this._size;
			}
		}

		// Token: 0x060057A0 RID: 22432 RVA: 0x002847E8 File Offset: 0x002829E8
		private void InitListItem()
		{
			if (this.listItemInstance != null)
			{
				return;
			}
			if (base.transform.childCount > 0)
			{
				this.listItemInstance = base.transform.GetChild(0);
				return;
			}
			Debug.LogError("List " + base.name + " initialized with no child component.");
		}

		// Token: 0x060057A1 RID: 22433 RVA: 0x00284840 File Offset: 0x00282A40
		public void SetListSize<T>(int newSize, bool inactive = false, bool toggleGameobjectActiveState = false)
		{
			if (newSize == 0)
			{
				this._typeList = new List<T>();
				base.gameObject.SetActive(false);
				this._size = 0;
				return;
			}
			if (toggleGameobjectActiveState)
			{
				base.gameObject.SetActive(false);
			}
			this.MakeChildListSize<T>(newSize, inactive);
			base.gameObject.SetActive(!inactive);
			this._size = newSize;
		}

		// Token: 0x060057A2 RID: 22434 RVA: 0x0028489C File Offset: 0x00282A9C
		private void MakeChildListSize<T>(int newSize, bool inactive)
		{
			if (this.listItemInstance == null)
			{
				this.InitListItem();
			}
			int num = base.transform.childCount;
			if (newSize > num)
			{
				for (int i = num; i < newSize; i++)
				{
					global::UnityEngine.Object.Instantiate<Transform>(this.listItemInstance, base.transform);
					if (inactive)
					{
						this.listItemInstance.gameObject.SetActive(false);
					}
				}
			}
			else if (TooltipManager.Instance != null && TooltipManager.Instance.TooltipContainer != null)
			{
				if (TooltipManager.Instance.TooltipContainer.transform.IsChildOf(base.transform))
				{
					TooltipManager.Instance.MoveContainerToDummyCanvas(false);
					num = base.transform.childCount;
				}
				for (int j = num - 1; j >= newSize; j--)
				{
					global::UnityEngine.Object.Destroy(base.transform.GetChild(j).gameObject);
				}
			}
			int num2 = 0;
			List<T> list = new List<T>();
			using (IEnumerator enumerator = base.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if (((Transform)enumerator.Current).TryGetComponent<T>(out t))
					{
						list.Add(t);
						if (num2 == newSize - 1)
						{
							break;
						}
						num2++;
					}
				}
			}
			this._typeList = list;
		}

		// Token: 0x060057A3 RID: 22435 RVA: 0x002849F4 File Offset: 0x00282BF4
		[return: Dynamic(new bool[] { false, true })]
		public IEnumerator<dynamic> GetEnumerator()
		{
			if (ListManagerBase.<>o__8.<>p__1 == null)
			{
				ListManagerBase.<>o__8.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(ListManagerBase), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = ListManagerBase.<>o__8.<>p__1.Target;
			CallSite <>p__ = ListManagerBase.<>o__8.<>p__1;
			if (ListManagerBase.<>o__8.<>p__0 == null)
			{
				ListManagerBase.<>o__8.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(ListManagerBase), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target(<>p__, ListManagerBase.<>o__8.<>p__0.Target(ListManagerBase.<>o__8.<>p__0, this._typeList, null)))
			{
				if (ListManagerBase.<>o__8.<>p__2 == null)
				{
					ListManagerBase.<>o__8.<>p__2 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(ListManagerBase)));
				}
				foreach (object obj in ListManagerBase.<>o__8.<>p__2.Target(ListManagerBase.<>o__8.<>p__2, this._typeList))
				{
					yield return obj;
				}
				IEnumerator enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x04003F4C RID: 16204
		[SerializeField]
		private Transform listItemInstance;

		// Token: 0x04003F4D RID: 16205
		private int _size;

		// Token: 0x04003F4E RID: 16206
		[Dynamic]
		private dynamic _typeList;
	}
}
