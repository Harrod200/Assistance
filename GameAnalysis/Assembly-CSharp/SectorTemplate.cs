using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200036E RID: 878
public struct SectorTemplate
{
	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06000FDF RID: 4063 RVA: 0x000526EC File Offset: 0x000508EC
	public TIHabModuleTemplate[] habModules
	{
		get
		{
			TIHabModuleTemplate[] array = new TIHabModuleTemplate[this.habModuleNames.Length];
			for (int i = 0; i < this.habModuleNames.Length; i++)
			{
				if (string.IsNullOrEmpty(this.habModuleNames[i]))
				{
					array[i] = null;
				}
				else
				{
					array[i] = TemplateManager.Find<TIHabModuleTemplate>(this.habModuleNames[i], false);
				}
			}
			return array;
		}
	}

	// Token: 0x0400101E RID: 4126
	public string faction;

	// Token: 0x0400101F RID: 4127
	public string[] habModuleNames;
}
