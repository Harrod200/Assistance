using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A8F RID: 2703
	public class SetAutofailValueForTurnedCouncilorAction : PlayerAction
	{
		// Token: 0x06006573 RID: 25971 RVA: 0x002FCFB8 File Offset: 0x002FB1B8
		public SetAutofailValueForTurnedCouncilorAction(TICouncilorState turnedCouncilor, float value)
		{
			this.councilorID = turnedCouncilor.ID;
			this.value = value;
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x002FCFD3 File Offset: 0x002FB1D3
		public override void Execute()
		{
			this.councilorID.GetState<TICouncilorState>(false).SetAutofailMissionsValue(this.value);
		}

		// Token: 0x040047B6 RID: 18358
		private GameStateID councilorID;

		// Token: 0x040047B7 RID: 18359
		private float value;
	}
}
