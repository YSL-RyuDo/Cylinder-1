# Cylinder

<!-- portfolio-languages -->
[![English (current)](https://ysl-ryudo.github.io/assets/diagrams/language-en-current.svg)](README.md) [![한국어](https://ysl-ryudo.github.io/assets/diagrams/language-ko.svg)](README.ko.md) [![日本語](https://ysl-ryudo.github.io/assets/diagrams/language-ja.svg)](README.ja.md)
<!-- /portfolio-languages -->

A team narrative game. My systems span branching dialogue, inventory, personality UI, portals and puzzles.

| Project | Details |
|---|---|
| Status | Team contribution |
| Stack | C#, Unity, CSV |

[Explore the case](https://ysl-ryudo.github.io/en/work/cylinder/), [Portfolio](https://ysl-ryudo.github.io/en/)

## Inside the game

[![Cylinder running from the ysl-development branch, the exploration scene and character view.](https://ysl-ryudo.github.io/assets/gameplay/cylinder-world.webp)](https://ysl-ryudo.github.io/assets/gameplay/cylinder-world.webp)

Cylinder running from the ysl-development branch, the exploration scene and character view.

## What I built across the game

I implemented CSV-driven dialogue, choices and command execution, inventory, personality and menu UI, portals, camera transitions and library/slider puzzles on the YSL branch.

[![My implementation flow](https://ysl-ryudo.github.io/assets/diagrams/cylinder-flow-en.svg)](https://ysl-ryudo.github.io/assets/diagrams/cylinder-flow-en.svg)

### Branching dialogue and system commands

Implemented and extended the dialogue node executor, including choices, conditions, flags and system-command handling. CSV loading separates authored dialogue from execution, while DialogueUI handles typing, portraits and choices.

[DialogueManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueManager.cs), [My change f2bade0](https://github.com/YSL-RyuDo/Cylinder-1/commit/f2bade0f41e38fe1d6b07cd38d9aff677152782d), [DialogueCSVLoader.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueCSVLoader.cs), [My change 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

### Inventory and registered items

Worked on item definitions, acquisition, removal, registration and use, along with inventory UI. A registered item can be consumed by a world interaction such as unlocking a portal, connecting the menu state to progression.

[InventoryManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Item/InventoryManager.cs), [My change 6ed8ded](https://github.com/YSL-RyuDo/Cylinder-1/commit/6ed8ded8f321ed7da22453f874316442aa2da7d3), [InventoryUI.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/InventoryUI.cs), [My change b21ce48](https://github.com/YSL-RyuDo/Cylinder-1/commit/b21ce4867f5cff4459ed22a8f97ef87d1c70e54c)

### Portals, fade and camera transitions

Implemented portal transfer with fade and player warp, item-based locking and a readiness guard against immediate transfer back. Worked on camera follow and external-control transitions so a room or puzzle can temporarily control the view.

[PortalTrigger.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Portal/PortalTrigger.cs), [My change 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [CameraFollow.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Map/CameraFollow.cs), [My change b5e01aa](https://github.com/YSL-RyuDo/Cylinder-1/commit/b5e01aa3498a702fa05e44b18782b3c70da0042a)

### Interaction and modal UI

Worked on player interaction and on-screen prompts, and implemented the IUIWindow-based menu controller. Menu opening pauses gameplay time; closing returns control. Personality selection, monologue and narration each have their own presentation rather than being folded into one dialogue label.

[UIManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/UIManager.cs), [My change 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [PersonalityUI.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/PersonalityUI/PersonalityUI.cs), [My change 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [PlayerInteraction.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Player/PlayerInteraction.cs), [My change 1cb584e](https://github.com/YSL-RyuDo/Cylinder-1/commit/1cb584e0032c653a1c14e1166f76505ddf6ae5f4)

### Library and sliding puzzles

Implemented library seal-group interaction and answer checking, and a sliding-puzzle board/UI loaded from puzzle data. These are playable rule and interface systems, distinct from dialogue presentation. The linked revisions show the concrete puzzle scope rather than attributing every puzzle in the project to me.

[LibraryPuzzlePanel.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Puzzle/LibraryPuzzle/LibraryPuzzlePanel.cs), [My change 24497c4](https://github.com/YSL-RyuDo/Cylinder-1/commit/24497c4aed3f61d3eafcdef1937fcc29ddaae436), [SliderPuzzleManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Puzzle/SliderPuzzleManager/SliderPuzzleManager.cs), [My change 24497c4](https://github.com/YSL-RyuDo/Cylinder-1/commit/24497c4aed3f61d3eafcdef1937fcc29ddaae436)

## Contribution evidence

[Dialogue rework / 대화 개편 / 会話改修](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

[Emotion state / 감정 상태 / 感情状態](https://github.com/YSL-RyuDo/Cylinder-1/commit/719fe90233e191f8d4df5fe5b05bb0143525edcb)

[Working branch / 작업 브랜치 / 作業ブランチ](https://github.com/YSL-RyuDo/Cylinder-1/tree/YSL)

## Implementation and design

Dialogue needs to respond to progression, emotion state and interaction triggers, while presenting the right portraits and UI.

I reworked CSV loading, dialogue nodes/UI, portraits, flags and triggers, and integrated tutorial/cutscene progression. Specific dialogue and emotion-state commits identify my work within the shared YSL branch.

## How the code works and why it is structured this way

[![Dialogue command execution](https://ysl-ryudo.github.io/assets/diagrams/cylinder-design-en.svg)](https://ysl-ryudo.github.io/assets/diagrams/cylinder-design-en.svg)

Immediate commands continue to the next node. Walk, delay and fade_wait resume through their completion path.

### A dialogue command may finish later

**Implementation**

In the YSL branch, DialogueManager executes data nodes and system commands. Immediate commands can continue the flow; fade_wait, walk and delay return without immediate continuation and resume at the next node from a callback or coroutine. The earlier JSON loader shown below is a separate, older implementation.

**Design rationale**

A dialogue event may need to wait for movement or a fade before showing the next line. Distinguishing immediate execution from deferred completion keeps the narrative sequence from advancing before the visible action finishes.

**Alternatives and the current tradeoff**

Hard-coded scene coroutines are direct but duplicate sequencing across events. A visual timeline supports authoring but needs integration with choices and flags. The current data-node command interpreter centralizes sequencing at the cost of maintaining a command vocabulary and valid references.

**Tradeoffs and verification**

String commands and scene lookups can fail at runtime. In fade_wait, a missing FadeManager returns without scheduling the next node; that path needs an explicit continuation or error policy.

[Cylinder-1/DialogueManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueManager.cs), [Cylinder-1/DialogueCSVLoader.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueCSVLoader.cs)

### Earlier version: JSON dialogue loading

**Implementation**

DialogueLoader loads JSON dialogue into ID-indexed data. DialogueManager selects preview or confirmation text, resolves an emotion portrait using a key, stops the previous typing coroutine and starts the new text with real-time waits.

**Design rationale**

Separating content from display avoids embedding every line and portrait choice in UI control flow. Cancelling the previous coroutine prevents two text producers from writing into the same field. Real-time waits keep dialogue advancing independently of gameplay time scale.

**Alternatives and the current tradeoff**

Hard-coded dialogue needs little tooling but scatters content changes through code. ScriptableObjects would offer Unity inspector integration; JSON offers a text-based content format with a separate loader. The current JSON approach needs schema and resource-key validation that the format does not provide automatically.

**Tradeoffs and verification**

String IDs and resource names form a data contract that needs validation. Character-by-character concatenation also allocates strings, so the display path has costs beyond loading the dialogue file.

[Cylinder-1/DialogueLoader.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueLoader.cs), [Cylinder-1/DialogueManager.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueManager.cs)

## Open and inspect

Open the project with the Unity version recorded in [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt). Keep its package lock and serialized assets intact. Select the documented branch before opening Unity.

Inspect the YSL branch's scenes and Build Settings; this overview does not prescribe an unverified startup scene.

## Further documentation

[Existing detailed development reference (Korean)](docs/DEVELOPMENT_REFERENCE.ko.md)

## Related repositories

[Cylinder-1](https://github.com/YSL-RyuDo/Cylinder-1), Public repository
