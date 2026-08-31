using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F2 RID: 1778
	public class TIStandaloneInputModule : StandaloneInputModule
	{
		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x060029D7 RID: 10711 RVA: 0x000E2BCF File Offset: 0x000E0DCF
		public static TIStandaloneInputModule current
		{
			get
			{
				return EventSystem.current.currentInputModule as TIStandaloneInputModule;
			}
		}

		// Token: 0x060029D8 RID: 10712 RVA: 0x000E2BE0 File Offset: 0x000E0DE0
		protected override void Start()
		{
			base.Start();
			this.layerUI = LayerMask.NameToLayer("UI");
			this.layerSpaceCombatUI = LayerMask.NameToLayer("Space Combat UI");
		}

		// Token: 0x060029D9 RID: 10713 RVA: 0x000E2C08 File Offset: 0x000E0E08
		public PointerEventData GetPointerData()
		{
			if (this.m_PointerData.ContainsKey(-1))
			{
				return this.m_PointerData[-1];
			}
			return null;
		}

		// Token: 0x060029DA RID: 10714 RVA: 0x000E2C28 File Offset: 0x000E0E28
		public string GetPointerLayerName()
		{
			int pointerLayerID = this.GetPointerLayerID();
			if (pointerLayerID == -1)
			{
				return "";
			}
			return LayerMask.LayerToName(pointerLayerID);
		}

		// Token: 0x060029DB RID: 10715 RVA: 0x000E2C4C File Offset: 0x000E0E4C
		public int GetPointerLayerID()
		{
			PointerEventData pointerData = this.GetPointerData();
			if (pointerData == null)
			{
				return -1;
			}
			RaycastResult pointerCurrentRaycast = pointerData.pointerCurrentRaycast;
			if (pointerCurrentRaycast.isValid)
			{
				return pointerCurrentRaycast.gameObject.layer;
			}
			return -1;
		}

		// Token: 0x060029DC RID: 10716 RVA: 0x000E2C84 File Offset: 0x000E0E84
		public bool IsPointerOverUIGameObject()
		{
			int pointerLayerID = this.GetPointerLayerID();
			return pointerLayerID != -1 && pointerLayerID == this.layerUI;
		}

		// Token: 0x060029DD RID: 10717 RVA: 0x000E2CA8 File Offset: 0x000E0EA8
		public bool IsPointerOverSpaceCombatUIGameObject()
		{
			int num = 1 << this.layerSpaceCombatUI;
			PointerEventData pointerData = this.GetPointerData();
			if (Physics.Raycast(Camera.main.ScreenPointToRay(pointerData.position), 3.4028235E+38f, num))
			{
				return true;
			}
			int pointerLayerID = this.GetPointerLayerID();
			return pointerLayerID != -1 && pointerLayerID == this.layerSpaceCombatUI;
		}

		// Token: 0x060029DE RID: 10718 RVA: 0x000E2D04 File Offset: 0x000E0F04
		public bool IsPointerOverAltWaypointSelectionUI()
		{
			PointerEventData pointerData = this.GetPointerData();
			if (pointerData == null)
			{
				return false;
			}
			RaycastResult pointerCurrentRaycast = pointerData.pointerCurrentRaycast;
			return pointerCurrentRaycast.isValid && pointerCurrentRaycast.gameObject.name == "Select Waypoint Button";
		}

		// Token: 0x060029DF RID: 10719 RVA: 0x000E2D48 File Offset: 0x000E0F48
		public bool IsPointerOverSurfaceIcon()
		{
			PointerEventData pointerData = this.GetPointerData();
			if (pointerData == null)
			{
				return false;
			}
			RaycastResult pointerCurrentRaycast = pointerData.pointerCurrentRaycast;
			if (!pointerCurrentRaycast.isValid)
			{
				return false;
			}
			string name = pointerCurrentRaycast.gameObject.name;
			return name == "CentralIcon" || name.StartsWith("CP");
		}

		// Token: 0x0400203D RID: 8253
		private int layerUI;

		// Token: 0x0400203E RID: 8254
		private int layerSpaceCombatUI;
	}
}
