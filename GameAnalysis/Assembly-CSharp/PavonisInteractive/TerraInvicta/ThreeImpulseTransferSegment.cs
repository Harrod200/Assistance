using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200079D RID: 1949
	public class ThreeImpulseTransferSegment : IPatchedTransferSegment
	{
		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06003E8C RID: 16012 RVA: 0x00195397 File Offset: 0x00193597
		public TIDateTime startTime
		{
			get
			{
				return this.burn0_startTime;
			}
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06003E8D RID: 16013 RVA: 0x0019539F File Offset: 0x0019359F
		public TIDateTime endTime
		{
			get
			{
				return this.burn2_endTime;
			}
		}

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06003E8E RID: 16014 RVA: 0x001953A7 File Offset: 0x001935A7
		// (set) Token: 0x06003E8F RID: 16015 RVA: 0x001953AF File Offset: 0x001935AF
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x06003E90 RID: 16016 RVA: 0x001953B8 File Offset: 0x001935B8
		public double DV_mps
		{
			get
			{
				return this.burn0_DV_mps + this.burn1_DV_mps + this.burn2_DV_mps;
			}
		}

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x06003E91 RID: 16017 RVA: 0x001953CE File Offset: 0x001935CE
		public double burn0_duration_s
		{
			get
			{
				return this.burn0_DV_mps / this.fleetAcceleration_mps2;
			}
		}

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x06003E92 RID: 16018 RVA: 0x001953DD File Offset: 0x001935DD
		public double burn1_duration_s
		{
			get
			{
				return this.burn1_DV_mps / this.fleetAcceleration_mps2;
			}
		}

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06003E93 RID: 16019 RVA: 0x001953EC File Offset: 0x001935EC
		public double burn2_duration_s
		{
			get
			{
				return this.burn2_DV_mps / this.fleetAcceleration_mps2;
			}
		}

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06003E94 RID: 16020 RVA: 0x001953FB File Offset: 0x001935FB
		public TIDateTime burn0_startTime
		{
			get
			{
				return new TIDateTime(this.burn0_midTime, -this.burn0_duration_s * 0.5);
			}
		}

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x06003E95 RID: 16021 RVA: 0x00195419 File Offset: 0x00193619
		public TIDateTime burn1_startTime
		{
			get
			{
				return new TIDateTime(this.burn1_midTime, -this.burn1_duration_s * 0.5);
			}
		}

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x06003E96 RID: 16022 RVA: 0x00195437 File Offset: 0x00193637
		public TIDateTime burn2_startTime
		{
			get
			{
				return new TIDateTime(this.burn2_midTime, -this.burn2_duration_s * 0.5);
			}
		}

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06003E97 RID: 16023 RVA: 0x00195455 File Offset: 0x00193655
		public TIDateTime burn0_endTime
		{
			get
			{
				return new TIDateTime(this.burn0_midTime, this.burn0_duration_s * 0.5);
			}
		}

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06003E98 RID: 16024 RVA: 0x00195472 File Offset: 0x00193672
		public TIDateTime burn1_endTime
		{
			get
			{
				return new TIDateTime(this.burn1_midTime, this.burn1_duration_s * 0.5);
			}
		}

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06003E99 RID: 16025 RVA: 0x0019548F File Offset: 0x0019368F
		public TIDateTime burn2_endTime
		{
			get
			{
				return new TIDateTime(this.burn2_midTime, this.burn2_duration_s * 0.5);
			}
		}

		// Token: 0x040026F7 RID: 9975
		public double fleetAcceleration_mps2;

		// Token: 0x040026F8 RID: 9976
		public double burn0_DV_mps;

		// Token: 0x040026F9 RID: 9977
		public double burn1_DV_mps;

		// Token: 0x040026FA RID: 9978
		public double burn2_DV_mps;

		// Token: 0x040026FB RID: 9979
		public TIDateTime burn0_midTime;

		// Token: 0x040026FC RID: 9980
		public TIDateTime burn1_midTime;

		// Token: 0x040026FD RID: 9981
		public TIDateTime burn2_midTime;

		// Token: 0x040026FE RID: 9982
		public OrbitalElementsState orbit01;

		// Token: 0x040026FF RID: 9983
		public OrbitalElementsState orbit12;
	}
}
