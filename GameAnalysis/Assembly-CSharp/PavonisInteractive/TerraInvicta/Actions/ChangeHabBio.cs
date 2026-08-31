using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A57 RID: 2647
	public class ChangeHabBio : PlayerAction
	{
		// Token: 0x060064FB RID: 25851 RVA: 0x002FACED File Offset: 0x002F8EED
		public ChangeHabBio(TIHabState hab, string name, string customIcon = null)
		{
			this.habID = hab.ID;
			this.name = name;
			this.customIcon = customIcon;
		}

		// Token: 0x060064FC RID: 25852 RVA: 0x002FAD10 File Offset: 0x002F8F10
		public override void Execute()
		{
			TIHabState state = this.habID.GetState<TIHabState>(false);
			state.SetDisplayName(this.name);
			if (!string.IsNullOrEmpty(this.customIcon))
			{
				state.SetCustomIconString(this.customIcon);
				return;
			}
			state.SetCustomIconString(string.Empty);
		}

		// Token: 0x04004722 RID: 18210
		private GameStateID habID;

		// Token: 0x04004723 RID: 18211
		private string name;

		// Token: 0x04004724 RID: 18212
		private string customIcon;
	}
}
