using System;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

// Token: 0x02000011 RID: 17
public abstract class AbstractEffectController : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000064 RID: 100 RVA: 0x00005210 File Offset: 0x00003410
	// (remove) Token: 0x06000065 RID: 101 RVA: 0x00005248 File Offset: 0x00003448
	public event Action OnStarted;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000066 RID: 102 RVA: 0x00005280 File Offset: 0x00003480
	// (remove) Token: 0x06000067 RID: 103 RVA: 0x000052B8 File Offset: 0x000034B8
	public event Action OnCompleted;

	// Token: 0x06000068 RID: 104 RVA: 0x000052ED File Offset: 0x000034ED
	protected virtual void Start()
	{
		this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
	}

	// Token: 0x06000069 RID: 105 RVA: 0x000052FF File Offset: 0x000034FF
	protected virtual void OnEnable()
	{
		if (this.m_playOnAwake)
		{
			this.Play();
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000530F File Offset: 0x0000350F
	protected virtual void OnDisable()
	{
		if (this.m_isPlaying)
		{
			this.Stop();
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00005320 File Offset: 0x00003520
	public void Play()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.m_isPlaying)
		{
			this.OnStop();
			this.CleanUp();
		}
		this.m_isPlaying = true;
		this.OnPlay();
		Action onStarted = this.OnStarted;
		if (onStarted != null)
		{
			onStarted();
		}
		if (this.m_oneShot)
		{
			this.m_playOnAwake = false;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00005377 File Offset: 0x00003577
	public void Pause()
	{
		this.m_isPaused = true;
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00005380 File Offset: 0x00003580
	public void Stop()
	{
		if (this.m_isPlaying)
		{
			this.m_isPlaying = false;
			this.OnStop();
			this.CleanUp();
			Action onCompleted = this.OnCompleted;
			if (onCompleted == null)
			{
				return;
			}
			onCompleted();
		}
	}

	// Token: 0x0600006E RID: 110 RVA: 0x000053AD File Offset: 0x000035AD
	protected void EffectCompleted()
	{
		this.m_isPlaying = false;
		this.CleanUp();
		Action onCompleted = this.OnCompleted;
		if (onCompleted == null)
		{
			return;
		}
		onCompleted();
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000053CC File Offset: 0x000035CC
	private void Update()
	{
		if (this.m_isUnPaused)
		{
			this.m_isUnPaused = false;
		}
		if (this.debugCanPause && this.gameTime.currentSpeed == 0f)
		{
			this.m_isPaused = true;
			this.OnPause();
			return;
		}
		if (this.m_isPaused)
		{
			this.m_isUnPaused = true;
			this.m_isPaused = false;
			this.OnUnPause();
		}
		if (this.m_isPlaying)
		{
			this.OnUpdate(Time.deltaTime);
		}
	}

	// Token: 0x06000070 RID: 112
	public abstract void CleanUp();

	// Token: 0x06000071 RID: 113
	protected abstract void OnPlay();

	// Token: 0x06000072 RID: 114
	protected abstract void OnUpdate(float deltaTime);

	// Token: 0x06000073 RID: 115
	protected abstract void OnStop();

	// Token: 0x06000074 RID: 116
	protected abstract void OnPause();

	// Token: 0x06000075 RID: 117
	protected abstract void OnUnPause();

	// Token: 0x0400005C RID: 92
	[SerializeField]
	private string m_Comment;

	// Token: 0x0400005D RID: 93
	[SerializeField]
	private bool m_playOnAwake;

	// Token: 0x0400005E RID: 94
	[SerializeField]
	private bool m_oneShot;

	// Token: 0x0400005F RID: 95
	protected bool m_isPlaying;

	// Token: 0x04000060 RID: 96
	protected bool m_isPaused;

	// Token: 0x04000061 RID: 97
	protected bool m_isUnPaused;

	// Token: 0x04000062 RID: 98
	private GameTimeManager gameTime;

	// Token: 0x04000065 RID: 101
	private bool debugCanPause = true;
}
