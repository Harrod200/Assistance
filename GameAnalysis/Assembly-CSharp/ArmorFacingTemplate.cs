using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003FA RID: 1018
public struct ArmorFacingTemplate
{
	// Token: 0x060014D7 RID: 5335 RVA: 0x000661BC File Offset: 0x000643BC
	public ArmorFacingTemplate(string materialName, int armorValue)
	{
		this.materialName = materialName;
		this.armorValue = armorValue;
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x060014D8 RID: 5336 RVA: 0x000661CC File Offset: 0x000643CC
	public TIShipArmorTemplate materialTemplate
	{
		get
		{
			return TemplateManager.Find<TIShipArmorTemplate>(this.materialName, false);
		}
	}

	// Token: 0x04001279 RID: 4729
	public string materialName;

	// Token: 0x0400127A RID: 4730
	public int armorValue;
}
