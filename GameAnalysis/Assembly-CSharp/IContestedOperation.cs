using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200033C RID: 828
public interface IContestedOperation
{
	// Token: 0x06000E24 RID: 3620
	float GetSuccessChance(TIGameState actor, TIGameState defender);
}
