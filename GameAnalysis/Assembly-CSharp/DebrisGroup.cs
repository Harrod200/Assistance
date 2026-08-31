using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
[Serializable]
public class DebrisGroup
{
	// Token: 0x04000080 RID: 128
	public float m_lifetime = 1f;

	// Token: 0x04000081 RID: 129
	[Header("Velocity")]
	public Vector3 m_motionOrigin = Vector3.zero;

	// Token: 0x04000082 RID: 130
	public Vector3 m_motionDirection = Vector3.zero;

	// Token: 0x04000083 RID: 131
	public float m_motionDirectionVariance;

	// Token: 0x04000084 RID: 132
	public float m_motionScaleFactor = 1f;

	// Token: 0x04000085 RID: 133
	public float m_motionScaleVariance;

	// Token: 0x04000086 RID: 134
	public Vector3 m_angleScaleFactors = Vector3.one;

	// Token: 0x04000087 RID: 135
	[Header("Target GameObjects")]
	public GameObject[] m_DebrisObjects;
}
