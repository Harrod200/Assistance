using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A54 RID: 2644
	public class ChangeArmyBio : PlayerAction
	{
		// Token: 0x060064F5 RID: 25845 RVA: 0x002FABBB File Offset: 0x002F8DBB
		public ChangeArmyBio(TIArmyState army, string name, string nameWithArticle)
		{
			this.armyID = army.ID;
			this.name = name;
			this.nameWithArticle = nameWithArticle;
			this.isMegafauna = army.AlienMegafaunaArmy;
		}

		// Token: 0x060064F6 RID: 25846 RVA: 0x002FABEC File Offset: 0x002F8DEC
		public override void Execute()
		{
			TIArmyState tiarmyState;
			if (this.isMegafauna)
			{
				tiarmyState = this.armyID.GetState<TIMegafaunaArmyState>(false);
			}
			else
			{
				tiarmyState = this.armyID.GetState<TIArmyState>(false);
			}
			if (tiarmyState != null)
			{
				tiarmyState.Rename(this.name, this.nameWithArticle);
			}
		}

		// Token: 0x04004716 RID: 18198
		private GameStateID armyID;

		// Token: 0x04004717 RID: 18199
		private string name;

		// Token: 0x04004718 RID: 18200
		private string nameWithArticle;

		// Token: 0x04004719 RID: 18201
		private bool isMegafauna;
	}
}
