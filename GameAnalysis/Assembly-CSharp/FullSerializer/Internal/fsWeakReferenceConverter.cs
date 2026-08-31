using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000480 RID: 1152
	public class fsWeakReferenceConverter : fsConverter
	{
		// Token: 0x06001899 RID: 6297 RVA: 0x0007F96B File Offset: 0x0007DB6B
		public override bool CanProcess(Type type)
		{
			return type == typeof(WeakReference);
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x0007F97D File Offset: 0x0007DB7D
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x0007F980 File Offset: 0x0007DB80
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x0007F984 File Offset: 0x0007DB84
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			WeakReference weakReference = (WeakReference)instance;
			fsResult fsResult = fsResult.Success;
			serialized = fsData.CreateDictionary();
			if (weakReference.IsAlive)
			{
				fsData fsData;
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + this.Serializer.TrySerialize<object>(weakReference.Target, out fsData));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				serialized.AsDictionary["Target"] = fsData;
				serialized.AsDictionary["TrackResurrection"] = new fsData(weakReference.TrackResurrection);
			}
			return fsResult;
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x0007FA04 File Offset: 0x0007DC04
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Object));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			if (data.AsDictionary.ContainsKey("Target"))
			{
				fsData fsData = data.AsDictionary["Target"];
				object obj = null;
				fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(fsData, typeof(object), ref obj));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				bool flag = false;
				if (data.AsDictionary.ContainsKey("TrackResurrection") && data.AsDictionary["TrackResurrection"].IsBool)
				{
					flag = data.AsDictionary["TrackResurrection"].AsBool;
				}
				instance = new WeakReference(obj, flag);
			}
			return fsResult;
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x0007FAD5 File Offset: 0x0007DCD5
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new WeakReference(null);
		}
	}
}
