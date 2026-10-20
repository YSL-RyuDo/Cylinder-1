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

## 실행, 코드 확인

[ProjectVersion.txt](ProjectSettings/ProjectVersion.txt)의 Unity 버전으로 프로젝트를 여세요. 패키지 잠금 파일과 직렬화된 에셋을 유지하고, Unity를 열기 전에 문서에서 설명한 브랜치를 선택하세요.

YSL 브랜치의 씬과 Build Settings를 확인하세요. 확인되지 않은 시작 씬을 임의로 지정하지 않습니다.

## 개발, 검증 범위

YSL 브랜치에 main에 없는 작업과 팀 공동 변경이 있습니다. 인용한 커밋으로 제 기여를 구분하며 Assets/Scripts/YSL 아래 모든 시스템을 단독 제작했다고 주장하지 않습니다. 시연 가능 상태는 선택한 브랜치에서 확인해야 합니다.

## 상세 문서

[기존 개발, 사용, 검증 상세 안내](docs/DEVELOPMENT_REFERENCE.ko.md)

## 관련 저장소

[Cylinder-1](https://github.com/YSL-RyuDo/Cylinder-1), 비공개, 접근 권한 필요
