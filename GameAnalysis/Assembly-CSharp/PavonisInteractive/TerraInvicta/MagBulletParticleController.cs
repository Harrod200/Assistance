using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005AD RID: 1453
	public class MagBulletParticleController : MonoBehaviour
	{
		// Token: 0x06002777 RID: 10103 RVA: 0x000D8178 File Offset: 0x000D6378
		public void UpdateMass(float currentMass_kg, float massAtLaunch_kg)
		{
			if (this.mainSystem == null)
			{
				this.mainSystem = base.GetComponentInChildren<ParticleSystem>();
			}
			ParticleSystem.MainModule main = this.mainSystem.main;
			Color color = this.mainSystem.main.startColor.color;
			main.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, currentMass_kg / massAtLaunch_kg));
		}

		// Token: 0x04001D5E RID: 7518
		[SerializeField]
		private ParticleSystem mainSystem;
	}
}
