using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000555 RID: 1365
	public class MarkerContainerController : MonoBehaviour
	{
		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x0600239F RID: 9119 RVA: 0x000BC6D8 File Offset: 0x000BA8D8
		public float canvasWidth
		{
			get
			{
				return this.rectTransform.rect.width;
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x060023A0 RID: 9120 RVA: 0x000BC6F8 File Offset: 0x000BA8F8
		public float canvasHeight
		{
			get
			{
				return this.rectTransform.rect.height;
			}
		}

		// Token: 0x060023A1 RID: 9121 RVA: 0x000BC718 File Offset: 0x000BA918
		private void Awake()
		{
			Error.IsNull<List<MarkerController>>(this.markers);
			this.cameraMgr = World.Active.GetExistingManager<CameraManager>();
			this.mainCamera = GameControl.control.mainCamera;
			this.thisCanvas = base.GetComponent<Canvas>();
			this.thisCanvas.worldCamera = this.mainCamera;
			this.adjustedScaleMultiplier = 100f / this.canvasWidth;
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x000BC780 File Offset: 0x000BA980
		private void Update()
		{
			if (this.cameraMgr.ForceVisualizationUpdate)
			{
				this.forceUpdate = true;
			}
			if (this.thisCanvas != null && (this.forceUpdate || this.cameraMgr.IsAltitudeChanging))
			{
				float newScale = this.GetNewScale();
				if (newScale != this.prevScale || this.forceUpdate)
				{
					foreach (MarkerController markerController in this.markers)
					{
						this.ScaleMarker(newScale, markerController);
					}
					this.AutoArrangeMarkers();
					this.prevScale = newScale;
					this.forceUpdate = false;
				}
			}
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x000BC838 File Offset: 0x000BAA38
		public void Refresh()
		{
			this.forceUpdate = true;
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x000BC844 File Offset: 0x000BAA44
		public void InitializeWithRegionInfo(RegionController owner, TIMapGroupVisualizerTemplate template)
		{
			this.region = owner;
			this.template = template;
			if (this.region != null && this.template != null)
			{
				this.map = this.region.mapVisualizer.transform;
			}
			this.thisCanvas.sortingOrder = template.sortingValue;
			base.gameObject.SetActive(false);
		}

		// Token: 0x060023A5 RID: 9125 RVA: 0x000BC8A8 File Offset: 0x000BAAA8
		protected MarkerController GetMarkerByPriority(int priority)
		{
			int num = 0;
			foreach (MarkerController markerController in this.markers)
			{
				if (markerController.gameObject != null && markerController.gameObject.activeSelf)
				{
					if (num == priority)
					{
						return markerController;
					}
					num++;
				}
			}
			return null;
		}

		// Token: 0x060023A6 RID: 9126 RVA: 0x000BC920 File Offset: 0x000BAB20
		public MarkerController ManageMarkerStack(MarkerController marker, bool delete, MarkerType mType, TIGameState location, string markerName = "", int forceIndex = -1, bool forceEmptyCreation = false)
		{
			if (delete && !forceEmptyCreation)
			{
				if (marker != null)
				{
					global::UnityEngine.Object.Destroy(marker.gameObject);
					this.RemoveMarker(marker);
					this.Refresh();
				}
				return null;
			}
			if (marker == null)
			{
				marker = this.AddMarker(forceIndex, markerName);
				marker.Initialize(mType, location);
				this.Refresh();
			}
			return marker;
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x000BC97C File Offset: 0x000BAB7C
		protected MarkerController AddMarker(int forceIndex = -1, string markerName = "")
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.markerPrefab, base.transform, false);
			MarkerController component = gameObject.GetComponent<MarkerController>();
			int numMarkers = this.GetNumMarkers();
			if (forceIndex >= 0)
			{
				if (forceIndex > numMarkers)
				{
					forceIndex = numMarkers;
				}
				this.markers.Insert(forceIndex, component);
				gameObject.transform.SetSiblingIndex(forceIndex);
			}
			else
			{
				this.markers.Add(component);
			}
			base.gameObject.SetActive(true);
			if (markerName != string.Empty)
			{
				component.name = markerName;
			}
			return component;
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x000BCA00 File Offset: 0x000BAC00
		public void ScaleMarker(float newScale, MarkerController marker)
		{
			if (marker == null || newScale <= 0f)
			{
				return;
			}
			newScale *= marker.relativeScaling;
			marker.rectTransform.localScale = Vector3.one * newScale;
			if (marker.particleEffectsContainer != null)
			{
				marker.particleEffectsContainer.transform.localScale = Vector3.one / newScale;
			}
			if ((marker.highPriority && this.region.mapVisualizer.mapTransform.localScale.magnitude < this.template.highPriorityScaleAppearanceThreshold) || (!marker.highPriority && this.region.mapVisualizer.mapTransform.localScale.magnitude < this.template.lowPriorityScaleAppearanceThreshold))
			{
				marker.SetActive(false);
				if (marker.animating)
				{
					marker.centralIconAnimator.gameObject.SetActive(false);
					return;
				}
			}
			else
			{
				if (marker.hasModel)
				{
					if (newScale <= 1.3f && !marker.hasBeenScaled)
					{
						if (!marker.modelActive)
						{
							marker.TurnOn3dElements();
						}
						switch (marker.markerType)
						{
						case MarkerType.Army:
							newScale += 1f;
							break;
						case MarkerType.HumanLaserFacility:
						case MarkerType.HumanLaunchFacility:
							newScale += 0.2f;
							break;
						}
						marker.model.transform.localScale *= newScale * 25f;
						marker.hasBeenScaled = true;
					}
					else if (newScale > 1.3f && marker.modelActive)
					{
						marker.TurnOff3dElements();
					}
				}
				marker.SetActive(true);
				if (marker.animating && !marker.modelActive)
				{
					marker.centralIconAnimator.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x000BCBB7 File Offset: 0x000BADB7
		public void RemoveMarker(MarkerController marker)
		{
			this.markers.Remove(marker);
			if (this.markers.Count == 0)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x000BCBDF File Offset: 0x000BADDF
		public int GetNumMarkers()
		{
			return this.GetMarkers().Count;
		}

		// Token: 0x060023AB RID: 9131 RVA: 0x000BCBEC File Offset: 0x000BADEC
		public List<MarkerController> GetMarkers()
		{
			return this.markers.Where<MarkerController>((MarkerController x) => x.gameObject.activeSelf).ToList<MarkerController>();
		}

		// Token: 0x060023AC RID: 9132 RVA: 0x000BCC20 File Offset: 0x000BAE20
		protected void AutoArrangeMarkers()
		{
			int numMarkers = this.GetNumMarkers();
			if (numMarkers > 0)
			{
				MarkerType markerType = this.markers[0].markerType;
				if (markerType - MarkerType.Councilor > 1)
				{
					if (markerType != MarkerType.Army)
					{
						switch (markerType)
						{
						case MarkerType.RegionalStatusIcon:
						case MarkerType.OccupationMarker:
						case MarkerType.Capital:
							if (this.region.region.mapRegionTemplate.verticalRegion)
							{
								this.ArrangeInColumn_FromTop(numMarkers, 1f);
								return;
							}
							if (this.region.region.mapRegionTemplate.smallRegion)
							{
								this.ArrangeInLine(numMarkers, false);
								return;
							}
							this.ArrangeInLine_FixedPositions(3);
							return;
						case MarkerType.NavalTransport:
							break;
						case MarkerType.Org:
							this.ArrangeMultipleTypes(true);
							return;
						default:
							this.ArrangeInLine(numMarkers, false);
							return;
						}
					}
					this.ArrangeInLine(numMarkers, false);
					return;
				}
				if (this.markers.Any<MarkerController>((MarkerController x) => x.markerType == MarkerType.Org))
				{
					this.ArrangeMultipleTypes(true);
					return;
				}
				this.ArrangeInLine(numMarkers, true);
				return;
			}
		}

		// Token: 0x060023AD RID: 9133 RVA: 0x000BCD1C File Offset: 0x000BAF1C
		protected void ArrangeInColumn(int numMarkers, bool exceedCanvas = false)
		{
			MarkerController markerByPriority = this.GetMarkerByPriority(0);
			if (numMarkers % 2 == 0)
			{
				float num = markerByPriority.scaledHeight / 2f;
				float scaledHeight = markerByPriority.scaledHeight;
				for (int i = 0; i < numMarkers; i++)
				{
					MarkerController markerByPriority2 = this.GetMarkerByPriority(i);
					float num2 = num + scaledHeight * (float)(i / 2);
					if (i % 2 == 0)
					{
						num2 *= -1f;
					}
					if (Mathf.Abs(num2) > this.canvasHeight && !exceedCanvas)
					{
						markerByPriority2.MoveMarker(Vector3.zero, 0.1f);
					}
					else
					{
						Vector2 vector = new Vector2(0f, num2);
						markerByPriority2.MoveMarker(vector, 0.1f);
					}
				}
				return;
			}
			markerByPriority.MoveMarker(Vector3.zero, 0.1f);
			float scaledHeight2 = markerByPriority.scaledHeight;
			for (int j = 1; j < numMarkers; j++)
			{
				MarkerController markerByPriority3 = this.GetMarkerByPriority(j);
				float num3 = scaledHeight2 * (float)((j + 1) / 2);
				if (j % 2 == 1)
				{
					num3 *= -1f;
				}
				if (Mathf.Abs(num3) > this.canvasHeight && !exceedCanvas)
				{
					markerByPriority3.MoveMarker(Vector3.zero, 0.1f);
				}
				else
				{
					Vector2 vector2 = new Vector2(0f, num3);
					markerByPriority3.MoveMarker(vector2, 0.1f);
				}
			}
		}

		// Token: 0x060023AE RID: 9134 RVA: 0x000BCE5C File Offset: 0x000BB05C
		protected void ArrangeInColumn_FromTop(int numMarkers, float offsetMultiplier = 1f)
		{
			MarkerController markerByPriority = this.GetMarkerByPriority(0);
			float num = markerByPriority.scaledHeight * offsetMultiplier;
			float num2 = markerByPriority.scaledHeight / 2f - num;
			for (int i = 0; i < numMarkers; i++)
			{
				MarkerController markerByPriority2 = this.GetMarkerByPriority(i);
				float num3 = num2 + num * (float)i;
				markerByPriority2.MoveMarker(new Vector2(0f, num3), 0f);
			}
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x000BCEB8 File Offset: 0x000BB0B8
		protected void ArrangeMultipleTypes(bool exceedCanvas = true)
		{
			int num = this.markers.Count<MarkerController>((MarkerController x) => x.markerType == MarkerType.Councilor || x.markerType == MarkerType.AlienCouncilor);
			this.ArrangeInLine(num, exceedCanvas);
			int num2 = this.markers.Where<MarkerController>((MarkerController x) => x.markerType == MarkerType.Org).Count<MarkerController>();
			MarkerController markerByPriority = this.GetMarkerByPriority(0);
			float scaledWidth = this.markers[num].scaledWidth;
			float num3 = scaledWidth / 2f;
			int num4 = num2 / 2;
			float num5 = 0f;
			float num6 = ((num > 0) ? (-markerByPriority.scaledHeight) : 0f);
			float num7 = 0f;
			float num8 = 0f;
			for (int i = num; i < num + num2; i++)
			{
				int num9 = i - num;
				switch (num9)
				{
				case 0:
					break;
				case 1:
					num7 = 1f;
					num8 = 0f;
					break;
				case 2:
					num7 = 0f;
					num8 = 1f;
					break;
				case 3:
					num7 = 1f;
					num8 = 1f;
					break;
				default:
				{
					int num10 = num9 / 2;
					if (num10 % 2 == 1)
					{
						num7 = -1f * ((float)num10 - 3f * ((float)num10 / 2f) - 0.5f);
					}
					else
					{
						num7 = (float)num10 - 3f * ((float)num10 / 2f);
					}
					num8 = (float)(num9 % 2);
					break;
				}
				}
				num7 = num5 + num7 * scaledWidth;
				num8 = num6 + num8 * scaledWidth;
				Vector2 vector = new Vector2(num7, num8);
				this.GetMarkerByPriority(i).MoveMarker(vector, 0.1f);
			}
		}

		// Token: 0x060023B0 RID: 9136 RVA: 0x000BD05C File Offset: 0x000BB25C
		protected void ArrangeInLine(int numMarkers, bool exceedCanvas = false)
		{
			MarkerController markerByPriority = this.GetMarkerByPriority(0);
			if (numMarkers % 2 == 0)
			{
				float num = markerByPriority.scaledWidth / 2f;
				float scaledWidth = markerByPriority.scaledWidth;
				for (int i = 0; i < numMarkers; i++)
				{
					MarkerController markerByPriority2 = this.GetMarkerByPriority(i);
					float num2 = num + scaledWidth * (float)(i / 2);
					if (i % 2 == 0)
					{
						num2 = -num2;
					}
					if (!exceedCanvas && Mathf.Abs(num2) > this.canvasWidth)
					{
						markerByPriority2.MoveMarker(Vector3.zero, 0.1f);
					}
					else
					{
						Vector2 vector = new Vector2(num2, 0f);
						markerByPriority2.MoveMarker(vector, 0.1f);
					}
				}
				return;
			}
			markerByPriority.MoveMarker(Vector3.zero, 0.1f);
			float scaledWidth2 = markerByPriority.scaledWidth;
			for (int j = 1; j < numMarkers; j++)
			{
				MarkerController markerByPriority3 = this.GetMarkerByPriority(j);
				float num3 = scaledWidth2 * (float)((j + 1) / 2);
				if (j % 2 == 1)
				{
					num3 = -num3;
				}
				if (!exceedCanvas && Mathf.Abs(num3) > this.canvasWidth)
				{
					markerByPriority3.MoveMarker(Vector3.zero, 0.1f);
				}
				else
				{
					Vector2 vector2 = new Vector2(num3, 0f);
					markerByPriority3.MoveMarker(vector2, 0.1f);
				}
			}
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x000BD194 File Offset: 0x000BB394
		protected void ArrangeInLine_FixedPositions(int totalPossible)
		{
			float scaledWidth = this.GetMarkerByPriority(0).scaledWidth;
			float num = scaledWidth / 2f - scaledWidth;
			for (int i = 0; i < totalPossible; i++)
			{
				MarkerController markerByPriority = this.GetMarkerByPriority(i);
				if (markerByPriority != null)
				{
					float num2 = num + scaledWidth * (float)i;
					markerByPriority.MoveMarker(new Vector2(num2, 0f), 0f);
				}
			}
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x000BD1F4 File Offset: 0x000BB3F4
		protected void ArrangeInLine_FromLeft(int numMarkers, float offsetMultiplier = 1f)
		{
			MarkerController markerByPriority = this.GetMarkerByPriority(0);
			float num = markerByPriority.scaledWidth * offsetMultiplier;
			float num2 = markerByPriority.scaledWidth / 2f - num;
			for (int i = 0; i < numMarkers; i++)
			{
				MarkerController markerByPriority2 = this.GetMarkerByPriority(i);
				float num3 = num2 + num * (float)i;
				markerByPriority2.MoveMarker(new Vector2(num3, 0f), 0f);
			}
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x000BD24E File Offset: 0x000BB44E
		public float GetNewScale()
		{
			return Mathf.Clamp(this.adjustedScaleMultiplier * this.DistanceForCamera(), 1f, 3f);
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x000BD26C File Offset: 0x000BB46C
		public float DistanceForCamera()
		{
			float magnitude = (base.transform.position - this.map.position).magnitude;
			return (this.mainCamera.transform.position - this.map.position).magnitude - magnitude;
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x000BD2C7 File Offset: 0x000BB4C7
		public void InitializeGeoscapeModel(MarkerController marker, string assetPath)
		{
			if (marker != null)
			{
				marker.cachedModel = GameControl.assetLoader.LoadAsset<GameObject>(assetPath);
				marker.SetMarkerModel(null);
			}
		}

		// Token: 0x04001AC5 RID: 6853
		public GameObject markerPrefab;

		// Token: 0x04001AC6 RID: 6854
		public Canvas thisCanvas;

		// Token: 0x04001AC7 RID: 6855
		public RectTransform rectTransform;

		// Token: 0x04001AC8 RID: 6856
		private RegionController region;

		// Token: 0x04001AC9 RID: 6857
		private TIMapGroupVisualizerTemplate template;

		// Token: 0x04001ACA RID: 6858
		private Camera mainCamera;

		// Token: 0x04001ACB RID: 6859
		[SerializeField]
		private Transform map;

		// Token: 0x04001ACC RID: 6860
		[SerializeField]
		private List<MarkerController> markers;

		// Token: 0x04001ACD RID: 6861
		private const float minScale = 1f;

		// Token: 0x04001ACE RID: 6862
		private const float maxScale = 3f;

		// Token: 0x04001ACF RID: 6863
		private const float iconTimeToSettle = 0.1f;

		// Token: 0x04001AD0 RID: 6864
		private const float scaleMultiplier = 100f;

		// Token: 0x04001AD1 RID: 6865
		public const float modelScaleFactor = 25f;

		// Token: 0x04001AD2 RID: 6866
		public const float armyModelScaleAdjust = 1f;

		// Token: 0x04001AD3 RID: 6867
		public const float launchFacilityScaleAdjust = 0.2f;

		// Token: 0x04001AD4 RID: 6868
		public const float cameraModelLoadThreshold = 1.3f;

		// Token: 0x04001AD5 RID: 6869
		private CameraManager cameraMgr;

		// Token: 0x04001AD6 RID: 6870
		private float prevScale;

		// Token: 0x04001AD7 RID: 6871
		private bool forceUpdate;

		// Token: 0x04001AD8 RID: 6872
		public float adjustedScaleMultiplier;
	}
}
