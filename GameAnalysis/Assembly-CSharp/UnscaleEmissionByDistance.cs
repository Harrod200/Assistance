using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class UnscaleEmissionByDistance : MonoBehaviour
{
	// Token: 0x060000C7 RID: 199 RVA: 0x00006C44 File Offset: 0x00004E44
	private void Start()
	{
		foreach (ParticleSystem particleSystem in base.gameObject.GetComponentsInChildren<ParticleSystem>())
		{
			Vector3 localScale = particleSystem.transform.localScale;
			Vector3 lossyScale = particleSystem.transform.lossyScale;
			float num = (localScale.x / lossyScale.x + localScale.y / lossyScale.y + localScale.z / lossyScale.z) / 3f;
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			float num2 = emission.rateOverDistanceMultiplier * num;
			emission.rateOverDistanceMultiplier = num2;
		}
		base.enabled = false;
	}
}
