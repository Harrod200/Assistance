using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000550 RID: 1360
	public class CanalMarkerController : SingleMarkerController
	{
		// Token: 0x0600231B RID: 8987 RVA: 0x000B7E67 File Offset: 0x000B6067
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			this.UpdateMarker();
		}

		// Token: 0x0600231C RID: 8988 RVA: 0x000B7E78 File Offset: 0x000B6078
		private string CanalTip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<TINationState> list = new List<TINationState>();
			string text = string.Empty;
			if (TIGlobalValuesState.GlobalValues.SuezRegion == base.region)
			{
				stringBuilder.AppendLine(Loc.T("UI.Markers.SuezCanal"));
				list = (from x in GameStateManager.AllExtantNations()
					where !TIRegionState.SuezAccess(x)
					select x).ToList<TINationState>();
				text = Loc.T("UI.Markers.SuezCanalExplainer", new object[]
				{
					TIGlobalValuesState.GlobalValues.SuezRegion.displayName,
					TIGlobalValuesState.GlobalValues.SuezRegion.nation.displayName
				});
			}
			else if (TIGlobalValuesState.GlobalValues.TurkishStraitRegion == base.region)
			{
				stringBuilder.AppendLine(Loc.T("UI.Markers.TurkishStraits"));
				list = (from x in GameStateManager.AllExtantNations()
					where !TIRegionState.TurkishStraitAccess(x)
					select x).ToList<TINationState>();
				text = Loc.T("UI.Markers.TurkishStraitsExplainer", new object[]
				{
					TIGlobalValuesState.GlobalValues.TurkishStraitRegion.displayName,
					TIGlobalValuesState.GlobalValues.TurkishStraitRegion.nation.displayName
				});
			}
			else if (TIGlobalValuesState.GlobalValues.PanamaRegion == base.region)
			{
				stringBuilder.AppendLine(Loc.T("UI.Markers.PanamaCanal"));
				list = (from x in GameStateManager.AllExtantNations()
					where !TIRegionState.PanamaAccess(x)
					select x).ToList<TINationState>();
				text = Loc.T("UI.Markers.PanamaCanalExplainer", new object[]
				{
					TIGlobalValuesState.GlobalValues.PanamaRegion.displayName,
					TIGlobalValuesState.GlobalValues.PanamaRegion.nation.displayName
				});
			}
			stringBuilder.AppendLine(text);
			if (list.Count > 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(Loc.T("UI.Markers.CanalPenaltySubjectNations"));
				foreach (TINationState tinationState in list)
				{
					stringBuilder.AppendLine(tinationState.displayName);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600231D RID: 8989 RVA: 0x000B80CC File Offset: 0x000B62CC
		public override void UpdateMarker()
		{
			if (TIGlobalValuesState.GlobalValues.SuezRegion == base.region || TIGlobalValuesState.GlobalValues.TurkishStraitRegion == base.region || TIGlobalValuesState.GlobalValues.PanamaRegion == base.region)
			{
				this.canalController = base.container.ManageMarkerStack(this.canalController, false, MarkerType.Canal, base.region, "Canal", -1, false);
				this.canalController.SetCentralIcon("mapicons/ICO_geoscape_strait");
				this.canalController.SetTooltip(() => this.CanalTip());
				return;
			}
			base.gameObject.SetActive(false);
			base.container.gameObject.SetActive(false);
		}

		// Token: 0x04001A8F RID: 6799
		private MarkerController canalController;
	}
}
