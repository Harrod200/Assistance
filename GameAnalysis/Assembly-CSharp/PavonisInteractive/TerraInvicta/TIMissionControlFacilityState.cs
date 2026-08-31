using System;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000775 RID: 1909
	public class TIMissionControlFacilityState : TIRegionSpaceFacilityState
	{
		// Token: 0x06003A11 RID: 14865 RVA: 0x001566D0 File Offset: 0x001548D0
		public override string GetDisplayName(TIFactionState faction)
		{
			string text = new StringBuilder("TIRegionTemplate.MissionControlName.").Append(base.region.template.localizationName).ToString();
			string text2 = Loc.T(text);
			if (text2 == string.Empty || text2 == text)
			{
				text2 = Loc.T("TIRegionTemplate.MissionControlName.Generic", new object[] { base.region.displayName });
			}
			return text2;
		}

		// Token: 0x06003A12 RID: 14866 RVA: 0x0015673F File Offset: 0x0015493F
		public override float GetAIValuation()
		{
			return (float)base.region.missionControl;
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06003A13 RID: 14867 RVA: 0x0015674D File Offset: 0x0015494D
		public override string descriptor
		{
			get
			{
				return Loc.T("UI.Nation.MissionControlFacility");
			}
		}

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06003A14 RID: 14868 RVA: 0x00156759 File Offset: 0x00154959
		public override string description
		{
			get
			{
				return Loc.T("UI.Nation.MissionControlDescription");
			}
		}

		// Token: 0x06003A15 RID: 14869 RVA: 0x00156765 File Offset: 0x00154965
		public override bool Extant()
		{
			return base.region.missionControl > 0;
		}

		// Token: 0x06003A16 RID: 14870 RVA: 0x00156775 File Offset: 0x00154975
		public override int GetSize()
		{
			if (base.region.missionControl >= 10)
			{
				return 3;
			}
			if (base.region.missionControl < 3)
			{
				return 1;
			}
			return 2;
		}

		// Token: 0x06003A17 RID: 14871 RVA: 0x0015679C File Offset: 0x0015499C
		public override Sprite GetIcon(TIFactionState faction)
		{
			switch (this.GetSize())
			{
			case 1:
				return AssetCacheManager.missionControlFacilitySmallIcon;
			case 3:
				return AssetCacheManager.missionControlFacilityLargeIcon;
			}
			return AssetCacheManager.missionControlFacilityMediumIcon;
		}

		// Token: 0x06003A18 RID: 14872 RVA: 0x001567D8 File Offset: 0x001549D8
		public override string GetIconResourcePath(TIFactionState faction)
		{
			switch (this.GetSize())
			{
			case 1:
				return TemplateManager.global.pathGeoscapeMissionControl1;
			case 3:
				return TemplateManager.global.pathGeoscapeMissionControl3;
			}
			return TemplateManager.global.pathGeoscapeMissionControl2;
		}

		// Token: 0x06003A19 RID: 14873 RVA: 0x00156824 File Offset: 0x00154A24
		public override string GetIllustrationPath(TIFactionState faction)
		{
			switch (this.GetSize())
			{
			default:
				return TemplateManager.global.illus_missionControlFacilitySmallPath;
			case 2:
				return TemplateManager.global.illus_missionControlFacilityMediumPath;
			case 3:
				return TemplateManager.global.illus_missionControlFacilityLargePath;
			}
		}
	}
}
