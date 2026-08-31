using System;

// Token: 0x02000451 RID: 1105
public static class ParseEnum
{
	// Token: 0x06001765 RID: 5989 RVA: 0x000799C8 File Offset: 0x00077BC8
	public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
	{
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return defaultValue;
		}
		return (TEnum)((object)Enum.Parse(typeof(TEnum), strEnumValue));
	}
}
