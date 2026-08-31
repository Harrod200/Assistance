using System;
using FullSerializer;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000455 RID: 1109
public static class StringSerializationAPI
{
	// Token: 0x06001772 RID: 6002 RVA: 0x00079CDF File Offset: 0x00077EDF
	public static string SerializePretty(Type type, object value)
	{
		return fsJsonPrinter.PrettyJson(StringSerializationAPI.Serialize(type, value));
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x00079CED File Offset: 0x00077EED
	public static string SerializeCompressed(Type type, object value)
	{
		return fsJsonPrinter.CompressedJson(StringSerializationAPI.Serialize(type, value));
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x00079CFC File Offset: 0x00077EFC
	public static object Deserialize(Type type, string serializedState)
	{
		fsData fsData = fsJsonParser.Parse(serializedState);
		object obj = null;
		try
		{
			StringSerializationAPI._serializer.TryDeserialize(fsData, type, ref obj).AssertSuccess();
		}
		finally
		{
			TIGameStateConverter.Reset();
		}
		return obj;
	}

	// Token: 0x06001775 RID: 6005 RVA: 0x00079D44 File Offset: 0x00077F44
	public static fsData Serialize(Type type, object value)
	{
		fsData fsData;
		try
		{
			StringSerializationAPI._serializer.TrySerialize(type, value, out fsData).AssertSuccess();
		}
		finally
		{
			TIGameStateConverter.Reset();
		}
		return fsData;
	}

	// Token: 0x040015BC RID: 5564
	private static readonly fsSerializer _serializer = new fsSerializer();
}
