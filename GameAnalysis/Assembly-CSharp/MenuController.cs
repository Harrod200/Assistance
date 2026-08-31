using System;
using UnityEngine;

// Token: 0x02000434 RID: 1076
[RequireComponent(typeof(Menu))]
public abstract class MenuController : MonoBehaviour
{
	// Token: 0x17000334 RID: 820
	// (get) Token: 0x0600164A RID: 5706 RVA: 0x00071C0D File Offset: 0x0006FE0D
	public Menu menu
	{
		get
		{
			return base.GetComponent<Menu>();
		}
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00071C15 File Offset: 0x0006FE15
	public virtual void OnOpen()
	{
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x00071C17 File Offset: 0x0006FE17
	public virtual void OnClose()
	{
	}
}
