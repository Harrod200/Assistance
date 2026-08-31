using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003F1 RID: 1009
public class TISpaceAssetTemplate : TISpaceObjectTemplate
{
	// Token: 0x170002AC RID: 684
	// (get) Token: 0x060013EC RID: 5100 RVA: 0x0005D92C File Offset: 0x0005BB2C
	public TIOrbitState orbit
	{
		get
		{
			return GameStateManager.FindByTemplate<TIOrbitState>(this.orbitTemplateName, false);
		}
	}

	// Token: 0x040011FA RID: 4602
	public string orbitTemplateName;
}
