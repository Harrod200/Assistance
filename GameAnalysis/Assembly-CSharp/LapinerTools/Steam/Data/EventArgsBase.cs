using System;

namespace LapinerTools.Steam.Data
{
	// Token: 0x02000539 RID: 1337
	public class EventArgsBase : EventArgs
	{
		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06002202 RID: 8706 RVA: 0x000B210D File Offset: 0x000B030D
		// (set) Token: 0x06002203 RID: 8707 RVA: 0x000B2115 File Offset: 0x000B0315
		public bool IsError { get; set; }

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06002204 RID: 8708 RVA: 0x000B211E File Offset: 0x000B031E
		// (set) Token: 0x06002205 RID: 8709 RVA: 0x000B2126 File Offset: 0x000B0326
		public string ErrorMessage { get; set; }

		// Token: 0x06002206 RID: 8710 RVA: 0x000B212F File Offset: 0x000B032F
		public EventArgsBase()
		{
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x000B2137 File Offset: 0x000B0337
		public EventArgsBase(EventArgsBase p_copyFromArgs)
		{
			if (p_copyFromArgs != null)
			{
				this.IsError = p_copyFromArgs.IsError;
				this.ErrorMessage = p_copyFromArgs.ErrorMessage;
			}
		}
	}
}
