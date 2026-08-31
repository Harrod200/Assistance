using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C4 RID: 1988
	public struct StratManeuver
	{
		// Token: 0x04002921 RID: 10529
		public TIDateTime maneuverStart;

		// Token: 0x04002922 RID: 10530
		public TIDateTime maneuverFinish;

		// Token: 0x04002923 RID: 10531
		public Quaternion startingOrientation;

		// Token: 0x04002924 RID: 10532
		public Quaternion desiredOrientation;

		// Token: 0x04002925 RID: 10533
		public Vector3d startingOffset;

		// Token: 0x04002926 RID: 10534
		public Vector3d desiredOffset;
	}
}
