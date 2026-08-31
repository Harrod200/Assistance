using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000159 RID: 345
public struct Alarm
{
	// Token: 0x04000271 RID: 625
	public TIGameState associatedGameState;

	// Token: 0x04000272 RID: 626
	public TIDateTime time;

	// Token: 0x04000273 RID: 627
	public AlarmType alarmType;

	// Token: 0x04000274 RID: 628
	public TITimeEvent alarmEvent;

	// Token: 0x04000275 RID: 629
	public string customPlayerString;
}
