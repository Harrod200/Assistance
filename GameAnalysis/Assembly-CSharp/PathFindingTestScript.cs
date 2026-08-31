using System;
using PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class PathFindingTestScript : MonoBehaviour
{
	// Token: 0x060000F1 RID: 241 RVA: 0x000083E1 File Offset: 0x000065E1
	private void Start()
	{
		this.ClearMap();
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000083E9 File Offset: 0x000065E9
	[ContextMenu("Clear")]
	public void ClearMap()
	{
		this._tree = new OcTree(this.SquareUnitCount, this.BaseUnitSize, Vector3.zero);
		this._pathFinder = new Pathfinding(this._tree);
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00008418 File Offset: 0x00006618
	[ContextMenu("Reset")]
	public void ResetMap()
	{
		this._tree = new OcTree(this.SquareUnitCount, this.BaseUnitSize, Vector3.one * 20f);
		this._pathFinder = new Pathfinding(this._tree);
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00008454 File Offset: 0x00006654
	[ContextMenu("Stress Test")]
	public void StressTest()
	{
		Debug.Log("Start: " + DateTime.Now.ToString());
		Debug.Log("End: " + DateTime.Now.ToString());
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x0000849C File Offset: 0x0000669C
	private void OnDrawGizmos()
	{
		if (!this.DrawDebug)
		{
			return;
		}
		if (this._tree != null)
		{
			this._tree.DrawGizmos(this.ForceDrawAll);
			for (int i = 0; i < this._positions.Length - 1; i++)
			{
				Debug.DrawLine(this._positions[i], this._positions[i + 1], Color.green);
			}
		}
	}

	// Token: 0x040000E9 RID: 233
	public int COUNT;

	// Token: 0x040000EA RID: 234
	public int DEPTH;

	// Token: 0x040000EB RID: 235
	public Transform _pointA;

	// Token: 0x040000EC RID: 236
	public Transform _pointB;

	// Token: 0x040000ED RID: 237
	[Range(1f, 100f)]
	public int SquareUnitCount = 10;

	// Token: 0x040000EE RID: 238
	public float BaseUnitSize = 100f;

	// Token: 0x040000EF RID: 239
	public bool DrawDebug;

	// Token: 0x040000F0 RID: 240
	public bool ForceDrawAll;

	// Token: 0x040000F1 RID: 241
	private OcTree _tree;

	// Token: 0x040000F2 RID: 242
	private Pathfinding _pathFinder;

	// Token: 0x040000F3 RID: 243
	private Vector3[] _positions = new Vector3[20];
}
