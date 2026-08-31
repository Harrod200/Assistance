using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200047D RID: 1149
	public class fsPrimitiveConverter : fsConverter
	{
		// Token: 0x06001883 RID: 6275 RVA: 0x0007F28D File Offset: 0x0007D48D
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsPrimitive || type == typeof(string) || type == typeof(decimal);
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x0007F2C0 File Offset: 0x0007D4C0
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x0007F2C3 File Offset: 0x0007D4C3
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x0007F2C6 File Offset: 0x0007D4C6
		private static bool UseBool(Type type)
		{
			return type == typeof(bool);
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x0007F2D8 File Offset: 0x0007D4D8
		private static bool UseInt64(Type type)
		{
			return type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong);
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0007F375 File Offset: 0x0007D575
		private static bool UseDouble(Type type)
		{
			return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0007F3AD File Offset: 0x0007D5AD
		private static bool UseString(Type type)
		{
			return type == typeof(string) || type == typeof(char);
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x0007F3D4 File Offset: 0x0007D5D4
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Type type = instance.GetType();
			if (this.Serializer.Config.Serialize64BitIntegerAsString && (type == typeof(long) || type == typeof(ulong)))
			{
				serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseBool(type))
			{
				serialized = new fsData((bool)instance);
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseInt64(type))
			{
				serialized = new fsData((long)Convert.ChangeType(instance, typeof(long)));
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseDouble(type))
			{
				if (instance.GetType() == typeof(float) && (float)instance != -3.4028235E+38f && (float)instance != 3.4028235E+38f && !float.IsInfinity((float)instance) && !float.IsNaN((float)instance))
				{
					serialized = new fsData((double)((decimal)((float)instance)));
					return fsResult.Success;
				}
				serialized = new fsData((double)Convert.ChangeType(instance, typeof(double)));
				return fsResult.Success;
			}
			else
			{
				if (fsPrimitiveConverter.UseString(type))
				{
					serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
					return fsResult.Success;
				}
				serialized = null;
				string text = "Unhandled primitive type ";
				Type type2 = instance.GetType();
				return fsResult.Fail(text + ((type2 != null) ? type2.ToString() : null));
			}
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x0007F56C File Offset: 0x0007D76C
		public override fsResult TryDeserialize(fsData storage, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			if (fsPrimitiveConverter.UseBool(storageType))
			{
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + base.CheckType(storage, fsDataType.Boolean));
				if (fsResult2.Succeeded)
				{
					instance = storage.AsBool;
				}
				return fsResult;
			}
			if (fsPrimitiveConverter.UseDouble(storageType) || fsPrimitiveConverter.UseInt64(storageType))
			{
				if (storage.IsDouble)
				{
					instance = Convert.ChangeType(storage.AsDouble, storageType);
				}
				else if (storage.IsInt64)
				{
					instance = Convert.ChangeType(storage.AsInt64, storageType);
				}
				else
				{
					if (!this.Serializer.Config.Serialize64BitIntegerAsString || !storage.IsString || (!(storageType == typeof(long)) && !(storageType == typeof(ulong))))
					{
						return fsResult.Fail(string.Concat(new string[]
						{
							base.GetType().Name,
							" expected number but got ",
							storage.Type.ToString(),
							" in ",
							(storage != null) ? storage.ToString() : null
						}));
					}
					instance = Convert.ChangeType(storage.AsString, storageType);
				}
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseString(storageType))
			{
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + base.CheckType(storage, fsDataType.String));
				if (fsResult2.Succeeded)
				{
					instance = storage.AsString;
				}
				return fsResult;
			}
			return fsResult.Fail(base.GetType().Name + ": Bad data; expected bool, number, string, but got " + ((storage != null) ? storage.ToString() : null));
		}
	}
}
