using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class MapArc : MonoBehaviour
{
	// Token: 0x06000031 RID: 49 RVA: 0x00002F04 File Offset: 0x00001104
	public void Init(Vector3 start, Vector3 end, Sprite sprite, TICouncilorState councilor, bool hide)
	{
		this.start = start;
		this.end = end;
		this.perp = Vector3.Cross(start, end);
		this.angle = Vector3.Angle(start, end);
		this.councilor = councilor;
		this.offsetGO = new GameObject("offset");
		this.offsetGO.transform.SetParent(base.transform);
		SpriteRenderer spriteRenderer = this.offsetGO.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		spriteRenderer.enabled = !hide && GameControl.control.viewMgr.currentView == ViewType.PoliticalMap;
		this.offsetGO.transform.localPosition = new Vector3(0f, 0f, start.magnitude);
		this.baseRotation = Quaternion.LookRotation(start, this.perp);
		base.transform.localRotation = this.baseRotation;
		this.offsetGO.transform.localRotation = Quaternion.identity;
		this.offsetGO.transform.localScale = 0.25f * Vector3.one;
		this.speed = 20f + this.angle / 4f;
	}

	// Token: 0x06000032 RID: 50 RVA: 0x0000302C File Offset: 0x0000122C
	private void Update()
	{
		float num = this.speed * Time.deltaTime;
		this.totalAngle += Mathf.Abs(num);
		if (this.totalAngle > this.angle)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			TICouncilorState ticouncilorState = this.councilor;
			if (ticouncilorState != null)
			{
				ticouncilorState.ExitTransit();
			}
			EventManager eventManager = GameControl.eventManager;
			TICouncilorState ticouncilorState2 = this.councilor;
			TICouncilorState ticouncilorState3 = this.councilor;
			GameEvent gameEvent = new CouncilorPositionUpdated(ticouncilorState2, (ticouncilorState3 != null) ? ticouncilorState3.location : null);
			string text = null;
			object[] array = new object[3];
			array[0] = this.councilor;
			int num2 = 1;
			TICouncilorState ticouncilorState4 = this.councilor;
			object obj;
			if (ticouncilorState4 == null)
			{
				obj = null;
			}
			else
			{
				TIGameState location = ticouncilorState4.location;
				obj = ((location != null) ? location.ref_region : null);
			}
			array[num2] = obj;
			int num3 = 2;
			TICouncilorState ticouncilorState5 = this.councilor;
			object obj2;
			if (ticouncilorState5 == null)
			{
				obj2 = null;
			}
			else
			{
				TIGameState location2 = ticouncilorState5.location;
				obj2 = ((location2 != null) ? location2.ref_nation : null);
			}
			array[num3] = obj2;
			eventManager.TriggerEvent(gameEvent, text, array);
			return;
		}
		base.transform.Rotate(Vector3.up, num);
	}

	// Token: 0x0400001E RID: 30
	public float speed = 10f;

	// Token: 0x0400001F RID: 31
	public Vector3 start;

	// Token: 0x04000020 RID: 32
	public Vector3 end;

	// Token: 0x04000021 RID: 33
	private Vector3 perp;

	// Token: 0x04000022 RID: 34
	private float angle;

	// Token: 0x04000023 RID: 35
	private float totalAngle;

	// Token: 0x04000024 RID: 36
	private GameObject offsetGO;

	// Token: 0x04000025 RID: 37
	private Quaternion baseRotation;

	// Token: 0x04000026 RID: 38
	private TICouncilorState councilor;
}
