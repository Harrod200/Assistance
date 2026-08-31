using System;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007BD RID: 1981
	public abstract class TISpaceObjectState : TISpaceGameState, IGameStateVisualizer
	{
		// Token: 0x17000D26 RID: 3366
		// (get) Token: 0x0600449C RID: 17564 RVA: 0x001C0CF4 File Offset: 0x001BEEF4
		// (set) Token: 0x0600449D RID: 17565 RVA: 0x001C0CFC File Offset: 0x001BEEFC
		[SerializeField]
		public TIDateTime epoch_DateTime { get; protected set; }

		// Token: 0x17000D27 RID: 3367
		// (get) Token: 0x0600449E RID: 17566 RVA: 0x001C0D05 File Offset: 0x001BEF05
		// (set) Token: 0x0600449F RID: 17567 RVA: 0x001C0D0D File Offset: 0x001BEF0D
		[fsIgnore]
		public SpaceObjectController controller { get; protected set; }

		// Token: 0x17000D28 RID: 3368
		// (get) Token: 0x060044A0 RID: 17568 RVA: 0x001C0D16 File Offset: 0x001BEF16
		public override bool isSpaceObjectState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D29 RID: 3369
		// (get) Token: 0x060044A1 RID: 17569 RVA: 0x001C0D19 File Offset: 0x001BEF19
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000D2A RID: 3370
		// (get) Token: 0x060044A2 RID: 17570 RVA: 0x001C0D1C File Offset: 0x001BEF1C
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D2B RID: 3371
		// (get) Token: 0x060044A3 RID: 17571 RVA: 0x001C0D1F File Offset: 0x001BEF1F
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D2C RID: 3372
		// (get) Token: 0x060044A4 RID: 17572 RVA: 0x001C0D22 File Offset: 0x001BEF22
		public TISpaceObjectTemplate template
		{
			get
			{
				return this.GetMyTemplate<TISpaceObjectTemplate>();
			}
		}

		// Token: 0x17000D2D RID: 3373
		// (get) Token: 0x060044A5 RID: 17573 RVA: 0x001C0D2A File Offset: 0x001BEF2A
		public string activePlayerDisplayName
		{
			get
			{
				return this.GetDisplayName(GameControl.control.activePlayer);
			}
		}

		// Token: 0x17000D2E RID: 3374
		// (get) Token: 0x060044A6 RID: 17574 RVA: 0x001C0D3C File Offset: 0x001BEF3C
		public virtual string modelResource
		{
			get
			{
				return this.template.ModelResource;
			}
		}

		// Token: 0x17000D2F RID: 3375
		// (get) Token: 0x060044A7 RID: 17575 RVA: 0x001C0D49 File Offset: 0x001BEF49
		public virtual float modelScale
		{
			get
			{
				return this.template.ModelScale;
			}
		}

		// Token: 0x17000D30 RID: 3376
		// (get) Token: 0x060044A8 RID: 17576 RVA: 0x001C0D56 File Offset: 0x001BEF56
		public virtual double mass_kg
		{
			get
			{
				return this.template.Mass;
			}
		}

		// Token: 0x17000D31 RID: 3377
		// (get) Token: 0x060044A9 RID: 17577 RVA: 0x001C0D63 File Offset: 0x001BEF63
		public double mass_EarthMasses
		{
			get
			{
				return this.mass_kg / 5.97219E+24;
			}
		}

		// Token: 0x17000D32 RID: 3378
		// (get) Token: 0x060044AA RID: 17578 RVA: 0x001C0D75 File Offset: 0x001BEF75
		public double mu
		{
			get
			{
				return 6.67384E-11 * this.mass_kg;
			}
		}

		// Token: 0x17000D33 RID: 3379
		// (get) Token: 0x060044AB RID: 17579 RVA: 0x001C0D87 File Offset: 0x001BEF87
		public virtual SpaceObjectType objectType
		{
			get
			{
				return this.template.objectType;
			}
		}

		// Token: 0x17000D34 RID: 3380
		// (get) Token: 0x060044AC RID: 17580 RVA: 0x001C0D94 File Offset: 0x001BEF94
		public virtual string iconResource
		{
			get
			{
				return this.template.symbolTexture;
			}
		}

		// Token: 0x17000D35 RID: 3381
		// (get) Token: 0x060044AD RID: 17581 RVA: 0x001C0DA1 File Offset: 0x001BEFA1
		public virtual double meanRadius_km
		{
			get
			{
				return 0.0;
			}
		}

		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x060044AE RID: 17582 RVA: 0x001C0DAC File Offset: 0x001BEFAC
		public virtual double meanRadius_m
		{
			get
			{
				return 0.0;
			}
		}

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x060044AF RID: 17583 RVA: 0x001C0DB7 File Offset: 0x001BEFB7
		public virtual double semiMajorAxis_m
		{
			get
			{
				return this.template.SemiMajorAxis_m;
			}
		}

		// Token: 0x17000D38 RID: 3384
		// (get) Token: 0x060044B0 RID: 17584 RVA: 0x001C0DC4 File Offset: 0x001BEFC4
		public double semiMajorAxis_km
		{
			get
			{
				return this.semiMajorAxis_m / 1000.0;
			}
		}

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x060044B1 RID: 17585 RVA: 0x001C0DD6 File Offset: 0x001BEFD6
		public double semiMajorAxis_AU
		{
			get
			{
				return this.semiMajorAxis_m / 149597870700.0;
			}
		}

		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x060044B2 RID: 17586 RVA: 0x001C0DE8 File Offset: 0x001BEFE8
		public virtual double ecc
		{
			get
			{
				return this.template.Eccentricity;
			}
		}

		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x060044B3 RID: 17587 RVA: 0x001C0DF5 File Offset: 0x001BEFF5
		public virtual double inclination_Rad
		{
			get
			{
				return this.template.Inclination_Rad;
			}
		}

		// Token: 0x17000D3C RID: 3388
		// (get) Token: 0x060044B4 RID: 17588 RVA: 0x001C0E02 File Offset: 0x001BF002
		public virtual double longAscendingNode_Rad
		{
			get
			{
				return this.template.LongitudeAscendingNode_Rad;
			}
		}

		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x060044B5 RID: 17589 RVA: 0x001C0E0F File Offset: 0x001BF00F
		public virtual double argPeriapsis_Rad
		{
			get
			{
				return this.template.ArgumentPeriapsis_Rad;
			}
		}

		// Token: 0x17000D3E RID: 3390
		// (get) Token: 0x060044B6 RID: 17590 RVA: 0x001C0E1C File Offset: 0x001BF01C
		public virtual double meanAnomalyAtEpoch_Rad
		{
			get
			{
				return this.template.MeanAnomalyAtEpoch_Rad;
			}
		}

		// Token: 0x060044B7 RID: 17591 RVA: 0x001C0E29 File Offset: 0x001BF029
		public double meanAnomaly_Rad(TIDateTime time)
		{
			return this.meanAnomalyAtEpoch_Rad + Mathd.Sqrt(this.barycenter.mu / (this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m)) * time.DifferenceInSeconds(this.epoch_DateTime);
		}

		// Token: 0x060044B8 RID: 17592 RVA: 0x001C0E64 File Offset: 0x001BF064
		public double meanLongitudeAtTime_Rad(TIDateTime time)
		{
			return this.meanAnomaly_Rad(time) + this.longAscendingNode_Rad + this.argPeriapsis_Rad;
		}

		// Token: 0x17000D3F RID: 3391
		// (get) Token: 0x060044B9 RID: 17593 RVA: 0x001C0E7B File Offset: 0x001BF07B
		public virtual double meanLongitude_Rad
		{
			get
			{
				return this.template.MeanLongitude_Rad;
			}
		}

		// Token: 0x17000D40 RID: 3392
		// (get) Token: 0x060044BA RID: 17594 RVA: 0x001C0E88 File Offset: 0x001BF088
		public virtual double orbitalPeriod_s
		{
			get
			{
				if (!(this.barycenter != null))
				{
					return 1.0;
				}
				return 6.283185307179586 * Mathd.Sqrt(this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m / (6.67384E-11 * this.barycenter.mass_kg));
			}
		}

		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x060044BB RID: 17595 RVA: 0x001C0EE6 File Offset: 0x001BF0E6
		public double orbitalPeriod_Hours
		{
			get
			{
				return this.orbitalPeriod_s / 3600.0;
			}
		}

		// Token: 0x17000D42 RID: 3394
		// (get) Token: 0x060044BC RID: 17596 RVA: 0x001C0EF8 File Offset: 0x001BF0F8
		public double orbitalPeriod_Days
		{
			get
			{
				return this.orbitalPeriod_s / 86400.0;
			}
		}

		// Token: 0x17000D43 RID: 3395
		// (get) Token: 0x060044BD RID: 17597 RVA: 0x001C0F0A File Offset: 0x001BF10A
		public double orbitalPeriod_Years
		{
			get
			{
				return this.orbitalPeriod_s / 31556924.0;
			}
		}

		// Token: 0x17000D44 RID: 3396
		// (get) Token: 0x060044BE RID: 17598 RVA: 0x001C0F1C File Offset: 0x001BF11C
		public double meanVelocity_mps
		{
			get
			{
				return 6.283185307179586 * this.semiMajorAxis_m / this.orbitalPeriod_s;
			}
		}

		// Token: 0x17000D45 RID: 3397
		// (get) Token: 0x060044BF RID: 17599 RVA: 0x001C0F35 File Offset: 0x001BF135
		public double velocity_mps
		{
			get
			{
				return this.GetVelocityAtTime(TITimeState.Now());
			}
		}

		// Token: 0x17000D46 RID: 3398
		// (get) Token: 0x060044C0 RID: 17600 RVA: 0x001C0F42 File Offset: 0x001BF142
		public double apsidalPrecession_Years
		{
			get
			{
				return this.template.apsidalPrecession_Years.GetValueOrDefault();
			}
		}

		// Token: 0x17000D47 RID: 3399
		// (get) Token: 0x060044C1 RID: 17601 RVA: 0x001C0F54 File Offset: 0x001BF154
		public double nodalPrecession_Years
		{
			get
			{
				return this.template.nodalPrecession_Years.GetValueOrDefault();
			}
		}

		// Token: 0x17000D48 RID: 3400
		// (get) Token: 0x060044C2 RID: 17602 RVA: 0x001C0F66 File Offset: 0x001BF166
		public virtual double epoch_JYears
		{
			get
			{
				return this.template.Epoch_floatJYears;
			}
		}

		// Token: 0x17000D49 RID: 3401
		// (get) Token: 0x060044C3 RID: 17603 RVA: 0x001C0F73 File Offset: 0x001BF173
		public virtual Quaterniond SpatialRotation
		{
			get
			{
				return Quaterniond.identity;
			}
		}

		// Token: 0x060044C4 RID: 17604 RVA: 0x001C0F7A File Offset: 0x001BF17A
		public virtual double GetSurfaceRotation_Rad(TIDateTime time)
		{
			return 0.0;
		}

		// Token: 0x17000D4A RID: 3402
		// (get) Token: 0x060044C5 RID: 17605 RVA: 0x001C0F88 File Offset: 0x001BF188
		public float longitude
		{
			get
			{
				if (this.barycenter == null || !(this.barycenter is TISpaceBodyState))
				{
					return 0f;
				}
				TISpaceBodyState tispaceBodyState = this.barycenter as TISpaceBodyState;
				ValueTuple<Vector3, Vector3> forwardAndUp = tispaceBodyState.GetForwardAndUp(TITimeState.Now());
				Vector3 item = forwardAndUp.Item1;
				Vector3 item2 = forwardAndUp.Item2;
				return -Vector3.SignedAngle((Vector3)(this.GetGlobalPosition() - tispaceBodyState.GetGlobalPosition()).normalized, item, item2);
			}
		}

		// Token: 0x17000D4B RID: 3403
		// (get) Token: 0x060044C6 RID: 17606 RVA: 0x001C1000 File Offset: 0x001BF200
		public GameObject gameObjectLink
		{
			get
			{
				if (!(this.controller != null))
				{
					return null;
				}
				return this.controller.gameObject;
			}
		}

		// Token: 0x17000D4C RID: 3404
		// (get) Token: 0x060044C7 RID: 17607 RVA: 0x001C101D File Offset: 0x001BF21D
		public Vector3 DisplayPositionNow
		{
			get
			{
				return this.controller.transform.position;
			}
		}

		// Token: 0x17000D4D RID: 3405
		// (get) Token: 0x060044C8 RID: 17608 RVA: 0x001C102F File Offset: 0x001BF22F
		public float radius_gameUnits
		{
			get
			{
				return this.controller.radius_gameUnits;
			}
		}

		// Token: 0x17000D4E RID: 3406
		// (get) Token: 0x060044C9 RID: 17609 RVA: 0x001C103C File Offset: 0x001BF23C
		public virtual bool inEarthSystem
		{
			get
			{
				TISpaceObjectState getSunOrbitingRelatedObject = this.GetSunOrbitingRelatedObject;
				return getSunOrbitingRelatedObject != null && getSunOrbitingRelatedObject.isEarth;
			}
		}

		// Token: 0x17000D4F RID: 3407
		// (get) Token: 0x060044CA RID: 17610 RVA: 0x001C104F File Offset: 0x001BF24F
		public Sprite icon
		{
			get
			{
				if (this._icon == null)
				{
					this._icon = GameControl.assetLoader.LoadAsset<Sprite>(this.iconResource);
				}
				return this._icon;
			}
		}

		// Token: 0x060044CB RID: 17611 RVA: 0x001C107B File Offset: 0x001BF27B
		public virtual Color GetSymbolColor()
		{
			return Color.clear;
		}

		// Token: 0x17000D50 RID: 3408
		// (get) Token: 0x060044CC RID: 17612 RVA: 0x001C1082 File Offset: 0x001BF282
		public bool isEarth
		{
			get
			{
				return GameStateManager.Earth() == this;
			}
		}

		// Token: 0x17000D51 RID: 3409
		// (get) Token: 0x060044CD RID: 17613 RVA: 0x001C108F File Offset: 0x001BF28F
		public bool isLuna
		{
			get
			{
				return GameStateManager.Luna() == this;
			}
		}

		// Token: 0x17000D52 RID: 3410
		// (get) Token: 0x060044CE RID: 17614 RVA: 0x001C109C File Offset: 0x001BF29C
		public bool isSun
		{
			get
			{
				return this.objectType == SpaceObjectType.Star;
			}
		}

		// Token: 0x17000D53 RID: 3411
		// (get) Token: 0x060044CF RID: 17615 RVA: 0x001C10A7 File Offset: 0x001BF2A7
		public bool isaMoon
		{
			get
			{
				return this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.PlanetaryMoon;
			}
		}

		// Token: 0x060044D0 RID: 17616 RVA: 0x001C10C0 File Offset: 0x001BF2C0
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TISpaceObjectTemplate tispaceObjectTemplate = template as TISpaceObjectTemplate;
			if (tispaceObjectTemplate == null)
			{
				return;
			}
			this.templateName = tispaceObjectTemplate.dataName;
			this.epoch_DateTime = new TIDateTime();
			this.epoch_DateTime.SetTime(this.epoch_JYears);
			this.displayName = template.displayName;
		}

		// Token: 0x060044D1 RID: 17617 RVA: 0x001C1114 File Offset: 0x001BF314
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			foreach (TINaturalSpaceObjectState tinaturalSpaceObjectState in GameStateManager.IterateByClass<TINaturalSpaceObjectState>(true))
			{
				if (tinaturalSpaceObjectState.template != null && this.template != null && tinaturalSpaceObjectState.template.dataName == this.template.barycenterName)
				{
					this.barycenter = tinaturalSpaceObjectState;
					break;
				}
			}
		}

		// Token: 0x060044D2 RID: 17618 RVA: 0x001C1190 File Offset: 0x001BF390
		public virtual void CreateVisualizer(TIDataTemplate myTemplate)
		{
			string path = "Prefabs/SpaceObject";
			switch (this.objectType)
			{
			case SpaceObjectType.Star:
				path = "Prefabs/Star";
				break;
			case SpaceObjectType.Planet:
			case SpaceObjectType.DwarfPlanet:
			case SpaceObjectType.Asteroid:
			case SpaceObjectType.Comet:
			case SpaceObjectType.PlanetaryMoon:
			case SpaceObjectType.AsteroidalMoon:
				path = "Prefabs/SpaceBody";
				break;
			case SpaceObjectType.Fleet:
				path = "Prefabs/Fleet";
				break;
			case SpaceObjectType.LagrangePoint:
				path = "Prefabs/NavigablePoint";
				break;
			}
			Log.Time("<color=#00cc00>LoadTime:</color> TISpaceObjectState Create Visualizer " + this.templateName, delegate
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(path));
				gameObject.SetActive(false);
				this.controller = gameObject.GetComponent<SpaceObjectController>();
				gameObject.name = this.ID.ToString();
				gameObject.layer = LayerMask.NameToLayer("Solar System");
				GameControl.solarSystem.AddObject(gameObject, true);
				gameObject.SetActive(true);
				this.controller.Initialize(this);
			}, false, this.objectType == SpaceObjectType.Fleet || (this.objectType == SpaceObjectType.Planet && this.templateName == "Earth"));
		}

		// Token: 0x060044D3 RID: 17619 RVA: 0x001C126C File Offset: 0x001BF46C
		public virtual CartesianState ToLocalCartesianStateAtTime(TIDateTime time)
		{
			if (this.barycenter == null)
			{
				return CartesianState.zero;
			}
			OrbitalElementsState orbitalElementsState = new OrbitalElementsState(this.longAscendingNode_Rad, this.argPeriapsis_Rad, this.inclination_Rad, this.semiMajorAxis_m, this.ecc, this.meanAnomalyAtEpoch_Rad, this.epoch_DateTime.ExportTime());
			return orbitalElementsState.ToCartesianStateAtTime(time.ExportTime(), this.barycenter.mass_kg);
		}

		// Token: 0x060044D4 RID: 17620 RVA: 0x001C12DC File Offset: 0x001BF4DC
		public virtual CartesianState ToGlobalCartesianStateAtTime(TIDateTime time)
		{
			CartesianState cartesianState = this.ToLocalCartesianStateAtTime(time);
			if (this.barycenter != null)
			{
				cartesianState = cartesianState.ToGlobal(this.barycenter, time);
			}
			return cartesianState;
		}

		// Token: 0x060044D5 RID: 17621 RVA: 0x001C130F File Offset: 0x001BF50F
		public virtual Vector3d GetGlobalPosition()
		{
			if (this.gameTime.Now != this.globalPositionTime)
			{
				return this.GetGlobalPositionAtTime(this.gameTime.currentTime);
			}
			return this.globalPosition;
		}

		// Token: 0x060044D6 RID: 17622 RVA: 0x001C1344 File Offset: 0x001BF544
		public virtual Vector3d GetGlobalPositionAtTime(TIDateTime time)
		{
			DateTime dateTime = time.ExportTime();
			if (this.globalPositionTime == dateTime)
			{
				return this.globalPosition;
			}
			this.globalPositionTime = dateTime;
			this.globalPosition = this.ToGlobalCartesianStateAtTime(time).position;
			return this.globalPosition;
		}

		// Token: 0x060044D7 RID: 17623 RVA: 0x001C138C File Offset: 0x001BF58C
		public Vector3d GetVelocityVectorAtTime(TIDateTime time)
		{
			return this.ToGlobalCartesianStateAtTime(time).velocity;
		}

		// Token: 0x060044D8 RID: 17624 RVA: 0x001C139C File Offset: 0x001BF59C
		public double GetVelocityAtTime(TIDateTime time)
		{
			return this.GetVelocityVectorAtTime(time).magnitude;
		}

		// Token: 0x060044D9 RID: 17625 RVA: 0x001C13B8 File Offset: 0x001BF5B8
		public virtual double GetAngularDiameter(double distanceInMeters)
		{
			return Mathd.AngularDiameterOfSphere(this.meanRadius_m, distanceInMeters);
		}

		// Token: 0x060044DA RID: 17626 RVA: 0x001C13C8 File Offset: 0x001BF5C8
		public double GetAngularDiameter(Vector3d viewingPosition)
		{
			Vector3d vector3d = this.GetGlobalPosition();
			return this.GetAngularDiameter(Vector3d.Distance(in viewingPosition, in vector3d));
		}

		// Token: 0x060044DB RID: 17627 RVA: 0x001C13EB File Offset: 0x001BF5EB
		public double GetAngularDiameter()
		{
			return this.GetAngularDiameter(CameraManager.Singleton.Position);
		}

		// Token: 0x060044DC RID: 17628 RVA: 0x001C1400 File Offset: 0x001BF600
		protected virtual OrbitalElementsState ToOrbitalElementsState(TIDateTime time = null)
		{
			if (time == null)
			{
				time = this.gameTime.currentTime;
			}
			double num = this.meanAnomalyAtEpoch_Rad + 6.283185307179586 * (time.DifferenceInSeconds(this.epoch_DateTime) / this.orbitalPeriod_s);
			return new OrbitalElementsState(this.longAscendingNode_Rad, this.argPeriapsis_Rad, this.inclination_Rad, this.semiMajorAxis_m, this.ecc, num, time.ExportTime());
		}

		// Token: 0x17000D54 RID: 3412
		// (get) Token: 0x060044DD RID: 17629 RVA: 0x001C1472 File Offset: 0x001BF672
		public double periapsis_AU
		{
			get
			{
				return this.semiMajorAxis_AU * (1.0 - this.ecc);
			}
		}

		// Token: 0x17000D55 RID: 3413
		// (get) Token: 0x060044DE RID: 17630 RVA: 0x001C148B File Offset: 0x001BF68B
		public double apoapsis_AU
		{
			get
			{
				return this.semiMajorAxis_AU * (1.0 + this.ecc);
			}
		}

		// Token: 0x17000D56 RID: 3414
		// (get) Token: 0x060044DF RID: 17631 RVA: 0x001C14A4 File Offset: 0x001BF6A4
		public double periapsis_km
		{
			get
			{
				return this.semiMajorAxis_km * (1.0 - this.ecc);
			}
		}

		// Token: 0x17000D57 RID: 3415
		// (get) Token: 0x060044E0 RID: 17632 RVA: 0x001C14BD File Offset: 0x001BF6BD
		public double apoapsis_km
		{
			get
			{
				return this.semiMajorAxis_km * (1.0 + this.ecc);
			}
		}

		// Token: 0x17000D58 RID: 3416
		// (get) Token: 0x060044E1 RID: 17633 RVA: 0x001C14D6 File Offset: 0x001BF6D6
		public double meanMotion_s
		{
			get
			{
				return 6.283185307179586 / this.orbitalPeriod_s;
			}
		}

		// Token: 0x060044E2 RID: 17634 RVA: 0x001C14E8 File Offset: 0x001BF6E8
		private DateTime TimeAtMeanAnomaly(double meanAnomaly, DateTime time)
		{
			double num = this.MeanAnomalyAtTime(time);
			double num2 = meanAnomaly - num;
			if (this.ecc < 1.0)
			{
				num2 = Mathd.ClampRadiansTwoPI(num2);
			}
			return time.AddSeconds(num2 / this.meanMotion_s);
		}

		// Token: 0x060044E3 RID: 17635 RVA: 0x001C1528 File Offset: 0x001BF728
		private double MeanAnomalyAtTime(DateTime time)
		{
			double num = (time - this.epoch_DateTime.ExportTime()).TotalSeconds * this.meanMotion_s;
			double num2 = this.meanAnomalyAtEpoch_Rad + num;
			if (this.ecc < 1.0)
			{
				num2 = Mathd.ClampRadiansTwoPI(num2);
			}
			return num2;
		}

		// Token: 0x060044E4 RID: 17636 RVA: 0x001C1578 File Offset: 0x001BF778
		public DateTime NextPeriapsisTime(DateTime time)
		{
			if (this.ecc < 1.0)
			{
				return this.TimeAtMeanAnomaly(0.0, time);
			}
			return time - TimeSpan.FromSeconds(this.MeanAnomalyAtTime(time) / this.meanMotion_s);
		}

		// Token: 0x060044E5 RID: 17637 RVA: 0x001C15B8 File Offset: 0x001BF7B8
		private double TrueToEccentric(double trueAnomaly)
		{
			double num = Mathd.Cos(trueAnomaly);
			if (this.ecc < 1.0)
			{
				double num2 = (this.ecc + num) / (1.0 + this.ecc * num);
				double num3 = Mathd.Sqrt(1.0 - num2 * num2);
				if (trueAnomaly > 3.141592653589793)
				{
					num3 *= -1.0;
				}
				return Mathd.ClampRadiansTwoPI(Mathd.Atan2(num3, num2));
			}
			double num4 = (this.ecc + num) / (1.0 + this.ecc * num);
			if (num4 < 1.0)
			{
				throw new ArgumentException("GetEccentricAnomalyAtTrueAnomaly: True anomaly of " + trueAnomaly.ToString() + " radians is not attained by orbit with eccentricity " + this.ecc.ToString());
			}
			double num5 = Mathd.ACosh(num4);
			if (trueAnomaly > 3.141592653589793)
			{
				num5 *= -1.0;
			}
			return num5;
		}

		// Token: 0x060044E6 RID: 17638 RVA: 0x001C16A4 File Offset: 0x001BF8A4
		private double EccentricToMean(double E)
		{
			double ecc = this.ecc;
			if (ecc < 1.0)
			{
				return Mathd.ClampRadiansTwoPI(E - ecc * Mathd.Sin(E));
			}
			return ecc * Mathd.Sinh(E) - E;
		}

		// Token: 0x060044E7 RID: 17639 RVA: 0x001C16E0 File Offset: 0x001BF8E0
		public DateTime TimeOfTrueAnomaly(double trueAnomaly, DateTime time)
		{
			double num = this.TrueToEccentric(trueAnomaly);
			double num2 = this.EccentricToMean(num);
			return this.TimeAtMeanAnomaly(num2, time);
		}

		// Token: 0x060044E8 RID: 17640 RVA: 0x001C1708 File Offset: 0x001BF908
		public double TrueAnomalyFromVector(Vector3d v)
		{
			double num = Mathd.Sin(this.inclination_Rad);
			Vector3d vector3d = Vector3d.Normalize(new Vector3d(num * Mathd.Sin(this.longAscendingNode_Rad), -num * Mathd.Cos(this.longAscendingNode_Rad), Mathd.Cos(this.inclination_Rad)));
			Vector3d vector3d2 = Vector3d.Exclude(vector3d, v);
			OrbitalElementsState orbitalElementsState = this.ToOrbitalElementsState(this.epoch_DateTime);
			DateTime dateTime = this.NextPeriapsisTime(this.epoch_DateTime.ExportTime());
			Vector3d position = orbitalElementsState.ToCartesianStateAtTime(dateTime, this.barycenter.mass_kg).position;
			double num2 = 0.017453292519943295 * Vector3d.Angle(in position, in vector3d2);
			double num3 = 0.017453292519943295;
			Vector3d vector3d3 = Vector3d.Cross(vector3d, position);
			if (Mathd.Abs(num3 * Vector3d.Angle(in vector3d2, in vector3d3)) < 1.5707963267948966)
			{
				return num2;
			}
			return 6.283185307179586 - num2;
		}

		// Token: 0x060044E9 RID: 17641 RVA: 0x001C17E8 File Offset: 0x001BF9E8
		public static double TransferDistance(IMobileAsset fleet, TIGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, bool generic)
		{
			if (fleet.ref_orbit == destination.ref_orbit)
			{
				return TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(fleet, destination.ref_spaceObject);
			}
			if (destination.isOrbitState && originValue.barycenter() == destinationValue.barycenter())
			{
				return Mathd.Abs(originValue.a_m() - destinationValue.a_m());
			}
			if (generic)
			{
				return TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(originValue.selfState().ref_spaceObject, destinationValue.selfState().ref_spaceObject);
			}
			return TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(fleet, destination.ref_spaceObject);
		}

		// Token: 0x060044EA RID: 17642 RVA: 0x001C1870 File Offset: 0x001BFA70
		public static double MaxDistanceBetweenTwoSpaceObjects_m(TISpaceObjectState object1, TISpaceObjectState object2)
		{
			if (object2.barycenter == object1)
			{
				return object2.semiMajorAxis_m;
			}
			if (object1.barycenter == object2)
			{
				return object1.semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter = object2.barycenter;
			if (((barycenter != null) ? barycenter.barycenter : null) == object1)
			{
				return object2.barycenter.semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter2 = object1.barycenter;
			if (((barycenter2 != null) ? barycenter2.barycenter : null) == object2)
			{
				return object1.barycenter.semiMajorAxis_m;
			}
			TISpaceObjectState getSunOrbitingRelatedObject = object1.GetSunOrbitingRelatedObject;
			TISpaceObjectState getSunOrbitingRelatedObject2 = object2.GetSunOrbitingRelatedObject;
			if (getSunOrbitingRelatedObject == getSunOrbitingRelatedObject2)
			{
				return Mathd.Abs(object1.semiMajorAxis_m + object2.semiMajorAxis_m);
			}
			return Mathd.Abs(getSunOrbitingRelatedObject.semiMajorAxis_m + getSunOrbitingRelatedObject2.semiMajorAxis_m);
		}

		// Token: 0x060044EB RID: 17643 RVA: 0x001C1930 File Offset: 0x001BFB30
		public static double MinDistanceBetweenTwoSpaceObjects_m(TISpaceObjectState object1, TISpaceObjectState object2)
		{
			if (object2.barycenter == object1)
			{
				return object2.semiMajorAxis_m;
			}
			if (object1.barycenter == object2)
			{
				return object1.semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter = object2.barycenter;
			if (((barycenter != null) ? barycenter.barycenter : null) == object1)
			{
				return object2.barycenter.semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter2 = object1.barycenter;
			if (((barycenter2 != null) ? barycenter2.barycenter : null) == object2)
			{
				return object1.barycenter.semiMajorAxis_m;
			}
			if (object1 == object2)
			{
				return 0.0;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState = object1.FindCommonBarycenter(object2);
			double num = ((object1.barycenter == tinaturalSpaceObjectState) ? object1.semiMajorAxis_m : ((object1.barycenter.barycenter == tinaturalSpaceObjectState) ? object1.barycenter.semiMajorAxis_m : object1.barycenter.barycenter.semiMajorAxis_m));
			double num2 = ((object2.barycenter == tinaturalSpaceObjectState) ? object2.semiMajorAxis_m : ((object2.barycenter.barycenter == tinaturalSpaceObjectState) ? object2.barycenter.semiMajorAxis_m : object2.barycenter.barycenter.semiMajorAxis_m));
			return Mathd.Abs(num - num2);
		}

		// Token: 0x060044EC RID: 17644 RVA: 0x001C1A62 File Offset: 0x001BFC62
		public static double MinDistanceBetweenTwoSpaceObjects_m(TISpaceFleetState object1, TISpaceObjectState object2)
		{
			return TISpaceObjectState.MinDistanceBetweenTwoSpaceObjects_m(object1, object2);
		}

		// Token: 0x060044ED RID: 17645 RVA: 0x001C1A6C File Offset: 0x001BFC6C
		public static double MinDistanceBetweenTwoSpaceObjects_m(IMobileAsset object1, TISpaceObjectState object2)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = object1.barycenter();
			if (object1.transferAssigned)
			{
				TISpaceFleetState tispaceFleetState = object1 as TISpaceFleetState;
				if (tispaceFleetState != null && TITimeState.Now() >= tispaceFleetState.trajectory.launchTime)
				{
					tinaturalSpaceObjectState = tispaceFleetState.trajectory.commonBarycenter;
				}
			}
			if (tinaturalSpaceObjectState == object2)
			{
				return object1.a_m();
			}
			if (tinaturalSpaceObjectState.barycenter == object2)
			{
				return object1.barycenter().semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
			if (((barycenter != null) ? barycenter.barycenter : null) == object2)
			{
				return object1.barycenterBarycenter().semiMajorAxis_m;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState2 = tinaturalSpaceObjectState.FindCommonBarycenter(object2);
			double num = ((object2 == tinaturalSpaceObjectState2) ? 0.0 : ((object2.barycenter == tinaturalSpaceObjectState2) ? object2.semiMajorAxis_m : ((object2.barycenter.barycenter == tinaturalSpaceObjectState2) ? object2.barycenter.semiMajorAxis_m : object2.barycenter.barycenter.semiMajorAxis_m)));
			return Mathd.Abs(object1.common_a_m(tinaturalSpaceObjectState2) - num);
		}

		// Token: 0x060044EE RID: 17646 RVA: 0x001C1B78 File Offset: 0x001BFD78
		public static double AverageDistanceBetweenTwoSpaceObjects_m(TISpaceObjectState object1, TISpaceObjectState object2)
		{
			if (object1.isHabState && object1.ref_hab.IsBase)
			{
				object1 = object1.ref_hab.habSite.parentBody;
			}
			if (object2.isHabState && object2.ref_hab.IsBase)
			{
				object2 = object2.ref_hab.habSite.parentBody;
			}
			if (object2.barycenter == object1)
			{
				return object2.semiMajorAxis_m;
			}
			if (object1.barycenter == object2)
			{
				return object1.semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter = object2.barycenter;
			if (((barycenter != null) ? barycenter.barycenter : null) == object1)
			{
				return object2.barycenter.semiMajorAxis_m;
			}
			TINaturalSpaceObjectState barycenter2 = object1.barycenter;
			if (((barycenter2 != null) ? barycenter2.barycenter : null) == object2)
			{
				return object1.barycenter.semiMajorAxis_m;
			}
			TISpaceObjectState getSunOrbitingRelatedObject = object1.GetSunOrbitingRelatedObject;
			TISpaceObjectState getSunOrbitingRelatedObject2 = object2.GetSunOrbitingRelatedObject;
			if (getSunOrbitingRelatedObject == getSunOrbitingRelatedObject2)
			{
				return (object1.semiMajorAxis_m + object2.semiMajorAxis_m + Mathd.Abs(object1.semiMajorAxis_m - object2.semiMajorAxis_m)) / 2.0;
			}
			return (getSunOrbitingRelatedObject.semiMajorAxis_m + getSunOrbitingRelatedObject2.semiMajorAxis_m + Mathd.Abs(getSunOrbitingRelatedObject.semiMajorAxis_m - getSunOrbitingRelatedObject2.semiMajorAxis_m)) / 2.0;
		}

		// Token: 0x060044EF RID: 17647 RVA: 0x001C1CB5 File Offset: 0x001BFEB5
		public static double ExactDistanceBetweenTwoSpaceObjects_m(TISpaceFleetState object1, TISpaceObjectState object2)
		{
			return TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(object1, object2);
		}

		// Token: 0x060044F0 RID: 17648 RVA: 0x001C1CC0 File Offset: 0x001BFEC0
		public static double ExactDistanceBetweenTwoSpaceObjects_m(TISpaceObjectState object1, TISpaceObjectState object2)
		{
			TIDateTime tidateTime = TITimeState.Now();
			if (object1.isHabState && object1.ref_hab.habType == HabType.Base)
			{
				object1 = object1.ref_habSite.parentBody;
			}
			Vector3d globalPositionAtTime = object1.GetGlobalPositionAtTime(tidateTime);
			if (object2.isHabState && object2.ref_hab.habType == HabType.Base)
			{
				object2 = object2.ref_habSite.parentBody;
			}
			Vector3d globalPositionAtTime2 = object2.GetGlobalPositionAtTime(tidateTime);
			return Vector3d.Distance(in globalPositionAtTime, in globalPositionAtTime2);
		}

		// Token: 0x060044F1 RID: 17649 RVA: 0x001C1D34 File Offset: 0x001BFF34
		public static double ExactDistanceBetweenTwoSpaceObjects_m(IMobileAsset object1, TISpaceObjectState object2)
		{
			TIDateTime tidateTime = TITimeState.Now();
			CartesianState? cartesianState = object1.tryToGetGlobalCartesianState(tidateTime);
			Vector3d vector3d = ((cartesianState != null) ? cartesianState.GetValueOrDefault().position : default(Vector3d));
			if (object2.isHabState && object2.ref_hab.habType == HabType.Base)
			{
				object2 = object2.ref_habSite.parentBody;
			}
			Vector3d globalPositionAtTime = object2.GetGlobalPositionAtTime(tidateTime);
			return Vector3d.Distance(in vector3d, in globalPositionAtTime);
		}

		// Token: 0x060044F2 RID: 17650 RVA: 0x001C1DA5 File Offset: 0x001BFFA5
		public static TINaturalSpaceObjectState FindCommonBarycenter(TISpaceObjectState firstObject, TIGameState secondObject)
		{
			return firstObject.FindCommonBarycenter(secondObject);
		}

		// Token: 0x060044F3 RID: 17651 RVA: 0x001C1DB0 File Offset: 0x001BFFB0
		public static bool IsAroundBarycenter(TISpaceObjectState firstObject, TINaturalSpaceObjectState barycenter)
		{
			if (!(((firstObject != null) ? firstObject.barycenter : null) == barycenter))
			{
				TIGameState tigameState;
				if (firstObject == null)
				{
					tigameState = null;
				}
				else
				{
					TINaturalSpaceObjectState barycenter2 = firstObject.barycenter;
					tigameState = ((barycenter2 != null) ? barycenter2.barycenter : null);
				}
				if (!(tigameState == barycenter))
				{
					TIGameState tigameState2;
					if (firstObject == null)
					{
						tigameState2 = null;
					}
					else
					{
						TINaturalSpaceObjectState barycenter3 = firstObject.barycenter;
						if (barycenter3 == null)
						{
							tigameState2 = null;
						}
						else
						{
							TINaturalSpaceObjectState barycenter4 = barycenter3.barycenter;
							tigameState2 = ((barycenter4 != null) ? barycenter4.barycenter : null);
						}
					}
					return tigameState2 == barycenter;
				}
			}
			return true;
		}

		// Token: 0x060044F4 RID: 17652 RVA: 0x001C1E20 File Offset: 0x001C0020
		public TINaturalSpaceObjectState FindCommonBarycenter(TIGameState secondSpaceObject)
		{
			GenericSpaceObject genericSpaceObject = new GenericSpaceObject();
			genericSpaceObject.AssignData(this);
			GenericSpaceObject genericSpaceObject2 = new GenericSpaceObject();
			genericSpaceObject2.AssignData(secondSpaceObject);
			return genericSpaceObject.FindCommonBarycenter(genericSpaceObject2);
		}

		// Token: 0x060044F5 RID: 17653 RVA: 0x001C1E4C File Offset: 0x001C004C
		public static void FindRelevantOrbitingObjectsForTransfer(TISpaceAssetState asset, TIGameState destination, out TINaturalSpaceObjectState commonBarycenter, out ITransferTarget itt_origin, out ITransferTarget itt_destination)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = null;
			TISpaceFleetState tispaceFleetState = destination as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launched)
			{
				tinaturalSpaceObjectState = tispaceFleetState.trajectory.GetBarycenterAtTime(TITimeState.Now());
			}
			TIHabState tihabState = asset as TIHabState;
			if (tihabState != null && tihabState.IsBase)
			{
				itt_origin = asset.ref_naturalSpaceObject.orbits[0];
				if (tinaturalSpaceObjectState == null)
				{
					commonBarycenter = asset.ref_naturalSpaceObject.FindCommonBarycenter(destination);
				}
				else
				{
					commonBarycenter = asset.ref_naturalSpaceObject.FindCommonBarycenter(tinaturalSpaceObjectState);
				}
			}
			else
			{
				itt_origin = asset;
				if (tinaturalSpaceObjectState == null)
				{
					commonBarycenter = asset.FindCommonBarycenter(destination);
				}
				else
				{
					commonBarycenter = asset.FindCommonBarycenter(tinaturalSpaceObjectState);
				}
			}
			ITransferTarget transferTarget2;
			if (!destination.isSpaceAssetState)
			{
				ITransferTarget transferTarget = destination.ref_orbit;
				transferTarget2 = transferTarget;
			}
			else
			{
				ITransferTarget transferTarget = destination.ref_spaceAsset;
				transferTarget2 = transferTarget;
			}
			itt_destination = transferTarget2;
		}

		// Token: 0x060044F6 RID: 17654 RVA: 0x001C1F18 File Offset: 0x001C0118
		public static TISpaceObjectState GetSunOrbitingRelatedObject_static(TISpaceObjectState testObject)
		{
			if (testObject.isHabState && testObject.ref_hab.IsBase)
			{
				return testObject.ref_hab.habSite.parentBody.GetSunOrbitingRelatedObject;
			}
			if (testObject.barycenter == null)
			{
				return null;
			}
			if (testObject.barycenter.isSun)
			{
				return testObject;
			}
			if (testObject.barycenter.barycenter != null && testObject.barycenter.barycenter.isSun)
			{
				return testObject.barycenter;
			}
			if (testObject.barycenter.barycenter != null && testObject.barycenter.barycenter.barycenter != null && testObject.barycenter.barycenter.barycenter.isSun)
			{
				return testObject.barycenter.barycenter;
			}
			if (testObject.barycenter.barycenter != null && testObject.barycenter.barycenter.barycenter != null && testObject.barycenter.barycenter.barycenter.barycenter != null && testObject.barycenter.barycenter.barycenter.barycenter.isSun)
			{
				return testObject.barycenter.barycenter.barycenter;
			}
			return null;
		}

		// Token: 0x17000D59 RID: 3417
		// (get) Token: 0x060044F7 RID: 17655 RVA: 0x001C2060 File Offset: 0x001C0260
		public virtual TISpaceObjectState GetSunOrbitingRelatedObject
		{
			get
			{
				if (this.isHabState && this.ref_hab.IsBase)
				{
					return this.ref_hab.habSite.parentBody.GetSunOrbitingRelatedObject;
				}
				if (this.isSun)
				{
					return this;
				}
				if (this.barycenter == null)
				{
					return null;
				}
				if (this.barycenter.isSun)
				{
					return this;
				}
				if (this.barycenter.barycenter != null && this.barycenter.barycenter.isSun)
				{
					return this.barycenter;
				}
				if (this.barycenter.barycenter != null && this.barycenter.barycenter.barycenter != null && this.barycenter.barycenter.barycenter.isSun)
				{
					return this.barycenter.barycenter;
				}
				if (this.barycenter.barycenter != null && this.barycenter.barycenter.barycenter != null && this.barycenter.barycenter.barycenter.barycenter != null && this.barycenter.barycenter.barycenter.barycenter.isSun)
				{
					return this.barycenter.barycenter.barycenter;
				}
				return null;
			}
		}

		// Token: 0x060044F8 RID: 17656 RVA: 0x001C21B4 File Offset: 0x001C03B4
		public static double genericSynodicPeriod_s(TIGameState origin, TIGameState destination, out bool isRetrograde)
		{
			if (origin.ref_habSite == origin)
			{
				origin = origin.ref_habSite.parentBody;
			}
			if (destination.ref_habSite == destination)
			{
				destination = destination.ref_habSite.parentBody;
			}
			if (origin.ref_orbit == origin)
			{
				origin = origin.ref_orbit.barycenter;
			}
			if (destination.ref_orbit == destination)
			{
				destination = destination.ref_orbit.barycenter;
			}
			TISpaceObjectState tispaceObjectState = origin.ref_spaceObject.GetSunOrbitingRelatedObject;
			TISpaceObjectState tispaceObjectState2 = destination.ref_spaceObject.GetSunOrbitingRelatedObject;
			if (tispaceObjectState == tispaceObjectState2)
			{
				tispaceObjectState = origin.ref_naturalSpaceObject;
				tispaceObjectState2 = destination.ref_naturalSpaceObject;
			}
			if (tispaceObjectState == tispaceObjectState2)
			{
				isRetrograde = false;
				return 0.0;
			}
			Vector3d vector3d = Vector3d.Normalize(new Vector3d(Mathd.Sin(tispaceObjectState.inclination_Rad) * Mathd.Sin(tispaceObjectState.longAscendingNode_Rad), -Mathd.Sin(tispaceObjectState.inclination_Rad) * Mathd.Cos(tispaceObjectState.longAscendingNode_Rad), Mathd.Cos(tispaceObjectState.inclination_Rad)));
			Vector3d vector3d2 = Vector3d.Normalize(new Vector3d(Mathd.Sin(tispaceObjectState2.inclination_Rad) * Mathd.Sin(tispaceObjectState2.longAscendingNode_Rad), -Mathd.Sin(tispaceObjectState2.inclination_Rad) * Mathd.Cos(tispaceObjectState2.longAscendingNode_Rad), Mathd.Cos(tispaceObjectState2.inclination_Rad)));
			int num = ((Vector3d.Dot(in vector3d, in vector3d2) > 0.0) ? 1 : (-1));
			isRetrograde = num == -1;
			return Mathd.Abs(1.0 / (1.0 / tispaceObjectState.orbitalPeriod_s - (double)num / tispaceObjectState2.orbitalPeriod_s));
		}

		// Token: 0x060044F9 RID: 17657 RVA: 0x001C2348 File Offset: 0x001C0548
		protected static double MeanLongitudeBetweenTwoSpaceObjects_deg(TINaturalSpaceObjectState origin, TINaturalSpaceObjectState destination, TIDateTime time)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = origin.FindCommonBarycenter(destination);
			if (origin == tinaturalSpaceObjectState || destination == tinaturalSpaceObjectState)
			{
				return 0.0;
			}
			if (origin.barycenter != tinaturalSpaceObjectState)
			{
				origin = origin.barycenter;
			}
			if (destination.barycenter != tinaturalSpaceObjectState)
			{
				destination = destination.barycenter;
			}
			OrbitalElementsState orbitalElementsState = origin.ToOrbitalElementsState(time);
			OrbitalElementsState orbitalElementsState2 = destination.ToOrbitalElementsState(time);
			DateTime dateTime = time.ExportTime();
			double num = orbitalElementsState.MeanLongitudeAtTime_Rad(dateTime, tinaturalSpaceObjectState.mass_kg);
			return Mathd.ClampRadiansTwoPI(orbitalElementsState2.MeanLongitudeAtTime_Rad(dateTime, tinaturalSpaceObjectState.mass_kg) - num) * 57.29577951308232;
		}

		// Token: 0x060044FA RID: 17658 RVA: 0x001C23EC File Offset: 0x001C05EC
		protected static double AngleBetweenTwoSpaceObjects_deg(TINaturalSpaceObjectState origin, TINaturalSpaceObjectState destination, TIDateTime time)
		{
			Vector3d globalPositionAtTime = origin.FindCommonBarycenter(destination).GetGlobalPositionAtTime(time);
			Vector3d vector3d = origin.GetGlobalPositionAtTime(time) - globalPositionAtTime;
			Vector3d vector3d2 = destination.GetGlobalPositionAtTime(time) - globalPositionAtTime;
			Vector3d vector3d3 = new Vector3d(0f, 0f, 1f);
			double num = Vector3d.SignedAngle(in vector3d, in vector3d2, in vector3d3);
			if (num < 0.0)
			{
				num += 360.0;
			}
			return num;
		}

		// Token: 0x060044FB RID: 17659 RVA: 0x001C2464 File Offset: 0x001C0664
		protected static double HohmannTransferTime_s(TIFactionState faction, GenericSpaceObject origin, GenericSpaceObject destination)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = origin.FindCommonBarycenter(destination);
			double relevantSemimajorAxis_m = origin.GetRelevantSemimajorAxis_m(tinaturalSpaceObjectState);
			double relevantSemimajorAxis_m2 = destination.GetRelevantSemimajorAxis_m(tinaturalSpaceObjectState);
			double num = Mathd.Pow(relevantSemimajorAxis_m + relevantSemimajorAxis_m2, 3.0);
			double num2 = 8.0 * tinaturalSpaceObjectState.mu;
			return 3.141592653589793 * Mathd.Sqrt(num / num2);
		}

		// Token: 0x060044FC RID: 17660 RVA: 0x001C24BC File Offset: 0x001C06BC
		public static double GetHohmannTimePenaltyFraction(TIFactionState faction, TIDateTime nextHohmann, double synodicPeriod_s, out bool penaltyFromPrior)
		{
			if (synodicPeriod_s == double.PositiveInfinity)
			{
				penaltyFromPrior = false;
				return 0.0;
			}
			double totalDays = (nextHohmann - TITimeState.Now()).TotalDays;
			TIDateTime tidateTime = new TIDateTime(nextHohmann);
			tidateTime.AddSeconds(-synodicPeriod_s);
			double totalDays2 = (TITimeState.Now() - tidateTime).TotalDays;
			float num = (float)Mathd.Min(totalDays, totalDays2);
			penaltyFromPrior = totalDays2 < totalDays;
			float num2 = (float)((double)num / (synodicPeriod_s / 86400.0));
			return (double)(num2 + TIEffectsState.SumEffectsModifiers(Context.GenericTransfer_OffDate_PCT, faction, num2, null));
		}

		// Token: 0x060044FD RID: 17661 RVA: 0x001C2548 File Offset: 0x001C0748
		public static double GenericTransferTime_s(TIFactionState faction, TIGameState origin, TIGameState destination)
		{
			GenericSpaceObject genericSpaceObject = new GenericSpaceObject();
			genericSpaceObject.AssignData(origin);
			GenericSpaceObject genericSpaceObject2 = new GenericSpaceObject();
			genericSpaceObject2.AssignData(destination);
			TINaturalSpaceObjectState tinaturalSpaceObjectState = genericSpaceObject.FindCommonBarycenter(genericSpaceObject2);
			double relevantSemimajorAxis_m = genericSpaceObject.GetRelevantSemimajorAxis_m(tinaturalSpaceObjectState);
			double relevantSemimajorAxis_m2 = genericSpaceObject2.GetRelevantSemimajorAxis_m(tinaturalSpaceObjectState);
			if (Mathd.Approximately(relevantSemimajorAxis_m, relevantSemimajorAxis_m2))
			{
				double num = 360.0 - TISpaceObjectState.AngleBetweenTwoSpaceObjects_deg(origin.ref_naturalSpaceObject, destination.ref_naturalSpaceObject, TITimeState.Now());
				return TISpaceObjectState.LagrangeTransferDuration_s(tinaturalSpaceObjectState, relevantSemimajorAxis_m, num);
			}
			if (origin.ref_naturalSpaceObject == destination.ref_naturalSpaceObject || origin.ref_naturalSpaceObject == tinaturalSpaceObjectState || destination.ref_naturalSpaceObject == tinaturalSpaceObjectState)
			{
				return TISpaceObjectState.HohmannTransferTime_s(faction, genericSpaceObject, genericSpaceObject2);
			}
			if (destination.isLagrangePointState && destination.ref_lagrangePoint.secondaryObject == origin && (destination.ref_lagrangePoint.lagrangeValue == LagrangeValue.L3 || destination.ref_lagrangePoint.lagrangeValue == LagrangeValue.L4 || destination.ref_lagrangePoint.lagrangeValue == LagrangeValue.L5))
			{
				return TISpaceObjectState.HohmannTransferTime_s(faction, genericSpaceObject, genericSpaceObject2);
			}
			if (origin.ref_naturalSpaceObject.barycenter != tinaturalSpaceObjectState)
			{
				origin = origin.ref_naturalSpaceObject.barycenter;
				genericSpaceObject.AssignData(origin);
			}
			if (destination.ref_naturalSpaceObject.barycenter != tinaturalSpaceObjectState)
			{
				destination = destination.ref_naturalSpaceObject.barycenter;
				genericSpaceObject2.AssignData(destination);
			}
			double num2;
			TIDateTime nextHohmannLaunchWindowDate = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(faction, origin.ref_naturalSpaceObject, destination.ref_naturalSpaceObject, TITimeState.Now(), out num2);
			bool flag;
			return TISpaceObjectState.HohmannTransferTime_s(faction, genericSpaceObject, genericSpaceObject2) * (1.0 + TISpaceObjectState.GetHohmannTimePenaltyFraction(faction, nextHohmannLaunchWindowDate, num2, out flag));
		}

		// Token: 0x060044FE RID: 17662 RVA: 0x001C26CC File Offset: 0x001C08CC
		private static double LagrangeTransferDuration_s(TINaturalSpaceObjectState transferBarycenter, double radius_m, double angle_Deg)
		{
			double num = 6.283185307179586 * Mathd.Sqrt(radius_m * radius_m * radius_m / transferBarycenter.mu);
			int num2 = ((angle_Deg < 90.0) ? 1 : 2);
			return num * ((double)num2 - angle_Deg / 360.0);
		}

		// Token: 0x060044FF RID: 17663 RVA: 0x001C2714 File Offset: 0x001C0914
		private static double GenericTransferDeltaV_mps(GenericSpaceObject origin, GenericSpaceObject destination, bool ignoreInclinationChange = false)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = origin.FindCommonBarycenter(destination);
			double relevantSemimajorAxis_m = origin.GetRelevantSemimajorAxis_m(tinaturalSpaceObjectState);
			double relevantSemimajorAxis_m2 = destination.GetRelevantSemimajorAxis_m(tinaturalSpaceObjectState);
			double num6;
			double num7;
			if (Mathd.Approximately(relevantSemimajorAxis_m, relevantSemimajorAxis_m2))
			{
				double num = 360.0 - TISpaceObjectState.AngleBetweenTwoSpaceObjects_deg(origin.trueState.ref_naturalSpaceObject, destination.trueState.ref_naturalSpaceObject, TITimeState.Now());
				double num2 = TISpaceObjectState.LagrangeTransferDuration_s(tinaturalSpaceObjectState, relevantSemimajorAxis_m, num);
				double num3 = Mathd.Pow(tinaturalSpaceObjectState.mu * (num2 * num2 / 39.47841760435743), 0.3333333333333333);
				double num4 = Mathd.Sqrt(tinaturalSpaceObjectState.mu * (2.0 / relevantSemimajorAxis_m - 1.0 / num3));
				double num5 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / relevantSemimajorAxis_m);
				num6 = Mathd.Abs(num4 - num5);
				num7 = num6;
			}
			else
			{
				double num8 = relevantSemimajorAxis_m + relevantSemimajorAxis_m2;
				double num9 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / relevantSemimajorAxis_m);
				double num10 = Mathd.Sqrt(2.0 * relevantSemimajorAxis_m2 / num8) - 1.0;
				num6 = num9 * num10;
				double num11 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / relevantSemimajorAxis_m2);
				double num12 = Mathd.Sqrt(2.0 * relevantSemimajorAxis_m / num8);
				double num13 = 1.0 - num12;
				num7 = num11 * num13;
			}
			if (!ignoreInclinationChange)
			{
				double num14 = Mathd.Abs(origin.GetRelevantInclination_Rad(tinaturalSpaceObjectState) - destination.GetRelevantInclination_Rad(tinaturalSpaceObjectState));
				double num15 = 2.0 * Mathd.Sin(num14 / 2.0) * Mathd.Min(origin.GetRelevantOrbitalVelocity_mps(tinaturalSpaceObjectState), destination.GetRelevantOrbitalVelocity_mps(tinaturalSpaceObjectState));
				return Mathd.Abs(num6 + num7) + num15;
			}
			return Mathd.Abs(num6 + num7);
		}

		// Token: 0x06004500 RID: 17664 RVA: 0x001C28B0 File Offset: 0x001C0AB0
		public static double GenericTransferDeltaV_mps(TIGameState origin, TIGameState destination, bool ignoreInclinationChange = false)
		{
			GenericSpaceObject genericSpaceObject = new GenericSpaceObject();
			genericSpaceObject.AssignData(origin);
			GenericSpaceObject genericSpaceObject2 = new GenericSpaceObject();
			genericSpaceObject2.AssignData(destination);
			return TISpaceObjectState.GenericTransferDeltaV_mps(genericSpaceObject, genericSpaceObject2, ignoreInclinationChange);
		}

		// Token: 0x06004501 RID: 17665 RVA: 0x001C28DD File Offset: 0x001C0ADD
		public static float ModifiedGenericTransferEV_kps(TIFactionState faction)
		{
			return 2.11f + TIEffectsState.SumEffectsModifiers(Context.GenericTransferEV_kps, faction, 2.11f, null);
		}

		// Token: 0x06004502 RID: 17666 RVA: 0x001C28F8 File Offset: 0x001C0AF8
		public static double GenericTransferBoostFromEarthSurface(TIFactionState faction, TIGameState destination, float mass_tons)
		{
			TIOrbitState ref_orbit = destination.ref_orbit;
			if (ref_orbit != null && ref_orbit.isEarthLEO)
			{
				return (double)(mass_tons * TemplateManager.global.spaceResourceToTons);
			}
			double num = TISpaceObjectState.GenericTransferDeltaVFromEarthLEO_kps(faction, destination, destination.ref_spaceBody != null && destination.ref_spaceBody.isEarth);
			if (destination.ref_habSite != null)
			{
				num += destination.ref_habSite.DeltaVToLandFromInterface_kps(null, 9.8, true, true);
			}
			else if (destination.isSpaceBodyState && destination.ref_spaceBody.habSites.Length != 0)
			{
				num += destination.ref_spaceBody.habSites[0].DeltaVToLandFromInterface_kps(null, 9.8, true, true);
			}
			float num2 = TISpaceObjectState.ModifiedGenericTransferEV_kps(faction);
			return (double)mass_tons * Mathd.Exp(num / (double)num2) * (double)TemplateManager.global.spaceResourceToTons;
		}

		// Token: 0x06004503 RID: 17667 RVA: 0x001C29CB File Offset: 0x001C0BCB
		private static double GenericTransferDeltaVFromEarthLEO_mps(TIFactionState faction, TIGameState destination, bool ignoreInclinationChange = false)
		{
			return TISpaceObjectState.GenericTransferDeltaV_mps(GameStateManager.LEOStates()[0], destination, ignoreInclinationChange);
		}

		// Token: 0x06004504 RID: 17668 RVA: 0x001C29DF File Offset: 0x001C0BDF
		private static double GenericTransferDeltaVFromEarthLEO_kps(TIFactionState faction, TIGameState destination, bool ignoreInclinationChange = false)
		{
			return TISpaceObjectState.GenericTransferDeltaVFromEarthLEO_mps(faction, destination, ignoreInclinationChange) / 1000.0;
		}

		// Token: 0x06004505 RID: 17669 RVA: 0x001C29F3 File Offset: 0x001C0BF3
		public static float GenericTransferTime_d(TIFactionState faction, TIGameState origin, TIGameState destination)
		{
			return (float)(TISpaceObjectState.GenericTransferTime_s(faction, origin, destination) / 86400.0);
		}

		// Token: 0x06004506 RID: 17670 RVA: 0x001C2A08 File Offset: 0x001C0C08
		public static float GenericTransferTimeFromEarthsSurface_d(TIFactionState faction, TIGameState destination)
		{
			return TISpaceObjectState.GenericTransferTime_d(faction, GameStateManager.Earth(), destination);
		}

		// Token: 0x06004507 RID: 17671 RVA: 0x001C2A18 File Offset: 0x001C0C18
		public static double GenericTransferTimeFromNearestHab_d(TIFactionState faction, TIGameState destination, TISpaceObjectState.HabClassification habClassification, out TIHabState nearestHab)
		{
			TIHabState tihabState = null;
			double num = double.PositiveInfinity;
			if (destination.isHabState && (habClassification == TISpaceObjectState.HabClassification.Any || (habClassification == TISpaceObjectState.HabClassification.Shipyard && destination.ref_hab.AllowsShipConstruction(faction, false, false)) || (habClassification == TISpaceObjectState.HabClassification.Resupply && destination.ref_hab.AllowsResupply(faction, false, false))))
			{
				nearestHab = destination.ref_hab;
				return 0.0;
			}
			TISpaceObjectState tispaceObjectState = null;
			if (destination.isSpaceObjectState && destination.ref_spaceObject.barycenter == null)
			{
				Log.Error(destination.templateName + " has no barycenter", Array.Empty<object>());
				nearestHab = null;
				return num;
			}
			if (destination.isSpaceObjectState && destination.ref_spaceObject.barycenter.isSun)
			{
				tispaceObjectState = destination.ref_spaceObject;
			}
			else if (destination.isSpaceObjectState)
			{
				tispaceObjectState = destination.ref_spaceObject.barycenter;
			}
			else if (destination.isHabSiteState)
			{
				tispaceObjectState = destination.ref_habSite.parentBody;
			}
			else if (destination.isOrbitState)
			{
				tispaceObjectState = destination.ref_orbit.barycenter;
			}
			foreach (TIHabState tihabState2 in faction.habs)
			{
				bool flag = false;
				switch (habClassification)
				{
				case TISpaceObjectState.HabClassification.Any:
					flag = true;
					break;
				case TISpaceObjectState.HabClassification.Resupply:
					flag = tihabState2.AllowsResupply(faction, false, false);
					break;
				case TISpaceObjectState.HabClassification.Shipyard:
					flag = tihabState2.AllowsShipConstruction(faction, false, false);
					break;
				}
				if (flag)
				{
					double num2 = TISpaceObjectState.GenericTransferTime_s(faction, tihabState2.barycenter, tispaceObjectState);
					if (num2 < num)
					{
						tihabState = tihabState2;
						num = num2;
					}
				}
			}
			nearestHab = tihabState;
			if (!(tihabState != null))
			{
				return -1.0;
			}
			return num / 86400.0;
		}

		// Token: 0x04002881 RID: 10369
		[SerializeField]
		protected double? _rnd_rotationOffset_Deg;

		// Token: 0x04002883 RID: 10371
		[SerializeField]
		protected Vector3d globalPosition;

		// Token: 0x04002884 RID: 10372
		protected DateTime globalPositionTime = new DateTime(2000, 1, 1, 0, 0, 0);

		// Token: 0x04002886 RID: 10374
		protected GameTimeManager gameTime;

		// Token: 0x04002887 RID: 10375
		[fsIgnore]
		protected Sprite _icon;

		// Token: 0x04002888 RID: 10376
		public const string symbolResource = "ui/SpaceObjectSymbol";

		// Token: 0x04002889 RID: 10377
		public const float GenericTransferEV_kps = 2.11f;

		// Token: 0x02000F62 RID: 3938
		public enum HabClassification
		{
			// Token: 0x04005DEE RID: 24046
			Any,
			// Token: 0x04005DEF RID: 24047
			Resupply,
			// Token: 0x04005DF0 RID: 24048
			Shipyard
		}
	}
}
