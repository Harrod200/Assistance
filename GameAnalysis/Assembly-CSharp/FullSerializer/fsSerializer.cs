using System;
using System.Collections.Generic;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000473 RID: 1139
	public class fsSerializer
	{
		// Token: 0x06001820 RID: 6176 RVA: 0x0007D013 File Offset: 0x0007B213
		public static bool IsReservedKeyword(string key)
		{
			return fsSerializer._reservedKeywords.Contains(key);
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0007D020 File Offset: 0x0007B220
		private static bool IsObjectReference(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey(fsSerializer.Key_ObjectReference);
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x0007D03C File Offset: 0x0007B23C
		private static bool IsObjectDefinition(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey(fsSerializer.Key_ObjectDefinition);
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x0007D058 File Offset: 0x0007B258
		private static bool IsVersioned(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey(fsSerializer.Key_Version);
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x0007D074 File Offset: 0x0007B274
		private static bool IsTypeSpecified(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey(fsSerializer.Key_InstanceType);
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x0007D090 File Offset: 0x0007B290
		private static bool IsWrappedData(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey(fsSerializer.Key_Content);
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x0007D0AC File Offset: 0x0007B2AC
		public static void StripDeserializationMetadata(ref fsData data)
		{
			if (data.IsDictionary && data.AsDictionary.ContainsKey(fsSerializer.Key_Content))
			{
				data = data.AsDictionary[fsSerializer.Key_Content];
			}
			if (data.IsDictionary)
			{
				Dictionary<string, fsData> asDictionary = data.AsDictionary;
				asDictionary.Remove(fsSerializer.Key_ObjectReference);
				asDictionary.Remove(fsSerializer.Key_ObjectDefinition);
				asDictionary.Remove(fsSerializer.Key_InstanceType);
				asDictionary.Remove(fsSerializer.Key_Version);
			}
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x0007D128 File Offset: 0x0007B328
		private static void ConvertLegacyData(ref fsData data)
		{
			if (!data.IsDictionary)
			{
				return;
			}
			Dictionary<string, fsData> asDictionary = data.AsDictionary;
			if (asDictionary.Count > 2)
			{
				return;
			}
			string text = "ReferenceId";
			string text2 = "SourceId";
			string text3 = "Data";
			string text4 = "Type";
			string text5 = "Data";
			if (asDictionary.Count == 2 && asDictionary.ContainsKey(text4) && asDictionary.ContainsKey(text5))
			{
				data = asDictionary[text5];
				fsSerializer.EnsureDictionary(data);
				fsSerializer.ConvertLegacyData(ref data);
				data.AsDictionary[fsSerializer.Key_InstanceType] = asDictionary[text4];
				return;
			}
			if (asDictionary.Count == 2 && asDictionary.ContainsKey(text2) && asDictionary.ContainsKey(text3))
			{
				data = asDictionary[text3];
				fsSerializer.EnsureDictionary(data);
				fsSerializer.ConvertLegacyData(ref data);
				data.AsDictionary[fsSerializer.Key_ObjectDefinition] = asDictionary[text2];
				return;
			}
			if (asDictionary.Count == 1 && asDictionary.ContainsKey(text))
			{
				data = fsData.CreateDictionary();
				data.AsDictionary[fsSerializer.Key_ObjectReference] = asDictionary[text];
			}
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0007D23C File Offset: 0x0007B43C
		private static void Invoke_OnBeforeSerialize(List<fsObjectProcessor> processors, Type storageType, object instance)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeSerialize(storageType, instance);
			}
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0007D268 File Offset: 0x0007B468
		private static void Invoke_OnAfterSerialize(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
		{
			for (int i = processors.Count - 1; i >= 0; i--)
			{
				processors[i].OnAfterSerialize(storageType, instance, ref data);
			}
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0007D298 File Offset: 0x0007B498
		private static void Invoke_OnBeforeDeserialize(List<fsObjectProcessor> processors, Type storageType, ref fsData data)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeDeserialize(storageType, ref data);
			}
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x0007D2C4 File Offset: 0x0007B4C4
		private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
			}
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x0007D2F4 File Offset: 0x0007B4F4
		private static void Invoke_OnAfterDeserialize(List<fsObjectProcessor> processors, Type storageType, object instance)
		{
			for (int i = processors.Count - 1; i >= 0; i--)
			{
				processors[i].OnAfterDeserialize(storageType, instance);
			}
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0007D324 File Offset: 0x0007B524
		private static void EnsureDictionary(fsData data)
		{
			if (!data.IsDictionary)
			{
				fsData fsData = data.Clone();
				data.BecomeDictionary();
				data.AsDictionary[fsSerializer.Key_Content] = fsData;
			}
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x0007D358 File Offset: 0x0007B558
		private void RemapAbstractStorageTypeToDefaultType(ref Type storageType)
		{
			if (!storageType.IsInterface && !storageType.IsAbstract)
			{
				return;
			}
			Type type2;
			if (storageType.IsGenericType)
			{
				Type type;
				if (this._abstractTypeRemap.TryGetValue(storageType.GetGenericTypeDefinition(), out type))
				{
					Type[] genericArguments = storageType.GetGenericArguments();
					storageType = type.MakeGenericType(genericArguments);
					return;
				}
			}
			else if (this._abstractTypeRemap.TryGetValue(storageType, out type2))
			{
				storageType = type2;
			}
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0007D3C0 File Offset: 0x0007B5C0
		public fsSerializer()
		{
			this._cachedConverterTypeInstances = new Dictionary<Type, fsBaseConverter>();
			this._cachedConverters = new Dictionary<Type, fsBaseConverter>();
			this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
			this._references = new fsCyclicReferenceManager();
			this._lazyReferenceWriter = new fsSerializer.fsLazyCycleDefinitionWriter();
			this._availableConverters = new List<fsConverter>
			{
				new fsNullableConverter
				{
					Serializer = this
				},
				new fsGuidConverter
				{
					Serializer = this
				},
				new fsTypeConverter
				{
					Serializer = this
				},
				new fsDateConverter
				{
					Serializer = this
				},
				new fsEnumConverter
				{
					Serializer = this
				},
				new fsPrimitiveConverter
				{
					Serializer = this
				},
				new fsArrayConverter
				{
					Serializer = this
				},
				new fsDictionaryConverter
				{
					Serializer = this
				},
				new fsIEnumerableConverter
				{
					Serializer = this
				},
				new fsKeyValuePairConverter
				{
					Serializer = this
				},
				new fsWeakReferenceConverter
				{
					Serializer = this
				},
				new fsReflectedConverter
				{
					Serializer = this
				}
			};
			this._availableDirectConverters = new Dictionary<Type, fsDirectConverter>();
			this._processors = new List<fsObjectProcessor>
			{
				new fsSerializationCallbackProcessor()
			};
			this._processors.Add(new fsSerializationCallbackReceiverProcessor());
			this._abstractTypeRemap = new Dictionary<Type, Type>();
			this.SetDefaultStorageType(typeof(ICollection<>), typeof(List<>));
			this.SetDefaultStorageType(typeof(IList<>), typeof(List<>));
			this.SetDefaultStorageType(typeof(IDictionary<, >), typeof(Dictionary<, >));
			this.Context = new fsContext();
			this.Config = new fsConfig();
			foreach (Type type in fsConverterRegistrar.Converters)
			{
				this.AddConverter((fsBaseConverter)Activator.CreateInstance(type));
			}
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x0007D5DC File Offset: 0x0007B7DC
		public void AddProcessor(fsObjectProcessor processor)
		{
			this._processors.Add(processor);
			this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x0007D5F8 File Offset: 0x0007B7F8
		public void RemoveProcessor<TProcessor>()
		{
			int i = 0;
			while (i < this._processors.Count)
			{
				if (this._processors[i] is TProcessor)
				{
					this._processors.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x0007D647 File Offset: 0x0007B847
		public void SetDefaultStorageType(Type abstractType, Type defaultStorageType)
		{
			if (!abstractType.IsInterface && !abstractType.IsAbstract)
			{
				throw new ArgumentException("|abstractType| must be an interface or abstract type");
			}
			this._abstractTypeRemap[abstractType] = defaultStorageType;
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x0007D674 File Offset: 0x0007B874
		private List<fsObjectProcessor> GetProcessors(Type type)
		{
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			List<fsObjectProcessor> list;
			if (attribute != null && attribute.Processor != null)
			{
				fsObjectProcessor fsObjectProcessor = (fsObjectProcessor)Activator.CreateInstance(attribute.Processor);
				list = new List<fsObjectProcessor>();
				list.Add(fsObjectProcessor);
				this._cachedProcessors[type] = list;
			}
			else if (!this._cachedProcessors.TryGetValue(type, out list))
			{
				list = new List<fsObjectProcessor>();
				for (int i = 0; i < this._processors.Count; i++)
				{
					fsObjectProcessor fsObjectProcessor2 = this._processors[i];
					if (fsObjectProcessor2.CanProcess(type))
					{
						list.Add(fsObjectProcessor2);
					}
				}
				this._cachedProcessors[type] = list;
			}
			return list;
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x0007D720 File Offset: 0x0007B920
		public void AddConverter(fsBaseConverter converter)
		{
			if (converter.Serializer != null)
			{
				throw new InvalidOperationException("Cannot add a single converter instance to multiple fsConverters -- please construct a new instance for " + ((converter != null) ? converter.ToString() : null));
			}
			if (converter is fsDirectConverter)
			{
				fsDirectConverter fsDirectConverter = (fsDirectConverter)converter;
				this._availableDirectConverters[fsDirectConverter.ModelType] = fsDirectConverter;
			}
			else
			{
				if (!(converter is fsConverter))
				{
					throw new InvalidOperationException("Unable to add converter " + ((converter != null) ? converter.ToString() : null) + "; the type association strategy is unknown. Please use either fsDirectConverter or fsConverter as your base type.");
				}
				this._availableConverters.Insert(0, (fsConverter)converter);
			}
			converter.Serializer = this;
			this._cachedConverters = new Dictionary<Type, fsBaseConverter>();
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x0007D7C8 File Offset: 0x0007B9C8
		private fsBaseConverter GetConverter(Type type, Type overrideConverterType)
		{
			if (overrideConverterType != null)
			{
				fsBaseConverter fsBaseConverter;
				if (!this._cachedConverterTypeInstances.TryGetValue(overrideConverterType, out fsBaseConverter))
				{
					fsBaseConverter = (fsBaseConverter)Activator.CreateInstance(overrideConverterType);
					fsBaseConverter.Serializer = this;
					this._cachedConverterTypeInstances[overrideConverterType] = fsBaseConverter;
				}
				return fsBaseConverter;
			}
			fsBaseConverter fsBaseConverter2;
			if (this._cachedConverters.TryGetValue(type, out fsBaseConverter2))
			{
				return fsBaseConverter2;
			}
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			if (attribute != null && attribute.Converter != null)
			{
				fsBaseConverter2 = (fsBaseConverter)Activator.CreateInstance(attribute.Converter);
				fsBaseConverter2.Serializer = this;
				return this._cachedConverters[type] = fsBaseConverter2;
			}
			fsForwardAttribute attribute2 = fsPortableReflection.GetAttribute<fsForwardAttribute>(type);
			if (attribute2 != null)
			{
				fsBaseConverter2 = new fsForwardConverter(attribute2);
				fsBaseConverter2.Serializer = this;
				return this._cachedConverters[type] = fsBaseConverter2;
			}
			if (!this._cachedConverters.TryGetValue(type, out fsBaseConverter2))
			{
				if (this._availableDirectConverters.ContainsKey(type))
				{
					fsBaseConverter2 = this._availableDirectConverters[type];
					return this._cachedConverters[type] = fsBaseConverter2;
				}
				for (int i = 0; i < this._availableConverters.Count; i++)
				{
					if (this._availableConverters[i].CanProcess(type))
					{
						fsBaseConverter2 = this._availableConverters[i];
						return this._cachedConverters[type] = fsBaseConverter2;
					}
				}
			}
			throw new InvalidOperationException("Internal error -- could not find a converter for " + ((type != null) ? type.ToString() : null));
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0007D935 File Offset: 0x0007BB35
		public fsResult TrySerialize<T>(T instance, out fsData data)
		{
			return this.TrySerialize(typeof(T), instance, out data);
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x0007D950 File Offset: 0x0007BB50
		public fsResult TryDeserialize<T>(fsData data, ref T instance)
		{
			object obj = instance;
			fsResult fsResult = this.TryDeserialize(data, typeof(T), ref obj);
			if (fsResult.Succeeded)
			{
				instance = (T)((object)obj);
			}
			return fsResult;
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x0007D993 File Offset: 0x0007BB93
		public fsResult TrySerialize(Type storageType, object instance, out fsData data)
		{
			return this.TrySerialize(storageType, null, instance, out data);
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x0007D9A0 File Offset: 0x0007BBA0
		public fsResult TrySerialize(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			List<fsObjectProcessor> processors = this.GetProcessors((instance == null) ? storageType : instance.GetType());
			fsSerializer.Invoke_OnBeforeSerialize(processors, storageType, instance);
			if (instance == null)
			{
				data = new fsData();
				fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
				return fsResult.Success;
			}
			fsResult fsResult = this.InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);
			fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
			return fsResult;
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x0007D9F8 File Offset: 0x0007BBF8
		private fsResult InternalSerialize_1_ProcessCycles(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			fsResult fsResult;
			try
			{
				this._references.Enter();
				if (!this.GetConverter(instance.GetType(), overrideConverterType).RequestCycleSupport(instance.GetType()))
				{
					fsResult = this.InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
				}
				else if (this._references.IsReference(instance))
				{
					data = fsData.CreateDictionary();
					this._lazyReferenceWriter.WriteReference(this._references.GetReferenceId(instance), data.AsDictionary);
					fsResult = fsResult.Success;
				}
				else
				{
					this._references.MarkSerialized(instance);
					fsResult fsResult2 = this.InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
					if (fsResult2.Failed)
					{
						fsResult = fsResult2;
					}
					else
					{
						this._lazyReferenceWriter.WriteDefinition(this._references.GetReferenceId(instance), data);
						fsResult = fsResult2;
					}
				}
			}
			finally
			{
				if (this._references.Exit())
				{
					this._lazyReferenceWriter.Clear();
				}
			}
			return fsResult;
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x0007DAE8 File Offset: 0x0007BCE8
		private fsResult InternalSerialize_2_Inheritance(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			fsResult fsResult = this.InternalSerialize_3_ProcessVersioning(overrideConverterType, instance, out data);
			if (fsResult.Failed)
			{
				return fsResult;
			}
			if (storageType != instance.GetType() && this.GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType))
			{
				fsSerializer.EnsureDictionary(data);
				data.AsDictionary[fsSerializer.Key_InstanceType] = new fsData(instance.GetType().FullName);
			}
			return fsResult;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x0007DB54 File Offset: 0x0007BD54
		private fsResult InternalSerialize_3_ProcessVersioning(Type overrideConverterType, object instance, out fsData data)
		{
			fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(instance.GetType());
			if (!versionedType.HasValue)
			{
				return this.InternalSerialize_4_Converter(overrideConverterType, instance, out data);
			}
			fsVersionedType value = versionedType.Value;
			fsResult fsResult = this.InternalSerialize_4_Converter(overrideConverterType, instance, out data);
			if (fsResult.Failed)
			{
				return fsResult;
			}
			fsSerializer.EnsureDictionary(data);
			data.AsDictionary[fsSerializer.Key_Version] = new fsData(value.VersionString);
			return fsResult;
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x0007DBC4 File Offset: 0x0007BDC4
		private fsResult InternalSerialize_4_Converter(Type overrideConverterType, object instance, out fsData data)
		{
			Type type = instance.GetType();
			return this.GetConverter(type, overrideConverterType).TrySerialize(instance, out data, type);
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x0007DBE8 File Offset: 0x0007BDE8
		public fsResult TryDeserialize(fsData data, Type storageType, ref object result)
		{
			return this.TryDeserialize(data, storageType, null, ref result);
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0007DBF4 File Offset: 0x0007BDF4
		public fsResult TryDeserialize(fsData data, Type storageType, Type overrideConverterType, ref object result)
		{
			if (data.IsNull)
			{
				result = null;
				List<fsObjectProcessor> processors = this.GetProcessors(storageType);
				fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
				fsSerializer.Invoke_OnAfterDeserialize(processors, storageType, null);
				return fsResult.Success;
			}
			fsSerializer.ConvertLegacyData(ref data);
			fsResult fsResult2;
			try
			{
				this._references.Enter();
				List<fsObjectProcessor> list;
				fsResult fsResult = this.InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out list);
				if (fsResult.Succeeded)
				{
					fsSerializer.Invoke_OnAfterDeserialize(list, storageType, result);
				}
				fsResult2 = fsResult;
			}
			finally
			{
				this._references.Exit();
			}
			return fsResult2;
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x0007DC80 File Offset: 0x0007BE80
		private fsResult InternalDeserialize_1_CycleReference(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			if (fsSerializer.IsObjectReference(data))
			{
				int num = int.Parse(data.AsDictionary[fsSerializer.Key_ObjectReference].AsString);
				result = this._references.GetReferenceObject(num);
				processors = this.GetProcessors(result.GetType());
				return fsResult.Success;
			}
			return this.InternalDeserialize_2_Version(overrideConverterType, data, storageType, ref result, out processors);
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x0007DCE4 File Offset: 0x0007BEE4
		private fsResult InternalDeserialize_2_Version(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			if (fsSerializer.IsVersioned(data))
			{
				string asString = data.AsDictionary[fsSerializer.Key_Version].AsString;
				fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(storageType);
				if (versionedType.HasValue && versionedType.Value.VersionString != asString)
				{
					fsResult fsResult = fsResult.Success;
					List<fsVersionedType> list;
					fsResult += fsVersionManager.GetVersionImportPath(asString, versionedType.Value, out list);
					if (fsResult.Failed)
					{
						processors = this.GetProcessors(storageType);
						return fsResult;
					}
					fsResult += this.InternalDeserialize_3_Inheritance(overrideConverterType, data, list[0].ModelType, ref result, out processors);
					if (fsResult.Failed)
					{
						return fsResult;
					}
					for (int i = 1; i < list.Count; i++)
					{
						result = list[i].Migrate(result);
					}
					if (fsSerializer.IsObjectDefinition(data))
					{
						int num = int.Parse(data.AsDictionary[fsSerializer.Key_ObjectDefinition].AsString);
						this._references.AddReferenceWithId(num, result);
					}
					processors = this.GetProcessors(fsResult.GetType());
					return fsResult;
				}
			}
			return this.InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x0007DE1C File Offset: 0x0007C01C
		private fsResult InternalDeserialize_3_Inheritance(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			fsResult fsResult = fsResult.Success;
			Type type = storageType;
			if (fsSerializer.IsTypeSpecified(data))
			{
				fsData fsData = data.AsDictionary[fsSerializer.Key_InstanceType];
				if (!fsData.IsString)
				{
					string key_InstanceType = fsSerializer.Key_InstanceType;
					string text = " value must be a string (in ";
					fsData fsData2 = data;
					fsResult.AddMessage(key_InstanceType + text + ((fsData2 != null) ? fsData2.ToString() : null) + ")");
				}
				else
				{
					string asString = fsData.AsString;
					Type type2 = fsTypeCache.GetType(asString);
					if (type2 == null)
					{
						fsResult += fsResult.Fail("Unable to locate specified type \"" + asString + "\"");
					}
					else if (!storageType.IsAssignableFrom(type2))
					{
						string text2 = "Ignoring type specifier; a field/property of type ";
						string text3 = ((storageType != null) ? storageType.ToString() : null);
						string text4 = " cannot hold an instance of ";
						Type type3 = type2;
						fsResult.AddMessage(text2 + text3 + text4 + ((type3 != null) ? type3.ToString() : null));
					}
					else
					{
						type = type2;
					}
				}
			}
			this.RemapAbstractStorageTypeToDefaultType(ref type);
			processors = this.GetProcessors(type);
			if (fsResult.Failed)
			{
				return fsResult;
			}
			fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
			if (result == null || result.GetType() != type)
			{
				result = this.GetConverter(type, overrideConverterType).CreateInstance(data, type);
			}
			fsSerializer.Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
			return fsResult += this.InternalDeserialize_4_Cycles(overrideConverterType, data, type, ref result);
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x0007DF68 File Offset: 0x0007C168
		private fsResult InternalDeserialize_4_Cycles(Type overrideConverterType, fsData data, Type resultType, ref object result)
		{
			if (fsSerializer.IsObjectDefinition(data))
			{
				int num = int.Parse(data.AsDictionary[fsSerializer.Key_ObjectDefinition].AsString);
				this._references.AddReferenceWithId(num, result);
			}
			return this.InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x0007DFB2 File Offset: 0x0007C1B2
		private fsResult InternalDeserialize_5_Converter(Type overrideConverterType, fsData data, Type resultType, ref object result)
		{
			if (fsSerializer.IsWrappedData(data))
			{
				data = data.AsDictionary[fsSerializer.Key_Content];
			}
			return this.GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
		}

		// Token: 0x04001602 RID: 5634
		private static HashSet<string> _reservedKeywords = new HashSet<string>
		{
			fsSerializer.Key_ObjectReference,
			fsSerializer.Key_ObjectDefinition,
			fsSerializer.Key_InstanceType,
			fsSerializer.Key_Version,
			fsSerializer.Key_Content
		};

		// Token: 0x04001603 RID: 5635
		private static readonly string Key_ObjectReference = string.Format("{0}ref", fsGlobalConfig.InternalFieldPrefix);

		// Token: 0x04001604 RID: 5636
		private static readonly string Key_ObjectDefinition = string.Format("{0}id", fsGlobalConfig.InternalFieldPrefix);

		// Token: 0x04001605 RID: 5637
		private static readonly string Key_InstanceType = string.Format("{0}type", fsGlobalConfig.InternalFieldPrefix);

		// Token: 0x04001606 RID: 5638
		private static readonly string Key_Version = string.Format("{0}version", fsGlobalConfig.InternalFieldPrefix);

		// Token: 0x04001607 RID: 5639
		private static readonly string Key_Content = string.Format("{0}content", fsGlobalConfig.InternalFieldPrefix);

		// Token: 0x04001608 RID: 5640
		private Dictionary<Type, fsBaseConverter> _cachedConverterTypeInstances;

		// Token: 0x04001609 RID: 5641
		private Dictionary<Type, fsBaseConverter> _cachedConverters;

		// Token: 0x0400160A RID: 5642
		private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;

		// Token: 0x0400160B RID: 5643
		private readonly List<fsConverter> _availableConverters;

		// Token: 0x0400160C RID: 5644
		private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;

		// Token: 0x0400160D RID: 5645
		private readonly List<fsObjectProcessor> _processors;

		// Token: 0x0400160E RID: 5646
		private readonly fsCyclicReferenceManager _references;

		// Token: 0x0400160F RID: 5647
		private readonly fsSerializer.fsLazyCycleDefinitionWriter _lazyReferenceWriter;

		// Token: 0x04001610 RID: 5648
		private readonly Dictionary<Type, Type> _abstractTypeRemap;

		// Token: 0x04001611 RID: 5649
		public fsContext Context;

		// Token: 0x04001612 RID: 5650
		public fsConfig Config;

		// Token: 0x02000C56 RID: 3158
		internal class fsLazyCycleDefinitionWriter
		{
			// Token: 0x06006C5C RID: 27740 RVA: 0x00306788 File Offset: 0x00304988
			public void WriteDefinition(int id, fsData data)
			{
				if (this._references.Contains(id))
				{
					fsSerializer.EnsureDictionary(data);
					data.AsDictionary[fsSerializer.Key_ObjectDefinition] = new fsData(id.ToString());
					return;
				}
				this._pendingDefinitions[id] = data;
			}

			// Token: 0x06006C5D RID: 27741 RVA: 0x003067C8 File Offset: 0x003049C8
			public void WriteReference(int id, Dictionary<string, fsData> dict)
			{
				if (this._pendingDefinitions.ContainsKey(id))
				{
					fsData fsData = this._pendingDefinitions[id];
					fsSerializer.EnsureDictionary(fsData);
					fsData.AsDictionary[fsSerializer.Key_ObjectDefinition] = new fsData(id.ToString());
					this._pendingDefinitions.Remove(id);
				}
				else
				{
					this._references.Add(id);
				}
				dict[fsSerializer.Key_ObjectReference] = new fsData(id.ToString());
			}

			// Token: 0x06006C5E RID: 27742 RVA: 0x00306843 File Offset: 0x00304A43
			public void Clear()
			{
				this._pendingDefinitions.Clear();
				this._references.Clear();
			}

			// Token: 0x04004E0D RID: 19981
			private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();

			// Token: 0x04004E0E RID: 19982
			private HashSet<int> _references = new HashSet<int>();
		}
	}
}
