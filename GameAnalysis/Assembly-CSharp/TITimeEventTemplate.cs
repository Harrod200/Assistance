using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200035C RID: 860
public class TITimeEventTemplate : TIDataTemplate
{
	// Token: 0x06000F1D RID: 3869 RVA: 0x0004B4F0 File Offset: 0x000496F0
	public TITimeEventTemplate(string name)
		: base(name)
	{
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0004B504 File Offset: 0x00049704
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TITimeEvent>();
		}
		return tigameState;
	}

	// Token: 0x04000F34 RID: 3892
	public TITimeQueueRepeatType eventType;

	// Token: 0x04000F35 RID: 3893
	public int? timeStep;

	// Token: 0x04000F36 RID: 3894
	public string eventName;

	// Token: 0x04000F37 RID: 3895
	public bool stopClock;

	// Token: 0x04000F38 RID: 3896
	public bool pauseTime;

	// Token: 0x04000F39 RID: 3897
	public float priority;

	// Token: 0x04000F3A RID: 3898
	public List<TITimeEventTemplate.RepeatChange> repeatChanges = new List<TITimeEventTemplate.RepeatChange>();

	// Token: 0x02000BA1 RID: 2977
	public struct RepeatChange
	{
		// Token: 0x17001139 RID: 4409
		// (get) Token: 0x06006943 RID: 26947 RVA: 0x003027A2 File Offset: 0x003009A2
		public bool ConditionMet
		{
			get
			{
				return this.triggerCondition.PassesCondition(null);
			}
		}

		// Token: 0x04004B10 RID: 19216
		public TIGlobalCondition triggerCondition;

		// Token: 0x04004B11 RID: 19217
		public TITimeQueueRepeatType updateEventType;
	}
}
