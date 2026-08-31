using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200047C RID: 1148
	public class fsNullableConverter : fsConverter
	{
		// Token: 0x0600187E RID: 6270 RVA: 0x0007F232 File Offset: 0x0007D432
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x0007F258 File Offset: 0x0007D458
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			return this.Serializer.TrySerialize(Nullable.GetUnderlyingType(storageType), instance, out serialized);
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x0007F26D File Offset: 0x0007D46D
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			return this.Serializer.TryDeserialize(data, Nullable.GetUnderlyingType(storageType), ref instance);
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x0007F282 File Offset: 0x0007D482
		public override object CreateInstance(fsData data, Type storageType)
		{
			return storageType;
		}
	}
}
