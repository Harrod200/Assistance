using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class EffectSequencer : AbstractEffectController
{
	// Token: 0x060000A1 RID: 161 RVA: 0x00006006 File Offset: 0x00004206
	protected override void Start()
	{
		base.Start();
		Array.Sort<EffectSequencer.SequencedEffect>(this.m_effects, new EffectSequencer.SequencedEffectComparer());
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x0000601E File Offset: 0x0000421E
	public override void CleanUp()
	{
		this.m_playingEffects.Clear();
		this.m_curTime = 0f;
		this.m_curEffectIndex = 0;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00006040 File Offset: 0x00004240
	private void StepSequence(float deltaTime)
	{
		this.m_curTime += deltaTime * this.m_playbackSpeed;
		for (int i = this.m_curEffectIndex; i < this.m_effects.Length; i++)
		{
			if (this.m_effects[i].time <= this.m_curTime)
			{
				AbstractEffectController curEffect = this.m_effects[i].effect;
				curEffect.OnCompleted += delegate
				{
					this.m_playingEffects.Remove(curEffect);
				};
				if (!this.m_playingEffects.Contains(curEffect))
				{
					this.m_playingEffects.Add(curEffect);
				}
				curEffect.Play();
				this.m_curEffectIndex = i + 1;
			}
		}
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x0000610C File Offset: 0x0000430C
	protected override void OnPlay()
	{
		this.StepSequence(0f);
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00006119 File Offset: 0x00004319
	protected override void OnUpdate(float deltaTime)
	{
		this.StepSequence(deltaTime);
		if (this.m_curEffectIndex >= this.m_effects.Length && this.m_playingEffects.Count == 0)
		{
			base.EffectCompleted();
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00006148 File Offset: 0x00004348
	protected override void OnStop()
	{
		for (int i = this.m_playingEffects.Count - 1; i >= 0; i--)
		{
			this.m_playingEffects[i].Stop();
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x0000617E File Offset: 0x0000437E
	protected override void OnPause()
	{
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00006180 File Offset: 0x00004380
	protected override void OnUnPause()
	{
	}

	// Token: 0x0400008B RID: 139
	[SerializeField]
	private float m_playbackSpeed = 1f;

	// Token: 0x0400008C RID: 140
	[SerializeField]
	private EffectSequencer.SequencedEffect[] m_effects;

	// Token: 0x0400008D RID: 141
	private List<AbstractEffectController> m_playingEffects = new List<AbstractEffectController>();

	// Token: 0x0400008E RID: 142
	private int m_curEffectIndex;

	// Token: 0x0400008F RID: 143
	private float m_curTime;

	// Token: 0x02000AB8 RID: 2744
	[Serializable]
	public struct SequencedEffect
	{
		// Token: 0x04004846 RID: 18502
		[SerializeField]
		private string label;

		// Token: 0x04004847 RID: 18503
		public float time;

		// Token: 0x04004848 RID: 18504
		public AbstractEffectController effect;
	}

	// Token: 0x02000AB9 RID: 2745
	private class SequencedEffectComparer : IComparer<EffectSequencer.SequencedEffect>
	{
		// Token: 0x060065EA RID: 26090 RVA: 0x002FEB5A File Offset: 0x002FCD5A
		public int Compare(EffectSequencer.SequencedEffect x, EffectSequencer.SequencedEffect y)
		{
			return (int)Mathf.Sign(x.time - y.time);
		}
	}
}
