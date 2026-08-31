using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RainbowArt
{
	// Token: 0x0200054A RID: 1354
	public class PlayAnimHelper : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x060022B9 RID: 8889 RVA: 0x000B4176 File Offset: 0x000B2376
		private void Start()
		{
			this.mAnimator = base.GetComponent<Animator>();
			if (this.triggerAnimType == TriggerAnimType.Auto)
			{
				this.mAnimator.ResetTrigger("Stop");
				this.mAnimator.Play("Start");
			}
		}

		// Token: 0x060022BA RID: 8890 RVA: 0x000B41B0 File Offset: 0x000B23B0
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.triggerAnimType == TriggerAnimType.Click)
			{
				if (!this.mAnimStarted)
				{
					this.mAnimStarted = true;
					this.mAnimator.ResetTrigger("Stop");
					this.mAnimator.Play("Start");
					return;
				}
				this.mAnimStarted = false;
				this.mAnimator.SetTrigger("Stop");
			}
		}

		// Token: 0x060022BB RID: 8891 RVA: 0x000B420D File Offset: 0x000B240D
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.triggerAnimType == TriggerAnimType.Hover)
			{
				this.mAnimator.ResetTrigger("Stop");
				this.mAnimator.Play("Start");
			}
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x000B4237 File Offset: 0x000B2437
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.triggerAnimType == TriggerAnimType.Hover)
			{
				this.mAnimator.SetTrigger("Stop");
			}
		}

		// Token: 0x04001A59 RID: 6745
		public TriggerAnimType triggerAnimType;

		// Token: 0x04001A5A RID: 6746
		private Animator mAnimator;

		// Token: 0x04001A5B RID: 6747
		private bool mAnimStarted;
	}
}
