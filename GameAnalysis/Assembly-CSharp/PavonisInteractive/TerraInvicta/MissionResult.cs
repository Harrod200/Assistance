using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000762 RID: 1890
	public struct MissionResult
	{
		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x0600350A RID: 13578 RVA: 0x0012F384 File Offset: 0x0012D584
		public bool Success
		{
			get
			{
				return this.missionOutcome == TIMissionOutcome.CriticalSuccess || this.missionOutcome == TIMissionOutcome.Success;
			}
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x0600350B RID: 13579 RVA: 0x0012F39A File Offset: 0x0012D59A
		public bool Failed
		{
			get
			{
				return this.missionOutcome == TIMissionOutcome.Failure || this.missionOutcome == TIMissionOutcome.CriticalFailure;
			}
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x0600350C RID: 13580 RVA: 0x0012F3B0 File Offset: 0x0012D5B0
		public bool NotAttempted
		{
			get
			{
				return this.missionOutcome == TIMissionOutcome.Aborted || this.missionOutcome == TIMissionOutcome.None;
			}
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x0600350D RID: 13581 RVA: 0x0012F3C6 File Offset: 0x0012D5C6
		public bool Attempted
		{
			get
			{
				return !this.NotAttempted;
			}
		}

		// Token: 0x040023CA RID: 9162
		public TICouncilorState councilor;

		// Token: 0x040023CB RID: 9163
		public TIMissionTemplate missionTemplate;

		// Token: 0x040023CC RID: 9164
		public TIGameState target;

		// Token: 0x040023CD RID: 9165
		public float successChance;

		// Token: 0x040023CE RID: 9166
		public TIMissionOutcome missionOutcome;

		// Token: 0x040023CF RID: 9167
		public float roll;

		// Token: 0x040023D0 RID: 9168
		public string valueChange;

		// Token: 0x040023D1 RID: 9169
		public float noiseModifier;
	}
}
