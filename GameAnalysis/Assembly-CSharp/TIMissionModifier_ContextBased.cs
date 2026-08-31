using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F9 RID: 505
public abstract class TIMissionModifier_ContextBased : TIMissionModifier
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00021BD0 File Offset: 0x0001FDD0
	public override string displayName
	{
		get
		{
			List<TIEffectTemplate> factionEffectsForContext = TIEffectsState.GetFactionEffectsForContext(this.context, this.sourceFaction);
			if (factionEffectsForContext.Count > 0)
			{
				List<string> list = new List<string>(factionEffectsForContext.Count);
				foreach (TIEffectTemplate tieffectTemplate in factionEffectsForContext)
				{
					list.Add(tieffectTemplate.displayName);
				}
				list = list.Distinct<string>().ToList<string>();
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					stringBuilder.Append(list[i]);
					if (i < list.Count - 1)
					{
						stringBuilder.Append("/");
					}
					else
					{
						i++;
					}
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}
	}

	// Token: 0x0400061F RID: 1567
	public Context context;

	// Token: 0x04000620 RID: 1568
	public TIFactionState sourceFaction;
}
