using System;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009FA RID: 2554
	public class WaypointPathRendering : IRenderPath
	{
		// Token: 0x060061A0 RID: 24992 RVA: 0x002DE524 File Offset: 0x002DC724
		public WaypointPathRendering(string name)
		{
			this._name = name;
			this._pathSegmentPool = new Stack<WaypointPathRendering.PathSegment>();
			this._activePathSegmentMap = new Dictionary<int, List<WaypointPathRendering.PathSegment>>();
			this._highlightedPathSegments = new HashSet<int>();
			this._invalidPlacementPathSegments = new HashSet<int>();
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x002DE574 File Offset: 0x002DC774
		public void ClearActivePathsToRender()
		{
			foreach (KeyValuePair<int, List<WaypointPathRendering.PathSegment>> keyValuePair in this._activePathSegmentMap)
			{
				foreach (WaypointPathRendering.PathSegment pathSegment in keyValuePair.Value)
				{
					pathSegment.ClearLine();
					this._pathSegmentPool.Push(pathSegment);
				}
				keyValuePair.Value.Clear();
			}
			this._activePathSegmentMap.Clear();
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x002DE628 File Offset: 0x002DC828
		public void SubmitPathToRender(List<Vector3> points, Color pathColor, int waypointID)
		{
			WaypointPathRendering.PathSegment pathSegment = this.ObtainPathSegment();
			pathSegment.RenderPath(points, pathColor);
			pathSegment.ToggleRenderPathDefaultColor(this._isRenderingWithDefaultColor);
			pathSegment.ToggleRenderPathHighlight(this._highlightedPathSegments.Contains(waypointID), this._invalidPlacementPathSegments.Contains(waypointID));
			this.MapPathSegmentToId(pathSegment, waypointID);
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x002DE678 File Offset: 0x002DC878
		public void SubmitPathToRender(List<Vector3> points, Color pathColor, Vector2 alphaBlend, int waypointID)
		{
			WaypointPathRendering.PathSegment pathSegment = this.ObtainPathSegment();
			if (!pathSegment.RenderPath(points, pathColor, alphaBlend))
			{
				pathSegment.ToggleRenderPathDefaultColor(this._isRenderingWithDefaultColor);
				pathSegment.ToggleRenderPathHighlight(this._highlightedPathSegments.Contains(waypointID), this._invalidPlacementPathSegments.Contains(waypointID));
			}
			this.MapPathSegmentToId(pathSegment, waypointID);
		}

		// Token: 0x060061A4 RID: 24996 RVA: 0x002DE6CC File Offset: 0x002DC8CC
		private WaypointPathRendering.PathSegment ObtainPathSegment()
		{
			WaypointPathRendering.PathSegment pathSegment;
			if (this._pathSegmentPool.Count != 0)
			{
				pathSegment = this._pathSegmentPool.Pop();
			}
			else
			{
				pathSegment = new WaypointPathRendering.PathSegment(this._name);
			}
			pathSegment.ToggleRenderState(this._isPathRendering);
			return pathSegment;
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x002DE70D File Offset: 0x002DC90D
		private void MapPathSegmentToId(WaypointPathRendering.PathSegment pathSegment, int waypointID)
		{
			if (!this._activePathSegmentMap.ContainsKey(waypointID))
			{
				this._activePathSegmentMap.Add(waypointID, new List<WaypointPathRendering.PathSegment>());
			}
			this._activePathSegmentMap[waypointID].Add(pathSegment);
		}

		// Token: 0x060061A6 RID: 24998 RVA: 0x002DE740 File Offset: 0x002DC940
		public void TogglePathRendering()
		{
			this._isPathRendering = !this._isPathRendering;
			foreach (KeyValuePair<int, List<WaypointPathRendering.PathSegment>> keyValuePair in this._activePathSegmentMap)
			{
				foreach (WaypointPathRendering.PathSegment pathSegment in keyValuePair.Value)
				{
					pathSegment.ToggleRenderState(this._isPathRendering);
				}
			}
		}

		// Token: 0x060061A7 RID: 24999 RVA: 0x002DE7E4 File Offset: 0x002DC9E4
		public void ToggleDefaultColorPathRender()
		{
			this._isRenderingWithDefaultColor = !this._isRenderingWithDefaultColor;
			foreach (KeyValuePair<int, List<WaypointPathRendering.PathSegment>> keyValuePair in this._activePathSegmentMap)
			{
				foreach (WaypointPathRendering.PathSegment pathSegment in keyValuePair.Value)
				{
					pathSegment.ToggleRenderPathDefaultColor(this._isRenderingWithDefaultColor);
				}
			}
		}

		// Token: 0x060061A8 RID: 25000 RVA: 0x002DE888 File Offset: 0x002DCA88
		public void EnableHighlightSegment(int segmentWaypointId, bool shouldRenderAsInvalidPlacement)
		{
			this.ToggleHighlightSegment(segmentWaypointId, true, shouldRenderAsInvalidPlacement);
		}

		// Token: 0x060061A9 RID: 25001 RVA: 0x002DE893 File Offset: 0x002DCA93
		public void DisableHighlightSegment(int segmentWaypointId)
		{
			this.ToggleHighlightSegment(segmentWaypointId, false, false);
		}

		// Token: 0x060061AA RID: 25002 RVA: 0x002DE89E File Offset: 0x002DCA9E
		private void ToggleHighlightSegment(int segmentWaypointId, bool shouldHighlight, bool shouldRenderAsInvalidPlacement)
		{
			this.RegisterSegmentHighlightState(segmentWaypointId, shouldHighlight, shouldRenderAsInvalidPlacement);
			this.UpdateSegmentHighlightState(segmentWaypointId, shouldHighlight, shouldRenderAsInvalidPlacement);
		}

		// Token: 0x060061AB RID: 25003 RVA: 0x002DE8B4 File Offset: 0x002DCAB4
		private void RegisterSegmentHighlightState(int segmentWaypointId, bool shouldHighlight, bool shouldRenderAsInvalidPlacement)
		{
			if (shouldHighlight)
			{
				if (!this._highlightedPathSegments.Contains(segmentWaypointId))
				{
					this._highlightedPathSegments.Add(segmentWaypointId);
				}
				if (shouldRenderAsInvalidPlacement && !this._invalidPlacementPathSegments.Contains(segmentWaypointId))
				{
					this._invalidPlacementPathSegments.Add(segmentWaypointId);
					return;
				}
			}
			else
			{
				this._highlightedPathSegments.Remove(segmentWaypointId);
				this._invalidPlacementPathSegments.Remove(segmentWaypointId);
			}
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x002DE918 File Offset: 0x002DCB18
		private void UpdateSegmentHighlightState(int segmentWaypointId, bool shouldHighlight, bool shouldRenderAsInvalidPlacement)
		{
			if (this._activePathSegmentMap.ContainsKey(segmentWaypointId))
			{
				foreach (WaypointPathRendering.PathSegment pathSegment in this._activePathSegmentMap[segmentWaypointId])
				{
					pathSegment.ToggleRenderPathHighlight(shouldHighlight, shouldRenderAsInvalidPlacement);
				}
			}
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x002DE980 File Offset: 0x002DCB80
		public void Destroy()
		{
			this.ClearActivePathsToRender();
			while (this._pathSegmentPool.Count != 0)
			{
				this._pathSegmentPool.Pop().Destroy();
			}
		}

		// Token: 0x04004494 RID: 17556
		private readonly string _name;

		// Token: 0x04004495 RID: 17557
		private readonly Stack<WaypointPathRendering.PathSegment> _pathSegmentPool;

		// Token: 0x04004496 RID: 17558
		private readonly Dictionary<int, List<WaypointPathRendering.PathSegment>> _activePathSegmentMap;

		// Token: 0x04004497 RID: 17559
		private readonly HashSet<int> _highlightedPathSegments;

		// Token: 0x04004498 RID: 17560
		private readonly HashSet<int> _invalidPlacementPathSegments;

		// Token: 0x04004499 RID: 17561
		private bool _isRenderingWithDefaultColor;

		// Token: 0x0400449A RID: 17562
		private bool _isPathRendering = true;

		// Token: 0x0200138B RID: 5003
		private class PathSegment
		{
			// Token: 0x06009180 RID: 37248 RVA: 0x003476F4 File Offset: 0x003458F4
			public PathSegment(string name)
			{
				WaypointPathRendering.PathSegment.s_lineCount++;
				string text = string.Format("{0}_{1}", name, WaypointPathRendering.PathSegment.s_lineCount);
				this._line = new VectorLine(text, new List<Vector3>(), 0.5f, LineType.Continuous);
				this._line.layer = LayerMask.NameToLayer("Space Combat UI");
				this._line.Draw3DAuto();
				GameControl.spaceCombat.container.Add(text, this._line.rectTransform.gameObject, false, false);
			}

			// Token: 0x06009181 RID: 37249 RVA: 0x003477A2 File Offset: 0x003459A2
			public void ToggleRenderState(bool shouldRender)
			{
				this._line.active = shouldRender;
			}

			// Token: 0x06009182 RID: 37250 RVA: 0x003477B0 File Offset: 0x003459B0
			public void ToggleRenderPathDefaultColor(bool shouldRenderDefault)
			{
				this._line.color = (shouldRenderDefault ? this._defaultColor : this._color);
			}

			// Token: 0x06009183 RID: 37251 RVA: 0x003477CE File Offset: 0x003459CE
			public void ToggleRenderPathHighlight(bool shouldRenderHighlight, bool shouldRenderAsInvalidPlacement)
			{
				this._line.SetWidth(shouldRenderHighlight ? 1f : 0.5f);
				this._line.color = (shouldRenderAsInvalidPlacement ? Color.red : this._color);
			}

			// Token: 0x06009184 RID: 37252 RVA: 0x00347805 File Offset: 0x00345A05
			public void RenderPath(List<Vector3> pathPoints, Color color)
			{
				this._color = color;
				this.ClearLine();
				this._line.points3.AddRange(pathPoints);
				this._line.color = this._color;
				this._line.smoothColor = false;
			}

			// Token: 0x06009185 RID: 37253 RVA: 0x00347844 File Offset: 0x00345A44
			public bool RenderPath(List<Vector3> pathPoints, Color color, Vector2 alphaBlend)
			{
				this._color = color;
				this.ClearLine();
				this._line.points3.AddRange(pathPoints);
				this._line.smoothColor = true;
				int num = this._line.points3.Count - 1;
				if (num > 0)
				{
					List<Color> list = new List<Color>();
					for (int i = 0; i < num; i++)
					{
						Color color2 = this._color;
						color2.a = Mathf.Lerp(alphaBlend.x, alphaBlend.y, 1f - (float)i / (float)num);
						list.Add(color2);
					}
					this._line.SetColors(list);
					return true;
				}
				this._line.SetColor(this._color);
				return false;
			}

			// Token: 0x06009186 RID: 37254 RVA: 0x003478F5 File Offset: 0x00345AF5
			public void ClearLine()
			{
				this._line.points3.Clear();
			}

			// Token: 0x06009187 RID: 37255 RVA: 0x00347907 File Offset: 0x00345B07
			public void Destroy()
			{
				VectorLine.Destroy(ref this._line);
			}

			// Token: 0x040071CC RID: 29132
			private static int s_lineCount;

			// Token: 0x040071CD RID: 29133
			private readonly Color _defaultColor = new Color(91f, 109f, 113f, 255f);

			// Token: 0x040071CE RID: 29134
			private VectorLine _line;

			// Token: 0x040071CF RID: 29135
			private Color _color;

			// Token: 0x040071D0 RID: 29136
			private const float baseWidth = 0.5f;

			// Token: 0x040071D1 RID: 29137
			private const float highlightWidth = 1f;
		}
	}
}
