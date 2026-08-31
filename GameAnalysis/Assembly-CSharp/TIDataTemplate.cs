using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020003FE RID: 1022
public class TIDataTemplate : TIDataClass
{
	// Token: 0x1700031C RID: 796
	// (get) Token: 0x060014E7 RID: 5351 RVA: 0x000662FB File Offset: 0x000644FB
	// (set) Token: 0x060014E8 RID: 5352 RVA: 0x00066303 File Offset: 0x00064503
	public string dataName { get; protected set; }

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x060014E9 RID: 5353 RVA: 0x0006630C File Offset: 0x0006450C
	// (set) Token: 0x060014EA RID: 5354 RVA: 0x00066314 File Offset: 0x00064514
	public string friendlyName { get; protected set; }

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x060014EB RID: 5355 RVA: 0x0006631D File Offset: 0x0006451D
	// (set) Token: 0x060014EC RID: 5356 RVA: 0x00066325 File Offset: 0x00064525
	public string referenceAlias { get; protected set; }

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x060014ED RID: 5357 RVA: 0x0006632E File Offset: 0x0006452E
	public string referenceName
	{
		get
		{
			if (string.IsNullOrEmpty(this.referenceAlias))
			{
				return this.dataName;
			}
			return this.referenceAlias;
		}
	}

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x060014EE RID: 5358 RVA: 0x0006634A File Offset: 0x0006454A
	// (set) Token: 0x060014EF RID: 5359 RVA: 0x00066352 File Offset: 0x00064552
	public string localizationAlias { get; protected set; }

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x060014F0 RID: 5360 RVA: 0x0006635B File Offset: 0x0006455B
	public string localizationName
	{
		get
		{
			if (string.IsNullOrEmpty(this.localizationAlias))
			{
				return this.dataName;
			}
			return this.localizationAlias;
		}
	}

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x060014F1 RID: 5361 RVA: 0x00066377 File Offset: 0x00064577
	// (set) Token: 0x060014F2 RID: 5362 RVA: 0x0006637F File Offset: 0x0006457F
	public bool disable { get; protected set; }

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x060014F3 RID: 5363 RVA: 0x00066388 File Offset: 0x00064588
	// (set) Token: 0x060014F4 RID: 5364 RVA: 0x00066390 File Offset: 0x00064590
	public string[] scenarioTags { get; protected set; }

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x060014F5 RID: 5365 RVA: 0x0006639C File Offset: 0x0006459C
	public virtual string displayName
	{
		get
		{
			if (this._displayName == null)
			{
				this._displayName = Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName.").Append(this.localizationName).ToString());
			}
			return this._displayName;
		}
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000663EC File Offset: 0x000645EC
	public string displayNameCurrentForStartScreen()
	{
		return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayName.").Append(this.localizationName).ToString());
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x0006641D File Offset: 0x0006461D
	public TIDataTemplate()
	{
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x00066425 File Offset: 0x00064625
	public TIDataTemplate(string templateName)
	{
		this.dataName = templateName;
		this.friendlyName = templateName;
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x0006643B File Offset: 0x0006463B
	public virtual TIGameState CreateGameState()
	{
		return null;
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x0006643E File Offset: 0x0006463E
	public virtual bool IsValid(out string error)
	{
		if (string.IsNullOrEmpty(this.dataName))
		{
			error = "empty or null dataName";
			return false;
		}
		error = string.Empty;
		return true;
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x0006645E File Offset: 0x0006465E
	public void RenameDataName(string dataName_)
	{
		this.dataName = dataName_;
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x00066467 File Offset: 0x00064667
	public override string ToString()
	{
		return string.Format("{0}: {1}", base.ToString(), this.dataName);
	}

	// Token: 0x04001288 RID: 4744
	[SerializeField]
	protected string _displayName;
}
