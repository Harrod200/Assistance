using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000478 RID: 1144
	public class fsForwardConverter : fsConverter
	{
		// Token: 0x06001860 RID: 6240 RVA: 0x0007EAE6 File Offset: 0x0007CCE6
		public fsForwardConverter(fsForwardAttribute attribute)
		{
			this._memberName = attribute.MemberName;
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0007EAFA File Offset: 0x0007CCFA
		public override bool CanProcess(Type type)
		{
			throw new NotSupportedException("Please use the [fsForward(...)] attribute.");
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x0007EB08 File Offset: 0x0007CD08
		private fsResult GetProperty(object instance, out fsMetaProperty property)
		{
			fsMetaProperty[] properties = fsMetaType.Get(this.Serializer.Config, instance.GetType()).Properties;
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].MemberName == this._memberName)
				{
					property = properties[i];
					return fsResult.Success;
				}
			}
			property = null;
			return fsResult.Fail("No property named \"" + this._memberName + "\" on " + instance.GetType().CSharpName());
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x0007EB88 File Offset: 0x0007CD88
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.Null;
			fsResult fsResult = fsResult.Success;
			fsMetaProperty fsMetaProperty;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + this.GetProperty(instance, out fsMetaProperty));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			object obj = fsMetaProperty.Read(instance);
			return this.Serializer.TrySerialize(fsMetaProperty.StorageType, obj, out serialized);
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x0007EBDC File Offset: 0x0007CDDC
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsMetaProperty fsMetaProperty;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + this.GetProperty(instance, out fsMetaProperty));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			object obj = null;
			fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(data, fsMetaProperty.StorageType, ref obj));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			fsMetaProperty.Write(instance, obj);
			return fsResult;
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x0007EC41 File Offset: 0x0007CE41
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}

		// Token: 0x04001615 RID: 5653
		private string _memberName;
	}
}
