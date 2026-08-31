using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200047F RID: 1151
	public class fsTypeConverter : fsConverter
	{
		// Token: 0x06001892 RID: 6290 RVA: 0x0007F8D2 File Offset: 0x0007DAD2
		public override bool CanProcess(Type type)
		{
			return typeof(Type).IsAssignableFrom(type);
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0007F8E4 File Offset: 0x0007DAE4
		public override bool RequestCycleSupport(Type type)
		{
			return false;
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0007F8E7 File Offset: 0x0007DAE7
		public override bool RequestInheritanceSupport(Type type)
		{
			return false;
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0007F8EC File Offset: 0x0007DAEC
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Type type = (Type)instance;
			serialized = new fsData(type.FullName);
			return fsResult.Success;
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0007F914 File Offset: 0x0007DB14
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (!data.IsString)
			{
				return fsResult.Fail("Type converter requires a string");
			}
			instance = fsTypeCache.GetType(data.AsString);
			if (instance == null)
			{
				return fsResult.Fail("Unable to find type " + data.AsString);
			}
			return fsResult.Success;
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x0007F960 File Offset: 0x0007DB60
		public override object CreateInstance(fsData data, Type storageType)
		{
			return storageType;
		}
	}
}
