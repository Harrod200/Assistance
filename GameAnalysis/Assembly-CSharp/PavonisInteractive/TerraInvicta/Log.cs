using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Util;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200090A RID: 2314
	public static class Log
	{
		// Token: 0x17000F2F RID: 3887
		// (get) Token: 0x0600587E RID: 22654 RVA: 0x00289565 File Offset: 0x00287765
		// (set) Token: 0x0600587F RID: 22655 RVA: 0x0028956C File Offset: 0x0028776C
		public static ILog logger { get; private set; } = new Log.NullLogger();

		// Token: 0x06005881 RID: 22657 RVA: 0x00289580 File Offset: 0x00287780
		internal static void Initialize()
		{
			if (Log.initialized)
			{
				return;
			}
			global::UnityEngine.Debug.Log("Initializing Logging");
			string text = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Logs");
			string text2 = Path.Combine(text, "TerraInvicta.log");
			Directory.CreateDirectory(text);
			GlobalContext.Properties["MainLogPath"] = text2;
			XmlConfigurator.ConfigureAndWatch(new FileInfo(Application.streamingAssetsPath + "/Config/log4net.xml"));
			if (!LogManager.GetRepository().Configured)
			{
				foreach (LogLog logLog in LogManager.GetRepository().ConfigurationMessages.Cast<LogLog>())
				{
					global::UnityEngine.Debug.LogError(logLog.Message);
					if (logLog.Exception != null)
					{
						global::UnityEngine.Debug.LogException(logLog.Exception);
					}
				}
			}
			global::UnityEngine.Debug.Log("Log Config Loaded");
			Log.logger = LogManager.GetLogger("Main");
			if (Log.logger != null)
			{
				Log.initialized = true;
				Log.logger.Info("Logger ready");
				return;
			}
			global::UnityEngine.Debug.Log("Failed to initialize logger");
		}

		// Token: 0x06005882 RID: 22658 RVA: 0x0028969C File Offset: 0x0028789C
		public static void Fatal(string message, params object[] args)
		{
			if (Log.logger.IsFatalEnabled)
			{
				Log.logger.FatalFormat(message, args);
			}
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x002896B6 File Offset: 0x002878B6
		public static void Error(string message, params object[] args)
		{
			if (Log.logger.IsErrorEnabled)
			{
				Log.logger.ErrorFormat(message, args);
			}
		}

		// Token: 0x06005884 RID: 22660 RVA: 0x002896D0 File Offset: 0x002878D0
		public static void Warn(string message, params object[] args)
		{
			if (Log.logger.IsWarnEnabled)
			{
				Log.logger.WarnFormat(message, args);
			}
		}

		// Token: 0x06005885 RID: 22661 RVA: 0x002896EA File Offset: 0x002878EA
		public static void Info(string message, params object[] args)
		{
			if (Log.logger.IsInfoEnabled)
			{
				Log.logger.InfoFormat(message, args);
			}
		}

		// Token: 0x06005886 RID: 22662 RVA: 0x00289704 File Offset: 0x00287904
		public static void Debug(string message, params object[] args)
		{
			if (Log.logger.IsDebugEnabled)
			{
				Log.logger.DebugFormat(message, args);
			}
		}

		// Token: 0x06005887 RID: 22663 RVA: 0x00289720 File Offset: 0x00287920
		public static void Time(string name, Action action, bool verboseLog = true, bool finalLog = true)
		{
			float realtimeSinceStartup = global::UnityEngine.Time.realtimeSinceStartup;
			action();
			float realtimeSinceStartup2 = global::UnityEngine.Time.realtimeSinceStartup;
			if (verboseLog || finalLog)
			{
				Log.Time(name, realtimeSinceStartup2 - realtimeSinceStartup, verboseLog, finalLog);
			}
		}

		// Token: 0x06005888 RID: 22664 RVA: 0x00289750 File Offset: 0x00287950
		private static void Time(string name, float time, bool verboseLog, bool finalLog)
		{
			string text = "s";
			if (time < 1f)
			{
				text = "ms";
				time *= 1000f;
			}
			if (verboseLog)
			{
				Log.Info("{0}: {1:F3}{2}", new object[] { name, time, text });
			}
			if (finalLog)
			{
				Log.clockLog = Log.clockLog + string.Format("{0}: {1:F3}{2}", name, time, text) + "\n";
			}
		}

		// Token: 0x04004057 RID: 16471
		private static bool initialized;

		// Token: 0x04004058 RID: 16472
		public static string clockLog;

		// Token: 0x020011EF RID: 4591
		private class NullLogger : ILog, ILoggerWrapper
		{
			// Token: 0x170012B1 RID: 4785
			// (get) Token: 0x060088DC RID: 35036 RVA: 0x00336FC8 File Offset: 0x003351C8
			public bool IsDebugEnabled
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170012B2 RID: 4786
			// (get) Token: 0x060088DD RID: 35037 RVA: 0x00336FCB File Offset: 0x003351CB
			public bool IsInfoEnabled
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170012B3 RID: 4787
			// (get) Token: 0x060088DE RID: 35038 RVA: 0x00336FCE File Offset: 0x003351CE
			public bool IsWarnEnabled
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170012B4 RID: 4788
			// (get) Token: 0x060088DF RID: 35039 RVA: 0x00336FD1 File Offset: 0x003351D1
			public bool IsErrorEnabled
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170012B5 RID: 4789
			// (get) Token: 0x060088E0 RID: 35040 RVA: 0x00336FD4 File Offset: 0x003351D4
			public bool IsFatalEnabled
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170012B6 RID: 4790
			// (get) Token: 0x060088E1 RID: 35041 RVA: 0x00336FD7 File Offset: 0x003351D7
			public global::log4net.Core.ILogger Logger
			{
				get
				{
					return null;
				}
			}

			// Token: 0x060088E2 RID: 35042 RVA: 0x00336FDA File Offset: 0x003351DA
			public void Debug(object message)
			{
			}

			// Token: 0x060088E3 RID: 35043 RVA: 0x00336FDC File Offset: 0x003351DC
			public void Debug(object message, Exception exception)
			{
			}

			// Token: 0x060088E4 RID: 35044 RVA: 0x00336FDE File Offset: 0x003351DE
			public void DebugFormat(string format, params object[] args)
			{
			}

			// Token: 0x060088E5 RID: 35045 RVA: 0x00336FE0 File Offset: 0x003351E0
			public void DebugFormat(string format, object arg0)
			{
			}

			// Token: 0x060088E6 RID: 35046 RVA: 0x00336FE2 File Offset: 0x003351E2
			public void DebugFormat(string format, object arg0, object arg1)
			{
			}

			// Token: 0x060088E7 RID: 35047 RVA: 0x00336FE4 File Offset: 0x003351E4
			public void DebugFormat(string format, object arg0, object arg1, object arg2)
			{
			}

			// Token: 0x060088E8 RID: 35048 RVA: 0x00336FE6 File Offset: 0x003351E6
			public void DebugFormat(IFormatProvider provider, string format, params object[] args)
			{
			}

			// Token: 0x060088E9 RID: 35049 RVA: 0x00336FE8 File Offset: 0x003351E8
			public void Error(object message)
			{
			}

			// Token: 0x060088EA RID: 35050 RVA: 0x00336FEA File Offset: 0x003351EA
			public void Error(object message, Exception exception)
			{
			}

			// Token: 0x060088EB RID: 35051 RVA: 0x00336FEC File Offset: 0x003351EC
			public void ErrorFormat(string format, params object[] args)
			{
			}

			// Token: 0x060088EC RID: 35052 RVA: 0x00336FEE File Offset: 0x003351EE
			public void ErrorFormat(string format, object arg0)
			{
			}

			// Token: 0x060088ED RID: 35053 RVA: 0x00336FF0 File Offset: 0x003351F0
			public void ErrorFormat(string format, object arg0, object arg1)
			{
			}

			// Token: 0x060088EE RID: 35054 RVA: 0x00336FF2 File Offset: 0x003351F2
			public void ErrorFormat(string format, object arg0, object arg1, object arg2)
			{
			}

			// Token: 0x060088EF RID: 35055 RVA: 0x00336FF4 File Offset: 0x003351F4
			public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
			{
			}

			// Token: 0x060088F0 RID: 35056 RVA: 0x00336FF6 File Offset: 0x003351F6
			public void Fatal(object message)
			{
			}

			// Token: 0x060088F1 RID: 35057 RVA: 0x00336FF8 File Offset: 0x003351F8
			public void Fatal(object message, Exception exception)
			{
			}

			// Token: 0x060088F2 RID: 35058 RVA: 0x00336FFA File Offset: 0x003351FA
			public void FatalFormat(string format, params object[] args)
			{
			}

			// Token: 0x060088F3 RID: 35059 RVA: 0x00336FFC File Offset: 0x003351FC
			public void FatalFormat(string format, object arg0)
			{
			}

			// Token: 0x060088F4 RID: 35060 RVA: 0x00336FFE File Offset: 0x003351FE
			public void FatalFormat(string format, object arg0, object arg1)
			{
			}

			// Token: 0x060088F5 RID: 35061 RVA: 0x00337000 File Offset: 0x00335200
			public void FatalFormat(string format, object arg0, object arg1, object arg2)
			{
			}

			// Token: 0x060088F6 RID: 35062 RVA: 0x00337002 File Offset: 0x00335202
			public void FatalFormat(IFormatProvider provider, string format, params object[] args)
			{
			}

			// Token: 0x060088F7 RID: 35063 RVA: 0x00337004 File Offset: 0x00335204
			public void Info(object message)
			{
			}

			// Token: 0x060088F8 RID: 35064 RVA: 0x00337006 File Offset: 0x00335206
			public void Info(object message, Exception exception)
			{
			}

			// Token: 0x060088F9 RID: 35065 RVA: 0x00337008 File Offset: 0x00335208
			public void InfoFormat(string format, params object[] args)
			{
			}

			// Token: 0x060088FA RID: 35066 RVA: 0x0033700A File Offset: 0x0033520A
			public void InfoFormat(string format, object arg0)
			{
			}

			// Token: 0x060088FB RID: 35067 RVA: 0x0033700C File Offset: 0x0033520C
			public void InfoFormat(string format, object arg0, object arg1)
			{
			}

			// Token: 0x060088FC RID: 35068 RVA: 0x0033700E File Offset: 0x0033520E
			public void InfoFormat(string format, object arg0, object arg1, object arg2)
			{
			}

			// Token: 0x060088FD RID: 35069 RVA: 0x00337010 File Offset: 0x00335210
			public void InfoFormat(IFormatProvider provider, string format, params object[] args)
			{
			}

			// Token: 0x060088FE RID: 35070 RVA: 0x00337012 File Offset: 0x00335212
			public void Warn(object message)
			{
			}

			// Token: 0x060088FF RID: 35071 RVA: 0x00337014 File Offset: 0x00335214
			public void Warn(object message, Exception exception)
			{
			}

			// Token: 0x06008900 RID: 35072 RVA: 0x00337016 File Offset: 0x00335216
			public void WarnFormat(string format, params object[] args)
			{
			}

			// Token: 0x06008901 RID: 35073 RVA: 0x00337018 File Offset: 0x00335218
			public void WarnFormat(string format, object arg0)
			{
			}

			// Token: 0x06008902 RID: 35074 RVA: 0x0033701A File Offset: 0x0033521A
			public void WarnFormat(string format, object arg0, object arg1)
			{
			}

			// Token: 0x06008903 RID: 35075 RVA: 0x0033701C File Offset: 0x0033521C
			public void WarnFormat(string format, object arg0, object arg1, object arg2)
			{
			}

			// Token: 0x06008904 RID: 35076 RVA: 0x0033701E File Offset: 0x0033521E
			public void WarnFormat(IFormatProvider provider, string format, params object[] args)
			{
			}
		}
	}
}
