using System;
using System.Collections.Generic;
using System.Text;

namespace Poly2Tri
{
	// Token: 0x020004D5 RID: 1237
	public class SplitComplexPolygonNode
	{
		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06001C9E RID: 7326 RVA: 0x00098F41 File Offset: 0x00097141
		public int NumConnected
		{
			get
			{
				return this.mConnected.Count;
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06001C9F RID: 7327 RVA: 0x00098F4E File Offset: 0x0009714E
		// (set) Token: 0x06001CA0 RID: 7328 RVA: 0x00098F56 File Offset: 0x00097156
		public Point2D Position
		{
			get
			{
				return this.mPosition;
			}
			set
			{
				this.mPosition = value;
			}
		}

		// Token: 0x17000407 RID: 1031
		public SplitComplexPolygonNode this[int index]
		{
			get
			{
				return this.mConnected[index];
			}
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x00098F6D File Offset: 0x0009716D
		public SplitComplexPolygonNode()
		{
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x00098F80 File Offset: 0x00097180
		public SplitComplexPolygonNode(Point2D pos)
		{
			this.mPosition = pos;
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x00098F9C File Offset: 0x0009719C
		public override bool Equals(object obj)
		{
			SplitComplexPolygonNode splitComplexPolygonNode = obj as SplitComplexPolygonNode;
			if (splitComplexPolygonNode == null)
			{
				return base.Equals(obj);
			}
			return this.Equals(splitComplexPolygonNode);
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x00098FC8 File Offset: 0x000971C8
		public bool Equals(SplitComplexPolygonNode pn)
		{
			return pn != null && this.mPosition != null && pn.Position != null && this.mPosition.Equals(pn.Position);
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x00098FF2 File Offset: 0x000971F2
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x00098FFA File Offset: 0x000971FA
		public static bool operator ==(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs)
		{
			if (lhs != null)
			{
				return lhs.Equals(rhs);
			}
			return rhs == null;
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x0009900D File Offset: 0x0009720D
		public static bool operator !=(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs)
		{
			if (lhs != null)
			{
				return !lhs.Equals(rhs);
			}
			return rhs != null;
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x00099024 File Offset: 0x00097224
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.Append(this.mPosition.ToString());
			stringBuilder.Append(" -> ");
			for (int i = 0; i < this.NumConnected; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(this.mConnected[i].Position.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x0009909D File Offset: 0x0009729D
		private bool IsRighter(double sinA, double cosA, double sinB, double cosB)
		{
			if (sinA < 0.0)
			{
				return sinB > 0.0 || cosA <= cosB;
			}
			return sinB >= 0.0 && cosA > cosB;
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x000990D4 File Offset: 0x000972D4
		private int remainder(int x, int modulus)
		{
			int i;
			for (i = x % modulus; i < 0; i += modulus)
			{
			}
			return i;
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x000990F0 File Offset: 0x000972F0
		public void AddConnection(SplitComplexPolygonNode toMe)
		{
			if (!this.mConnected.Contains(toMe) && toMe != this)
			{
				this.mConnected.Add(toMe);
			}
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x00099115 File Offset: 0x00097315
		public void RemoveConnection(SplitComplexPolygonNode fromMe)
		{
			this.mConnected.Remove(fromMe);
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x00099124 File Offset: 0x00097324
		private void RemoveConnectionByIndex(int index)
		{
			if (index < 0 || index >= this.mConnected.Count)
			{
				return;
			}
			this.mConnected.RemoveAt(index);
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x00099145 File Offset: 0x00097345
		public void ClearConnections()
		{
			this.mConnected.Clear();
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x00099152 File Offset: 0x00097352
		private bool IsConnectedTo(SplitComplexPolygonNode me)
		{
			return this.mConnected.Contains(me);
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x00099160 File Offset: 0x00097360
		public SplitComplexPolygonNode GetRightestConnection(SplitComplexPolygonNode incoming)
		{
			if (this.NumConnected == 0)
			{
				throw new Exception("the connection graph is inconsistent");
			}
			if (this.NumConnected == 1)
			{
				return incoming;
			}
			Point2D point2D = this.mPosition - incoming.mPosition;
			double num = point2D.Magnitude();
			point2D.Normalize();
			if (num <= MathUtil.EPSILON)
			{
				throw new Exception("Length too small");
			}
			SplitComplexPolygonNode splitComplexPolygonNode = null;
			for (int i = 0; i < this.NumConnected; i++)
			{
				if (!(this.mConnected[i] == incoming))
				{
					Point2D point2D2 = this.mConnected[i].mPosition - this.mPosition;
					double num2 = point2D2.MagnitudeSquared();
					point2D2.Normalize();
					if (num2 <= MathUtil.EPSILON * MathUtil.EPSILON)
					{
						throw new Exception("Length too small");
					}
					double num3 = Point2D.Dot(point2D, point2D2);
					double num4 = Point2D.Cross(point2D, point2D2);
					if (splitComplexPolygonNode != null)
					{
						Point2D point2D3 = splitComplexPolygonNode.mPosition - this.mPosition;
						point2D3.Normalize();
						double num5 = Point2D.Dot(point2D, point2D3);
						double num6 = Point2D.Cross(point2D, point2D3);
						if (this.IsRighter(num4, num3, num6, num5))
						{
							splitComplexPolygonNode = this.mConnected[i];
						}
					}
					else
					{
						splitComplexPolygonNode = this.mConnected[i];
					}
				}
			}
			return splitComplexPolygonNode;
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x000992AC File Offset: 0x000974AC
		public SplitComplexPolygonNode GetRightestConnection(Point2D incomingDir)
		{
			SplitComplexPolygonNode splitComplexPolygonNode = new SplitComplexPolygonNode(this.mPosition - incomingDir);
			return this.GetRightestConnection(splitComplexPolygonNode);
		}

		// Token: 0x04001797 RID: 6039
		private List<SplitComplexPolygonNode> mConnected = new List<SplitComplexPolygonNode>();

		// Token: 0x04001798 RID: 6040
		private Point2D mPosition;
	}
}
