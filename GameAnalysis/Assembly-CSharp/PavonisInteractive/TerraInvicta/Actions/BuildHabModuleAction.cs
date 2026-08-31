using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A53 RID: 2643
	public class BuildHabModuleAction : PlayerAction
	{
		// Token: 0x060064F3 RID: 25843 RVA: 0x002FAB3C File Offset: 0x002F8D3C
		public BuildHabModuleAction(TIHabModuleTemplate moduleTemplate, TISectorState sector, int slot, TIResourcesCost cost, Action callback = null)
		{
			this.moduleTemplate = moduleTemplate;
			this.sectorID = sector.ID;
			this.slot = slot;
			this.cost = cost;
			this.callback = callback;
		}

		// Token: 0x060064F4 RID: 25844 RVA: 0x002FAB70 File Offset: 0x002F8D70
		public override void Execute()
		{
			TISectorState state = this.sectorID.GetState<TISectorState>(false);
			state.hab.InitiateModuleConstruction(state, this.slot, this.moduleTemplate, this.cost);
			if (this.callback != null)
			{
				this.callback();
			}
		}

		// Token: 0x04004711 RID: 18193
		private TIHabModuleTemplate moduleTemplate;

		// Token: 0x04004712 RID: 18194
		private GameStateID sectorID;

		// Token: 0x04004713 RID: 18195
		private int slot;

		// Token: 0x04004714 RID: 18196
		private TIResourcesCost cost;

		// Token: 0x04004715 RID: 18197
		private Action callback;
	}
}
