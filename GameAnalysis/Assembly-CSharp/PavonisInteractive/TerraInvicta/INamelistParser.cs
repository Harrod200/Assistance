using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006FE RID: 1790
	public interface INamelistParser<out TKey> where TKey : INamelistKey
	{
		// Token: 0x06002A78 RID: 10872
		TKey ParseKey(string[] values);

		// Token: 0x06002A79 RID: 10873
		NamelistEntry ParseEntry(string[] values);
	}
}
