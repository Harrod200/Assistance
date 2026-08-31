using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000703 RID: 1795
	public class SpaceAssetNameParser : INamelistParser<SpaceAssetName>
	{
		// Token: 0x06002A81 RID: 10881 RVA: 0x000E6AEE File Offset: 0x000E4CEE
		public SpaceAssetName ParseKey(string[] values)
		{
			return new SpaceAssetName(values[0], values[1]);
		}

		// Token: 0x06002A82 RID: 10882 RVA: 0x000E6AFC File Offset: 0x000E4CFC
		public NamelistEntry ParseEntry(string[] values)
		{
			TILocalizationTemplate currentLocalizationTemplate = Loc.currentLocalizationTemplate;
			if (currentLocalizationTemplate != null && values.Length > currentLocalizationTemplate.nameListOffset + 2)
			{
				string text = values[2 + currentLocalizationTemplate.nameListOffset];
				if (!string.IsNullOrEmpty(text))
				{
					return new NamelistEntry(text, 1);
				}
			}
			return new NamelistEntry(values[2], 1);
		}
	}
}
