using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A98 RID: 2712
	public class SetPlanetTagAction : PlayerAction
	{
		// Token: 0x06006585 RID: 25989 RVA: 0x002FD24D File Offset: 0x002FB44D
		public SetPlanetTagAction(TISpaceBodyState body, PlayerTag tag)
		{
			this.spaceBodyID = body.ID;
			this.tag = tag;
		}

		// Token: 0x06006586 RID: 25990 RVA: 0x002FD268 File Offset: 0x002FB468
		public override void Execute()
		{
			this.spaceBodyID.GetState<TISpaceBodyState>(false).ChangePlayerTag(this.tag);
		}

		// Token: 0x040047CD RID: 18381
		private GameStateID spaceBodyID;

		// Token: 0x040047CE RID: 18382
		private PlayerTag tag;
	}
}
