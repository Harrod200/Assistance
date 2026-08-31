using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000372 RID: 882
public abstract class TINavigablePosition
{
	// Token: 0x06000FF1 RID: 4081
	public abstract Vector3d GetPosition(TISpaceObjectState relatedObject, TIDateTime dateTime = null, bool display = true);

	// Token: 0x06000FF2 RID: 4082
	public abstract Vector3d GetPosition(Vector3d position, Vector3d barycenterPos, double m1, double m2);
}
