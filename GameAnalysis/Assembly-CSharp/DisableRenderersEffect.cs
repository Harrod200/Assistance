using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class DisableRenderersEffect : AbstractEffectController
{
	// Token: 0x0600009A RID: 154 RVA: 0x00005FBC File Offset: 0x000041BC
	public override void CleanUp()
	{
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00005FC0 File Offset: 0x000041C0
	protected override void OnPlay()
	{
		MeshRenderer[] componentsInChildren = this.m_rootGameObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		base.EffectCompleted();
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00005FF6 File Offset: 0x000041F6
	protected override void OnStop()
	{
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00005FF8 File Offset: 0x000041F8
	protected override void OnUpdate(float deltaTime)
	{
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00005FFA File Offset: 0x000041FA
	protected override void OnPause()
	{
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00005FFC File Offset: 0x000041FC
	protected override void OnUnPause()
	{
	}

	// Token: 0x0400008A RID: 138
	[SerializeField]
	private GameObject m_rootGameObject;
}
