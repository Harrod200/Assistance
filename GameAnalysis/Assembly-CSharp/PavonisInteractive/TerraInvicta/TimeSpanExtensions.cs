using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007FB RID: 2043
	public static class TimeSpanExtensions
	{
		// Token: 0x06004A2F RID: 18991 RVA: 0x001F2400 File Offset: 0x001F0600
		public static TimeSpan Seconds(this int value)
		{
			return TimeSpan.FromSeconds((double)value);
		}

		// Token: 0x06004A30 RID: 18992 RVA: 0x001F2409 File Offset: 0x001F0609
		public static TimeSpan Minutes(this int value)
		{
			return TimeSpan.FromMinutes((double)value);
		}

		// Token: 0x06004A31 RID: 18993 RVA: 0x001F2412 File Offset: 0x001F0612
		public static TimeSpan Hours(this int value)
		{
			return TimeSpan.FromHours((double)value);
		}

		// Token: 0x06004A32 RID: 18994 RVA: 0x001F241B File Offset: 0x001F061B
		public static TimeSpan Days(this int value)
		{
			return TimeSpan.FromDays((double)value);
		}

		// Token: 0x06004A33 RID: 18995 RVA: 0x001F2424 File Offset: 0x001F0624
		public static TimeSpan Seconds(this double value)
		{
			return TimeSpan.FromSeconds(value);
		}

		// Token: 0x06004A34 RID: 18996 RVA: 0x001F242C File Offset: 0x001F062C
		public static TimeSpan Minutes(this double value)
		{
			return TimeSpan.FromMinutes(value);
		}

		// Token: 0x06004A35 RID: 18997 RVA: 0x001F2434 File Offset: 0x001F0634
		public static TimeSpan Hours(this double value)
		{
			return TimeSpan.FromHours(value);
		}

		// Token: 0x06004A36 RID: 18998 RVA: 0x001F243C File Offset: 0x001F063C
		public static TimeSpan Days(this double value)
		{
			return TimeSpan.FromDays(value);
		}
	}
}
