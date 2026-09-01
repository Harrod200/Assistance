# Voice-Over Event Fix - v0.5.3 Hotfix

## Issue
When the Assist mission was assigned to councilors, the game attempted to play voice-over (VO) events that don't exist in the mod:

```
AudioManager failed to play sound event: event:/VO/ENG/LATN/M/0/Assist/Assigned
AudioManager failed to play sound event: event:/VO/ENG/HING/M/1/Assist/Assigned
AudioManager failed to play sound event: event:/VO/ENG/AFR/M/1/Assist/Assigned
```

## Root Cause
The game's `TICouncilorVoiceTemplate.cs` automatically generates VO event paths when a mission is assigned:
- Format: `event:/VO/{VoiceTemplate}/{MissionDataName}/{VoiceSituation}`
- Our mission uses dataName="Assist", so it looks for VO events like:
  - `event:/VO/ENG/LATN/M/0/Assist/Assigned` (on mission assignment)
  - `event:/VO/ENG/LATN/M/0/Assist/Success` (on mission completion - success)
  - `event:/VO/ENG/LATN/M/0/Assist/Failure` (on mission completion - failure)

Since the Assist mission doesn't have recorded voice lines, these events don't exist, causing the AudioManager to fail gracefully with error messages.

## Solution
Added empty VO event entries to `TIMissionTemplate.en` to suppress the errors:

```
TIMissionTemplate_Assist.voiceEvent.Assigned.Assist=
TIMissionTemplate_Assist.voiceEvent.Success.Assist=
TIMissionTemplate_Assist.voiceEvent.Failure.Assist=
TIMissionTemplate_Assist.voiceEvent.Aborted.Assist=
```

These entries explicitly define (as empty) the VO events for the Assist mission, preventing the game from attempting to load non-existent audio files.

## Impact
- ✅ Eliminates AudioManager errors in Player.log
- ✅ No functional impact on the mod (mission works identically)
- ✅ Cleaner log output for better debugging

## Future Enhancement
If voice lines are recorded for the Assist mission, these entries can be replaced with the actual VO event paths:
```
TIMissionTemplate_Assist.voiceEvent.Assigned.Assist=event:/VO/ENG/LATN/M/0/Assist/Assigned
```

## Testing
Deploy v0.5.3-hotfix and verify no "AudioManager failed to play sound event" errors appear for the Assist mission in Player.log.
