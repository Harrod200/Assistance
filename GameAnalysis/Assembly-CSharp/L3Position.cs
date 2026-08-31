using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000376 RID: 886
public class L3Position : LagrangePosition
{
	// Token: 0x06001002 RID: 4098 RVA: 0x00052FB2 File Offset: 0x000511B2
	public override LagrangeValue GetLagrangePointNumber()
	{
		return LagrangeValue.L3;
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00052FB5 File Offset: 0x000511B5
	public override Vector3d GetPosition(Vector3d position, Vector3d barycenterPos, double m1, double m2)
	{
		return barycenterPos - (1.0 + 7.0 * m2 / (12.0 * m1)) * (position - barycenterPos);
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x00052FEC File Offset: 0x000511EC
	public override Vector3d GetPosition(TISpaceObjectState relatedObject, TIDateTime dateTime = null, bool display = true)
	{
		if (Error.IsNull<TISpaceObjectState>(relatedObject))
		{
			return Vector3d.zero;
		}
		base.GetStates(relatedObject, dateTime);
		double num = 1.0 + 7.0 * this.M2 / (12.0 * this.M1);
		if (display)
		{
			return this.center.positionDisplay - num * (this.state.positionDisplay - this.center.positionDisplay);
		}
		return this.center.position - num * (this.state.position - this.center.position);
	}
}
