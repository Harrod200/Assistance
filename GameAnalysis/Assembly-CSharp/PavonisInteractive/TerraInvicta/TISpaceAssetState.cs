using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007AE RID: 1966
	public abstract class TISpaceAssetState : TISpaceObjectState, ITransferTarget
	{
		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x060041E5 RID: 16869 RVA: 0x001AA6F7 File Offset: 0x001A88F7
		// (set) Token: 0x060041E6 RID: 16870 RVA: 0x001AA6FF File Offset: 0x001A88FF
		public TIFactionState faction { get; protected set; }

		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x060041E7 RID: 16871 RVA: 0x001AA708 File Offset: 0x001A8908
		// (set) Token: 0x060041E8 RID: 16872 RVA: 0x001AA710 File Offset: 0x001A8910
		public TIOrbitState orbitState { get; protected set; }

		// Token: 0x060041E9 RID: 16873
		public abstract bool IsAlien();

		// Token: 0x060041EA RID: 16874
		public abstract float CombatRange_km();

		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x060041EB RID: 16875 RVA: 0x001AA719 File Offset: 0x001A8919
		public override bool isSpaceAssetState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x060041EC RID: 16876 RVA: 0x001AA71C File Offset: 0x001A891C
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x060041ED RID: 16877 RVA: 0x001AA71F File Offset: 0x001A891F
		public override double semiMajorAxis_m
		{
			get
			{
				return this.orbitState.semiMajorAxis_m;
			}
		}

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x060041EE RID: 16878 RVA: 0x001AA72C File Offset: 0x001A892C
		public override double ecc
		{
			get
			{
				return this.orbitState.eccentricity;
			}
		}

		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x060041EF RID: 16879 RVA: 0x001AA739 File Offset: 0x001A8939
		public override double inclination_Rad
		{
			get
			{
				return this.orbitState.inclination_Rad;
			}
		}

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x060041F0 RID: 16880 RVA: 0x001AA746 File Offset: 0x001A8946
		public override double longAscendingNode_Rad
		{
			get
			{
				return this.orbitState.longitudeAscendingNode_Rad;
			}
		}

		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x060041F1 RID: 16881 RVA: 0x001AA753 File Offset: 0x001A8953
		public override double argPeriapsis_Rad
		{
			get
			{
				return this.orbitState.argPeriapsis_Rad;
			}
		}

		// Token: 0x17000C49 RID: 3145
		// (get) Token: 0x060041F2 RID: 16882 RVA: 0x001AA760 File Offset: 0x001A8960
		public override double meanAnomalyAtEpoch_Rad
		{
			get
			{
				return this._meanAnomalyAtEpoch_Rad;
			}
		}

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x060041F3 RID: 16883 RVA: 0x001AA768 File Offset: 0x001A8968
		public override double epoch_JYears
		{
			get
			{
				return this._epoch_JYears;
			}
		}

		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x060041F4 RID: 16884 RVA: 0x001AA770 File Offset: 0x001A8970
		public override double meanLongitude_Rad
		{
			get
			{
				return this.meanAnomalyAtEpoch_Rad + this.longAscendingNode_Rad + this.argPeriapsis_Rad;
			}
		}

		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x060041F5 RID: 16885 RVA: 0x001AA786 File Offset: 0x001A8986
		public virtual TISpaceGameState location
		{
			get
			{
				return this.orbitState;
			}
		}

		// Token: 0x060041F6 RID: 16886
		public abstract float SpaceCombatValue();

		// Token: 0x060041F7 RID: 16887
		public abstract float AssaultCombatValue(bool defense);

		// Token: 0x060041F8 RID: 16888 RVA: 0x001AA78E File Offset: 0x001A898E
		TIGameState ITransferTarget.selfState()
		{
			return this;
		}

		// Token: 0x060041F9 RID: 16889 RVA: 0x001AA791 File Offset: 0x001A8991
		TINaturalSpaceObjectState ITransferTarget.barycenter()
		{
			return this.barycenter;
		}

		// Token: 0x060041FA RID: 16890 RVA: 0x001AA799 File Offset: 0x001A8999
		TINaturalSpaceObjectState ITransferTarget.barycenterBarycenter()
		{
			return this.barycenter.barycenter;
		}

		// Token: 0x060041FB RID: 16891 RVA: 0x001AA7A6 File Offset: 0x001A89A6
		TINaturalSpaceObjectState ITransferTarget.barycenterBarycenterBarycenter()
		{
			TINaturalSpaceObjectState barycenter = this.barycenter.barycenter;
			if (barycenter == null)
			{
				return null;
			}
			return barycenter.barycenter;
		}

		// Token: 0x060041FC RID: 16892 RVA: 0x001AA7BE File Offset: 0x001A89BE
		double ITransferTarget.a_m()
		{
			return this.semiMajorAxis_m;
		}

		// Token: 0x060041FD RID: 16893 RVA: 0x001AA7C6 File Offset: 0x001A89C6
		double ITransferTarget.e()
		{
			return this.ecc;
		}

		// Token: 0x060041FE RID: 16894 RVA: 0x001AA7CE File Offset: 0x001A89CE
		double ITransferTarget.i_rad()
		{
			return this.inclination_Rad;
		}

		// Token: 0x060041FF RID: 16895 RVA: 0x001AA7D6 File Offset: 0x001A89D6
		double ITransferTarget.Ω_rad()
		{
			return this.longAscendingNode_Rad;
		}

		// Token: 0x06004200 RID: 16896 RVA: 0x001AA7DE File Offset: 0x001A89DE
		double ITransferTarget.ω_rad()
		{
			return this.argPeriapsis_Rad;
		}

		// Token: 0x06004201 RID: 16897 RVA: 0x001AA7E6 File Offset: 0x001A89E6
		double ITransferTarget.M0_rad()
		{
			return this.meanAnomalyAtEpoch_Rad;
		}

		// Token: 0x06004202 RID: 16898 RVA: 0x001AA7EE File Offset: 0x001A89EE
		double ITransferTarget.t0_jy()
		{
			return this.epoch_JYears;
		}

		// Token: 0x06004203 RID: 16899 RVA: 0x001AA7F6 File Offset: 0x001A89F6
		double ITransferTarget.L0_rad()
		{
			return this.meanLongitude_Rad;
		}

		// Token: 0x06004204 RID: 16900 RVA: 0x001AA7FE File Offset: 0x001A89FE
		double ITransferTarget.μ()
		{
			return base.mu;
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x001AA806 File Offset: 0x001A8A06
		double ITransferTarget.period_days()
		{
			return this.orbitalPeriod_s / 86400.0;
		}

		// Token: 0x06004206 RID: 16902 RVA: 0x001AA818 File Offset: 0x001A8A18
		Vector3d ITransferTarget.globalPositionValue(TISpaceFleetState fleet, TIDateTime time)
		{
			return this.GetGlobalPositionAtTime(time);
		}

		// Token: 0x06004207 RID: 16903 RVA: 0x001AA821 File Offset: 0x001A8A21
		Vector3 ITransferTarget.visualizationPositionValue()
		{
			return base.controller.transform.position;
		}

		// Token: 0x06004208 RID: 16904 RVA: 0x001AA834 File Offset: 0x001A8A34
		public virtual double common_a_m(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.semiMajorAxis_m;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.semiMajorAxis_m;
			}
			if (this.barycenter.barycenter == null)
			{
				string[] array = new string[6];
				array[0] = "No semimajor axis around a non-common barycenter.  This = ";
				array[1] = this.displayName;
				array[2] = ", this.barycenter = ";
				int num = 3;
				TINaturalSpaceObjectState barycenter = this.barycenter;
				array[num] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[4] = ", commonBarycenter = ";
				array[5] = ((commonBarycenter != null) ? commonBarycenter.displayName : null) ?? "null";
				Log.Error(string.Concat(array), Array.Empty<object>());
				return -1.0;
			}
			TINaturalSpaceObjectState barycenter2 = this.barycenter.barycenter;
			if (commonBarycenter == ((barycenter2 != null) ? barycenter2.barycenter : null))
			{
				return this.barycenter.barycenter.semiMajorAxis_m;
			}
			Log.Error("Can't find semimajor axis for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004209 RID: 16905 RVA: 0x001AA948 File Offset: 0x001A8B48
		double ITransferTarget.common_e(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.ecc;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.ecc;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.ecc;
			}
			Log.Error("Can't find ecc for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600420A RID: 16906 RVA: 0x001AA9CC File Offset: 0x001A8BCC
		public virtual double common_i_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.inclination_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.inclination_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.inclination_Rad;
			}
			Log.Error("Can't find i for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600420B RID: 16907 RVA: 0x001AAA50 File Offset: 0x001A8C50
		double ITransferTarget.common_Ω_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.longAscendingNode_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.longAscendingNode_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.longAscendingNode_Rad;
			}
			Log.Error("Can't find i for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600420C RID: 16908 RVA: 0x001AAAD4 File Offset: 0x001A8CD4
		double ITransferTarget.common_ω_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.argPeriapsis_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.argPeriapsis_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.argPeriapsis_Rad;
			}
			Log.Error("Can't find ω for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600420D RID: 16909 RVA: 0x001AAB58 File Offset: 0x001A8D58
		double ITransferTarget.common_M0_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.meanAnomalyAtEpoch_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.meanAnomalyAtEpoch_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.meanAnomalyAtEpoch_Rad;
			}
			Log.Error("Can't find M0 for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600420E RID: 16910 RVA: 0x001AABDC File Offset: 0x001A8DDC
		public virtual double common_M_rad(TINaturalSpaceObjectState commonBarycenter, TIDateTime time)
		{
			if (commonBarycenter == this.barycenter)
			{
				return base.meanAnomaly_Rad(time);
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.meanAnomaly_Rad(time);
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.meanAnomaly_Rad(time);
			}
			Log.Error("Can't find M for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x001AAC64 File Offset: 0x001A8E64
		double ITransferTarget.common_L0_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.meanLongitude_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.meanLongitude_Rad;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.meanLongitude_Rad;
			}
			Log.Error("Can't find M0 for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004210 RID: 16912 RVA: 0x001AACE8 File Offset: 0x001A8EE8
		double ITransferTarget.common_t0_jy(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.epoch_JYears;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.epoch_JYears;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.epoch_JYears;
			}
			Log.Error("Can't find epoch for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004211 RID: 16913 RVA: 0x001AAD6C File Offset: 0x001A8F6C
		double ITransferTarget.common_μ(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return base.mu;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.mu;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.mu;
			}
			Log.Error("Can't find epoch for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004212 RID: 16914 RVA: 0x001AADF0 File Offset: 0x001A8FF0
		double ITransferTarget.common_period_days(TINaturalSpaceObjectState commonBarycenter)
		{
			if (commonBarycenter == this.barycenter)
			{
				return this.orbitalPeriod_s / 86400.0;
			}
			if (commonBarycenter == this.barycenter.barycenter)
			{
				return this.barycenter.orbitalPeriod_s / 86400.0;
			}
			if (commonBarycenter == this.barycenter.barycenter.barycenter)
			{
				return this.barycenter.barycenter.orbitalPeriod_s / 86400.0;
			}
			Log.Error("Can't find period value for commonbarycenter", Array.Empty<object>());
			return -1.0;
		}

		// Token: 0x06004213 RID: 16915 RVA: 0x001AAE90 File Offset: 0x001A9090
		public double relevant_orbit_m(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter == commonBarycenter)
			{
				return this.semiMajorAxis_m;
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.semiMajorAxis_m;
			}
			return this.barycenter.barycenter.semiMajorAxis_m;
		}

		// Token: 0x06004214 RID: 16916 RVA: 0x001AAEE4 File Offset: 0x001A90E4
		CartesianState ITransferTarget.relevantGlobalCartesianState(TINaturalSpaceObjectState commonBarycenter, TIDateTime dateTime)
		{
			if (this.barycenter == commonBarycenter)
			{
				return this.ToGlobalCartesianStateAtTime(dateTime);
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.ToGlobalCartesianStateAtTime(dateTime);
			}
			return this.barycenter.barycenter.ToGlobalCartesianStateAtTime(dateTime);
		}

		// Token: 0x06004215 RID: 16917 RVA: 0x001AAF38 File Offset: 0x001A9138
		double ITransferTarget.relevant_escapeVelocity_mps(TINaturalSpaceObjectState commonBarycenter)
		{
			if (this.barycenter == commonBarycenter)
			{
				return 0.0;
			}
			if (this.barycenter.barycenter == commonBarycenter)
			{
				return this.barycenter.localEscapeVelocity_mps(this.relevant_orbit_m(commonBarycenter));
			}
			return this.barycenter.barycenter.localEscapeVelocity_mps(this.relevant_orbit_m(commonBarycenter));
		}

		// Token: 0x06004216 RID: 16918 RVA: 0x001AAF9A File Offset: 0x001A919A
		public virtual CartesianState? tryToGetGlobalCartesianState(TIDateTime time)
		{
			return new CartesianState?(this.ToGlobalCartesianStateAtTime(time));
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x001AAFA8 File Offset: 0x001A91A8
		public virtual bool tryToGetLocalCartesianState(TIDateTime time, out CartesianState cartesianState, out TINaturalSpaceObjectState barycenter)
		{
			cartesianState = this.ToLocalCartesianStateAtTime(time);
			barycenter = this.barycenter;
			return true;
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x001AAFC0 File Offset: 0x001A91C0
		public virtual TINaturalSpaceObjectState localBarycenter(TIDateTime time)
		{
			return this.barycenter;
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x001AAFC8 File Offset: 0x001A91C8
		public virtual void getOrbitalElementsState(TIDateTime time, out OrbitalElementsState orbitalElementsState, out TINaturalSpaceObjectState barycenter, out bool meanAnomalyIsGood)
		{
			barycenter = this.barycenter;
			orbitalElementsState = new OrbitalElementsState(this, this.meanAnomalyAtEpoch_Rad, base.epoch_DateTime);
			meanAnomalyIsGood = true;
		}

		// Token: 0x0600421A RID: 16922 RVA: 0x001AAFEE File Offset: 0x001A91EE
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.gameTime == null)
			{
				this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			}
			base.PostGlobalGameStateCreateInit_2();
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x001AB00E File Offset: 0x001A920E
		public override void SetDisplayName(string newName)
		{
			if (this.displayName != newName)
			{
				this.displayName = newName;
			}
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x001AB028 File Offset: 0x001A9228
		public static string GetRandomAssetName(TIGameState asset, TIFactionState faction)
		{
			TISpaceShipState tispaceShipState = asset as TISpaceShipState;
			TIHabState tihabState = asset as TIHabState;
			string returnName = string.Empty;
			List<TIMapRegionTemplate> list = new List<TIMapRegionTemplate>();
			if (faction != null && !faction.IsAlienFaction)
			{
				list = (from x in faction.executiveNations.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions)
					select x.mapRegionTemplate).ToList<TIMapRegionTemplate>();
			}
			string text;
			if (asset.isSpaceShipState)
			{
				if (tispaceShipState.hull.largeHull || tispaceShipState.hull.hugeHull)
				{
					text = ((faction.scenarioCustomizations.usingCustomizations && faction.scenarioCustomizations.customFactionText.ContainsKey(faction.templateName) && !string.IsNullOrEmpty(faction.scenarioCustomizations.customFactionText[faction.templateName].customLargeShipNameListIdx)) ? faction.scenarioCustomizations.customFactionText[faction.templateName].customLargeShipNameListIdx : faction.template.largeShipNameListIdx);
				}
				else if (tispaceShipState.hull.smallHull)
				{
					text = ((faction.scenarioCustomizations.usingCustomizations && faction.scenarioCustomizations.customFactionText.ContainsKey(faction.templateName) && !string.IsNullOrEmpty(faction.scenarioCustomizations.customFactionText[faction.templateName].customSmallShipNameListIdx)) ? faction.scenarioCustomizations.customFactionText[faction.templateName].customSmallShipNameListIdx : faction.template.smallShipNameListIdx);
				}
				else
				{
					text = ((faction.scenarioCustomizations.usingCustomizations && faction.scenarioCustomizations.customFactionText.ContainsKey(faction.templateName) && !string.IsNullOrEmpty(faction.scenarioCustomizations.customFactionText[faction.templateName].customMediumShipNameListIdx)) ? faction.scenarioCustomizations.customFactionText[faction.templateName].customMediumShipNameListIdx : faction.template.mediumShipNameListIdx);
				}
			}
			else
			{
				text = ((faction.scenarioCustomizations.usingCustomizations && faction.scenarioCustomizations.customFactionText.ContainsKey(faction.templateName) && !string.IsNullOrEmpty(faction.scenarioCustomizations.customFactionText[faction.templateName].customHabNameListIdx)) ? faction.scenarioCustomizations.customFactionText[faction.templateName].customHabNameListIdx : faction.template.habNameListIdx);
			}
			bool flag = true;
			int num = 0;
			string text2 = null;
			Func<TISpaceShipState, bool> <>9__3;
			Func<TIHabState, bool> <>9__5;
			while (flag && num < 100)
			{
				flag = false;
				TIMapRegionTemplate timapRegionTemplate;
				if ((float)num < 33.3f && list.Count > 0)
				{
					timapRegionTemplate = list.SelectRandomItem<TIMapRegionTemplate>();
				}
				else if (num % 2 == 0)
				{
					timapRegionTemplate = null;
				}
				else
				{
					timapRegionTemplate = TemplateManager.IterateByClass<TIMapRegionTemplate>(true).SelectRandomItem<TIMapRegionTemplate>();
				}
				string text3 = ((timapRegionTemplate != null) ? timapRegionTemplate.dataName : null) ?? string.Empty;
				SpaceAssetName spaceAssetName = new SpaceAssetName(text, text3);
				if (GameControl.namelists.TryGetName<SpaceAssetName>(spaceAssetName, out returnName))
				{
					if (asset.isSpaceShipState)
					{
						using (IEnumerator<TISpaceShipState> enumerator = GameStateManager.IterateByClass<TISpaceShipState>(false).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.displayName == returnName)
								{
									if ((float)num < 66.7f)
									{
										flag = true;
										break;
									}
									IEnumerable<TISpaceShipState> enumerable = GameStateManager.IterateByClass<TISpaceShipState>(false);
									Func<TISpaceShipState, bool> func;
									if ((func = <>9__3) == null)
									{
										func = (<>9__3 = delegate(TISpaceShipState x)
										{
											if (x.faction == faction)
											{
												string displayName = x.displayName;
												return displayName != null && displayName.StartsWith(returnName);
											}
											return false;
										});
									}
									text2 = TISpaceAssetState.<GetRandomAssetName>g__GetNextNumberForMultiAsset|79_2((from x in enumerable.Where<TISpaceShipState>(func)
										select x.displayName).ToList<string>()).ToString();
									break;
								}
							}
							goto IL_0585;
						}
						goto IL_04B7;
					}
					goto IL_04B7;
					IL_0585:
					num++;
					continue;
					IL_04B7:
					foreach (TIHabState tihabState2 in GameStateManager.IterateByClass<TIHabState>(false))
					{
						if (tihabState2.displayName != null && tihabState2.displayName.Contains(returnName))
						{
							if ((float)num < 66.7f)
							{
								flag = true;
								break;
							}
							IEnumerable<TIHabState> enumerable2 = GameStateManager.IterateByClass<TIHabState>(false);
							Func<TIHabState, bool> func2;
							if ((func2 = <>9__5) == null)
							{
								func2 = (<>9__5 = delegate(TIHabState x)
								{
									if (x.faction == faction)
									{
										string displayName2 = x.displayName;
										return displayName2 != null && displayName2.StartsWith(returnName);
									}
									return false;
								});
							}
							text2 = TISpaceAssetState.<GetRandomAssetName>g__GetNextNumberForMultiAsset|79_2((from x in enumerable2.Where<TIHabState>(func2)
								select x.displayName).ToList<string>()).ToString();
							break;
						}
					}
					goto IL_0585;
				}
				if (timapRegionTemplate != null)
				{
					list.Remove(timapRegionTemplate);
				}
				num++;
				flag = true;
			}
			if (tihabState != null)
			{
				if (tihabState.IsStation)
				{
					returnName = Loc.T("UI.Habs.StationName", new object[] { returnName });
				}
				else
				{
					returnName = Loc.T("UI.Habs.BaseName", new object[] { returnName });
				}
				if (text2 != null)
				{
					returnName = new StringBuilder(returnName).Append(" ").Append(text2).ToString();
				}
			}
			return returnName;
		}

		// Token: 0x0600421D RID: 16925 RVA: 0x001AB66C File Offset: 0x001A986C
		protected void SetNewOrbitalElements(OrbitalElementsState orbitalElements)
		{
			this.SetNewOrbitalElements(orbitalElements.meanAnomalyAtEpoch_Rad, new TIDateTime(orbitalElements.epoch));
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x001AB688 File Offset: 0x001A9888
		protected void SetNewOrbitalElements(double meanAnomalyAtEpoch_Rad, TIDateTime epoch)
		{
			this._meanAnomalyAtEpoch_Rad = meanAnomalyAtEpoch_Rad;
			base.epoch_DateTime = epoch;
			this._epoch_JYears = epoch.ToJulianDate();
			if (base.controller != null)
			{
				base.controller.UpdateOrbitComponentForAsset(false);
			}
			GameControl.eventManager.TriggerEvent(new OrbitChangedEvent(this), null, new object[] { this });
		}

		// Token: 0x0600421F RID: 16927 RVA: 0x001AB6E4 File Offset: 0x001A98E4
		private void AssumeOrbit(TIOrbitState orbitState)
		{
			TIOrbitState orbitState2 = this.orbitState;
			if (orbitState2 != null)
			{
				orbitState2.assetsInOrbit.Remove(this);
			}
			this.orbitState = orbitState;
			orbitState.assetsInOrbit.AddUnique(this);
			this.barycenter = orbitState.barycenter;
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x001AB720 File Offset: 0x001A9920
		public void AssumeMatchingOrbitFromState(TISpaceAssetState matchingAsset, bool docking)
		{
			this.AssumeOrbit(matchingAsset.orbitState);
			if (!this.orbitState.isAdHocOrbit)
			{
				this.SetNewOrbitalElements(this.orbitState.template.Generate(false, this.orbitState.TestAndCorrectAnomalyToAvoidOverlap(this, matchingAsset.meanAnomalyAtEpoch_Rad, docking, false), matchingAsset.epoch_DateTime));
				return;
			}
			this.SetNewOrbitalElements(matchingAsset.meanAnomalyAtEpoch_Rad, matchingAsset.epoch_DateTime);
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x001AB78C File Offset: 0x001A998C
		public void AssumeOrbitFromState(TIOrbitState orbitState, double meanAnomalyAtEpoch_Rad = 0.0, TIDateTime epoch = null)
		{
			if (epoch == null)
			{
				epoch = TITimeState.Now();
			}
			this.AssumeOrbit(orbitState);
			meanAnomalyAtEpoch_Rad = orbitState.TestAndCorrectAnomalyToAvoidOverlap(this, meanAnomalyAtEpoch_Rad, false, false);
			if (!orbitState.isAdHocOrbit)
			{
				this.SetNewOrbitalElements(orbitState.template.Generate(false, meanAnomalyAtEpoch_Rad, epoch));
				return;
			}
			this.SetNewOrbitalElements(meanAnomalyAtEpoch_Rad, epoch);
		}

		// Token: 0x06004222 RID: 16930 RVA: 0x001AB7E1 File Offset: 0x001A99E1
		public void AssumeOrbitStateGivenMeanAnomalyAtEpoch(TIOrbitState newOrbit, TIDateTime epoch, double meanAnomalyAtEpoch_Rad)
		{
			this.AssumeOrbit(newOrbit);
			if (!this.orbitState.isAdHocOrbit)
			{
				this.SetNewOrbitalElements(this.orbitState.template.Generate(false, meanAnomalyAtEpoch_Rad, epoch));
				return;
			}
			this.SetNewOrbitalElements(meanAnomalyAtEpoch_Rad, epoch);
		}

		// Token: 0x06004223 RID: 16931 RVA: 0x001AB81C File Offset: 0x001A9A1C
		public void AssumeOrbitStateFromPosition(TIOrbitState newOrbit, Vector3d globalPosition, Vector3d barycenterPosition, TIDateTime time, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			OrbitalElementsState orbitalElementsState = newOrbit.ToOrbitalElementsState(time, 0.0);
			Vector3d vector3d = globalPosition - newOrbit.barycenter.GetGlobalPositionAtTime(time);
			vector3d = (Quaterniond.Inverse(newOrbit.barycenter.SpatialRotation) * vector3d.xzy).xzy;
			double num = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, newOrbit.barycenter, vector3d, time, precision);
			this.AssumeOrbitStateGivenMeanAnomalyAtEpoch(newOrbit, time, num);
		}

		// Token: 0x06004224 RID: 16932 RVA: 0x001AB890 File Offset: 0x001A9A90
		public static double CalculateMeanAnomalyFromPosition(ITransferTarget transferTarget, Vector3d localPosition, TIDateTime time, bool isPlayer)
		{
			TISpaceAssetState.MeanAnomalyPrecision meanAnomalyPrecision = (isPlayer ? TISpaceAssetState.MeanAnomalyPrecision.Player : TISpaceAssetState.MeanAnomalyPrecision.AI);
			return TISpaceAssetState.CalculateMeanAnomalyFromPosition(transferTarget, localPosition, time, meanAnomalyPrecision);
		}

		// Token: 0x06004225 RID: 16933 RVA: 0x001AB8B0 File Offset: 0x001A9AB0
		public static double CalculateMeanAnomalyFromPosition(ITransferTarget transferTarget, Vector3d localPosition, TIDateTime time, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			TISpaceFleetState tispaceFleetState = transferTarget as TISpaceFleetState;
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime < time)
			{
				orbitalElementsState = tispaceFleetState.trajectory.GetOrbitalElementsAtTime(time);
				tinaturalSpaceObjectState = tispaceFleetState.trajectory.GetBarycenterAtTime(time);
			}
			else
			{
				orbitalElementsState = new OrbitalElementsState
				{
					epoch = time.ExportTime(),
					semiMajorAxis_m = transferTarget.a_m(),
					eccentricity = transferTarget.e(),
					inclination_Rad = transferTarget.i_rad(),
					longAscendingNode_Rad = transferTarget.Ω_rad(),
					argPeriapsis_Rad = transferTarget.ω_rad(),
					meanAnomalyAtEpoch_Rad = 0.0
				};
				tinaturalSpaceObjectState = transferTarget.barycenter();
			}
			return TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, tinaturalSpaceObjectState, localPosition, time, precision);
		}

		// Token: 0x06004226 RID: 16934 RVA: 0x001AB978 File Offset: 0x001A9B78
		public static double CalculateMeanAnomalyFromPosition(OrbitalElementsState orbit, TINaturalSpaceObjectState barycenter, Vector3d localPosition, TIDateTime time, bool isPlayer)
		{
			TISpaceAssetState.MeanAnomalyPrecision meanAnomalyPrecision = (isPlayer ? TISpaceAssetState.MeanAnomalyPrecision.Player : TISpaceAssetState.MeanAnomalyPrecision.AI);
			return TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbit, barycenter, localPosition, time, meanAnomalyPrecision);
		}

		// Token: 0x06004227 RID: 16935 RVA: 0x001AB998 File Offset: 0x001A9B98
		public static double CalculateMeanAnomalyFromPosition(OrbitalElementsState orbit, TINaturalSpaceObjectState barycenter, Vector3d localPosition, TIDateTime time, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			Vector3d vector3d = Vector3d.Flatten(localPosition, orbit.normalVector);
			if (orbit.eccentricity == 0.0)
			{
				Vector3d normalized = vector3d.normalized;
				Vector3d vector3d2 = orbit.ascendingNodeVector;
				double num = Mathd.Acos(Vector3d.Dot(in vector3d2, in normalized));
				vector3d2 = orbit.normalVector;
				Vector3d vector3d3 = Vector3d.Cross(orbit.ascendingNodeVector, normalized);
				if (Vector3d.Dot(in vector3d2, in vector3d3) < 0.0)
				{
					num = -num;
				}
				return Mathd.ClampRadiansTwoPI(num - orbit.argPeriapsis_Rad);
			}
			if (orbit.eccentricity < 1.0)
			{
				Vector3d normalVector = orbit.normalVector;
				Vector3d vector3d4 = orbit.PeriapsisDirection();
				Vector3d vector3d5 = Vector3d.Cross(normalVector, vector3d4);
				Vector3d vector3d2 = orbit.PeriapsisDirection();
				Vector3d vector3d3 = vector3d.normalized;
				double num2 = Mathd.Acos(Vector3d.Dot(in vector3d2, in vector3d3));
				if (Vector3d.Dot(in vector3d, in vector3d5) < 0.0)
				{
					num2 = -num2;
				}
				double eccentricAnomalyFromTrueAnomaly = orbit.GetEccentricAnomalyFromTrueAnomaly(num2);
				return Mathd.ClampRadiansTwoPI(orbit.GetMeanAnomalyFromEccentricAnomaly(eccentricAnomalyFromTrueAnomaly));
			}
			Log.Error("Attempting to find mean anomaly from position when 'orbit' is hyperbolic.", Array.Empty<object>());
			return 0.0;
		}

		// Token: 0x06004228 RID: 16936 RVA: 0x001ABAB8 File Offset: 0x001A9CB8
		private static double ScoreMeanAnomaly(OrbitalElementsState orbit, TINaturalSpaceObjectState barycenter, Vector3d targetPosition, double meanAnomalyAtEpoch_Rad, TIDateTime time)
		{
			orbit.meanAnomalyAtEpoch_Rad = meanAnomalyAtEpoch_Rad;
			Vector3d position = orbit.ToCartesianStateAtTime(time.ExportTime(), barycenter.mass_kg).position;
			return Vector3d.Dot(in targetPosition, in position) / position.magnitude;
		}

		// Token: 0x06004229 RID: 16937 RVA: 0x001ABAF8 File Offset: 0x001A9CF8
		public void SetRandomizedOrbitFromState(TIOrbitState orbitState, bool variableAxisAndInclination = true)
		{
			this.AssumeOrbit(orbitState);
			this.SetNewOrbitalElements(orbitState.template.Generate(variableAxisAndInclination, true));
		}

		// Token: 0x0600422A RID: 16938 RVA: 0x001ABB14 File Offset: 0x001A9D14
		public bool VisibleToFaction(TIFactionState faction)
		{
			return !base.deleted && faction.HasIntelOnSpaceAssetLocation(this);
		}

		// Token: 0x0600422B RID: 16939 RVA: 0x001ABB27 File Offset: 0x001A9D27
		public bool UndercoverCouncilorsVisibleToFaction(TIFactionState faction)
		{
			return faction.HasIntelOnUndercoverCouncilorsInSpaceAsset(this);
		}

		// Token: 0x0600422C RID: 16940 RVA: 0x001ABB30 File Offset: 0x001A9D30
		public virtual TINaturalSpaceObjectState GetSphereOfInfluence(bool exact = false)
		{
			return this.orbitState.barycenter;
		}

		// Token: 0x0600422D RID: 16941 RVA: 0x001ABB3D File Offset: 0x001A9D3D
		public bool SamePlanetarySystem(TISpaceObjectState spaceObject)
		{
			return this.GetSunOrbitingRelatedObject == spaceObject.GetSunOrbitingRelatedObject;
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x001ABB50 File Offset: 0x001A9D50
		public float IntelOnCreation(TIFactionState detectingFaction, TIFactionState owningFaction)
		{
			if (detectingFaction == owningFaction)
			{
				if (owningFaction.IsAlienFaction)
				{
					return TemplateManager.global.alienMySpaceAssetBaselineIntel;
				}
				return TemplateManager.global.humanMySpaceAssetBaselineIntel;
			}
			else
			{
				if (detectingFaction.IsActiveHumanFaction && owningFaction.IsAlienFaction)
				{
					return this.BaselineIntelOnAlienAsset(detectingFaction);
				}
				if (detectingFaction.IsActiveHumanFaction && owningFaction.IsActiveHumanFaction)
				{
					return TemplateManager.global.humanSpaceAssetBaselineIntel;
				}
				return 0f;
			}
		}

		// Token: 0x0600422F RID: 16943 RVA: 0x001ABBBC File Offset: 0x001A9DBC
		public float BaselineIntelOnAlienAsset(TIFactionState detectingFaction)
		{
			if (detectingFaction.IsAlienFaction)
			{
				return 1f;
			}
			if (this.isSpaceFleetState && this.ref_fleet.ships.Count == 0)
			{
				return 0f;
			}
			if (detectingFaction.fullSpaceVisibility)
			{
				return TemplateManager.global.intelToSeeSpaceAssetLocationandComposition;
			}
			double num = TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(this, GameStateManager.Sol());
			double getAlienDetectionRange_m = detectingFaction.GetAlienDetectionRange_m;
			if (num <= getAlienDetectionRange_m)
			{
				return TemplateManager.global.intelToSeeSpaceAssetLocationandComposition;
			}
			return TemplateManager.global.alienSpaceAssetBaselineIntel;
		}

		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x06004230 RID: 16944 RVA: 0x001ABC34 File Offset: 0x001A9E34
		public double localEscapeVelocity_kps
		{
			get
			{
				return Mathd.Sqrt(1.334768E-10 * this.barycenter.mass_kg / this.semiMajorAxis_m) / 1000.0;
			}
		}

		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x06004231 RID: 16945 RVA: 0x001ABC61 File Offset: 0x001A9E61
		public double localGravity
		{
			get
			{
				return 6.67384E-11 * this.barycenter.mass_kg / (this.semiMajorAxis_m * this.semiMajorAxis_m);
			}
		}

		// Token: 0x06004232 RID: 16946
		public abstract List<TISpaceFleetState> GetNearbyIdleAlliedFleets(TIDateTime time = null);

		// Token: 0x06004234 RID: 16948 RVA: 0x001ABC90 File Offset: 0x001A9E90
		[CompilerGenerated]
		internal static int <GetRandomAssetName>g__GetNextNumberForMultiAsset|79_2(List<string> duplicateNames)
		{
			int num = 1;
			foreach (string text in duplicateNames)
			{
				string text2 = string.Concat<char>(text.Reverse<char>().TakeWhile<char>(new Func<char, bool>(char.IsNumber)).Reverse<char>());
				if (text2.Length > 0)
				{
					int num2 = int.Parse(text2);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num + 1;
		}

		// Token: 0x040027B8 RID: 10168
		private const int MEAN_ANOMALY_PRECISION_AI = 5;

		// Token: 0x040027B9 RID: 10169
		private const int MEAN_ANOMALY_PRECISION_PLAYER = 8;

		// Token: 0x040027BA RID: 10170
		private const int MEAN_ANOMALY_PRECISION_MAXIMUM = 12;

		// Token: 0x040027BD RID: 10173
		public bool inCombat;

		// Token: 0x040027BE RID: 10174
		[SerializeField]
		protected double _meanAnomalyAtEpoch_Rad;

		// Token: 0x040027BF RID: 10175
		[SerializeField]
		protected double _epoch_JYears;

		// Token: 0x040027C0 RID: 10176
		public const float spaceAssetModelScale_Hab = 525f;

		// Token: 0x040027C1 RID: 10177
		public const float spaceAssetModelScale_Ship = 525f;

		// Token: 0x02000F1E RID: 3870
		public enum MeanAnomalyPrecision
		{
			// Token: 0x04005C58 RID: 23640
			AI,
			// Token: 0x04005C59 RID: 23641
			Player,
			// Token: 0x04005C5A RID: 23642
			Maximum
		}
	}
}
