using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A7 RID: 1959
	public class HabPreferences : Dictionary<HabMetric, float>
	{
		// Token: 0x060040C2 RID: 16578 RVA: 0x001A2B78 File Offset: 0x001A0D78
		public HabPreferences()
		{
			foreach (HabMetric habMetric in TIHabState.HabMetrics)
			{
				base[habMetric] = 1f;
			}
		}

		// Token: 0x17000BD4 RID: 3028
		public float this[FactionResource resource]
		{
			get
			{
				switch (resource)
				{
				case FactionResource.Money:
					return base[HabMetric.Money];
				case FactionResource.Influence:
					return base[HabMetric.Influence];
				case FactionResource.Operations:
					return base[HabMetric.Operations];
				case FactionResource.Research:
					return base[HabMetric.Research];
				case FactionResource.Boost:
					return base[HabMetric.Boost];
				case FactionResource.MissionControl:
					return base[HabMetric.MissionControl];
				case FactionResource.Water:
				case FactionResource.Volatiles:
				case FactionResource.Metals:
				case FactionResource.NobleMetals:
				case FactionResource.Fissiles:
					return base[HabMetric.SpaceResources];
				}
				return 1f;
			}
		}

		// Token: 0x17000BD5 RID: 3029
		// (get) Token: 0x060040C4 RID: 16580 RVA: 0x001A2C34 File Offset: 0x001A0E34
		// (set) Token: 0x060040C5 RID: 16581 RVA: 0x001A2C6F File Offset: 0x001A0E6F
		public float Weight
		{
			get
			{
				float num = Mathf.Sqrt((float)TIHabState.HabMetrics.Count<HabMetric>());
				return Mathf.Sqrt(TIHabState.HabMetrics.Sum<HabMetric>((HabMetric x) => Mathf.Pow(base[x], 2f))) / num;
			}
			set
			{
				this.Scale(value / this.Weight);
			}
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x001A2C7F File Offset: 0x001A0E7F
		public HabPreferences Normalized()
		{
			return this.Scaled(1f / this.Weight);
		}

		// Token: 0x060040C7 RID: 16583 RVA: 0x001A2C94 File Offset: 0x001A0E94
		public void Scale(float scalar)
		{
			foreach (HabMetric habMetric in TIHabState.HabMetrics)
			{
				base[habMetric] *= scalar;
			}
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x001A2CC9 File Offset: 0x001A0EC9
		public HabPreferences Scaled(float scalar)
		{
			HabPreferences habPreferences = this.Copy();
			habPreferences.Scale(scalar);
			return habPreferences;
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x001A2CD8 File Offset: 0x001A0ED8
		public HabPreferences Multiplied(HabPreferences other)
		{
			HabPreferences habPreferences = new HabPreferences();
			foreach (HabMetric habMetric in TIHabState.HabMetrics)
			{
				habPreferences[habMetric] = base[habMetric] * other[habMetric];
			}
			return habPreferences;
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x001A2D1C File Offset: 0x001A0F1C
		public HabPreferences Copy()
		{
			HabPreferences habPreferences = new HabPreferences();
			foreach (HabMetric habMetric in TIHabState.HabMetrics)
			{
				habPreferences[habMetric] = base[habMetric];
			}
			return habPreferences;
		}
	}
}
