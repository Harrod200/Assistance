using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E7 RID: 2535
	public interface IWaypoint
	{
		// Token: 0x17001095 RID: 4245
		// (get) Token: 0x06006018 RID: 24600
		// (set) Token: 0x06006019 RID: 24601
		float AlphaBlendValue { get; set; }

		// Token: 0x17001096 RID: 4246
		// (get) Token: 0x0600601A RID: 24602
		// (set) Token: 0x0600601B RID: 24603
		Vector3 Position { get; set; }

		// Token: 0x17001097 RID: 4247
		// (get) Token: 0x0600601C RID: 24604
		// (set) Token: 0x0600601D RID: 24605
		Vector3 Velocity { get; set; }

		// Token: 0x17001098 RID: 4248
		// (get) Token: 0x0600601E RID: 24606
		// (set) Token: 0x0600601F RID: 24607
		Vector3 Heading { get; set; }

		// Token: 0x06006020 RID: 24608
		void SetHeading(Vector3 direction, bool preserveRoll);

		// Token: 0x17001099 RID: 4249
		// (get) Token: 0x06006021 RID: 24609
		// (set) Token: 0x06006022 RID: 24610
		Quaternion Rotation { get; set; }

		// Token: 0x1700109A RID: 4250
		// (get) Token: 0x06006023 RID: 24611
		// (set) Token: 0x06006024 RID: 24612
		TIDateTime Timing { get; set; }
	}
}
