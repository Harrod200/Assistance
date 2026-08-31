using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000035 RID: 53
public class TICondition
{
	// Token: 0x060001FA RID: 506 RVA: 0x0000FCD0 File Offset: 0x0000DED0
	public virtual ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.none;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000FCD3 File Offset: 0x0000DED3
	public virtual bool PassesCondition(TIGameState state)
	{
		return true;
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000FCD6 File Offset: 0x0000DED6
	public virtual bool TargetPassesCondition(TIGameState state, TIGameState targetedState)
	{
		return this.PassesCondition(targetedState);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000FCDF File Offset: 0x0000DEDF
	public bool IsValid()
	{
		return this.sign != ConditionSign.none && !string.IsNullOrEmpty(this.strValue);
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060001FE RID: 510 RVA: 0x0000FCF9 File Offset: 0x0000DEF9
	public virtual List<string> descriptionParams
	{
		get
		{
			return new List<string>();
		}
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000FD00 File Offset: 0x0000DF00
	public virtual string GetDescriptionPath()
	{
		return base.GetType().ToString();
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000FD0D File Offset: 0x0000DF0D
	public string GetDescriptionPathWithValue()
	{
		return new StringBuilder(base.GetType().ToString()).Append(this.strValue).ToString();
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000FD2F File Offset: 0x0000DF2F
	public string GetDescriptionPathWithSign()
	{
		return new StringBuilder(base.GetType().ToString()).Append(this.sign.ToString()).ToString();
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000FD5C File Offset: 0x0000DF5C
	public StringBuilder GetDescription()
	{
		string descriptionPath = this.GetDescriptionPath();
		object[] array = this.descriptionParams.ToArray();
		return new StringBuilder(Loc.T(descriptionPath, array));
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000203 RID: 515 RVA: 0x0000FD86 File Offset: 0x0000DF86
	public virtual string symbolResource
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000FD90 File Offset: 0x0000DF90
	public string GetNumericComparisonString(bool percent = false)
	{
		if (percent)
		{
			this.strValue = (TIUtilities.GetFloatValue(this.strValue) * 100f).ToPercent("P0");
		}
		return Loc.T(new StringBuilder("TICondition.Numeric.").Append(this.sign.ToString()).ToString(), new object[] { this.strValue });
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000FDFC File Offset: 0x0000DFFC
	public static bool PassesComparison(ConditionSign sign, double value1, double value2)
	{
		switch (sign)
		{
		case ConditionSign.EqualTo:
			return value1 == value2;
		case ConditionSign.NotEqualTo:
			return value1 != value2;
		case ConditionSign.GreaterThan:
			return value1 > value2;
		case ConditionSign.GreaterThanOrEqualTo:
			return value1 >= value2;
		case ConditionSign.LessThan:
			return value1 < value2;
		case ConditionSign.LessThanOrEqualTo:
			return value1 <= value2;
		default:
			Log.Error("Bad condition sign passed to PassesNumericCondition", Array.Empty<object>());
			return false;
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000FE64 File Offset: 0x0000E064
	public static bool PassesComparison(ConditionSign sign, float value1, float value2)
	{
		switch (sign)
		{
		case ConditionSign.EqualTo:
			return value1 == value2;
		case ConditionSign.NotEqualTo:
			return value1 != value2;
		case ConditionSign.GreaterThan:
			return value1 > value2;
		case ConditionSign.GreaterThanOrEqualTo:
			return value1 >= value2;
		case ConditionSign.LessThan:
			return value1 < value2;
		case ConditionSign.LessThanOrEqualTo:
			return value1 <= value2;
		default:
			Log.Error("Bad condition sign passed to PassesNumericCondition", Array.Empty<object>());
			return false;
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000FECC File Offset: 0x0000E0CC
	public static bool PassesComparison(ConditionSign sign, int value1, int value2)
	{
		switch (sign)
		{
		case ConditionSign.EqualTo:
			return value1 == value2;
		case ConditionSign.NotEqualTo:
			return value1 != value2;
		case ConditionSign.GreaterThan:
			return value1 > value2;
		case ConditionSign.GreaterThanOrEqualTo:
			return value1 >= value2;
		case ConditionSign.LessThan:
			return value1 < value2;
		case ConditionSign.LessThanOrEqualTo:
			return value1 <= value2;
		default:
			Log.Error("Bad condition sign passed to PassesNumericCondition", Array.Empty<object>());
			return false;
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000FF32 File Offset: 0x0000E132
	public static bool PassesComparison(ConditionSign sign, bool bvalue1, bool bvalue2)
	{
		if (sign == ConditionSign.EqualTo)
		{
			return bvalue1 == bvalue2;
		}
		if (sign != ConditionSign.NotEqualTo)
		{
			Log.Error("Bad condition sign passed to PassesBoolComparison", Array.Empty<object>());
			return false;
		}
		return bvalue1 != bvalue2;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000FF5B File Offset: 0x0000E15B
	public static bool PassesComparison(ConditionSign sign, string sValue1, string sValue2)
	{
		if (sign == ConditionSign.EqualTo)
		{
			return sValue1 == sValue2;
		}
		if (sign != ConditionSign.NotEqualTo)
		{
			Log.Error("Bad condition sign passed to PassesEqualityComparison", Array.Empty<object>());
			return false;
		}
		return sValue1 != sValue2;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000FF87 File Offset: 0x0000E187
	public static bool PassesComparison<T>(ConditionSign sign, T sValue1, T sValue2) where T : class
	{
		if (sign == ConditionSign.EqualTo)
		{
			return sValue1 == sValue2;
		}
		if (sign != ConditionSign.NotEqualTo)
		{
			Log.Error("Bad condition sign passed to PassesEqualityComparison", Array.Empty<object>());
			return false;
		}
		return sValue1 != sValue2;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
	public static bool PassesComparison<T>(ConditionSign sign, T item, IList<T> collection)
	{
		if (item == null)
		{
			throw new ArgumentException("Missing item", "item");
		}
		if (collection == null)
		{
			throw new ArgumentException("Missing collection", "collection");
		}
		switch (sign)
		{
		case ConditionSign.Has:
			return collection.Contains(item);
		case ConditionSign.DoesNotHave:
			return !collection.Contains(item);
		case ConditionSign.OnlyHas:
			return collection.Contains(item) && collection.Count == 1;
		default:
			Log.Error("Bad condition sign passed to PassesCollectionComparison", Array.Empty<object>());
			return false;
		}
	}

	// Token: 0x0400020B RID: 523
	public string strIdx;

	// Token: 0x0400020C RID: 524
	public ConditionSign sign;

	// Token: 0x0400020D RID: 525
	public string strValue;

	// Token: 0x0400020E RID: 526
	public const string pass = "_Pass";
}
