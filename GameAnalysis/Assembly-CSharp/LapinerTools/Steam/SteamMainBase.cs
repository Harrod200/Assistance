using System;
using System.Collections.Generic;
using LapinerTools.Steam.Data;
using LapinerTools.Steam.Data.Internal;
using Steamworks;
using UnityEngine;

namespace LapinerTools.Steam
{
	// Token: 0x02000531 RID: 1329
	public class SteamMainBase<SteamMainT> : MonoBehaviour where SteamMainT : SteamMainBase<SteamMainT>
	{
		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06002135 RID: 8501 RVA: 0x000ABCA4 File Offset: 0x000A9EA4
		public static SteamMainT Instance
		{
			get
			{
				if (SteamMainBase<SteamMainT>.s_instance == null)
				{
					SteamMainBase<SteamMainT>.s_instance = global::UnityEngine.Object.FindObjectOfType<SteamMainT>();
				}
				if (SteamMainBase<SteamMainT>.s_instance == null)
				{
					SteamMainBase<SteamMainT>.s_instance = new GameObject(typeof(SteamMainT).Name).AddComponent<SteamMainT>();
				}
				return SteamMainBase<SteamMainT>.s_instance;
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06002136 RID: 8502 RVA: 0x000ABD02 File Offset: 0x000A9F02
		public static bool IsInstanceSet
		{
			get
			{
				return SteamMainBase<SteamMainT>.s_instance != null;
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06002137 RID: 8503 RVA: 0x000ABD14 File Offset: 0x000A9F14
		// (remove) Token: 0x06002138 RID: 8504 RVA: 0x000ABD4C File Offset: 0x000A9F4C
		public event Action<ErrorEventArgs> OnError;

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06002139 RID: 8505 RVA: 0x000ABD81 File Offset: 0x000A9F81
		// (set) Token: 0x0600213A RID: 8506 RVA: 0x000ABD89 File Offset: 0x000A9F89
		public bool IsDebugLogEnabled
		{
			get
			{
				return this.m_isDebugLogEnabled;
			}
			set
			{
				this.m_isDebugLogEnabled = value;
			}
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x000ABD94 File Offset: 0x000A9F94
		public void Execute<T>(SteamAPICall_t p_steamCall, CallResult<T>.APIDispatchDelegate p_onCompleted)
		{
			CallResult<T> callResult = CallResult<T>.Create(p_onCompleted);
			callResult.Set(p_steamCall, null);
			this.m_pendingRequests.Add<T>(callResult);
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x000ABDBC File Offset: 0x000A9FBC
		protected virtual void OnDisable()
		{
			if (this.m_pendingRequests != null)
			{
				this.m_pendingRequests.Cancel();
			}
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x000ABDD4 File Offset: 0x000A9FD4
		protected virtual void LateUpdate()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_pendingRequests.RemoveInactive();
				if (this.IsDebugLogEnabled && Time.frameCount % 300 == 0)
				{
					if (this.m_pendingRequests.Count() > 0)
					{
						Debug.Log(typeof(SteamMainT).Name + ": pending requests left: " + this.m_pendingRequests.Count().ToString());
					}
					foreach (KeyValuePair<string, List<object>> keyValuePair in this.m_singleShotEventHandlers)
					{
						if (keyValuePair.Value.Count > 0)
						{
							Debug.Log(string.Concat(new string[]
							{
								typeof(SteamMainT).Name,
								": pending signle shot event handlers for '",
								keyValuePair.Key,
								"' left: ",
								keyValuePair.Value.Count.ToString()
							}));
						}
					}
				}
			}
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x000ABF2C File Offset: 0x000AA12C
		protected virtual bool CheckAndLogResultNoEvent<Trequest>(string p_logText, EResult p_result, bool p_bIOFailure)
		{
			Action<object> action = null;
			return this.CheckAndLogResult<Trequest, object>(p_logText, p_result, p_bIOFailure, null, ref action);
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x000ABF48 File Offset: 0x000AA148
		protected virtual bool CheckAndLogResult<Trequest, Tevent>(string p_logText, EResult p_result, bool p_bIOFailure, string p_eventName, ref Action<Tevent> p_event)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_pendingRequests.RemoveInactive<Trequest>();
				if (this.IsDebugLogEnabled)
				{
					Debug.Log(string.Concat(new string[]
					{
						p_logText,
						": (fail:",
						p_bIOFailure.ToString(),
						") ",
						p_result.ToString(),
						" requests left: ",
						this.m_pendingRequests.Count<Trequest>().ToString()
					}));
				}
			}
			if (p_result == EResult.k_EResultOK && !p_bIOFailure)
			{
				return true;
			}
			ErrorEventArgs e = ErrorEventArgs.Create(p_result);
			this.HandleError(p_logText + ": failed! ", e);
			if (p_eventName != null && p_event != null)
			{
				this.CallSingleShotEventHandlers<Tevent>(p_eventName, (Tevent)((object)Activator.CreateInstance(typeof(Tevent), new object[] { e })), ref p_event);
			}
			return false;
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x000AC048 File Offset: 0x000AA248
		protected virtual void HandleError(string p_logPrefix, ErrorEventArgs p_error)
		{
			Debug.LogError(p_logPrefix + p_error.ErrorMessage);
			this.InvokeEventHandlerSafely<ErrorEventArgs>(this.OnError, p_error);
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x000AC068 File Offset: 0x000AA268
		protected virtual void InvokeEventHandlerSafely<T>(Action<T> p_handler, T p_data)
		{
			try
			{
				if (p_handler != null)
				{
					p_handler(p_data);
				}
			}
			catch (Exception ex)
			{
				string[] array = new string[7];
				array[0] = typeof(SteamMainT).Name;
				array[1] = ": your event handler ('";
				int num = 2;
				object target = p_handler.Target;
				array[num] = ((target != null) ? target.ToString() : null);
				array[3] = "' - System.Action<";
				int num2 = 4;
				Type typeFromHandle = typeof(T);
				array[num2] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
				array[5] = ">) has thrown an excepotion!\n";
				int num3 = 6;
				Exception ex2 = ex;
				array[num3] = ((ex2 != null) ? ex2.ToString() : null);
				Debug.LogError(string.Concat(array));
			}
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x000AC10C File Offset: 0x000AA30C
		protected virtual void SetSingleShotEventHandler<T>(string p_eventName, ref Action<T> p_event, Action<T> p_handler)
		{
			if (p_handler != null)
			{
				if (!this.m_singleShotEventHandlers.ContainsKey(p_eventName))
				{
					this.m_singleShotEventHandlers.Add(p_eventName, new List<object>());
				}
				this.m_singleShotEventHandlers[p_eventName].Add(p_handler.Target);
				p_event = (Action<T>)Delegate.Combine(p_event, p_handler);
			}
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x000AC164 File Offset: 0x000AA364
		protected virtual void CallSingleShotEventHandlers<T>(string p_eventName, T p_args, ref Action<T> p_event)
		{
			if (p_event != null && this.m_singleShotEventHandlers.ContainsKey(p_eventName))
			{
				int count = this.m_singleShotEventHandlers[p_eventName].Count;
				Delegate[] invocationList = p_event.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					if (this.m_singleShotEventHandlers[p_eventName].Contains(@delegate.Target))
					{
						p_event = (Action<T>)Delegate.Remove(p_event, (Action<T>)@delegate);
						this.m_singleShotEventHandlers[p_eventName].Remove(@delegate.Target);
						try
						{
							@delegate.DynamicInvoke(new object[] { p_args });
						}
						catch (Exception ex)
						{
							string[] array2 = new string[7];
							array2[0] = typeof(SteamMainT).Name;
							array2[1] = ": your event handler ('";
							int num = 2;
							object target = @delegate.Target;
							array2[num] = ((target != null) ? target.ToString() : null);
							array2[3] = "' - System.Action<";
							int num2 = 4;
							Type typeFromHandle = typeof(T);
							array2[num2] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
							array2[5] = ">) has thrown an excepotion!\n";
							int num3 = 6;
							Exception ex2 = ex;
							array2[num3] = ((ex2 != null) ? ex2.ToString() : null);
							Debug.LogError(string.Concat(array2));
						}
					}
				}
				if (this.IsDebugLogEnabled)
				{
					string[] array3 = new string[11];
					array3[0] = typeof(SteamMainT).Name;
					array3[1] = ": CallSingleShotEventHandlers '";
					array3[2] = p_eventName;
					array3[3] = "' left handlers: ";
					int num4 = 4;
					int i = ((p_event != null) ? p_event.GetInvocationList().Length : 0);
					array3[num4] = i.ToString();
					array3[5] = "/";
					int num5 = 6;
					i = invocationList.Length;
					array3[num5] = i.ToString();
					array3[7] = " left single shots: ";
					int num6 = 8;
					i = this.m_singleShotEventHandlers[p_eventName].Count;
					array3[num6] = i.ToString();
					array3[9] = "/";
					array3[10] = count.ToString();
					Debug.Log(string.Concat(array3));
				}
			}
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x000AC35C File Offset: 0x000AA55C
		protected virtual void ClearSingleShotEventHandlers<T>(string p_eventName, ref Action<T> p_event)
		{
			if (p_event != null && this.m_singleShotEventHandlers.ContainsKey(p_eventName))
			{
				int count = this.m_singleShotEventHandlers[p_eventName].Count;
				Delegate[] invocationList = p_event.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					if (this.m_singleShotEventHandlers[p_eventName].Contains(@delegate.Target))
					{
						p_event = (Action<T>)Delegate.Remove(p_event, (Action<T>)@delegate);
						this.m_singleShotEventHandlers[p_eventName].Remove(@delegate.Target);
					}
				}
				if (this.IsDebugLogEnabled)
				{
					Debug.Log(string.Concat(new string[]
					{
						typeof(SteamMainT).Name,
						": ClearSingleShotEventHandler '",
						p_eventName,
						"' left handlers: ",
						((p_event != null) ? p_event.GetInvocationList().Length : 0).ToString(),
						"/",
						invocationList.Length.ToString(),
						" left single shots: ",
						this.m_singleShotEventHandlers[p_eventName].Count.ToString(),
						"/",
						count.ToString()
					}));
				}
			}
		}

		// Token: 0x0400199A RID: 6554
		protected static SteamMainT s_instance;

		// Token: 0x0400199B RID: 6555
		protected SteamRequestList m_pendingRequests = new SteamRequestList();

		// Token: 0x0400199C RID: 6556
		private Dictionary<string, List<object>> m_singleShotEventHandlers = new Dictionary<string, List<object>>();

		// Token: 0x0400199D RID: 6557
		protected object m_lock = new object();

		// Token: 0x0400199F RID: 6559
		[SerializeField]
		[Tooltip("Set this property to true if you want to see a detailed log in the console. Disabled by default.")]
		protected bool m_isDebugLogEnabled = true;
	}
}
