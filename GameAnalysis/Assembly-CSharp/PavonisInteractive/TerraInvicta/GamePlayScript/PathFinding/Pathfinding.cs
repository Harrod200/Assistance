using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding
{
	// Token: 0x02000A0E RID: 2574
	public class Pathfinding
	{
		// Token: 0x0600640E RID: 25614 RVA: 0x002F3E66 File Offset: 0x002F2066
		public Pathfinding(OcTree navVol)
		{
			this._navVolume = navVol;
		}

		// Token: 0x0600640F RID: 25615 RVA: 0x002F3E78 File Offset: 0x002F2078
		public void FindPath(Vector3 startPosition, Vector3 startHeading, Vector3 endPosition, CombatFleetController agents, ref Vector3[] positions)
		{
			if (positions == null || positions.Length == 0)
			{
				return;
			}
			OcTreeNode childNodeAtPosition = this._navVolume.GetChildNodeAtPosition(endPosition, OcTree.DepthPriority.Lowest);
			if (childNodeAtPosition == null)
			{
				return;
			}
			OcTreeNode ocTreeNode = this._navVolume.GetChildNodeAtPosition(startPosition, OcTree.DepthPriority.Lowest);
			if (ocTreeNode == null)
			{
				return;
			}
			Quaternion rotation = GameControl.spaceCombat.container.transform.rotation;
			positions[0] = rotation * ocTreeNode.Center;
			Vector3 vector = startHeading;
			for (int i = 1; i < positions.Length; i++)
			{
				Vector3 vector2 = vector;
				OcTreeNode ocTreeNode2 = null;
				for (float num = -1f + vector.x; num <= 1f + vector.x; num += 1f)
				{
					for (float num2 = -1f + vector.y; num2 <= 1f + vector.y; num2 += 1f)
					{
						for (float num3 = -1f + vector.z; num3 <= 1f + vector.z; num3 += 1f)
						{
							OcTreeNode ocTreeNode3;
							this.GetNeighbourNode(ocTreeNode.Center + new Vector3(num, num2, num3) * ocTreeNode.Length, out ocTreeNode3);
							if (ocTreeNode3 != null && (ocTreeNode2 == null || this.EvaluateScoreForNode(ocTreeNode3, childNodeAtPosition.Center, agents) < this.EvaluateScoreForNode(ocTreeNode2, childNodeAtPosition.Center, agents)))
							{
								ocTreeNode2 = ocTreeNode3;
								vector2.x = num;
								vector2.y = num2;
								vector2.z = num3;
							}
						}
					}
				}
				if (ocTreeNode2 != null)
				{
					positions[i] = rotation * ocTreeNode2.Center;
					ocTreeNode = ocTreeNode2;
					vector = vector2;
				}
				if (ocTreeNode2 == childNodeAtPosition || ocTreeNode2 == null)
				{
					Array.Resize<Vector3>(ref positions, i + 1);
					return;
				}
			}
		}

		// Token: 0x06006410 RID: 25616 RVA: 0x002F4036 File Offset: 0x002F2236
		private void GetNeighbourNode(Vector3 point, out OcTreeNode node)
		{
			node = this._navVolume.GetChildNodeAtPosition(point, OcTree.DepthPriority.Lowest);
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x002F4047 File Offset: 0x002F2247
		private float EvaluateScoreForNode(OcTreeNode node, Vector3 target, CombatFleetController agents)
		{
			return Vector3.SqrMagnitude(target - node.Center);
		}

		// Token: 0x040046BE RID: 18110
		private OcTree _navVolume;
	}
}
