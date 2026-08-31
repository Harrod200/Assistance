using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020002C9 RID: 713
public class TI2DCinematicTemplate : TIDataTemplate
{
	// Token: 0x06000A6A RID: 2666 RVA: 0x00032B40 File Offset: 0x00030D40
	public List<TI2DCinematicTemplate.CinematicTextSequence> GetCinematicTextSequences()
	{
		return this.cinematicSequences.Where<TI2DCinematicTemplate.CinematicTextSequence>((TI2DCinematicTemplate.CinematicTextSequence x) => x.displayTime[0] > 0f).ToList<TI2DCinematicTemplate.CinematicTextSequence>();
	}

	// Token: 0x040008AB RID: 2219
	public int textSequences;

	// Token: 0x040008AC RID: 2220
	public float textTimeStamp1;

	// Token: 0x040008AD RID: 2221
	public float textTimeStamp2;

	// Token: 0x040008AE RID: 2222
	public float textTimeStamp3;

	// Token: 0x040008AF RID: 2223
	public float textTimeStamp4;

	// Token: 0x040008B0 RID: 2224
	public float textTimeStamp5;

	// Token: 0x040008B1 RID: 2225
	public float textTimeStamp6;

	// Token: 0x040008B2 RID: 2226
	public float textTimeStamp7;

	// Token: 0x040008B3 RID: 2227
	public float textTimeStamp8;

	// Token: 0x040008B4 RID: 2228
	public float textTimeStamp9;

	// Token: 0x040008B5 RID: 2229
	public float textTimeStamp10;

	// Token: 0x040008B6 RID: 2230
	public float textTimeStamp11;

	// Token: 0x040008B7 RID: 2231
	public float textTimeStamp12;

	// Token: 0x040008B8 RID: 2232
	public float textTimeStamp13;

	// Token: 0x040008B9 RID: 2233
	public float textTimeStamp14;

	// Token: 0x040008BA RID: 2234
	public float textTimeStamp15;

	// Token: 0x040008BB RID: 2235
	public float textTimeStamp16;

	// Token: 0x040008BC RID: 2236
	public float textTimeStamp17;

	// Token: 0x040008BD RID: 2237
	public float textTimeStamp18;

	// Token: 0x040008BE RID: 2238
	public float textTimeStamp19;

	// Token: 0x040008BF RID: 2239
	public float textTimeStamp20;

	// Token: 0x040008C0 RID: 2240
	public string trigger;

	// Token: 0x040008C1 RID: 2241
	public List<TI2DCinematicTemplate.CinematicTextSequence> cinematicSequences;

	// Token: 0x02000B52 RID: 2898
	public struct CinematicTextSequence
	{
		// Token: 0x04004A33 RID: 18995
		public List<float> displayTime;

		// Token: 0x04004A34 RID: 18996
		public List<string> voiceOverPath;

		// Token: 0x04004A35 RID: 18997
		public List<string> locTextPath;
	}
}
