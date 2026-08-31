using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000170 RID: 368
public struct CouncilorMessage
{
	// Token: 0x06000553 RID: 1363 RVA: 0x000179FE File Offset: 0x00015BFE
	public CouncilorMessage(TIGameState speaker, string message)
	{
		this.speaker = speaker;
		this.message = message;
	}

	// Token: 0x040004B8 RID: 1208
	public TIGameState speaker;

	// Token: 0x040004B9 RID: 1209
	public string message;
}
