using System;
using System.Text;

namespace Poly2Tri
{
	// Token: 0x020004C3 RID: 1219
	public class AdvancingFront
	{
		// Token: 0x06001BB1 RID: 7089 RVA: 0x00094B71 File Offset: 0x00092D71
		public AdvancingFront(AdvancingFrontNode head, AdvancingFrontNode tail)
		{
			this.Head = head;
			this.Tail = tail;
			this.Search = head;
			this.AddNode(head);
			this.AddNode(tail);
		}

		// Token: 0x06001BB2 RID: 7090 RVA: 0x00094B9C File Offset: 0x00092D9C
		public void AddNode(AdvancingFrontNode node)
		{
		}

		// Token: 0x06001BB3 RID: 7091 RVA: 0x00094B9E File Offset: 0x00092D9E
		public void RemoveNode(AdvancingFrontNode node)
		{
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x00094BA0 File Offset: 0x00092DA0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (AdvancingFrontNode advancingFrontNode = this.Head; advancingFrontNode != this.Tail; advancingFrontNode = advancingFrontNode.Next)
			{
				stringBuilder.Append(advancingFrontNode.Point.X).Append("->");
			}
			stringBuilder.Append(this.Tail.Point.X);
			return stringBuilder.ToString();
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x00094C05 File Offset: 0x00092E05
		private AdvancingFrontNode FindSearchNode(double x)
		{
			return this.Search;
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x00094C0D File Offset: 0x00092E0D
		public AdvancingFrontNode LocateNode(TriangulationPoint point)
		{
			return this.LocateNode(point.X);
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x00094C1C File Offset: 0x00092E1C
		private AdvancingFrontNode LocateNode(double x)
		{
			AdvancingFrontNode advancingFrontNode = this.FindSearchNode(x);
			if (x < advancingFrontNode.Value)
			{
				while ((advancingFrontNode = advancingFrontNode.Prev) != null)
				{
					if (x >= advancingFrontNode.Value)
					{
						this.Search = advancingFrontNode;
						return advancingFrontNode;
					}
				}
			}
			else
			{
				while ((advancingFrontNode = advancingFrontNode.Next) != null)
				{
					if (x < advancingFrontNode.Value)
					{
						this.Search = advancingFrontNode.Prev;
						return advancingFrontNode.Prev;
					}
				}
			}
			return null;
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x00094C84 File Offset: 0x00092E84
		public AdvancingFrontNode LocatePoint(TriangulationPoint point)
		{
			double x = point.X;
			AdvancingFrontNode advancingFrontNode = this.FindSearchNode(x);
			double x2 = advancingFrontNode.Point.X;
			if (x == x2)
			{
				if (point != advancingFrontNode.Point)
				{
					if (point == advancingFrontNode.Prev.Point)
					{
						advancingFrontNode = advancingFrontNode.Prev;
					}
					else
					{
						if (point != advancingFrontNode.Next.Point)
						{
							throw new Exception("Failed to find Node for given afront point");
						}
						advancingFrontNode = advancingFrontNode.Next;
					}
				}
			}
			else if (x < x2)
			{
				while ((advancingFrontNode = advancingFrontNode.Prev) != null)
				{
					if (point == advancingFrontNode.Point)
					{
						break;
					}
				}
			}
			else
			{
				while ((advancingFrontNode = advancingFrontNode.Next) != null && point != advancingFrontNode.Point)
				{
				}
			}
			this.Search = advancingFrontNode;
			return advancingFrontNode;
		}

		// Token: 0x0400175F RID: 5983
		public AdvancingFrontNode Head;

		// Token: 0x04001760 RID: 5984
		public AdvancingFrontNode Tail;

		// Token: 0x04001761 RID: 5985
		protected AdvancingFrontNode Search;
	}
}
