using System;
using System.Collections;
using System.Collections.Generic;

namespace FullSerializer.Internal
{
	// Token: 0x02000474 RID: 1140
	public class fsArrayConverter : fsConverter
	{
		// Token: 0x06001845 RID: 6213 RVA: 0x0007DFDF File Offset: 0x0007C1DF
		public override bool CanProcess(Type type)
		{
			return type.IsArray;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0007DFE7 File Offset: 0x0007C1E7
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x0007DFEA File Offset: 0x0007C1EA
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x0007DFF0 File Offset: 0x0007C1F0
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			IList list = (Array)instance;
			Type elementType = storageType.GetElementType();
			fsResult success = fsResult.Success;
			serialized = fsData.CreateList(list.Count);
			List<fsData> asList = serialized.AsList;
			for (int i = 0; i < list.Count; i++)
			{
				object obj = list[i];
				fsData fsData;
				fsResult fsResult = this.Serializer.TrySerialize(elementType, obj, out fsData);
				success.AddMessages(fsResult);
				if (!fsResult.Failed)
				{
					asList.Add(fsData);
				}
			}
			return success;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0007E074 File Offset: 0x0007C274
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Array));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			Type elementType = storageType.GetElementType();
			List<fsData> asList = data.AsList;
			ArrayList arrayList = new ArrayList(asList.Count);
			int count = arrayList.Count;
			for (int i = 0; i < asList.Count; i++)
			{
				fsData fsData = asList[i];
				object obj = null;
				if (i < count)
				{
					obj = arrayList[i];
				}
				fsResult fsResult3 = this.Serializer.TryDeserialize(fsData, elementType, ref obj);
				fsResult.AddMessages(fsResult3);
				if (!fsResult3.Failed)
				{
					if (i < count)
					{
						arrayList[i] = obj;
					}
					else
					{
						arrayList.Add(obj);
					}
				}
			}
			instance = arrayList.ToArray(elementType);
			return fsResult;
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x0007E141 File Offset: 0x0007C341
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}
	}
}
