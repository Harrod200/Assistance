using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public class EnvMapAnimator : MonoBehaviour
{
	// Token: 0x06001628 RID: 5672 RVA: 0x00070A8A File Offset: 0x0006EC8A
	private void Awake()
	{
		this.m_textMeshPro = base.GetComponent<TMP_Text>();
		this.m_material = this.m_textMeshPro.fontSharedMaterial;
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x00070AA9 File Offset: 0x0006ECA9
	private IEnumerator Start()
	{
		Matrix4x4 matrix = default(Matrix4x4);
		for (;;)
		{
			matrix.SetTRS(Vector3.zero, Quaternion.Euler(Time.time * this.RotationSpeeds.x, Time.time * this.RotationSpeeds.y, Time.time * this.RotationSpeeds.z), Vector3.one);
			this.m_material.SetMatrix("_EnvMatrix", matrix);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04001437 RID: 5175
	public Vector3 RotationSpeeds;

	// Token: 0x04001438 RID: 5176
	private TMP_Text m_textMeshPro;

	// Token: 0x04001439 RID: 5177
	private Material m_material;
}
