using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Steamworks;

namespace LapinerTools.Steam.Data.Internal
{
	// Token: 0x02000543 RID: 1347
	public class SteamRequestList
	{
		// Token: 0x06002271 RID: 8817 RVA: 0x000B2698 File Offset: 0x000B0898
		public void Add<T>(CallResult<T> p_request)
		{
			Type typeFromHandle = typeof(T);
			List<object> list;
			if (!this.m_requests.TryGetValue(typeFromHandle, out list))
			{
				list = new List<object>();
				this.m_requests.Add(typeFromHandle, list);
			}
			list.Add(p_request);
		}

		// Token: 0x06002272 RID: 8818 RVA: 0x000B26DA File Offset: 0x000B08DA
		public int Count()
		{
			return this.m_requests.Values.Sum<List<object>>((List<object> requestList) => requestList.Count);
		}

		// Token: 0x06002273 RID: 8819 RVA: 0x000B270C File Offset: 0x000B090C
		public int Count<T>()
		{
			Type typeFromHandle = typeof(T);
			List<object> list;
			if (this.m_requests.TryGetValue(typeFromHandle, out list))
			{
				return list.Count;
			}
			return 0;
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x000B273C File Offset: 0x000B093C
		public void Clear<T>()
		{
			Type typeFromHandle = typeof(T);
			List<object> list;
			if (this.m_requests.TryGetValue(typeFromHandle, out list))
			{
				list.Clear();
			}
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x000B276C File Offset: 0x000B096C
		public void RemoveInactive()
		{
			foreach (KeyValuePair<Type, List<object>> keyValuePair in this.m_requests)
			{
				base.GetType().GetMethod("RemoveInactiveInternal", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[] { keyValuePair.Key })
					.Invoke(this, new object[] { keyValuePair.Value });
			}
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x000B27F8 File Offset: 0x000B09F8
		public void RemoveInactive<T>()
		{
			Type typeFromHandle = typeof(T);
			List<object> list;
			if (this.m_requests.TryGetValue(typeFromHandle, out list))
			{
				base.GetType().GetMethod("RemoveInactiveInternal", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[] { typeFromHandle })
					.Invoke(this, new object[] { list });
			}
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x000B2854 File Offset: 0x000B0A54
		public void Cancel()
		{
			foreach (KeyValuePair<Type, List<object>> keyValuePair in this.m_requests)
			{
				base.GetType().GetMethod("CancelInternal", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[] { keyValuePair.Key })
					.Invoke(this, new object[] { keyValuePair.Value });
			}
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x000B28E0 File Offset: 0x000B0AE0
		public void Cancel<T>()
		{
			Type typeFromHandle = typeof(T);
			List<object> list;
			if (this.m_requests.TryGetValue(typeFromHandle, out list))
			{
				base.GetType().GetMethod("CancelInternal", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[] { typeFromHandle })
					.Invoke(this, new object[] { list });
			}
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x000B293C File Offset: 0x000B0B3C
		private static void CancelInternal<T>(List<object> p_requests)
		{
			for (int i = p_requests.Count - 1; i >= 0; i--)
			{
				(p_requests[i] as CallResult<T>).Cancel();
			}
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000B2970 File Offset: 0x000B0B70
		private static void RemoveInactiveInternal<T>(List<object> p_requests)
		{
			for (int i = p_requests.Count - 1; i >= 0; i--)
			{
				if (!(p_requests[i] as CallResult<T>).IsActive())
				{
					p_requests.RemoveAt(i);
				}
			}
		}

		// Token: 0x04001A2D RID: 6701
		private Dictionary<Type, List<object>> m_requests = new Dictionary<Type, List<object>>();
	}
}
