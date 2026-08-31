using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006FD RID: 1789
	public struct NamelistEntry
	{
		// Token: 0x06002A77 RID: 10871 RVA: 0x000E69F0 File Offset: 0x000E4BF0
		public NamelistEntry(string name, int weight = 1)
		{
			this.name = name;
			this.weight = weight;
		}

		// Token: 0x040020A2 RID: 8354
		public readonly int weight;

		// Token: 0x040020A3 RID: 8355
		public readonly string name;
	}
}
