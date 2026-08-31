using System;
using UnityEngine;

namespace LapinerTools.uMyGUI
{
	// Token: 0x0200052C RID: 1324
	public class uMyGUI_ScrollbarHandleUnityFix : MonoBehaviour
	{
		// Token: 0x060020E9 RID: 8425 RVA: 0x000AA550 File Offset: 0x000A8750
		public void Awake()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.anchoredPosition3D = Vector3.zero;
			component.anchorMin = this.m_anchorMin;
			component.anchorMax = this.m_anchorMax;
			component.pivot = this.m_pivot;
			component.offsetMin = this.m_offsetMin;
			component.offsetMax = this.m_offsetMax;
		}

		// Token: 0x04001966 RID: 6502
		[SerializeField]
		private Vector2 m_anchorMin = new Vector2(0.8f, 0f);

		// Token: 0x04001967 RID: 6503
		[SerializeField]
		private Vector2 m_anchorMax = new Vector2(1f, 1f);

		// Token: 0x04001968 RID: 6504
		[SerializeField]
		private Vector2 m_pivot = new Vector2(0.5f, 0.5f);

		// Token: 0x04001969 RID: 6505
		[SerializeField]
		private Vector2 m_offsetMin = new Vector2(-10f, -10f);

		// Token: 0x0400196A RID: 6506
		[SerializeField]
		private Vector2 m_offsetMax = new Vector2(10f, 10f);
	}
}
