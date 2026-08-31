using System;

namespace Poly2Tri
{
	// Token: 0x020004C9 RID: 1225
	public class DTSweepDebugContext : TriangulationDebugContext
	{
		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001BF2 RID: 7154 RVA: 0x00096683 File Offset: 0x00094883
		// (set) Token: 0x06001BF3 RID: 7155 RVA: 0x0009668B File Offset: 0x0009488B
		public DelaunayTriangle PrimaryTriangle
		{
			get
			{
				return this._primaryTriangle;
			}
			set
			{
				this._primaryTriangle = value;
				this._tcx.Update("set PrimaryTriangle");
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06001BF4 RID: 7156 RVA: 0x000966A4 File Offset: 0x000948A4
		// (set) Token: 0x06001BF5 RID: 7157 RVA: 0x000966AC File Offset: 0x000948AC
		public DelaunayTriangle SecondaryTriangle
		{
			get
			{
				return this._secondaryTriangle;
			}
			set
			{
				this._secondaryTriangle = value;
				this._tcx.Update("set SecondaryTriangle");
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x000966C5 File Offset: 0x000948C5
		// (set) Token: 0x06001BF7 RID: 7159 RVA: 0x000966CD File Offset: 0x000948CD
		public TriangulationPoint ActivePoint
		{
			get
			{
				return this._activePoint;
			}
			set
			{
				this._activePoint = value;
				this._tcx.Update("set ActivePoint");
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001BF8 RID: 7160 RVA: 0x000966E6 File Offset: 0x000948E6
		// (set) Token: 0x06001BF9 RID: 7161 RVA: 0x000966EE File Offset: 0x000948EE
		public AdvancingFrontNode ActiveNode
		{
			get
			{
				return this._activeNode;
			}
			set
			{
				this._activeNode = value;
				this._tcx.Update("set ActiveNode");
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06001BFA RID: 7162 RVA: 0x00096707 File Offset: 0x00094907
		// (set) Token: 0x06001BFB RID: 7163 RVA: 0x0009670F File Offset: 0x0009490F
		public DTSweepConstraint ActiveConstraint
		{
			get
			{
				return this._activeConstraint;
			}
			set
			{
				this._activeConstraint = value;
				this._tcx.Update("set ActiveConstraint");
			}
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x00096728 File Offset: 0x00094928
		public DTSweepDebugContext(DTSweepContext tcx)
			: base(tcx)
		{
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x00096731 File Offset: 0x00094931
		public bool IsDebugContext
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001BFE RID: 7166 RVA: 0x00096734 File Offset: 0x00094934
		public override void Clear()
		{
			this.PrimaryTriangle = null;
			this.SecondaryTriangle = null;
			this.ActivePoint = null;
			this.ActiveNode = null;
			this.ActiveConstraint = null;
		}

		// Token: 0x04001775 RID: 6005
		private DelaunayTriangle _primaryTriangle;

		// Token: 0x04001776 RID: 6006
		private DelaunayTriangle _secondaryTriangle;

		// Token: 0x04001777 RID: 6007
		private TriangulationPoint _activePoint;

		// Token: 0x04001778 RID: 6008
		private AdvancingFrontNode _activeNode;

		// Token: 0x04001779 RID: 6009
		private DTSweepConstraint _activeConstraint;
	}
}
