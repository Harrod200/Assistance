using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200093F RID: 2367
	public class NotADecision : HabSchematicDecision
	{
		// Token: 0x17000F54 RID: 3924
		// (get) Token: 0x06005A95 RID: 23189 RVA: 0x002B31F6 File Offset: 0x002B13F6
		// (set) Token: 0x06005A96 RID: 23190 RVA: 0x002B31FE File Offset: 0x002B13FE
		public TIHabModuleTemplate HabModuleTemplate { get; private set; }

		// Token: 0x17000F55 RID: 3925
		// (get) Token: 0x06005A97 RID: 23191 RVA: 0x002B3207 File Offset: 0x002B1407
		// (set) Token: 0x06005A98 RID: 23192 RVA: 0x002B320F File Offset: 0x002B140F
		public bool CheckForValidity { get; private set; }

		// Token: 0x06005A99 RID: 23193 RVA: 0x002B3218 File Offset: 0x002B1418
		public NotADecision(TIHabModuleTemplate habModuleTemplate, bool checkForValidity)
		{
			this.HabModuleTemplate = habModuleTemplate;
			this.CheckForValidity = checkForValidity;
		}

		// Token: 0x06005A9A RID: 23194 RVA: 0x002B3230 File Offset: 0x002B1430
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			IEnumerable<TIHabModuleTemplate> enumerable = Enumerable.Empty<TIHabModuleTemplate>().Append(this.HabModuleTemplate);
			if (this.CheckForValidity)
			{
				enumerable = enumerable.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => HabSchematicDecision.IsValidModule(faction, location, x, order));
			}
			return enumerable;
		}
	}
}
