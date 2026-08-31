using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F9 RID: 2553
	public interface IRenderPath
	{
		// Token: 0x0600619E RID: 24990
		void SubmitPathToRender(List<Vector3> points, Color pathColor, int waypointID);

		// Token: 0x0600619F RID: 24991
		void SubmitPathToRender(List<Vector3> points, Color pathColor, Vector2 alphaBlend, int waypointID);
	}
}
