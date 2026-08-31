using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000662 RID: 1634
	public class ObjectSelectedEvent : GameEvent
	{
		// Token: 0x06002889 RID: 10377 RVA: 0x000DA60C File Offset: 0x000D880C
		public ObjectSelectedEvent(GameObject selectedObject)
		{
			this.selectedObject = selectedObject;
		}

		// Token: 0x04001EC0 RID: 7872
		public GameObject selectedObject;
	}
}
