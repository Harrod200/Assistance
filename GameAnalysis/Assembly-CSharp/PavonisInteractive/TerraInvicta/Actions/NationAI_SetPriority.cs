using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AAB RID: 2731
	public class NationAI_SetPriority : SimulationAction
	{
		// Token: 0x060065B1 RID: 26033 RVA: 0x002FDB5F File Offset: 0x002FBD5F
		public NationAI_SetPriority(TIControlPoint controlPoint, PriorityType priority, int value, bool onlyIfHigher)
		{
			this.controlPointID = controlPoint.ID;
			this.priority = priority;
			this.onlyIfHigher = onlyIfHigher;
			this.value = value;
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x002FDB8C File Offset: 0x002FBD8C
		public override void Execute()
		{
			TIControlPoint state = this.controlPointID.GetState<TIControlPoint>(false);
			if (state.faction == null)
			{
				if (this.onlyIfHigher)
				{
					int controlPointPriority = state.GetControlPointPriority(this.priority, false);
					this.value = ((this.value > controlPointPriority) ? this.value : controlPointPriority);
				}
				state.SetControlPointPriority(this.priority, this.value, false, false, false);
			}
		}

		// Token: 0x04004800 RID: 18432
		private GameStateID controlPointID;

		// Token: 0x04004801 RID: 18433
		private PriorityType priority;

		// Token: 0x04004802 RID: 18434
		private bool onlyIfHigher;

		// Token: 0x04004803 RID: 18435
		private int value;
	}
}
