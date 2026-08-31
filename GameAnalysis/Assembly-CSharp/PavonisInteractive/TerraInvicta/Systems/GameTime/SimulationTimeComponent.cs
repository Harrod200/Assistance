using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.GameTime
{
	// Token: 0x020009AD RID: 2477
	public class SimulationTimeComponent : MonoBehaviour
	{
		// Token: 0x17001003 RID: 4099
		// (get) Token: 0x06005D72 RID: 23922 RVA: 0x002C8614 File Offset: 0x002C6814
		public TIDateTime now
		{
			get
			{
				return this.state.Time_Now();
			}
		}

		// Token: 0x040042D4 RID: 17108
		public TITimeState state;
	}
}
