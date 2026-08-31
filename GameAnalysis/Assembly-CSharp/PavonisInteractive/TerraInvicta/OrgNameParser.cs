using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000705 RID: 1797
	public class OrgNameParser : INamelistParser<OrgName>
	{
		// Token: 0x06002A87 RID: 10887 RVA: 0x000E6B80 File Offset: 0x000E4D80
		public OrgName ParseKey(string[] values)
		{
			return new OrgName(values[0].ToEnum(OrgType.Any), values[1]);
		}

		// Token: 0x06002A88 RID: 10888 RVA: 0x000E6B94 File Offset: 0x000E4D94
		public NamelistEntry ParseEntry(string[] values)
		{
			int num;
			if (!int.TryParse(values[2], out num))
			{
				Error.Log("Weight value is not an int: " + values[2], Array.Empty<object>());
				num = 1;
			}
			TILocalizationTemplate currentLocalizationTemplate = Loc.currentLocalizationTemplate;
			if (currentLocalizationTemplate != null && values.Length > currentLocalizationTemplate.nameListOffset + 3)
			{
				string text = values[3 + currentLocalizationTemplate.nameListOffset];
				if (!string.IsNullOrEmpty(text))
				{
					return new NamelistEntry(text, num);
				}
			}
			return new NamelistEntry(values[3], num);
		}
	}
}
