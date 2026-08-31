using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E6 RID: 1766
	public class EventManager : MonoBehaviour, IEventManager
	{
		// Token: 0x06002912 RID: 10514 RVA: 0x000DB000 File Offset: 0x000D9200
		public void ClearAllEvents()
		{
			this.deferredQueue.Clear();
			this.allQueuedDelegates.Clear();
			this.delegates.Clear();
			this.delegateLookup.Clear();
			this.delegatePreFilters.Clear();
			this.delegateGOs.Clear();
			this.delegateRequireGO.Clear();
			this.delegatesQueueable.Clear();
			this.onceLookups.Clear();
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x000DB070 File Offset: 0x000D9270
		private EventManager.EventDelegate AddDelegate<T>(EventManager.EventDelegate<T> del, string eventName = null) where T : GameEvent
		{
			EventManager.<>c__DisplayClass13_0<T> CS$<>8__locals1 = new EventManager.<>c__DisplayClass13_0<T>();
			CS$<>8__locals1.del = del;
			if (this.delegateLookup.ContainsKey(new ValueTuple<Delegate, string>(CS$<>8__locals1.del, eventName)))
			{
				return null;
			}
			this.delegateLookup[new ValueTuple<Delegate, string>(CS$<>8__locals1.del, eventName)] = new EventManager.EventDelegate(CS$<>8__locals1.<AddDelegate>g__internalDelegate|0);
			EventManager.EventKey eventKey = new EventManager.EventKey(typeof(T), eventName);
			EventManager.EventDelegate eventDelegate;
			if (this.delegates.TryGetValue(eventKey, out eventDelegate))
			{
				eventDelegate = (this.delegates[eventKey] = (EventManager.EventDelegate)Delegate.Combine(eventDelegate, new EventManager.EventDelegate(CS$<>8__locals1.<AddDelegate>g__internalDelegate|0)));
			}
			else
			{
				this.delegates[eventKey] = new EventManager.EventDelegate(CS$<>8__locals1.<AddDelegate>g__internalDelegate|0);
			}
			return new EventManager.EventDelegate(CS$<>8__locals1.<AddDelegate>g__internalDelegate|0);
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x000DB138 File Offset: 0x000D9338
		public void AddListener<T>(EventManager.EventDelegate<T> del, string eventName = null, object preFilterObject = null, bool queueable = true, bool callOnce = false) where T : GameEvent
		{
			EventManager.EventDelegate eventDelegate = this.AddDelegate<T>(del, eventName);
			if (eventDelegate != null)
			{
				MonoBehaviour monoBehaviour = del.Target as MonoBehaviour;
				if (monoBehaviour != null)
				{
					this.delegateGOs[eventDelegate] = monoBehaviour;
					this.delegateRequireGO[eventDelegate] = true;
				}
				if (preFilterObject != null)
				{
					this.delegatePreFilters[eventDelegate] = preFilterObject;
				}
				if (queueable)
				{
					this.delegatesQueueable.Add(eventDelegate);
				}
				if (callOnce)
				{
					this.onceLookups[new ValueTuple<Delegate, string>(eventDelegate, eventName)] = del;
				}
			}
		}

		// Token: 0x06002915 RID: 10517 RVA: 0x000DB1B8 File Offset: 0x000D93B8
		public void RemoveListener<T>(EventManager.EventDelegate<T> del, string eventName = null) where T : GameEvent
		{
			EventManager.EventDelegate eventDelegate;
			if (this.delegateLookup.TryGetValue(new ValueTuple<Delegate, string>(del, eventName), out eventDelegate))
			{
				EventManager.EventKey eventKey = new EventManager.EventKey(typeof(T), eventName);
				EventManager.EventDelegate eventDelegate2;
				if (this.delegates.TryGetValue(eventKey, out eventDelegate2))
				{
					eventDelegate2 = (EventManager.EventDelegate)Delegate.Remove(eventDelegate2, eventDelegate);
					if (eventDelegate2 == null)
					{
						this.delegates.Remove(eventKey);
					}
					else
					{
						this.delegates[eventKey] = eventDelegate2;
					}
				}
				this.delegateLookup.Remove(new ValueTuple<Delegate, string>(del, eventName));
				if (this.delegateGOs.ContainsKey(del))
				{
					this.delegateGOs.Remove(del);
				}
				if (this.delegateRequireGO.ContainsKey(del))
				{
					this.delegateRequireGO.Remove(del);
				}
				if (this.delegatePreFilters.ContainsKey(del))
				{
					this.delegatePreFilters.Remove(del);
				}
				if (this.delegatesQueueable.Contains(del))
				{
					this.delegatesQueueable.Remove(del);
				}
				if (this.onceLookups.ContainsKey(new ValueTuple<Delegate, string>(del, eventName)))
				{
					this.onceLookups.Remove(new ValueTuple<Delegate, string>(del, eventName));
				}
			}
		}

		// Token: 0x06002916 RID: 10518 RVA: 0x000DB2D4 File Offset: 0x000D94D4
		public void RemoveAll()
		{
			this.delegates.Clear();
			this.delegateLookup.Clear();
			this.delegateGOs.Clear();
			this.delegateRequireGO.Clear();
			this.delegatePreFilters.Clear();
			this.delegatesQueueable.Clear();
			this.onceLookups.Clear();
		}

		// Token: 0x06002917 RID: 10519 RVA: 0x000DB32E File Offset: 0x000D952E
		public void ClearPendingEvents(GameEvent evt, string eventName = null, params object[] sourceObjects)
		{
			this.ClearQueueOfEvents(ref evt.immediateQueue, evt, eventName, sourceObjects);
			this.ClearQueueOfEvents(ref this.deferredQueue, evt, eventName, sourceObjects);
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x000DB350 File Offset: 0x000D9550
		private bool SameEvent(QueuedDelegate qd, QueuedDelegate qd2)
		{
			return qd.evt == qd2.evt && (qd.eventName == null || qd2.eventName == null || qd.eventName == qd2.eventName) && object.Equals(qd.sourceObjects, qd2.sourceObjects);
		}

		// Token: 0x06002919 RID: 10521 RVA: 0x000DB3A4 File Offset: 0x000D95A4
		private void ClearQueueOfEvents(ref List<QueuedDelegate> queue, GameEvent evt, string eventName = null, params object[] sourceObjects)
		{
			for (int i = 0; i < queue.Count; i++)
			{
				if (queue[i].evt == evt && (sourceObjects == null || sourceObjects == queue[i].sourceObjects) && (eventName == null || eventName == queue[i].eventName))
				{
					queue.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600291A RID: 10522 RVA: 0x000DB408 File Offset: 0x000D9608
		public void TriggerEvent(GameEvent evt, string eventName = null, params object[] sourceObjects)
		{
			evt.immediateQueue = new List<QueuedDelegate>();
			QueuedDelegate queuedDelegate = new QueuedDelegate
			{
				evt = evt,
				sourceObjects = sourceObjects,
				eventName = eventName
			};
			EventManager.EventKey eventKey = new EventManager.EventKey(evt.GetType(), eventName);
			EventManager.EventDelegate eventDelegate;
			if (this.delegates.TryGetValue(eventKey, out eventDelegate))
			{
				Delegate[] invocationList = eventDelegate.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					EventManager.EventDelegate eventDelegate2 = invocationList[i] as EventManager.EventDelegate;
					object obj;
					if (eventDelegate2 != null && (!this.delegatePreFilters.TryGetValue(eventDelegate2, out obj) || (sourceObjects != null && sourceObjects.Contains(obj))))
					{
						queuedDelegate.del = eventDelegate2;
						this.allQueuedDelegates.Add(eventDelegate2);
						if (this.delegatesQueueable.Contains(eventDelegate2))
						{
							this.deferredQueue.Add(queuedDelegate);
						}
						else
						{
							evt.immediateQueue.Add(queuedDelegate);
						}
					}
				}
				foreach (EventManager.EventDelegate eventDelegate3 in this.delegates[eventKey].GetInvocationList())
				{
					if (this.allQueuedDelegates.Contains(eventDelegate3) && this.onceLookups.ContainsKey(new ValueTuple<Delegate, string>(eventDelegate3, eventName)))
					{
						Dictionary<EventManager.EventKey, EventManager.EventDelegate> dictionary = this.delegates;
						EventManager.EventKey eventKey2 = eventKey;
						dictionary[eventKey2] = (EventManager.EventDelegate)Delegate.Remove(dictionary[eventKey2], eventDelegate3);
						if (this.delegates[eventKey] == null)
						{
							this.delegates.Remove(eventKey);
						}
						this.delegateLookup.Remove(new ValueTuple<Delegate, string>(this.onceLookups[new ValueTuple<Delegate, string>(eventDelegate3, eventName)], eventName));
						this.onceLookups.Remove(new ValueTuple<Delegate, string>(eventDelegate3, eventName));
					}
				}
				this.allQueuedDelegates.Clear();
				while (evt.immediateQueue.Count > 0)
				{
					QueuedDelegate queuedDelegate2 = evt.immediateQueue[0];
					((EventManager.EventDelegate)queuedDelegate2.del)(queuedDelegate2.evt);
					if (evt.immediateQueue.Contains(queuedDelegate2))
					{
						evt.immediateQueue.Remove(queuedDelegate2);
					}
				}
				evt.immediateQueue.Clear();
			}
		}

		// Token: 0x0600291B RID: 10523 RVA: 0x000DB634 File Offset: 0x000D9834
		private void Start()
		{
			for (int i = 0; i < 30; i++)
			{
				this.secondsWorkedPerUpdate.Enqueue(0f);
				this.eventsProcessedPerUpdate.Enqueue(0);
			}
			for (int j = 0; j < 90; j++)
			{
				this.eventsQueuedPerFrame.Enqueue(0);
			}
		}

		// Token: 0x0600291C RID: 10524 RVA: 0x000DB684 File Offset: 0x000D9884
		private void Update()
		{
			float num = (float)Metrics.secondsSinceStartOfUpdate;
			float num2;
			if (Metrics.lastFramerate < 30f)
			{
				num2 = Mathf.Min(0.005f, Metrics.lastFrametime * 0.1f);
			}
			else
			{
				float num3 = 60f;
				if (Metrics.lastFramerate < 60f)
				{
					num3 = 30f;
				}
				num2 = 1f / num3 - num;
				num2 *= 0.9f;
			}
			float num4 = this.secondsWorkedPerUpdate.Sum() / (float)this.eventsProcessedPerUpdate.Sum();
			if (this.eventsProcessedPerUpdate.Sum() > 0)
			{
				float num5 = num4 * (float)this.deferredQueue.Count;
				float num6 = this.secondsWorkedPerUpdate.Average() / Metrics.lastFrametime;
				if (num5 / num6 > 1f)
				{
					float num7 = (float)this.eventsQueuedPerFrame.Average() * num4;
					float num8 = num5 / (2f * Metrics.lastFramerate);
					float num9 = num7 + num8;
					if (num9 + num > 0.1f)
					{
						num9 = Mathf.Max(0.1f - num, 0f);
					}
					num2 = Mathf.Max(num2, num9);
				}
			}
			num2 = Mathf.Max(num2, 0.0005f);
			int num10 = this.deferredQueue.Count - this.eventCountLastUpdate;
			this.eventsQueuedPerFrame.Enqueue(num10);
			this.eventsQueuedPerFrame.Dequeue();
			int num11 = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (this.deferredQueue.Count > 0 && stopwatch.GetElapsedSeconds() < (double)num2)
			{
				QueuedDelegate queuedDelegate = this.deferredQueue[0];
				bool flag = true;
				bool flag2;
				MonoBehaviour monoBehaviour;
				if (this.delegateRequireGO.TryGetValue(queuedDelegate.del, out flag2) && flag2 && (!this.delegateGOs.TryGetValue(queuedDelegate.del, out monoBehaviour) || monoBehaviour == null || monoBehaviour.gameObject == null))
				{
					flag = false;
				}
				if (flag)
				{
					((EventManager.EventDelegate)queuedDelegate.del)(queuedDelegate.evt);
				}
				this.deferredQueue.Remove(queuedDelegate);
				num11++;
			}
			stopwatch.Stop();
			this.secondsWorkedPerUpdate.Enqueue((float)stopwatch.GetElapsedSeconds());
			this.secondsWorkedPerUpdate.Dequeue();
			this.eventsProcessedPerUpdate.Enqueue(num11);
			this.eventsProcessedPerUpdate.Dequeue();
			this.eventCountLastUpdate = this.deferredQueue.Count;
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x000DB8D8 File Offset: 0x000D9AD8
		public void OnApplicationQuit()
		{
			this.RemoveAll();
		}

		// Token: 0x04001F79 RID: 8057
		private List<QueuedDelegate> deferredQueue = new List<QueuedDelegate>();

		// Token: 0x04001F7A RID: 8058
		private HashSet<Delegate> allQueuedDelegates = new HashSet<Delegate>();

		// Token: 0x04001F7B RID: 8059
		private Dictionary<EventManager.EventKey, EventManager.EventDelegate> delegates = new Dictionary<EventManager.EventKey, EventManager.EventDelegate>();

		// Token: 0x04001F7C RID: 8060
		private Dictionary<ValueTuple<Delegate, string>, EventManager.EventDelegate> delegateLookup = new Dictionary<ValueTuple<Delegate, string>, EventManager.EventDelegate>();

		// Token: 0x04001F7D RID: 8061
		private Dictionary<Delegate, object> delegatePreFilters = new Dictionary<Delegate, object>();

		// Token: 0x04001F7E RID: 8062
		private Dictionary<Delegate, MonoBehaviour> delegateGOs = new Dictionary<Delegate, MonoBehaviour>();

		// Token: 0x04001F7F RID: 8063
		private Dictionary<Delegate, bool> delegateRequireGO = new Dictionary<Delegate, bool>();

		// Token: 0x04001F80 RID: 8064
		private HashSet<Delegate> delegatesQueueable = new HashSet<Delegate>();

		// Token: 0x04001F81 RID: 8065
		[TupleElementNames(new string[] { "del", "eventName" })]
		private Dictionary<ValueTuple<Delegate, string>, Delegate> onceLookups = new Dictionary<ValueTuple<Delegate, string>, Delegate>();

		// Token: 0x04001F82 RID: 8066
		private Queue<float> secondsWorkedPerUpdate = new Queue<float>();

		// Token: 0x04001F83 RID: 8067
		private Queue<int> eventsProcessedPerUpdate = new Queue<int>();

		// Token: 0x04001F84 RID: 8068
		private Queue<int> eventsQueuedPerFrame = new Queue<int>();

		// Token: 0x04001F85 RID: 8069
		private int eventCountLastUpdate;

		// Token: 0x04001F86 RID: 8070
		private const float minAvailableSeconds = 0.0005f;

		// Token: 0x04001F87 RID: 8071
		private const float maxDelayInSeconds = 1f;

		// Token: 0x04001F88 RID: 8072
		private const float catchupTimeInSeconds = 2f;

		// Token: 0x04001F89 RID: 8073
		private const float maxFrametime = 0.1f;

		// Token: 0x02000D0D RID: 3341
		private struct EventKey
		{
			// Token: 0x06006EF2 RID: 28402 RVA: 0x0030D94E File Offset: 0x0030BB4E
			public EventKey(Type myType, string myName = null)
			{
				this.eventType = myType;
				this.eventName = myName;
			}

			// Token: 0x06006EF3 RID: 28403 RVA: 0x0030D960 File Offset: 0x0030BB60
			public override int GetHashCode()
			{
				if (string.IsNullOrEmpty(this.eventName))
				{
					return this.eventType.GetHashCode();
				}
				if (this.eventType.Equals(typeof(NamedEvent)))
				{
					return this.eventName.GetHashCode();
				}
				return (17 * 23 + this.eventType.GetHashCode()) * 23 + this.eventName.GetHashCode();
			}

			// Token: 0x0400504E RID: 20558
			public Type eventType;

			// Token: 0x0400504F RID: 20559
			public string eventName;
		}

		// Token: 0x02000D0E RID: 3342
		// (Invoke) Token: 0x06006EF5 RID: 28405
		public delegate void EventDelegate<T>(T e) where T : GameEvent;

		// Token: 0x02000D0F RID: 3343
		// (Invoke) Token: 0x06006EF9 RID: 28409
		private delegate void EventDelegate(GameEvent e);
	}
}
