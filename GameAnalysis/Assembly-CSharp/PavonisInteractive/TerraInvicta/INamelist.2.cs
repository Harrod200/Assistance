using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006FB RID: 1787
	public interface INamelist<in TKey> : INamelist where TKey : INamelistKey<TKey>
	{
		// Token: 0x06002A73 RID: 10867
		string GetName(TKey key);
	}
}
