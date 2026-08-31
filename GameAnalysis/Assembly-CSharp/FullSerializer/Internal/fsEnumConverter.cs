using System;
using System.Collections.Generic;
using System.Text;

namespace FullSerializer.Internal
{
	// Token: 0x02000477 RID: 1143
	public class fsEnumConverter : fsConverter
	{
		// Token: 0x06001858 RID: 6232 RVA: 0x0007E8C9 File Offset: 0x0007CAC9
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsEnum;
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0007E8D6 File Offset: 0x0007CAD6
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0007E8D9 File Offset: 0x0007CAD9
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0007E8DC File Offset: 0x0007CADC
		public override object CreateInstance(fsData data, Type storageType)
		{
			return Enum.ToObject(storageType, 0);
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0007E8EC File Offset: 0x0007CAEC
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			if (this.Serializer.Config.SerializeEnumsAsInteger)
			{
				serialized = new fsData(Convert.ToInt64(instance));
			}
			else if (fsPortableReflection.GetAttribute<FlagsAttribute>(storageType) != null)
			{
				long num = Convert.ToInt64(instance);
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (object obj in Enum.GetValues(storageType))
				{
					long num2 = Convert.ToInt64(obj);
					if ((num & num2) != 0L)
					{
						if (!flag)
						{
							stringBuilder.Append(",");
						}
						flag = false;
						stringBuilder.Append(obj.ToString());
					}
				}
				serialized = new fsData(stringBuilder.ToString());
			}
			else
			{
				serialized = new fsData(Enum.GetName(storageType, instance));
			}
			return fsResult.Success;
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x0007E9D0 File Offset: 0x0007CBD0
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (data.IsString)
			{
				string[] array = data.AsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				long num = 0L;
				foreach (string text in array)
				{
					if (!fsEnumConverter.ArrayContains<string>(Enum.GetNames(storageType), text))
					{
						return fsResult.Fail("Cannot find enum name " + text + " on type " + ((storageType != null) ? storageType.ToString() : null));
					}
					long num2 = (long)Convert.ChangeType(Enum.Parse(storageType, text), typeof(long));
					num |= num2;
				}
				instance = Enum.ToObject(storageType, num);
				return fsResult.Success;
			}
			if (data.IsInt64)
			{
				int num3 = (int)data.AsInt64;
				instance = Enum.ToObject(storageType, num3);
				return fsResult.Success;
			}
			return fsResult.Fail("EnumConverter encountered an unknown JSON data type");
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x0007EAAC File Offset: 0x0007CCAC
		private static bool ArrayContains<T>(T[] values, T value)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(values[i], value))
				{
					return true;
				}
			}
			return false;
		}
	}
}
