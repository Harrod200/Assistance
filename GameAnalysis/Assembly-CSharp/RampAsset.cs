using System;
using UnityEngine;

// Token: 0x0200041E RID: 1054
[CreateAssetMenu]
public class RampAsset : ScriptableObject
{
	// Token: 0x040013FB RID: 5115
	public Gradient gradient = new Gradient();

	// Token: 0x040013FC RID: 5116
	public int size = 16;

	// Token: 0x040013FD RID: 5117
	public bool up;

	// Token: 0x040013FE RID: 5118
	public bool overwriteExisting = true;
}
