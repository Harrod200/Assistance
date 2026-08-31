using System;
using System.Collections.Generic;

namespace FullSerializer
{
	// Token: 0x02000462 RID: 1122
	public sealed class fsContext
	{
		// Token: 0x060017AC RID: 6060 RVA: 0x0007B3DD File Offset: 0x000795DD
		public void Reset()
		{
			this._contextObjects.Clear();
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x0007B3EA File Offset: 0x000795EA
		public void Set<T>(T obj)
		{
			this._contextObjects[typeof(T)] = obj;
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x0007B407 File Offset: 0x00079607
		public bool Has<T>()
		{
			return this._contextObjects.ContainsKey(typeof(T));
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x0007B420 File Offset: 0x00079620
		public T Get<T>()
		{
			object obj;
			if (this._contextObjects.TryGetValue(typeof(T), out obj))
			{
				return (T)((object)obj);
			}
			string text = "There is no context object of type ";
			Type typeFromHandle = typeof(T);
			throw new InvalidOperationException(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x040015E3 RID: 5603
		private readonly Dictionary<Type, object> _contextObjects = new Dictionary<Type, object>();
	}
}
