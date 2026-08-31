using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E8 RID: 1768
	public interface IEventManager
	{
		// Token: 0x0600292B RID: 10539
		void AddListener<T>(EventManager.EventDelegate<T> del, string eventName = null, object preFilterObject = null, bool queueable = true, bool callOnce = false) where T : GameEvent;

		// Token: 0x0600292C RID: 10540
		void RemoveListener<T>(EventManager.EventDelegate<T> del, string eventName = null) where T : GameEvent;

		// Token: 0x0600292D RID: 10541
		void TriggerEvent(GameEvent evt, string eventName = null, params object[] sourceObjects);
	}
}
