using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000624 RID: 1572
	public class ArmyTakesDamage : GameEvent
	{
		// Token: 0x06002849 RID: 10313 RVA: 0x000DA12A File Offset: 0x000D832A
		public ArmyTakesDamage(TIArmyState state)
		{
			this.state = state;
		}

		// Token: 0x04001E5C RID: 7772
		public TIArmyState state;
	}
}
