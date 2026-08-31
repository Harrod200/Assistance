using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding
{
	// Token: 0x02000A0C RID: 2572
	public class OcTree
	{
		// Token: 0x060063F4 RID: 25588 RVA: 0x002F3564 File Offset: 0x002F1764
		public OcTreeNode RandomNode()
		{
			int num = TIUtilities.RandomRange(0, this._navVolume.GetLength(0));
			int num2 = TIUtilities.RandomRange(0, this._navVolume.GetLength(1));
			int num3 = TIUtilities.RandomRange(0, this._navVolume.GetLength(2));
			return this._navVolume[num, num2, num3];
		}

		// Token: 0x060063F5 RID: 25589 RVA: 0x002F35B8 File Offset: 0x002F17B8
		public OcTree(int volumeSize, float baseNodeSize, Vector3 center)
		{
			this.BASE_MAP_SIZE = volumeSize;
			this.BASE_NODE_SIZE = baseNodeSize;
			this._navVolume = new OcTreeNode[this.BASE_MAP_SIZE, this.BASE_MAP_SIZE, this.BASE_MAP_SIZE];
			Vector3 vector = new Vector3(this.BASE_NODE_SIZE * ((float)(-(float)this.BASE_MAP_SIZE) * 0.5f), this.BASE_NODE_SIZE * ((float)(-(float)this.BASE_MAP_SIZE) * 0.5f), this.BASE_NODE_SIZE * ((float)(-(float)this.BASE_MAP_SIZE) * 0.5f));
			this._offset = center;
			for (int i = 0; i < this.BASE_MAP_SIZE; i++)
			{
				for (int j = 0; j < this.BASE_MAP_SIZE; j++)
				{
					for (int k = 0; k < this.BASE_MAP_SIZE; k++)
					{
						this._navVolume[i, j, k] = new OcTreeNode(this.BASE_NODE_SIZE, vector + this._offset, 0);
						this._offset.z = this._offset.z + this.BASE_NODE_SIZE;
					}
					this._offset.y = this._offset.y + this.BASE_NODE_SIZE;
					this._offset.z = center.z;
				}
				this._offset.x = this._offset.x + this.BASE_NODE_SIZE;
				this._offset.y = center.y;
			}
			this._offset = center;
		}

		// Token: 0x060063F6 RID: 25590 RVA: 0x002F370C File Offset: 0x002F190C
		public bool IsValidPosition(Vector3 position, out int x, out int y, out int z)
		{
			x = (int)Math.Round((double)((position.x - this._offset.x) / this.BASE_NODE_SIZE)) + (int)((float)this.BASE_MAP_SIZE * 0.5f);
			y = (int)Math.Round((double)((position.y - this._offset.y) / this.BASE_NODE_SIZE)) + (int)((float)this.BASE_MAP_SIZE * 0.5f);
			z = (int)Math.Round((double)((position.z - this._offset.z) / this.BASE_NODE_SIZE)) + (int)((float)this.BASE_MAP_SIZE * 0.5f);
			return x >= 0 && x < this._navVolume.GetLength(0) && y >= 0 && y < this._navVolume.GetLength(1) && z >= 0 && z < this._navVolume.GetLength(2);
		}

		// Token: 0x060063F7 RID: 25591 RVA: 0x002F37F4 File Offset: 0x002F19F4
		public OcTreeNode GetChildNodeAtPosition(Vector3 position, int depth)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (!this.IsValidPosition(position, out num, out num2, out num3))
			{
				return null;
			}
			OcTreeNode ocTreeNode = this._navVolume[num, num2, num3].GetChildNode(position);
			if (ocTreeNode != null)
			{
				while (ocTreeNode.Depth > depth)
				{
					OcTreeNode childNode = ocTreeNode.GetChildNode(position);
					if (childNode == null)
					{
						break;
					}
					ocTreeNode = childNode;
				}
			}
			return ocTreeNode;
		}

		// Token: 0x060063F8 RID: 25592 RVA: 0x002F384C File Offset: 0x002F1A4C
		public OcTreeNode GetChildNodeAtPosition(Vector3 position, OcTree.DepthPriority priority)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (!this.IsValidPosition(position, out num, out num2, out num3))
			{
				return null;
			}
			OcTreeNode ocTreeNode = null;
			switch (priority)
			{
			case OcTree.DepthPriority.Highest:
				ocTreeNode = this._navVolume[num, num2, num3].Root;
				break;
			case OcTree.DepthPriority.Lowest:
				ocTreeNode = this._navVolume[num, num2, num3];
				if (ocTreeNode != null)
				{
					while (ocTreeNode.CanContainChildren)
					{
						OcTreeNode childNode = ocTreeNode.GetChildNode(position);
						if (childNode == null)
						{
							break;
						}
						ocTreeNode = childNode;
					}
				}
				break;
			case OcTree.DepthPriority.LowestAvailable:
				ocTreeNode = this._navVolume[num, num2, num3];
				if (ocTreeNode != null)
				{
					if (!ocTreeNode.HasChildren)
					{
						return ocTreeNode;
					}
					while (ocTreeNode.HasChildren)
					{
						ocTreeNode = ocTreeNode.GetChildNode(position);
					}
				}
				break;
			}
			return ocTreeNode;
		}

		// Token: 0x060063F9 RID: 25593 RVA: 0x002F38F8 File Offset: 0x002F1AF8
		public void DrawGizmos(bool forceAll = false)
		{
			if (this._navVolume != null)
			{
				for (int i = 0; i < this.BASE_MAP_SIZE; i++)
				{
					for (int j = 0; j < this.BASE_MAP_SIZE; j++)
					{
						for (int k = 0; k < this.BASE_MAP_SIZE; k++)
						{
							this._navVolume[i, j, k].DrawAllBounds(forceAll);
						}
					}
				}
			}
		}

		// Token: 0x060063FA RID: 25594 RVA: 0x002F3954 File Offset: 0x002F1B54
		public void DrawGizmos(int COUNT, int depth = 0)
		{
			if (this._navVolume != null)
			{
				int num = 0;
				for (int i = 0; i < this.BASE_MAP_SIZE; i++)
				{
					for (int j = 0; j < this.BASE_MAP_SIZE; j++)
					{
						for (int k = 0; k < this.BASE_MAP_SIZE; k++)
						{
							if (num >= COUNT)
							{
								return;
							}
							OcTreeNode ocTreeNode = this._navVolume[i, j, k];
							ocTreeNode.DrawImmediateBounds();
							if (ocTreeNode.Depth < depth)
							{
								ocTreeNode.DrawChildrenNodes(depth);
							}
							num++;
						}
					}
				}
			}
		}

		// Token: 0x040046B1 RID: 18097
		private readonly int BASE_MAP_SIZE;

		// Token: 0x040046B2 RID: 18098
		private readonly float BASE_NODE_SIZE;

		// Token: 0x040046B3 RID: 18099
		private Vector3 _offset;

		// Token: 0x040046B4 RID: 18100
		private OcTreeNode[,,] _navVolume;

		// Token: 0x020013C2 RID: 5058
		public enum DepthPriority
		{
			// Token: 0x040072BE RID: 29374
			Highest,
			// Token: 0x040072BF RID: 29375
			Lowest,
			// Token: 0x040072C0 RID: 29376
			LowestAvailable
		}
	}
}
