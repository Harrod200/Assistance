using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000702 RID: 1794
	public struct CouncilorName : INamelistKey<CouncilorName>, INamelistKey, IEquatable<CouncilorName>
	{
		// Token: 0x06002A7E RID: 10878 RVA: 0x000E6A84 File Offset: 0x000E4C84
		public CouncilorName(string group, string segment, string gender)
		{
			this.group = group;
			this.gender = gender;
			this.segment = segment;
		}

		// Token: 0x06002A7F RID: 10879 RVA: 0x000E6A9B File Offset: 0x000E4C9B
		public bool Equals(CouncilorName key)
		{
			return this.group == key.group && this.gender == key.gender && this.segment == key.segment;
		}

		// Token: 0x06002A80 RID: 10880 RVA: 0x000E6AD6 File Offset: 0x000E4CD6
		public CouncilorName Any()
		{
			return new CouncilorName("any", this.segment, this.gender);
		}

		// Token: 0x040020A4 RID: 8356
		public string group;

		// Token: 0x040020A5 RID: 8357
		public string gender;

		// Token: 0x040020A6 RID: 8358
		public string segment;
	}
}
