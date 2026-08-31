using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x0200052D RID: 1325
	public class uMyGUI_SliderSynchronizer : MonoBehaviour
	{
		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x060020EB RID: 8427 RVA: 0x000AA630 File Offset: 0x000A8830
		// (set) Token: 0x060020EC RID: 8428 RVA: 0x000AA638 File Offset: 0x000A8838
		public Slider[] Sliders
		{
			get
			{
				return this.m_sliders;
			}
			set
			{
				this.m_sliders = value;
			}
		}

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x060020ED RID: 8429 RVA: 0x000AA641 File Offset: 0x000A8841
		public bool IsSynchronizeOnStart
		{
			get
			{
				return this.m_isSynchronizeOnStart;
			}
		}

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x060020EE RID: 8430 RVA: 0x000AA649 File Offset: 0x000A8849
		public float Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x000AA654 File Offset: 0x000A8854
		private void Start()
		{
			if (this.m_sliders.Length != 0)
			{
				this.m_value = this.m_sliders[0].value;
			}
			for (int i = 0; i < this.m_sliders.Length; i++)
			{
				this.m_sliders[i].onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
			}
			if (this.m_isSynchronizeOnStart)
			{
				this.OnSliderChanged(this.m_value);
			}
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x000AA6C4 File Offset: 0x000A88C4
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_sliders.Length; i++)
			{
				this.m_sliders[i].onValueChanged.RemoveListener(new UnityAction<float>(this.OnSliderChanged));
			}
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x000AA704 File Offset: 0x000A8904
		private void OnSliderChanged(float p_value)
		{
			this.m_value = p_value;
			for (int i = 0; i < this.m_sliders.Length; i++)
			{
				if (this.m_sliders[i].value != this.m_value)
				{
					this.m_sliders[i].value = this.m_value;
				}
			}
		}

		// Token: 0x0400196B RID: 6507
		[SerializeField]
		private Slider[] m_sliders = new Slider[0];

		// Token: 0x0400196C RID: 6508
		[SerializeField]
		private bool m_isSynchronizeOnStart = true;

		// Token: 0x0400196D RID: 6509
		[SerializeField]
		private float m_value;
	}
}
