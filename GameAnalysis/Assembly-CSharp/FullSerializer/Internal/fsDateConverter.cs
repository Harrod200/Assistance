using System;
using System.Globalization;

namespace FullSerializer.Internal
{
	// Token: 0x02000475 RID: 1141
	public class fsDateConverter : fsConverter
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x0600184C RID: 6220 RVA: 0x0007E161 File Offset: 0x0007C361
		private string DateTimeFormatString
		{
			get
			{
				return this.Serializer.Config.CustomDateTimeFormatString ?? "o";
			}
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x0007E17C File Offset: 0x0007C37C
		public override bool CanProcess(Type type)
		{
			return type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x0007E1B4 File Offset: 0x0007C3B4
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			if (instance is DateTime)
			{
				serialized = new fsData(((DateTime)instance).ToString(this.DateTimeFormatString));
				return fsResult.Success;
			}
			if (instance is DateTimeOffset)
			{
				serialized = new fsData(((DateTimeOffset)instance).ToString("o"));
				return fsResult.Success;
			}
			if (instance is TimeSpan)
			{
				serialized = new fsData(((TimeSpan)instance).ToString());
				return fsResult.Success;
			}
			throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected serialization type");
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x0007E248 File Offset: 0x0007C448
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (!data.IsString)
			{
				return fsResult.Fail("Date deserialization requires a string, not " + data.Type.ToString());
			}
			if (storageType == typeof(DateTime))
			{
				DateTime dateTime;
				if (DateTime.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out dateTime))
				{
					instance = dateTime;
					return fsResult.Success;
				}
				if (fsGlobalConfig.AllowInternalExceptions)
				{
					try
					{
						instance = Convert.ToDateTime(data.AsString);
						return fsResult.Success;
					}
					catch (Exception ex)
					{
						string text = "Unable to parse ";
						string asString = data.AsString;
						string text2 = " into a DateTime; got exception ";
						Exception ex2 = ex;
						return fsResult.Fail(text + asString + text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
				}
				return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTime");
			}
			else if (storageType == typeof(DateTimeOffset))
			{
				DateTimeOffset dateTimeOffset;
				if (DateTimeOffset.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out dateTimeOffset))
				{
					instance = dateTimeOffset;
					return fsResult.Success;
				}
				return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTimeOffset");
			}
			else
			{
				if (!(storageType == typeof(TimeSpan)))
				{
					throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected deserialization type");
				}
				TimeSpan timeSpan;
				if (TimeSpan.TryParse(data.AsString, out timeSpan))
				{
					instance = timeSpan;
					return fsResult.Success;
				}
				return fsResult.Fail("Unable to parse " + data.AsString + " into a TimeSpan");
			}
		}

		// Token: 0x04001613 RID: 5651
		private const string DefaultDateTimeFormatString = "o";

		// Token: 0x04001614 RID: 5652
		private const string DateTimeOffsetFormatString = "o";
	}
}
