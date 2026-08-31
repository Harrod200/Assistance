using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009DF RID: 2527
	public class CombatShipController : CombatantShipController
	{
		// Token: 0x17001045 RID: 4165
		// (get) Token: 0x06005ECE RID: 24270 RVA: 0x002CEB5D File Offset: 0x002CCD5D
		public override IDamageableType damageableType
		{
			get
			{
				return IDamageableType.Ship;
			}
		}

		// Token: 0x06005ECF RID: 24271 RVA: 0x002CEB60 File Offset: 0x002CCD60
		public override IDamageableType GetCombatantType()
		{
			return IDamageableType.Ship;
		}

		// Token: 0x17001046 RID: 4166
		// (get) Token: 0x06005ED0 RID: 24272 RVA: 0x002CEB63 File Offset: 0x002CCD63
		// (set) Token: 0x06005ED1 RID: 24273 RVA: 0x002CEB6B File Offset: 0x002CCD6B
		public ShipVisController visualizationController { get; private set; }

		// Token: 0x17001047 RID: 4167
		// (get) Token: 0x06005ED2 RID: 24274 RVA: 0x002CEB74 File Offset: 0x002CCD74
		// (set) Token: 0x06005ED3 RID: 24275 RVA: 0x002CEB7C File Offset: 0x002CCD7C
		public override ShipModelController ModelController { get; protected set; }

		// Token: 0x17001048 RID: 4168
		// (get) Token: 0x06005ED4 RID: 24276 RVA: 0x002CEB85 File Offset: 0x002CCD85
		// (set) Token: 0x06005ED5 RID: 24277 RVA: 0x002CEB8D File Offset: 0x002CCD8D
		public override Vector3 velocityVector { get; protected set; }

		// Token: 0x17001049 RID: 4169
		// (get) Token: 0x06005ED6 RID: 24278 RVA: 0x002CEB96 File Offset: 0x002CCD96
		// (set) Token: 0x06005ED7 RID: 24279 RVA: 0x002CEB9E File Offset: 0x002CCD9E
		public float angularVelocity_kps { get; private set; }

		// Token: 0x1700104A RID: 4170
		// (get) Token: 0x06005ED8 RID: 24280 RVA: 0x002CEBA7 File Offset: 0x002CCDA7
		// (set) Token: 0x06005ED9 RID: 24281 RVA: 0x002CEBAF File Offset: 0x002CCDAF
		public bool thrusting { get; protected set; }

		// Token: 0x1700104B RID: 4171
		// (get) Token: 0x06005EDA RID: 24282 RVA: 0x002CEBB8 File Offset: 0x002CCDB8
		public bool CanWaypointsBeAdjusted
		{
			get
			{
				return this._waypointNavigationController.CanWaypointsBeAdjusted;
			}
		}

		// Token: 0x1700104C RID: 4172
		// (get) Token: 0x06005EDB RID: 24283 RVA: 0x002CEBC5 File Offset: 0x002CCDC5
		// (set) Token: 0x06005EDC RID: 24284 RVA: 0x002CEBCD File Offset: 0x002CCDCD
		public IHull hull { get; private set; }

		// Token: 0x1700104D RID: 4173
		// (get) Token: 0x06005EDD RID: 24285 RVA: 0x002CEBD6 File Offset: 0x002CCDD6
		// (set) Token: 0x06005EDE RID: 24286 RVA: 0x002CEBDE File Offset: 0x002CCDDE
		public override List<Collider> hitColliders { get; protected set; }

		// Token: 0x1700104E RID: 4174
		// (get) Token: 0x06005EDF RID: 24287 RVA: 0x002CEBE7 File Offset: 0x002CCDE7
		// (set) Token: 0x06005EE0 RID: 24288 RVA: 0x002CEBEF File Offset: 0x002CCDEF
		public override TISpaceShipState ShipState { get; protected set; }

		// Token: 0x1700104F RID: 4175
		// (get) Token: 0x06005EE1 RID: 24289 RVA: 0x002CEBF8 File Offset: 0x002CCDF8
		// (set) Token: 0x06005EE2 RID: 24290 RVA: 0x002CEC00 File Offset: 0x002CCE00
		public CombatantController primaryTarget { get; set; }

		// Token: 0x17001050 RID: 4176
		// (get) Token: 0x06005EE3 RID: 24291 RVA: 0x002CEC09 File Offset: 0x002CCE09
		// (set) Token: 0x06005EE4 RID: 24292 RVA: 0x002CEC11 File Offset: 0x002CCE11
		public CombatantController oldPrimaryTarget { get; private set; }

		// Token: 0x17001051 RID: 4177
		// (get) Token: 0x06005EE5 RID: 24293 RVA: 0x002CEC1A File Offset: 0x002CCE1A
		// (set) Token: 0x06005EE6 RID: 24294 RVA: 0x002CEC22 File Offset: 0x002CCE22
		public CombatantController maneuverTarget { get; set; }

		// Token: 0x17001052 RID: 4178
		// (get) Token: 0x06005EE7 RID: 24295 RVA: 0x002CEC2B File Offset: 0x002CCE2B
		// (set) Token: 0x06005EE8 RID: 24296 RVA: 0x002CEC33 File Offset: 0x002CCE33
		public CombatantController oldManeuverTarget { get; private set; }

		// Token: 0x17001053 RID: 4179
		// (get) Token: 0x06005EE9 RID: 24297 RVA: 0x002CEC3C File Offset: 0x002CCE3C
		public TIDateTime TimeOfNextWaypoint
		{
			get
			{
				return this._waypointNavigationController.TimeOfFirstWaypoint;
			}
		}

		// Token: 0x17001054 RID: 4180
		// (get) Token: 0x06005EEA RID: 24298 RVA: 0x002CEC49 File Offset: 0x002CCE49
		public override CombatShipController ref_shipController
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06005EEB RID: 24299 RVA: 0x002CEC4C File Offset: 0x002CCE4C
		public override Vector3 positionAtTime(DateTime currentTime)
		{
			return this._waypointNavigationController.PositionAtTime(new TIDateTime(currentTime));
		}

		// Token: 0x17001055 RID: 4181
		// (get) Token: 0x06005EEC RID: 24300 RVA: 0x002CEC5F File Offset: 0x002CCE5F
		public Vector3 heading
		{
			get
			{
				return base.transform.forward;
			}
		}

		// Token: 0x06005EED RID: 24301 RVA: 0x002CEC6C File Offset: 0x002CCE6C
		public Vector3 headingAtTime(DateTime currentTime)
		{
			return this._waypointNavigationController.HeadingAtTime(new TIDateTime(currentTime));
		}

		// Token: 0x06005EEE RID: 24302 RVA: 0x002CEC7F File Offset: 0x002CCE7F
		public Vector3 velocityAtTime(DateTime currentTime)
		{
			return this._waypointNavigationController.VelocityAtTime(new TIDateTime(currentTime));
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x002CEC92 File Offset: 0x002CCE92
		public Vector3 accelerationAtTime(DateTime currentTime)
		{
			return this._waypointNavigationController.AccelerationAtTime(new TIDateTime(currentTime));
		}

		// Token: 0x17001056 RID: 4182
		// (get) Token: 0x06005EF0 RID: 24304 RVA: 0x002CECA5 File Offset: 0x002CCEA5
		public Quaternion rotation
		{
			get
			{
				return base.transform.rotation;
			}
		}

		// Token: 0x17001057 RID: 4183
		// (get) Token: 0x06005EF1 RID: 24305 RVA: 0x002CECB2 File Offset: 0x002CCEB2
		public float angular_acceleration_rads2
		{
			get
			{
				return this.ShipState.angular_acceleration_rads2;
			}
		}

		// Token: 0x17001058 RID: 4184
		// (get) Token: 0x06005EF2 RID: 24306 RVA: 0x002CECBF File Offset: 0x002CCEBF
		public float max_angular_velocity_rads_s
		{
			get
			{
				return this.ShipState.max_angular_velocity_rad_s;
			}
		}

		// Token: 0x17001059 RID: 4185
		// (get) Token: 0x06005EF3 RID: 24307 RVA: 0x002CECCC File Offset: 0x002CCECC
		public float acceleration
		{
			get
			{
				return this.ShipState.combatAcceleration_kps2 * 0.05f;
			}
		}

		// Token: 0x1700105A RID: 4186
		// (get) Token: 0x06005EF4 RID: 24308 RVA: 0x002CECDF File Offset: 0x002CCEDF
		public float cruiseAcceleration
		{
			get
			{
				return this.ShipState.cruiseAcceleration_kps2 * 0.05f;
			}
		}

		// Token: 0x1700105B RID: 4187
		// (get) Token: 0x06005EF5 RID: 24309 RVA: 0x002CECF2 File Offset: 0x002CCEF2
		public bool activePlayerShip
		{
			get
			{
				return this.ShipState.fleet.faction == GameControl.control.activePlayer;
			}
		}

		// Token: 0x06005EF6 RID: 24310 RVA: 0x002CED13 File Offset: 0x002CCF13
		public override CombatTargetableState GetCombatantState()
		{
			return this.ShipState;
		}

		// Token: 0x1700105C RID: 4188
		// (get) Token: 0x06005EF7 RID: 24311 RVA: 0x002CED1B File Offset: 0x002CCF1B
		public override Vector3 velocityVector_kps
		{
			get
			{
				return this.velocityVector / 0.05f;
			}
		}

		// Token: 0x1700105D RID: 4189
		// (get) Token: 0x06005EF8 RID: 24312 RVA: 0x002CED2D File Offset: 0x002CCF2D
		public CombatTargetableState primaryTargetState
		{
			get
			{
				return this.primaryTarget.GetCombatantState();
			}
		}

		// Token: 0x1700105E RID: 4190
		// (get) Token: 0x06005EF9 RID: 24313 RVA: 0x002CED3A File Offset: 0x002CCF3A
		public IList<IHullSection> sections
		{
			get
			{
				return this.hull.sections;
			}
		}

		// Token: 0x06005EFA RID: 24314 RVA: 0x002CED47 File Offset: 0x002CCF47
		public override SpaceCombatAssetUIController UIController()
		{
			return this.visualizationController.UIController;
		}

		// Token: 0x1700105F RID: 4191
		// (get) Token: 0x06005EFB RID: 24315 RVA: 0x002CED54 File Offset: 0x002CCF54
		public bool InCollisionAvoidanceManeuver
		{
			get
			{
				return this._waypointNavigationController.TimeOfCollisionPassed != null && this._waypointNavigationController.TimeOfCollisionPassed > this.gameTime.currentTime;
			}
		}

		// Token: 0x17001060 RID: 4192
		// (get) Token: 0x06005EFC RID: 24316 RVA: 0x002CED86 File Offset: 0x002CCF86
		public override Vector3 accelerationVector
		{
			get
			{
				return this._cachedAccelerationVector;
			}
		}

		// Token: 0x17001061 RID: 4193
		// (get) Token: 0x06005EFD RID: 24317 RVA: 0x002CED8E File Offset: 0x002CCF8E
		public override Vector3 accelerationVector_kps
		{
			get
			{
				return this._cachedAccelerationVector / 0.05f;
			}
		}

		// Token: 0x17001062 RID: 4194
		// (get) Token: 0x06005EFE RID: 24318 RVA: 0x002CEDA0 File Offset: 0x002CCFA0
		public IEnumerable<IWeapon> weapons
		{
			get
			{
				return this.hull.IterateByClass<IWeapon>();
			}
		}

		// Token: 0x06005EFF RID: 24319 RVA: 0x002CEDB0 File Offset: 0x002CCFB0
		public float GetDVConservingAceleration_kps2(bool isProactiveBurn)
		{
			float num = 900f;
			if (!isProactiveBurn)
			{
				num /= 2f;
			}
			float num2 = this.ShipState.AvailableDeltaVForCombat_kps();
			return this.ShipState.GetDVConservingCombatAcceleration_mps2(num, num2) / 1000f;
		}

		// Token: 0x06005F00 RID: 24320 RVA: 0x002CEDED File Offset: 0x002CCFED
		public float GetDVConservingAceleration_unity(bool isProactiveBurn)
		{
			return this.GetDVConservingAceleration_kps2(isProactiveBurn) * 0.05f;
		}

		// Token: 0x06005F01 RID: 24321 RVA: 0x002CEDFC File Offset: 0x002CCFFC
		public AccelerationConstraints GetAccelerationConstraints()
		{
			return new AccelerationConstraints(this.acceleration, this.cruiseAcceleration, this.angular_acceleration_rads2, this.max_angular_velocity_rads_s);
		}

		// Token: 0x06005F02 RID: 24322 RVA: 0x002CEE1B File Offset: 0x002CD01B
		public AccelerationConstraints GetDVConservingAccelerationConstraints(bool isProactiveBurn)
		{
			return new AccelerationConstraints(this.GetDVConservingAceleration_unity(isProactiveBurn), this.cruiseAcceleration, this.angular_acceleration_rads2, this.max_angular_velocity_rads_s);
		}

		// Token: 0x06005F03 RID: 24323 RVA: 0x002CEE3B File Offset: 0x002CD03B
		private void Start()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}

		// Token: 0x06005F04 RID: 24324 RVA: 0x002CEE4D File Offset: 0x002CD04D
		private void OnDestroy()
		{
			this._waypointNavigationController.OnShipDestructionTriggered();
		}

		// Token: 0x06005F05 RID: 24325 RVA: 0x002CEE5C File Offset: 0x002CD05C
		public void Initialize(TISpaceShipState state, Vector3 initialVelocity)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (Error.IsInvalidGameState<TISpaceShipState>(state))
			{
				return;
			}
			base.destructionTriggered = false;
			this.inDefensiveManuever = false;
			base.WeaponCarrierState = state;
			this.ShipState = state;
			this.cachedFaction = base.faction;
			state.prevPartsBeingRepaired.Clear();
			state.prevSystemsBeingRepaired.Clear();
			base.name = this.ShipState.ID.ToString();
			base.combatMgr = GameControl.spaceCombat;
			GameObject gameObjectLink = this.ShipState.fleet.gameObjectLink;
			base.combatantTransform = base.transform;
			if (gameObjectLink == null)
			{
				Log.Error("Shipstate:" + base.name + " missing fleetObject for fleet:" + this.ShipState.fleet.ID.ToString(), Array.Empty<object>());
				return;
			}
			if (gameObjectLink.GetComponentInChildren<FleetVisController>(true) == null)
			{
				Log.Error("Shipstate:" + base.name + " missing FleetVisController for fleet:" + this.ShipState.fleet.ID.ToString(), Array.Empty<object>());
				return;
			}
			if (this.ShipState.visualizerLink == null)
			{
				Log.Error("Shipstate:" + base.name + " missing visualizerLink", Array.Empty<object>());
				return;
			}
			this.shipVisualizerTransform = this.ShipState.visualizerLink.transform;
			if (this.shipVisualizerTransform.GetComponentInChildren<ShipModelController>(true) == null)
			{
				Log.Error("Shipstate:" + base.name + " missing ShipModelController", Array.Empty<object>());
				return;
			}
			this.shipVisualizerTransform.GetComponentInChildren<ShipModelController>(true).transform.rotation = Quaternion.identity;
			this.localPositionToRestore = this.shipVisualizerTransform.position;
			this.localScaleToRestore = this.shipVisualizerTransform.localScale;
			this.StrategyShipControllerTransform = this.shipVisualizerTransform.parent.transform;
			this.shipVisualizerTransform.SetParent(base.combatantTransform, true);
			this.shipVisualizerTransform.position = base.combatantTransform.position;
			this.shipVisualizerTransform.localScale = Vector3.one * GameControl.spaceCombat.modelScalingFactor;
			this.visualizationController = this.shipVisualizerTransform.GetComponent<ShipVisController>();
			this.visualizationController.enabled = true;
			this.ModelController = this.visualizationController.ModelController;
			this.ModelController.SetVectorThrusters(state.drive, state.template.designingFaction);
			this.ModelController.DisableRadiatorEmissives();
			this.ModelController.ResetManeuverCommandUI();
			this.hitColliders = this.ModelController.GetComponentsInChildren<Collider>().ToList<Collider>();
			this.rootCollider = this.ModelController.GetComponent<Collider>();
			this.rootCollider.enabled = base.combatMgr.initialized && !base.combatMgr.IsInFormationSelectionMode;
			if (this.activePlayerShip)
			{
				this.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
			}
			else
			{
				this.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.RedTargetSquare);
			}
			List<IHullSection> list = TISpaceShipState.SetUpArmorSections(this.ShipState);
			this.hull = new Hull(list, this);
			for (int i = 0; i < this.ShipState.noseWeapons.Count; i++)
			{
				this.hull.AddComponentMap<IWeapon>(ComponentMap.single, "Nose" + i.ToString());
			}
			for (int j = 0; j < this.ShipState.hullWeapons.Count; j++)
			{
				this.hull.AddComponentMap<IWeapon>(ComponentMap.single, "Lateral" + j.ToString());
			}
			int num = 0;
			int num2 = 0;
			foreach (ModuleDataEntry moduleDataEntry in this.ShipState.template.allWeapons)
			{
				TIShipWeaponTemplate ref_weapon = moduleDataEntry.moduleTemplate.ref_weapon;
				IWeapon weapon = null;
				if (moduleDataEntry.moduleTemplate is TIBeamWeaponTemplate)
				{
					weapon = new BeamWeapon(this, moduleDataEntry);
				}
				else if (moduleDataEntry.moduleTemplate is TIGunTypeWeaponTemplate)
				{
					weapon = new ProjectileWeapon(this, moduleDataEntry);
				}
				else if (moduleDataEntry.moduleTemplate.ref_missileWeapon != null)
				{
					weapon = new MissileWeapon(this, moduleDataEntry);
				}
				if (weapon != null)
				{
					if (ref_weapon.hullWeapon)
					{
						string text = "Lateral" + num.ToString();
						num++;
						if (!this.hull.Attach<IWeapon>(weapon, text, Array.Empty<IHullSection>()))
						{
							Error.Log(string.Concat(new string[]
							{
								"Could not attach weapon to hull: ",
								this.ShipState.displayName,
								": ",
								ref_weapon.displayName,
								" Slot ",
								moduleDataEntry.slotIndex.ToString()
							}), Array.Empty<object>());
						}
					}
					else if (ref_weapon.noseWeapon)
					{
						string text2 = "Nose" + num2.ToString();
						num2++;
						if (!this.hull.Attach<IWeapon>(weapon, text2, Array.Empty<IHullSection>()))
						{
							Error.Log(string.Concat(new string[]
							{
								"Could not attach weapon to nose: ",
								this.ShipState.displayName,
								": ",
								ref_weapon.displayName,
								" Slot ",
								moduleDataEntry.slotIndex.ToString()
							}), Array.Empty<object>());
						}
					}
				}
			}
			foreach (ShipWeaponVisController shipWeaponVisController in this.ModelController.allWeaponControllers)
			{
				shipWeaponVisController.CreatePrefabs();
			}
			this.ModelController.allWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
			{
				x.ClearStratLayerTarget();
			});
			this.ModelController.allWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
			{
				x.ClearTarget();
			});
			this._spinPortRotation = new Quaternion(0f, -0.7f, 0f, 0.7f);
			this._spinStarboardRotation = new Quaternion(0f, 0.7f, 0f, 0.7f);
			this._spinVentralRotation = new Quaternion(0.4f, 0f, 0f, 0.9f);
			this._spinDorsalRotation = new Quaternion(-0.4f, 0f, 0f, 0.9f);
			this.initialized = true;
			this.alliedCombatants = new List<CombatantController>();
			this.enemyCombatants = new List<CombatantController>();
			this.velocityVector = initialVelocity;
			this.angularVelocity_kps = 0f;
			bool flag;
			if (this.ShipState.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMissileWeapon))
			{
				flag = this.ShipState.allWeaponTemplates.All<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMissileWeapon || !x.attackMode);
			}
			else
			{
				flag = false;
			}
			this.AI_IsMissileBoat = flag;
			this.InitializeWaypoints(state.ID);
			this.SetupListeners(state);
		}

		// Token: 0x06005F06 RID: 24326 RVA: 0x002CF5E8 File Offset: 0x002CD7E8
		private void SetupListeners(TISpaceShipState state)
		{
			GameControl.eventManager.AddListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.OnShipDeltaVChange), null, state, false, false);
			GameControl.eventManager.AddListener<ShipHeatChange>(new EventManager.EventDelegate<ShipHeatChange>(this.OnShipHeatChange), null, state, false, false);
			GameControl.eventManager.AddListener<CombatShipPropulsionValuesUpdated>(new EventManager.EventDelegate<CombatShipPropulsionValuesUpdated>(this.OnPropulsionValuesUpdated), null, state, false, false);
		}

		// Token: 0x06005F07 RID: 24327 RVA: 0x002CF644 File Offset: 0x002CD844
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.OnShipDeltaVChange), null);
			GameControl.eventManager.RemoveListener<ShipHeatChange>(new EventManager.EventDelegate<ShipHeatChange>(this.OnShipHeatChange), null);
			GameControl.eventManager.RemoveListener<CombatShipPropulsionValuesUpdated>(new EventManager.EventDelegate<CombatShipPropulsionValuesUpdated>(this.OnPropulsionValuesUpdated), null);
		}

		// Token: 0x06005F08 RID: 24328 RVA: 0x002CF696 File Offset: 0x002CD896
		public void SetAccelerationVector()
		{
			this._cachedAccelerationVector = this._waypointNavigationController.VelocityAtTime(new TIDateTime(TITimeState.Now(), 1.0)) - this._waypointNavigationController.VelocityAtTime(new TIDateTime(TITimeState.Now()));
		}

		// Token: 0x06005F09 RID: 24329 RVA: 0x002CF6D6 File Offset: 0x002CD8D6
		public void SetInitialVelocityVector(Vector3 newInitialVelocity)
		{
			this.velocityVector = newInitialVelocity;
			this.ReinitializeWaypoints();
		}

		// Token: 0x06005F0A RID: 24330 RVA: 0x002CF6E8 File Offset: 0x002CD8E8
		private void InitializeWaypoints(GameStateID id)
		{
			bool debug_suppressCombatAI = TemplateManager.global.debug_suppressCombatAI;
			int debug_suppressCombatAIAfterXPasses = TemplateManager.global.debug_suppressCombatAIAfterXPasses;
			bool flag = debug_suppressCombatAI && debug_suppressCombatAIAfterXPasses == -1;
			WaypointSharedData waypointSharedData = new WaypointSharedData((this.activePlayerShip || flag) ? GameControl.spaceCombat.waypointPrefab : GameControl.spaceCombat.enemyWaypointPrefab, base.combatMgr.waypointTimeDelta, this.acceleration, this.cruiseAcceleration, this.angular_acceleration_rads2, this.max_angular_velocity_rads_s);
			this._waypointNavigationController = new WaypointNavigationController(base.name, base.combatMgr.waypointCount, this.velocityVector, base.position, this.gameTime.currentTime, waypointSharedData, this.ShipState, this.BoundingBoxSize, base.combatMgr.mainCamera, this);
			GameObject waypointContainer = this._waypointNavigationController.WaypointContainer;
			base.combatMgr.container.Add(waypointContainer.name, waypointContainer, false, false);
			waypointContainer.transform.localScale = Vector3.one;
			waypointContainer.transform.SetLayer(LayerMask.NameToLayer("Space Combat UI"), true);
			this.UpdateShipPositioning(this.gameTime.currentTime);
		}

		// Token: 0x06005F0B RID: 24331 RVA: 0x002CF806 File Offset: 0x002CDA06
		public void ReinitializeWaypoints()
		{
			base.combatMgr.container.Remove(this._waypointNavigationController.WaypointContainer.name, true);
			this._waypointNavigationController.CleanUpWaypoints();
			this.InitializeWaypoints(this.ShipState.ID);
		}

		// Token: 0x06005F0C RID: 24332 RVA: 0x002CF846 File Offset: 0x002CDA46
		public void EnableRootCollider()
		{
			this.rootCollider.enabled = true;
		}

		// Token: 0x17001063 RID: 4195
		// (get) Token: 0x06005F0D RID: 24333 RVA: 0x002CF854 File Offset: 0x002CDA54
		public Vector3 BoundingBoxSize
		{
			get
			{
				if (this._boundingBoxSize == null)
				{
					Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
					for (int i = 0; i < this.ModelController._shipModalPhysicsColliders.Length; i++)
					{
						Collider collider = this.ModelController._shipModalPhysicsColliders[i] as BoxCollider;
						if (collider != null)
						{
							bounds.Encapsulate(collider.bounds.min);
							bounds.Encapsulate(collider.bounds.max);
						}
						else
						{
							collider = this.ModelController._shipModalPhysicsColliders[i] as CapsuleCollider;
							if (collider != null)
							{
								bounds.Encapsulate(collider.bounds.min);
								bounds.Encapsulate(collider.bounds.max);
							}
							else
							{
								collider = this.ModelController._shipModalPhysicsColliders[i] as SphereCollider;
								if (collider != null)
								{
									bounds.Encapsulate(collider.bounds.min);
									bounds.Encapsulate(collider.bounds.max);
								}
							}
						}
					}
					this._boundingBoxSize = new Vector3?(new Vector3(bounds.size.x * base.transform.localScale.x, bounds.size.y * base.transform.localScale.y, bounds.size.z * base.transform.localScale.z));
				}
				return this._boundingBoxSize.Value;
			}
		}

		// Token: 0x06005F0E RID: 24334 RVA: 0x002CF9F1 File Offset: 0x002CDBF1
		public bool AlwaysShowWaypoints()
		{
			return base.combatMgr.combatHUD.selectedFriendlyShip == this || base.combatMgr.combatHUD.groupSelectedFriendlyShips.Contains(this);
		}

		// Token: 0x06005F0F RID: 24335 RVA: 0x002CFA23 File Offset: 0x002CDC23
		public void SetWaypointVisualization(bool setActive)
		{
			this._waypointNavigationController.SetWaypointVisualization(setActive);
		}

		// Token: 0x06005F10 RID: 24336 RVA: 0x002CFA31 File Offset: 0x002CDC31
		public void ToggleWaypointVisualization()
		{
			this._waypointNavigationController.ToggleWaypointVisualization();
		}

		// Token: 0x06005F11 RID: 24337 RVA: 0x002CFA3E File Offset: 0x002CDC3E
		public void ToggleEnemyShipDetailedPathRendering()
		{
			if (!this.activePlayerShip)
			{
				this._waypointNavigationController.TogglePathRenderer();
			}
		}

		// Token: 0x06005F12 RID: 24338 RVA: 0x002CFA53 File Offset: 0x002CDC53
		private void UpdateWaypoints(TIDateTime currentTime)
		{
			this._waypointNavigationController.UpdateWaypointNavigation(currentTime);
		}

		// Token: 0x06005F13 RID: 24339 RVA: 0x002CFA61 File Offset: 0x002CDC61
		public void ProposePath(Vector3[] path, ProposedWaypoint end, AccelerationConstraints constraints)
		{
			this._waypointNavigationController.ProposePath(this.gameTime.currentTime, path, end, constraints);
		}

		// Token: 0x06005F14 RID: 24340 RVA: 0x002CFA7C File Offset: 0x002CDC7C
		public void ProposeRotation(Quaternion rotation)
		{
			this._waypointNavigationController.ProposeRotation(rotation);
		}

		// Token: 0x06005F15 RID: 24341 RVA: 0x002CFA8A File Offset: 0x002CDC8A
		public void ProposeWaypoint(ProposedWaypoint proposed)
		{
			this._waypointNavigationController.ProposeWaypoint(proposed);
		}

		// Token: 0x06005F16 RID: 24342 RVA: 0x002CFA98 File Offset: 0x002CDC98
		public void ProposePlacement(Vector3 position)
		{
			this._waypointNavigationController.ProposePlacement(position);
		}

		// Token: 0x06005F17 RID: 24343 RVA: 0x002CFAA6 File Offset: 0x002CDCA6
		public void ResetWaypoints()
		{
			this._waypointNavigationController.ResetWaypoints();
		}

		// Token: 0x06005F18 RID: 24344 RVA: 0x002CFAB4 File Offset: 0x002CDCB4
		public bool TryToKeepNoseTowardsThreat(List<ProjectileController> threateningProjectiles)
		{
			if (threateningProjectiles.Count == 0 || this.ShipState.AvailableDeltaVForCombat_kps() <= 0f)
			{
				return false;
			}
			Vector3 vector = Vector3.zero;
			float num = float.MaxValue;
			foreach (ProjectileController projectileController in threateningProjectiles)
			{
				float num2 = (base.position - projectileController.position).magnitude / SpaceCombatManager.vector_km_to_scale(projectileController.velocityVector_kps).magnitude;
				num = Mathf.Min(num, num2);
				vector += (base.position - projectileController.position).normalized;
			}
			vector.Normalize();
			this.ResetWaypoints();
			this.ProposeRotation(Quaternion.LookRotation(-vector));
			return true;
		}

		// Token: 0x06005F19 RID: 24345 RVA: 0x002CFBA0 File Offset: 0x002CDDA0
		public bool TryAssignDefensivePosition(List<ProjectileController> threateningProjectiles, AccelerationConstraints constraints)
		{
			if (threateningProjectiles.Count == 0 || this.ShipState.AvailableDeltaVForCombat_kps() <= 0f)
			{
				return false;
			}
			Vector3 vector = Vector3.zero;
			float num = float.MaxValue;
			foreach (ProjectileController projectileController in threateningProjectiles)
			{
				float num2 = (base.position - projectileController.position).magnitude / SpaceCombatManager.vector_km_to_scale(projectileController.velocityVector_kps).magnitude;
				num = Mathf.Min(num, num2);
				vector += (base.position - projectileController.position).normalized;
			}
			vector.Normalize();
			float num3 = 1.5707964f;
			float num4 = PhysicsHelpers.TimeFromDisplacementAndAcceleration(num3 * 0.5f, constraints.AngularAcceleration);
			float num5 = Mathf.Min(constraints.MaxAngularVelocity, PhysicsHelpers.VelocityFromAccelerationAndTime(constraints.AngularAcceleration, num4));
			float num6 = num3 / num5 + num5 / constraints.AngularAcceleration;
			float num7 = constraints.MaxAngularVelocity / constraints.AngularAcceleration;
			float num8 = Mathf.Max(num6 - num7, 0f) * 2f;
			float num9 = num7 + num8;
			float num10 = num + num9;
			float num11 = Mathf.Min(num, GameControl.spaceCombat.waypointTimeDelta);
			float num12 = PhysicsHelpers.DisplacementFromAccelerationAndTime(constraints.LinearAcceleration, num11);
			float num13 = PhysicsHelpers.DisplacementFromAccelerationAndTime(constraints.LinearAcceleration, num10);
			TIDateTime tidateTime = new TIDateTime(this.TimeOfNextWaypoint);
			Vector3 vector2 = this.positionAtTime(tidateTime.ExportTime());
			Vector3 vector3 = Vector3.Cross(vector, this.velocityVector.normalized).normalized;
			if (vector3.sqrMagnitude < 1E-45f)
			{
				vector3 = Quaternion.Euler(90f, 0f, 0f) * this.velocityVector.normalized;
			}
			Vector3 vector4 = vector2 + vector3 * num12;
			if (num13 > this.ref_shipController.ShipState.hull.length_m * GameControl.spaceCombat.modelScalingFactor)
			{
				this.ProposePlacement(vector4);
				Utilities.DebugDrawPoint(vector2, 1f, Color.green, 5f);
				Utilities.DebugDrawPoint(vector4, 1f, Color.magenta, 5f);
				Debug.DrawRay(base.position, this.velocityVector.normalized, Color.green, 5f);
				Debug.DrawRay(base.position, vector, Color.red, 5f);
				Debug.DrawRay(base.position, vector3, Color.yellow, 5f);
			}
			else
			{
				this.ResetWaypoints();
				this.ProposeRotation(Quaternion.LookRotation(-vector));
				Debug.DrawRay(vector2, vector, Color.red, 5f);
			}
			return true;
		}

		// Token: 0x06005F1A RID: 24346 RVA: 0x002CFE6C File Offset: 0x002CE06C
		public void BeginDefensiveManuevers()
		{
			this.inDefensiveManuever = true;
			this._waypointNavigationController.BeginDefensiveManeuvers();
		}

		// Token: 0x06005F1B RID: 24347 RVA: 0x002CFE80 File Offset: 0x002CE080
		public void CancelDefensiveManuevers()
		{
			this.inDefensiveManuever = false;
			this.nextDefensiveManueverUpdateTime = this.gameTime.currentTime;
			this.ShipState.RemoveCombatManeuver(CombatManeuver.DefensiveManuevers);
			this._waypointNavigationController.CancelDefensiveManeuvers();
		}

		// Token: 0x06005F1C RID: 24348 RVA: 0x002CFEB4 File Offset: 0x002CE0B4
		public float GetShipEffectiveScaledCombatRange()
		{
			float num = 0f;
			int num2 = 0;
			foreach (ShipWeaponVisController shipWeaponVisController in this.ModelController.allWeaponControllers)
			{
				if (shipWeaponVisController.weaponTemplate != null && base.WeaponCarrierState.WeaponIsOperable(shipWeaponVisController.weaponModuleData) && !shipWeaponVisController.transform.name.Contains("Dorsal") && (shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Focus) || shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Offense)))
				{
					if (shipWeaponVisController.weaponTemplate.isLaserWeapon && shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Offense))
					{
						num += shipWeaponVisController.weaponTemplate.targetingRange_km * 0.6f;
					}
					else
					{
						num += shipWeaponVisController.weaponTemplate.targetingRange_km * 0.75f;
					}
					num2++;
				}
			}
			return SpaceCombatManager.km_to_scale(num / (float)num2);
		}

		// Token: 0x06005F1D RID: 24349 RVA: 0x002CFFCC File Offset: 0x002CE1CC
		public float GetShipMaxScaledCombatRange()
		{
			float num = 0f;
			int num2 = 0;
			foreach (ShipWeaponVisController shipWeaponVisController in this.ModelController.allWeaponControllers)
			{
				if (shipWeaponVisController.weaponTemplate != null && base.WeaponCarrierState.WeaponIsOperable(shipWeaponVisController.weaponModuleData) && !shipWeaponVisController.transform.name.Contains("Dorsal") && (shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Focus) || shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Offense)))
				{
					if (shipWeaponVisController.weaponTemplate.isLaserWeapon && shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Offense))
					{
						num += shipWeaponVisController.weaponTemplate.targetingRange_km;
					}
					else
					{
						num += shipWeaponVisController.weaponTemplate.targetingRange_km;
					}
					num2++;
				}
			}
			return SpaceCombatManager.km_to_scale(num / (float)num2);
		}

		// Token: 0x06005F1E RID: 24350 RVA: 0x002D00D8 File Offset: 0x002CE2D8
		public List<ProjectileController> GetAllThreateningProjectiles()
		{
			List<ProjectileController> list = new List<ProjectileController>(4);
			foreach (ProjectileController projectileController in GameControl.spaceCombat._projectiles.Values)
			{
				if (!(projectileController == null) && !projectileController.hasHit && !projectileController.beenDestroyed && projectileController.clearedLauncher && projectileController.projectileState.shootingFaction != base.faction && projectileController.warheadMass_kg > 1f && TIUtilities.WillHitSphere(base.position, this.velocityVector, projectileController.position, projectileController.velocityVector, this.ShipState.hull.length_m) && !this.IsProjectileContested(projectileController))
				{
					list.Add(projectileController);
				}
			}
			return list;
		}

		// Token: 0x06005F1F RID: 24351 RVA: 0x002D01C0 File Offset: 0x002CE3C0
		private List<MissileController> GetAllMissilesTargetingMe()
		{
			List<MissileController> list = new List<MissileController>();
			foreach (ProjectileController projectileController in GameControl.spaceCombat._projectiles.Values)
			{
				if (!(projectileController == null) && projectileController.isMissile && !(projectileController.projectileState.shootingFaction == this.cachedFaction) && !projectileController.hasHit && !projectileController.beenDestroyed)
				{
					MissileController missileController = projectileController as MissileController;
					if (missileController.target == this && TIUtilities.MovingTowardsTarget(base.position, this.velocityVector, missileController.position, missileController.velocityVector))
					{
						list.Add(missileController);
					}
				}
			}
			return list;
		}

		// Token: 0x06005F20 RID: 24352 RVA: 0x002D028C File Offset: 0x002CE48C
		public void FilterForImminentImpactThreats(ref List<ProjectileController> projectiles)
		{
			List<ProjectileController> list = new List<ProjectileController>(4);
			foreach (ProjectileController projectileController in projectiles)
			{
				float magnitude = (projectileController.velocityVector_kps - this.velocityVector_kps).magnitude;
				if ((projectileController.position - base.position).magnitude / magnitude > base.combatMgr.waypointTimeDelta * 3f)
				{
					list.Add(projectileController);
				}
			}
			foreach (ProjectileController projectileController2 in list)
			{
				projectiles.Remove(projectileController2);
			}
		}

		// Token: 0x06005F21 RID: 24353 RVA: 0x002D0370 File Offset: 0x002CE570
		public bool IsProjectileContested(ProjectileController projectile)
		{
			foreach (IWeapon weapon in this.hull.IterateByClass<IWeapon>())
			{
				DefenseFireMode defenseFireMode = weapon.currentFireMode as DefenseFireMode;
				if (defenseFireMode != null)
				{
					float num = Vector3.Distance(weapon.combatant.position, projectile.position);
					if (defenseFireMode.GetExpectedDamage(num * 0.05f, projectile) >= projectile.projectileState.originWeapon.minDamageForPDToFire)
					{
						using (List<CombatWeaponCarrierState>.Enumerator enumerator2 = projectile.projectileState.enemiesTargetingMe.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.GetFaction() != base.faction)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06005F22 RID: 24354 RVA: 0x002D0468 File Offset: 0x002CE668
		protected void UpdateIsMissileSaturated()
		{
			TIDateTime currentTime = this.gameTime.currentTime;
			if (this.lastTimeChecked < currentTime)
			{
				List<MissileController> allMissilesTargetingMe = this.GetAllMissilesTargetingMe();
				base.isMissileSaturated = this.EstimateShipKillDamageThreshold() < this.EstimatedIncomingMissileDamage(allMissilesTargetingMe);
				this.lastTimeChecked = currentTime;
			}
		}

		// Token: 0x06005F23 RID: 24355 RVA: 0x002D04B2 File Offset: 0x002CE6B2
		private float EstimateShipKillDamageThreshold()
		{
			return (float)(this.ShipState.hull.structuralIntegrity * 6) * (1f + this.ShipState.sumArmorValue / 20f);
		}

		// Token: 0x06005F24 RID: 24356 RVA: 0x002D04E0 File Offset: 0x002CE6E0
		public int EstimatedMaxProjectilesPointDefenseCanHandle()
		{
			int num = 0;
			foreach (Weapon weapon in from x in this.hull.IterateByClass<IWeapon>()
				select x as Weapon)
			{
				if (this.ShipState.WeaponIsOperable(weapon.weaponData))
				{
					if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x is DefenseFireMode || x is GuardianFireMode))
					{
						num += Mathf.CeilToInt(120f / weapon.weaponData.weaponTemplate.cooldown_s);
					}
				}
			}
			return num;
		}

		// Token: 0x06005F25 RID: 24357 RVA: 0x002D05B0 File Offset: 0x002CE7B0
		public float EstimatedIncomingMissileDamage(List<MissileController> incomingMissiles)
		{
			float num = 0f;
			int num2 = this.EstimatedMaxProjectilesPointDefenseCanHandle();
			foreach (MissileController missileController in incomingMissiles.OrderBy<MissileController, float>((MissileController x) => TIUtilities.RandomFloatValue()))
			{
				if (num2 > 0)
				{
					num2--;
				}
				else
				{
					num += missileController.GetEstimatedDamage_Points();
				}
			}
			return num;
		}

		// Token: 0x06005F26 RID: 24358 RVA: 0x002D0638 File Offset: 0x002CE838
		public override float ApplyDamage(DamageSource source)
		{
			float num = 0f;
			if (base.destructionTriggered)
			{
				return num;
			}
			float num2;
			if (source.damage.weapon != null && source.damage.weapon.isProjectileWeapon && this.ShipState.thrustersActive && (!source.damage.weapon.isMissileWeapon || !source.damage.weapon.ref_missileWeapon.AOEWeapon) && (this.hull as Hull).StruckFacing(source, base.transform.position, base.transform.forward, out num2) == ArmorFacing.Tail)
			{
				ProjectileDamageSource projectileDamageSource = source as ProjectileDamageSource;
				if (projectileDamageSource != null)
				{
					float num3 = this.ShipState.drive.thrustPower_GW * 10f;
					float warheadMass_kg = projectileDamageSource.warheadMass_kg;
					float num4 = Mathf.Min((1f - (180f - Mathf.Abs(num2)) / 45f) * num3 / warheadMass_kg, 0.5f);
					if (TIUtilities.RandomFloatValue() < num4)
					{
						return num;
					}
				}
			}
			num += this.hull.ApplyDamage(source, base.transform);
			this.ModelController.ApplyDamageVisualizations(source.hitPosition, source.damage.type, source.damage.amount / 8f);
			if (this.hull.IsDestroyed())
			{
				this.TriggerShipDestruction(source.attacker.ref_shipCarrier(), source.damage.weapon);
			}
			return num;
		}

		// Token: 0x06005F27 RID: 24359 RVA: 0x002D07C2 File Offset: 0x002CE9C2
		public void ApplyDamageVisualization(Vector3 hitPoint, DamageType damageType, float damageValue)
		{
			if (base.destructionTriggered)
			{
				return;
			}
			this.ModelController.ApplyDamageVisualizations(hitPoint, damageType, damageValue);
		}

		// Token: 0x06005F28 RID: 24360 RVA: 0x002D07DB File Offset: 0x002CE9DB
		private void OnShipHeatChange(ShipHeatChange e)
		{
			if (base.destructionTriggered)
			{
				return;
			}
			if (this.hull.IsDestroyed())
			{
				this.TriggerShipDestruction(e.ship, null);
			}
		}

		// Token: 0x06005F29 RID: 24361 RVA: 0x002D0800 File Offset: 0x002CEA00
		private void OnShipDeltaVChange(ShipDeltaVChange e)
		{
			if (this.ShipState.AvailableDeltaVForCombat_kps() > 0f)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			foreach (CombatManeuver combatManeuver in this.ShipState.activeCombatManeuvers)
			{
				if (combatManeuver <= CombatManeuver.AllStop)
				{
					if (combatManeuver != CombatManeuver.Padlock)
					{
						switch (combatManeuver)
						{
						case CombatManeuver.SpinPort:
							flag4 = true;
							break;
						case CombatManeuver.SpinStarboard:
							flag5 = true;
							break;
						case CombatManeuver.AllStop:
							flag2 = true;
							break;
						}
					}
					else
					{
						flag = true;
					}
				}
				else if (combatManeuver != CombatManeuver.MatchVelocity)
				{
					if (combatManeuver == CombatManeuver.DefensiveManuevers)
					{
						flag6 = true;
					}
				}
				else
				{
					flag3 = true;
				}
			}
			if (flag)
			{
				ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is CancelPadlockPrimaryTargetCommand).OnCommandExecute(this.ShipState, null);
			}
			if (flag2)
			{
				ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is CancelAllStopCommand).OnCommandExecute(this.ShipState, null);
			}
			if (flag3)
			{
				ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is CancelMatchVelocityCommand).OnCommandExecute(this.ShipState, null);
			}
			if (flag4)
			{
				ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is CancelSpinPortCommand).OnCommandExecute(this.ShipState, null);
			}
			if (flag5)
			{
				ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is CancelSpinStarboardCommand).OnCommandExecute(this.ShipState, null);
			}
			if (flag6)
			{
				ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is CancelDefensiveManeuversCommand).OnCommandExecute(this.ShipState, null);
			}
		}

		// Token: 0x06005F2A RID: 24362 RVA: 0x002D0A10 File Offset: 0x002CEC10
		private void OnPropulsionValuesUpdated(CombatShipPropulsionValuesUpdated e)
		{
			this._waypointNavigationController.CachePropulsionValues(this.acceleration, this.cruiseAcceleration, this.angular_acceleration_rads2, this.max_angular_velocity_rads_s);
		}

		// Token: 0x06005F2B RID: 24363 RVA: 0x002D0A38 File Offset: 0x002CEC38
		public void ShipDepartureCleanup()
		{
			this._waypointNavigationController.CleanUpWaypoints();
			foreach (CombatShipController combatShipController in base.combatMgr.activeShips)
			{
				if (combatShipController.primaryTarget == this)
				{
					combatShipController.ShipState.faction.playerControl.StartAction(new ClearPrimaryTargetAction(combatShipController.ShipState));
					foreach (IWeapon weapon in combatShipController.hull.IterateByClass<IWeapon>())
					{
						if (weapon.currentFireMode.mode == FireMode.Focus)
						{
							if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Offense))
							{
								combatShipController.ShipState.faction.playerControl.StartAction(new SetWeaponModeAction(combatShipController.ShipState, weapon as Weapon, FireMode.Offense));
							}
						}
					}
					GameControl.eventManager.TriggerEvent(new ShipPrimaryTargetDestroyed(combatShipController.ShipState), null, new object[] { combatShipController.ShipState });
				}
				if (combatShipController.maneuverTarget == this)
				{
					combatShipController.ShipState.faction.playerControl.StartAction(new ClearManeuverTargetAction(combatShipController.ShipState));
					GameControl.eventManager.TriggerEvent(new ShipManuverTargetDestroyed(combatShipController.ShipState), null, new object[] { combatShipController.ShipState });
				}
			}
			this._waypointNavigationController.OnShipDestructionTriggered();
		}

		// Token: 0x06005F2C RID: 24364 RVA: 0x002D0C04 File Offset: 0x002CEE04
		public void TriggerShipDestruction(TIGameState killerCombatanat, TIShipWeaponTemplate killerWeapon)
		{
			base.combatMgr.combatState.shipDestroyedTriggers++;
			this.killer = killerCombatanat;
			TIGameState tigameState = this.killer;
			this.killerFaction = ((tigameState != null) ? tigameState.ref_faction : null);
			this.killerWeapon = killerWeapon;
			base.destructionTriggered = true;
			this.ModelController.DeactivateThrusters(true);
			this.ModelController.DeactivateAllVectorThrusters();
			this.ShipState.DeactivateThrusters();
			this.timeOfDeath = new TIDateTime(this.gameTime.currentTime);
			this.positionOfDeath = new Vector3(base.position.x, base.position.y, base.position.z);
			GameControl.eventManager.TriggerEvent(new ShipDestroyed(this.ShipState, this.killer, killerWeapon, this.timeOfDeath), null, Array.Empty<object>());
			GameControl.spaceCombat.PreRemoveShip(this);
			this.DestroyShipVisualization();
			this.ShipDepartureCleanup();
		}

		// Token: 0x06005F2D RID: 24365 RVA: 0x002D0CFC File Offset: 0x002CEEFC
		private void HandleShipBeingDestroyed()
		{
			float num = (float)this.gameTime.currentTime.DifferenceInSeconds(this.timeOfDeath);
			base.transform.position = this.positionOfDeath + this.velocityVector * num;
		}

		// Token: 0x06005F2E RID: 24366 RVA: 0x002D0D44 File Offset: 0x002CEF44
		private void DestroyShipVisualization()
		{
			Mood.TriggerEvent(Mood.Event.SDKL_Explosion);
			if (this.ModelController.destructionEffectController)
			{
				this.ModelController.destructionEffectController.OnCompleted += this.OnDestructionComplete;
				this.ModelController.StartDestructionSequence();
				return;
			}
			this.ToggleExplosions();
			base.Invoke("DestroyShipParts", 0.75f);
			base.StartCoroutine(this.RemoveShipObject());
		}

		// Token: 0x06005F2F RID: 24367 RVA: 0x002D0DB4 File Offset: 0x002CEFB4
		private void OnDestructionComplete()
		{
			if (this.onDestructionCompleteAlreadyCalled)
			{
				return;
			}
			this.onDestructionCompleteAlreadyCalled = true;
			this.RemoveListeners();
			GameControl.spaceCombat.DestroyShip(this, this.killer, this.killerFaction, this.killerWeapon);
		}

		// Token: 0x06005F30 RID: 24368 RVA: 0x002D0DE9 File Offset: 0x002CEFE9
		public void FinishUpImmediately()
		{
			if (base.destructionTriggered)
			{
				this.OnDestructionComplete();
				ShipModelController modelController = this.ModelController;
				if (modelController == null)
				{
					return;
				}
				modelController.OnDestructionComplete();
			}
		}

		// Token: 0x06005F31 RID: 24369 RVA: 0x002D0E0C File Offset: 0x002CF00C
		private void ToggleExplosions()
		{
			foreach (ParticleSystem particleSystem in this.ModelController.smallExplosionParticleSystems)
			{
				particleSystem.gameObject.SetActive(false);
				particleSystem.transform.localPosition = new Vector3(particleSystem.transform.localPosition.x - 0.1f + TIUtilities.RandomRange(0f, 0.2f), particleSystem.transform.localPosition.y - 0.1f + TIUtilities.RandomRange(0f, 0.2f), particleSystem.transform.localPosition.z - 0.2f + TIUtilities.RandomRange(0f, 0.4f));
				particleSystem.transform.localScale = particleSystem.transform.localScale * TIUtilities.RandomRange(0.5f, 1.5f);
				base.StartCoroutine(this.Boom(particleSystem, TIUtilities.RandomRange(0f, 2f)));
				base.StartCoroutine(this.Boom(particleSystem, TIUtilities.RandomRange(2f, 4f)));
			}
		}

		// Token: 0x06005F32 RID: 24370 RVA: 0x002D0F64 File Offset: 0x002CF164
		private IEnumerator Boom(ParticleSystem explosion, float delay)
		{
			yield return new WaitForSeconds(delay);
			while (explosion.isPlaying)
			{
				yield return null;
			}
			explosion.gameObject.SetActive(false);
			explosion.gameObject.SetActive(true);
			yield break;
		}

		// Token: 0x06005F33 RID: 24371 RVA: 0x002D0F7C File Offset: 0x002CF17C
		private void DestroyShipParts()
		{
			this.ModelController.destructionExplosionParticleSystem.transform.localScale = this.ModelController.destructionExplosionParticleSystem.transform.localScale * TIUtilities.RandomRange(2f, 3f);
			this.ModelController.destructionExplosionParticleSystem.gameObject.SetActive(true);
			this.ModelController.OnDestructionStart();
			this.visualizationOff = true;
		}

		// Token: 0x06005F34 RID: 24372 RVA: 0x002D0FEF File Offset: 0x002CF1EF
		private IEnumerator RemoveShipObject()
		{
			while (!this.visualizationOff)
			{
				yield return null;
			}
			while (this.ModelController.destructionExplosionParticleSystem.isPlaying)
			{
				yield return null;
			}
			this.OnDestructionComplete();
			this.ModelController.OnDestructionComplete();
			yield break;
		}

		// Token: 0x06005F35 RID: 24373 RVA: 0x002D1000 File Offset: 0x002CF200
		public void ReturnToStrategyLayerFleet()
		{
			if (!base.destructionTriggered)
			{
				foreach (ShipWeaponVisController shipWeaponVisController in this.ModelController.allWeaponControllers)
				{
					shipWeaponVisController.ClearTarget();
					shipWeaponVisController.RotateToTarget(true);
				}
				this.ModelController.ResetManeuverCommandUI();
				this.ModelController.DeactivateThrusters(false);
				this.ModelController.DeactivateAllVectorThrusters();
				this.ModelController.transform.localRotation = Quaternion.identity;
				this.shipVisualizerTransform.SetParent(this.StrategyShipControllerTransform);
				this.shipVisualizerTransform.transform.SetPositionAndRotation(this.localPositionToRestore, Quaternion.identity);
				this.shipVisualizerTransform.transform.localScale = this.localScaleToRestore;
				this.shipVisualizerTransform.GetComponent<ShipVisController>().transform.localRotation = Quaternion.identity;
				Vector3d fleetFormationOffset = this.visualizationController.shipState.fleetFormationOffset;
				this.visualizationController.transform.localPosition = (Vector3)fleetFormationOffset;
				this.visualizationController.transform.localScale = Vector3.one;
				Debug.Log(TIUtilities.CombineStrings(new string[]
				{
					"ship returning to strategy layer: ",
					this.ShipState.ID.ToString(),
					", ",
					this.ShipState.displayName
				}));
			}
			this.RemoveListeners();
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06005F36 RID: 24374 RVA: 0x002D1194 File Offset: 0x002CF394
		public void UpdateShip()
		{
			if (!this.initialized || this.departed)
			{
				return;
			}
			if (base.destructionTriggered)
			{
				this.HandleShipBeingDestroyed();
				return;
			}
			if (this.primaryTarget != null && (this.primaryTarget.isDestroyed || this.primaryTarget.destructionTriggered))
			{
				this.primaryTarget = null;
			}
			if (this.maneuverTarget != null && (this.maneuverTarget.isDestroyed || this.maneuverTarget.destructionTriggered))
			{
				this.maneuverTarget = null;
			}
			this.HandleActiveShipManeuvers();
			this.HandleShipStandardUpdate(base.destructionTriggered);
			this.ModelController.UpdateReticle();
			this.UpdateIsMissileSaturated();
		}

		// Token: 0x06005F37 RID: 24375 RVA: 0x002D1244 File Offset: 0x002CF444
		private void HandleShipStandardUpdate(bool destructionTriggered)
		{
			TIDateTime currentTime = this.gameTime.currentTime;
			this.UpdateWaypoints(currentTime);
			if (!destructionTriggered)
			{
				this.UpdateThrusterVisuals(currentTime);
			}
			if (this.gameTime.currentSpeedIndex > 0)
			{
				this.UpdateShipPositioning(currentTime);
			}
		}

		// Token: 0x06005F38 RID: 24376 RVA: 0x002D1284 File Offset: 0x002CF484
		private Dictionary<CombatManeuver, bool> GetManeuverStates()
		{
			Dictionary<CombatManeuver, bool> dictionary = new Dictionary<CombatManeuver, bool>();
			IEnumerable<CombatManeuver> enumerable = this.ShipState.activeCombatManeuvers.Where<CombatManeuver>((CombatManeuver m) => !this._handledManeuvers.Contains(m));
			IEnumerable<CombatManeuver> enumerable2 = this._handledManeuvers.Where<CombatManeuver>((CombatManeuver m) => !this.ShipState.activeCombatManeuvers.Contains(m));
			if (this.ShipState.activeCombatManeuvers.Contains(CombatManeuver.AllStop))
			{
				dictionary[CombatManeuver.AllStop] = true;
			}
			if (this.ShipState.activeCombatManeuvers.Contains(CombatManeuver.MatchVelocity))
			{
				dictionary[CombatManeuver.MatchVelocity] = true;
			}
			if (this.ShipState.activeCombatManeuvers.Contains(CombatManeuver.DefensiveManuevers))
			{
				dictionary[CombatManeuver.DefensiveManuevers] = true;
			}
			foreach (CombatManeuver combatManeuver in enumerable)
			{
				dictionary[combatManeuver] = true;
			}
			foreach (CombatManeuver combatManeuver2 in enumerable2)
			{
				dictionary[combatManeuver2] = false;
			}
			return dictionary;
		}

		// Token: 0x06005F39 RID: 24377 RVA: 0x002D1398 File Offset: 0x002CF598
		private void HandleActiveShipManeuvers()
		{
			AccelerationConstraints accelerationConstraints = this.GetAccelerationConstraints();
			if (this.ShipState.combatAIControl)
			{
				accelerationConstraints = this.GetDVConservingAccelerationConstraints(true);
				this.GetDVConservingAccelerationConstraints(false);
			}
			bool flag = false;
			Dictionary<CombatManeuver, bool> maneuverStates = this.GetManeuverStates();
			this._handledManeuvers.RemoveAll((CombatManeuver x) => !this.ShipState.activeCombatManeuvers.Contains(x));
			if (this.primaryTarget != this._waypointNavigationController.PrimaryTarget)
			{
				this._waypointNavigationController.PrimaryTarget = this.primaryTarget;
			}
			if (this.maneuverTarget != this._waypointNavigationController.ManeuverTarget)
			{
				this._waypointNavigationController.ManeuverTarget = this.maneuverTarget;
			}
			foreach (CombatManeuver combatManeuver in this.ShipState.activeCombatManeuvers)
			{
				if (!this._handledManeuvers.Contains(combatManeuver))
				{
					this._handledManeuvers.Add(combatManeuver);
				}
			}
			this.ShipState.activeCombatManeuvers.RemoveAll((CombatManeuver x) => x == CombatManeuver.Roll180 || x == CombatManeuver.Roll90Port || x == CombatManeuver.Roll90Starboard);
			Quaternion quaternion = Quaternion.identity;
			foreach (KeyValuePair<CombatManeuver, bool> keyValuePair in maneuverStates)
			{
				switch (keyValuePair.Key)
				{
				case CombatManeuver.Padlock:
					if (keyValuePair.Value)
					{
						this.ResetWaypoints();
						this._waypointNavigationController.PadlockEnabled = true;
					}
					else
					{
						this._waypointNavigationController.PadlockEnabled = false;
					}
					break;
				case CombatManeuver.Roll90Port:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= Quaternion.Euler(0f, 0f, 90f);
					}
					break;
				case CombatManeuver.Roll90Starboard:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= Quaternion.Euler(0f, 0f, -90f);
					}
					break;
				case CombatManeuver.Roll180:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= Quaternion.Euler(0f, 0f, 180f);
					}
					break;
				case CombatManeuver.SpinDorsal:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= this._spinDorsalRotation;
					}
					else
					{
						quaternion *= Quaternion.Inverse(this._spinDorsalRotation);
					}
					break;
				case CombatManeuver.SpinVentral:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= this._spinVentralRotation;
					}
					else
					{
						quaternion *= Quaternion.Inverse(this._spinVentralRotation);
					}
					break;
				case CombatManeuver.SpinPort:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= this._spinPortRotation;
					}
					else
					{
						quaternion *= Quaternion.Inverse(this._spinPortRotation);
					}
					break;
				case CombatManeuver.SpinStarboard:
					flag = true;
					if (keyValuePair.Value)
					{
						quaternion *= this._spinStarboardRotation;
					}
					else
					{
						quaternion *= Quaternion.Inverse(this._spinStarboardRotation);
					}
					break;
				case CombatManeuver.AllStop:
					if (keyValuePair.Value)
					{
						this._waypointNavigationController.AllStop(accelerationConstraints);
					}
					else
					{
						this._waypointNavigationController.CancelAllStop();
					}
					break;
				case CombatManeuver.FullSpeedAhead:
					if (keyValuePair.Value)
					{
						this._waypointNavigationController.FullSpeedAhead(accelerationConstraints);
					}
					break;
				case CombatManeuver.InterceptCourse:
					if (keyValuePair.Value)
					{
						this._waypointNavigationController.InterceptCourse(accelerationConstraints);
					}
					break;
				case CombatManeuver.MatchVelocity:
					if (keyValuePair.Value)
					{
						this._waypointNavigationController.MatchVelocity();
					}
					else
					{
						this._waypointNavigationController.CancelMatchVelocity();
						this.ShipState.faction.playerControl.StartAction(new ClearManeuverTargetAction(this.ShipState));
					}
					break;
				case CombatManeuver.DefensiveManuevers:
					if (keyValuePair.Value)
					{
						if (!(this.gameTime.currentTime < this.nextDefensiveManueverUpdateTime))
						{
							if (!this.inDefensiveManuever)
							{
								this.inDefensiveManuever = true;
								this.BeginDefensiveManuevers();
							}
							List<ProjectileController> allThreateningProjectiles = this.GetAllThreateningProjectiles();
							this.FilterForImminentImpactThreats(ref allThreateningProjectiles);
							this.TryAssignDefensivePosition(allThreateningProjectiles, this.GetAccelerationConstraints());
							this.nextDefensiveManueverUpdateTime = this.TimeOfNextWaypoint;
						}
					}
					else
					{
						this.CancelDefensiveManuevers();
					}
					break;
				case CombatManeuver.FaceVelocityVector:
					if (keyValuePair.Value)
					{
						this._waypointNavigationController.FaceVelocityVector();
					}
					break;
				}
			}
			if (flag)
			{
				this._waypointNavigationController.SetAppendWaypointRotation(quaternion);
			}
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x002D185C File Offset: 0x002CFA5C
		private void UpdateThrusterVisuals(TIDateTime currentTime)
		{
			bool flag = this._waypointNavigationController.IsInBurn(currentTime);
			if (flag && !this.thrusting)
			{
				this.thrusting = true;
				this.ShipState.ActivateThrusters();
				this.ModelController.ActivateThrusters(true);
			}
			else if (!flag && this.thrusting)
			{
				this.thrusting = false;
				this.ShipState.DeactivateThrusters();
				this.ModelController.DeactivateThrusters(false);
			}
			this.HandleRightLeftAcceleration(currentTime);
			this.HandleUpDownAcceleration(currentTime);
			this.HandleRollAcceleration(currentTime);
		}

		// Token: 0x06005F3B RID: 24379 RVA: 0x002D18E0 File Offset: 0x002CFAE0
		private void HandleRightLeftAcceleration(TIDateTime currentTime)
		{
			HoldTrajectory trajectoryAtTime = this._waypointNavigationController.GetTrajectoryAtTime(currentTime);
			if (trajectoryAtTime is DriftTrajectory)
			{
				if (this.horizontalAccelerationState != AxisState.Drifting)
				{
					this.ModelController.DeactivateRightTurnVectorThrusters();
					this.ModelController.DeactivateLeftTurnVectorThrusters();
					this.horizontalAccelerationState = AxisState.Drifting;
				}
				this.EvaluateRightLeftDrift(currentTime);
				return;
			}
			if (trajectoryAtTime is RotationTrajectory)
			{
				if (this.horizontalAccelerationState != AxisState.Rotating)
				{
					this.ModelController.DeactivateSlideRightVectorThrusters();
					this.ModelController.DeactivateSlideLeftVectorThrusters();
					this.horizontalAccelerationState = AxisState.Rotating;
				}
				this.EvaluateRightLeftRotation(currentTime);
				return;
			}
			if (this.horizontalAccelerationState == AxisState.Drifting)
			{
				this.ModelController.DeactivateSlideRightVectorThrusters();
				this.ModelController.DeactivateSlideLeftVectorThrusters();
				this.horizontalAccelerationState = AxisState.None;
			}
			else if (this.horizontalAccelerationState == AxisState.Rotating)
			{
				this.ModelController.DeactivateRightTurnVectorThrusters();
				this.ModelController.DeactivateLeftTurnVectorThrusters();
				this.horizontalAccelerationState = AxisState.None;
			}
			this.acceleratingRight = false;
			this.acceleratingLeft = false;
			this.acceleratingDown = false;
			this.acceleratingUp = false;
		}

		// Token: 0x06005F3C RID: 24380 RVA: 0x002D19D4 File Offset: 0x002CFBD4
		private bool EvaluateRightLeftDrift(TIDateTime currentTime)
		{
			bool flag = this._waypointNavigationController.IsAcceleratingRight(currentTime);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = this._waypointNavigationController.IsAcceleratingLeft(currentTime);
			}
			if (!this.acceleratingRight && flag)
			{
				this.ModelController.ActivateSlideRightVectorThrusters();
				this.acceleratingRight = true;
			}
			else if (!this.acceleratingLeft && flag2)
			{
				this.ModelController.ActivateSlideLeftVectorThrusters();
				this.acceleratingLeft = true;
			}
			if (this.acceleratingRight && !flag)
			{
				this.acceleratingRight = false;
				this.ModelController.DeactivateSlideRightVectorThrusters();
			}
			if (this.acceleratingLeft && !flag2)
			{
				this.acceleratingLeft = false;
				this.ModelController.DeactivateSlideLeftVectorThrusters();
			}
			return this.acceleratingRight || this.acceleratingLeft;
		}

		// Token: 0x06005F3D RID: 24381 RVA: 0x002D1A8C File Offset: 0x002CFC8C
		private bool EvaluateRightLeftRotation(TIDateTime currentTime)
		{
			bool flag = this._waypointNavigationController.IsAcceleratingRight(currentTime);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = this._waypointNavigationController.IsAcceleratingLeft(currentTime);
			}
			if (!this.acceleratingRight && flag)
			{
				if (this.ShipState.faction == GameControl.control.activePlayer)
				{
					this.ModelController.ActivateRightTurnVectorThrusters();
				}
				this.acceleratingRight = true;
			}
			else if (!this.acceleratingLeft && flag2)
			{
				if (this.ShipState.faction == GameControl.control.activePlayer)
				{
					this.ModelController.ActivateLeftTurnVectorThrusters();
				}
				this.acceleratingLeft = true;
			}
			if (this.acceleratingRight && !flag)
			{
				if (this.ShipState.faction == GameControl.control.activePlayer)
				{
					this.acceleratingRight = false;
				}
				this.ModelController.DeactivateRightTurnVectorThrusters();
			}
			if (this.acceleratingLeft && !flag2)
			{
				if (this.ShipState.faction == GameControl.control.activePlayer)
				{
					this.acceleratingLeft = false;
				}
				this.ModelController.DeactivateLeftTurnVectorThrusters();
			}
			return this.acceleratingRight || this.acceleratingLeft;
		}

		// Token: 0x06005F3E RID: 24382 RVA: 0x002D1BB4 File Offset: 0x002CFDB4
		private void HandleUpDownAcceleration(TIDateTime currentTime)
		{
			HoldTrajectory trajectoryAtTime = this._waypointNavigationController.GetTrajectoryAtTime(currentTime);
			if (trajectoryAtTime is DriftTrajectory)
			{
				if (this.verticalAccelerationState != AxisState.Drifting)
				{
					this.ModelController.DeactivatePitchUpVectorThrusters();
					this.ModelController.DeactivatePitchDownVectorThrusters();
					this.verticalAccelerationState = AxisState.Drifting;
				}
				this.EvaluateUpDownDrift(currentTime);
				return;
			}
			if (trajectoryAtTime is RotationTrajectory)
			{
				if (this.verticalAccelerationState != AxisState.Rotating)
				{
					this.ModelController.DeactivateSlideUpVectorThrusters();
					this.ModelController.DeactivateSlideDownVectorThrusters();
					this.verticalAccelerationState = AxisState.Rotating;
				}
				this.EvaluateUpDownRotation(currentTime);
				return;
			}
			if (this.verticalAccelerationState == AxisState.Drifting)
			{
				this.ModelController.DeactivateSlideUpVectorThrusters();
				this.ModelController.DeactivateSlideDownVectorThrusters();
				this.verticalAccelerationState = AxisState.None;
				return;
			}
			if (this.verticalAccelerationState == AxisState.Rotating)
			{
				this.ModelController.DeactivatePitchUpVectorThrusters();
				this.ModelController.DeactivatePitchDownVectorThrusters();
				this.verticalAccelerationState = AxisState.None;
			}
		}

		// Token: 0x06005F3F RID: 24383 RVA: 0x002D1C8C File Offset: 0x002CFE8C
		private bool EvaluateUpDownDrift(TIDateTime currentTime)
		{
			bool flag = this._waypointNavigationController.IsAcceleratingUp(currentTime);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = this._waypointNavigationController.IsAcceleratingDown(currentTime);
			}
			if (!this.acceleratingUp && flag)
			{
				this.ModelController.ActivateSlideUpVectorThrusters();
				this.acceleratingUp = true;
			}
			else if (!this.acceleratingDown && flag2)
			{
				this.ModelController.ActivateSlideDownVectorThrusters();
				this.acceleratingDown = true;
			}
			if (this.acceleratingUp && !flag)
			{
				this.acceleratingUp = false;
				this.ModelController.DeactivateSlideUpVectorThrusters();
			}
			if (this.acceleratingDown && !flag2)
			{
				this.acceleratingDown = false;
				this.ModelController.DeactivateSlideDownVectorThrusters();
			}
			return this.acceleratingUp || this.acceleratingDown;
		}

		// Token: 0x06005F40 RID: 24384 RVA: 0x002D1D44 File Offset: 0x002CFF44
		private bool EvaluateUpDownRotation(TIDateTime currentTime)
		{
			bool flag = this._waypointNavigationController.IsAcceleratingUp(currentTime);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = this._waypointNavigationController.IsAcceleratingDown(currentTime);
			}
			if (!this.acceleratingUp && flag)
			{
				this.ModelController.ActivatePitchUpVectorThrusters();
				this.acceleratingUp = true;
			}
			else if (!this.acceleratingDown && flag2)
			{
				this.ModelController.ActivatePitchDownVectorThrusters();
				this.acceleratingDown = true;
			}
			if (this.acceleratingUp && !flag)
			{
				this.acceleratingUp = false;
				this.ModelController.DeactivatePitchUpVectorThrusters();
			}
			if (this.acceleratingDown && !flag2)
			{
				this.acceleratingDown = false;
				this.ModelController.DeactivatePitchDownVectorThrusters();
			}
			return this.acceleratingUp || this.acceleratingDown;
		}

		// Token: 0x06005F41 RID: 24385 RVA: 0x002D1DFC File Offset: 0x002CFFFC
		private void HandleRollAcceleration(TIDateTime currentTime)
		{
			if (!(this._waypointNavigationController.GetTrajectoryAtTime(currentTime) is RotationTrajectory))
			{
				if (this.rollAccelerationState == AxisState.Rotating)
				{
					this.ModelController.DeactivateRollRightVectorThrusters();
					this.ModelController.DeactivateRollLeftVectorThrusters();
					this.rollAccelerationState = AxisState.None;
				}
				this.acceleratingRollRight = false;
				this.acceleratingRollLeft = false;
				return;
			}
			if (this.rollAccelerationState == AxisState.None)
			{
				this.rollAccelerationState = AxisState.Rotating;
			}
			this.EvaluateRollRotation(currentTime);
		}

		// Token: 0x06005F42 RID: 24386 RVA: 0x002D1E68 File Offset: 0x002D0068
		private bool EvaluateRollRotation(TIDateTime currentTime)
		{
			bool flag = this._waypointNavigationController.IsAcceleratingRollRight(currentTime);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = this._waypointNavigationController.IsAcceleratingRollLeft(currentTime);
			}
			if (!this.acceleratingRollRight && flag)
			{
				this.ModelController.ActivateRollRightVectorThrusters();
				this.acceleratingRollRight = true;
			}
			else if (!this.acceleratingRollLeft && flag2)
			{
				this.ModelController.ActivateRollLeftVectorThrusters();
				this.acceleratingRollLeft = true;
			}
			if (this.acceleratingRollRight && !flag)
			{
				this.acceleratingRollRight = false;
				this.ModelController.DeactivateRollRightVectorThrusters();
			}
			if (this.acceleratingRollLeft && !flag2)
			{
				this.acceleratingRollLeft = false;
				this.ModelController.DeactivateRollLeftVectorThrusters();
			}
			return this.acceleratingRollRight || this.acceleratingRollLeft;
		}

		// Token: 0x06005F43 RID: 24387 RVA: 0x002D1F20 File Offset: 0x002D0120
		private void UpdateShipPositioning(TIDateTime currentTime)
		{
			Transform transform = base.transform;
			Vector3 vector = new Vector3(this.velocityVector.x, this.velocityVector.y, this.velocityVector.z);
			this.velocityVector = this._waypointNavigationController.VelocityAtTime(currentTime);
			float num = this._waypointNavigationController.CurrentAcceleration();
			float num2 = SpaceCombatManager.acceleration_kps(vector, this.velocityVector);
			float angularVelocity_kps = this.angularVelocity_kps;
			float num3 = this._waypointNavigationController.AngularVelocityAt_Rad(currentTime);
			this.angularVelocity_kps = num3 * (this.ShipState.hull.length_m / 1000f * 0.5f);
			float num4 = Mathf.Abs(this.angularVelocity_kps - angularVelocity_kps);
			if (this.angularVelocity_kps > this.ShipState.template.maxDamageControlAngularVelocity_mps / 1000f)
			{
				if (!this.ShipState.isDamageControlSuspended)
				{
					GameControl.eventManager.TriggerEvent(new ShipDamageControlRotationStatusChanged(this.ShipState, false), null, new object[] { this.ShipState });
				}
			}
			else if (this.ShipState.isDamageControlSuspended)
			{
				GameControl.eventManager.TriggerEvent(new ShipDamageControlRotationStatusChanged(this.ShipState, true), null, new object[] { this.ShipState });
			}
			this.ShipState.ConsumeDeltaV(num4, true);
			float num5 = num / 0.05f;
			this.ShipState.RunDriveInCombat(num2, num5, this.ShipState.currentMass_kg);
			transform.rotation = this._waypointNavigationController.RotationAtTime(currentTime);
			transform.localPosition = this._waypointNavigationController.PositionAtTime(currentTime);
		}

		// Token: 0x06005F44 RID: 24388 RVA: 0x002D20A7 File Offset: 0x002D02A7
		public void UpdateActiveWaypointPlacementSegment()
		{
			this._waypointNavigationController.UpdateActiveWaypointPlacementSegment();
		}

		// Token: 0x06005F45 RID: 24389 RVA: 0x002D20B4 File Offset: 0x002D02B4
		public void ClearActiveWaypointPlacementSegment()
		{
			this._waypointNavigationController.ClearActiveWaypointPlacementSegment();
		}

		// Token: 0x06005F46 RID: 24390 RVA: 0x002D20C1 File Offset: 0x002D02C1
		public SegmentProximityData FindNearestSegment()
		{
			return this._waypointNavigationController.FindNearestSegment();
		}

		// Token: 0x06005F47 RID: 24391 RVA: 0x002D20CE File Offset: 0x002D02CE
		public bool UpdateWaypointPlacementLocation()
		{
			return this._waypointNavigationController.UpdateWaypointPlacementLocation();
		}

		// Token: 0x06005F48 RID: 24392 RVA: 0x002D20DB File Offset: 0x002D02DB
		public void FinalizeWaypointPlacement()
		{
			this._waypointNavigationController.FinalizeWaypointPlacement();
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x002D20E8 File Offset: 0x002D02E8
		public string GetGroupMembershipString()
		{
			this.controlGroups = this.controlGroups.OrderBy<int, int>((int x) => x).ToList<int>();
			if (this.controlGroups.Remove(0))
			{
				this.controlGroups.Add(0);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.controlGroups.Count; i++)
			{
				stringBuilder.Append(this.controlGroups[i].ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005F4A RID: 24394 RVA: 0x002D2184 File Offset: 0x002D0384
		public void SetPrimaryTarget(CombatantController newPrimaryTarget)
		{
			this.oldPrimaryTarget = this.primaryTarget;
			this.primaryTarget = newPrimaryTarget;
			if (this.activePlayerShip)
			{
				if (this.oldPrimaryTarget != null)
				{
					(this.oldPrimaryTarget.UIController().combatantListItemController as EnemyShipListItemController).OnPrimaryTargetSelected();
				}
				if (this.primaryTarget != null)
				{
					(this.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).OnPrimaryTargetSelected();
				}
				if (this.primaryTarget == null && this.oldPrimaryTarget != null)
				{
					(this.oldPrimaryTarget.UIController().combatantListItemController as EnemyShipListItemController).ClearPrimaryTarget();
				}
			}
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x002D2238 File Offset: 0x002D0438
		public void SetManeuverTarget(CombatantController newManeuverTarget)
		{
			this.oldManeuverTarget = this.maneuverTarget;
			this.maneuverTarget = newManeuverTarget;
			if (this.activePlayerShip)
			{
				if (this.oldManeuverTarget != null)
				{
					this.oldManeuverTarget.UIController().combatantListItemController.OnManeuverTargetSelected();
				}
				if (this.maneuverTarget != null)
				{
					this.maneuverTarget.UIController().combatantListItemController.OnManeuverTargetSelected();
				}
				if (this.maneuverTarget == null && this.oldManeuverTarget != null)
				{
					this.oldManeuverTarget.UIController().combatantListItemController.ClearManeuverTarget();
				}
			}
		}

		// Token: 0x06005F4C RID: 24396 RVA: 0x002D22D8 File Offset: 0x002D04D8
		private void OnTriggerEnter(Collider other)
		{
			CombatShipController componentInParent = other.gameObject.GetComponentInParent<CombatShipController>();
			if (componentInParent != null)
			{
				this._waypointNavigationController.AddAgentShipController(componentInParent);
				return;
			}
			HabModuleController componentInParent2 = other.gameObject.GetComponentInParent<HabModuleController>();
			if (componentInParent2 != null)
			{
				this._waypointNavigationController.AddHabModuleController(componentInParent2, other);
			}
		}

		// Token: 0x06005F4D RID: 24397 RVA: 0x002D232C File Offset: 0x002D052C
		private void OnTriggerExit(Collider other)
		{
			CombatShipController componentInParent = other.gameObject.GetComponentInParent<CombatShipController>();
			if (componentInParent != null)
			{
				this._waypointNavigationController.RemoveAgentShipController(componentInParent);
				return;
			}
			HabModuleController componentInParent2 = other.gameObject.GetComponentInParent<HabModuleController>();
			if (componentInParent2 != null)
			{
				this._waypointNavigationController.RemoveHabModuleController(componentInParent2);
			}
		}

		// Token: 0x06005F4E RID: 24398 RVA: 0x002D237C File Offset: 0x002D057C
		private void OnCollisionEnter(Collision other)
		{
			CombatShipController componentInParent = other.gameObject.GetComponentInParent<CombatShipController>();
			if (componentInParent != null && componentInParent == this.primaryTarget)
			{
				if (this.ShipState.currentMass_tons <= componentInParent.ShipState.currentMass_tons)
				{
					componentInParent.faction.UnlockAchievement("ramming");
					if (!componentInParent.destructionTriggered && !componentInParent.isDestroyed)
					{
						CollisionImpact collisionImpact = new CollisionImpact(other.GetContact(0).point, this, componentInParent);
						componentInParent.ApplyDamage(collisionImpact);
					}
					if (!base.destructionTriggered)
					{
						float num;
						this.ShipState.ApplyDamageToSystem(ShipSystem.NoseStructure, 10000f, out num);
						float num2;
						this.ShipState.ApplyDamageToSystem(ShipSystem.CentralStructure, 10000f, out num2);
						float num3;
						this.ShipState.ApplyDamageToSystem(ShipSystem.TailStructure, 10000f, out num3);
						this.TriggerShipDestruction(componentInParent.ShipState, null);
						return;
					}
				}
				else
				{
					this.ShipState.faction.UnlockAchievement("ramming");
					if (!base.isDestroyed && !base.destructionTriggered)
					{
						CollisionImpact collisionImpact2 = new CollisionImpact(other.contacts[0].point, componentInParent, this);
						this.ApplyDamage(collisionImpact2);
					}
					if (!componentInParent.destructionTriggered)
					{
						float num4;
						componentInParent.ShipState.ApplyDamageToSystem(ShipSystem.NoseStructure, 10000f, out num4);
						float num5;
						componentInParent.ShipState.ApplyDamageToSystem(ShipSystem.CentralStructure, 10000f, out num5);
						float num6;
						componentInParent.ShipState.ApplyDamageToSystem(ShipSystem.TailStructure, 10000f, out num6);
						componentInParent.TriggerShipDestruction(this.ShipState, null);
					}
				}
			}
		}

		// Token: 0x040043A1 RID: 17313
		private bool initialized;

		// Token: 0x040043A2 RID: 17314
		private GameTimeManager gameTime;

		// Token: 0x040043A3 RID: 17315
		private Transform StrategyShipControllerTransform;

		// Token: 0x040043A4 RID: 17316
		private Transform shipVisualizerTransform;

		// Token: 0x040043A5 RID: 17317
		private Vector3 localScaleToRestore;

		// Token: 0x040043A6 RID: 17318
		private Vector3 localPositionToRestore;

		// Token: 0x040043AC RID: 17324
		private AxisState horizontalAccelerationState;

		// Token: 0x040043AD RID: 17325
		private AxisState verticalAccelerationState;

		// Token: 0x040043AE RID: 17326
		private AxisState rollAccelerationState;

		// Token: 0x040043AF RID: 17327
		private bool acceleratingRight;

		// Token: 0x040043B0 RID: 17328
		private bool acceleratingLeft;

		// Token: 0x040043B1 RID: 17329
		private bool acceleratingUp;

		// Token: 0x040043B2 RID: 17330
		private bool acceleratingDown;

		// Token: 0x040043B3 RID: 17331
		private bool acceleratingRollRight;

		// Token: 0x040043B4 RID: 17332
		private bool acceleratingRollLeft;

		// Token: 0x040043B5 RID: 17333
		private bool visualizationOff;

		// Token: 0x040043B6 RID: 17334
		public bool departed;

		// Token: 0x040043B7 RID: 17335
		private TIDateTime timeOfDeath;

		// Token: 0x040043B8 RID: 17336
		private Vector3 positionOfDeath;

		// Token: 0x040043B9 RID: 17337
		private Vector3 initialVelocity;

		// Token: 0x040043BA RID: 17338
		private Quaternion _spinPortRotation;

		// Token: 0x040043BB RID: 17339
		private Quaternion _spinStarboardRotation;

		// Token: 0x040043BC RID: 17340
		private Quaternion _spinVentralRotation;

		// Token: 0x040043BD RID: 17341
		private Quaternion _spinDorsalRotation;

		// Token: 0x040043BE RID: 17342
		private Collider rootCollider;

		// Token: 0x040043BF RID: 17343
		private List<CombatManeuver> _handledManeuvers = new List<CombatManeuver>();

		// Token: 0x040043C0 RID: 17344
		public WaypointNavigationController _waypointNavigationController;

		// Token: 0x040043C8 RID: 17352
		public List<int> controlGroups = new List<int>();

		// Token: 0x040043C9 RID: 17353
		private bool inDefensiveManuever;

		// Token: 0x040043CA RID: 17354
		private TIDateTime nextDefensiveManueverUpdateTime;

		// Token: 0x040043CB RID: 17355
		private TIFactionState cachedFaction;

		// Token: 0x040043CC RID: 17356
		public bool AI_IsMissileBoat;

		// Token: 0x040043CD RID: 17357
		private Vector3 _cachedAccelerationVector;

		// Token: 0x040043CE RID: 17358
		private Vector3? _boundingBoxSize;

		// Token: 0x040043CF RID: 17359
		private TIDateTime lastTimeChecked = new TIDateTime();

		// Token: 0x040043D0 RID: 17360
		private TIGameState killer;

		// Token: 0x040043D1 RID: 17361
		private TIFactionState killerFaction;

		// Token: 0x040043D2 RID: 17362
		private TIShipWeaponTemplate killerWeapon;

		// Token: 0x040043D3 RID: 17363
		private bool onDestructionCompleteAlreadyCalled;
	}
}
