# Cylinder

[English](README.md), [Korean](README.ko.md), [Japanese](README.ja.md)

대화와 초상화, 감정 상태, 진행 조건을 다룬 내러티브 게임의 시스템 개발 사례입니다.

| 프로젝트 정보 | 내용 |
|---|---|
| 개발 상태 | 팀 프로젝트 기여 |
| 사용 기술 | C#, Unity, CSV |

[개발 사례 보기](https://ysl-ryudo.github.io/ko/work/cylinder/), [Portfolio](https://ysl-ryudo.github.io/ko/)

## 내 기여

CSV 로딩, 대화 노드, UI, 초상화, 플래그와 상호작용 트리거를 연결해 대화 시스템을 개편했습니다.

대사, 감정 상태 변화와 튜토리얼 진행, 컷신 연동을 작업했습니다.

인벤토리, 포털, 상호작용 UI를 작업했습니다. YSL 브랜치에도 팀원 커밋이 섞여 있으므로 폴더, 브랜치 이름만으로 기여를 판단하지 않습니다.

## 기여 근거

[Dialogue rework / 대화 개편 / 会話改修](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

[Emotion state / 감정 상태 / 感情状態](https://github.com/YSL-RyuDo/Cylinder-1/commit/719fe90233e191f8d4df5fe5b05bb0143525edcb)

[Working branch / 작업 브랜치 / 作業ブランチ](https://github.com/YSL-RyuDo/Cylinder-1/tree/YSL)

커밋은 설명한 변경의 근거이며 기여율 점수가 아닙니다. 외부 에셋과 팀원 작업의 크레딧은 구분합니다.

## 구현과 설계

진행 상황, 감정 상태, 상호작용 조건에 맞춰 대화가 바뀌고, 초상화와 UI가 함께 갱신되어야 합니다.

CSV 로딩, 대화 노드, UI, 초상화, 플래그와 트리거를 개편하고 튜토리얼, 컷씬 진행을 연동했습니다. 공용 변경이 섞인 YSL 브랜치에서는 대화, 감정 상태 커밋으로 제 작업을 구분합니다.

| 영역 | 구현 |
|---|---|
| 제작 데이터 | CSV 로딩과 대화 노드 |
| 진행 조건 | 플래그, 감정 상태, 상호작용 트리거 |
| 화면 표현 | 대화 UI, 초상화, 튜토리얼, 컷신 연동 |

## 코드의 동작과 구현 방식을 택한 이유

확인한 구현이 어떤 제약을 다루며 어떤 장단점이 생기는지 설명합니다. 설계 효과에 대한 설명은 코드 분석이며, 비교한 모든 대안을 당시 직접 구현했다는 뜻은 아닙니다. 기여 링크는 공동 프로젝트 안의 담당 변경을 가리키고 소스 링크는 기존 접근 권한을 유지합니다.

### 대사 데이터와 출력 처리를 나눈 이유

**구현 흐름**

DialogueLoader는 JSON 대사를 읽어 ID로 찾을 수 있는 데이터로 구성합니다. DialogueManager는 미리보기와 확정 대사를 구분하고 키로 감정 초상화를 찾은 뒤, 이전 타이핑 코루틴을 멈추고 새 대사를 실제 시간 기준으로 출력합니다.

**이 구조가 필요한 이유**

내용과 표시를 나누면 대사마다 UI 제어 코드에 문장과 초상화 분기를 추가하지 않아도 됩니다. 이전 코루틴을 중지하는 이유는 두 출력 작업이 같은 텍스트 필드를 동시에 갱신하지 않도록 하기 위해서입니다. 실제 시간 대기는 게임 시간 배율과 대사 출력 속도를 분리합니다.

**대안과 현재 구현의 절충**

대사를 코드에 직접 쓰면 제작 도구가 적게 필요하지만 내용 수정이 코드에 흩어집니다. ScriptableObject는 Unity 인스펙터 편집에 유리하고, JSON은 별도 로더를 통해 텍스트 형식으로 내용을 다룰 수 있습니다. 현재 JSON 방식은 형식이 자동 보장하지 않는 스키마와 리소스 키 검증이 필요합니다.

**장단점과 검증 범위**

문자열 ID와 리소스 이름 규칙은 검증이 필요한 데이터 계약입니다. 데이터를 분리했다고 다국어 제작 파이프라인까지 완성되는 것은 아니며, 글자를 붙여 출력하는 구현에는 문자열 할당 비용도 남습니다. 이는 로더와 표시를 분리하는 것과 별도로 다룰 문제입니다.

[Cylinder-1/DialogueLoader.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueLoader.cs), [Cylinder-1/DialogueManager.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueManager.cs)

## 실행, 코드 확인

[ProjectVersion.txt](ProjectSettings/ProjectVersion.txt)의 Unity 버전으로 프로젝트를 여세요. 패키지 잠금 파일과 직렬화된 에셋을 유지하고, Unity를 열기 전에 문서에서 설명한 브랜치를 선택하세요.

YSL 브랜치의 씬과 Build Settings를 확인하세요. 확인되지 않은 시작 씬을 임의로 지정하지 않습니다.

## 개발, 검증 범위

YSL 브랜치에 main에 없는 작업과 팀 공동 변경이 있습니다. 인용한 커밋으로 제 기여를 구분하며 Assets/Scripts/YSL 아래 모든 시스템을 단독 제작했다고 주장하지 않습니다. 시연 가능 상태는 선택한 브랜치에서 확인해야 합니다.

## 상세 문서

[기존 개발, 사용, 검증 상세 안내](docs/DEVELOPMENT_REFERENCE.ko.md)

## 관련 저장소

[Cylinder-1](https://github.com/YSL-RyuDo/Cylinder-1), 비공개, 접근 권한 필요
