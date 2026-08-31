using System;

namespace FullSerializer
{
	// Token: 0x0200045A RID: 1114
	public interface fsIAotConverter
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001788 RID: 6024
		Type ModelType { get; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001789 RID: 6025
		fsAotVersionInfo VersionInfo { get; }
	}
}
