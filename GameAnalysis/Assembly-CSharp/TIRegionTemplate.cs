using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002C7 RID: 711
public class TIRegionTemplate : TIDataTemplate
{
	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000A60 RID: 2656 RVA: 0x00032428 File Offset: 0x00030628
	public override string displayName
	{
		get
		{
			if (this._displayName == null)
			{
				this._displayName = Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayName.").Append(base.localizationName).ToString());
			}
			return this._displayName;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000A61 RID: 2657 RVA: 0x00032478 File Offset: 0x00030678
	public string displayNameSentIn
	{
		get
		{
			if (this._displayNameSentIn == null)
			{
				this._displayNameSentIn = Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayNameSentIn.").Append(base.localizationName).ToString());
			}
			return this._displayNameSentIn;
		}
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000A62 RID: 2658 RVA: 0x000324C8 File Offset: 0x000306C8
	public string displayNameSentOf
	{
		get
		{
			if (this._displayNameSentOf == null)
			{
				this._displayNameSentOf = Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayNameSentOf.").Append(base.localizationName).ToString());
			}
			return this._displayNameSentOf;
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000A63 RID: 2659 RVA: 0x00032518 File Offset: 0x00030718
	public string fighterSquadronName
	{
		get
		{
			if (this._fighterSquadronName == null)
			{
				this._fighterSquadronName = Loc.T(new StringBuilder(base.GetType().Name).Append(".STOFighterName.").Append(base.localizationName).ToString());
			}
			return this._fighterSquadronName;
		}
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00032568 File Offset: 0x00030768
	public override bool IsValid(out string error)
	{
		error = string.Empty;
		float? num = this.afr;
		float num2 = 0f;
		if ((num.GetValueOrDefault() > num2) & (num != null))
		{
			if (string.IsNullOrEmpty(this.acc_afr))
			{
				error += "Need AFR Accent";
			}
			string[] array = this.afrPersonal;
			bool flag;
			if (array == null)
			{
				flag = true;
			}
			else
			{
				flag = array.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag)
			{
				error += "Need AFR personal namelist";
			}
			string[] array2 = this.afrFamily;
			bool flag2;
			if (array2 == null)
			{
				flag2 = true;
			}
			else
			{
				flag2 = array2.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag2)
			{
				error += "Need AFR family namelist";
			}
		}
		num = this.asi;
		num2 = 0f;
		if ((num.GetValueOrDefault() > num2) & (num != null))
		{
			if (string.IsNullOrEmpty(this.acc_asi))
			{
				error += "Need ASI Accent";
			}
			string[] array3 = this.asiPersonal;
			bool flag3;
			if (array3 == null)
			{
				flag3 = true;
			}
			else
			{
				flag3 = array3.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag3)
			{
				error += "Need ASI personal namelist";
			}
			string[] array4 = this.asiFamily;
			bool flag4;
			if (array4 == null)
			{
				flag4 = true;
			}
			else
			{
				flag4 = array4.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag4)
			{
				error += "Need ASI family namelist";
			}
		}
		num = this.eas;
		num2 = 0f;
		if ((num.GetValueOrDefault() > num2) & (num != null))
		{
			if (string.IsNullOrEmpty(this.acc_eas))
			{
				error += "Need EAS Accent";
			}
			string[] array5 = this.easPersonal;
			bool flag5;
			if (array5 == null)
			{
				flag5 = true;
			}
			else
			{
				flag5 = array5.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag5)
			{
				error += "Need EAS personal namelist";
			}
			string[] array6 = this.easFamily;
			bool flag6;
			if (array6 == null)
			{
				flag6 = true;
			}
			else
			{
				flag6 = array6.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag6)
			{
				error += "Need EAS family namelist";
			}
		}
		num = this.eur;
		num2 = 0f;
		if ((num.GetValueOrDefault() > num2) & (num != null))
		{
			if (string.IsNullOrEmpty(this.acc_eur))
			{
				error += "Need EUR Accent";
			}
			string[] array7 = this.eurPersonal;
			bool flag7;
			if (array7 == null)
			{
				flag7 = true;
			}
			else
			{
				flag7 = array7.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag7)
			{
				error += "Need EUR personal namelist";
			}
			string[] array8 = this.eurFamily;
			bool flag8;
			if (array8 == null)
			{
				flag8 = true;
			}
			else
			{
				flag8 = array8.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag8)
			{
				error += "Need EUR family namelist";
			}
		}
		num = this.his;
		num2 = 0f;
		if ((num.GetValueOrDefault() > num2) & (num != null))
		{
			if (string.IsNullOrEmpty(this.acc_his))
			{
				error += "Need HIS Accent";
			}
			string[] array9 = this.hisPersonal;
			bool flag9;
			if (array9 == null)
			{
				flag9 = true;
			}
			else
			{
				flag9 = array9.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag9)
			{
				error += "Need HIS personal namelist";
			}
			string[] array10 = this.hisFamily;
			bool flag10;
			if (array10 == null)
			{
				flag10 = true;
			}
			else
			{
				flag10 = array10.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag10)
			{
				error += "Need HIS family namelist";
			}
		}
		num = this.oce;
		num2 = 0f;
		if ((num.GetValueOrDefault() > num2) & (num != null))
		{
			if (string.IsNullOrEmpty(this.acc_oce))
			{
				error += "Need OCE Accent";
			}
			string[] array11 = this.ocePersonal;
			bool flag11;
			if (array11 == null)
			{
				flag11 = true;
			}
			else
			{
				flag11 = array11.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag11)
			{
				error += "Need OCE personal namelist";
			}
			string[] array12 = this.oceFamily;
			bool flag12;
			if (array12 == null)
			{
				flag12 = true;
			}
			else
			{
				flag12 = array12.All<string>((string x) => string.IsNullOrEmpty(x));
			}
			if (flag12)
			{
				error += "Need OCE family namelist";
			}
		}
		if (error != string.Empty)
		{
			error = base.dataName + " ERROR: " + error;
			return false;
		}
		return true;
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00032A28 File Offset: 0x00030C28
	public string accent(CouncilorAncestry ancestry)
	{
		switch (ancestry)
		{
		case CouncilorAncestry.African:
			return this.acc_afr;
		case CouncilorAncestry.Asian:
			return this.acc_asi;
		case CouncilorAncestry.EastAsian:
			return this.acc_eas;
		case CouncilorAncestry.European:
			return this.acc_eur;
		case CouncilorAncestry.Hispanic:
			return this.acc_his;
		case CouncilorAncestry.Oceanic:
			return this.acc_oce;
		default:
			return string.Empty;
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00032A88 File Offset: 0x00030C88
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIRegionState>();
		}
		return tigameState;
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000A67 RID: 2663 RVA: 0x00032AAC File Offset: 0x00030CAC
	public float baseBoostPerYear_dekatons
	{
		get
		{
			return (this.boostPerYear_tons * TemplateManager.global.spaceResourceToTons).GetValueOrDefault();
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000A68 RID: 2664 RVA: 0x00032AF5 File Offset: 0x00030CF5
	public List<string> illustrationPaths
	{
		get
		{
			return this.illustrationPathStrs.Where<string>((string x) => !string.IsNullOrEmpty(x)).ToList<string>();
		}
	}

	// Token: 0x04000873 RID: 2163
	public string mapRegionName;

	// Token: 0x04000874 RID: 2164
	public float population_Millions;

	// Token: 0x04000875 RID: 2165
	public bool mining;

	// Token: 0x04000876 RID: 2166
	public bool oilResource;

	// Token: 0x04000877 RID: 2167
	public bool coreEco;

	// Token: 0x04000878 RID: 2168
	public bool oilCapable;

	// Token: 0x04000879 RID: 2169
	public bool mineCapable;

	// Token: 0x0400087A RID: 2170
	public EnvironmentType environment = EnvironmentType.Standard;

	// Token: 0x0400087B RID: 2171
	public float annualPopGrowthModifier;

	// Token: 0x0400087C RID: 2172
	public float? boostPerYear_tons;

	// Token: 0x0400087D RID: 2173
	public int? missionControl;

	// Token: 0x0400087E RID: 2174
	public WorldOceanType worldOcean;

	// Token: 0x0400087F RID: 2175
	public float? afr;

	// Token: 0x04000880 RID: 2176
	public float? asi;

	// Token: 0x04000881 RID: 2177
	public float? eas;

	// Token: 0x04000882 RID: 2178
	public float? eur;

	// Token: 0x04000883 RID: 2179
	public float? his;

	// Token: 0x04000884 RID: 2180
	public float? oce;

	// Token: 0x04000885 RID: 2181
	public string[] afrPersonal;

	// Token: 0x04000886 RID: 2182
	public string[] afrFamily;

	// Token: 0x04000887 RID: 2183
	public float[] afrWeight;

	// Token: 0x04000888 RID: 2184
	public string[] asiPersonal;

	// Token: 0x04000889 RID: 2185
	public string[] asiFamily;

	// Token: 0x0400088A RID: 2186
	public float[] asiWeight;

	// Token: 0x0400088B RID: 2187
	public string[] easPersonal;

	// Token: 0x0400088C RID: 2188
	public string[] easFamily;

	// Token: 0x0400088D RID: 2189
	public float[] easWeight;

	// Token: 0x0400088E RID: 2190
	public string[] eurPersonal;

	// Token: 0x0400088F RID: 2191
	public string[] eurFamily;

	// Token: 0x04000890 RID: 2192
	public float[] eurWeight;

	// Token: 0x04000891 RID: 2193
	public string[] hisPersonal;

	// Token: 0x04000892 RID: 2194
	public string[] hisFamily;

	// Token: 0x04000893 RID: 2195
	public float[] hisWeight;

	// Token: 0x04000894 RID: 2196
	public string[] ocePersonal;

	// Token: 0x04000895 RID: 2197
	public string[] oceFamily;

	// Token: 0x04000896 RID: 2198
	public float[] oceWeight;

	// Token: 0x04000897 RID: 2199
	public string language;

	// Token: 0x04000898 RID: 2200
	public string acc_afr;

	// Token: 0x04000899 RID: 2201
	public string acc_asi;

	// Token: 0x0400089A RID: 2202
	public string acc_eas;

	// Token: 0x0400089B RID: 2203
	public string acc_eur;

	// Token: 0x0400089C RID: 2204
	public string acc_his;

	// Token: 0x0400089D RID: 2205
	public string acc_oce;

	// Token: 0x0400089E RID: 2206
	public List<string> illustrationPathStrs = new List<string>();

	// Token: 0x0400089F RID: 2207
	public string occupyingNation;

	// Token: 0x040008A0 RID: 2208
	public float occupationValue;

	// Token: 0x040008A1 RID: 2209
	public int? nuclearDetonations;

	// Token: 0x040008A2 RID: 2210
	public RegionalHeadwear asi_RegionalHeadwear;

	// Token: 0x040008A3 RID: 2211
	private string _displayNameSentIn;

	// Token: 0x040008A4 RID: 2212
	private string _displayNameSentOf;

	// Token: 0x040008A5 RID: 2213
	private string _fighterSquadronName;
}
