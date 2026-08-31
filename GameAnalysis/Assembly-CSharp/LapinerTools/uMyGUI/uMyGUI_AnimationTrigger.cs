using System;
using System.Collections;
using UnityEngine;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000521 RID: 1313
	public class uMyGUI_AnimationTrigger : MonoBehaviour
	{
		// Token: 0x06002061 RID: 8289 RVA: 0x000A8407 File Offset: 0x000A6607
		private void OnEnable()
		{
			if (this.m_condition == uMyGUI_AnimationTrigger.ETriggerMode.ON_ENABLE)
			{
				this.Play();
			}
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x000A8417 File Offset: 0x000A6617
		private void OnDisable()
		{
			if (this.m_condition == uMyGUI_AnimationTrigger.ETriggerMode.ON_DISABLE)
			{
				this.Play();
			}
		}

		// Token: 0x06002063 RID: 8291 RVA: 0x000A8428 File Offset: 0x000A6628
		private void uMyGUI_OnActivateTab()
		{
			if (this.m_condition == uMyGUI_AnimationTrigger.ETriggerMode.ON_UMYGUI_ACTIVATETAB)
			{
				this.Play();
				return;
			}
			if (this.m_condition == uMyGUI_AnimationTrigger.ETriggerMode.REDIRECT_ONMYGUI_EVENTS)
			{
				if (this.m_redirectDestination == null)
				{
					Debug.LogError("LE_AnimationTrigger: uMyGUI_OnActivateTab: REDIRECT_ONMYGUI_EVENTS mode requires m_redirectDestination to be set!");
					return;
				}
				if (this.m_redirectDestination.activeInHierarchy)
				{
					this.m_redirectDestination.SendMessage("uMyGUI_OnActivateTab");
				}
			}
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x000A8484 File Offset: 0x000A6684
		private void uMyGUI_OnDeactivateTab()
		{
			if (this.m_condition == uMyGUI_AnimationTrigger.ETriggerMode.ON_UMYGUI_DEACTIVATETAB)
			{
				this.Play();
				return;
			}
			if (this.m_condition == uMyGUI_AnimationTrigger.ETriggerMode.REDIRECT_ONMYGUI_EVENTS)
			{
				if (this.m_redirectDestination == null)
				{
					Debug.LogError("LE_AnimationTrigger: uMyGUI_OnDeactivateTab: REDIRECT_ONMYGUI_EVENTS mode requires m_redirectDestination to be set!");
					return;
				}
				if (this.m_redirectDestination.activeInHierarchy)
				{
					this.m_redirectDestination.SendMessage("uMyGUI_OnDeactivateTab");
				}
			}
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x000A84E0 File Offset: 0x000A66E0
		private void Play()
		{
			if (this.m_animation != null)
			{
				if (this.m_isActivateOnAnimStart)
				{
					this.m_animation.gameObject.SetActive(true);
				}
				if (this.m_isDeactivateOnAnimEnd && this.m_animation[this.m_clipName] != null)
				{
					((this.m_alternativeCoroutineWorker != null) ? this.m_alternativeCoroutineWorker : this).StartCoroutine(this.DeactivateAfterDelay(this.m_animation.gameObject, this.m_animation[this.m_clipName].length));
				}
				this.m_animation.Play(this.m_clipName);
				return;
			}
			Debug.LogError("LE_AnimationTrigger: OnDisable: lost reference to Animation!");
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x000A8599 File Offset: 0x000A6799
		private IEnumerator DeactivateAfterDelay(GameObject p_object, float p_delay)
		{
			yield return new WaitForSeconds(p_delay);
			if (p_object != null)
			{
				p_object.SetActive(false);
			}
			yield break;
		}

		// Token: 0x04001911 RID: 6417
		[SerializeField]
		private Animation m_animation;

		// Token: 0x04001912 RID: 6418
		[SerializeField]
		private string m_clipName;

		// Token: 0x04001913 RID: 6419
		[SerializeField]
		private uMyGUI_AnimationTrigger.ETriggerMode m_condition;

		// Token: 0x04001914 RID: 6420
		[SerializeField]
		private bool m_isActivateOnAnimStart;

		// Token: 0x04001915 RID: 6421
		[SerializeField]
		private bool m_isDeactivateOnAnimEnd;

		// Token: 0x04001916 RID: 6422
		[SerializeField]
		private MonoBehaviour m_alternativeCoroutineWorker;

		// Token: 0x04001917 RID: 6423
		[SerializeField]
		private GameObject m_redirectDestination;

		// Token: 0x02000C8B RID: 3211
		public enum ETriggerMode
		{
			// Token: 0x04004EE2 RID: 20194
			ON_ENABLE,
			// Token: 0x04004EE3 RID: 20195
			ON_DISABLE,
			// Token: 0x04004EE4 RID: 20196
			ON_UMYGUI_ACTIVATETAB,
			// Token: 0x04004EE5 RID: 20197
			ON_UMYGUI_DEACTIVATETAB,
			// Token: 0x04004EE6 RID: 20198
			REDIRECT_ONMYGUI_EVENTS
		}
	}
}
