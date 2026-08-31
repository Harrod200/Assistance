using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003FD RID: 1021
public class ModuleDataEntry
{
	// Token: 0x17000318 RID: 792
	// (get) Token: 0x060014DC RID: 5340 RVA: 0x0006620D File Offset: 0x0006440D
	// (set) Token: 0x060014DD RID: 5341 RVA: 0x00066215 File Offset: 0x00064415
	public string moduleTemplateName { get; private set; }

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x060014DE RID: 5342 RVA: 0x0006621E File Offset: 0x0006441E
	// (set) Token: 0x060014DF RID: 5343 RVA: 0x00066226 File Offset: 0x00064426
	public int slotIndex { get; private set; }

	// Token: 0x060014E0 RID: 5344 RVA: 0x0006622F File Offset: 0x0006442F
	public ModuleDataEntry(TIShipPartTemplate moduleTemplate, int slotIndex)
	{
		this.moduleTemplateName = moduleTemplate.dataName;
		this.cachedModuleTemplate = moduleTemplate;
		this.slotIndex = slotIndex;
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x00066251 File Offset: 0x00064451
	public ModuleDataEntry()
	{
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x00066259 File Offset: 0x00064459
	public void CorrectBrokenSlot(int correctIndex)
	{
		this.slotIndex = correctIndex;
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x00066264 File Offset: 0x00064464
	public override bool Equals(object obj)
	{
		ModuleDataEntry moduleDataEntry = (ModuleDataEntry)obj;
		return moduleDataEntry != null && this.moduleTemplateName == moduleDataEntry.moduleTemplateName && this.slotIndex == moduleDataEntry.slotIndex;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x000662A0 File Offset: 0x000644A0
	public override int GetHashCode()
	{
		return new ValueTuple<string, int>(this.moduleTemplateName, this.slotIndex).GetHashCode();
	}

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x060014E5 RID: 5349 RVA: 0x000662CC File Offset: 0x000644CC
	public TIShipPartTemplate moduleTemplate
	{
		get
		{
			if (this.cachedModuleTemplate == null)
			{
				this.cachedModuleTemplate = TemplateManager.Find<TIShipPartTemplate>(this.moduleTemplateName, true);
			}
			return this.cachedModuleTemplate;
		}
	}

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x060014E6 RID: 5350 RVA: 0x000662EE File Offset: 0x000644EE
	public TIShipWeaponTemplate weaponTemplate
	{
		get
		{
			return this.moduleTemplate.ref_weapon;
		}
	}

	// Token: 0x04001281 RID: 4737
	private TIShipPartTemplate cachedModuleTemplate;
}
