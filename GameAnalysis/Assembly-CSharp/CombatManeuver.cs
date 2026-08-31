using System;

// Token: 0x020003CC RID: 972
public enum CombatManeuver
{
	// Token: 0x040010C1 RID: 4289
	None,
	// Token: 0x040010C2 RID: 4290
	Padlock,
	// Token: 0x040010C3 RID: 4291
	CancelPadlock,
	// Token: 0x040010C4 RID: 4292
	Roll90Port,
	// Token: 0x040010C5 RID: 4293
	Roll90Starboard,
	// Token: 0x040010C6 RID: 4294
	Roll180,
	// Token: 0x040010C7 RID: 4295
	SpinDorsal,
	// Token: 0x040010C8 RID: 4296
	SpinVentral,
	// Token: 0x040010C9 RID: 4297
	SpinPort,
	// Token: 0x040010CA RID: 4298
	SpinStarboard,
	// Token: 0x040010CB RID: 4299
	AllStop,
	// Token: 0x040010CC RID: 4300
	CancelAllStop,
	// Token: 0x040010CD RID: 4301
	CancelSpinDorsal,
	// Token: 0x040010CE RID: 4302
	CancelSpinVentral,
	// Token: 0x040010CF RID: 4303
	CancelSpinPort,
	// Token: 0x040010D0 RID: 4304
	CancelSpinStarboard,
	// Token: 0x040010D1 RID: 4305
	FullSpeedAhead,
	// Token: 0x040010D2 RID: 4306
	InterceptCourse,
	// Token: 0x040010D3 RID: 4307
	MatchVelocity,
	// Token: 0x040010D4 RID: 4308
	CancelMatchVelocity,
	// Token: 0x040010D5 RID: 4309
	DefensiveManuevers,
	// Token: 0x040010D6 RID: 4310
	CancelDefensiveManeuvers,
	// Token: 0x040010D7 RID: 4311
	FaceVelocityVector
}
