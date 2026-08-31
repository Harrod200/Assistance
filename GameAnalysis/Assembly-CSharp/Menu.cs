using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200043D RID: 1085
public class Menu : MonoBehaviour
{
	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001677 RID: 5751 RVA: 0x00072CFE File Offset: 0x00070EFE
	// (set) Token: 0x06001678 RID: 5752 RVA: 0x00072D20 File Offset: 0x00070F20
	public bool IsOpen
	{
		get
		{
			return this._animator != null && this._animator.GetBool("IsOpen");
		}
		set
		{
			if (this._animator != null)
			{
				this._animator.SetBool("IsOpen", value);
			}
		}
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x00072D44 File Offset: 0x00070F44
	public void Awake()
	{
		this._animator = base.GetComponent<Animator>();
		this._canvasGroup = base.GetComponent<CanvasGroup>();
		if (this._animator == null)
		{
			Debug.Log("Error initializing animator: null _animator");
		}
		RectTransform component = base.GetComponent<RectTransform>();
		Vector2 vector = new Vector2(0f, 0f);
		component.offsetMin = vector;
		component.offsetMax = vector;
		this._canvasGroup.blocksRaycasts = false;
		this._canvasGroup.interactable = false;
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x00072DC0 File Offset: 0x00070FC0
	public void Open()
	{
		base.gameObject.SetActive(true);
		MenuController component = base.gameObject.GetComponent<MenuController>();
		if (component != null)
		{
			component.OnOpen();
		}
		base.StartCoroutine(this.SetInteractable());
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x00072E01 File Offset: 0x00071001
	public void Close()
	{
		MenuController component = base.gameObject.GetComponent<MenuController>();
		if (component != null)
		{
			component.OnClose();
		}
		base.StartCoroutine(this.SetNonInteractable());
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x00072E26 File Offset: 0x00071026
	private IEnumerator SetInteractable()
	{
		while (!this._animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
		{
			yield return new WaitForSeconds(0.1f);
		}
		this._canvasGroup.blocksRaycasts = true;
		this._canvasGroup.interactable = true;
		yield break;
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x00072E35 File Offset: 0x00071035
	private IEnumerator SetNonInteractable()
	{
		while (this._animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
		{
			yield return new WaitForSeconds(0.1f);
		}
		this._canvasGroup.blocksRaycasts = false;
		this._canvasGroup.interactable = false;
		yield break;
	}

	// Token: 0x040014D5 RID: 5333
	public Animator _animator;

	// Token: 0x040014D6 RID: 5334
	public CanvasGroup _canvasGroup;
}
