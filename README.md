# Cylinder

[English](README.md), [Korean](README.ko.md), [Japanese](README.ja.md)

A narrative game contribution focused on dialogue, portraits, emotion state and progression.

| Project | Details |
|---|---|
| Status | Team contribution |
| Stack | C#, Unity, CSV |

[Explore the case](https://ysl-ryudo.github.io/en/work/cylinder/), [Portfolio](https://ysl-ryudo.github.io/en/)

## My contribution

Reworked dialogue around CSV loading, dialogue nodes/UI, portraits, flags and interaction triggers.

Added dialogue/emotion-state changes and worked on tutorial progression and cutscene integration.

Worked on inventory, portals and interaction UI. The YSL branch also includes teammates' commits; a folder or branch name alone is not authorship evidence.

## Contribution evidence

[Dialogue rework / 대화 개편 / 会話改修](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

[Emotion state / 감정 상태 / 感情状態](https://github.com/YSL-RyuDo/Cylinder-1/commit/719fe90233e191f8d4df5fe5b05bb0143525edcb)

[Working branch / 작업 브랜치 / 作業ブランチ](https://github.com/YSL-RyuDo/Cylinder-1/tree/YSL)

Commits are evidence of the stated changes, not a percentage-of-authorship score. Third-party assets and teammates' work remain separately credited.

## Implementation and design

Dialogue needs to respond to progression, emotion state and interaction triggers, while presenting the right portraits and UI.

I reworked CSV loading, dialogue nodes/UI, portraits, flags and triggers, and integrated tutorial/cutscene progression. Specific dialogue and emotion-state commits identify my work within the shared YSL branch.

| Area | Implementation |
|---|---|
| Authored data | CSV loading and dialogue nodes |
| Progression | Flags, emotion state and interaction triggers |
| Presentation | Dialogue UI, portraits, tutorial and cutscene integration |

## How the code works and why it is structured this way

These notes connect the inspected implementation to the constraints it addresses and its tradeoffs. Explanations of design consequences are code analysis, not a claim that every alternative was prototyped. Contribution links identify my changes within team projects; source links retain their original access permissions.

### Dialogue content and presentation change independently

**Implementation**

DialogueLoader loads JSON dialogue into ID-indexed data. DialogueManager selects preview or confirmation text, resolves an emotion portrait using a key, stops the previous typing coroutine and starts the new text with real-time waits.

**Design rationale**

Separating content from display avoids embedding every line and portrait choice in UI control flow. Cancelling the previous coroutine prevents two text producers from writing into the same field. Real-time waits keep dialogue advancing independently of gameplay time scale.

**Alternatives and the current tradeoff**

Hard-coded dialogue needs little tooling but scatters content changes through code. ScriptableObjects would offer Unity inspector integration; JSON offers a text-based content format with a separate loader. The current JSON approach needs schema and resource-key validation that the format does not provide automatically.

**Tradeoffs and verification**

String IDs and resource naming form a content contract that needs validation. Data-driven dialogue is not automatically a complete localization pipeline, and repeatedly appending strings during typing still has allocation costs. These are separate concerns from the loader/presentation split.

[Cylinder-1/DialogueLoader.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueLoader.cs), [Cylinder-1/DialogueManager.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueManager.cs)

## Open and inspect

Open the project with the Unity version recorded in [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt). Keep its package lock and serialized assets intact. Select the documented branch before opening Unity.

Inspect the YSL branch's scenes and Build Settings; this overview does not prescribe an unverified startup scene.

## Development and verification scope

The YSL branch contains substantial work absent from main and includes shared team changes. The cited commits identify my contribution; no claim is made that every system under Assets/Scripts/YSL was solely authored by me. Demonstration readiness must be assessed on the selected branch.

## Further documentation

[Existing detailed development reference (Korean)](docs/DEVELOPMENT_REFERENCE.ko.md)

## Related repositories

[Cylinder-1](https://github.com/YSL-RyuDo/Cylinder-1), Private, access required
