using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000587 RID: 1415
	public abstract class ShipModelController : MonoBehaviour
	{
		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06002542 RID: 9538 RVA: 0x000C8996 File Offset: 0x000C6B96
		private ParticleSystem frontRightVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[0];
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06002543 RID: 9539 RVA: 0x000C89A0 File Offset: 0x000C6BA0
		private ParticleSystem frontLeftVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[1];
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06002544 RID: 9540 RVA: 0x000C89AA File Offset: 0x000C6BAA
		private ParticleSystem backRightVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[2];
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06002545 RID: 9541 RVA: 0x000C89B4 File Offset: 0x000C6BB4
		private ParticleSystem backLeftVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[3];
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06002546 RID: 9542 RVA: 0x000C89BE File Offset: 0x000C6BBE
		private ParticleSystem frontDorsalVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[4];
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06002547 RID: 9543 RVA: 0x000C89C8 File Offset: 0x000C6BC8
		private ParticleSystem frontVentralVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[5];
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06002548 RID: 9544 RVA: 0x000C89D2 File Offset: 0x000C6BD2
		private ParticleSystem backDorsalVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[6];
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06002549 RID: 9545 RVA: 0x000C89DC File Offset: 0x000C6BDC
		private ParticleSystem backVentralVectorThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[7];
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x0600254A RID: 9546 RVA: 0x000C89E6 File Offset: 0x000C6BE6
		private ParticleSystem forwardRollRightThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[8];
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x0600254B RID: 9547 RVA: 0x000C89F0 File Offset: 0x000C6BF0
		private ParticleSystem forwardRollLeftThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[9];
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x0600254C RID: 9548 RVA: 0x000C89FB File Offset: 0x000C6BFB
		private ParticleSystem rearRollRightThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[10];
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x0600254D RID: 9549 RVA: 0x000C8A06 File Offset: 0x000C6C06
		private ParticleSystem rearRollLeftThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[11];
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x0600254E RID: 9550 RVA: 0x000C8A11 File Offset: 0x000C6C11
		private ParticleSystem forwardCounterRollRightThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[12];
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x0600254F RID: 9551 RVA: 0x000C8A1C File Offset: 0x000C6C1C
		private ParticleSystem forwardCounterRollLeftThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[13];
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06002550 RID: 9552 RVA: 0x000C8A27 File Offset: 0x000C6C27
		private ParticleSystem rearCounterRollRightThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[14];
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06002551 RID: 9553 RVA: 0x000C8A32 File Offset: 0x000C6C32
		private ParticleSystem rearCounterRollLeftThrusterEffect
		{
			get
			{
				return this.vectorThrusterEffect[15];
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06002552 RID: 9554 RVA: 0x000C8A3D File Offset: 0x000C6C3D
		// (set) Token: 0x06002553 RID: 9555 RVA: 0x000C8A45 File Offset: 0x000C6C45
		public bool RadiatorsEmitting { get; protected set; }

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06002554 RID: 9556 RVA: 0x000C8A4E File Offset: 0x000C6C4E
		// (set) Token: 0x06002555 RID: 9557 RVA: 0x000C8A56 File Offset: 0x000C6C56
		public bool RadiatorsExtended { get; protected set; }

		// Token: 0x06002556 RID: 9558
		public abstract void SetRadiators(TISpaceShipTemplate ship);

		// Token: 0x06002557 RID: 9559
		public abstract List<GameObject> WhichRadiators(TISpaceShipTemplate ship);

		// Token: 0x06002558 RID: 9560
		public abstract void SetSkin(TISpaceShipTemplate ship);

		// Token: 0x06002559 RID: 9561
		public abstract int SlotToWeaponMountIndex(int slot, Mount mount);

		// Token: 0x0600255A RID: 9562 RVA: 0x000C8A5F File Offset: 0x000C6C5F
		public Vector3 GetMouseColliderDimensions(TIShipHullTemplate hull)
		{
			return new Vector3(hull.width_m, hull.length_m, 0f);
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x0600255B RID: 9563 RVA: 0x000C8A77 File Offset: 0x000C6C77
		// (set) Token: 0x0600255C RID: 9564 RVA: 0x000C8A7F File Offset: 0x000C6C7F
		public MarkerController.MarkerAnimations currentSelectionAnimation { get; private set; }

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x0600255D RID: 9565 RVA: 0x000C8A88 File Offset: 0x000C6C88
		public List<ShipWeaponVisController> allWeaponControllers
		{
			get
			{
				return this.noseWeaponControllers.Concat<ShipWeaponVisController>(this.dorsalHullWeaponControllers).Concat<ShipWeaponVisController>(this.ventralHullWeaponControllers).ToList<ShipWeaponVisController>();
			}
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x000C8AAC File Offset: 0x000C6CAC
		public void UpdateReticle()
		{
			if (!this.selectionAnimObject.activeSelf && !this.groupSelectionAnimObject.activeSelf && !this.padlockIconObject.activeSelf)
			{
				return;
			}
			if (this.mainCamT == null)
			{
				this.mainCamT = Camera.main.transform;
			}
			float num = Vector3.Distance(this.selectionAnimObject.transform.position, this.mainCamT.position);
			if (this.selectionAnimObject.activeSelf)
			{
				this.selectionAnimObject.transform.LookAt(this.mainCamT.position);
				this.selectionAnimObject.transform.localScale = this.baseScale + new Vector3(num * this.modScale, num * this.modScale, num * this.modScale);
			}
			if (this.groupSelectionAnimObject.activeSelf)
			{
				this.groupSelectionAnimObject.transform.LookAt(this.mainCamT.position);
				this.groupSelectionAnimObject.transform.localScale = this.baseScale * 1.33f + new Vector3(num * this.modScale, num * this.modScale, num * this.modScale);
			}
			if (this.padlockIconObject.activeSelf)
			{
				this.padlockIconObject.transform.LookAt(this.mainCamT.position);
				this.padlockIconObject.transform.localScale = 1.66f * new Vector3(1f + num * this.modScale, 1f + num * this.modScale, 1f + num * this.modScale);
			}
		}

		// Token: 0x0600255F RID: 9567 RVA: 0x000C8C5B File Offset: 0x000C6E5B
		public void ResetManeuverCommandUI()
		{
			this.padlockIconObject.SetActive(false);
		}

		// Token: 0x06002560 RID: 9568 RVA: 0x000C8C69 File Offset: 0x000C6E69
		public void InitDamageLayer()
		{
			this.ship.InitDamageLayer(this.damageLayer);
		}

		// Token: 0x06002561 RID: 9569 RVA: 0x000C8C7C File Offset: 0x000C6E7C
		public TISpaceShipState GetRefShipState()
		{
			return this.ship;
		}

		// Token: 0x06002562 RID: 9570 RVA: 0x000C8C84 File Offset: 0x000C6E84
		public void ActivateThrusters(bool playAudio)
		{
			for (int i = 0; i < this.thrusters; i++)
			{
				this.thrusterEffectContainers[i].Play();
			}
			if (playAudio)
			{
				if (!this.eventInstance.isValid() && TIGlobalValuesState.isSpaceCombatEnabled)
				{
					this.eventInstance = AudioManager.CreateFMODInstance(this.ship.ThrusterSFXString());
					if (TIGlobalValuesState.isSpaceCombatEnabled)
					{
						this.eventInstance.SetDistance(AudioManager.GetCombatAudioMaxDistance(this.eventInstance), 1f);
					}
				}
				this.PlayThrusterAudio();
				return;
			}
			this.StopThrusterAudio();
		}

		// Token: 0x06002563 RID: 9571 RVA: 0x000C8D14 File Offset: 0x000C6F14
		public void DeactivateThrusters(bool alsoObjects = false)
		{
			for (int i = 0; i < this.thrusters; i++)
			{
				this.thrusterEffectContainers[i].Stop();
				if (alsoObjects)
				{
					foreach (GameObject gameObject in this.thrusterLocations)
					{
						if (gameObject.transform.childCount > 0)
						{
							gameObject.transform.GetChild(0).gameObject.SetActive(false);
						}
					}
				}
			}
			this.StopThrusterAudio();
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x000C8D90 File Offset: 0x000C6F90
		public void PlayThrusterAudio()
		{
			if (this.eventInstance.isValid() && !this.eventInstance.IsPlaying())
			{
				RuntimeManager.AttachInstanceToGameObject(this.eventInstance, base.transform);
				this.eventInstance.Play(base.gameObject);
			}
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x000C8DCF File Offset: 0x000C6FCF
		public void StopThrusterAudio()
		{
			if (this.eventInstance.isValid() && this.eventInstance.IsPlaying())
			{
				this.eventInstance.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}

		// Token: 0x06002566 RID: 9574 RVA: 0x000C8DF8 File Offset: 0x000C6FF8
		public ParticleSystem ActivateRandomThruster()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			int num = global::UnityEngine.Random.Range(0, 12);
			ParticleSystem particleSystem = null;
			switch (num)
			{
			case 0:
				particleSystem = this.backRightVectorThrusterEffect;
				break;
			case 1:
				particleSystem = this.frontLeftVectorThrusterEffect;
				break;
			case 2:
				particleSystem = this.backLeftVectorThrusterEffect;
				break;
			case 3:
				particleSystem = this.frontRightVectorThrusterEffect;
				break;
			case 4:
				particleSystem = this.frontDorsalVectorThrusterEffect;
				break;
			case 5:
				particleSystem = this.frontVentralVectorThrusterEffect;
				break;
			case 6:
				particleSystem = this.backDorsalVectorThrusterEffect;
				break;
			case 7:
				particleSystem = this.backVentralVectorThrusterEffect;
				break;
			case 8:
				particleSystem = this.forwardRollRightThrusterEffect;
				break;
			case 9:
				particleSystem = this.forwardRollLeftThrusterEffect;
				break;
			case 10:
				particleSystem = this.rearRollRightThrusterEffect;
				break;
			case 11:
				particleSystem = this.rearRollLeftThrusterEffect;
				break;
			}
			if (particleSystem.isStopped)
			{
				particleSystem.Play();
			}
			return particleSystem;
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x000C8EF0 File Offset: 0x000C70F0
		public void ActivateLeftTurnVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backRightVectorThrusterEffect == null || this.frontDorsalVectorThrusterEffect == null)
			{
				Debug.LogWarning("Left Turn Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backRightVectorThrusterEffect.isStopped)
			{
				this.backRightVectorThrusterEffect.Play();
			}
			if (this.frontLeftVectorThrusterEffect.isStopped)
			{
				this.frontLeftVectorThrusterEffect.Play();
			}
		}

		// Token: 0x06002568 RID: 9576 RVA: 0x000C8F94 File Offset: 0x000C7194
		public void DeactivateLeftTurnVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backRightVectorThrusterEffect == null || this.frontLeftVectorThrusterEffect == null)
			{
				Debug.LogWarning("Left Turn Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backRightVectorThrusterEffect.isPlaying)
			{
				this.backRightVectorThrusterEffect.Stop();
			}
			if (this.frontLeftVectorThrusterEffect.isPlaying)
			{
				this.frontLeftVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x06002569 RID: 9577 RVA: 0x000C9014 File Offset: 0x000C7214
		public void ActivateRightTurnVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backLeftVectorThrusterEffect == null || this.frontRightVectorThrusterEffect == null)
			{
				Debug.LogWarning("Right Turn Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backLeftVectorThrusterEffect.isStopped)
			{
				this.backLeftVectorThrusterEffect.Play();
			}
			if (this.frontRightVectorThrusterEffect.isStopped)
			{
				this.frontRightVectorThrusterEffect.Play();
			}
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x000C90B8 File Offset: 0x000C72B8
		public void DeactivateRightTurnVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backLeftVectorThrusterEffect == null || this.frontRightVectorThrusterEffect == null)
			{
				Debug.LogWarning("Right Turn Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backLeftVectorThrusterEffect.isPlaying)
			{
				this.backLeftVectorThrusterEffect.Stop();
			}
			if (this.frontRightVectorThrusterEffect.isPlaying)
			{
				this.frontRightVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x0600256B RID: 9579 RVA: 0x000C9138 File Offset: 0x000C7338
		public void ActivatePitchDownVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backVentralVectorThrusterEffect == null || this.frontDorsalVectorThrusterEffect == null)
			{
				Debug.LogWarning("Pitch Down Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backVentralVectorThrusterEffect.isStopped)
			{
				this.backVentralVectorThrusterEffect.Play();
			}
			if (this.frontDorsalVectorThrusterEffect.isStopped)
			{
				this.frontDorsalVectorThrusterEffect.Play();
			}
		}

		// Token: 0x0600256C RID: 9580 RVA: 0x000C91DC File Offset: 0x000C73DC
		public void DeactivatePitchDownVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backVentralVectorThrusterEffect == null || this.frontDorsalVectorThrusterEffect == null)
			{
				Debug.LogWarning("Pitch Down Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backVentralVectorThrusterEffect.isPlaying)
			{
				this.backVentralVectorThrusterEffect.Stop();
			}
			if (this.frontDorsalVectorThrusterEffect.isPlaying)
			{
				this.frontDorsalVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x000C925C File Offset: 0x000C745C
		public void ActivatePitchUpVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backDorsalVectorThrusterEffect == null || this.frontVentralVectorThrusterEffect == null)
			{
				Debug.LogWarning("Pitch Up Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backDorsalVectorThrusterEffect.isStopped)
			{
				this.backDorsalVectorThrusterEffect.Play();
			}
			if (this.frontVentralVectorThrusterEffect.isStopped)
			{
				this.frontVentralVectorThrusterEffect.Play();
			}
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x000C9300 File Offset: 0x000C7500
		public void DeactivatePitchUpVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backDorsalVectorThrusterEffect == null || this.frontVentralVectorThrusterEffect == null)
			{
				Debug.LogWarning("Pitch Up Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backDorsalVectorThrusterEffect.isPlaying)
			{
				this.backDorsalVectorThrusterEffect.Stop();
			}
			if (this.frontVentralVectorThrusterEffect.isPlaying)
			{
				this.frontVentralVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x000C9380 File Offset: 0x000C7580
		public void ActivateSlideLeftVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backLeftVectorThrusterEffect == null || this.frontLeftVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Left Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backLeftVectorThrusterEffect.isStopped)
			{
				this.backLeftVectorThrusterEffect.Play();
			}
			if (this.frontLeftVectorThrusterEffect.isStopped)
			{
				this.frontLeftVectorThrusterEffect.Play();
			}
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x000C9424 File Offset: 0x000C7624
		public void DeactivateSlideLeftVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backLeftVectorThrusterEffect == null || this.frontLeftVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Left Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backLeftVectorThrusterEffect.isPlaying)
			{
				this.backLeftVectorThrusterEffect.Stop();
			}
			if (this.frontLeftVectorThrusterEffect.isPlaying)
			{
				this.frontLeftVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x000C94A4 File Offset: 0x000C76A4
		public void ActivateSlideRightVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backRightVectorThrusterEffect == null || this.frontRightVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Right Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backRightVectorThrusterEffect.isStopped)
			{
				this.backRightVectorThrusterEffect.Play();
			}
			if (this.frontRightVectorThrusterEffect.isStopped)
			{
				this.frontRightVectorThrusterEffect.Play();
			}
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x000C9548 File Offset: 0x000C7748
		public void DeactivateSlideRightVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backRightVectorThrusterEffect == null || this.frontRightVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Right Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backRightVectorThrusterEffect.isPlaying)
			{
				this.backRightVectorThrusterEffect.Stop();
			}
			if (this.frontRightVectorThrusterEffect.isPlaying)
			{
				this.frontRightVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x06002573 RID: 9587 RVA: 0x000C95C8 File Offset: 0x000C77C8
		public void ActivateSlideDownVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backDorsalVectorThrusterEffect == null || this.frontDorsalVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Down Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backDorsalVectorThrusterEffect.isStopped)
			{
				this.backDorsalVectorThrusterEffect.Play();
			}
			if (this.frontDorsalVectorThrusterEffect.isStopped)
			{
				this.frontDorsalVectorThrusterEffect.Play();
			}
		}

		// Token: 0x06002574 RID: 9588 RVA: 0x000C966C File Offset: 0x000C786C
		public void DeactivateSlideDownVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backDorsalVectorThrusterEffect == null || this.frontDorsalVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Down Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backDorsalVectorThrusterEffect.isPlaying)
			{
				this.backDorsalVectorThrusterEffect.Stop();
			}
			if (this.frontDorsalVectorThrusterEffect.isPlaying)
			{
				this.frontDorsalVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x06002575 RID: 9589 RVA: 0x000C96EC File Offset: 0x000C78EC
		public void ActivateSlideUpVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.backVentralVectorThrusterEffect == null || this.frontVentralVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Up Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backVentralVectorThrusterEffect.isStopped)
			{
				this.backVentralVectorThrusterEffect.Play();
			}
			if (this.frontVentralVectorThrusterEffect.isStopped)
			{
				this.frontVentralVectorThrusterEffect.Play();
			}
		}

		// Token: 0x06002576 RID: 9590 RVA: 0x000C9790 File Offset: 0x000C7990
		public void DeactivateSlideUpVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.backVentralVectorThrusterEffect == null || this.frontVentralVectorThrusterEffect == null)
			{
				Debug.LogWarning("Slide Up Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.backVentralVectorThrusterEffect.isPlaying)
			{
				this.backVentralVectorThrusterEffect.Stop();
			}
			if (this.frontVentralVectorThrusterEffect.isPlaying)
			{
				this.frontVentralVectorThrusterEffect.Stop();
			}
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x000C9810 File Offset: 0x000C7A10
		public void ActivateRollRightVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.forwardRollRightThrusterEffect == null || this.rearRollRightThrusterEffect == null)
			{
				Debug.LogWarning("Roll Right Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.forwardRollRightThrusterEffect.isStopped)
			{
				this.forwardRollRightThrusterEffect.Play();
			}
			if (this.rearRollRightThrusterEffect.isStopped)
			{
				this.rearRollRightThrusterEffect.Play();
			}
			if (this.forwardCounterRollRightThrusterEffect.isStopped)
			{
				this.forwardCounterRollRightThrusterEffect.Play();
			}
			if (this.rearCounterRollRightThrusterEffect.isStopped)
			{
				this.rearCounterRollRightThrusterEffect.Play();
			}
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x000C98E4 File Offset: 0x000C7AE4
		public void DeactivateRollRightVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.forwardRollRightThrusterEffect == null || this.rearRollRightThrusterEffect == null)
			{
				Debug.LogWarning("Roll Right Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.forwardRollRightThrusterEffect.isPlaying)
			{
				this.forwardRollRightThrusterEffect.Stop();
			}
			if (this.rearRollRightThrusterEffect.isPlaying)
			{
				this.rearRollRightThrusterEffect.Stop();
			}
			if (this.forwardCounterRollRightThrusterEffect.isPlaying)
			{
				this.forwardCounterRollRightThrusterEffect.Stop();
			}
			if (this.rearCounterRollRightThrusterEffect.isPlaying)
			{
				this.rearCounterRollRightThrusterEffect.Stop();
			}
		}

		// Token: 0x06002579 RID: 9593 RVA: 0x000C9994 File Offset: 0x000C7B94
		public void ActivateRollLeftVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				this.SetVectorThrusters(this.ship.template.driveTemplate, this.ship.template.designingFaction);
			}
			if (this.forwardRollLeftThrusterEffect == null || this.rearRollLeftThrusterEffect == null)
			{
				Debug.LogWarning("Roll Left Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.forwardRollLeftThrusterEffect.isStopped)
			{
				this.forwardRollLeftThrusterEffect.Play();
			}
			if (this.rearRollLeftThrusterEffect.isStopped)
			{
				this.rearRollLeftThrusterEffect.Play();
			}
			if (this.forwardCounterRollLeftThrusterEffect.isStopped)
			{
				this.forwardCounterRollLeftThrusterEffect.Play();
			}
			if (this.rearCounterRollLeftThrusterEffect.isStopped)
			{
				this.rearCounterRollLeftThrusterEffect.Play();
			}
		}

		// Token: 0x0600257A RID: 9594 RVA: 0x000C9A68 File Offset: 0x000C7C68
		public void DeactivateRollLeftVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			if (this.forwardRollLeftThrusterEffect == null || this.rearRollLeftThrusterEffect == null)
			{
				Debug.LogWarning("Roll Left Vector Thruster Effects Missing! - " + this.ship.displayName);
				return;
			}
			if (this.forwardRollLeftThrusterEffect.isPlaying)
			{
				this.forwardRollLeftThrusterEffect.Stop();
			}
			if (this.rearRollLeftThrusterEffect.isPlaying)
			{
				this.rearRollLeftThrusterEffect.Stop();
			}
			if (this.forwardCounterRollLeftThrusterEffect.isPlaying)
			{
				this.forwardCounterRollLeftThrusterEffect.Stop();
			}
			if (this.rearCounterRollLeftThrusterEffect.isPlaying)
			{
				this.rearCounterRollLeftThrusterEffect.Stop();
			}
		}

		// Token: 0x0600257B RID: 9595 RVA: 0x000C9B15 File Offset: 0x000C7D15
		public void DeactivateAllVectorThrusters()
		{
			if (!this.initVectorThrusters)
			{
				return;
			}
			this.DeactivateLeftTurnVectorThrusters();
			this.DeactivateRightTurnVectorThrusters();
			this.DeactivatePitchDownVectorThrusters();
			this.DeactivatePitchUpVectorThrusters();
			this.DeactivateRollRightVectorThrusters();
			this.DeactivateRollLeftVectorThrusters();
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x0600257C RID: 9596 RVA: 0x000C9B44 File Offset: 0x000C7D44
		// (set) Token: 0x0600257D RID: 9597 RVA: 0x000C9B4C File Offset: 0x000C7D4C
		public List<ParticleSystem> smallExplosionParticleSystems { get; protected set; }

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x0600257E RID: 9598 RVA: 0x000C9B55 File Offset: 0x000C7D55
		// (set) Token: 0x0600257F RID: 9599 RVA: 0x000C9B5D File Offset: 0x000C7D5D
		public ParticleSystem destructionExplosionParticleSystem { get; protected set; }

		// Token: 0x06002580 RID: 9600 RVA: 0x000C9B66 File Offset: 0x000C7D66
		public void StartDestructionSequence()
		{
			this.destructionEffectController.Play();
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x000C9B74 File Offset: 0x000C7D74
		public void AddExplosions()
		{
			if (this.destructionEffectController)
			{
				this.destructionEffectController.OnStarted += this.OnDestructionStart;
				this.destructionEffectController.OnCompleted += this.OnDestructionComplete;
				return;
			}
			this.smallExplosionParticleSystems = new List<ParticleSystem>();
			MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>("spaceCombat/BigExplosion"), meshRenderer.transform);
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				this.smallExplosionParticleSystems.Add(component);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = new Vector3(Vector3.one.x / gameObject.transform.lossyScale.x, Vector3.one.y / gameObject.transform.lossyScale.y, Vector3.one.z / gameObject.transform.lossyScale.z) * global::UnityEngine.Random.Range(1.5f, 8f);
				gameObject.SetActive(false);
			}
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>("spaceCombat/FinalExplosion"), componentsInChildren[0].transform);
			this.destructionExplosionParticleSystem = gameObject2.GetComponent<ParticleSystem>();
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.SetActive(false);
			gameObject2.transform.localScale = new Vector3(Vector3.one.x / gameObject2.transform.lossyScale.x, Vector3.one.y / gameObject2.transform.lossyScale.y, Vector3.one.z / gameObject2.transform.lossyScale.z) * global::UnityEngine.Random.Range(1f, 1.1f);
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x000C9D64 File Offset: 0x000C7F64
		public void OnDestructionStart()
		{
			this.StopSelectionAnimation();
			this.StopGroupSelectionAnimation();
			this.padlockIconObject.SetActive(false);
			this.mainHullDestroyed = true;
			SphereCollider[] componentsInChildren = base.GetComponentsInChildren<SphereCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			CapsuleCollider[] componentsInChildren2 = base.GetComponentsInChildren<CapsuleCollider>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].enabled = false;
			}
			BoxCollider[] componentsInChildren3 = base.GetComponentsInChildren<BoxCollider>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].enabled = false;
			}
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x000C9DEC File Offset: 0x000C7FEC
		public void OnDestructionComplete()
		{
			if (this.onDestructionCompleteAlreadyCalled)
			{
				return;
			}
			this.onDestructionCompleteAlreadyCalled = true;
			MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			foreach (ParticleSystem particleSystem in this.vectorThrusterEffect)
			{
				if (particleSystem != null)
				{
					TIVFXManager.ReturnVFX(this.vectorThrusterFXPath, particleSystem.gameObject);
				}
			}
		}

		// Token: 0x06002584 RID: 9604 RVA: 0x000C9E5C File Offset: 0x000C805C
		public void ApplyDamageVisualizations(Vector3 hitPoint, DamageType damageType, float damageValue)
		{
			if (this.damageLayer == null)
			{
				Debug.LogError("Prefab is missing assigned DamageLayerComponent");
				return;
			}
			damageValue *= 1f + (float)this.ship.damagedParts.Count * 0.025f;
			this.damageLayer.AddDamagePoint(hitPoint, damageValue, damageType);
			this.ship.damagePoints = this.damageLayer.GetDamagePoints();
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06002585 RID: 9605 RVA: 0x000C9EC7 File Offset: 0x000C80C7
		public virtual int MaxShipBuildSteps { get; } = 4;

		// Token: 0x06002586 RID: 9606 RVA: 0x000C9ED0 File Offset: 0x000C80D0
		public void SetRadiatorsActive(TISpaceShipTemplate ship, bool active)
		{
			foreach (GameObject gameObject in this.WhichRadiators(ship))
			{
				gameObject.SetActive(active);
			}
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x000C9F24 File Offset: 0x000C8124
		public void SetWeaponsActive(bool active)
		{
			foreach (ShipWeaponVisController shipWeaponVisController in this.noseWeaponControllers)
			{
				shipWeaponVisController.gameObject.SetActive(active);
			}
			foreach (ShipWeaponVisController shipWeaponVisController2 in this.dorsalHullWeaponControllers)
			{
				shipWeaponVisController2.gameObject.SetActive(active);
			}
			foreach (ShipWeaponVisController shipWeaponVisController3 in this.ventralHullWeaponControllers)
			{
				shipWeaponVisController3.gameObject.SetActive(active);
			}
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x000CA004 File Offset: 0x000C8204
		public void SetDrive(string resource, GameObject targetObject, int thrusters, TIDriveTemplate drive, TIFactionState faction, bool variableMaterial, int hullAppearanceIndex, bool simpleHull)
		{
			GameObject gameObject2;
			if (!simpleHull)
			{
				GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(resource);
				targetObject.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
				MeshRenderer component = targetObject.GetComponent<MeshRenderer>();
				if (variableMaterial)
				{
					component.sharedMaterial = GameControl.assetLoader.LoadAsset<Material>(drive.GetMaterialPath(faction, hullAppearanceIndex));
				}
				else if (GameControl.control.skirmishMode && faction.IsAlienFaction && !this.ship.isAlien)
				{
					component.sharedMaterial = GameControl.assetLoader.LoadAsset<Material>(drive.GetMaterialPath(GameStateManager.AlienProxy(), hullAppearanceIndex));
				}
				else
				{
					component.sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
				}
				targetObject.transform.localScale = gameObject.transform.localScale;
				gameObject2 = gameObject;
			}
			else
			{
				gameObject2 = targetObject;
			}
			targetObject.SetActive(true);
			List<Transform> list = new List<Transform>();
			foreach (Transform transform in gameObject2.GetComponentsInChildren<Transform>())
			{
				if ((transform.name.Contains("ThrusterPoint") || transform.name.Contains("Thruster") || transform.name.Contains("thruster")) && !transform.name.Contains("Thruster_Alien"))
				{
					list.Add(transform);
				}
			}
			GameObject gameObject3 = AssetCacheManager.thrusterFXPrefabs[drive.MainThrusterFXResource(faction.IsAlienFaction)];
			this.thrusterEffectContainers = new List<MultiEffectContainer>();
			for (int j = 0; j < this.thrusterLocations.Length; j++)
			{
				if (j < thrusters)
				{
					if (list.Count > j && list[j] != null)
					{
						this.thrusterLocations[j].transform.localPosition = list[j].localPosition;
						GameObject gameObject4 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject3, this.thrusterLocations[j].transform);
						gameObject4.transform.localScale = base.transform.localScale;
						gameObject4.transform.localPosition = Vector3.zero;
						this.thrusterEffectContainers.Add(new MultiEffectContainer(gameObject4.GetComponentsInChildren<ParticleSystem>(true).ToList<ParticleSystem>()));
						this.thrusterLocations[j].SetActive(true);
					}
				}
				else
				{
					this.thrusterLocations[j].SetActive(false);
				}
			}
			if (this.eventInstance.isValid())
			{
				this.eventInstance.SetVolume(0.5f);
			}
			this.DeactivateThrusters(false);
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x000CA284 File Offset: 0x000C8484
		public void SetVectorThrusters(TIDriveTemplate drive, TIFactionState faction)
		{
			if (this.initVectorThrusters)
			{
				return;
			}
			this.vectorThrusterFXPath = drive.VectorThrusterFXResource(faction.IsAlienFaction);
			for (int i = 0; i < 16; i++)
			{
				if (i > 11)
				{
					Transform transform = this.vectorThrusterGOs[i - 4].transform;
					GameObject gameObject = new GameObject(TIUtilities.CombineStrings(new string[] { transform.name, "Counter" }));
					gameObject.transform.SetParent(transform.transform.parent, true);
					gameObject.transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y, transform.localPosition.z);
					gameObject.transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
					gameObject.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, transform.localEulerAngles.z);
					this.vectorThrusterGOs.Add(gameObject);
				}
				GameObject vfx = TIVFXManager.GetVFX(this.vectorThrusterFXPath, this.vectorThrusterGOs[i].transform);
				vfx.SetActive(true);
				this.vectorThrusterEffect[i] = vfx.GetComponentsInChildren<ParticleSystem>()[0];
				vfx.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
				vfx.transform.localPosition = Vector3.zero;
				vfx.transform.localScale = Vector3.one;
			}
			this.DeactivateAllVectorThrusters();
			this.initVectorThrusters = true;
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x000CA438 File Offset: 0x000C8638
		public static void SetShipPart(string resource, GameObject targetObject)
		{
			GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(resource);
			targetObject.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			targetObject.GetComponent<MeshRenderer>().sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
			targetObject.transform.localScale = gameObject.transform.localScale;
			targetObject.SetActive(true);
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x000CA49C File Offset: 0x000C869C
		public static void SetWeapon(string resource, ShipVisController parentController, ShipWeaponVisController targetController, ModuleDataEntry moduleDataEntry, bool forVisualizationOnly)
		{
			targetController.Initialize(parentController, moduleDataEntry, forVisualizationOnly);
			GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(resource);
			Transform child = gameObject.transform.GetChild(0);
			targetController.baseObject.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			targetController.baseObject.GetComponent<MeshRenderer>().sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
			targetController.weaponObject.GetComponent<MeshFilter>().sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
			targetController.weaponObject.GetComponent<MeshRenderer>().sharedMaterial = child.GetComponent<MeshRenderer>().sharedMaterial;
			targetController.weaponObject.transform.localPosition = child.transform.localPosition;
			if (targetController.weaponModuleData.moduleTemplate.ref_weapon.staticLauncher)
			{
				targetController.weaponObject.transform.localRotation = child.transform.localRotation;
			}
			if (child.transform.childCount > 0)
			{
				targetController.firePoint.transform.localPosition = child.transform.GetChild(0).localPosition;
			}
			else
			{
				Log.Error("Missing FirePoint for resource " + resource, Array.Empty<object>());
			}
			TISpaceShipState shipState = targetController.shipVisController.shipState;
			if (shipState != null && !shipState.hull.simpleHull)
			{
				targetController.baseObject.transform.localScale = gameObject.transform.localScale;
				targetController.weaponObject.transform.localScale = child.transform.localScale;
			}
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x000CA624 File Offset: 0x000C8824
		public void BuildShip(ShipVisController parentController, TISpaceShipTemplate ship, TISpaceShipState shipState = null, bool buildVectorThrusters = false)
		{
			this.ship = shipState;
			this.SetSkin(ship);
			this.BuildDrives(ship);
			if (buildVectorThrusters)
			{
				this.BuildVectorThrusters(ship);
			}
			this.SetRadiators(ship);
			this.BuildWeapons(parentController, ship, shipState);
			this.AddExplosions();
			this.SetShadows(true);
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x000CA680 File Offset: 0x000C8880
		private void BuildDrives(TISpaceShipTemplate ship)
		{
			if (ship.driveTemplate != null)
			{
				this.thrusters = ship.driveTemplate.thrusters * ship.hullTemplate.thrusterMultiplier;
				this.SetDrive(ship.driveTemplate.modelResource(ship.hullTemplate, ship.GetHullAppearanceIndex), this.thrusterModel, this.thrusters, ship.driveTemplate, ship.designingFaction, ship.designingFaction.IsActiveHumanFaction, ship.GetHullAppearanceIndex, ship.hullTemplate.simpleHull);
				return;
			}
			this.thrusterModel.SetActive(false);
			this.thrusters = 0;
		}

		// Token: 0x0600258E RID: 9614 RVA: 0x000CA717 File Offset: 0x000C8917
		private void BuildVectorThrusters(TISpaceShipTemplate ship)
		{
			if (ship.driveTemplate != null)
			{
				this.SetVectorThrusters(ship.driveTemplate, ship.designingFaction);
			}
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x000CA734 File Offset: 0x000C8934
		private void BuildWeapons(ShipVisController parentController, TISpaceShipTemplate ship, TISpaceShipState shipState = null)
		{
			foreach (ShipWeaponVisController shipWeaponVisController in this.allWeaponControllers)
			{
				shipWeaponVisController.baseObject.SetActive(false);
			}
			foreach (ModuleDataEntry moduleDataEntry in ship.noseWeapons)
			{
				ShipWeaponVisController shipWeaponVisController2 = this.noseWeaponControllers[this.SlotToWeaponMountIndex(moduleDataEntry.slotIndex, moduleDataEntry.moduleTemplate.ref_weapon.mount)];
				shipWeaponVisController2.weaponTemplate = moduleDataEntry.moduleTemplate.ref_weapon;
				ShipModelController.SetWeapon(moduleDataEntry.moduleTemplate.modelResource, parentController, shipWeaponVisController2, moduleDataEntry, shipState == null);
				shipWeaponVisController2.baseObject.SetActive(true);
			}
			foreach (ModuleDataEntry moduleDataEntry2 in ship.hullWeapons)
			{
				if (this.dorsalHullWeaponControllers.Count > 0)
				{
					ShipWeaponVisController shipWeaponVisController3 = this.dorsalHullWeaponControllers[this.SlotToWeaponMountIndex(moduleDataEntry2.slotIndex, moduleDataEntry2.moduleTemplate.ref_weapon.mount)];
					shipWeaponVisController3.weaponTemplate = moduleDataEntry2.moduleTemplate.ref_weapon;
					ShipModelController.SetWeapon(moduleDataEntry2.moduleTemplate.modelResource, parentController, this.dorsalHullWeaponControllers[this.SlotToWeaponMountIndex(moduleDataEntry2.slotIndex, moduleDataEntry2.moduleTemplate.ref_weapon.mount)], moduleDataEntry2, shipState == null);
					shipWeaponVisController3.baseObject.SetActive(true);
				}
				if (this.ventralHullWeaponControllers.Count > 0)
				{
					ShipWeaponVisController shipWeaponVisController4 = this.ventralHullWeaponControllers[this.SlotToWeaponMountIndex(moduleDataEntry2.slotIndex, moduleDataEntry2.moduleTemplate.ref_weapon.mount)];
					shipWeaponVisController4.weaponTemplate = moduleDataEntry2.moduleTemplate.ref_weapon;
					ShipModelController.SetWeapon(moduleDataEntry2.moduleTemplate.modelResource, parentController, this.ventralHullWeaponControllers[this.SlotToWeaponMountIndex(moduleDataEntry2.slotIndex, moduleDataEntry2.moduleTemplate.ref_weapon.mount)], moduleDataEntry2, shipState == null);
					shipWeaponVisController4.baseObject.SetActive(true);
				}
			}
		}

		// Token: 0x06002590 RID: 9616 RVA: 0x000CA9B4 File Offset: 0x000C8BB4
		private void SetShadows(bool off)
		{
			MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].shadowCastingMode = (off ? ShadowCastingMode.Off : ShadowCastingMode.On);
			}
		}

		// Token: 0x06002591 RID: 9617 RVA: 0x000CA9E6 File Offset: 0x000C8BE6
		public void SetShipCopy(TISpaceShipState ship)
		{
			this.selectionAnimObject.SetActive(false);
			this.groupSelectionAnimObject.SetActive(false);
			this.padlockIconObject.SetActive(false);
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x000CAA0C File Offset: 0x000C8C0C
		public void OnWeaponsRepaired()
		{
			foreach (ShipWeaponVisController shipWeaponVisController in this.allWeaponControllers)
			{
				if (shipWeaponVisController.weaponObject != null && shipWeaponVisController.baseObject != null)
				{
					shipWeaponVisController.OnWeaponRepaired();
				}
				else if (this.ship != null)
				{
					TISpaceFleetState fleet = this.ship.fleet;
					string text = ((fleet != null) ? fleet.ID.ToString() : null);
					Debug.LogError(string.Concat(new string[]
					{
						"Missing weapon objects from ",
						this.ship.ID.ToString(),
						", ",
						this.ship.displayName,
						text
					}));
				}
				else
				{
					Debug.LogError("Missing weapon objects");
				}
			}
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x000CAB14 File Offset: 0x000C8D14
		public void SetRadiatorEmissiveKelvinRange(double low, double high)
		{
			if (this.radiatorEmissivesFx == null)
			{
				return;
			}
			foreach (ColorAnimationEffect colorAnimationEffect in this.radiatorEmissivesFx)
			{
				colorAnimationEffect.SetColors(new Color[]
				{
					this.ConvertKelvinToRGB(low),
					this.ConvertKelvinToRGB(high)
				});
			}
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x000CAB90 File Offset: 0x000C8D90
		public Color ConvertKelvinToRGB(double kelvin)
		{
			double num = kelvin / 100.0;
			Color white = Color.white;
			if (num <= 66.0)
			{
				white.r = 1f;
			}
			else
			{
				double num2 = num - 60.0;
				num2 = 329.698727446 * Math.Pow(num2, -0.1332047592);
				if (num2 < 0.0)
				{
					num2 = 0.0;
				}
				if (num2 > 255.0)
				{
					num2 = 255.0;
				}
				white.r = (float)num2 / 255f;
			}
			double num3;
			if (num <= 66.0)
			{
				num3 = num;
				num3 = 99.4708025861 * Math.Log(num3) - 161.1195681661;
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				if (num3 > 255.0)
				{
					num3 = 255.0;
				}
			}
			else
			{
				num3 = num - 60.0;
				num3 = 288.1221695283 * Math.Pow(num3, -0.0755148492);
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				if (num3 > 255.0)
				{
					num3 = 255.0;
				}
			}
			white.g = (float)num3 / 255f;
			double num4;
			if (num >= 66.0)
			{
				num4 = 255.0;
			}
			else if (num <= 19.0)
			{
				num4 = 0.0;
			}
			else
			{
				num4 = num - 10.0;
				num4 = 138.5177312231 * Math.Log(num4) - 305.0447927307;
				if (num4 < 0.0)
				{
					num4 = 0.0;
				}
				if (num4 > 255.0)
				{
					num4 = 255.0;
				}
			}
			white.b = (float)num4 / 255f;
			return white;
		}

		// Token: 0x06002595 RID: 9621 RVA: 0x000CAD80 File Offset: 0x000C8F80
		public void EnableRadiatorEmissives()
		{
			if (!this.RadiatorsEmitting)
			{
				this.RadiatorsEmitting = true;
				foreach (ColorAnimationEffect colorAnimationEffect in this.radiatorEmissivesFx)
				{
					colorAnimationEffect.Play();
				}
			}
		}

		// Token: 0x06002596 RID: 9622 RVA: 0x000CADE0 File Offset: 0x000C8FE0
		public void DisableRadiatorEmissives()
		{
			if (this.RadiatorsEmitting)
			{
				this.RadiatorsEmitting = false;
				foreach (ColorAnimationEffect colorAnimationEffect in this.radiatorEmissivesFx)
				{
					colorAnimationEffect.PlayReversed();
				}
			}
		}

		// Token: 0x06002597 RID: 9623 RVA: 0x000CAE40 File Offset: 0x000C9040
		public void ResetRadiatorEmissives()
		{
			foreach (ColorAnimationEffect colorAnimationEffect in this.radiatorEmissivesFx)
			{
				colorAnimationEffect.CleanUp();
			}
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x000CAE90 File Offset: 0x000C9090
		private void OnRetractRadiatorsInitiate(InitiateRetractRadiatorsEvent e)
		{
			this.RadiatorsExtended = false;
			foreach (Animator animator in this.radiatorAnimators)
			{
				if (!(animator == null))
				{
					animator.SetBool("RadiatorsExtended", false);
				}
			}
			this.AdjustAnimTime(60f, this.gameTime.currentSpeed);
			this.DisableRadiatorEmissives();
		}

		// Token: 0x06002599 RID: 9625 RVA: 0x000CAF14 File Offset: 0x000C9114
		private void OnExtendRadiatorsInitiate(InitiateExtendRadiatorsEvent e)
		{
			this.RadiatorsExtended = true;
			foreach (Animator animator in this.radiatorAnimators)
			{
				if (!(animator == null))
				{
					animator.SetBool("RadiatorsExtended", true);
				}
			}
			this.AdjustAnimTime(60f, this.gameTime.currentSpeed);
		}

		// Token: 0x0600259A RID: 9626 RVA: 0x000CAF94 File Offset: 0x000C9194
		private void OnGameTimeSpeedChanged(GameTimeSpeedChanged e)
		{
			this.AdjustAnimTime(60f, this.gameTime.currentSpeed);
			this.AdjustRadiatorExplosionsTiming(this.gameTime.currentSpeed);
			this.AdjustWeaponExplosionsTiming(this.gameTime.currentSpeed);
			this.AdjustThrusterVFXTiming(this.gameTime.currentSpeed);
		}

		// Token: 0x0600259B RID: 9627 RVA: 0x000CAFEA File Offset: 0x000C91EA
		private void OnPadlockStateChanged(ShipPadlockStateChanged e)
		{
			this.padlockIconObject.SetActive(e.padlockEnabled);
		}

		// Token: 0x0600259C RID: 9628 RVA: 0x000CB000 File Offset: 0x000C9200
		private void AdjustAnimTime(float targetAnimTime, float speed)
		{
			foreach (Animator animator in this.radiatorAnimators)
			{
				if (animator != null && animator.gameObject.activeInHierarchy && animator.runtimeAnimatorController.animationClips.Length != 0)
				{
					AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
					if (currentAnimatorClipInfo.Length != 0)
					{
						float num = currentAnimatorClipInfo[0].clip.length / targetAnimTime;
						animator.speed = speed * num;
					}
				}
			}
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x000CB09C File Offset: 0x000C929C
		private void AdjustRadiatorExplosionsTiming(float speed)
		{
			foreach (Animator animator in this.radiatorAnimators)
			{
				if (animator != null && animator.gameObject.activeInHierarchy)
				{
					RadiatorVisController component = animator.GetComponent<RadiatorVisController>();
					if (speed == 0f)
					{
						if (component != null)
						{
							component.OnPause();
						}
					}
					else if (component != null)
					{
						component.OnPlay();
					}
				}
			}
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x000CB124 File Offset: 0x000C9324
		private void AdjustWeaponExplosionsTiming(float speed)
		{
			if (speed == 0f)
			{
				using (List<ShipWeaponVisController>.Enumerator enumerator = this.allWeaponControllers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ShipWeaponVisController shipWeaponVisController = enumerator.Current;
						shipWeaponVisController.OnGameTimePause();
					}
					return;
				}
			}
			foreach (ShipWeaponVisController shipWeaponVisController2 in this.allWeaponControllers)
			{
				shipWeaponVisController2.OnGameTimePlay();
			}
		}

		// Token: 0x0600259F RID: 9631 RVA: 0x000CB1BC File Offset: 0x000C93BC
		private void AdjustThrusterVFXTiming(float speed)
		{
			if (speed == 0f)
			{
				using (List<MultiEffectContainer>.Enumerator enumerator = this.thrusterEffectContainers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MultiEffectContainer multiEffectContainer = enumerator.Current;
						foreach (ParticleSystem particleSystem in multiEffectContainer.effects)
						{
							if (particleSystem != null && particleSystem.isPlaying)
							{
								particleSystem.Pause();
							}
						}
					}
					return;
				}
			}
			foreach (MultiEffectContainer multiEffectContainer2 in this.thrusterEffectContainers)
			{
				foreach (ParticleSystem particleSystem2 in multiEffectContainer2.effects)
				{
					if (particleSystem2 != null && particleSystem2.isPaused)
					{
						particleSystem2.Play();
					}
				}
			}
		}

		// Token: 0x060025A0 RID: 9632 RVA: 0x000CB2F0 File Offset: 0x000C94F0
		private void OnRadiatorDestroyed(ShipRadiatorDestroyed e)
		{
			foreach (GameObject gameObject in this.WhichRadiators(this.ship.template))
			{
				if (gameObject != null && !e.ship.hull.simpleHull)
				{
					gameObject.GetComponent<RadiatorVisController>().OnRadiatorDestroyed(!this.ship.radiatorsExtended);
				}
			}
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x000CB37C File Offset: 0x000C957C
		public void OnRadiatorRepaired()
		{
			foreach (GameObject gameObject in this.WhichRadiators(this.ship.template))
			{
				if (gameObject != null && !this.ship.hull.simpleHull)
				{
					gameObject.GetComponent<RadiatorVisController>().OnRadiatorRepaired();
				}
			}
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x000CB3FC File Offset: 0x000C95FC
		public void AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations animationValue)
		{
			if (this.currentSelectionAnimation != animationValue)
			{
				if (this.selectionAnim != null && this.selectionRenderer != null)
				{
					Sprite sprite;
					RuntimeAnimatorController runtimeAnimatorController;
					switch (animationValue)
					{
					case MarkerController.MarkerAnimations.Targeting:
						sprite = Resources.Load<Sprite>("Selection Reticle/ReticleSpriteSheet");
						runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Selection Reticle/TI_selection_reticle");
						this.selectionAnimatorController = runtimeAnimatorController;
						this.selectionRenderer.sprite = sprite;
						goto IL_0156;
					case MarkerController.MarkerAnimations.RedSquare:
						sprite = Resources.Load<Sprite>("Square Reticle/RedSquare/RedSquareReticleSS");
						runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/RedSquare/RedAnimator");
						this.selectionAnimatorController = runtimeAnimatorController;
						this.selectionRenderer.sprite = sprite;
						goto IL_0156;
					case MarkerController.MarkerAnimations.GreenSquare:
						sprite = Resources.Load<Sprite>("Square Reticle/GreenSquare/GreenSquareReticleSS");
						runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/GreenSquare/GreenAnimator");
						this.selectionAnimatorController = runtimeAnimatorController;
						this.selectionRenderer.sprite = sprite;
						goto IL_0156;
					case MarkerController.MarkerAnimations.AlienChevron:
						sprite = Resources.Load<Sprite>("AlienReticle/AlienReticle_Anim_SS");
						runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AlienReticle/AlienReticle_Anim_Animator");
						this.selectionAnimatorController = runtimeAnimatorController;
						this.selectionRenderer.sprite = sprite;
						goto IL_0156;
					case MarkerController.MarkerAnimations.RedTargetSquare:
						sprite = Resources.Load<Sprite>("Square Reticle/RedTarget/RedTargetZoomSS");
						runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/RedTarget/RedAnimator");
						this.selectionAnimatorController = runtimeAnimatorController;
						this.selectionRenderer.sprite = sprite;
						goto IL_0156;
					}
					sprite = Resources.Load<Sprite>("Square Reticle/CyanSquare/CyanSquareReticleSS");
					runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/CyanSquare/CyanAnimator");
					this.selectionAnimatorController = runtimeAnimatorController;
					this.selectionRenderer.sprite = sprite;
					IL_0156:
					this.selectionAnim.runtimeAnimatorController = runtimeAnimatorController;
				}
				this.currentSelectionAnimation = animationValue;
			}
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x000CB574 File Offset: 0x000C9774
		public void StartSelectionAnimation()
		{
			if (this.selectionAnimObject == null)
			{
				return;
			}
			if (this.selectionAnimating)
			{
				this.StopSelectionAnimation();
			}
			if (this.selectionAnimObject != null)
			{
				this.selectionAnimObject.SetActive(true);
			}
			if (this.selectionAnim != null)
			{
				this.selectionAnim.SetTrigger("Active");
			}
			this.selectionAnimating = true;
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x000CB5E0 File Offset: 0x000C97E0
		public void StopSelectionAnimation()
		{
			if (this.selectionAnimating)
			{
				if (this.selectionAnim != null)
				{
					this.selectionAnim.SetTrigger("Exit");
				}
				if (this.selectionAnimObject != null)
				{
					this.selectionAnimObject.SetActive(false);
				}
				this.selectionAnimating = false;
			}
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x000CB634 File Offset: 0x000C9834
		public void StartGroupSelectionAnimation()
		{
			if (this.groupSelectionAnimObject == null)
			{
				return;
			}
			if (this.groupSelectionAnimating)
			{
				this.StopGroupSelectionAnimation();
			}
			if (this.groupSelectionAnimObject != null)
			{
				this.groupSelectionAnimObject.SetActive(true);
			}
			if (this.groupSelectionAnim != null)
			{
				this.groupSelectionAnim.SetTrigger("Active");
			}
			this.groupSelectionAnimating = true;
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x000CB6A0 File Offset: 0x000C98A0
		public void StopGroupSelectionAnimation()
		{
			if (this.groupSelectionAnimating)
			{
				if (this.groupSelectionAnim != null)
				{
					this.groupSelectionAnim.SetTrigger("Exit");
				}
				if (this.groupSelectionAnimObject != null)
				{
					this.groupSelectionAnimObject.SetActive(false);
				}
				this.groupSelectionAnimating = false;
			}
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x000CB6F4 File Offset: 0x000C98F4
		public SpaceCouncilorController AddCouncilorMarker(TICouncilorState councilor, TISpaceShipState ship, int idx)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>("ui/StationCouncilorMarker"), base.transform);
			gameObject.name = new StringBuilder(gameObject.name).Append("_").Append(idx).ToString();
			gameObject.transform.localPosition = new Vector3(0f, this.GetMouseColliderDimensions(ship.hull).x + 8f, (float)(idx * 15 * ((idx % 2 == 0) ? 1 : (-1))));
			gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			SpaceCouncilorController component = gameObject.GetComponent<SpaceCouncilorController>();
			component.Initialize(this, ship);
			component.UpdateController(councilor);
			return component;
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x000CB7AF File Offset: 0x000C99AF
		private void Awake()
		{
			this.RadiatorsExtended = true;
		}

		// Token: 0x060025A9 RID: 9641 RVA: 0x000CB7B8 File Offset: 0x000C99B8
		private void OnDisable()
		{
			GameControl.eventManager.RemoveListener<ShipPadlockStateChanged>(new EventManager.EventDelegate<ShipPadlockStateChanged>(this.OnPadlockStateChanged), null);
			GameControl.eventManager.RemoveListener<InitiateExtendRadiatorsEvent>(new EventManager.EventDelegate<InitiateExtendRadiatorsEvent>(this.OnExtendRadiatorsInitiate), null);
			GameControl.eventManager.RemoveListener<InitiateRetractRadiatorsEvent>(new EventManager.EventDelegate<InitiateRetractRadiatorsEvent>(this.OnRetractRadiatorsInitiate), null);
			GameControl.eventManager.RemoveListener<ShipRadiatorDestroyed>(new EventManager.EventDelegate<ShipRadiatorDestroyed>(this.OnRadiatorDestroyed), null);
			GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null);
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x000CB838 File Offset: 0x000C9A38
		private void OnDestroy()
		{
			this.StopSelectionAnimation();
			this.StopGroupSelectionAnimation();
			GameControl.eventManager.RemoveListener<InitiateExtendRadiatorsEvent>(new EventManager.EventDelegate<InitiateExtendRadiatorsEvent>(this.OnExtendRadiatorsInitiate), null);
			GameControl.eventManager.RemoveListener<InitiateRetractRadiatorsEvent>(new EventManager.EventDelegate<InitiateRetractRadiatorsEvent>(this.OnRetractRadiatorsInitiate), null);
			GameControl.eventManager.RemoveListener<ShipRadiatorDestroyed>(new EventManager.EventDelegate<ShipRadiatorDestroyed>(this.OnRadiatorDestroyed), null);
			GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null);
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.eventInstance.Release();
			}
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x000CB8D4 File Offset: 0x000C9AD4
		private void OnEnable()
		{
			if (this.ship != null)
			{
				GameControl.eventManager.AddListener<ShipPadlockStateChanged>(new EventManager.EventDelegate<ShipPadlockStateChanged>(this.OnPadlockStateChanged), null, this.ship, true, false);
				GameControl.eventManager.AddListener<InitiateExtendRadiatorsEvent>(new EventManager.EventDelegate<InitiateExtendRadiatorsEvent>(this.OnExtendRadiatorsInitiate), null, this.ship, true, false);
				GameControl.eventManager.AddListener<InitiateRetractRadiatorsEvent>(new EventManager.EventDelegate<InitiateRetractRadiatorsEvent>(this.OnRetractRadiatorsInitiate), null, this.ship, true, false);
				GameControl.eventManager.AddListener<ShipRadiatorDestroyed>(new EventManager.EventDelegate<ShipRadiatorDestroyed>(this.OnRadiatorDestroyed), null, this.ship, true, false);
				GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null, null, true, false);
				this.InitDamageLayer();
			}
			if (this.radiatorAnimators != null)
			{
				foreach (Animator animator in this.radiatorAnimators)
				{
					if (animator != null && animator.gameObject.activeSelf)
					{
						if (this.RadiatorsExtended)
						{
							animator.Play("Extend", 0, 1f);
						}
						else
						{
							animator.Play("Collapse", 0, 1f);
						}
					}
				}
			}
		}

		// Token: 0x04001BE5 RID: 7141
		protected TISpaceShipState ship;

		// Token: 0x04001BE6 RID: 7142
		public Collider[] _shipModalPhysicsColliders;

		// Token: 0x04001BE7 RID: 7143
		public GameObject thrusterModel;

		// Token: 0x04001BE8 RID: 7144
		public GameObject[] thrusterLocations;

		// Token: 0x04001BE9 RID: 7145
		private List<MultiEffectContainer> thrusterEffectContainers;

		// Token: 0x04001BEA RID: 7146
		public List<GameObject> vectorThrusterGOs;

		// Token: 0x04001BEB RID: 7147
		private ParticleSystem[] vectorThrusterEffect = new ParticleSystem[16];

		// Token: 0x04001BEC RID: 7148
		private bool initVectorThrusters;

		// Token: 0x04001BED RID: 7149
		private string vectorThrusterFXPath;

		// Token: 0x04001BEE RID: 7150
		public EventInstance eventInstance;

		// Token: 0x04001BEF RID: 7151
		public List<ShipWeaponVisController> noseWeaponControllers;

		// Token: 0x04001BF0 RID: 7152
		public List<ShipWeaponVisController> dorsalHullWeaponControllers;

		// Token: 0x04001BF1 RID: 7153
		public List<ShipWeaponVisController> ventralHullWeaponControllers;

		// Token: 0x04001BF2 RID: 7154
		private GameTimeManager gameTime;

		// Token: 0x04001BF3 RID: 7155
		[Header("Radiators")]
		public GameObject radiator12;

		// Token: 0x04001BF4 RID: 7156
		public GameObject radiator130;

		// Token: 0x04001BF5 RID: 7157
		public GameObject radiator3;

		// Token: 0x04001BF6 RID: 7158
		public GameObject radiator4;

		// Token: 0x04001BF7 RID: 7159
		public GameObject radiator430;

		// Token: 0x04001BF8 RID: 7160
		public GameObject radiator6;

		// Token: 0x04001BF9 RID: 7161
		public GameObject radiator730;

		// Token: 0x04001BFA RID: 7162
		public GameObject radiator8;

		// Token: 0x04001BFB RID: 7163
		public GameObject radiator9;

		// Token: 0x04001BFC RID: 7164
		public GameObject radiator1030;

		// Token: 0x04001BFF RID: 7167
		protected List<Animator> radiatorAnimators = new List<Animator>();

		// Token: 0x04001C00 RID: 7168
		protected List<ColorAnimationEffect> radiatorEmissivesFx = new List<ColorAnimationEffect>();

		// Token: 0x04001C01 RID: 7169
		public static UnityEvent SpeedChangeEvent;

		// Token: 0x04001C02 RID: 7170
		protected int thrusters;

		// Token: 0x04001C03 RID: 7171
		[HideInInspector]
		public bool mainHullDestroyed;

		// Token: 0x04001C04 RID: 7172
		[Header("Selection UI")]
		public GameObject selectionAnimObject;

		// Token: 0x04001C05 RID: 7173
		public Animator selectionAnim;

		// Token: 0x04001C06 RID: 7174
		public SpriteRenderer selectionRenderer;

		// Token: 0x04001C07 RID: 7175
		private RuntimeAnimatorController selectionAnimatorController;

		// Token: 0x04001C08 RID: 7176
		[HideInInspector]
		public bool selectionAnimating;

		// Token: 0x04001C0A RID: 7178
		public GameObject groupSelectionAnimObject;

		// Token: 0x04001C0B RID: 7179
		public Animator groupSelectionAnim;

		// Token: 0x04001C0C RID: 7180
		public SpriteRenderer groupSelectionRenderer;

		// Token: 0x04001C0D RID: 7181
		[HideInInspector]
		public bool groupSelectionAnimating;

		// Token: 0x04001C0E RID: 7182
		public GameObject padlockIconObject;

		// Token: 0x04001C0F RID: 7183
		private Vector3 baseScale = new Vector3(0.3f, 0.3f, 0.3f);

		// Token: 0x04001C10 RID: 7184
		private float modScale = 0.005f;

		// Token: 0x04001C11 RID: 7185
		private Transform mainCamT;

		// Token: 0x04001C12 RID: 7186
		[Header("VFX")]
		public AbstractEffectController destructionEffectController;

		// Token: 0x04001C13 RID: 7187
		public DamageLayer damageLayer;

		// Token: 0x04001C16 RID: 7190
		private bool onDestructionCompleteAlreadyCalled;
	}
}
