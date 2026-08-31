using System;
using log4net.Appender;
using log4net.Core;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Logging
{
	// Token: 0x0200091C RID: 2332
	public class UnityConsoleAppender : AppenderSkeleton
	{
		// Token: 0x0600592D RID: 22829 RVA: 0x0028F050 File Offset: 0x0028D250
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (loggingEvent.Level < Level.Warn)
			{
				Debug.Log(loggingEvent.RenderedMessage);
				return;
			}
			if (loggingEvent.Level > Level.Warn)
			{
				Debug.LogError(loggingEvent.RenderedMessage);
				return;
			}
			Debug.LogWarning(loggingEvent.RenderedMessage);
		}
	}
}
