using System;
using UnityEngine;

namespace FullSerializer.Internal
{
	// Token: 0x0200048B RID: 1163
	public class fsSerializationCallbackReceiverProcessor : fsObjectProcessor
	{
		// Token: 0x060018F5 RID: 6389 RVA: 0x00080A5C File Offset: 0x0007EC5C
		public override bool CanProcess(Type type)
		{
			return typeof(ISerializationCallbackReceiver).IsAssignableFrom(type);
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x00080A6E File Offset: 0x0007EC6E
		public override void OnBeforeSerialize(Type storageType, object instance)
		{
			if (instance == null)
			{
				return;
			}
			((ISerializationCallbackReceiver)instance).OnBeforeSerialize();
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x00080A7F File Offset: 0x0007EC7F
		public override void OnAfterDeserialize(Type storageType, object instance)
		{
			if (instance == null)
			{
				return;
			}
			((ISerializationCallbackReceiver)instance).OnAfterDeserialize();
		}
	}
}
