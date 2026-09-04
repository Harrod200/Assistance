# Mission Complete Screen Call Flow

## Overview
This document traces where the mission complete screen is called in the decompiled Terra Invicta code.

## Call Chain

### 1. Mission Resolution
**File:** `PavonisInteractive/TerraInvicta/TIMissionState.cs`
- **Method:** `ResolveMission()` (line ~304)
- **Action:** Calculates mission outcome and prepares result
- **Outcome:** Creates a `MissionResult` struct with mission outcome

### 2. Logging Mission Outcome
**File:** `PavonisInteractive/TerraInvicta/TIMissionState.cs` 
- **Line:** ~563
- **Code:**
  ```csharp
  TINotificationQueueState.LogMissionOutcome(this, missionResult, ref_faction, list3, list2, false, abortReasonDetail);
  ```
- **Purpose:** Sends mission outcome to notification system

### 3. Creating Notification Item
**File:** `PavonisInteractive/TerraInvicta/TINotificationQueueState.cs`
- **Method:** `LogMissionOutcome()` (line ~3662)
- **Actions:**
  1. Creates `NotificationQueueItem` with mission details
  2. Determines if contested or uncontested mission
  3. Sets headline, summary, and detail text
  4. Adds illustration resource
  5. Adds special notification delegates (RepeatMission, PermanentAssignment, etc.)
  6. Calls `AddItem()` to queue the notification (line ~3867)

### 4. Adding to Notification Queue
**File:** `PavonisInteractive/TerraInvicta/TINotificationQueueState.cs`
- **Method:** `AddItem()` (line ~106)
- **Actions:**
  1. Validates notification template
  2. Sets timestamp and date string
  3. Inserts notification into `notificationQueue` (line ~131)
  4. Maintains queue size (max 60 items)
  5. Triggers alerts/prompts if needed
  6. Creates summary item for display

## Key Classes and Structures

### NotificationQueueItem
Contains all mission result information:
- `itemHeadline` - Mission name and outcome
- `itemSummary` - Brief summary with date
- `itemDetail` - Detailed mission result with context
- `icon` - Mission icon
- `popupResource1` - Councilor icon
- `popupResource2` - Mission icon
- `backgroundColor` - Faction color
- `outcome` - Mission outcome enum
- `mission` - Reference to TIMissionState

### Key Properties Set for Mission Complete Screen
```csharp
notificationQueueItem.outcome = result.missionOutcome;
notificationQueueItem.mission = mission;
notificationQueueItem.illustrationResource = (flag ? mission.missionTemplate.GetCompletedIllustrationResource(mission.target, ticontrolPoint) : string.Empty);
notificationQueueItem.animationSpriteSheetPath = mission.missionTemplate.resolvingAnimation;
notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
notificationQueueItem.popupResource2 = mission.missionTemplate.missionIconImagePath_Off;
```

## Contested vs Uncontested

The notification type varies based on mission type:

**Contested Mission:**
- Template: `LogMissionOutcome` or `LogMissionOutcome_Permanent` or `LogMissionOutcome_Spy`
- Shows success chance and roll roll result
- Format: `UI.Notifications.ContestedResult`

**Uncontested Mission:**
- Appends `_Uncontested` to template name
- Format: `UI.Notifications.UncontestedResult`
- No success chance display

## Patching Opportunity

To intercept the mission complete screen, you could patch:

1. **`TINotificationQueueState.LogMissionOutcome()`** - Modify notification before it's queued
   - Change content, appearance, or add custom data
   - Can access full mission context and result

2. **`TINotificationQueueState.AddItem()`** - Modify queued item before display
   - Last chance to modify notification item
   - Queue is finalized here

3. **`TIMissionState.ResolveMission()`** - Earliest interception point
   - Modify mission result before notification creation
   - Can access all mission context

## Related Files
- `PavonisInteractive/TerraInvicta/TIMissionState.cs` - Mission state and resolution
- `PavonisInteractive/TerraInvicta/TINotificationQueueState.cs` - Notification queuing (6,872 lines)
- `PavonisInteractive/TerraInvicta/Actions/FinalizeCouncilorMissions.cs` - Mission phase finalization
