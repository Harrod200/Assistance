using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200000F RID: 15
public class RotateCloudsStartScene : MonoBehaviour
{
	// Token: 0x0600005E RID: 94 RVA: 0x0000505C File Offset: 0x0000325C
	private void Awake()
	{
		string name = SceneManager.GetActiveScene().name;
		if (name != null && name == "StartScreenScene")
		{
			Color color = base.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
			base.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(color.r, color.g, color.b, 0.4f));
			return;
		}
		global::UnityEngine.Object.Destroy(this);
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000050DF File Offset: 0x000032DF
	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, -0.12f * Time.deltaTime, 0f));
	}

	// Token: 0x04000054 RID: 84
	private const float speedY = -0.12f;
}
