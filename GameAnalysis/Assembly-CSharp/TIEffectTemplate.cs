using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020002CB RID: 715
public class TIEffectTemplate : TIDataTemplate
{
	// Token: 0x06000A6F RID: 2671 RVA: 0x00032C78 File Offset: 0x00030E78
	public override bool IsValid(out string error)
	{
		error = string.Empty;
		if (!base.disable && this.instantEffect == InstantEffect.None && (this.contexts.Count == 0 || this.contexts[0] == Context.None))
		{
			error = "No context or instant trigger assigned for " + base.dataName;
			return false;
		}
		return true;
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000A70 RID: 2672 RVA: 0x00032CCC File Offset: 0x00030ECC
	public List<TIFactionState> InitialFactions
	{
		get
		{
			List<TIFactionState> list = new List<TIFactionState>();
			foreach (string text in this.initialFactionsStr)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(text, true);
					if (tifactionState != null)
					{
						list.Add(tifactionState);
					}
					else if (TemplateManager.Find<TIFactionTemplate>(text, false) == null)
					{
						Log.Error("bad faction entry " + text + " in initialFactionStr in TIEffectTemplate", Array.Empty<object>());
					}
				}
			}
			return list;
		}
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00032D64 File Offset: 0x00030F64
	public List<Context> GetContexts()
	{
		if (this._contexts == null)
		{
			this._contexts = this.contexts.Where<Context>((Context x) => x > Context.None).ToList<Context>();
		}
		return this._contexts;
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00032DB4 File Offset: 0x00030FB4
	public string description(TIGameState state1, TIGameState state2)
	{
		string text = new StringBuilder("TIEffectTemplate.description.").Append(base.localizationName).ToString();
		object[] array = new object[30];
		array[0] = TIUtilities.FormatBigOrSmallNumber(this.value, 1, 7, 0, false, false) ?? string.Empty;
		array[1] = ((state1 != null) ? state1.displayName : null) ?? string.Empty;
		array[2] = ((state2 != null) ? state2.displayName : null) ?? string.Empty;
		array[3] = this.value.ToPercent("P0") ?? string.Empty;
		array[4] = (1f - this.value).ToPercent("P0") ?? string.Empty;
		int num = 5;
		object obj;
		if (state1 == null)
		{
			obj = null;
		}
		else
		{
			TINationState ref_nation = state1.ref_nation;
			obj = ((ref_nation != null) ? ref_nation.displayNameWithArticleAndPlacePrep : null);
		}
		array[num] = obj;
		int num2 = 6;
		object obj2;
		if (state2 == null)
		{
			obj2 = null;
		}
		else
		{
			TINationState ref_nation2 = state2.ref_nation;
			obj2 = ((ref_nation2 != null) ? ref_nation2.displayNameWithArticleAndPlacePrep : null);
		}
		array[num2] = obj2;
		array[7] = TIUtilities.FormatBigOrSmallNumber(this.duration_months, 1, 7, 0, false, false) ?? string.Empty;
		array[8] = (this.value - 1f).ToPercent("P0") ?? string.Empty;
		int num3 = 9;
		string text2;
		if (state1 == null)
		{
			text2 = null;
		}
		else
		{
			TINationState ref_nation3 = state1.ref_nation;
			text2 = ((ref_nation3 != null) ? ref_nation3.displayName : null);
		}
		string text3;
		if ((text3 = text2) == null)
		{
			text3 = ((state1 != null) ? state1.displayName : null) ?? string.Empty;
		}
		array[num3] = text3;
		int num4 = 10;
		string text4;
		if (state1 == null)
		{
			text4 = null;
		}
		else
		{
			TIRegionState ref_region = state1.ref_region;
			text4 = ((ref_region != null) ? ref_region.displayName : null);
		}
		string text5;
		if ((text5 = text4) == null)
		{
			text5 = ((state1 != null) ? state1.displayName : null) ?? string.Empty;
		}
		array[num4] = text5;
		int num5 = 11;
		string text6;
		if (state2 == null)
		{
			text6 = null;
		}
		else
		{
			TINationState ref_nation4 = state2.ref_nation;
			text6 = ((ref_nation4 != null) ? ref_nation4.displayName : null);
		}
		string text7;
		if ((text7 = text6) == null)
		{
			text7 = ((state2 != null) ? state2.displayName : null) ?? string.Empty;
		}
		array[num5] = text7;
		int num6 = 12;
		string text8;
		if (state2 == null)
		{
			text8 = null;
		}
		else
		{
			TIRegionState ref_region2 = state2.ref_region;
			text8 = ((ref_region2 != null) ? ref_region2.displayName : null);
		}
		string text9;
		if ((text9 = text8) == null)
		{
			text9 = ((state2 != null) ? state2.displayName : null) ?? string.Empty;
		}
		array[num6] = text9;
		int num7 = 13;
		TIDataTemplate tidataTemplate = TemplateManager.Find<TIDataTemplate>(this.strValue, true);
		array[num7] = ((tidataTemplate != null) ? tidataTemplate.displayName : null) ?? this.strValue;
		int num8 = 14;
		string text10;
		if (state1 == null)
		{
			text10 = null;
		}
		else
		{
			TIFactionState ref_faction = state1.ref_faction;
			text10 = ((ref_faction != null) ? ref_faction.displayNameWithColor : null);
		}
		string text11;
		if ((text11 = text10) == null)
		{
			text11 = ((state1 != null) ? state1.displayName : null) ?? string.Empty;
		}
		array[num8] = text11;
		int num9 = 15;
		string text12;
		if (state2 == null)
		{
			text12 = null;
		}
		else
		{
			TIFactionState ref_faction2 = state2.ref_faction;
			text12 = ((ref_faction2 != null) ? ref_faction2.displayNameWithColor : null);
		}
		string text13;
		if ((text13 = text12) == null)
		{
			text13 = ((state2 != null) ? state2.displayName : null) ?? string.Empty;
		}
		array[num9] = text13;
		int num10 = 16;
		string text14 = ", ";
		List<string> list;
		if ((list = TICouncilorState.GetAllTraitsOfGrouping((int)this.value).ConvertAll<string>((TITraitTemplate x) => x.displayName)) == null)
		{
			(list = new List<string>()).Add(string.Empty);
		}
		array[num10] = string.Join(text14, list) ?? string.Empty;
		int num11 = 17;
		object obj3;
		if (state1 == null)
		{
			obj3 = null;
		}
		else
		{
			TICouncilorState ref_councilor = state1.ref_councilor;
			if (ref_councilor == null)
			{
				obj3 = null;
			}
			else
			{
				TITraitTemplate traitGrouping = ref_councilor.GetTraitGrouping((int)this.value);
				obj3 = ((traitGrouping != null) ? traitGrouping.displayName : null);
			}
		}
		array[num11] = obj3 ?? string.Empty;
		array[18] = (1f / this.value - 1f).ToPercent("P0") ?? string.Empty;
		array[19] = TIUtilities.FormatBigOrSmallNumber(-this.value, 1, 7, 0, false, false) ?? string.Empty;
		array[20] = TIUtilities.GetAttributeString(this.strValue.ToEnum(CouncilorAttribute.None));
		int num12 = 21;
		string text15;
		if (state1 == null)
		{
			text15 = null;
		}
		else
		{
			TIFactionState ref_faction3 = state1.ref_faction;
			text15 = ((ref_faction3 != null) ? ref_faction3.displayNameCapitalizedWithColor : null);
		}
		string text16;
		if ((text16 = text15) == null)
		{
			text16 = ((state1 != null) ? state1.displayName : null) ?? string.Empty;
		}
		array[num12] = text16;
		int num13 = 22;
		string text17;
		if (state2 == null)
		{
			text17 = null;
		}
		else
		{
			TIFactionState ref_faction4 = state2.ref_faction;
			text17 = ((ref_faction4 != null) ? ref_faction4.displayNameCapitalizedWithColor : null);
		}
		string text18;
		if ((text18 = text17) == null)
		{
			text18 = ((state2 != null) ? state2.displayName : null) ?? string.Empty;
		}
		array[num13] = text18;
		int num14 = 23;
		float num15 = this.value;
		float? num16;
		if (state1 == null)
		{
			num16 = null;
		}
		else
		{
			TINationState ref_nation5 = state1.ref_nation;
			num16 = ((ref_nation5 != null) ? new float?(ref_nation5.priorityEffectPopScaling) : null);
		}
		array[num14] = TIUtilities.FormatSmallNumber(Mathf.Clamp((num15 * num16) ?? 1f, TIEffectsState.MinScaledTenPointStatEffect(this.value), TIEffectsState.MaxScaledTenPointStatEffect(this.value)), 7, 0, true, false);
		int num17 = 24;
		num15 = this.value;
		float? num18;
		if (state2 == null)
		{
			num18 = null;
		}
		else
		{
			TINationState ref_nation6 = state2.ref_nation;
			num18 = ((ref_nation6 != null) ? new float?(ref_nation6.priorityEffectPopScaling) : null);
		}
		array[num17] = TIUtilities.FormatSmallNumber(Mathf.Clamp((num15 * num18) ?? 1f, TIEffectsState.MinScaledTenPointStatEffect(this.value), TIEffectsState.MaxScaledTenPointStatEffect(this.value)), 7, 0, true, false);
		array[25] = Mathf.Abs(this.value).ToPercent("P0") ?? string.Empty;
		int num19 = 26;
		string text19 = ", ";
		List<string> list2;
		if (state1 == null)
		{
			list2 = null;
		}
		else
		{
			TIFactionState ref_faction5 = state1.ref_faction;
			if (ref_faction5 == null)
			{
				list2 = null;
			}
			else
			{
				list2 = ref_faction5.executiveNations.ConvertAll<string>((TINationState x) => x.displayName);
			}
		}
		List<string> list3;
		if ((list3 = list2) == null)
		{
			(list3 = new List<string>()).Add(string.Empty);
		}
		array[num19] = string.Join(text19, list3) ?? string.Empty;
		array[27] = (1f - 1f / this.value).ToPercent("P0") ?? string.Empty;
		array[28] = (this.value - 1f).ToPercent("+0%;-0%;0%") ?? string.Empty;
		array[29] = this.value.ToPercent("+0%;-0%;0%") ?? string.Empty;
		return Loc.T(text, array);
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x000333C4 File Offset: 0x000315C4
	public string allDescription(int code)
	{
		string text = string.Empty;
		switch (code)
		{
		case 0:
			text = Loc.T("UI.NarrativeEvent.AllHumanNations");
			break;
		case 1:
			text = Loc.T("UI.NarrativeEvent.AllNations");
			break;
		case 2:
			text = Loc.T("UI.NarrativeEvent.AllRegions");
			break;
		}
		string text2 = new StringBuilder("TIEffectTemplate.description.").Append(base.localizationName).ToString();
		object[] array = new object[26];
		array[0] = TIUtilities.FormatBigOrSmallNumber(this.value, 1, 7, 0, false, false) ?? string.Empty;
		array[1] = text;
		array[2] = text;
		array[3] = this.value.ToPercent("P0") ?? string.Empty;
		array[4] = (1f - this.value).ToPercent("P0") ?? string.Empty;
		array[5] = text;
		array[6] = text;
		array[7] = TIUtilities.FormatBigOrSmallNumber(this.duration_months, 1, 7, 0, false, false) ?? string.Empty;
		array[8] = (this.value - 1f).ToPercent("P0") ?? string.Empty;
		array[9] = text;
		array[10] = text;
		array[11] = text;
		array[12] = text;
		int num = 13;
		TIDataTemplate tidataTemplate = TemplateManager.Find<TIDataTemplate>(this.strValue, true);
		array[num] = ((tidataTemplate != null) ? tidataTemplate.displayName : null) ?? this.strValue;
		array[14] = text;
		array[15] = text;
		int num2 = 16;
		string text3 = ", ";
		List<string> list;
		if ((list = TICouncilorState.GetAllTraitsOfGrouping((int)this.value).ConvertAll<string>((TITraitTemplate x) => x.displayName)) == null)
		{
			(list = new List<string>()).Add(string.Empty);
		}
		array[num2] = string.Join(text3, list) ?? string.Empty;
		array[17] = text;
		array[18] = (1f / this.value - 1f).ToPercent("P0") ?? string.Empty;
		array[19] = TIUtilities.FormatBigOrSmallNumber(-this.value, 1, 7, 0, false, false) ?? string.Empty;
		array[20] = TIUtilities.GetAttributeString(this.strValue.ToEnum(CouncilorAttribute.None));
		array[21] = text;
		array[22] = text;
		array[23] = TIUtilities.FormatSmallNumber(this.value, 7, 0, true, false);
		array[24] = TIUtilities.FormatSmallNumber(this.value, 7, 0, true, false);
		array[25] = Mathf.Abs(this.value).ToPercent("P0") ?? string.Empty;
		return Loc.T(text2, array);
	}

	// Token: 0x040008CA RID: 2250
	public List<Context> contexts = new List<Context>();

	// Token: 0x040008CB RID: 2251
	public InstantEffect instantEffect;

	// Token: 0x040008CC RID: 2252
	public bool stackable;

	// Token: 0x040008CD RID: 2253
	public StatModSetOperation operation;

	// Token: 0x040008CE RID: 2254
	public float value;

	// Token: 0x040008CF RID: 2255
	public float instantRnd;

	// Token: 0x040008D0 RID: 2256
	public string strValue = string.Empty;

	// Token: 0x040008D1 RID: 2257
	public EffectTargetType effectTarget;

	// Token: 0x040008D2 RID: 2258
	public EffectSecondaryStateType effectSecondaryTarget;

	// Token: 0x040008D3 RID: 2259
	public EffectDuration effectDuration;

	// Token: 0x040008D4 RID: 2260
	public float duration_months;

	// Token: 0x040008D5 RID: 2261
	public List<string> initialFactionsStr = new List<string>();

	// Token: 0x040008D6 RID: 2262
	public TotalEffectDisplayBehavior showTotal;

	// Token: 0x040008D7 RID: 2263
	private List<Context> _contexts;
}
