using System;
using System.Collections;

namespace FullSerializer.Internal
{
	// Token: 0x0200047E RID: 1150
	public class fsReflectedConverter : fsConverter
	{
		// Token: 0x0600188D RID: 6285 RVA: 0x0007F708 File Offset: 0x0007D908
		public override bool CanProcess(Type type)
		{
			return !type.Resolve().IsArray && !typeof(ICollection).IsAssignableFrom(type);
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x0007F72C File Offset: 0x0007D92C
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.CreateDictionary();
			fsResult success = fsResult.Success;
			fsMetaType fsMetaType = fsMetaType.Get(this.Serializer.Config, instance.GetType());
			fsMetaType.EmitAotData(false);
			for (int i = 0; i < fsMetaType.Properties.Length; i++)
			{
				fsMetaProperty fsMetaProperty = fsMetaType.Properties[i];
				if (fsMetaProperty.CanRead)
				{
					fsData fsData;
					fsResult fsResult = this.Serializer.TrySerialize(fsMetaProperty.StorageType, fsMetaProperty.OverrideConverterType, fsMetaProperty.Read(instance), out fsData);
					success.AddMessages(fsResult);
					if (!fsResult.Failed)
					{
						serialized.AsDictionary[fsMetaProperty.JsonName] = fsData;
					}
				}
			}
			return success;
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0007F7D4 File Offset: 0x0007D9D4
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Object));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			fsMetaType fsMetaType = fsMetaType.Get(this.Serializer.Config, storageType);
			fsMetaType.EmitAotData(false);
			for (int i = 0; i < fsMetaType.Properties.Length; i++)
			{
				fsMetaProperty fsMetaProperty = fsMetaType.Properties[i];
				fsData fsData;
				if (fsMetaProperty.CanWrite && data.AsDictionary.TryGetValue(fsMetaProperty.JsonName, out fsData))
				{
					object obj = null;
					if (fsMetaProperty.CanRead)
					{
						obj = fsMetaProperty.Read(instance);
					}
					fsResult fsResult3 = this.Serializer.TryDeserialize(fsData, fsMetaProperty.StorageType, fsMetaProperty.OverrideConverterType, ref obj);
					fsResult.AddMessages(fsResult3);
					if (!fsResult3.Failed)
					{
						fsMetaProperty.Write(instance, obj);
					}
				}
			}
			return fsResult;
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0007F8B2 File Offset: 0x0007DAB2
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}
	}
}
