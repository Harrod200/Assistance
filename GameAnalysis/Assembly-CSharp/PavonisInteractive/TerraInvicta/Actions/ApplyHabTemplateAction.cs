using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A4B RID: 2635
	public class ApplyHabTemplateAction : PlayerAction
	{
		// Token: 0x060064E3 RID: 25827 RVA: 0x002FA3DB File Offset: 0x002F85DB
		public ApplyHabTemplateAction(TIHabState targetHab, TIHabTemplate habDesign, bool replaceExisting)
		{
			this.habID = targetHab.ID;
			this.habDesign = habDesign;
			this.replaceExisting = replaceExisting;
		}

		// Token: 0x060064E4 RID: 25828 RVA: 0x002FA400 File Offset: 0x002F8600
		public override void Execute()
		{
			TIResourcesCost tiresourcesCost;
			float num;
			List<TIHabModuleTemplate> list;
			this.habID.GetState<TIHabState>(false).ApplySavedTemplate(this.habDesign, false, this.replaceExisting, out tiresourcesCost, out num, out list);
		}

		// Token: 0x040046FC RID: 18172
		public GameStateID habID;

		// Token: 0x040046FD RID: 18173
		public TIHabTemplate habDesign;

		// Token: 0x040046FE RID: 18174
		public bool replaceExisting;
	}
}
