using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000299 RID: 665
public struct StatModifier
{
	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000934 RID: 2356 RVA: 0x0002C980 File Offset: 0x0002AB80
	public bool conditionalModifier
	{
		get
		{
			TICondition ticondition = this.condition;
			return ticondition != null && ticondition.IsValid();
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0002C993 File Offset: 0x0002AB93
	public StatModifier(CouncilorAttribute stat, StatModSetOperation operation, string strValue, TICondition condition)
	{
		this.stat = stat;
		this.operation = operation;
		this.strValue = strValue;
		this.condition = condition;
		this._modifierValue = TIUtilities.GetIntValue(strValue);
	}

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000936 RID: 2358 RVA: 0x0002C9BE File Offset: 0x0002ABBE
	public int modifierValue
	{
		get
		{
			if (this._modifierValue == 0 && this.ModifierHasNumericValue())
			{
				this._modifierValue = TIUtilities.GetIntValue(this.strValue);
			}
			return this._modifierValue;
		}
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0002C9E8 File Offset: 0x0002ABE8
	private bool ModifierHasNumericValue()
	{
		StatModSetOperation statModSetOperation = this.operation;
		return statModSetOperation - StatModSetOperation.AdditivePer > 1 && statModSetOperation != StatModSetOperation.SetToAnotherAttribute;
	}

	// Token: 0x0400071A RID: 1818
	public CouncilorAttribute stat;

	// Token: 0x0400071B RID: 1819
	public StatModSetOperation operation;

	// Token: 0x0400071C RID: 1820
	public string strValue;

	// Token: 0x0400071D RID: 1821
	public TICondition condition;

	// Token: 0x0400071E RID: 1822
	private int _modifierValue;
}
