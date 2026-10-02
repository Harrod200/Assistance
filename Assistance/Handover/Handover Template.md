# <Mod Name> — Handover Notes vX (supersedes vX-1)

For the next dev picking this up. Read this first.

## 0. Fast-start checklist (do these before anything else)
- [ ] Working copy: real git clone of <url>, branch <branch>, at commit <sha>
- [ ] Build: run `<build command>` — must be green before you touch anything
- [ ] Read `Docs/Terra Invicta Class & Method Reference.md` (2.9 MB — verified
      full-game class/method index). Search it before decompiling anything.
      Trust it over any other summary doc; if a summary conflicts with it or
      with decompiled source, the summary is wrong.
- [ ] Read the API cheat sheet (`Docs/Vanilla API Cheat Sheet.md`) — do NOT
      re-derive vanilla API semantics from the decompiled corpus if the answer
      is already there. Add new findings back to the cheat sheet at end of session.
- [ ] Read the checkpoint table (section 4). Claim the next one.

## 1. Session summary
1. What changed since last handover (commits, one line each)
2. Decisions made and WHY (one paragraph max each)
3. Bugs found / fixed (with repro)

## 2. Environment (so a fresh VM doesn't burn an hour rediscovering it)
- Build command, target framework, and any FrameworkPathOverride / HintPath shim
- Persistent reference DLLs location (must survive /tmp wipes — keep in handover folder)
- Bake/tool scripts and how to run them
- Known environment gotchas (NuGet offline, SDK version, etc.)

## 3. Repository layout
- Table: path -> what it is -> in git or local-only (and why, if excluded e.g. vanilla code)

## 4. Checkpoint table
| # | Deliverable | Verify | Est. credits | Status |
|---|---|---|---|---|
| C1 | ... | Builds + spot-check | ~N | done (sha) |

Rules:
- Each checkpoint leaves the build green and the repo pushable.
- Size each checkpoint to fit ONE runtime window (~75 tool steps). Split anything bigger.
- Record actual credit cost per checkpoint (compare vs estimate; recalibrate future estimates).

## 5. Design decisions in force (do not relitigate without owner sign-off)
- Amendment log with dates. Anything that reverses a previous decision is marked AMENDMENT.

## 6. Vanilla API cheat sheet (grow this, never shrink it)
- Decompiled method bodies (verbatim) for every API the mod depends on
- Semantics notes: what each overload/flag actually does in game terms
- Cross-references: which mod components use which API, and why that choice
- Common pitfalls discovered this project

## 7. Known unknowns / open questions
- Explicitly listed so the next dev doesn't burn credits rediscovering that nobody knows.

## 8. Docs in this package
- List every doc, one line each, with a read order.

## 9. Local-only files (excluded from git, present in this folder)
- Table: item -> size -> why excluded -> needed for what
