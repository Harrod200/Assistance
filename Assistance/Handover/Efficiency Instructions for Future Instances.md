# Efficiency Instructions for Future Instances

Read this BEFORE anything else. It distils the lessons from sessions that
wasted 30-50% of their budget re-deriving known things.

## Golden rules

1. **Run PREFLIGHT.sh first.** It installs the SDK, restores the NuGet cache,
   builds the code, and verifies the runtime caches. Cold start to green
   build in one command. Do not troubleshoot build errors before running it
   - 90% are environmental (wiped home), not code errors.

2. **Search the reference docs before decompiling.**
   - Docs/Terra Invicta Class & Method Reference.md (2.9 MB, VERIFIED):
     full class/method/field index of the decompiled game. Always search
     this before opening decompiled source.
   - Docs/Vanilla API Cheat Sheet.md (project-specific, grows): what the
     project actually uses, with semantics.
   - If any summary doc conflicts with either of these, the summary is wrong.
   - If the cheat sheet is missing a fact you had to derive, ADD IT before
     the session ends. The cheat sheet only helps if it grows.

3. **Do not trust AI-generated summary docs** without spot-checking at least
   two claims against decompiled source. Known-bad files (do not use, do not
   put in a handover zip): Terra_Invicta_Decompiled_Architecture.md and
   Terra_Invicta_Decompiled_Architecture(1).md. They invent methods (e.g.
   TIGameState.PreTurnUpdate) that do not exist.

4. **Persist findings as you go, not at session end.** Append to the cheat
   sheet and the decision log the moment a fact is confirmed. Sessions can
   die mid-work; end-of-session write-ups die with them.

5. **Do not re-derive cached results.** Cached Data/ holds the baked polygon
   distances. Tools/ holds the bake script. If data is missing, re-run the
   bake - never re-implement the maths.

6. **Env quirks worth knowing immediately:**
   - $HOME and /tmp are wiped on VM boot; nothing there survives.
   - /rool-drive persists across boots. Keep handover zips there.
   - The csproj is old-style net48 built with the modern SDK via
     FrameworkPathOverride - see Build Setup.md before "fixing" it.
   - HintPaths in the csproj may point at the owner's Steam install. Correct
     paths live in ref-dlls/.

7. **Update the session log at every milestone** (build green, C-task done,
   decision made), not just at the end. The next instance reads the log
   first; stale logs cause duplicate work.

8. **State of the project lives in Handover Notes vN.md + the decision
   log.** If you finish a C-task, bump the version, update the notes, and
   re-zip so the next instance starts from your end state, not from the
   last packaged one.

## What NOT to spend time on

- Re-verifying facts already in the cheat sheet or decision log.
- Exploring the decompiled tree "to get context" - the reference doc is the map.
- Reading full vanilla code for a feature unless the docs say it's needed.
- Rewriting the build system; it works. Use it.
