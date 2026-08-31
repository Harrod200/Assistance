using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class CoroutineDummy : MonoBehaviour
{
	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600010B RID: 267 RVA: 0x00008C8A File Offset: 0x00006E8A
	public static CoroutineDummy Singleton
	{
		get
		{
			if (CoroutineDummy.singleton == null)
			{
				CoroutineDummy.singleton = new GameObject("Coroutine Dummy").AddComponent<CoroutineDummy>();
			}
			return CoroutineDummy.singleton;
		}
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00008CB2 File Offset: 0x00006EB2
	public void StopAll()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00008CBA File Offset: 0x00006EBA
	public void PauseAll()
	{
		this.pauseAll = true;
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00008CC3 File Offset: 0x00006EC3
	public void UnpauseAll()
	{
		this.pauseAll = false;
	}

	// Token: 0x0400010A RID: 266
	private static CoroutineDummy singleton;

	// Token: 0x0400010B RID: 267
	public bool pauseAll;
}
