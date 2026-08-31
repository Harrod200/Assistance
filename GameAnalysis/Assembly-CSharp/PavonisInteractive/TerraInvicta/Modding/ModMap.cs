using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Modding
{
	// Token: 0x02000958 RID: 2392
	public class ModMap
	{
		// Token: 0x17000F5A RID: 3930
		// (get) Token: 0x06005B14 RID: 23316 RVA: 0x002BE11D File Offset: 0x002BC31D
		// (set) Token: 0x06005B15 RID: 23317 RVA: 0x002BE125 File Offset: 0x002BC325
		public string FilePath { get; set; }

		// Token: 0x17000F5B RID: 3931
		// (get) Token: 0x06005B16 RID: 23318 RVA: 0x002BE12E File Offset: 0x002BC32E
		// (set) Token: 0x06005B17 RID: 23319 RVA: 0x002BE136 File Offset: 0x002BC336
		public string Hash { get; set; }

		// Token: 0x17000F5C RID: 3932
		// (get) Token: 0x06005B18 RID: 23320 RVA: 0x002BE13F File Offset: 0x002BC33F
		// (set) Token: 0x06005B19 RID: 23321 RVA: 0x002BE147 File Offset: 0x002BC347
		public bool Valid { get; set; }

		// Token: 0x17000F5D RID: 3933
		// (get) Token: 0x06005B1A RID: 23322 RVA: 0x002BE150 File Offset: 0x002BC350
		// (set) Token: 0x06005B1B RID: 23323 RVA: 0x002BE158 File Offset: 0x002BC358
		public List<string> errorMessages { get; set; } = new List<string>();
	}
}
