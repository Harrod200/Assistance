using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000374 RID: 884
public class L1Position : LagrangePosition
{
	// Token: 0x06000FFA RID: 4090 RVA: 0x00052DB0 File Offset: 0x00050FB0
	public override LagrangeValue GetLagrangePointNumber()
	{
		return LagrangeValue.L1;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x00052DB4 File Offset: 0x00050FB4
	public override Vector3d GetPosition(Vector3d position, Vector3d barycenterPos, double m1, double m2)
	{
		double magnitude = (position - barycenterPos).magnitude;
		double num = 1.0 - base.HillRadius(magnitude, m1, m2) / magnitude;
		return barycenterPos * (1.0 - num) + position * num;
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x00052E08 File Offset: 0x00051008
	public override Vector3d GetPosition(TISpaceObjectState relatedObject, TIDateTime dateTime = null, bool display = true)
	{
		if (Error.IsNull<TISpaceObjectState>(relatedObject))
		{
			return Vector3d.zero;
		}
		base.GetStates(relatedObject, dateTime);
		double num = 1.0 - base.GetHillRadius() / this.relpos.magnitude;
		if (display)
		{
			return (1.0 - num) * this.center.positionDisplay + num * this.state.positionDisplay;
		}
		return this.center.position + num * (this.state.position - this.center.position);
	}
}
