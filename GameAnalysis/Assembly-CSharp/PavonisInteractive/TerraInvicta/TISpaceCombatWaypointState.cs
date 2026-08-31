using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B9 RID: 1977
	[Serializable]
	public class TISpaceCombatWaypointState : TIGameState
	{
		// Token: 0x06004351 RID: 17233 RVA: 0x001B493C File Offset: 0x001B2B3C
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			base.PostGameStateCreateInit_OnCreationOnly_1();
		}

		// Token: 0x04002841 RID: 10305
		[SerializeField]
		public Vector3 position;

		// Token: 0x04002842 RID: 10306
		[SerializeField]
		public Vector3 velocityVector;

		// Token: 0x04002843 RID: 10307
		[SerializeField]
		public TISpaceShipState ship;

		// Token: 0x04002844 RID: 10308
		[SerializeField]
		public string name;

		// Token: 0x04002845 RID: 10309
		[SerializeField]
		public TIDateTime eta;

		// Token: 0x04002846 RID: 10310
		[SerializeField]
		public bool locked;

		// Token: 0x04002847 RID: 10311
		[SerializeField]
		public bool enabled;
	}
}
