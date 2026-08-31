using System;
using System.IO;

// Token: 0x02000454 RID: 1108
public static class FSSaveLoad
{
	// Token: 0x0600176C RID: 5996 RVA: 0x00079C30 File Offset: 0x00077E30
	public static void SavePrettyConfig(string path, object obj, Type type)
	{
		string text = StringSerializationAPI.SerializePretty(type, obj);
		File.WriteAllText(path, text);
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x00079C4C File Offset: 0x00077E4C
	public static void SavePretty(string path, object obj)
	{
		string text = StringSerializationAPI.SerializePretty(obj.GetType(), obj);
		File.WriteAllText(path, text);
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x00079C70 File Offset: 0x00077E70
	public static void SaveCompressedConfig(string path, object obj, Type type)
	{
		string text = StringSerializationAPI.SerializeCompressed(type, obj);
		File.WriteAllText(path, text);
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x00079C8C File Offset: 0x00077E8C
	public static object LoadConfig(string path, Type type)
	{
		if (File.Exists(path))
		{
			return Convert.ChangeType(StringSerializationAPI.Deserialize(type, File.ReadAllText(path)), type);
		}
		return null;
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x00079CAA File Offset: 0x00077EAA
	public static TIDataTemplate[] LoadDataTemplates(string path, Type type)
	{
		if (File.Exists(path))
		{
			return StringSerializationAPI.Deserialize(type, File.ReadAllText(path)) as TIDataTemplate[];
		}
		return null;
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x00079CC7 File Offset: 0x00077EC7
	public static TIDataTemplate[] LoadDataTemplatesFromString(string content, Type type)
	{
		if (!string.IsNullOrEmpty(content))
		{
			return StringSerializationAPI.Deserialize(type, content) as TIDataTemplate[];
		}
		return null;
	}
}
