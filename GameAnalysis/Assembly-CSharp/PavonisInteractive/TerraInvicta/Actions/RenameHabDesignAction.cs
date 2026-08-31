using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A7D RID: 2685
	public class RenameHabDesignAction : PlayerAction
	{
		// Token: 0x0600654E RID: 25934 RVA: 0x002FC328 File Offset: 0x002FA528
		public RenameHabDesignAction(TIHabTemplate habDesign, string newName)
		{
			this.habDesign = habDesign;
			this.newName = newName;
		}

		// Token: 0x0600654F RID: 25935 RVA: 0x002FC33E File Offset: 0x002FA53E
		public override void Execute()
		{
			this.habDesign.SetDisplayName(this.newName);
			GameControl.eventManager.TriggerEvent(new HabDesignTemplateModified(), null, Array.Empty<object>());
		}

		// Token: 0x0400477A RID: 18298
		public TIHabTemplate habDesign;

		// Token: 0x0400477B RID: 18299
		public string newName;
	}
}
