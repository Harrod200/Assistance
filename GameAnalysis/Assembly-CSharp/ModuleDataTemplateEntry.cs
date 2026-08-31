using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003FC RID: 1020
public struct ModuleDataTemplateEntry
{
	// Token: 0x17000317 RID: 791
	// (get) Token: 0x060014DA RID: 5338 RVA: 0x000661EA File Offset: 0x000643EA
	public TIShipPartTemplate moduleTemplate
	{
		get
		{
			return TemplateManager.Find<TIShipPartTemplate>(this.moduleName, true);
		}
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x000661F8 File Offset: 0x000643F8
	public ModuleDataTemplateEntry(TIShipPartTemplate part, int slot)
	{
		this.moduleName = part.dataName;
		this.slot = slot;
	}

	// Token: 0x0400127D RID: 4733
	public string moduleName;

	// Token: 0x0400127E RID: 4734
	public int slot;
}
