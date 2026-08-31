using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Modding
{
	// Token: 0x0200095A RID: 2394
	public class ConflictingMod
	{
		// Token: 0x17000F5E RID: 3934
		// (get) Token: 0x06005B28 RID: 23336 RVA: 0x002BE7AC File Offset: 0x002BC9AC
		// (set) Token: 0x06005B29 RID: 23337 RVA: 0x002BE7B4 File Offset: 0x002BC9B4
		public object Mod { get; set; }

		// Token: 0x17000F5F RID: 3935
		// (get) Token: 0x06005B2A RID: 23338 RVA: 0x002BE7BD File Offset: 0x002BC9BD
		// (set) Token: 0x06005B2B RID: 23339 RVA: 0x002BE7C5 File Offset: 0x002BC9C5
		public List<string> Description { get; set; } = new List<string>();
	}
}
