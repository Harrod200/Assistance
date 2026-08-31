using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000479 RID: 1145
	public class fsGuidConverter : fsConverter
	{
		// Token: 0x06001866 RID: 6246 RVA: 0x0007EC59 File Offset: 0x0007CE59
		public override bool CanProcess(Type type)
		{
			return type == typeof(Guid);
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x0007EC6B File Offset: 0x0007CE6B
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x0007EC6E File Offset: 0x0007CE6E
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x0007EC74 File Offset: 0x0007CE74
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = new fsData(((Guid)instance).ToString());
			return fsResult.Success;
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x0007ECA1 File Offset: 0x0007CEA1
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (data.IsString)
			{
				instance = new Guid(data.AsString);
				return fsResult.Success;
			}
			return fsResult.Fail("fsGuidConverter encountered an unknown JSON data type");
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x0007ECD0 File Offset: 0x0007CED0
		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Guid);
		}
	}
}
