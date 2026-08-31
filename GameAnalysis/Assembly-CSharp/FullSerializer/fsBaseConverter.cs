using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x0200045F RID: 1119
	public abstract class fsBaseConverter
	{
		// Token: 0x0600179E RID: 6046 RVA: 0x0007B044 File Offset: 0x00079244
		public virtual object CreateInstance(fsData data, Type storageType)
		{
			if (this.RequestCycleSupport(storageType))
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Please override CreateInstance for ",
					base.GetType().FullName,
					"; the object graph for ",
					(storageType != null) ? storageType.ToString() : null,
					" can contain potentially contain cycles, so separated instance creation is needed"
				}));
			}
			return storageType;
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x0007B0A2 File Offset: 0x000792A2
		public virtual bool RequestCycleSupport(Type storageType)
		{
			return !(storageType == typeof(string)) && (storageType.Resolve().IsClass || storageType.Resolve().IsInterface);
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x0007B0D2 File Offset: 0x000792D2
		public virtual bool RequestInheritanceSupport(Type storageType)
		{
			return !storageType.Resolve().IsSealed;
		}

		// Token: 0x060017A1 RID: 6049
		public abstract fsResult TrySerialize(object instance, out fsData serialized, Type storageType);

		// Token: 0x060017A2 RID: 6050
		public abstract fsResult TryDeserialize(fsData data, ref object instance, Type storageType);

		// Token: 0x060017A3 RID: 6051 RVA: 0x0007B0E4 File Offset: 0x000792E4
		protected fsResult FailExpectedType(fsData data, params fsDataType[] types)
		{
			string[] array = new string[7];
			array[0] = base.GetType().Name;
			array[1] = " expected one of ";
			array[2] = string.Join(", ", types.Select<fsDataType, string>((fsDataType t) => t.ToString()).ToArray<string>());
			array[3] = " but got ";
			array[4] = data.Type.ToString();
			array[5] = " in ";
			array[6] = ((data != null) ? data.ToString() : null);
			return fsResult.Fail(string.Concat(array));
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x0007B188 File Offset: 0x00079388
		protected fsResult CheckType(fsData data, fsDataType type)
		{
			if (data.Type != type)
			{
				return fsResult.Fail(string.Concat(new string[]
				{
					base.GetType().Name,
					" expected ",
					type.ToString(),
					" but got ",
					data.Type.ToString(),
					" in ",
					(data != null) ? data.ToString() : null
				}));
			}
			return fsResult.Success;
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x0007B211 File Offset: 0x00079411
		protected fsResult CheckKey(fsData data, string key, out fsData subitem)
		{
			return this.CheckKey(data.AsDictionary, key, out subitem);
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x0007B224 File Offset: 0x00079424
		protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
		{
			if (!data.TryGetValue(key, out subitem))
			{
				return fsResult.Fail(string.Concat(new string[]
				{
					base.GetType().Name,
					" requires a <",
					key,
					"> key in the data ",
					(data != null) ? data.ToString() : null
				}));
			}
			return fsResult.Success;
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x0007B284 File Offset: 0x00079484
		protected fsResult SerializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, T value)
		{
			fsData fsData;
			fsResult fsResult = this.Serializer.TrySerialize(typeof(T), overrideConverterType, value, out fsData);
			if (fsResult.Succeeded)
			{
				data[name] = fsData;
			}
			return fsResult;
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0007B2C4 File Offset: 0x000794C4
		protected fsResult DeserializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, out T value)
		{
			fsData fsData;
			if (!data.TryGetValue(name, out fsData))
			{
				value = default(T);
				return fsResult.Fail("Unable to find member \"" + name + "\"");
			}
			object obj = null;
			fsResult fsResult = this.Serializer.TryDeserialize(fsData, typeof(T), overrideConverterType, ref obj);
			value = (T)((object)obj);
			return fsResult;
		}

		// Token: 0x040015D5 RID: 5589
		public fsSerializer Serializer;
	}
}
