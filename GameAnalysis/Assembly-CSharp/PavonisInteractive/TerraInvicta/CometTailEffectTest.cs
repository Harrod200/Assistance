using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A9 RID: 1449
	public class CometTailEffectTest : MonoBehaviour
	{
		// Token: 0x06002761 RID: 10081 RVA: 0x000D79BD File Offset: 0x000D5BBD
		private void Start()
		{
			this._particleSystems = this._cometTransform.GetComponentsInChildren<ParticleSystem>();
		}

		// Token: 0x06002762 RID: 10082 RVA: 0x000D79D0 File Offset: 0x000D5BD0
		private void Update()
		{
			this._cometTransform.position = this.GetPosition(Time.time);
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x000D79E8 File Offset: 0x000D5BE8
		private Vector3 GetPosition(float time)
		{
			float num = this._a * Mathf.Cos(this._speed * time * 3.1415927f) + this._orbitTarget.position.x;
			float num2 = this._b * Mathf.Sin(this._speed * time * 3.1415927f) + this._orbitTarget.position.z;
			return new Vector3(num, this._orbitTarget.position.y, num2);
		}

		// Token: 0x04001D49 RID: 7497
		[SerializeField]
		private Transform _cometTransform;

		// Token: 0x04001D4A RID: 7498
		[SerializeField]
		private Transform _orbitTarget;

		// Token: 0x04001D4B RID: 7499
		[SerializeField]
		private float _a = 5f;

		// Token: 0x04001D4C RID: 7500
		[SerializeField]
		private float _b = 3f;

		// Token: 0x04001D4D RID: 7501
		[SerializeField]
		private float _speed = 1f;

		// Token: 0x04001D4E RID: 7502
		private ParticleSystem[] _particleSystems;
	}
}
