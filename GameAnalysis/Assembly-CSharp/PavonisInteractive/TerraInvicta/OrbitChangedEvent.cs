using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005BD RID: 1469
	public class OrbitChangedEvent : GameEvent
	{
		// Token: 0x060027E2 RID: 10210 RVA: 0x000D9A21 File Offset: 0x000D7C21
		public OrbitChangedEvent(TISpaceObjectState selectedObject)
		{
			this.orbitChangedObject = selectedObject;
		}

		// Token: 0x04001DD6 RID: 7638
		public TISpaceObjectState orbitChangedObject;
	}
}
