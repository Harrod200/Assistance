using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200054F RID: 1359
	public class ArmyPathController : MonoBehaviour
	{
		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06002313 RID: 8979 RVA: 0x000B7265 File Offset: 0x000B5465
		private IEnumerable<BezierLineController> Segments
		{
			get
			{
				return base.GetComponentsInChildren<BezierLineController>();
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06002314 RID: 8980 RVA: 0x000B7270 File Offset: 0x000B5470
		private bool ShouldHide
		{
			get
			{
				if (CameraManager.Singleton.LOD != CameraManagerLOD.Surface)
				{
					return true;
				}
				TIArmyState army = this.MarkerController.Army;
				if (army == null)
				{
					return true;
				}
				return !army.currentOperations.Any<OperationData>((OperationData x) => x.operation is DeployArmyOperation) && !this.ShouldDisplayProspectivePath;
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06002315 RID: 8981 RVA: 0x000B72DC File Offset: 0x000B54DC
		private bool ShouldDisplayProspectivePath
		{
			get
			{
				return (ArmyDetailController.Singleton.myArmy == this.MarkerController.Army || OperationCanvasController.Singleton.GetSelectedArmies().Contains(this.MarkerController.Army)) && OperationCanvasController.Singleton.IsInTargetSelectionMode && OperationCanvasController.Singleton.QueuedTargets.Count > 0 && (OperationCanvasController.Singleton.SelectedOperation is DeployArmyOperation || OperationCanvasController.Singleton.SelectedOperation is DeployArmiesOperation);
			}
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x000B7368 File Offset: 0x000B5568
		private void Update()
		{
			if (this.MarkerController == null)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			bool flag = TIFrameCounter.FrameCount % 30 == 0;
			this.UpdateVisualization(flag);
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x000B73A2 File Offset: 0x000B55A2
		private void OnEnable()
		{
			this.ResetVisualization();
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x000B73AC File Offset: 0x000B55AC
		private void ResetVisualization()
		{
			foreach (BezierLineController bezierLineController in this.Segments.ToList<BezierLineController>())
			{
				global::UnityEngine.Object.Destroy(bezierLineController.gameObject);
			}
			this.lastPath = null;
		}

		// Token: 0x06002319 RID: 8985 RVA: 0x000B7410 File Offset: 0x000B5610
		public void UpdateVisualization(bool updatePath)
		{
			if (this.ShouldHide)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Vector3 earthPosition = GameStateManager.Earth().controller.transform.position;
			float pathRadius = GameStateManager.Earth().controller.radius_gameUnits * 1.01f;
			TIArmyState army = this.MarkerController.Army;
			army.currentOperations.Where<OperationData>((OperationData x) => x.operation is DeployArmyOperation).FirstOrDefault<OperationData>();
			List<TIRegionState> list = this.lastPath;
			if (list == null || updatePath)
			{
				if (this.ShouldDisplayProspectivePath)
				{
					if (OperationCanvasController.Singleton.GetSelectedArmies().Count > 0 && OperationCanvasController.Singleton.prospectiveQueuedTargetsDictionary.ContainsKey(army))
					{
						list = OperationCanvasController.Singleton.prospectiveQueuedTargetsDictionary[army].ToList<TIRegionState>();
					}
					else
					{
						list = OperationCanvasController.Singleton.QueuedTargets.Select<TIGameState, TIRegionState>((TIGameState x) => x.ref_region).ToList<TIRegionState>();
					}
				}
				else
				{
					list = army.destinationQueue.ToList<TIRegionState>();
				}
				OperationData operationData = army.currentOperations.FirstOrDefault<OperationData>((OperationData x) => x.operation is DeployArmyOperation);
				if (!this.ShouldDisplayProspectivePath && operationData != null && operationData.target.ref_region != list.FirstOrDefault<TIRegionState>())
				{
					list.Insert(0, operationData.target.ref_region);
				}
				list.Insert(0, army.currentRegion);
			}
			this.lastPath = list;
			Func<TIRegionState, Vector3> func = delegate(TIRegionState region)
			{
				ArmyMarkerController armyMarkerController = (from x in region.Controller.GetIMarkerControllers()
					select x as ArmyMarkerController).FirstOrDefault<ArmyMarkerController>((ArmyMarkerController x) => x != null);
				return earthPosition + (armyMarkerController.transform.position - earthPosition).normalized * pathRadius;
			};
			List<int> list2 = new List<int>();
			Vector3 vector = Vector3.zero;
			List<Vector3> targetNodes = new List<Vector3>();
			Action<Vector3, Vector3, int, float> action = delegate(Vector3 originNode, Vector3 destinationNode, int minimunSubdivisionCount, float curvinessFactor)
			{
				float num4 = Vector3.Angle(originNode - earthPosition, destinationNode - earthPosition);
				int num5 = Mathf.Max((int)(num4 / 10f), minimunSubdivisionCount);
				if (num4 > 10f)
				{
					curvinessFactor /= Mathf.Pow(num4 / 12f, 1.4f);
				}
				for (int m = 0; m < num5; m++)
				{
					Vector3 vector10 = Vector3.Lerp(originNode, destinationNode, ((float)m + 1f) / (float)(num5 + 1));
					vector10 += curvinessFactor * originNode.Crossed(destinationNode).normalized * originNode.Distance(destinationNode) / 5f;
					Vector3 normalized4 = (vector10 - earthPosition).normalized;
					vector10 = earthPosition + normalized4 * pathRadius;
					targetNodes.Add(vector10);
				}
			};
			for (int i = 0; i < list.Count; i++)
			{
				TIRegionState region = list[i];
				bool flag = false;
				if (i > 0)
				{
					flag = TIArmyState.GetRequiredDeploymentType(list[i - 1], region, army) == DeploymentType.Naval;
				}
				bool flag2 = false;
				if (i < list.Count - 1)
				{
					TIRegionState tiregionState = list[i + 1];
					flag2 = TIArmyState.GetRequiredDeploymentType(region, tiregionState, army) == DeploymentType.Naval;
				}
				Vector3 regionNode = func(region);
				Func<Vector3> func2 = delegate
				{
					Vector3 vector11;
					region.Controller.GetSeaLocation(out vector11);
					vector11 = region.Controller.transform.TransformPoint(vector11);
					float num6 = pathRadius / 16f;
					if (vector11.Distance(regionNode) < num6)
					{
						vector11 = regionNode + (vector11 - regionNode).normalized * num6;
					}
					return earthPosition + (vector11 - earthPosition).normalized * pathRadius;
				};
				if (flag)
				{
					Vector3 vector2 = func2();
					targetNodes.Add(vector2);
					action(vector2, regionNode, 0, 1f);
				}
				else if (!flag2 && i > 0)
				{
					bool flag3 = list.Count == 2 && !flag;
					float num = 1f;
					if (i < list.Count - 1)
					{
						Vector3 vector3 = regionNode - func(list[i - 1]);
						Vector3 vector4 = func(list[i + 1]) - regionNode;
						num = Mathf.Pow(1f - Vector3.Angle(vector3, vector4) / 180f, 2f);
					}
					action(vector, regionNode, flag3 ? 1 : 0, num);
				}
				targetNodes.Add(regionNode);
				vector = regionNode;
				if (flag2)
				{
					Vector3 vector5 = func2();
					action(regionNode, vector5, 0, 1f);
					list2.Add(targetNodes.Count);
					targetNodes.Add(vector5);
				}
			}
			Func<Vector3, Vector3> LocalToGlobal = (Vector3 point) => this.transform.TransformPoint(point);
			Func<Vector3, Vector3> GlobalToLocal = (Vector3 point) => this.transform.InverseTransformPoint(point);
			int num2 = targetNodes.Count - 1;
			List<BezierLineController> segments = this.Segments.ToList<BezierLineController>();
			if (segments.Count > num2)
			{
				List<BezierLineController> list3 = segments.Take<BezierLineController>(segments.Count - num2).ToList<BezierLineController>();
				if (!list3.All<BezierLineController>((BezierLineController x) => Vector3.Distance((Vector3)x.BezierCurve.A, (Vector3)x.BezierCurve.B) < 0.01f))
				{
					goto IL_0502;
				}
				using (List<BezierLineController>.Enumerator enumerator = list3.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BezierLineController bezierLineController = enumerator.Current;
						global::UnityEngine.Object.Destroy(bezierLineController.gameObject);
						segments.Remove(bezierLineController);
					}
					goto IL_0502;
				}
			}
			if (segments.Count < num2)
			{
				segments.AddRange((num2 - segments.Count).Range().Select<int, BezierLineController>(delegate(int x)
				{
					Vector3 vector12 = GlobalToLocal(targetNodes.First<Vector3>());
					if (segments.Count > 0)
					{
						vector12 = (Vector3)segments.Last<BezierLineController>().BezierCurve.B;
					}
					BezierLineController bezierLineController5 = global::UnityEngine.Object.Instantiate<BezierLineController>(this.SegmentPrefab, this.transform, false);
					bezierLineController5.BezierCurve.A = (bezierLineController5.BezierCurve.B = vector12);
					return bezierLineController5;
				}));
			}
			IL_0502:
			if (segments.Count == 0)
			{
				return;
			}
			foreach (BezierLineController bezierLineController2 in segments)
			{
				Material material = bezierLineController2.LineRenderer.material;
				TIFactionState faction = army.faction;
				material.color = ((faction != null) ? faction.template.color : Color.white);
				bezierLineController2.LineRenderer.enabled = !list2.Contains(segments.IndexOf(bezierLineController2));
			}
			List<Vector3> list4 = segments.Select<BezierLineController, Vector3>((BezierLineController x) => LocalToGlobal((Vector3)x.BezierCurve.A)).ToList<Vector3>();
			list4.Add(LocalToGlobal((Vector3)segments.Last<BezierLineController>().BezierCurve.B));
			for (int j = 0; j < list4.Count; j++)
			{
				int num3 = Mathf.Max(0, j + targetNodes.Count - list4.Count);
				Vector3 normalized = (Vector3.Lerp(list4[j], targetNodes[num3], 4f * Time.deltaTime) - earthPosition).normalized;
				list4[j] = earthPosition + normalized * pathRadius;
			}
			for (int k = 0; k < segments.Count; k++)
			{
				segments[k].BezierCurve.A = GlobalToLocal(list4[k]);
				segments[k].BezierCurve.B = GlobalToLocal(list4[k + 1]);
			}
			for (int l = 1; l < list4.Count - 1; l++)
			{
				Vector3 vector6 = list4[l - 1];
				Vector3 vector7 = list4[l];
				Vector3 vector8 = list4[l + 1];
				Vector3 normalized2 = (vector7 - earthPosition).normalized;
				(vector6 + vector8) / 2f;
				Vector3 vector9 = (vector7 - vector6).normalized + (vector7 - vector8).normalized;
				Vector3 normalized3 = vector9.Crossed(vector7 - vector6).Crossed(vector9).normalized;
				BezierLineController bezierLineController3 = segments[l - 1];
				BezierLineController bezierLineController4 = segments[l];
				if (list2.Contains(l))
				{
					bezierLineController3.BezierCurve.ControlPointB = GlobalToLocal(vector7 + (vector6 - vector7 + vector6.Crossed(vector7).normalized * 0.4f).normalized * vector6.Distance(vector7) * 0.4f);
				}
				else
				{
					bezierLineController3.BezierCurve.ControlPointB = GlobalToLocal(vector7 - normalized3 * vector6.Distance(vector7) * 0.4f);
				}
				if (list2.Contains(l - 1))
				{
					bezierLineController4.BezierCurve.ControlPointA = GlobalToLocal(vector7 + (vector8 - vector7 + vector8.Crossed(vector7).normalized * 0.4f).normalized * vector7.Distance(vector8) * 0.4f);
				}
				else
				{
					bezierLineController4.BezierCurve.ControlPointA = GlobalToLocal(vector7 + normalized3 * vector7.Distance(vector8) * 0.4f);
				}
				if (l == 1)
				{
					bezierLineController3.BezierCurve.ControlPointA = GlobalToLocal(vector6 + (LocalToGlobal((Vector3)bezierLineController3.BezierCurve.ControlPointB) - vector6).normalized * vector6.Distance(vector7) * 0.4f);
				}
				if (l == list4.Count - 2)
				{
					bezierLineController4.BezierCurve.ControlPointB = GlobalToLocal(vector8 + (LocalToGlobal((Vector3)bezierLineController4.BezierCurve.ControlPointA) - vector8).normalized * vector8.Distance(vector7) * 0.4f);
				}
			}
		}

		// Token: 0x04001A8B RID: 6795
		private List<TIRegionState> lastPath = new List<TIRegionState>();

		// Token: 0x04001A8C RID: 6796
		public MarkerController MarkerController;

		// Token: 0x04001A8D RID: 6797
		public BezierLineController SegmentPrefab;

		// Token: 0x04001A8E RID: 6798
		public BezierLineController SegmentPrefab_Water;
	}
}
