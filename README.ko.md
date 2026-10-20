# Cylinder

<!-- portfolio-languages -->
[![English](https://ysl-ryudo.github.io/assets/diagrams/language-en.svg)](README.md) [![한국어 (현재 언어)](https://ysl-ryudo.github.io/assets/diagrams/language-ko-current.svg)](README.ko.md) [![日本語](https://ysl-ryudo.github.io/assets/diagrams/language-ja.svg)](README.ja.md)
<!-- /portfolio-languages -->

팀으로 만든 내러티브 게임입니다. 분기 대화와 인벤토리, 인격 UI, 포털과 퍼즐 시스템을 작업했습니다.

| 프로젝트 정보 | 내용 |
|---|---|
| 개발 상태 | 팀 프로젝트 기여 |
| 사용 기술 | C#, Unity, CSV |

[개발 사례 보기](https://ysl-ryudo.github.io/ko/work/cylinder/), [Portfolio](https://ysl-ryudo.github.io/ko/)

## 게임에서 보는 구현

[![ysl-development 브랜치로 실행한 Cylinder의 탐험 씬과 캐릭터 화면.](https://ysl-ryudo.github.io/assets/gameplay/cylinder-world.webp)](https://ysl-ryudo.github.io/assets/gameplay/cylinder-world.webp)

ysl-development 브랜치로 실행한 Cylinder의 탐험 씬과 캐릭터 화면.

## 게임 전체에서 맡은 구현

YSL 브랜치에서 CSV 기반 대화와 선택지, 명령 실행, 인벤토리, 인격과 메뉴 UI, 포털과 카메라 전환, 도서관과 슬라이더 퍼즐을 구현했습니다.

[![담당 구현 흐름](https://ysl-ryudo.github.io/assets/diagrams/cylinder-flow-ko.svg)](https://ysl-ryudo.github.io/assets/diagrams/cylinder-flow-ko.svg)

### 분기 대화와 시스템 명령

선택지와 조건, 플래그와 시스템 명령을 처리하는 대화 노드 실행기를 구현하고 확장했습니다. CSV 로딩으로 작성한 대화와 실행 처리를 나누고 DialogueUI에서 타이핑, 초상화와 선택지를 표시합니다.

[DialogueManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueManager.cs), [내 변경 f2bade0](https://github.com/YSL-RyuDo/Cylinder-1/commit/f2bade0f41e38fe1d6b07cd38d9aff677152782d), [DialogueCSVLoader.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueCSVLoader.cs), [내 변경 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

### 인벤토리와 등록 아이템

아이템 정의와 획득, 제거, 등록과 사용, 인벤토리 UI를 작업했습니다. 등록한 아이템을 포털 해금 같은 월드 상호작용에서 사용할 수 있도록 메뉴의 선택과 진행 상태를 연결했습니다.

[InventoryManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Item/InventoryManager.cs), [내 변경 6ed8ded](https://github.com/YSL-RyuDo/Cylinder-1/commit/6ed8ded8f321ed7da22453f874316442aa2da7d3), [InventoryUI.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/InventoryUI.cs), [내 변경 b21ce48](https://github.com/YSL-RyuDo/Cylinder-1/commit/b21ce4867f5cff4459ed22a8f97ef87d1c70e54c)

### 포털, 페이드와 카메라 전환

페이드와 플레이어 이동을 연결한 포털 전환, 아이템 기반 잠금과 즉시 되돌아가는 전환을 막는 준비 상태를 구현했습니다. 카메라 추적과 외부 제어 전환도 작업해 방이나 퍼즐에서 화면 제어를 넘겨받도록 했습니다.

[PortalTrigger.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Portal/PortalTrigger.cs), [내 변경 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [CameraFollow.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Map/CameraFollow.cs), [내 변경 b5e01aa](https://github.com/YSL-RyuDo/Cylinder-1/commit/b5e01aa3498a702fa05e44b18782b3c70da0042a)

### 상호작용과 창 제어

플레이어 상호작용과 안내 표시, IUIWindow 기반 메뉴 제어를 작업했습니다. 메뉴를 열면 게임 시간을 멈추고 닫으면 조작을 돌려줍니다. 인격 선택과 독백, 내레이션도 각각의 표시 흐름으로 구현했습니다.

[UIManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/UIManager.cs), [내 변경 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [PersonalityUI.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/PersonalityUI/PersonalityUI.cs), [내 변경 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [PlayerInteraction.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Player/PlayerInteraction.cs), [내 변경 1cb584e](https://github.com/YSL-RyuDo/Cylinder-1/commit/1cb584e0032c653a1c14e1166f76505ddf6ae5f4)

### 도서관과 슬라이딩 퍼즐

도서관의 봉인 그룹 조작과 정답 검사, 퍼즐 데이터로 구성하는 슬라이딩 보드와 UI를 구현했습니다. 대화 출력과 별개의 플레이 규칙과 인터페이스 작업입니다. 프로젝트의 모든 퍼즐을 제 작업으로 묶지 않고 해당 버전에서 확인되는 범위를 연결했습니다.

[LibraryPuzzlePanel.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Puzzle/LibraryPuzzle/LibraryPuzzlePanel.cs), [내 변경 24497c4](https://github.com/YSL-RyuDo/Cylinder-1/commit/24497c4aed3f61d3eafcdef1937fcc29ddaae436), [SliderPuzzleManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Puzzle/SliderPuzzleManager/SliderPuzzleManager.cs), [내 변경 24497c4](https://github.com/YSL-RyuDo/Cylinder-1/commit/24497c4aed3f61d3eafcdef1937fcc29ddaae436)

## 기여 근거

[Dialogue rework / 대화 개편 / 会話改修](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

[Emotion state / 감정 상태 / 感情状態](https://github.com/YSL-RyuDo/Cylinder-1/commit/719fe90233e191f8d4df5fe5b05bb0143525edcb)

[Working branch / 작업 브랜치 / 作業ブランチ](https://github.com/YSL-RyuDo/Cylinder-1/tree/YSL)

## 구현과 설계

진행 상황, 감정 상태, 상호작용 조건에 맞춰 대화가 바뀌고, 초상화와 UI가 함께 갱신되어야 합니다.

CSV 로딩, 대화 노드, UI, 초상화, 플래그와 트리거를 개편하고 튜토리얼, 컷씬 진행을 연동했습니다. 공용 변경이 섞인 YSL 브랜치에서는 대화, 감정 상태 커밋으로 제 작업을 구분합니다.

## 코드의 동작과 구현 방식을 택한 이유

[![대화 명령 실행과 이어가기](https://ysl-ryudo.github.io/assets/diagrams/cylinder-design-ko.svg)](https://ysl-ryudo.github.io/assets/diagrams/cylinder-design-ko.svg)

즉시 명령은 다음 노드로 진행하고, walk와 delay, fade_wait는 완료 경로에서 실행을 이어갑니다.

### 대화 명령이 즉시 끝나지 않을 때

**구현 흐름**

YSL 브랜치의 DialogueManager는 데이터 노드와 시스템 명령을 실행합니다. 즉시 처리하는 명령은 흐름을 이어가고, fade_wait와 walk, delay는 바로 다음으로 넘어가지 않은 채 콜백이나 코루틴에서 다음 노드를 실행합니다. 아래의 JSON 로더 설명은 별도의 초기 구현입니다.

**이 구조가 필요한 이유**

대화 이벤트는 이동이나 페이드가 끝나야 다음 문장을 보여줄 수 있습니다. 즉시 실행과 완료를 기다리는 명령을 구분해야 화면의 행동이 끝나기 전에 이야기가 먼저 진행되는 문제를 줄일 수 있습니다.

**대안과 현재 구현의 절충**

씬마다 코루틴을 작성하면 직접적이지만 이벤트마다 순서 제어가 중복됩니다. 시각적 타임라인은 편집에 유리하지만 선택지와 플래그 연동이 필요합니다. 현재의 데이터 노드와 명령 실행기는 순서를 모으는 대신 명령 종류와 참조의 유효성을 관리해야 합니다.

**장단점과 검증 범위**

문자열 명령과 씬 객체 검색은 실행 중 실패할 수 있습니다. fade_wait에서 FadeManager가 없으면 다음 노드를 예약하지 않고 반환하므로, 이 경로에 이어 실행하거나 오류를 처리하는 규칙이 필요합니다.

[Cylinder-1/DialogueManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueManager.cs), [Cylinder-1/DialogueCSVLoader.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueCSVLoader.cs)

### 초기 버전의 JSON 대화 로딩

**구현 흐름**

DialogueLoader는 JSON 대사를 읽어 ID로 찾을 수 있는 데이터로 구성합니다. DialogueManager는 미리보기와 확정 대사를 구분하고 키로 감정 초상화를 찾은 뒤, 이전 타이핑 코루틴을 멈추고 새 대사를 실제 시간 기준으로 출력합니다.

**이 구조가 필요한 이유**

내용과 표시를 나누면 대사마다 UI 제어 코드에 문장과 초상화 분기를 추가하지 않아도 됩니다. 이전 코루틴을 중지하는 이유는 두 출력 작업이 같은 텍스트 필드를 동시에 갱신하지 않도록 하기 위해서입니다. 실제 시간 대기는 게임 시간 배율과 대사 출력 속도를 분리합니다.

**대안과 현재 구현의 절충**

대사를 코드에 직접 쓰면 제작 도구가 적게 필요하지만 내용 수정이 코드에 흩어집니다. ScriptableObject는 Unity 인스펙터 편집에 유리하고, JSON은 별도 로더를 통해 텍스트 형식으로 내용을 다룰 수 있습니다. 현재 JSON 방식은 형식이 자동 보장하지 않는 스키마와 리소스 키 검증이 필요합니다.

**장단점과 검증 범위**

문자열 ID와 리소스 이름 규칙을 데이터 계약으로 관리하고 검사해야 합니다. 글자를 붙여 출력하는 방식에는 문자열 할당 비용도 있어 대사 파일 로딩과 별도로 표시 경로를 고려해야 합니다.

[Cylinder-1/DialogueLoader.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueLoader.cs), [Cylinder-1/DialogueManager.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueManager.cs)

## 실행, 코드 확인

[ProjectVersion.txt](ProjectSettings/ProjectVersion.txt)의 Unity 버전으로 프로젝트를 여세요. 패키지 잠금 파일과 직렬화된 에셋을 유지하고, Unity를 열기 전에 문서에서 설명한 브랜치를 선택하세요.

YSL 브랜치의 씬과 Build Settings를 확인하세요. 확인되지 않은 시작 씬을 임의로 지정하지 않습니다.

## 상세 문서

[기존 개발, 사용, 검증 상세 안내](docs/DEVELOPMENT_REFERENCE.ko.md)

## 관련 저장소

[Cylinder-1](https://github.com/YSL-RyuDo/Cylinder-1), 비공개, 접근 권한 필요
