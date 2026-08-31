using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200078E RID: 1934
	public interface ITransferTarget
	{
		// Token: 0x06003D98 RID: 15768
		TIGameState selfState();

		// Token: 0x06003D99 RID: 15769
		TINaturalSpaceObjectState barycenter();

		// Token: 0x06003D9A RID: 15770
		TINaturalSpaceObjectState barycenterBarycenter();

		// Token: 0x06003D9B RID: 15771
		TINaturalSpaceObjectState barycenterBarycenterBarycenter();

		// Token: 0x06003D9C RID: 15772
		double a_m();

		// Token: 0x06003D9D RID: 15773
		double e();

		// Token: 0x06003D9E RID: 15774
		double i_rad();

		// Token: 0x06003D9F RID: 15775
		double Ω_rad();

		// Token: 0x06003DA0 RID: 15776
		double ω_rad();

		// Token: 0x06003DA1 RID: 15777
		double M0_rad();

		// Token: 0x06003DA2 RID: 15778
		double L0_rad();

		// Token: 0x06003DA3 RID: 15779
		double t0_jy();

		// Token: 0x06003DA4 RID: 15780
		double μ();

		// Token: 0x06003DA5 RID: 15781
		double period_days();

		// Token: 0x06003DA6 RID: 15782
		Vector3d globalPositionValue(TISpaceFleetState forFleet, TIDateTime time);

		// Token: 0x06003DA7 RID: 15783
		Vector3 visualizationPositionValue();

		// Token: 0x06003DA8 RID: 15784
		double common_a_m(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DA9 RID: 15785
		double common_e(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DAA RID: 15786
		double common_i_rad(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DAB RID: 15787
		double common_Ω_rad(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DAC RID: 15788
		double common_ω_rad(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DAD RID: 15789
		double common_M0_rad(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DAE RID: 15790
		double common_M_rad(TINaturalSpaceObjectState commonBarycenter, TIDateTime time);

		// Token: 0x06003DAF RID: 15791
		double common_L0_rad(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DB0 RID: 15792
		double common_t0_jy(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DB1 RID: 15793
		double common_μ(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DB2 RID: 15794
		double common_period_days(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DB3 RID: 15795
		double relevant_orbit_m(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DB4 RID: 15796
		double relevant_escapeVelocity_mps(TINaturalSpaceObjectState commonBarycenter);

		// Token: 0x06003DB5 RID: 15797
		CartesianState relevantGlobalCartesianState(TINaturalSpaceObjectState commonBarycenter, TIDateTime time);

		// Token: 0x06003DB6 RID: 15798
		CartesianState? tryToGetGlobalCartesianState(TIDateTime time);

		// Token: 0x06003DB7 RID: 15799
		bool tryToGetLocalCartesianState(TIDateTime time, out CartesianState cartesianState, out TINaturalSpaceObjectState barycenter);

		// Token: 0x06003DB8 RID: 15800
		TINaturalSpaceObjectState localBarycenter(TIDateTime time);

		// Token: 0x06003DB9 RID: 15801
		void getOrbitalElementsState(TIDateTime time, out OrbitalElementsState orbitalElementsState, out TINaturalSpaceObjectState barycenter, out bool meanAnomalyIsGood);
	}
}
