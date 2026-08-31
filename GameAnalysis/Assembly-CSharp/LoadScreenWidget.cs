using System;
using System.Collections;
using PavonisInteractive.TerraInvicta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200041A RID: 1050
public class LoadScreenWidget : MonoBehaviour
{
	// Token: 0x06001567 RID: 5479 RVA: 0x000698AB File Offset: 0x00067AAB
	private void Awake()
	{
		this.InitLoadWidget();
		this.fillBarImage.fillAmount = 0.05f;
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x000698C3 File Offset: 0x00067AC3
	private void Start()
	{
		Loc.SwapFonts(base.gameObject);
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000698D0 File Offset: 0x00067AD0
	public void InitLoadWidget()
	{
		Debug.Log("Init LoadWidget");
		this.LoadNextTip();
		this.imageShowTime = Time.time;
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000698F0 File Offset: 0x00067AF0
	public void LoadIllustration()
	{
		int num = global::UnityEngine.Random.Range(0, TemplateManager.global.illus_loadingScreens.Count);
		GameControl.control._assetLoader.LoadAssetForImageAssignment(TemplateManager.global.illus_loadingScreens[num], this.loadingIllustration);
		this.imageShowTime = Time.time;
		this.firstImageLoaded = true;
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x0006994C File Offset: 0x00067B4C
	public void LoadNextTip()
	{
		this.randomPick = global::UnityEngine.Random.Range(0, TemplateManager.global.numberOfLoadingScreenTips);
		this.flavorText.text = Loc.T("UI.Options.Tip" + this.randomPick.ToString());
		this.textShowTime = Time.time;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x000699A0 File Offset: 0x00067BA0
	private void Update()
	{
		if (Time.time - this.textChangeInterval > this.textShowTime)
		{
			this.LoadNextTip();
		}
		if (Time.time - this.imageChangeInterval > this.imageShowTime && this.firstImageLoaded)
		{
			this.LoadIllustration();
			this.imageShowTime = Time.time;
		}
		if (Time.time - this.dotInterval > this.dotShowTime)
		{
			this.fillBarImage.fillAmount += global::UnityEngine.Random.Range(0.002f, 0.004f);
			this.dotShowTime = Time.time + global::UnityEngine.Random.Range(0.01f, 0.02f);
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00069A44 File Offset: 0x00067C44
	public void SetBar(float value, bool close = false)
	{
		this.fillBarImage.fillAmount = value;
		if (close)
		{
			base.StartCoroutine(this.CloseLoadScreen());
		}
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x00069A62 File Offset: 0x00067C62
	public IEnumerator CloseLoadScreen()
	{
		yield return null;
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x00069A71 File Offset: 0x00067C71
	public void HideWidget()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040012C1 RID: 4801
	public TextMeshProUGUI flavorText;

	// Token: 0x040012C2 RID: 4802
	public TextMeshProUGUI progressText;

	// Token: 0x040012C3 RID: 4803
	public TextMeshProUGUI cycleText;

	// Token: 0x040012C4 RID: 4804
	public Image fillBarImage;

	// Token: 0x040012C5 RID: 4805
	public Image loadingIllustration;

	// Token: 0x040012C6 RID: 4806
	private float textChangeInterval = 15f;

	// Token: 0x040012C7 RID: 4807
	private float imageChangeInterval = 15f;

	// Token: 0x040012C8 RID: 4808
	private float textShowTime;

	// Token: 0x040012C9 RID: 4809
	private float imageShowTime;

	// Token: 0x040012CA RID: 4810
	private int randomPick;

	// Token: 0x040012CB RID: 4811
	private float dotInterval = 0.2f;

	// Token: 0x040012CC RID: 4812
	private float dotShowTime;

	// Token: 0x040012CD RID: 4813
	private bool firstImageLoaded;
}
