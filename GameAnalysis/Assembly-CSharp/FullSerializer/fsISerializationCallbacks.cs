using System;

namespace FullSerializer
{
	// Token: 0x0200046A RID: 1130
	public interface fsISerializationCallbacks
	{
		// Token: 0x060017DF RID: 6111
		void OnBeforeSerialize(Type storageType);

		// Token: 0x060017E0 RID: 6112
		void OnAfterSerialize(Type storageType, ref fsData data);

		// Token: 0x060017E1 RID: 6113
		void OnBeforeDeserialize(Type storageType, ref fsData data);

		// Token: 0x060017E2 RID: 6114
		void OnAfterDeserialize(Type storageType);
	}
}
