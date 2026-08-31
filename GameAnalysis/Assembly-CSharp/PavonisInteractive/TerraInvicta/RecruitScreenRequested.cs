using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005DA RID: 1498
	public class RecruitScreenRequested : GameEvent
	{
		// Token: 0x060027FF RID: 10239 RVA: 0x000D9C05 File Offset: 0x000D7E05
		public RecruitScreenRequested(TIFactionState council, GameObject callingObject)
		{
			this.council = council;
			this.callingObject = callingObject;
		}

		// Token: 0x04001DFA RID: 7674
		public TIFactionState council;

		// Token: 0x04001DFB RID: 7675
		public GameObject callingObject;
	}
}
