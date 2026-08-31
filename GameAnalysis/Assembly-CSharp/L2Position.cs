using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class L2Position : LagrangePosition
{
	// Token: 0x06000FFE RID: 4094 RVA: 0x00052EB6 File Offset: 0x000510B6
	public override LagrangeValue GetLagrangePointNumber()
	{
		return LagrangeValue.L2;
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x00052EBC File Offset: 0x000510BC
	public override Vector3d GetPosition(Vector3d position, Vector3d barycenterPos, double m1, double m2)
	{
		double magnitude = (position - barycenterPos).magnitude;
		double num = 1.0 + base.HillRadius(magnitude, m1, m2) / magnitude;
		return barycenterPos + num * (position - barycenterPos);
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00052F04 File Offset: 0x00051104
	public override Vector3d GetPosition(TISpaceObjectState relatedObject, TIDateTime dateTime = null, bool display = true)
	{
		if (Error.IsNull<TISpaceObjectState>(relatedObject))
		{
			return Vector3d.zero;
		}
		base.GetStates(relatedObject, dateTime);
		double num = 1.0 + base.GetHillRadius() / this.relpos.magnitude;
		if (display)
		{
			return (1.0 - num) * this.center.positionDisplay + num * this.state.positionDisplay;
		}
		return this.center.position + num * (this.state.position - this.center.position);
	}
}
