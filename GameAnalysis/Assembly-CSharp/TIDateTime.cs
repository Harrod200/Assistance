using System;
using System.Globalization;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200014C RID: 332
[Serializable]
public class TIDateTime : IComparable
{
	// Token: 0x060004E8 RID: 1256 RVA: 0x00015E9A File Offset: 0x0001409A
	public TIDateTime()
	{
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00015EA2 File Offset: 0x000140A2
	public TIDateTime(DateTime time)
	{
		this.ImportTime(time);
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00015EB1 File Offset: 0x000140B1
	public TIDateTime(TIDateTime initTime)
	{
		this.CopyDateTime(initTime);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00015EC0 File Offset: 0x000140C0
	public TIDateTime(TIDateTime initTime, double adjustment_s)
	{
		this.CopyDateTime(initTime);
		this.AddSeconds(adjustment_s);
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00015ED6 File Offset: 0x000140D6
	public TIDateTime(DateTime dt, double adjustment_s)
	{
		this.ImportTime(dt);
		this.AddSeconds(adjustment_s);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00015EEC File Offset: 0x000140EC
	public TIDateTime(int year, int month, int day)
	{
		this.year = year;
		this.month = month;
		this.day = day;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00015F09 File Offset: 0x00014109
	public TIDateTime(int year, int month, int day, int hour, int minute)
	{
		this.year = year;
		this.month = month;
		this.day = day;
		this.hour = hour;
		this.minute = minute;
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00015F38 File Offset: 0x00014138
	public void ImportTime(DateTime dt)
	{
		this.year = dt.Year;
		this.month = dt.Month;
		this.day = dt.Day;
		this.hour = dt.Hour;
		this.minute = dt.Minute;
		this.second = dt.Second;
		this.millisecond = dt.Millisecond;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00015FA0 File Offset: 0x000141A0
	public DateTime ExportTime()
	{
		if (this.year == 0 && this.month == 0 && this.day == 0 && this.hour == 0 && this.minute == 0 && this.second == 0 && this.millisecond == 0)
		{
			return DateTime.MinValue;
		}
		return new DateTime(this.year, this.month, this.day, this.hour, this.minute, this.second, this.millisecond);
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0001601C File Offset: 0x0001421C
	public void AddSeconds(double number = 1.0)
	{
		DateTime dateTime = this.ExportTime();
		if ((DateTime.MaxValue - dateTime).TotalSeconds > number && (DateTime.MinValue - dateTime).TotalSeconds < number)
		{
			this.ImportTime(dateTime.AddSeconds(number));
			return;
		}
		Log.Error("Date would have exceeded capacity of DateTime.  Was adding " + number.ToString() + " seconds to " + ((this != null) ? this.ToString() : null), Array.Empty<object>());
		if (number > 0.0)
		{
			this.ImportTime(DateTime.MaxValue);
			return;
		}
		this.ImportTime(DateTime.MinValue);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x000160BC File Offset: 0x000142BC
	public void AddHours(double number = 1.0)
	{
		this.ImportTime(this.ExportTime().AddHours(number));
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x000160E0 File Offset: 0x000142E0
	public void AddDays(float number = 1f)
	{
		this.ImportTime(this.ExportTime().AddDays((double)number));
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00016104 File Offset: 0x00014304
	public bool TryAddDays(float number = 1f)
	{
		bool flag;
		try
		{
			this.AddDays(number);
			flag = true;
		}
		catch
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00016134 File Offset: 0x00014334
	public TIDateTime DaysAdded(float number)
	{
		TIDateTime tidateTime = new TIDateTime(this);
		tidateTime.AddDays(number);
		return tidateTime;
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00016144 File Offset: 0x00014344
	public void AddMonths(int number = 1)
	{
		this.ImportTime(this.ExportTime().AddMonths(number));
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00016168 File Offset: 0x00014368
	public void AddYears(int number = 1)
	{
		this.ImportTime(this.ExportTime().AddYears(number));
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001618C File Offset: 0x0001438C
	public void AddMilliseconds(int number = 1)
	{
		this.ImportTime(this.ExportTime().AddMilliseconds((double)number));
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x000161AF File Offset: 0x000143AF
	public void CopyDateTime(TIDateTime newDateTime)
	{
		this.ImportTime(newDateTime.ExportTime());
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x000161BD File Offset: 0x000143BD
	public void SetTime(int newYear, int newMonth, int newDay, int newHour = 0, int newMinute = 0, int newSecond = 0, int newMillisec = 0)
	{
		this.year = newYear;
		this.month = newMonth;
		this.day = newDay;
		this.hour = newHour;
		this.minute = newMinute;
		this.second = newSecond;
		this.millisecond = newMillisec;
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x000161F4 File Offset: 0x000143F4
	public TIDateTime SetTime(double julianEpoch)
	{
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		DateTime dateTime = new DateTime(1, 1, 1);
		dateTime = gregorianCalendar.AddYears(dateTime, (int)julianEpoch - 1);
		double num = julianEpoch - (double)((int)julianEpoch);
		dateTime = gregorianCalendar.AddSeconds(dateTime, (int)(num * 31557600.0));
		dateTime = gregorianCalendar.AddHours(dateTime, 12);
		this.ImportTime(dateTime);
		return this;
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00016248 File Offset: 0x00014448
	public double ToJulianDate()
	{
		return this.ExportTime().ToOADate() + 2415018.5;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001626D File Offset: 0x0001446D
	public double ToJulianDateInSeconds()
	{
		return this.ToJulianDate() * 24.0 * 60.0 * 60.0;
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00016293 File Offset: 0x00014493
	public double ToJulianEpoch()
	{
		return this.ToJulianDateInSeconds() / 31557600.0;
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x000162A8 File Offset: 0x000144A8
	public double DifferenceInMillis(TIDateTime toSubtract)
	{
		return (this.ExportTime() - toSubtract.ExportTime()).TotalMilliseconds;
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x000162D0 File Offset: 0x000144D0
	public double DifferenceInSeconds(TIDateTime toSubtract)
	{
		return (this.ExportTime() - toSubtract.ExportTime()).TotalSeconds;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x000162F8 File Offset: 0x000144F8
	public double DifferenceInHours(TIDateTime toSubtract)
	{
		return (this.ExportTime() - toSubtract.ExportTime()).TotalHours;
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00016320 File Offset: 0x00014520
	public double DifferenceInDays(TIDateTime toSubtract)
	{
		return (this.ExportTime() - toSubtract.ExportTime()).TotalDays;
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00016348 File Offset: 0x00014548
	public double DifferenceInJulianYears(TIDateTime toSubtract)
	{
		return (this.ExportTime() - toSubtract.ExportTime()).TotalDays / 365.2421875;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00016378 File Offset: 0x00014578
	public string ToShortDateString()
	{
		return this.ExportTime().ToShortDateString();
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00016394 File Offset: 0x00014594
	public string ToCustomDateString()
	{
		return new StringBuilder(this.ExportTime().ToString("dd ")).Append(TIDateTime.GetMonthString(this.month)).Append(this.ExportTime().ToString(" yyyy")).ToString();
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x000163E8 File Offset: 0x000145E8
	public string ToLongDateString()
	{
		return this.ExportTime().ToLongDateString();
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00016404 File Offset: 0x00014604
	public string ToLongTimeString()
	{
		return this.ExportTime().ToLongTimeString();
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00016420 File Offset: 0x00014620
	public string ToShortTimeString()
	{
		return this.ExportTime().ToShortTimeString();
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001643C File Offset: 0x0001463C
	public string ToCustomTimeString()
	{
		return this.ExportTime().ToString("HH:mm:ss");
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001645C File Offset: 0x0001465C
	public string ToCustomTimeDateString()
	{
		return new StringBuilder(this.ExportTime().ToString("HH:mm dd ")).Append(TIDateTime.GetMonthString(this.month)).Append(this.ExportTime().ToString(" yyyy")).ToString();
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x000164AE File Offset: 0x000146AE
	public static string GetMonthString(int monthIdx)
	{
		return Loc.T(new StringBuilder("UI.Global.Month").Append(monthIdx).ToString());
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x000164CC File Offset: 0x000146CC
	public override string ToString()
	{
		return this.ExportTime().ToString();
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x000164E8 File Offset: 0x000146E8
	public string ToString(string param)
	{
		return this.ExportTime().ToString(param);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00016504 File Offset: 0x00014704
	public static bool operator <(TIDateTime val1, TIDateTime val2)
	{
		return !(val1 == null) && !(val2 == null) && val1.ExportTime() < val2.ExportTime();
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001652B File Offset: 0x0001472B
	public static bool operator <=(TIDateTime val1, TIDateTime val2)
	{
		return !(val1 == null) && !(val2 == null) && val1.ExportTime() <= val2.ExportTime();
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00016552 File Offset: 0x00014752
	public static bool operator >(TIDateTime val1, TIDateTime val2)
	{
		return !(val1 == null) && !(val2 == null) && val1.ExportTime() > val2.ExportTime();
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x00016579 File Offset: 0x00014779
	public static bool operator >=(TIDateTime val1, TIDateTime val2)
	{
		return !(val1 == null) && !(val2 == null) && val1.ExportTime() >= val2.ExportTime();
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x000165A0 File Offset: 0x000147A0
	public static bool operator ==(TIDateTime val1, TIDateTime val2)
	{
		if (val1 == null)
		{
			return val2 == null;
		}
		return !(val2 == null) && val1.ExportTime() == val2.ExportTime();
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x000165C6 File Offset: 0x000147C6
	public static TimeSpan operator -(TIDateTime val1, TIDateTime val2)
	{
		if (val1 == null || val2 == null)
		{
			throw new Exception();
		}
		return val1.ExportTime() - val2.ExportTime();
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x000165F1 File Offset: 0x000147F1
	public static bool operator !=(TIDateTime val1, TIDateTime val2)
	{
		return !(val1 == val2);
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x00016600 File Offset: 0x00014800
	public override bool Equals(object obj)
	{
		TIDateTime tidateTime = obj as TIDateTime;
		return tidateTime != null && tidateTime == this;
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x00016628 File Offset: 0x00014828
	public override int GetHashCode()
	{
		return this.ExportTime().GetHashCode();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00016644 File Offset: 0x00014844
	public int CompareTo(object obj)
	{
		TIDateTime tidateTime = obj as TIDateTime;
		if (tidateTime == null)
		{
			return 1;
		}
		return this.ExportTime().CompareTo(tidateTime.ExportTime());
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x00016671 File Offset: 0x00014871
	public static TIDateTime Min(TIDateTime a, TIDateTime b)
	{
		if (!(a < b))
		{
			return b;
		}
		return a;
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001667F File Offset: 0x0001487F
	public static TIDateTime Max(TIDateTime a, TIDateTime b)
	{
		if (!(a > b))
		{
			return b;
		}
		return a;
	}

	// Token: 0x0400022A RID: 554
	public int year;

	// Token: 0x0400022B RID: 555
	public int month;

	// Token: 0x0400022C RID: 556
	public int day;

	// Token: 0x0400022D RID: 557
	public int hour;

	// Token: 0x0400022E RID: 558
	public int minute;

	// Token: 0x0400022F RID: 559
	public int second;

	// Token: 0x04000230 RID: 560
	public int millisecond;

	// Token: 0x04000231 RID: 561
	private const float GregorianYear_s = 31556952f;

	// Token: 0x04000232 RID: 562
	private const float JulianYear_s = 31557600f;
}
