using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200063D RID: 1597
	public class FireMissionOrder : GameEvent
	{
		// Token: 0x06002862 RID: 10338 RVA: 0x000DA304 File Offset: 0x000D8504
		public FireMissionOrder(TISpaceShipState ship, TIGameState target, ModuleDataEntry moduleData, Vector3 targetDisplayPosition, float targetLongitude, float targetLatitude, Transform parentSpaceBody, TIDateTime time, bool doNotVisualize = false)
		{
			this.ship = ship;
			this.target = target;
			this.moduleData = new ModuleDataEntry(moduleData.moduleTemplate, moduleData.slotIndex);
			this.targetDisplayPosition = targetDisplayPosition;
			this.targetLongitude = targetLongitude;
			this.targetLatitude = targetLatitude;
			this.parentSpaceBody = parentSpaceBody;
			this.time = time;
			this.doNotVisualize = doNotVisualize;
		}

		// Token: 0x04001E83 RID: 7811
		public TISpaceShipState ship;

		// Token: 0x04001E84 RID: 7812
		public TIGameState target;

		// Token: 0x04001E85 RID: 7813
		public ModuleDataEntry moduleData;

		// Token: 0x04001E86 RID: 7814
		public Vector3 targetDisplayPosition;

		// Token: 0x04001E87 RID: 7815
		public float targetLongitude;

		// Token: 0x04001E88 RID: 7816
		public float targetLatitude;

		// Token: 0x04001E89 RID: 7817
		public Transform parentSpaceBody;

		// Token: 0x04001E8A RID: 7818
		public SphereCollider parentCollider;

		// Token: 0x04001E8B RID: 7819
		public TIDateTime time;

		// Token: 0x04001E8C RID: 7820
		public bool doNotVisualize;
	}
}
