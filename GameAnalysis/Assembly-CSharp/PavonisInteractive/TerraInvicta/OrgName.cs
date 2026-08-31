using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000706 RID: 1798
	public struct OrgName : INamelistKey<OrgName>, INamelistKey, IEquatable<OrgName>
	{
		// Token: 0x06002A8A RID: 10890 RVA: 0x000E6C08 File Offset: 0x000E4E08
		public OrgName(OrgType orgType, string segment)
		{
			this.orgType = orgType;
			this.segment = segment;
		}

		// Token: 0x06002A8B RID: 10891 RVA: 0x000E6C18 File Offset: 0x000E4E18
		public bool Equals(OrgName key)
		{
			return this.orgType == key.orgType && this.segment == key.segment;
		}

		// Token: 0x06002A8C RID: 10892 RVA: 0x000E6C3B File Offset: 0x000E4E3B
		public OrgName Any()
		{
			return new OrgName(OrgType.Any, this.segment);
		}

		// Token: 0x040020A9 RID: 8361
		private readonly OrgType orgType;

		// Token: 0x040020AA RID: 8362
		private readonly string segment;
	}
}
