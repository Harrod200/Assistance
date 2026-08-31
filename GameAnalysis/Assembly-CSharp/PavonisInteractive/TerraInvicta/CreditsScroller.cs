using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000804 RID: 2052
	public class CreditsScroller : MonoBehaviour
	{
		// Token: 0x06004A5F RID: 19039 RVA: 0x001F30C9 File Offset: 0x001F12C9
		private void Start()
		{
			this.layoutGroup = base.GetComponent<VerticalLayoutGroup>();
			this.layoutGroup.enabled = true;
		}

		// Token: 0x06004A60 RID: 19040 RVA: 0x001F30E4 File Offset: 0x001F12E4
		private void Update()
		{
			if (!this.init)
			{
				this.init = true;
				return;
			}
			if (!this.associatedMenu.IsOpen)
			{
				base.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.layoutGroup.enabled = false;
				return;
			}
			base.StartCoroutine(this.ResetLayoutGroup());
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - Time.deltaTime * this.scrollSpeed, base.transform.localPosition.z);
		}

		// Token: 0x06004A61 RID: 19041 RVA: 0x001F3197 File Offset: 0x001F1397
		public IEnumerator EnableLayoutGroup()
		{
			yield return null;
			this.layoutGroup.enabled = true;
			yield break;
		}

		// Token: 0x06004A62 RID: 19042 RVA: 0x001F31A6 File Offset: 0x001F13A6
		public IEnumerator ResetLayoutGroup()
		{
			yield return null;
			this.layoutGroup.enabled = false;
			yield return null;
			base.StartCoroutine(this.EnableLayoutGroup());
			yield break;
		}

		// Token: 0x04002B42 RID: 11074
		public float scrollSpeed = 10f;

		// Token: 0x04002B43 RID: 11075
		public Menu associatedMenu;

		// Token: 0x04002B44 RID: 11076
		private VerticalLayoutGroup layoutGroup;

		// Token: 0x04002B45 RID: 11077
		private float scrollPos;

		// Token: 0x04002B46 RID: 11078
		private bool init;
	}
}
