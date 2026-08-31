using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000522 RID: 1314
	public class uMyGUI_ColorPicker : MonoBehaviour
	{
		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06002068 RID: 8296 RVA: 0x000A85B7 File Offset: 0x000A67B7
		// (set) Token: 0x06002069 RID: 8297 RVA: 0x000A85BF File Offset: 0x000A67BF
		public Slider RedSlider
		{
			get
			{
				return this.m_redSlider;
			}
			set
			{
				this.m_redSlider = value;
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x0600206A RID: 8298 RVA: 0x000A85C8 File Offset: 0x000A67C8
		// (set) Token: 0x0600206B RID: 8299 RVA: 0x000A85D0 File Offset: 0x000A67D0
		public Slider GreenSlider
		{
			get
			{
				return this.m_greenSlider;
			}
			set
			{
				this.m_greenSlider = value;
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x0600206C RID: 8300 RVA: 0x000A85D9 File Offset: 0x000A67D9
		// (set) Token: 0x0600206D RID: 8301 RVA: 0x000A85E1 File Offset: 0x000A67E1
		public Slider BlueSlider
		{
			get
			{
				return this.m_blueSlider;
			}
			set
			{
				this.m_blueSlider = value;
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x0600206E RID: 8302 RVA: 0x000A85EA File Offset: 0x000A67EA
		// (set) Token: 0x0600206F RID: 8303 RVA: 0x000A85F2 File Offset: 0x000A67F2
		public Color PickedColor
		{
			get
			{
				return this.m_pickedColor;
			}
			set
			{
				if (this.m_pickedColor != value)
				{
					this.m_pickedColor = value;
					this.UpdateColor();
					if (this.m_onChanged != null)
					{
						this.m_onChanged(this, new uMyGUI_ColorPicker.ColorEventArgs(this.m_pickedColor));
					}
				}
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06002070 RID: 8304 RVA: 0x000A862E File Offset: 0x000A682E
		// (set) Token: 0x06002071 RID: 8305 RVA: 0x000A8636 File Offset: 0x000A6836
		public Graphic ColorPreview
		{
			get
			{
				return this.m_colorPreview;
			}
			set
			{
				this.m_colorPreview = value;
			}
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x000A8640 File Offset: 0x000A6840
		private void Start()
		{
			if (this.m_redSlider == null || this.m_greenSlider == null || this.m_blueSlider == null)
			{
				Debug.LogError("uMyGUI_ColorPicker: all three sliders (RGB) must be set in inspector!");
				base.enabled = false;
				return;
			}
			this.UpdateColor();
			this.m_redSlider.onValueChanged.AddListener(new UnityAction<float>(this.SetRedValue));
			this.m_greenSlider.onValueChanged.AddListener(new UnityAction<float>(this.SetGreenValue));
			this.m_blueSlider.onValueChanged.AddListener(new UnityAction<float>(this.SetBlueValue));
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x000A86E4 File Offset: 0x000A68E4
		private void OnDestroy()
		{
			this.m_onChanged = null;
			this.m_redSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.SetRedValue));
			this.m_greenSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.SetGreenValue));
			this.m_blueSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.SetBlueValue));
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x000A874C File Offset: 0x000A694C
		private void SetRedValue(float p_redValue)
		{
			if (this.m_pickedColor.r != p_redValue)
			{
				this.m_pickedColor.r = p_redValue;
				this.UpdateColor();
				if (this.m_onChanged != null)
				{
					this.m_onChanged(this, new uMyGUI_ColorPicker.ColorEventArgs(this.m_pickedColor));
				}
			}
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x000A8798 File Offset: 0x000A6998
		private void SetGreenValue(float p_greenValue)
		{
			if (this.m_pickedColor.g != p_greenValue)
			{
				this.m_pickedColor.g = p_greenValue;
				this.UpdateColor();
				if (this.m_onChanged != null)
				{
					this.m_onChanged(this, new uMyGUI_ColorPicker.ColorEventArgs(this.m_pickedColor));
				}
			}
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x000A87E4 File Offset: 0x000A69E4
		private void SetBlueValue(float p_blueValue)
		{
			if (this.m_pickedColor.b != p_blueValue)
			{
				this.m_pickedColor.b = p_blueValue;
				this.UpdateColor();
				if (this.m_onChanged != null)
				{
					this.m_onChanged(this, new uMyGUI_ColorPicker.ColorEventArgs(this.m_pickedColor));
				}
			}
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x000A8830 File Offset: 0x000A6A30
		private void UpdateColor()
		{
			this.m_redSlider.value = this.m_pickedColor.r;
			this.m_greenSlider.value = this.m_pickedColor.g;
			this.m_blueSlider.value = this.m_pickedColor.b;
			if (this.m_colorPreview != null)
			{
				this.m_colorPreview.color = this.m_pickedColor;
			}
		}

		// Token: 0x04001918 RID: 6424
		[SerializeField]
		private Slider m_redSlider;

		// Token: 0x04001919 RID: 6425
		[SerializeField]
		private Slider m_greenSlider;

		// Token: 0x0400191A RID: 6426
		[SerializeField]
		private Slider m_blueSlider;

		// Token: 0x0400191B RID: 6427
		[SerializeField]
		private Color m_pickedColor = Color.gray;

		// Token: 0x0400191C RID: 6428
		[SerializeField]
		private Graphic m_colorPreview;

		// Token: 0x0400191D RID: 6429
		public EventHandler<uMyGUI_ColorPicker.ColorEventArgs> m_onChanged;

		// Token: 0x02000C8D RID: 3213
		public class ColorEventArgs : EventArgs
		{
			// Token: 0x06006D21 RID: 27937 RVA: 0x0030A62B File Offset: 0x0030882B
			public ColorEventArgs(Color p_value)
			{
				this.Value = p_value;
			}

			// Token: 0x04004EEB RID: 20203
			public readonly Color Value;
		}
	}
}
