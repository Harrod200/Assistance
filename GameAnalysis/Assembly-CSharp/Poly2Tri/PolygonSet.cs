using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004D2 RID: 1234
	public class PolygonSet
	{
		// Token: 0x06001C80 RID: 7296 RVA: 0x000979AB File Offset: 0x00095BAB
		public PolygonSet()
		{
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x000979BE File Offset: 0x00095BBE
		public PolygonSet(Polygon poly)
		{
			this._polygons.Add(poly);
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x000979DD File Offset: 0x00095BDD
		public void Add(Polygon p)
		{
			this._polygons.Add(p);
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001C83 RID: 7299 RVA: 0x000979EB File Offset: 0x00095BEB
		public IEnumerable<Polygon> Polygons
		{
			get
			{
				return this._polygons;
			}
		}

		// Token: 0x04001793 RID: 6035
		protected List<Polygon> _polygons = new List<Polygon>();
	}
}
