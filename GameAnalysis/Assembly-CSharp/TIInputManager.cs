using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class TIInputManager : MonoBehaviour
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000135 RID: 309 RVA: 0x0000945D File Offset: 0x0000765D
	// (set) Token: 0x06000136 RID: 310 RVA: 0x00009464 File Offset: 0x00007664
	public static TIInputManager inputManager { get; set; }

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000137 RID: 311 RVA: 0x0000946C File Offset: 0x0000766C
	private static Camera mainCamera
	{
		get
		{
			if (TIInputManager._mainCamera == null)
			{
				TIInputManager._mainCamera = Camera.main;
			}
			return TIInputManager._mainCamera;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000138 RID: 312 RVA: 0x0000948A File Offset: 0x0000768A
	public static bool IsDragSelecting
	{
		get
		{
			return TIInputManager._isDragSelecting;
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00009494 File Offset: 0x00007694
	public static void LoadProfileKeybindings()
	{
		TIPlayerProfileManager.GetKBList();
		for (int i = 0; i < TIInputManager.keyBindings.Count; i++)
		{
			if (TIPlayerProfileManager.savedKeybind(i) != KeyCode.None || TIPlayerProfileManager.savedEmptyKeybind(i))
			{
				TIInputManager.keyBindings[i] = TIPlayerProfileManager.savedKeybind(i);
			}
		}
		for (int j = 0; j < TIInputManager.keyBindingModifiers.Count; j++)
		{
			if (TIPlayerProfileManager.savedKeybind(j) != KeyCode.None || TIPlayerProfileManager.savedEmptyKeybind(j))
			{
				TIInputManager.keyBindingModifiers[j] = TIPlayerProfileManager.savedKeybindModifier(j);
			}
		}
		Debug.Log("Loaded saved Keybindings");
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000951C File Offset: 0x0000771C
	public static void InitBindingArray()
	{
		TIInputManager.keyBindings.Add(TIInputManager.Objectives);
		TIInputManager.keyBindings.Add(TIInputManager.PoliticalEarth);
		TIInputManager.keyBindings.Add(TIInputManager.SolarSystem);
		TIInputManager.keyBindings.Add(TIInputManager.Councilors);
		TIInputManager.keyBindings.Add(TIInputManager.Nations);
		TIInputManager.keyBindings.Add(TIInputManager.Habitats);
		TIInputManager.keyBindings.Add(TIInputManager.Fleets);
		TIInputManager.keyBindings.Add(TIInputManager.Research);
		TIInputManager.keyBindings.Add(TIInputManager.Intel);
		TIInputManager.keyBindings.Add(TIInputManager.CycleRecolorEarthMap);
		TIInputManager.keyBindings.Add(TIInputManager.ToggleOrbitTrails);
		TIInputManager.keyBindings.Add(TIInputManager.IncreaseSpeed);
		TIInputManager.keyBindings.Add(TIInputManager.DecreaseSpeed);
		TIInputManager.keyBindings.Add(TIInputManager.PauseSpeed);
		TIInputManager.keyBindings.Add(TIInputManager.ToggleExpandNewsFeed);
		TIInputManager.keyBindings.Add(TIInputManager.QuickSave);
		TIInputManager.keyBindings.Add(TIInputManager.ToggleHelper);
		TIInputManager.keyBindings.Add(TIInputManager.cameraLeft);
		TIInputManager.keyBindings.Add(TIInputManager.cameraRight);
		TIInputManager.keyBindings.Add(TIInputManager.cameraUp);
		TIInputManager.keyBindings.Add(TIInputManager.cameraDown);
		TIInputManager.keyBindings.Add(TIInputManager.cameraZoomIn);
		TIInputManager.keyBindings.Add(TIInputManager.cameraZoomOut);
		TIInputManager.keyBindings.Add(TIInputManager.cycleShipsUp);
		TIInputManager.keyBindings.Add(TIInputManager.cycleShipsDown);
		TIInputManager.keyBindings.Add(TIInputManager.toggleGrid);
		TIInputManager.keyBindings.Add(TIInputManager.toggleCombatUI);
		TIInputManager.keyBindings.Add(TIInputManager.toggleShipWaypoints);
		TIInputManager.keyBindings.Add(TIInputManager.toggleFPSWidget);
		TIInputManager.keyBindings.Add(TIInputManager.ToggleDistanceSymbols);
		TIInputManager.keyBindings.Add(TIInputManager.SetSpeedIndex1);
		TIInputManager.keyBindings.Add(TIInputManager.SetSpeedIndex2);
		TIInputManager.keyBindings.Add(TIInputManager.SetSpeedIndex3);
		TIInputManager.keyBindings.Add(TIInputManager.SetSpeedIndex4);
		TIInputManager.keyBindings.Add(TIInputManager.SetSpeedIndex5);
		TIInputManager.keyBindings.Add(TIInputManager.SetSpeedIndex6);
		TIInputManager.keyBindings.Add(TIInputManager.PauseSpeedNoToggle);
		TIInputManager.keyBindings.Add(TIInputManager.ToggleProspectData);
		TIInputManager.keyBindings.Add(TIInputManager.yawControl);
		TIInputManager.keyBindings.Add(TIInputManager.altitudeControl);
		TIInputManager.keyBindings.Add(TIInputManager.pitchControl);
		TIInputManager.keyBindings.Add(TIInputManager.lateralControl);
		TIInputManager.keyBindings.Add(TIInputManager.rollControl);
		TIInputManager.keyBindings.Add(TIInputManager.burnControl);
		TIInputManager.keyBindings.Add(TIInputManager.OpenShipDesigner);
		TIInputManager.keyBindings.Add(TIInputManager.OpenConstructionManager);
		TIInputManager.keyBindings.Add(TIInputManager.ToggleShowAllColonizedBodyNames);
		TIInputManager.keyBindings.Add(TIInputManager.OpenGlobalSearch);
		TIInputManager.keyBindings.Add(TIInputManager.fleetCommandSelectPrimaryTarget);
		TIInputManager.keyBindings.Add(TIInputManager.fleetCommandLaunchMissileSalvo);
		TIInputManager.keyBindings.Add(TIInputManager.AccessibilityMagnifier);
		for (int i = 0; i < TIInputManager.keyBindings.Count; i++)
		{
			if (i == 47)
			{
				TIInputManager.keyBindingModifiers.Add(KeyCode.LeftControl);
			}
			else
			{
				TIInputManager.keyBindingModifiers.Add(KeyCode.None);
			}
		}
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000985C File Offset: 0x00007A5C
	public static void UpdateBindings(bool saveData = true)
	{
		TIInputManager.Objectives = TIInputManager.keyBindings[0];
		TIInputManager.PoliticalEarth = TIInputManager.keyBindings[1];
		TIInputManager.SolarSystem = TIInputManager.keyBindings[2];
		TIInputManager.Councilors = TIInputManager.keyBindings[3];
		TIInputManager.Nations = TIInputManager.keyBindings[4];
		TIInputManager.Habitats = TIInputManager.keyBindings[5];
		TIInputManager.Fleets = TIInputManager.keyBindings[6];
		TIInputManager.Research = TIInputManager.keyBindings[7];
		TIInputManager.Intel = TIInputManager.keyBindings[8];
		TIInputManager.CycleRecolorEarthMap = TIInputManager.keyBindings[9];
		TIInputManager.ToggleOrbitTrails = TIInputManager.keyBindings[10];
		TIInputManager.IncreaseSpeed = TIInputManager.keyBindings[11];
		TIInputManager.DecreaseSpeed = TIInputManager.keyBindings[12];
		TIInputManager.PauseSpeed = TIInputManager.keyBindings[13];
		TIInputManager.ToggleExpandNewsFeed = TIInputManager.keyBindings[14];
		TIInputManager.QuickSave = TIInputManager.keyBindings[15];
		TIInputManager.ToggleHelper = TIInputManager.keyBindings[16];
		TIInputManager.cameraLeft = TIInputManager.keyBindings[17];
		TIInputManager.cameraRight = TIInputManager.keyBindings[18];
		TIInputManager.cameraUp = TIInputManager.keyBindings[19];
		TIInputManager.cameraDown = TIInputManager.keyBindings[20];
		TIInputManager.cameraZoomIn = TIInputManager.keyBindings[21];
		TIInputManager.cameraZoomOut = TIInputManager.keyBindings[22];
		TIInputManager.cycleShipsUp = TIInputManager.keyBindings[23];
		TIInputManager.cycleShipsDown = TIInputManager.keyBindings[24];
		TIInputManager.toggleGrid = TIInputManager.keyBindings[25];
		TIInputManager.toggleCombatUI = TIInputManager.keyBindings[26];
		TIInputManager.toggleShipWaypoints = TIInputManager.keyBindings[27];
		TIInputManager.toggleFPSWidget = TIInputManager.keyBindings[28];
		TIInputManager.ToggleDistanceSymbols = TIInputManager.keyBindings[29];
		TIInputManager.SetSpeedIndex1 = TIInputManager.keyBindings[30];
		TIInputManager.SetSpeedIndex2 = TIInputManager.keyBindings[31];
		TIInputManager.SetSpeedIndex3 = TIInputManager.keyBindings[32];
		TIInputManager.SetSpeedIndex4 = TIInputManager.keyBindings[33];
		TIInputManager.SetSpeedIndex5 = TIInputManager.keyBindings[34];
		TIInputManager.SetSpeedIndex6 = TIInputManager.keyBindings[35];
		TIInputManager.PauseSpeedNoToggle = TIInputManager.keyBindings[36];
		TIInputManager.ToggleProspectData = TIInputManager.keyBindings[37];
		TIInputManager.yawControl = TIInputManager.keyBindings[38];
		TIInputManager.altitudeControl = TIInputManager.keyBindings[39];
		TIInputManager.pitchControl = TIInputManager.keyBindings[40];
		TIInputManager.lateralControl = TIInputManager.keyBindings[41];
		TIInputManager.rollControl = TIInputManager.keyBindings[42];
		TIInputManager.burnControl = TIInputManager.keyBindings[43];
		TIInputManager.OpenShipDesigner = TIInputManager.keyBindings[44];
		TIInputManager.OpenConstructionManager = TIInputManager.keyBindings[45];
		TIInputManager.ToggleShowAllColonizedBodyNames = TIInputManager.keyBindings[46];
		TIInputManager.OpenGlobalSearch = TIInputManager.keyBindings[47];
		TIInputManager.fleetCommandSelectPrimaryTarget = TIInputManager.keyBindings[48];
		TIInputManager.fleetCommandLaunchMissileSalvo = TIInputManager.keyBindings[49];
		TIInputManager.AccessibilityMagnifier = TIInputManager.keyBindings[50];
		if (saveData)
		{
			TIPlayerProfileManager.SavePlayerConfig();
		}
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00009BCC File Offset: 0x00007DCC
	public static bool IsHotkeyTriggered(KeyCode hotkey, TIInputManager.KeyPressMode keyPressMode = TIInputManager.KeyPressMode.Down)
	{
		switch (keyPressMode)
		{
		case TIInputManager.KeyPressMode.Down:
			if (!Input.GetKeyDown(hotkey))
			{
				return false;
			}
			break;
		case TIInputManager.KeyPressMode.Up:
			if (!Input.GetKeyUp(hotkey))
			{
				return false;
			}
			break;
		case TIInputManager.KeyPressMode.Continous:
			if (!Input.GetKey(hotkey))
			{
				return false;
			}
			break;
		}
		int num = TIInputManager.keyBindings.IndexOf(hotkey);
		return (TIInputManager.keyBindingModifiers[num] == KeyCode.None && !TIInputManager.IsModifierKeyDown) || (TIInputManager.keyBindingModifiers[num] != KeyCode.None && Input.GetKey(TIInputManager.keyBindingModifiers[num]));
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600013D RID: 317 RVA: 0x00009C4C File Offset: 0x00007E4C
	public static bool IsAltKeyDown
	{
		get
		{
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600013E RID: 318 RVA: 0x00009C66 File Offset: 0x00007E66
	public static bool IsControlKeyDown
	{
		get
		{
			return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600013F RID: 319 RVA: 0x00009C80 File Offset: 0x00007E80
	public static bool IsShiftKeyDown
	{
		get
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000140 RID: 320 RVA: 0x00009C9C File Offset: 0x00007E9C
	public static bool ControlGroupKeyPressedThisFrame
	{
		get
		{
			return Input.GetKeyDown(TIInputManager.controlGroup0) || Input.GetKeyDown(TIInputManager.controlGroup1) || Input.GetKeyDown(TIInputManager.controlGroup2) || Input.GetKeyDown(TIInputManager.controlGroup3) || Input.GetKeyDown(TIInputManager.controlGroup4) || Input.GetKeyDown(TIInputManager.controlGroup5) || Input.GetKeyDown(TIInputManager.controlGroup6) || Input.GetKeyDown(TIInputManager.controlGroup7) || Input.GetKeyDown(TIInputManager.controlGroup8) || Input.GetKeyDown(TIInputManager.controlGroup9) || Input.GetKeyDown(TIInputManager.controlGroup0);
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000141 RID: 321 RVA: 0x00009D2D File Offset: 0x00007F2D
	public static bool IsModifierKeyDown
	{
		get
		{
			return TIInputManager.IsAltKeyDown || TIInputManager.IsControlKeyDown || TIInputManager.IsShiftKeyDown;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000142 RID: 322 RVA: 0x00009D44 File Offset: 0x00007F44
	public static bool IsLeftMouseButtonDown
	{
		get
		{
			return Input.GetMouseButton(0);
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000143 RID: 323 RVA: 0x00009D4C File Offset: 0x00007F4C
	public static bool WasLeftMouseButtonClicked
	{
		get
		{
			return Input.GetMouseButtonUp(0);
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000144 RID: 324 RVA: 0x00009D54 File Offset: 0x00007F54
	public static bool IsRightMouseButtonDown
	{
		get
		{
			return Input.GetMouseButton(1);
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000145 RID: 325 RVA: 0x00009D5C File Offset: 0x00007F5C
	public static bool WasRightMouseButtonClicked
	{
		get
		{
			return Input.GetMouseButtonUp(1);
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000146 RID: 326 RVA: 0x00009D64 File Offset: 0x00007F64
	public static bool IsCameraMovementKeyPressed
	{
		get
		{
			return Input.GetKey(TIInputManager.cameraDown) || Input.GetKey(TIInputManager.cameraUp) || Input.GetKey(TIInputManager.cameraLeft) || Input.GetKey(TIInputManager.cameraRight);
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000147 RID: 327 RVA: 0x00009D98 File Offset: 0x00007F98
	public static bool IsMouseHoveringApplication
	{
		get
		{
			return new Rect(0f, 0f, (float)Screen.width, (float)Screen.height).Contains(Input.mousePosition);
		}
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00009DD0 File Offset: 0x00007FD0
	public static bool DoubleClickedGameState(TIGameState gameState, bool registerClick = false)
	{
		bool flag = false;
		if (Time.time - TIInputManager.lastClickedTimeStamp < 0.5f && TIInputManager.lastClickedGameState == gameState)
		{
			flag = true;
		}
		if (registerClick)
		{
			TIInputManager.lastClickedGameState = gameState;
			TIInputManager.lastClickedTimeStamp = Time.time;
		}
		return flag;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00009E14 File Offset: 0x00008014
	private void Awake()
	{
		TIInputManager.factionCursor = new Texture2D(128, 128);
		TIInputManager.cursorResist = this.cursor_Resist;
		TIInputManager.cursorAlien = this.cursor_Alien;
		TIInputManager.cursorAppease = this.cursor_Appease;
		TIInputManager.cursorCooperate = this.cursor_Cooperate;
		TIInputManager.cursorDestroy = this.cursor_Destroy;
		TIInputManager.cursorEscape = this.cursor_Escape;
		TIInputManager.cursorExploit = this.cursor_Exploit;
		TIInputManager.cursorMain = this.cursor_Main;
		TIInputManager.cursorNeutral = this.cursor_Neutral;
		TIInputManager.targetCursor = this.target_Cursor;
		TIInputManager.targetCursorValid = this.target_CursorValid;
		TIInputManager.targetCursorInvalid = this.target_CursorInvalid;
		TIInputManager.defaultCursor = this.default_Cursor;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00009EC4 File Offset: 0x000080C4
	public static void Init()
	{
		TIInputManager.LoadProfileKeybindings();
		TIInputManager.UpdateBindings(true);
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00009ED4 File Offset: 0x000080D4
	public static void BlockKeybindings()
	{
		if (!TIInputManager.acceptingInput)
		{
			return;
		}
		TIInputManager.acceptingInput = false;
		TIInputManager.keyBindingsReserve.Clear();
		foreach (KeyCode keyCode in TIInputManager.keyBindings)
		{
			TIInputManager.keyBindingsReserve.Add(keyCode);
		}
		for (int i = 0; i < TIInputManager.keyBindings.Count; i++)
		{
			TIInputManager.keyBindings[i] = KeyCode.None;
		}
		TIInputManager.UpdateBindings(false);
	}

	// Token: 0x0600014C RID: 332 RVA: 0x00009F6C File Offset: 0x0000816C
	public static void RestoreKeybindings()
	{
		if (TIInputManager.keyBindingsReserve.Count == 0)
		{
			return;
		}
		TIInputManager.acceptingInput = true;
		for (int i = 0; i < TIInputManager.keyBindings.Count; i++)
		{
			TIInputManager.keyBindings[i] = TIInputManager.keyBindingsReserve[i];
		}
		TIInputManager.UpdateBindings(false);
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00009FBD File Offset: 0x000081BD
	public static void RemoveKeyBind(Keybind_UIMenuObject uiKeybind)
	{
		TIInputManager.currentRebind = uiKeybind;
		TIInputManager.SetNewKeybind(TIInputManager.currentRebind.keybindIndex, KeyCode.None, KeyCode.None);
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00009FD8 File Offset: 0x000081D8
	public static void ResetKeybindsToDefault()
	{
		TIInputManager.Objectives = KeyCode.F1;
		TIInputManager.PoliticalEarth = KeyCode.F2;
		TIInputManager.SolarSystem = KeyCode.F3;
		TIInputManager.Councilors = KeyCode.F4;
		TIInputManager.Nations = KeyCode.F5;
		TIInputManager.Habitats = KeyCode.F6;
		TIInputManager.Fleets = KeyCode.F7;
		TIInputManager.Research = KeyCode.F8;
		TIInputManager.Intel = KeyCode.F9;
		TIInputManager.CycleRecolorEarthMap = KeyCode.Home;
		TIInputManager.ToggleOrbitTrails = KeyCode.End;
		TIInputManager.ToggleExpandNewsFeed = KeyCode.N;
		TIInputManager.QuickSave = KeyCode.F10;
		TIInputManager.ToggleHelper = KeyCode.F11;
		TIInputManager.ToggleDistanceSymbols = KeyCode.Insert;
		TIInputManager.ToggleProspectData = KeyCode.P;
		TIInputManager.OpenShipDesigner = KeyCode.U;
		TIInputManager.OpenConstructionManager = KeyCode.I;
		TIInputManager.ToggleShowAllColonizedBodyNames = KeyCode.L;
		TIInputManager.IncreaseSpeed = KeyCode.KeypadPlus;
		TIInputManager.DecreaseSpeed = KeyCode.KeypadMinus;
		TIInputManager.PauseSpeed = KeyCode.Space;
		TIInputManager.PauseSpeedNoToggle = KeyCode.Backspace;
		TIInputManager.SetSpeedIndex1 = KeyCode.Alpha1;
		TIInputManager.SetSpeedIndex2 = KeyCode.Alpha2;
		TIInputManager.SetSpeedIndex3 = KeyCode.Alpha3;
		TIInputManager.SetSpeedIndex4 = KeyCode.Alpha4;
		TIInputManager.SetSpeedIndex5 = KeyCode.Alpha5;
		TIInputManager.SetSpeedIndex6 = KeyCode.Alpha6;
		TIInputManager.cameraLeft = KeyCode.A;
		TIInputManager.cameraRight = KeyCode.D;
		TIInputManager.cameraUp = KeyCode.W;
		TIInputManager.cameraDown = KeyCode.S;
		TIInputManager.cameraZoomIn = KeyCode.T;
		TIInputManager.cameraZoomOut = KeyCode.B;
		TIInputManager.cycleShipsUp = KeyCode.PageUp;
		TIInputManager.cycleShipsDown = KeyCode.PageDown;
		TIInputManager.toggleGrid = KeyCode.G;
		TIInputManager.toggleCombatUI = KeyCode.Delete;
		TIInputManager.toggleShipWaypoints = KeyCode.V;
		TIInputManager.fleetCommandSelectPrimaryTarget = KeyCode.H;
		TIInputManager.fleetCommandLaunchMissileSalvo = KeyCode.J;
		TIInputManager.altitudeControl = KeyCode.Q;
		TIInputManager.lateralControl = KeyCode.E;
		TIInputManager.burnControl = KeyCode.R;
		TIInputManager.yawControl = KeyCode.Z;
		TIInputManager.pitchControl = KeyCode.X;
		TIInputManager.rollControl = KeyCode.C;
		TIInputManager.toggleFPSWidget = KeyCode.Slash;
		TIInputManager.OpenGlobalSearch = KeyCode.F;
		TIInputManager.AccessibilityMagnifier = KeyCode.Backslash;
		TIInputManager.keyBindings.Clear();
		TIInputManager.keyBindingModifiers.Clear();
		TIInputManager.InitBindingArray();
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000A198 File Offset: 0x00008398
	private void OnApplicationFocus(bool focus)
	{
		if (TIPlayerProfileManager.muteInBackground)
		{
			if (!focus)
			{
				BusManager.SetVolume(BusManager.Master, 0f);
				return;
			}
			BusManager.SetVolume(BusManager.Master, TIPlayerProfileManager.masterVolumeModifier());
		}
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000A1C4 File Offset: 0x000083C4
	private void Update()
	{
		if (TIInputManager.acceptingInput && GameControl.loadcycle100 && ((TIGlobalValuesState.isSpaceCombatEnabled && !GameControl.spaceCombat.combatEnded && !GameControl.spaceCombat.IsInFormationSelectionMode) || (!GameControl.control.skirmishMode && !TIGlobalValuesState.isSpaceCombatEnabled && OperationCanvasController.Singleton.CanSelectArmyGroup())))
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!TIStandaloneInputModule.current.IsPointerOverUIGameObject() && !TIStandaloneInputModule.current.IsPointerOverSpaceCombatUIGameObject())
				{
					TIInputManager._boxSelectStartPosition = Input.mousePosition;
					TIInputManager._dragSelectValid = true;
				}
				else
				{
					TIInputManager._dragSelectValid = false;
				}
			}
			if (Input.GetMouseButtonUp(0) && TIInputManager._isDragSelecting)
			{
				this.OnBoxSelectReleased();
				TIInputManager._isDragSelecting = false;
			}
			if (Input.GetMouseButton(0) && TIInputManager._dragSelectValid && (TIInputManager._boxSelectStartPosition - Input.mousePosition).magnitude > 40f)
			{
				TIInputManager._isDragSelecting = true;
			}
		}
		if (TIInputManager.waitingForKeybind)
		{
			foreach (object obj in Enum.GetValues(typeof(KeyCode)))
			{
				KeyCode keyCode = (KeyCode)obj;
				if (Input.GetKey(keyCode))
				{
					if (this.modifierKeys.Contains(keyCode))
					{
						this.lastModifierKeycode = keyCode;
					}
					if (Input.GetKeyUp(keyCode) && this.lastModifierKeycode == keyCode)
					{
						this.lastModifierKeycode = KeyCode.None;
					}
					if (this.CheckNewKeybind(keyCode, this.lastModifierKeycode) && !this.modifierKeys.Contains(keyCode))
					{
						TIInputManager.SetNewKeybind(TIInputManager.currentRebind.keybindIndex, keyCode, this.lastModifierKeycode);
						this.lastModifierKeycode = KeyCode.None;
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
						return;
					}
					if (!this.modifierKeys.Contains(keyCode))
					{
						TIInputManager.waitingForKeybind = false;
						this.lastModifierKeycode = KeyCode.None;
						TIInputManager.currentRebind.currentKeybindText.text = TIUtilities.CombineStrings(new string[]
						{
							(TIInputManager.keyBindingModifiers[TIInputManager.currentRebind.keybindIndex] != KeyCode.None) ? TIUtilities.CombineStrings(new string[]
							{
								TIInputManager.keyBindingModifiers[TIInputManager.currentRebind.keybindIndex].ToString(),
								"+"
							}) : "",
							TIInputManager.GetKeybind(TIInputManager.currentRebind.keybindIndex)
						});
						TIInputManager.currentRebind = null;
						TIInputManager.UpdateBindings(true);
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
						return;
					}
				}
			}
		}
		if (TIGlobalValuesState.isSpaceCombatEnabled && GeneralControlsController.UIPlayerInTargetingMode && Input.GetKeyUp(KeyCode.Mouse1))
		{
			GeneralControlsController.ShutdownUIGlobalTargetingMode(GameControl.control.activePlayer);
			if (TIGlobalValuesState.isSpaceCombatEnabled)
			{
				GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(null, false, false), null, Array.Empty<object>());
			}
		}
		if (TIGlobalValuesState.isSpaceCombatEnabled && (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)))
		{
			TIInputManager.altitudeHeightOffset = 0f;
		}
		if (TIInputManager.hidingUI && Input.GetKeyUp(KeyCode.Escape) && !TIGlobalValuesState.isSpaceCombatEnabled)
		{
			IHud strategyHud = World.Active.GetExistingManager<CanvasManager>().StrategyHud;
			if (((strategyHud != null) ? strategyHud.GameObject : null) != null)
			{
				TIInputManager.hidingUI = false;
				World.Active.GetExistingManager<CanvasManager>().StrategyHud.GameObject.GetComponent<GeneralControlsController>().RestoreHiddenUI();
			}
		}
		if (TIInputManager.acceptingInput && Input.GetKeyUp(TIInputManager.toggleCombatUI) && !TIGlobalValuesState.isSpaceCombatEnabled)
		{
			IHud strategyHud2 = World.Active.GetExistingManager<CanvasManager>().StrategyHud;
			if (((strategyHud2 != null) ? strategyHud2.GameObject : null) != null)
			{
				if (!TIInputManager.IsShiftKeyDown)
				{
					TIInputManager.hidingUI = !TIInputManager.hidingUI;
					World.Active.GetExistingManager<CanvasManager>().StrategyHud.GameObject.GetComponent<GeneralControlsController>().DebugToggleUI();
					return;
				}
				TIInputManager.ToggleCursorVisibility();
			}
		}
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000A5A8 File Offset: 0x000087A8
	public static void ToggleCursorVisibility()
	{
		Cursor.visible = !Cursor.visible;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000A5B8 File Offset: 0x000087B8
	private void CursorTest()
	{
		this.flickTimer -= Time.deltaTime;
		if (this.flickTimer <= 0f)
		{
			this.flickTimer = this.flickTime;
			if (!this.cSwap)
			{
				TIInputManager.SetCursor(null, false);
				this.cSwap = true;
				return;
			}
			if (this.cSwap)
			{
				TIInputManager.SetCursor(TIInputManager.defaultCursor, false);
				this.cSwap = false;
			}
		}
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000A621 File Offset: 0x00008821
	private void LateUpdate()
	{
		TIInputManager.lastMousePos = Input.mousePosition;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000A630 File Offset: 0x00008830
	public bool CheckNewKeybind(KeyCode newKeycode, KeyCode newModifierKeycode)
	{
		using (List<KeyCode>.Enumerator enumerator = this.forbiddenBindings.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == newKeycode)
				{
					return false;
				}
			}
		}
		foreach (KeyCode keyCode in TIInputManager.keyBindings)
		{
			if (keyCode == newKeycode && TIInputManager.keyBindingModifiers[TIInputManager.keyBindings.IndexOf(keyCode)] == newModifierKeycode)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000A6E0 File Offset: 0x000088E0
	public static void SetNewKeybind(int index, KeyCode newKeycode, KeyCode newModifierKeycode = KeyCode.None)
	{
		TIInputManager.waitingForKeybind = false;
		TIInputManager.currentRebind.currentKeybindText.text = TIUtilities.CombineStrings(new string[]
		{
			(newModifierKeycode != KeyCode.None) ? TIUtilities.CombineStrings(new string[]
			{
				newModifierKeycode.ToString(),
				"+"
			}) : "",
			newKeycode.ToString()
		});
		TIInputManager.keyBindings[index] = newKeycode;
		TIInputManager.keyBindingModifiers[index] = newModifierKeycode;
		TIInputManager.UpdateBindings(true);
		TIInputManager.currentRebind = null;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000A774 File Offset: 0x00008974
	public static string GetKeybind(int index)
	{
		return TIInputManager.keyBindings[index].ToString();
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000A79C File Offset: 0x0000899C
	public static string GetReserveKeybind(int index)
	{
		if (TIInputManager.acceptingInput)
		{
			return TIInputManager.GetKeybind(index);
		}
		List<KeyCode> list = TIInputManager.keyBindingsReserve;
		if (list != null && list.Count <= index)
		{
			return KeyCode.None.ToString();
		}
		return TIInputManager.keyBindingsReserve[index].ToString();
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000A7FC File Offset: 0x000089FC
	public static string GetKeybindWithModifiers(int index)
	{
		return TIUtilities.CombineStrings(new string[]
		{
			(TIInputManager.keyBindingModifiers[index] == KeyCode.None) ? "" : TIUtilities.CombineStrings(new string[]
			{
				TIInputManager.keyBindingModifiers[index].ToString(),
				"+"
			}),
			TIInputManager.GetReserveKeybind(index)
		});
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000A864 File Offset: 0x00008A64
	public static void SetDefaultCursor(bool useFactionCursor = true)
	{
		if (useFactionCursor && GameControl.control.activePlayer != null)
		{
			TIInputManager.defaultCursor = GameControl.assetLoader.LoadAssetForTexture2DAssignment(GameControl.control.activePlayer.cursorPath);
		}
		else
		{
			TIInputManager.defaultCursor = TIInputManager.cursorNeutral;
		}
		TIInputManager.SetCursor(TIInputManager.defaultCursor, false);
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000A8BC File Offset: 0x00008ABC
	public static Texture2D GetFactionCursor(string presetName)
	{
		if (GameControl.assetLoader != null)
		{
			TIInputManager.factionCursor = GameControl.assetLoader.LoadAssetForTexture2DAssignment(GameControl.control.activePlayer.cursorPath);
		}
		if (GameControl.assetLoader != null)
		{
			TIInputManager.targetCursorValid = GameControl.assetLoader.LoadAssetForTexture2DAssignment(new StringBuilder(GameControl.control.activePlayer.cursorPath).Append("_Valid").ToString());
		}
		if (GameControl.assetLoader != null)
		{
			TIInputManager.targetCursor = GameControl.assetLoader.LoadAssetForTexture2DAssignment(new StringBuilder(GameControl.control.activePlayer.cursorPath).Append("_Invalid").ToString());
		}
		return TIInputManager.factionCursor;
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000A968 File Offset: 0x00008B68
	public static Texture2D CombineCursors(Texture2D cur1, Texture2D cur2)
	{
		Color[] pixels = cur1.GetPixels();
		Color[] pixels2 = cur2.GetPixels();
		Color[] array = new Color[pixels.Length];
		for (int i = 0; i < pixels.Length; i++)
		{
			float num = pixels2[i].r * pixels2[i].a + pixels[i].r * (1f - pixels2[i].a);
			float num2 = pixels2[i].g * pixels2[i].a + pixels[i].g * (1f - pixels2[i].a);
			float num3 = pixels2[i].b * pixels2[i].a + pixels[i].b * (1f - pixels2[i].a);
			float num4 = pixels2[i].a + pixels[i].a * (1f - pixels2[i].a);
			array[i] = new Color(num, num2, num3, num4);
		}
		Texture2D texture2D = new Texture2D(128, 128, TextureFormat.RGBA32, false);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600015C RID: 348 RVA: 0x0000AAB4 File Offset: 0x00008CB4
	public static Texture2D ScaleTextureLinux(Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D texture2D = new Texture2D(targetWidth, targetHeight, source.format, true);
		Color[] pixels = texture2D.GetPixels(0);
		float num = 1f / (float)targetWidth;
		float num2 = 1f / (float)targetHeight;
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = source.GetPixelBilinear(num * ((float)i % (float)targetWidth), num2 * Mathf.Floor((float)(i / targetWidth)));
		}
		texture2D.SetPixels(pixels, 0);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600015D RID: 349 RVA: 0x0000AB30 File Offset: 0x00008D30
	public static void SetCursorNew(Texture2D cursorSprite, bool targetting = false)
	{
		if (TIPlayerProfileManager.usingWindowsCursor)
		{
			cursorSprite = null;
		}
		if (cursorSprite != null)
		{
			if (targetting)
			{
				Cursor.SetCursor(cursorSprite, new Vector2((float)(cursorSprite.width / 2), (float)(cursorSprite.height / 2)), CursorMode.Auto);
			}
			else
			{
				Cursor.SetCursor(cursorSprite, new Vector2((float)(cursorSprite.width / 2), (float)(cursorSprite.height / 2)), CursorMode.Auto);
			}
		}
		if (cursorSprite == null)
		{
			Cursor.SetCursor(cursorSprite, new Vector2(0f, 0f), CursorMode.Auto);
			if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Windows && !Cursor.visible)
			{
				Cursor.visible = true;
			}
			if (TIInputManager.inTargetingMode)
			{
				TIInputManager.inTargetingMode = false;
			}
		}
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000ABD2 File Offset: 0x00008DD2
	public static void SetCursor(Texture2D cursorSprite, bool targetting = false)
	{
		TIInputManager.SetCursorNew(cursorSprite, targetting);
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000ABDC File Offset: 0x00008DDC
	public static void CreateTargetingCursor(Texture2D cursorSprite)
	{
		new Texture2D(128, 128, TextureFormat.RGBA32, false);
		Texture2D texture2D = TIInputManager.GetFactionCursor(GameControl.control.activePlayer.template.defaultPresetName);
		Texture2D texture2D2 = new Texture2D(128, 128, TextureFormat.RGBA32, false);
		texture2D2 = TIInputManager.CombineCursors(texture2D, cursorSprite);
		if (cursorSprite == TIInputManager.targetCursorValid)
		{
			TIInputManager.faction_TargetCursorValid = texture2D2;
		}
		if (cursorSprite == TIInputManager.targetCursor)
		{
			TIInputManager.faction_TargetCursorInvalid = texture2D2;
		}
		if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Linux)
		{
			Cursor.SetCursor(texture2D2, new Vector2((float)(texture2D2.width / 2), (float)(texture2D2.height / 2)), CursorMode.Auto);
		}
		if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
		{
			Cursor.SetCursor(TIInputManager.ScaleTextureLinux(texture2D2, 32, 32), new Vector2((float)(cursorSprite.width / 8), (float)(cursorSprite.height / 8)), CursorMode.ForceSoftware);
		}
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000ACA8 File Offset: 0x00008EA8
	private void OnBoxSelectReleased()
	{
		TIInputManager._verts = new Vector3[4];
		TIInputManager._vecs = new Vector3[4];
		int num = 0;
		TIInputManager._boxSelectEndPosition = Input.mousePosition;
		TIInputManager._corners = this.getBoundingBox(TIInputManager._boxSelectStartPosition, TIInputManager._boxSelectEndPosition);
		if (TIInputManager._corners[1].x == TIInputManager._corners[2].x || TIInputManager._corners[1].y == TIInputManager._corners[2].y)
		{
			return;
		}
		foreach (Vector2 vector in TIInputManager._corners)
		{
			Ray ray = TIInputManager.mainCamera.ScreenPointToRay(vector);
			TIInputManager._verts[num] = TIInputManager.mainCamera.ScreenToWorldPoint(new Vector3(vector.x, vector.y, 50000f));
			TIInputManager._vecs[num] = ray.origin - TIInputManager._verts[num];
			Debug.DrawLine(TIInputManager.mainCamera.ScreenToWorldPoint(vector), TIInputManager.hit.point, Color.red, 1f);
			num++;
		}
		TIInputManager._selectionMesh = this.generateSelectionMesh(TIInputManager._verts, TIInputManager._vecs);
		TIInputManager._selectionBox = base.gameObject.AddComponent<MeshCollider>();
		TIInputManager._selectionBox.sharedMesh = TIInputManager._selectionMesh;
		TIInputManager._selectionBox.convex = true;
		TIInputManager._selectionBox.isTrigger = true;
		base.Invoke("CleanUpSelectionBox", 0.02f);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000AE44 File Offset: 0x00009044
	private Vector2[] getBoundingBox(Vector2 p1, Vector2 p2)
	{
		Vector3 vector = Vector3.Min(p1, p2);
		Vector3 vector2 = Vector3.Max(p1, p2);
		return new Vector2[]
		{
			new Vector2(vector.x, vector2.y),
			new Vector2(vector2.x, vector2.y),
			new Vector2(vector.x, vector.y),
			new Vector2(vector2.x, vector.y)
		};
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000AEDC File Offset: 0x000090DC
	private Mesh generateSelectionMesh(Vector3[] corners, Vector3[] vecs)
	{
		Vector3[] array = new Vector3[8];
		int[] array2 = new int[]
		{
			0, 1, 2, 2, 1, 3, 4, 6, 0, 0,
			6, 2, 6, 7, 2, 2, 7, 3, 7, 5,
			3, 3, 5, 1, 5, 0, 1, 1, 4, 0,
			4, 5, 6, 6, 5, 7
		};
		for (int i = 0; i < 4; i++)
		{
			array[i] = corners[i];
		}
		for (int j = 4; j < 8; j++)
		{
			array[j] = corners[j - 4] + vecs[j - 4];
		}
		return new Mesh
		{
			vertices = array,
			triangles = array2
		};
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000AF5C File Offset: 0x0000915C
	private void CleanUpSelectionBox()
	{
		if (TIGlobalValuesState.isSpaceCombatEnabled)
		{
			bool isControlKeyDown = TIInputManager.IsControlKeyDown;
			bool flag = TIInputManager.IsShiftKeyDown && !isControlKeyDown;
			if (GameControl.spaceCombat._boxSelectedUIControllers.Count > 0 && !flag && !isControlKeyDown)
			{
				GameControl.spaceCombat.combatHUD.ClearGroupSelect();
			}
			for (int i = 0; i < GameControl.spaceCombat._boxSelectedUIControllers.Count; i++)
			{
				ShipUIController shipUIController = GameControl.spaceCombat._boxSelectedUIControllers[i];
				if (!shipUIController.ship.ShipDestroyed())
				{
					GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(shipUIController.ship, true, i == 0), null, Array.Empty<object>());
				}
			}
			GameControl.spaceCombat._boxSelectedUIControllers.Clear();
			global::UnityEngine.Object.Destroy(TIInputManager._selectionBox);
		}
		if (TIInputManager._boxSelectedMarkerControllers == null)
		{
			List<MarkerController> boxSelectedMarkerControllers = TIInputManager._boxSelectedMarkerControllers;
			if (boxSelectedMarkerControllers != null)
			{
				boxSelectedMarkerControllers.Clear();
			}
			global::UnityEngine.Object.Destroy(TIInputManager._selectionBox);
			return;
		}
		int count = TIInputManager._boxSelectedMarkerControllers.Count;
		List<TIArmyState> list = new List<TIArmyState>();
		for (int j = 0; j < TIInputManager._boxSelectedMarkerControllers.Count; j++)
		{
			MarkerController markerController = TIInputManager._boxSelectedMarkerControllers[j];
			if (TIGameState.Valid(markerController.Army))
			{
				list.Add(markerController.Army);
				List<TIArmyState> list2 = null;
				if (markerController.ArmyMarkerController.defendingArmies.Contains(markerController.Army))
				{
					list2 = markerController.ArmyMarkerController.defendingArmies;
				}
				else if (markerController.ArmyMarkerController.attackingArmies.Contains(markerController.Army))
				{
					list2 = markerController.ArmyMarkerController.attackingArmies;
				}
				else if (markerController.ArmyMarkerController.megafaunaArmies.Contains(markerController.Army))
				{
					list2 = markerController.ArmyMarkerController.megafaunaArmies;
				}
				if (list2 != null)
				{
					foreach (TIArmyState tiarmyState in list2)
					{
						if (TIGameState.Valid(tiarmyState) && tiarmyState.ref_faction == GameControl.control.activePlayer && !list.Contains(tiarmyState))
						{
							list.Add(tiarmyState);
						}
					}
				}
			}
		}
		GameControl.eventManager.TriggerEvent(new MultiSelectArmiesSelected(list), null, Array.Empty<object>());
		TIInputManager._boxSelectedMarkerControllers.Clear();
		global::UnityEngine.Object.Destroy(TIInputManager._selectionBox);
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000B1C0 File Offset: 0x000093C0
	public void CancelBoxSelect(CombatPauseMenuOpened e)
	{
		if (TIInputManager._isDragSelecting)
		{
			TIInputManager._isDragSelecting = false;
			global::UnityEngine.Object.Destroy(TIInputManager._selectionBox);
		}
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000B1D9 File Offset: 0x000093D9
	public static void CancelBoxSelect()
	{
		if (TIInputManager._isDragSelecting)
		{
			TIInputManager._isDragSelecting = false;
			global::UnityEngine.Object.Destroy(TIInputManager._selectionBox);
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000B1F4 File Offset: 0x000093F4
	private void OnTriggerEnter(Collider other)
	{
		if (TIGlobalValuesState.isSpaceCombatEnabled)
		{
			ShipUIController component = other.GetComponent<ShipUIController>();
			if (component && !component.IsShipDestroyed() && component.ship.faction == GameControl.control.activePlayer)
			{
				GameControl.spaceCombat._boxSelectedUIControllers.Add(component);
				return;
			}
		}
		else
		{
			MarkerController component2 = other.GetComponent<MarkerController>();
			if (component2 == null)
			{
				return;
			}
			ArmyMarkerController armyMarkerController = null;
			if (component2.IsArmyMarker)
			{
				armyMarkerController = component2.ArmyMarkerController;
			}
			if (component2.associatedState == null || !component2.associatedState.isArmyState)
			{
				return;
			}
			if (armyMarkerController != null && component2.associatedState.ref_faction == GameControl.control.activePlayer)
			{
				TIInputManager._boxSelectedMarkerControllers.Add(component2);
			}
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000B2C0 File Offset: 0x000094C0
	private void OnGUI()
	{
		if (TIInputManager._isDragSelecting)
		{
			TIInputManager.BoxSelectionUtils.DrawScreenRectBorder(TIInputManager.BoxSelectionUtils.GetScreenRect(TIInputManager._boxSelectStartPosition, Input.mousePosition), 2f, TIInputManager._boxColor);
		}
	}

	// Token: 0x04000129 RID: 297
	public static bool acceptingInput = true;

	// Token: 0x0400012A RID: 298
	public static bool waitingForKeybind = false;

	// Token: 0x0400012B RID: 299
	public static bool inTargetingMode = false;

	// Token: 0x0400012C RID: 300
	public static bool blockSelectionRaycasts = false;

	// Token: 0x0400012D RID: 301
	public static bool blockCombatZoom = false;

	// Token: 0x0400012E RID: 302
	public static bool receivingInputForNarrativeHotkeys = false;

	// Token: 0x0400012F RID: 303
	private static bool hidingUI = false;

	// Token: 0x04000130 RID: 304
	public static Keybind_UIMenuObject currentRebind;

	// Token: 0x04000131 RID: 305
	public static List<KeyCode> keyBindings = new List<KeyCode>();

	// Token: 0x04000132 RID: 306
	public static List<KeyCode> keyBindingModifiers = new List<KeyCode>();

	// Token: 0x04000133 RID: 307
	public static List<KeyCode> keyBindingsReserve = new List<KeyCode>();

	// Token: 0x04000134 RID: 308
	[Tooltip("These are hard-bound to specific game functions and are not allowed to be bound by the user")]
	public List<KeyCode> forbiddenBindings = new List<KeyCode>
	{
		KeyCode.Escape,
		KeyCode.LeftControl,
		KeyCode.RightControl,
		KeyCode.LeftShift,
		KeyCode.RightShift,
		KeyCode.LeftAlt,
		KeyCode.RightAlt,
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Tilde,
		KeyCode.Tab
	};

	// Token: 0x04000135 RID: 309
	public List<KeyCode> modifierKeys = new List<KeyCode>
	{
		KeyCode.LeftControl,
		KeyCode.RightControl,
		KeyCode.LeftShift,
		KeyCode.RightShift,
		KeyCode.LeftAlt,
		KeyCode.RightAlt
	};

	// Token: 0x04000136 RID: 310
	public static KeyCode Objectives = KeyCode.F1;

	// Token: 0x04000137 RID: 311
	public static KeyCode PoliticalEarth = KeyCode.F2;

	// Token: 0x04000138 RID: 312
	public static KeyCode SolarSystem = KeyCode.F3;

	// Token: 0x04000139 RID: 313
	public static KeyCode Councilors = KeyCode.F4;

	// Token: 0x0400013A RID: 314
	public static KeyCode Nations = KeyCode.F5;

	// Token: 0x0400013B RID: 315
	public static KeyCode Habitats = KeyCode.F6;

	// Token: 0x0400013C RID: 316
	public static KeyCode Fleets = KeyCode.F7;

	// Token: 0x0400013D RID: 317
	public static KeyCode Research = KeyCode.F8;

	// Token: 0x0400013E RID: 318
	public static KeyCode Intel = KeyCode.F9;

	// Token: 0x0400013F RID: 319
	public static KeyCode CycleRecolorEarthMap = KeyCode.Home;

	// Token: 0x04000140 RID: 320
	public static KeyCode ToggleOrbitTrails = KeyCode.End;

	// Token: 0x04000141 RID: 321
	public static KeyCode ToggleExpandNewsFeed = KeyCode.N;

	// Token: 0x04000142 RID: 322
	public static KeyCode QuickSave = KeyCode.F10;

	// Token: 0x04000143 RID: 323
	public static KeyCode ToggleHelper = KeyCode.F11;

	// Token: 0x04000144 RID: 324
	public static KeyCode ToggleDistanceSymbols = KeyCode.Insert;

	// Token: 0x04000145 RID: 325
	public static KeyCode ToggleProspectData = KeyCode.P;

	// Token: 0x04000146 RID: 326
	public static KeyCode ToggleShowAllColonizedBodyNames = KeyCode.L;

	// Token: 0x04000147 RID: 327
	public static KeyCode OpenShipDesigner = KeyCode.U;

	// Token: 0x04000148 RID: 328
	public static KeyCode OpenConstructionManager = KeyCode.I;

	// Token: 0x04000149 RID: 329
	public static KeyCode OpenGlobalSearch = KeyCode.F;

	// Token: 0x0400014A RID: 330
	public static KeyCode AccessibilityMagnifier = KeyCode.Backslash;

	// Token: 0x0400014B RID: 331
	public static KeyCode IncreaseSpeed = KeyCode.KeypadPlus;

	// Token: 0x0400014C RID: 332
	public static KeyCode DecreaseSpeed = KeyCode.KeypadMinus;

	// Token: 0x0400014D RID: 333
	public static KeyCode PauseSpeed = KeyCode.Space;

	// Token: 0x0400014E RID: 334
	public static KeyCode PauseSpeedNoToggle = KeyCode.Backspace;

	// Token: 0x0400014F RID: 335
	public static KeyCode SetSpeedIndex1 = KeyCode.Alpha1;

	// Token: 0x04000150 RID: 336
	public static KeyCode SetSpeedIndex2 = KeyCode.Alpha2;

	// Token: 0x04000151 RID: 337
	public static KeyCode SetSpeedIndex3 = KeyCode.Alpha3;

	// Token: 0x04000152 RID: 338
	public static KeyCode SetSpeedIndex4 = KeyCode.Alpha4;

	// Token: 0x04000153 RID: 339
	public static KeyCode SetSpeedIndex5 = KeyCode.Alpha5;

	// Token: 0x04000154 RID: 340
	public static KeyCode SetSpeedIndex6 = KeyCode.Alpha6;

	// Token: 0x04000155 RID: 341
	public static KeyCode cameraLeft = KeyCode.A;

	// Token: 0x04000156 RID: 342
	public static KeyCode cameraRight = KeyCode.D;

	// Token: 0x04000157 RID: 343
	public static KeyCode cameraUp = KeyCode.W;

	// Token: 0x04000158 RID: 344
	public static KeyCode cameraDown = KeyCode.S;

	// Token: 0x04000159 RID: 345
	public static KeyCode cameraZoomIn = KeyCode.T;

	// Token: 0x0400015A RID: 346
	public static KeyCode cameraZoomOut = KeyCode.B;

	// Token: 0x0400015B RID: 347
	public static KeyCode altitudeControl = KeyCode.Q;

	// Token: 0x0400015C RID: 348
	public static KeyCode lateralControl = KeyCode.E;

	// Token: 0x0400015D RID: 349
	public static KeyCode burnControl = KeyCode.R;

	// Token: 0x0400015E RID: 350
	public static KeyCode yawControl = KeyCode.Z;

	// Token: 0x0400015F RID: 351
	public static KeyCode pitchControl = KeyCode.X;

	// Token: 0x04000160 RID: 352
	public static KeyCode rollControl = KeyCode.C;

	// Token: 0x04000161 RID: 353
	public static KeyCode cycleShipsUp = KeyCode.PageUp;

	// Token: 0x04000162 RID: 354
	public static KeyCode cycleShipsDown = KeyCode.PageDown;

	// Token: 0x04000163 RID: 355
	public static KeyCode toggleGrid = KeyCode.G;

	// Token: 0x04000164 RID: 356
	public static KeyCode toggleCombatUI = KeyCode.Delete;

	// Token: 0x04000165 RID: 357
	public static KeyCode toggleShipWaypoints = KeyCode.V;

	// Token: 0x04000166 RID: 358
	public static KeyCode toggleFPSWidget = KeyCode.Slash;

	// Token: 0x04000167 RID: 359
	public static KeyCode fleetCommandSelectPrimaryTarget = KeyCode.H;

	// Token: 0x04000168 RID: 360
	public static KeyCode fleetCommandLaunchMissileSalvo = KeyCode.J;

	// Token: 0x04000169 RID: 361
	public static KeyCode controlGroup1 = KeyCode.Alpha1;

	// Token: 0x0400016A RID: 362
	public static KeyCode controlGroup2 = KeyCode.Alpha2;

	// Token: 0x0400016B RID: 363
	public static KeyCode controlGroup3 = KeyCode.Alpha3;

	// Token: 0x0400016C RID: 364
	public static KeyCode controlGroup4 = KeyCode.Alpha4;

	// Token: 0x0400016D RID: 365
	public static KeyCode controlGroup5 = KeyCode.Alpha5;

	// Token: 0x0400016E RID: 366
	public static KeyCode controlGroup6 = KeyCode.Alpha6;

	// Token: 0x0400016F RID: 367
	public static KeyCode controlGroup7 = KeyCode.Alpha7;

	// Token: 0x04000170 RID: 368
	public static KeyCode controlGroup8 = KeyCode.Alpha8;

	// Token: 0x04000171 RID: 369
	public static KeyCode controlGroup9 = KeyCode.Alpha9;

	// Token: 0x04000172 RID: 370
	public static KeyCode controlGroup0 = KeyCode.Alpha0;

	// Token: 0x04000173 RID: 371
	public Texture2D cursor_Resist;

	// Token: 0x04000174 RID: 372
	public Texture2D cursor_Alien;

	// Token: 0x04000175 RID: 373
	public Texture2D cursor_Appease;

	// Token: 0x04000176 RID: 374
	public Texture2D cursor_Cooperate;

	// Token: 0x04000177 RID: 375
	public Texture2D cursor_Destroy;

	// Token: 0x04000178 RID: 376
	public Texture2D cursor_Escape;

	// Token: 0x04000179 RID: 377
	public Texture2D cursor_Exploit;

	// Token: 0x0400017A RID: 378
	public Texture2D cursor_Main;

	// Token: 0x0400017B RID: 379
	public Texture2D cursor_Neutral;

	// Token: 0x0400017C RID: 380
	public Texture2D default_Cursor;

	// Token: 0x0400017D RID: 381
	public Texture2D target_Cursor;

	// Token: 0x0400017E RID: 382
	public Texture2D target_CursorValid;

	// Token: 0x0400017F RID: 383
	public Texture2D target_CursorInvalid;

	// Token: 0x04000180 RID: 384
	public static Texture2D faction_TargetCursorInvalid;

	// Token: 0x04000181 RID: 385
	public static Texture2D faction_TargetCursorValid;

	// Token: 0x04000182 RID: 386
	public static Texture2D cursorResist;

	// Token: 0x04000183 RID: 387
	public static Texture2D cursorAlien;

	// Token: 0x04000184 RID: 388
	public static Texture2D cursorAppease;

	// Token: 0x04000185 RID: 389
	public static Texture2D cursorCooperate;

	// Token: 0x04000186 RID: 390
	public static Texture2D cursorDestroy;

	// Token: 0x04000187 RID: 391
	public static Texture2D cursorEscape;

	// Token: 0x04000188 RID: 392
	public static Texture2D cursorExploit;

	// Token: 0x04000189 RID: 393
	public static Texture2D cursorMain;

	// Token: 0x0400018A RID: 394
	public static Texture2D cursorNeutral;

	// Token: 0x0400018B RID: 395
	public static Texture2D targetCursor;

	// Token: 0x0400018C RID: 396
	public static Texture2D targetCursorValid;

	// Token: 0x0400018D RID: 397
	public static Texture2D targetCursorInvalid;

	// Token: 0x0400018E RID: 398
	public static Texture2D defaultCursor;

	// Token: 0x0400018F RID: 399
	public bool cursorInit;

	// Token: 0x04000190 RID: 400
	private float flickTime = 0.25f;

	// Token: 0x04000191 RID: 401
	private float flickTimer = 0.25f;

	// Token: 0x04000192 RID: 402
	private bool cSwap;

	// Token: 0x04000193 RID: 403
	public static Vector3 lastMousePos;

	// Token: 0x04000194 RID: 404
	public static TIGameState lastClickedGameState;

	// Token: 0x04000195 RID: 405
	public static float lastClickedTimeStamp;

	// Token: 0x04000196 RID: 406
	private static bool _isDragSelecting;

	// Token: 0x04000197 RID: 407
	private static bool _dragSelectValid;

	// Token: 0x04000198 RID: 408
	private static Vector3 _boxSelectStartPosition;

	// Token: 0x04000199 RID: 409
	private static Vector3 _boxSelectEndPosition;

	// Token: 0x0400019A RID: 410
	private static Color _boxColor = new Color(0.843f, 0.98f, 0.988f);

	// Token: 0x0400019B RID: 411
	private static RaycastHit hit;

	// Token: 0x0400019C RID: 412
	private static MeshCollider _selectionBox;

	// Token: 0x0400019D RID: 413
	private static Mesh _selectionMesh;

	// Token: 0x0400019E RID: 414
	private static Camera _mainCamera;

	// Token: 0x0400019F RID: 415
	private static List<MarkerController> _boxSelectedMarkerControllers = new List<MarkerController>();

	// Token: 0x040001A0 RID: 416
	private static Vector2[] _corners;

	// Token: 0x040001A1 RID: 417
	private static Vector3[] _verts;

	// Token: 0x040001A2 RID: 418
	private static Vector3[] _vecs;

	// Token: 0x040001A3 RID: 419
	public KeyCode lastModifierKeycode;

	// Token: 0x040001A4 RID: 420
	public static float altitudeHeightOffset;

	// Token: 0x040001A5 RID: 421
	private static Texture2D factionCursor;

	// Token: 0x02000AC4 RID: 2756
	public static class BoxSelectionUtils
	{
		// Token: 0x1700112E RID: 4398
		// (get) Token: 0x0600660F RID: 26127 RVA: 0x002FEECF File Offset: 0x002FD0CF
		public static Texture2D WhiteTexture
		{
			get
			{
				if (TIInputManager.BoxSelectionUtils._whiteTexture == null)
				{
					TIInputManager.BoxSelectionUtils._whiteTexture = new Texture2D(1, 1);
					TIInputManager.BoxSelectionUtils._whiteTexture.SetPixel(0, 0, Color.white);
					TIInputManager.BoxSelectionUtils._whiteTexture.Apply();
				}
				return TIInputManager.BoxSelectionUtils._whiteTexture;
			}
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x002FEF0A File Offset: 0x002FD10A
		public static void DrawScreenRect(Rect rect, Color color)
		{
			GUI.color = color;
			GUI.DrawTexture(rect, TIInputManager.BoxSelectionUtils.WhiteTexture);
			GUI.color = Color.white;
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x002FEF28 File Offset: 0x002FD128
		public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
		{
			TIInputManager.BoxSelectionUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
			TIInputManager.BoxSelectionUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
			TIInputManager.BoxSelectionUtils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
			TIInputManager.BoxSelectionUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x002FEFC0 File Offset: 0x002FD1C0
		public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
		{
			screenPosition1.y = (float)Screen.height - screenPosition1.y;
			screenPosition2.y = (float)Screen.height - screenPosition2.y;
			Vector3 vector = Vector3.Min(screenPosition1, screenPosition2);
			Vector3 vector2 = Vector3.Max(screenPosition1, screenPosition2);
			return Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
		}

		// Token: 0x04004868 RID: 18536
		private static Texture2D _whiteTexture;
	}

	// Token: 0x02000AC5 RID: 2757
	public enum KeyPressMode
	{
		// Token: 0x0400486A RID: 18538
		Down,
		// Token: 0x0400486B RID: 18539
		Up,
		// Token: 0x0400486C RID: 18540
		Continous
	}
}
