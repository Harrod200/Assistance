using System;
using Pixelplacement;
using UnityEngine;

// Token: 0x0200001C RID: 28
[ExecuteInEditMode]
[RequireComponent(typeof(Spline))]
public class SplineSpawnParticleSystem : MonoBehaviour
{
	// Token: 0x060000C4 RID: 196 RVA: 0x00006B0B File Offset: 0x00004D0B
	private void OnEnable()
	{
		this._spline = base.GetComponent<Spline>();
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00006B1C File Offset: 0x00004D1C
	private void LateUpdate()
	{
		if (this._particleSystem == null)
		{
			return;
		}
		ParticleSystem.MainModule main = this._particleSystem.main;
		float num = main.startSpeed.Evaluate(global::UnityEngine.Random.value);
		float num2 = main.startLifetime.Evaluate(global::UnityEngine.Random.value);
		Debug.Log(main.startLifetimeMultiplier);
		float value = global::UnityEngine.Random.value;
		if (this._spline.GetDirection(value, false) == Vector3.zero)
		{
			return;
		}
		Vector3 position = this._spline.GetPosition(value, false);
		float num3 = num * num2;
		float length = this._spline.Length;
		float num4 = num3 / length;
		Vector3 position2 = this._spline.GetPosition(Mathf.Clamp01(value + num4), false);
		Vector3 vector = global::UnityEngine.Random.onUnitSphere * this.spreadRange;
		Vector3 vector2 = (position2 - position + vector).normalized * num;
		this._particleSystem.Emit(new ParticleSystem.EmitParams
		{
			position = position,
			velocity = vector2,
			startLifetime = num2
		}, 1);
	}

	// Token: 0x040000A9 RID: 169
	[Range(0f, 1f)]
	public float spreadRange;

	// Token: 0x040000AA RID: 170
	[SerializeField]
	private ParticleSystem _particleSystem;

	// Token: 0x040000AB RID: 171
	private Spline _spline;
}
