using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002A4 RID: 676
public class TIMapRegionTemplate : TIDataTemplate
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000948 RID: 2376 RVA: 0x0002CEAF File Offset: 0x0002B0AF
	public bool smallRegion
	{
		get
		{
			return this.area_km2 < TIGlobalConfig.globalConfig.smallRegionDefinition_km2;
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000949 RID: 2377 RVA: 0x0002CEC3 File Offset: 0x0002B0C3
	public override string displayName
	{
		get
		{
			if (this._displayName == null)
			{
				TIRegionState tiregionState = GameStateManager.MapRegionLookup(base.dataName);
				this._displayName = ((tiregionState != null) ? tiregionState.displayName : null) ?? null;
			}
			return this._displayName;
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x0002CEF8 File Offset: 0x0002B0F8
	public static float GetSeaTravelMultiplier(CoastRegion region1, CoastRegion region2, bool SuezAccess, bool PanamaAccess, bool arcticOpen = false)
	{
		float num = TIMapRegionTemplate._defaultSeaTravelModifiers[region1][region2];
		if (!SuezAccess && TIMapRegionTemplate.SuezRoute(region1, region2, arcticOpen))
		{
			num *= 1.6f;
		}
		if (!PanamaAccess && TIMapRegionTemplate.PanamaRoute(region1, region2, arcticOpen))
		{
			num *= 1.4f;
		}
		return num;
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0002CF44 File Offset: 0x0002B144
	public static bool TurkishStraitRoute(CoastRegion region1, CoastRegion region2)
	{
		if (region1 != CoastRegion.BlackSea)
		{
			return region1 != CoastRegion.BlackMed && region2 == CoastRegion.BlackSea;
		}
		return region2 != CoastRegion.BlackSea && region2 != CoastRegion.BlackMed;
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x0002CF68 File Offset: 0x0002B168
	public static bool SuezRoute(CoastRegion region1, CoastRegion region2, bool arcticOpen)
	{
		if (region1 == region2)
		{
			return false;
		}
		switch (region1)
		{
		case CoastRegion.Arctic:
		case CoastRegion.Caribbean:
			return region2 == CoastRegion.Indian;
		case CoastRegion.BalticSea:
		case CoastRegion.NortheastAtlantic:
			if (region2 != CoastRegion.Indian)
			{
				switch (region2)
				{
				case CoastRegion.NorthwestPacific:
					return !arcticOpen;
				case CoastRegion.SouthChinaSea:
				case CoastRegion.SouthwestPacific:
					return true;
				}
				return false;
			}
			return true;
		case CoastRegion.BlackSea:
		case CoastRegion.Mediterranean:
			if (region2 != CoastRegion.Indian)
			{
				switch (region2)
				{
				case CoastRegion.NorthwestPacific:
				case CoastRegion.SouthChinaSea:
				case CoastRegion.SouthwestPacific:
					return true;
				}
				return false;
			}
			return true;
		case CoastRegion.Indian:
			switch (region2)
			{
			case CoastRegion.Arctic:
			case CoastRegion.BalticSea:
			case CoastRegion.BlackSea:
			case CoastRegion.Caribbean:
			case CoastRegion.Mediterranean:
			case CoastRegion.NortheastAtlantic:
			case CoastRegion.NorthwestAtlantic:
				return true;
			}
			return false;
		case CoastRegion.NorthwestAtlantic:
			return region2 == CoastRegion.Indian;
		case CoastRegion.NorthwestPacific:
			switch (region2)
			{
			case CoastRegion.BalticSea:
			case CoastRegion.NortheastAtlantic:
				return !arcticOpen;
			case CoastRegion.BlackSea:
			case CoastRegion.Mediterranean:
				return true;
			}
			return false;
		case CoastRegion.SouthChinaSea:
		case CoastRegion.SouthwestPacific:
			return region2 - CoastRegion.BalticSea <= 1 || region2 - CoastRegion.Mediterranean <= 1;
		}
		return false;
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0002D094 File Offset: 0x0002B294
	public static bool PanamaRoute(CoastRegion region1, CoastRegion region2, bool arcticOpen)
	{
		if (region1 == region2)
		{
			return false;
		}
		switch (region1)
		{
		case CoastRegion.Caribbean:
			switch (region2)
			{
			case CoastRegion.NortheastPacific:
			case CoastRegion.NorthwestPacific:
			case CoastRegion.SouthChinaSea:
			case CoastRegion.SoutheastPacific:
			case CoastRegion.SouthwestPacific:
				return true;
			}
			return false;
		case CoastRegion.NortheastAtlantic:
			if (region2 != CoastRegion.NortheastPacific)
			{
				return region2 == CoastRegion.SoutheastPacific;
			}
			return !arcticOpen;
		case CoastRegion.NortheastPacific:
			if (region2 != CoastRegion.Caribbean)
			{
				switch (region2)
				{
				case CoastRegion.NortheastAtlantic:
				case CoastRegion.NorthwestAtlantic:
					return !arcticOpen;
				case CoastRegion.SouthAtlantic:
					return true;
				}
				return false;
			}
			return true;
		case CoastRegion.NorthwestAtlantic:
			switch (region2)
			{
			case CoastRegion.NortheastPacific:
			case CoastRegion.NorthwestPacific:
			case CoastRegion.SouthChinaSea:
				return !arcticOpen;
			case CoastRegion.SoutheastPacific:
			case CoastRegion.SouthwestPacific:
				return true;
			}
			return false;
		case CoastRegion.NorthwestPacific:
			if (region2 != CoastRegion.Caribbean)
			{
				if (region2 == CoastRegion.NorthwestAtlantic)
				{
					return !arcticOpen;
				}
				if (region2 != CoastRegion.SouthAtlantic)
				{
					return false;
				}
			}
			return true;
		case CoastRegion.SouthAtlantic:
			return region2 == CoastRegion.NortheastPacific || region2 == CoastRegion.NorthwestPacific;
		case CoastRegion.SouthChinaSea:
		case CoastRegion.SouthwestPacific:
			return region2 == CoastRegion.Caribbean || region2 == CoastRegion.NorthwestAtlantic;
		case CoastRegion.SoutheastPacific:
			return region2 == CoastRegion.Caribbean || region2 == CoastRegion.NortheastAtlantic || region2 == CoastRegion.NorthwestAtlantic;
		}
		return false;
	}

	// Token: 0x04000782 RID: 1922
	public TerrainType terrain = TerrainType.Standard;

	// Token: 0x04000783 RID: 1923
	public SupraRegion supraRegion;

	// Token: 0x04000784 RID: 1924
	public CoastRegion coast;

	// Token: 0x04000785 RID: 1925
	public bool island;

	// Token: 0x04000786 RID: 1926
	public float latitude;

	// Token: 0x04000787 RID: 1927
	public float longitude;

	// Token: 0x04000788 RID: 1928
	public float boostLatitude;

	// Token: 0x04000789 RID: 1929
	public string solarBody = "Earth";

	// Token: 0x0400078A RID: 1930
	public bool verticalRegion;

	// Token: 0x0400078B RID: 1931
	public float area_km2;

	// Token: 0x0400078C RID: 1932
	public int visualId;

	// Token: 0x0400078D RID: 1933
	public string parent;

	// Token: 0x0400078E RID: 1934
	public int oilId;

	// Token: 0x0400078F RID: 1935
	private static readonly Dictionary<CoastRegion, Dictionary<CoastRegion, float>> _defaultSeaTravelModifiers = new Dictionary<CoastRegion, Dictionary<CoastRegion, float>>
	{
		{
			CoastRegion.Arctic,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1f
				},
				{
					CoastRegion.BalticSea,
					4.5f
				},
				{
					CoastRegion.BlackSea,
					3f
				},
				{
					CoastRegion.Caribbean,
					1.1f
				},
				{
					CoastRegion.Indian,
					3.5f
				},
				{
					CoastRegion.Mediterranean,
					3f
				},
				{
					CoastRegion.NortheastAtlantic,
					1.1f
				},
				{
					CoastRegion.NortheastPacific,
					1.1f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.1f
				},
				{
					CoastRegion.NorthwestPacific,
					1.1f
				},
				{
					CoastRegion.SouthAtlantic,
					1.1f
				},
				{
					CoastRegion.SouthChinaSea,
					1.25f
				},
				{
					CoastRegion.SoutheastPacific,
					1.1f
				},
				{
					CoastRegion.SouthwestPacific,
					1.1f
				}
			}
		},
		{
			CoastRegion.BalticSea,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					4.5f
				},
				{
					CoastRegion.BalticSea,
					1f
				},
				{
					CoastRegion.BlackSea,
					6.5f
				},
				{
					CoastRegion.Caribbean,
					1.5f
				},
				{
					CoastRegion.Indian,
					2.65f
				},
				{
					CoastRegion.Mediterranean,
					3.5f
				},
				{
					CoastRegion.NortheastAtlantic,
					1.05f
				},
				{
					CoastRegion.NortheastPacific,
					1.5f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.25f
				},
				{
					CoastRegion.NorthwestPacific,
					3f
				},
				{
					CoastRegion.SouthAtlantic,
					1.3f
				},
				{
					CoastRegion.SouthChinaSea,
					3f
				},
				{
					CoastRegion.SoutheastPacific,
					1.5f
				},
				{
					CoastRegion.SouthwestPacific,
					1.9f
				}
			}
		},
		{
			CoastRegion.BlackSea,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					3f
				},
				{
					CoastRegion.BalticSea,
					6.5f
				},
				{
					CoastRegion.BlackSea,
					1f
				},
				{
					CoastRegion.Caribbean,
					1.31f
				},
				{
					CoastRegion.Indian,
					1.8f
				},
				{
					CoastRegion.Mediterranean,
					1.5f
				},
				{
					CoastRegion.NortheastAtlantic,
					3.8f
				},
				{
					CoastRegion.NortheastPacific,
					2f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.4f
				},
				{
					CoastRegion.NorthwestPacific,
					2.4f
				},
				{
					CoastRegion.SouthAtlantic,
					1.8f
				},
				{
					CoastRegion.SouthChinaSea,
					2.15f
				},
				{
					CoastRegion.SoutheastPacific,
					1.5f
				},
				{
					CoastRegion.SouthwestPacific,
					1.4f
				}
			}
		},
		{
			CoastRegion.Caribbean,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.5f
				},
				{
					CoastRegion.BlackSea,
					1.31f
				},
				{
					CoastRegion.Caribbean,
					1f
				},
				{
					CoastRegion.Indian,
					1.25f
				},
				{
					CoastRegion.Mediterranean,
					1.15f
				},
				{
					CoastRegion.NortheastAtlantic,
					1f
				},
				{
					CoastRegion.NortheastPacific,
					2.25f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1f
				},
				{
					CoastRegion.NorthwestPacific,
					1.75f
				},
				{
					CoastRegion.SouthAtlantic,
					1f
				},
				{
					CoastRegion.SouthChinaSea,
					1.75f
				},
				{
					CoastRegion.SoutheastPacific,
					1.5f
				},
				{
					CoastRegion.SouthwestPacific,
					1.2f
				}
			}
		},
		{
			CoastRegion.Indian,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					3.5f
				},
				{
					CoastRegion.BalticSea,
					2.65f
				},
				{
					CoastRegion.BlackSea,
					1.8f
				},
				{
					CoastRegion.Caribbean,
					1.25f
				},
				{
					CoastRegion.Indian,
					1f
				},
				{
					CoastRegion.Mediterranean,
					1.5f
				},
				{
					CoastRegion.NortheastAtlantic,
					2f
				},
				{
					CoastRegion.NortheastPacific,
					1.33f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.25f
				},
				{
					CoastRegion.NorthwestPacific,
					1.5f
				},
				{
					CoastRegion.SouthAtlantic,
					1.3f
				},
				{
					CoastRegion.SouthChinaSea,
					1.9f
				},
				{
					CoastRegion.SoutheastPacific,
					1.7f
				},
				{
					CoastRegion.SouthwestPacific,
					1.25f
				}
			}
		},
		{
			CoastRegion.Mediterranean,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					3f
				},
				{
					CoastRegion.BalticSea,
					3.5f
				},
				{
					CoastRegion.BlackSea,
					1.5f
				},
				{
					CoastRegion.Caribbean,
					1.15f
				},
				{
					CoastRegion.Indian,
					1.5f
				},
				{
					CoastRegion.Mediterranean,
					1f
				},
				{
					CoastRegion.NortheastAtlantic,
					2f
				},
				{
					CoastRegion.NortheastPacific,
					1.25f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.4f
				},
				{
					CoastRegion.NorthwestPacific,
					1.7f
				},
				{
					CoastRegion.SouthAtlantic,
					1.3f
				},
				{
					CoastRegion.SouthChinaSea,
					1.75f
				},
				{
					CoastRegion.SoutheastPacific,
					1.5f
				},
				{
					CoastRegion.SouthwestPacific,
					1.2f
				}
			}
		},
		{
			CoastRegion.NortheastAtlantic,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.05f
				},
				{
					CoastRegion.BlackSea,
					3.8f
				},
				{
					CoastRegion.Caribbean,
					1f
				},
				{
					CoastRegion.Indian,
					2f
				},
				{
					CoastRegion.Mediterranean,
					2f
				},
				{
					CoastRegion.NortheastAtlantic,
					1f
				},
				{
					CoastRegion.NortheastPacific,
					1.5f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1f
				},
				{
					CoastRegion.NorthwestPacific,
					2.25f
				},
				{
					CoastRegion.SouthAtlantic,
					1f
				},
				{
					CoastRegion.SouthChinaSea,
					2f
				},
				{
					CoastRegion.SoutheastPacific,
					1.5f
				},
				{
					CoastRegion.SouthwestPacific,
					1.5f
				}
			}
		},
		{
			CoastRegion.NortheastPacific,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.5f
				},
				{
					CoastRegion.BlackSea,
					2f
				},
				{
					CoastRegion.Caribbean,
					2.25f
				},
				{
					CoastRegion.Indian,
					1.33f
				},
				{
					CoastRegion.Mediterranean,
					1.25f
				},
				{
					CoastRegion.NortheastAtlantic,
					1.5f
				},
				{
					CoastRegion.NortheastPacific,
					1f
				},
				{
					CoastRegion.NorthwestAtlantic,
					2.7f
				},
				{
					CoastRegion.NorthwestPacific,
					1f
				},
				{
					CoastRegion.SouthAtlantic,
					1.6f
				},
				{
					CoastRegion.SouthChinaSea,
					1.08f
				},
				{
					CoastRegion.SoutheastPacific,
					1f
				},
				{
					CoastRegion.SouthwestPacific,
					1f
				}
			}
		},
		{
			CoastRegion.NorthwestAtlantic,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.25f
				},
				{
					CoastRegion.BlackSea,
					1.4f
				},
				{
					CoastRegion.Caribbean,
					1f
				},
				{
					CoastRegion.Indian,
					1.25f
				},
				{
					CoastRegion.Mediterranean,
					1.4f
				},
				{
					CoastRegion.NortheastAtlantic,
					1f
				},
				{
					CoastRegion.NortheastPacific,
					2.7f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1f
				},
				{
					CoastRegion.NorthwestPacific,
					1.9f
				},
				{
					CoastRegion.SouthAtlantic,
					1.25f
				},
				{
					CoastRegion.SouthChinaSea,
					1.6f
				},
				{
					CoastRegion.SoutheastPacific,
					1.1f
				},
				{
					CoastRegion.SouthwestPacific,
					1.3f
				}
			}
		},
		{
			CoastRegion.NorthwestPacific,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					3f
				},
				{
					CoastRegion.BlackSea,
					2.4f
				},
				{
					CoastRegion.Caribbean,
					1.75f
				},
				{
					CoastRegion.Indian,
					1.5f
				},
				{
					CoastRegion.Mediterranean,
					1.7f
				},
				{
					CoastRegion.NortheastAtlantic,
					2.25f
				},
				{
					CoastRegion.NortheastPacific,
					1f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.9f
				},
				{
					CoastRegion.NorthwestPacific,
					1f
				},
				{
					CoastRegion.SouthAtlantic,
					1.5f
				},
				{
					CoastRegion.SouthChinaSea,
					1.1f
				},
				{
					CoastRegion.SoutheastPacific,
					1f
				},
				{
					CoastRegion.SouthwestPacific,
					1f
				}
			}
		},
		{
			CoastRegion.SouthAtlantic,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.3f
				},
				{
					CoastRegion.BlackSea,
					1.8f
				},
				{
					CoastRegion.Caribbean,
					1f
				},
				{
					CoastRegion.Indian,
					1.3f
				},
				{
					CoastRegion.Mediterranean,
					1.3f
				},
				{
					CoastRegion.NortheastAtlantic,
					1f
				},
				{
					CoastRegion.NortheastPacific,
					1.6f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.25f
				},
				{
					CoastRegion.NorthwestPacific,
					1.5f
				},
				{
					CoastRegion.SouthAtlantic,
					1f
				},
				{
					CoastRegion.SouthChinaSea,
					1.25f
				},
				{
					CoastRegion.SoutheastPacific,
					3.5f
				},
				{
					CoastRegion.SouthwestPacific,
					1.5f
				}
			}
		},
		{
			CoastRegion.SouthChinaSea,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.25f
				},
				{
					CoastRegion.BalticSea,
					3f
				},
				{
					CoastRegion.BlackSea,
					2.15f
				},
				{
					CoastRegion.Caribbean,
					1.75f
				},
				{
					CoastRegion.Indian,
					1.9f
				},
				{
					CoastRegion.Mediterranean,
					1.75f
				},
				{
					CoastRegion.NortheastAtlantic,
					2f
				},
				{
					CoastRegion.NortheastPacific,
					1.08f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.6f
				},
				{
					CoastRegion.NorthwestPacific,
					1.1f
				},
				{
					CoastRegion.SouthAtlantic,
					1.25f
				},
				{
					CoastRegion.SouthChinaSea,
					1f
				},
				{
					CoastRegion.SoutheastPacific,
					1.15f
				},
				{
					CoastRegion.SouthwestPacific,
					1.1f
				}
			}
		},
		{
			CoastRegion.SoutheastPacific,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.5f
				},
				{
					CoastRegion.BlackSea,
					1.5f
				},
				{
					CoastRegion.Caribbean,
					1.5f
				},
				{
					CoastRegion.Indian,
					1.7f
				},
				{
					CoastRegion.Mediterranean,
					1.5f
				},
				{
					CoastRegion.NortheastAtlantic,
					1.5f
				},
				{
					CoastRegion.NortheastPacific,
					1f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.1f
				},
				{
					CoastRegion.NorthwestPacific,
					1f
				},
				{
					CoastRegion.SouthAtlantic,
					3.5f
				},
				{
					CoastRegion.SouthChinaSea,
					1.15f
				},
				{
					CoastRegion.SoutheastPacific,
					1f
				},
				{
					CoastRegion.SouthwestPacific,
					1f
				}
			}
		},
		{
			CoastRegion.SouthwestPacific,
			new Dictionary<CoastRegion, float>
			{
				{
					CoastRegion.Arctic,
					1.1f
				},
				{
					CoastRegion.BalticSea,
					1.9f
				},
				{
					CoastRegion.BlackSea,
					1.4f
				},
				{
					CoastRegion.Caribbean,
					1.2f
				},
				{
					CoastRegion.Indian,
					1.25f
				},
				{
					CoastRegion.Mediterranean,
					1.2f
				},
				{
					CoastRegion.NortheastAtlantic,
					1.5f
				},
				{
					CoastRegion.NortheastPacific,
					1f
				},
				{
					CoastRegion.NorthwestAtlantic,
					1.3f
				},
				{
					CoastRegion.NorthwestPacific,
					1f
				},
				{
					CoastRegion.SouthAtlantic,
					1.5f
				},
				{
					CoastRegion.SouthChinaSea,
					1.1f
				},
				{
					CoastRegion.SoutheastPacific,
					1f
				},
				{
					CoastRegion.SouthwestPacific,
					1f
				}
			}
		}
	};

	// Token: 0x04000790 RID: 1936
	public const float ClosedSuezDelay = 1.6f;

	// Token: 0x04000791 RID: 1937
	public const float ClosedPanamaDelay = 1.4f;
}
