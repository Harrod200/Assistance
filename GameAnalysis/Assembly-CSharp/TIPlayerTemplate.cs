using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000401 RID: 1025
public class TIPlayerTemplate : TIDataTemplate
{
	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001507 RID: 5383 RVA: 0x000668AD File Offset: 0x00064AAD
	public string name
	{
		get
		{
			return base.dataName;
		}
	}

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001508 RID: 5384 RVA: 0x000668B5 File Offset: 0x00064AB5
	public new string displayName
	{
		get
		{
			return base.dataName;
		}
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000668BD File Offset: 0x00064ABD
	public TIPlayerTemplate(string name, string council)
		: base(name)
	{
		this.council = council;
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x000668D0 File Offset: 0x00064AD0
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIPlayerState>();
		}
		return tigameState;
	}

	// Token: 0x0400129A RID: 4762
	public string council;
}
