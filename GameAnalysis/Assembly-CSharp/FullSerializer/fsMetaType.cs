using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FullSerializer.Internal;
using UnityEngine;

namespace FullSerializer
{
	// Token: 0x0200045E RID: 1118
	public class fsMetaType
	{
		// Token: 0x06001790 RID: 6032 RVA: 0x0007A8CC File Offset: 0x00078ACC
		public static fsMetaType Get(fsConfig config, Type type)
		{
			Type typeFromHandle = typeof(fsMetaType);
			Dictionary<Type, fsMetaType> dictionary;
			lock (typeFromHandle)
			{
				if (!fsMetaType._configMetaTypes.TryGetValue(config, out dictionary))
				{
					dictionary = (fsMetaType._configMetaTypes[config] = new Dictionary<Type, fsMetaType>());
				}
			}
			fsMetaType fsMetaType;
			if (!dictionary.TryGetValue(type, out fsMetaType))
			{
				fsMetaType = new fsMetaType(config, type);
				dictionary[type] = fsMetaType;
			}
			return fsMetaType;
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x0007A94C File Offset: 0x00078B4C
		public static void ClearCache()
		{
			Type typeFromHandle = typeof(fsMetaType);
			lock (typeFromHandle)
			{
				fsMetaType._configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();
			}
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x0007A994 File Offset: 0x00078B94
		private fsMetaType(fsConfig config, Type reflectedType)
		{
			this.ReflectedType = reflectedType;
			List<fsMetaProperty> list = new List<fsMetaProperty>();
			fsMetaType.CollectProperties(config, list, reflectedType);
			this.Properties = list.ToArray();
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x0007A9C8 File Offset: 0x00078BC8
		private static void CollectProperties(fsConfig config, List<fsMetaProperty> properties, Type reflectedType)
		{
			bool flag = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
			bool flag2 = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(reflectedType);
			if (attribute != null)
			{
				flag = attribute.MemberSerialization == fsMemberSerialization.OptIn;
				flag2 = attribute.MemberSerialization == fsMemberSerialization.OptOut;
			}
			MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
			MemberInfo[] array = declaredMembers;
			for (int i = 0; i < array.Length; i++)
			{
				MemberInfo member = array[i];
				if (!config.IgnoreSerializeAttributes.Any<Type>((Type t) => fsPortableReflection.HasAttribute(member, t)))
				{
					PropertyInfo propertyInfo = member as PropertyInfo;
					FieldInfo fieldInfo = member as FieldInfo;
					if ((!(propertyInfo == null) || !(fieldInfo == null)) && (!(propertyInfo != null) || config.EnablePropertySerialization) && (!flag || config.SerializeAttributes.Any<Type>((Type t) => fsPortableReflection.HasAttribute(member, t))) && (!flag2 || !config.IgnoreSerializeAttributes.Any<Type>((Type t) => fsPortableReflection.HasAttribute(member, t))))
					{
						if (propertyInfo != null)
						{
							if (fsMetaType.CanSerializeProperty(config, propertyInfo, declaredMembers, flag2))
							{
								properties.Add(new fsMetaProperty(config, propertyInfo));
							}
						}
						else if (fieldInfo != null && fsMetaType.CanSerializeField(config, fieldInfo, flag2))
						{
							properties.Add(new fsMetaProperty(config, fieldInfo));
						}
					}
				}
			}
			if (reflectedType.Resolve().BaseType != null)
			{
				fsMetaType.CollectProperties(config, properties, reflectedType.Resolve().BaseType);
			}
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x0007AB4A File Offset: 0x00078D4A
		private static bool IsAutoProperty(PropertyInfo property, MemberInfo[] members)
		{
			return property.CanWrite && property.CanRead && fsPortableReflection.HasAttribute(property.GetGetMethod(), typeof(CompilerGeneratedAttribute), false);
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x0007AB74 File Offset: 0x00078D74
		private static bool CanSerializeProperty(fsConfig config, PropertyInfo property, MemberInfo[] members, bool annotationFreeValue)
		{
			if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
			{
				return false;
			}
			MethodInfo getMethod = property.GetGetMethod(false);
			MethodInfo setMethod = property.GetSetMethod(false);
			return (!(getMethod != null) || !getMethod.IsStatic) && (!(setMethod != null) || !setMethod.IsStatic) && property.GetIndexParameters().Length == 0 && (config.SerializeAttributes.Any<Type>((Type t) => fsPortableReflection.HasAttribute(property, t)) || (property.CanRead && property.CanWrite && ((getMethod != null && (config.SerializeNonPublicSetProperties || setMethod != null) && (config.SerializeNonAutoProperties || fsMetaType.IsAutoProperty(property, members))) || annotationFreeValue)));
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0007AC64 File Offset: 0x00078E64
		private static bool CanSerializeField(fsConfig config, FieldInfo field, bool annotationFreeValue)
		{
			return !typeof(Delegate).IsAssignableFrom(field.FieldType) && !field.IsDefined(typeof(CompilerGeneratedAttribute), false) && !field.IsStatic && (config.SerializeAttributes.Any<Type>((Type t) => fsPortableReflection.HasAttribute(field, t)) || annotationFreeValue || field.IsPublic);
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x0007ACF4 File Offset: 0x00078EF4
		public void EmitAotData(bool throwException)
		{
			fsAotCompilationManager.AotCandidateTypes.Add(this.ReflectedType);
			if (!throwException)
			{
				return;
			}
			for (int i = 0; i < this.Properties.Length; i++)
			{
				if (!this.Properties[i].IsPublic)
				{
					throw new fsMetaType.AotFailureException(this.ReflectedType.CSharpName(true) + "::" + this.Properties[i].MemberName + " is not public");
				}
				if (this.Properties[i].IsReadOnly)
				{
					throw new fsMetaType.AotFailureException(this.ReflectedType.CSharpName(true) + "::" + this.Properties[i].MemberName + " is readonly");
				}
			}
			if (!this.HasDefaultConstructor)
			{
				throw new fsMetaType.AotFailureException(this.ReflectedType.CSharpName(true) + " does not have a default constructor");
			}
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06001798 RID: 6040 RVA: 0x0007ADCA File Offset: 0x00078FCA
		// (set) Token: 0x06001799 RID: 6041 RVA: 0x0007ADD2 File Offset: 0x00078FD2
		public fsMetaProperty[] Properties { get; private set; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600179A RID: 6042 RVA: 0x0007ADDC File Offset: 0x00078FDC
		public bool HasDefaultConstructor
		{
			get
			{
				if (this._hasDefaultConstructorCache == null)
				{
					if (this.ReflectedType.Resolve().IsArray)
					{
						this._hasDefaultConstructorCache = new bool?(true);
						this._isDefaultConstructorPublicCache = new bool?(true);
					}
					else if (this.ReflectedType.Resolve().IsValueType)
					{
						this._hasDefaultConstructorCache = new bool?(true);
						this._isDefaultConstructorPublicCache = new bool?(true);
					}
					else
					{
						ConstructorInfo declaredConstructor = this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
						this._hasDefaultConstructorCache = new bool?(declaredConstructor != null);
						if (declaredConstructor != null)
						{
							this._isDefaultConstructorPublicCache = new bool?(declaredConstructor.IsPublic);
						}
					}
				}
				return this._hasDefaultConstructorCache.Value;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600179B RID: 6043 RVA: 0x0007AE99 File Offset: 0x00079099
		public bool IsDefaultConstructorPublic
		{
			get
			{
				if (this._isDefaultConstructorPublicCache == null)
				{
					bool hasDefaultConstructor = this.HasDefaultConstructor;
				}
				return this._isDefaultConstructorPublicCache.Value;
			}
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0007AEBC File Offset: 0x000790BC
		public object CreateInstance()
		{
			if (this.ReflectedType.Resolve().IsInterface || this.ReflectedType.Resolve().IsAbstract)
			{
				string text = "Cannot create an instance of an interface or abstract type for ";
				Type reflectedType = this.ReflectedType;
				throw new Exception(text + ((reflectedType != null) ? reflectedType.ToString() : null));
			}
			if (typeof(ScriptableObject).IsAssignableFrom(this.ReflectedType))
			{
				return ScriptableObject.CreateInstance(this.ReflectedType);
			}
			if (typeof(string) == this.ReflectedType)
			{
				return string.Empty;
			}
			if (!this.HasDefaultConstructor)
			{
				return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
			}
			if (this.ReflectedType.Resolve().IsArray)
			{
				return Array.CreateInstance(this.ReflectedType.GetElementType(), 0);
			}
			object obj;
			try
			{
				obj = Activator.CreateInstance(this.ReflectedType, true);
			}
			catch (MissingMethodException ex)
			{
				string text2 = "Unable to create instance of ";
				Type reflectedType2 = this.ReflectedType;
				throw new InvalidOperationException(text2 + ((reflectedType2 != null) ? reflectedType2.ToString() : null) + "; there is no default constructor", ex);
			}
			catch (TargetInvocationException ex2)
			{
				string text3 = "Constructor of ";
				Type reflectedType3 = this.ReflectedType;
				throw new InvalidOperationException(text3 + ((reflectedType3 != null) ? reflectedType3.ToString() : null) + " threw an exception when creating an instance", ex2);
			}
			catch (MemberAccessException ex3)
			{
				string text4 = "Unable to access constructor of ";
				Type reflectedType4 = this.ReflectedType;
				throw new InvalidOperationException(text4 + ((reflectedType4 != null) ? reflectedType4.ToString() : null), ex3);
			}
			return obj;
		}

		// Token: 0x040015D0 RID: 5584
		private static Dictionary<fsConfig, Dictionary<Type, fsMetaType>> _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();

		// Token: 0x040015D1 RID: 5585
		public Type ReflectedType;

		// Token: 0x040015D3 RID: 5587
		private bool? _hasDefaultConstructorCache;

		// Token: 0x040015D4 RID: 5588
		private bool? _isDefaultConstructorPublicCache;

		// Token: 0x02000C50 RID: 3152
		public class AotFailureException : Exception
		{
			// Token: 0x06006C4D RID: 27725 RVA: 0x003066E7 File Offset: 0x003048E7
			public AotFailureException(string reason)
				: base(reason)
			{
			}
		}
	}
}
