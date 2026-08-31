using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004C5 RID: 1221
	public static class DTSweep
	{
		// Token: 0x06001BBC RID: 7100 RVA: 0x00094D5C File Offset: 0x00092F5C
		public static void Triangulate(DTSweepContext tcx)
		{
			tcx.CreateAdvancingFront();
			DTSweep.Sweep(tcx);
			DTSweep.FixupConstrainedEdges(tcx);
			if (tcx.TriangulationMode == TriangulationMode.Polygon)
			{
				DTSweep.FinalizationPolygon(tcx);
			}
			else
			{
				DTSweep.FinalizationConvexHull(tcx);
				if (tcx.TriangulationMode == TriangulationMode.Constrained)
				{
					tcx.FinalizeTriangulation();
				}
				else
				{
					tcx.FinalizeTriangulation();
				}
			}
			tcx.Done();
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x00094DB0 File Offset: 0x00092FB0
		private static void Sweep(DTSweepContext tcx)
		{
			List<TriangulationPoint> points = tcx.Points;
			for (int i = 1; i < points.Count; i++)
			{
				TriangulationPoint triangulationPoint = points[i];
				AdvancingFrontNode advancingFrontNode = DTSweep.PointEvent(tcx, triangulationPoint);
				if (advancingFrontNode != null && triangulationPoint.HasEdges)
				{
					foreach (DTSweepConstraint dtsweepConstraint in triangulationPoint.Edges)
					{
						DTSweep.EdgeEvent(tcx, dtsweepConstraint, advancingFrontNode);
					}
				}
				tcx.Update(null);
			}
		}

		// Token: 0x06001BBE RID: 7102 RVA: 0x00094E44 File Offset: 0x00093044
		private static void FixupConstrainedEdges(DTSweepContext tcx)
		{
			foreach (DelaunayTriangle delaunayTriangle in tcx.Triangles)
			{
				for (int i = 0; i < 3; i++)
				{
					if (!delaunayTriangle.GetConstrainedEdgeCCW(delaunayTriangle.Points[i]))
					{
						DTSweepConstraint dtsweepConstraint = null;
						if (delaunayTriangle.GetEdgeCCW(delaunayTriangle.Points[i], out dtsweepConstraint))
						{
							delaunayTriangle.MarkConstrainedEdge((i + 2) % 3);
						}
					}
				}
			}
		}

		// Token: 0x06001BBF RID: 7103 RVA: 0x00094ED4 File Offset: 0x000930D4
		private static void FinalizationConvexHull(DTSweepContext tcx)
		{
			AdvancingFrontNode advancingFrontNode = tcx.Front.Head.Next;
			AdvancingFrontNode advancingFrontNode2 = advancingFrontNode.Next;
			TriangulationPoint triangulationPoint = advancingFrontNode.Point;
			DTSweep.TurnAdvancingFrontConvex(tcx, advancingFrontNode, advancingFrontNode2);
			advancingFrontNode = tcx.Front.Tail.Prev;
			DelaunayTriangle delaunayTriangle;
			if (advancingFrontNode.Triangle.Contains(advancingFrontNode.Next.Point) && advancingFrontNode.Triangle.Contains(advancingFrontNode.Prev.Point))
			{
				delaunayTriangle = advancingFrontNode.Triangle.NeighborAcrossFrom(advancingFrontNode.Point);
				DTSweep.RotateTrianglePair(advancingFrontNode.Triangle, advancingFrontNode.Point, delaunayTriangle, delaunayTriangle.OppositePoint(advancingFrontNode.Triangle, advancingFrontNode.Point));
				tcx.MapTriangleToNodes(advancingFrontNode.Triangle);
				tcx.MapTriangleToNodes(delaunayTriangle);
			}
			advancingFrontNode = tcx.Front.Head.Next;
			if (advancingFrontNode.Triangle.Contains(advancingFrontNode.Prev.Point) && advancingFrontNode.Triangle.Contains(advancingFrontNode.Next.Point))
			{
				delaunayTriangle = advancingFrontNode.Triangle.NeighborAcrossFrom(advancingFrontNode.Point);
				DTSweep.RotateTrianglePair(advancingFrontNode.Triangle, advancingFrontNode.Point, delaunayTriangle, delaunayTriangle.OppositePoint(advancingFrontNode.Triangle, advancingFrontNode.Point));
				tcx.MapTriangleToNodes(advancingFrontNode.Triangle);
				tcx.MapTriangleToNodes(delaunayTriangle);
			}
			triangulationPoint = tcx.Front.Head.Point;
			advancingFrontNode2 = tcx.Front.Tail.Prev;
			delaunayTriangle = advancingFrontNode2.Triangle;
			TriangulationPoint triangulationPoint2 = advancingFrontNode2.Point;
			advancingFrontNode2.Triangle = null;
			for (;;)
			{
				tcx.RemoveFromList(delaunayTriangle);
				triangulationPoint2 = delaunayTriangle.PointCCWFrom(triangulationPoint2);
				if (triangulationPoint2 == triangulationPoint)
				{
					break;
				}
				DelaunayTriangle delaunayTriangle2 = delaunayTriangle.NeighborCCWFrom(triangulationPoint2);
				delaunayTriangle.Clear();
				delaunayTriangle = delaunayTriangle2;
			}
			triangulationPoint = tcx.Front.Head.Next.Point;
			triangulationPoint2 = delaunayTriangle.PointCWFrom(tcx.Front.Head.Point);
			DelaunayTriangle delaunayTriangle3 = delaunayTriangle.NeighborCWFrom(tcx.Front.Head.Point);
			delaunayTriangle.Clear();
			delaunayTriangle = delaunayTriangle3;
			while (triangulationPoint2 != triangulationPoint)
			{
				tcx.RemoveFromList(delaunayTriangle);
				triangulationPoint2 = delaunayTriangle.PointCCWFrom(triangulationPoint2);
				DelaunayTriangle delaunayTriangle4 = delaunayTriangle.NeighborCCWFrom(triangulationPoint2);
				delaunayTriangle.Clear();
				delaunayTriangle = delaunayTriangle4;
			}
			tcx.Front.Head = tcx.Front.Head.Next;
			tcx.Front.Head.Prev = null;
			tcx.Front.Tail = tcx.Front.Tail.Prev;
			tcx.Front.Tail.Next = null;
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x00095148 File Offset: 0x00093348
		private static void TurnAdvancingFrontConvex(DTSweepContext tcx, AdvancingFrontNode b, AdvancingFrontNode c)
		{
			AdvancingFrontNode advancingFrontNode = b;
			while (c != tcx.Front.Tail)
			{
				if (TriangulationUtil.Orient2d(b.Point, c.Point, c.Next.Point) == Orientation.CCW)
				{
					DTSweep.Fill(tcx, c);
					c = c.Next;
				}
				else if (b != advancingFrontNode && TriangulationUtil.Orient2d(b.Prev.Point, b.Point, c.Point) == Orientation.CCW)
				{
					DTSweep.Fill(tcx, b);
					b = b.Prev;
				}
				else
				{
					b = c;
					c = c.Next;
				}
			}
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x000951D8 File Offset: 0x000933D8
		private static void FinalizationPolygon(DTSweepContext tcx)
		{
			DelaunayTriangle delaunayTriangle = tcx.Front.Head.Next.Triangle;
			TriangulationPoint point = tcx.Front.Head.Next.Point;
			while (!delaunayTriangle.GetConstrainedEdgeCW(point))
			{
				DelaunayTriangle delaunayTriangle2 = delaunayTriangle.NeighborCCWFrom(point);
				if (delaunayTriangle2 == null)
				{
					break;
				}
				delaunayTriangle = delaunayTriangle2;
			}
			tcx.MeshClean(delaunayTriangle);
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x00095230 File Offset: 0x00093430
		private static void FinalizationConstraints(DTSweepContext tcx)
		{
			DelaunayTriangle delaunayTriangle = tcx.Front.Head.Triangle;
			TriangulationPoint point = tcx.Front.Head.Point;
			while (!delaunayTriangle.GetConstrainedEdgeCW(point))
			{
				DelaunayTriangle delaunayTriangle2 = delaunayTriangle.NeighborCCWFrom(point);
				if (delaunayTriangle2 == null)
				{
					break;
				}
				delaunayTriangle = delaunayTriangle2;
			}
			tcx.MeshClean(delaunayTriangle);
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x00095280 File Offset: 0x00093480
		private static AdvancingFrontNode PointEvent(DTSweepContext tcx, TriangulationPoint point)
		{
			AdvancingFrontNode advancingFrontNode = tcx.LocateNode(point);
			if (advancingFrontNode == null || point == null)
			{
				return null;
			}
			AdvancingFrontNode advancingFrontNode2 = DTSweep.NewFrontTriangle(tcx, point, advancingFrontNode);
			if (point.X <= advancingFrontNode.Point.X)
			{
				DTSweep.Fill(tcx, advancingFrontNode);
			}
			tcx.AddNode(advancingFrontNode2);
			DTSweep.FillAdvancingFront(tcx, advancingFrontNode2);
			return advancingFrontNode2;
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x000952D0 File Offset: 0x000934D0
		private static AdvancingFrontNode NewFrontTriangle(DTSweepContext tcx, TriangulationPoint point, AdvancingFrontNode node)
		{
			DelaunayTriangle delaunayTriangle = new DelaunayTriangle(point, node.Point, node.Next.Point);
			delaunayTriangle.MarkNeighbor(node.Triangle);
			tcx.Triangles.Add(delaunayTriangle);
			AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(point);
			advancingFrontNode.Next = node.Next;
			advancingFrontNode.Prev = node;
			node.Next.Prev = advancingFrontNode;
			node.Next = advancingFrontNode;
			tcx.AddNode(advancingFrontNode);
			if (!DTSweep.Legalize(tcx, delaunayTriangle))
			{
				tcx.MapTriangleToNodes(delaunayTriangle);
			}
			return advancingFrontNode;
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x00095354 File Offset: 0x00093554
		private static void EdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			try
			{
				tcx.EdgeEvent.ConstrainedEdge = edge;
				tcx.EdgeEvent.Right = edge.P.X > edge.Q.X;
				if (!DTSweep.IsEdgeSideOfTriangle(node.Triangle, edge.P, edge.Q))
				{
					DTSweep.FillEdgeEvent(tcx, edge, node);
					DTSweep.EdgeEvent(tcx, edge.P, edge.Q, node.Triangle, edge.Q);
				}
			}
			catch (PointOnEdgeException)
			{
				throw;
			}
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000953E8 File Offset: 0x000935E8
		private static void FillEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (tcx.EdgeEvent.Right)
			{
				DTSweep.FillRightAboveEdgeEvent(tcx, edge, node);
				return;
			}
			DTSweep.FillLeftAboveEdgeEvent(tcx, edge, node);
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x00095408 File Offset: 0x00093608
		private static void FillRightConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			DTSweep.Fill(tcx, node.Next);
			if (node.Next.Point != edge.P && TriangulationUtil.Orient2d(edge.Q, node.Next.Point, edge.P) == Orientation.CCW && TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
			{
				DTSweep.FillRightConcaveEdgeEvent(tcx, edge, node);
			}
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x00095484 File Offset: 0x00093684
		private static void FillRightConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (TriangulationUtil.Orient2d(node.Next.Point, node.Next.Next.Point, node.Next.Next.Next.Point) == Orientation.CCW)
			{
				DTSweep.FillRightConcaveEdgeEvent(tcx, edge, node.Next);
				return;
			}
			if (TriangulationUtil.Orient2d(edge.Q, node.Next.Next.Point, edge.P) == Orientation.CCW)
			{
				DTSweep.FillRightConvexEdgeEvent(tcx, edge, node.Next);
			}
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x00095508 File Offset: 0x00093708
		private static void FillRightBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (node.Point.X < edge.P.X)
			{
				if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
				{
					DTSweep.FillRightConcaveEdgeEvent(tcx, edge, node);
					return;
				}
				DTSweep.FillRightConvexEdgeEvent(tcx, edge, node);
				DTSweep.FillRightBelowEdgeEvent(tcx, edge, node);
			}
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x00095570 File Offset: 0x00093770
		private static void FillRightAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			while (node.Next.Point.X < edge.P.X)
			{
				if (TriangulationUtil.Orient2d(edge.Q, node.Next.Point, edge.P) == Orientation.CCW)
				{
					DTSweep.FillRightBelowEdgeEvent(tcx, edge, node);
				}
				else
				{
					node = node.Next;
				}
			}
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x000955D0 File Offset: 0x000937D0
		private static void FillLeftConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (TriangulationUtil.Orient2d(node.Prev.Point, node.Prev.Prev.Point, node.Prev.Prev.Prev.Point) == Orientation.CW)
			{
				DTSweep.FillLeftConcaveEdgeEvent(tcx, edge, node.Prev);
				return;
			}
			if (TriangulationUtil.Orient2d(edge.Q, node.Prev.Prev.Point, edge.P) == Orientation.CW)
			{
				DTSweep.FillLeftConvexEdgeEvent(tcx, edge, node.Prev);
			}
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x00095654 File Offset: 0x00093854
		private static void FillLeftConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			DTSweep.Fill(tcx, node.Prev);
			if (node.Prev.Point != edge.P && TriangulationUtil.Orient2d(edge.Q, node.Prev.Point, edge.P) == Orientation.CW && TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
			{
				DTSweep.FillLeftConcaveEdgeEvent(tcx, edge, node);
			}
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x000956D0 File Offset: 0x000938D0
		private static void FillLeftBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (node.Point.X > edge.P.X)
			{
				if (TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
				{
					DTSweep.FillLeftConcaveEdgeEvent(tcx, edge, node);
					return;
				}
				DTSweep.FillLeftConvexEdgeEvent(tcx, edge, node);
				DTSweep.FillLeftBelowEdgeEvent(tcx, edge, node);
			}
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x00095738 File Offset: 0x00093938
		private static void FillLeftAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			while (node.Prev.Point.X > edge.P.X)
			{
				if (TriangulationUtil.Orient2d(edge.Q, node.Prev.Point, edge.P) == Orientation.CW)
				{
					DTSweep.FillLeftBelowEdgeEvent(tcx, edge, node);
				}
				else
				{
					node = node.Prev;
				}
			}
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x00095794 File Offset: 0x00093994
		private static bool IsEdgeSideOfTriangle(DelaunayTriangle triangle, TriangulationPoint ep, TriangulationPoint eq)
		{
			int num = triangle.EdgeIndex(ep, eq);
			if (num == -1)
			{
				return false;
			}
			triangle.MarkConstrainedEdge(num);
			triangle = triangle.Neighbors[num];
			if (triangle != null)
			{
				triangle.MarkConstrainedEdge(ep, eq);
			}
			return true;
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x000957D4 File Offset: 0x000939D4
		private static void EdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle triangle, TriangulationPoint point)
		{
			if (triangle == null)
			{
				return;
			}
			if (DTSweep.IsEdgeSideOfTriangle(triangle, ep, eq))
			{
				return;
			}
			TriangulationPoint triangulationPoint = triangle.PointCCWFrom(point);
			Orientation orientation = TriangulationUtil.Orient2d(eq, triangulationPoint, ep);
			if (orientation == Orientation.Collinear)
			{
				if (triangle.Contains(eq) && triangle.Contains(triangulationPoint))
				{
					triangle.MarkConstrainedEdge(eq, triangulationPoint);
					tcx.EdgeEvent.ConstrainedEdge.Q = triangulationPoint;
					triangle = triangle.NeighborAcrossFrom(point);
					DTSweep.EdgeEvent(tcx, ep, triangulationPoint, triangle, triangulationPoint);
					return;
				}
				throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", ep, eq, triangulationPoint);
			}
			else
			{
				TriangulationPoint triangulationPoint2 = triangle.PointCWFrom(point);
				Orientation orientation2 = TriangulationUtil.Orient2d(eq, triangulationPoint2, ep);
				if (orientation2 == Orientation.Collinear)
				{
					if (triangle.Contains(eq) && triangle.Contains(triangulationPoint2))
					{
						triangle.MarkConstrainedEdge(eq, triangulationPoint2);
						tcx.EdgeEvent.ConstrainedEdge.Q = triangulationPoint2;
						triangle = triangle.NeighborAcrossFrom(point);
						DTSweep.EdgeEvent(tcx, ep, triangulationPoint2, triangle, triangulationPoint2);
						return;
					}
					throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", ep, eq, triangulationPoint2);
				}
				else
				{
					if (orientation == orientation2)
					{
						if (orientation == Orientation.CW)
						{
							triangle = triangle.NeighborCCWFrom(point);
						}
						else
						{
							triangle = triangle.NeighborCWFrom(point);
						}
						DTSweep.EdgeEvent(tcx, ep, eq, triangle, point);
						return;
					}
					DTSweep.FlipEdgeEvent(tcx, ep, eq, triangle, point);
					return;
				}
			}
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x000958EC File Offset: 0x00093AEC
		private static void FlipEdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle t, TriangulationPoint p)
		{
			DelaunayTriangle delaunayTriangle = t.NeighborAcrossFrom(p);
			TriangulationPoint triangulationPoint = delaunayTriangle.OppositePoint(t, p);
			if (delaunayTriangle == null)
			{
				throw new InvalidOperationException("[BUG:FIXME] FLIP failed due to missing triangle");
			}
			if (TriangulationUtil.InScanArea(p, t.PointCCWFrom(p), t.PointCWFrom(p), triangulationPoint))
			{
				DTSweep.RotateTrianglePair(t, p, delaunayTriangle, triangulationPoint);
				tcx.MapTriangleToNodes(t);
				tcx.MapTriangleToNodes(delaunayTriangle);
				if (p != eq || triangulationPoint != ep)
				{
					Orientation orientation = TriangulationUtil.Orient2d(eq, triangulationPoint, ep);
					t = DTSweep.NextFlipTriangle(tcx, orientation, t, delaunayTriangle, p, triangulationPoint);
					DTSweep.FlipEdgeEvent(tcx, ep, eq, t, p);
					return;
				}
				if (eq == tcx.EdgeEvent.ConstrainedEdge.Q && ep == tcx.EdgeEvent.ConstrainedEdge.P)
				{
					t.MarkConstrainedEdge(ep, eq);
					delaunayTriangle.MarkConstrainedEdge(ep, eq);
					DTSweep.Legalize(tcx, t);
					DTSweep.Legalize(tcx, delaunayTriangle);
					return;
				}
			}
			else
			{
				TriangulationPoint triangulationPoint2 = null;
				if (DTSweep.NextFlipPoint(ep, eq, delaunayTriangle, triangulationPoint, out triangulationPoint2))
				{
					DTSweep.FlipScanEdgeEvent(tcx, ep, eq, t, delaunayTriangle, triangulationPoint2);
					DTSweep.EdgeEvent(tcx, ep, eq, t, p);
				}
			}
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x000959E8 File Offset: 0x00093BE8
		private static bool NextFlipPoint(TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle ot, TriangulationPoint op, out TriangulationPoint newP)
		{
			newP = null;
			switch (TriangulationUtil.Orient2d(eq, op, ep))
			{
			case Orientation.CW:
				newP = ot.PointCCWFrom(op);
				return true;
			case Orientation.CCW:
				newP = ot.PointCWFrom(op);
				return true;
			case Orientation.Collinear:
				return false;
			default:
				throw new NotImplementedException("Orientation not handled");
			}
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x00095A3C File Offset: 0x00093C3C
		private static DelaunayTriangle NextFlipTriangle(DTSweepContext tcx, Orientation o, DelaunayTriangle t, DelaunayTriangle ot, TriangulationPoint p, TriangulationPoint op)
		{
			int num;
			if (o == Orientation.CCW)
			{
				num = ot.EdgeIndex(p, op);
				ot.EdgeIsDelaunay[num] = true;
				DTSweep.Legalize(tcx, ot);
				ot.EdgeIsDelaunay.Clear();
				return t;
			}
			num = t.EdgeIndex(p, op);
			t.EdgeIsDelaunay[num] = true;
			DTSweep.Legalize(tcx, t);
			t.EdgeIsDelaunay.Clear();
			return ot;
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x00095AA8 File Offset: 0x00093CA8
		private static void FlipScanEdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle flipTriangle, DelaunayTriangle t, TriangulationPoint p)
		{
			DelaunayTriangle delaunayTriangle = t.NeighborAcrossFrom(p);
			TriangulationPoint triangulationPoint = delaunayTriangle.OppositePoint(t, p);
			if (delaunayTriangle == null)
			{
				throw new Exception("[BUG:FIXME] FLIP failed due to missing triangle");
			}
			if (TriangulationUtil.InScanArea(eq, flipTriangle.PointCCWFrom(eq), flipTriangle.PointCWFrom(eq), triangulationPoint))
			{
				DTSweep.FlipEdgeEvent(tcx, eq, triangulationPoint, delaunayTriangle, triangulationPoint);
				return;
			}
			TriangulationPoint triangulationPoint2;
			if (DTSweep.NextFlipPoint(ep, eq, delaunayTriangle, triangulationPoint, out triangulationPoint2))
			{
				DTSweep.FlipScanEdgeEvent(tcx, ep, eq, flipTriangle, delaunayTriangle, triangulationPoint2);
			}
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x00095B14 File Offset: 0x00093D14
		private static void FillAdvancingFront(DTSweepContext tcx, AdvancingFrontNode n)
		{
			AdvancingFrontNode advancingFrontNode = n.Next;
			while (advancingFrontNode.HasNext)
			{
				double num = DTSweep.HoleAngle(advancingFrontNode);
				if (num > 1.5707963267948966 || num < -1.5707963267948966)
				{
					break;
				}
				DTSweep.Fill(tcx, advancingFrontNode);
				advancingFrontNode = advancingFrontNode.Next;
			}
			advancingFrontNode = n.Prev;
			while (advancingFrontNode.HasPrev)
			{
				double num = DTSweep.HoleAngle(advancingFrontNode);
				if (num > 1.5707963267948966 || num < -1.5707963267948966)
				{
					break;
				}
				DTSweep.Fill(tcx, advancingFrontNode);
				advancingFrontNode = advancingFrontNode.Prev;
			}
			if (n.HasNext && n.Next.HasNext)
			{
				double num = DTSweep.BasinAngle(n);
				if (num < 2.356194490192345)
				{
					DTSweep.FillBasin(tcx, n);
				}
			}
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x00095BCC File Offset: 0x00093DCC
		private static void FillBasin(DTSweepContext tcx, AdvancingFrontNode node)
		{
			if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
			{
				tcx.Basin.leftNode = node;
			}
			else
			{
				tcx.Basin.leftNode = node.Next;
			}
			tcx.Basin.bottomNode = tcx.Basin.leftNode;
			while (tcx.Basin.bottomNode.HasNext && tcx.Basin.bottomNode.Point.Y >= tcx.Basin.bottomNode.Next.Point.Y)
			{
				tcx.Basin.bottomNode = tcx.Basin.bottomNode.Next;
			}
			if (tcx.Basin.bottomNode == tcx.Basin.leftNode)
			{
				return;
			}
			tcx.Basin.rightNode = tcx.Basin.bottomNode;
			while (tcx.Basin.rightNode.HasNext && tcx.Basin.rightNode.Point.Y < tcx.Basin.rightNode.Next.Point.Y)
			{
				tcx.Basin.rightNode = tcx.Basin.rightNode.Next;
			}
			if (tcx.Basin.rightNode == tcx.Basin.bottomNode)
			{
				return;
			}
			tcx.Basin.width = tcx.Basin.rightNode.Point.X - tcx.Basin.leftNode.Point.X;
			tcx.Basin.leftHighest = tcx.Basin.leftNode.Point.Y > tcx.Basin.rightNode.Point.Y;
			DTSweep.FillBasinReq(tcx, tcx.Basin.bottomNode);
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x00095DC0 File Offset: 0x00093FC0
		private static void FillBasinReq(DTSweepContext tcx, AdvancingFrontNode node)
		{
			if (DTSweep.IsShallow(tcx, node))
			{
				return;
			}
			DTSweep.Fill(tcx, node);
			if (node.Prev == tcx.Basin.leftNode && node.Next == tcx.Basin.rightNode)
			{
				return;
			}
			if (node.Prev == tcx.Basin.leftNode)
			{
				if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CW)
				{
					return;
				}
				node = node.Next;
			}
			else if (node.Next == tcx.Basin.rightNode)
			{
				if (TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CCW)
				{
					return;
				}
				node = node.Prev;
			}
			else if (node.Prev.Point.Y < node.Next.Point.Y)
			{
				node = node.Prev;
			}
			else
			{
				node = node.Next;
			}
			DTSweep.FillBasinReq(tcx, node);
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00095ED0 File Offset: 0x000940D0
		private static bool IsShallow(DTSweepContext tcx, AdvancingFrontNode node)
		{
			double num;
			if (tcx.Basin.leftHighest)
			{
				num = tcx.Basin.leftNode.Point.Y - node.Point.Y;
			}
			else
			{
				num = tcx.Basin.rightNode.Point.Y - node.Point.Y;
			}
			return tcx.Basin.width > num;
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x00095F44 File Offset: 0x00094144
		private static double HoleAngle(AdvancingFrontNode node)
		{
			double x = node.Point.X;
			double y = node.Point.Y;
			double num = node.Next.Point.X - x;
			double num2 = node.Next.Point.Y - y;
			double num3 = node.Prev.Point.X - x;
			double num4 = node.Prev.Point.Y - y;
			return Math.Atan2(num * num4 - num2 * num3, num * num3 + num2 * num4);
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x00095FD0 File Offset: 0x000941D0
		private static double BasinAngle(AdvancingFrontNode node)
		{
			double num = node.Point.X - node.Next.Next.Point.X;
			return Math.Atan2(node.Point.Y - node.Next.Next.Point.Y, num);
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x00096028 File Offset: 0x00094228
		private static void Fill(DTSweepContext tcx, AdvancingFrontNode node)
		{
			DelaunayTriangle delaunayTriangle = new DelaunayTriangle(node.Prev.Point, node.Point, node.Next.Point);
			delaunayTriangle.MarkNeighbor(node.Prev.Triangle);
			delaunayTriangle.MarkNeighbor(node.Triangle);
			tcx.Triangles.Add(delaunayTriangle);
			node.Prev.Next = node.Next;
			node.Next.Prev = node.Prev;
			tcx.RemoveNode(node);
			if (!DTSweep.Legalize(tcx, delaunayTriangle))
			{
				tcx.MapTriangleToNodes(delaunayTriangle);
			}
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000960BC File Offset: 0x000942BC
		private static bool Legalize(DTSweepContext tcx, DelaunayTriangle t)
		{
			for (int i = 0; i < 3; i++)
			{
				if (!t.EdgeIsDelaunay[i])
				{
					DelaunayTriangle delaunayTriangle = t.Neighbors[i];
					if (delaunayTriangle != null)
					{
						TriangulationPoint triangulationPoint = t.Points[i];
						TriangulationPoint triangulationPoint2 = delaunayTriangle.OppositePoint(t, triangulationPoint);
						int num = delaunayTriangle.IndexOf(triangulationPoint2);
						if (delaunayTriangle.EdgeIsConstrained[num] || delaunayTriangle.EdgeIsDelaunay[num])
						{
							t.SetConstrainedEdgeAcross(triangulationPoint, delaunayTriangle.EdgeIsConstrained[num]);
						}
						else if (TriangulationUtil.SmartIncircle(triangulationPoint, t.PointCCWFrom(triangulationPoint), t.PointCWFrom(triangulationPoint), triangulationPoint2))
						{
							t.EdgeIsDelaunay[i] = true;
							delaunayTriangle.EdgeIsDelaunay[num] = true;
							DTSweep.RotateTrianglePair(t, triangulationPoint, delaunayTriangle, triangulationPoint2);
							if (!DTSweep.Legalize(tcx, t))
							{
								tcx.MapTriangleToNodes(t);
							}
							if (!DTSweep.Legalize(tcx, delaunayTriangle))
							{
								tcx.MapTriangleToNodes(delaunayTriangle);
							}
							t.EdgeIsDelaunay[i] = false;
							delaunayTriangle.EdgeIsDelaunay[num] = false;
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x000961D4 File Offset: 0x000943D4
		private static void RotateTrianglePair(DelaunayTriangle t, TriangulationPoint p, DelaunayTriangle ot, TriangulationPoint op)
		{
			DelaunayTriangle delaunayTriangle = t.NeighborCCWFrom(p);
			DelaunayTriangle delaunayTriangle2 = t.NeighborCWFrom(p);
			DelaunayTriangle delaunayTriangle3 = ot.NeighborCCWFrom(op);
			DelaunayTriangle delaunayTriangle4 = ot.NeighborCWFrom(op);
			bool constrainedEdgeCCW = t.GetConstrainedEdgeCCW(p);
			bool constrainedEdgeCW = t.GetConstrainedEdgeCW(p);
			bool constrainedEdgeCCW2 = ot.GetConstrainedEdgeCCW(op);
			bool constrainedEdgeCW2 = ot.GetConstrainedEdgeCW(op);
			bool delaunayEdgeCCW = t.GetDelaunayEdgeCCW(p);
			bool delaunayEdgeCW = t.GetDelaunayEdgeCW(p);
			bool delaunayEdgeCCW2 = ot.GetDelaunayEdgeCCW(op);
			bool delaunayEdgeCW2 = ot.GetDelaunayEdgeCW(op);
			t.Legalize(p, op);
			ot.Legalize(op, p);
			ot.SetDelaunayEdgeCCW(p, delaunayEdgeCCW);
			t.SetDelaunayEdgeCW(p, delaunayEdgeCW);
			t.SetDelaunayEdgeCCW(op, delaunayEdgeCCW2);
			ot.SetDelaunayEdgeCW(op, delaunayEdgeCW2);
			ot.SetConstrainedEdgeCCW(p, constrainedEdgeCCW);
			t.SetConstrainedEdgeCW(p, constrainedEdgeCW);
			t.SetConstrainedEdgeCCW(op, constrainedEdgeCCW2);
			ot.SetConstrainedEdgeCW(op, constrainedEdgeCW2);
			t.Neighbors.Clear();
			ot.Neighbors.Clear();
			if (delaunayTriangle != null)
			{
				ot.MarkNeighbor(delaunayTriangle);
			}
			if (delaunayTriangle2 != null)
			{
				t.MarkNeighbor(delaunayTriangle2);
			}
			if (delaunayTriangle3 != null)
			{
				t.MarkNeighbor(delaunayTriangle3);
			}
			if (delaunayTriangle4 != null)
			{
				ot.MarkNeighbor(delaunayTriangle4);
			}
			t.MarkNeighbor(ot);
		}

		// Token: 0x04001767 RID: 5991
		private const double PI_div2 = 1.5707963267948966;

		// Token: 0x04001768 RID: 5992
		private const double PI_3div4 = 2.356194490192345;
	}
}
