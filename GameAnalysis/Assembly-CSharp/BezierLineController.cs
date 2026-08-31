using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200044D RID: 1101
[ExecuteAlways]
public class BezierLineController : MonoBehaviour
{
	// Token: 0x06001718 RID: 5912 RVA: 0x0007796D File Offset: 0x00075B6D
	private void Start()
	{
		this.SetPositions();
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x00077975 File Offset: 0x00075B75
	private void Update()
	{
		this.SetPositions();
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x00077980 File Offset: 0x00075B80
	private void SetPositions()
	{
		if (this.BezierCurve.Type == BezierCurveType.Linear)
		{
			this.LineRenderer.positionCount = 2;
			this.LineRenderer.SetPosition(0, (Vector3)this.BezierCurve.A);
			this.LineRenderer.SetPosition(1, (Vector3)this.BezierCurve.B);
			return;
		}
		int num = Mathf.Max(1, this.SampleCount - 1);
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i <= num; i++)
		{
			list.Add((Vector3)this.BezierCurve.GetPosition((double)((float)i * 1f / (float)num)));
		}
		this.LineRenderer.positionCount = list.Count;
		this.LineRenderer.SetPositions(list.ToArray());
		this.SetDebugHandlesPositions();
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x00077A4C File Offset: 0x00075C4C
	private void SetDebugHandlesPositions()
	{
		if (this.DebugHandleA != null)
		{
			if (this.LineRenderer.useWorldSpace)
			{
				this.DebugHandleA.transform.position = (Vector3)this.BezierCurve.A;
			}
			else
			{
				this.DebugHandleA.transform.localPosition = (Vector3)this.BezierCurve.A;
			}
		}
		if (this.DebugHandleB != null)
		{
			if (this.LineRenderer.useWorldSpace)
			{
				this.DebugHandleB.transform.position = (Vector3)this.BezierCurve.B;
			}
			else
			{
				this.DebugHandleB.transform.localPosition = (Vector3)this.BezierCurve.B;
			}
		}
		if (this.DebugHandleControlA != null)
		{
			if (this.LineRenderer.useWorldSpace)
			{
				this.DebugHandleControlA.transform.position = (Vector3)this.BezierCurve.ControlPointA;
			}
			else
			{
				this.DebugHandleControlA.transform.localPosition = (Vector3)this.BezierCurve.ControlPointA;
			}
		}
		if (this.DebugHandleControlB != null)
		{
			if (this.LineRenderer.useWorldSpace)
			{
				this.DebugHandleControlB.transform.position = (Vector3)this.BezierCurve.ControlPointB;
				return;
			}
			this.DebugHandleControlB.transform.localPosition = (Vector3)this.BezierCurve.ControlPointB;
		}
	}

	// Token: 0x0400159C RID: 5532
	public LineRenderer LineRenderer;

	// Token: 0x0400159D RID: 5533
	public GameObject DebugHandleControlA;

	// Token: 0x0400159E RID: 5534
	public GameObject DebugHandleControlB;

	// Token: 0x0400159F RID: 5535
	public GameObject DebugHandleA;

	// Token: 0x040015A0 RID: 5536
	public GameObject DebugHandleB;

	// Token: 0x040015A1 RID: 5537
	public BezierCurve BezierCurve;

	// Token: 0x040015A2 RID: 5538
	public int SampleCount = 10;
}
