using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PavonisInteractive.TerraInvicta.Modding
{
	// Token: 0x0200095B RID: 2395
	public class JsonMod : Mod
	{
		// Token: 0x17000F60 RID: 3936
		// (get) Token: 0x06005B2D RID: 23341 RVA: 0x002BE7E1 File Offset: 0x002BC9E1
		// (set) Token: 0x06005B2E RID: 23342 RVA: 0x002BE7E9 File Offset: 0x002BC9E9
		public string ModFileName { get; set; }

		// Token: 0x17000F61 RID: 3937
		// (get) Token: 0x06005B2F RID: 23343 RVA: 0x002BE7F2 File Offset: 0x002BC9F2
		// (set) Token: 0x06005B30 RID: 23344 RVA: 0x002BE7FA File Offset: 0x002BC9FA
		public string ModFilePath { get; set; }

		// Token: 0x17000F62 RID: 3938
		// (get) Token: 0x06005B31 RID: 23345 RVA: 0x002BE803 File Offset: 0x002BCA03
		// (set) Token: 0x06005B32 RID: 23346 RVA: 0x002BE80B File Offset: 0x002BCA0B
		public string TargetFilePath { get; set; }

		// Token: 0x17000F63 RID: 3939
		// (get) Token: 0x06005B33 RID: 23347 RVA: 0x002BE814 File Offset: 0x002BCA14
		// (set) Token: 0x06005B34 RID: 23348 RVA: 0x002BE81C File Offset: 0x002BCA1C
		public int LoadOrder { get; set; }

		// Token: 0x17000F64 RID: 3940
		// (get) Token: 0x06005B35 RID: 23349 RVA: 0x002BE825 File Offset: 0x002BCA25
		// (set) Token: 0x06005B36 RID: 23350 RVA: 0x002BE82D File Offset: 0x002BCA2D
		public List<string> TemplatesToConcatArrays { get; set; }

		// Token: 0x17000F65 RID: 3941
		// (get) Token: 0x06005B37 RID: 23351 RVA: 0x002BE836 File Offset: 0x002BCA36
		// (set) Token: 0x06005B38 RID: 23352 RVA: 0x002BE83E File Offset: 0x002BCA3E
		public List<string> TemplatesToReplaceArrays { get; set; }

		// Token: 0x17000F66 RID: 3942
		// (get) Token: 0x06005B39 RID: 23353 RVA: 0x002BE847 File Offset: 0x002BCA47
		// (set) Token: 0x06005B3A RID: 23354 RVA: 0x002BE84F File Offset: 0x002BCA4F
		public List<string> TemplatesToReplace { get; set; }

		// Token: 0x17000F67 RID: 3943
		// (get) Token: 0x06005B3B RID: 23355 RVA: 0x002BE858 File Offset: 0x002BCA58
		// (set) Token: 0x06005B3C RID: 23356 RVA: 0x002BE860 File Offset: 0x002BCA60
		public List<JObject> FileContents { get; set; }

		// Token: 0x17000F68 RID: 3944
		// (get) Token: 0x06005B3D RID: 23357 RVA: 0x002BE869 File Offset: 0x002BCA69
		// (set) Token: 0x06005B3E RID: 23358 RVA: 0x002BE871 File Offset: 0x002BCA71
		public bool foundVanillaMatch { get; private set; }

		// Token: 0x06005B3F RID: 23359 RVA: 0x002BE87C File Offset: 0x002BCA7C
		public JObject GetJObject(string dataName)
		{
			return this.FileContents.Single<JObject>((JObject x) => x.Property("dataName").Value.ToString() == dataName);
		}

		// Token: 0x06005B40 RID: 23360 RVA: 0x002BE8AD File Offset: 0x002BCAAD
		public HashSet<string> GetDataNames()
		{
			return new HashSet<string>(this.FileContents.Select<JObject, string>((JObject s) => s["dataName"].ToString()).ToList<string>());
		}

		// Token: 0x06005B41 RID: 23361 RVA: 0x002BE8E3 File Offset: 0x002BCAE3
		public void SetFoundMatch()
		{
			this.foundVanillaMatch = true;
		}
	}
}
