using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000701 RID: 1793
	public class CouncilorNameParser : INamelistParser<CouncilorName>
	{
		// Token: 0x06002A7B RID: 10875 RVA: 0x000E6A00 File Offset: 0x000E4C00
		public CouncilorName ParseKey(string[] values)
		{
			return new CouncilorName(values[0], values[1], values[2]);
		}

		// Token: 0x06002A7C RID: 10876 RVA: 0x000E6A10 File Offset: 0x000E4C10
		public NamelistEntry ParseEntry(string[] values)
		{
			int num;
			if (!int.TryParse(values[3], out num))
			{
				Error.Log("Weight value is not an int: " + values[3], Array.Empty<object>());
				num = 1;
			}
			TILocalizationTemplate currentLocalizationTemplate = Loc.currentLocalizationTemplate;
			if (currentLocalizationTemplate != null && values.Length > currentLocalizationTemplate.nameListOffset + 4)
			{
				string text = values[4 + currentLocalizationTemplate.nameListOffset];
				if (!string.IsNullOrEmpty(text))
				{
					return new NamelistEntry(text, num);
				}
			}
			return new NamelistEntry(values[4], num);
		}
	}
}
