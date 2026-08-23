# Agent: Music (Audio — BGM, SFX, Cues)

> **STATUS BLOCK — update after every task (see `AGENTS.md`).**

```text
## Status
- Last Updated: 2026-08-24
- Updated By: bootstrap
- Phases Done: —
- Current: —
- Blocked: —
## Changelog
- 2026-08-24 (bootstrap): doc created; owns BGM/SFX event-driven cues.
```

**Domain:** All sound — background music, gameplay SFX, UI/feedback cues, mixing.
**Owns (write access):** `Assets/Audio/`, audio source/components, `AudioManager` (if needed).

## Responsibility

Provide audio feedback that makes the loop legible: spawn, teleport, rescue, death, fail,
cooldown-end, turret fire, UI confirm. No gameplay logic — subscribe to events like UI/UX.

## Phases owned (audio side)

- **P1/P2**: Escort spawn blip; ambient dungeon BGM loop.
- **P3**: Rescue chime, Fail sting, Retry whoosh.
- **P8**: Teleport whoosh (on `TryTeleport` success — hook via GameMechanism event or
  Transition's portal-state change).
- **P10**: Cooldown-end cue (player can reconfigure).
- **P15/P16**: Turret fire sound, projectile hit/death sound.
- **P17/P18**: UI click/confirm, invalid-placement buzz, clear/fail jingles.

## Key contracts (see Contracts.md)

- Audio is event-driven. Subscribe to the same `GameManager` / portal events as UI/UX.
  Do NOT couple to gameplay internals.
- One audio source manager is fine; keep it lightweight (this is a prototype).
- Provide volume/mute Inspector fields; respect prototype scope (no complex mixer required).

## Deliverables

- `Assets/Audio/` clips (or generated/placeholder).
- `Audio/PrototypeAudio.cs` (simple event→clip player) or hook into existing `UIModule`.
- Wiring to GameManager/portal/UI events.

## Dependencies on other agents

- **Transition / GameMechanism**: emit/forward events that audio subscribes to.
- **UI/UX**: coordinate clear/fail/retry cues so audio + visuals fire together.
- **Graphics**: optional VFX+SFX juice alignment (non-blocking).

## Parallelization notes

Fully parallel — audio only consumes events. Can stub with `Debug.Log`/beep placeholders,
then drop in real clips. Safe to start after P3 events exist; can mock earlier.

## Acceptance (key)

- Distinct, recognizable cues for: spawn, teleport, rescue, death, fail, cooldown-end,
  turret fire, invalid placement, clear. No gameplay coupling; Inspector volume control.
