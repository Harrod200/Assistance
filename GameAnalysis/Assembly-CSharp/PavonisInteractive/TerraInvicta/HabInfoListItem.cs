using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200086A RID: 2154
	public class HabInfoListItem : MonoBehaviour
	{
		// Token: 0x17000ECE RID: 3790
		// (get) Token: 0x06004FE4 RID: 20452 RVA: 0x00227F7D File Offset: 0x0022617D
		public HorizontalLayoutGroup Group
		{
			get
			{
				return this.group;
			}
		}

		// Token: 0x06004FE5 RID: 20453 RVA: 0x00227F85 File Offset: 0x00226185
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x00227F8D File Offset: 0x0022618D
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x00227F95 File Offset: 0x00226195
		private void Init()
		{
			if (this.hasInit)
			{
				return;
			}
			this.CacheComponents();
			this.hasInit = true;
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x00227FAD File Offset: 0x002261AD
		private void CacheComponents()
		{
			this.group = base.GetComponent<HorizontalLayoutGroup>();
			this.text = base.gameObject.GetComponentOnChild<TMP_Text>("Text");
		}

		// Token: 0x06004FE9 RID: 20457 RVA: 0x00227FD1 File Offset: 0x002261D1
		public void IgnoreSetText()
		{
			this.ignoreSetText = true;
		}

		// Token: 0x06004FEA RID: 20458 RVA: 0x00227FDA File Offset: 0x002261DA
		public void SetText(float value)
		{
			this.SetText((value != 0f) ? value.ToString() : "-");
		}

		// Token: 0x06004FEB RID: 20459 RVA: 0x00227FF8 File Offset: 0x002261F8
		public void SetText(string text)
		{
			if (!this.hasInit)
			{
				this.Init();
			}
			if (this.ignoreSetText)
			{
				return;
			}
			this.text.text = text;
		}

		// Token: 0x06004FEC RID: 20460 RVA: 0x0022801D File Offset: 0x0022621D
		public string GetText()
		{
			return this.text.text;
		}

		// Token: 0x06004FED RID: 20461 RVA: 0x0022802A File Offset: 0x0022622A
		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004FEE RID: 20462 RVA: 0x00228038 File Offset: 0x00226238
		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x0400334E RID: 13134
		private bool hasInit;

		// Token: 0x0400334F RID: 13135
		private HorizontalLayoutGroup group;

		// Token: 0x04003350 RID: 13136
		private TMP_Text text;

		// Token: 0x04003351 RID: 13137
		private bool ignoreSetText;
	}
}
