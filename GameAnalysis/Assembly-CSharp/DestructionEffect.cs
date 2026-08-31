using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class DestructionEffect : AbstractEffectController
{
	// Token: 0x06000093 RID: 147 RVA: 0x00005D72 File Offset: 0x00003F72
	public override void CleanUp()
	{
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00005D74 File Offset: 0x00003F74
	protected override void OnPlay()
	{
		float num = this.m_debrisGroup.m_motionScaleFactor + global::UnityEngine.Random.Range(-this.m_debrisGroup.m_motionScaleVariance, this.m_debrisGroup.m_motionScaleVariance);
		float num2 = global::UnityEngine.Random.Range(-this.m_debrisGroup.m_motionDirectionVariance, this.m_debrisGroup.m_motionDirectionVariance);
		foreach (GameObject gameObject in this.m_debrisGroup.m_DebrisObjects)
		{
			Vector3 motionDirection = this.m_debrisGroup.m_motionDirection;
			Vector3 vector = motionDirection * num;
			Vector3 vector2 = num * Mathf.Tan(num2) * new Vector3(-motionDirection.z, 0f, motionDirection.x);
			vector += vector2;
			Vector3 normalized = this.m_debrisGroup.m_motionOrigin.normalized;
			Vector3 vector3 = new Vector3(-normalized.z, 0f, normalized.x);
			Vector3 eulerAngles = Quaternion.AngleAxis(Vector3.Angle(normalized, vector.normalized), vector3).eulerAngles;
			eulerAngles.Scale(this.m_debrisGroup.m_angleScaleFactors);
			gameObject.transform.DOBlendableLocalMoveBy(vector, this.m_debrisGroup.m_lifetime, false).SetEase(Ease.OutExpo);
			gameObject.transform.DOBlendableLocalRotateBy(eulerAngles, this.m_debrisGroup.m_lifetime, RotateMode.Fast).SetEase(Ease.OutQuad);
			if (this.m_affectDebrisIndividually)
			{
				num = this.m_debrisGroup.m_motionScaleFactor + global::UnityEngine.Random.Range(-this.m_debrisGroup.m_motionScaleVariance, this.m_debrisGroup.m_motionScaleVariance);
				num2 = global::UnityEngine.Random.Range(-this.m_debrisGroup.m_motionDirectionVariance, this.m_debrisGroup.m_motionDirectionVariance);
			}
		}
		base.Invoke("EffectCompleted", this.m_debrisGroup.m_lifetime);
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00005F37 File Offset: 0x00004137
	protected override void OnStop()
	{
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00005F39 File Offset: 0x00004139
	protected override void OnUpdate(float deltaTime)
	{
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00005F3C File Offset: 0x0000413C
	protected override void OnPause()
	{
		GameObject[] debrisObjects = this.m_debrisGroup.m_DebrisObjects;
		for (int i = 0; i < debrisObjects.Length; i++)
		{
			debrisObjects[i].transform.DOPause();
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00005F74 File Offset: 0x00004174
	protected override void OnUnPause()
	{
		GameObject[] debrisObjects = this.m_debrisGroup.m_DebrisObjects;
		for (int i = 0; i < debrisObjects.Length; i++)
		{
			debrisObjects[i].transform.DOPlay();
		}
	}

	// Token: 0x04000088 RID: 136
	[SerializeField]
	private bool m_affectDebrisIndividually;

	// Token: 0x04000089 RID: 137
	[SerializeField]
	private DebrisGroup m_debrisGroup = new DebrisGroup();
}
