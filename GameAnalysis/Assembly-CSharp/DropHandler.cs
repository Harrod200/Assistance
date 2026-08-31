using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200043C RID: 1084
public class DropHandler : MonoBehaviour, IHasChanged, IEventSystemHandler
{
	// Token: 0x06001674 RID: 5748 RVA: 0x00072CEC File Offset: 0x00070EEC
	private void Start()
	{
		this.HasChanged();
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x00072CF4 File Offset: 0x00070EF4
	public void HasChanged()
	{
	}

	// Token: 0x040014D4 RID: 5332
	[SerializeField]
	private Transform DragDestination;
}
