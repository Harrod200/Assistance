using System;
using System.Collections;
using System.IO;
using LapinerTools.Steam.Data;
using LapinerTools.uMyGUI;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LapinerTools.Steam.UI
{
	// Token: 0x02000537 RID: 1335
	public class SteamWorkshopUIUpload : MonoBehaviour
	{
		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x060021DC RID: 8668 RVA: 0x000B115F File Offset: 0x000AF35F
		public static SteamWorkshopUIUpload Instance
		{
			get
			{
				if (SteamWorkshopUIUpload.s_instance == null)
				{
					SteamWorkshopUIUpload.s_instance = global::UnityEngine.Object.FindObjectOfType<SteamWorkshopUIUpload>();
				}
				return SteamWorkshopUIUpload.s_instance;
			}
		}

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x060021DD RID: 8669 RVA: 0x000B1180 File Offset: 0x000AF380
		// (remove) Token: 0x060021DE RID: 8670 RVA: 0x000B11B8 File Offset: 0x000AF3B8
		public event Action<string> OnNameSet;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060021DF RID: 8671 RVA: 0x000B11F0 File Offset: 0x000AF3F0
		// (remove) Token: 0x060021E0 RID: 8672 RVA: 0x000B1228 File Offset: 0x000AF428
		public event Action<string> OnDescriptionSet;

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060021E1 RID: 8673 RVA: 0x000B1260 File Offset: 0x000AF460
		// (remove) Token: 0x060021E2 RID: 8674 RVA: 0x000B1298 File Offset: 0x000AF498
		public event Action<int> OnTagSet;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x060021E3 RID: 8675 RVA: 0x000B12D0 File Offset: 0x000AF4D0
		// (remove) Token: 0x060021E4 RID: 8676 RVA: 0x000B1308 File Offset: 0x000AF508
		public event Action<string> OnIconFilePathSet;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060021E5 RID: 8677 RVA: 0x000B1340 File Offset: 0x000AF540
		// (remove) Token: 0x060021E6 RID: 8678 RVA: 0x000B1378 File Offset: 0x000AF578
		public event Action<Texture2D> OnIconTextureSet;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060021E7 RID: 8679 RVA: 0x000B13B0 File Offset: 0x000AF5B0
		// (remove) Token: 0x060021E8 RID: 8680 RVA: 0x000B13E8 File Offset: 0x000AF5E8
		public event Action<WorkshopItemUpdateEventArgs> OnStartedUpload;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060021E9 RID: 8681 RVA: 0x000B1420 File Offset: 0x000AF620
		// (remove) Token: 0x060021EA RID: 8682 RVA: 0x000B1458 File Offset: 0x000AF658
		public event Action<WorkshopItemUpdateEventArgs> OnFinishedUpload;

		// Token: 0x060021EB RID: 8683 RVA: 0x000B1490 File Offset: 0x000AF690
		public virtual void SetItemData(WorkshopItemUpdate p_itemData)
		{
			this.m_itemData = ((p_itemData != null) ? p_itemData : new WorkshopItemUpdate());
			if (this.m_itemData.Name == null)
			{
				this.m_itemData.Name = "";
			}
			if (this.m_itemData.Description == null)
			{
				this.m_itemData.Description = "";
			}
			if (this.NAME_INPUT != null)
			{
				this.NAME_INPUT.text = this.m_itemData.Name;
			}
			else
			{
				Debug.LogError("SteamWorkshopUIUpload: SetItemData: NAME_INPUT is not set in inspector!");
			}
			if (this.DESCRIPTION_INPUT != null)
			{
				if (!string.IsNullOrEmpty(this.m_itemData.Description))
				{
					base.StartCoroutine(this.SetDescriptionSafe(this.m_itemData.Description));
				}
				else
				{
					this.DESCRIPTION_INPUT.text = "";
				}
			}
			else
			{
				Debug.LogError("SteamWorkshopUIUpload: SetItemData: DESCRIPTION_INPUT is not set in inspector!");
			}
			if (!(this.ICON != null))
			{
				Debug.LogError("SteamWorkshopUIUpload: SetItemData: ICON is not set in inspector!");
				return;
			}
			if (!string.IsNullOrEmpty(this.m_itemData.IconPath))
			{
				base.StartCoroutine(this.LoadIcon(this.m_itemData.IconPath));
				return;
			}
			this.ICON.texture = null;
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x000B15C0 File Offset: 0x000AF7C0
		protected virtual void Start()
		{
			SteamMainBase<SteamWorkshopMain>.Instance.OnUploaded += this.ShowSuccessMessage;
			SteamMainBase<SteamWorkshopMain>.Instance.OnError += this.ShowErrorMessage;
			if (this.NAME_INPUT != null)
			{
				this.NAME_INPUT.onEndEdit.AddListener(new UnityAction<string>(this.OnEditName));
			}
			else
			{
				Debug.LogError("SteamWorkshopUIUpload: NAME_INPUT is not set in inspector!");
			}
			if (this.DESCRIPTION_INPUT != null)
			{
				this.DESCRIPTION_INPUT.onEndEdit.AddListener(new UnityAction<string>(this.OnEditDescription));
			}
			else
			{
				Debug.LogError("SteamWorkshopUIUpload: DESCRIPTION_INPUT is not set in inspector!");
			}
			if (this.TagsDropdown != null)
			{
				this.TagsDropdown.options.Clear();
				for (int i = 0; i < ModManager.WorkshopTags.Length - 1; i++)
				{
					this.TagsDropdown.options.Add(new TMP_Dropdown.OptionData(Loc.T(TIUtilities.CombineStrings(new string[]
					{
						"UI.StartScreen.Mods.Tags.",
						i.ToString()
					}))));
				}
				this.TagsDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnEditTag));
			}
			else
			{
				Debug.LogError("SteamWorkshopUIUpload: TagDropdown is not set in inspector!");
			}
			if (this.SCREENSHOT_BUTTON != null)
			{
				this.SCREENSHOT_BUTTON.onClick.AddListener(new UnityAction(this.OnScreenshotButtonClick));
			}
			else
			{
				Debug.LogError("SteamWorkshopUIUpload: SCREENSHOT_BUTTON is not set in inspector!");
			}
			if (this.UPLOAD_BUTTON != null)
			{
				this.UPLOAD_BUTTON.onClick.AddListener(new UnityAction(this.OnUploadButtonClick));
				return;
			}
			Debug.LogError("SteamWorkshopUIUpload: UPLOAD_BUTTON is not set in inspector!");
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x000B1768 File Offset: 0x000AF968
		protected virtual void LateUpdate()
		{
			if (this.m_improveNavigationFocus)
			{
				EventSystem current = EventSystem.current;
				if (current != null && (current.currentSelectedGameObject == null || !current.currentSelectedGameObject.activeInHierarchy))
				{
					if (current.lastSelectedGameObject != null && current.lastSelectedGameObject.activeInHierarchy)
					{
						current.SetSelectedGameObject(current.lastSelectedGameObject);
						return;
					}
					if (this.NAME_INPUT != null)
					{
						this.NAME_INPUT.Select();
					}
				}
			}
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x000B17E8 File Offset: 0x000AF9E8
		protected virtual void OnDestroy()
		{
			if (this.ICON != null)
			{
				global::UnityEngine.Object.Destroy(this.ICON.texture);
			}
			if (this.m_pendingImageDownload != null)
			{
				this.m_pendingImageDownload.Dispose();
				this.m_pendingImageDownload = null;
			}
			if (SteamMainBase<SteamWorkshopMain>.IsInstanceSet)
			{
				SteamMainBase<SteamWorkshopMain>.Instance.OnUploaded -= this.ShowSuccessMessage;
				SteamMainBase<SteamWorkshopMain>.Instance.OnError -= this.ShowErrorMessage;
			}
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x000B1862 File Offset: 0x000AFA62
		protected virtual void OnEditName(string p_name)
		{
			this.m_itemData.Name = p_name;
			this.InvokeEventHandlerSafely<string>(this.OnNameSet, p_name);
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x000B187D File Offset: 0x000AFA7D
		protected virtual void OnEditDescription(string p_description)
		{
			this.m_itemData.Description = p_description;
			this.InvokeEventHandlerSafely<string>(this.OnDescriptionSet, p_description);
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x000B1898 File Offset: 0x000AFA98
		protected virtual void OnEditTag(int p_tagIndex)
		{
			this.m_itemData.Tags.Clear();
			if (p_tagIndex == 0)
			{
				return;
			}
			this.m_itemData.Tags.Add(ModManager.WorkshopTags[p_tagIndex]);
			this.InvokeEventHandlerSafely<int>(this.OnTagSet, p_tagIndex);
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x000B18D2 File Offset: 0x000AFAD2
		public void OnExplorerHereClicked()
		{
			Application.OpenURL(this.m_itemData.ContentPath);
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x000B18E4 File Offset: 0x000AFAE4
		protected virtual void OnScreenshotButtonClick()
		{
			if (string.IsNullOrEmpty(this.m_itemData.ContentPath))
			{
				this.m_itemData.ContentPath = Path.Combine(Application.persistentDataPath, this.m_itemData.Name);
			}
			string iconFilePath = Path.Combine(this.m_itemData.ContentPath, this.m_itemData.Name + ".png");
			SteamMainBase<SteamWorkshopMain>.Instance.RenderIcon(Camera.main, this.ICON_WIDTH, this.ICON_HEIGHT, iconFilePath, delegate(Texture2D p_renderedIcon)
			{
				this.m_itemData.IconPath = iconFilePath;
				this.InvokeEventHandlerSafely<string>(this.OnIconFilePathSet, iconFilePath);
				if (this.ICON != null)
				{
					this.ICON.texture = p_renderedIcon;
					this.InvokeEventHandlerSafely<Texture2D>(this.OnIconTextureSet, p_renderedIcon);
					return;
				}
				Debug.LogError("SteamWorkshopUIUpload: OnScreenshotButtonClick: ICON is not set in inspector!");
			});
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x000B1988 File Offset: 0x000AFB88
		protected virtual void OnUploadButtonClick()
		{
			if (string.IsNullOrEmpty(this.m_itemData.IconPath))
			{
				string text = Path.Combine(this.m_itemData.ContentPath, this.m_itemData.Name + ".png");
				if (File.Exists(text))
				{
					this.m_itemData.IconPath = text;
				}
			}
			if (string.IsNullOrEmpty(this.m_itemData.Name))
			{
				((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.InvalidNameTitle"), Loc.T("UI.StartScreen.Mods.InvalidNameDesc")).ShowButton("ok");
				return;
			}
			if (string.IsNullOrEmpty(this.m_itemData.Description))
			{
				((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.InvalidDescTitle"), Loc.T("UI.StartScreen.Mods.InvalidDescDesc")).ShowButton("ok");
				return;
			}
			if (string.IsNullOrEmpty(this.m_itemData.IconPath))
			{
				((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.InvalidIconTitle"), Loc.T("UI.StartScreen.Mods.InvalidIconDesc")).ShowButton("ok");
				return;
			}
			this.m_isUploading = true;
			base.StartCoroutine(this.ShowUploadProgress());
			SteamMainBase<SteamWorkshopMain>.Instance.Upload(this.m_itemData, null);
			if (this.OnStartedUpload != null)
			{
				this.OnStartedUpload(new WorkshopItemUpdateEventArgs
				{
					Item = this.m_itemData
				});
			}
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x000B1B0C File Offset: 0x000AFD0C
		protected virtual void ShowSuccessMessage(WorkshopItemUpdateEventArgs p_successArgs)
		{
			this.m_isUploading = false;
			if (!p_successArgs.IsError && p_successArgs.Item != null)
			{
				((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.ModUploadedTitle"), p_successArgs.Item.Name + Loc.T("UI.StartScreen.Mods.ModUploadedDesc")).ShowButton("ok");
			}
			if (this.OnFinishedUpload != null)
			{
				this.OnFinishedUpload(p_successArgs);
			}
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x000B1B8C File Offset: 0x000AFD8C
		protected virtual void ShowErrorMessage(LapinerTools.Steam.Data.ErrorEventArgs p_errorArgs)
		{
			this.m_isUploading = false;
			((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.SteamError"), p_errorArgs.ErrorMessage).ShowButton("ok");
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x000B1BCC File Offset: 0x000AFDCC
		protected virtual void InvokeEventHandlerSafely<T>(Action<T> p_handler, T p_data)
		{
			try
			{
				if (p_handler != null)
				{
					p_handler(p_data);
				}
			}
			catch (Exception ex)
			{
				string[] array = new string[6];
				array[0] = "SteamWorkshopUIUpload: your event handler (";
				int num = 1;
				object target = p_handler.Target;
				array[num] = ((target != null) ? target.ToString() : null);
				array[2] = " - System.Action<";
				int num2 = 3;
				Type typeFromHandle = typeof(T);
				array[num2] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
				array[4] = ">) has thrown an excepotion!\n";
				int num3 = 5;
				Exception ex2 = ex;
				array[num3] = ((ex2 != null) ? ex2.ToString() : null);
				Debug.LogError(string.Concat(array));
			}
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x000B1C60 File Offset: 0x000AFE60
		protected virtual IEnumerator ShowUploadProgress()
		{
			while (this.m_itemData != null && this.m_isUploading)
			{
				float uploadProgress = SteamMainBase<SteamWorkshopMain>.Instance.GetUploadProgress(this.m_itemData);
				((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup("text")).SetText(Loc.T("UI.StartScreen.Mods.Uploading"), "<size=32>" + ((int)(uploadProgress * 100f)).ToString() + "%</size>");
				yield return new WaitForSeconds(0.4f);
			}
			yield break;
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x000B1C6F File Offset: 0x000AFE6F
		protected virtual IEnumerator SetDescriptionSafe(string p_description)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			if (this.DESCRIPTION_INPUT != null)
			{
				this.DESCRIPTION_INPUT.text = p_description;
			}
			yield break;
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x000B1C85 File Offset: 0x000AFE85
		protected virtual IEnumerator LoadIcon(string p_filePath)
		{
			if (!string.IsNullOrEmpty(p_filePath))
			{
				this.m_pendingImageDownload = new WWW("file:///" + p_filePath);
				yield return this.m_pendingImageDownload;
				if (this.m_pendingImageDownload != null)
				{
					if (this.m_pendingImageDownload.isDone && string.IsNullOrEmpty(this.m_pendingImageDownload.error))
					{
						if (this.ICON != null)
						{
							this.ICON.texture = this.m_pendingImageDownload.texture;
						}
					}
					else
					{
						Debug.LogError("SteamWorkshopUIUpload: LoadIcon: could not load icon at '" + p_filePath + "'\n" + this.m_pendingImageDownload.error);
					}
					this.m_pendingImageDownload = null;
				}
			}
			yield break;
		}

		// Token: 0x040019E5 RID: 6629
		protected static SteamWorkshopUIUpload s_instance;

		// Token: 0x040019E6 RID: 6630
		[SerializeField]
		protected int ICON_WIDTH = 512;

		// Token: 0x040019E7 RID: 6631
		[SerializeField]
		protected int ICON_HEIGHT = 512;

		// Token: 0x040019E8 RID: 6632
		[SerializeField]
		protected TMP_InputField NAME_INPUT;

		// Token: 0x040019E9 RID: 6633
		[SerializeField]
		protected TMP_InputField DESCRIPTION_INPUT;

		// Token: 0x040019EA RID: 6634
		[SerializeField]
		protected TMP_Dropdown TagsDropdown;

		// Token: 0x040019EB RID: 6635
		[SerializeField]
		protected RawImage ICON;

		// Token: 0x040019EC RID: 6636
		[SerializeField]
		protected Button SCREENSHOT_BUTTON;

		// Token: 0x040019ED RID: 6637
		[SerializeField]
		protected Button UPLOAD_BUTTON;

		// Token: 0x040019EE RID: 6638
		[SerializeField]
		protected bool m_improveNavigationFocus = true;

		// Token: 0x040019EF RID: 6639
		protected bool m_isUploading;

		// Token: 0x040019F0 RID: 6640
		protected WWW m_pendingImageDownload;

		// Token: 0x040019F1 RID: 6641
		protected WorkshopItemUpdate m_itemData = new WorkshopItemUpdate();
	}
}
