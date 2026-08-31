using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000803 RID: 2051
	public class WaypointUIController : MonoBehaviour
	{
		// Token: 0x06004A54 RID: 19028 RVA: 0x001F2A28 File Offset: 0x001F0C28
		private void Start()
		{
			this.mainCamera = GameControl.spaceCombat.mainCamera;
			base.name = "WaypointUICanvas" + base.transform.parent.GetSiblingIndex().ToString();
			base.transform.parent = base.transform.parent.parent;
			this.maxDisplayDistanceSqr = this.maxDisplayDistance * this.maxDisplayDistance;
			this.minDisplayDistanceSqr = this.minDisplayDistance * this.minDisplayDistance;
			this.startRadiusAdjustDistanceSqr = this.startRadiusAdjustDistance * this.startRadiusAdjustDistance;
			this.stopRadiusAdjustDistanceSqr = this.stopRadiusAdjustDistance * this.stopRadiusAdjustDistance;
			this.collisionText.SetText(Loc.T("UI.SpaceCombat.WaypointAvoidance"));
		}

		// Token: 0x06004A55 RID: 19029 RVA: 0x001F2AEC File Offset: 0x001F0CEC
		public void Initialize(WaypointVisual wayPointVisual, TISpaceShipState shipState)
		{
			this.waypointVisual = wayPointVisual;
			this.UIcanvas.enabled = true;
			this.dvText.gameObject.SetActive(false);
			this.collisionText.gameObject.SetActive(false);
			this.collisionWarningFlag = false;
			this.mainCamera = GameControl.spaceCombat.mainCamera;
			Loc.SwapFonts(base.gameObject);
			this._incrementRadians = 1.2566371f;
			this.altWaypointSelection.color = this.waypointVisual.BaseColor;
			this._altWaypointSelectionButton = this.altWaypointSelection.GetComponent<Button>();
			this.UpdateButtonSprites(this.waypointVisual.BaseColorIndex);
			GameControl.eventManager.AddListener<WaypointsCycled>(new EventManager.EventDelegate<WaypointsCycled>(this.OnWaypointCycled), null, shipState, false, false);
		}

		// Token: 0x06004A56 RID: 19030 RVA: 0x001F2BAD File Offset: 0x001F0DAD
		public void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<WaypointsCycled>(new EventManager.EventDelegate<WaypointsCycled>(this.OnWaypointCycled), null);
		}

		// Token: 0x06004A57 RID: 19031 RVA: 0x001F2BC6 File Offset: 0x001F0DC6
		public void ToggleVisibility(bool show)
		{
			if (show)
			{
				this.UIcanvas.enabled = true;
				return;
			}
			this.UIcanvas.enabled = false;
		}

		// Token: 0x06004A58 RID: 19032 RVA: 0x001F2BE4 File Offset: 0x001F0DE4
		public void ToggleDVText(bool show)
		{
			if (show)
			{
				this.dvText.gameObject.SetActive(true);
				this.showingDVText = true;
				return;
			}
			this.dvText.gameObject.SetActive(false);
			this.showingDVText = false;
		}

		// Token: 0x06004A59 RID: 19033 RVA: 0x001F2C1A File Offset: 0x001F0E1A
		public void ToggleCollisionWarning(bool show)
		{
			if (show)
			{
				this.collisionText.gameObject.SetActive(this.collisionWarningFlag);
				return;
			}
			if (!this.collisionWarningFlag)
			{
				this.collisionText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004A5A RID: 19034 RVA: 0x001F2C4F File Offset: 0x001F0E4F
		public void SetCollisionWarningFlag(bool on)
		{
			this.collisionWarningFlag = on;
		}

		// Token: 0x06004A5B RID: 19035 RVA: 0x001F2C58 File Offset: 0x001F0E58
		private void LateUpdate()
		{
			Vector3 vector = this.waypointVisual.transform.position - this.mainCamera.transform.position;
			bool flag = Vector3.Dot(this.mainCamera.transform.forward, vector.normalized) < 0f;
			if (this.dvText.gameObject.activeInHierarchy)
			{
				if (flag)
				{
					this.dvText.gameObject.SetActive(false);
				}
				else
				{
					this.position = RectTransformUtility.WorldToScreenPoint(this.mainCamera, this.waypointVisual.transform.position);
					this.dvText.transform.position = new Vector2(this.position.x, this.position.y + 30f);
				}
			}
			if (this.collisionText.gameObject.activeInHierarchy)
			{
				if (flag)
				{
					this.collisionText.gameObject.SetActive(false);
				}
				else
				{
					this.position = RectTransformUtility.WorldToScreenPoint(this.mainCamera, this.waypointVisual.transform.position);
					this.collisionText.transform.position = new Vector2(this.position.x, this.position.y + 50f);
				}
			}
			Vector3 shipPosition = this.waypointVisual.GetShipPosition();
			Vector3 vector2 = shipPosition - this.mainCamera.transform.position;
			float sqrMagnitude = vector2.sqrMagnitude;
			bool flag2 = Vector3.Dot(this.mainCamera.transform.forward, vector2.normalized) < 0f;
			if (this.waypointVisual.IsOverlapping && this.waypointVisual.IsVisible && this.waypointVisual.BaseColorIndex != 1 && sqrMagnitude < this.maxDisplayDistanceSqr && sqrMagnitude > this.minDisplayDistanceSqr)
			{
				if (!flag2)
				{
					if (!this.altWaypointSelection.gameObject.activeInHierarchy)
					{
						this.altWaypointSelection.gameObject.SetActive(true);
					}
					Vector2 vector3 = RectTransformUtility.WorldToScreenPoint(this.mainCamera, shipPosition);
					float num = 0f;
					if (sqrMagnitude < this.startRadiusAdjustDistanceSqr)
					{
						if (sqrMagnitude > this.stopRadiusAdjustDistanceSqr)
						{
							num = 1f - (sqrMagnitude - this.stopRadiusAdjustDistanceSqr) / (this.startRadiusAdjustDistanceSqr - this.stopRadiusAdjustDistanceSqr);
						}
						else
						{
							num = 1f;
						}
					}
					float num2 = Mathf.Clamp(this.maxRadius * num * SpaceCombatManager.GetScalingAdjustmentFactor(), this.baseRadius, this.maxRadius);
					float num3 = 6.2831855f - this._incrementRadians * (float)this.waypointVisual.BaseColorIndex;
					vector3 -= new Vector2(Mathf.Cos(num3) * num2, Mathf.Sin(num3) * num2);
					this.altWaypointSelection.transform.position = vector3;
					return;
				}
				if (this.altWaypointSelection.gameObject.activeInHierarchy)
				{
					this.altWaypointSelection.gameObject.SetActive(false);
					return;
				}
			}
			else if (this.altWaypointSelection.gameObject.activeInHierarchy)
			{
				this.altWaypointSelection.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004A5C RID: 19036 RVA: 0x001F2F82 File Offset: 0x001F1182
		private void OnWaypointCycled(WaypointsCycled e)
		{
			this.altWaypointSelection.color = this.waypointVisual.BaseColor;
			this.UpdateButtonSprites(this.waypointVisual.BaseColorIndex);
		}

		// Token: 0x06004A5D RID: 19037 RVA: 0x001F2FAC File Offset: 0x001F11AC
		private void UpdateButtonSprites(int baseColorIndex)
		{
			Sprite sprite;
			Sprite sprite2;
			switch (baseColorIndex)
			{
			case 2:
				sprite = this.normalIcon_1;
				sprite2 = this.hoverIcon_1;
				break;
			case 3:
				sprite = this.normalIcon_2;
				sprite2 = this.hoverIcon_2;
				break;
			case 4:
				sprite = this.normalIcon_3;
				sprite2 = this.hoverIcon_3;
				break;
			case 5:
				sprite = this.normalIcon_4;
				sprite2 = this.hoverIcon_4;
				break;
			case 6:
				sprite = this.normalIcon_5;
				sprite2 = this.hoverIcon_5;
				break;
			default:
				sprite = this.normalIcon_0;
				sprite2 = this.hoverIcon_0;
				break;
			}
			this._altWaypointSelectionButton.image.overrideSprite = sprite;
			SpriteState spriteState = new SpriteState
			{
				disabledSprite = sprite2,
				pressedSprite = sprite2,
				highlightedSprite = sprite2
			};
			this._altWaypointSelectionButton.spriteState = spriteState;
		}

		// Token: 0x04002B20 RID: 11040
		public Camera mainCamera;

		// Token: 0x04002B21 RID: 11041
		public TMP_Text dvText;

		// Token: 0x04002B22 RID: 11042
		public TMP_Text collisionText;

		// Token: 0x04002B23 RID: 11043
		public Canvas UIcanvas;

		// Token: 0x04002B24 RID: 11044
		public WaypointVisual waypointVisual;

		// Token: 0x04002B25 RID: 11045
		public Image altWaypointSelection;

		// Token: 0x04002B26 RID: 11046
		private Button _altWaypointSelectionButton;

		// Token: 0x04002B27 RID: 11047
		private bool altWaypointOnScreen;

		// Token: 0x04002B28 RID: 11048
		public float maxDisplayDistance = 12.5f;

		// Token: 0x04002B29 RID: 11049
		private float maxDisplayDistanceSqr;

		// Token: 0x04002B2A RID: 11050
		public float minDisplayDistance = 0.75f;

		// Token: 0x04002B2B RID: 11051
		private float minDisplayDistanceSqr;

		// Token: 0x04002B2C RID: 11052
		public float baseRadius = 20f;

		// Token: 0x04002B2D RID: 11053
		public float maxRadius = 50f;

		// Token: 0x04002B2E RID: 11054
		public float startRadiusAdjustDistance = 10f;

		// Token: 0x04002B2F RID: 11055
		private float startRadiusAdjustDistanceSqr;

		// Token: 0x04002B30 RID: 11056
		public float stopRadiusAdjustDistance = 1.5f;

		// Token: 0x04002B31 RID: 11057
		private float stopRadiusAdjustDistanceSqr;

		// Token: 0x04002B32 RID: 11058
		private float _incrementRadians;

		// Token: 0x04002B33 RID: 11059
		[Header("Waypoint Icons")]
		public Sprite normalIcon_0;

		// Token: 0x04002B34 RID: 11060
		public Sprite hoverIcon_0;

		// Token: 0x04002B35 RID: 11061
		public Sprite normalIcon_1;

		// Token: 0x04002B36 RID: 11062
		public Sprite hoverIcon_1;

		// Token: 0x04002B37 RID: 11063
		public Sprite normalIcon_2;

		// Token: 0x04002B38 RID: 11064
		public Sprite hoverIcon_2;

		// Token: 0x04002B39 RID: 11065
		public Sprite normalIcon_3;

		// Token: 0x04002B3A RID: 11066
		public Sprite hoverIcon_3;

		// Token: 0x04002B3B RID: 11067
		public Sprite normalIcon_4;

		// Token: 0x04002B3C RID: 11068
		public Sprite hoverIcon_4;

		// Token: 0x04002B3D RID: 11069
		public Sprite normalIcon_5;

		// Token: 0x04002B3E RID: 11070
		public Sprite hoverIcon_5;

		// Token: 0x04002B3F RID: 11071
		private bool collisionWarningFlag;

		// Token: 0x04002B40 RID: 11072
		private Vector2 position;

		// Token: 0x04002B41 RID: 11073
		public bool showingDVText;
	}
}
