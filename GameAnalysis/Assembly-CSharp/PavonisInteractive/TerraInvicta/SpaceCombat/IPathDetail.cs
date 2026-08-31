using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009EB RID: 2539
	public interface IPathDetail
	{
		// Token: 0x0600602B RID: 24619
		bool IsInBurn(TIDateTime time);

		// Token: 0x0600602C RID: 24620
		bool IsAcceleratingRight(TIDateTime time);

		// Token: 0x0600602D RID: 24621
		bool IsAcceleratingLeft(TIDateTime time);

		// Token: 0x0600602E RID: 24622
		bool IsAcceleratingUp(TIDateTime time);

		// Token: 0x0600602F RID: 24623
		bool IsAcceleratingDown(TIDateTime time);

		// Token: 0x06006030 RID: 24624
		bool IsAcceleratingRollRight(TIDateTime time);

		// Token: 0x06006031 RID: 24625
		bool IsAcceleratingRollLeft(TIDateTime time);

		// Token: 0x06006032 RID: 24626
		Vector3 PositionAt(TIDateTime time);

		// Token: 0x06006033 RID: 24627
		Vector3 VelocityAt(TIDateTime time);

		// Token: 0x06006034 RID: 24628
		Vector3 AccelerationAt(TIDateTime time);

		// Token: 0x06006035 RID: 24629
		Vector3 HeadingAt(TIDateTime time);

		// Token: 0x06006036 RID: 24630
		Quaternion RotationAt(TIDateTime time);

		// Token: 0x06006037 RID: 24631
		float AngularVelocityAt_Rad(TIDateTime time);

		// Token: 0x06006038 RID: 24632
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		List<ValueTuple<TIDateTime, bool>> GetBurnTimings();
	}
}
