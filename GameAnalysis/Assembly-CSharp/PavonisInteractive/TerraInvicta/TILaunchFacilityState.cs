using System;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000774 RID: 1908
	public class TILaunchFacilityState : TIRegionSpaceFacilityState
	{
		// Token: 0x06003A07 RID: 14855 RVA: 0x00156516 File Offset: 0x00154716
		public override float GetAIValuation()
		{
			return base.region.boostPerYear_dekatons + (float)base.region.numSTOFighters;
		}

		// Token: 0x06003A08 RID: 14856 RVA: 0x00156530 File Offset: 0x00154730
		public override string GetDisplayName(TIFactionState faction)
		{
			string text = new StringBuilder("TIRegionTemplate.BoostFacilityName.").Append(base.region.template.localizationName).ToString();
			string text2 = Loc.T(text);
			if (text2 == string.Empty || text2 == text)
			{
				text2 = Loc.T("TIRegionTemplate.BoostFacilityName.Generic", new object[] { base.region.displayName });
			}
			return text2;
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06003A09 RID: 14857 RVA: 0x0015659F File Offset: 0x0015479F
		public override string descriptor
		{
			get
			{
				return Loc.T("UI.Nation.LaunchFacility");
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06003A0A RID: 14858 RVA: 0x001565AB File Offset: 0x001547AB
		public override string description
		{
			get
			{
				return Loc.T("UI.Nation.LaunchDescription");
			}
		}

		// Token: 0x06003A0B RID: 14859 RVA: 0x001565B7 File Offset: 0x001547B7
		public override bool Extant()
		{
			return base.region.boostPerYear_dekatons > 0f;
		}

		// Token: 0x06003A0C RID: 14860 RVA: 0x001565CB File Offset: 0x001547CB
		public override int GetSize()
		{
			if (base.region.boostPerYear_dekatons >= 100f)
			{
				return 3;
			}
			if (base.region.boostPerYear_dekatons < 50f)
			{
				return 1;
			}
			return 2;
		}

		// Token: 0x06003A0D RID: 14861 RVA: 0x001565F8 File Offset: 0x001547F8
		public override Sprite GetIcon(TIFactionState faction)
		{
			switch (this.GetSize())
			{
			case 1:
				return AssetCacheManager.launchFacilitySmallIcon;
			case 3:
				return AssetCacheManager.launchFacilityLargeIcon;
			}
			return AssetCacheManager.launchFacilityMediumIcon;
		}

		// Token: 0x06003A0E RID: 14862 RVA: 0x00156634 File Offset: 0x00154834
		public override string GetIconResourcePath(TIFactionState faction)
		{
			switch (this.GetSize())
			{
			case 1:
				return TemplateManager.global.pathGeoscapeLaunchSite1;
			case 3:
				return TemplateManager.global.pathGeoscapeLaunchSite3;
			}
			return TemplateManager.global.pathGeoscapeLaunchSite2;
		}

		// Token: 0x06003A0F RID: 14863 RVA: 0x00156680 File Offset: 0x00154880
		public override string GetIllustrationPath(TIFactionState faction)
		{
			switch (this.GetSize())
			{
			default:
				return TemplateManager.global.illus_launchFacilitySmallPath;
			case 2:
				return TemplateManager.global.illus_launchFacilityMediumPath;
			case 3:
				return TemplateManager.global.illus_launchFacilityLargePath;
			}
		}
	}
}
