using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding
{
	// Token: 0x02000A0D RID: 2573
	public class OcTreeNode
	{
		// Token: 0x17001111 RID: 4369
		// (get) Token: 0x060063FB RID: 25595 RVA: 0x002F39D0 File Offset: 0x002F1BD0
		// (set) Token: 0x060063FC RID: 25596 RVA: 0x002F39D8 File Offset: 0x002F1BD8
		private bool Marked { get; set; }

		// Token: 0x17001112 RID: 4370
		// (get) Token: 0x060063FD RID: 25597 RVA: 0x002F39E1 File Offset: 0x002F1BE1
		public bool HasChildren
		{
			get
			{
				return this._children != null;
			}
		}

		// Token: 0x17001113 RID: 4371
		// (get) Token: 0x060063FE RID: 25598 RVA: 0x002F39EC File Offset: 0x002F1BEC
		public bool CanContainChildren
		{
			get
			{
				return this._depth < 3;
			}
		}

		// Token: 0x17001114 RID: 4372
		// (get) Token: 0x060063FF RID: 25599 RVA: 0x002F39F7 File Offset: 0x002F1BF7
		public int Depth
		{
			get
			{
				return this._depth;
			}
		}

		// Token: 0x17001115 RID: 4373
		// (get) Token: 0x06006400 RID: 25600 RVA: 0x002F39FF File Offset: 0x002F1BFF
		public float Length
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x17001116 RID: 4374
		// (get) Token: 0x06006401 RID: 25601 RVA: 0x002F3A07 File Offset: 0x002F1C07
		public Vector3 Center
		{
			get
			{
				return this._center;
			}
		}

		// Token: 0x17001117 RID: 4375
		// (get) Token: 0x06006402 RID: 25602 RVA: 0x002F3A0F File Offset: 0x002F1C0F
		public OcTreeNode Root
		{
			get
			{
				this._root.Marked = true;
				return this._root;
			}
		}

		// Token: 0x17001118 RID: 4376
		// (get) Token: 0x06006403 RID: 25603 RVA: 0x002F3A23 File Offset: 0x002F1C23
		public OcTreeNode Parent
		{
			get
			{
				this._parent.Marked = true;
				return this._parent;
			}
		}

		// Token: 0x06006404 RID: 25604 RVA: 0x002F3A37 File Offset: 0x002F1C37
		public OcTreeNode(float length, Vector3 center, int depth = 0)
		{
			this._center = center;
			this._length = length;
			this._depth = depth;
			this._root = this;
			this._parent = this;
			this.SetValues(length, center);
		}

		// Token: 0x06006405 RID: 25605 RVA: 0x002F3A6A File Offset: 0x002F1C6A
		public OcTreeNode(OcTreeNode root, OcTreeNode parent, Bounds bounds, float length, int depth = 0)
		{
			this._depth = depth;
			this._root = root;
			this._parent = parent;
			this._center = bounds.center;
			this._length = length;
			this._bounds = bounds;
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x002F3AA4 File Offset: 0x002F1CA4
		public OcTreeNode GetChildNode(Vector3 point)
		{
			if (!this.Contains(point))
			{
				return null;
			}
			this.Marked = true;
			if (this._children == null && this.CanContainChildren)
			{
				this.CreateChildren();
			}
			if (this._children != null)
			{
				for (int i = 0; i < this._children.Length; i++)
				{
					this._children[i].Marked = true;
					if (this._children[i].Contains(point))
					{
						return this._children[i];
					}
				}
			}
			return null;
		}

		// Token: 0x06006407 RID: 25607 RVA: 0x002F3B1C File Offset: 0x002F1D1C
		public bool Contains(Vector3 p)
		{
			return this._bounds.Contains(p);
		}

		// Token: 0x06006408 RID: 25608 RVA: 0x002F3B2A File Offset: 0x002F1D2A
		public bool Intersects(ref Bounds b)
		{
			return this._bounds.Intersects(b);
		}

		// Token: 0x06006409 RID: 25609 RVA: 0x002F3B40 File Offset: 0x002F1D40
		private void SetValues(float lengthVal, Vector3 centerVal)
		{
			this._center = centerVal;
			this._length = lengthVal;
			Vector3 vector = new Vector3(lengthVal, lengthVal, lengthVal);
			this._bounds = new Bounds(this._center, vector);
		}

		// Token: 0x0600640A RID: 25610 RVA: 0x002F3B78 File Offset: 0x002F1D78
		private void CreateChildren()
		{
			this._children = new OcTreeNode[8];
			float num = this._length * 0.25f;
			float num2 = this._length * 0.5f;
			Vector3 vector = new Vector3(num2, num2, num2);
			Bounds[] array = new Bounds[]
			{
				new Bounds(this._center + new Vector3(-num, num, -num), vector),
				new Bounds(this._center + new Vector3(num, num, -num), vector),
				new Bounds(this._center + new Vector3(-num, num, num), vector),
				new Bounds(this._center + new Vector3(num, num, num), vector),
				new Bounds(this._center + new Vector3(-num, -num, -num), vector),
				new Bounds(this._center + new Vector3(num, -num, -num), vector),
				new Bounds(this._center + new Vector3(-num, -num, num), vector),
				new Bounds(this._center + new Vector3(num, -num, num), vector)
			};
			for (int i = 0; i < this._children.Length; i++)
			{
				this._children[i] = new OcTreeNode(this._root, this, array[i], num2, this._depth + 1);
			}
		}

		// Token: 0x0600640B RID: 25611 RVA: 0x002F3D08 File Offset: 0x002F1F08
		public void DrawAllBounds(bool forceAll = false)
		{
			float num = (float)(this._depth / 3);
			Gizmos.color = new Color(0f, 0f, 1f - num);
			if (this.Marked || forceAll)
			{
				Bounds bounds = new Bounds(this._center, new Vector3(this._length, this._length, this._length));
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
			if (this._children != null)
			{
				for (int i = 0; i < this._children.Length; i++)
				{
					this._children[i].DrawAllBounds(forceAll);
				}
			}
			Gizmos.color = Color.white;
		}

		// Token: 0x0600640C RID: 25612 RVA: 0x002F3DB0 File Offset: 0x002F1FB0
		public void DrawImmediateBounds()
		{
			float num = (float)(this._depth / 3);
			Gizmos.color = new Color(0f, 0f, 1f - num);
			Bounds bounds = new Bounds(this._center, new Vector3(this._length, this._length, this._length));
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}

		// Token: 0x0600640D RID: 25613 RVA: 0x002F3E1C File Offset: 0x002F201C
		public void DrawChildrenNodes(int depth)
		{
			if (this._children == null)
			{
				this.CreateChildren();
			}
			foreach (OcTreeNode ocTreeNode in this._children)
			{
				ocTreeNode.DrawImmediateBounds();
				if (ocTreeNode.Depth < depth)
				{
					ocTreeNode.DrawChildrenNodes(depth);
				}
			}
		}

		// Token: 0x040046B5 RID: 18101
		private const int MAX_DEPTH = 3;

		// Token: 0x040046B7 RID: 18103
		private Bounds _bounds;

		// Token: 0x040046B8 RID: 18104
		private Vector3 _center;

		// Token: 0x040046B9 RID: 18105
		private int _depth;

		// Token: 0x040046BA RID: 18106
		private float _length;

		// Token: 0x040046BB RID: 18107
		private OcTreeNode _root;

		// Token: 0x040046BC RID: 18108
		private OcTreeNode _parent;

		// Token: 0x040046BD RID: 18109
		private OcTreeNode[] _children;
	}
}
