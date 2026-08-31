using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200041B RID: 1051
public class SimpleRotatePlanet : MonoBehaviour
{
	// Token: 0x06001571 RID: 5489 RVA: 0x00069AA8 File Offset: 0x00067CA8
	private void Awake()
	{
		string name = SceneManager.GetActiveScene().name;
		if (name == null || !(name == "StartScreenScene"))
		{
			global::UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x00069AD9 File Offset: 0x00067CD9
	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, this.speedY * Time.deltaTime, 0f));
	}

	// Token: 0x040012CE RID: 4814
	public float speedY;
}
