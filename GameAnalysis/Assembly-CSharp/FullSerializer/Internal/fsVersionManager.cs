using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x02000485 RID: 1157
	public static class fsVersionManager
	{
		// Token: 0x060018C6 RID: 6342 RVA: 0x0008000C File Offset: 0x0007E20C
		public static fsResult GetVersionImportPath(string currentVersion, fsVersionedType targetVersion, out List<fsVersionedType> path)
		{
			path = new List<fsVersionedType>();
			if (!fsVersionManager.GetVersionImportPathRecursive(path, currentVersion, targetVersion))
			{
				return fsResult.Fail(string.Concat(new string[] { "There is no migration path from \"", currentVersion, "\" to \"", targetVersion.VersionString, "\"" }));
			}
			path.Add(targetVersion);
			return fsResult.Success;
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x00080070 File Offset: 0x0007E270
		private static bool GetVersionImportPathRecursive(List<fsVersionedType> path, string currentVersion, fsVersionedType current)
		{
			for (int i = 0; i < current.Ancestors.Length; i++)
			{
				fsVersionedType fsVersionedType = current.Ancestors[i];
				if (fsVersionedType.VersionString == currentVersion || fsVersionManager.GetVersionImportPathRecursive(path, currentVersion, fsVersionedType))
				{
					path.Add(fsVersionedType);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x000800C0 File Offset: 0x0007E2C0
		public static fsOption<fsVersionedType> GetVersionedType(Type type)
		{
			fsOption<fsVersionedType> fsOption;
			if (!fsVersionManager._cache.TryGetValue(type, out fsOption))
			{
				fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
				if (attribute != null && (!string.IsNullOrEmpty(attribute.VersionString) || attribute.PreviousModels != null))
				{
					if (attribute.PreviousModels != null && string.IsNullOrEmpty(attribute.VersionString))
					{
						throw new Exception("fsObject attribute on " + ((type != null) ? type.ToString() : null) + " contains a PreviousModels specifier - it must also include a VersionString modifier");
					}
					fsVersionedType[] array = new fsVersionedType[(attribute.PreviousModels != null) ? attribute.PreviousModels.Length : 0];
					for (int i = 0; i < array.Length; i++)
					{
						fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(attribute.PreviousModels[i]);
						if (versionedType.IsEmpty)
						{
							throw new Exception("Unable to create versioned type for ancestor " + versionedType.ToString() + "; please add an [fsObject(VersionString=\"...\")] attribute");
						}
						array[i] = versionedType.Value;
					}
					fsVersionedType fsVersionedType = new fsVersionedType
					{
						Ancestors = array,
						VersionString = attribute.VersionString,
						ModelType = type
					};
					fsVersionManager.VerifyUniqueVersionStrings(fsVersionedType);
					fsVersionManager.VerifyConstructors(fsVersionedType);
					fsOption = fsOption.Just<fsVersionedType>(fsVersionedType);
				}
				fsVersionManager._cache[type] = fsOption;
			}
			return fsOption;
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x000801F0 File Offset: 0x0007E3F0
		private static void VerifyConstructors(fsVersionedType type)
		{
			ConstructorInfo[] declaredConstructors = type.ModelType.GetDeclaredConstructors();
			for (int i = 0; i < type.Ancestors.Length; i++)
			{
				Type modelType = type.Ancestors[i].ModelType;
				bool flag = false;
				for (int j = 0; j < declaredConstructors.Length; j++)
				{
					ParameterInfo[] parameters = declaredConstructors[j].GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == modelType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new fsMissingVersionConstructorException(type.ModelType, modelType);
				}
			}
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0008027C File Offset: 0x0007E47C
		private static void VerifyUniqueVersionStrings(fsVersionedType type)
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Queue<fsVersionedType> queue = new Queue<fsVersionedType>();
			queue.Enqueue(type);
			while (queue.Count > 0)
			{
				fsVersionedType fsVersionedType = queue.Dequeue();
				if (dictionary.ContainsKey(fsVersionedType.VersionString) && dictionary[fsVersionedType.VersionString] != fsVersionedType.ModelType)
				{
					throw new fsDuplicateVersionNameException(dictionary[fsVersionedType.VersionString], fsVersionedType.ModelType, fsVersionedType.VersionString);
				}
				dictionary[fsVersionedType.VersionString] = fsVersionedType.ModelType;
				foreach (fsVersionedType fsVersionedType2 in fsVersionedType.Ancestors)
				{
					queue.Enqueue(fsVersionedType2);
				}
			}
		}

		// Token: 0x04001620 RID: 5664
		private static readonly Dictionary<Type, fsOption<fsVersionedType>> _cache = new Dictionary<Type, fsOption<fsVersionedType>>();
	}
}
