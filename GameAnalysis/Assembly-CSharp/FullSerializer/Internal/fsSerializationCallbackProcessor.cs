using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200048A RID: 1162
	public class fsSerializationCallbackProcessor : fsObjectProcessor
	{
		// Token: 0x060018EF RID: 6383 RVA: 0x000809B4 File Offset: 0x0007EBB4
		public override bool CanProcess(Type type)
		{
			return typeof(fsISerializationCallbacks).IsAssignableFrom(type);
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x000809C6 File Offset: 0x0007EBC6
		public override void OnBeforeSerialize(Type storageType, object instance)
		{
			if (instance == null)
			{
				return;
			}
			((fsISerializationCallbacks)instance).OnBeforeSerialize(storageType);
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x000809D8 File Offset: 0x0007EBD8
		public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
		{
			if (instance == null)
			{
				return;
			}
			((fsISerializationCallbacks)instance).OnAfterSerialize(storageType, ref data);
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x000809EC File Offset: 0x0007EBEC
		public override void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data)
		{
			if (!(instance is fsISerializationCallbacks))
			{
				string text = "Please ensure the converter for ";
				string text2 = ((storageType != null) ? storageType.ToString() : null);
				string text3 = " actually returns an instance of it, not an instance of ";
				Type type = instance.GetType();
				throw new InvalidCastException(text + text2 + text3 + ((type != null) ? type.ToString() : null));
			}
			((fsISerializationCallbacks)instance).OnBeforeDeserialize(storageType, ref data);
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x00080A42 File Offset: 0x0007EC42
		public override void OnAfterDeserialize(Type storageType, object instance)
		{
			if (instance == null)
			{
				return;
			}
			((fsISerializationCallbacks)instance).OnAfterDeserialize(storageType);
		}
	}
}
