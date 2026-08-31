using System;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x02000487 RID: 1159
	public class fsMetaProperty
	{
		// Token: 0x060018D2 RID: 6354 RVA: 0x0008041C File Offset: 0x0007E61C
		internal fsMetaProperty(fsConfig config, FieldInfo field)
		{
			this._memberInfo = field;
			this.StorageType = field.FieldType;
			this.MemberName = field.Name;
			this.IsPublic = field.IsPublic;
			this.IsReadOnly = field.IsInitOnly;
			this.CanRead = true;
			this.CanWrite = true;
			this.CommonInitialize(config);
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x0008047C File Offset: 0x0007E67C
		internal fsMetaProperty(fsConfig config, PropertyInfo property)
		{
			this._memberInfo = property;
			this.StorageType = property.PropertyType;
			this.MemberName = property.Name;
			this.IsPublic = property.GetGetMethod() != null && property.GetGetMethod().IsPublic && property.GetSetMethod() != null && property.GetSetMethod().IsPublic;
			this.IsReadOnly = false;
			this.CanRead = property.CanRead;
			this.CanWrite = property.CanWrite;
			this.CommonInitialize(config);
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x00080514 File Offset: 0x0007E714
		private void CommonInitialize(fsConfig config)
		{
			fsPropertyAttribute attribute = fsPortableReflection.GetAttribute<fsPropertyAttribute>(this._memberInfo);
			if (attribute != null)
			{
				this.JsonName = attribute.Name;
				this.OverrideConverterType = attribute.Converter;
			}
			if (string.IsNullOrEmpty(this.JsonName))
			{
				this.JsonName = config.GetJsonNameFromMemberName(this.MemberName, this._memberInfo);
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060018D5 RID: 6357 RVA: 0x00080572 File Offset: 0x0007E772
		// (set) Token: 0x060018D6 RID: 6358 RVA: 0x0008057A File Offset: 0x0007E77A
		public Type StorageType { get; private set; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060018D7 RID: 6359 RVA: 0x00080583 File Offset: 0x0007E783
		// (set) Token: 0x060018D8 RID: 6360 RVA: 0x0008058B File Offset: 0x0007E78B
		public Type OverrideConverterType { get; private set; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x060018D9 RID: 6361 RVA: 0x00080594 File Offset: 0x0007E794
		// (set) Token: 0x060018DA RID: 6362 RVA: 0x0008059C File Offset: 0x0007E79C
		public bool CanRead { get; private set; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060018DB RID: 6363 RVA: 0x000805A5 File Offset: 0x0007E7A5
		// (set) Token: 0x060018DC RID: 6364 RVA: 0x000805AD File Offset: 0x0007E7AD
		public bool CanWrite { get; private set; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060018DD RID: 6365 RVA: 0x000805B6 File Offset: 0x0007E7B6
		// (set) Token: 0x060018DE RID: 6366 RVA: 0x000805BE File Offset: 0x0007E7BE
		public string JsonName { get; private set; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060018DF RID: 6367 RVA: 0x000805C7 File Offset: 0x0007E7C7
		// (set) Token: 0x060018E0 RID: 6368 RVA: 0x000805CF File Offset: 0x0007E7CF
		public string MemberName { get; private set; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x060018E1 RID: 6369 RVA: 0x000805D8 File Offset: 0x0007E7D8
		// (set) Token: 0x060018E2 RID: 6370 RVA: 0x000805E0 File Offset: 0x0007E7E0
		public bool IsPublic { get; private set; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x060018E3 RID: 6371 RVA: 0x000805E9 File Offset: 0x0007E7E9
		// (set) Token: 0x060018E4 RID: 6372 RVA: 0x000805F1 File Offset: 0x0007E7F1
		public bool IsReadOnly { get; private set; }

		// Token: 0x060018E5 RID: 6373 RVA: 0x000805FC File Offset: 0x0007E7FC
		public void Write(object context, object value)
		{
			FieldInfo fieldInfo = this._memberInfo as FieldInfo;
			PropertyInfo propertyInfo = this._memberInfo as PropertyInfo;
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(context, value);
				return;
			}
			if (propertyInfo != null)
			{
				MethodInfo setMethod = propertyInfo.GetSetMethod(true);
				if (setMethod != null)
				{
					setMethod.Invoke(context, new object[] { value });
				}
			}
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x0008065F File Offset: 0x0007E85F
		public object Read(object context)
		{
			if (this._memberInfo is PropertyInfo)
			{
				return ((PropertyInfo)this._memberInfo).GetValue(context, new object[0]);
			}
			return ((FieldInfo)this._memberInfo).GetValue(context);
		}

		// Token: 0x04001624 RID: 5668
		private MemberInfo _memberInfo;
	}
}
