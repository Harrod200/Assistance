using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003F0 RID: 1008
public class TISpaceCombatTemplate : TIDataTemplate
{
	// Token: 0x060013EA RID: 5098 RVA: 0x0005D900 File Offset: 0x0005BB00
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TISpaceCombatState>();
		}
		return tigameState;
	}
}
