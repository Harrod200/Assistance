using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x02000489 RID: 1161
	public static class fsTypeCache
	{
		// Token: 0x060018E8 RID: 6376 RVA: 0x00080724 File Offset: 0x0007E924
		static fsTypeCache()
		{
			Type typeFromHandle = typeof(fsTypeCache);
			lock (typeFromHandle)
			{
				fsTypeCache._assembliesByName = new Dictionary<string, Assembly>();
				fsTypeCache._assembliesByIndex = new List<Assembly>();
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					fsTypeCache._assembliesByName[assembly.FullName] = assembly;
					fsTypeCache._assembliesByIndex.Add(assembly);
				}
				fsTypeCache._cachedTypes = new Dictionary<string, Type>();
				AppDomain.CurrentDomain.AssemblyLoad += fsTypeCache.OnAssemblyLoaded;
			}
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x000807E0 File Offset: 0x0007E9E0
		private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			Type typeFromHandle = typeof(fsTypeCache);
			lock (typeFromHandle)
			{
				fsTypeCache._assembliesByName[args.LoadedAssembly.FullName] = args.LoadedAssembly;
				fsTypeCache._assembliesByIndex.Add(args.LoadedAssembly);
				fsTypeCache._cachedTypes = new Dictionary<string, Type>();
			}
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x00080854 File Offset: 0x0007EA54
		private static bool TryDirectTypeLookup(string assemblyName, string typeName, out Type type)
		{
			Assembly assembly;
			if (assemblyName != null && fsTypeCache._assembliesByName.TryGetValue(assemblyName, out assembly))
			{
				type = assembly.GetType(typeName, false);
				return type != null;
			}
			type = null;
			return false;
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x0008088C File Offset: 0x0007EA8C
		private static bool TryIndirectTypeLookup(string typeName, out Type type)
		{
			for (int i = 0; i < fsTypeCache._assembliesByIndex.Count; i++)
			{
				Assembly assembly = fsTypeCache._assembliesByIndex[i];
				type = assembly.GetType(typeName);
				if (type != null)
				{
					return true;
				}
			}
			for (int i = 0; i < fsTypeCache._assembliesByIndex.Count; i++)
			{
				foreach (Type type2 in fsTypeCache._assembliesByIndex[i].GetTypes())
				{
					if (type2.FullName == typeName)
					{
						type = type2;
						return true;
					}
				}
			}
			type = null;
			return false;
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x00080921 File Offset: 0x0007EB21
		public static void Reset()
		{
			fsTypeCache._cachedTypes = new Dictionary<string, Type>();
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x0008092D File Offset: 0x0007EB2D
		public static Type GetType(string name)
		{
			return fsTypeCache.GetType(name, null);
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x00080938 File Offset: 0x0007EB38
		public static Type GetType(string name, string assemblyHint)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Type typeFromHandle = typeof(fsTypeCache);
			Type type2;
			lock (typeFromHandle)
			{
				Type type;
				if (!fsTypeCache._cachedTypes.TryGetValue(name, out type))
				{
					if (!fsTypeCache.TryDirectTypeLookup(assemblyHint, name, out type))
					{
						fsTypeCache.TryIndirectTypeLookup(name, out type);
					}
					fsTypeCache._cachedTypes[name] = type;
				}
				type2 = type;
			}
			return type2;
		}

		// Token: 0x0400162D RID: 5677
		private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

		// Token: 0x0400162E RID: 5678
		private static Dictionary<string, Assembly> _assembliesByName;

		// Token: 0x0400162F RID: 5679
		private static List<Assembly> _assembliesByIndex;
	}
}
